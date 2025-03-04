module Partas.Solid.Tests.Shared.LibraryImport

open Partas.Solid
open Fable.Core

[<Import("FakeImport", "FakeLibrary")>]
type [<Erase>] Imported() =
    inherit RegularNode()
    member val importedProperty: int = jsNative with get,set

type [<Erase>] CustomTag() =
    inherit RegularNode()
    member val userProperty: string = jsNative with get,set