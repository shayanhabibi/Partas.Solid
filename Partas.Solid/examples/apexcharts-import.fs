module Partas.Solid.examples.apexcharts_import

open Partas.Solid
open Partas.Solid.Start
open Fable.Core
open Fable.Core.JsInterop
open Fable.Core.JS

let private ClientOnly = clientOnly(fun () -> importComponent "./apexcharts.fs.jsx")

[<SolidComponent>]
let ApexChartEg () =
    ClientOnly % div()
