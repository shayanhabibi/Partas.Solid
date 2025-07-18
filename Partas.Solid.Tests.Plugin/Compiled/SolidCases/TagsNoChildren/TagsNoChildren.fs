module Partas.Solid.Tests.SolidCases.TagsNoChildren

open Partas.Solid
open Fable.Core

type [<Erase>] MyTag() =
    interface RegularNode
    [<SolidTypeComponent>]
    member props.typeDef =
        div()

type [<Erase>] TagWithProp() =
    interface RegularNode
    [<SolidTypeComponent>]
    member props.typeDef =
        div(class' = "SomeClass")

type [<Erase>] TagWithProps() =
    interface RegularNode
    [<SolidTypeComponent>]
    member props.typeDef =
        div(class' = "SomeClass", draggable = "drag")


