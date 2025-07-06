namespace Partas.Solid.Kobalte

open Partas.Solid
open Fable.Core


[<Erase; Import("Root", Spec.dropdownMenu)>]
type DropdownMenu() =
    interface RegularNode
    interface Polymorph
    [<DefaultValue>] val mutable onOpenChange : (bool -> unit) 
    [<DefaultValue>] val mutable id : string 
    [<DefaultValue>] val mutable modal : bool 
    [<DefaultValue>] val mutable preventScroll : bool 
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
module DropdownMenu =
    [<Erase; Import("Trigger", Spec.dropdownMenu)>]
    type Trigger() =
        inherit Button()
        interface Polymorph
    [<Erase; Import("Content", Spec.dropdownMenu)>]
    type Content() =
        inherit div()
        interface Polymorph
        [<DefaultValue>] val mutable onOpenAutoFocus : Browser.Types.Event -> unit 
        [<DefaultValue>] val mutable onCloseAutoFocus : Browser.Types.Event -> unit 
        [<DefaultValue>] val mutable onEscapeKeyDown : Browser.Types.KeyboardEvent -> unit 
        [<DefaultValue>] val mutable onPointerDownOutside : Browser.Types.PointerEvent -> unit 
        [<DefaultValue>] val mutable onFocusOutside : Browser.Types.FocusEvent -> unit 
        [<DefaultValue>] val mutable onInteractOutside : Browser.Types.Event -> unit 
    [<Erase; Import("Arrow", Spec.dropdownMenu)>]
    type Arrow() =
        inherit div()
        interface Polymorph
        [<DefaultValue>] val mutable size : int 
    [<Erase; Import("Item", Spec.dropdownMenu)>]
    type Item() =
        inherit div()
        interface Polymorph
        [<DefaultValue>] val mutable textValue : string 
        [<DefaultValue>] val mutable disabled : bool 
        [<DefaultValue>] val mutable closeOnSelect : bool 
        [<DefaultValue>] val mutable onSelect : unit -> unit 
    [<Erase; Import("ItemIndicator", Spec.dropdownMenu)>]
    type ItemIndicator() =
        inherit div()
        interface Polymorph
        [<DefaultValue>] val mutable forceMount : bool 
    [<Erase; Import("RadioGroup", Spec.dropdownMenu)>]
    type RadioGroup() =
        inherit div()
        interface Polymorph
        [<DefaultValue>] val mutable value : string 
        [<DefaultValue>] val mutable defaultValue : string 
        [<DefaultValue>] val mutable onChange : string -> unit 
        [<DefaultValue>] val mutable disabled : bool 
    [<Erase; Import("RadioItem", Spec.dropdownMenu)>]
    type RadioItem() =
        inherit div()
        interface Polymorph
        [<DefaultValue>] val mutable value : string 
        [<DefaultValue>] val mutable textValue : string 
        [<DefaultValue>] val mutable disabled : bool 
        [<DefaultValue>] val mutable closeOnSelect : bool 
        [<DefaultValue>] val mutable onSelect : unit -> unit 
    [<Erase; Import("CheckboxItem", Spec.dropdownMenu)>]
    type CheckboxItem() =
        inherit div()
        interface Polymorph
        [<DefaultValue>] val mutable checked' : bool 
        [<DefaultValue>] val mutable defaultChecked : bool 
        [<DefaultValue>] val mutable onChange : bool -> unit 
        [<DefaultValue>] val mutable textValue : string 
        [<DefaultValue>] val mutable indeterminate : bool 
        [<DefaultValue>] val mutable disabled : bool 
        [<DefaultValue>] val mutable closeOnSelect : bool 
        [<DefaultValue>] val mutable onSelect : unit -> unit 
    [<Erase; Import("Sub", Spec.dropdownMenu)>]
    type Sub() =
        inherit div()
        interface Polymorph
        [<DefaultValue>] val mutable open' : bool 
        [<DefaultValue>] val mutable defaultOpen : bool 
        [<DefaultValue>] val mutable onOpenChange : (bool -> unit) 

        [<DefaultValue>] val mutable getAnchorRect : HtmlElement -> obj 
        [<DefaultValue>] val mutable gutter : int 
        [<DefaultValue>] val mutable shift : int 
        [<DefaultValue>] val mutable slide : bool 
        [<DefaultValue>] val mutable overlap : bool 
        [<DefaultValue>] val mutable fitViewport : bool 
        [<DefaultValue>] val mutable hideWhenDetached : bool 
        [<DefaultValue>] val mutable detachedPadding : int 
        [<DefaultValue>] val mutable arrowPadding : int 
        [<DefaultValue>] val mutable overflowPadding : int 

    [<Erase; Import("SubTrigger", Spec.dropdownMenu)>]
    type SubTrigger() =
        inherit Button()
        interface Polymorph
        [<DefaultValue>] val mutable textValue : string 
        [<DefaultValue>] val mutable disabled : bool 
    [<Erase; Import("SubContent", Spec.dropdownMenu)>]
    type SubContent() =
        inherit div()
        interface Polymorph
        [<DefaultValue>] val mutable onEscapeKeyDown : Browser.Types.KeyboardEvent -> unit 
        [<DefaultValue>] val mutable onPointerDownOutside : Browser.Types.PointerEvent -> unit 
        [<DefaultValue>] val mutable onFocusOutside : Browser.Types.FocusEvent -> unit 
        [<DefaultValue>] val mutable onInteractOutside : Browser.Types.Event -> unit 
    [<Erase; Import("Icon", Spec.dropdownMenu)>]
    type Icon() =
        inherit div()
        interface Polymorph
    [<Erase; Import("Portal", Spec.dropdownMenu)>]
    type Portal() =
        inherit div()
        interface Polymorph
    [<Erase; Import("Separator", Spec.dropdownMenu)>]
    type Separator() =
        inherit hr()
        interface Polymorph
    [<Erase; Import("Group", Spec.dropdownMenu)>]
    type Group() =
        inherit div()
        interface Polymorph
    [<Erase; Import("GroupLabel", Spec.dropdownMenu)>]
    type GroupLabel() =
        inherit span()
        interface Polymorph
    [<Erase; Import("ItemLabel", Spec.dropdownMenu)>]
    type ItemLabel() =
        inherit div()
        interface Polymorph
    [<Erase; Import("ItemDescription", Spec.dropdownMenu)>]
    type ItemDescription() =
        inherit div()
        interface Polymorph
