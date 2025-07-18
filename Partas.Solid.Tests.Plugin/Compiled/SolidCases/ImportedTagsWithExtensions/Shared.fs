module Partas.Solid.Tests.Shared.ImportedTagsWithExtensions

open Partas.Solid
open Fable.Core

[<Import("FakeImport", "FakeLibrary")>]
type [<Erase>] Imported() =
    interface RegularNode
    member val importedProperty: int = jsNative with get,set

type [<Erase>] CustomTag() =
    interface RegularNode
    member val userProperty: string = jsNative with get,set
