namespace Partas.Solid.Kobalte

open Fable.Core
open Fable.Core.JsInterop
open Partas.Solid


[<Erase; Import("Root", Spec.select)>]
type Select<'T>() =
    inherit div()
    interface Polymorph
    [<DefaultValue>] val mutable options : 'T[] 
    [<DefaultValue>] val mutable optionValue : 'T -> U3<string, float, int> 
    [<DefaultValue>] val mutable optionTextValue : 'T -> string 
    [<DefaultValue>] val mutable optionDisabled : 'T -> bool 
    [<DefaultValue>] val mutable optionGroupChildren : string 
    [<DefaultValue>] val mutable itemComponent : ItemComponentProps<'T> -> HtmlElement 
    [<DefaultValue>] val mutable sectionComponent : SectionComponentProps<'T> -> HtmlElement 
    [<DefaultValue>] val mutable multiple : bool 
    [<DefaultValue>] val mutable placeholder : HtmlElement 
    [<DefaultValue>] val mutable value : 'T 
    member this.values
        with inline set(values: 'T[]) = this.value <- !!values 
    [<DefaultValue>] val mutable defaultValue : 'T 
    member this.defaultValues
        with inline set(values: 'T[]) = this.defaultValue <- !!values
    [<DefaultValue>] val mutable onChange : 'T -> unit 
    member this.onChanges
        with inline set(handler: 'T[] -> unit) = this.onChange <- !!handler
    [<DefaultValue>] val mutable open' : bool 
    [<DefaultValue>] val mutable defaultOpen : bool 
    [<DefaultValue>] val mutable onOpenChange : (bool -> unit) 
    [<DefaultValue>] val mutable allowDuplicateSelectionEvents : bool 
    [<DefaultValue>] val mutable disallowEmptySelection : bool 
    [<DefaultValue>] val mutable closeOnSelection : bool 
    [<DefaultValue>] val mutable selectionBehavior : SelectionBehavior 
    [<DefaultValue>] val mutable virtualized : bool 
    [<DefaultValue>] val mutable modal : bool 
    [<DefaultValue>] val mutable preventScroll : bool 
    [<DefaultValue>] val mutable forceMount : bool 
    [<DefaultValue>] val mutable name : string 
    [<DefaultValue>] val mutable validationState : ValidationState 
    [<DefaultValue>] val mutable required : bool 
    [<DefaultValue>] val mutable disabled : bool 
    [<DefaultValue>] val mutable readOnly : bool 
    [<DefaultValue>] val mutable autoComplete : string 

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
type Select = Select<obj>
[<RequireQualifiedAccess; Erase>]
module Select =
    [<Interface; Erase>]
    type ValueState<'T> =
        abstract selectedOption: Accessor<'T>
        abstract selectedOptions: Accessor<'T[]>
        abstract remove: ('T -> unit)
        abstract clear: (unit -> unit)
    [<Erase; Import("Trigger", Spec.select)>]
    type Trigger() =
        inherit Button()
        interface Polymorph
    [<Erase; Import("Value", Spec.select)>]
    type Value<'T>() =
        interface HtmlTag
        interface Polymorph
        interface ChildLambdaProvider<ValueState<'T>>
        [<DefaultValue>] val mutable selectedOption : 'T Accessor 
        [<DefaultValue>] val mutable selectedOptions : 'T[] Accessor 
        [<DefaultValue>] val mutable remove : 'T -> unit 
        [<DefaultValue>] val mutable clear : unit -> unit 
    type Value = Value<obj>
    [<Erase; Import("Icon", Spec.select)>]
    type Icon() =
        inherit div()
        interface Polymorph
    [<Erase; Import("ErrorMessage", Spec.select)>]
    type ErrorMessage() =
        inherit div()
        interface Polymorph
        [<DefaultValue>] val mutable forceMount : bool 
    [<Erase; Import("Content", Spec.select)>]
    type Content() =
        inherit div()
        interface Polymorph
    [<Erase; Import("Arrow", Spec.select)>]
    type Arrow() =
        inherit div()
        interface Polymorph
        [<DefaultValue>] val mutable size : int 
    [<Erase; Import("Listbox", Spec.select)>]
    type Listbox() =
        inherit div()
        interface Polymorph
        [<DefaultValue>] val mutable scrollRef : unit -> HtmlElement 
        [<DefaultValue>] val mutable scrollToItem : string -> unit 
        [<DefaultValue>] val mutable children : obj[] -> HtmlElement 
    [<Erase; Import("Item", Spec.select)>]
    type Item<'T>() =
        inherit div()
        interface Polymorph
        [<DefaultValue>] val mutable item : 'T 
    type Item = Item<obj>
    [<Erase; Import("ItemIndicator", Spec.select)>]
    type ItemIndicator() =
        inherit div()
        interface Polymorph
        [<DefaultValue>] val mutable forceMount : bool 
    [<Erase; Import("Label", Spec.select)>]
    type Label() =
        inherit span()
        interface Polymorph
    [<Erase; Import("Description", Spec.select)>]
    type Description() =
        inherit div()
        interface Polymorph
    [<Erase; Import("Portal", Spec.select)>]
    type Portal() =
        inherit div()
        interface Polymorph
    [<Erase; Import("Section", Spec.select)>]
    type Section() =
        inherit li()
        interface Polymorph
    [<Erase; Import("ItemLabel", Spec.select)>]
    type ItemLabel() =
        inherit div()
        interface Polymorph
    [<Erase; Import("ItemDescription", Spec.select)>]
    type ItemDescription() =
        inherit div()
        interface Polymorph
    [<Erase; Import("HiddenSelect", Spec.select)>]
    type HiddenSelect() =
        inherit div()
        interface Polymorph

