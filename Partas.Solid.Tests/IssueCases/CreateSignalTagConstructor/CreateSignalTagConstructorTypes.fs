module Partas.Solid.CreateSignalTagConstructor.Types

open Partas.Solid
open Fable.Core

[<Import("FakeImport", "FakeLibrary")>]
type [<Erase>] Imported() =
    interface RegularNode
