module Partas.Solid.Tests.Runtime.Primitives.Experimental.OwnerPrimitives

open Partas.Solid
open Fable.Core

// ------------------------------------------------------------- signals

/// Signal options through the ParamObject overload: a debug name and a case-insensitive comparator.
type NamedSignal =
    { value: Accessor<string>
      set: Setter<string>
      notifications: unit -> int
      dispose: unit -> unit }

let makeNamedSignal () : NamedSignal =
    createRoot (fun (dispose: unit -> unit) ->
        let notifications = ref 0

        let value, setValue =
            createSignal (
                "Hello",
                name = "greeting",
                equals = EqualityFunc (fun (prev: string) next -> prev.ToLower () = next.ToLower ())
            )

        createEffect ((fun _ -> value ()), (fun (_: string) -> notifications.Value <- notifications.Value + 1))

        { value = value
          set = setValue
          notifications = fun () -> notifications.Value
          dispose = dispose })

/// Function-form signal (writable derived) with a comparator through MixedSignalMemoOptions.
type DerivedClamp =
    { setSource: Setter<int>
      clamped: Accessor<int>
      setClamped: Setter<int>
      notifications: unit -> int
      dispose: unit -> unit }

let makeDerivedClamp () : DerivedClamp =
    createRoot (fun (dispose: unit -> unit) ->
        let notifications = ref 0
        let source, setSource = createSignal 5

        let clamped, setClamped =
            createSignal (
                (fun () -> min 10 (source ())),
                MixedSignalMemoOptions<int> (equals = EqualityFunc (fun prev next -> prev = next))
            )

        createEffect ((fun _ -> clamped ()), (fun (_: int) -> notifications.Value <- notifications.Value + 1))

        { setSource = setSource
          clamped = clamped
          setClamped = setClamped
          notifications = fun () -> notifications.Value
          dispose = dispose })

/// Function-form signal with named optional args (ParamObject overload).
let makeDerivedNamed () : DerivedClamp =
    createRoot (fun (dispose: unit -> unit) ->
        let notifications = ref 0
        let source, setSource = createSignal 5

        let clamped, setClamped =
            createSignal<int> ((fun () -> min 10 (source ())), name = "clamped")

        createEffect ((fun _ -> clamped ()), (fun (_: int) -> notifications.Value <- notifications.Value + 1))

        { setSource = setSource
          clamped = clamped
          setClamped = setClamped
          notifications = fun () -> notifications.Value
          dispose = dispose })

// ------------------------------------------------------------- owners

/// getOwner / Owner.ToRoot / isDisposed inside a root and inside a memo.
type OwnerShape =
    { rootIsRoot: bool
      memoOwnerIsRoot: Accessor<bool>
      rootDisposed: unit -> bool
      dispose: unit -> unit }

let makeOwnerShape () : OwnerShape =
    createRoot (fun (dispose: unit -> unit) ->
        let owner = getOwner ()

        let rootIsRoot =
            match owner with
            | Some o -> (o.ToRoot ()).IsSome
            | None -> false

        let memoOwnerIsRoot =
            createMemo (fun _ ->
                match getOwner () with
                | Some o -> (o.ToRoot ()).IsSome
                | None -> true)

        { rootIsRoot = rootIsRoot
          memoOwnerIsRoot = memoOwnerIsRoot
          rootDisposed = fun () -> owner.IsNone || isDisposed owner.Value
          dispose = dispose })

/// createOwner + runWithOwner + Root.dispose: an owner created imperatively and torn down later.
type ManualOwner =
    { set: Setter<int>
      log: ResizeArray<string>
      runResult: int
      disposeAll: unit -> unit
      disposeChildrenOnly: unit -> unit
      isDisposed: unit -> bool }

let makeManualOwner () : ManualOwner =
    let log = ResizeArray<string> ()
    let value, setValue = createSignal 1
    let owner = createOwner ()

    let runResult =
        runWithOwner (
            owner,
            fun () ->
                onCleanup (fun () -> log.Add "owner-cleanup")

                createRoot (fun () -> onCleanup (fun () -> log.Add "child-cleanup"))

                createEffect ((fun _ -> value ()), (fun (v: int) -> log.Add $"effect:{v}"))
                42
        )

    { set = setValue
      log = log
      runResult = runResult
      disposeAll = fun () -> owner.dispose None
      disposeChildrenOnly = fun () -> owner.dispose (Some false)
      isDisposed = fun () -> isDisposed owner }

/// runWithOwner from outside any owner (the spec calls `attachLater` after an await): work created
/// under the captured owner is owned by it.
type AsyncOwner =
    { set: Setter<int>
      log: ResizeArray<string>
      attachLater: unit -> string
      dispose: unit -> unit }

let makeAsyncOwner () : AsyncOwner =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string> ()
        let value, setValue = createSignal 1
        let owner = getOwner ()

        let attachLater () =
            let outside = getOwner().IsSome

            let label =
                runWithOwner (
                    owner,
                    fun () ->
                        onCleanup (fun () -> log.Add "late-cleanup")
                        createEffect ((fun _ -> value ()), (fun (v: int) -> log.Add $"late:{v}"))
                        $"inside:{getOwner().IsSome}"
                )

            $"outside:{outside} {label}"

        { set = setValue
          log = log
          attachLater = attachLater
          dispose = dispose })

/// Disposal order: children before the owner's own cleanups; own cleanups in registration order.
type Ordering =
    { log: ResizeArray<string>
      disposeFirstChild: unit -> unit
      dispose: unit -> unit }

let makeOrdering () : Ordering =
    let log = ResizeArray<string> ()

    createRoot (fun (dispose: unit -> unit) ->
        onCleanup (fun () -> log.Add "parent-1")

        let disposeFirst =
            createRoot (fun (d: unit -> unit) ->
                onCleanup (fun () -> log.Add "child-a")
                d)

        createRoot (fun () -> onCleanup (fun () -> log.Add "child-b"))
        onCleanup (fun () -> log.Add "parent-2")

        { log = log
          disposeFirstChild = disposeFirst
          dispose = dispose })

/// A child root's effect stops when the parent root is disposed.
type NestedEffect =
    { set: Setter<int>
      log: ResizeArray<string>
      disposeChild: unit -> unit
      dispose: unit -> unit }

let makeNestedEffect () : NestedEffect =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string> ()
        let value, setValue = createSignal 1
        createEffect ((fun _ -> value ()), (fun (v: int) -> log.Add $"parent:{v}"))

        let disposeChild =
            createRoot (fun (d: unit -> unit) ->
                createEffect ((fun _ -> value ()), (fun (v: int) -> log.Add $"child:{v}"))
                d)

        { set = setValue
          log = log
          disposeChild = disposeChild
          dispose = dispose })

// ------------------------------------------------------------- errors

/// createErrorBoundary as a primitive: the fallback receives the error accessor and a reset.
type Boundary =
    { set: Setter<int>
      result: Accessor<string>
      reset: unit -> unit
      dispose: unit -> unit }

let makeBoundary () : Boundary =
    createRoot (fun (dispose: unit -> unit) ->
        let value, setValue = createSignal 1
        let mutable resetFn: unit -> unit = ignore

        let result =
            createErrorBoundary (
                (fun () ->
                    let v = value ()

                    if v < 0 then
                        failwith $"bad:{v}"

                    $"ok:{v}"),
                System.Func<Accessor<objnull>, ResetFunc, string> (fun err reset ->
                    resetFn <- reset
                    $"error:{(unbox<exn> (err ())).Message}")
            )

        let asString = createMemo (fun _ -> unbox<string> (result ()))
        createEffect ((fun _ -> asString ()), (fun (_: string) -> ()))

        { set = setValue
          result = asString
          reset = fun () -> resetFn ()
          dispose = dispose })

// ------------------------------------------------------------- flush

/// flush(fn) batches writes and returns fn's result; untrack inside an effect's compute.
type FlushProbe =
    { log: ResizeArray<string>
      writeBoth: int -> int -> string
      setA: Setter<int>
      setPeek: Setter<int>
      dispose: unit -> unit }

let makeFlushProbe () : FlushProbe =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string> ()
        let a, setA = createSignal 1
        let b, setB = createSignal 2
        let peek, setPeek = createSignal 0

        createEffect ((fun _ -> $"{a ()}+{b ()}|{untrack peek}"), (fun (s: string) -> log.Add s))

        { log = log
          writeBoth =
            fun x y ->
                flush (fun () ->
                    setA x
                    setB y
                    $"wrote:{x},{y}")
          setA = setA
          setPeek = setPeek
          dispose = dispose })
