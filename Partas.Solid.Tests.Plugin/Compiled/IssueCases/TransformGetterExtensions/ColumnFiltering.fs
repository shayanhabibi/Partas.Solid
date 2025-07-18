namespace Partas.Solid.TanStack.Table

open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop

[<AutoOpen; Erase>]
module rec ColumnFilteringTypes =
    [<AllowNullLiteral; Interface>]
    type ColumnFiltersTableState =
        abstract member columnFilters: ColumnFiltersState with get,set
    
    type ColumnFiltersState = ColumnFilter[]
    
    [<AllowNullLiteral; Interface>]
    type ColumnFilter =
        abstract member id: string with get
        abstract member value: obj with get

    type FilterFn = interface end
    type BuiltInFilterFn = FilterFn
    /// <summary>
    /// Every filter function receives:
    /// <ul>
    /// <li>The row to filter</li>
    /// <li>The columnId to use to retrieve the row's value</li>
    /// <li>The filter value</li>
    /// </ul><br/>
    /// and should return true if the row should be included in the filtered rows, and false if it should be removed.
    /// </summary>
    [<Import("filterFns", "@tanstack/table-core")>]
    [<AllowNullLiteral; Interface>]
    type filterFns =
        /// Case-insensitive string inclusion
        abstract member includesString: BuiltInFilterFn with get
        /// Case-sensitive string inclusion
        abstract member includesStringSensitive: BuiltInFilterFn with get
        /// Case-insensitive string equality
        abstract member equalsString: BuiltInFilterFn with get
        /// Case-sensitive string equality
        abstract member equalsStringSensitive: BuiltInFilterFn with get
        /// Item inclusion within an array
        abstract member arrIncludes: BuiltInFilterFn with get
        /// All items included in an array
        abstract member arrIncludesAll: BuiltInFilterFn with get
        /// Some items included in an array
        abstract member arrIncludesSome: BuiltInFilterFn with get
        /// <summary>Object/referential equality <c>Object.is</c>/<c>===</c></summary>
        abstract member equals: BuiltInFilterFn with get
        /// <summary>Weak object/referential equality <c>==</c></summary>
        abstract member weakEquals: BuiltInFilterFn with get
        /// Number range inclusion
        abstract member inNumberRange: BuiltInFilterFn with get
    

    type ColumnDef<'Data>  with
        /// <summary>
        /// The filter function to use with this column.<br/>
        /// Options:
        /// <ul>
        /// <li>A string referencing a built-in filter function)</li>
        /// <li>A custom filter function</li>
        /// </ul>
        /// </summary>
        member _.filterFn with set(value: FilterFn) = () and get(): FilterFn = unbox null
        /// Enables/disables the column filter for this column.
        member _.enableColumnFilter with set(value: bool) = () and get(): bool = unbox null


    type Column<'Data> with
        member _.getCanFilter with get(): (unit -> bool) = unbox null
        member _.getFilterIndex with get(): (unit -> int) = unbox null
        member _.getIsFiltered with get(): (unit -> bool) = unbox null
        member _.getFilterValue with get(): (unit -> obj) = unbox null
        member _.setFilterValue with get(): (Updater<obj> -> unit) = unbox null
        member _.getAutoFilterFn with get(): (string -> FilterFn) = unbox null
        member _.getFilterFn with get(): (string -> FilterFn) = unbox null

    type Row<'Data> with
        member _.columnFilters with get(): JS.Object = unbox null
        member _.columnFiltersMeta with get(): JS.Object = unbox null

    type TableOptions<'Data> with
        member _.filterFns with set(value: obj) = ()
        member _.filterFromLeafRows with set(value: bool) = ()
        member _.maxLeafRowFilterDepth with set(value: int) = ()
        member _.enableFilters with set(value: bool) = ()
        member _.manualFiltering with set(value: bool) = ()
        member _.onColumnFiltersChange with set(value: OnChangeFn<ColumnFiltersState>) = ()
        member _.enableColumnFilters with set(value: bool) = ()
        member _.getFilteredRowModel with set(fn: Table<'Data> -> (unit -> RowModel<'Data>)) = ()
    
    type Table<'Data> with
        member _.setColumnFilters with get(): Updater<ColumnFiltersState> -> unit = unbox null
        member _.resetColumnFilters with get(): bool -> unit = unbox null
        member _.getPreFilteredRowModel with get(): (unit -> RowModel<'Data>) = unbox null
        member _.getFilteredRowModel with get(): (unit -> RowModel<'Data>) = unbox null
        