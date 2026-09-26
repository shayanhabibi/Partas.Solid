module Partas.Solid.Tests.Runtime.Primitives.StoresDeep.DerivedDeep

open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop

// ---------------------------------------------------------------------------------------------
// fn-form createStore over another store, keyed projections over lists, the selection
// projection that replaces createSelector, and createOptimisticStore rollback via an action.
// ---------------------------------------------------------------------------------------------

type Task =
    { id: int
      mutable title: string
      mutable finished: bool
      mutable owner: string }

type Summary =
    { id: string
      mutable pending: int
      mutable closed: int
      mutable owners: ResizeArray<string> }

let task id title isDone owner =
    { id = id
      title = title
      finished = isDone
      owner = owner }

let private seedTasks () =
    ResizeArray [ task 1 "plan" false "ann"; task 2 "build" true "bob"; task 3 "ship" false "ann" ]

type SummaryHarness =
    { tasks: Store<ResizeArray<Task>>
      summary: Store<Summary>
      derives: unit -> int
      toggle: int -> unit
      retitle: int -> string -> unit
      add: int -> string -> string -> unit
      dispose: unit -> unit }

/// A derived store (fn-form createStore) that summarises a source store by mutating its draft.
let makeSummary () : SummaryHarness =
    createRoot (fun (dispose: unit -> unit) ->
        let tasks, setTasks = createStore (seedTasks ())
        let mutable derives = 0

        let summary, _ =
            createStore (
                (fun (d: Summary) ->
                    derives <- derives + 1
                    let ts = tasks.Value
                    d.pending <- ts |> Seq.filter (fun t -> not t.finished) |> Seq.length
                    d.closed <- ts |> Seq.filter (fun t -> t.finished) |> Seq.length
                    d.owners <- ResizeArray(ts |> Seq.map (fun t -> t.owner) |> Seq.distinct |> Seq.sort)
                    U2.Case1 None),
                { id = "summary"
                  pending = 0
                  closed = 0
                  owners = ResizeArray() }
            )

        createEffect ((fun (_: int option) -> summary.Value.pending), ignore)

        { tasks = tasks
          summary = summary.AsStore
          derives = fun () -> derives
          toggle =
            fun id ->
                setTasks (fun ts ->
                    for t in ts do
                        if t.id = id then
                            t.finished <- not t.finished

                    ts)
          retitle =
            fun id title ->
                setTasks (fun ts ->
                    for t in ts do
                        if t.id = id then
                            t.title <- title

                    ts)
          add =
            fun id title owner ->
                setTasks (fun ts ->
                    ts.Add(task id title false owner)
                    ts)
          dispose = dispose })

type View = { id: int; label: string }

type FilteredHarness =
    { view: Store<ResizeArray<View>>
      setOwner: string -> unit
      retitle: int -> string -> unit
      derives: unit -> int
      dispose: unit -> unit }

/// fn-form createStore over an array root that RETURNS a fresh list each time. Items are keyed by
/// "id" (ProjectionOptions key) so surviving rows keep their proxy identity across re-derives.
let makeFilteredView () : FilteredHarness =
    createRoot (fun (dispose: unit -> unit) ->
        let tasks, setTasks = createStore (seedTasks ())
        let owner, setOwner = createSignal "ann"
        let mutable derives = 0

        let view, _ =
            createStore (
                (fun (_: ResizeArray<View>) ->
                    derives <- derives + 1
                    let o = owner ()

                    tasks.Value
                    |> Seq.filter (fun t -> t.owner = o)
                    |> Seq.map (fun t -> { id = t.id; label = t.title })
                    |> ResizeArray
                    |> Some
                    |> U2.Case1),
                ResizeArray<View>(),
                ProjectionOptions<ResizeArray<View>>(key = U3.Case1 "id")
            )

        createEffect ((fun (_: int option) -> view.Value.Count), ignore)

        { view = view.AsStore
          setOwner = setOwner
          retitle =
            fun id title ->
                setTasks (fun ts ->
                    for t in ts do
                        if t.id = id then
                            t.title <- title

                    ts)
          derives = fun () -> derives
          dispose = dispose })

// ---------------------------------------------------------------------------------------------
// Selection projection (the Solid 2 replacement for createSelector).
// createProjection is declared to return (store * setter), but upstream returns the store itself
// (known binding bug), so the whole return is boxed and used as the store.
// ---------------------------------------------------------------------------------------------

type SelectionHarness =
    { isSelected: obj
      rowRuns: obj
      rowMemos: obj
      select: int -> unit
      dispose: unit -> unit }

let makeSelection (rowIds: int array) : SelectionHarness =
    createRoot (fun (dispose: unit -> unit) ->
        let selectedId, setSelected = createSignal 1
        let mutable prev: int option = None

        let isSelected: obj =
            box (
                createProjection (
                    (fun (d: obj) ->
                        let s = selectedId ()

                        match prev with
                        | Some p when p <> s -> d?(string p) <- false
                        | _ -> ()

                        d?(string s) <- true
                        prev <- Some s
                        U2.Case1 None),
                    createObj [],
                    ProjectionOptions<obj>()
                )
            )

        let rowRuns = createObj []
        let rowMemos = createObj []

        for id in rowIds do
            rowRuns?(string id) <- 0

            let m =
                createMemo (fun (_: bool option) ->
                    let current: int = rowRuns?(string id)
                    rowRuns?(string id) <- current + 1
                    let v: bool option = isSelected?(string id)
                    v = Some true)

            createEffect ((fun (_: bool option) -> m ()), ignore)
            rowMemos?(string id) <- m

        { isSelected = isSelected
          rowRuns = rowRuns
          rowMemos = rowMemos
          select = setSelected
          dispose = dispose })

// ---------------------------------------------------------------------------------------------
// createOptimisticStore rollback driven by an action. F# has no generator syntax; the generator
// protocol is authored by hand (same shape as StoresAsync/D-Async.fs).
// ---------------------------------------------------------------------------------------------

let private iterResult (isDone: bool) (value: obj) =
    createObj [ "done" ==> isDone; "value" ==> value ]

let private makeIterator (steps: (obj -> obj) list) (finish: obj -> obj) : obj =
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

type Settings = { mutable theme: string; mutable fontSize: int }

type Account =
    { id: int
      mutable name: string
      mutable settings: Settings }

type OptimisticAccountHarness =
    { server: Store<Account>
      view: Store<Account>
      log: ResizeArray<string>
      /// action: optimistically set theme + font, await server, then commit the server's theme.
      applyTheme: string -> JS.Promise<string> -> JS.Promise<unit>
      dispose: unit -> unit }

let makeOptimisticAccount () : OptimisticAccountHarness =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string>()

        let server, setServer =
            createStore
                { id = 1
                  name = "ann"
                  settings = { theme = "light"; fontSize = 12 } }

        let view, setView =
            createOptimisticStore<Account> (
                (fun (_: Account) ->
                    let s = server.Value

                    U3.Case1
                        { id = s.id
                          name = s.name
                          settings =
                            { theme = s.settings.theme
                              fontSize = s.settings.fontSize } }),
                { id = 1
                  name = ""
                  settings = { theme = ""; fontSize = 0 } }
            )

        createEffect (
            (fun (_: string option) -> view.Value.settings.theme + "/" + string view.Value.settings.fontSize),
            (fun (v: string) -> log.Add v)
        )

        let applyTheme (theme: string) : JS.Promise<string> -> JS.Promise<unit> =
            action (fun (reply: JS.Promise<string>) ->
                makeIterator
                    [ fun _ ->
                          setView (fun d ->
                              d.settings.theme <- theme
                              d.settings.fontSize <- 16
                              d)

                          box reply
                      fun confirmed ->
                          setServer (fun d ->
                              d.settings.theme <- unbox<string> confirmed
                              d)

                          null ]
                    (fun _ -> null))

        { server = server
          view = view.AsStore
          log = log
          applyTheme = applyTheme
          dispose = dispose })
