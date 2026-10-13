module Partas.Solid.Tests.Runtime.Integration.FlowAdvanced.ListCases

open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop

type Item = { id: int; label: string }

/// Switch whose Match conditions overlap: the first truthy one wins.
[<Erase>]
type Grader() =
    inherit div()

    [<Erase>]
    member val score: int = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "grade") {
            Switch(fallback = span (class' = "f") { "F" }) {
                Match(when' = (props.score >= 90)) { span (class' = "a") { "A" } }
                Match(when' = (props.score >= 80)) { span (class' = "b") { "B" } }
                Match(when' = (props.score >= 70)) { span (class' = "c") { "C" } }
            }
        }

/// Switch without a fallback.
[<Erase>]
type NoFallbackSwitch() =
    inherit div()

    [<Erase>]
    member val mode: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "nfs") {
            Switch() {
                Match(when' = (props.mode = "x")) { span (class' = "x") { "X" } }
                Match(when' = (props.mode = "y")) { span (class' = "y") { "Y" } }
            }
        }

/// Match.Keyed / Match.NonKeyed callback children.
[<Erase>]
type SelectionView() =
    inherit div()

    [<Erase>]
    member val selected: Item option = unbox null with get, set

    [<Erase>]
    member val count: int = unbox null with get, set

    [<Erase>]
    member val onMount: string -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "sel") {
            Switch(fallback = span (class' = "none") { "none" }) {
                Match.Keyed(when'option = props.selected) {
                    yield
                        fun (it: Item) ->
                            props.onMount it.label
                            span (class' = "item") { it.label }
                }

                Match.NonKeyed(when' = props.count) {
                    yield fun (n: Accessor<int>) -> span (class' = "count") { "count " + string (n ()) }
                }
            }
        }

/// Tabs app: Switch driven by local state; each tab keeps a static child.
[<SolidComponent>]
let Tabs () =
    let tab, setTab = createSignal "home"

    div (class' = "tabs") {
        nav () {
            button (class' = "go-home", onClick = fun _ -> setTab "home") { "home" }
            button (class' = "go-settings", onClick = fun _ -> setTab "settings") { "settings" }
            button (class' = "go-missing", onClick = fun _ -> setTab "missing") { "missing" }
        }

        Switch(fallback = p (class' = "not-found") { "404" }) {
            Match(when' = (tab () = "home")) { section (class' = "home") { "home page" } }
            Match(when' = (tab () = "settings")) { section (class' = "settings") { input (class' = "setting") } }
        }
    }

/// Keyed For with an item fallback and index accessor (pure expression child).
[<Erase>]
type KeyedRows() =
    inherit ul()

    [<Erase>]
    member val items: Item[] = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        ul (class' = "keyed") {
            For.Keyed(each = props.items, fallback = li (class' = "empty") { "empty" }) {
                yield fun item index -> li (class' = "row", id = "row-" + string item.id) { item.label + "#" + string (index ()) }
            }
        }

/// For keyed by a key function: rows survive new object identities with equal keys.
[<Erase>]
type KeyFnRows() =
    inherit ul()

    [<Erase>]
    member val items: Item[] = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        ul (class' = "keyfn") {
            For.KeyedFn(each = props.items, keyed = (fun (i: Item) -> box i.id)) {
                yield fun item index -> li (class' = "row") { item().label + "#" + string (index ()) }
            }
        }

/// Non-keyed For with a fallback.
[<Erase>]
type SlotRows() =
    inherit ul()

    [<Erase>]
    member val items: string[] = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        ul (class' = "slots") {
            For.NonKeyed(each = props.items, fallback = li (class' = "empty") { "no slots" }) {
                yield fun item index -> li (class' = "row") { string index + "=" + item () }
            }
        }

/// Nested For: a grid of rows and cells.
[<Erase>]
type Grid() =
    inherit table()

    [<Erase>]
    member val rows: string[][] = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        table (class' = "grid") {
            tbody () {
                For.Keyed(each = props.rows) {
                    yield
                        fun row ri ->
                            tr () {
                                For.Keyed(each = row) {
                                    yield fun cell ci -> td () { string (ri ()) + "," + string (ci ()) + ":" + cell }
                                }
                            }
                }
            }
        }

/// Selectable list: Show inside each keyed row, selection held in local state.
[<SolidComponent>]
let Picker () =
    let items, setItems =
        createSignal [| { id = 1; label = "one" }; { id = 2; label = "two" }; { id = 3; label = "three" } |]

    let selected, setSelected = createSignal 0

    div (class' = "picker") {
        button (class' = "drop-first", onClick = fun _ -> setItems (Array.tail (items ()))) { "drop" }

        ul () {
            For.Keyed(each = items ()) {
                yield
                    fun item _ ->
                        li (class' = "opt", id = "opt-" + string item.id, onClick = fun _ -> setSelected item.id) {
                            item.label

                            Show(when' = (selected () = item.id)) { b (class' = "tick") { "*" } }
                        }
            }
        }

        span (class' = "current") { string (selected ()) }
    }

/// Repeat without a fallback.
[<Erase>]
type Dots() =
    inherit div()

    [<Erase>]
    member val count: int = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "dots") { Repeat(count = props.count) { yield fun n -> i (class' = "dot") { string n } } }

/// Repeat nested inside a keyed For (per-row rating stars).
[<Erase>]
type Ratings() =
    inherit ul()

    [<Erase>]
    member val items: Item[] = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        ul (class' = "ratings") {
            For.Keyed(each = props.items) {
                yield
                    fun item _ ->
                        li (class' = "rating") {
                            Repeat(count = item.id) { yield fun _ -> b () { "*" } }
                        }
            }
        }

/// Same as SelectionView, but Match.Keyed is typed over the option and uses the plain `when'` prop.
[<Erase>]
type SelectionViewPlain() =
    inherit div()

    [<Erase>]
    member val selected: Item option = unbox null with get, set

    [<Erase>]
    member val count: int = unbox null with get, set

    [<Erase>]
    member val onMount: string -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "sel") {
            Switch(fallback = span (class' = "none") { "none" }) {
                Match.Keyed(when' = props.selected) {
                    yield
                        fun (it: Item option) ->
                            props.onMount it.Value.label
                            span (class' = "item") { it.Value.label }
                }

                Match.NonKeyed(when' = props.count) {
                    yield fun (n: Accessor<int>) -> span (class' = "count") { "count " + string (n ()) }
                }
            }
        }
