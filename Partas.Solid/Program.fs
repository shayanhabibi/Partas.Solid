module Partas.Solid.Program

open Partas.Solid
open Partas.Solid.Router
open Partas.Solid.Meta
open Partas.Solid.Start
open Fable.Core

[<ImportMember("@kobalte/solidbase/client")>]
type SolidBaseRoot() =
    interface VoidNode

[<SolidComponent>]
let Program () =
    Router(root = !@SolidBaseRoot) {
        unbox<Route>(FileRoutes())
    }
