module Partas.Solid.Tests.Runtime.Dom.Elements.Events

open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop

/// Event handlers report (name, event) pairs back to the spec through a `log` prop.
[<Erase>]
type EventLog() =
    inherit div()

    [<Erase>]
    member val log: string -> obj -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "events") {
            button (id = "btn", onClick = fun e -> props.log "click" e) { "go" }
            input (id = "txt", onInput = fun e -> props.log "input" e)
            input (id = "keys", onKeyDown = fun e -> props.log "keydown" e)
            input (id = "foc", onFocus = (fun e -> props.log "focus" e), onBlur = (fun e -> props.log "blur" e))
            select (id = "sel", onChange = fun e -> props.log "change" e) {
                option' (value = "a") { "A" }
                option' (value = "b") { "B" }
            }
            div (id = "outer", onClick = fun e -> props.log "outer" e) {
                span (id = "inner", onClick = fun e -> props.log "inner" e) { "inner" }
            }
            div (id = "stopper", onClick = fun e -> props.log "stopper-outer" e) {
                button (
                    id = "stop",
                    onClick =
                        fun e ->
                            e.stopPropagation ()
                            props.log "stop" e
                ) {
                    "stop"
                }
            }
            form (
                id = "frm",
                onSubmit =
                    fun e ->
                        e.preventDefault ()
                        props.log "submit" e
            ) {
                button (id = "submit", type' = "submit") { "send" }
            }
            button (id = "dbl", onDblClick = fun e -> props.log "dblclick" e) { "dbl" }
            div (id = "md", onMouseDown = (fun e -> props.log "mousedown" e), onMouseUp = (fun e -> props.log "mouseup" e)) {
                "md"
            }
        }

/// A handler that is a prop passed straight through (non-literal handler expression).
[<Erase>]
type PassThrough() =
    inherit div()

    [<Erase>]
    member val handler: Fable.Core.TS.Dom.MouseEvent -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View = button (id = "pt", onClick = props.handler) { "pt" }
