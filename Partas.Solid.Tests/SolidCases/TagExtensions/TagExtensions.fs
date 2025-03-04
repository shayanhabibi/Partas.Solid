module Partas.Solid.Tests.SolidCases.TagExtensions

open Partas.Solid
open Fable.Core

type [<Erase>] NoPropsOneExtension() =
    inherit RegularNode()
    [<SolidTypeComponent>]
    member props.typeDef =
        div().attr("key", "value")

type [<Erase>] PropsWithExtension() =
    inherit RegularNode()
    [<SolidTypeComponent>]
    member props.typeDef =
        div(class' = "testClass").attr("key", "value")
        
type [<Erase>] NoPropsExtensionChain() =
    inherit RegularNode()
    [<SolidTypeComponent>]
    member props.typeDef =
        div().attr("key", "value").attr("key2", "value2")

type [<Erase>] PropsWithExtensionChain() =
    inherit RegularNode()
    [<SolidTypeComponent>]
    member props.typeDef =
        div(class' = "Tester", draggable = "drag").attr("key", "value").attr("key2", "value2").attr("key3", "value3").attr("key4", "value4")