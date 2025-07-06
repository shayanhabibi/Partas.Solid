namespace Partas.Solid.Kobalte

open Partas.Solid
open Fable.Core

[<Erase; Import("Root", Spec.alert)>]
type Alert() =
    inherit div() //v0.13.9
    interface Polymorph //v0.13.9
