module Partas.Solid.Tests.Runtime.Primitives.StoresAsync.Derived

open Partas.Solid
open Fable.Core

// ---------------------------------------------------------------------------------------------
// fn-form createStore(fn, seed): a writable derived store.
// ---------------------------------------------------------------------------------------------

type Filtered =
    { id: string
      mutable items: ResizeArray<int>
      mutable total: int }

type DerivedHarness =
    { derived: RefreshableStore<Filtered>
      setDerived: StoreSetter<Filtered>
      setThreshold: int -> unit
      runs: unit -> int
      /// Manual write through the derived store's own setter.
      overrideTotal: int -> unit
      /// refresh() on the derived store; resolves with the store.
      refreshIt: unit -> JS.Promise<Filtered>
      dispose: unit -> unit }

let makeDerivedStore (source: int array) : DerivedHarness =
    createRoot (fun (dispose: unit -> unit) ->
        let threshold, setThreshold = createSignal 2
        let mutable runs = 0

        let derived, setDerived =
            createStore (
                (fun (d: Filtered) ->
                    runs <- runs + 1
                    let t = threshold ()
                    d.items <- ResizeArray(source |> Array.filter (fun x -> x > t))
                    d.total <- d.items |> Seq.sum
                    U2.Case1 None),
                { id = "filtered"
                  items = ResizeArray()
                  total = 0 }
            )

        { derived = derived
          setDerived = setDerived
          setThreshold = setThreshold
          runs = fun () -> runs
          overrideTotal =
            fun n ->
                setDerived (fun d ->
                    d.total <- n
                    d)
          refreshIt = fun () -> refresh derived.AsRefreshable
          dispose = dispose })

/// fn-form createStore returning a NEW value (Some) instead of mutating the draft.
let makeDerivedReturning () =
    createRoot (fun (dispose: unit -> unit) ->
        let n, setN = createSignal 1

        let derived, _ =
            createStore (
                (fun (_: Filtered) ->
                    let v = n ()

                    U2.Case1(
                        Some
                            { id = "ret"
                              items = ResizeArray [ v; v * 2 ]
                              total = v * 3 }
                    )),
                { id = "ret"
                  items = ResizeArray()
                  total = 0 }
            )

        {| derived = derived.Value
           setN = setN
           dispose = dispose |})

/// fn-form createStore with an async derive: returns a promise of the next state.
let makeAsyncDerived (load: int -> JS.Promise<int>) =
    createRoot (fun (dispose: unit -> unit) ->
        let n, setN = createSignal 1
        let log = ResizeArray<int>()

        let derived, _ =
            createStore (
                (fun (_: Filtered) ->
                    let v = n ()

                    load(v)
                        .``then``(fun (total: int) ->
                            Some
                                { id = "async"
                                  items = ResizeArray [ v ]
                                  total = total })
                    |> U2.Case2),
                { id = "async"
                  items = ResizeArray()
                  total = -1 }
            )

        createEffect ((fun (_: int option) -> derived.Value.total), (fun (v: int) -> log.Add v))

        {| derived = derived.Value
           setN = setN
           log = log
           dispose = dispose |})

// ---------------------------------------------------------------------------------------------
// createProjection: selection projection with per-key granularity.
// ---------------------------------------------------------------------------------------------

type Selection =
    { id: string
      mutable a: bool
      mutable b: bool
      mutable c: bool }

let private selectionSeed () =
    { id = "sel"
      a = false
      b = false
      c = false }

let private project (selected: Accessor<string>) (d: Selection) =
    let s = selected ()
    d.a <- (s = "a")
    d.b <- (s = "b")
    d.c <- (s = "c")
    U2.Case1 None

/// Idiomatic use of the binding: destructure the declared `RefreshableStoreReturn` tuple.
let makeProjectionIdiomatic () =
    createRoot (fun (dispose: unit -> unit) ->
        let selected, setSelected = createSignal "a"
        let sel, _ = createProjection (project selected, selectionSeed ())

        {| readA = fun () -> sel.Value.a
           setSelected = setSelected
           dispose = dispose |})

type ProjectionHarness =
    { raw: obj
      setSelected: string -> unit
      runsA: unit -> int
      runsB: unit -> int
      runsC: unit -> int
      memoA: Accessor<bool>
      memoB: Accessor<bool>
      memoC: Accessor<bool>
      dispose: unit -> unit }

/// Same projection, but the return value is kept whole (boxed) so the runtime shape can be
/// inspected and used regardless of the declared tuple type.
let makeProjectionRaw () : ProjectionHarness =
    createRoot (fun (dispose: unit -> unit) ->
        let selected, setSelected = createSignal "a"
        let raw: obj = box (createProjection (project selected, selectionSeed ()))
        let sel: Selection = unbox raw
        let mutable runsA = 0
        let mutable runsB = 0
        let mutable runsC = 0

        let memoA =
            createMemo (fun (_: bool option) ->
                runsA <- runsA + 1
                sel.a)

        let memoB =
            createMemo (fun (_: bool option) ->
                runsB <- runsB + 1
                sel.b)

        let memoC =
            createMemo (fun (_: bool option) ->
                runsC <- runsC + 1
                sel.c)

        { raw = raw
          setSelected = setSelected
          runsA = fun () -> runsA
          runsB = fun () -> runsB
          runsC = fun () -> runsC
          memoA = memoA
          memoB = memoB
          memoC = memoC
          dispose = dispose })

// ---------------------------------------------------------------------------------------------
// createOptimistic / createOptimisticStore.
// ---------------------------------------------------------------------------------------------

type OptimisticHarness =
    { value: Accessor<int>
      setValue: Setter<int>
      log: ResizeArray<int>
      dispose: unit -> unit }

let makeOptimisticSignal () : OptimisticHarness =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<int>()
        let value, setValue = createOptimistic 1
        createEffect ((fun (_: int option) -> value ()), (fun (v: int) -> log.Add v))

        { value = value
          setValue = setValue
          log = log
          dispose = dispose })

type Profile = { id: int; mutable name: string }

type OptimisticStoreHarness =
    { profile: Store<Profile>
      rename: string -> unit
      log: ResizeArray<string>
      dispose: unit -> unit }

let makeOptimisticStore () : OptimisticStoreHarness =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string>()
        let profile, setProfile = createOptimisticStore { id = 1; name = "John" }
        createEffect ((fun (_: string option) -> profile.Value.name), (fun (v: string) -> log.Add v))

        { profile = profile
          rename =
            fun name ->
                setProfile (fun p ->
                    p.name <- name
                    p)
          log = log
          dispose = dispose })
