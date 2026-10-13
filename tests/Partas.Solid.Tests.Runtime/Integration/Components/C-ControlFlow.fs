module Partas.Solid.Tests.Runtime.Integration.Components.ControlFlow

open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop

type Todo = { id: int; title: string }

/// Keyed For: rows are keyed by item identity; each row reports its creation through `onRowCreated`.
[<Erase>]
type KeyedList() =
    inherit ul()

    [<Erase>]
    member val items: Todo[] = unbox null with get, set

    [<Erase>]
    member val onRowCreated: string -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        ul (class' = "keyed") {
            For.Keyed(each = props.items, fallback = li (class' = "empty") { "nothing" }) {
                yield
                    fun item index ->
                        props.onRowCreated item.title
                        li (class' = "row") { item.title + "@" + string (index ()) }
            }
        }

/// Keyed For whose row callback is a pure expression (no statements before the element).
[<Erase>]
type PureKeyedList() =
    inherit ul()

    [<Erase>]
    member val items: Todo[] = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        ul (class' = "keyed") {
            For.Keyed(each = props.items, fallback = li (class' = "empty") { "nothing" }) {
                yield fun item index -> li (class' = "row") { item.title + "@" + string (index ()) }
            }
        }

/// Keyed For whose row callback starts with a let binding.
[<Erase>]
type LetKeyedList() =
    inherit ul()

    [<Erase>]
    member val items: Todo[] = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        ul (class' = "keyed") {
            For.Keyed(each = props.items) {
                yield
                    fun item index ->
                        let label = item.title.ToUpper()
                        li (class' = "row") { label }
            }
        }

/// Non-keyed For whose row callback is a pure expression.
[<Erase>]
type PureIndexList() =
    inherit ul()

    [<Erase>]
    member val items: string[] = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        ul (class' = "nonkeyed") {
            For.NonKeyed(each = props.items) { yield fun item index -> li (class' = "row") { string index + ":" + item () } }
        }

/// Non-keyed For: rows are keyed by position; the item is an accessor.
[<Erase>]
type IndexList() =
    inherit ul()

    [<Erase>]
    member val items: string[] = unbox null with get, set

    [<Erase>]
    member val onRowCreated: int -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        ul (class' = "nonkeyed") {
            For.NonKeyed(each = props.items) {
                yield
                    fun item index ->
                        props.onRowCreated index
                        li (class' = "row") { string index + ":" + item () }
            }
        }

/// Real usage: a todo list that owns its state and edits it from buttons.
[<SolidComponent>]
let TodoApp () =
    let todos, setTodos =
        createSignal [| { id = 1; title = "a" }; { id = 2; title = "b" } |]

    let nextId, setNextId = createSignal 3

    div (class' = "todo-app") {
        button (
            class' = "add",
            onClick =
                fun _ ->
                    let id = nextId ()
                    setNextId (id + 1)
                    setTodos (Array.append (todos ()) [| { id = id; title = "t" + string id } |])
        ) {
            "add"
        }

        button (class' = "remove-first", onClick = fun _ -> setTodos (Array.tail (todos ()))) { "remove" }
        button (class' = "reverse", onClick = fun _ -> setTodos (Array.rev (todos ()))) { "reverse" }
        span (class' = "total") { Array.length (todos ()) }

        ul () {
            For.Keyed(each = todos ()) {
                yield fun todo _ -> li (class' = "todo", id = "todo-" + string todo.id) { todo.title }
            }
        }
    }

/// Repeat: count-driven list with an offset and a fallback.
[<Erase>]
type Stars() =
    inherit div()

    [<Erase>]
    member val count: int = unbox null with get, set

    [<Erase>]
    member val from: int = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "stars") {
            Repeat(count = props.count, from = props.from, fallback = em () { "no stars" }) {
                yield fun i -> b (class' = "star") { string i }
            }
        }

/// Show with a static child and a fallback.
[<Erase>]
type Gate() =
    inherit div()

    [<Erase>]
    member val isOpen: bool = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "gate") {
            Show(when' = props.isOpen, fallback = span (class' = "closed") { "closed" }) {
                span (class' = "open") { "open" }
            }
        }

/// Show.Keyed: the callback child receives the raw value and remounts when its identity changes.
[<Erase>]
type UserCard() =
    inherit div()

    [<Erase>]
    member val user: Todo option = unbox null with get, set

    [<Erase>]
    member val onMount: string -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "user") {
            Show.Keyed(when' = props.user, fallback = span (class' = "anon") { "anonymous" }) {
                yield
                    fun (u: Todo option) ->
                        let u = u.Value
                        props.onMount u.title
                        span (class' = "who") { u.title }
            }
        }

/// Show.NonKeyed: the callback child receives an accessor and is preserved across truthy values.
[<Erase>]
type LiveUserCard() =
    inherit div()

    [<Erase>]
    member val user: Todo option = unbox null with get, set

    [<Erase>]
    member val onMount: string -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "user") {
            Show.NonKeyed(when' = props.user) {
                yield
                    fun (u: Accessor<Todo option>) ->
                        props.onMount "mounted"
                        span (class' = "who") { u().Value.title }
            }
        }

/// Switch/Match with a fallback.
[<Erase>]
type TrafficLight() =
    inherit div()

    [<Erase>]
    member val state: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "light") {
            Switch(fallback = span (class' = "unknown") { "unknown" }) {
                Match(when' = (props.state = "red")) { span (class' = "red") { "stop" } }
                Match(when' = (props.state = "amber")) { span (class' = "amber") { "wait" } }
                Match(when' = (props.state = "green")) { span (class' = "green") { "go" } }
            }
        }

/// Errored: a child that throws when its prop crosses a threshold; the fallback exposes the error and a reset.
[<SolidComponent>]
let Bomb (value: Accessor<int>) =
    let checkedValue =
        createMemo (fun _ ->
            let v = value ()

            if v > 2 then
                failwith ("boom at " + string v)

            v)

    span (class' = "bomb") { checkedValue () }

[<Erase>]
type SafeZone() =
    inherit div()

    [<Erase>]
    member val value: Accessor<int> = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "zone") {
            Errored(
                fallbackFn =
                    ErrorBoundary.Fallback(fun err reset ->
                        div (class' = "error") {
                            span (class' = "message") { (err () :?> exn).Message }
                            button (class' = "reset", onClick = fun _ -> reset ())
                        })
            ) {
                Bomb props.value
            }
        }

/// A component whose render always throws.
[<SolidComponent>]
let Explode () =
    let v = createMemo (fun (_: string option) -> (failwith "always": string))
    span () { v () }

/// Same boundary, but setting the raw `fallback` U2 property instead of the `fallbackFn` helper.
[<Erase>]
type SafeZoneRaw() =
    inherit div()

    [<Erase>]
    member val value: Accessor<int> = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "zone") {
            Errored(
                fallback =
                    !^(ErrorBoundary.Fallback(fun err reset ->
                        div (class' = "error") {
                            span (class' = "message") { (err () :?> exn).Message }
                            button (class' = "reset", onClick = fun _ -> reset ())
                        }))
            ) {
                Bomb props.value
            }
        }

/// Errored with a static element fallback.
[<SolidComponent>]
let StaticErrorZone () =
    div (class' = "zone") {
        Errored(fallbackEle = span (class' = "error") { "static fallback" }) { Explode() }
    }

/// Loading around an async memo keyed by a prop.
[<Erase>]
type AsyncDetail() =
    inherit div()

    [<Erase>]
    member val id: int = unbox null with get, set

    [<Erase>]
    member val fetch: int -> JS.Promise<string> = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let detail = createMemo (fun (_: string option) -> props.fetch props.id)

        div (class' = "detail") {
            h3 (class' = "chrome") { "Detail" }

            Loading(fallback = span (class' = "spinner") { "loading" }) {
                span (class' = "content") { detail () }
            }
        }

/// Errored with a static element fallback through the raw `fallback` property.
[<SolidComponent>]
let StaticErrorZoneRaw () =
    div (class' = "zone") {
        Errored(fallback = !^(span (class' = "error") { "static fallback" })) { Explode() }
    }

/// Reveal coordinating two Loading boundaries.
[<Erase>]
type RevealPair() =
    inherit div()

    [<Erase>]
    member val first: unit -> JS.Promise<string> = unbox null with get, set

    [<Erase>]
    member val second: unit -> JS.Promise<string> = unbox null with get, set

    [<Erase>]
    member val order: Reveal.Order = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let a = createMemo (fun (_: string option) -> props.first ())
        let b = createMemo (fun (_: string option) -> props.second ())

        div (class' = "reveal") {
            Reveal(order = props.order) {
                Loading(fallback = span (class' = "fb-a") { "..." }) { span (class' = "a") { a () } }
                Loading(fallback = span (class' = "fb-b") { "..." }) { span (class' = "b") { b () } }
            }
        }
