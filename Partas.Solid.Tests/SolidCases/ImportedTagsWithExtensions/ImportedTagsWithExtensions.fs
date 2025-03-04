module Partas.Solid.Tests.SolidCases.ImportedTagsWithExtensions

open Partas.Solid.Tests.Shared.ImportedTagsWithExtensions
open Partas.Solid
open Fable.Core

type [<Erase>] NoPropsOneExtension() =
    inherit RegularNode()
    [<SolidTypeComponent>]
    member props.typeDef =
        Imported().attr("key", "value")

type [<Erase>] PropsWithExtension() =
    inherit RegularNode()
    [<SolidTypeComponent>]
    member props.typeDef =
        CustomTag(class' = "testClass").attr("key", "value")
        
type [<Erase>] NoPropsExtensionChain() =
    inherit RegularNode()
    [<SolidTypeComponent>]
    member props.typeDef =
        CustomTag().attr("key", "value").attr("key2", "value2")

type [<Erase>] PropsWithExtensionChain() =
    inherit RegularNode()
    [<SolidTypeComponent>]
    member props.typeDef =
        Imported(class' = "Tester", draggable = "drag").attr("key", "value").attr("key2", "value2").attr("key3", "value3").attr("key4", "value4")
