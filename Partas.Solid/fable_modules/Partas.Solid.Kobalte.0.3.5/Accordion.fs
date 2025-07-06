namespace Partas.Solid.Kobalte

open Partas.Solid
open Fable.Core

[<Erase; Import("Root", Spec.accordion)>]
type Accordion() =
    inherit div()
    interface Polymorph
    [<DefaultValue>] val mutable value : string[] //v0.13.9
    [<DefaultValue>] val mutable defaultValue : string[] //v0.13.9
    [<DefaultValue>] val mutable onChange : string[] -> unit //v0.13.9
    [<DefaultValue>] val mutable multiple : bool //v0.13.9
    [<DefaultValue>] val mutable collapsible : bool //v0.13.9
    [<DefaultValue>] val mutable shouldFocusWrap : bool //v0.13.9

[<Erase>]
module Accordion =
    /// <summary>
    /// data-expanded: present when accordion item is expanded<br/>
    /// data-closed: present when accordion item is collapsed<br/>
    /// data-disabled: present when accordion item is disabled
    /// </summary>
    [<Erase; Import("Item", Spec.accordion)>]
    type Item() =
        inherit Collapsible()  //v0.13.9
        interface Polymorph //v0.13.9
        [<DefaultValue>] val mutable open' : bool //v0.13.9
        [<DefaultValue>] val mutable defaultOpen : bool //v0.13.9
        [<DefaultValue>] val mutable onOpenChange : bool -> unit //v0.13.9
        [<DefaultValue>] val mutable disabled : bool //v0.13.9
        [<DefaultValue>] val mutable forceMount : bool //v0.13.9
        [<DefaultValue>] val mutable value : string //v0.13.9
    [<Erase; Import("Trigger", Spec.accordion)>]
    type Trigger() =
        inherit Collapsible.Trigger() //v0.13.9
        // inherit button()
        interface Polymorph //v0.13.9
    [<Erase; Import("Content", Spec.accordion)>]
    type Content() =
        inherit Collapsible.Content() //v0.13.9
        // inherit div()
        interface Polymorph  //v0.13.9
    [<Erase; Import("Header", Spec.accordion)>]
    type Header() =
        inherit h3() //v0.13.9
        interface Polymorph //v0.13.9
