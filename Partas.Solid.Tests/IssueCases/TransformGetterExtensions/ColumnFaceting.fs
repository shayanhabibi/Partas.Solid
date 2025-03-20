namespace Partas.Solid.TanStack.Table

open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop

[<Erase; AutoOpen>]
module mt =
    type Column<'Data> with
        member _.getFacetedRowModel with get(): (unit -> RowModel<'Data>) = unbox null
        member _.getFacetedUniqueValues with get(): (unit -> Map<obj, int>) = unbox null
        member _.getFacetedMinMaxValues with get(): (unit -> Map<obj, int>) = unbox null

    type Table<'Data> with
        member _.getColumnFacetedRowModel with set(columnId: string -> RowModel<'Data>) = ()
