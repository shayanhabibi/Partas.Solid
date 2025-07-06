namespace Partas.Solid.Kobalte

open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop
open Browser.Types
open Partas.Solid.Experimental.U

[<RequireQualifiedAccess; Erase>]
module Combobox =
    [<AllowNullLiteral>]
    [<Interface>]
    type DataSet =
        abstract member ``data-expanded``: string with get,set
        abstract member ``data-closed``: string with get,set
    [<AllowNullLiteral>]
    [<Interface>]
    type ControlState<'T> =
        inherit FormControlDataSet
        inherit DataSet
        abstract member selectedOptions : Accessor<'T[]> with get
        abstract member remove : ('T -> unit) with get
        abstract member clear : (unit -> unit) with get 

    /// <summary>
    /// </summary>
    /// <param name="data-valid">Present when the combobox is valid</param>
    /// <param name="data-invalid">Present when the combobox is invalid</param>
    /// <param name="data-required">Present when the combobox is required</param>
    /// <param name="data-disabled">Present when the combobox is disabled</param>
    /// <param name="data-readonly">Present when the combobox is readonly</param>
    [<Erase; Import("Control", Spec.combobox)>]
    type Control<'T>() = // v0.13.9
        interface HtmlTag
        interface Polymorph
        interface ChildLambdaProvider<ControlState<'T>>
    [<Erase; Import("Control", Spec.combobox)>]
    type Control = Control<obj>
    /// <summary>
    /// </summary>
    /// <param name="data-valid">Present when the combobox is valid</param>
    /// <param name="data-invalid">Present when the combobox is invalid</param>
    /// <param name="data-required">Present when the combobox is required</param>
    /// <param name="data-disabled">Present when the combobox is disabled</param>
    /// <param name="data-readonly">Present when the combobox is readonly</param>
    /// <param name="data-expanded">Present when combobox is open</param>
    /// <param name="data-closed">Present when combobox is closed</param>
    [<Erase; Import("Trigger", Spec.combobox)>]
    type Trigger() = // v0.13.9
        inherit Button()
        interface Polymorph
    /// <param name="data-expanded">Present when combobox is open</param>
    /// <param name="data-closed">Present when combobox is closed</param>
    [<Erase; Import("Icon", Spec.combobox)>]
    type Icon() = // v0.13.9
        inherit div()
        interface Polymorph
    /// <summary>
    /// </summary>
    /// <param name="data-valid">Present when the combobox is valid</param>
    /// <param name="data-invalid">Present when the combobox is invalid</param>
    /// <param name="data-required">Present when the combobox is required</param>
    /// <param name="data-disabled">Present when the combobox is disabled</param>
    /// <param name="data-readonly">Present when the combobox is readonly</param>
    [<Erase; Import("ErrorMessage", Spec.combobox)>]
    type ErrorMessage() = // v0.13.9
        inherit div()
        interface Polymorph
        [<Erase>] [<DefaultValue>] val mutable forceMount : bool  // v0.13.9
    /// <param name="data-expanded">Present when combobox is open</param>
    /// <param name="data-closed">Present when combobox is closed</param>
    [<Erase; Import("Content", Spec.combobox)>]
    type Content() = // v0.13.9
        inherit div()
        interface Polymorph
        /// <summary>
        /// Event handler called when focus moves to the trigger after closing.
        /// It can be prevented by calling <c>event.preventDefault</c>.
        /// </summary>
        [<DefaultValue>]
        val mutable onCloseAutoFocus: (Event -> unit) option

        /// <summary>
        /// Event handler called when a pointer event occurs outside the bounds of the component.
        /// It can be prevented by calling <c>event.preventDefault</c>.
        /// </summary>
        [<DefaultValue>]
        val mutable onPointerDownOutside: (CustomEvent<EventDetails<PointerEvent>> -> unit) option

        /// <summary>
        /// Event handler called when the focus moves outside the bounds of the component.
        /// It can be prevented by calling <c>event.preventDefault</c>.
        /// </summary>
        [<DefaultValue>]
        val mutable onFocusOutside: (CustomEvent<EventDetails<FocusEvent>> -> unit) option

        /// <summary>
        /// Event handler called when an interaction (pointer or focus event) happens outside the bounds of the component.
        /// It can be prevented by calling <c>event.preventDefault</c>.
        /// </summary>
        [<DefaultValue>]
        val mutable onInteractOutside: (CustomEvent<EventDetails<PointerEvent>> -> unit) option
    [<Erase; Import("Arrow", Spec.combobox)>]
    type Arrow() = // v0.13.9
        inherit div()
        interface Polymorph
        [<Erase>] [<DefaultValue>] val mutable size : int  // v0.13.9
    [<Erase; Import("Listbox", Spec.combobox)>]
    type Listbox<'T>() = // v0.13.9
        inherit div()
        interface Polymorph
        [<Erase>] [<DefaultValue>] val mutable scrollRef : Accessor<HtmlElement>  // v0.13.9
        [<Erase>] [<DefaultValue>] val mutable scrollToItem : string -> unit  // v0.13.9
    [<Erase; Import("Listbox", Spec.combobox)>]
    type ListboxVirtualiser<'T>() =
        interface HtmlTag
        interface Polymorph
        interface ChildLambdaProvider<'T[]>
        [<Erase>] [<DefaultValue>] val mutable scrollRef : Accessor<HtmlElement>  // v0.13.9
        [<Erase>] [<DefaultValue>] val mutable scrollToItem : string -> unit  // v0.13.9
    [<Erase; Import("Listbox", Spec.combobox)>]
    type Listbox = Listbox<obj>
    [<Erase; Import("Listbox", Spec.combobox)>]
    type ListboxVirtualiser = ListboxVirtualiser<obj>
    /// <param name="data-disabled">Present when the item is disabled</param>
    /// <param name="data-selected">Present when the item is selected</param>
    /// <param name="data-highlighted">Present when the item is highlighted</param>
    [<Erase; Import("Item", Spec.combobox)>]
    type Item<'Value>() = // v0.13.9
        inherit li()
        interface Polymorph
        [<Erase>] [<DefaultValue>] val mutable item : 'Value 
    [<Erase; Import("Item", Spec.combobox)>]
    type Item = Item<obj>
    /// <param name="data-disabled">Present when the item is disabled</param>
    /// <param name="data-selected">Present when the item is selected</param>
    /// <param name="data-highlighted">Present when the item is highlighted</param>
    [<Erase; Import("ItemIndicator", Spec.combobox)>]
    type ItemIndicator() = // v0.13.9
        inherit div()
        interface Polymorph
        [<Erase>] [<DefaultValue>] val mutable forceMount : bool 
    /// <param name="data-disabled">Present when the item is disabled</param>
    /// <param name="data-selected">Present when the item is selected</param>
    /// <param name="data-highlighted">Present when the item is highlighted</param>
    [<Erase; Import("ItemLabel", Spec.combobox)>]
    type ItemLabel() = // v0.13.9
        inherit div()
        interface Polymorph
    /// <param name="data-disabled">Present when the item is disabled</param>
    /// <param name="data-selected">Present when the item is selected</param>
    /// <param name="data-highlighted">Present when the item is highlighted</param>
    [<Erase; Import("ItemDescription", Spec.combobox)>]
    type ItemDescription() = // v0.13.9
        inherit div()
        interface Polymorph
    [<Erase; Import("Section", Spec.combobox)>]
    type Section() = // v0.13.9
        inherit li()
        interface Polymorph
    /// <summary>
    /// </summary>
    /// <param name="data-valid">Present when the combobox is valid</param>
    /// <param name="data-invalid">Present when the combobox is invalid</param>
    /// <param name="data-required">Present when the combobox is required</param>
    /// <param name="data-disabled">Present when the combobox is disabled</param>
    /// <param name="data-readonly">Present when the combobox is readonly</param>
    [<Erase; Import("Label", Spec.combobox)>]
    type Label() = // v0.13.9
        inherit label()
        interface Polymorph
    /// <summary>
    /// </summary>
    /// <param name="data-valid">Present when the combobox is valid</param>
    /// <param name="data-invalid">Present when the combobox is invalid</param>
    /// <param name="data-required">Present when the combobox is required</param>
    /// <param name="data-disabled">Present when the combobox is disabled</param>
    /// <param name="data-readonly">Present when the combobox is readonly</param>
    [<Erase; Import("Description", Spec.combobox)>]
    type Description() = // v0.13.9
        inherit div()
        interface Polymorph
    [<Erase; Import("Portal", Spec.combobox)>]
    type Portal() = // v0.13.9
        inherit div()
        interface Polymorph
    [<Erase; Import("HiddenSelect", Spec.combobox)>]
    type HiddenSelect() = // v0.13.9
        interface RegularNode
        interface Polymorph
    /// <summary>
    /// </summary>
    /// <param name="data-valid">Present when the combobox is valid</param>
    /// <param name="data-invalid">Present when the combobox is invalid</param>
    /// <param name="data-required">Present when the combobox is required</param>
    /// <param name="data-disabled">Present when the combobox is disabled</param>
    /// <param name="data-readonly">Present when the combobox is readonly</param>
    [<Erase; Import("Input",  Spec.combobox)>]
    type Input() = // v0.13.9
        inherit input()
        interface Polymorph

/// <summary>
/// </summary>
/// <param name="data-valid">Present when the combobox is valid</param>
/// <param name="data-invalid">Present when the combobox is invalid</param>
/// <param name="data-required">Present when the combobox is required</param>
/// <param name="data-disabled">Present when the combobox is disabled</param>
/// <param name="data-readonly">Present when the combobox is readonly</param>
[<Erase; Import("Root", Spec.combobox)>]
type Combobox<'Value>() =
    inherit div()
    interface Polymorph
    [<Erase>] [<DefaultValue>] val mutable defaultFilter : ComboboxFilter  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable options : 'Value[]  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable optionValue : 'Value -> string  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable optionTextValue : 'Value -> string  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable optionLabel : 'Value -> string  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable optionDisabled : 'Value -> bool  // v0.13.9
    /// <summary>
    /// Key property that refers to children of the option group.
    /// </summary>
    [<Erase>] [<DefaultValue>] val mutable optionGroupChildren : string  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable itemComponent : ItemComponentProps<'Value> -> HtmlElement  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable sectionComponent : SectionComponentProps<'Value> -> HtmlElement  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable multiple : bool  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable placeholder : string  // v0.13.9
    member this.placeholder' // v0.13.9
        with set(value: HtmlElement) = () // v0.13.9
        and get(): HtmlElement = unbox null // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable value : 'Value  // will add an s when it is an array and reroute it to value
    member this.values // v0.13.9
        with inline set(values: 'Value[]) = this.value <- !!values // v0.13.9
        and inline get(): 'Value[] = !!this.value // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable defaultValue : 'Value  // v0.13.9
    member this.defaultValues // v0.13.9
        with inline set(values: 'Value[]) = this.defaultValue <- !!values // v0.13.9
        and inline get(): 'Value[] = !!this.defaultValue // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable onChange : 'Value -> unit  // v0.13.9
    member this.onChanges // v0.13.9
        with inline set(values: 'Value[] -> unit) = this.onChange <- !!values // v0.13.9
        and inline get(): 'Value[] -> unit = !!this.onChange // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable open' : bool  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable defaultOpen : bool  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable onOpenChange : bool * TriggerMode -> unit  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable onInputChange : string -> unit  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable triggerMode : TriggerMode  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable removeOnBackspace : bool  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable allowDuplicateSelectionEvents : bool  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable disallowEmptySelection : bool  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable allowsEmptyCollection : bool  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable closeOnSelection : bool  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable selectionBehavior : SelectionBehavior  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable virtualized : bool  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable modal : bool  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable preventScroll : bool  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable forceMount : bool  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable name : string  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable validationState : ValidationState  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable required : bool  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable disabled : bool  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable readOnly : bool  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable translations : string  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable autoComplete : string  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable noResetInputOnBlur : bool  // v0.13.9

    [<Erase>] [<DefaultValue>] val mutable placement : KobaltePlacement  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable gutter : int  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable shift : int  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable flip : bool  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable slide : bool  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable overlap : bool  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable sameWidth : bool  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable fitViewport : bool  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable hideWhenDetached : bool  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable detachedPadding : int  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable arrowPadding : int  // v0.13.9
    [<Erase>] [<DefaultValue>] val mutable overflowPadding : int  // v0.13.9
[<Erase; Import("Root", Spec.combobox)>]
type Combobox = Combobox<obj>

[<AutoOpen; Erase>]
module ComboboxContext =
    [<RequireQualifiedAccess; Erase>]
    module ComboboxContext =
        type open' =
            delegate of focusStrategy: U2<obj, bool> * ?triggerMode: TriggerMode -> unit

        type toggle =
            delegate of focusStrategy: U2<obj, bool> * ?triggerMode: TriggerMode -> unit


    [<AllowNullLiteral>]
    [<Interface>]
    type ComboboxContext<'Value> =
        abstract member dataset: Accessor<Combobox.DataSet> with get, set
        abstract member isOpen: Accessor<bool> with get, set
        abstract member isDisabled: Accessor<bool> with get, set
        abstract member isMultiple: Accessor<bool> with get, set
        abstract member isVirtualized: Accessor<bool> with get, set
        abstract member isModal: Accessor<bool> with get, set
        abstract member preventScroll: Accessor<bool> with get, set
        abstract member isInputFocused: Accessor<bool> with get, set
        abstract member allowsEmptyCollection: Accessor<bool> with get, set
        abstract member shouldFocusWrap: Accessor<bool> with get, set
        abstract member removeOnBackspace: Accessor<bool> with get, set
        abstract member selectedOptions: Accessor<'Value[]> with get, set
        abstract member contentPresent: Accessor<bool> with get, set
        abstract member autoFocus: Accessor<U2<obj, bool>> with get, set
        abstract member activeDescendant: Accessor<string option> with get, set
        abstract member inputValue: Accessor<string option> with get, set
        abstract member triggerMode: Accessor<TriggerMode> with get, set
        abstract member controlRef: Accessor<HTMLElement option> with get, set
        abstract member inputRef: Accessor<HTMLInputElement option> with get, set
        abstract member triggerRef: Accessor<HTMLElement option> with get, set
        abstract member contentRef: Accessor<HTMLElement option> with get, set
        abstract member listboxId: Accessor<string option> with get, set
        abstract member triggerAriaLabel: Accessor<string option> with get, set
        abstract member listboxAriaLabel: Accessor<string option> with get, set
        abstract member listState: Accessor<ListState<'Value>> with get, set
        abstract member keyboardDelegate: Accessor<KeyboardDelegate> with get, set
        abstract member resetInputValue: (Set<string> -> unit) with get, set
        abstract member setIsInputFocused: (bool -> unit) with get, set
        abstract member setInputValue: (string -> unit) with get, set
        abstract member setControlRef: (HTMLElement -> unit) with get, set
        abstract member setInputRef: (HTMLInputElement -> unit) with get, set
        abstract member setTriggerRef: (HTMLElement -> unit) with get, set
        abstract member setContentRef: (HTMLElement -> unit) with get, set
        abstract member setListboxRef: (HTMLElement -> unit) with get, set
        abstract member ``open``: ComboboxContext.open' with get, set
        abstract member close: (unit -> unit) with get, set
        abstract member toggle: ComboboxContext.toggle with get, set
        abstract member placeholder: Accessor<HtmlElement> with get, set
        abstract member renderItem: (CollectionNode<'Value> -> HtmlElement) with get, set
        abstract member renderSection: (CollectionNode<'Value> -> HtmlElement) with get, set
        abstract member removeOptionFromSelection: (obj -> unit) with get, set
        abstract member onInputKeyDown: (KeyboardEvent -> unit) with get, set
        abstract member generateId: (string -> string) with get, set
        abstract member registerListboxId: (string -> (unit -> unit)) with get, set

    [<Import("useComboboxContext", Spec.combobox)>]
    let useComboboxContext<'Value>(): ComboboxContext<'Value> = jsNative
