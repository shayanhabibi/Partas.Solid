module Partas.Solid.Tests.Runtime.Dom.Forms.Selects

open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop

/// A controlled select: value bound to a signal, options from a list, change handler writes back.
[<SolidComponent>]
let ControlledSelect () =
    let fruit, setFruit = createSignal "pear"

    div (class' = "cs") {
        select (id = "cs-sel", value = fruit (), onChange = fun e -> setFruit (!!e.currentTarget?value)) {
            For.Keyed (each = [| "apple"; "pear"; "plum" |]) {
                yield fun f _ -> option' (value = f) { f }
            }
        }
        span (id = "cs-out") { fruit () }
        button (id = "cs-plum", onClick = fun _ -> setFruit "plum") { "plum" }
    }

/// A multi-select with selected options driven per option.
[<SolidComponent>]
let MultiSelect () =
    let picked, setPicked = createSignal [| "b"; "c" |]

    div (class' = "ms") {
        select (id = "ms-sel", multiple = true, size = 4, name = "letters") {
            For.Keyed (each = [| "a"; "b"; "c"; "d" |]) {
                yield fun l _ -> option' (value = l, selected = Array.contains l (picked ())) { l.ToUpper () }
            }
        }
        button (id = "ms-only-a", onClick = fun _ -> setPicked [| "a" |]) { "only a" }
    }

/// optgroup with label and disabled, option label attribute.
[<SolidComponent>]
let Groups () =
    select (id = "grp") {
        optgroup (label = "Fruit") {
            option' (value = "apple") { "Apple" }
            option' (value = "pear", label = "Pear (ripe)") { "Pear" }
        }
        optgroup (label = "Veg", disabled = true) {
            option' (value = "kale") { "Kale" }
        }
    }

/// A select whose options arrive after the value is set: Solid defers select.value to a microtask.
[<SolidComponent>]
let LateOptions () =
    let choice, _ = createSignal "z"

    select (id = "late", value = choice ()) {
        option' (value = "x") { "X" }
        option' (value = "y") { "Y" }
        option' (value = "z") { "Z" }
    }

/// A datalist bound to an input via list.
[<SolidComponent>]
let DataList () =
    div (class' = "dl") {
        input (id = "dl-in", list = "dl-opts")
        datalist (id = "dl-opts") {
            option' (value = "red")
            option' (value = "green")
        }
    }

/// label for / htmlFor associations and a label wrapping its control.
[<SolidComponent>]
let Labels () =
    div (class' = "labels") {
        label (id = "lb-for", for' = "lb-target") { "For" }
        input (id = "lb-target", type' = "checkbox")
        label (id = "lb-wrap") {
            "Wrapped"
            input (id = "lb-inner", type' = "checkbox")
        }
        output (id = "lb-out", for' = "lb-target lb-inner", name = "result") { "0" }
    }

/// fieldset disabled disables every descendant control, toggled by a signal.
[<SolidComponent>]
let FieldsetDisabled () =
    let locked, setLocked = createSignal true

    div (class' = "fd") {
        fieldset (id = "fd-set", disabled = locked (), name = "grp") {
            legend () { "Group" }
            input (id = "fd-in", name = "a", value = "1")
            button (id = "fd-btn", type' = "button") { "inner" }
        }
        button (id = "fd-toggle", onClick = fun _ -> setLocked (not (locked ()))) { "toggle" }
    }

/// Button types: default submit inside a form, explicit button, reset.
[<Erase>]
type ButtonTypes() =
    inherit div()

    [<Erase>]
    member val onSubmitted: string -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        form (
            id = "bt-form",
            onSubmit =
                fun e ->
                    e.preventDefault ()
                    props.onSubmitted (!!e.submitter?id)
        ) {
            input (id = "bt-in", name = "q", value = "initial")
            button (id = "bt-default") { "default" }
            button (id = "bt-button", type' = "button") { "button" }
            button (id = "bt-reset", type' = "reset") { "reset" }
            button (id = "bt-submit", type' = "submit", name = "action", value = "save") { "save" }
        }

/// A form that reads its fields through FormData on submit.
[<Erase>]
type FormDataSubmit() =
    inherit div()

    [<Erase>]
    member val onData: obj -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        form (
            id = "fds",
            action = "/never",
            method = "post",
            noValidate = true,
            onSubmit =
                fun e ->
                    e.preventDefault ()
                    props.onData (emitJsExpr e.currentTarget "Object.fromEntries(new FormData($0))")
        ) {
            input (name = "user", value = "ann")
            input (name = "agree", type' = "checkbox", checked' = true)
            input (name = "skip", type' = "checkbox")
            input (name = "gone", disabled = true, value = "x")
            select (name = "tier") {
                option' (value = "free") { "Free" }
                option' (value = "pro", selected = true) { "Pro" }
            }
            textarea (name = "note") { "hi" }
            button (id = "fds-go", type' = "submit") { "go" }
        }

/// onReset and onInvalid handlers.
[<Erase>]
type ResetInvalid() =
    inherit div()

    [<Erase>]
    member val log: string -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        form (id = "ri", onReset = fun _ -> props.log "reset") {
            input (id = "ri-req", required = true, onInvalid = fun _ -> props.log "invalid")
        }
