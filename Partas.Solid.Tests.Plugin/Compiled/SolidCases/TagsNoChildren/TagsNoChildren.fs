module Partas.Solid.Tests.SolidCases.TagsNoChildren

open Partas.Solid
open Fable.Core

[<Erase>]
type MyTag() =
    interface RegularNode

    [<SolidTypeComponent>]
    member props.typeDef = div ()

[<Erase>]
type TagWithProp() =
    interface RegularNode

    [<SolidTypeComponent>]
    member props.typeDef = div (class' = "SomeClass")

[<Erase>]
type TagWithProps() =
    interface RegularNode

    [<SolidTypeComponent>]
    member props.typeDef = div (class' = "SomeClass", draggable = "drag")
