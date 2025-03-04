module Partas.Solid.Tests.SolidCases.LibraryImport

open Partas.Solid
open Fable.Core
open Partas.Solid.Tests.Shared.LibraryImport


type [<Erase>] MyTag() =
    inherit RegularNode()
    [<SolidTypeComponent>]
    member props.typeDef =
        Imported()

type [<Erase>] MyOtherTag() =
    inherit RegularNode()
    [<SolidTypeComponent>]
    member props.typeDef =
        CustomTag()
        
type [<Erase>] ImportedTagWithProps() =
    inherit RegularNode()
    [<SolidTypeComponent>]
    member props.typeDef =
        Imported(class' = "Test props")

type [<Erase>] UserTagWithProps() =
    inherit RegularNode()
    [<SolidTypeComponent>]
    member props.typeDef =
        CustomTag(class' = "test bops")
