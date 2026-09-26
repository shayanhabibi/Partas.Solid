---
title: The DataTable component
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

With the styled bases in place, build the basic `DataTable` component. It receives the `@tanstack/solid-table` object
and renders it. Create `DataTable.fs`:

```fsharp
namespace Partas.Solid.Example

open Partas.Solid
open Partas.Solid.TanStack.Table
open Fable.Core
open Fable.Core.JsInterop

[<Erase>]
type DataTable<'T>() =
    interface VoidNode
```

:::details Why VoidNode?
`DataTable` takes no children, so it implements `VoidNode`. Passing it children is a compile error.
:::

`DataTable` needs a property that receives the TanStack `table` object. In `Partas.Solid.TanStack.Table` its type is
`Table<'T>`.

F# has several ways to declare an optional constructor parameter. Here it is `[<DefaultValue>]` with `val mutable`,
which makes a get/set property. The value you would give it here never reaches the compiled output; only the type
matters.

```fsharp
    [<DefaultValue>]
    val mutable table: Table<'T>
```

The rest needs some reading about `@tanstack/table` to follow. If you plan to use the library, you will need that
reading anyway.

```fsharp
[<Erase>]
type DataTable<'T>() =
    interface VoidNode
    [<DefaultValue>] val mutable table: Table<'T>
    [<SolidTypeComponent>]
    member props.__ =
        let table = props.table
        Table() {
            TableHeader() {
                For(each = table.getHeaderGroups()) {
                    yield fun headerGroup _ ->
                        TableRow() {
                            For(each = headerGroup.headers) {
                                yield fun header _ ->
                                    TableHead(colspan = header.colSpan) {
                                        Show(when' = not header.isPlaceholder) {
                                            flexRender(header.column.columnDef.header, header.getContext())
                                        }
                                    }
                            }
                        }
                }
            }
            TableBody() {
                Show(
                    when' = unbox (table.getRowModel().rows.Length)
                    ,fallback = (TableRow() { TableCell(colspan = (8), class' = "h-24 text-center") { "No Results." } })
                    ) {
                    For(each = table.getRowModel().rows) {
                        yield fun row _ ->
                            TableRow().data("state", !!(row.getIsSelected() && !!"selected")) {
                                For(each = row.getVisibleCells()) {
                                    yield fun cell _ ->
                                        TableCell() {
                                            flexRender(cell.column.columnDef.cell, cell.getContext())
                                        }
                                }
                            }
                    }
                }
            }
        }
```

:::note
Partas.Solid 3.0 has no plain `For`. Solid 2 folds `Index` into `For` through a `keyed` prop, and the bindings expose
the two modes as separate types. The list above would use `For.Keyed`, which keys rows by identity:

```fsharp
For.Keyed(each = table.getRowModel().rows) {
    yield fun row _ -> TableRow() { (* ... *) }
}
```

`For.NonKeyed` replaces the old `Index`. The two differ in which argument is reactive. `For.Keyed` passes the item and
an index accessor, so you read the index as `index ()`. `For.NonKeyed` passes an item accessor and a plain `int`
index, so you read the item as `item ()`. See [Migrating to Solid 2](../../guide/migrating-to-solid-2.md).
:::

:::details Why SolidTypeComponent and not SolidComponent?
With this implementation there is no real reason to use `SolidTypeComponent`, except that it lets you use the
component the same way as the rest of the DSL.

`DataTable` does not spread any of the optional properties it might receive, such as `class`. To get something out
of `SolidTypeComponent`, spread `props` into the top-level `Table`.
:::

Next: [Column definitions](column-defs.md).
