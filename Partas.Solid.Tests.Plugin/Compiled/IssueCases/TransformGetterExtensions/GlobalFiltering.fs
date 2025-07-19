namespace Partas.Solid.TanStack.Table

open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop

[<AutoOpen; Erase>]
module uuuu =
    type ColumnDef<'Data> with
        member _.enableGlobalFilter
            with set (value: bool) = ()

    type Column<'Data> with
        member _.getCanGlobalFilter: (unit -> bool) = unbox null

    type Row<'Data> with
        member _.columnFiltersMeta: Map<string, obj> = unbox null


    type TableOptions<'Data> with
        member _.filterFns
            with set (value: Map<string, FilterFn>) = ()

        member _.filterFromLeafRows
            with set (value: bool) = ()

        member _.maxLeafRowFilterDepth
            with set (value: int) = ()

        member _.enableFilters
            with set (value: bool) = ()

        member _.manualFiltering
            with set (value: bool) = ()

        member _.getFilteredRowModel
            with set (value: Table<'Data> -> (unit -> RowModel<'Data>)) = ()

        member _.globalFilterFn
            with set (value: FilterFn) = ()

        member _.onGlobalFilterChange
            with set (value: OnChangeFn<GlobalFilterState>) = ()

        member _.enableGlobalFilter
            with set (value: bool) = ()

        member _.getColumnCanGlobalFilter
            with set (value: Column<'Data> -> bool) = ()

    type Table<'Data> with
        member _.getPreFilteredRowModel: (unit -> RowModel<'Data>) = unbox null
        member _.getFilteredRowModel: (unit -> RowModel<'Data>) = unbox null
        member _.setGlobalFilter: (Updater<obj> -> unit) = unbox null
        member _.resetGlobalFilter: bool -> unit = unbox null
        member _.getGlobalAutoFilterFn: string -> FilterFn option = unbox null
        member _.getGlobalFilterFn: string -> FilterFn option = unbox null
