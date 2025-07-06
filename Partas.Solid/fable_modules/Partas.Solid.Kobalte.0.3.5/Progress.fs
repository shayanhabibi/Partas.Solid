namespace Partas.Solid.Kobalte

open Fable.Core
open Partas.Solid


[<Erase; Import("Root", Spec.progress)>]
type Progress() =
    inherit div()
    interface Polymorph
    [<DefaultValue>] val mutable value : int 
    [<DefaultValue>] val mutable minValue : int 
    [<DefaultValue>] val mutable maxValue : int 
    [<DefaultValue>] val mutable getValueLabel : {| value: int ; min : int ; max : int |} -> string 
    [<DefaultValue>] val mutable indeterminate : bool 

[<Erase; RequireQualifiedAccess>]
module Progress =
    [<Erase; Import("Label", Spec.progress)>]
    type Label() =
        inherit span()
        interface Polymorph
    [<Erase; Import("ValueLabel", Spec.progress)>]
    type ValueLabel() =
        inherit div()
        interface Polymorph
    [<Erase; Import("Track", Spec.progress)>]
    type Track() =
        inherit div()
        interface Polymorph
    [<Erase; Import("Fill", Spec.progress)>]
    type Fill() =
        inherit div()
        interface Polymorph

