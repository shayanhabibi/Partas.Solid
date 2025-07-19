module Partas.Solid.Tests.SolidCases.ChildrenSimple

open Partas.Solid
open Fable.Core

[<Erase>]
type TypeDefTest() =
    interface RegularNode

    [<SolidTypeComponent>]
    member props.typeDef = button () { "Button" }

[<Erase>]
type TypeDefTestWithProps() =
    interface RegularNode

    [<SolidTypeComponent>]
    member props.typeDef =
        button (class' = "ButtonClass", draggable = "indubitably") {
            "Button"
            span (class' = "sr-only") { "Span" }
        }

[<Erase>]
type TypeDefTestWithExtensions() =
    interface RegularNode

    [<SolidTypeComponent>]
    member props.typeDef =
        button(class' = "ButtonClass").attr ("Key", "Value") {
            "Button"
            span (class' = "sr-only") { "Span" }
            div(class' = "DivBelow").attr ("key", "value")
            "TextBelow"
        }
