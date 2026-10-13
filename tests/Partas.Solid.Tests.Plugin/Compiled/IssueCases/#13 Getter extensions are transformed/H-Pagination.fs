namespace Partas.Solid.TanStack.Table

open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop
open Browser.Types

[<AutoOpen; Erase>]
module ppp =
    type TableOptions<'Data> with
        member _.manualPagination
            with set (value: bool) = ()

        member _.pageCount
            with set (value: int) = ()

        member _.rowCount
            with set (value: int) = ()

        member _.autoResetPageIndex
            with set (value: bool) = ()

        member _.onPaginationChange
            with set (value: OnChangeFn<PaginationState>) = ()

        member _.getPaginationRowModel
            with set (value: Table<'Data> -> (unit -> RowModel<'Data>)) = ()

    type Table<'Data> with
        member _.setPagination: Updater<PaginationState> -> unit = unbox null
        member _.resetPagination: bool -> unit = unbox null
        member _.setPageIndex: Updater<int> -> unit = unbox null
        member _.resetPageIndex: bool -> unit = unbox null
        member _.setPageSize: Updater<int> -> unit = unbox null
        member _.resetPageSize: bool -> unit = unbox null
        member _.getPageOptions: (unit -> int[]) = unbox null
        member _.getCanPreviousPage: (unit -> bool) = unbox null
        member _.getCanNextPage: (unit -> bool) = unbox null
        member _.previousPage: (unit -> unit) = unbox null
        member _.nextPage: (unit -> unit) = unbox null
        member _.firstPage: (unit -> unit) = unbox null
        member _.lastPage: (unit -> unit) = unbox null
        member _.getPageCount: (unit -> int) = unbox null
        member _.getPrePaginationRowModel: (unit -> RowModel<'Data>) = unbox null
        member _.getPaginationRowModel: (unit -> RowModel<'Data>) = unbox null
