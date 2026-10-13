module Partas.Solid.Tests.Runtime.Dom.Elements.Web

open Partas.Solid
open Partas.Solid.Web
open Fable.Core
open Fable.Core.JsInterop

/// Portal into an explicit mount node.
[<Erase>]
type Portaled() =
    inherit div()

    [<Erase>]
    member val target: Browser.Types.Element = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "host") {
            span (class' = "before") { "before" }
            Portal(mount = props.target) { div (class' = "modal") { "in portal" } }
            span (class' = "after") { "after" }
        }

/// Portal without a mount renders into document.body.
[<SolidComponent>]
let PortalBody () =
    div (class' = "host2") { Portal() { p (id = "body-portal") { "to body" } } }

/// Dynamic without builder children: a string tag with spread props.
[<SolidComponent>]
let DynamicNoChildren () =
    let dynProps = createObj [ "id", box "dyn-empty"; "title", box "t" ]
    Dynamic<obj>(componentAsString = "section").spread (dynProps)

/// Dynamic whose component is a tag name from a prop, no children.
[<Erase>]
type DynamicTagProp() =
    inherit div()

    [<Erase>]
    member val tag: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "dyn-host") { Dynamic<obj>(componentAsString = props.tag) }

/// Dynamic whose component comes from a prop, set through the raw component' field.
[<Erase>]
type DynamicTagField() =
    inherit div()

    [<Erase>]
    member val tag: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "dyn-field") { Dynamic<obj>(component' = unbox props.tag) }
