module Partas.Solid.Tests.SolidCases.SplitProps

open Partas.Solid
open Fable.Core
open Partas.Solid.Tests.SolidCases.SplitPropsTypes

[<Erase>]
type SplitProps() =
    interface RegularNode

    [<SolidTypeComponent>]
    member props.SplitProps =
        div (class' = props.class') { button (draggable = props.draggable) }

[<Erase>]
type NestedSplitProps() =
    interface RegularNode

    [<SolidTypeComponent>]
    member props.SplitProps =
        div () {
            button (class' = props.class')
            div () { button (onClick = (fun _ -> System.Console.WriteLine props.class')) }
            "text"
        }

[<Erase>]
type SplitValMutable() =
    interface RegularNode

    [<DefaultValue>]
    val mutable index: int

    [<SolidTypeComponent>]
    member props.__ = div (tabindex = props.index)

[<Erase>]
type SplitInheritedValMutable() =
    inherit MyComponent()

    [<SolidTypeComponent>]
    member props.__ = div (tabindex = props.index)
