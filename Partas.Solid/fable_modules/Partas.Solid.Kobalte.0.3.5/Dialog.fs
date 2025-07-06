namespace Partas.Solid.Kobalte

open Partas.Solid
open Fable.Core
open Browser.Types
    
[<Erase; Import("Root", Spec.dialog)>]
type Dialog() =
    interface RegularNode
    interface Polymorph
    [<DefaultValue>] val mutable open' : bool 
    [<DefaultValue>] val mutable defaultOpen : bool 
    [<DefaultValue>] val mutable onOpenChange : (bool -> unit) 
    [<DefaultValue>] val mutable id : string 
    [<DefaultValue>] val mutable modal : bool 
    [<DefaultValue>] val mutable preventScroll : bool 
    [<DefaultValue>] val mutable forceMount : bool 
    [<DefaultValue>] val mutable translations : string 

[<RequireQualifiedAccess; Erase>]
module Dialog =
    [<Erase; Import("Trigger", Spec.dialog)>]
    type Trigger() =
        inherit Button()
        interface Polymorph
    [<Erase; Import("Content", Spec.dialog)>]
    type Content() =
        inherit div()
        interface Polymorph
        [<DefaultValue>] val mutable onOpenAutoFocus : Browser.Types.Event -> unit 
        [<DefaultValue>] val mutable onCloseAutoFocus : Browser.Types.Event -> unit 
        [<DefaultValue>] val mutable onEscapeKeyDown : KeyboardEvent -> unit 
        [<DefaultValue>] val mutable onPointerDownOutside : PointerEvent -> unit 
        [<DefaultValue>] val mutable onFocusOutside : FocusEvent -> unit 
        [<DefaultValue>] val mutable onInteractOutside : Browser.Types.Event -> unit 
    [<Erase; Import("Overlay", Spec.dialog)>]
    type Overlay() =
        inherit div()
        interface Polymorph
    [<Erase; Import("CloseButton", Spec.dialog)>]
    type CloseButton() =
        inherit Button()
        interface Polymorph
    [<Erase; Import("Title", Spec.dialog)>]
    type Title() =
        inherit h2()
        interface Polymorph
    [<Erase; Import("Description", Spec.dialog)>]
    type Description() =
        inherit p()
        interface Polymorph
    [<Erase; Import("Portal", Spec.dialog)>]
    type Portal() =
        inherit div()
        interface Polymorph

[<Erase; AutoOpen>]
module DialogContext =
    [<AllowNullLiteral; Erase>]
    type DialogContext =
        abstract member translations: Accessor<obj> with get
        abstract member isOpen: bool Accessor with get
        abstract member modal: bool Accessor with get
        abstract member preventScroll: bool Accessor with get
        abstract member contentId: string option Accessor with get
        abstract member titleId: string option Accessor with get
        abstract member descriptionId: string option Accessor with get
        abstract member triggerRef: HTMLElement option Accessor with get
        abstract member overlayRef: HTMLElement option Accessor with get
        abstract member setOverlayRef: HTMLElement Setter with get
        abstract member contentRef: HTMLElement option Accessor with get
        abstract member setContentRef: HTMLElement Setter with get
        abstract member overlayPresent: bool Accessor with get
        abstract member contentPresent: bool Accessor with get
        abstract member close: (unit -> unit) with get
        abstract member toggle: (unit -> unit) with get
        abstract member setTriggerRef: HTMLElement Setter with get
        abstract member generateId: string -> string with get
        abstract member registerContentId: string -> (unit -> unit)
        abstract member registerTitleId: string -> (unit -> unit)
        abstract member registerDescriptionId: string -> (unit -> unit)
    
    [<Import("useDialogContext", Spec.dialog)>]
    let useDialogContext (): DialogContext = jsNative


