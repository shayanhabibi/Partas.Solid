namespace Partas.Solid.Kobalte

open Fable.Core
open Partas.Solid


[<Erase; Import("Root", Spec.popover)>]
type Popover() =
    inherit div()
    interface Polymorph
    [<DefaultValue>] val mutable open' : bool 
    [<DefaultValue>] val mutable defaultOpen : bool 
    [<DefaultValue>] val mutable onOpenChange : (bool -> unit) 
    [<DefaultValue>] val mutable id : string 
    [<DefaultValue>] val mutable modal : bool 
    [<DefaultValue>] val mutable preventScroll : bool 
    [<DefaultValue>] val mutable forceMount : bool 
    [<DefaultValue>] val mutable translations : string 

    [<DefaultValue>] val mutable getAnchorRect : HtmlElement -> obj 
    [<DefaultValue>] val mutable anchorRef : unit -> HtmlElement 
    [<DefaultValue>] val mutable placement : Popover.Placement 
    [<DefaultValue>] val mutable gutter : int 
    [<DefaultValue>] val mutable shift : int 
    [<DefaultValue>] val mutable flip : bool 
    [<DefaultValue>] val mutable slide : bool 
    [<DefaultValue>] val mutable overlap : bool 
    [<DefaultValue>] val mutable sameWidth : bool 
    [<DefaultValue>] val mutable fitViewport : bool 
    [<DefaultValue>] val mutable hideWhenDetached : bool 
    [<DefaultValue>] val mutable detachedPadding : int 
    [<DefaultValue>] val mutable arrowPadding : int 
    [<DefaultValue>] val mutable overflowPadding : int 

[<RequireQualifiedAccess; Erase>]
module Popover =
    [<Erase; Import("Trigger", Spec.popover)>]
    type Trigger() =
        inherit Button()
        interface Polymorph
    [<Erase; Import("Content", Spec.popover)>]
    type Content() =
        inherit div()
        interface Polymorph
        [<DefaultValue>] val mutable gutter : int 
        [<DefaultValue>] val mutable onOpenAutoFocus : Browser.Types.Event -> unit 
        [<DefaultValue>] val mutable onCloseAutoFocus : Browser.Types.Event -> unit 
        [<DefaultValue>] val mutable onEscapeKeyDown : Browser.Types.KeyboardEvent -> unit 
        [<DefaultValue>] val mutable onPointerDownOutside : Browser.Types.PointerEvent -> unit 
        [<DefaultValue>] val mutable onFocusOutside : Browser.Types.FocusEvent -> unit 
        [<DefaultValue>] val mutable onInteractOutside : Browser.Types.Event -> unit 
    [<Erase; Import("Arrow", Spec.popover)>]
    type Arrow() =
        inherit div()
        interface Polymorph
        [<DefaultValue>] val mutable size : int 
    [<Erase; Import("Portal", Spec.popover)>]
    type Portal() =
        inherit div()
        interface Polymorph
    [<Erase; Import("Anchor", Spec.popover)>]
    type Anchor() =
        inherit div()
        interface Polymorph
    [<Erase; Import("CloseButton", Spec.popover)>]
    type CloseButton() =
        inherit Button()
        interface Polymorph
    [<Erase; Import("Title", Spec.popover)>]
    type Title() =
        inherit h2()
        interface Polymorph
    [<Erase; Import("Description", Spec.popover)>]
    type Description() =
        inherit p()
        interface Polymorph

