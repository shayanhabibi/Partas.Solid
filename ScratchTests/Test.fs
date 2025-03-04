module Partas.Solid.Test.Burger

open Partas.Solid
open Partas.Solid.Test
open Fable.Core


type [<Erase>] TagWithTextChild() =
    inherit RegularNode()
    [<SolidTypeComponent>]
    member props.typeDef =
        div(class' = "boobs") {
                 "textabove"
                 div()
                 "textbelow"
            }
        