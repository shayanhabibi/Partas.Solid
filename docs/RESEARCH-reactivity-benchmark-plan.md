# Reactivity benchmark — feasibility research

**Status:** groundwork only. Nothing has been installed, built, or run. No benchmark code exists yet.
**Date:** 2026-09-22.
**Question asked:** can we benchmark Fable.Ripple against Solid v1, Solid v2 — and (added mid-task) against
Partas.Solid — across core reactive primitives, in one in-process JS harness?

**Short answer:** yes for three of the four entrants, and the harness already exists and already ships
adapters for two of them. But the fourth entrant (Partas.Solid) does not have a reactive core to measure,
and that fact reframes the whole exercise. See §7.

---

## 1. The harness: `js-reactivity-benchmark`

### 1.1 It exists, it is canonical, it is alive

- Repo: <https://github.com/milomg/js-reactivity-benchmark> (milomg = Milo Mighdoll, author of
  [Reactively](https://github.com/milomg/reactively), ex-SolidJS team).
- 164 stars / 43 forks / 84 commits. Live results site: <https://js-reactivity-bench.milomg.dev>.
- **Most recent commit `2026-02-15T03:09:50Z`** (`GET /repos/milomg/js-reactivity-benchmark/commits`).
  Not dead, not renamed.
- Lineage claim in the brief is correct: it grew out of Milo's Reactively work
  (<https://milomg.dev/2022-12-01/reactivity>) and is the suite the Vue team used when fixing the
  `@vue/reactivity` regression found by it (<https://github.com/vuejs/core/issues/11928>), and that MobX
  triaged in <https://github.com/mobxjs/mobx/issues/3926>.
- There is a well-known fork, <https://github.com/transitive-bullshit/js-reactivity-benchmark>, used for
  the TC39-Signals-inclusive comparison. **Use the milomg original**; the fork is a snapshot, the original
  is the one that gets fixes.

`js-framework-benchmark` (Krausest) is a *DOM* benchmark and is not this. It is relevant only as the
separate optional axis discussed in §7.4 — do not conflate them.

### 1.2 The adapter interface — verbatim

Source: `packages/core/src/util/reactiveFramework.ts`
(<https://github.com/milomg/js-reactivity-benchmark/blob/main/packages/core/src/util/reactiveFramework.ts>)

```ts
/** interface for a reactive framework.
 *
 * Implement this interface to add a new reactive framework to the performance test suite.
 */
export interface ReactiveFramework {
  name: string;
  signal<T>(initialValue: T): Signal<T>;
  computed<T>(fn: () => T): Computed<T>;
  effect(fn: () => void): void;
  withBatch<T>(fn: () => T): void;
  withBuild<T>(fn: () => T): T;
  cleanup(): void;
}

export interface Signal<T> {
  read(): T;
  write(v: T): void;
}

export interface Computed<T> {
  read(): T;
}
```

So a library must supply exactly six things: a name, a writable source with `read`/`write`, a cached
derived node with `read`, an effect registration, a batch wrapper, a root/scope wrapper (`withBuild`)
and a matching teardown (`cleanup`). There is no `runSync`; flushing is folded into `withBatch`.

A framework is registered by appending a `FrameworkInfo` to `packages/core/src/frameworksList.ts`:

```ts
export interface FrameworkInfo {
  /** wrapper/adapter for a reactive framework */
  framework: ReactiveFramework;
  /** verify the number of nodes executed matches the expected number */
  testPullCounts?: boolean;
}
```
(`packages/core/src/util/frameworkTypes.ts`)

`testPullCounts: true` asserts the framework is *laziness-exact* — that exactly the expected number of
nodes recomputed. Note the comment already in `frameworksList.ts`:

> `{ framework: solidFramework },` `// solid can't testPullCounts because batch executes all leaf nodes even if unread`

That is a live example of a scheduling difference forcing a correctness relaxation, and is directly
relevant to Ripple (§6.4).

### 1.3 Test suites shipped, and what each measures

From `packages/core/src/index.ts` — `runTests` calls, in order: `sbench`, `kairoBench`, `cellxbench`,
`dynamicBench`.

| Suite | File | What it actually measures |
| --- | --- | --- |
| **sBench** (S.js legacy) | `benches/sBench.ts` | Raw construction and update cost: `createSignals`, `createComputations0..8`, `updateComputations`. Dominated by allocation, not propagation. |
| **kairo** | `benches/kairoBench.ts` + `benches/kairo/*` | The propagation-shape suite. Eight graph shapes, each its own file: `avoidable.ts` (propagation that a correct cutoff should suppress), `broad.ts` (wide fan-out), `deep.ts` (long chain), `diamond.ts` (**the glitch case** — one source, two paths, one sink), `mux.ts` (**dynamic dependency switching**), `repeated.ts` (same source read many times by one node), `triangle.ts`, `unstable.ts` (dependency set churns every run), plus `molBench.ts` ($mol's own benchmark). |
| **cellx** | `benches/cellxBench.ts` | The classic CellX layered-graph benchmark, `cellx1000` / `cellx2500`. Creation + update on a static layered graph. |
| **dynamic / reactively** | `benches/reactively/dynamicBench.ts` + `dependencyGraph.ts` | The configurable graph. Driven by `packages/core/src/config.ts`. |

`config.ts` ships six configurations (`width` × `totalLayers`, `staticFraction`, `nSources`,
`readFraction`, `iterations`, plus an `expected: { sum, count }` used for verification):

| width | totalLayers | staticFraction | nSources | readFraction | iterations |
| --- | --- | --- | --- | --- | --- |
| 10 | 5 | 1 | 2 | 0.2 | 600 000 |
| 10 | 10 | 3/4 | 6 | 0.2 | 15 000 |
| 1000 | 12 | 0.95 | 4 | 1 | 7 000 |
| 1000 | 5 | 1 | 25 | 1 | 3 000 |
| 5 | 500 | 1 | 3 | 1 | 500 |
| 100 | 15 | 0.5 | 6 | 1 | 2 000 |

This covers every axis the brief listed: write/read throughput (sBench, `updateSignals`), derived
propagation (kairo `broad`/`deep`/`triangle`), deep chains (`5×500`), wide fan-out (`1000×5`,
`nSources: 25`), diamond/glitch (`kairo/diamond.ts`), dynamic dependency switching (`kairo/mux.ts`,
`kairo/unstable.ts`, `staticFraction < 1`), and batching (`withBatch` is exercised throughout kairo).

**Nothing new needs to be written on the test side.** The work is adapters only.

### 1.4 Runner

Three workspace packages (`packages/core`, `packages/node`, `packages/web`), pnpm workspace,
`packageManager: pnpm@10.29.3` (core) / `pnpm@10.14.0` (node).

- `packages/node/src/index.ts` is a ~25-line `main()` that calls
  `runTests(frameworkInfo, logPerfResult)` and prints CSV.
- Build/run: `esbuild src/index.ts --bundle --format=esm --target=esnext --outdir=dist` then
  `node dist/index.js`.
- `packages/core/README` advertises v8 intrinsics for warmup/cleanup and per-test GC tracking, so **run
  node with `--expose-gc --allow-natives-syntax`** or the GC columns are meaningless.

---

## 2. Solid v1 — already done

`packages/core/src/frameworks/solid.ts` already exists and is already in `frameworksList.ts`:

```ts
import { batch, createEffect, createMemo, createRoot, createSignal } from "solid-js/dist/solid.cjs";

export const solidFramework: ReactiveFramework = {
  name: "SolidJS",
  signal: (initialValue) => { const [getter, setter] = createSignal(initialValue);
                              return { write: (v) => setter(v as any), read: () => getter() }; },
  computed: (fn) => { const memo = createMemo(fn); return { read: () => memo() }; },
  effect: (fn) => createEffect(fn),
  withBatch: (fn) => batch(fn),
  withBuild: (fn) => createRoot((dispose) => { solidFramework.cleanup = dispose; return fn(); }),
  cleanup: () => {},
};
```

Pinned at `"solid-js": "^1.9.11"` in `packages/core/package.json`. npm `solid-js` dist-tags today:
`latest: 1.9.15`, `next: 2.0.0-rc.9`. **Zero work.** Pin to an exact version rather than `^` (§6.6).

---

## 3. Solid v2 — already done, but the pin is wrong

### 3.1 The adapter already exists, under a misleading name

`packages/core/src/frameworks/xReactivity.ts`, registered as `{ framework: xReactivityFramework, testPullCounts: true }`:

```ts
import { flush, createEffect, createMemo, createRoot, createSignal } from "@solidjs/signals";

export const xReactivityFramework: ReactiveFramework = {
  name: "x-reactivity",
  signal: (initialValue) => { const [getter, setter] = createSignal(initialValue as any);
                              return { write: (v) => setter(v as any), read: () => getter() }; },
  computed: (fn) => { const memo = createMemo(fn); return { read: () => memo() }; },
  effect: (fn) => createEffect(fn, () => {}),
  withBatch: (fn) => { fn(); flush(); },
  withBuild: (fn) => createRoot((dispose) => { xReactivityFramework.cleanup = dispose; return fn(); }),
  cleanup: () => {},
};
```

`@solidjs/signals` **is** Solid 2's reactive core. Our submodule at
`solid/packages/signals/package.json` declares exactly that name, version `2.0.0-rc.3`.
So "x-reactivity" in the published charts already *is* Solid v2. Rename it in our fork for clarity.

### 3.2 The version mismatch you must fix

`packages/core/package.json` pins `"@solidjs/signals": "^0.10.2"`. That caret range **cannot** resolve to
`2.0.0-rc.3`. The npm registry shows `@solidjs/signals` dist-tags `latest: 2.0.0-rc.0`, `next: 2.0.0-rc.9`,
with `2.0.0-rc.2 … 2.0.0-rc.9` published. So the charted "x-reactivity" numbers are from the *0.10.x*
line, not from the 2.0-rc line our submodule tracks. Bump to an exact `2.0.0-rc.3` and re-verify the
adapter compiles — the two-argument `createEffect` it uses is still correct
(`solid/packages/signals/src/signals.ts:482-502`), and single-argument `createEffect` is now a hard
`never` with a `[MISSING_EFFECT_FN]` diagnostic (`:488-497`), so an old adapter would fail loudly, not
silently.

### 3.3 Do **not** build the submodule

`solid/packages/signals` *can* be built standalone in principle — its `package.json` `devDependencies`
are self-contained (rollup, typescript, vitest; no `workspace:*` entries) and `src/index.ts` imports only
relative paths (`./core/index.js`, `./signals.js`, `./affects.js`, `./map.js`, `./store/index.js`).
But its build script is

```
"build": "npm-run-all -nl build:* && pnpm types"
  build:js    → rollup -c && node ./scripts/mangle-props.mjs dist/prod dist/node.cjs
                            && node ./scripts/check-pure.mjs dist/prod
  types       → tsc -p tsconfig.build.json && node ../../scripts/sync-dual-types.mjs ...
```

— `sync-dual-types.mjs` reaches up into the monorepo `scripts/` dir, and the root is a pnpm workspace
(`pnpm-workspace.yaml`: `packages/*`, plus `overrides` for `@solidjs/signals`) requiring
`pnpm@11.1.1` and turbo. The submodule has **no `node_modules` and no `dist`** right now (verified).

Also: the prop-mangling step means a locally-built `dist/prod` is *not* byte-equivalent to any npm
tarball, so a self-build silently changes what you are measuring.

**Recommendation: install `@solidjs/signals@2.0.0-rc.3` from npm.** It is the same version as the
submodule pin, it is the artifact real users get, and it costs zero build setup. Reserve building the
submodule for the case where you need to benchmark an unreleased commit — our pin is
`47fc4b22` = `v2.0.0-rc.0-154-g47fc4b22`, i.e. 154 commits past rc.0, which is *near* rc.3 but not
provably identical to it. If that gap matters, build; otherwise do not.

### 3.4 Flag: `flush` is not `batch`

Solid 2 has no `batch`. The adapter substitutes `fn(); flush();`. That is a semantic substitution, not an
equivalence, and it is a confound — see §6.4.

---

## 4. Fable.Ripple — adaptable, every required member exists

### 4.1 Provenance and volatility

- Repo <https://github.com/fable-hub/Fable.Ripple>, author Maxime Mangel (of Fable / Nacara).
- **First commit `2026-09-16T16:09:17Z`. Latest commit `0eeb2ca2` `2026-09-19T16:25:27Z`.** Six days old
  at time of writing. 12 commits, 8 stars.
- NuGet `Fable.Ripple` versions: `0.0.0`, `1.0.0-beta.1`, `1.0.0-beta.2`. Latest nuspec repository commit
  `0eeb2ca2b35ae93cc8be108f71f16cf0f4b3ef5c`.
- Dependencies (`.nuspec`, netstandard2.1): `FSharp.Core 10.1.301`, **`Fable.Core 5.2.0`**. So Fable 5.x
  tooling. Our repo already pins `fable 5.13.0` in `.config/dotnet-tools.json` — compatible, no new
  toolchain.
- Install: `dotnet add package Fable.Ripple --prerelease`.
- **The NuGet package ships its F# sources** under `fable/` (`Api.fs`, `Types.fs`, `ReactiveNode.fs`,
  `Builder.fs`, `Internal/{Graph,Scheduler,Scope,Tracking}.fs` — 859 lines total), which is how the
  analysis below was done without cloning.

### 4.2 API → adapter mapping (all six members satisfiable)

| Harness member | Ripple API | Source |
| --- | --- | --- |
| `signal(v)` | `Var.create : 'T -> Var<'T>`; `.Value` get/set, `.Peek()`, `.Set v` | `Api.fs:26`, `Types.fs:45-61` |
| `computed(fn)` | `Signal.computed : (unit -> 'T) -> Signal<'T>` | `Api.fs:51` |
| `effect(fn)` | `Signal.effect : (unit -> unit) -> IDisposable` | `Api.fs:119-125` |
| `withBatch(fn)` | `Signal.batch : (unit -> unit) -> unit` | `Api.fs:136` → `Scheduler.batch`, `Scheduler.fs:63-72` |
| `withBuild(fn)` / `cleanup()` | `Signal.root : (unit -> 'a) -> 'a * IDisposable` | `Api.fs:142` → `Scope.root` |
| — | `Signal.untracked`, `Signal.onCleanup`, `Signal.observerCount` (diagnostic) | `Api.fs:139,145,149` |

**Nothing the harness requires is missing.** Batch exists, scoped disposal exists, effect disposal exists.
`Signal.observerCount` is even a free gift for asserting `testPullCounts`-style invariants.

### 4.3 Exporting from F# to the harness

The adapter must be a plain JS module the harness can `import`. Two options; take the first.

**Option A (recommended): write the whole `ReactiveFramework` object in F#, export it via Fable.**

```fsharp
module RippleAdapter

open Fable.Core
open Fable.Core.JsInterop
open Fable.Ripple

// `===`, to neutralise the structural-equality confound (see §6.2)
[<Emit("$0 === $1")>]
let inline private refEq (a: 'T) (b: 'T) : bool = jsNative

let mutable private disposeRoot : System.IDisposable option = None
let private effectHandles = ResizeArray<System.IDisposable>()

let framework =
    createObj [
        "name"      ==> "Fable.Ripple"
        "signal"    ==> fun (v: obj) ->
                          let var = Var.createWith refEq v
                          createObj [ "read"  ==> fun () -> var.Value
                                      "write" ==> fun (nv: obj) -> var.Value <- nv ]
        "computed"  ==> fun (fn: unit -> obj) ->
                          let s = Signal.computedWith refEq fn
                          createObj [ "read" ==> fun () -> s.Value ]
        "effect"    ==> fun (fn: unit -> unit) -> effectHandles.Add(Signal.effect fn)
        "withBatch" ==> fun (fn: unit -> obj) -> Signal.batch (fn >> ignore)
        "withBuild" ==> fun (fn: unit -> obj) ->
                          let result, d = Signal.root fn
                          disposeRoot <- Some d
                          result
        "cleanup"   ==> fun () ->
                          for h in effectHandles do h.Dispose()
                          effectHandles.Clear()
                          disposeRoot |> Option.iter (fun d -> d.Dispose())
                          disposeRoot <- None
    ]
```

Compile with `dotnet fable RippleAdapter.fsproj -o out --lang js -e .js -c Release --optimize`, then in
the harness fork:

```ts
import { ReactiveFramework } from "../util/reactiveFramework";
export const rippleFramework = (await import("../../../ripple-adapter/out/RippleAdapter.js"))
  .framework as ReactiveFramework;
```

**Option B:** export `Var.create`, `Signal.computed`, … individually from Fable and write the adapter
object in TypeScript. Cleaner types, but adds a JS→F#-closure boundary crossing on *every* call that the
other adapters do not pay. Do not do this; it manufactures the very confound §6.1 is about.

Either way the harness bundles with esbuild, so Fable's `fable_modules/fable-library-js` gets tree-shaken
and bundled alongside — verify the bundle actually shrank (`--analyze`) rather than dragging in
`Map`/`Set`/`Comparer` machinery, which would be a red flag for a structural-equality leak (§6.2).

---

## 5. Partas.Solid — the fourth entrant, and why it is not one

This section addresses the coordinator's added scope directly. Two of the three thesis claims hold; the
third — the one the strategy rests on — does not.

### 5.1 ✅ CONFIRMED: Partas.Solid erases to real Solid

`Partas.Solid/SolidBindings.fs` declares its primitives on an `[<AutoOpen>] [<Erase>] type Bindings`
(`SolidBindings.fs:563-565`) where **every** member is `[<ImportMember "solid-js">] … = jsNative`:

- `createSignal` — nine overloads, `SolidBindings.fs:646-658`
- `createMemo` — `:611-621`
- `createEffect` — fifteen overloads, `:572-605`
- `createRoot` — `:778-784`
- `flush` — `:660-662`

These are direct ESM imports. There is no Partas-authored reactive code; there is nothing for Fable to
compile except the call site. **On primitives, Partas.Solid is Solid v2, byte for byte.** Thesis
confirmed — and stronger than stated, because these are not even thin wrappers, they are erased imports.

Corollary: **yes, they are usable outside a component.** They are plain static members with no dependency
on the plugin, the JSX path, or `[<SolidComponent>]`. An adapter is writable today.

### 5.2 ✅ CONFIRMED: Ripple compiles its core through Fable, with no plugin of its own

Verified against the repo tree and the shipped sources:

- **Fable.Ripple has no compiler plugin.** A recursive tree listing of `fable-hub/Fable.Ripple` contains
  no `*.FablePlugin*` project, no file matching `plugin` in any path, and no
  `MemberDeclarationPluginAttribute`. The `.fsproj` list is: `src/Fable.Ripple`, `src/Fable.Ripple.Dom`,
  `src/Fable.Ripple.Dom.Test`, `src/Fable.Ripple.Form`, `src/Fable.Ripple.Form.Plain`,
  `src/Fable.UrlParser`, `build/EasyBuild.fsproj`, `demo/Demo.fsproj`, `bench/apps/fable-ripple/App.fsproj`.
- **Almost no raw-JS escape hatches.** Across all 859 lines of the shipped core there is exactly **one**
  interop attribute: `[<Struct; Erase>]` on `Signal<'T>` (`Types.fs:67`). Zero `[<Emit>]`, zero
  `[<Import>]`, zero `JsInterop`. The graph, the scheduler, the tracking and the scope machinery are
  100% Fable-generated JS.

So the asymmetry the coordinator described is real and now documented.

### 5.3 ⚠️ BUT: "naive Fable codegen" is the wrong mental model of Ripple

This is the part of the thesis that needs correcting before it drives a benchmark design. Ripple's core
is not idiomatic F# accidentally compiled to JS. It is F# **written backwards from the JS Fable emits**,
with the comments to prove it. From the shipped sources:

- **Bit-state as an enum, explicitly for erasure.** `ReactiveNode.fs:3-12`: *"an F# enum so it is plain
  int at runtime (erased by Fable) — named and type-checked, zero cost vs raw ints."* States are
  `Clean=0 < Check=1 < Dirty=2`, compared with `int node.State < int target` (`Scheduler.fs:19`) —
  the same ordered-state trick Solid uses.
- **Inline-first-edge layout to dodge allocation.** `ReactiveNode.fs:44-61`: `FirstSource` /
  `RestSources` / `FirstObserver` / `RestObservers`, with the comment *"Almost every node in a
  fine-grained graph has 0 or 1 edges, so this avoids a `ResizeArray` (list object + backing array) per
  single-edge node — the dominant per-node allocation."* That is a deliberate anti-Fable-tax measure.
- **Shared default closure.** `ReactiveNode.fs:14-18` / `:66`: `Defaults.noRecompute` is a module-level
  `unit -> bool` so the default costs *"a reference copy per node rather than a fresh closure."*
- **`voption` everywhere, not `option`.** `FirstSource: ReactiveNode voption`, `EffectFn: (unit -> unit) voption`,
  `currentGets: ResizeArray<ReactiveNode> voption` — struct options, which Fable lowers to
  `undefined`-sentinel checks rather than allocating `Some` cells.
- **Reference equality on the hot tracking path.** `Tracking.fs:34` uses `obj.ReferenceEquals` to match a
  read against the previous run's source list, and the fast path *"just advance the index (no edge
  mutation)"* (`:24-36`) allocates nothing for a stable graph. `currentGets` (`ResizeArray`) is allocated
  lazily, only on the first dependency-set divergence (`:38-44`).
- **Edge reconciliation only on divergence**, and only on success, so a throwing computation cannot
  corrupt the graph (`Tracking.fs:81-98`).
- **The scheduler is a flat pending array with a re-entrancy guard**, not a tick/microtask queue
  (`Scheduler.fs:10-14, 30-45`), and `flush` deliberately tolerates the queue growing mid-flush
  (`:37-38`).

The one published release note on `1.0.0-beta.2` is literally *"Share the default Recompute closure
across nodes"* — a Fable-codegen micro-optimisation. This author is already playing the game the
benchmark was meant to score.

**Conclusion: a four-way race will not show a large "Fable tax" gap on the algorithm.** The remaining
Fable-attributable overheads in Ripple are narrower than the thesis assumes, and are listed in §6.1.

### 5.4 The actual hot read path, for the record

`Var.Value` getter (`Types.fs:45-49`) is three operations:

```fsharp
member this.Value
    get (): 'T =
        Tracking.track this          // module-level mutable `current` check; no alloc on stable graph
        Tracking.updateIfNecessary this
        value
```

Two things to note, both benchmarkable:

1. **`updateIfNecessary` is called on every read, including reads of plain sources** — for which it is
   pure overhead: `State = Check`? no. `State = Dirty`? no. `State <- Clean` (a redundant *write* to a
   property on every single source read). `Tracking.fs:49-62`. Solid short-circuits sources entirely.
2. **`member val … with get, set`** — Fable emits these as real JS accessor properties, not plain fields.
   Every `node.State`, `node.Queued`, `node.FirstSource` touch in the graph goes through a getter/setter
   pair rather than a direct slot. Solid's core uses plain fields. **This is the single most plausible
   mechanical Fable tax in Ripple, and it is exactly the kind of thing a compiler plugin could erase.**
   *Verify this in the emitted JS before asserting it* — Fable's auto-property lowering has changed
   across versions, and V8 inlines monomorphic accessors well.

### 5.5 ❌ The thesis's strategic conclusion does not follow

> "can Partas.Solid beat Fable.Ripple by emitting more performant JS?"

At the primitive level, **Partas.Solid does not emit reactive JS at all** (§5.1). Adding it as a fourth
entrant would produce a bar essentially identical to Solid v2's, differing only by the cost of the
adapter's own F# closures. That delta measures *the adapter*, not the framework. Reporting it as
"Partas.Solid's performance" would be misleading.

The compiler-plugin asymmetry is real, but it lives in the **rendering** layer, not the reactive layer.
`Partas.Solid.FablePlugin` rewrites F# AST into JSX (`Plugin.fs`), and its optimisations —
`SkipPojoOptimisation`, `SkipCEOptimisation`, `SkipOmit`, `SpreadProps` (`Types.fs` `ComponentFlag`) —
all concern prop objects and JSX shape. None of them can touch `createSignal`, because `createSignal` is
an import.

---

## 6. Confound analysis

This is the section that determines whether any number produced is worth publishing.

### 6.1 Fable closure / allocation overhead at the adapter boundary

**Risk:** the harness calls `signal.read()` hundreds of millions of times. If the Ripple adapter's `read`
is an F# closure wrapping a call to an F# property getter, while Solid's `read` is an arrow function
wrapping a call to a plain JS function, the measured difference includes one extra frame that has nothing
to do with either algorithm.

**Controls:**
1. Write the adapter object in F# (§4.3 Option A) so all three adapters have exactly one wrapper layer.
2. Inspect the emitted `RippleAdapter.js` by hand and confirm `read` is `() => var_1.Value` and not
   `() => { const x = ...; return x; }` with currying artefacts. Fable can emit curried applications for
   multi-arg F# functions — `createObj [ "write" ==> fun v -> ... ]` is single-arg, which avoids this.
3. **Build a null adapter**: a `ReactiveFramework` over a trivial hand-written JS signal, and the *same*
   trivial signal reimplemented in F# and Fable-compiled. The ratio between those two runs on the same
   suite is a direct, numeric measurement of the adapter-level Fable tax, and every other result should
   be read net of it. This is the cheapest high-value thing in this whole plan.

### 6.2 Equality / cutoff semantics — **the single biggest confound**

**Ripple's default is structural equality, Solid's is `===`.**

- `Api.fs:11` — `Var.defaultEquals` is `Unchecked.equals a b`.
- `Api.fs:26` — `Var.create initial = createWith defaultEquals initial`.
- `Api.fs:37` — `Signal.defaultEquals` is likewise `Unchecked.equals a b`.
- `Api.fs:51` — `Signal.computed f = computedWith defaultEquals f`.
- It is consulted on **every write** (`Types.fs:51`: `if not (equals value v) then …`) and on **every
  computed recompute** (`Types.fs:31`: `if equals value nv then false else …`).

Fable lowers `Unchecked.equals` to a call into `fable-library-js`'s `equals(x, y)`, which type-switches
before falling through to `===`. Solid's default is a bare `a === b` inline in the signal setter.
In `updateSignals` — 600 000 iterations of "write a number, read it back" — that is a library function
call per write versus an inline comparison. **This alone could plausibly account for the entire
difference between first and last place**, and it would be reported as "Ripple's algorithm is slow",
which would be false.

**Controls (do all three):**
1. **Primary run:** use `Var.createWith refEq` and `Signal.computedWith refEq` with
   `[<Emit("$0 === $1")>]`, making the cutoff semantics *identical* to Solid's. This is the
   apples-to-apples run.
2. **Secondary run:** use `Var.create` / `Signal.computed` (structural). This is the
   out-of-the-box-experience run. Publish both, labelled.
3. **Report the gap between (1) and (2) as its own finding.** It is the most actionable result for
   Ripple's author and the most likely to be mis-attributed by a reader.

Note Ripple *does* ship `Signal.referenceEquals` (`Api.fs:40`) but it is constrained
`when 'T: not struct`, so it cannot be used for the `number` payloads the benchmark uses. Hence the
`[<Emit>]`.

### 6.3 Is a hand-written-JS Ripple baseline needed?

**Yes, and §5.3 is why.** Without it you cannot separate "Ripple's algorithm is worse than Solid's" from
"Ripple's algorithm is fine but Fable's codegen costs 20%". Given how carefully Ripple is already
written for Fable, the honest prior is that the codegen gap is small — which makes measuring it *more*
important, not less, because a small gap is the finding.

Scope it tightly: transliterate `ReactiveNode` + `Graph` + `Tracking` + `Scheduler` (859 lines, of which
maybe 400 matter) into plain JS, preserving the algorithm exactly — same inline-first-edge layout, same
ordered states, same reconciliation. Use **plain fields, not accessors** (§5.4), so the diff isolates
precisely the `member val` question. Two adapters, `Fable.Ripple` and `Ripple-JS`, same suite.

That gives the clean 2×2 the coordinator asked for:

| | algorithm = Solid's | algorithm = Ripple's |
| --- | --- | --- |
| **codegen = hand JS** | `SolidJS` / `x-reactivity` adapters | **`Ripple-JS` (to be written)** |
| **codegen = Fable** | `Partas.Solid` adapter (≈ identical to Solid v2 — see §5.5) | `Fable.Ripple` adapter |

Read column-wise for Fable tax, row-wise for algorithm quality. Note the bottom-left cell is the one that
does real work; the bottom-right… top-left comparison is the headline the user actually wants.

### 6.4 Scheduling differences — genuinely non-comparable cases

Ripple's claim ("synchronous; the write settles before the next line runs; no scheduler, no tick") is
substantiated by `Scheduler.fs:57-61`: `notifyChange` marks observers and then, `if batchDepth = 0 then
flush ()` — inline, on the writing thread, no microtask.

This creates three comparability hazards:

1. **`withBatch` is not the same operation in all three.** Solid v1 has a real `batch`. Solid v2 has no
   batch at all — the existing adapter fakes it with `fn(); flush()`. Ripple has a real nesting-depth
   batch. A test that measures "cost of batching N writes" is measuring three different things.
2. **`testPullCounts` will differ.** The harness already disables it for Solid v1 because *"batch
   executes all leaf nodes even if unread"*. Ripple only queues *effects* (`Scheduler.fs:23-25`:
   `if node.IsEffect && not node.Queued`) and pulls computeds lazily on read
   (`ReactiveNode.fs:28`: *"Computeds and sources are pulled on read instead, never queued"*). So Ripple
   is likely *more* pull-exact than Solid v1. **Try `testPullCounts: true` for Ripple first;** if it
   passes, that is a correctness win worth reporting separately from timing.
3. **Effects in `kairo`** are driven by `framework.effect(...)` then a write then a read. With Ripple's
   synchronous flush the effect has already run by the time the harness reads; with Solid v2 the adapter's
   `flush()` does it. Verify the `expected: { sum, count }` assertions in `config.ts` pass for Ripple
   before trusting a single timing number — **a failing sum assertion means the adapter is measuring a
   different computation, and the time is meaningless.**

### 6.5 Reporting hygiene

- Same machine, same Node build, one run at a time, CPU governor pinned, no other load.
- Run all frameworks in the *same process invocation* (the harness already does this) so JIT/GC state is
  shared — that is the whole point of "in-process apples-to-apples", and it holds: all four entrants are
  JS modules in one bundle. **The brief's core assumption is verified.**
- Report medians of ≥5 full runs, plus the GC columns, not a single best-of.
- Publish every adapter's source next to the numbers.

### 6.6 Pinning a moving target

Ripple is six days old (§4.1) and its one release note is a perf fix. Any number will be stale within
weeks. Record, in the results file itself:

- `Fable.Ripple` NuGet version **and** the `repository commit` from the nuspec
  (`1.0.0-beta.2` → `0eeb2ca2b35ae93cc8be108f71f16cf0f4b3ef5c`).
- `solid-js` exact version (not `^`), `@solidjs/signals` exact version.
- The `Partas.Solid` commit SHA, and the `solid` submodule SHA (currently `47fc4b22`,
  `v2.0.0-rc.0-154-g47fc4b22`).
- `dotnet fable --version`, `node --version`, and the harness fork's commit.
- Date and hardware.

State plainly in the writeup that the Ripple number is a snapshot of a beta under active optimisation and
should be re-run before being cited.

---

## 7. Verdict

### 7.1 Is the benchmark feasible?

**Yes.** Every assumption in the brief checks out: one harness, one process, four JS modules, no DOM, no
cross-language RPC. Two of four adapters already exist upstream. The third (Ripple) maps 1:1 onto the
interface with nothing missing. Total new code: one F# adapter (~40 lines), one registration line, one
version bump, and — if you want the result to mean anything — one hand-written JS baseline (§6.3).

### 7.2 Is it worth running?

**For Ripple vs Solid v1 vs Solid v2: yes, with the §6.2 equality control.** That is a real, unanswered
question, the entrants are genuinely different algorithms, and the result is actionable for Ripple's
author. Without the equality control it is worse than useless — it would publish a wrong conclusion.

**For Partas.Solid as a fourth bar: no.** It would be a duplicate of the Solid v2 bar plus adapter noise.
Include it only as an explicitly-labelled *control* — "this is what the Fable adapter boundary costs on
top of raw Solid v2" — which is genuinely useful as the calibration described in §6.1, but must not be
presented as a framework comparison.

### 7.3 Blunt answer on the "out-compile Ripple" framing

**It is the wrong framing, for two independent reasons, and the second is the interesting one.**

*First:* Partas.Solid has no reactive core to compile (§5.1). You cannot out-compile a competitor in a
category you do not compete in. On primitives, Partas.Solid inherits Solid's performance by construction
— which is a genuine architectural advantage worth stating in prose, but it is not a benchmark result,
and no plugin optimisation can improve it because there is nothing left to optimise.

*Second, and more important:* even if you did write a reactive core, Ripple's is already written by
someone who is optimising *for Fable's codegen specifically* (§5.3) — erased enums, `voption`, inline-first
edges, shared default closures, a release note about sharing a closure. The available margin is small.
Betting the strategy on out-compiling that is betting on a narrow margin against an attentive author.

**The decisive battleground is the rendering layer.** That is where Partas.Solid's plugin actually emits
code, where it has a structural advantage that Ripple architecturally cannot match, and where the
performance difference is large rather than marginal:

- Partas.Solid compiles to JSX, which `@solidjs/babel-plugin` + `dom-expressions` turn into **cloned
  template fragments** with surgical binding — `cloneNode(true)` on a pre-parsed `<template>`, plus a
  handful of `insert`/`setAttribute` calls.
- Fable.Ripple.Dom builds the DOM from an F#-authored HTML DSL (`src/Fable.Ripple.Dom/{Base,Attributes,…}.fs`),
  i.e. imperative `createElement`/`appendChild` driven by F# list construction, with no template cloning
  and no compile-time specialisation. Its README says so: *"HTML DSL, no virtual tree."* No virtual tree
  is not the same as template cloning.

That is a multiple-x structural difference in element-creation cost, and it is the one Partas.Solid's
plugin is actually positioned to win.

### 7.4 Which benchmark would settle it

**`js-framework-benchmark` (Krausest)** — <https://github.com/krausest/js-framework-benchmark> — the
DOM benchmark the brief correctly identified as a separate axis. Run `partas-solid` against
`fable-ripple` on *create 10 000 rows*, *replace all rows*, *swap rows*, *select row*, *clear*, plus the
memory columns. Those are precisely the metrics that separate template-cloning from imperative
construction.

Encouragingly, **Ripple already ships a comparable harness**: `bench/harness/run.mjs` with apps at
`bench/apps/fable-ripple`, `bench/apps/vanjs`, and `bench/apps/manual` (a hand-written-JS control — the
author had the same idea as §6.3). Adding a `bench/apps/partas-solid` there, or better, adding both to
Krausest's standard suite, is a smaller job than it looks and produces a number that means something to
outsiders.

**Recommended sequencing:** run the primitives benchmark first because it is nearly free and the
equality finding (§6.2) is worth having on its own — but treat it as due diligence, not as the
strategic question. Budget the real effort for the DOM benchmark.

### 7.5 Solid 2's async primitives — feature-presence table, not numbers

Confirmed: `solid/packages/signals/src/index.ts` exports an async-aware surface with **no counterpart in
Solid v1 or in Ripple**:

`NotReadyError` (`:5`), `isPending` (`:21`), `latest` (`:22`), `refresh` (`:23`),
`enforceLoadingBoundary` (`:29`), `createTrackedEffect` (`:58`), `createOptimistic` (`:60`),
`resolve` (`:61`), `onSettled` (`:62`), `action` (`:6`), plus `flush` (`:11`) and the projection
primitives `affects` (`:78`), `mapArray`/`repeat` (`:79`).

Ripple's entire public surface is `Api.fs` — `Var.create/createWith`, `Signal.constant/computed/
computedWith/map*/map2*/map3/bind/effect/subscribe/batch/untracked/root/onCleanup/observerCount` —
with **no async, no suspense, no optimistic-update, no loading-state primitive anywhere**.

There is nothing to race. Any timing comparison on this axis would be a comparison against absence.
**Recommendation accepted: present §6-adjacent async capability as a feature-presence matrix**
(primitive × {Solid v1, Solid v2, Fable.Ripple, Partas.Solid}) with ✓/✗ and a one-line note on what the
absence forces the user to hand-roll. Partas.Solid's column is Solid v2's column, since it binds all of
them (`SolidBindings.fs`). That table is, incidentally, a stronger argument for Partas.Solid than any
primitives chart would be.

---

## 8. Exact setup commands

Nothing below has been run. No package has been installed.

```bash
# 1. Fork and clone the harness (NOT into this repo)
gh repo fork milomg/js-reactivity-benchmark --clone
cd js-reactivity-benchmark
corepack enable && pnpm install

# 2. Correct the Solid v2 pin (currently ^0.10.2, which predates the 2.0 line)
pnpm --filter js-reactivity-benchmark add -D @solidjs/signals@2.0.0-rc.3
pnpm --filter js-reactivity-benchmark add -D solid-js@1.9.15      # exact, not ^

# 3. Rename the misleading adapter name in packages/core/src/frameworks/xReactivity.ts
#      name: "x-reactivity"  ->  name: "Solid v2 (@solidjs/signals 2.0.0-rc.3)"

# 4. Baseline run, before adding anything, to confirm the harness is sane on this machine
pnpm --filter js-reactivity-benchmark-node build
node --expose-gc --allow-natives-syntax packages/node/dist/index.js | tee baseline.csv

# 5. Ripple adapter project (separate dir, NOT inside Partas.Solid)
dotnet new classlib -lang F# -o ripple-adapter && cd ripple-adapter
dotnet add package Fable.Ripple --prerelease          # 1.0.0-beta.2
dotnet add package Fable.Core --version 5.2.0
dotnet tool install fable --version 5.13.0 --create-manifest-if-needed
#   ... write RippleAdapter.fs per §4.3 Option A ...
dotnet fable . -o out --lang js -e .js -c Release --optimize

# 6. Wire it in: import in frameworksList.ts, append { framework: rippleFramework, testPullCounts: true }
#    Then rebuild and run. If the config.ts sum/count assertions fail, STOP and fix the adapter
#    before reading any timing (see §6.4.3).
```

---

## 9. Sources

Repository files read (this repo, read-only):

- `solid/package.json`, `solid/pnpm-workspace.yaml`
- `solid/packages/signals/package.json` (name `@solidjs/signals`, version `2.0.0-rc.3`, build scripts, exports map)
- `solid/packages/signals/src/index.ts:1-80` (export surface, async primitives)
- `solid/packages/signals/src/signals.ts:482-512` (`createEffect` two-arg requirement, `MISSING_EFFECT_FN`)
- `solid/packages/signals/src/core/scheduler.ts:928-930` (`flush`)
- `Partas.Solid/SolidBindings.fs:563-565, 572-605, 611-621, 646-662, 778-784` (erased `ImportMember` primitives)
- `git submodule status` → `solid` @ `47fc4b22` (`v2.0.0-rc.0-154-g47fc4b22`), `dom-expressions` @ `e97e4290`

`js-reactivity-benchmark` @ `main` (via raw.githubusercontent / GitHub API):

- `packages/core/src/util/reactiveFramework.ts` — interface quoted verbatim in §1.2
- `packages/core/src/util/frameworkTypes.ts` — `FrameworkInfo`, `TestConfig`
- `packages/core/src/frameworks/solid.ts`, `frameworks/xReactivity.ts`, `frameworks/alienSignals.ts`, `frameworks/preactSignals.ts`
- `packages/core/src/frameworksList.ts` — registration, and the `testPullCounts` comment on Solid
- `packages/core/src/index.ts` — `runTests` ordering
- `packages/core/src/config.ts` — the six graph configurations
- `packages/core/package.json`, `packages/node/package.json`, `packages/node/src/index.ts`
- `GET /repos/milomg/js-reactivity-benchmark/commits` — last commit `2026-02-15T03:09:50Z`

`Fable.Ripple` (NuGet `1.0.0-beta.2`, sources shipped inside the package under `fable/`):

- `fable/Api.fs:11, 26, 37, 40, 51, 119-125, 136, 139, 142, 145, 149`
- `fable/Types.fs:11-19, 31, 45-61, 67-84, 106-110`
- `fable/ReactiveNode.fs:3-18, 22-70`
- `fable/Internal/Scheduler.fs:10-14, 18-28, 30-45, 57-72`
- `fable/Internal/Tracking.fs:11-22, 24-46, 49-62, 64-113, 115-122`
- `.nuspec` — `FSharp.Core 10.1.301`, `Fable.Core 5.2.0`, repository commit `0eeb2ca2`
- `GET /repos/fable-hub/Fable.Ripple/git/trees/main?recursive=1` — no plugin project anywhere
- `GET /repos/fable-hub/Fable.Ripple/commits` — first `2026-09-16T16:09:17Z`, latest `2026-09-19T16:25:27Z`
- <https://fable-hub.github.io/Fable.Ripple/reference/fable-ripple/fable-ripple/{var,signal,reactivenode,varextensions}/>

Registry metadata:

- `registry.npmjs.org/@solidjs/signals` — dist-tags `latest: 2.0.0-rc.0`, `next: 2.0.0-rc.9`
- `registry.npmjs.org/solid-js` — dist-tags `latest: 1.9.15`, `beta: 1.10.0-beta.0`, `next: 2.0.0-rc.9`
- `api.nuget.org/v3-flatcontainer/fable.ripple/index.json` — `0.0.0`, `1.0.0-beta.1`, `1.0.0-beta.2`

Other:

- <https://github.com/krausest/js-framework-benchmark> (§7.4, DOM axis — separate benchmark)
- <https://milomg.dev/2022-12-01/reactivity>, <https://github.com/vuejs/core/issues/11928>,
  <https://github.com/mobxjs/mobx/issues/3926> (harness provenance/impact)
