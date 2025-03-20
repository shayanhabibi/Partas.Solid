module Partas.Solid.Tests.IssueCases.TransformGetterExtensions

open Partas.Solid
open Partas.Solid.TanStack.Table
open Fable.Core

[<Erase>]
type TransformGetterExtensions() =
    inherit VoidNode()
    [<SolidTypeComponentAttribute>]
    member props.constructor =
        let table = createTable<int>(TableOptions())
        div() {
            Show(when' = unbox (table.getRowModel().rows.Length), fallback = (div())) {
                For(each = table.getRowModel().rows) {
                    yield fun row index ->
                        div().data("state", unbox (row.getIsSelected() && unbox "selected")) {
                            For(each = row.getVisibleCells()) {
                                yield fun cell index ->
                                    div() { flexRender(cell.column.columnDef.cell, cell.getContext) }
                            }
                        }
                }
            }
        }