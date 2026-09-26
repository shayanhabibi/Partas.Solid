module Partas.Solid.Tests.Runtime.Dom.Forms.CustomElements

open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop

/// A user-declared custom element tag (a web component), as a user would bind it:
/// an erased tag type named after the element, with typed attributes.
[<Erase>]
type ``x-counter``() =
    interface RegularNode

    [<Erase>]
    member val label: string = unbox null with get, set

    [<Erase>]
    member val count: int = unbox null with get, set

/// Static and dynamic attributes on a custom element, plus light-DOM children.
[<SolidComponent>]
let UsesCustomElement () =
    let n, setN = createSignal 1

    div (class' = "ce") {
        ``x-counter`` (id = "xc", label = "Clicks", count = n ()) { span (class' = "light") { "child" } }
        button (id = "xc-inc", onClick = fun _ -> setN (n () + 1)) { "inc" }
    }

/// Arbitrary attributes on a custom element through .attr, including a prop: namespaced one
/// that must reach the element as a DOM property (Solid 2 keeps prop: as its only namespace).
[<SolidComponent>]
let CustomElementProps () =
    let items, setItems = createSignal [| "a"; "b" |]

    div (class' = "cep") {
        ``x-counter``(id = "xp").attr("data-kind", "k").attr ("prop:items", items ())
        button (id = "xp-set", onClick = fun _ -> setItems [| "c" |]) { "set" }
    }

/// The global `is` attribute (customised built-in) and part/exportparts/slot globals.
[<SolidComponent>]
let GlobalsForComponents () =
    div (class' = "gfc") {
        button (id = "is-btn", is = "fancy-button") { "fancy" }
        span (id = "slotted", slot = "title", part = "label") { "t" }
        div (id = "exp", exportparts = "label: title-label") { "e" }
    }

/// template and slot are not bound as tags; user-declared erased tag types stand in for them.
/// (template content is left empty: the Solid dev template validator cannot round-trip template
/// children through innerHTML and rejects them at vite-transform time; that is upstream, not Partas.)
[<Erase>]
type template() =
    interface RegularNode

[<Erase>]
type slot() =
    interface RegularNode

    [<Erase>]
    member val name: string = unbox null with get, set

[<SolidComponent>]
let TemplateAndSlot () =
    div (class' = "ts") {
        template (id = "tpl")
        slot (id = "sl", name = "footer") { "fallback" }
    }

/// Events on a custom element: a lower-case custom event name via on + name.
[<Erase>]
type CustomEventHost() =
    inherit div()

    [<Erase>]
    member val log: string -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "ceh") {
            ``x-counter`` (id = "xe", onClick = fun _ -> props.log "click") { "tap" }
        }
