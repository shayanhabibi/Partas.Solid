namespace Partas.Solid.TanStack.Table

open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop
open Browser.Types

[<AutoOpen; Erase>]
module oooo =
    type Row<'Data> with
        member _.toggleExpanded with get(): (bool -> unit) = unbox null
        member _.getIsExpanded with get(): (unit -> bool) = unbox null
        member _.getIsAllParentsExpanded with get(): (unit -> bool) = unbox null
        member _.getCanExpand with get(): (unit -> bool) = unbox null
        member _.getToggleExpandedHandler with get(): (unit -> (unit -> unit)) = unbox null

    type TableOptions<'Data> with
        member _.manualExpanding with set(value: bool) = ()
        member _.onExpandedChange with set(value: OnChangeFn<ExpandedState>) = ()
        member _.autoResetExpanded with set(value: bool) = ()
        member _.enableExpanding with set(value: bool) = ()
        member _.getExpandedRowModel with set(value: Table<'Data> -> (unit -> RowModel<'Data>)) = ()
        member _.getIsRowExpanded with set(value: Row<'Data> -> bool) = ()
        member _.getRowCanExpand with set(value: Row<'Data> -> bool) = ()
        member _.paginateExpandedRows with set(value: bool) = ()
    
    type Table<'Data> with
        member _.setExpanded with get(): Updater<ExpandedState> -> unit = unbox null
        member _.toggleAllRowsExpanded with get(): bool -> unit = unbox null
        member _.resetExpanded with get(): bool -> unit = unbox null
        member _.getCanSomeRowsExpand with get(): (unit -> bool) = unbox null
        member _.getToggleAllRowsExpandedHandler with get(): (unit -> (Event -> unit)) = unbox null
        member _.getIsSomeRowsExpanded with get(): (unit -> bool) = unbox null
        member _.getIsAllRowsExpanded with get(): (unit -> bool) = unbox null
        member _.getExpandedDepth with get(): (unit -> int) = unbox null
        member _.getExpandedRowModel with get(): (unit -> RowModel<'Data>) = unbox null
        member _.getPreExpandedRowModel with get(): (unit -> RowModel<'Data>) = unbox null
