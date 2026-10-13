namespace Partas.Solid.TanStack.Table

open Fable.Core

[<AutoOpen; Erase>]
module internal Paths =
    [<Literal>]
    let solidTable = "@tanstack/solid-table"

    [<Literal>]
    let tableCore = "@tanstack/table-core"

[<AutoOpen; Erase>]
module ColumnGrouping =
    type AggregationFn = interface end
    type BuiltInAggregationFn = AggregationFn

    [<Import("aggregationFns", tableCore)>]
    [<AllowNullLiteral; Interface>]
    type aggregationFns =
        abstract member sum: BuiltInAggregationFn with get
        abstract member min: BuiltInAggregationFn with get
        abstract member max: BuiltInAggregationFn with get
        abstract member extent: BuiltInAggregationFn with get
        abstract member mean: BuiltInAggregationFn with get
        abstract member median: BuiltInAggregationFn with get
        abstract member unique: BuiltInAggregationFn with get
        abstract member uniqueCount: BuiltInAggregationFn with get
        abstract member count: BuiltInAggregationFn with get


[<AutoOpen; Erase>]
module RowSorting =
    type SortingFn = interface end
    type BuiltInSortingFn = SortingFn

    [<Import("sortingFns", tableCore)>]
    [<AllowNullLiteral; Interface>]
    type sortingFns =
        abstract member alphanumeric: BuiltInSortingFn with get
        abstract member alphanumericCaseSensitive: BuiltInSortingFn with get
        abstract member text: BuiltInSortingFn with get
        abstract member textCaseSensitive: BuiltInSortingFn with get
        abstract member datetime: BuiltInSortingFn with get
        abstract member basic: BuiltInSortingFn with get
