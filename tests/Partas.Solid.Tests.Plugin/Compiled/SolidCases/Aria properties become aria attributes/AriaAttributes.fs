module Partas.Solid.Tests.SolidCases.AriaAttributes

open Partas.Solid
open Partas.Solid.Aria

[<SolidComponent>]
let AriaAttributes (isOpen: unit -> bool) =
    div (
        role = "dialog",
        ariaLabelledBy = "title",
        ariaExpanded = (if isOpen () then "true" else "false"),
        ariaModal = true,
        ariaHidden = false,
        ariaDisabled = isOpen ()
    ) { "content" }
