namespace Partas.Solid.Kobalte

open Partas.Solid
open Fable.Core

[<Erase; Import("Root", Spec.contextMenu)>]
type ContextMenu() =
    interface RegularNode
    interface Polymorph
    [<DefaultValue>] val mutable onOpenChange : bool -> unit  // v0.13.9
    [<DefaultValue>] val mutable id : string  // v0.13.9
    [<DefaultValue>] val mutable modal : bool  // v0.13.9
    [<DefaultValue>] val mutable preventScroll : bool  // v0.13.9
    [<DefaultValue>] val mutable forceMount : bool  // v0.13.9

    [<DefaultValue>] val mutable placement : KobaltePlacement  // v0.13.9
    [<DefaultValue>] val mutable gutter : int  // v0.13.9
    [<DefaultValue>] val mutable shift : int  // v0.13.9
    [<DefaultValue>] val mutable flip : bool  // v0.13.9
    [<DefaultValue>] val mutable slide : bool  // v0.13.9
    [<DefaultValue>] val mutable overlap : bool  // v0.13.9
    [<DefaultValue>] val mutable sameWidth : bool  // v0.13.9
    [<DefaultValue>] val mutable fitViewport : bool  // v0.13.9
    [<DefaultValue>] val mutable hideWhenDetached : bool  // v0.13.9
    [<DefaultValue>] val mutable detachedPadding : int  // v0.13.9
    [<DefaultValue>] val mutable arrowPadding : int  // v0.13.9
    [<DefaultValue>] val mutable overflowPadding : int  // v0.13.9

[<RequireQualifiedAccess; Erase>]
module ContextMenu =
    /// <param name="data-expanded">Present when the trigger is expanded</param>
    /// <param name="data-closed">Present when the trigger is closed</param>
    /// <param name="data-disabled">Present when the trigger is disabled</param>
    [<Erase ; Import("Trigger", Spec.contextMenu)>]
    type Trigger() = // v0.13.9
        inherit Button()
        interface Polymorph
        [<DefaultValue>] val mutable disabled : bool  // v0.13.9
    /// <param name="data-expanded">Present when the trigger is expanded</param>
    [<Erase ; Import("Content", Spec.contextMenu)>]
    type Content() =
        inherit div()
        interface Polymorph
        [<DefaultValue>] val mutable onOpenAutoFocus : Browser.Types.Event -> unit  // v0.13.9
        [<DefaultValue>] val mutable onCloseAutoFocus : Browser.Types.Event -> unit  // v0.13.9
        [<DefaultValue>] val mutable onEscapeKeyDown : Browser.Types.KeyboardEvent -> unit  // v0.13.9
        [<DefaultValue>] val mutable onPointerDownOutside : Browser.Types.PointerEvent -> unit  // v0.13.9
        [<DefaultValue>] val mutable onFocusOutside : Browser.Types.FocusEvent -> unit  // v0.13.9
        [<DefaultValue>] val mutable onInteractOutside : Browser.Types.Event -> unit  // v0.13.9
    [<Erase ; Import("Arrow", Spec.contextMenu)>]
    type Arrow() =
        inherit div()
        interface Polymorph
        [<DefaultValue>] val mutable size : int  // v0.13.9
    /// <param name="data-disabled">Present when the item is disabled</param>
    /// <param name="data-highlighted">Present when the item is highlighted</param>
    [<Erase ; Import("Item", Spec.contextMenu)>]    
    type Item() =
        inherit div()
        interface Polymorph
        [<DefaultValue>] val mutable textValue : string  // v0.13.9
        [<DefaultValue>] val mutable disabled : bool  // v0.13.9
        [<DefaultValue>] val mutable closeOnSelect : bool  // v0.13.9
        [<DefaultValue>] val mutable onSelect : unit -> unit  // v0.13.9
    /// <param name="data-disabled">Present when the item is disabled</param>
    /// <param name="data-highlighted">Present when the item is highlighted</param>
    [<Erase; Import("ItemIndicator", Spec.contextMenu)>]
    type ItemIndicator() =
        inherit div()
        interface Polymorph
        [<DefaultValue>] val mutable forceMount : bool  // v0.13.9
    [<Erase; Import("RadioGroup", Spec.contextMenu)>]
    type RadioGroup() =
        inherit div()
        interface Polymorph
        [<DefaultValue>] val mutable value : string  // v0.13.9
        [<DefaultValue>] val mutable defaultValue : string  // v0.13.9
        [<DefaultValue>] val mutable onChange : string -> unit  // v0.13.9
        [<DefaultValue>] val mutable disabled : bool  // v0.13.9
    /// <param name="data-disabled">Present when the item is disabled</param>
    /// <param name="data-checked">Present when the item is checked</param>
    /// <param name="data-highlighted">Present when the item is highlighted</param>
    [<Erase; Import("RadioItem", Spec.contextMenu)>]
    type RadioItem() =
        inherit div()
        interface Polymorph
        [<DefaultValue>] val mutable value : string  // v0.13.9
        [<DefaultValue>] val mutable textValue : string  // v0.13.9
        [<DefaultValue>] val mutable disabled : bool  // v0.13.9
        [<DefaultValue>] val mutable closeOnSelect : bool  // v0.13.9
        [<DefaultValue>] val mutable onSelect : unit -> unit  // v0.13.9
    /// <param name="data-disabled">Present when the item is disabled</param>
    /// <param name="data-indeterminate">Present when the item is indeterminate</param>
    /// <param name="data-checked">Present when the item is checked</param>
    /// <param name="data-highlighted">Present when the item is highlighted</param>
    [<Erase; Import("CheckboxItem", Spec.contextMenu)>]
    type CheckboxItem() =
        inherit div()
        interface Polymorph
        [<DefaultValue>] val mutable checked' : bool  // v0.13.9
        [<DefaultValue>] val mutable defaultChecked : bool  // v0.13.9
        [<DefaultValue>] val mutable onChange : bool -> unit  // v0.13.9
        [<DefaultValue>] val mutable textValue : string  // v0.13.9
        [<DefaultValue>] val mutable indeterminate : bool  // v0.13.9
        [<DefaultValue>] val mutable disabled : bool  // v0.13.9
        [<DefaultValue>] val mutable closeOnSelect : bool  // v0.13.9
        [<DefaultValue>] val mutable onSelect : unit -> unit  // v0.13.9
    [<Erase; Import("Sub", Spec.contextMenu)>]
    type Sub() =
        inherit div()
        interface Polymorph
        [<DefaultValue>] val mutable open' : bool  // v0.13.9
        [<DefaultValue>] val mutable defaultOpen : bool  // v0.13.9
        [<DefaultValue>] val mutable onOpenChange : (bool -> unit)  // v0.13.9

        [<DefaultValue>] val mutable getAnchorRect : HtmlElement -> obj  // v0.13.9
        [<DefaultValue>] val mutable gutter : int  // v0.13.9
        [<DefaultValue>] val mutable shift : int  // v0.13.9
        [<DefaultValue>] val mutable slide : bool  // v0.13.9
        [<DefaultValue>] val mutable overlap : bool  // v0.13.9
        [<DefaultValue>] val mutable fitViewport : bool  // v0.13.9
        [<DefaultValue>] val mutable hideWhenDetached : bool  // v0.13.9
        [<DefaultValue>] val mutable detachedPadding : int  // v0.13.9
        [<DefaultValue>] val mutable arrowPadding : int  // v0.13.9
        [<DefaultValue>] val mutable overflowPadding : int  // v0.13.9
    /// <param name="data-expanded">Present when the trigger is expanded</param>
    [<Erase; Import("SubTrigger", Spec.contextMenu)>]
    type SubTrigger() =
        inherit Button()
        interface Polymorph
        [<DefaultValue>] val mutable textValue : string  // v0.13.9
        [<DefaultValue>] val mutable disabled : bool  // v0.13.9
    /// <param name="data-expanded">Present when the trigger is expanded</param>
    [<Erase; Import("SubContent", Spec.contextMenu)>]
    type SubContent() =
        inherit div()
        interface Polymorph
        [<DefaultValue>] val mutable onEscapeKeyDown : Browser.Types.KeyboardEvent -> unit  // v0.13.9
        [<DefaultValue>] val mutable onPointerDownOutside : Browser.Types.PointerEvent -> unit  // v0.13.9
        [<DefaultValue>] val mutable onFocusOutside : Browser.Types.FocusEvent -> unit  // v0.13.9
        [<DefaultValue>] val mutable onInteractOutside : Browser.Types.Event -> unit  // v0.13.9
    /// <param name="data-expanded">Present when the trigger is expanded</param>
    [<Erase; Import("Icon", Spec.contextMenu)>]
    type Icon() =
        inherit div()
        interface Polymorph
    [<Erase; Import("Portal", Spec.contextMenu)>]
    type Portal() =
        inherit div()
        interface Polymorph
    [<Erase; Import("Separator", Spec.contextMenu)>]
    type Separator() =
        inherit hr()
        interface Polymorph
    [<Erase; Import("Group", Spec.contextMenu)>]
    type Group() =
        inherit div()
        interface Polymorph
    [<Erase; Import("GroupLabel", Spec.contextMenu)>]
    type GroupLabel() =
        inherit span()
        interface Polymorph
    /// <param name="data-disabled">Present when the item is disabled</param>
    /// <param name="data-highlighted">Present when the item is highlighted</param>
    [<Erase; Import("ItemLabel", Spec.contextMenu)>]
    type ItemLabel() =
        inherit div()
        interface Polymorph
    /// <param name="data-disabled">Present when the item is disabled</param>
    /// <param name="data-highlighted">Present when the item is highlighted</param>
    [<Erase; Import("ItemDescription", Spec.contextMenu)>]
    type ItemDescription() =
        inherit div()
        interface Polymorph

