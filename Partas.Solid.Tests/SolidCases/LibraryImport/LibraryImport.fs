module Partas.Solid.Tests.SolidCases.LibraryImport

open Partas.Solid
open Fable.Core
open Partas.Solid.Tests.Shared.LibraryImport


type [<Erase>] MyTag() =
    interface RegularNode
    [<SolidTypeComponent>]
    member props.typeDef =
        Imported()

type [<Erase>] MyOtherTag() =
    interface RegularNode
    [<SolidTypeComponent>]
    member props.typeDef =
        CustomTag()
        
type [<Erase>] ImportedTagWithProps() =
    interface RegularNode
    [<SolidTypeComponent>]
    member props.typeDef =
        Imported(class' = "Test props")

type [<Erase>] UserTagWithProps() =
    interface RegularNode
    [<SolidTypeComponent>]
    member props.typeDef =
        CustomTag(class' = "test bops")
