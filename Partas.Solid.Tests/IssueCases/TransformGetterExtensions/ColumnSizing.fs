namespace Partas.Solid.TanStack.Table

open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop
open Browser.Types

[<AutoOpen; Erase>]
module aaa =
    type ColumnDef<'Data> with
        member _.enableResizing with set(value: bool) = ()
        member _.size with set(value: int) = ()
        member _.minSize with set(value: int) = ()
        member _.maxSize with set(value: int) = ()
    
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

    [<StringEnum>]
    type ColumnResizingMode =
        | OnChange
        | OnEnd
    [<StringEnum>]
    type ColumnResizingDirection =
        | Ltr
        | Rtl
    type TableOptions<'Data> with
        member _.enableColumnResizing with set(value: bool) = ()
        member _.columnResizeMode with set(value: ColumnResizingMode) = ()
        member _.columnResizeDirection with set(value: ColumnResizingDirection) = ()
        member _.onColumnSizingChange with set(value: OnChangeFn<ColumnSizingState>) = ()
        member _.onColumnSizingInfoChange with set(value: OnChangeFn<ColumnSizingInfoState>) = ()
    
    type Table<'Data> with
        member _.setColumnSizing with get(): Updater<ColumnSizingState> -> unit = unbox null
        member _.setColumnSizingInfo with get(): bool -> unit = unbox null
        member _.resetColumnSizing with get(): bool -> unit = unbox null
        member _.resetHeaderSizeInfo with get(): bool -> unit = unbox null
        member _.getTotalSize with get(): (unit -> int) = unbox null
        member _.getLeftTotalSize with get(): (unit -> int) = unbox null
        member _.getCenterTotalSize with get(): (unit -> int) = unbox null
        member _.getRightTotalSize with get(): (unit -> int) = unbox null
        