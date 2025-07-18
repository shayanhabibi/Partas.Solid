module Partas.Solid.Tests.SolidCases.SplitProps

open Partas.Solid
open Fable.Core
open Partas.Solid.Tests.SolidCases.SplitPropsTypes

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

type [<Erase>] SplitValMutable() =
    interface RegularNode
    [<DefaultValue>]
    val mutable index: int
    [<SolidTypeComponent>]
    member props.__ =
        div(tabindex = props.index)

type [<Erase>] SplitInheritedValMutable() =
    inherit MyComponent()
    [<SolidTypeComponent>]
    member props.__ =
        div(tabindex = props.index)
