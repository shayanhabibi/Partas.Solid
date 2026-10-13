module Partas.Solid.Tests.SolidCases.TagExtensions

open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop

[<Erase>]
type NoPropsOneExtension() =
    interface RegularNode

    [<SolidTypeComponent>]
    member props.typeDef = div().attr ("key", "value")

[<Erase>]
type PropsWithExtension() =
    interface RegularNode

    [<SolidTypeComponent>]
    member props.typeDef = div(class' = "testClass").attr ("key", "value")

[<Erase>]
type NoPropsExtensionChain() =
    interface RegularNode

    [<SolidTypeComponent>]
    member props.typeDef = div().attr("key", "value").attr ("key2", "value2")

[<Erase>]
type PropsWithExtensionChain() =
    interface RegularNode

    [<SolidTypeComponent>]
    member props.typeDef =
        div(class' = "Tester", draggable = Attr.Draggable.True).attr("key", "value").attr("key2", "value2").attr("key3", "value3").attr ("key4", "value4")

[<Erase>]
type ClassObjExtension() =
    interface RegularNode

    [<SolidTypeComponent>]
    member props.typeDef =
        div().classObj (createObj [ "active", box true; "disabled", box false ])

