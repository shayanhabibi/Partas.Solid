namespace Partas.Solid.Kobalte

open Partas.Solid
open Fable.Core

[<Erase; Import("Root", Spec.alertDialog)>]
type AlertDialog() =
    interface RegularNode
    interface Polymorph
    [<DefaultValue>] val mutable open' : bool //v0.13.9
    [<DefaultValue>] val mutable defaultOpen : bool //v0.13.9
    [<DefaultValue>] val mutable onOpenChange : bool -> unit //v0.13.9
    [<DefaultValue>] val mutable id : string //v0.13.9
    [<DefaultValue>] val mutable modal : bool //v0.13.9
    [<DefaultValue>] val mutable preventScroll : bool //v0.13.9
    [<DefaultValue>] val mutable forceMount : bool //v0.13.9

[<Erase; RequireQualifiedAccess>]
module AlertDialog =
    /// <summary>
    /// data-expanded: Present when the dialog is open<br/>
    /// data-closed: Present when the dialog is closed
    /// </summary>
    [<Erase; Import("Trigger", Spec.alertDialog)>]
    type Trigger() =
        inherit Button() //v0.13.9
        interface Polymorph
    /// <summary>
    /// data-expanded: Present when the dialog is open<br/>
    /// data-closed: Present when the dialog is closed
    /// </summary>
    [<Erase; Import("Content", Spec.alertDialog)>]
    type Content() =
        inherit div()
        interface Polymorph
        [<DefaultValue>] val mutable onOpenAutoFocus : Browser.Types.Event -> unit //v0.13.9
        [<DefaultValue>] val mutable onCloseAutoFocus : Browser.Types.Event -> unit //v0.13.9
        [<DefaultValue>] val mutable onEscapeKeyDown : Browser.Types.KeyboardEvent -> unit //v0.13.9
        [<DefaultValue>] val mutable onPointerDownOutside : Browser.Types.PointerEvent -> unit //v0.13.9
        [<DefaultValue>] val mutable onFocusOutside : Browser.Types.FocusEvent -> unit //v0.13.9
        [<DefaultValue>] val mutable onInteractOutside : Browser.Types.Event -> unit //v0.13.9
    [<Erase; Import("Portal", Spec.alertDialog)>]
    type Portal() = //v0.13.9
        inherit div()
        interface Polymorph
    /// <summary>
    /// data-expanded: Present when the dialog is open<br/>
    /// data-closed: Present when the dialog is closed
    /// </summary>
    [<Erase; Import("Overlay", Spec.alertDialog)>]
    type Overlay() = //v0.13.9
        inherit div()
        interface Polymorph
    [<Erase; Import("CloseButton", Spec.alertDialog)>]
    type CloseButton() = //v0.13.9
        inherit Button()
        interface Polymorph
    [<Erase; Import("Title", Spec.alertDialog)>]
    type Title() = //v0.13.9
        inherit h2()
        interface Polymorph
    [<Erase; Import("Description", Spec.alertDialog)>]
    type Description() = //v0.13.9
        inherit p()
        interface Polymorph
