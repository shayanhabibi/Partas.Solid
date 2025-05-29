module Partas.Solid.Tests.IssueCases.EmptyCE

open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop

let cn (listofclasses: string list) = listofclasses |> List.fold ((+) >> id) ""
[<SolidComponent(ComponentFlag.DebugMode)>]
let Compponent (show: bool) =
    div(class' = cn [
        "testcase"
        if show then
            "showthis"
        "i'm always here"
    ]) {
        if show then "I show sometimes!"
        "I show always!"
        For(each = !![
            1;2;3
            if show then
                4
            5
        ]) {
        }
    }