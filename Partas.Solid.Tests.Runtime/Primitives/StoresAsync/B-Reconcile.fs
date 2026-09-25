module Partas.Solid.Tests.Runtime.Primitives.StoresAsync.Reconcile

open Partas.Solid
open Fable.Core
open Partas.Solid.Tests.Runtime.Primitives.StoresAsync.Stores

// ---------------------------------------------------------------------------------------------
// reconcile: all three key overloads (default "id", named key, key function) and positional (null).
// ---------------------------------------------------------------------------------------------

type ReconcileHarness =
    { todos: Store<ResizeArray<Todo>>
      /// Log of the title of whatever sits at index 0 (per effect run).
      log: ResizeArray<string>
      byDefaultKey: ResizeArray<Todo> -> unit
      byNamedKey: ResizeArray<Todo> -> unit
      byKeyFn: ResizeArray<Todo> -> unit
      positional: ResizeArray<Todo> -> unit
      keyFnCalls: unit -> int
      dispose: unit -> unit }

let makeReconcileStore () : ReconcileHarness =
    createRoot (fun (dispose: unit -> unit) ->
        let log = ResizeArray<string>()
        let mutable keyFnCalls = 0

        let todos, setTodos =
            createStore (
                ResizeArray
                    [ { id = 1; title = "one"; completed = false }
                      { id = 2; title = "two"; completed = false } ]
            )

        createEffect ((fun (_: string option) -> todos.Value[0].title), (fun (v: string) -> log.Add v))

        { todos = todos
          log = log
          byDefaultKey = fun next -> setTodos (reconcile next)
          byNamedKey = fun next -> setTodos (reconcile (next, "id"))
          byKeyFn =
            fun next ->
                setTodos (
                    reconcile (
                        next,
                        fun (t: Todo) ->
                            keyFnCalls <- keyFnCalls + 1
                            box t.id
                    )
                )
          positional = fun next -> setTodos (reconcile (next, (null: string)))
          keyFnCalls = fun () -> keyFnCalls
          dispose = dispose })

/// Build a fresh (unwrapped) todo list for the spec to reconcile with.
let todo (id: int) (title: string) (completed: bool) =
    { id = id
      title = title
      completed = completed }

let todoList (items: Todo array) = ResizeArray items
