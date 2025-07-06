namespace Partas.Solid.Kobalte

open Partas.Solid
open Fable.Core

/// <summary></summary>
/// <param name="data-expanded">Present when the collapsible is expanded</param>
/// <param name="data-closed">Present when the collapsible is closed</param>
/// <param name="data-disabled">Present when the collapsible is disabled</param>
[<Erase; Import("Root", Spec.collapsible)>]
type Collapsible() =
    inherit div()
    interface Polymorph
    /// <summary>
    /// The controlled open state of the collapsible.
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
    /// Event handler called when the open state of the collapsible changes.
    /// </summary>
    [<DefaultValue>]
    val mutable onOpenChange: (bool -> unit)
    /// <summary>
    /// Whether the collapsible is disabled.
    /// </summary>
    [<DefaultValue>]
    val mutable disabled: bool
    /// <summary>
    /// Used to force mounting the collapsible content when more control is needed.
    /// Useful when controlling animation with SolidJS animation libraries.
    /// </summary>
    [<DefaultValue>]
    val mutable forceMount: bool

[<RequireQualifiedAccess; Erase>]
module Collapsible =
    /// <summary></summary>
    /// <param name="data-expanded">Present when the collapsible is expanded</param>
    /// <param name="data-closed">Present when the collapsible is closed</param>
    /// <param name="data-disabled">Present when the collapsible is disabled</param>
    [<Erase; Import("Trigger", Spec.collapsible)>]
    type Trigger() = // v0.13.9
        inherit button()
        interface Polymorph
    /// <summary></summary>
    /// <param name="data-expanded">Present when the collapsible is expanded</param>
    /// <param name="data-closed">Present when the collapsible is closed</param>
    /// <param name="data-disabled">Present when the collapsible is disabled</param>
    [<Erase; Import("Content", Spec.collapsible)>]
    type Content() = // v0.13.9
        interface RegularNode
        interface Polymorph

[<Erase; AutoOpen>]
module CollapsibleContext =
    [<AllowNullLiteral>]
    [<Interface>]
    type CollapsibleDataSet =
        abstract member ``data-expanded``: string option with get, set
        abstract member ``data-closed``: string option with get, set
        abstract member ``data-disabled``: string option with get, set
    [<AllowNullLiteral; Interface>]
    type CollapsibleContext =
        abstract member dataset: Accessor<CollapsibleDataSet> with get, set
        abstract member isOpen: Accessor<bool> with get, set
        abstract member disabled: Accessor<bool> with get, set
        abstract member shouldMount: Accessor<bool> with get, set
        abstract member contentId: Accessor<string option> with get, set
        abstract member toggle: (unit -> unit) with get, set
        abstract member generateId: (string -> string) with get, set
        abstract member registerContentId: (string -> (unit -> unit)) with get, set
        
    [<Import("useCollapsibleContext", Spec.collapsible)>]
    let useCollapsibleContext (): CollapsibleContext = jsNative
