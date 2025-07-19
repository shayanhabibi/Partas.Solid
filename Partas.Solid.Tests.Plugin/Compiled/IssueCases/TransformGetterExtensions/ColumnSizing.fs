namespace Partas.Solid.TanStack.Table

open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop
open Browser.Types

[<AutoOpen; Erase>]
module aaa =
    type ColumnDef<'Data> with
        member _.enableResizing
            with set (value: bool) = ()

        member _.size
            with set (value: int) = ()

        member _.minSize
            with set (value: int) = ()

        member _.maxSize
            with set (value: int) = ()

    type Column<'Data> with
        member _.getSize: (unit -> int) = unbox null
        member _.getStart: ColumnPinningPosition option -> int = unbox null
        member _.getAfter: ColumnPinningPosition option -> int = unbox null
        member _.getCanResize: (unit -> bool) = unbox null
        member _.getIsResizing: (unit -> bool) = unbox null
        member _.resetSize: (unit -> unit) = unbox null

    type Header<'Data> with
        member _.getSize: (unit -> int) = unbox null
        member _.getStart: ColumnPinningPosition option -> int = unbox null
        member _.getResizeHandler: (unit -> (MouseEvent -> unit)) = unbox null

    [<StringEnum>]
    type ColumnResizingMode =
        | OnChange
        | OnEnd

    [<StringEnum>]
    type ColumnResizingDirection =
        | Ltr
        | Rtl

    type TableOptions<'Data> with
        member _.enableColumnResizing
            with set (value: bool) = ()

        member _.columnResizeMode
            with set (value: ColumnResizingMode) = ()

        member _.columnResizeDirection
            with set (value: ColumnResizingDirection) = ()

        member _.onColumnSizingChange
            with set (value: OnChangeFn<ColumnSizingState>) = ()

        member _.onColumnSizingInfoChange
            with set (value: OnChangeFn<ColumnSizingInfoState>) = ()

    type Table<'Data> with
        member _.setColumnSizing: Updater<ColumnSizingState> -> unit = unbox null
        member _.setColumnSizingInfo: bool -> unit = unbox null
        member _.resetColumnSizing: bool -> unit = unbox null
        member _.resetHeaderSizeInfo: bool -> unit = unbox null
        member _.getTotalSize: (unit -> int) = unbox null
        member _.getLeftTotalSize: (unit -> int) = unbox null
        member _.getCenterTotalSize: (unit -> int) = unbox null
        member _.getRightTotalSize: (unit -> int) = unbox null
