namespace Partas.Solid.TanStack.Table

open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop
open Browser.Types



[<Erase; AutoOpen>]
module GroupingExtensions =

    
    type Column<'Data> with
        member _.aggregationFn with get(): AggregationFn<'Data> = unbox null
        member _.getCanGroup with get(): (unit -> bool) = unbox null
        member _.getIsGrouped with get(): (unit -> bool) = unbox null
        member _.getGroupedIndex with get(): (unit -> int) = unbox null
        member _.toggleGrouping with get(): (unit -> unit) = unbox null
        member _.getToggleGroupingHandler with get(): (unit -> (unit -> unit)) = unbox null
        member _.getAutoAggregationFn with get(): (unit -> AggregationFn<'Data> option) = unbox null
        member _.getAggregationFn with get(): (unit -> AggregationFn<'Data> option) = unbox null

    type Row<'Data> with
        member _.groupingColumnId with get(): string = unbox null
        member _.groupingValue with get(): obj = unbox null
        member _.getIsGrouped with get(): (unit -> bool) = unbox null
        member _.getGroupingValue with get(): (string -> obj) = unbox null


    

    type Table<'Data> with
        member _.setGrouping with get(): Updater<GroupingState> -> unit = unbox null
        member _.resetGrouping with get(): bool -> unit = unbox null
        member _.getPreGroupedRowModel with get(): (unit -> RowModel<'Data>) = unbox null
        member _.getGroupedRowModel with get(): (unit -> RowModel<'Data>) = unbox null

    type Cell<'Data> with
        member _.getIsAggregated with get(): (unit -> bool) = unbox null
        member _.getIsGrouped with get(): (unit -> bool) = unbox null
        member _.getIsPlaceholder with get(): (unit -> bool) = unbox null
        
