module Partas.Solid.Tests.SolidCases.SplitProps

open Partas.Solid
open Fable.Core

type [<Erase>] SplitProps() =
    interface RegularNode
    [<SolidTypeComponent>]
    member props.SplitProps =
        div(class' = props.class') {
            button(draggable = props.draggable)
        }

type [<Erase>] NestedSplitProps() =
    interface RegularNode
    [<SolidTypeComponent>]
    member props.SplitProps =
        div() {
            button(class' = props.class')
            div() {
                button(onClick = (fun _ -> System.Console.WriteLine props.class'))
            }
            "text"
        }

    
