module Partas.Solid.Tests.SolidCases.SolidComponentAsTagValues.SolidComponentAsTagValuesTypes

open Partas.Solid
open Fable.Core

#nowarn 64

[<SolidComponent>]
let ModuleTag() =
    div()

[<Import("FakeImportedTag", "fakeLibrary")>]
type Imported() =
    interface RegularNode
    [<Erase>]
    member val other: TagValue = jsNative with get,set
