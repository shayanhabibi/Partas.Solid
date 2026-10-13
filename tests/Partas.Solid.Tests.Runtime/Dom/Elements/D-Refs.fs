module Partas.Solid.Tests.Runtime.Dom.Elements.Refs

open Partas.Solid
open Fable.Core
open Browser.Types

/// A ref callback receives the element it is attached to.
[<Erase>]
type RefCallback() =
    inherit div()

    [<Erase>]
    member val got: obj -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "rc") {
            div(id = "target").ref (fun (el: HTMLDivElement) -> props.got (box el)) { "t" }
        }

/// A mutable local captures the element; a click handler uses it afterwards.
[<SolidComponent>]
let RefVariable () =
    let mutable inputRef: HTMLInputElement = JS.undefined

    div (class' = "rv") {
        input(id = "field").ref (inputRef)
        button (id = "fill", onClick = fun _ -> inputRef.value <- "filled via ref") { "fill" }
    }

/// The ref callback observes the element with its attributes and children already applied.
[<Erase>]
type RefSeesAttributes() =
    inherit div()

    [<Erase>]
    member val report: string -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        section(id = "sec", class' = "has-attrs").ref (fun (el: HTMLElement) ->
            props.report (el.tagName + "|" + el.id + "|" + el.className)) {
            span () { "child" }
        }
