namespace Partas.Solid.TanStack.Table

open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop
open Browser.Types

[<AutoOpen; Erase>]
module ColumnSizing =
    
    type Column<'Data> with
        member _.getSize with get(): (unit -> int) = unbox null
        member _.getStart with get(): ColumnPinningPosition option -> int = unbox null
        member _.getAfter with get(): ColumnPinningPosition option -> int = unbox null
        member _.getCanResize with get(): (unit -> bool) = unbox null
        member _.getIsResizing with get(): (unit -> bool) = unbox null
        member _.resetSize with get(): (unit -> unit) = unbox null

    type Header<'Data> with
        member _.getSize with get(): (unit -> int) = unbox null
        member _.getStart with get(): ColumnPinningPosition option -> int = unbox null
        member _.getResizeHandler with get(): (unit -> (MouseEvent -> unit)) = unbox null

    type Table<'Data> with
        member _.setColumnSizing with get(): Updater<ColumnSizingState> -> unit = unbox null
        member _.setColumnSizingInfo with get(): bool -> unit = unbox null
        member _.resetColumnSizing with get(): bool -> unit = unbox null
        member _.resetHeaderSizeInfo with get(): bool -> unit = unbox null
        member _.getTotalSize with get(): (unit -> int) = unbox null
        member _.getLeftTotalSize with get(): (unit -> int) = unbox null
        member _.getCenterTotalSize with get(): (unit -> int) = unbox null
        member _.getRightTotalSize with get(): (unit -> int) = unbox null
        