namespace Partas.Solid.TanStack.Table

open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop
open Browser.Types

[<AutoOpen; Erase>]
module oooo =
    type Row<'Data> with
        member _.toggleExpanded: (bool -> unit) = unbox null
        member _.getIsExpanded: (unit -> bool) = unbox null
        member _.getIsAllParentsExpanded: (unit -> bool) = unbox null
        member _.getCanExpand: (unit -> bool) = unbox null
        member _.getToggleExpandedHandler: (unit -> (unit -> unit)) = unbox null

    type TableOptions<'Data> with
        member _.manualExpanding
            with set (value: bool) = ()

        member _.onExpandedChange
            with set (value: OnChangeFn<ExpandedState>) = ()

        member _.autoResetExpanded
            with set (value: bool) = ()

        member _.enableExpanding
            with set (value: bool) = ()

        member _.getExpandedRowModel
            with set (value: Table<'Data> -> (unit -> RowModel<'Data>)) = ()

        member _.getIsRowExpanded
            with set (value: Row<'Data> -> bool) = ()

        member _.getRowCanExpand
            with set (value: Row<'Data> -> bool) = ()

        member _.paginateExpandedRows
            with set (value: bool) = ()

    type Table<'Data> with
        member _.setExpanded: Updater<ExpandedState> -> unit = unbox null
        member _.toggleAllRowsExpanded: bool -> unit = unbox null
        member _.resetExpanded: bool -> unit = unbox null
        member _.getCanSomeRowsExpand: (unit -> bool) = unbox null
        member _.getToggleAllRowsExpandedHandler: (unit -> (Event -> unit)) = unbox null
        member _.getIsSomeRowsExpanded: (unit -> bool) = unbox null
        member _.getIsAllRowsExpanded: (unit -> bool) = unbox null
        member _.getExpandedDepth: (unit -> int) = unbox null
        member _.getExpandedRowModel: (unit -> RowModel<'Data>) = unbox null
        member _.getPreExpandedRowModel: (unit -> RowModel<'Data>) = unbox null
