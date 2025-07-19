namespace Partas.Solid.TanStack.Table

open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop

[<AllowNullLiteral; Interface>]
type ColumnOrderTableState = interface end

type ColumnOrderState = string[]

[<Erase; AutoOpen>]
module ordrr =
    type ColumnOrderTableState with
        member _.columnOrder
            with set (columnOrder: ColumnOrderState) = ()
            and get (): ColumnOrderState = unbox null

    type TableOptions<'Data> with
        member _.onColumnOrderChange
            with set (value: OnChangeFn<ColumnOrderState>) = ()
            and get () = unbox null

    type Table<'Data> with
        member _.setColumnOrder: Updater<ColumnOrderState> -> unit = unbox null
        member _.resetColumnOrder: bool -> unit = unbox null

    type Column<'Data> with
        member _.getIndex: ColumnPinningPosition -> int = unbox null
        member _.getIsFirstColumn: ColumnPinningPosition -> bool = unbox null
        member _.getIsLastColumn: ColumnPinningPosition -> bool = unbox null
