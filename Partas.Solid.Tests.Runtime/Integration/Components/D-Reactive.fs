module Partas.Solid.Tests.Runtime.Integration.Components.Reactive

open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop

/// Reactive attributes, class, style (string and object) and class object, all driven by props.
[<Erase>]
type StatusChip() =
    inherit div()

    [<Erase>]
    member val status: string = unbox null with get, set

    [<Erase>]
    member val active: bool = unbox null with get, set

    [<Erase>]
    member val color: string = unbox null with get, set

    [<Erase>]
    member val size: int = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "chip chip-" + props.status, title = "status: " + props.status) {
            span(class' = "dot").style' ({| color = props.color; ``font-size`` = string props.size + "px" |})
            span(class' = "text", style = "color: " + props.color) { props.status }
            // `.class'(obj)` cannot be called on tags: the `class'` string property shadows the extension.
            span (class' = !!{| on = props.active; off = not props.active |})
            button (class' = "act", disabled = not props.active) { "act" }
            input (class' = "field", value = props.status)
        }

/// Real usage: a text field that mirrors its value into a preview, with a length-driven class.
[<SolidComponent>]
let MirrorField () =
    let value, setValue = createSignal ""

    div (class' = "mirror") {
        input (class' = "src", onInput = fun e -> setValue (e.target?value))
        p (class' = (if value().Length > 3 then "preview long" else "preview short")) { value () }
        span (class' = "len") { value().Length }
    }

/// Real usage: toggle + counter where several attributes depend on one signal.
[<SolidComponent>]
let ToggleCounter () =
    let on, setOn = createSignal false
    let count, setCount = createSignal 0

    div (class' = "toggle-counter") {
        button(class' = "toggle", onClick = (fun _ -> setOn (not (on ())))).attr ("aria-pressed", (if on () then "true" else "false")) {
            if on () then "ON" else "OFF"
        }

        button (class' = "inc", disabled = not (on ()), onClick = fun _ -> setCount (count () + 1)) { "+1" }
        output(class' = "count").data ("count", string (count ())) { count () }
    }
