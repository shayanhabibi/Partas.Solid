module Partas.Solid.Tests.SolidCases.AriaAttributes

open Partas.Solid
open Partas.Solid.Aria

[<SolidComponent>]
let AriaAttributes (isOpen: unit -> bool) =
    div (
        role = Attr.AriaRole.Dialog,
        ariaLabelledBy = "title",
        ariaExpanded = isOpen (),
        ariaModal = true,
        ariaHidden = false,
        ariaDisabled = isOpen ()
    ) { "content" }
