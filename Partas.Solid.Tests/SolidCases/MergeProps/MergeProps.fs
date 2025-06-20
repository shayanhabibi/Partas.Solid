﻿module Partas.Solid.Tests.SolidCases.MergeProps

open Partas.Solid
open Fable.Core
open Partas.Solid.Tests.SolidCases.MergePropsTypes

type [<Erase>] SimpleMerge() =
    interface RegularNode
    [<SolidTypeComponent>]
    member props.SimpleMerge =
        props.class' <- "DefaultClass"
        div()
        
type [<Erase>] MultipleMerges() =
    interface RegularNode
    [<SolidTypeComponent>]
    member props.SimpleMerge =
        props.class' <- "DefaultClass"
        props.draggable <- "stub"
        props.dir <- "ltr"
        div()

type [<Erase>] ComplexPropMerge() =
    interface RegularNode
    [<SolidTypeComponent>]
    member props.SimpleMerge =
        props.onKeyPress <- fun _ -> System.Console.WriteLine "Key pressed"
        div()

type [<Erase>] SimpleNestedMerge() =
    interface RegularNode
    [<SolidTypeComponent>]
    member props.whatever =
        div() {
            button() {
                props.class' <- "defaultClass"
            }
        }

type [<Erase>] NestedMerge() =
    interface RegularNode
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
    interface RegularNode
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
        
type [<Erase>] ValMutableMerge() =
    interface RegularNode
    [<DefaultValue>]
    val mutable index: int
    [<SolidTypeComponent>]
    member props.__ =
        props.index <- 5
        div()

type [<Erase>] ValMutableMergeInherited() =
    inherit MyComponent()
    [<SolidTypeComponent>]
    member props.__ =
        props.index <- 5
        div()
