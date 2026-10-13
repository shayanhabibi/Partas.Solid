module Partas.Solid.Tests.Runtime.Primitives.StoresAsync.Stores

open Partas.Solid
open Fable.Core

// ---------------------------------------------------------------------------------------------
// Plain createStore: object, nested, arrays, draft mutation and returned replacements.
// Records compile to Fable classes; Solid 2 wraps user class instances (they stringify as
// [object Object]), so an F# record with mutable fields is the idiomatic draft shape.
// ---------------------------------------------------------------------------------------------

type Address = { mutable city: string; mutable zip: string }
type User = { mutable name: string; mutable address: Address }
type Todo = { id: int; mutable title: string; mutable completed: bool }

type AppState =
    { mutable count: int
      mutable user: User
      mutable todos: ResizeArray<Todo> }

type AppHarness =
    { state: Store<AppState>
      setState: StoreSetter<AppState>
      /// Effect log, in effect-run order.
      log: ResizeArray<string>
      increment: unit -> unit
      /// Three writes to the same key inside one updater (the draft sees its own writes).
      incrementThrice: unit -> unit
      rename: string -> unit
      moveCity: string -> unit
      /// Replace the whole nested user object through the draft.
      replaceUser: string -> string -> unit
      addTodo: string -> unit
      toggle: int -> unit
      retitle: int -> string -> unit
      /// The canonical filter idiom: assign a filtered array to the draft.
      removeCompleted: unit -> unit
      /// In-place splice on the draft array.
      removeFirst: unit -> unit
      /// Updater that returns a brand-new record (adoption of the returned object).
      replaceState: unit -> unit
      /// Updater that returns a record copy of the draft ({ s with ... }).
      copyWithCount: int -> unit
      /// Snapshot of the store (plain, unwrapped).
      snap: unit -> AppState
      dispose: unit -> unit }

let initialState () =
    { count = 0
      user =
        { name = "Ada"
          address = { city = "London"; zip = "N1" } }
      todos =
        ResizeArray
            [ { id = 1; title = "write tests"; completed = false }
              { id = 2; title = "run tests"; completed = true } ] }

let makeAppStore () : AppHarness =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string>()
        let state, setState = createStore (initialState ())

        createEffect ((fun (_: int option) -> state.Value.count), (fun (v: int) -> log.Add $"count:{v}"))
        createEffect ((fun (_: string option) -> state.Value.user.name), (fun (v: string) -> log.Add $"name:{v}"))
        createEffect (
            (fun (_: string option) -> state.Value.user.address.city),
            (fun (v: string) -> log.Add $"city:{v}")
        )
        createEffect ((fun (_: int option) -> state.Value.todos.Count), (fun (v: int) -> log.Add $"len:{v}"))
        createEffect (
            (fun (_: int option) ->
                state.Value.todos
                |> Seq.sumBy (fun t -> if t.completed then 1 else 0)),
            (fun (v: int) -> log.Add $"done:{v}")
        )

        let mutable nextId = 3

        { state = state
          setState = setState
          log = log
          increment =
            fun () ->
                setState (fun s ->
                    s.count <- s.count + 1
                    s)
          incrementThrice =
            fun () ->
                setState (fun s ->
                    s.count <- s.count + 1
                    s.count <- s.count + 1
                    s.count <- s.count + 1
                    s)
          rename =
            fun name ->
                setState (fun s ->
                    s.user.name <- name
                    s)
          moveCity =
            fun city ->
                setState (fun s ->
                    s.user.address.city <- city
                    s)
          replaceUser =
            fun name city ->
                setState (fun s ->
                    s.user <-
                        { name = name
                          address = { city = city; zip = "?" } }

                    s)
          addTodo =
            fun title ->
                let id = nextId
                nextId <- nextId + 1

                setState (fun s ->
                    s.todos.Add { id = id; title = title; completed = false }
                    s)
          toggle =
            fun id ->
                setState (fun s ->
                    for t in s.todos do
                        if t.id = id then
                            t.completed <- not t.completed

                    s)
          retitle =
            fun id title ->
                setState (fun s ->
                    for t in s.todos do
                        if t.id = id then
                            t.title <- title

                    s)
          removeCompleted =
            fun () ->
                setState (fun s ->
                    s.todos <- ResizeArray(s.todos |> Seq.filter (fun t -> not t.completed))
                    s)
          removeFirst =
            fun () ->
                setState (fun s ->
                    s.todos.RemoveAt 0
                    s)
          replaceState =
            fun () ->
                setState (fun _ ->
                    { count = 100
                      user =
                        { name = "Zed"
                          address = { city = "Nowhere"; zip = "0" } }
                      todos = ResizeArray() })
          copyWithCount = fun n -> setState (fun s -> { s with count = n })
          snap = fun () -> snapshot state
          dispose = dispose })

// ---------------------------------------------------------------------------------------------
// Store option overloads: `name` / `shallow` (ParamObject) and the StoreOptions pojo.
// ---------------------------------------------------------------------------------------------

type Row = { mutable label: string }
type Table = { mutable rows: ResizeArray<Row> }

type ShallowHarness =
    { table: Store<Table>
      /// Mutate a nested row in place through the draft.
      mutateRowInPlace: string -> unit
      /// Replace the root-level `rows` key with a new array.
      replaceRows: string -> unit
      log: ResizeArray<string>
      dispose: unit -> unit }

let makeShallowStore (shallow: bool) : ShallowHarness =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string>()

        let table, setTable =
            createStore ({ rows = ResizeArray [ { label = "a" } ] }, name = "table", shallow = shallow)

        createEffect ((fun (_: string option) -> table.Value.rows[0].label), (fun (v: string) -> log.Add v))

        { table = table
          mutateRowInPlace =
            fun label ->
                setTable (fun t ->
                    t.rows[0].label <- label
                    t)
          replaceRows =
            fun label ->
                setTable (fun t ->
                    t.rows <- ResizeArray [ { label = label } ]
                    t)
          log = log
          dispose = dispose })

/// The StoreOptions overload: returns the store proxy so the spec can inspect it.
let makeNamedStore () =
    let s, _ = createStore ({ label = "named" }, StoreOptions(name = "named-store"))
    s.Value

/// createStore(Store<'T>): wrapping an existing store returns that store's proxy.
/// NOTE: `createStore first` does not compile (FS0041): the `store: 'T` and `store: Store<'T>`
/// overloads are ambiguous for a Store argument, so the type argument must be explicit.
let wrapExistingStore () =
    let first, _ = createStore { label = "x" }
    let second, _ = createStore<Row> first
    first.Value, second.Value

// ---------------------------------------------------------------------------------------------
// deep(): an effect over the whole tree.
// ---------------------------------------------------------------------------------------------

type DeepHarness =
    { runs: unit -> int
      moveCity: string -> unit
      increment: unit -> unit
      dispose: unit -> unit }

let makeDeepStore () : DeepHarness =
    createRoot (fun (dispose: unit -> unit) ->
        let mutable runs = 0
        let state, setState = createStore (initialState ())

        createEffect (
            (fun (_: AppState option) -> deep state),
            (fun (_: AppState) -> runs <- runs + 1)
        )

        { runs = fun () -> runs
          moveCity =
            fun city ->
                setState (fun s ->
                    s.user.address.city <- city
                    s)
          increment =
            fun () ->
                setState (fun s ->
                    s.count <- s.count + 1
                    s)
          dispose = dispose })

/// Without deep(), an effect reading only the root reference never reruns on nested writes.
let makeShallowReadStore () : DeepHarness =
    createRoot (fun (dispose: unit -> unit) ->
        let mutable runs = 0
        let state, setState = createStore (initialState ())

        createEffect ((fun (_: User option) -> state.Value.user), (fun (_: User) -> runs <- runs + 1))

        { runs = fun () -> runs
          moveCity =
            fun city ->
                setState (fun s ->
                    s.user.address.city <- city
                    s)
          increment =
            fun () ->
                setState (fun s ->
                    s.count <- s.count + 1
                    s)
          dispose = dispose })
