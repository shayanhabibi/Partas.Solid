namespace Partas.Solid.TanStack.Table

open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop

[<AutoOpen; Erase>]
module ColumnPinning =
    type Table<'Data> with
        member _.setColumnPinning with get(): Updater<ColumnPinningState> -> unit = unbox null
        member _.resetColumnPinning with get(): bool -> unit = unbox null
        member _.getIsSomeColumnPinned with get(): ColumnPinningState -> unit = unbox null
        member _.getLeftHeaderGroups with get(): HeaderGroup<'Data>[] = unbox null
        member _.getCenterHeaderGroups with get(): HeaderGroup<'Data>[] = unbox null
        member _.getRightHeaderGroups with get(): HeaderGroup<'Data>[] = unbox null
        member _.getLeftFooterGroups with get(): HeaderGroup<'Data>[] = unbox null
        member _.getCenterFooterGroups with get(): HeaderGroup<'Data>[] = unbox null
        member _.getRightFooterGroups with get(): HeaderGroup<'Data>[] = unbox null
        member _.getLeftFlatHeaders with get(): Header<'Data>[] = unbox null
        member _.getCenterFlatHeaders with get(): Header<'Data>[] = unbox null
        member _.getRightFlatHeaders with get(): Header<'Data>[] = unbox null
        member _.getLeftLeafHeaders with get(): Header<'Data>[] = unbox null
        member _.getCenterLeafHeaders with get(): Header<'Data>[] = unbox null
        member _.getRightLeafHeaders with get(): Header<'Data>[] = unbox null
        member _.getLeftLeafColumns with get(): Column<'Data>[] = unbox null
        member _.getCenterLeafColumns with get(): Column<'Data>[] = unbox null
        member _.getRightLeafColumns with get(): Column<'Data>[] = unbox null

    type Column<'Data> with
        member _.getCanPin with get(): (unit -> bool) = unbox null
        member _.getPinnedIndex with get(): (unit -> int) = unbox null
        member _.getIsPinned with get(): (unit -> ColumnPinningPosition) = unbox null
        member _.pin with get(): ColumnPinningPosition -> unit = unbox null

    type Row<'Data> with
        member _.getLeftVisibleCells with get(): (unit -> Cell<'Data>[]) = unbox null
        member _.getRightVisibleCells with get(): (unit -> Cell<'Data>[]) = unbox null
        member _.getCenterVisibleCells with get(): (unit -> Cell<'Data>[]) = unbox null
        
