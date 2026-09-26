module Partas.Solid.Tests.Runtime.Integration.AppsMore.Modal

open Partas.Solid
open Partas.Solid.Web
open Partas.Solid.Aria
open Fable.Core
open Fable.Core.JsInterop
open Browser.Types
open Browser

/// The dialog panel itself. It only exists while the modal is open, so its body is the
/// "on open" hook: it listens for Escape on the document, focuses itself once mounted,
/// and on cleanup removes the listener and restores focus to whatever had it before.
[<Erase>]
type DialogPanel() =
    inherit div()

    [<Erase>]
    member val heading: string = unbox null with get, set

    [<Erase>]
    member val onClose: unit -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let previouslyFocused = document.activeElement
        let mutable panelRef: HTMLDivElement = JS.undefined

        let onDocKey =
            fun (e: Event) ->
                let ke = e :?> KeyboardEvent

                if ke.key = "Escape" then
                    props.onClose ()

        document.addEventListener ("keydown", onDocKey)

        onSettled (fun () ->
            if not (isNull (box panelRef)) then
                panelRef.focus ())

        onCleanup (fun () ->
            document.removeEventListener ("keydown", onDocKey)

            if not (isNull previouslyFocused) then
                (previouslyFocused :?> HTMLElement).focus ())

        div (
            class' = "backdrop",
            onClick =
                fun e ->
                    // Only a click on the backdrop itself (not bubbled from the dialog) closes.
                    if e.target = e.currentTarget then
                        props.onClose ()
        ) {
            div(
                role = "dialog",
                class' = "dialog",
                ariaModal = true,
                ariaLabelledBy = "modal-title",
                tabindex = -1
            )
                .ref (panelRef) {
                h2 (id = "modal-title", class' = "modal-title") { props.heading }
                div (class' = "modal-body") { props.children }
                button (class' = "modal-close", onClick = fun _ -> props.onClose ()) { "Close" }
            }
        }

/// Modal: renders nothing while closed; while open, portals the dialog panel into document.body.
[<Erase>]
type Modal() =
    inherit div()

    [<Erase>]
    member val isOpen: bool = unbox null with get, set

    [<Erase>]
    member val heading: string = unbox null with get, set

    [<Erase>]
    member val onClose: unit -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        Show (when' = props.isOpen) {
            Portal() {
                DialogPanel(heading = props.heading, onClose = props.onClose) { props.children }
            }
        }

/// A "delete item" page: a list of items, each with a Delete button that asks for
/// confirmation in a modal. Confirm removes the item; Cancel/Escape/backdrop keep it.
[<Erase>]
type ConfirmDeleteApp() =
    inherit div()

    [<Erase>]
    member val items: string array = unbox null with get, set

    [<Erase>]
    member val onDeleted: string -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let items, setItems = createSignal props.items
        let pending, setPending = createSignal<string option> None
        let closes, setCloses = createSignal 0

        let close () =
            setPending None
            setCloses (closes () + 1)

        let confirm () =
            match pending () with
            | Some name ->
                setItems (items () |> Array.filter (fun x -> x <> name))
                props.onDeleted name
            | None -> ()

            setPending None

        div (class' = "confirm-app") {
            ul (class' = "items") {
                For.Keyed (each = items ()) {
                    yield
                        fun name _ ->
                            li(class' = "item").data ("name", name) {
                                span (class' = "name") { name }

                                button (class' = "delete", onClick = fun _ -> setPending (Some name)) { "Delete" }
                            }
                }
            }

            span (class' = "closes") { $"closed {closes ()}" }

            Modal(
                isOpen = pending().IsSome,
                heading = "Confirm delete",
                onClose = close
            ) {
                p (class' = "question") {
                    $"""Delete {pending () |> Option.defaultValue ""}?"""
                }

                button (class' = "confirm", onClick = fun _ -> confirm ()) { "Delete it" }
            }
        }
