namespace Partas.Solid.TanStack.Table

open System.Runtime.CompilerServices
open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop
open Browser.Types

[<AutoOpen;Erase>]
module aa =
    type ColumnDef<'Data> with
        member _.enableHiding with set(value: bool) = ()
    
    type Column<'Data> with
        member _.getCanHide with get(): (unit -> bool) = unbox null
        member _.getIsVisible with get(): (unit -> bool) = unbox null
        member _.toggleVisibility with get(): (bool -> unit) = unbox null
        member _.getToggleVisibilityHandler with get(): (unit -> (Event -> unit)) = unbox null


    type TableOptions<'Data> with
        member _.onColumnVisibilityChange with set(value: OnChangeFn<VisibilityState>) = ()
        member _.enableHiding with set(value: bool) = ()
    
    type Table<'Data> with
        member _.getVisibleFlatColumns with get(): (unit -> Column<'Data>[]) = unbox null
        member _.getVisibleLeafColumns with get(): (unit -> Column<'Data>[]) = unbox null
        member _.getLeftVisibleLeafColumns with get(): (unit -> Column<'Data>[]) = unbox null
        member _.getRightVisibleLeafColumns with get(): (unit -> Column<'Data>[]) = unbox null
        member _.getCenterVisibleLeafColumns with get(): (unit -> Column<'Data>[]) = unbox null
        member _.setColumnVisibility with get(): (Updater<VisibilityState> -> unit) = unbox null
        member _.resetColumnVisibility with get(): bool -> unit = unbox null
        member _.toggleAllColumnsVisible with get(): bool -> unit = unbox null
        member _.getIsAllColumnsVisible with get(): (unit -> bool) = unbox null
        member _.getIsSomeColumnsVisible with get(): (unit -> bool) = unbox null
        member _.getToggleAllColumnsVisibilityHandler with get(): (unit -> (Event -> unit)) = unbox null

    type Row<'Data> with
        member _.getVisibleCells with get(): (unit -> Cell<'Data>[]) = unbox null
