namespace Partas.Solid.Kobalte

open Fable.Core
open Partas.Solid

/// <summary>
/// data-disabled: present when the button is disabled
/// </summary>
[<Erase; Import("Root", Spec.button)>]
type Button() =
    inherit button()
    interface Polymorph
    [<DefaultValue>]
    val mutable disabled : bool
