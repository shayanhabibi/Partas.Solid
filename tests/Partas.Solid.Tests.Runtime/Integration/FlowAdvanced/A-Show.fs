module Partas.Solid.Tests.Runtime.Integration.FlowAdvanced.ShowCases

open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop

type Person = { id: int; name: string }

/// Two nested Shows, each with its own fallback.
[<Erase>]
type NestedGate() =
    inherit div()

    [<Erase>]
    member val signedIn: bool = unbox null with get, set

    [<Erase>]
    member val isAdmin: bool = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "nested") {
            Show(when' = props.signedIn, fallback = span (class' = "login") { "please log in" }) {
                span (class' = "welcome") { "welcome" }

                Show(when' = props.isAdmin, fallback = span (class' = "user-panel") { "user" }) {
                    span (class' = "admin-panel") { "admin" }
                }
            }
        }

/// Show with no fallback renders nothing when falsy.
[<Erase>]
type BareShow() =
    inherit div()

    [<Erase>]
    member val visible: bool = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "bare") {
            Show(when' = props.visible) { span (class' = "content") { "here" } }
        }

/// Show driven by component-local state and a button, with the condition derived from two signals.
[<SolidComponent>]
let Toggler () =
    let isOpen, setOpen = createSignal false
    let count, setCount = createSignal 0

    div (class' = "toggler") {
        button (class' = "toggle", onClick = fun _ -> setOpen (not (isOpen ()))) { "toggle" }
        button (class' = "inc", onClick = fun _ -> setCount (count () + 1)) { "inc" }

        Show(when' = (isOpen () && count () > 1), fallback = span (class' = "hidden") { "hidden" }) {
            span (class' = "shown") { "count " + string (count ()) }
        }
    }

/// Show.Keyed over a number: 0 is falsy, a changed number remounts the child.
[<Erase>]
type KeyedNumber() =
    inherit div()

    [<Erase>]
    member val value: int = unbox null with get, set

    [<Erase>]
    member val onMount: int -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "kn") {
            Show.Keyed(when' = props.value, fallback = span (class' = "zero") { "zero" }) {
                yield
                    fun (n: int) ->
                        props.onMount n
                        span (class' = "num") { "n=" + string n }
            }
        }

/// Show.NonKeyed over a number: the child is kept while truthy and reads through the accessor.
[<Erase>]
type LiveNumber() =
    inherit div()

    [<Erase>]
    member val value: int = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "ln") {
            Show.NonKeyed(when' = props.value, fallback = span (class' = "zero") { "zero" }) {
                yield fun (n: Accessor<int>) -> span (class' = "num") { "n=" + string (n ()) }
            }
        }

/// Show.Keyed with a pure-expression child (no statements) over a record.
[<Erase>]
type KeyedPerson() =
    inherit div()

    [<Erase>]
    member val person: Person option = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "kp") {
            Show.Keyed(when' = props.person) {
                yield fun (p: Person option) -> span (class' = "name") { p.Value.name }
            }
        }

/// Nested keyed Shows: the inner child reads both narrowed values.
[<Erase>]
type NestedKeyed() =
    inherit div()

    [<Erase>]
    member val person: Person option = unbox null with get, set

    [<Erase>]
    member val title: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "nk") {
            Show.Keyed(when' = props.person, fallback = span (class' = "nobody") { "nobody" }) {
                yield
                    fun (p: Person option) ->
                        Show.Keyed(when' = props.title, fallback = span (class' = "plain") { p.Value.name }) {
                            yield fun (t: string) -> span (class' = "titled") { t + " " + p.Value.name }
                        }
            }
        }

/// Show whose `keyed` flag is set on the base Show type, with a static child.
[<Erase>]
type ShowKeyedFlag() =
    inherit div()

    [<Erase>]
    member val value: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "flag") {
            Show(when' = (props.value <> null && props.value <> ""), keyed = true, fallback = em () { "empty" }) {
                span (class' = "value") { props.value }
            }
        }
