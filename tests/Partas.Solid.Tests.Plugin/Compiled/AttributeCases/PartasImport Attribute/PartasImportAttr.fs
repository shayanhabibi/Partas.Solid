module Partas.Solid.Tests.AttributeCases.PartasProxyImportAttr

open Partas.Solid
open Partas.Solid.Router
open Fable.Core

// todo-replace with a better test
[<Erase>]
type PartasImportAttr() =
    interface VoidNode

    [<SolidTypeComponent>]
    member props.constructor = Router () { Route () }
