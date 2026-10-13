module Partas.Solid.Tests.SolidCases.LocalTagValues

open Partas.Solid
open Fable.Core

[<Erase>]
type Pill() =
    inherit span()

    [<SolidTypeComponent>]
    member props.View = span(class' = "pill").spread props { props.children }

[<SolidComponent>]
let SolidPill () = span (class' = "solid-pill")

[<SolidComponent>]
let LocalTag () =
    let Wrapper = !@Pill
    Wrapper % span (title = "local") { "local" }

[<SolidComponent>]
let LocalTagRecord () =
    let Wrapper = !@Pill
    Wrapper % {| title = "rec" |}

[<SolidComponent>]
let LocalTagLet () =
    let Wrapper = !@SolidPill
    Wrapper % {| title = "let" |}
