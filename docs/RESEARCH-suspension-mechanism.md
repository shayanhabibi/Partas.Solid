# How should a .NET reactive graph suspend? — evaluating four mechanisms

**Status.** Research + experiments run in a scratch directory outside the repo. Nothing in this
repository was changed other than the addition of this file. Nothing committed.

**Revision (§3.1, §A.4).** The cost table has been **re-measured on .NET 10.0.12 and 11.0.0-rc.1** — the
original figures were `net9.0` only and were flagged as provisional. Throws are ~30% cheaper than first
reported; every relative conclusion survives. Separately, §A.3's diamond has been **executed as code**
rather than reasoned about (§A.4): the glitch reproduces exactly, 21 vs 22. Harness retained at
`scratchpad/reactive-spike/`.

**Context.** Companion to [`RESEARCH-async-reactive-graph-dotnet.md`](RESEARCH-async-reactive-graph-dotnet.md)
(hereafter *R1*) and [`RESEARCH-reactivity-benchmark-plan.md`](RESEARCH-reactivity-benchmark-plan.md)
(*R2*). The plan those two converged on is settled and assumed here: build a **new** reactive graph
library with Solid 2.0's async-aware semantics (the pending channel), **native .NET first**, Fable/JS
as a real secondary target, **no DOM layer**, **separate repo**, competing with Fable.Ripple on
primitives.

**The question.** Solid throws a `NotReadyError` sentinel to suspend a computation that reads an
unresolved source, then re-runs the body from the top when the source settles. JS throws because it has
no cheap continuation capture. Does a .NET implementation have to throw at all?

**Conventions.** Repo-relative links resolve in IDE preview, not on GitHub (submodule paths), per
[`API-COVERAGE-solid2.md`](API-COVERAGE-solid2.md). **Sourced fact and my own speculation are
separated throughout**; speculation is marked ⚠️. R1's correction still applies: the reactive core is
at `solid/packages/signals/`, not `solid-signals/`.

---

## §0. Executive summary

| # | Mechanism | Verdict | One-line reason |
| --- | --- | --- | --- |
| **1** | Thrown sentinel exception (Solid's), + .NET-specific fast path | ✅ **Recommended base** | Only mechanism that preserves transparency through arbitrary call depth; the only one whose cost is bounded by *user reads of pending sources* rather than graph size; survives Fable for free. |
| **2** | Explicit DU `Pending \| Ready \| Failed` in a CE | ✅ **Recommended as opt-in escape hatch** | Sound, allocation-free, Fable-clean — but colours every call site, which is the thing Solid 2 exists to delete. Not the default. |
| **3** | F# resumable code / `ResumableStateMachine` | ❌ **Rejected as the suspension mechanism** | **Proven**: cannot suspend across a non-`inline` call boundary (FS3501), so it *is* mechanism 2 with faster codegen, not a transparent mechanism. **Proven**: does not compile under Fable 5.13. And the resume-vs-re-run win evaporates once you add the glitch-safety check §A shows is mandatory. |
| **4** | `MailboxProcessor` at node level | ❌ **Rejected at node level**, ✅ accepted at graph/owner boundary | **Measured**: 4.9 µs per `PostAndReply` round trip on .NET 10, **9.6 µs on .NET 11 RC** — *worse than throwing on both*. Also destroys synchronous settle. At the boundary, fire-and-forget `Post` is 71.7 ns (.NET 10) / 138.2 ns (.NET 11 RC) and Fable-portable. |
| **5** | Non-throwing sentinel *value* returned from typed reads | ❌ **Rejected as unsound** | There is no spare value in `'T`. Making the read return `Pending<'T>` *is* mechanism 2. |

**Two headline measurements** (new; R1 §8 listed both as unsettled):

1. **Caching the exception object buys nothing on .NET.** Fresh throw 2.98 µs vs cached throw 3.26 µs
   through 10 frames on .NET 10 — fresh is *cheaper*, and the same ordering holds on .NET 9 and 11.
   Unlike V8 (where zeroing `stackTraceLimit` is the win —
   [`core/error.ts:34-39`](../solid/packages/signals/src/core/error.ts#L34)), .NET's cost is two-pass
   SEH unwinding, which caching cannot avoid. **Upstream's specific optimisation does not port.**
2. **Fable 5.13.0 emits seven distinct hard errors** on hand-written resumable code. `task { }` *is*
   supported since Fable 5.5.0 — but by name-matching FSharp.Core's builder in a replacements table,
   not by supporting resumable code. This is decisive for question B.

---

## §1. Mechanism 1 — F# resumable code

### 1.1 The thesis, stated fairly

> A reactive CE built on resumable code lets a computation **suspend and resume** rather than
> throw-and-re-run: no exception cost, no per-step allocation, strictly more powerful than Solid's
> re-run-from-top.

Three sub-claims. The first is true, the second is true, **the third is false**, and the second is
achievable without the first.

### 1.2 Is the machinery public and usable by third parties? — Yes, with a caveat

**Fact.** `FSharp.Core` 10.1.301 exports the whole surface publicly. Verified by reading the assembly:

```
$ grep -aoE "[ -~]{6,}" ~/.nuget/packages/fsharp.core/10.1.301/lib/netstandard2.0/FSharp.Core.dll \
    | grep -iE "resumab|InlineIfLambda|MoveNextMethodImpl"
IResumableStateMachine`1
InlineIfLambdaAttribute
Microsoft.FSharp.Core.CompilerServices.IResumableStateMachine<'Data>.get_Data
Microsoft.FSharp.Core.CompilerServices.IResumableStateMachine<'Data>.get_ResumptionPoint
MoveNextMethodImpl`1
ResumableCode
ResumableCode`2
ResumableStateMachine`1
__resumableEntry
__useResumableCode
```

**Fact.** [FS-1087](https://github.com/fsharp/fslang-design/blob/main/FSharp-6.0/FS-1087-resumable-code.md)
describes it as a general low-level capability, "designed for use only by highly skilled F# developers
to implement low-allocation computation expression builders". It is **not** reserved to `task`.

**Caveat, sourced.** The RFC's *Alternatives* section explicitly preserves the option of withdrawing
external use: a "possible future release" could "only make this feature non-preview within
FSharp.Core, withdraw feature external use." Third-party use is therefore supported-but-not-guaranteed.
`FSharp.Control.TaskSeq` (NuGet 1.1.1) is the well-known production counter-example — it ships a
`taskSeq { }` CE built on resumable code and has done for years.

**Confirmed by experiment.** I built a working resumable-code coroutine builder from scratch in ~35
lines against FSharp.Core 10.1.301 / SDK 11.0.100-rc.1, and it compiled its state machine statically
(no FS3511 "will not be compiled efficiently" warning). **So yes — a reactive CE on resumable code is
actually expressible.** The thesis is not blocked here.

### 1.3 Where it *is* blocked: suspension cannot cross a non-`inline` boundary

This is the decisive finding, and it is not a subtlety — it is a hard compiler error.

**Experiment.** Taking my working coroutine builder and removing `inline` from exactly one combinator
(`Delay`):

```
warning FS3501: Invalid resumable code. Any method or function accepting or returning
                resumable code must be marked 'inline'
```

and removing the `__useResumableCode` guard produces `error FS3402: The construct '__resumableEntry'
may only be used in valid resumable code.`

**What this means for the design.** The RFC's rule is that resumption points must be statically visible
to the compiler *after inlining* — guards are excluded ("The guard expression is not resumable code and
can't contain resumption point"), `with` handlers are excluded, a state machine containing an
"unreduced use `ResumableCode` parameter" is not compilable, and `let rec` forces the dynamic fallback.

Now hold that against R1 §1.2's statement of *why* Solid throws:

> non-local … helper calls deep, through helpers that know nothing about asynchrony. Nothing short of
> an exception provides that.

A reactive graph's transparency claim is precisely this: user code writes

```fsharp
let displayName (u: User) = u.FirstName + " " + u.LastName   // knows nothing about reactivity
let greeting = Memo.create (fun () -> "Hi " + displayName (user ()))
```

and `user ()` suspends *from inside `displayName`'s caller frame*, non-locally. Under resumable code,
`user ()` can only suspend if it is a `let!` syntactically in the CE body, and `displayName` would have
to be `inline` and itself return `ResumableCode`. Every helper in the call graph becomes colour-infected.

> **Conclusion (mine, but forced by FS3501).** Resumable code does not give you transparent suspension.
> It gives you a **faster runtime representation of mechanism 2**. The choice "resumable code vs
> explicit DU" is a codegen choice *within* the monadic design; it is not an alternative to throwing.
> The thesis's word "strictly" is wrong: the two mechanisms are incomparable, not ordered. Solid's
> throw is more powerful on transparency; resumable code is cheaper per suspension.

### 1.4 And the resume-vs-re-run win is smaller than it looks

See §A. Short version: resuming is only sound if nothing read *before* the suspension point changed
while suspended, so you must version-check the prefix at resume time; once you do, the saving is
(re-running the prefix) − (version-checking the prefix), which for a typical 2–5 read memo body is
approximately zero. Measured baseline for scale: a 10-frame plain call chain is **21.7 ns** on .NET 10.

---

## §2. Mechanism 2 — explicit DU `Pending | Ready 'T | Failed of exn`

**Assessment.** Sound, cheap, Fable-clean, and *not* the default.

- Represent as `[<Struct>] Reading<'T> = Ready of value:'T | Pending | Failed of exn` — allocation-free
  on .NET, and `voption`-style struct DUs erase well under Fable (R1 §4.2, Ripple uses `voption`
  throughout for exactly this reason).
- The CE's `Bind` short-circuits on `Pending`, exactly a `Maybe` monad with two failure colours.
- **Cost: colouring.** R1 §3.2 already recorded the objection and I endorse it: a sentinel return does
  not *abort*; a helper three frames down must itself return `Reading<_>` and every caller must
  propagate it. That is the viral monadic colouring Solid 2 exists to delete
  ([`solid/packages/solid/src/index.ts:167`](../solid/packages/solid/src/index.ts#L167) —
  `createResource` removed with the comment `// all computations`).

**Where it earns its place.** Not as the base mechanism, but as an **opt-in escape hatch** for:

1. hot bodies where the user measured the throw and cares;
2. library-internal code (boundaries, `isPending`, `latest`, projection read-traps) — the engine should
   *never* throw at itself, it should use a non-throwing internal `TryRead`;
3. users who genuinely want to handle pending explicitly rather than delegate to a boundary.

⚠️ **Speculation.** I expect fewer than 5% of user bodies will want this, and that offering it early
will cause people to reach for it by default and lose the transparency benefit. Ship it late.

---

## §3. Mechanism 3 — MailboxProcessor

The prompt asks two separate questions: is the perf claim real, and is the role at the **graph/owner
boundary** different from the role at the **node** level? Yes to both, and they answer oppositely.

### 3.1 Measured numbers (new)

> **Re-measured on .NET 10 and 11.** The original figures in this section were `net9.0` only and §D.2
> flagged them as needing confirmation. They have been re-run on `net10.0` (runtime **10.0.12**) and
> `net11.0` (runtime **11.0.0-rc.1**); the .NET 9 column is retained for comparison but is
> **superseded**. Harness: `scratchpad/reactive-spike/ThrowCost.fs`.

Stopwatch harness, AMD Ryzen 9 9900X, Release, workstation GC, **no debugger attached**,
`[<MethodImpl(NoInlining)>]` on the recursion so the frames are real, 200-iteration warmup and a forced
GC before each timed run.

| Operation | .NET 10 ns | .NET 11 ns | B/op | .NET 9 (superseded) |
| --- | ---: | ---: | ---: | ---: |
| plain call, 10 frames (baseline) | **21.7** | 21.4 | 0 | 25.5 |
| **`try`/`catch` entry only, no throw** | **25.8** | 26.0 | 0 | *not measured* |
| sentinel return + flag, 10 frames | **41.0** | 40.3 | **0** | 58.6 – 60.7 |
| throw **cached** exception, 1 frame | 1 290 | 1 263 | 344 | 1 510 – 1 653 |
| throw **cached** exception, 10 frames | **3 257** | 3 349 | 1 192 | 4 221 – 5 389 |
| throw **fresh** exception, 10 frames | **2 978** | 3 162 | 1 312 | 4 174 – 4 860 |
| throw cached exception, 40 frames | 8 266 | 8 685 | 4 344 | 11 981 |
| **`MailboxProcessor.PostAndReply`** | **4 869** | **9 604** | 327 | 5 597 |
| `MailboxProcessor.Post` (fire-and-forget) | **71.7** | **138.2** | 49 | 32.5 |

**Five readings, all load-bearing:**

1. **Caching the exception buys nothing — now confirmed across three runtimes.** Fresh is *cheaper*
   than cached on .NET 9, 10 and 11 alike (2 978 vs 3 257 on .NET 10). Upstream's V8 `stackTraceLimit`
   trick ([`error.ts:34-39`](../solid/packages/signals/src/core/error.ts#L34) — see R1 §1.2)
   **does not have a .NET analogue**. R1 §7 step 1 proposed distinguishing (i) fresh from
   (ii) cached-no-stack-capture as the architecture-deciding measurement; the answer is **they are the
   same number**, so that fork does not exist. Do not build the caching machinery.
2. **Throw cost is linear in frame depth**, ≈220 ns/frame on .NET 10 (1.3 µs @1 → 3.3 µs @10 →
   8.3 µs @40). This mirrors upstream's note that V8's eager stack capture is "proportional stack depth
   — real cost under SSR". Deep component trees are where it bites on both runtimes.
3. **Throws got ~30% cheaper on .NET 10/11** (4 221–5 389 → 3 257). The conclusion is unchanged: still
   microseconds, still ~150× a plain call and ~80× a sentinel return.
4. **`try`/`catch` *entry* costs 4 ns over a plain call** — 25.8 vs 21.7, and zero allocation. This is
   the measured foundation for §4.1's tolerant read: **wrapping is free, only throwing is expensive.**
   The premise holds.
5. ⚠️ **`MailboxProcessor` regressed sharply on the .NET 11 RC** — `PostAndReply` nearly doubled
   (4 869 → 9 604) and `Post` doubled (71.7 → 138.2). It is a release candidate and may not ship this
   way, but it reinforces §3.2: keep actors off the hot path entirely. Note also that `Post` is slower
   on .NET 10 than the .NET 9 figure this section originally reported (71.7 vs 32.5), so §3.3's "that is
   free" claim has ~2× less headroom than first stated — still fine at one message per flush cycle.

### 3.2 Node level — rejected on the numbers *and* on the semantics

`PostAndReply` at **4.9 µs** (.NET 10) / **9.6 µs** (.NET 11 RC) is *slower than throwing*. A graph read
is single-digit nanoseconds; this is a ~1000× tax on the hottest path in the library, and the .NET 11 RC
moved it in the wrong direction. That alone ends it.

The semantic objection is independent and worse: a message-passing node cannot settle synchronously.
R2 §6.4 records Ripple's headline guarantee — *"Synchronous write settles before the next line runs. No
scheduler, no tick"* — substantiated at `Internal/Scheduler.fs`. If we are competing with Ripple on
primitives (R2 §7.3), handing back synchronous settle to adopt a mechanism that is also 1000× slower is
not a trade, it is a forfeit.

### 3.3 Graph / owner boundary — accepted, and it is the right reading of Clef

`Post` fire-and-forget is **71.7 ns / 49 B** on .NET 10 (138.2 ns on the .NET 11 RC). At one message per
*flush cycle* or per *cross-thread marshal* — not per node — that is still cheap, though with ~2× less
headroom than the original .NET 9 measurement of 32.5 ns suggested.

This is the role R1 §3.1 already identified without naming the primitive: "non-overlapping thread
affinity, pumped on pool … Exactly SignalsDotnet's `SignalIsland<T>`; also where Clef's actor/region
scoping lands." Clef's normative rule 7 (actor/region-scoped lifetime, deterministic disposal, R1 §2.3)
is an **ownership** statement, not a node-update statement. R1 §3.1 also flagged that a pending node
must survive losing its last subscriber ([`async.ts:138`](../solid/packages/signals/src/core/async.ts#L138),
[`:459-466`](../solid/packages/signals/src/core/async.ts#L459)) and that an in-flight `Task` holds a
strong reference — so deterministic teardown is non-negotiable, and an actor-shaped owner is a good
place to put it.

**Recommendation.** Use a mailbox (or, ⚠️ my preference, a `System.Threading.Channels` bounded channel
— same shape, lower allocation, though I did not benchmark it) **at the graph boundary only**: one
inbox per *graph context*, carrying (a) settle continuations arriving from `Task` completions on pool
threads, and (b) external writes from other threads. Node-to-node propagation stays direct calls.

**Fable note.** `fable-library-js` ships `MailboxProcessor.js` (verified in
`~/.nuget/packages/fable/5.13.0/fable-library-js/`), so a mailbox at the boundary is portable. This is
one of the few places where the native-vs-JS asymmetry costs nothing.

---

## §4. Mechanism 4 — a non-throwing sentinel value returned from reads

**Can it preserve transparency given that user code writes `user().Name` directly? No.**

The argument is short and, I believe, airtight:

- The transparent read has signature `Signal<'T>.Value : 'T`. To return "pending" you need a value of
  type `'T` that is distinguishable from every legitimate `'T`.
- For unconstrained `'T` there is no such value. `Unchecked.defaultof<'T>` is a legitimate value of
  every type (`0`, `false`, `null`, a zeroed struct).
- Constraining to `'T : not struct` and returning a private sentinel instance makes `user().Name` a
  `NullReferenceException` or a read of a fake object — an *accidental* exception, i.e. you have
  re-invented mechanism 1 with worse diagnostics and no `_pendingSources` bookkeeping. (Note R2 §6.2:
  Ripple already hit the `not struct` constraint wall on `Signal.referenceEquals`, which is why the
  benchmark plan needs `[<Emit("$0 === $1")>]`. Same wall.)
- Changing the signature to `Signal<'T>.Value : Reading<'T>` **is mechanism 2.**

So mechanism 4 is not a distinct viable option. It is either unsound or it is mechanism 2 wearing a hat.

### 4.1 ⚠️ But there *is* a genuinely .NET-flavoured variant worth naming — "tolerant read"

**This is speculation. I found no prior art for it and did not implement it.**

The prompt's premise — ".NET may have better primitives than JS" — turns out to be true, but the better
primitive is not continuation capture. It is that **`try`/`catch` entry is free on .NET when nothing
throws**, whereas *throwing* is expensive. That asymmetry suggests:

1. The engine wraps every computation body in `try ... with`.
2. A read of a pending source **does not throw**. It records `source` into the consumer's
   `PendingSources` set, sets a per-computation `SawPending` flag, and returns
   `Unchecked.defaultof<'T>`.
3. The body runs on to completion with a garbage value in hand.
4. When the body returns, the engine checks `SawPending`; if set, it **discards the result** and marks
   the node `STATUS_PENDING` — exactly as if it had aborted.
5. If the body instead blows up on the garbage (`NullReferenceException` from `user().Name`), the
   surrounding `try ... with` catches it, sees `SawPending`, and treats it as a pending abort rather
   than an error.

Observable semantics are identical to throwing, because the discarded result is never published. Cost
in the common case — `s() + a()`, `if flag() then x else y`, anything arithmetic or structural that
tolerates a zero — drops from ~4.5 µs to roughly the 58 ns sentinel-return row.

**Why I do not recommend making this the base, only a construction-time policy flag:**

- **Side effects run with garbage.** Solid's re-run model already discourages side effects in bodies,
  and R1 §1.2 notes bodies are expected to survive re-runs; but "runs twice" and "runs once with
  nonsense inputs" are different hazards.
- **Non-termination.** `while user().HasNext do ...` against a zeroed struct could loop forever. There
  is no `try/catch` for that.
- **Diagnostics.** A user debugging an NRE deep in their own helper will see a confusing stack.

⚠️ A bounded variant: tolerate the *first* pending read, then have subsequent reads throw once
`SawPending` is already set. That caps damage to one "garbage-carrying" segment while still eliding the
throw in the overwhelmingly common single-pending-source case. I have not thought this through far
enough to recommend it; it is the second-most interesting thing in this document after §A.

---

## §5. Ranking and recommendation

### 5.1 Ranking

1. **Thrown sentinel exception** — base mechanism, non-negotiable for transparency.
2. **Explicit DU / monadic CE** — opt-in escape hatch; also the engine's *internal* read path.
3. **Resumable code** — not a suspension mechanism. Optional future codegen for #2's `Bind`, native
   backend only. Do not build on it.
4. **MailboxProcessor / Channels** — wrong at node level, right at the graph/owner boundary.
5. **Non-throwing sentinel value in `'T`** — unsound; discard the idea.

### 5.2 Why throwing still wins despite costing 4–5 µs

The number sounds alarming until you ask **how many throws there actually are**. The throw count is
bounded not by graph size but by *transparent user reads of pending sources*:

- The engine's own traversals — `recompute` pulling deps, a boundary sampling `_statusFlags`,
  `isPending` / `latest` ambient read modes
  ([`core.ts:125-126`](../solid/packages/signals/src/core/core.ts#L125),
  [`:941`](../solid/packages/signals/src/core/core.ts#L941)) — **never need to throw**. They use the
  internal `TryRead : Reading<'T>` path (mechanism 2). Solid conflates these because JS throws are
  cheap; we should not.
- A consumer throws **at most once per body run** — the first pending read aborts it.
- A body re-runs once per invalidation cycle, not per read.

⚠️ **Estimate, mine.** A page-scale graph with 50 suspended consumers over ~4 settle cycles ≈ 200
throws ≈ **1 ms total**. That is acceptable. A 5 000-node SSR render where most nodes suspend is
~25 ms, which is not, and is exactly where the §4.1 tolerant-read policy would earn its keep. This is
an estimate, not a measurement.

### 5.3 Concrete F# API sketch

The shape below assumes R1 §7's conclusion: API parity by discipline with
[`SolidBindings.fs`](../Partas.Solid/SolidBindings.fs), so the same user code targets native .NET here
and real Solid via the existing erased bindings on the browser.

```fsharp
namespace Reactive

/// The pending channel, second flag axis — orthogonal to dirtiness.
/// Mirrors solid/packages/signals/src/core/constants.ts:65-68.
[<Flags>]
type Status =
    | None          = 0
    | Pending       = 1
    | Error         = 2
    | Uninitialized = 4

/// Non-throwing read result. This is mechanism 2, and it is the ENGINE's
/// internal read path as well as the user's opt-in escape hatch.
[<Struct; NoComparison>]
type Reading<'T> =
    | Ready   of value: 'T
    | Pending
    | Failed  of error: exn

/// Thrown by transparent reads. Carries the source so the consumer can
/// populate PendingSources. NOT cached: caching saves nothing on .NET (§3.1).
exception NotReadyException of source: INode

type Signal<'T> =
    /// TRANSPARENT read. Tracks, and throws NotReadyException if pending.
    /// This is the 99% path; `user().Name` just works.
    member Value : 'T with get, set
    /// NON-THROWING read. Tracks (links the edge) exactly as Value does,
    /// so a Pending result still wakes the consumer when the source settles.
    member TryValue : Reading<'T>
    /// Untracked.
    member Peek : 'T

[<RequireQualifiedAccess>]
module Memo =
    /// Synchronous body. May read pending sources transparently.
    val create      : compute: (unit -> 'T) -> Memo<'T>
    /// Async body: reads happen SYNCHRONOUSLY before the Task is returned;
    /// the engine owns the awaiting. Mirrors R1 §3.3. Task, not ValueTask
    /// (ValueTask cannot be awaited twice and the engine re-consults flights).
    val createAsync : compute: (CancellationToken -> Task<'T>) -> Memo<'T>
    /// IAsyncEnumerable form — Solid's AsyncIterable branch
    /// (solid/packages/signals/src/core/async.ts:246).
    val createStream : compute: (CancellationToken -> IAsyncEnumerable<'T>) -> Memo<'T>

[<RequireQualifiedAccess>]
module Boundary =
    /// ONE status-parameterised function, exactly boundaries.ts:469-475.
    /// Boundaries are graph constructs; they come free with no UI layer (R1 §1.4).
    val create : status: Status -> body: (unit -> 'T) -> fallback: (unit -> 'T) -> Memo<'T>

/// Construction-time policy. Target-divergent concerns are injected here,
/// NOT behind #if FABLE_COMPILER in the core (R1 §4.2).
type GraphOptions =
    { /// JS build passes reference equality; native may want structural.
      Equality      : IEqualityComparer<obj>
      /// §4.1. Default Throw. Tolerant is opt-in and documented as unsafe
      /// for side-effecting or potentially-non-terminating bodies.
      SuspendPolicy : SuspendPolicy      // Throw | TolerantRead
      /// Superseded in-flight requests.
      FlightPolicy  : FlightPolicy       // CancelPrevious | KeepLatest | Queue
      /// Where settle continuations are marshalled back to. Mailbox/Channel
      /// lives HERE, at the boundary — never at node level (§3.3).
      Dispatcher    : IGraphDispatcher }

/// Opt-in suspension-aware CE for bodies that want explicit control.
/// `let! x = signal` yields 'T and short-circuits the whole body to Pending.
type ReactiveBuilder =
    member Bind   : Reading<'T> * ('T -> Reading<'U>) -> Reading<'U>
    member Return : 'T -> Reading<'T>
    member Run    : Reading<'T> -> 'T   // engine unwraps; Pending aborts the body
```

Two non-obvious invariants the sketch encodes, both from R1 §1.3, both **mandatory**:

- **`TryValue` must link the edge before returning `Pending`**, exactly as `Value` links before
  throwing. Upstream's stranded-forever-pending bug (#2893,
  [`async.ts:41-44`](../solid/packages/signals/src/core/async.ts#L41)) is what happens otherwise.
- **`PendingSources` is a `HashSet<INode>` from the start**, never a promoted single slot. Multiple
  flights must each be tracked.

---

## §A. Dynamic dependency re-tracking under resumption

**This is the deepest technical risk in the plan, and the answer is: naive resumption breaks
glitch-freedom. It is fixable, and the fix removes most of resumption's value.**

### A.1 What Solid actually relies on

Three cooperating facts (R1 §1.2), all sourced:

1. The dep list is **rebuilt from scratch each run** — `recompute` resets `_depsTail` and sets
   `REACTIVE_RECOMPUTING_DEPS` ([`core.ts:239-252`](../solid/packages/signals/src/core/core.ts#L239)).
2. On an aborted run, the **previous run's edges are retained** — the reconciliation never completes,
   so nothing is unlinked. R1 §1.3 step 1: "`c`'s body aborts mid-way, retaining **both** edges."
3. Re-adding an already-present pending source is a no-op via the `has` guard
   ([`async.ts:45-49`](../solid/packages/signals/src/core/async.ts#L45),
   [`:70`](../solid/packages/signals/src/core/async.ts#L70)).

So a suspended Solid node holds a **conservative superset**: prefix deps of the aborted run ∪ all deps
of the last completed run. Conservative = it may wake unnecessarily, never fails to wake.

### A.2 The dependency *set* is fine under resumption. The dependency *values* are not.

First, dispose of a false worry. Under resumption the suffix deps **are** still collected — the resumed
continuation really does execute the rest of the body, inside the tracking scope. So after a completed
resume the set is (prefix from part 1) ∪ (suffix from part 2), which is complete and correct. The dep
set is not the problem.

The problem is that **the prefix was read at T₀ and the suffix executes at T₁**, and the graph's
central guarantee is that every node's value is consistent with a single instant.

### A.3 Worked example — the diamond, by hand

```
s : Signal<int>                                   (synchronous)
a : Memo<int>   = Memo.createAsync (fun ct -> fetch (s.Value, ct))    -- depends on s
c : Memo<int>   = Memo.create      (fun ()  -> s.Value + a.Value)     -- diamond join
```

Edges: `s → a → c` and `s → c`. Suppose `fetch x` eventually yields `10 * x`.

**Under Solid (re-run from top):**

| t | event | `c` |
| --- | --- | --- |
| T₀ | `s = 1`; `c` runs: reads `s`→1, reads `a`→pending, **throws**. Edges `{s, a}` retained. | pending, no value |
| T₁ | flight for `s=1` lands, `a = 10`. `c` **re-runs from top**: `s`→1, `a`→10. | **11** ✅ |
| T₂ | write `s = 2`. `a` invalidated, new flight starts, `a` pending. `c` dirty → re-runs: `s`→2, `a`→pending, throws. | pending, **no value published** ✅ |
| T₃ | flight for `s=2` lands, `a = 20`. `c` re-runs: `s`→2, `a`→20. | **22** ✅ |

`c` is never observed holding a value inconsistent with a single `s`.

**Under naive resumption:**

| t | event | `c` |
| --- | --- | --- |
| T₀ | `c` runs: reads `s`→1, **captures 1 into the state machine's field**, suspends at `a`. | pending |
| T₂′ | write `s = 2`. `c` is dirty on `s` — but `c` is suspended mid-body. | pending |
| T₃′ | flight for `s=2` lands, `a = 20`. **Resume the continuation**: it adds the *captured* `1` to `20`. | **21** ❌ |

**`c = 21` corresponds to no state the graph was ever in.** `s = 1` never coexisted with `a = 20`. This
is a textbook glitch — precisely the failure that node-height scheduling exists to prevent (R1 §2,
Clef's normative rule 6: *"Stabilization SHALL process nodes in ascending height order"*), reintroduced
through a side door. And note R1 §7 step 3 assertion 4 already named this scenario as "the assertion
that distinguishes a real pending channel from a boolean flag" — naive resumption **fails it**.

Worse, this is not a rare race. It is the *normal* shape of a data-dependent fetch: a selector signal
feeding both the request and the rendering of the response. Anyone who writes
`memo (fun () -> $"User {selectedId()}: {userData().Name}")` hits it on the second selection.

The conditional form is the same bug with a different symptom:

```fsharp
Memo.create (fun () -> if isAdmin.Value then adminData.Value else publicData.Value)
```

T₀: `isAdmin`→true, suspend at `adminData`. T₁: write `isAdmin = false`. T₂: `adminData` settles →
resume → publishes admin data to a non-admin. The dep set even ends up containing `adminData` and *not*
`publicData`, so the node is now wired to the wrong branch until something forces a full re-run. Under
Solid the T₁ write marks `c` dirty, `c` re-runs from the top, reads `isAdmin`→false, reads
`publicData`, and the edge on `adminData` is dropped by reconciliation.

### A.4 The worked example, executed — the bug reproduces exactly

**The §A.3 diamond has now been run as code**, not just reasoned about. Harness:
`scratchpad/reactive-spike/{Diamond.fs,Program.fs}`, `net10.0` and `net11.0`, both runtimes agreeing.
The computation body is split into an explicit prefix (reads `s`) and suffix (reads `a`) — which is
precisely the shape a resumable state machine produces — and the three completion policies are run
against it.

**Case 1 — `s` written while `a` is in flight:**

| Policy | `c` | prefix executions |
| --- | ---: | ---: |
| `ReRun` (Solid's behaviour) | **22** | 2 |
| `NaiveResume` (resumable state machine) | **21** | 1 |
| `VersionedResume` | **22** | 2 |

`a` is requested at `s = 1` but resolves late carrying 20, the value for the `s = 2` written mid-flight.
The only states the graph is ever in are `(s=1, a=10) ⇒ c=11` and `(s=2, a=20) ⇒ c=22`. **21 is neither
— it is `s`-old + `a`-new.** Confirmed: the glitch is not theoretical.

**Case 2 — control, nothing written while `a` is in flight:** all three policies yield 21, and
`VersionedResume` records **1** prefix execution. This is the load-bearing control: it proves the
version check *genuinely resumes* when nothing changed, rather than silently degrading into
always-re-run and passing Case 1 for the wrong reason.

8/8 assertions pass on both runtimes. §A.6's remaining divergence (side-effect counts) is unaffected by
this result and still stands.

### A.5 The fix, and why it eats the benefit

**Resumption is sound iff every dependency read before the suspension point still holds the value it
held when it was read.** So:

1. Give every node a monotonically increasing `Version` (bumped on value change, cutoff-aware).
2. When suspending, record `(dep, versionAtRead)` for each prefix dep.
3. At settle, **validate**: if all prefix versions match, resume; if any differ, **discard the
   continuation and re-run from the top**.

This is correct, and it is a strict *optimisation* of Solid's semantics that degenerates to Solid's
behaviour exactly in the cases where the two differ. Good. But now price it:

- Validation is O(prefix) version compares, and in the fast path you *also* pay state-machine setup,
  field writes, and a `__resumeAt` switch on every suspension.
- Re-running the prefix is O(prefix) signal reads. **A signal read is a few nanoseconds; a version
  compare is a few nanoseconds.** For the 2–5 read bodies that dominate real graphs, the difference is
  in the noise — recall the measured baseline of 21.7 ns for a *ten-frame call chain*.
- Resumption only wins when the prefix contains expensive **pure** computation *and* nothing upstream
  changed. ⚠️ My estimate: a small single-digit percentage of bodies.

### A.6 Where the divergence from Solid remains, and whether it matters

Even with the validity check, one observable difference survives: **Solid re-executes prefix side
effects on every settle cycle; resumption executes them once.** Resumption's behaviour is arguably
*nicer*. But the settled plan (R1 §7) is API-and-semantics parity across two implementations — native
here, real Solid in the browser via [`SolidBindings.fs`](../Partas.Solid/SolidBindings.fs). A user whose
body logs, or writes to a store untracked, would see different output on the two backends. That is a
parity bug even though it is a semantics improvement.

**Verdict on question A.** Resumption is correct only with a prefix-validity check; without it, it
breaks glitch-freedom in the most common async shape there is. With the check it is correct but nearly
worthless for realistic body sizes, and still leaves a side-effect-count divergence from Solid.
**Combined with §1.3 (it cannot be transparent anyway) and §B (it cannot compile under Fable), this
closes mechanism 1.**

---

## §B. Does the chosen mechanism survive Fable?

### B.1 Does Fable 5.13 support F# resumable code? — **No. Proven by experiment.**

I wrote a minimal resumable-code coroutine builder (a `CoroBuilder` with `Delay`/`Zero`/`Combine`/
`Yield`/`Run`, using `ResumableCode.Yield()`, `__useResumableCode`, `__resumeAt`, `__stateMachine`,
`MoveNextMethodImpl`, `SetStateMachineMethodImpl`, `AfterCode`), confirmed it builds and statically
compiles its state machine on .NET, then ran `dotnet fable ... --lang js --noCache` with **Fable
5.13.0** (the version pinned in this repo's `.config/dotnet-tools.json`). Result — **seven distinct
hard errors, compilation failed:**

```
warning FABLE: Fable only supports a subset of standard .NET API ...
error FABLE: Microsoft.FSharp.Core.CompilerServices.ResumableCode.Yield (static) is not supported by Fable
error FABLE: Microsoft.FSharp.Core.CompilerServices.ResumableCode`2.Invoke is not supported by Fable
error FABLE: Microsoft.FSharp.Core.CompilerServices.ResumableCode.Delay (static) is not supported by Fable
error FABLE: Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers.__useResumableCode (static) is not supported by Fable
error FABLE: Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers.__resumeAt (static) is not supported by Fable
error FABLE: Microsoft.FSharp.Core.CompilerServices.ResumableCode`2.Invoke is not supported by Fable
error FABLE: Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers.__stateMachine (static) is not supported by Fable
Compilation failed
```

The partial JS Fable emitted before failing is itself diagnostic — `__useResumableCode` degenerated to
`null`, so the state-machine branch vanished and only the dynamic fallback constant survived:

```js
export function test() {
    return (null ? null : -1) | 0;
}
```

**This is not a missing-package problem.** For comparison I also tried the real third-party resumable
library, `FSharp.Control.TaskSeq` 1.1.1 — it fails earlier and for a different reason
(`error EXCEPTION: Cannot find inline member: FSharp.Control.TaskSeqBuilder__Yield_1505`, i.e. the
package ships no `fable/` sources), so it does not prove anything about resumable code. The in-project
experiment above does.

Corroborating negative evidence: grepping Fable 5.13.0's own assemblies for any resumable-code handling
finds none. `Fable.Transforms.dll` yields only the string `MoveNext`; `Fable.AST.dll` yields nothing.

### B.2 Does Fable support `task { }`? — **Yes since 5.5.0, but by special-casing, not by supporting resumable code.** The prior was right in substance, out of date in detail.

`~/.nuget/packages/fable/5.13.0/content/CHANGELOG.md`, release **5.5.0 (2026-06-30)**:

> `*(js/ts)* Map task { } to Promise<T>` ([`97f54d36`](https://github.com/fable-compiler/Fable/commit/97f54d3692e5ba881c249b289c20b4fad5ac27e2))

Reading that commit: it adds ~78 lines to `src/Fable.Transforms/Replacements.fs` plus two new runtime
files, `src/fable-library-ts/Task.ts` and `src/fable-library-ts/TaskBuilder.ts` (both present in the
shipped 5.13.0 package as `fable-library-js/Task.js` and `TaskBuilder.js` — verified). The mechanism is
a **name-based entry in the `replacedModules` table**:

```
"Microsoft.FSharp.Control.TaskBuilder",                       taskBuilder
"Microsoft.FSharp.Control.TaskBuilderBase",                   taskBuilder
"Microsoft.FSharp.Control.TaskBuilderModule",                 taskBuilder
"Microsoft.FSharp.Control.TaskBuilderExtensions.HighPriority", taskBuilder
"Microsoft.FSharp.Control.TaskBuilderExtensions.LowPriority",  taskBuilder
```

Those exact names are observable as string literals inside the shipped `Fable.Transforms.dll`
(`taskBuilder`, `taskBuilderB`, `taskBuilderHP`, `taskBuilderM`, `taskBuilderModule`,
`Microsoft.FSharp.Control.TaskBuilderExtensions`). FSharp.Core's builder-method calls are intercepted
by fully-qualified name and redirected to a hand-written TypeScript `TaskBuilder` class whose `Bind` is
`computation.then(binder)`. The state-machine intrinsics are never reached because the builder members
are replaced *before* inlining exposes them.

**Consequence, and it is the whole answer to B.** "Fable supports `task`" is a statement about one
hard-coded builder in FSharp.Core. It confers nothing on a third-party resumable CE, which gets no
table entry and therefore hits the raw intrinsics — exactly as §B.1 demonstrates.

⚠️ Could we get our own table entry upstreamed? In principle yes — it is the same trick. But it would
mean Fable shipping a hand-written JS implementation of our reactive builder, i.e. maintaining the JS
half *in TypeScript inside the Fable repo*. That is strictly worse than writing the JS half ourselves.

### B.3 Would "implement the mechanism twice behind one API" be acceptable, or fatal?

**Fatal — but only for mechanism 1. For the recommended design the question does not arise.**

Distinguish two kinds of twice-implemented:

| | Native | Fable/JS | Same observable semantics? | Same user-facing API? |
| --- | --- | --- | --- | --- |
| **Recommended (throw)** | `raise (NotReadyException src)` | `raise` → JS `throw` | ✅ yes | ✅ identical |
| **Mechanism 2 (DU)** | struct DU | struct DU, erases fine | ✅ yes | ✅ identical |
| **Mechanism 1 (resumable)** | state machine | **cannot compile** → must fall back to throw | ⚠️ no (§A.6 side-effect count) | ❌ **no — resumable requires `let!` colouring, throw does not** |

The killer for mechanism 1 is the last column. A "throw-based JS / resumable-state-machine native"
split behind one API is not a divergence in *internals*; it is a divergence in **what user code has to
look like**. Resumable code needs the read to be `let! u = user` inside the CE; throw-based lets it be
`user().Name` inside an arbitrary helper. You cannot hide that behind a shared signature, because the
native backend rejects the transparent form (FS3501 on the helper) while the JS backend accepts it.
Users would write code that compiles on one target and not the other. That is fatal.

R1 §4.2 set the bar as "zero or near-zero `#if FABLE_COMPILER` in the graph core, with target-divergent
policy injected at construction." Throw-based suspension clears that bar trivially: the `SuspendPolicy`
of §5.3 is a construction-time value, and the JS build simply always picks `Throw`.

### B.4 One more consideration the plan should absorb

R1 §7 already concluded that the browser target should keep *erasing to real Solid* rather than running
our graph through Fable. If that holds, question B is largely moot for the graph itself — Fable only
has to compile the *bindings*, which it already does. ⚠️ My reading: keep Fable-compilability of the
core as a **design constraint and a CI check**, not as a shipped artefact. It costs nothing (it is just
"don't use exotic FSharp.Core intrinsics") and it preserves the option. Adopting resumable code would
throw that option away permanently.

---

## §C. The smallest experiment that de-risks the recommendation

R1 §7 proposed a ~350–400 line spike in three steps. **Step 1 is now done** (§3.1) and **the Fable
question is now done** (§B.1), so the remaining spike shrinks considerably.

### C.1 What is already settled, so do not re-do it

- **Exception cost.** Measured, and **re-measured on .NET 10.0.12 and 11.0.0-rc.1** (§3.1). Throws are
  ~30% cheaper than the original .NET 9 figures — **3.3 µs** through 10 frames, not 4–5. The relative
  conclusions are unchanged and confirmed across three runtimes: throw ≈ 80× sentinel and ~150× a plain
  call; caching buys nothing; cost is linear in depth. ⚠️ Residual caveat: Stopwatch, not
  BenchmarkDotNet; single machine (AMD Ryzen 9 9900X).
- **Fable + resumable code.** Settled negative (§B.1). Reproducible in ~10 minutes.

### C.2 The one experiment that still matters — ~80 lines, one sitting

**Assert the diamond, against a deliberately naive resumption, and watch it fail.** This is the
cheapest way to convince yourself (and anyone who revives the resumable-code thesis) that §A is real
rather than theoretical.

Build the smallest possible graph — a `Signal`, a `Memo`, a `Memo.createAsync` backed by a manually
completed `TaskCompletionSource`, and a version counter per node — then write **three** tests:

1. **Solid-equivalent baseline (throw + re-run).** Exactly R1 §7 step 3 assertion 4: `a` async-pending,
   `b` sync-settled, `c = f(a, b)`; assert `c` pending; write `b` alone and assert `c` is *still*
   pending and has produced **no** value; settle `a` and assert `c` completes exactly once. This is the
   assertion no library in R1 §2 can pass.
2. **§A.3's diamond under naive resumption.** `s → a → c` and `s → c`. Suspend `c` at `a` with `s = 1`,
   write `s = 2` while the flight is in air, settle `a = 20`, resume. **Assert the observed value is
   21 — i.e. assert the bug.** Then add the version check and assert it becomes 22. A test that pins
   the wrong answer is the clearest possible documentation of why the check is mandatory.
3. **Prefix-validity check cost.** Time "re-run a 4-read prefix" against "version-check a 4-read
   prefix" and confirm they are within noise of each other. If they are, §A.5's conclusion holds and
   resumption is closed for good. If re-running is meaningfully more expensive than validating, reopen
   mechanism 1 as an *internal* optimisation of mechanism 2's `Bind` — native only, never user-visible.

**Stop condition.** If test 2 reproduces the 21 and test 3 shows parity, mechanism 1 is dead and the
recommendation in §5 stands with no further work. Expected effort: an afternoon.

### C.3 Optional fourth, if §4.1 tempts you

Prototype the tolerant read on the same toy graph, and run the §C.2 test 1 assertions against it. The
question to answer is narrow: **does discarding a garbage-carrying body produce observably identical
results to aborting it?** If yes, you have a 70× fast path for the common case behind a policy flag. If
you find a case where it does not, write it down and drop the idea.

---

## §D. What I could not settle

1. **Whether `System.Threading.Channels` beats `MailboxProcessor` at the boundary.** I measured
   `MailboxProcessor` only. My preference for `Channels` in §3.3 is ⚠️ speculation based on its
   allocation profile, not measurement. The 71.7 ns `Post` figure (.NET 10) is still fast enough that
   this may not matter — though the .NET 11 RC's 138.2 ns halves that margin.
2. ~~**Whether .NET 10/11 changed exception cost.**~~ **Settled** — re-measured on .NET 10.0.12 and
   11.0.0-rc.1 (§3.1). Throws are ~30% cheaper than on .NET 9; every relative conclusion survives. The
   one surprise was a `MailboxProcessor` regression on the .NET 11 RC (§3.1 reading 5).
3. **Whether the §4.1 tolerant-read idea has prior art.** I searched and found none, but "reactive
   library computes with a poison value and discards the result" is an awkward thing to search for. I
   would not claim novelty.
4. **Whether Fable would accept an upstream replacements-table entry for a third-party reactive
   builder.** I did not ask the Fable maintainers and found no precedent for a non-FSharp.Core builder
   getting one. §B.2's dismissal is reasoning, not a sourced refusal.
5. **Exact allocation attribution for the 1 192 B/op on a cached throw.** The exception object is
   cached, so that allocation is CLR-internal (stack-trace capture / `ExceptionDispatchInfo`). I did
   not profile it. It matters only if someone wants to argue caching *should* have helped.
6. **Whether Solid's `core/verdict.ts` (593 lines) or `core/attribution.ts` (818 lines) contain
   pending-channel behaviour that changes any of the above.** Still unread, as R1 §8 items 2–3 noted.
7. **Whether my coroutine builder's state machine is semantically *correct***, as opposed to merely
   compiling statically. I checked for the absence of FS3511 but did not execute it. This only affects
   §1.2's "it is expressible" claim, which the FS3501/FS3402 findings make moot anyway.
8. **Real-world throw counts.** §5.2's "50 suspended consumers × 4 cycles ≈ 1 ms" is ⚠️ an estimate
   with no measurement behind it. It is the number that decides whether §4.1 is needed.

---

## §E. Sources

**Experiments run for this document** (scratch dir, outside the repo; not committed):

- Resumable-code coroutine builder, FSharp.Core 10.1.301, SDK 11.0.100-rc.1 — compiled on .NET;
  diagnostics FS3402, FS3501, FS3513 reproduced; `dotnet fable 5.13.0 --lang js --noCache` produced the
  seven `error FABLE:` lines quoted in §B.1.
- `FSharp.Control.TaskSeq` 1.1.1 under Fable 5.13.0 → `Cannot find inline member:
  FSharp.Control.TaskSeqBuilder__Yield_1505`.
- Stopwatch micro-benchmark, originally `net9.0` / runtime .NET 9.0.20; **re-run on `net10.0`
  (runtime 10.0.12) and `net11.0` (runtime 11.0.0-rc.1)**, Release, AMD Ryzen 9 9900X, no debugger,
  `[<MethodImpl(NoInlining)>]` recursion, 200-iteration warmup and forced GC per row — table in §3.1.
  Harness retained at `scratchpad/reactive-spike/ThrowCost.fs`.
- **Diamond glitch test** (`scratchpad/reactive-spike/Diamond.fs`, `Program.fs`), `net10.0` and
  `net11.0` — the §A.3 worked example executed as code; 8/8 assertions pass on both. See §A.4.

**Read locally:**

- `~/.nuget/packages/fsharp.core/10.1.301/lib/netstandard2.0/FSharp.Core.dll` — resumable-code public
  surface (§1.2).
- `~/.nuget/packages/fable/5.13.0/content/CHANGELOG.md` — "5.5.0 — `*(js/ts)* Map task { } to
  Promise<T>`"; "5.6.0 — Split Fable.Transforms into per-target projects".
- `~/.nuget/packages/fable/5.13.0/tools/net10.0/any/{Fable.Transforms,Fable.AST,Fable.Compiler}.dll` —
  presence of `taskBuilder*` / `Microsoft.FSharp.Control.TaskBuilderExtensions` literals, absence of
  any resumable-code handling.
- `~/.nuget/packages/fable/5.13.0/fable-library-js/{Task.js,TaskBuilder.js,MailboxProcessor.js}`.

**External:**

- <https://github.com/fsharp/fslang-design/blob/main/FSharp-6.0/FS-1087-resumable-code.md> — resumable
  code RFC: intended audience, `__resumableEntry` / `__resumeAt` / `__stateMachine` /
  `MoveNextMethodImpl` semantics, non-compilable cases, `__useResumableCode` dynamic fallback, the
  reserved option to withdraw external use.
- <https://github.com/fable-compiler/Fable/commit/97f54d3692e5ba881c249b289c20b4fad5ac27e2> —
  `Replacements.fs` name-based `task` → Promise mapping (§B.2).
- <https://github.com/fable-compiler/Fable/issues/3672> — "Task vs Promise", the JS/Python split that
  motivated it.
- <https://github.com/dotnet/fsharp/pull/6811> — original tasks / resumable state machines PR.
- <https://github.com/dotnet/fsharp/pull/20469> — resumable state-machine lowering in Debug; the
  "no dynamic implementation forced onto the dynamic path" failure class.
- <https://fable.io/fable-promise/documentation/computation-expression.html> — the `promise { }`
  alternative on JS.

**Repo / submodule anchors relied on** (all via R1, re-checked for path validity):

- [`solid/packages/signals/src/core/constants.ts:65-68`](../solid/packages/signals/src/core/constants.ts#L65) — `STATUS_*` flags.
- [`solid/packages/signals/src/core/core.ts:239-252`](../solid/packages/signals/src/core/core.ts#L239) — dep list rebuilt per run.
- [`solid/packages/signals/src/core/core.ts:125-126`](../solid/packages/signals/src/core/core.ts#L125), [`:941`](../solid/packages/signals/src/core/core.ts#L941) — ambient `isPending` / `latest` read modes.
- [`solid/packages/signals/src/core/async.ts:41-49`](../solid/packages/signals/src/core/async.ts#L41), [`:70`](../solid/packages/signals/src/core/async.ts#L70), [`:138`](../solid/packages/signals/src/core/async.ts#L138), [`:246`](../solid/packages/signals/src/core/async.ts#L246), [`:459-466`](../solid/packages/signals/src/core/async.ts#L459) — `_pendingSources` set, auto-dispose exemption, `AsyncIterable` dispatch.
- [`solid/packages/signals/src/core/error.ts:34-39`](../solid/packages/signals/src/core/error.ts#L34) — V8 `stackTraceLimit` zeroing (§3.1 finding: no .NET analogue).
- [`solid/packages/signals/src/boundaries.ts:469-475`](../solid/packages/signals/src/boundaries.ts#L469) — the one status-parameterised boundary constructor.
- [`solid/packages/solid/src/index.ts:167`](../solid/packages/solid/src/index.ts#L167) — `createResource` removed, `// all computations`.
- [`Partas.Solid/SolidBindings.fs`](../Partas.Solid/SolidBindings.fs) — the erased binding surface the native API should stay source-compatible with.
