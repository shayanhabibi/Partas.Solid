module Partas.Solid.Tests.Runtime.Dom.Styles.ClassAttr

open System
open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop

/// clsx-like helper (real apps use this to build class strings).
let cn (classes: string array) =
    classes |> Array.filter (fun c -> not (String.IsNullOrEmpty c)) |> String.concat " "

/// A static class string with extra whitespace between tokens.
[<SolidComponent>]
let StaticClass () =
    div (id = "sc", class' = "  a   b c ") { "s" }

/// Solid 2 accepts arrays (clsx-style), including nested arrays and object entries.
[<SolidComponent>]
let ArrayClass () =
    div (id = "ac", class' = !!([| box "a"; box [| "b"; "c" |]; box (createObj [ "d", box true; "e", box false ]) |])) {
        "arr"
    }

/// An object map whose keys contain several space-separated class names.
[<SolidComponent>]
let MultiTokenKeys () =
    div (id = "mt", class' = !!(createObj [ "p-2 rounded", box true; "shadow hidden", box false ])) { "mt" }

/// An anonymous-record class map with a quoted key.
[<SolidComponent>]
let RecordClassMap () =
    div (id = "rcm", class' = !!{| ``is-open`` = true; closed = false |}) { "r" }

/// A class computed through a helper with an optional user class.
[<Erase>]
type Badge() =
    inherit span()

    [<Erase>]
    member val tone: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        props.tone <- "neutral"
        span (class' = cn [| "badge"; "badge-" + props.tone; props.class' |]) { props.children }

/// A class string from a prop.
[<Erase>]
type ClassProp() =
    inherit div()

    [<Erase>]
    member val cls: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View = div (id = "cp", class' = props.cls) { "cp" }

/// A class object map from a prop.
[<Erase>]
type ClassMapProp() =
    inherit div()

    [<Erase>]
    member val selected: bool = unbox null with get, set

    [<Erase>]
    member val disabled: bool = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (id = "cmp", class' = !!(createObj [ "item", box true; "selected", box props.selected; "disabled", box props.disabled ])) {
            "item"
        }

/// A user class passed to a component whose root spreads unconsumed props.
[<Erase>]
type Card() =
    inherit div()

    [<SolidTypeComponent>]
    member props.View = div(id = "card", class' = cn [| "card"; props.class' |]).spread props { "card" }

/// A class toggled between a string and nothing (null removes the attribute).
[<SolidComponent>]
let ToggleClass () =
    let on, setOn = createSignal true

    div (id = "tc") {
        div (class' = (if on () then "active" else unbox null)) { "t" }
        button (class' = "toggle", onClick = fun _ -> setOn (not (on ()))) { "toggle" }
    }

/// A class object map driven by signals: only the changed classes toggle.
[<SolidComponent>]
let SignalClassMap () =
    let open', setOpen = createSignal false
    let err, setErr = createSignal false

    div (id = "scm") {
        div (class' = !!(createObj [ "panel", box true; "open", box (open' ()); "error", box (err ()) ])) { "p" }
        button (class' = "open-btn", onClick = fun _ -> setOpen (not (open' ()))) { "open" }
        button (class' = "err-btn", onClick = fun _ -> setErr (not (err ()))) { "err" }
    }

/// A class array driven by a signal.
[<SolidComponent>]
let SignalClassArray () =
    let size, setSize = createSignal "sm"

    div (id = "sca") {
        div (class' = !!([| box "btn"; box ("btn-" + size ()); box (createObj [ "big", box (size () = "lg") ]) |])) { "b" }
        button (class' = "lg", onClick = fun _ -> setSize "lg") { "lg" }
    }

/// A class switched from a string to an object map.
[<SolidComponent>]
let StringThenMap () =
    let asMap, setAsMap = createSignal false

    div (id = "stm") {
        div (class' = (if asMap () then !!(createObj [ "m1", box true; "m2", box false ]) else "s1 s2")) { "t" }
        button (class' = "toggle", onClick = fun _ -> setAsMap (not (asMap ()))) { "toggle" }
    }

/// Tabs: the selected tab gets a class through a comparison.
[<SolidComponent>]
let Tabs () =
    let current, setCurrent = createSignal 0

    div (id = "tabs") {
        For.Keyed (each = [| "One"; "Two"; "Three" |]) {
            yield
                fun label index ->
                    button (class' = (if current () = index () then "tab selected" else "tab"), onClick = fun _ -> setCurrent (index ())) {
                        label
                    }
        }
    }

/// class and style both driven by the same signal.
[<SolidComponent>]
let ClassAndStyle () =
    let danger, setDanger = createSignal false

    div (id = "cas") {
        div(class' = (if danger () then "alert danger" else "alert")).style' [ "color" ==> (if danger () then "red" else "black") ] {
            "a"
        }
        button (class' = "toggle", onClick = fun _ -> setDanger (not (danger ()))) { "toggle" }
    }

/// An SVG element with a class.
[<SolidComponent>]
let SvgClass () =
    Svg.svg (id = "svgc", class' = "icon icon-lg", viewBox = "0 0 1 1") { Svg.rect (class' = "fill", width = 1.0, height = 1.0) }
