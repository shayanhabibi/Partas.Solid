module Partas.Solid.Tests.Runtime.Dom.Styles.StyleAttr

open Partas.Solid
open Partas.Solid.Style
open Partas.Solid.Experimental.U
open Fable.Core
open Fable.Core.JsInterop

// ---------------------------------------------------------------------------------------------
// Static StyleSpec lists
// ---------------------------------------------------------------------------------------------

/// Typed Style members with StringEnum values (kebab-cased compiled names).
[<SolidComponent>]
let TypedEnums () =
    div(id = "te").style' [
        Style.display Display.InlineFlex
        Style.position Position.Absolute
        Style.cursor Cursor.Pointer
        Style.textAlign TextAlign.Center
        Style.fontWeight FontWeight.Bold
        Style.visibility Visibility.Hidden
    ] { "enums" }

/// Typed Style members fed plain strings through the implicit U2 conversion.
[<SolidComponent>]
let TypedStrings () =
    div(id = "ts").style' [
        Style.width "50%"
        Style.marginTop "1.5em"
        Style.lineHeight "20px"
        Style.transform "translateX(10px)"
        Style.gridTemplateColumns "1fr 2fr"
        Style.gap "8px"
        Style.zIndex "3"
        Style.opacity "0.25"
    ] { "strings" }

/// Shorthand properties expand into their longhands.
[<SolidComponent>]
let Shorthands () =
    div(id = "sh").style' [ Style.margin "0 auto"; Style.padding "1px 2px 3px 4px" ] { "sh" }

/// A CSS-wide keyword (Globals) through a typed member.
[<SolidComponent>]
let GlobalKeyword () =
    div(id = "gk").style' [ Style.opacity Globals.Inherit; Style.color Color.Inherit ] { "gk" }

/// Units and CSS functions through the raw ==> operator.
[<SolidComponent>]
let Units () =
    div(id = "u").style' [
        "width" ==> "calc(100% - 10px)"
        "height" ==> "50vh"
        "font-size" ==> "1.25rem"
        "min-width" ==> "12ch"
        "max-width" ==> "80vw"
        "border-radius" ==> "4px 8px"
    ] { "units" }

/// Numeric values through ==> : unitless properties accept numbers.
[<SolidComponent>]
let Numbers () =
    div(id = "num").style' [ "opacity" ==> 0.5; "z-index" ==> 7; "flex-grow" ==> 2; "line-height" ==> 1.5 ] { "n" }

/// CSS custom properties and var() consumption.
[<SolidComponent>]
let CustomProps () =
    div(id = "cp").style' [
        "--brand" ==> "rgb(255, 0, 0)"
        "--gap-size" ==> "6px"
        Style.color "var(--brand)"
        "gap" ==> "var(--gap-size, 2px)"
    ] { "cp" }

/// A StyleSpec list bound once to a let and reused on two elements.
[<SolidComponent>]
let SharedList () =
    let card = [ Style.display Display.Block; "border" ==> "1px solid black" ]

    div () {
        div(id = "s1").style' card { "one" }
        div(id = "s2").style' card { "two" }
    }

/// The plain string `style` property with a custom property inside the string.
[<SolidComponent>]
let StringWithVar () =
    div (id = "swv", style = "--x: 3px; padding: var(--x); color: red") { "s" }

/// An empty StyleSpec list: no declarations.
[<SolidComponent>]
let EmptyList () =
    div(id = "empty").style' [] { "e" }

/// A StyleSpec on an SVG element.
[<SolidComponent>]
let SvgStyled () =
    Svg.svg (id = "svg", viewBox = "0 0 10 10") {
        Svg.circle(id = "circ", cx = 5.0, cy = 5.0, r = 5.0).style' [ SvgStyle.fill "red"; SvgStyle.strokeWidth 2 ]
    }

/// A style' on a component root together with a class and an attribute.
[<SolidComponent>]
let MixedWithAttrs () =
    span(id = "mx", class' = "tag", title = "t").style' [ Style.color Color.Green ] { "mixed" }

// ---------------------------------------------------------------------------------------------
// Reactive style driven by props
// ---------------------------------------------------------------------------------------------

/// A typed member fed by a prop.
[<Erase>]
type ColorBox() =
    inherit div()

    [<Erase>]
    member val fg: string = unbox null with get, set

    [<Erase>]
    member val width: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div(id = "cb").style' [ Style.color props.fg; "width" ==> props.width; Style.display Display.Block ] {
            "box"
        }

/// A plain string style from a prop.
[<Erase>]
type CssText() =
    inherit div()

    [<Erase>]
    member val css: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View = div (id = "ct", style = props.css) { "css" }

/// A whole style object handed in as a prop.
[<Erase>]
type StyleObjProp() =
    inherit div()

    [<Erase>]
    member val styles: obj = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View = div(id = "sop").style' props.styles { "obj" }

/// Consumer-provided style reaches the root via the spread of props.
[<Erase>]
type PassThrough() =
    inherit div()

    [<SolidTypeComponent>]
    member props.View = div(id = "pt").spread props { "pass" }

/// A custom property driven by a prop (CSS theming pattern).
[<Erase>]
type Themed() =
    inherit div()

    [<Erase>]
    member val accent: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div(id = "th").style' [ "--accent" ==> props.accent; Style.color "var(--accent)" ] { "themed" }

// ---------------------------------------------------------------------------------------------
// Reactive style driven by internal signals
// ---------------------------------------------------------------------------------------------

/// A progress bar: width computed from a numeric signal with a unit suffix.
[<SolidComponent>]
let Progress () =
    let pct, setPct = createSignal 10

    div (id = "prog") {
        div(class' = "bar").style' [ "width" ==> $"{pct ()}%%"; Style.height "4px" ]
        button (class' = "inc", onClick = fun _ -> setPct (pct () + 15)) { "+" }
    }

/// A property that disappears: the conditional value becomes null/undefined and must be removed.
[<SolidComponent>]
let RemovableProp () =
    let on, setOn = createSignal true

    div (id = "rp") {
        div(class' = "target").style' [
            "color" ==> (if on () then box "red" else null)
            "background-color" ==> (if on () then box "blue" else emitJsExpr () "undefined")
            "padding" ==> "2px"
        ] { "t" }
        button (class' = "toggle", onClick = fun _ -> setOn (not (on ()))) { "toggle" }
    }

/// A conditional list comprehension: declarations are yielded only when a flag is set.
[<SolidComponent>]
let ConditionalList () =
    let active, setActive = createSignal false

    div (id = "cl") {
        div(class' = "target").style' [
            Style.display Display.Block
            if active () then
                Style.color Color.Red
                "outline" ==> "1px solid red"
        ] { "t" }
        button (class' = "toggle", onClick = fun _ -> setActive (not (active ()))) { "toggle" }
    }

/// A whole style object swapped for another object with different keys.
[<SolidComponent>]
let SwapObject () =
    let alt, setAlt = createSignal false

    let styles () =
        if alt () then
            createObj [ "color", box "blue"; "margin-left", box "3px" ]
        else
            createObj [ "color", box "red"; "padding-top", box "5px" ]

    div (id = "so") {
        div(class' = "target").style' (styles ()) { "t" }
        button (class' = "toggle", onClick = fun _ -> setAlt (not (alt ()))) { "toggle" }
    }

/// A memoised StyleSpec list.
[<SolidComponent>]
let MemoList () =
    let size, setSize = createSignal 1

    let styles =
        createMemo (fun (_: (string * obj) list option) -> [ "font-size" ==> $"{size ()}em"; "--size" ==> string (size ()) ])

    div (id = "ml") {
        div(class' = "target").style' (styles ()) { "t" }
        button (class' = "grow", onClick = fun _ -> setSize (size () + 1)) { "grow" }
    }

/// A style driven by a store field.
[<SolidComponent>]
let StoreDriven () =
    let state, setState = createStore {| color = "red"; hidden = false |}

    div (id = "sd") {
        div(class' = "target").style' [
            Style.color state.Value.color
            Style.visibility (if state.Value.hidden then Visibility.Hidden else Visibility.Visible)
        ] { "t" }
        button (class' = "recolor", onClick = fun _ -> setState (fun s -> {| s with color = "blue" |})) { "recolor" }
        button (class' = "hide", onClick = fun _ -> setState (fun s -> {| s with hidden = true |})) { "hide" }
    }

/// A string style toggled to null: the whole style attribute goes away.
[<SolidComponent>]
let StringToNull () =
    let on, setOn = createSignal true

    div (id = "stn") {
        div (class' = "target", style = (if on () then "color: red" else unbox null)) { "t" }
        button (class' = "toggle", onClick = fun _ -> setOn (not (on ()))) { "toggle" }
    }

/// Switching between the string form and the object form of style.
[<SolidComponent>]
let StringThenObject () =
    let asObj, setAsObj = createSignal false

    div (id = "sto") {
        div(class' = "target").style' (
            if asObj () then
                createObj [ "margin-top", box "7px" ]
            else
                box "color: red; padding: 1px"
        ) { "t" }
        button (class' = "toggle", onClick = fun _ -> setAsObj (not (asObj ()))) { "toggle" }
    }
