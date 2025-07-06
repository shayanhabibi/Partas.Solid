namespace Partas.Solid.Kobalte

open Partas.Solid
open Fable.Core
open Browser.Types


[<Erase; Import("Root", Spec.tooltip)>]
type Tooltip() =
    inherit div()
    interface Polymorph
    [<DefaultValue>] val mutable open' : bool 
    [<DefaultValue>] val mutable defaultOpen : bool 
    [<DefaultValue>] val mutable onOpenChange : (bool -> unit) 
    [<DefaultValue>] val mutable triggerOnFocusOnly : bool 
    [<DefaultValue>] val mutable openDelay : int 
    [<DefaultValue>] val mutable skipDelayDuration : bool 
    [<DefaultValue>] val mutable closeDelay : int 
    [<DefaultValue>] val mutable ignoreSafeArea : bool 
    [<DefaultValue>] val mutable id : string 
    [<DefaultValue>] val mutable forceMount : bool 

    [<DefaultValue>] val mutable getAnchorRect : HtmlElement -> obj 
    [<DefaultValue>] val mutable placement : KobaltePlacement 
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
module Tooltip =
    [<AllowNullLiteral>]
    [<Interface>]
    type DataSet =
        abstract member ``data-expanded``: string option with get, set
        abstract member ``data-closed``: string option with get, set
        
    [<Erase; Import("Trigger", Spec.tooltip)>]
    type Trigger() =
        inherit Button()
        interface Polymorph
    [<Erase; Import("Content", Spec.tooltip)>]
    type Content() =
        inherit div()
        interface Polymorph
        [<DefaultValue>] val mutable onEscapeKeyDown : Browser.Types.KeyboardEvent -> unit 
        [<DefaultValue>] val mutable onPointerDownOutside : Browser.Types.PointerEvent -> unit 
    [<Erase; Import("Arrow", Spec.tooltip)>]
    type Arrow() =
        inherit div()
        interface Polymorph
        [<DefaultValue>] val mutable size : int 
    [<Erase; Import("Portal", Spec.tooltip)>]
    type Portal() =
        inherit div()
        interface Polymorph

[<AutoOpen; Erase>]
module TooltipContext =

    [<AllowNullLiteral>]
    [<Interface>]
    type TooltipContext =
        abstract member dataset: Accessor<Tooltip.DataSet> with get
        abstract member isOpen: Accessor<bool> with get
        abstract member isDisabled: Accessor<bool> with get
        abstract member triggerOnFocusOnly: Accessor<bool> with get
        abstract member contentId: Accessor<string option> with get
        abstract member contentPresent: Accessor<bool> with get
        /// Can pass a boolean as to whether to immediately
        /// open/hide the tooltip
        abstract member openTooltip: (bool -> unit) with get
        /// Can pass a boolean as to whether to immediately
        /// open/hide the tooltip
        abstract member hideTooltip: (bool -> unit) with get
        abstract member cancelOpening: (unit -> unit) with get
        abstract member generateId: (string -> string) with get
        abstract member registerContentId: (string -> (unit -> unit)) with get
        abstract member isTargetOnTooltip: (Node option -> bool) with get
        abstract member setTriggerRef: (HTMLElement -> unit) with get
        abstract member setContentRef: (HTMLElement -> unit) with get

    [<ImportMember(Spec.tooltip)>]
    let useTooltipContext(): TooltipContext = jsNative
