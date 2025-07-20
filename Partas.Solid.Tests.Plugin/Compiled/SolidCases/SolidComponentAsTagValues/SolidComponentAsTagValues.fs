module Partas.Solid.Tests.SolidCases.SolidComponentAsTagValues.SolidComponentAsTagValues

open Partas.Solid.Tests.SolidCases.SolidComponentAsTagValues.SolidComponentAsTagValuesTypes
open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop

[<Erase>]
type CustomTag() =
    interface RegularNode
    [<Erase>]
    member val icon: TagValue = unbox null with get,set
    [<SolidTypeComponent>]
    member props.constructor =
        props.icon <- !@button
        div() {
            props.icon % {| class' = "KeyVal" |}
            props.icon % {| class' = "KeyVal2" |}
            props.icon % div(class' = "constructor") { button() { "internal" } }

        }

[<SolidComponent>]
let Rock () =
    let Comp = !@Imported
    CustomTag(icon = unbox !@a) {
        Comp % Imported(other = !@ModuleTag )
    }
