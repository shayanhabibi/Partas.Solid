module Partas.Solid.Tests.SolidCases.TagsNoChildren

open Partas.Solid
open Fable.Core

type [<Erase>] MyTag() =
    inherit RegularNode()
    [<SolidTypeComponent>]
    member props.typeDef =
        div()

type [<Erase>] TagWithProp() =
    inherit RegularNode()
    [<SolidTypeComponent>]
    member props.typeDef =
        div(class' = "SomeClass")

type [<Erase>] TagWithProps() =
    inherit RegularNode()
    [<SolidTypeComponent>]
    member props.typeDef =
        div(class' = "SomeClass", draggable = "drag")


