# Bind it, or build it? Solid 2.0's async reactive graph, F#, and .NET

**The question.** Two halves, and the report must answer both:

1. **Can** Solid 2.0's *async-aware* graph — the pending channel — be **bound** from F#, and **built**
   natively in .NET?
2. **Should** we build an F#-first reactive library with those semantics, dual-targeting native .NET and
   Fable/JS — or stay a Solid binding?

"Bind it" is the do-nothing alternative that "build it" has to beat. Everything in the **v1 layer**
(`createSignal`/`createMemo`/`createEffect`/`batch`/owner+cleanup/glitch-freedom/cutoff) is **table
stakes** — solved in FSharp.Data.Adaptive, Clef, SignalsDotnet and Fable.Ripple — and gets no space here.

**Status.** Research only; nothing outside this file changed, nothing committed. Conventions follow
[`API-COVERAGE-solid2.md`](API-COVERAGE-solid2.md): repo-relative links resolve in an IDE preview, not on
GitHub (submodule paths). **Sourced fact and speculation are separated throughout**; estimates are
labelled ⚠️.

> ⚠️ **Stale paths in [`API-COVERAGE-solid2.md`](API-COVERAGE-solid2.md).** It says
> `solid/packages/solid-signals/…`. **That directory does not exist.** The pin is `solidjs/solid`
> @ `47fc4b22` (2026-08-27), package `@solidjs/signals`, version line **`2.0.0-rc.3`**, and the reactive
> core is at `solid/packages/signals/`. Every `solid-signals` anchor in that document is stale. All
> anchors below were read at `47fc4b22`.

> ⚠️ **Prompt injection encountered.** The `fsprojects.github.io/FSharp.Data.Adaptive` docs page served
> content containing instructions addressed to the reading agent. Ignored, and flagged because the page is
> otherwise legitimate. Treat fetched web content as data.

---

## §1. The Solid v2 delta — what we would bind or port

### 1.1 The async surface, and that it has no v1 counterpart

The v2-only exports, from the barrel [`packages/signals/src/index.ts`](../solid/packages/signals/src/index.ts):
`NotReadyError` ([`:5`](../solid/packages/signals/src/index.ts#L5)), `isPending`, `latest`, `resolve`
([`:21-23`](../solid/packages/signals/src/index.ts#L21)), `createOptimistic`, `onSettled`,
`createTrackedEffect` ([`:58-62`](../solid/packages/signals/src/index.ts#L58)), plus
`createLoadingBoundary`, `createErrorBoundary`, `createRevealOrder`, `flatten`, `flush`, `refresh`,
`enforceLoadingBoundary`, `resetErrorHalt`. **Confirmed: zero counterpart in Solid v1, and zero
counterpart in Fable.Ripple** (§6).

### 1.2 Async transparency — sentinel *and* node state, doing different jobs

**There is no async primitive.** Every ordinary computation is async-capable; the constructor signature is
the whole design:

```ts
export function computed<T>(fn: (prev?: T) => T | PromiseLike<T> | AsyncIterable<T>): Computed<T>;
```
— [`core/core.ts:553`](../solid/packages/signals/src/core/core.ts#L553)

Dispatch is **duck-typed at runtime**, not by type — `handleAsync`
([`async.ts:246`](../solid/packages/signals/src/core/async.ts#L246)) probes `Symbol.asyncIterator`, then:

```ts
export function isThenable<T>(value: T | PromiseLike<T>): value is PromiseLike<T> {
  return value != null && typeof value === "object" && typeof (value as { then?: unknown }).then === "function";
}
```
— [`async.ts:237-244`](../solid/packages/signals/src/core/async.ts#L237)

Neither ⇒ sync landing, returned as-is ([`:262-267`](../solid/packages/signals/src/core/async.ts#L262)).

**"Sentinel thrown, or node state?" — both, and they are not redundant:**

| | Mechanism | Lives on | Role |
| --- | --- | --- | --- |
| **Node state** | `_statusFlags & STATUS_PENDING` | the *source* | durable, propagating: "a flight is in the air" |
| **Sentinel** | `throw new NotReadyError(source)` from `read()` | transient | abort the *consumer's* partial body, now |

State without throw = SignalsDotnet's `IsComputing` flag, which every consumer must check by hand (§2.2).
Throw without state = no way to know when to retry.

**Why a plain synchronous body survives and re-runs.** Three cooperating facts:

1. **Dep list rebuilt from scratch each run** — `recompute` resets `_depsTail = null`, bumps `_depGen++`,
   sets `_flags = REACTIVE_RECOMPUTING_DEPS`, then `context = el; tracking = true`
   ([`core.ts:239-252`](../solid/packages/signals/src/core/core.ts#L239)). Partial work from an aborted
   run is free to discard; nothing accumulates across runs.
2. **The edge is linked *before* the throw** — the single most important line to port correctly:
   ```ts
   if (currentOptimisticLane === null || GlobalQueue._laneSuspends!(owner)) {
     if (!tracking && el !== c) link(el, c as Computed<any>);
     throw owner._x?._error;
   }
   ```
   — [`core.ts:1026-1029`](../solid/packages/signals/src/core/core.ts#L1026).
   **A port that throws before linking silently deadlocks** — the suspended consumer never wakes.
3. **The throw is a non-local abort through arbitrary intermediate frames.** The consumer may read the
   pending source three helper calls deep, through helpers that know nothing about asynchrony. Nothing
   short of an exception provides that. This is the crux of §3.2.

Upstream is explicit that this is control flow, not error handling, and pays to keep it cheap:

> Control-flow throw: it happens on every read of a pending source, so in production skip V8's eager
> stack capture (proportional to stack depth — real cost under SSR) by zeroing the V8-specific
> stackTraceLimit around super().
> — [`core/error.ts:34-39`](../solid/packages/signals/src/core/error.ts#L34)

An errored async source **self-heals**: it is retried on a genuine later-cycle reactive re-read, not on
every read ([`core.ts:1058-1063`](../solid/packages/signals/src/core/core.ts#L1058)).

### 1.3 Pending and error as first-class channels

`_statusFlags` is a **third flag word**, orthogonal to `_flags` (dirtiness) and `_config` (static config) —
[`core/constants.ts`](../solid/packages/signals/src/core/constants.ts): `STATUS_NONE`
[`:65`](../solid/packages/signals/src/core/constants.ts#L65), `STATUS_PENDING`
[`:66`](../solid/packages/signals/src/core/constants.ts#L66), `STATUS_ERROR`
[`:67`](../solid/packages/signals/src/core/constants.ts#L67), `STATUS_UNINITIALIZED`
[`:68`](../solid/packages/signals/src/core/constants.ts#L68).

**"Stale" and "not ready" are orthogonal.** `_flags` carries `REACTIVE_CHECK`/`REACTIVE_DIRTY`
([`:2-3`](../solid/packages/signals/src/core/constants.ts#L2)) — the classic marking every prior-art
library has. `_statusFlags` is a *second axis that no prior-art library has at all* (§2). **That
orthogonality is the entire v2 delta in one sentence.**

#### Pending propagates as a *set*, not a bit

```ts
export function addPendingSource(el: Computed<any>, source: Computed<any>): boolean {
  if (el._x?._pendingSources?.has(source)) return false;
  (ext(el)._pendingSources ??= new Set()).add(source);
  return true;
}
```
— [`async.ts:45-49`](../solid/packages/signals/src/core/async.ts#L45)

Reachability is checked *through* those sets — `if (dep === source || dep._x?._pendingSources?.has(source)) return true;`
([`:70`](../solid/packages/signals/src/core/async.ts#L70)). On suspension the blocking source is recorded
while *preserving* uninitialized:

```ts
if (status === STATUS_PENDING && pendingSource) {
  addPendingSource(el, pendingSource);
  el._statusFlags = STATUS_PENDING | (el._statusFlags & STATUS_UNINITIALIZED);
}
```
— [`async.ts:746-748`](../solid/packages/signals/src/core/async.ts#L746)

and clears only when the set **drains** ([`:200`](../solid/packages/signals/src/core/async.ts#L200),
[`:211`](../solid/packages/signals/src/core/async.ts#L211)).

#### The diamond: one pending branch, one settled branch

`s → a` (async, pending), `s → b` (sync, settled), `c = f(a, b)`:

1. `c` reads `b` (links, gets a value), then reads `a` — pending, so `read()` **links the edge and
   throws** `NotReadyError(a)`. `c`'s body aborts mid-way, retaining **both** edges.
2. `c._pendingSources = {a}`, `c._statusFlags |= STATUS_PENDING`.
3. **A later write to `b` alone** marks `c` dirty on the ordinary `_flags` path; `c` re-runs, re-throws on
   `a`; the `has` guard makes re-adding a no-op; `c` **stays pending and produces no value**. Correct: a
   settled branch changing must not make a blocked join readable.
4. When `a` settles, the walk removes `a`, the set drains, `STATUS_PENDING` clears, `c` completes once.

Two rules a port must honour:

- **Use a set from the start.** [`async.ts:41-44`](../solid/packages/signals/src/core/async.ts#L41)
  records that an earlier single-slot-promoted-to-Set design produced stranded-forever-pending bugs
  (#2893). Multiple flights must each be tracked.
- **A pending node is exempt from auto-dispose** ([`:138`](../solid/packages/signals/src/core/async.ts#L138),
  [`:459-466`](../solid/packages/signals/src/core/async.ts#L459)) — a suspended node that loses its last
  subscriber must survive, or its flight lands on a dead node.

Error rides the same channel, with a .NET-relevant subtlety: **a rejected promise carrying a
`NotReadyError` is pending, not errored** — `let stillPending = error instanceof NotReadyError;`
([`:319-320`](../solid/packages/signals/src/core/async.ts#L319)) →
`notifyStatus(el, stillPending ? STATUS_PENDING : STATUS_ERROR, error)`
([`:349`](../solid/packages/signals/src/core/async.ts#L349)). A .NET port must do this *through*
`AggregateException` unwrapping, which JS never faces.

### 1.4 Boundaries are graph primitives, not JSX

```ts
export function createLoadingBoundary<T, U>(
  fn: () => T,
  fallback: () => U,
  options?: { on?: () => any }
): Accessor<T | U> {
  return createCollectionBoundary<T | U>(STATUS_PENDING, fn, () => fallback(), options?.on);
}
```
— [`boundaries.ts:469-475`](../solid/packages/signals/src/boundaries.ts#L469)

Note the first argument: **the boundary is parameterised by the status flag it collects.**
`createErrorBoundary` ([`:500`](../solid/packages/signals/src/boundaries.ts#L500)) is the same function
with `STATUS_ERROR`. One mechanism, two channels. The catch consults **both** channels:

```ts
try { read(tree); } catch (e) {
  if (e instanceof NotReadyError) pending = true;
  else throw e;
}
queue._pending = pending || !!(tree._statusFlags & type) || tree._x?._error instanceof NotReadyError;
```
— [`boundaries.ts:407-416`](../solid/packages/signals/src/boundaries.ts#L407)

`flattenArray` shows the *collecting* variant — catch per child, keep going, rethrow one at the end, so
siblings all get to **start** their flights rather than serialising
([`:661-668`](../solid/packages/signals/src/boundaries.ts#L661)).

`<Loading>` ([`flow.ts:446`](../solid/packages/solid/src/client/flow.ts#L446)) and `<Errored>`
([`flow.ts:395`](../solid/packages/solid/src/client/flow.ts#L395)) are thin wrappers — `Loading` is
literally `createLoadingBoundary(() => props.children, () => props.fallback, …)`
([`flow.ts:452-455`](../solid/packages/solid/src/client/flow.ts#L452)).
**Boundaries come free with no UI layer. They are graph constructs.**

`isPending`/`latest` are **ambient read modes**: `pendingCheckActive`
([`core.ts:125`](../solid/packages/signals/src/core/core.ts#L125)) and `latestReadActive`
([`:126`](../solid/packages/signals/src/core/core.ts#L126)), dispatched at the top of `read()`
([`:941`](../solid/packages/signals/src/core/core.ts#L941), [`:957`](../solid/packages/signals/src/core/core.ts#L957)).

### 1.5 Projections — the `createAsync` successor, verbatim

**Nothing is named `createAsync`.** The successors, as Partas already transcribes them:

```fsharp
// Async memo — compute returns a Promise.            Partas.Solid/SolidBindings.fs:613
static member createMemo<'T>(compute: 'T option -> JS.Promise<'T>, ?name: string,
                             ?transparent: bool, ?equals: EqualityFunc<'T>,
                             ?unobserved: unit -> unit, ?``lazy``: bool, ?sync: bool,
                             ?loadingValue: 'T) : Accessor<'T>

// Async effect —                                     SolidBindings.fs:590
static member createEffect<'T>(compute: 'T option -> JS.Promise<'T>, effectFn: 'T -> unit,
                               ?defer: bool, ?schedule: bool, ?sync: bool, ?transparent: bool) : unit

// Projection = a computed STORE.                     SolidBindings.fs:686
static member inline createProjection<'T, 'I when 'T:(member id: 'I)>(
    fn: 'T -> U2<'T option, JS.Promise<'T option>>, seed: 'T) : RefreshableStoreReturn<'T>
```

The projection is where the pending channel must be enforced at the **read-trap** level:

> every family node carries the projection computed as its firewall — reads link the derive's status and
> lifecycle natively. **The §6c status gate in the traps makes an uninitialized async derive's seed
> unobservable through every read surface.**
> — [`store/next/projection.ts:1-11`](../solid/packages/signals/src/store/next/projection.ts#L1)

The mechanism is a **firewall** node interposed between store leaves and consumers, so a leaf read links
the *derive's* status — `const owner = firewall || el;`
([`core.ts:948-950`](../solid/packages/signals/src/core/core.ts#L948)). **Skip the store layer and
firewalls disappear with it.**

### 1.6 The v1→v2 removals, read as design intent

[`solid/packages/solid/src/index.ts:153-211`](../solid/packages/solid/src/index.ts#L153) is a commented
block of every deleted 1.x export, each with a one-word reason:

| Removed | Line | Reason given | What it proves |
| --- | --- | --- | --- |
| `createResource` | [`:167`](../solid/packages/solid/src/index.ts#L167) | `// all computations` | **The thesis in three words.** Async is not a primitive; it is a property every computation has. |
| `Suspense` | [`:186`](../solid/packages/solid/src/index.ts#L186) | `// Loading` | Suspense was a *render* concept; `Loading` is a *graph* concept. The boundary moved down a layer. |
| `SuspenseList` | [`:187`](../solid/packages/solid/src/index.ts#L187) | `// replaced by Reveal + createRevealOrder` | Reveal ordering is graph-level now. |
| `onMount` | [`:181`](../solid/packages/solid/src/index.ts#L181) | `// onSettled` | "After first render" is meaningless when first render can suspend. |
| `batch` | [`:163`](../solid/packages/solid/src/index.ts#L163) | `// flush` | The operation is now *drain the queue*, and the queue includes async settle continuations. |
| `onError` | [`:180`](../solid/packages/solid/src/index.ts#L180) | `// handled by ErrorBoundary` | Error stops being a callback, becomes a graph channel. |
| `resetErrorBoundaries` | [`:182`](../solid/packages/solid/src/index.ts#L182) | `// no longer needed with healing` | Recovery is automatic re-read-driven retry (§1.2). |
| `from`, `observable` | [`:172`](../solid/packages/solid/src/index.ts#L172), [`:179`](../solid/packages/solid/src/index.ts#L179) | `// handled by async iterators` | Rx interop replaced by `AsyncIterable` in the compute return type. |
| `createDeferred` | [`:166`](../solid/packages/solid/src/index.ts#L166) | `// take it outside` | Subsumed by transitions/lanes. |
| `createSelector` | [`:169`](../solid/packages/solid/src/index.ts#L169) | `// createProjection` | |

Plus the whole `Resource*` type family ([`:199-208`](../solid/packages/solid/src/index.ts#L199)).

**Conclusion: v2 is not "v1 plus async."** It is v1 *restructured around* async, with every construct that
modelled asynchrony as a special case deleted. A port that bolts a pending channel onto a v1-shaped graph
gets the easy 80%; this list is the map of the other 20%.

### 1.7 Size of the target

| Area | Lines |
| --- | --- |
| `core/core.ts` | 1406 |
| `core/scheduler.ts` | 1044 |
| `core/attribution.ts` | 818 |
| `core/async.ts` | 807 |
| `boundaries.ts` | 668 |
| `core/verdict.ts` | 593 |
| `store/next/store.ts` | 1892 |
| **`packages/signals/src` total** | **~14,300** |

⚠️ **Estimate:** the **v2 delta specifically** (`async.ts` + `verdict.ts` + `boundaries.ts` + the status
branches of `read`/`recompute`) is **~2,200–2,600 lines**. Hold that number — §5 dwarfs it.

### 1.8 The binding side is already done

Partas binds essentially the entire v2 async surface, all as `[<Erase>]` +
`[<ImportMember "solid-js">] … jsNative` — **no compiled reactive core whatsoever**
([`SolidBindings.fs:563-565`](../Partas.Solid/SolidBindings.fs#L563),
[`:646-662`](../Partas.Solid/SolidBindings.fs#L646)):

async `createMemo` [`:613`](../Partas.Solid/SolidBindings.fs#L613) · async `createEffect`
[`:590`](../Partas.Solid/SolidBindings.fs#L590) · async `createSignal`
[`:658`](../Partas.Solid/SolidBindings.fs#L658) · `createProjection`
[`:686`](../Partas.Solid/SolidBindings.fs#L686) · `createOptimistic`
[`:627`](../Partas.Solid/SolidBindings.fs#L627) · `flush` [`:660`](../Partas.Solid/SolidBindings.fs#L660) ·
`isPending` [`:664`](../Partas.Solid/SolidBindings.fs#L664) · `latest`
[`:666`](../Partas.Solid/SolidBindings.fs#L666) · `onSettled` [`:746`](../Partas.Solid/SolidBindings.fs#L746) ·
`refresh` [`:751`](../Partas.Solid/SolidBindings.fs#L751) · `resolve`
[`:877`](../Partas.Solid/SolidBindings.fs#L877) · `createErrorBoundary`/`createLoadingBoundary`
[`:843-845`](../Partas.Solid/SolidBindings.fs#L843) · `<Loading>`
[`:172`](../Partas.Solid/SolidBindings.fs#L172) · `<Errored>`
[`:85`](../Partas.Solid/SolidBindings.fs#L85) · `<Reveal>` [`:191`](../Partas.Solid/SolidBindings.fs#L191).

**One real gap: `NotReadyError` is unbound** — the only way to write a correct custom boundary in user
code. Also unbound: `enforceLoadingBoundary`, `resetErrorHalt`, `isEqual`, `createOwner`. ⚠️ Estimated
effort to close: **under a day.**

**Answer to half one of the question: binding the v2 async graph from F# is not merely feasible, it is
~95% done, and it works with no plugin involvement at all.**

---

## §2. Is the pending channel novel?

> Does any prior art propagate a **pending** channel through a dependency graph, or do they all stop at
> value + cutoff?

**They all stop at value + cutoff. Plainly and without qualification.**

| Library | v1 layer | Glitch-free | **Pending channel through the graph** |
| --- | --- | --- | --- |
| Adapton / Acar | ✅ | ✅ | ❌ |
| Salsa | ✅ | ✅ | ❌ |
| Jane Street `Incremental` (OCaml) | ✅ | ✅ | ❌ |
| FSharp.Data.Adaptive | ✅ explicit token | ✅ level queue | ❌ **none** |
| Clef `Incremental<'T>` | ✅ | ✅ normative ascending height | ❌ **`[Not yet specified]`** |
| SignalsDotnet | ✅ ambient | ❌ push/Rx | ⚠️ `IsComputing` side-flag — no suspension, no propagation |
| Cortex.Net (MobX, Fody) | ✅ | ⚠️ not claimed | ❌ not surfaced |
| Angular signals | ✅ | ✅ | ❌ (`resource()` is a value-with-status object, not a graph channel) |
| **Fable.Ripple** | ✅ ambient, dynamic | ✅ | ❌ `NodeState = Clean\|Check\|Dirty`, full stop |
| Rx / ReactiveUI | ❌ manual deps | ❌ glitchy | ❌ `OnError`/`OnCompleted` are *terminal* |
| **Solid 2** | ✅ | ✅ height heap | ✅ **`STATUS_PENDING` + `_pendingSources` + `NotReadyError`** |

### 2.1 FSharp.Data.Adaptive

Tracking is **by explicit token, not ambient**: `EvaluateAlways` reads `let caller = token.caller`, calls
`f (token.WithCaller x)`, then `x.Outputs.Add caller` and
`caller.Level <- max caller.Level (x.Level + 1)`. That is *why* `adaptive { }` needs `let!` — the token
must be threaded. It is exactly the monadic model §3.2 warns against. Glitch-freedom is Solid's
mechanism by another name (`TransactQueue` keyed on `Level`, drained ascending, `LevelChangedException`
re-levels). `transact` state is `[<ThreadStatic>]`, and a bespoke `AdaptiveSynchronizationContext` exists
**solely** because WPF inlines work on contended locks and corrupts those thread-statics — the clearest
empirical warning available against §3.1's locking option. **No pending channel of any kind.**

### 2.2 SignalsDotnet — the instructive near-miss

Auto-tracking *is* ambient and read-based. And async exists — and **does not suspend**:
`Signal.AsyncComputed(async ct -> …, defaultValue, ConcurrentChangeStrategy)` returns
`IAsyncReadOnlySignal<T>` with a **separate `IsComputing` signal**. While unresolved, `.Value` returns the
last value. *"There is no loading sentinel or exception on read."* Consumers compose
`!x.IsComputing.Value && x.Value` **by hand, at every consumer**, and it does **not propagate** — a
consumer of a consumer must re-derive it. **This is precisely the manual-flag model Solid deleted
`createResource` to abolish** (§1.6). It is also push-based on R3 and not glitch-free
(`Effect.AtomicOperation` exists because intermediate inconsistent states are otherwise observable),
declares itself not thread-safe, and its README **warns against relying on `[ThreadStatic]`** —
corroborating §3.1. It is *ahead* of Solid on exactly one axis: `CancelCurrent`/`ScheduleNext` +
`CancellationToken`.

### 2.3 Clef — nearest prior art, same hole

[clef-lang.com](https://clef-lang.com), an F#-dialect systems language. From the spec drafts:

- `Signal`/`Memo`/`Effect`/`Batch`/`Store` are explicitly "in the style of SolidJS", a "thin surface
  layer, not a separate reactive engine", desugaring onto `Observable<'T>` (opaque push) and
  `Incremental<'T>` (demand-driven, cutoff by equality).
- `Incremental<'T>` is Solid's v1 layer by another name — node fields `value, stale, height,
  dependencies, dependents, cutoff, recompute`; normative rule 6: *"Stabilization SHALL process nodes in
  ascending height order."*
- **No async/pending/suspense anywhere.** The SolidJS comparison table (§14) has exactly six rows —
  `createSignal`, the getter call, `setSignal`, `createMemo`, `createEffect`, `batch`. **No
  `createResource` row, no `Suspense` row** — neither equivalent nor stated absence. `Incremental` §6.2
  marks *"asynchronous suspension and failure behavior"* `[Not yet specified]`, noting it would "require a
  complete observable contract". `Observable<'T>` has **no pending state, no error channel, no completion
  signal**.
- **Tracking is resolved at compile time via the PSG** — applicative tracking yields static edges,
  dynamic tracking only where unprovable, unsupported cases *"SHALL be diagnosed rather than silently
  treated as fixed dependencies"*; §12 explicitly **retires the runtime `CurrentTracking` global**. *Clef
  buys away §3.1's threading problem with a compiler; a library pays it.*
- Rule 7: lifetimes *"SHALL be tied to the enclosing actor/region with deterministic release … no GC or
  finalizer SHALL be required to dispose."*
- Lineage: declared debt to **FSharp.Data.Adaptive** and **Jane Street `Incremental`**; references
  **Adapton** (PLDI '14) and Acar. *(Salsa is not mentioned on the page — unconfirmed for Clef
  specifically, though Salsa itself is value+cutoff.)*

> **Caveat:** working draft (`/spec/draft/`, "Status: Revised", 2026-09-18). §14 states the SolidJS
> comparison *"is not a claim of complete semantic equivalence"*. Treat Clef as well-specified *intent*,
> not a shipped reference implementation.

### 2.4 Why push-based streams are the wrong model

Glitches (`c = a + b` both from `s` → `c` emits on a mixed state; Rx has no node height to suppress it);
no auto-tracking (deps enumerated by hand, so control-flow-dependent dependency sets are inexpressible);
and `OnError`/`OnCompleted` are **terminal**, whereas pending is non-terminal, recoverable and per-node.

### 2.5 The finding

> **A per-node, non-terminal, recoverable, transitively-propagated "not ready" status, such that a
> consumer written as ordinary straight-line code needs no knowledge that any of its inputs are
> asynchronous, and a boundary — not the consumer — decides what happens meanwhile.**

**Nobody has this.** Everything else already exists in .NET. The pending channel is genuinely novel within
the Adapton → Jane Street → Adaptive → Clef lineage, **and it is the only defensible reason to build
anything.** Hold that too — §7 turns on it.

---

## §3. Native .NET feasibility

### 3.0 No compiler is needed — settled from source

Tracking is entirely a runtime ambient-variable discipline: `context` and `tracking` are module-level
mutables ([`core.ts:111`](../solid/packages/signals/src/core/core.ts#L111),
[`:127`](../solid/packages/signals/src/core/core.ts#L127)); `recompute` sets them
([`:239-252`](../solid/packages/signals/src/core/core.ts#L239)); `read` consults and links
([`:1004`](../solid/packages/signals/src/core/core.ts#L1004)). **There is no Babel involvement anywhere in
`packages/signals/`.** Solid's compiler does JSX→DOM only. A faithful .NET port of the *graph* needs no
compiler at all. (Fable.Ripple independently confirms this: a working F# graph with **no plugin** — §6.)

### 3.1 Threading and ownership

JS gives Solid zero synchronization cost; every global above is unguarded. The **pending channel is what
forces the issue** — a `Task` continuation lands on a pool thread and must write into the graph and run
the settle walk.

| Option | Verdict |
| --- | --- |
| **(1) Dispatcher affinity** (WPF/Avalonia `Dispatcher`, Blazor `InvokeAsync`) | **Recommended for UI.** Honest analogue of JS's single thread; ambient state stays plain mutable. |
| **(2) Serializing island** — non-overlapping turns, no thread affinity, pumped on the pool | **Recommended for server/headless.** Exactly SignalsDotnet's `SignalIsland<T>`; also where Clef's actor/region scoping lands. |
| (3) Per-node `Monitor` (Adaptive's model) | Possible, but Adaptive needed a custom `SynchronizationContext` to survive WPF (§2.1). Strong warning. |
| (4) Lock-free concurrent graph | No prior art attempts it. Out of scope. |

**Ambient-state trap specific to the pending channel:** the tracking global must **not** be
`[ThreadStatic]` (continuations resume elsewhere) and must **not** be naive `AsyncLocal` — *it flows into
awaited continuations*, which is exactly wrong, since code resuming after an `await` is outside the
tracked synchronous window and must register no dependencies. Correct: an **instance-held graph context**
with `using`-scoped push/pop, asserting the scope exits on the same logical turn.

**Ownership × pending:** a pending node must survive losing its last subscriber
([`async.ts:138`](../solid/packages/signals/src/core/async.ts#L138),
[`:459-466`](../solid/packages/signals/src/core/async.ts#L459)). On .NET: the in-flight `Task` holds a
strong reference to its node, and disposing a scope with pending children must either cancel via
`CancellationToken` or drain. Clef's rule 7 and Ripple's `Scope` ("No weak references: disposal is
explicit and deterministic") both independently land on deterministic teardown; a pending channel makes it
non-negotiable.

### 3.2 Exceptions as the not-ready sentinel

In JS, throwing a stack-limit-zeroed `Error` is cheap enough to do on **every read of every pending
node**, and upstream still optimised it
([`error.ts:34-39`](../solid/packages/signals/src/core/error.ts#L34)). On .NET a throw means stack
unwinding, two-pass SEH on Windows, and a first-chance exception that stops the debugger by default.

| Design | Transparent straight-line authoring? | Notes |
| --- | --- | --- |
| **(i) Port the throw** (`NotReadyException`) | ✅ fully | **Cache one instance per source** — Solid effectively already reuses `ext(el)._error` ([`async.ts:97-103`](../solid/packages/signals/src/core/async.ts#L97)). A cached, stack-trace-free throw is far cheaper than `throw new`. |
| **(ii) Sentinel return + CE whose `Bind` short-circuits** | ⚠️ **No** — every read needs `let!`, virally, through every helper | This *is* `adaptive { }`. It is v1 plus a Maybe monad, and it **destroys the thesis**. |
| **(iii) Hybrid** — sentinel inside a CE, cached throw for plain reads | ✅ | Two semantics to keep consistent; boundaries must handle both. |

**Why (ii) is not a real alternative:** a sentinel return does not *abort*. §1.2 point 3 — the throw's job
is non-local abort through frames that know nothing about asynchrony. With a sentinel, a helper three
frames down must itself return the sentinel and its caller must propagate it: the viral monadic colouring
that v2 exists to delete.

> ⚠️ **Speculation, labelled.** Not benchmarked. Common figures (JS throw ~0.1–1 µs, .NET ~5–50 µs) are
> general knowledge, not measurement here. **This one number decides the architecture** and is item 1 of
> the §7 experiment.

### 3.3 Feeding a `Task` into a synchronous, re-runnable body

The body must never `await` (it restarts from the top). Solid's resolution: the body *returns* the
promise; the engine owns the awaiting.

```fsharp
Memo.createAsync (fun (ct: CancellationToken) ->
    let id = selectedId.Value          // tracked — synchronous, before the return
    fetchUserAsync (id, ct))           // Task<User>; engine awaits, sets Pending
```

| Problem | Recommendation |
| --- | --- |
| `Task` vs `ValueTask` vs `Async<'T>` | **`Task<'T>` primitive.** `ValueTask` cannot be awaited twice — fatal, since the engine stores the flight and may re-consult it. `Async<'T>` (cold, restartable) has arguably better semantics; ship it as an adapter. |
| Continuation thread | `ConfigureAwait(false)` + **explicit marshal** into the graph context. Relying on captured context ties the library to UI hosts. |
| Superseded flight | **Policy enum + `CancellationToken` into the body.** The one place .NET is *strictly better* — Solid has no cancellation; a superseded promise runs to completion and its result is dropped. |
| `AsyncIterable` | `IAsyncEnumerable<'T>`. Direct, and better typed. |
| Faulted task carrying not-ready | Port [`async.ts:319-320`](../solid/packages/signals/src/core/async.ts#L319) **through `AggregateException` unwrapping**. |

**Answer to half one, second part: a native .NET port of the v2 graph is feasible. No compiler needed.
Three real problems, all with known resolutions, one of which (3.2) is unmeasured and decisive.**

---

## §4. Dual-target architecture — one F# source, .NET + Fable/JS

### 4.1 Is the architecture coherent?

**Yes, structurally — and it is exactly what Clef's normative rule 8 describes:** one source compiling to
both the LLVM native pathway and the JSIR JavaScript pathway, achieved by defining reactivity as
**compiler intrinsics** desugared through a middle end that *"commits to no target"* — i.e. semantics
specified above both backends and **implemented twice**.

**The asymmetry in our favour is real:** we get the .NET backend *free* (it is just F#), and hand-tune the
JS side via a Fable plugin. Clef built an entire compiler to get what we would get from `dotnet build`
plus `dotnet fable`.

**But the asymmetry also cuts the other way.** Clef's own
[Carrying Proofs into JavaScript](https://clef-lang.com/blog/carrying-proofs-into-javascript/) states that
**the F#/Fable path is what works today while the JSIR backend is a *proposal*** — even the project that
made dual-targeting normative has not shipped the native→JS half.
[Native Reactivity in Clef](https://clef-lang.com/blog/native-reactivity-in-clef/) concedes the semantics
*diverge* across backends: native "keeps cached values valid across admitted region lifetimes" whereas
JavaScript "uses host-managed storage, explicit logical retirement", with native pointers/arenas excluded
from the JS-facing API.

**⚠️ My read: the asymmetry is workable *for the graph*,** because the graph is small (§1.7) and the
divergent concerns — threading, deterministic disposal, exception cost — are precisely the ones that
**vanish** on the JS side rather than conflicting with it. One thread, GC teardown and cheap throws are a
*relaxation* of the native constraints, not a contradiction. A native-correct graph compiled to JS is
**over-engineered for JS, not wrong for JS**. You pay a little overhead, not correctness. That is a
tolerable asymmetry — and it is a much weaker claim than "the DOM layer dual-targets", which is §5.

### 4.2 Where native and JS optimisation pull in opposite directions

| Concern | Native .NET wants | Fable/JS wants | Opposed? |
| --- | --- | --- | --- |
| Node representation | pooled class, cache-dense fields | plain object, **monomorphic hidden class**, fields assigned in constant order | **No — same discipline.** Both want a fixed, flat, non-polymorphic node shape. (`[<Struct>]` on a node is wrong for *both* — nodes are identity-bearing and aliased.) |
| Optionality | `voption` — no allocation | `voption` erases cleanly; `option` allocates | **No — `voption` is right for both.** |
| Dep storage | `ArrayPool`/`Span`, avoid `ResizeArray` churn | avoid per-node array allocation; inline the first edge | **No — same.** Ripple's inline-first-edge layout is the trick both targets want. |
| Equality / cutoff | devirtualized comparer; structural equality often *desirable* | **reference `===`** or a monomorphic call; `Unchecked.equals` is structural and slow | **YES — genuinely opposed.** |
| Closures | avoid allocation; static lambdas / interface dispatch | closures are cheap and idiomatic; interface dispatch risks megamorphism | **Mildly opposed.** |
| Exceptions | **expensive** — the §3.2 blocker | cheap | **YES — the deepest divergence.** |
| Threading | dispatcher/island + scoped ambient context | irrelevant; single thread | Not opposed — JS ignores it, but ships the scaffolding. |
| Disposal | deterministic, no finalizers | GC adequate | Not opposed; JS overpays. |

**Honest tally: only two real oppositions — equality lowering and exception cost.** Both are resolvable
**without `#if FABLE_COMPILER` in the core**, by making them *construction-time policy* rather than
inline branches:

- **Equality:** cutoff becomes a parameter of node construction, defaulted per target at the library's
  *edge* (module initialisation), never branched in the read path. A plugin can then lower the JS default
  to a literal `===`. This is a genuine, narrow, high-value plugin job — and note it is exactly where
  Ripple is weakest (`Unchecked.equals`, `Api.fs:11,37`).
- **Exceptions:** if §3.2's benchmark forces hybrid design (iii) natively, the JS build can still use the
  throw path unconditionally — again a construction-time choice.

**⚠️ Claim, labelled:** a ~2,500-line core can serve both targets with **zero or near-zero
`#if FABLE_COMPILER` in the graph**, provided target-divergent policy is injected at construction. That
is achievable — but it is a discipline that must hold for the life of the project, and disciplines erode.

### 4.3 How much leverage does a plugin actually have on graph internals?

**Less than the proposal assumes, and this is the finding that should change the plan.**

Established fact (verified from Ripple's shipped F# sources): Fable.Ripple has **no Fable plugin at all**,
and exactly **one** interop attribute in 859 lines of core (`[<Struct; Erase>]`, `Types.fs:67`) — the core
is 100% Fable-generated. **And yet it is not naive F#.** It is written *backwards from Fable's codegen*:
erased int enums (`ReactiveNode.fs:3-12`), an inline-first-edge layout deliberately avoiding a
`ResizeArray` per node (`:44-61`), a shared default closure (`:14-18`), `voption` throughout,
`obj.ReferenceEquals` on the tracking fast path (`Tracking.fs:34`), allocation-free reads on a stable
graph.

**The conclusion is uncomfortable but clear: most of what a graph-internals plugin would do can be had by
writing F# that already knows what Fable emits.** The remaining codegen margin is small. The one clear
exception is the cutoff comparer (above).

**⚠️ Plugin scope estimate, graph only.** Current plugin: `Plugin.fs` 1132 + `Utils.fs` 651 + `Spec.fs`
428 + `Types.fs` 274 = **2,485 lines** (excluding `Storybook.fs` 1432), doing one thing — recognise a DSL
shape, rewrite it to JSX. A graph-internals optimiser is a **different kind of pass**: it must reason
about value flow rather than match syntactic shapes, which is where Fable plugins are weakest —
`MemberDeclarationPluginAttribute` hands you one member's AST, not a whole-program view. **Estimate:
~600–1,200 lines for a narrow version (equality lowering + accessor inlining), buying perhaps single-digit
percent over well-written F#** — because Ripple has already demonstrated the ceiling without one.

**Recommendation for §4: do not spend plugin effort on the reactive graph.** Write codegen-aware F#, as
Ripple did, and choose the equality default at construction. Save the plugin for §5 — where the multiples
actually are.

---

## §5. The DOM cost centre

**Confirmed, emphatically: this dominates the effort estimate and is larger than the reactive graph — by
roughly an order of magnitude.** Measured in this checkout:

| Component | Lines | What it is |
| --- | --- | --- |
| `dom-expressions/packages/babel-plugin-jsx/src` | **6,312** | The **compiler**: template extraction, precise bindings |
| — of which `dom/element.ts` | 1,670 | per-element attribute/event/property lowering |
| — `shared/utils.ts` | 839 | |
| — `dom/template.ts` | 426 | template hoisting / cloning |
| `dom-expressions/packages/runtime/src` (`.js` only) | **15,555** | `template()`, `insert()`, `spread()`, event delegation, hydration, SSR |
| **DOM total** | **~21,900** | |
| | | |
| `solid/packages/signals/src` (whole reactive engine) | 14,300 | |
| **v2 delta only** (§1.7) | **~2,400** | |

**The DOM layer is ~9× the v2 delta and ~1.5× the entire reactive engine** — and that is the *mature*
version, whose per-element rules encode a decade of browser quirks (`dom/element.ts` alone is 1,670 lines
of exactly that).

**Would our plugin need template extraction? To match Solid's rendering, yes.** Fine-grained DOM
performance in Solid does not come from the reactive graph; it comes from the compiler turning static JSX
subtrees into one cloned `<template>` plus a handful of precise bindings. Without it you get what every
hand-written DOM DSL gets: per-element imperative construction — Ripple's `Fable.Ripple.Dom` approach.
Legitimate, works, and structurally slower on first render and large-list reconciliation.

**I confirm the coordinator's read, and put it more strongly: the DOM layer is not a cost centre within
the project — it *is* the project.** The reactive graph, pending channel included, is a ~2,400-line
footnote beside it. Going F#-first means either (a) our plugin emits template-cloning code itself —
dom-expressions-scale compiler work — or (b) we hand-write an imperative DOM layer and concede the
performance argument plus a multi-year maturity gap on quirk handling. There is no third option.

**One nuance where I diverge slightly from the other agent's framing:** *if the target is native .NET
only* (Avalonia, WPF, Blazor Server, headless), **this entire section evaporates** — there is no DOM to
compile; the host toolkit renders. The DOM cost is a cost of *dual-targeting to the browser*, not a cost
of the pending channel. **That distinction is what §7 turns on.**

---

## §6. Competitive position — Fable.Ripple

[github.com/fable-hub/Fable.Ripple](https://github.com/fable-hub/Fable.Ripple) · Maxime Mangel · MIT ·
beta · ~12 commits · ~1 week old. Verified from the shipped F# sources, not the docs site (which is thin —
the reference index lists six type names, the guide states no semantic guarantees).

| Question | Answer |
| --- | --- |
| Primitive surface | `Var.create/createWith/.Value`; `Signal.constant/computed/computedWith/map/mapWith/map2/map3/bind/peek/subscribe/effect/batch/untracked/root/onCleanup/observerCount`; `SignalBuilder`. 150 lines in `Api.fs`. Satisfies signal/computed/effect/batch/root. |
| Tracking automatic **and dynamic**? | **Yes, both.** `let mutable private current: ReactiveNode voption` (`Internal/Tracking.fs`) — ambient global, same shape as Solid's `context`. Edges are *reconciled*: reads matched positionally against the previous run, divergent tail re-collected. Conditional reads re-track per run. |
| Owner/scope/cleanup? | **Yes.** `Internal/Scope.fs` — nested `Scope` with `Nodes`/`Cleanups`/`Children`, innermost-out teardown, *"No weak references: disposal is explicit and deterministic."* |
| **Any async in the core graph?** | **No.** Zero `Promise`/`Async`/`Task`/`pending`/`loading` in `Api.fs`. `NodeState = Clean = 0 \| Check = 1 \| Dirty = 2`, *"Ordered Clean < Check < Dirty"* (`ReactiveNode.fs:3-12`). **One axis. Value + cutoff.** The only async in the ecosystem is debounced validation in `Fable.Ripple.Form` — outside the graph. |
| Native .NET target? | **No.** `netstandard2.1` + `<FablePackageType>library</FablePackageType>` + `<PackageTags>fable-javascript</PackageTags>`. Fable/JS by intent. (Internals *look* portable — no JS interop in the graph — but that is incidental and untested.) |
| **Own Fable plugin? `[<Emit>]`ed hot paths?** | **Neither.** No plugin anywhere in the repo tree; one interop attribute in 859 lines of core (`[<Struct; Erase>]`, `Types.fs:67`). 100% Fable-generated. |
| Hot read path allocates? | **No** on a stable graph — inline-first-edge layout (`ReactiveNode.fs:44-61`), shared default closure (`:14-18`), `voption` throughout, `obj.ReferenceEquals` on the tracking fast path (`Tracking.fs:34`). |
| Structural equality? | **Yes — its one soft spot.** Cutoff is `Unchecked.equals` (`Api.fs:11,37`) vs Solid's inline `===`. |
| From-scratch F#? | **Yes.** `ReactiveNode.fs`, `Internal/{Graph,Tracking,Scheduler,Scope}.fs`, `Types.fs`, `Api.fs`, `Builder.fs`. Not a binding over a JS signals library. Fable.Core 5.2.0. |

**It is written backwards from Fable's codegen.** That is the single most important competitive fact here,
because it collapses §4's plugin thesis: **the available codegen margin over Ripple's graph is small.**

### 6.1 Is "synchronous settle, no tick" structurally incompatible with a v2 async graph?

The stated guarantee — *"Synchronous write settles before next line runs. No scheduler, no tick."* — is
substantiated by source:

```fsharp
/// A source's value changed: mark observers and flush unless batching.
let notifyChange (source: ReactiveNode) =
    Graph.iterObservers source (fun o -> stale o NodeState.Dirty)
    if batchDepth = 0 then flush ()
```
— `Internal/Scheduler.fs`

versus Solid's deferral: `if (!syncDepth && !globalQueue._running && !projectionWriteActive) queueMicrotask(flush);`
([`scheduler.ts:240`](../solid/packages/signals/src/core/scheduler.ts#L240)).

**My answer refines rather than simply confirms the coordinator's read: the incompatibility is real, but
it is in the node layout, not in the settle guarantee.**

- The guarantee concerns propagation of *already-known* values: a write's effect queue drains before the
  next line.
- A v2 pending channel does **not** violate that. In Solid, a consumer reading a pending source also
  completes propagation immediately — it completes *by suspending*. The `NotReadyError` is thrown and
  caught within the same synchronous flush; the boundary switches to its fallback synchronously. Nothing
  waits.
- What changes is the guarantee's *meaning*: "settled" would have to mean "the graph reached a consistent
  state, possibly a partially-suspended one" rather than "every node holds a final value."

**But the node representation genuinely blocks it.** `NodeState` is a single *ordered* axis (`stale` uses
`int node.State < int target`). Pending is **orthogonal** to dirtiness (§1.3), so it cannot be a fourth
enum case: it needs a second field plus a `_pendingSources` set on every node. That is a breaking change
to node layout *and* to the meaning of `flush`, in a library whose pitch is the simplicity of the current
model.

**Conclusion: not architecturally excluded, but excluded by positioning.** Ripple is unlikely to grow a v2
pending channel because doing so contradicts its headline claim. **That is durable differentiation, not a
race** — the one thing we could hold that they would not take back.

### 6.2 What they already ship that we would have to match

DOM (`Fable.Ripple.Dom`, no vDOM), forms (`.Form`, `.Form.Plain`), router (`Fable.UrlParser`), component
tests (`.Dom.Test`, Playwright/Chromium), docs site, benchmarks. In ~1 week, under an active maintainer
with the Fable community's attention.

---

## §7. Verdict

### The two paths, side by side

| | **(a) Stay a Solid binding** | **(b) F#-first dual-target framework** |
| --- | --- | --- |
| Reactive graph | **Upstream's.** Already bound — §1.8: the v2 async surface is ~95% done and needs **no plugin**. | Ours. ~2,400 lines of novel v2 delta atop a table-stakes v1 layer. |
| Pending channel | **Free today**, and free as upstream evolves. | Ours to build *and keep correct* — the `_pendingSources` set exists because upstream already shipped the stranded-pending bug and fixed it ([`async.ts:41-44`](../solid/packages/signals/src/core/async.ts#L41)). |
| DOM | **Free** — dom-expressions, ~21,900 lines of compiler + runtime (§5). | **Ours.** Either dom-expressions-scale compiler work in the plugin, or a hand-written imperative layer and concede performance. |
| Native .NET | ❌ impossible | ✅ the unique capability |
| Plugin scope | Current 2,485 lines, DSL→JSX. Stable. | Graph optimiser (⚠️ ~600–1,200 lines, **small payoff** — §4.3) **plus** template extraction (⚠️ multi-thousand, **the whole ballgame** — §5). |
| Upstream tax | Track a moving RC. Cheap — bindings are `[<Erase>]` declarations. | None. But you inherit every bug upstream already found and fixed. |
| Differentiation vs Ripple | **Total and structural** — we erase to real Solid, they hand-roll F#. Different products. | **Converges onto Ripple's turf** — same bet, same target, same audience — and arrives second, without DOM/forms/router/tests. |
| ⚠️ Time to parity | — | Multi-year for the browser story. Ripple reached shipping DOM + forms + router + Playwright in ~1 week *because it skipped the compiler*. Matching Solid's rendering means not skipping it. |
| Risk | Low; RC churn. | High, concentrated in §5 — the part with the least novelty and the most accumulated domain knowledge. |

**Stated bluntly, as asked: path (b) makes Partas.Solid's erase-to-Solid premise obsolete.** Own the graph
*and* the DOM and we no longer need Solid, and this repo's thesis — that F# should be a typed authoring
surface over a real, fast, upstream-maintained runtime — is discarded. **That is a fork, not an
increment.**

### The call

**Take path (a) for the framework. Do not build an F#-first dual-target framework.**

**And separately — build the pending-channel graph as a native-.NET-only library, in its own repo.**

The reasoning, in one chain:

1. **The only novel thing here is the pending channel** (§2.5). Everything else — v1 primitives,
   glitch-freedom, DOM, forms, routing — already exists, in several places, done well.
2. **On the JS target we already have the pending channel, free, from upstream** (§1.8), with zero port
   risk and zero maintenance. Rebuilding it in F# to run in a browser pays ~2,400 lines plus perpetual
   correctness risk for something we already possess.
3. **On the JS target the plugin has almost no leverage on the graph** (§4.3 — Ripple proves the ceiling
   *without* a plugin) and **dom-expressions-scale obligations on the DOM** (§5). The lever is in the
   wrong place: powerful exactly where we would be reinventing mature work, weak exactly where the novelty
   is.
4. **On native .NET the pending channel is unobtainable any other way** — no Solid runtime exists, and
   every option in §2 stops at value + cutoff. **And §5 evaporates**, because there is no DOM to compile:
   Avalonia, WPF, Blazor Server and headless hosts bring their own rendering.
5. Therefore the value is **concentrated entirely in the intersection of "pending channel" and "native
   .NET"** — and the dual-target framework proposal spends nearly its whole budget *outside* that
   intersection.

**The dual-target idea is not wrong; it is aimed at the wrong artefact.** Keep it, and apply it to the
*API surface* instead of the *implementation*: design the native library's `Signal`/`Memo`/`Effect`/`Loading`
shape to be source-compatible with [`SolidBindings.fs`](../Partas.Solid/SolidBindings.fs), so user code
written once targets either — native via the .NET implementation, browser via the existing erased bindings
onto real Solid. **That is API parity by discipline across two implementations, which is precisely what
Clef's rule 8 actually is** (semantics above both backends, implemented twice) — at a fraction of the
cost. It is *not* achieved by running Fable over the native implementation; per §4.1, even Clef has not
shipped that direction.

**Where it lives: a separate repo.** Partas.Solid's coherent position is that it erases to real Solid
while Ripple hand-rolls F#. A third thing — a from-scratch native graph — inside the same repo weakens all
three stories, cannot interoperate with the erased graph (a native `Memo` would be invisible to a
`<Loading>` boundary), and worsens an already awkward TFM matrix (`net6.0` shipped, `net8.0` plugin host,
`net9.0` fable tests, `net10.0` CLI).

**Do immediately regardless:** bind `NotReadyError` (⚠️ under a day). It is the only way to write a
correct custom boundary in user code today and the one real gap in §1.8.

### The smallest experiment that de-risks the recommendation

**An Expecto/console spike, ~350–400 lines, native .NET only. Not a ScratchTests case** — this has nothing
to do with Fable or JSX.

**Step 1 — benchmark before writing any graph (~40 lines, one hour).** BenchmarkDotNet, ~10 frames of call
depth, with and without a debugger attached: (i) throw/catch of a freshly allocated exception; (ii)
throw/catch of a **cached singleton with no stack capture**; (iii) a sentinel-return baseline. **This is
blocker §3.2 and it decides the architecture.** If (ii) is ≲2 µs, design (i) stands and transparent
authoring is affordable. If not, the answer is FSharp.Data.Adaptive plus a monadic CE — v1 with a Maybe
bolted on — the thesis is dead, and **the recommendation collapses to "just stay a Solid binding," saving
the entire project.**

**Step 2 — minimal graph: borrow the v1 layer, build only the delta.** Take the v1 shape from Ripple's
`Api.fs`/`Internal/*.fs` (MIT, and already codegen-aware); do not redesign it. Add only: a second axis
`[<Flags>] Status = None | Pending | Error | Uninitialized` **plus `PendingSources : HashSet<Node>` on
every node** (a set from the start — §1.3); `Memo.createAsync : (CancellationToken -> Task<'T>) -> Memo<'T>`;
`read` throwing a **cached** `NotReadyException`, **linking the edge before throwing**; and
`Boundary.create` as one status-parameterised function mirroring
[`boundaries.ts:469`](../solid/packages/signals/src/boundaries.ts#L469).

**Step 3 — five assertions.** The first is v1 regression insurance; the rest are the delta:

1. **Glitch:** `s → a`, `s → b`, `c = a + b`; one write to `s` ⇒ `c`'s effect fires exactly once, never on
   a mixed state.
2. **Transparent suspension:** `c = f(asyncMemo)` where `f` is plain straight-line F# **two helper
   functions deep**, with no async awareness in `f` or the helpers. Boundary shows fallback, then content;
   `c` never observes a default or null.
3. **Wake-up:** the suspended consumer re-runs when the `Task` completes — i.e. link-before-throw is right.
4. **Diamond with one pending branch:** `a` async-pending, `b` sync-settled, `c = f(a,b)`. Assert `c` is
   pending; write **`b` alone** and assert `c` is *still* pending and produced **no** value; then settle
   `a` and assert `c` completes exactly once. **This is the assertion that distinguishes a real pending
   channel from a boolean flag, and the one no library in §2 could pass.**
5. **Threading:** the `Task` continuation completes on a pool thread and marshals back into the graph
   context without corrupting ambient tracking state.

All five pass ⇒ the native library is sound and the rest is volume, not risk — and the framework question
was never coupled to it. If 2 or 4 fails, .NET cannot carry these semantics, and path (a) was right for
both halves.

---

## §8. What this research could not settle

1. **.NET exception cost was not measured.** §3.2's figures are general knowledge. Blocker #1; step 1 of §7.
2. **Transitions / optimistic lanes read only at the read-path gate.**
   [`core/lanes.ts`](../solid/packages/signals/src/core/lanes.ts),
   [`core/optimistic.ts`](../solid/packages/signals/src/core/optimistic.ts) and most of
   [`core/verdict.ts`](../solid/packages/signals/src/core/verdict.ts) (593 lines of pending/error
   classification) not read in full. Full parity needs them; the §7 spike does not.
3. **`core/attribution.ts` (818 lines) not read.** Appears to be dev-time attribution. Assumed
   non-load-bearing — *not verified*.
4. **Plugin and DOM effort figures in §4.3 and §5 are my estimates**, anchored on measured line counts
   (2,485 current plugin; 6,312 babel-plugin-jsx; 15,555 runtime) but not on any prototype.
5. **Ripple's guarantee sentences were not located verbatim** on the README, landing page, guide
   introduction or reference index; §6.1 rests on `Internal/Scheduler.fs`, which is stronger evidence
   anyway.
6. **Whether Ripple's core compiles for a native .NET TFM was not tested.**
7. **Cortex.Net's async story unconfirmed** from primary source; its ❌ means "not surfaced", not
   "confirmed absent". Same for Salsa's presence in Clef's declared lineage (Adapton, Acar, Jane Street
   `Incremental`, FSharp.Data.Adaptive *are* confirmed).
8. **No Clef implementation inspected** — spec drafts and blog posts only, which self-describe as intended
   semantics.
