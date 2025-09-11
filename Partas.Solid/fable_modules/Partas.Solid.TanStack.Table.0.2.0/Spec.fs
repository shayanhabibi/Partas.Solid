namespace Partas.Solid.TanStack.Table

open Fable.Core
open System

[<AutoOpen; Erase>]
module Spec =
    let [<Erase; Literal>] root = "@tanstack/"
    let [<Erase; Literal>] solidTable = root + "solid-table"
    let [<Erase; Literal>] coreTable = root + "table-core"

[<AutoOpen; Erase>]
module Enums =
    [<StringEnum; RequireQualifiedAccess>]
    type ColumnResizingMode =
        | OnChange
        | OnEnd
    [<StringEnum; RequireQualifiedAccess>]
    type ColumnResizingDirection =
        | Ltr
        | Rtl
    [<StringEnum; RequireQualifiedAccess>]
    type SortUndefined =
        | First
        | Last
        | [<CompiledValue(false)>] False
        | [<CompiledValue(-1)>] Negative
        | [<CompiledValue(1)>] Positive
    [<StringEnum; RequireQualifiedAccess>]
    type GroupedColumnMode =
        | [<CompiledValue(false)>] False
        | Reorder
        | Remove
    [<StringEnum; RequireQualifiedAccess>]
    type ColumnPinningPosition =
        | [<CompiledValue(false)>] False
        | Left
        | Right
    [<StringEnum>]
    type SortDirection =
        | Asc
        | Desc
    [<RequireQualifiedAccess>]
    module Column =
        module Resizing =
            type Mode = ColumnResizingMode
            type Direction = ColumnResizingDirection
        module Sorting =
            type Undefined = SortUndefined
            type Direction = SortDirection
        module Grouping =
            type Mode = GroupedColumnMode
        module Pinning =
            type Position = ColumnPinningPosition
