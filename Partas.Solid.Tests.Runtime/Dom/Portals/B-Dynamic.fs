module Partas.Solid.Tests.Runtime.Dom.Portals.Dynamic

open Partas.Solid
open Partas.Solid.Web
open Fable.Core
open Fable.Core.JsInterop

/// A plain component used as a Dynamic target.
[<Erase>]
type Badge() =
    inherit span()

    [<Erase>]
    member val label: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View = span (class' = "badge") { props.label }

/// A second component, reporting its disposal.
[<Erase>]
type Fancy() =
    inherit div()

    [<Erase>]
    member val label: string = unbox null with get, set

    [<Erase>]
    member val onDispose: string -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        onCleanup (fun () -> if not (isNull (box props.onDispose)) then props.onDispose "fancy")
        strong (class' = "fancy") { "*" + props.label + "*" }

/// Dynamic whose component is a Partas component, props passed through spread.
[<SolidComponent>]
let DynamicComponent () =
    let badgeProps = createObj [ "label", box "new" ]
    Dynamic<obj>(component' = !@Badge).spread (badgeProps)

/// Dynamic switching between intrinsic tags from a signal, using the component' field.
[<SolidComponent>]
let DynamicTagSwitch () =
    let level, setLevel = createSignal 1

    div (class' = "tag-switch") {
        button (id = "next-level", onClick = fun _ -> setLevel (level () + 1)) { "next" }
        Dynamic<obj>(component' = unbox ("h" + string (level ())))
    }

/// Dynamic whose component (tag name or component) comes from a TagValue prop; props via spread.
[<Erase>]
type DynamicFromProp() =
    inherit div()

    [<Erase>]
    member val comp: TagValue = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let fwd = createObj [ "label", box "hi" ]
        div (class' = "dfp") { Dynamic<obj>(component' = props.comp).spread (fwd) }

/// Dynamic with a falsy component renders nothing, then the tag when the signal sets it.
[<SolidComponent>]
let DynamicMaybe () =
    let tag, setTag = createSignal<string> null

    div (class' = "maybe") {
        button (id = "set-tag", onClick = fun _ -> setTag "article") { "set" }
        button (id = "unset-tag", onClick = fun _ -> setTag null) { "unset" }
        Dynamic<obj>(component' = unbox (tag ()))
    }

/// Dynamic forwarding an event handler and attributes to an intrinsic element through spread.
[<Erase>]
type DynamicButton() =
    inherit div()

    [<Erase>]
    member val onPress: unit -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let btnProps =
            createObj [
                "id", box "dyn-btn"
                "type", box "button"
                "onClick", box (fun (_: obj) -> props.onPress ())
                "children", box "press"
            ]

        div (class' = "dyn-button") { Dynamic<obj>(component' = unbox "button").spread (btnProps) }

/// Dynamic creating an SVG element from a tag name inside an svg.
[<SolidComponent>]
let DynamicSvg () =
    let circleProps = createObj [ "r", box "5"; "cx", box "10"; "cy", box "10" ]
    Svg.svg (class' = "dyn-svg") { Dynamic<obj>(component' = unbox "circle").spread (circleProps) }

/// Real usage: a polymorphic heading whose level comes from a prop, through component'.
[<Erase>]
type Heading() =
    inherit div()

    [<Erase>]
    member val level: int = unbox null with get, set

    [<Erase>]
    member val text: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let headingProps = createObj [ "class", box "heading"; "children", box props.text ]
        Dynamic<obj>(component' = unbox ("h" + string props.level)).spread (headingProps)
