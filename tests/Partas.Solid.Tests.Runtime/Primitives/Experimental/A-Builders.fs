module Partas.Solid.Tests.Runtime.Primitives.Experimental.Builders

open Partas.Solid
open Partas.Solid.Experimental
open Fable.Core
open Fable.Core.JsInterop

/// Shared harness: a primary signal, a secondary signal, a log, and the owning root's dispose.
type Probe =
    { set: Setter<int>
      setOther: Setter<int>
      log: ResizeArray<string>
      dispose: unit -> unit }

let private probe set setOther log dispose =
    { set = set
      setOther = setOther
      log = log
      dispose = dispose }

// ---------------------------------------------------------------- effect

/// `effect { let! v = source; ... }`: the source is the tracked compute phase, the rest the effect phase.
/// The compute is logged by wrapping the source in `lambda`.
let makeEffect () : Probe =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string> ()
        let value, setValue = createSignal 1
        let other, setOther = createSignal 100

        effect {
            let! v =
                lambda {
                    log.Add "compute"
                    value ()
                }

            log.Add $"effect:{v}:{other ()}"
        }

        probe setValue setOther log dispose)

/// Tracking two sources by binding a tuple-returning lambda.
let makeEffectTuple () : Probe =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string> ()
        let a, setA = createSignal 1
        let b, setB = createSignal 10

        effect {
            let! x, y = lambda { a (), b () }
            log.Add $"pair:{x}:{y}"
        }

        probe setA setB log dispose)

/// Statements before the `let!` run once, immediately, not as part of the effect.
let makeEffectPreamble () : Probe =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string> ()
        let value, setValue = createSignal 1
        let _, setOther = createSignal 0

        effect {
            log.Add "preamble"
            let! v = value
            log.Add $"effect:{v}"
        }

        probe setValue setOther log dispose)

/// Branching in the effect phase: a match whose last arm holds an if/else.
let makeEffectBranching () : Probe =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string> ()
        let value, setValue = createSignal 1
        let _, setOther = createSignal 0

        effect {
            let! v = value

            match v with
            | 1 -> log.Add "one"
            | 2 -> log.Add "two"
            | n when n < 0 -> log.Add "negative"
            | _ -> if v % 2 = 0 then log.Add "even" else log.Add "odd"
        }

        probe setValue setOther log dispose)

/// An `if ... then` with no else as the last statement of the effect phase (uses the builder's Zero).
let makeEffectGuard () : Probe =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string> ()
        let value, setValue = createSignal 1
        let _, setOther = createSignal 0

        effect {
            let! v = value

            if v > 2 then
                log.Add $"big:{v}"
        }

        probe setValue setOther log dispose)

/// The source is a memo built with the `memo` builder, so the effect only re-runs when the memo's
/// value changes (parity).
let makeEffectOverMemo () : Probe =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string> ()
        let value, setValue = createSignal 1
        let _, setOther = createSignal 0

        let parity =
            memo {
                log.Add "parity"
                if value () % 2 = 0 then "even" else "odd"
            }

        effect {
            let! p = parity
            log.Add $"effect:{p}"
        }

        probe setValue setOther log dispose)

/// Writes to another signal from the effect phase (the canonical "sync state" use).
type Mirror =
    { set: Setter<int>
      mirrored: Accessor<int>
      dispose: unit -> unit }

let makeEffectMirror () : Mirror =
    createRoot (fun (dispose: unit -> unit) ->
        let value, setValue = createSignal 1
        let mirrored, setMirrored = createSignal 0

        effect {
            let! v = value
            setMirrored (v * 10)
        }

        { set = setValue
          mirrored = mirrored
          dispose = dispose })

// ------------------------------------------------------------------ memo

type MemoProbe =
    { set: Setter<int>
      setOther: Setter<int>
      value: Accessor<int>
      runs: unit -> int
      log: ResizeArray<string>
      dispose: unit -> unit }

/// `memo { ... }` over one signal, counting its computations.
let makeMemo () : MemoProbe =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string> ()
        let runs = ref 0
        let value, setValue = createSignal 2
        let _, setOther = createSignal 0

        let doubled =
            memo {
                runs.Value <- runs.Value + 1
                value () * 2
            }

        { set = setValue
          setOther = setOther
          value = doubled
          runs = fun () -> runs.Value
          log = log
          dispose = dispose })

/// `memo` whose body uses `untrack` to peek at a second signal.
let makeMemoUntrack () : MemoProbe =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string> ()
        let runs = ref 0
        let value, setValue = createSignal 1
        let other, setOther = createSignal 10

        let sum =
            memo {
                runs.Value <- runs.Value + 1
                value () + untrack other
            }

        { set = setValue
          setOther = setOther
          value = sum
          runs = fun () -> runs.Value
          log = log
          dispose = dispose })

/// `cleanup { }` registered inside a `memo { }` runs before every recomputation and on disposal.
let makeMemoWithCleanup () : MemoProbe =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string> ()
        let runs = ref 0
        let value, setValue = createSignal 1
        let _, setOther = createSignal 0

        let m =
            memo {
                let v = value ()
                runs.Value <- runs.Value + 1
                log.Add $"memo:{v}"
                cleanup { log.Add $"memo-cleanup:{v}" }
                v
            }

        effect {
            let! v = m
            log.Add $"effect:{v}"
        }

        { set = setValue
          setOther = setOther
          value = m
          runs = fun () -> runs.Value
          log = log
          dispose = dispose })

/// Branching memo bodies: if/elif/else and match as the result expression.
type Classified =
    { set: Setter<int>
      sign: Accessor<string>
      size: Accessor<string>
      dispose: unit -> unit }

let makeClassified () : Classified =
    createRoot (fun (dispose: unit -> unit) ->
        let value, setValue = createSignal 0

        let sign =
            memo {
                let v = value ()
                if v > 0 then "positive" elif v < 0 then "negative" else "zero"
            }

        let size =
            memo {
                match abs (value ()) with
                | 0 -> "none"
                | n when n < 10 -> "small"
                | _ -> "large"
            }

        { set = setValue
          sign = sign
          size = size
          dispose = dispose })

/// A chain of memo builders, the second reading the first.
type MemoChain =
    { set: Setter<int>
      total: Accessor<int>
      label: Accessor<string>
      labelRuns: unit -> int
      dispose: unit -> unit }

let makeMemoChain () : MemoChain =
    createRoot (fun (dispose: unit -> unit) ->
        let labelRuns = ref 0
        let items, setItems = createSignal 3

        let total = memo { items () * 5 }

        let label =
            memo {
                labelRuns.Value <- labelRuns.Value + 1
                $"total={total ()}"
            }

        { set = setItems
          total = total
          label = label
          labelRuns = fun () -> labelRuns.Value
          dispose = dispose })

/// `memo { let! v = source; return ... }`: the bind shape the builder exposes.
type MemoBind =
    { set: Setter<int>
      value: unit -> obj
      runs: unit -> int
      dispose: unit -> unit }

let makeMemoBind () : MemoBind =
    createRoot (fun (dispose: unit -> unit) ->
        let runs = ref 0
        let value, setValue = createSignal 2

        let doubled =
            memo {
                let! v = value
                runs.Value <- runs.Value + 1
                return v * 2
            }

        { set = setValue
          value = fun () -> box (doubled ())
          runs = fun () -> runs.Value
          dispose = dispose })

// ------------------------------------------------------------------ mount

/// `mount { }` (onSettled): runs after the first flush, once, untracked.
let makeMount () : Probe =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string> ()
        let value, setValue = createSignal 1
        let _, setOther = createSignal 0

        mount { log.Add $"mounted:{value ()}" }
        log.Add "body"

        probe setValue setOther log dispose)

/// Several statements in `mount { }`, including an `if ... then` without else in the middle.
let makeMountStatements () : Probe =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string> ()
        let value, setValue = createSignal 5
        let _, setOther = createSignal 0

        mount {
            log.Add "first"

            if value () > 2 then
                log.Add "guarded"

            log.Add "last"
        }

        probe setValue setOther log dispose)

/// A `match` followed by another statement in `mount { }`.
let makeMountMatch () : Probe =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string> ()
        let value, setValue = createSignal 1
        let _, setOther = createSignal 0

        mount {
            match value () with
            | 1 -> log.Add "one"
            | _ -> log.Add "other"

            log.Add "after-match"
        }

        probe setValue setOther log dispose)

// ---------------------------------------------------------------- cleanup

/// `cleanup { }` (onCleanup) on a root, several of them, plus one on a nested root.
let makeCleanup () : Probe =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string> ()
        let _, setValue = createSignal 0
        let _, setOther = createSignal 0

        cleanup { log.Add "first" }
        cleanup { log.Add "second" }

        createRoot (fun () -> cleanup { log.Add "child" })

        cleanup { log.Add "third" }

        probe setValue setOther log dispose)

/// Several statements in `cleanup { }`, including an `if ... then` without else in the middle.
let makeCleanupStatements () : Probe =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string> ()
        let value, setValue = createSignal 5
        let _, setOther = createSignal 0

        cleanup {
            log.Add "release-a"

            if untrack value > 2 then
                log.Add "release-guarded"

            log.Add "release-b"
        }

        probe setValue setOther log dispose)

/// A `match` followed by another statement in `cleanup { }`.
let makeCleanupMatch () : Probe =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string> ()
        let value, setValue = createSignal 1
        let _, setOther = createSignal 0

        cleanup {
            match untrack value with
            | 1 -> log.Add "one"
            | _ -> log.Add "other"

            log.Add "after-match"
        }

        probe setValue setOther log dispose)

/// `cleanup { }` inside the tracked compute of an `effect` source: runs before each re-compute.
let makeCleanupInCompute () : Probe =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string> ()
        let value, setValue = createSignal 1
        let _, setOther = createSignal 0

        effect {
            let! v =
                lambda {
                    let v = value ()
                    cleanup { log.Add $"compute-cleanup:{v}" }
                    v
                }

            log.Add $"effect:{v}"
        }

        probe setValue setOther log dispose)

// --------------------------------------------------------------- reaction

type ReactionProbe =
    { arm: unit -> unit
      set: Setter<int>
      setOther: Setter<int>
      log: ResizeArray<string>
      dispose: unit -> unit }

/// `reaction { }` (createReaction): fires once, on the first change after arming.
let makeReaction () : ReactionProbe =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string> ()
        let value, setValue = createSignal 0
        let other, setOther = createSignal 0

        let track = reaction { log.Add $"fired:{untrack value}:{untrack other}" }

        { arm = fun () -> track (fun () -> box (value ()))
          set = setValue
          setOther = setOther
          log = log
          dispose = dispose })

/// A reaction that re-arms itself from its own body, so it fires on every change.
let makeRearmingReaction () : ReactionProbe =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string> ()
        let value, setValue = createSignal 0
        let _, setOther = createSignal 0
        let mutable track: (unit -> objnull) -> unit = ignore
        let arm () = track (fun () -> box (value ()))

        track <-
            reaction {
                log.Add $"fired:{untrack value}"
                arm ()
            }

        { arm = arm
          set = setValue
          setOther = setOther
          log = log
          dispose = dispose })

/// Multiple statements inside `reaction { }`. (An `if ... then` followed by another statement does
/// not type-check here: CreateReactionBuilder.Zero returns unit but Combine expects `'T -> unit`.)
let makeReactionStatements () : ReactionProbe =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string> ()
        let value, setValue = createSignal 0
        let _, setOther = createSignal 0

        let track =
            reaction {
                log.Add "start"
                log.Add $"value:{untrack value}"
                log.Add "end"
            }

        { arm = fun () -> track (fun () -> box (value ()))
          set = setValue
          setOther = setOther
          log = log
          dispose = dispose })

// ----------------------------------------------------------------- lambda

type LambdaProbe =
    { set: Setter<int>
      read: unit -> int
      describe: unit -> string
      guarded: unit -> int
      calls: unit -> int
      dispose: unit -> unit }

/// `lambda { }` defers its body and re-evaluates on every call; mutable locals, match and a guard.
let makeLambda () : LambdaProbe =
    createRoot (fun (dispose: unit -> unit) ->
        let calls = ref 0
        let value, setValue = createSignal 1

        let read =
            lambda {
                calls.Value <- calls.Value + 1
                value () + 1
            }

        let describe =
            lambda {
                let mutable label = "?"

                match value () with
                | 1 -> label <- "one"
                | 2 -> label <- "two"
                | _ -> ()

                label
            }

        let guarded =
            lambda {
                if value () > 10 then
                    calls.Value <- calls.Value + 100

                value () * 3
            }

        { set = setValue
          read = read
          describe = describe
          guarded = guarded
          calls = fun () -> calls.Value
          dispose = dispose })

// --------------------------------------------------------------- children

type ChildrenProbe =
    { set: Setter<int>
      resolved: unit -> obj
      asArray: unit -> obj array
      runs: unit -> int
      dispose: unit -> unit }

/// `children { }` resolves nested arrays and thunks into a flat, memoised list.
let makeChildren () : ChildrenProbe =
    createRoot (fun (dispose: unit -> unit) ->
        let runs = ref 0
        let count, setCount = createSignal 2

        let resolved =
            children {
                runs.Value <- runs.Value + 1

                unbox<HtmlElement> [|
                    box "a"
                    box [| box "b"; box (fun () -> "c") |]
                    box (fun () -> $"n={count ()}")
                |]
            }

        { set = setCount
          resolved = fun () -> box (resolved.Invoke ())
          asArray = fun () -> unbox (resolved.toArray ())
          runs = fun () -> runs.Value
          dispose = dispose })

/// `children { }` over a single value: `toArray` wraps it.
let makeSingleChild () : ChildrenProbe =
    createRoot (fun (dispose: unit -> unit) ->
        let value, setValue = createSignal 1
        let resolved = children { unbox<HtmlElement> (fun () -> $"only:{value ()}") }

        { set = setValue
          resolved = fun () -> box (resolved.Invoke ())
          asArray = fun () -> unbox (resolved.toArray ())
          runs = fun () -> 0
          dispose = dispose })

// --------------------------------------------------------------- lazyload

[<SolidComponent>]
let LazyTarget () = span (class' = "lazy-target") { "loaded" }

let mutable lazyLoads = 0

/// `lazyload { }` (lazy): the loader returns a module-shaped object with a `default` component.
let LazyGreeting: LazyComponent<HtmlElement> =
    lazyload {
        lazyLoads <- lazyLoads + 1
        JS.Constructors.Promise.resolve (unbox<HtmlElement> (createObj [ "default" ==> LazyTarget ]))
    }

let getLazyLoads () = lazyLoads

// ------------------------------------------------------ inside components

/// The builders used together inside a component body, driven by a prop.
[<Erase>]
type BuilderHost() =
    inherit div()

    [<Erase>]
    member val value: int = unbox null with get, set

    [<Erase>]
    member val log: ResizeArray<string> = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let doubled = memo { props.value * 2 }

        effect {
            let! v = doubled
            props.log.Add $"effect:{v}"
        }

        mount { props.log.Add "mount" }
        cleanup { props.log.Add "cleanup" }

        div (class' = "host") { doubled () }
