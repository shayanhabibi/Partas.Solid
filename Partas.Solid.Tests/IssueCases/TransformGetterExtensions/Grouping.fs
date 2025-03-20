namespace Partas.Solid.TanStack.Table

open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop
open Browser.Types

[<Pojo>]
type Renderable<'Data>(
        table: Table<'Data>,
        row: Row<'Data>,
        column: Column<'Data>,
        cell: Cell<'Data>,
        getValue: (unit -> obj),
        renderValue: (unit -> obj)
    ) =
    member val table = table with get,set
    member val row = row with get,set
    member val column = column with get,set
    member val cell = cell with get,set
    member val getValue = getValue with get,set
    member val renderValue = renderValue with get,set

[<AutoOpen; Erase>]
module ouo =
    type ColumnDef<'Data> with
        member _.aggregationFn with set(value: AggregationFn) = ()
        member _.aggregatedCell with set(value: Renderable<'Data>) = ()
        member _.enableGrouping with set(value: bool) = ()
        member _.getGroupingValue with set(value: 'Data -> obj) = ()
    
    type Column<'Data> with
        member _.aggregationFn with get(): AggregationFn = unbox null
        member _.getCanGroup with get(): (unit -> bool) = unbox null
        member _.getIsGrouped with get(): (unit -> bool) = unbox null
        member _.getGroupedIndex with get(): (unit -> int) = unbox null
        member _.toggleGrouping with get(): (unit -> unit) = unbox null
        member _.getToggleGroupingHandler with get(): (unit -> (unit -> unit)) = unbox null
        member _.getAutoAggregationFn with get(): (unit -> AggregationFn option) = unbox null
        member _.getAggregationFn with get(): (unit -> AggregationFn option) = unbox null

    type Row<'Data> with
        member _.groupingColumnId with get(): string = unbox null
        member _.groupingValue with get(): obj = unbox null
        member _.getIsGrouped with get(): (unit -> bool) = unbox null
        member _.getGroupingValue with get(): (string -> obj) = unbox null

    [<StringEnum>]
    type GroupedColumnMode =
        | [<CompiledValue(false)>] False
        | Reorder
        | Remove
    
    type TableOptions<'Data> with
        member _.aggregationFns with set(value: Map<string, AggregationFn>) = ()
        member _.manualGrouping with set(value: bool) = ()
        member _.onGroupingChange with set(value: OnChangeFn<GroupingState>) = ()
        member _.enableGrouping with set(value: bool) = ()
        member _.getGroupedRowModel with set(value: Table<'Data> -> (unit -> RowModel<'Data>)) = ()
        member _.groupedColumnMode with set(value: GroupedColumnMode) = ()
    
    type Table<'Data> with
        member _.setGrouping with get(): Updater<GroupingState> -> unit = unbox null
        member _.resetGrouping with get(): bool -> unit = unbox null
        member _.getPreGroupedRowModel with get(): (unit -> RowModel<'Data>) = unbox null
        member _.getGroupedRowModel with get(): (unit -> RowModel<'Data>) = unbox null

    type Cell<'Data> with
        member _.getIsAggregated with get(): (unit -> bool) = unbox null
        member _.getIsGrouped with get(): (unit -> bool) = unbox null
        member _.getIsPlaceholder with get(): (unit -> bool) = unbox null
        
