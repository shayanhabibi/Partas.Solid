namespace Partas.Solid.Kobalte

open Partas.Solid
open Fable.Core

[<Erase; AllowNullLiteral; Interface>]
type ToggleButtonState =
    abstract member pressed: Accessor<bool> with get

[<Erase; Import("Root", Spec.toggleButton)>]
type ToggleButton() =
    interface HtmlTag
    interface Polymorph
    interface ChildLambdaProvider<ToggleButtonState>
    [<DefaultValue>] val mutable pressed : bool 
    [<DefaultValue>] val mutable defaultPressed : bool 
    [<DefaultValue>] val mutable onChange : bool -> unit 

