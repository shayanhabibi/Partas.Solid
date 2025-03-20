namespace Partas.Solid.TanStack.Table
open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop
open Browser.Types

[<AutoOpen; Erase>]
module qwe =
    type TableOptions<'Data> with
        member _.enableRowSelection with set(value: Row<'Data> -> bool) = ()
        member _.enableMultiRowSelection with set(value: Row<'Data> -> bool) = ()
        member _.enableSubRowSelection with set(value: Row<'Data> -> bool) = ()
        member _.onRowSelectionChange with set(value: OnChangeFn<RowSelectionState>) = ()
    
    type Table<'Data> with
        member _.getToggleAllRowsSelectedHandler with get(): (unit -> (Event -> unit)) = unbox null
        member _.getToggleAllPageRowsSelectedHandler with get(): (unit -> (Event -> unit)) = unbox null
        member _.setRowSelection with get(): (Updater<RowSelectionState> -> unit ) = unbox null
        member _.resetRowSelection with get(): bool option -> unit = unbox null
        member _.getIsAllRowsSelected with get(): (unit -> bool) = unbox null
        member _.getIsAllPageRowsSelected with get(): (unit -> bool) = unbox null
        member _.getIsSomeRowsSelected with get(): (unit -> bool) = unbox null
        member _.getIsSomePageRowsSelected with get(): (unit -> bool) = unbox null
        member _.toggleAllRowsSelected with get(): (bool -> unit) = unbox null
        member _.toggleAllPageRowsSelected with get(): bool -> unit = unbox null
        member _.getPreSelectedRowModel with get(): (unit -> RowModel<'Data>) = unbox null
        member _.getSelectedRowModel with get(): (unit -> RowModel<'Data>) = unbox null
        member _.getFilteredSelectedRowModel with get(): (unit -> RowModel<'Data>) = unbox null
        member _.getGroupedSelectedRowModel with get(): (unit -> RowModel<'Data>) = unbox null

    type Row<'Data> with
        member _.getIsSelected with get(): (unit -> bool) = unbox null
        member _.getIsSomeSelected with get(): (unit -> bool) = unbox null
        member _.getIsAllSubRowsSelected with get(): (unit -> bool) = unbox null
        member _.getCanSelect with get(): (unit -> bool) = unbox null
        member _.getCanMultiSelect with get(): (unit -> bool) = unbox null
        member _.getCanSelectSubRows with get(): (unit -> bool) = unbox null
        member _.toggleSelect with get(): (bool -> unit) = unbox null
        member _.getToggleSelectedHandler with get(): (unit -> (Event -> unit)) = unbox null
        

