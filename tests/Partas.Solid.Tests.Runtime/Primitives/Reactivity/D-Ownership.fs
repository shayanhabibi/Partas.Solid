module Partas.Solid.Tests.Runtime.Primitives.Reactivity.Ownership

open Partas.Solid
open Fable.Core

/// getOwner outside/inside a root, and runWithOwner re-entering a root later (e.g. from an async callback).
type OwnerProbe =
    {
        ownerOutside: bool
        ownerInside: bool
        /// Registers a cleanup on the captured root from outside any reactive scope.
        registerLater: string -> unit
        /// Creates an effect under the captured root from outside any reactive scope.
        effectLater: unit -> unit
        setValue: Setter<int>
        log: ResizeArray<string>
        dispose: unit -> unit
    }

let makeOwnerProbe () : OwnerProbe =
    let ownerOutside = getOwner().IsSome

    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string> ()
        let owner = getOwner ()
        let value, setValue = createSignal 1

        { ownerOutside = ownerOutside
          ownerInside = owner.IsSome
          registerLater =
            fun name ->
                runWithOwner (owner, (fun () -> onCleanup (fun () -> log.Add $"cleanup:{name}")))
                |> ignore
          effectLater = fun () -> runWithOwner (owner, fun () -> createEffect ((fun _ -> value ()), (fun (v: int) -> log.Add $"late-effect:{v}")))
          setValue = setValue
          log = log
          dispose = dispose })

/// Parent root with a nested child root; each registers a cleanup.
type NestedRoots =
    { disposeChild: unit -> unit
      disposeParent: unit -> unit
      log: ResizeArray<string> }

let makeNestedRoots () : NestedRoots =
    let log = ResizeArray<string> ()

    createRoot (fun (disposeParent: unit -> unit) ->
        onCleanup (fun () -> log.Add "parent")

        let disposeChild =
            createRoot (fun (disposeChild: unit -> unit) ->
                onCleanup (fun () -> log.Add "child")
                disposeChild)

        { disposeChild = disposeChild
          disposeParent = disposeParent
          log = log })

/// untrack: a memo depends on `tracked` but only peeks at `peeked`.
type Untracked =
    { setTracked: Setter<int>
      setPeeked: Setter<int>
      combined: Accessor<string>
      runs: unit -> int
      dispose: unit -> unit }

let makeUntracked () : Untracked =
    createRoot (fun (dispose: unit -> unit) ->
        let runs = ref 0
        let tracked, setTracked = createSignal 1
        let peeked, setPeeked = createSignal 10

        let combined =
            createMemo (fun _ ->
                runs.Value <-
                    runs.Value
                    + 1

                $"{tracked ()}/{untrack (fun () -> peeked ())}")

        createEffect ((fun _ -> combined ()), (fun (_: string) -> ()))

        { setTracked = setTracked
          setPeeked = setPeeked
          combined = combined
          runs = fun () -> runs.Value
          dispose = dispose })

/// createReaction: fires once per arm, the next change after `track`.
type Reaction =
    { arm: unit -> unit
      setA: Setter<int>
      setB: Setter<int>
      fired: unit -> int
      dispose: unit -> unit }

let makeReaction () : Reaction =
    createRoot (fun (dispose: unit -> unit) ->
        let fired = ref 0
        let a, setA = createSignal 0
        let b, setB = createSignal 0

        let track =
            createReaction (fun () ->
                fired.Value <-
                    fired.Value
                    + 1)

        { arm = fun () -> track (fun () -> box (a ()))
          setA = setA
          setB = setB
          fired = fun () -> fired.Value
          dispose = dispose })

/// createContext default value, and the default-less form failing through `tryUseContext`.
/// The error messages are surfaced so the spec can tell *which* upstream error was caught.
type ContextProbe =
    { withDefault: int
      withoutDefaultIsError: bool
      withoutDefaultMessage: string
      noOwnerIsError: bool
      noOwnerMessage: string }

let NumberContext = createContext 42
let RequiredContext: Context<string> = createContext<string> ()

let private errorMessage (result: Result<'T, ContextNotFoundError>) =
    match result with
    | Ok _ -> ""
    | Error e -> (unbox<exn> e).Message

let probeContext () : ContextProbe =
    let noOwner = tryUseContext NumberContext

    createRoot (fun () ->
        let required = tryUseContext RequiredContext

        { withDefault = useContext NumberContext
          withoutDefaultIsError = Result.isError required
          withoutDefaultMessage = errorMessage required
          noOwnerIsError = Result.isError noOwner
          noOwnerMessage = errorMessage noOwner })

/// getObserver is Some only while a computation is tracking; isDisposed reflects root disposal.
type ObserverProbe =
    { observerOutside: bool
      observerInRootBody: bool
      observerInMemo: Accessor<bool>
      observerInUntrack: Accessor<bool>
      rootDisposed: unit -> bool
      dispose: unit -> unit }

let makeObserverProbe () : ObserverProbe =
    let outside = getObserver().IsSome

    createRoot (fun (dispose: unit -> unit) ->
        let owner = (getOwner ()).Value
        let inBody = getObserver().IsSome
        let inMemo = createMemo (fun _ -> getObserver().IsSome)
        let inUntrack = createMemo (fun _ -> untrack (fun () -> getObserver().IsSome))

        { observerOutside = outside
          observerInRootBody = inBody
          observerInMemo = inMemo
          observerInUntrack = inUntrack
          rootDisposed = fun () -> isDisposed owner
          dispose = dispose })
