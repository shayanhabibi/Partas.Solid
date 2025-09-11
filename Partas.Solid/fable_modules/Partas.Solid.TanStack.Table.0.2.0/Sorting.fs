namespace Partas.Solid.TanStack.Table

open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop



[<AutoOpen; Erase>]
module Sorting =
    type Column<'Data> with
        member _.getAutoSortingFn with get(): (unit -> SortingFn<'Data>) = unbox null
        member _.getAutoSortDir with get(): (unit -> SortDirection) = unbox null
        member _.getSortingFn with get(): (unit -> SortingFn<'Data>) = unbox null
        member _.getNextSortingOrder with get(): (unit -> U2<bool, SortDirection>) = unbox null
        member _.getCanSort with get(): (unit -> bool) = unbox null
        member _.getCanMultiSort with get(): (unit -> bool) = unbox null
        member _.getSortIndex with get(): (unit -> int) = unbox null
        member _.getIsSorted with get(): (unit -> U2<bool, SortDirection>) = unbox null
        member _.getFirstSortDir with get(): (unit -> SortDirection) = unbox null
        member _.clearSorting with get(): (unit -> unit) = unbox null
        member _.toggleSorting with get(): ((bool option * bool option) -> unit) = unbox null
        member _.getToggleSortingHandler with get(): (Browser.Types.Event -> unit) = unbox null

        
    type Table<'Data> with
        member _.setSorting with get(): Browser.Types.Event -> unit = unbox null
        member _.resetSorting with get(): bool -> unit = unbox null
        member _.getPreSortedRowModel with get(): (unit -> RowModel<'Data>) = unbox null
        member _.getSortedRowModel with get(): (unit -> RowModel<'Data>) = unbox null
        