module Partas.Solid.Tests.Runtime.Dom.Styles.Composition

open Partas.Solid
open Partas.Solid.Style
open Partas.Solid.Experimental.U
open Partas.Solid.Tests.Runtime.Dom.Styles.StyleAttr
open Partas.Solid.Tests.Runtime.Dom.Styles.ClassAttr
open Fable.Core
open Fable.Core.JsInterop

/// A component that forwards the consumer's `style` string prop explicitly.
[<Erase>]
type StyleForward() =
    inherit div()

    [<SolidTypeComponent>]
    member props.View = div (id = "sf", style = props.style) { "fwd" }

/// A component that forwards the consumer's class explicitly (no spread).
[<Erase>]
type ClassForward() =
    inherit div()

    [<SolidTypeComponent>]
    member props.View = div (id = "cf", class' = props.class') { "fwd" }

/// F# consumers styling a component through style' and class'.
[<SolidComponent>]
let StyledConsumers () =
    div (id = "consumers") {
        PassThrough().style' [ Style.color Color.Red; "--pt" ==> "2px" ]
        Card(class' = "wide", title = "card-title")
        Badge(class' = "extra", tone = "ok") { "B" }
        StyleForward(style = "color: blue")
        ClassForward(class' = "forwarded")
    }

/// F# consumer driving a component's style from a signal.
[<SolidComponent>]
let ReactiveConsumer () =
    let big, setBig = createSignal false

    div (id = "rc") {
        ColorBox(fg = (if big () then "red" else "black"), width = (if big () then "100px" else "10px"))
        CssText(css = (if big () then "font-size: 20px" else "font-size: 10px"))
        button (class' = "toggle", onClick = fun _ -> setBig (not (big ()))) { "toggle" }
    }

/// Base styles concatenated with overrides (list append).
[<SolidComponent>]
let AppendedLists () =
    let baseStyles = [ Style.display Display.Flex; Style.color Color.Black ]
    let over, setOver = createSignal false

    div (id = "al") {
        div(class' = "target").style' (baseStyles @ [ if over () then Style.color Color.Red ]) { "t" }
        button (class' = "toggle", onClick = fun _ -> setOver (not (over ()))) { "toggle" }
    }

/// A style object with a computed (signal-dependent) key.
[<SolidComponent>]
let ComputedKey () =
    let vertical, setVertical = createSignal false

    div (id = "ck") {
        div(class' = "target").style' (createObj [ (if vertical () then "height" else "width"), box "10px" ]) { "t" }
        button (class' = "toggle", onClick = fun _ -> setVertical (not (vertical ()))) { "toggle" }
    }

/// Per-item style in a list rendering (colour swatches).
[<SolidComponent>]
let Swatches () =
    div (id = "sw") {
        For.Keyed (each = [| "red"; "green"; "blue" |]) {
            yield fun c i -> span(class' = "swatch", title = c).style' [ Style.backgroundColor c; "--i" ==> string (i ()) ]
        }
    }

/// The .attr("style", ...) escape hatch and .attr("class", ...).
[<SolidComponent>]
let AttrEscapeHatch () =
    div(id = "eh").attr("style", "color: red").attr ("class", "via-attr") { "eh" }

/// Style list with a typed numeric SVG member.
[<SolidComponent>]
let SvgNumbers () =
    Svg.svg (id = "svgn", viewBox = "0 0 1 1") {
        Svg.rect(id = "rn", width = 1.0, height = 1.0).style' [ SvgStyle.opacity 0.5; SvgStyle.strokeOpacity 0.25 ]
    }

/// Duplicate keys in one list: the last declaration wins (like CSS).
[<SolidComponent>]
let DuplicateKeys () =
    div(id = "dk").style' [ Style.color Color.Red; Style.color Color.Blue ] { "dk" }

/// A style list whose values are computed by let-bound helpers.
[<SolidComponent>]
let HelperValues () =
    let px (n: int) = $"{n}px"
    let n, setN = createSignal 2

    div (id = "hv") {
        div(class' = "target").style' [ "padding" ==> px (n ()); Style.margin (px (n () * 2)) ] { "t" }
        button (class' = "inc", onClick = fun _ -> setN (n () + 1)) { "inc" }
    }

/// Default class and style set through props setters (merged defaults).
[<Erase>]
type Defaults() =
    inherit div()

    [<SolidTypeComponent>]
    member props.View =
        props.class' <- "default-class"
        props.style <- "color: green"
        div (id = "df", class' = props.class', style = props.style) { "d" }

/// A spread of a let-bound anonymous record carrying class and style.
/// (Spreading the record literal inline fails to compile: "Unhandled extension: spread".)
[<SolidComponent>]
let SpreadRecord () =
    let extra = {| style = "color: maroon"; ``class`` = "from-spread" |}
    div(id = "sr").spread extra { "sr" }

/// A style prop typed as a StyleSpec list, applied with style'.
[<Erase>]
type ListProp() =
    inherit div()

    [<Erase>]
    member val styles: (string * obj) list = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View = div(id = "lp").style' props.styles { "lp" }

[<SolidComponent>]
let ListPropConsumer () =
    let on, setOn = createSignal false

    div (id = "lpc") {
        ListProp(styles = [ Style.display Display.Grid; if on () then Style.color Color.Red ])
        button (class' = "toggle", onClick = fun _ -> setOn (not (on ()))) { "toggle" }
    }

/// A component that appends consumer StyleSpec overrides to its own base list (F# list ops on a list prop).
[<Erase>]
type Panel() =
    inherit div()

    [<Erase>]
    member val overrides: (string * obj) list = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        props.overrides <- []
        div(id = "panel").style' ([ Style.display Display.Block; Style.color Color.Black ] @ props.overrides) {
            $"{List.length props.overrides} overrides"
        }

[<SolidComponent>]
let PanelConsumer () =
    div (id = "pc") {
        Panel(overrides = [ Style.color Color.Red; "--pad" ==> "3px" ])
    }

[<SolidComponent>]
let PanelDefault () = Panel()

/// A consumer whose overrides list is a comprehension with a conditional element.
[<SolidComponent>]
let PanelConditional () =
    let warn, setWarn = createSignal true

    div (id = "pcd") {
        Panel(overrides = [ "--pad" ==> "3px"; if warn () then Style.color Color.Orange ])
        button (class' = "toggle", onClick = fun _ -> setWarn (not (warn ()))) { "toggle" }
    }

/// An anonymous record used as the style object with a custom property field.
[<SolidComponent>]
let RecordStyle () =
    let w, setW = createSignal 5

    div (id = "rs") {
        div(class' = "target").style' {| width = $"{w ()}px"; ``--depth`` = string (w ()) |} { "t" }
        button (class' = "inc", onClick = fun _ -> setW (w () + 5)) { "inc" }
    }
