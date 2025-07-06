namespace Partas.Solid.Kobalte

open Fable.Core
open Partas.Solid
open Browser.Types
open Partas.Solid.Experimental.U

[<Erase; Import("Root", Spec.hoverCard)>]
type HoverCard() =
    interface RegularNode
    interface Polymorph
    /// <summary>
    /// The controlled open state of the hovercard.
    /// </summary>
    [<DefaultValue>]
    val mutable open': bool
    /// <summary>
    /// The default open state when initially rendered.
    /// Useful when you do not need to control the open state.
    /// </summary>
    [<DefaultValue>]
    val mutable defaultOpen: bool
    /// <summary>
    /// Event handler called when the open state of the hovercard changes.
    /// </summary>
    [<DefaultValue>]
    val mutable onOpenChange: (bool -> unit)
    /// <summary>
    /// The duration from when the mouse enters the trigger until the hovercard opens.
    /// </summary>
    [<DefaultValue>]
    val mutable openDelay: float
    /// <summary>
    /// The duration from when the mouse leaves the trigger or content until the hovercard closes.
    /// </summary>
    [<DefaultValue>]
    val mutable closeDelay: float
    /// <summary>
    /// Whether to close the hovercard even if the user cursor is inside the safe area between the trigger and hovercard.
    /// </summary>
    [<DefaultValue>]
    val mutable ignoreSafeArea: bool
    /// <summary>
    /// Used to force mounting the hovercard (portal and content) when more control is needed.
    /// Useful when controlling animation with SolidJS animation libraries.
    /// </summary>
    [<DefaultValue>]
    val mutable forceMount: bool
    /// <summary>
    /// A unique identifier for the component.
    /// The id is used to generate id attributes for nested components.
    /// If no id prop is provided, a generated id will be used.
    /// </summary>
    [<DefaultValue>]
    val mutable id: string

    /// <summary>
    /// Function that returns the anchor element's DOMRect. If this is explicitly
    /// passed, it will override the anchor <c>getBoundingClientRect</c> method.
    /// </summary>
    [<DefaultValue>]
    val mutable getAnchorRect: (HTMLElement -> obj)
    /// <summary>
    /// The placement of the popper.
    /// </summary>
    [<DefaultValue>]
    val mutable placement: KobaltePlacement
    /// <summary>
    /// The distance between the popper and the anchor element.
    /// By default, it's 0 plus half of the arrow offset, if it exists.
    /// </summary>
    [<DefaultValue>]
    val mutable gutter: float
    /// <summary>
    /// The skidding of the popper along the anchor element.
    /// </summary>
    [<DefaultValue>]
    val mutable shift: float
    /// <summary>
    /// Controls the behavior of the popper when it overflows the viewport:
    ///   - If a <c>boolean</c>, specifies whether the popper should flip to the
    ///     opposite side when it overflows.
    ///   - If a <c>string</c>, indicates the preferred fallback placements when it
    ///     overflows. The placements must be spaced-delimited, e.g. "top left".
    /// </summary>
    [<DefaultValue>]
    val mutable flip: U2<bool, string>
    /// <summary>
    /// Whether the popper should slide when it overflows.
    /// </summary>
    [<DefaultValue>]
    val mutable slide: bool
    /// <summary>
    /// Whether the popper can overlap the anchor element when it overflows.
    /// </summary>
    [<DefaultValue>]
    val mutable overlap: bool
    /// <summary>
    /// Whether the popper should have the same width as the anchor element.
    /// This will be exposed to CSS as <c>--kb-popper-anchor-width</c>.
    /// </summary>
    [<DefaultValue>]
    val mutable sameWidth: bool
    /// <summary>
    /// Whether the popper should fit the viewport. If this is set to true, the
    /// popper positioner will have <c>maxWidth</c> and <c>maxHeight</c> set to the viewport size.
    /// This will be exposed to CSS as <c>--kb-popper-content-available-width</c> and <c>--kb-popper-content-available-height</c>.
    /// </summary>
    [<DefaultValue>]
    val mutable fitViewport: bool
    /// <summary>
    /// Whether to hide the popper when the anchor element becomes occluded.
    /// </summary>
    [<DefaultValue>]
    val mutable hideWhenDetached: bool
    /// <summary>
    /// The minimum padding in order to consider the anchor element occluded.
    /// </summary>
    [<DefaultValue>]
    val mutable detachedPadding: float
    /// <summary>
    /// The minimum padding between the arrow and the popper corner.
    /// </summary>
    [<DefaultValue>]
    val mutable arrowPadding: float
    /// <summary>
    /// The minimum padding between the popper and the viewport edge.
    /// This will be exposed to CSS as <c>--kb-popper-content-overflow-padding</c>.
    /// </summary>
    [<DefaultValue>]
    val mutable overflowPadding: float

[<RequireQualifiedAccess; Erase>]
module HoverCard =
    [<AllowNullLiteral>]
    [<Interface>]
    type DataSet =
        abstract member ``data-expanded``: string option with get
        abstract member ``data-closed``: string option with get
        
    [<Erase; Import("Trigger", Spec.hoverCard)>]
    type Trigger() =
        inherit Link()
        interface Polymorph
    [<Erase; Import("Portal", Spec.hoverCard)>]
    type Portal() =
        inherit div()
        interface Polymorph
    [<Erase; Import("Content", Spec.hoverCard)>]
    type Content() =
        inherit div()
        interface Polymorph
    [<Erase; Import("Arrow", Spec.hoverCard)>]
    type Arrow() =
        inherit div()
        interface Polymorph

[<AutoOpen; Erase>]
module HoverCardContext =
    [<AllowNullLiteral>]
    [<Interface>]
    type HoverCardContext =
        abstract member dataset: Accessor<HoverCard.DataSet> with get
        abstract member isOpen: Accessor<bool> with get
        abstract member contentPresent: Accessor<bool> with get
        abstract member openWithDelay: (unit -> unit) with get
        abstract member closeWithDelay: (unit -> unit) with get
        abstract member cancelOpening: (unit -> unit) with get
        abstract member cancelClosing: (unit -> unit) with get
        abstract member close: (unit -> unit) with get
        abstract member isTargetOnHoverCard: (Node option -> bool) with get
        abstract member setTriggerRef: (HTMLElement -> unit) with get
        abstract member setContentRef: (HTMLElement -> unit) with get

    [<ImportMember(Spec.hoverCard)>]
    let useHoverCardContext (): HoverCardContext = nativeOnly
