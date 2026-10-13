module Partas.Solid.Tests.AttributeCases.PartasProxyImportAttr

open Partas.Solid
open Fable.Core

// Two components imported through [<PartasImport>], which (unlike [<Import>]) leaves builder parameter names alone.
[<Erase; PartasImport("Router", "@solidjs/router")>]
type Router() =
    interface HtmlContainer

[<Erase; PartasImport("Route", "@solidjs/router")>]
type Route() =
    interface HtmlElement

[<Erase>]
type PartasImportAttr() =
    interface VoidNode

    [<SolidTypeComponent>]
    member props.constructor = Router () { Route () }
