module Partas.Solid.Tests.Runtime.Primitives.StoresAsync.Async

open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop

// ---------------------------------------------------------------------------------------------
// Async memo (Promise compute), isPending, latest.
// ---------------------------------------------------------------------------------------------

type AsyncMemoHarness =
    { user: Accessor<string>
      setId: int -> unit
      /// Values the plain effect saw (it only runs on settled values).
      values: ResizeArray<string>
      /// isPending(user) as observed by an effect.
      pending: ResizeArray<bool>
      /// latest(user) as observed by an effect.
      latestValues: ResizeArray<string>
      loads: unit -> int
      dispose: unit -> unit }

let makeAsyncMemo (load: int -> JS.Promise<string>) : AsyncMemoHarness =
    createRoot (fun (dispose: unit -> unit) ->
        let id, setId = createSignal 1
        let mutable loads = 0
        let values = ResizeArray<string>()
        let pending = ResizeArray<bool>()
        let latestValues = ResizeArray<string>()

        let user =
            createMemo (fun (_: string option) ->
                loads <- loads + 1
                load (id ()))

        createEffect ((fun (_: string option) -> user ()), (fun (v: string) -> values.Add v))
        createEffect ((fun (_: bool option) -> isPending (fun () -> box (user ()))), (fun (v: bool) -> pending.Add v))
        createEffect ((fun (_: string option) -> latest (fun () -> user ())), (fun (v: string) -> latestValues.Add v))

        { user = user
          setId = setId
          values = values
          pending = pending
          latestValues = latestValues
          loads = fun () -> loads
          dispose = dispose })

/// createMemo(asyncFn, loadingValue): the ParamObject overload places `loadingValue` in options.
let makeAsyncMemoWithLoadingValue (load: unit -> JS.Promise<string>) =
    createRoot (fun (dispose: unit -> unit) ->
        let values = ResizeArray<string>()
        let m = createMemo ((fun (_: string) -> load ()), "loading…")
        createEffect ((fun (_: string option) -> m ()), (fun (v: string) -> values.Add v))

        {| values = values
           read = m
           dispose = dispose |})

// ---------------------------------------------------------------------------------------------
// until
// ---------------------------------------------------------------------------------------------

type UntilHarness =
    { count: Accessor<int>
      setCount: int -> unit
      /// until(count >= target)
      waitAtLeast: int -> JS.Promise<bool>
      /// until(count >= target, timeout) via the ParamObject overload
      waitWithTimeout: int -> int -> JS.Promise<bool>
      /// until via the UntilOptions pojo overload
      waitWithOptions: int -> int -> JS.Promise<bool>
      /// Like waitWithTimeout, but classifies the rejection in F# with a type test on TimeoutError.
      classify: int -> int -> JS.Promise<string>
      /// until with an abort signal
      waitWithSignal: int -> obj -> JS.Promise<bool>
      /// until called inside a tracking scope (a memo) — should throw in dev.
      untilInsideMemo: unit -> string
      dispose: unit -> unit }

let makeUntil () : UntilHarness =
    createRoot (fun (dispose: unit -> unit) ->
        let count, setCount = createSignal 0

        { count = count
          setCount = setCount
          waitAtLeast = fun target -> until (fun () -> count () >= target)
          waitWithTimeout = fun target ms -> until ((fun () -> count () >= target), timeout = ms)
          waitWithOptions = fun target ms -> until ((fun () -> count () >= target), UntilOptions(timeout = ms))
          classify =
            fun target ms ->
                until((fun () -> count () >= target), timeout = ms)
                    .``then``(
                        (fun (_: bool) -> "resolved"),
                        (fun (e: obj) ->
                            match e with
                            | :? TimeoutError as t -> "timeout:" + t.Message
                            | _ -> "other")
                    )
          waitWithSignal = fun target signal -> until ((fun () -> count () >= target), signal = signal)
          untilInsideMemo =
            fun () ->
                let m =
                    createRoot (fun () ->
                        createMemo (fun (_: string option) ->
                            try
                                until (fun () -> true) |> ignore
                                "no-throw"
                            with e ->
                                "threw:" + e.Message))

                m ()
          dispose = dispose })

// ---------------------------------------------------------------------------------------------
// refresh
// ---------------------------------------------------------------------------------------------

type RefreshHarness =
    { memo: Accessor<int>
      refreshMemo: unit -> JS.Promise<int>
      runs: unit -> int
      dispose: unit -> unit }

let makeRefreshSync () : RefreshHarness =
    createRoot (fun (dispose: unit -> unit) ->
        let mutable runs = 0

        let m =
            createMemo (fun (_: int option) ->
                runs <- runs + 1
                runs * 10)

        createEffect ((fun (_: int option) -> m ()), (fun (_: int) -> ()))

        { memo = m
          refreshMemo = fun () -> refresh m
          runs = fun () -> runs
          dispose = dispose })

let makeRefreshAsync (load: int -> JS.Promise<string>) =
    createRoot (fun (dispose: unit -> unit) ->
        let mutable calls = 0
        let values = ResizeArray<string>()

        let m =
            createMemo (fun (_: string option) ->
                calls <- calls + 1
                load calls)

        createEffect ((fun (_: string option) -> m ()), (fun (v: string) -> values.Add v))

        {| refreshIt = fun () -> refresh m
           calls = fun () -> calls
           values = values
           dispose = dispose |})

/// refresh() of something that is not a Solid source (a plain lambda).
let refreshPlainFunction () =
    try
        refresh (fun () -> 1) |> ignore
        "no-throw"
    with e ->
        e.Message

// ---------------------------------------------------------------------------------------------
// action: F# has no generator syntax, so the generator protocol (`next(v)` / `throw(e)`
// returning `{ done, value }`) is authored by hand as a pojo — the shape `action` drives.
// ---------------------------------------------------------------------------------------------

let private iterResult (isDone: bool) (value: obj) =
    createObj [ "done" ==> isDone; "value" ==> value ]

/// A generator-shaped iterator: each step receives the value sent in by next(v) and yields the
/// value it returns; after the last step, `finish` computes the return value.
let makeIterator (steps: (obj -> obj) list) (finish: obj -> obj) : obj =
    let steps = List.toArray steps
    let mutable i = 0

    createObj
        [ "next"
          ==> fun (v: obj) ->
              if i < steps.Length then
                  let step = steps[i]
                  i <- i + 1
                  iterResult false (step v)
              else
                  i <- i + 1
                  iterResult true (finish v)
          "throw" ==> fun (e: obj) -> raise (unbox<exn> e) ]

type LikeHarness =
    { likes: Accessor<int>
      optimistic: Accessor<int>
      /// log of "opt:n" / "likes:n" effect runs
      log: ResizeArray<string>
      /// action: optimistically +1, await the server, then write the authoritative count.
      like: JS.Promise<int> -> JS.Promise<string>
      /// Calls the action synchronously inside an owned scope (a createRoot body).
      likeInsideOwnedScope: unit -> string
      dispose: unit -> unit }

let makeLikeAction () : LikeHarness =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string>()
        let likes, setLikes = createSignal 10
        // NOTE: `createOptimistic (fun () -> likes ())` does not compile (FS0041): the value and
        // fn overloads are ambiguous for a lambda, so the type argument must be explicit.
        let optimistic, setOptimistic = createOptimistic<int> (fun () -> likes ())

        createEffect ((fun (_: int option) -> optimistic ()), (fun (v: int) -> log.Add $"opt:{v}"))
        createEffect ((fun (_: int option) -> likes ()), (fun (v: int) -> log.Add $"likes:{v}"))

        let like: JS.Promise<int> -> JS.Promise<string> =
            action (fun (server: JS.Promise<int>) ->
                makeIterator
                    [ fun _ ->
                          setOptimistic (optimistic () + 1)
                          box server
                      fun confirmed ->
                          setLikes (unbox<int> confirmed)
                          null ]
                    (fun _ -> box $"liked:{likes ()}"))

        { likes = likes
          optimistic = optimistic
          log = log
          like = like
          likeInsideOwnedScope =
            fun () ->
                createRoot (fun () ->
                    try
                        like (JS.Constructors.Promise.resolve 1) |> ignore
                        "no-throw"
                    with e ->
                        e.Message)
          dispose = dispose })

/// A synchronous action (all steps yield plain values) that returns the sum of what it was sent.
let syncSumAction: int -> JS.Promise<int> =
    action (fun (start: int) ->
        let mutable acc = start

        makeIterator
            [ fun _ -> box 1
              fun v ->
                  acc <- acc + unbox<int> v
                  box 2 ]
            (fun v -> box (acc + unbox<int> v)))

// ---------------------------------------------------------------------------------------------
// createOptimisticStore driven by an action (its intended use): the optimistic draft write is
// visible while the action is in flight, then either replaced by the authoritative write or
// reverted when the action fails.
// ---------------------------------------------------------------------------------------------

type Item = { id: int; mutable name: string }

type OptimisticListHarness =
    { items: Store<ResizeArray<Item>>
      optimisticItems: Store<ResizeArray<Item>>
      log: ResizeArray<int>
      /// action: optimistically append, await the server, then write the authoritative list.
      add: string -> JS.Promise<string> -> JS.Promise<unit>
      dispose: unit -> unit }

let makeOptimisticListAction () : OptimisticListHarness =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<int>()

        let items, setItems =
            createStore (ResizeArray [ { id = 1; name = "one" } ])

        let optimisticItems, setOptimistic =
            createOptimisticStore<ResizeArray<Item>> (
                (fun (_: ResizeArray<Item>) ->
                    U3.Case1(ResizeArray(items.Value |> Seq.map (fun i -> { id = i.id; name = i.name })))),
                ResizeArray<Item>()
            )

        createEffect ((fun (_: int option) -> optimisticItems.Value.Count), (fun (v: int) -> log.Add v))

        let add (name: string) : JS.Promise<string> -> JS.Promise<unit> =
            action (fun (server: JS.Promise<string>) ->
                makeIterator
                    [ fun _ ->
                          setOptimistic (fun d ->
                              d.Add { id = -1; name = name }
                              d)

                          box server
                      fun confirmed ->
                          setItems (fun d ->
                              d.Add { id = d.Count + 1; name = unbox<string> confirmed }
                              d)

                          null ]
                    (fun _ -> null))

        { items = items
          optimisticItems = optimisticItems.AsStore
          log = log
          add = add
          dispose = dispose })
