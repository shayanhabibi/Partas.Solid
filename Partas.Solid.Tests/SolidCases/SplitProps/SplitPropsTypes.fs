module Partas.Solid.Tests.SolidCases.SplitPropsTypes

open Partas.Solid
open Fable.Core

[<Erase>]
type MyComponent() =
    interface RegularNode
    [<DefaultValue>]
    val mutable index: int
    
