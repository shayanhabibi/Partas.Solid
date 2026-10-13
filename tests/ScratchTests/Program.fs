module Partas.Solid.Tests

open Browser.Types
open Fable.Core
open Partas.Solid.Aria
open Fable.Core.JsInterop

[<Erase>]
type TransformGetterExtensions() =
    interface VoidNode

    [<Erase>]
    member val data: int[] = unbox null with get, set

    [<SolidTypeComponent(ComponentFlag.DebugMode)>]
    member props.constructor =
        For.Keyed() {  }
