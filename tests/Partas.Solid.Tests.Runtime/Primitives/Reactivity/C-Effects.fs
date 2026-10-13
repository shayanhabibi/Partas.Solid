module Partas.Solid.Tests.Runtime.Primitives.Reactivity.Effects

open Partas.Solid
open Fable.Core

/// A generic "signal + log" harness shared by the effect fixtures.
type Logged =
    { set: Setter<int>
      setOther: Setter<int>
      log: ResizeArray<string>
      dispose: unit -> unit }

/// Two-phase createEffect: compute (tracked) then effect (untracked) returning a cleanup.
let makeTwoPhase () : Logged =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string> ()
        let value, setValue = createSignal 1
        let other, setOther = createSignal 100

        createEffect (
            (fun _ ->
                let v = value ()
                log.Add $"compute:{v}"
                v),
            fun (v: int) ->
                // reads in the effect phase are not tracked: changing `other` must not re-run this effect
                log.Add $"effect:{v}:{other ()}"
                fun () -> log.Add $"cleanup:{v}"
        )

        { set = setValue
          setOther = setOther
          log = log
          dispose = dispose })

/// Compute receives the previous computed value (None on the first run).
let makeComputePrev () : Logged =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string> ()
        let value, setValue = createSignal 1
        let _, setOther = createSignal 0

        createEffect (
            (fun (prev: int option) ->
                log.Add (
                    match prev with
                    | Some p -> $"prev:{p}"
                    | None -> "prev:none"
                )

                value ()
                * 2),
            fun (v: int) -> log.Add $"effect:{v}"
        )

        { set = setValue
          setOther = setOther
          log = log
          dispose = dispose })

/// `{ effect, error }` bundle through the inline `createEffect(compute, effect, error: EffectErrorHandler)` overload:
/// compute throws for negative values; the error arm receives the thrown error and the effect's cleanup.
let makeErrorBundle () : Logged =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string> ()
        let value, setValue = createSignal 1
        let _, setOther = createSignal 0

        createEffect (
            (fun _ ->
                let v = value ()

                if v < 0 then
                    failwith $"negative:{v}"

                v),
            (fun (v: int) ->
                log.Add $"effect:{v}"
                fun () -> log.Add $"cleanup:{v}"),
            EffectErrorHandler (fun err cleanup ->
                log.Add $"error:{(unbox<exn> err).Message}"
                cleanup ())
        )

        { set = setValue
          setOther = setOther
          log = log
          dispose = dispose })

/// Same bundle through the `createEffect(compute, effect: 'T -> unit, error: obj -> unit)` ParamObject overload.
let makeErrorPojo () : Logged =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string> ()
        let value, setValue = createSignal 1
        let _, setOther = createSignal 0

        createEffect (
            (fun _ ->
                let v = value ()

                if v = 13 then
                    failwith "unlucky"

                v),
            effect = (fun (v: int) -> log.Add $"effect:{v}"),
            error = (fun (err: obj) -> log.Add $"error:{(unbox<exn> err).Message}")
        )

        { set = setValue
          setOther = setOther
          log = log
          dispose = dispose })

/// `defer = true`: the effect phase skips the initial value and first runs on the first change.
let makeDeferred () : Logged =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string> ()
        let value, setValue = createSignal 1
        let _, setOther = createSignal 0

        createEffect ((fun _ -> value ()), (fun (v: int) -> log.Add $"effect:{v}"), defer = true)

        { set = setValue
          setOther = setOther
          log = log
          dispose = dispose })

/// Render effect and user effect over the same signal: the render effect runs its first effect phase
/// synchronously at creation, and precedes user effects within a flush.
let makeRenderVsUser () : Logged =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string> ()
        let value, setValue = createSignal 1
        let _, setOther = createSignal 0

        createEffect ((fun _ -> value ()), (fun (v: int) -> log.Add $"user:{v}"))
        createRenderEffect ((fun _ -> value ()), (fun (v: int) -> log.Add $"render:{v}"))

        { set = setValue
          setOther = setOther
          log = log
          dispose = dispose })

/// Render effect returning a cleanup (inline `'T -> DisposalFunc` overload).
let makeRenderCleanup () : Logged =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string> ()
        let value, setValue = createSignal 1
        let _, setOther = createSignal 0

        createRenderEffect (
            (fun _ -> value ()),
            fun (v: int) ->
                log.Add $"render:{v}"
                fun () -> log.Add $"cleanup:{v}"
        )

        { set = setValue
          setOther = setOther
          log = log
          dispose = dispose })

/// createTrackedEffect: tracking and side effect in one function, with a returned cleanup.
/// Dynamic subscription: reads `other` only while `value` is even.
let makeTracked () : Logged =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string> ()
        let value, setValue = createSignal 1
        let other, setOther = createSignal 100

        createTrackedEffect (fun () ->
            let v = value ()

            if v % 2 = 0 then
                log.Add $"run:{v}:{other ()}"
            else
                log.Add $"run:{v}"

            fun () -> log.Add $"cleanup:{v}")

        { set = setValue
          setOther = setOther
          log = log
          dispose = dispose })

/// onSettled (with and without a returned cleanup) and onCleanup registered on the root.
let makeLifecycle () : Logged =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string> ()
        let value, setValue = createSignal 1
        let _, setOther = createSignal 0

        onCleanup (fun () -> log.Add "root-cleanup")
        onSettled (fun () -> log.Add "settled")

        onSettled (fun () ->
            log.Add "settled-with-cleanup"
            fun () -> log.Add "settled-cleanup")

        createEffect ((fun _ -> value ()), (fun (v: int) -> log.Add $"effect:{v}"))

        { set = setValue
          setOther = setOther
          log = log
          dispose = dispose })

/// onCleanup inside a memo body runs before every recomputation and on disposal.
let makeMemoCleanup () : Logged =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string> ()
        let value, setValue = createSignal 1
        let _, setOther = createSignal 0

        let memo =
            createMemo (fun _ ->
                let v = value ()
                log.Add $"memo:{v}"
                onCleanup (fun () -> log.Add $"memo-cleanup:{v}")
                v)

        createEffect ((fun _ -> memo ()), (fun (_: int) -> ()))

        { set = setValue
          setOther = setOther
          log = log
          dispose = dispose })

/// Several writes before a flush collapse into one compute + one effect with the final values.
type Batching =
    { setA: Setter<int>
      setB: Setter<int>
      log: ResizeArray<string>
      writeBothInFlush: int -> int -> int
      dispose: unit -> unit }

let makeBatching () : Batching =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string> ()
        let a, setA = createSignal 1
        let b, setB = createSignal 2

        createEffect (
            (fun _ ->
                a ()
                + b ()),
            (fun (v: int) -> log.Add $"sum:{v}")
        )

        { setA = setA
          setB = setB
          log = log
          writeBothInFlush =
            fun x y ->
                flush (fun () ->
                    setA x
                    setB y
                    x * y)
          dispose = dispose })
