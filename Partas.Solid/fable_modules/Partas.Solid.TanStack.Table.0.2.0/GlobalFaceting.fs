namespace Partas.Solid.TanStack.Table

open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop

[<AutoOpen; Erase>]
module GlobalFaceting =
    type Table<'Data> with
        member _.getGlobalFacetedRowModel with get(): (unit -> RowModel<'Data>) = unbox null
        member _.getGlobalFacetedUniqueValues with get(): (unit -> Map<obj, int>) = unbox null
        member _.getGlobalFacetedMinMaxValues with get(): (unit -> int * int) = unbox null
