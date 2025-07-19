module Partas.Solid.Tests.SolidCases.MergeProps

open Partas.Solid
open Fable.Core
open Partas.Solid.Tests.SolidCases.MergePropsTypes

[<Erase>]
type SimpleMerge() =
    interface RegularNode

    [<SolidTypeComponent>]
    member props.SimpleMerge =
        props.class' <- "DefaultClass"
        div ()

[<Erase>]
type MultipleMerges() =
    interface RegularNode

    [<SolidTypeComponent>]
    member props.SimpleMerge =
        props.class' <- "DefaultClass"
        props.draggable <- "stub"
        props.dir <- "ltr"
        div ()

[<Erase>]
type ComplexPropMerge() =
    interface RegularNode

    [<SolidTypeComponent>]
    member props.SimpleMerge =
        props.onKeyPress <- fun _ -> System.Console.WriteLine "Key pressed"
        div ()

[<Erase>]
type SimpleNestedMerge() =
    interface RegularNode

    [<SolidTypeComponent>]
    member props.whatever = div () { button () { props.class' <- "defaultClass" } }

[<Erase>]
type NestedMerge() =
    interface RegularNode

    [<SolidTypeComponent>]
    member props.SimpleMerge =
        div () {
            props.dir <- "ltr"
            div ()

            button () {
                props.class' <- "DefaultClass"
                props.draggable <- "stub"
            }
        }

[<Erase>]
type NestedMergeWithChildren() =
    interface RegularNode

    [<SolidTypeComponent>]
    member props.SimpleMerge =
        div () {
            props.dir <- "ltr"
            div ()

            button () {
                props.class' <- "DefaultClass"
                "Button"
                props.draggable <- "stub"
            }
        }

[<Erase>]
type ValMutableMerge() =
    interface RegularNode

    [<DefaultValue>]
    val mutable index: int

    [<SolidTypeComponent>]
    member props.__ =
        props.index <- 5
        div ()

[<Erase>]
type ValMutableMergeInherited() =
    inherit MyComponent()

    [<SolidTypeComponent>]
    member props.__ =
        props.index <- 5
        div ()
