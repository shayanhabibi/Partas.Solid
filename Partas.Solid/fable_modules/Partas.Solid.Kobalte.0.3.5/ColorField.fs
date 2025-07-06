namespace Partas.Solid.Kobalte

open Partas.Solid
open Fable.Core

[<Import("Root", Spec.colorField)>]
type ColorField() =
    inherit TextField()
    interface Polymorph

[<Erase;RequireQualifiedAccess>]
module ColorField =
    [<Import("Description", Spec.colorField)>]
    type Description() =
        inherit TextField.Description()
        interface Polymorph
    [<Import("ErrorMessage", Spec.colorField)>]
    type ErrorMessage() =
        inherit TextField.ErrorMessage()
        interface Polymorph
    [<Import("Input", Spec.colorField)>]
    type Input() =
        inherit input()
        interface Polymorph
    [<Import("Label", Spec.colorField)>]
    type Label() =
        inherit TextField.Label()
        interface Polymorph
