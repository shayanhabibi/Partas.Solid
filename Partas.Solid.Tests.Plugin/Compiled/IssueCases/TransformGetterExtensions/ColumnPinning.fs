namespace Partas.Solid.TanStack.Table

open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop

[<AutoOpen; Erase>]
module aaaaaa =
    type TableOptions<'Data> with
        member _.enableColumnPinning
            with set (value: bool) = ()

        member _.onColumnPinningChange
            with set (value: OnChangeFn<ColumnPinningState>) = ()

    type Table<'Data> with
        member _.setColumnPinning: Updater<ColumnPinningState> -> unit = unbox null
        member _.resetColumnPinning: bool -> unit = unbox null
        member _.getIsSomeColumnPinned: ColumnPinningState -> unit = unbox null
        member _.getLeftHeaderGroups: HeaderGroup<'Data>[] = unbox null
        member _.getCenterHeaderGroups: HeaderGroup<'Data>[] = unbox null
        member _.getRightHeaderGroups: HeaderGroup<'Data>[] = unbox null
        member _.getLeftFooterGroups: HeaderGroup<'Data>[] = unbox null
        member _.getCenterFooterGroups: HeaderGroup<'Data>[] = unbox null
        member _.getRightFooterGroups: HeaderGroup<'Data>[] = unbox null
        member _.getLeftFlatHeaders: Header<'Data>[] = unbox null
        member _.getCenterFlatHeaders: Header<'Data>[] = unbox null
        member _.getRightFlatHeaders: Header<'Data>[] = unbox null
        member _.getLeftLeafHeaders: Header<'Data>[] = unbox null
        member _.getCenterLeafHeaders: Header<'Data>[] = unbox null
        member _.getRightLeafHeaders: Header<'Data>[] = unbox null
        member _.getLeftLeafColumns: Column<'Data>[] = unbox null
        member _.getCenterLeafColumns: Column<'Data>[] = unbox null
        member _.getRightLeafColumns: Column<'Data>[] = unbox null

    type ColumnDef<'Data> with
        member _.onColumnPinningChange
            with set (value: OnChangeFn<ColumnPinningState>) = ()

    type Column<'Data> with
        member _.getCanPin: (unit -> bool) = unbox null
        member _.getPinnedIndex: (unit -> int) = unbox null
        member _.getIsPinned: (unit -> ColumnPinningPosition) = unbox null
        member _.pin: ColumnPinningPosition -> unit = unbox null

    type Row<'Data> with
        member _.getLeftVisibleCells: (unit -> Cell<'Data>[]) = unbox null
        member _.getRightVisibleCells: (unit -> Cell<'Data>[]) = unbox null
        member _.getCenterVisibleCells: (unit -> Cell<'Data>[]) = unbox null
