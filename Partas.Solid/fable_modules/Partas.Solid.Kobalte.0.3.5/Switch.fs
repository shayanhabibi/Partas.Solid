namespace Partas.Solid.Kobalte

open Fable.Core
open Partas.Solid


[<Erase; Import("Root", Spec.switch)>]
type Switch() =
    inherit div()
    interface Polymorph
    [<DefaultValue>] val mutable checked' : bool 
    [<DefaultValue>] val mutable defaultChecked : bool 
    [<DefaultValue>] val mutable onChange : bool -> unit 
    [<DefaultValue>] val mutable name : string 
    [<DefaultValue>] val mutable validationState : ValidationState 
    [<DefaultValue>] val mutable required : bool 
    [<DefaultValue>] val mutable disabled : bool 
    [<DefaultValue>] val mutable readOnly : bool 
    [<DefaultValue>] val mutable value : string 
    
    member inline this.Checked : unit -> bool = fun _ -> this.checked'

[<Erase; RequireQualifiedAccess>]
module Switch =
    [<Erase; Import("Input", Spec.switch)>]
    type Input() =
        inherit input()
        interface Polymorph
    [<Erase; Import("Control", Spec.switch)>]
    type Control() =
        inherit div()
        interface Polymorph
    [<Erase; Import("Indicator", Spec.switch)>]
    type Indicator() =
        inherit div()
        interface Polymorph
    [<Erase; Import("Label", Spec.switch)>]
    type Label() =
        inherit label()
        interface Polymorph
    [<Erase; Import("Description", Spec.switch)>]
    type Description() =
        inherit div()
        interface Polymorph
    [<Erase; Import("ErrorMessage", Spec.switch)>]
    type ErrorMessage() =
        inherit div()
        interface Polymorph
        [<DefaultValue>] val mutable forceMount : bool 
    [<Erase; Import("Thumb", Spec.switch)>]
    type Thumb() =
        inherit div()
        interface Polymorph


