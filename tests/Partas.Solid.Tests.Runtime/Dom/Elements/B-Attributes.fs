module Partas.Solid.Tests.Runtime.Dom.Elements.Attributes

open Partas.Solid
open Partas.Solid.Aria
open Partas.Solid.Style
open Fable.Core
open Fable.Core.JsInterop

/// Global attributes on a single element.
[<SolidComponent>]
let Globals () =
    div (
        id = "g",
        class' = "a b",
        title = "tip",
        lang = "en",
        dir = "rtl",
        tabindex = 3,
        draggable = "true",
        accessKey = "k"
    ) {
        "globals"
    }

/// data-* through the .data extension and arbitrary attributes through .attr.
[<SolidComponent>]
let DataAttrs () =
    div(class' = "d").data("user-id", "42").data("role", "admin").attr ("custom-attr", "yes") { "data" }

/// aria-* through the Aria module properties (camelCase lowered).
[<SolidComponent>]
let AriaAttrs () =
    button (
        class' = "aria",
        ariaLabel = "Close dialog",
        ariaExpanded = "false",
        ariaControls = "menu-1",
        ariaHidden = true
    ) {
        "x"
    }

/// aria-hidden with false.
[<SolidComponent>]
let AriaHiddenFalse () =
    span (class' = "ah", ariaHidden = false) { "visible" }

/// Boolean attributes set true.
[<SolidComponent>]
let BoolsTrue () =
    div () {
        input (id = "dis", disabled = true)
        input (id = "chk", type' = "checkbox", checked' = true)
        input (id = "ro", readonly = true)
        input (id = "req", required = true)
        button (id = "btn", disabled = true) { "b" }
        div (id = "hid", hidden = !^true) { "h" }
    }

/// Boolean attributes set false: Solid 2 removes the attribute.
[<SolidComponent>]
let BoolsFalse () =
    div () {
        input (id = "dis", disabled = false)
        input (id = "chk", type' = "checkbox", checked' = false)
        input (id = "ro", readonly = false)
        button (id = "btn", disabled = false) { "b" }
        div (id = "hid", hidden = !^false) { "h" }
    }

/// Boolean attributes driven by a runtime (non-literal) value.
[<Erase>]
type BoolProps() =
    inherit div()

    [<Erase>]
    member val on: bool = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "bp") {
            input (id = "dis", disabled = props.on)
            input (id = "chk", type' = "checkbox", checked' = props.on)
            button (id = "btn", disabled = props.on) { "b" }
        }

/// The .bool extension: a named boolean attribute.
[<SolidComponent>]
let BoolExt () =
    div () {
        div(id = "t").bool ("itemscope", true) { "t" }
        div(id = "f").bool ("itemscope", false) { "f" }
    }

/// style' with the Style DSL and a custom property.
[<SolidComponent>]
let StyleSpecEl () =
    div(class' = "styled").style' [ Style.backgroundColor Color.Red; Style.display Display.Flex; "--my-var" ==> "12px" ] {
        "s"
    }

/// Plain string style attribute.
[<SolidComponent>]
let StyleString () =
    div (class' = "sstr", style = "color: blue; margin-top: 4px") { "s" }

/// style' with a POJO created by createObj.
[<SolidComponent>]
let StyleObj () =
    div(class' = "sobj").style' (createObj [ "color", box "green"; "padding-left", box "2px" ]) { "o" }

/// Solid 2 class accepts an object map. The .class'(obj) extension is shadowed by the class'
/// string property on tags (does not compile), so the object is passed through the property.
[<SolidComponent>]
let ClassList () =
    div (class' = !!(createObj [ "on", box true; "off", box false; "also-on", box true ])) { "c" }

/// Per-element attributes: anchors, images, labels, inputs, textarea, select, form.
[<SolidComponent>]
let PerElement () =
    form (id = "f", action = "/submit", method = "post") {
        a (id = "lnk", href = "https://example.com/x?y=1", target = "_blank", rel = "noopener") { "link" }
        img (id = "pic", src = "/pic.png", alt = "A picture", width = 20)
        label (id = "lbl", for' = "name") { "Name" }
        input (id = "name", type' = "text", value = "initial", placeholder = "Your name", name = "name", maxlength = 10)
        textarea (id = "ta", placeholder = "Type here", rows = 3, cols = 20) { "prefilled" }
        select (id = "sel", name = "pick") {
            option' (value = "a") { "Alpha" }
            option' (value = "b", selected = true) { "Beta" }
            option' (value = "c", disabled = true) { "Gamma" }
        }
    }

/// innerHTML and textContent props.
[<SolidComponent>]
let InnerHtml () =
    div () {
        div (id = "ih", innerHTML = "<em>raw</em><i>html</i>")
        div (id = "tc", textContent = "<em>not html</em>")
    }

/// innerHTML from a prop value (dynamic).
[<Erase>]
type HtmlProp() =
    inherit div()

    [<Erase>]
    member val html: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View = div (id = "ihp", innerHTML = props.html)

/// input value given by a prop: must be the DOM property, not just the attribute.
[<Erase>]
type ValueProp() =
    inherit div()

    [<Erase>]
    member val v: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View = input (id = "vp", value = props.v)

/// aria-* through the generic .attr extension (the escape hatch).
[<SolidComponent>]
let AriaViaAttr () =
    button(class' = "aria-attr").attr("aria-label", "Close").attr ("aria-pressed", "true") { "x" }

/// tabindex 0 and negative.
[<SolidComponent>]
let TabIndexes () =
    div () {
        div (id = "t0", tabindex = 0) { "zero" }
        div (id = "tneg", tabindex = -1) { "neg" }
    }
