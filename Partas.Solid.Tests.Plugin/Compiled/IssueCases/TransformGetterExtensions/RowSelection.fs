namespace Partas.Solid.TanStack.Table

open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop
open Browser.Types

[<AutoOpen; Erase>]
module qwe =
    type TableOptions<'Data> with
        member _.enableRowSelection
            with set (value: Row<'Data> -> bool) = ()

        member _.enableMultiRowSelection
            with set (value: Row<'Data> -> bool) = ()

        member _.enableSubRowSelection
            with set (value: Row<'Data> -> bool) = ()

        member _.onRowSelectionChange
            with set (value: OnChangeFn<RowSelectionState>) = ()

    type Table<'Data> with
        member _.getToggleAllRowsSelectedHandler: (unit -> (Event -> unit)) = unbox null
        member _.getToggleAllPageRowsSelectedHandler: (unit -> (Event -> unit)) = unbox null
        member _.setRowSelection: (Updater<RowSelectionState> -> unit) = unbox null
        member _.resetRowSelection: bool option -> unit = unbox null
        member _.getIsAllRowsSelected: (unit -> bool) = unbox null
        member _.getIsAllPageRowsSelected: (unit -> bool) = unbox null
        member _.getIsSomeRowsSelected: (unit -> bool) = unbox null
        member _.getIsSomePageRowsSelected: (unit -> bool) = unbox null
        member _.toggleAllRowsSelected: (bool -> unit) = unbox null
        member _.toggleAllPageRowsSelected: bool -> unit = unbox null
        member _.getPreSelectedRowModel: (unit -> RowModel<'Data>) = unbox null
        member _.getSelectedRowModel: (unit -> RowModel<'Data>) = unbox null
        member _.getFilteredSelectedRowModel: (unit -> RowModel<'Data>) = unbox null
        member _.getGroupedSelectedRowModel: (unit -> RowModel<'Data>) = unbox null

    type Row<'Data> with
        member _.getIsSelected: (unit -> bool) = unbox null
        member _.getIsSomeSelected: (unit -> bool) = unbox null
        member _.getIsAllSubRowsSelected: (unit -> bool) = unbox null
        member _.getCanSelect: (unit -> bool) = unbox null
        member _.getCanMultiSelect: (unit -> bool) = unbox null
        member _.getCanSelectSubRows: (unit -> bool) = unbox null
        member _.toggleSelect: (bool -> unit) = unbox null
        member _.getToggleSelectedHandler: (unit -> (Event -> unit)) = unbox null
