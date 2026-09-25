module Partas.Solid.Tests.Runtime.Integration.Apps.Todo

open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop

/// A todo item. Mutable fields so store drafts can be mutated in place by setters.
type Todo =
    { id: int
      mutable text: string
      mutable completed: bool }

[<StringEnum>]
type Filter =
    | All
    | Active
    | Completed

/// TodoMVC-style app: store-backed list, memo-derived counts, filters, keyed rows.
[<Erase>]
type TodoApp() =
    inherit div()

    /// Texts of the initial todos (all start active).
    [<Erase>]
    member val initial: string array = unbox null with get, set

    /// Called (from an effect) every time the "remaining" count settles on a new value.
    [<Erase>]
    member val onRemainingChange: int -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        props.initial <- [||]
        props.onRemainingChange <- ignore

        let seed =
            props.initial
            |> Array.mapi (fun i t ->
                { id = i + 1
                  text = t
                  completed = false })

        let nextId = ref (seed.Length + 1)
        let state, setState = createStore {| todos = seed |}
        let draft, setDraft = createSignal ""
        let filter, setFilter = createSignal All

        let remaining =
            createMemo (fun (_: int option) ->
                state.Value.todos
                |> Array.filter (fun t -> not t.completed)
                |> Array.length)

        let completedCount =
            createMemo (fun (_: int option) -> state.Value.todos.Length - remaining ())

        let visible =
            createMemo (fun (_: Todo array option) ->
                match filter () with
                | All -> state.Value.todos
                | Active -> state.Value.todos |> Array.filter (fun t -> not t.completed)
                | Completed -> state.Value.todos |> Array.filter (fun t -> t.completed))

        createEffect ((fun (_: int option) -> remaining ()), (fun (n: int) -> props.onRemainingChange n))

        let addTodo () =
            let text = draft().Trim ()

            if text <> "" then
                let id = nextId.Value
                nextId.Value <- id + 1

                setState (fun s ->
                    {| todos =
                        Array.append
                            s.todos
                            [| { id = id
                                 text = text
                                 completed = false } |] |})

            setDraft ""

        let toggle (id: int) =
            setState (fun s ->
                for t in s.todos do
                    if t.id = id then
                        t.completed <- not t.completed

                s)

        let toggleAll () =
            let target = remaining () > 0

            setState (fun s ->
                for t in s.todos do
                    t.completed <- target

                s)

        let remove (id: int) =
            setState (fun s -> {| todos = s.todos |> Array.filter (fun t -> t.id <> id) |})

        let clearCompleted () =
            setState (fun s -> {| todos = s.todos |> Array.filter (fun t -> not t.completed) |})

        div (class' = "todoapp") {
            form (
                class' = "add",
                onSubmit =
                    fun e ->
                        e.preventDefault ()
                        addTodo ()
            ) {
                input (
                    class' = "new-todo",
                    placeholder = "What needs to be done?",
                    value = draft (),
                    onInput = fun e -> setDraft (!!e.currentTarget?value)
                )

                button (type' = "submit", class' = "add-btn") { "Add" }
            }

            button (class' = "toggle-all", onClick = fun _ -> toggleAll ()) { "Toggle all" }

            ul (class' = "todo-list") {
                For.Keyed (each = visible (), fallback = li (class' = "empty") { "Nothing to show" }) {
                    yield
                        fun todo index ->
                            li(class' = (if todo.completed then "todo done" else "todo"))
                                .data ("id", string todo.id) {
                                input (
                                    type' = "checkbox",
                                    class' = "toggle",
                                    checked' = todo.completed,
                                    onChange = fun _ -> toggle todo.id
                                )

                                span (class' = "label") { todo.text }
                                span (class' = "pos") { index () + 1 }
                                button (class' = "remove", onClick = fun _ -> remove todo.id) { "x" }
                            }
                }
            }

            footer (class' = "footer") {
                span (class' = "count") {
                    $"""{remaining ()} {if remaining () = 1 then "item" else "items"} left"""
                }

                For.Keyed (each = [| All; Active; Completed |]) {
                    yield
                        fun f _ ->
                            button(class' = (if filter () = f then "filter selected" else "filter"), onClick = fun _ -> setFilter f)
                                .data ("filter", unbox<string> f) {
                                unbox<string> f
                            }
                }

                Show (when' = (completedCount () > 0)) {
                    button (class' = "clear", onClick = fun _ -> clearCompleted ()) {
                        $"Clear completed ({completedCount ()})"
                    }
                }
            }
        }
