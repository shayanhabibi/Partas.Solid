﻿module Partas.Solid.Tests.SolidCases.ChildrenSimple

open Partas.Solid
open Fable.Core

type [<Erase>] TypeDefTest() =
    inherit RegularNode()
    [<SolidTypeComponent>]
    member props.typeDef =
        button() {
            "Button"
        }
type [<Erase>] TypeDefTestWithProps() =
    inherit RegularNode()
    [<SolidTypeComponent>]
    member props.typeDef =
        button(class' = "ButtonClass", draggable="indubitably") {
            "Button"
            span(class' = "sr-only") { "Span" }
        } 
type [<Erase>] TypeDefTestWithExtensions() =
    inherit RegularNode()
    [<SolidTypeComponent>]
    member props.typeDef =
        button(class' = "ButtonClass").attr("Key", "Value") {
            "Button"
            span(class' = "sr-only") { "Span" }
            div(class' = "DivBelow").attr("key", "value")
            "TextBelow"
        } 
        