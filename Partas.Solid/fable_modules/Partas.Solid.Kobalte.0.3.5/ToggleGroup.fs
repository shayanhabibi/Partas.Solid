namespace Partas.Solid.Kobalte

open Fable.Core
open Partas.Solid
open Partas.Solid.Experimental.U


[<Erase; Import("Root", Spec.toggleGroup)>]
type ToggleGroup() =
    inherit div()
    interface Polymorph
    /// <summary>
    /// The controlled value of the toggle group.
    /// </summary>
    [<DefaultValue>]
    val mutable value: U2<string, string[]>

    /// <summary>
    /// The value of the select when initially rendered.
    /// Useful when you do not need to control the value.
    /// </summary>
    [<DefaultValue>]
    val mutable defaultValue: U2<string, string[]>

    /// <summary>
    /// Event handler called when the value changes.
    /// </summary>
    [<DefaultValue>]
    val mutable onChange: (U2<string, string[]> -> unit)

    /// <summary>
    /// The type of selection that is allowed in the toggle group.
    /// </summary>
    [<DefaultValue>]
    val mutable selectionMode: SelectionMode

    /// <summary>
    /// Whether the toggle group is disabled.
    /// </summary>
    [<DefaultValue>]
    val mutable disabled: bool

    /// <summary>
    /// The axis the toggle group items should align with.
    /// </summary>
    [<DefaultValue>]
    val mutable orientation: Orientation

[<RequireQualifiedAccess; Erase>]
module ToggleGroup =
    [<Erase; Import("Item", Spec.toggleGroup)>]
    type Item() =
        inherit div()
        interface Polymorph
        [<DefaultValue>] val mutable value : string 
        [<DefaultValue>] val mutable disabled : bool 
        [<DefaultValue>] val mutable children : Fragment 
        [<DefaultValue>] val mutable pressed : unit -> bool 

[<Erase; AutoOpen>]
module ToggleGroupContext =
    [<AllowNullLiteral>]
    [<Interface>]
    type ToggleGroupContext =
        abstract member isMultiple: Accessor<bool> with get, set
        abstract member isDisabled: Accessor<bool> with get, set
        abstract member listState: Accessor<ListState<string>> with get, set
        abstract member generateId: (string -> string) with get, set
        abstract member orientation: Accessor<Orientation> with get, set
    
    [<ImportMember(Spec.toggleGroup)>]
    let useToggleGroupContext(): ToggleGroupContext = jsNative
