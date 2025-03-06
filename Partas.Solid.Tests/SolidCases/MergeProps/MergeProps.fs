module Partas.Solid.Tests.SolidCases.MergeProps

open Partas.Solid
open Fable.Core

type [<Erase>] SimpleMerge() =
    inherit RegularNode()
    [<SolidTypeComponent>]
    member props.SimpleMerge =
        props.class' <- "DefaultClass"
        div()
        
type [<Erase>] MultipleMerges() =
    inherit RegularNode()
    [<SolidTypeComponent>]
    member props.SimpleMerge =
        props.class' <- "DefaultClass"
        props.draggable <- "stub"
        props.dir <- "ltr"
        div()

type [<Erase>] ComplexPropMerge() =
    inherit RegularNode()
    [<SolidTypeComponent>]
    member props.SimpleMerge =
        props.onKeyPress <- fun _ -> System.Console.WriteLine "Key pressed"
        div()

type [<Erase>] SimpleNestedMerge() =
    inherit RegularNode()
    [<SolidTypeComponent>]
    member props.whatever =
        div() {
            button() {
                props.class' <- "defaultClass"
            }
        }

type [<Erase>] NestedMerge() =
    inherit RegularNode()
    [<SolidTypeComponent>]
    member props.SimpleMerge =
        div() {
            props.dir <- "ltr"
            div()
            button() {
                props.class' <- "DefaultClass"
                props.draggable <- "stub"
            }
        }
        
type [<Erase>] NestedMergeWithChildren() =
    inherit RegularNode()
    [<SolidTypeComponent>]
    member props.SimpleMerge =
        div() {
            props.dir <- "ltr"
            div()
            button() {
                props.class' <- "DefaultClass"
                "Button"
                props.draggable <- "stub"
            }
        }
        

