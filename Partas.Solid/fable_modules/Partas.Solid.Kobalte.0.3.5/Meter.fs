namespace Partas.Solid.Kobalte

open Fable.Core
open Partas.Solid


[<Erase; Import("Root", Spec.meter)>]
type Meter() =
    inherit div()
    interface Polymorph
    [<DefaultValue>] val mutable value : int 
    [<DefaultValue>] val mutable minValue : int 
    [<DefaultValue>] val mutable maxValue : int 
    [<DefaultValue>] val mutable getValueLabel : {| value: int ; min : int ; max : int |} -> string 

[<Erase; RequireQualifiedAccess>]
module Meter =
    [<Erase; Import("Label", Spec.meter)>]
    type Label() =
        inherit span()
        interface Polymorph
    [<Erase; Import("ValueLabel", Spec.meter)>]
    type ValueLabel() =
        inherit div()
        interface Polymorph
    [<Erase; Import("Track", Spec.meter)>]
    type Track() =
        inherit div()
        interface Polymorph
    [<Erase; Import("Fill", Spec.meter)>]
    type Fill() =
        inherit div()
        interface Polymorph


