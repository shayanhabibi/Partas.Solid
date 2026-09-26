module Partas.Solid.Tests.Shared.LibraryImport

open Partas.Solid
open Fable.Core

[<Import("FakeImport", "FakeLibrary")>]
[<Erase>]
type Imported() =
    interface RegularNode
    member val importedProperty: int = jsNative with get, set

[<Erase>]
type CustomTag() =
    interface RegularNode
    member val userProperty: string = jsNative with get, set
