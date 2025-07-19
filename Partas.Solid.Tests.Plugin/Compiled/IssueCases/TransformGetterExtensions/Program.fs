namespace Partas.Solid.TanStack.Table

open Fable.Core.JS
open Microsoft.FSharp.Core
open Partas.Solid
open Fable.Core
open System

[<AutoOpen; Erase>]
module rec Types =
    type CoreRowModel<'Data> = ((TableOptions<'Data> -> unit) -> RowModel<'Data>)

    [<AllowNullLiteral; Interface>]
    type HeaderRenderProp<'Data> = interface end

    [<AllowNullLiteral; Interface>]
    type TableRenderProp<'Data> = interface end

    [<AllowNullLiteral; Interface>]
    type ColumnRenderProp<'Data> = interface end

    [<AllowNullLiteral; Interface>]
    type RowRenderProp<'Data> = interface end

    [<AllowNullLiteral; Interface>]
    type CellRenderProp<'Data> = interface end


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
        abstract member getValue: ((unit -> obj)) with get
        abstract member renderValue: ((unit -> obj)) with get

    [<AllowNullLiteral; JS.Pojo>]
    type ColumnDef<'Data>
        (
            ?id: string,
            ?accessorKey: string,
            ?accessorFn: ('Data * int -> obj),
            ?columns: 'Data[],
            ?header: HeaderRenderProps<'Data> -> obj,
            ?footer: FooterRenderProps<'Data> -> obj,
            ?cell: CellRenderProps<'Data> -> obj,
            ?meta: obj
        ) =
        /// <summary>
        /// The unique identifier for the column. <br/>
        /// <br/>
        /// 🧠 A column ID is optional when:<br/>
        /// <br/>
        /// An accessor column is created with an object key accessor<br/>
        /// The column header is defined as a string<br/>
        /// </summary>
        member val id: string = id.Value with get, set
        /// The key of the row object to use when extracting the value for the column.
        member val accessorKey: string = accessorKey.Value with get, set
        /// The accessor function to use when extracting the value for the column from each row.
        member val accessorFn: ('Data * int -> obj) = accessorFn.Value with get, set
        /// The child column defs to include in a group column.
        member val columns: 'Data[] = columns.Value with get, set
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
        member val header: HeaderRenderProps<'Data> -> obj = header.Value with get, set
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
        member val footer: FooterRenderProps<'Data> -> obj = footer.Value with get, set
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
        member val cell: CellRenderProps<'Data> -> obj = cell.Value with get, set
        member val meta: obj = meta.Value with get, set


    [<AllowNullLiteral; Interface>]
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

    type Updater<'T> = U2<('T -> 'T), 'T>

    [<AllowNullLiteral; Interface>]
    type TableFeature = interface end

    [<JS.Pojo>]
    type TableOptions<'Data>
        (
            ?data: 'Data[],
            ?columns: ColumnDef<'Data>[],
            ?defaultColumn: ColumnDef<'Data>,
            ?initialState: TableState,
            ?autoResetAll: bool,
            ?meta: obj,
            ?state: TableState,
            ?onStateChange: Updater<'Data> -> unit,
            ?debugAll: bool,
            ?debugTable: bool,
            ?debugHeaders: bool,
            ?debugColumns: bool,
            ?debugRows: bool,
            ?_features: TableFeature[],
            ?getCoreRowModel: ((TableOptions<'Data> -> unit) -> RowModel<'Data>),
            ?getSubRows: 'Data * int -> 'Data[],
            ?getRowId: 'Data * int * Row<'Data> -> string
        ) =
        member val data: 'Data[] option = data with get, set
        member val columns: ColumnDef<'Data>[] option = columns with get, set
        member val defaultColumn: ColumnDef<'Data> option = defaultColumn with get, set
        member val initialState: TableState option = initialState with get, set
        member val autoResetAll: bool option = autoResetAll with get, set
        member val meta: obj option = meta with get, set
        member val state: TableState option = state with get, set
        member val onStateChange: (Updater<'Data> -> unit) option = onStateChange with get, set
        member val debugAll: bool option = debugAll with get, set
        member val debugTable: bool option = debugTable with get, set
        member val debugHeaders: bool option = debugHeaders with get, set
        member val debugColumns: bool option = debugColumns with get, set
        member val debugRows: bool option = debugRows with get, set
        member val _features: TableFeature[] option = _features with get, set
        member val getCoreRowModel: ((TableOptions<'Data> -> unit) -> RowModel<'Data>) option = getCoreRowModel with get, set
        member val getSubRows: ('Data * int -> 'Data[]) option = getSubRows with get, set
        member val getRowId: ('Data * int * Row<'Data> -> string) option = getRowId with get, set

    [<AllowNullLiteral; Interface>]
    type RowModel<'Data> =
        abstract member rows: Row<'Data>[] with get, set
        abstract member flatRows: Row<'Data>[] with get, set
        abstract member rowsById: System.Collections.Generic.IDictionary<string, Row<'Data>> with get, set

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
        abstract member getColumn: string -> Column<'Data> option with get
        /// Returns the header groups for the table.
        abstract member getHeaderGroups: (unit -> HeaderGroup<'Data>[]) with get
        /// Returns the footer groups for the table.
        abstract member getFooterGroups: (unit -> HeaderGroup<'Data>[]) with get
        /// Returns a flattened array of Header objects for the table, including parent headers.
        abstract member getFlatHeaders: (unit -> Header<'Data>[]) with get
        /// Returns a flattened array of leaf-node Header objects for the table.
        abstract member getLeafHeaders: (unit -> Header<'Data>[]) with get

    [<AllowNullLiteral; Interface>]
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

    [<AllowNullLiteral; Interface>]
    type HeaderGroup<'Data> =
        /// The unique identifier for the header group.
        abstract member id: string with get, set
        /// The depth of the header group, zero-indexed based.
        abstract member depth: int with get, set
        /// An array of Header objects that belong to this header group
        abstract member headers: Header<'Data>[] with get, set

    [<AllowNullLiteral; Interface>]
    type Header<'Data, 'Value> =
        inherit Header<'Data>

    [<AllowNullLiteral; Interface>]
    type Column<'Data, 'Value> =
        inherit Column<'Data>

    [<AllowNullLiteral; Interface>]
    type HeaderContext<'Data, 'Value> =
        abstract member table: TableOptions<'Data> with get, set
        abstract member header: Header<'Data, 'Value> with get, set
        abstract member column: Column<'Data, 'Value> with get, set

    [<AllowNullLiteral; Interface>]
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
            member _.getHeaderGroups: (unit -> HeaderGroup<'Data>[]) = unbox null
            /// If pinning, returns the header groups for the left pinned columns.
            member _.getLeftHeaderGroups: (unit -> HeaderGroup<'Data>[]) = unbox null
            /// If pinning, returns the header groups for columns that are not pinned.
            member _.getCenterHeaderGroups: (unit -> HeaderGroup<'Data>[]) = unbox null
            /// If pinning, returns the header groups for the right pinned columns.
            member _.getRightHeaderGroups: (unit -> HeaderGroup<'Data>[]) = unbox null

            /// Returns all footer groups for the table.
            member _.getFooterGroups: (unit -> HeaderGroup<'Data>[]) = unbox null
            /// If pinning, returns the footer groups for the left pinned columns.
            member _.getLeftFooterGroups: (unit -> HeaderGroup<'Data>[]) = unbox null
            /// If pinning, returns the footer groups for columns that are not pinned.
            member _.getCenterFooterGroups: (unit -> HeaderGroup<'Data>[]) = unbox null
            /// If pinning, returns the footer groups for the right pinned columns.
            member _.getRightFooterGroups: (unit -> HeaderGroup<'Data>[]) = unbox null

            /// Returns headers for all columns in the table, including parent headers.
            member _.getFlatHeaders: (unit -> Header<'Data, obj>[]) = unbox null
            /// If pinning, returns headers for all left pinned columns in the table, including parent headers.
            member _.getLeftFlatHeaders: (unit -> Header<'Data, obj>[]) = unbox null
            /// If pinning, returns headers for all columns that are not pinned, including parent headers.
            member _.getCenterFlatHeaders: (unit -> Header<'Data, obj>[]) = unbox null
            /// If pinning, returns headers for all right pinned columns in the table, including parent headers.
            member _.getRightFlatHeaders: (unit -> Header<'Data, obj>[]) = unbox null

            /// Returns headers for all leaf columns in the table, (not including parent headers).
            member _.getLeafHeaders: (unit -> Header<'Data, obj>[]) = unbox null
            /// If pinning, returns headers for all left pinned leaf columns in the table, (not including parent headers).
            member _.getLeftLeafHeaders: (unit -> Header<'Data, obj>[]) = unbox null
            /// If pinning, returns headers for all columns that are not pinned, (not including parent headers).
            member _.getCenterLeafHeaders: (unit -> Header<'Data, obj>[]) = unbox null
            /// If pinning, returns headers for all right pinned leaf columns in the table, (not including parent headers).
            member _.getRightLeafHeaders: (unit -> Header<'Data, obj>[]) = unbox null

    [<AllowNullLiteral; Interface>]
    type Row<'Data> =
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

    [<AllowNullLiteral; Interface>]
    type CellContext<'Data, 'Value> =
        inherit TableRenderProp<'Data>
        inherit RowRenderProp<'Data>
        abstract member column: Column<'Data, 'Value> with get, set
        abstract member cell: Cell<'Data, 'Value> with get, set
        abstract member getValue: (unit -> 'Value) with get, set
        abstract member renderValue: (unit -> 'Value option) with get, set

    [<AllowNullLiteral; Interface>]
    type Cell<'Data, 'Value> =
        inherit Cell<'Data>


    [<AllowNullLiteral; Interface>]
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
        abstract member getContext: CellContext<'Data, obj> with get


    [<AllowNullLiteral; Interface>]
    type FilterFnProps<'Data> =
        abstract member row: Row<'Data> with get
        abstract member columnId: string with get
        abstract member filterValue: obj with get
        abstract member addMeta: (obj -> unit) with get

    [<AutoOpen; Erase>]
    module States =
        [<AllowNullLiteral; Interface>]
        type VisibilityTableState = interface end

        type VisibilityState = Map<string, bool>



        [<AllowNullLiteral; Interface>]
        type ColumnOrderTableState = interface end

        [<AllowNullLiteral; Erase>]
        type ColumnPinningTableState = interface end

        [<AllowNullLiteral; Interface>]
        type FiltersTableState = interface end

        [<StringEnum>]
        type SortDirection =
            | Asc
            | Desc

        [<Interface; AllowNullLiteral>]
        type SortingTableState = interface end

        [<Pojo>]
        type ColumnSort(id: string, desc: bool) =
            member val id = id with get, set
            member val desc = desc with get, set

        type SortingState = ColumnSort[]


        [<Interface; AllowNullLiteral>]
        type ExpandedTableState = interface end

        [<StringEnum>]
        type ExpandedState = U2<bool, Map<string, bool>>



        [<AllowNullLiteral; Interface>]
        type GroupingTableState = interface end

        type GroupingState = string[]


        [<AllowNullLiteral; Interface>]
        type PaginationTableState = interface end

        [<Pojo>]
        type PaginationState(pageIndex: int, pageSize: int) =
            member val pageIndex = pageIndex with get, set
            member val pageSize = pageSize with get, set

        [<AllowNullLiteral; Interface>]
        type PaginationInitialTableState = interface end


        [<AllowNullLiteral; Interface>]
        type RowSelectionTableState = interface end

        type RowSelectionState = Map<string, bool>



        [<AllowNullLiteral; Interface>]
        type OnChangeFn<'State> = interface end

        [<StringEnum>]
        type ColumnPinningPosition =
            | [<CompiledValue(false)>] False
            | Left
            | Right

        [<JS.Pojo>]
        type ColumnPinningState(?left: string[], ?right: string[]) =
            member val left = left with get, set
            member val right = right with get, set

        [<AllowNullLiteral; Interface>]
        type ColumnSizingTableState = interface end

        type ColumnSizingState = Map<string, int>

        [<Pojo>]
        type ColumnSizingInfoState
            (
                ?startOffset: int,
                ?startSize: int,
                ?deltaOffset: int,
                ?deltaPercentage: int,
                ?isResizingColumn: string,
                ?columnSizingStart: (string * int)[]
            ) =
            member val startOffset = startOffset with get, set
            member val startSize = startSize with get, set
            member val deltaOffset = deltaOffset with get, set
            member val deltaPercentage = deltaPercentage with get, set
            member val isResizingColumn = isResizingColumn with get, set
            member val columnSizingStart = columnSizingStart with get, set

        [<Pojo>]
        type GlobalFilterTableState(globalFilter: obj) =
            member val globalFilter = globalFilter with get, set

        [<AllowNullLiteral; Interface>]
        type GlobalFilterState = interface end

        [<Pojo>]
        type RowPinningRowState(rowPinning: RowPinningState) =
            member val rowPinning = rowPinning with get, set

        [<Pojo>]
        type RowPinningState(?top: string[], ?bottom: string[]) =
            member val top = top with get, set
            member val bottom = bottom with get, set

        [<StringEnum>]
        type RowPinningPosition =
            | [<CompiledValue(false)>] False
            | Top
            | Bottom


[<AutoOpen>]
module TanStack =
    [<Import("createSolidTable", "@tanstack/solid-table")>]
    let createTable<'Data> (options: TableOptions<'Data>) : Table<'Data> = jsNative

    [<Import("getCoreRowModel", "@tanstack/solid-table")>]
    let getCoreRowModel<'Data> () : CoreRowModel<'Data> = jsNative

    [<Import("flexRender", "@tanstack/solid-table")>]
    let flexRender<'Data> ([<ParamArray>] values) : HtmlElement = jsNative

    type HeaderRenderProp<'Data> with
        member _.header
            with set (value: Header<'Data>) = ()
            and get (): Header<'Data> = unbox null

    type TableRenderProp<'Data> with
        member _.table
            with set (value: TableOptions<'Data>) = ()
            and get (): TableOptions<'Data> = unbox null

    type ColumnRenderProp<'Data> with
        member _.column
            with set (value: Column<'Data>) = ()
            and get () = unbox null

    type RowRenderProp<'Data> with
        member _.row
            with set (value: Row<'Data>) = ()
            and get () = unbox null

    type CellRenderProp<'Data> with
        member _.cell
            with set (value: Cell<'Data>) = ()
            and get () = unbox null

    type VisibilityTableState with
        member _.columnVisibility
            with set (value: VisibilityState) = ()
            and get (): VisibilityState = unbox null

    type ColumnPinningTableState with
        member _.columnPinning
            with set (value: ColumnPinningState) = ()
            and get (): ColumnPinningState = unbox null

    type SortingTableState with
        member _.sorting
            with set (value: SortingState) = ()
            and get (): SortingState = unbox null

    type ExpandedTableState with
        member _.expanded
            with set (value: ExpandedState) = ()
            and get (): ExpandedState = unbox null

    type GroupingTableState with
        member _.grouping
            with set (value: GroupingState) = ()
            and get (): GroupingState = unbox null

    type PaginationTableState with
        member _.pagination
            with set (value: PaginationState) = ()
            and get (): PaginationState = unbox null

    type PaginationInitialTableState with
        member _.pagination
            with set (value: PaginationState option) = ()
            and get (): PaginationState option = unbox null

    type RowSelectionTableState with
        member _.rowSelection
            with set (value: RowSelectionState) = ()
            and get (): RowSelectionState = unbox null

    type ColumnSizingTableState with
        member _.columnSizing
            with set (value: ColumnSizingState) = ()
            and get (): ColumnSizingState = unbox null

        member _.columnSizingInfo
            with set (value: ColumnSizingInfoState) = ()
            and get (): ColumnSizingInfoState = unbox null
