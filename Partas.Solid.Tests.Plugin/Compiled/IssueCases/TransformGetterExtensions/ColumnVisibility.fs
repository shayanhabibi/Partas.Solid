namespace Partas.Solid.TanStack.Table

open System.Runtime.CompilerServices
open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop
open Browser.Types

[<AutoOpen; Erase>]
module aa =
    type ColumnDef<'Data> with
        member _.enableHiding
            with set (value: bool) = ()

    type Column<'Data> with
        member _.getCanHide: (unit -> bool) = unbox null
        member _.getIsVisible: (unit -> bool) = unbox null
        member _.toggleVisibility: (bool -> unit) = unbox null
        member _.getToggleVisibilityHandler: (unit -> (Event -> unit)) = unbox null


    type TableOptions<'Data> with
        member _.onColumnVisibilityChange
            with set (value: OnChangeFn<VisibilityState>) = ()

        member _.enableHiding
            with set (value: bool) = ()

    type Table<'Data> with
        member _.getVisibleFlatColumns: (unit -> Column<'Data>[]) = unbox null
        member _.getVisibleLeafColumns: (unit -> Column<'Data>[]) = unbox null
        member _.getLeftVisibleLeafColumns: (unit -> Column<'Data>[]) = unbox null
        member _.getRightVisibleLeafColumns: (unit -> Column<'Data>[]) = unbox null
        member _.getCenterVisibleLeafColumns: (unit -> Column<'Data>[]) = unbox null
        member _.setColumnVisibility: (Updater<VisibilityState> -> unit) = unbox null
        member _.resetColumnVisibility: bool -> unit = unbox null
        member _.toggleAllColumnsVisible: bool -> unit = unbox null
        member _.getIsAllColumnsVisible: (unit -> bool) = unbox null
        member _.getIsSomeColumnsVisible: (unit -> bool) = unbox null
        member _.getToggleAllColumnsVisibilityHandler: (unit -> (Event -> unit)) = unbox null

    type Row<'Data> with
        member _.getVisibleCells: (unit -> Cell<'Data>[]) = unbox null
