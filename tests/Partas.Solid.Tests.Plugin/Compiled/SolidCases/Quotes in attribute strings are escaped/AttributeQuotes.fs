module Partas.Solid.Tests.SolidCases.AttributeQuotes

open Partas.Solid

[<SolidComponent>]
let AttributeQuotes () =
    div (title = "say \"hi\"", class' = "a & b", innerHTML = "<p class=\"row\">x &amp; y</p>") { "text" }
