namespace Partas.Solid.TanStack.Table

open System.Runtime.CompilerServices
open Fable.Core.JS
open FSharp.Core
open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop
open System
open Partas.Solid.Experimental.U

#if !FABLE_COMPILER
type StringEnumAttribute() = inherit Attribute()
type CompiledValueAttribute(value) =
    inherit Attribute()
    new (value: int) = CompiledValueAttribute(true)
    new (value: string) = CompiledValueAttribute(true)
type ImportAttribute(value,path) =
    inherit Attribute()
type EraseAttribute() =
    inherit Attribute()
type PojoAttribute() =
    inherit Attribute()
type IndexerAttribute() =
    inherit Attribute()
type EmitIndexerAttribute() =
    inherit Attribute()
#endif

[<AutoOpen; Erase>]
module rec Table =
    [<AutoOpen>]
    type Exports =
        [<Import("createSolidTable", "@tanstack/solid-table")>]
        static member createTable<'Data>(options: TableOptions<'Data>): Table<'Data> = jsNative
        [<Import("getCoreRowModel", "@tanstack/solid-table")>]
        static member getCoreRowModel<'Data>(): CoreRowModel<'Data> = jsNative
        [<Import("getPaginationRowModel", "@tanstack/solid-table")>]
        static member getPaginationRowModel<'Data>(): CoreRowModel<'Data> = jsNative
        [<ImportMember(solidTable)>]
        static member getFilteredRowModel<'Data>(): CoreRowModel<'Data> = jsNative
        [<ImportMember(solidTable)>]
        static member getSortedRowModel<'Data>(): CoreRowModel<'Data> = jsNative
        [<ImportMember(solidTable)>]
        static member getFacetedRowModel<'Data>(): unit -> RowModel<'Data> = jsNative
        [<ImportMember(solidTable)>]
        static member getFacetedUniqueValues<'Data>(): unit -> JS.Map<_,int> = jsNative
        [<ImportMember(solidTable)>]
        static member getFacetedMinMaxValues<'Data>(): JS.Map<_,int> = jsNative
        [<Import("flexRender", "@tanstack/solid-table")>]
        static member flexRender<'Data>(x, y): HtmlElement = jsNative
    

    [<AllowNullLiteral;Interface>]
    type TableFeature = interface end
    
    [<AutoOpen; Erase>]
    module ColumnFeatures =
        [<AutoOpen; Erase>]
        module Filter =
            [<AllowNullLiteral; Interface>]
            type ColumnFilter =
                abstract member id: string with get
                abstract member value: obj with get
            type BuiltInFilterFn = FilterFn<obj>
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
            type FilterFns =
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

  
    [<AutoOpen; Erase>]
    module Models =
        [<AllowNullLiteral; Interface>]
        type RowModel<'Data> =
            abstract member rows: Row<'Data>[] with get,set
            abstract member flatRows: Row<'Data>[] with get,set
            abstract member rowsById: System.Collections.Generic.IDictionary<string, Row<'Data>> with get,set
            
        type CoreRowModel<'Data> = ((Table<'Data> -> unit) -> RowModel<'Data>)
    
    [<AutoOpen; Erase>]
    module RenderProps =
        [<Pojo>]
        type Renderable<'Data>(
                table: Table<'Data>,
                row: Row<'Data>,
                column: Column<'Data>,
                cell: Cell<'Data>,
                getValue: (unit -> obj),
                renderValue: (unit -> obj)
            ) =
            member val table = table with get,set
            member val row = row with get,set
            member val column = column with get,set
            member val cell = cell with get,set
            member val getValue = getValue with get,set
            member val renderValue = renderValue with get,set
        [<AllowNullLiteral; Interface>]
        type HeaderRenderProp<'Data> =
            abstract member header: Header<'Data> with get
    
        [<AllowNullLiteral; Interface>]
        type TableRenderProp<'Data> =
            abstract member table: Table<'Data> with get
            
        [<AllowNullLiteral; Interface>]
        type ColumnRenderProp<'Data> =
            abstract member column: Column<'Data> with get
    
        [<AllowNullLiteral; Interface>]
        type RowRenderProp<'Data> =
            abstract member row: Row<'Data> with get
    
        [<AllowNullLiteral; Interface>]
        type CellRenderProp<'Data> =
            abstract member cell: Cell<'Data> with get
    
    
        [<AllowNullLiteral; Interface>]
        type HeaderRenderProps<'Data> =
            inherit HeaderRenderProp<'Data>
            inherit TableRenderProp<'Data>
            inherit ColumnRenderProp<'Data>

        [<AllowNullLiteral; Interface>]
        type FooterRenderProps<'Data> =
            inherit HeaderRenderProp<'Data>
            inherit TableRenderProp<'Data>
            inherit ColumnRenderProp<'Data>

        [<AllowNullLiteral; Interface>]
        type CellRenderProps<'Data> = 
            inherit TableRenderProp<'Data>
            inherit RowRenderProp<'Data>
            inherit ColumnRenderProp<'Data>
            inherit CellRenderProp<'Data>
            abstract member getValue: (unit -> obj) with get
            abstract member renderValue: (unit -> obj) with get
        
    [<AutoOpen; Erase>]
    module States =
        let inline private mkProperty this name getter =
            Constructors.Object.defineProperty(this, name, unbox<PropertyDescriptor> (createObj [ "get" ==> getter ]))
            |> ignore
            this
        
        [<Erase>]
        type TableStateExtensions =
            [<Extension; Erase>]
            static member inline columnOrder(this: #ColumnOrderTableState, value: unit -> ColumnOrderState): #ColumnOrderTableState =
                mkProperty this "columnOrder" value
            [<Extension; Erase>]
            static member inline columnFilters(this: #ColumnFiltersTableState, value: unit -> ColumnFiltersState): #ColumnFiltersTableState =
                mkProperty this "columnFilters" value
            [<Extension;  Erase>]
            static member inline columnVisibility(this: #VisibilityTableState, value: unit -> VisibilityState): #VisibilityTableState =
                mkProperty this "columnVisibility" value
            [<Extension; Erase>]
            static member inline columnPinning(this: #ColumnPinningTableState, value: unit -> ColumnPinningState): #ColumnPinningTableState =
                mkProperty this "columnPinning" value
            [<Extension; Erase>]
            static member inline sorting(this: #SortingTableState, value: unit -> SortingState): #SortingTableState =
                mkProperty this "sorting" value
            [<Extension; Erase>]
            static member inline expanded(this: #ExpandedTableState, value: unit -> ExpandedState): #ExpandedTableState =
                mkProperty this "expanded" value
            [<Extension; Erase>]
            static member inline grouping(this: #GroupingTableState, value: unit -> GroupingState): #GroupingTableState =
                mkProperty this "grouping" value
            [<Extension; Erase>]
            static member inline pagination(this: #PaginationTableState, value: unit -> PaginationState): #PaginationTableState =
                mkProperty this "pagination" value
            [<Extension; Erase>]
            static member inline rowSelection(this: #RowSelectionTableState, value: unit -> RowSelectionState): #RowSelectionTableState =
                mkProperty this "rowSelection" value
            [<Extension; Erase>]
            static member inline columnSizing(this: #ColumnSizingTableState, value: unit -> ColumnSizingState): #ColumnSizingTableState =
                mkProperty this "columnSizing" value
            [<Extension; Erase>]
            static member inline columnSizingInfo(this: #ColumnSizingTableState, value: unit -> ColumnSizingState): #ColumnSizingTableState =
                mkProperty this "columnSizingInfo" value

        [<AllowNullLiteral; Interface>]
        type ColumnOrderTableState =
            abstract member columnOrder: ColumnOrderState with get,set
                
        type ColumnOrderState = string[]
        [<AllowNullLiteral; Interface>]
        type ColumnFiltersTableState =
            abstract member columnFilters: ColumnFiltersState with get,set
        
        type ColumnFiltersState = ColumnFilter[]
        [<Erase; RequireQualifiedAccess>]
        module ColumnFiltersState = let init (): ColumnFiltersState = [||]
        /// <summary>
        /// Interface to interact with table state.<br/>
        /// When trying to instantiate a <c>TableState</c> (such as in the context of
        /// defining the <c>state</c> field in your <c>TableOptions</c>) it is best
        /// to use one of the defined helpers.<br/>
        /// The helpers will assign the fields as <c>getter properties</c> within JS
        /// which is important to maintain reactivity in Solid-js.<br/>
        /// Instead of instantiating a pojo using a constructor, we instead initialise an
        /// empty object and chain field assignment in fluent style. Alternatively, if instantiating
        /// the object for <c>TableOptions</c>, you can use the <c>stateFn</c> member for <c>TableOptions</c>
        /// </summary>
        /// <example><code lang="fsharp">
        /// let options =
        ///     TableOptions()
        ///         .stateFn(
        ///         _.rowSelection( ... )
        ///             .columnVisibility( ... )        
        ///         )
        /// </code></example>
        [<AllowNullLiteral;Interface>]
        type TableState =
            inherit VisibilityTableState
            inherit ColumnOrderTableState
            inherit ColumnPinningTableState
            inherit FiltersTableState
            inherit SortingTableState
            inherit ExpandedTableState
            inherit GroupingTableState
            inherit ColumnSizingTableState
            inherit PaginationTableState
            inherit RowSelectionTableState
            inherit ColumnFiltersTableState
            /// <summary>
            /// Chain with fluent patterns to set fields
            /// </summary>
            static member inline init: TableState = createEmpty
        [<AllowNullLiteral; Interface>]
        type VisibilityTableState =
            abstract member columnVisibility: VisibilityState with get
        [<AllowNullLiteral; Interface>]
        type VisibilityState =
            [<EmitIndexer>]
            abstract member Item: key: string -> bool with get,set
            static member inline init (): VisibilityState = createEmpty
        
        

        [<AllowNullLiteral; Erase>]
        type ColumnPinningTableState =
            abstract member columnPinning: ColumnPinningState with get
        [<JS.Pojo>]
        type ColumnPinningState(?left: string[], ?right: string[]) =
            member val left = left.Value with get,set
            member val right = right.Value with get,set
            static member inline init (): ColumnPinningState = ColumnPinningState([||], [||])
            
        [<AllowNullLiteral;Interface>]
        type FiltersTableState = interface end
        

        [<Interface; AllowNullLiteral>]
        type SortingTableState =
            abstract member sorting: SortingState with get
            
        [<Pojo>]
        type ColumnSort(id: string, desc: bool) =
            member val id = id with get,set
            member val desc = desc with get,set
        type SortingState = ColumnSort[]
        [<Erase; RequireQualifiedAccess>]
        module SortingState = let init (): SortingState = [||]

        
        
        [<Interface; AllowNullLiteral>]
        type ExpandedTableState =
            abstract member expanded: ExpandedState with get
        
        [<Interface; AllowNullLiteral>]
        type ExpandedState =
            [<EmitIndexer>]
            abstract member Item: key: string -> bool with get,set
            static member inline init (): ExpandedState = createEmpty
        
        
        
        [<AllowNullLiteral; Interface>]
        type GroupingTableState =
            abstract member grouping: GroupingState with get
        
        type GroupingState = string[]
        [<Erase; RequireQualifiedAccess>]
        module GroupingState = let init (): GroupingState = [||]

        
        [<AllowNullLiteral; Interface>]
        type PaginationTableState =
            abstract member pagination: PaginationState with get
        
        [<Pojo>]
        type PaginationState(pageIndex: int, pageSize: int) =
            member val pageIndex = pageIndex with get,set
            member val pageSize = pageSize with get,set
            // static member inline init (): PaginationState = PaginationState(0, 10)
        [<AllowNullLiteral; Interface>]
        type PaginationInitialTableState =
            abstract member pagination: PaginationState with get
        
        
        [<AllowNullLiteral; Interface>]
        type RowSelectionTableState =
            abstract member rowSelection: RowSelectionState with get
        [<AllowNullLiteral; Interface>]
        type RowSelectionState =
            [<EmitIndexer>]
            abstract member Item: key: string -> bool with get, set
            static member inline init (): RowSelectionState = createEmpty
        type OnChangeFn<'State> = Updater<'State> -> unit     
        
 
        
        [<AllowNullLiteral; Interface>]
        type ColumnSizingTableState =
            abstract member columnSizing: ColumnSizingState with get
            abstract member columnSizingInfo: ColumnSizingInfoState with get
        [<AllowNullLiteral; Interface>]
        type ColumnSizingState =
            [<EmitIndexer>]
            abstract member Item: key: string -> bool with get,set
        
        [<Pojo>]
        type ColumnSizingInfoState(
                ?startOffset: int,
                ?startSize: int,
                ?deltaOffset: int,
                ?deltaPercentage: int,
                ?isResizingColumn: string,
                ?columnSizingStart: (string * int)[]
            ) =
            member val startOffset = startOffset with get,set
            member val startSize = startSize with get,set
            member val deltaOffset = deltaOffset with get,set
            member val deltaPercentage = deltaPercentage with get,set
            member val isResizingColumn = isResizingColumn with get,set
            member val columnSizingStart = columnSizingStart with get,set
            
        [<Pojo>]
        type GlobalFilterTableState(globalFilter: obj) = member val globalFilter = globalFilter with get,set
        [<AllowNullLiteral; Interface>]
        type GlobalFilterState = interface end
        
        [<Pojo>]
        type RowPinningRowState(rowPinning: RowPinningState) = member val rowPinning = rowPinning with get,set
        [<Pojo>]
        type RowPinningState(
                ?top: string[],
                ?bottom: string[]
            ) =
            member val top = top with get,set
            member val bottom = bottom with get,set
        
        [<StringEnum>]
        type RowPinningPosition =
            | [<CompiledValue(false)>] False
            | Top
            | Bottom        
    [<AllowNullLiteral; JS.Pojo>]
    type ColumnDef<'Data>
        (
            ?id: string
            ,?accessorKey: string
            ,?accessorFn: ('Data -> int -> obj)
            ,?columns: ColumnDef<'Data>[]
            ,?header: HeaderRenderProps<'Data> -> obj
            ,?footer: FooterRenderProps<'Data> -> obj
            ,?cell: CellRenderProps<'Data> -> obj
            ,?meta: obj
            
            // Feature
            // ColumnFiltering
            ,?filterFn: FilterFn<'Data>
            ,?enableColumnFilter: bool
            
            // Feature
            // ColumnPinning
            ,?onColumnPinningChange: OnChangeFn<ColumnPinningState>
            
            // Feature
            // ColumnSizing
            ,?enableResizing: bool
            ,?size: int
            ,?minSize: int
            ,?maxSize: int
            
            // Feature
            // ColumnVisibility
            ,?enableHiding: bool
            
            // Feature
            // GlobalFiltering
            ,?enableGlobalFilter: bool
            
            // Feature
            // Sorting
            ,?sortingFn: SortingFn<'Data>
            ,?sortDescFirst: bool
            ,?enableSorting: bool
            ,?enableMultiSort: bool
            ,?invertSorting: bool
            ,?sortUndefined: SortUndefined
            
            // Feature
            // Grouping
            ,?aggregationFn: AggregationFn<'Data>
            ,?aggregatedCell: Renderable<'Data>
            ,?enableGrouping: bool
            ,?getGroupingValue: 'Data -> obj

        ) =
        
        /// <summary>
        /// The unique identifier for the column. <br/>
        /// <br/>
        /// 🧠 A column ID is optional when:<br/>
        /// <br/>
        /// An accessor column is created with an object key accessor<br/>
        /// The column header is defined as a string<br/>
        /// </summary>
        member val id: string = id.Value with get,set
        
        /// The key of the row object to use when extracting the value for the column.
        member val accessorKey: string = accessorKey.Value with get,set
        
        /// The accessor function to use when extracting the value for the column from each row.
        member val accessorFn: ('Data -> int -> obj) = accessorFn.Value with get,set
        
        /// The child column defs to include in a group column.
        member val columns: ColumnDef<'Data>[] = columns.Value with get,set
        
        /// <summary>
        /// The header to display for the column. If a string is passed, it can be used as a default for the column ID. If a function is passed, it will be passed a props object for the header and should return the rendered header value (the exact type depends on the adapter being used).
        /// <code>
        /// header?:
        ///   | string
        ///   | ((props: {
        ///       table: Table TData>
        ///       header: Header TData>
        ///       column: Column TData>
        ///     }) => unknown)
        /// </code>
        /// </summary>
        member val header: (HeaderRenderProps<'Data> -> obj) = header.Value with get,set
        
        /// <summary>
        /// The footer to display for the column. If a function is passed, it will be passed a props object for the footer and should return the rendered footer value (the exact type depends on the adapter being used).
        /// <code>
        /// footer?:
        ///   | string
        ///   | ((props: {
        ///       table: Table TData>
        ///       header: Header TData>
        ///       column: Column TData>
        ///     }) => unknown)
        /// </code>
        /// </summary>
        member val footer: (FooterRenderProps<'Data> -> obj) = footer.Value with get,set
        
        /// <summary>
        /// The cell to display each row for the column. If a function is passed, it will be passed a props object for the cell and should return the rendered cell value (the exact type depends on the adapter being used).
        /// <code>
        /// cell?:
        ///   | string
        ///   | ((props: {
        ///       table: Table TData>
        ///       row: Row TData>
        ///       column: Column TData>
        ///       cell: Cell TData>
        ///       getValue: () => any
        ///       renderValue: () => any
        ///     }) => unknown)
        /// </code>
        /// </summary>
        member val cell: CellRenderProps<'Data> -> obj = cell.Value with get,set
        
        member val meta: obj = meta.Value with get,set
        /// <summary>
        /// The filter function to use with this column.<br/>
        /// Options:
        /// <ul>
        /// <li>A string referencing a built-in filter function)</li>
        /// <li>A custom filter function</li>
        /// </ul>
        /// </summary>
        member _.filterFn with set(value: FilterFn<'Data>) = () and get(): FilterFn<'Data> = unbox null
        /// Enables/disables the column filter for this column.
        member _.enableColumnFilter with set(value: bool) = () and get(): bool = unbox null
        
        // Feature
        // ColumnPinning
        member _.onColumnPinningChange with set(value: OnChangeFn<ColumnPinningState>) = ()
        
        // Feature
        // ColumnSizing
        member _.enableResizing with set(value: bool) = ()
        member _.size with set(value: int) = ()
        member _.minSize with set(value: int) = ()
        member _.maxSize with set(value: int) = ()
        
        // Feature
        // ColumnVisibility
        member _.enableHiding with set(value: bool) = ()
        
        // Feature
        // GlobalFiltering
        member _.enableGlobalFilter with set(value: bool) = ()
        
        // Feature
        // Grouping
        member _.aggregationFn with set(value: AggregationFn<'Data>) = ()
        member _.aggregatedCell with set(value: Renderable<'Data>) = ()
        member _.enableGrouping with set(value: bool) = ()
        member _.getGroupingValue with set(value: 'Data -> obj) = ()
        
    [<JS.Pojo>]
    type TableOptions<'Data>
        (
            ?data: 'Data[]
            ,?columns: ColumnDef<'Data>[]
            ,?defaultColumn: ColumnDef<'Data>
            ,?initialState: TableState
            ,?autoResetAll: bool
            ,?meta: obj
            ,?state: TableState
            ,?onStateChange: Updater<'Data> -> unit
            ,?debugAll: bool
            ,?debugTable: bool
            ,?debugHeaders: bool
            ,?debugColumns: bool
            ,?debugRows: bool
            ,?_features: TableFeature[]
            ,?getCoreRowModel: CoreRowModel<'Data>
            ,?getSubRows: 'Data * int -> 'Data[]
            ,?getRowId: 'Data * int * Row<'Data> -> string
            
            // Feature
            // ColumnFiltering
            ,?filterFns: Map<string, FilterFn<'Data>>
            ,?filterFromLeafRows: bool
            ,?maxLeafRowFilterDepth: int
            ,?enableFilters: bool
            ,?manualFiltering: bool
            ,?onColumnFiltersChange: OnChangeFn<ColumnFiltersState>
            ,?enableColumnFilters: bool
            ,?getFilteredRowModel: CoreRowModel<'Data>
            
            // Feature
            // ColumnOrdering
            ,?onColumnOrderChange: OnChangeFn<ColumnOrderState>
            
            // Feature
            // ColumnPinning
            ,?enableColumnPinning: bool
            ,?onColumnPinningChange: OnChangeFn<ColumnPinningState>
            
            // Feature
            // ColumnSizing
            ,?enableColumnResizing: bool
            ,?columnResizeMode: ColumnResizingMode
            ,?columnResizeDirection: ColumnResizingDirection
            ,?onColumnSizingChange: OnChangeFn<ColumnSizingState>
            ,?onColumnSizingInfoChange: OnChangeFn<ColumnSizingInfoState>
            
            // Feature
            // ColumnVisibility
            ,?onColumnVisibilityChange: OnChangeFn<VisibilityState>
            ,?enableHiding: bool
            
            // Feature
            // GlobalFiltering
            ,?globalFilterFn: FilterFn<'Data>
            ,?onGlobalFilterChange: OnChangeFn<GlobalFilterState>
            ,?enableGlobalFilter: bool
            ,?getColumnCanGlobalFilter: Column<'Data> -> bool
            
            // Feature
            // Sorting
            ,?sortingFns: Map<string, SortingFn<'Data>>
            ,?manualSorting: bool
            ,?onSortingChange: OnChangeFn<SortingState>
            ,?enableSorting: bool
            ,?enableSortingRemoval: bool
            ,?enableMultiRemove: bool
            ,?enableMultiSort: bool
            ,?sortDescFirst: bool
            ,?getSortedRowModel: CoreRowModel<'Data>
            ,?maxMultiSortColCount: int
            ,?isMultiSortEvent: Browser.Types.Event -> bool
            
            // Feature
            // Grouping
            ,?aggregationFns: Map<string, AggregationFn<'Data>>
            ,?manualGrouping: bool
            ,?onGroupingChange: OnChangeFn<GroupingState>
            ,?enableGrouping: bool
            ,?getGroupedRowModel: CoreRowModel<'Data>
            ,?groupedColumnMode: GroupedColumnMode
            
            // Feature
            // Expanding
            ,?manualExpanding: bool
            ,?onExpandedChange: OnChangeFn<ExpandedState>
            ,?autoResetExpanded: bool
            ,?enableExpanding: bool
            ,?getExpandedRowModel: CoreRowModel<'Data>
            ,?getIsRowExpanded: Row<'Data> -> bool
            ,?getRowCanExpand: Row<'Data> -> bool
            ,?paginateExpandedRows: bool
            
            // Feature
            // Pagination
            ,?manualPagination: bool
            ,?pageCount: int
            ,?rowCount: int
            ,?autoResetPageIndex: bool
            ,?onPaginationChange: OnChangeFn<PaginationState>
            ,?getPaginationRowModel: CoreRowModel<'Data>
            
            // Feature
            // RowPinning
            ,?enableRowPinning: Row<'Data> -> bool
            ,?keepPinnedRows: bool
            ,?onRowPinningChanged: OnChangeFn<RowPinningState>
            
            // Feature
            // RowSelection
            ,?enableRowSelection: Row<'Data> -> bool
            ,?enableMultiRowSelection: Row<'Data> -> bool
            ,?enableSubRowSelection: Row<'Data> -> bool
            ,?onRowSelectionChange: OnChangeFn<RowSelectionState>
            
            // Feature
            // ColumnFaceting
            /// columnId -> RowModel
            ,?getColumnFacetedRowModel: (string -> RowModel<'Data>)
            ,?getFacetedRowModel: (unit -> RowModel<'Data>)
            ,?getFacetedUniqueValues: (unit -> JS.Map<_,int>)
        ) =
        /// <summary>
        /// The data for the table to display. This array should match the type you provided to <c>table.setRowType[...]</c>, but in theory could be an array of anything. It's common for each item in the array to be an object of key/values but this is not required. Columns can access this data via string/index or a functional accessor to return anything they want.<br/><br/>
        /// When the data option changes reference (compared via Object.is), the table will reprocess the data. Any other data processing that relies on the core data model (such as grouping, sorting, filtering, etc) will also be reprocessed.<br/><br/>
        /// 🧠 Make sure your <c>data</c> option is only changing when you want the table to reprocess. Providing an inline [] or constructing the data array as a new object every time you want to render the table will result in a lot of unnecessary re-processing. This can easily go unnoticed in smaller tables, but you will likely notice it in larger tables.
        /// </summary>
        /// <remarks>For reactivity, you should use the helper function <c>TableOptions.dataGetter</c> to set a `get` property</remarks>
        member val data: 'Data[] option = data with get,set
        
        /// The array of column defs to use for the table. See the Column Defs Guide for more information on creating column definitions.
        member val columns: ColumnDef<'Data>[] option = columns with get,set
        
        /// <summary>Default column options to use for all column defs supplied to the table. This is useful for providing default cell/header/footer renderers, sorting/filtering/grouping options, etc. All column definitions passed to <c>options.columns</c> are merged with this default column definition to produce the final column definitions.</summary>
        member val defaultColumn: ColumnDef<'Data> option = defaultColumn with get,set
        
        /// <summary>
        /// Use this option to optionally pass initial state to the table. This state will be used when resetting various table states either automatically by the table (eg. <c>options.autoResetPageIndex</c>) or via functions like <c>table.resetRowSelection()</c>. Most reset function allow you optionally pass a flag to reset to a blank/default state instead of the initial state.<br/><br/>
        /// 🧠 Table state will not be reset when this object changes, which also means that the initial state object does not need to be stable.
        /// </summary>
        member val initialState: TableState option = initialState with get,set
        
        /// <summary>
        /// Set this option to override any of the <c>autoReset...</c> feature options.
        /// </summary>
        member val autoResetAll: bool option = autoResetAll with get,set
        
        /// <summary>
        /// You can pass any object to <c>options.meta</c> and access it anywhere the table is available via <c>table.options.meta</c> This type is global to all tables and can be extended like so:
        /// <code>
        /// declare module '@tanstack/table-core' {
        ///   interface TableMeta [TData extends RowData] {
        ///     foo: string
        ///   }
        /// }
        /// </code>
        /// 🧠 Think of this option as an arbitrary "context" for your table. This is a great way to pass arbitrary data or functions to your table without having to pass it to every thing the table touches. A good example is passing a locale object to your table to use for formatting dates, numbers, etc or even a function that can be used to update editable data like in the editable-data example.
        /// </summary>
        member val meta: obj option = meta with get,set
        
        /// <summary>
        /// The <c>state</c> option can be used to optionally control part or all of the table state. The state you pass here will merge with and overwrite the internal automatically-managed state to produce the final state for the table. You can also listen to state changes via the <c>onStateChange</c> option.
        /// </summary>
        member val state: TableState option = state with get,set
        
        /// <summary>
        /// The <c>onStateChange</c> option can be used to optionally listen to state changes within the table. If you provide this options, you will be responsible for controlling and updating the table state yourself. You can provide the state back to the table with the <c>state</c> option.
        /// </summary>
        member val onStateChange: (Updater<'Data> -> unit) option = onStateChange with get,set
        
        /// <summary>
        /// ⚠️ Debugging is only available in development mode.<br/><br/>
        /// </summary>
        member val debugAll: bool option = debugAll with get,set
        
        /// <summary>
        /// ⚠️ Debugging is only available in development mode.<br/><br/>
        /// Set this option to true to output table debugging information to the console.
        /// </summary>
        member val debugTable: bool option = debugTable with get,set
        
        /// <summary>
        /// ⚠️ Debugging is only available in development mode.<br/><br/>
        /// Set this option to true to output header debugging information to the console.
        /// </summary>
        member val debugHeaders: bool option = debugHeaders with get,set
        
        /// <summary>
        /// ⚠️ Debugging is only available in development mode.<br/><br/>
        /// Set this option to true to output column debugging information to the console.
        /// </summary>
        member val debugColumns: bool option = debugColumns with get,set
        
        /// <summary>
        /// ⚠️ Debugging is only available in development mode.<br/><br/>
        /// Set this option to true to output row debugging information to the console.
        /// </summary>
        member val debugRows: bool option = debugRows with get,set
        
        /// An array of extra features that you can add to the table instance.
        member val _features: TableFeature[] option = _features with get,set
        
        /// <summary>
        /// This required option is a factory for a function that computes and returns the core row model for the table. It is called once per table and should return a new function which will calculate and return the row model for the table. <br/><br/>
        /// A default implementation is provided via any table adapter's <c>{ getCoreRowModel }</c> export.
        /// </summary>
        member val getCoreRowModel: CoreRowModel<'Data> option = getCoreRowModel with get,set 
        
        /// This optional function is used to access the sub rows for any given row. If you are using nested rows, you will need to use this function to return the sub rows object (or undefined) from the row.
        member val getSubRows: ('Data * int -> 'Data[]) option = getSubRows with get,set
        
        // Feature
        // ColumnFiltering
        member val filterFns: obj = filterFns with get,set 
        member val filterFromLeafRows: bool option = filterFromLeafRows with get,set 
        member val maxLeafRowFilterDepth: int option = maxLeafRowFilterDepth with get,set 
        member val enableFilters: bool option = enableFilters with get,set 
        member val manualFiltering: bool option = manualFiltering with get,set 
        member val onColumnFiltersChange: OnChangeFn<ColumnFiltersState> option = onColumnFiltersChange with get,set 
        member val enableColumnFilters: bool option = enableColumnFilters with get,set 
        member val getFilteredRowModel: CoreRowModel<'Data > option = getFilteredRowModel with get,set 
        // Feature
        // ColumnOrdering
        member val onColumnOrderChange: OnChangeFn<ColumnOrderState> option = onColumnOrderChange with get,set
        // Feature
        // ColumnPinning
        member val enableColumnPinning: bool option = enableColumnPinning with get,set
        member val onColumnPinningChange: OnChangeFn<ColumnPinningState> option = onColumnPinningChange with get,set
        // Feature
        // ColumnSizing
        member val enableColumnResizing: bool option = enableColumnResizing with get,set
        member val columnResizeMode: ColumnResizingMode option = columnResizeMode with get,set
        member val columnResizeDirection: ColumnResizingDirection option = columnResizeDirection with get,set
        member val onColumnSizingChange: OnChangeFn<ColumnSizingState> option = onColumnSizingChange with get,set
        member val onColumnSizingInfoChange: OnChangeFn<ColumnSizingInfoState> option = onColumnSizingInfoChange with get,set
        // Feature
        // ColumnVisibility
        member val onColumnVisibilityChange: OnChangeFn<VisibilityState> option = onColumnVisibilityChange with get,set
        member val enableHiding: bool option = enableHiding with get,set
        // Feature
        // GlobalFiltering
        member val globalFilterFn: FilterFn<'Data> option = globalFilterFn with get,set
        member val onGlobalFilterChange: OnChangeFn<GlobalFilterState> option = onGlobalFilterChange with get,set
        member val enableGlobalFilter: bool option = enableGlobalFilter with get,set
        member val getColumnCanGlobalFilter: (Column<'Data> -> bool) option = getColumnCanGlobalFilter with get,set
        // Feature
        // Sorting
        member val sortingFns: Map<string, SortingFn<'Data>> option = sortingFns with get,set
        member val manualSorting: bool option = manualSorting with get,set
        member val onSortingChange: OnChangeFn<SortingState> option = onSortingChange with get,set
        member val enableSorting: bool option = enableSorting with get,set
        member val enableSortingRemoval: bool option = enableSortingRemoval with get,set
        member val enableMultiRemove: bool option = enableMultiRemove with get,set
        member val enableMultiSort: bool option = enableMultiSort with get,set
        member val sortDescFirst: bool option = sortDescFirst with get,set
        member val getSortedRowModel: CoreRowModel<'Data> option = getSortedRowModel with get,set
        member val maxMultiSortColCount: int option = maxMultiSortColCount with get,set
        member val isMultiSortEvent: (Browser.Types.Event -> bool) option = isMultiSortEvent with get,set
        // Feature
        // Grouping
        member val aggregationFns: Map<string, AggregationFn<'Data>> option = aggregationFns with get,set
        member val manualGrouping: bool option = manualGrouping with get,set
        member val onGroupingChange: OnChangeFn<GroupingState> option = onGroupingChange with get,set
        member val enableGrouping: bool option = enableGrouping with get,set
        member val getGroupedRowModel: CoreRowModel<'Data> option = getGroupedRowModel with get,set
        member val groupedColumnMode: GroupedColumnMode option = groupedColumnMode with get,set
        // Feature
        // Expanding
        member val manualExpanding: bool option = manualExpanding with get,set
        member val onExpandedChange: OnChangeFn<ExpandedState> option = onExpandedChange with get,set
        member val autoResetExpanded: bool option = autoResetExpanded with get,set
        member val enableExpanding: bool option = enableExpanding with get,set
        member val getExpandedRowModel: CoreRowModel<'Data> option = getExpandedRowModel with get,set
        member val getIsRowExpanded: (Row<'Data> -> bool) option = getIsRowExpanded with get,set
        member val getRowCanExpand: (Row<'Data> -> bool) option = getRowCanExpand with get,set
        member val paginateExpandedRows: bool option = paginateExpandedRows with get,set
        
        // Feature
        // Pagination
        member val manualPagination: bool option = manualPagination with get,set
        member val pageCount: int option = pageCount with get,set
        member val rowCount: int option = rowCount with get,set
        member val autoResetPageIndex: bool option = autoResetPageIndex with get,set
        member val onPaginationChange: OnChangeFn<PaginationState> option = onPaginationChange with get,set
        member val getPaginationRowModel: CoreRowModel<'Data> option = getPaginationRowModel with get,set
        
        // Feature
        // RowPinning
        member val enableRowPinning: (Row<'Data> -> bool) option = enableRowPinning with get,set
        member val keepPinnedRows: bool option = keepPinnedRows with get,set
        member val onRowPinningChanged: OnChangeFn<RowPinningState> option = onRowPinningChanged with get,set
    
        // Feature
        // RowSelection
        member val enableRowSelection: (Row<'Data> -> bool) option = enableRowSelection with get,set
        member val enableMultiRowSelection: (Row<'Data> -> bool) option = enableMultiRowSelection with get,set
        member val enableSubRowSelection: (Row<'Data> -> bool) option = enableSubRowSelection with get,set
        member val onRowSelectionChange: OnChangeFn<RowSelectionState> option = onRowSelectionChange with get,set
    
        // Feature
        // ColumnFaceting
        member val getColumnFacetedRowModel: (string -> RowModel<'Data>) option = getColumnFacetedRowModel with get,set 
        member val getFacetedRowModel = getFacetedRowModel with get,set 
        member val getFacetedUniqueValues = getFacetedUniqueValues with get,set 
        
        /// <summary>
        /// This optional function is used to derive a unique ID for any given row. If not provided the rows index is used (nested rows join together with <c>.</c> using their grandparents' index eg. <c>index.index.index</c>). If you need to identify individual rows that are originating from any server-side operations, it's suggested you use this function to return an ID that makes sense regardless of network IO/ambiguity eg. a userId, taskId, database ID field, etc.
        /// </summary>
        member val getRowId: ('Data * int * Row<'Data> -> string) option = getRowId with get,set
    
    let inline private mkProperty this name getter =
        Constructors.Object.defineProperty(this, name, unbox<PropertyDescriptor> (createObj [ "get" ==> getter ]))
        |> ignore
        this
    [<AutoOpen; Erase>]
    type TableOptionsExtensions =
        /// Properly provides reactivity by making the data key a property
        [<System.Runtime.CompilerServices.Extension; Erase>]
        static member inline data (this: TableOptions<'Data>, value: unit -> 'Data[]) = mkProperty this "data" value
        /// Properly provides reactivity by making the columns key a property
        [<System.Runtime.CompilerServices.Extension; Erase>]
        static member inline columns (this: TableOptions<'Data>, value: unit -> ColumnDef<'Data>[]) = mkProperty this "columns" value
        [<Extension; Erase>]
        static member inline stateFn(this: TableOptions<'Data>, handler: (TableState -> TableState)): TableOptions<'Data> =
            this.state <- Some (TableState.init |> handler)
            this
    
    [<AllowNullLiteral; Interface>]
    type Table<'Data> =
        /// This is the resolved initial state of the table.
        abstract member initialState: TableState with get
        /// Call this function to reset the table state to the initial state.
        abstract member reset: ((unit -> unit)) with get
        /// <summary>
        /// Call this function to get the table's current state. It's recommended to use this function and its state, especially when managing the table state manually. It is the exact same state used internally by the table for every feature and function it provides.
        /// <br/> <br/>
        /// 🧠 The state returned by this function is the shallow-merged result of the automatically-managed internal table-state and any manually-managed state passed via options.state.
        /// </summary>
        abstract member getState: (unit -> TableState) with get
        /// <summary>
        /// Call this function to update the table state. It's recommended you pass an updater function in the form of (prevState) => newState to update the state, but a direct object can also be passed.
        /// <br/> <br/>
        /// 🧠 If options.onStateChange is provided, it will be triggered by this function with the new state.
        /// </summary>
        abstract member setState: Updater<TableState> -> unit with get
        /// <summary>
        /// A read-only reference to the table's current options. <br/><br/>
        /// ⚠️ This property is generally used internally or by adapters. It can be updated by passing new options to your table. This is different per adapter. For adapters themselves, table options must be updated via the setOptions function.
        /// </summary>
        abstract member options: TableOptions<'Data> with get
        /// <summary>
        /// ⚠️ This function is generally used by adapters to update the table options. It can be used to update the table options directly, but it is generally not recommended to bypass your adapters strategy for updating table options.
        /// </summary>
        abstract member setOptions: Updater<TableOptions<'Data>> -> unit with get
        /// Returns the core row model before any processing has been applied.
        abstract member getCoreRowModel: (unit -> RowModel<'Data>) with get
        /// Returns the final model after all processing from other used features has been applied.
        abstract member getRowModel: (unit -> RowModel<'Data>) with get
        /// Returns all columns in the table in their normalized and nested hierarchy, mirrored from the column defs passed to the table.
        abstract member getAllColumns: (unit -> Column<'Data>[]) with get
        /// Returns all columns in the table flattened to a single level. This includes parent column objects throughout the hierarchy.
        abstract member getAllFlatColumns: (unit -> Column<'Data>[]) with get
        /// Returns all leaf-node columns in the table flattened to a single level. This does not include parent columns.
        abstract member getAllLeafColumns: (unit -> Column<'Data>[]) with get
        /// Returns a single column by its ID.
        abstract member getColumn: (string -> Column<'Data> option) with get
        /// Returns the header groups for the table.
        abstract member getHeaderGroups: (unit -> HeaderGroup<'Data>[]) with get
        /// Returns the footer groups for the table.
        abstract member getFooterGroups: (unit -> HeaderGroup<'Data>[]) with get
        /// Returns a flattened array of Header objects for the table, including parent headers.
        abstract member getFlatHeaders: (unit -> Header<'Data>[]) with get
        /// Returns a flattened array of leaf-node Header objects for the table.
        abstract member getLeafHeaders: (unit -> Header<'Data>[]) with get
     
    [<AllowNullLiteral;Interface>]
    type Column<'Data> =
        /// <summary>
        /// The resolved unique identifier for the column resolved in this priority:
        /// <ul>
        /// <li>A manual id property from the column def</li>
        /// <li>The accessor key from the column def</li>
        /// <li>The header string from the column def</li>
        /// </ul>
        /// </summary>
        abstract member id: string with get
        /// The depth of the column (if grouped) relative to the root column def array.
        abstract member depth: int with get
        /// The resolved accessor function to use when extracting the value for the column from each row. Will only be defined if the column def has a valid accessor key or function defined.
        abstract member accessorFn: ('Data -> obj) with get
        /// The original column def used to create the column.
        abstract member columnDef: ColumnDef<'Data> with get
        /// The child column (if the column is a group column). Will be an empty array if the column is not a group column.
        abstract member columns: ColumnDef<'Data>[] with get
        /// The parent column for this column. Will be undefined if this is a root column.
        abstract member parent: Column<'Data> option with get
        /// Returns the flattened array of this column and all child/grand-child columns for this column.
        abstract member getFlatColumns: (unit -> Column<'Data>[]) with get
        /// Returns an array of all leaf-node columns for this column. If a column has no children, it is considered the only leaf-node column.
        abstract member getLeafColumns: (unit -> Column<'Data>[]) with get
        
    [<AllowNullLiteral;Interface>]
    type HeaderGroup<'Data> =
        /// The unique identifier for the header group.
        abstract member id: string with get,set
        /// The depth of the header group, zero-indexed based.
        abstract member depth: int with get,set
        /// An array of Header objects that belong to this header group
        abstract member headers: Header<'Data>[] with get,set
     
    [<AllowNullLiteral;Interface>]
    type Header<'Data, 'Value> =
        inherit Header<'Data>

    [<AllowNullLiteral;Interface>]
    type Column<'Data, 'Value> =
        inherit Column<'Data>

    [<AllowNullLiteral;Interface>]
    type HeaderContext<'Data, 'Value> =
        abstract member table: TableOptions<'Data> with get,set
        abstract member header: Header<'Data, 'Value> with get,set
        abstract member column: Column<'Data, 'Value> with get,set

    [<AllowNullLiteral;Interface>]
    type Header<'Data> =
        /// The unique identifier for the header.
        abstract member id: string with get
        /// The index for the header within the header group.
        abstract member index: int with get
        /// The depth of the header, zero-indexed based.
        abstract member depth: int with get
        /// The header's associated Column object
        abstract member column: Column<'Data> with get
        /// The header's associated HeaderGroup object
        abstract member headerGroup: HeaderGroup<'Data> with get
        /// The header's hierarchical sub/child headers. Will be empty if the header's associated column is a leaf-column.
        abstract member subHeaders: Header<'Data>[] with get
        /// The col-span for the header.
        abstract member colSpan: int with get
        /// The row-span for the header.
        abstract member rowSpan: int with get
        /// Returns the leaf headers hierarchically nested under this header.
        abstract member getLeafHeaders: (unit -> Header<'Data>[]) with get
        /// A boolean denoting if the header is a placeholder header
        abstract member isPlaceholder: bool with get
        /// If the header is a placeholder header, this will be a unique header ID that does not conflict with any other headers across the table
        abstract member placeholderId: string option with get
        /// <summary>
        /// Returns the rendering context (or props) for column-based components like headers, footers and filters. Use these props with your framework's flexRender utility to render these using the template of your choice:
        /// </summary>
        /// <example><code>
        /// flexRender(header.column.columnDef.header, header.getContext())
        /// </code></example>
        abstract member getContext: (unit -> HeaderContext<'Data, obj>) with get
    [<Erase; AutoOpen>]
    module Header =
        type Table<'Data> with
            /// Returns all header groups for the table.
            member _.getHeaderGroups with get(): (unit -> HeaderGroup<'Data>[]) = unbox null
            /// If pinning, returns the header groups for the left pinned columns.
            member _.getLeftHeaderGroups with get(): (unit -> HeaderGroup<'Data>[]) = unbox null
            /// If pinning, returns the header groups for columns that are not pinned.
            member _.getCenterHeaderGroups with get(): (unit -> HeaderGroup<'Data>[]) = unbox null
            /// If pinning, returns the header groups for the right pinned columns.
            member _.getRightHeaderGroups with get(): (unit -> HeaderGroup<'Data>[]) = unbox null
        
            /// Returns all footer groups for the table.
            member _.getFooterGroups with get(): (unit -> HeaderGroup<'Data>[]) = unbox null
            /// If pinning, returns the footer groups for the left pinned columns.
            member _.getLeftFooterGroups with get(): (unit -> HeaderGroup<'Data>[]) = unbox null
            /// If pinning, returns the footer groups for columns that are not pinned.
            member _.getCenterFooterGroups with get(): (unit -> HeaderGroup<'Data>[]) = unbox null
            /// If pinning, returns the footer groups for the right pinned columns.
            member _.getRightFooterGroups with get(): (unit -> HeaderGroup<'Data>[]) = unbox null
        
            /// Returns headers for all columns in the table, including parent headers.
            member _.getFlatHeaders with get(): (unit -> Header<'Data, obj>[]) = unbox null
            /// If pinning, returns headers for all left pinned columns in the table, including parent headers.
            member _.getLeftFlatHeaders with get(): (unit -> Header<'Data, obj>[]) = unbox null
            /// If pinning, returns headers for all columns that are not pinned, including parent headers.
            member _.getCenterFlatHeaders with get(): (unit -> Header<'Data, obj>[]) = unbox null
            /// If pinning, returns headers for all right pinned columns in the table, including parent headers.
            member _.getRightFlatHeaders with get(): (unit -> Header<'Data, obj>[]) = unbox null
        
            /// Returns headers for all leaf columns in the table, (not including parent headers).
            member _.getLeafHeaders with get(): (unit -> Header<'Data, obj>[]) = unbox null
            /// If pinning, returns headers for all left pinned leaf columns in the table, (not including parent headers).
            member _.getLeftLeafHeaders with get(): (unit -> Header<'Data, obj>[]) = unbox null
            /// If pinning, returns headers for all columns that are not pinned, (not including parent headers).
            member _.getCenterLeafHeaders with get(): (unit -> Header<'Data, obj>[]) = unbox null
            /// If pinning, returns headers for all right pinned leaf columns in the table, (not including parent headers).
            member _.getRightLeafHeaders with get(): (unit -> Header<'Data, obj>[]) = unbox null

    [<AllowNullLiteral;Interface>]
    type CellContext<'Data, 'Value> =
        inherit TableRenderProp<'Data>
        inherit RowRenderProp<'Data>
        abstract member column: Column<'Data, 'Value> with get,set
        abstract member cell: Cell<'Data, 'Value> with get,set
        abstract member getValue: (unit -> 'Value) with get,set
        abstract member renderValue: (unit -> 'Value option) with get,set
       
    [<AllowNullLiteral;Interface>]
    type Cell<'Data, 'Value> =
        inherit Cell<'Data>


    [<AllowNullLiteral;Interface>]
    type Cell<'Data> =
        /// The unique ID for the cell across the entire table.
        abstract member id: string with get
        /// Returns the value for the cell, accessed via the associated column's accessor key or accessor function.
        abstract member getValue: ((unit -> obj)) with get
        /// <summary>
        /// Renders the value for a cell the same as getValue, but will return the <c>renderFallbackValue</c> if no value is found.
        /// </summary>
        abstract member renderValue: ((unit -> obj)) with get
        /// The associated Row object for the cell.
        abstract member row: Row<'Data> with get
        /// The associated Column object for the cell.
        abstract member column: Column<'Data> with get
        /// <summary>
        /// Returns the rendering context (or props) for cell-based components like cells and aggregated cells. Use these props with your framework's <c>flexRender</c> utility to render these using the template of your choice:
        /// </summary>
        /// <example>
        /// <code>flexRender(cell.column.columnDef.cell, cell.getContext())</code>
        /// </example> 
        abstract member getContext: (unit -> CellContext<'Data, obj>) with get

    /// <summary>
    /// An Updater denotes a parameter or argument which can either be the raw value,
    /// or a function which acts on the previous value. <br/><br/>
    /// You can use the extensions Updater.fromFun or Updater.fromValue helpers.
    /// <br/> Otherwise, just use the Fable operator <c>!^</c>
    /// </summary>
    type Updater<'T> = U2<('T -> 'T), 'T>
    [<Erase; RequireQualifiedAccess>]
    module Updater =
        /// <summary>
        /// Auto lifts the handler into a <c>Updater</c> type
        /// </summary>
        /// <param name="handler">A handler which updates the previous value into a new value</param>
        let inline fromFun (handler: 'T -> 'T): Updater<'T> = !^handler
        /// <summary>
        /// Auto lifts the value into a <c>Updater</c> type
        /// </summary>
        /// <param name="value">The new value</param>
        let inline fromValue (value: 'T): Updater<'T> = !^value

    [<AllowNullLiteral;Interface>]
    type Row<'Data> =
        [<EmitIndexer>]
        abstract Item<'Value>: string -> 'Value
        /// The resolved unique identifier for the row resolved via the options.getRowId option. Defaults to the row's index (or relative index if it is a subRow)
        abstract member id: string with get
        /// The depth of the row (if nested or grouped) relative to the root row array.
        abstract member depth: int with get
        /// The index of the row within its parent array (or the root data array)
        abstract member index: int with get
        /// The original row object provided to the table.<br/><br/>
        /// 🧠 If the row is a grouped row, the original row object will be the first original in the group.
        abstract member original: 'Data with get
        /// If nested, this row's parent row id.
        abstract member parentId: string with get
        /// Returns the value from the row for a given columnId
        abstract member getValue: string -> obj with get
        /// <summary>Renders the value from the row for a given columnId, but will return the <c>renderFallbackValue</c> if no value is found.</summary>
        abstract member renderValue: string -> obj with get
        /// Returns a unique array of values from the row for a given columnId.
        abstract member getUniqueValues: string -> obj[] with get
        /// <summary>An array of subRows for the row as returned and created by the <c>options.getSubRows</c> option.</summary>
        abstract member subRows: Row<'Data>[] with get
        /// Returns the parent row for the row, if it exists.
        abstract member getParentRow: (unit -> Row<'Data> option) with get
        /// Returns the parent rows for the row, all the way up to a root row.
        abstract member getParentRows: (unit -> Row<'Data>[] option) with get
        /// Returns the leaf rows for the row, not including any parent rows.
        abstract member getLeafRows: (unit -> Row<'Data>[]) with get
        /// <summary>An array of the original subRows as returned by the <c>options.getSubRows</c> option.</summary>
        abstract member originalSubRows: Row<'Data>[] with get
        /// Returns all of the Cells for the row.
        abstract member getAllCells: (unit -> Cell<'Data>[]) with get

    /// <summary>
    /// The first parameter passed to an AggregationFn. A function which provides the leaf values
    /// of the groups rows.
    /// </summary>
    type LeafRowAccessor<'Data> = unit -> Row<'Data>[]
    /// <summary>
    /// The second parameter passed to an AggregationFn. A function which provides the immediate child values
    /// of the groups rows.
    /// </summary>
    type ChildRowAccessor<'Data> = unit -> Row<'Data>[]
    /// <summary>
    /// A grouping function is a handler that is passed a function to retrieve
    /// the leaf values of the groups rows and the immediate child values of the groups
    /// rows and should return a value (usually a primitive) to build the aggregated row model.
    /// </summary>
    /// <remarks>The TanStack defined aggregation functions are included</remarks>
    type AggregationFn<'Data> = LeafRowAccessor<'Data> -> ChildRowAccessor<'Data> -> obj

    [<Erase; RequireQualifiedAccess>]
    module AggregationFn =
        /// <summary>
        /// Predefined aggregation function for Partas.Solid.TanStack.Table
        /// </summary>
        /// <remarks>Compiles to the string "sum"</remarks>
        [<Emit("'sum'")>]
        let sum<'Data>: AggregationFn<'Data> = jsNative
        /// <summary>
        /// Predefined aggregation function for Partas.Solid.TanStack.Table
        /// </summary>
        /// <remarks>Compiles to the string "min"</remarks>
        [<Emit("'min'")>]
        let min<'Data>: AggregationFn<'Data> = jsNative
        /// <summary>
        /// Predefined aggregation function for Partas.Solid.TanStack.Table
        /// </summary>
        /// <remarks>Compiles to the string "max"</remarks>
        [<Emit("'max'")>]
        let max<'Data>: AggregationFn<'Data> = jsNative
        /// <summary>
        /// Predefined aggregation function for Partas.Solid.TanStack.Table
        /// </summary>
        /// <remarks>Compiles to the string "extent"</remarks>
        [<Emit("'extent'")>]
        let extent<'Data>: AggregationFn<'Data> = jsNative
        /// <summary>
        /// Predefined aggregation function for Partas.Solid.TanStack.Table
        /// </summary>
        /// <remarks>Compiles to the string "mean"</remarks>
        [<Emit("'mean'")>]
        let mean<'Data>: AggregationFn<'Data> = jsNative
        /// <summary>
        /// Predefined aggregation function for Partas.Solid.TanStack.Table
        /// </summary>
        /// <remarks>Compiles to the string "median"</remarks>
        [<Emit("'median'")>]
        let median<'Data>: AggregationFn<'Data> = jsNative
        /// <summary>
        /// Predefined aggregation function for Partas.Solid.TanStack.Table
        /// </summary>
        /// <remarks>Compiles to the string "unique"</remarks>
        [<Emit("'unique'")>]
        let unique<'Data>: AggregationFn<'Data> = jsNative
        /// <summary>
        /// Predefined aggregation function for Partas.Solid.TanStack.Table
        /// </summary>
        /// <remarks>Compiles to the string "uniqueCount"</remarks>
        [<Emit("'uniqueCount'")>]
        let uniqueCount<'Data>: AggregationFn<'Data> = jsNative
        /// <summary>
        /// Predefined aggregation function for Partas.Solid.TanStack.Table
        /// </summary>
        /// <remarks>Compiles to the string "count"</remarks>
        [<Emit("'count'")>]
        let count<'Data>: AggregationFn<'Data> = jsNative

    /// <summary>
    /// A Sorting function receives 2 rows, and a column ID, and are expected
    /// to compare the two rows using the column ID to return <c>-1</c>, <c>0</c> or
    /// <c>1</c> in ascending order.
    /// </summary>
    /// <remarks>Prebuilt SortingFns can be found in the SortingFn module.</remarks>
    type SortingFn<'Data> = Row<'Data> -> Row<'Data> -> string -> int

    #nowarn 3535
    [<Import("sortingFns", coreTable)>]
    type SortingFns<'Data> =
        /// <summary>
        /// Predefined sorting function for Partas.Solid.TanStack.Table
        /// </summary>
        /// <remarks>Compiles to the string "alphanumeric"</remarks>
        static member alphanumeric: SortingFn<'Data> = unbox jsNative
        /// <summary>
        /// Predefined sorting function for Partas.Solid.TanStack.Table
        /// </summary>
        /// <remarks>Compiles to the string "alphanumericCaseSensitive"</remarks>
        static member alphanumericCaseSensitive: SortingFn<'Data> = jsNative
        /// <summary>
        /// Predefined sorting function for Partas.Solid.TanStack.Table
        /// </summary>
        /// <remarks>Compiles to the string "text"</remarks>
        static member text: SortingFn<'Data> = jsNative
        /// <summary>
        /// Predefined sorting function for Partas.Solid.TanStack.Table
        /// </summary>
        /// <remarks>Compiles to the string "textCaseSensitive"</remarks>
        static member textCaseSensitive: SortingFn<'Data> = jsNative
        /// <summary>
        /// Predefined sorting function for Partas.Solid.TanStack.Table
        /// </summary>
        /// <remarks>Compiles to the string "datetime"</remarks>
        static member datetime: SortingFn<'Data> = unbox jsNative
        /// <summary>
        /// Predefined sorting function for Partas.Solid.TanStack.Table
        /// </summary>
        /// <remarks>Compiles to the string "basic"</remarks>
        static member basic: SortingFn<'Data> = jsNative
        /// <summary>
        /// Predefined sorting function for Partas.Solid.TanStack.Table
        /// </summary>
        /// <remarks>Compiles to the string "auto"</remarks>
        static member auto: SortingFn<'Data> = jsNative
        // let fromType<'Data, 'Value when 'Value: comparison>: SortingFn<'Data> =
        //     fun rowA rowB columnId ->
        //         if rowA[columnId] = rowB[columnId]
        //         then 0
        //         
        //         elif rowA[columnId] > rowB[columnId]
        //         then 1
        //         
        //         else -1
        // let inline fromValueHandler<'Data, 'Value> (handler) = ()

    /// <summary>
    /// Pass a POJO to the meta function
    /// </summary>
    type AddMetaFn = obj -> unit

    /// <summary>
    /// Best type safety is provided when you directly supply a filter function to a column definition, as the type signature
    /// will be mapped by the accessorFn. 
    /// </summary>
    /// <param name="row">The row to filter/include</param>
    /// <param name="columnId">The column Id of the row</param>
    /// <param name="filterValue">The filter value</param>
    /// <param name="addMeta">Function to include metadata such as why an action occured</param>
    /// <returns><c>true</c> to include the row and <c>false</c> to filter the row out</returns>
    type FilterFn<'Data, 'Value> = Row<'Data> -> string -> 'Value -> AddMetaFn -> bool
    /// <summary>
    /// Best type safety is provided when you directly supply a filter function to a column definition, as the type signature
    /// will be mapped by the accessorFn. 
    /// </summary>
    /// <param name="row">The row to filter/include</param>
    /// <param name="columnId">The column Id of the row</param>
    /// <param name="filterValue">The filter value</param>
    /// <param name="addMeta">Function to include metadata such as why an action occured</param>
    /// <returns><c>true</c> to include the row and <c>false</c> to filter the row out</returns>
    type FilterFn<'Data> = FilterFn<'Data, obj>

    [<RequireQualifiedAccess; Erase>]
    module FilterFn =
        /// <summary>
        /// Predefined filter function for Partas.Solid.TanStack.Table
        /// </summary>
        /// <remarks>Compiles to the string "includesString"</remarks>
        [<Emit("'includesString'")>]
        let includesString<'Data>: FilterFn<'Data> = jsNative
        /// <summary>
        /// Predefined filter function for Partas.Solid.TanStack.Table
        /// </summary>
        /// <remarks>Compiles to the string "includesStringSensitive"</remarks>
        [<Emit("'includesStringSensitive'")>]
        let includesStringSensitive<'Data>: FilterFn<'Data> = jsNative
        /// <summary>
        /// Predefined filter function for Partas.Solid.TanStack.Table
        /// </summary>
        /// <remarks>Compiles to the string "equalsString"</remarks>
        [<Emit("'equalsString'")>]
        let equalsString<'Data>: FilterFn<'Data> = jsNative
        /// <summary>
        /// Predefined filter function for Partas.Solid.TanStack.Table
        /// </summary>
        /// <remarks>Compiles to the string "equalsStringSensitive"</remarks>
        [<Emit("'equalsStringSensitive'")>]
        let equalsStringSensitive<'Data>: FilterFn<'Data> = jsNative
        /// <summary>
        /// Predefined filter function for Partas.Solid.TanStack.Table
        /// </summary>
        /// <remarks>Compiles to the string "arrIncludes"</remarks>
        [<Emit("'arrIncludes'")>]
        let arrIncludes<'Data>: FilterFn<'Data> = jsNative
        /// <summary>
        /// Predefined filter function for Partas.Solid.TanStack.Table
        /// </summary>
        /// <remarks>Compiles to the string "arrIncludesAll"</remarks>
        [<Emit("'arrIncludesAll'")>]
        let arrIncludesAll<'Data>: FilterFn<'Data> = jsNative
        /// <summary>
        /// Predefined filter function for Partas.Solid.TanStack.Table
        /// </summary>
        /// <remarks>Compiles to the string "arrIncludesSome"</remarks>
        [<Emit("'arrIncludesSome'")>]
        let arrIncludesSome<'Data>: FilterFn<'Data> = jsNative
        /// <summary>
        /// Predefined filter function for Partas.Solid.TanStack.Table
        /// </summary>
        /// <remarks>Compiles to the string "equals"</remarks>
        [<Emit("'equals'")>]
        let equals<'Data>: FilterFn<'Data> = jsNative
        /// <summary>
        /// Predefined filter function for Partas.Solid.TanStack.Table
        /// </summary>
        /// <remarks>Compiles to the string "weakEquals"</remarks>
        [<Emit("'weakEquals'")>]
        let weakEquals<'Data>: FilterFn<'Data> = jsNative
        /// <summary>
        /// Predefined filter function for Partas.Solid.TanStack.Table
        /// </summary>
        /// <remarks>Compiles to the string "inNumberRange"</remarks>
        [<Emit("'inNumberRange'")>]
        let inNumberRange<'Data>: FilterFn<'Data> = jsNative
        /// <summary>
        /// Predefined filter function for Partas.Solid.TanStack.Table
        /// </summary>
        /// <remarks>Compiles to the string "auto"</remarks>
        [<Emit("'auto'")>]
        let auto<'Data>: FilterFn<'Data> = jsNative

    
    [<AllowNullLiteral; Interface>]
    type FilterFnProps<'Data> =
        abstract member row: Row<'Data> with get
        abstract member columnId: string with get
        abstract member filterValue: obj with get
        abstract member addMeta: (obj -> unit) with get

    [<AutoOpen; Erase>]
    module ColumnGrouping =
        type BuiltInAggregationFn = AggregationFn<obj>
        
        [<Import("aggregationFns", coreTable)>]
        [<AllowNullLiteral; Interface>]
        type aggregationFns =
            abstract member sum: BuiltInAggregationFn with get
            abstract member min: BuiltInAggregationFn with get
            abstract member max: BuiltInAggregationFn with get
            abstract member extent: BuiltInAggregationFn with get
            abstract member mean: BuiltInAggregationFn with get
            abstract member median: BuiltInAggregationFn with get
            abstract member unique: BuiltInAggregationFn with get
            abstract member uniqueCount: BuiltInAggregationFn with get
            abstract member count: BuiltInAggregationFn with get
        

    [<AutoOpen; Erase>]
    module RowSorting =
        type BuiltInSortingFn = SortingFn<obj>
        
        [<Import("sortingFns", coreTable)>]
        [<AllowNullLiteral; Interface>]
        type sortingFns =
            abstract member alphanumeric: BuiltInSortingFn with get
            abstract member alphanumericCaseSensitive: BuiltInSortingFn with get
            abstract member text: BuiltInSortingFn with get
            abstract member textCaseSensitive: BuiltInSortingFn with get
            abstract member datetime: BuiltInSortingFn with get
            abstract member basic: BuiltInSortingFn with get
        