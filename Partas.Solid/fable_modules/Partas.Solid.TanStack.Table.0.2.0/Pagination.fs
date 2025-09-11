namespace Partas.Solid.TanStack.Table

open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop
open Browser.Types

[<AutoOpen; Erase>]
module Pagination =
 
    type Table<'Data> with
        member _.setPagination with get(): Updater<PaginationState> -> unit = unbox null
        member _.resetPagination with get(): bool -> unit = unbox null
        member _.setPageIndex with get(): Updater<int> -> unit = unbox null
        member _.resetPageIndex with get(): bool -> unit = unbox null
        member _.setPageSize with get(): Updater<int> -> unit = unbox null
        member _.resetPageSize with get(): bool -> unit = unbox null
        member _.getPageOptions with get(): (unit -> int[]) = unbox null
        member _.getCanPreviousPage with get(): (unit -> bool) = unbox null
        member _.getCanNextPage with get(): (unit -> bool) = unbox null
        member _.previousPage with get(): (unit -> unit) = unbox null
        member _.nextPage with get(): (unit -> unit) = unbox null
        member _.firstPage with get(): (unit -> unit) = unbox null
        member _.lastPage with get(): (unit -> unit) = unbox null
        member _.getPageCount with get(): (unit -> int) = unbox null
        member _.getPrePaginationRowModel with get(): (unit -> RowModel<'Data>) = unbox null
        member _.getPaginationRowModel with get(): (unit -> RowModel<'Data>) = unbox null
        
