namespace Partas.Solid.Kobalte

open Partas.Solid
open Fable.Core


[<Erase; Import("Root", Spec.tabs)>]
type Tabs() =
    inherit div()
    interface Polymorph
    [<DefaultValue>] val mutable value : string 
    [<DefaultValue>] val mutable defaultValue : string 
    [<DefaultValue>] val mutable onChange : string -> unit 
    [<DefaultValue>] val mutable orientation : Orientation 
    [<DefaultValue>] val mutable activationMode : ActivationMode 
    [<DefaultValue>] val mutable disabled : bool 

[<RequireQualifiedAccess; Erase>]
module Tabs =
    [<Erase; Import("Trigger", Spec.tabs)>]
    type Trigger() =
        inherit Button()
        interface Polymorph
        [<DefaultValue>] val mutable value : string 
        [<DefaultValue>] val mutable disabled : bool 
    [<Erase; Import("Content", Spec.tabs)>]
    type Content() =
        inherit div()
        interface Polymorph
        [<DefaultValue>] val mutable value : string 
        [<DefaultValue>] val mutable forceMount : bool 
    [<Erase; Import("List", Spec.tabs)>]
    type List() =
        inherit div()
        interface Polymorph
    [<Erase; Import("Indicator", Spec.tabs)>]
    type Indicator() =
        inherit div()
        interface Polymorph

