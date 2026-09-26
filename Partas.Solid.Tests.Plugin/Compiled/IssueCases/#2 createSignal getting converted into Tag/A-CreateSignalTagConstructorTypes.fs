module Partas.Solid.CreateSignalTagConstructor.Types

open Partas.Solid
open Fable.Core

[<Import("FakeImport", "FakeLibrary")>]
[<Erase>]
type Imported() =
    interface RegularNode
