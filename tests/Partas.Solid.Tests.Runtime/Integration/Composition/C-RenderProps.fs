module Partas.Solid.Tests.Runtime.Integration.Composition.RenderProps

open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop

/// Render prop as a named prop: the component owns the state and hands a value to `render`.
[<Erase>]
type Counter() =
    inherit div()

    [<Erase>]
    member val start: int = unbox null with get, set

    [<Erase>]
    member val render: Accessor<int> -> HtmlElement = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let count, setCount = createSignal props.start

        div (class' = "counter") {
            button (class' = "inc", onClick = fun _ -> setCount (count () + 1)) { "+" }
            props.render count
        }

/// Function-as-children: the component calls `props.children` with an accessor.
[<Erase>]
type Toggle() =
    inherit div()
    interface ChildLambdaProvider<Accessor<bool>>

    [<SolidTypeComponent>]
    member props.View =
        let on, setOn = createSignal false
        let renderChild: Accessor<bool> -> HtmlElement = unbox props.children

        div (class' = "toggle") {
            button (class' = "flip", onClick = fun _ -> setOn (not (on ()))) { "flip" }
            renderChild on
        }

/// Function-as-children receiving two arguments (value + setter).
[<Erase>]
type Stateful() =
    inherit div()
    interface ChildLambdaProvider2<Accessor<string>, (string -> unit)>

    [<Erase>]
    member val initial: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let value, setValue = createSignal props.initial
        let renderChild: System.Func<Accessor<string>, (string -> unit), HtmlElement> = unbox props.children
        div (class' = "stateful") { renderChild.Invoke(value, (fun v -> setValue v)) }

/// A list component whose rows are rendered by a render prop.
[<Erase>]
type ListOf() =
    inherit ul()

    [<Erase>]
    member val items: string[] = unbox null with get, set

    [<Erase>]
    member val renderItem: string -> int -> HtmlElement = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        ul (class' = "list-of") {
            For.Keyed(each = props.items) { yield fun item index -> li (class' = "row") { props.renderItem item (index ()) } }
        }

[<SolidComponent>]
let CounterHost () =
    Counter(start = 5, render = fun c -> span (class' = "shown") { "value=" + string (c ()) })

[<SolidComponent>]
let ToggleHost () =
    Toggle() {
        yield fun on -> span (class' = "state") { if on () then "ON" else "OFF" }
    }

[<SolidComponent>]
let StatefulHost () =
    Stateful(initial = "a") {
        yield
            fun value setValue ->
                Fragment() {
                    span (class' = "value") { value () }
                    button (class' = "set-b", onClick = fun _ -> setValue "b") { "b" }
                }
    }

[<SolidComponent>]
let ListOfHost () =
    let items, setItems = createSignal [| "x"; "y" |]

    div (class' = "listof-host") {
        button (class' = "add", onClick = fun _ -> setItems (Array.append (items ()) [| "z" |])) { "add" }
        ListOf(items = items (), renderItem = fun item i -> b (class' = "item") { string i + "-" + item })
    }
