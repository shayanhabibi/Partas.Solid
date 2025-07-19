namespace Partas.Solid.TanStack.Table

open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop

[<StringEnum>]
type SortUndefined =
    | First
    | Last
    | [<CompiledValue(false)>] False
    | [<CompiledValue(-1)>] Negative
    | [<CompiledValue(1)>] Positive

[<AutoOpen; Erase>]
module uwu =

    type ColumnDef<'Data> with
        member _.sortingFn
            with set (value: SortingFn) = ()

        member _.sortDescFirst
            with set (value: bool) = ()

        member _.enableSorting
            with set (value: bool) = ()

        member _.enableMultiSort
            with set (value: bool) = ()

        member _.invertSorting
            with set (value: bool) = ()

        member _.sortUndefined
            with set (value: SortUndefined) = ()

    type Column<'Data> with
        member _.getAutoSortingFn: (unit -> SortingFn) = unbox null
        member _.getAutoSortDir: (unit -> SortDirection) = unbox null
        member _.getSortingFn: (unit -> SortingFn) = unbox null
        member _.getNextSortingOrder: (unit -> U2<bool, SortDirection>) = unbox null
        member _.getCanSort: (unit -> bool) = unbox null
        member _.getCanMultiSort: (unit -> bool) = unbox null
        member _.getSortIndex: (unit -> int) = unbox null
        member _.getIsSorted: (unit -> U2<bool, SortDirection>) = unbox null
        member _.getFirstSortDir: (unit -> SortDirection) = unbox null
        member _.clearSorting: (unit -> unit) = unbox null
        member _.toggleSorting: ((bool option * bool option) -> unit) = unbox null
        member _.getToggleSortingHandler: (Browser.Types.Event -> unit) = unbox null

    type TableOptions<'Data> with
        member _.sortingFns
            with set (value: Map<string, SortingFn>) = ()

        member _.manualSorting
            with set (value: bool) = ()

        member _.onSortingChange
            with set (value: OnChangeFn<SortingState>) = ()

        member _.enableSorting
            with set (value: bool) = ()

        member _.enableSortingRemoval
            with set (value: bool) = ()

        member _.enableMultiRemove
            with set (value: bool) = ()

        member _.enableMultiSort
            with set (value: bool) = ()

        member _.sortDescFirst
            with set (value: bool) = ()

        member _.getSortedRowModel
            with set (value: Table<'Data> -> (unit -> RowModel<'Data>)) = ()

        member _.maxMultiSortColCount
            with set (value: int) = ()

        member _.isMultiSortEvent
            with set (value: Browser.Types.Event -> bool) = ()

    type Table<'Data> with
        member _.setSorting: Browser.Types.Event -> unit = unbox null
        member _.resetSorting: bool -> unit = unbox null
        member _.getPreSortedRowModel: (unit -> RowModel<'Data>) = unbox null
        member _.getSortedRowModel: (unit -> RowModel<'Data>) = unbox null
