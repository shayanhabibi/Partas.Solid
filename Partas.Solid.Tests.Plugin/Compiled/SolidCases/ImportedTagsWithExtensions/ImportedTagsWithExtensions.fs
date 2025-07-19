module Partas.Solid.Tests.SolidCases.ImportedTagsWithExtensions

open Partas.Solid.Tests.Shared.ImportedTagsWithExtensions
open Partas.Solid
open Fable.Core

[<Erase>]
type NoPropsOneExtension() =
    interface RegularNode

    [<SolidTypeComponent>]
    member props.typeDef = Imported().attr ("key", "value")

[<Erase>]
type PropsWithExtension() =
    interface RegularNode

    [<SolidTypeComponent>]
    member props.typeDef = CustomTag(class' = "testClass").attr ("key", "value")

[<Erase>]
type NoPropsExtensionChain() =
    interface RegularNode

    [<SolidTypeComponent>]
    member props.typeDef = CustomTag().attr("key", "value").attr ("key2", "value2")

[<Erase>]
type PropsWithExtensionChain() =
    interface RegularNode

    [<SolidTypeComponent>]
    member props.typeDef =
        Imported(class' = "Tester", draggable = "drag").attr("key", "value").attr("key2", "value2").attr("key3", "value3").attr ("key4", "value4")
