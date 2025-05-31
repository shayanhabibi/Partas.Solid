module Partas.Solid.Tests.IssueCases.TransformInsideStringInterpolation

open Partas.Solid
open Fable.Core

[<Erase>]
type TestStringInterpolation() =
    interface VoidNode
    [<Erase>]
    member val index: int = unbox null with get,set
    [<SolidTypeComponentAttribute>]
    member props.constructor =
        div().style' {| paddingLeft = $"{props.index}px" |} {
            div().style' {| paddingLeft = $"{props.index}px" |} {
                div().style' {| paddingLeft = $"{props.index / 2}px" |}
            }
        } 
