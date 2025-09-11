namespace Partas.Solid.TanStack.Table

open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop

[<AutoOpen; Erase>]
module rec ColumnFilteringTypes =

    type Column<'Data> with
        member _.getCanFilter with get(): (unit -> bool) = unbox null
        member _.getFilterIndex with get(): (unit -> int) = unbox null
        member _.getIsFiltered with get(): (unit -> bool) = unbox null
        member _.getFilterValue with get(): (unit -> obj) = unbox null
        member _.setFilterValue with get(): (Updater<obj> -> unit) = unbox null
        member _.getAutoFilterFn with get(): (string -> FilterFn<'Data>) = unbox null
        member _.getFilterFn with get(): (string -> FilterFn<'Data>) = unbox null

    type Row<'Data> with
        member _.columnFilters with get(): JS.Object = unbox null
        member _.columnFiltersMeta with get(): JS.Object = unbox null


    type Table<'Data> with
        member _.setColumnFilters with get(): Updater<ColumnFiltersState> -> unit = unbox null
        member _.resetColumnFilters with get(): bool -> unit = unbox null
        member _.getPreFilteredRowModel with get(): (unit -> RowModel<'Data>) = unbox null
        member _.getFilteredRowModel with get(): (unit -> RowModel<'Data>) = unbox null
        