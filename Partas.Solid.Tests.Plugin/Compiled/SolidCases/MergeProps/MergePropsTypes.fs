module Partas.Solid.Tests.SolidCases.MergePropsTypes

open Partas.Solid
open Fable.Core

[<Erase>]
type MyComponent() =
    interface RegularNode
    [<DefaultValue>]
    val mutable index: int
