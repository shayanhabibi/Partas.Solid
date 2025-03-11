module Partas.Solid.Tests.SolidCases.TagsAsValuesSimple

open Partas.Solid
open Fable.Core
open Partas.Solid.Tests.SolidCases.TagsAsValuesSimpleTypes

[<Erase>]
type CustomTag() =
    inherit RegularNode()
    [<Erase>]
    member val icon: TagValue<_> = unbox null with get,set
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
        Comp % Imported(other = unbox !@ModuleTag )
    }