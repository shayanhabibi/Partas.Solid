module Partas.Solid.Tests.SolidCases.FieldGetExpressionsTransformed

open Partas.Solid
open Fable.Core.JS
open Fable.Core
open Fable.Core.JsInterop

[<Erase>]
type WordRotate() =
    inherit div()
    [<Erase>] member val words: string[] = unbox null with get,set
    [<Erase>] member val duration: int = unbox null with get,set
    [<SolidTypeComponentAttribute>]
    member props.constructor =
        let index,setIndex = createSignal(0)
        let chog = props.words.Length
        div()

