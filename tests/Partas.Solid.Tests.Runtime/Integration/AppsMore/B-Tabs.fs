module Partas.Solid.Tests.Runtime.Integration.AppsMore.Tabs

open Partas.Solid
open Partas.Solid.Aria
open Fable.Core
open Fable.Core.JsInterop
open Browser

type TabDef =
    {| id: string
       label: string
       disabled: bool |}

/// Vertical settings tabs.
/// - Up/Down arrows move (and focus) the selection, skipping disabled tabs and wrapping.
/// - All panels stay mounted; inactive ones are `hidden` (so panel state survives switching).
/// - Optionally controlled: when `value` is supplied it wins over the internal selection, and
///   the component only reports requested changes through `onChange`.
[<Erase>]
type VerticalTabs() =
    inherit div()

    [<Erase>]
    member val tabs: TabDef array = unbox null with get, set

    [<Erase>]
    member val value: string = unbox null with get, set

    [<Erase>]
    member val onChange: string -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let firstEnabled () =
            props.tabs |> Array.find (fun t -> not t.disabled) |> fun t -> t.id

        let internalSel, setInternalSel = createSignal (firstEnabled ())

        let selected () =
            if isNull props.value then internalSel () else props.value

        let select (id: string) =
            if id <> selected () then
                setInternalSel id

                if not (isNull (box props.onChange)) then
                    props.onChange id

            let el = document.getElementById ($"vtab-{id}")

            if not (isNull el) then
                el.focus ()

        /// Next enabled tab in direction `step` (+1/-1), wrapping.
        let move (step: int) =
            let n = props.tabs.Length
            let start = props.tabs |> Array.findIndex (fun t -> t.id = selected ())
            let mutable i = (start + step + n) % n

            while props.tabs[i].disabled && i <> start do
                i <- (i + step + n) % n

            select props.tabs[i].id

        div (class' = "vtabs") {
            div (
                role = "tablist",
                class' = "vtablist",
                ariaOrientation = "vertical",
                onKeyDown =
                    fun e ->
                        match e.key with
                        | "ArrowDown" ->
                            e.preventDefault ()
                            move 1
                        | "ArrowUp" ->
                            e.preventDefault ()
                            move -1
                        | _ -> ()
            ) {
                For.Keyed (each = props.tabs) {
                    yield
                        fun tab _ ->
                            button (
                                role = "tab",
                                id = $"vtab-{tab.id}",
                                class' = (if selected () = tab.id then "vtab selected" else "vtab"),
                                ariaSelected = (if selected () = tab.id then "true" else "false"),
                                disabled = tab.disabled,
                                tabindex = (if selected () = tab.id then 0 else -1),
                                onClick = fun _ -> select tab.id
                            ) {
                                tab.label
                            }
                }
            }

            For.Keyed (each = props.tabs) {
                yield
                    fun tab _ ->
                        div (
                            role = "tabpanel",
                            id = $"vpanel-{tab.id}",
                            class' = "vpanel",
                            hidden = !^(selected () <> tab.id)
                        ) {
                            label () { tab.label }
                            input (class' = $"field field-{tab.id}", type' = "text")
                        }
            }

            p (class' = "vtabs-current") { $"Current: {selected ()}" }
        }
