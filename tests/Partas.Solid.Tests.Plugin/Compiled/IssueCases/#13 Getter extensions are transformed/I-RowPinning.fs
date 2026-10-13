namespace Partas.Solid.TanStack.Table

open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop
open Browser.Types

[<AutoOpen; Erase>]
module kkk =
    type TableOptions<'Data> with
        member _.enableRowPinning
            with set (value: Row<'Data> -> bool) = ()

        member _.keepPinnedRows
            with set (value: bool) = ()

        member _.onRowPinningChanged
            with set (value: OnChangeFn<RowPinningState>) = ()

    type Table<'Data> with
        member _.setRowPinning: Updater<RowPinningState> -> unit = unbox null
        member _.resetRowPinning: bool option -> unit = unbox null
        member _.getIsSomeRowsPinned: RowPinningPosition option -> bool = unbox null
        member _.getTopRows: (unit -> Row<'Data>[]) = unbox null
        member _.getBottomRows: (unit -> Row<'Data>[]) = unbox null
        member _.getCenterRows: (unit -> Row<'Data>[]) = unbox null

    type Row<'Data> with
        member _.pin: RowPinningPosition -> unit = unbox null
        member _.getCanPin: (unit -> bool) = unbox null
        member _.getIsPinned: (unit -> RowPinningPosition) = unbox null
        member _.getPinnedIndex: (unit -> int) = unbox null
