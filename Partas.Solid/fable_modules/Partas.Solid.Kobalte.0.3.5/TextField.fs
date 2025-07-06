namespace Partas.Solid.Kobalte

open Partas.Solid
open Fable.Core



[<Erase; Import("Root", Spec.textField)>]
type TextField() =
    inherit div()
    interface Polymorph
    [<DefaultValue>] val mutable value : string 
    [<DefaultValue>] val mutable defaultValue : string 
    [<DefaultValue>] val mutable onChange : string -> unit 
    [<DefaultValue>] val mutable onBlur: Browser.Types.FocusEvent -> unit
    [<DefaultValue>] val mutable name : string 
    [<DefaultValue>] val mutable validationState : ValidationState 
    [<DefaultValue>] val mutable required : bool 
    [<DefaultValue>] val mutable disabled : bool 
    [<DefaultValue>] val mutable readOnly : bool 

[<Erase; RequireQualifiedAccess>]
module TextField =
    [<Erase; Import("TextArea", Spec.textField)>]
    type TextArea() =
        inherit textarea()
        interface Polymorph
        [<DefaultValue>] val mutable autoResize : bool 
        [<DefaultValue>] val mutable submitOnEnter : bool 
    [<Erase; Import("ErrorMessage", Spec.textField)>]
    type ErrorMessage() =
        inherit div()
        interface Polymorph
        [<DefaultValue>] val mutable forceMount : bool 
    [<Erase; Import("Label", Spec.textField)>]
    type Label() =
        inherit label()
        interface Polymorph
    [<Erase; Import("Input", Spec.textField)>]
    type Input() =
        inherit input()
        interface Polymorph
    [<Erase; Import("Description", Spec.textField)>]
    type Description() =
        inherit div()
        interface Polymorph

