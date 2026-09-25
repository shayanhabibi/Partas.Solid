module Partas.Solid.Tests.Runtime.Dom.Elements.SvgCases

open Partas.Solid
open Partas.Solid.Experimental.U
open Fable.Core

/// An inline SVG icon with namespaced children and typical attributes.
[<SolidComponent>]
let Icon () =
    Svg.svg (
        id = "icon",
        viewBox = "0 0 24 24",
        width = 24.0,
        height = "24",
        fill = "none",
        xmlns = "http://www.w3.org/2000/svg"
    ) {
        Svg.path (id = "p", d = "M0 0L24 24", stroke = "red", ``stroke-width`` = 2.0)
        Svg.circle (id = "c", cx = 12.0, cy = 12.0, r = 5.0)
        Svg.g (id = "grp", class' = "group") {
            Svg.rect (id = "r", x = 1.0, y = "2", width = 3.0, height = 4.0)
        }
        Svg.defs () { Svg.symbol (id = "sym", viewBox = "0 0 10 10") { Svg.circle (cx = 5.0, cy = 5.0, r = 5.0) } }
        Svg.use' (id = "u1", href = "#sym")
        Svg.use'(id = "u2").attr ("xlink:href", "#sym")
        Svg.text (id = "t", x = 0.0, y = 10.0) { "label" }
    }

/// An SVG child element rendered from its own component (no <svg> parent at compile time).
[<SolidComponent>]
let Dot () =
    Svg.circle (id = "dot", cx = 1.0, cy = 1.0, r = 1.0)

[<SolidComponent>]
let Chart () =
    Svg.svg (id = "chart", viewBox = "0 0 2 2") { Dot () }
