module Partas.Solid.Tests

open Fable.Core.TS.Dom
open Fable.Core
open Partas.Solid.Aria
open Fable.Core.JsInterop
open Fable.Core.TS

[<Erase>]
type TransformGetterExtensions() =
    interface VoidNode

    [<Erase>]
    member val data: int[] = unbox null with get, set

    [<SolidTypeComponent(ComponentFlag.DebugMode)>]
    member props.constructor =
        For.Keyed() {  }
