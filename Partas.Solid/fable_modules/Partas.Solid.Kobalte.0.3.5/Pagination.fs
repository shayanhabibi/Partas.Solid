namespace Partas.Solid.Kobalte

open Partas.Solid
open Fable.Core

[<Erase; Import("Root", Spec.pagination)>]
type Pagination() =
    inherit div()
    interface Polymorph
    /// <summary>
    /// The controlled page number of the pagination. (1-indexed)
    /// </summary>
    [<DefaultValue>]
    val mutable page: int
    /// <summary>
    /// The default page number when initially rendered. (1-indexed)
    /// Useful when you do not need to control the page number.
    /// </summary>
    [<DefaultValue>]
    val mutable defaultPage: int
    /// <summary>
    /// Event handler called when the page number changes.
    /// </summary>
    [<DefaultValue>]
    val mutable onPageChange: (int -> unit)
    /// <summary>
    /// The number of pages for the pagination.
    /// </summary>
    [<DefaultValue>]
    val mutable count: int
    /// <summary>
    /// The number of siblings to show around the current page item.
    /// </summary>
    [<DefaultValue>]
    val mutable siblingCount: int
    /// <summary>
    /// Whether to always show the first page item.
    /// </summary>
    [<DefaultValue>]
    val mutable showFirst: bool
    /// <summary>
    /// Whether to always show the last page item.
    /// </summary>
    [<DefaultValue>]
    val mutable showLast: bool
    /// <summary>
    /// Whether to always show the same number of items (to avoid content shift).
    /// Special value: "no-ellipsis" does not count the ellipsis as an item (used when ellipsis are disabled).
    /// </summary>
    [<DefaultValue>]
    val mutable fixedItems: PaginationFixedItems
    /// <summary>
    /// The component to render as an item in the <c>Pagination.List</c>.
    /// </summary>
    [<DefaultValue>]
    val mutable itemComponent: ({| page: int |} -> HtmlElement)
    /// <summary>
    /// The component to render as an ellipsis item in the <c>Pagination.List</c>.
    /// </summary>
    [<DefaultValue>]
    val mutable ellipsisComponent: (unit -> HtmlElement)
    /// <summary>
    /// Whether the pagination is disabled.
    /// </summary>
    [<DefaultValue>]
    val mutable disabled: bool

[<Erase; RequireQualifiedAccess>]
module Pagination =
    [<Erase; Import("Item", Spec.pagination)>]
    type Item() =
        inherit Button()
        interface Polymorph
        [<DefaultValue>] val mutable page : int 
    [<Erase; Import("Ellipsis", Spec.pagination)>]
    type Ellipsis() =
        inherit div()
        interface Polymorph
    [<Erase; Import("Previous", Spec.pagination)>]
    type Previous() =
        inherit Button()
        interface Polymorph
    [<Erase; Import("Next", Spec.pagination)>]
    type Next() =
        inherit Button()
        interface Polymorph
    [<Erase; Import("Items", Spec.pagination)>]
    type Items() =
        inherit div()
        interface Polymorph
    

[<AutoOpen;Erase>]
module PaginationContext =
    [<AllowNullLiteral>]
    [<Interface>]
    type PaginationContext =
        abstract member count: Accessor<int> with get
        abstract member siblingCount: Accessor<int> with get
        abstract member showFirst: Accessor<bool> with get
        abstract member showLast: Accessor<bool> with get
        abstract member fixedItems: Accessor<PaginationFixedItems> with get
        abstract member isDisabled: Accessor<bool> with get
        abstract member renderItem: (int -> JSX.Element) with get
        abstract member renderEllipsis: (unit -> JSX.Element) with get
        abstract member page: Accessor<int> with get
        abstract member setPage: Setter<int> with get

    [<ImportMember(Spec.pagination)>]
    let usePaginationContext () : PaginationContext = nativeOnly
