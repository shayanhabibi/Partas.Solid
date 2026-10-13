module Partas.Solid.Tests.Runtime.Integration.Apps.Disclosure

open Partas.Solid
open Partas.Solid.Aria
open Fable.Core
open Fable.Core.JsInterop

type Section =
    {| id: string
       label: string
       content: string |}

/// A counter that lives inside a panel, to observe whether panel state survives tab switches.
[<SolidComponent>]
let PanelCounter () =
    let count, setCount = createSignal 0

    button (class' = "panel-counter", onClick = fun _ -> setCount (count () + 1)) { $"clicked {count ()}" }

/// WAI-ARIA tabs: roving tabindex, aria-selected, arrow/Home/End keyboard navigation.
/// Only the selected panel is mounted (each panel sits under its own <Show>).
[<Erase>]
type Tabs() =
    inherit div()

    [<Erase>]
    member val items: Section array = unbox null with get, set

    [<Erase>]
    member val defaultValue: string = unbox null with get, set

    [<Erase>]
    member val onTabChange: string -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let selected, setSelected =
            createSignal (
                if isNull props.defaultValue then
                    props.items[0].id
                else
                    props.defaultValue
            )

        let select (id: string) =
            if id <> selected () then
                setSelected id
                props.onTabChange id

        let indexOf (id: string) =
            props.items |> Array.findIndex (fun t -> t.id = id)

        div (class' = "tabs") {
            div (
                role = "tablist",
                class' = "tablist",
                onKeyDown =
                    fun e ->
                        let n = props.items.Length
                        let i = indexOf (selected ())

                        match e.key with
                        | "ArrowRight" -> select props.items[(i + 1) % n].id
                        | "ArrowLeft" -> select props.items[(i - 1 + n) % n].id
                        | "Home" -> select props.items[0].id
                        | "End" -> select props.items[n - 1].id
                        | _ -> ()
            ) {
                For.Keyed (each = props.items) {
                    yield
                        fun tab _ ->
                            button (
                                role = "tab",
                                id = $"tab-{tab.id}",
                                class' = (if selected () = tab.id then "tab active" else "tab"),
                                ariaSelected = (if selected () = tab.id then "true" else "false"),
                                ariaControls = $"panel-{tab.id}",
                                tabindex = (if selected () = tab.id then 0 else -1),
                                onClick = fun _ -> select tab.id
                            ) {
                                tab.label
                            }
                }
            }

            For.Keyed (each = props.items) {
                yield
                    fun tab _ ->
                        Show (when' = (selected () = tab.id)) {
                            div (role = "tabpanel", id = $"panel-{tab.id}", class' = "panel") {
                                p (class' = "panel-content") { tab.content }
                                PanelCounter ()
                            }
                        }
            }
        }

/// Accordion: single or multiple expansion, aria-expanded + data-state per item.
[<Erase>]
type Accordion() =
    inherit div()

    [<Erase>]
    member val items: Section array = unbox null with get, set

    [<Erase>]
    member val multiple: bool = unbox null with get, set

    [<Erase>]
    member val defaultOpen: string array = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        props.multiple <- false
        props.defaultOpen <- [||]

        let openIds, setOpenIds = createSignal props.defaultOpen

        let isOpen (id: string) = openIds () |> Array.contains id

        let toggle (id: string) =
            if isOpen id then
                setOpenIds (openIds () |> Array.filter (fun x -> x <> id))
            elif props.multiple then
                setOpenIds (Array.append (openIds ()) [| id |])
            else
                setOpenIds [| id |]

        div (class' = "accordion") {
            For.Keyed (each = props.items) {
                yield
                    fun item _ ->
                        div(class' = "acc-item").data ("state", (if isOpen item.id then "open" else "closed")) {
                            h3 (class' = "acc-header") {
                                button (
                                    class' = "acc-trigger",
                                    ariaExpanded = (if isOpen item.id then "true" else "false"),
                                    ariaControls = $"acc-{item.id}",
                                    onClick = fun _ -> toggle item.id
                                ) {
                                    item.label
                                }
                            }

                            Show (when' = isOpen item.id) {
                                div (class' = "acc-panel", id = $"acc-{item.id}", role = "region") { item.content }
                            }
                        }
            }

            span (class' = "open-count") { $"{openIds().Length} open" }
        }
