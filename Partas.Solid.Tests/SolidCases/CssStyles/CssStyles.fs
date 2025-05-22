module Partas.Solid.Tests.SolidCases.CssStyles

open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop
open Partas.Solid
open Partas.Solid.Style
open Partas.Solid.Experimental.U

[<SolidComponent>]
let CssPropStyleTest () =
    div().style' [
        Style.backgroundColor BackgroundColor.Slategray
        "--custom-var" ==> "Chocolate"
    ]