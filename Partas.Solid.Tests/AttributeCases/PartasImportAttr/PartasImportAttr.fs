module Partas.Solid.Tests.AttributeCases.PartasImportAttr

open Partas.Solid
open Partas.Solid.Router
open Fable.Core

// todo-replace with a better test
[<Erase>]
type PartasImportAttr() =
    inherit VoidNode()
    [<SolidTypeComponentAttribute>]
    member props.constructor =
        Router() {
            Route()
        }
