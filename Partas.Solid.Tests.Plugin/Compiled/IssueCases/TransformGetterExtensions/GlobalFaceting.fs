namespace Partas.Solid.TanStack.Table

open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop

[<AutoOpen; Erase>]
module uu =
    type Table<'Data> with
        member _.getGlobalFacetedRowModel: (unit -> RowModel<'Data>) = unbox null
        member _.getGlobalFacetedUniqueValues: (unit -> Map<obj, int>) = unbox null
        member _.getGlobalFacetedMinMaxValues: (unit -> int * int) = unbox null
