module Partas.Solid.Tests.Runtime.Dom.Elements.Spread

open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop

/// Spreads the remaining props onto the button; `label` is consumed and must not leak.
[<Erase>]
type SpreadButton() =
    inherit button()

    [<Erase>]
    member val label: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View = button(class' = "sp").spread props { props.label }

/// A default class set in the component, other props spread onto a nested element.
[<Erase>]
type DefaultedSpread() =
    inherit div()

    [<SolidTypeComponent>]
    member props.View =
        props.class' <- "default-class"

        div (id = "outer") {
            span (id = "cls", class' = props.class') { "c" }
            div(id = "rest").spread props { "rest" }
        }

/// Spreading a locally created object.
[<SolidComponent>]
let LocalSpread () =
    let extra = createObj [ "id", box "local"; "title", box "from object"; "data-x", box "1" ]
    div(class' = "ls").spread (extra) { "local" }

/// Spread with no incoming props at all.
[<Erase>]
type EmptySpread() =
    inherit div()

    [<SolidTypeComponent>]
    member props.View = div(id = "empty-spread").spread props { "e" }
