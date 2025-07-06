namespace Partas.Solid.Kobalte

open Fable.Core
open Partas.Solid




[<Erase; Import("Root", Spec.slider)>]
type Slider() =
    inherit div()
    interface Polymorph
    [<DefaultValue>] val mutable value : int[] 
    [<DefaultValue>] val mutable defaultValue : int[] 
    [<DefaultValue>] val mutable onChange : int[] -> unit 
    [<DefaultValue>] val mutable onChangeEnd : int[] -> unit 
    [<DefaultValue>] val mutable inverted : bool 
    [<DefaultValue>] val mutable minValue : int 
    [<DefaultValue>] val mutable maxValue : int 
    [<DefaultValue>] val mutable step : int 
    [<DefaultValue>] val mutable minStepsBetweenThumbs : int 
    [<DefaultValue>] val mutable getValueLabel : {| values: int[] ; min : int ; max : int |} -> string 
    [<DefaultValue>] val mutable orientation : Orientation 
    [<DefaultValue>] val mutable name : string 
    [<DefaultValue>] val mutable validationState : ValidationState 
    [<DefaultValue>] val mutable required : bool 
    [<DefaultValue>] val mutable disabled : bool 
    [<DefaultValue>] val mutable readOnly : bool 

[<Erase; RequireQualifiedAccess>]
module Slider =
    [<Erase; Import("Track", Spec.slider)>]
    type Track() =
        inherit div()
        interface Polymorph
    [<Erase; Import("Fill", Spec.slider)>]
    type Fill() =
        inherit div()
        interface Polymorph
    [<Erase; Import("Thumb", Spec.slider)>]
    type Thumb() =
        inherit span()
        interface Polymorph
    [<Erase; Import("Input", Spec.slider)>]
    type Input() =
        inherit input()
        interface Polymorph
    [<Erase; Import("ValueLabel", Spec.slider)>]
    type ValueLabel() =
        inherit div()
        interface Polymorph
    [<Erase; Import("Label", Spec.slider)>]
    type Label() =
        inherit div()
        interface Polymorph
    [<Erase; Import("Description", Spec.slider)>]
    type Description() =
        inherit div()
        interface Polymorph
    [<Erase; Import("ErrorMessage", Spec.slider)>]
    type ErrorMessage() =
        inherit div()
        interface Polymorph

