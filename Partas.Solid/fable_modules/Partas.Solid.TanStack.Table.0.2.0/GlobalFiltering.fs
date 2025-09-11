namespace Partas.Solid.TanStack.Table

open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop

[<AutoOpen; Erase>]
module GlobalFiltering =
    
    type Column<'Data> with
        member _.getCanGlobalFilter with get(): (unit -> bool) = unbox null

    type Row<'Data> with
        member _.columnFiltersMeta with get(): Map<string, obj> = unbox null

    type Table<'Data> with
        member _.getPreFilteredRowModel with get(): (unit -> RowModel<'Data>) = unbox null
        member _.getFilteredRowModel with get(): (unit -> RowModel<'Data>) = unbox null
        member _.setGlobalFilter with get(): (Updater<obj> -> unit) = unbox null
        member _.resetGlobalFilter with get(): bool -> unit = unbox null
        member _.getGlobalAutoFilterFn with get(): string -> FilterFn<'Data> option = unbox null
        member _.getGlobalFilterFn with get(): string -> FilterFn<'Data> option = unbox null
        