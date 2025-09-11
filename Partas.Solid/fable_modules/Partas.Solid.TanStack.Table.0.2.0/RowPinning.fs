namespace Partas.Solid.TanStack.Table

open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop
open Browser.Types

[<AutoOpen; Erase>]
module RowPinning =
    type Table<'Data> with
        member _.setRowPinning with get(): Updater<RowPinningState> -> unit = unbox null
        member _.resetRowPinning with get(): bool option -> unit = unbox null
        member _.getIsSomeRowsPinned with get(): RowPinningPosition option -> bool = unbox null
        member _.getTopRows with get(): (unit -> Row<'Data>[]) = unbox null
        member _.getBottomRows with get(): (unit -> Row<'Data>[]) = unbox null
        member _.getCenterRows with get(): (unit -> Row<'Data>[]) = unbox null

    type Row<'Data> with
        member _.pin with get(): RowPinningPosition -> unit = unbox null
        member _.getCanPin with get(): (unit -> bool) = unbox null
        member _.getIsPinned with get(): (unit -> RowPinningPosition) = unbox null
        member _.getPinnedIndex with get(): (unit -> int) = unbox null