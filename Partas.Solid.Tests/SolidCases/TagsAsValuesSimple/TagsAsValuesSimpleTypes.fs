module Partas.Solid.Tests.SolidCases.TagsAsValuesSimpleTypes

open Partas.Solid
open Fable.Core

#nowarn 64
    
[<Erase>]
type ModuleTag() =
    inherit RegularNode()
    [<Erase>]
    member val other: TagValue = jsNative with get,set
    [<SolidTypeComponent>]
    member props.constructor =
        div()
        
[<Import("FakeImportedTag", "fakeLibrary")>]
type Imported() =
    inherit RegularNode()
    [<Erase>]
    member val other: TagValue = jsNative with get,set
