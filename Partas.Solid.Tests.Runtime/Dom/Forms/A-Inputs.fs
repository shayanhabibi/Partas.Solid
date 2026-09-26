module Partas.Solid.Tests.Runtime.Dom.Forms.Inputs

open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop

/// Many input types with their type-specific attributes, all static.
[<SolidComponent>]
let InputTypes () =
    div (class' = "types") {
        input (id = "t-text", type' = "text", value = "hello", placeholder = "p", maxlength = 8, minlength = 2, pattern = "[a-z]+", autocomplete = "off")
        input (id = "t-num", type' = "number", value = "5", min = 0, max = 10, step = "0.5")
        input (id = "t-range", type' = "range", value = "30", min = 0, max = 200, step = "10")
        input(id = "t-date", type' = "date", value = "2024-03-15").attr("min", "2024-01-01").attr ("max", "2024-12-31")
        input (id = "t-email", type' = "email", value = "a@b.co", multiple = true)
        input (id = "t-pass", type' = "password", value = "secret", minLength = 4)
        input (id = "t-hidden", type' = "hidden", name = "token", value = "xyz")
        input (id = "t-color", type' = "color", value = "#ff0000")
        input (id = "t-file", type' = "file", accept = "image/*", multiple = true)
        input (id = "t-search", type' = "search", inputmode = "search", enterkeyhint = "search")
    }

/// A checkbox whose checked state is owned by a signal; a button resets it from outside.
[<SolidComponent>]
let ControlledCheckbox () =
    let on, setOn = createSignal false

    div (class' = "cc") {
        input (id = "cb", type' = "checkbox", checked' = on (), onChange = fun e -> setOn (!!e.currentTarget?``checked``))
        span (id = "cb-state") { if on () then "on" else "off" }
        button (id = "cb-set", onClick = fun _ -> setOn true) { "set" }
        button (id = "cb-clear", onClick = fun _ -> setOn false) { "clear" }
    }

/// A radio group bound to one signal: checked = (selected = value).
[<SolidComponent>]
let RadioGroup () =
    let size, setSize = createSignal "m"

    fieldset (id = "rg") {
        legend () { "Size" }
        For.Keyed (each = [| "s"; "m"; "l" |]) {
            yield
                fun v _ ->
                    label () {
                        input (type' = "radio", name = "size", value = v, checked' = (size () = v), onChange = fun _ -> setSize v)
                        v
                    }
        }
        output (id = "rg-out") { size () }
        button (id = "rg-large", type' = "button", onClick = fun _ -> setSize "l") { "large" }
    }

/// A range slider driving a number read-out and a number input (two views of one signal).
[<SolidComponent>]
let LinkedRange () =
    let level, setLevel = createSignal 40

    div (class' = "lr") {
        input (id = "lr-range", type' = "range", min = 0, max = 100, value = string (level ()), onInput = fun e -> setLevel (!!e.currentTarget?valueAsNumber))
        input (id = "lr-num", type' = "number", min = 0, max = 100, value = string (level ()), onInput = fun e -> setLevel (!!e.currentTarget?valueAsNumber))
        span (id = "lr-out") { level () }
    }

/// Static value vs dynamic value: a literal is an attribute (default value), an expression is the property.
[<SolidComponent>]
let ValueSemantics () =
    let text, setText = createSignal "dynamic"

    div (class' = "vs") {
        input (id = "vs-static", value = "static")
        input (id = "vs-dyn", value = text (), onInput = fun e -> setText (!!e.currentTarget?value))
        span (id = "vs-echo") { text () }
        button (id = "vs-reset", onClick = fun _ -> setText "reset") { "reset" }
    }

/// Controlled textarea: value comes from a signal, not from children.
[<SolidComponent>]
let ControlledTextarea () =
    let body, setBody = createSignal "first line"

    div (class' = "ct") {
        textarea (id = "ct-ta", rows = 4, cols = 30, wrap = "soft", value = body (), onInput = fun e -> setBody (!!e.currentTarget?value))
        span (id = "ct-len") { body().Length }
        button (id = "ct-clear", onClick = fun _ -> setBody "") { "clear" }
    }

/// An input whose value prop may be undefined.
[<Erase>]
type MaybeValue() =
    inherit div()

    [<Erase>]
    member val v: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View = input (id = "mv", value = props.v)

/// Boolean input attributes toggled at runtime.
[<SolidComponent>]
let ToggledInputFlags () =
    let flag, setFlag = createSignal true

    div (class' = "tf") {
        input (id = "tf-in", disabled = flag (), readonly = flag (), required = flag (), autofocus = false)
        textarea (id = "tf-ta", disabled = flag (), readOnly = flag ())
        button (id = "tf-toggle", onClick = fun _ -> setFlag (not (flag ()))) { "toggle" }
    }

/// Global enumerated attribute spellcheck: disabling spellcheck on an editable region.
[<SolidComponent>]
let SpellcheckOff () =
    div (id = "sp", contenteditable = "true", spellcheck = false) { "edit me" }

/// Global enumerated attribute spellcheck set on.
[<SolidComponent>]
let SpellcheckOn () =
    div (id = "sp-on", contenteditable = "true", spellcheck = true) { "edit me" }

/// Input value property escape hatch via .attr("prop:...") - forces a property write.
[<SolidComponent>]
let PropNamespace () =
    let v, setV = createSignal "a"

    div (class' = "pn") {
        input(id = "pn-in").attr ("prop:value", v ())
        input(id = "pn-dv").attr ("prop:defaultValue", "dv")
        button (id = "pn-b", onClick = fun _ -> setV "b") { "b" }
    }

/// Uncontrolled inputs: defaultValue / defaultChecked are not bound, so they go through .attr.
[<SolidComponent>]
let Uncontrolled () =
    form (id = "unc") {
        input(id = "unc-text", name = "t").attr ("defaultValue", "start")
        input(id = "unc-cb", name = "c", type' = "checkbox").attr ("defaultChecked", true)
        textarea(id = "unc-ta", name = "ta").attr ("defaultValue", "body")
    }

/// Global boolean inert.
[<SolidComponent>]
let Inert () =
    div () {
        div (id = "inert-on", inert = true) { button () { "x" } }
        div (id = "inert-off", inert = false) { button () { "y" } }
    }
