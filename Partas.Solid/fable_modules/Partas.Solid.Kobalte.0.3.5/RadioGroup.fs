namespace Partas.Solid.Kobalte

open Partas.Solid
open Fable.Core


[<Erase; Import("Root", Spec.radioGroup)>]
type RadioGroup() =
    inherit div()
    interface Polymorph
    [<DefaultValue>] val mutable ref: HtmlElement 
    [<DefaultValue>] val mutable value : string 
    [<DefaultValue>] val mutable defaultValue : string 
    [<DefaultValue>] val mutable onChange : string -> unit 
    [<DefaultValue>] val mutable orientation : Orientation 
    [<DefaultValue>] val mutable disabled : bool 
    [<DefaultValue>] val mutable name : string 
    [<DefaultValue>] val mutable validationState : ValidationState 
    [<DefaultValue>] val mutable required : bool 
    [<DefaultValue>] val mutable readOnly : bool 

[<RequireQualifiedAccess; Erase>]
module RadioGroup =
    [<Erase; Import("Label", Spec.radioGroup)>]
    type Label() =
        inherit span()
        interface Polymorph
    [<Erase; Import("Description", Spec.radioGroup)>]
    type Description() =
        inherit div()
        interface Polymorph
    [<Erase; Import("ErrorMessage", Spec.radioGroup)>]
    type ErrorMessage() =
        inherit div()
        interface Polymorph
        [<DefaultValue>] val mutable forceMount : bool 
    [<Erase; Import("Item", Spec.radioGroup)>]
    type Item() =
        inherit div()
        interface Polymorph
        [<DefaultValue>] val mutable ref : HtmlElement 
        [<DefaultValue>] val mutable value : string 
        [<DefaultValue>] val mutable disabled : bool 
    [<Erase; Import("ItemInput", Spec.radioGroup)>]
    type ItemInput() =
        inherit input()
        interface Polymorph
    [<Erase; Import("ItemControl", Spec.radioGroup)>]
    type ItemControl() =
        inherit div()
        interface Polymorph
    [<Erase; Import("ItemIndicator", Spec.radioGroup)>]
    type ItemIndicator() =
        inherit div()
        interface Polymorph
    [<Erase; Import("ItemLabel", Spec.radioGroup)>]
    type ItemLabel() =
        inherit label()
        interface Polymorph
    [<Erase; Import("ItemDescription", Spec.radioGroup)>]
    type ItemDescription() =
        inherit div()
        interface Polymorph

