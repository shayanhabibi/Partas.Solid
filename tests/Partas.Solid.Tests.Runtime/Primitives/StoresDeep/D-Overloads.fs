module Partas.Solid.Tests.Runtime.Primitives.StoresDeep.Overloads

open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop

// ---------------------------------------------------------------------------------------------
// Immutable F# shapes in a store: anonymous records (copy-update only) and F# lists.
// ---------------------------------------------------------------------------------------------

type Prefs = {| theme: string; size: int; tags: string list |}

let makeAnonStore () =
    createRoot (fun (dispose: unit -> unit) ->
        let prefs, setPrefs =
            createStore<Prefs> {| theme = "light"; size = 12; tags = [ "a"; "b" ] |}

        let mutable themeRuns = 0

        let theme =
            createMemo (fun (_: string option) ->
                themeRuns <- themeRuns + 1
                prefs.Value.theme)

        let tagText =
            createMemo (fun (_: string option) -> prefs.Value.tags |> List.map (fun t -> t.ToUpper()) |> String.concat "|")

        createEffect ((fun (_: string option) -> theme ()), ignore)
        createEffect ((fun (_: string option) -> tagText ()), ignore)

        {| theme = theme
           tagText = tagText
           themeRuns = fun () -> themeRuns
           setSize = fun (n: int) -> setPrefs (fun p -> {| p with size = n |})
           setTheme = fun (t: string) -> setPrefs (fun p -> {| p with theme = t |})
           consTag = fun (t: string) -> setPrefs (fun p -> {| p with tags = t :: p.tags |})
           dropHead = fun () -> setPrefs (fun p -> {| p with tags = List.tail p.tags |})
           size = fun () -> prefs.Value.size
           dispose = dispose |})

// ---------------------------------------------------------------------------------------------
// Key overloads. Upstream calls the key function once PER ITEM of an array (ProjectionOptions.key:
// `(item) => any`), but the F# bindings type it as `'T -> objnull` over the whole store value.
// For an array root that forces an unbox; the runtime behaviour is checked here.
// ---------------------------------------------------------------------------------------------

type Row = { id: int; label: string }

type KeyedHarness =
    { rows: Store<ResizeArray<Row>>
      optRows: Store<ResizeArray<Row>>
      keyCalls: unit -> int
      setSource: Row array -> unit
      dispose: unit -> unit }

let makeKeyed () : KeyedHarness =
    createRoot (fun (dispose: unit -> unit) ->
        let source, setSource = createSignal [| { id = 1; label = "one" }; { id = 2; label = "two" } |]
        let mutable keyCalls = 0

        // `key: 'T -> objnull` with 'T = ResizeArray<Row>; at runtime the argument is a Row.
        let perItemKey (x: ResizeArray<Row>) : objnull =
            keyCalls <- keyCalls + 1
            box (unbox<Row> x).id

        let rows, _ =
            createStore (
                (fun (_: ResizeArray<Row>) -> U2.Case1(Some(ResizeArray(source ())))),
                ResizeArray<Row>(),
                key = perItemKey
            )

        let optRows, _ =
            createOptimisticStore (
                (fun (_: ResizeArray<Row>) -> U3.Case1(ResizeArray(source ()))),
                ResizeArray<Row>(),
                key = perItemKey
            )

        createEffect ((fun (_: int option) -> rows.Value.Count + optRows.Value.Count), ignore)

        { rows = rows.AsStore
          optRows = optRows.AsStore
          keyCalls = fun () -> keyCalls
          setSource = setSource
          dispose = dispose })

/// Plain-value createOptimisticStore with a record that happens to carry an `id` (required by the
/// binding's SRTP constraint on this overload; upstream does not need it).
type Draft = { id: string; mutable text: string }

let makePlainOptimistic () =
    createRoot (fun (dispose: unit -> unit) ->
        let draft, setDraft = createOptimisticStore { id = "d"; text = "saved" }
        let log = ResizeArray<string>()
        createEffect ((fun (_: string option) -> draft.Value.text), (fun (v: string) -> log.Add v))

        {| draft = draft.Value
           log = log
           edit =
            fun (t: string) ->
                setDraft (fun d ->
                    d.text <- t
                    d)
           dispose = dispose |})

// ---------------------------------------------------------------------------------------------
// snapshot of a derived store, and deep() over a derived store.
// ---------------------------------------------------------------------------------------------

type Tally = { id: string; mutable counts: ResizeArray<int> }

let makeTally () =
    createRoot (fun (dispose: unit -> unit) ->
        let n, setN = createSignal 2
        let mutable deepRuns = 0

        let tally, _ =
            createStore (
                (fun (d: Tally) ->
                    d.counts <- ResizeArray [ 1 .. n () ]
                    U2.Case1 None),
                { id = "tally"; counts = ResizeArray() }
            )

        createEffect ((fun (_: Tally option) -> deep tally.AsStore), (fun (_: Tally) -> deepRuns <- deepRuns + 1))

        {| snap = fun () -> snapshot tally.AsStore
           live = tally.Value
           setN = setN
           deepRuns = fun () -> deepRuns
           dispose = dispose |})
