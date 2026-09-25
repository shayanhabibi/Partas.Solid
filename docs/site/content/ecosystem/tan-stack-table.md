---
title: TanStack Table
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

`Partas.Solid.TanStack.Table` binds [TanStack Table](https://tanstack.com/table/latest). See its docs
for usage, and select the `Solid` framework when you read them.

## Differences

This is a large library, so there are a few more things to keep in mind.

### 1. Build all options with the provided Pojo constructors

```fsharp
[<SolidComponent>] // <-- apply the attribute so the code is in plugin scope
let idColumn = ColumnDef<Patient>(
    id = "id"
    ,accessorFn = fun patient _ -> patient.Id
    ,enableSorting = false // v---- the type is inferred by the compiler
    ,cell = fun (props: CellRenderProps<Patient>) ->
        span(class' = "text-xs text-muted-foreground justify-items-start") {
            props.cell.getValue() |> unbox<string>
        }
    ,header = fun props ->
        div(class' = "text-lg") { "Id" }
)
```

### 2. Set getter fields with the extension methods

Some option fields must be getters to be reactive in Solid. `TableOptions` is the main example: set those fields with
the extension methods, not the constructor. The fields that need this are:

- `data`
- `columns`
- the state accessors

```fsharp
TableOptions<'Data>(
    getCoreRowModel = getCoreRowModel()
    ,getFacetedRowModel = getFacetedRowModel() //v--- Signal
    ,onRowSelectionChanged = unbox(snd rowSelection)
)   .data(fun _ -> dataSource.data)
    .columns(fun _ -> columnDefs)
    .stateFn(fun (state: TableState) ->//v--- Signal
        state.rowSelection(fst rowSelection)
            .sorting(fst sorting)
            .pagination(fst pagination)
    )
```

Use the `.init()` helpers to create state signals:

```fsharp
let rowSelection = RowSelectionState.init() |> createSignal
```

Otherwise usage follows the upstream guides and examples. Some typings still need work, but most of the problems with
binding this library have been ironed out.

:::note
TanStack Table's core (`@tanstack/table-core`) does not depend on Solid; only the Solid adapter does.
:::
