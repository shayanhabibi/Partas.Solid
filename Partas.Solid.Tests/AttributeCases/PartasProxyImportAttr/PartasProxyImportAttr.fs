module Partas.Solid.Tests.AttributeCases.PartasImportAttr

open Partas.Solid
open Partas.Solid.Router
open Fable.Core

[<Erase; PartasProxyImport("div", "motion","motion-one")>]
type Test() =
    inherit RegularNode()

[<SolidComponent>]
let Bock () =
    Test()

[<SolidComponent>]
let chock () =
    Route()