module Partas.Solid.Tests.SolidCases.CombinedSpread

open Partas.Solid
open Fable.Core

type [<Erase>] SimpleCombination() =
    inherit RegularNode()
    [<SolidTypeComponent>]
    member props.CombinedSpread =
        props.class' <- "ClassDefault"
        div() {
            "Some text"
            span(class' = props.class') { "More text" }
            div().spread(props) {
                "All but class have been spread into this tag!"
            }
            
        }

