module Partas.Solid.Tests.SolidCases.LibraryImport

open Partas.Solid
open Fable.Core
open Partas.Solid.Tests.Shared.LibraryImport


[<Erase>]
type MyTag() =
    interface RegularNode

    [<SolidTypeComponent>]
    member props.typeDef = Imported ()

[<Erase>]
type MyOtherTag() =
    interface RegularNode

    [<SolidTypeComponent>]
    member props.typeDef = CustomTag ()

[<Erase>]
type ImportedTagWithProps() =
    interface RegularNode

    [<SolidTypeComponent>]
    member props.typeDef = Imported (class' = "Test props")

[<Erase>]
type UserTagWithProps() =
    interface RegularNode

    [<SolidTypeComponent>]
    member props.typeDef = CustomTag (class' = "test bops")
