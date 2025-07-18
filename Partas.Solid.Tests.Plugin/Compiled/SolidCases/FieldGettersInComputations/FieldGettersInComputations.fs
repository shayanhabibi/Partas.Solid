module Partas.Solid.Tests.SolidCases.FieldGettersInComputations

open Partas.Solid

type MenuItem = { Title: string; Url: string; Icon: HtmlElement }
module App =
    [<SolidComponent>]
    let App() =
        let items = [
            { Title = "Home"
              Url = "#"
              Icon = div() }
        ]
        div() {
            button()
            items[0].Icon
        }

