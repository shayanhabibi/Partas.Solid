---
title: Data tables
---

:::warning
The walkthrough after the live example below targets Partas.Solid 2.x on Solid 1.9. TanStack Table, Kobalte and Lucide
have not been ported to Solid 2, so it does not run on Partas.Solid 3.0 yet. Notes point out what changes in 3.0.
:::

The walkthrough builds a data table with `@tanstack/solid-table`, through `Partas.Solid.TanStack.Table`, styled with
Tailwind. It follows the `solid-ui` port of the `shadcn-ui` data table.

For a table on Partas.Solid 3.0 today, the live example below sorts one with `@tanstack/table-core`. The repository's
runtime tests also hold a sortable, filterable table in plain Partas.Solid:
`tests/Partas.Solid.Tests.Runtime/Integration/Apps/E-DataTable.fs`.

## Try it on Partas.Solid 3.0

`@tanstack/solid-table` is still Solid 1, but `@tanstack/table-core` has no framework dependency at all, so it
works with Solid 2 today. You bind the three functions you need with `[<Import>]`, and describe the parts of the
table object you call with a few F# interfaces. Nothing here is generated: an interface member such as
`getRowModel: unit -> RowModel` compiles to a plain `table.getRowModel()` call.

```fsharp solid setup
// Hidden: the rows the example sorts.
type Planet = { name: string; moons: int; diameter: int; au: float }

let planets =
    [| { name = "Mercury"; moons = 0; diameter = 4879; au = 0.39 }
       { name = "Venus"; moons = 0; diameter = 12104; au = 0.72 }
       { name = "Earth"; moons = 1; diameter = 12742; au = 1.0 }
       { name = "Mars"; moons = 2; diameter = 6779; au = 1.52 }
       { name = "Jupiter"; moons = 95; diameter = 139820; au = 5.2 }
       { name = "Saturn"; moons = 146; diameter = 116460; au = 9.54 }
       { name = "Uranus"; moons = 28; diameter = 50724; au = 19.19 }
       { name = "Neptune"; moons = 16; diameter = 49244; au = 30.07 } |]
```

```fsharp solid
[<Import("createTable", "@tanstack/table-core")>]
let createTable (options: obj): obj = jsNative

[<Import("getCoreRowModel", "@tanstack/table-core")>]
let getCoreRowModel (): obj = jsNative

[<Import("getSortedRowModel", "@tanstack/table-core")>]
let getSortedRowModel (): obj = jsNative

type ColumnSort = {| id: string; desc: bool |}

type Column =
    abstract id: string
    abstract columnDef: {| header: string; meta: {| numeric: bool |} |}
    /// false, "asc" or "desc"
    abstract getIsSorted: unit -> obj
    abstract getToggleSortingHandler: unit -> (obj -> unit)

type Header =
    abstract id: string
    abstract column: Column

type Cell =
    abstract id: string
    abstract column: Column
    abstract getValue: unit -> obj

type Row =
    abstract id: string
    abstract getVisibleCells: unit -> Cell array

type RowModel =
    abstract rows: Row array

type Table =
    abstract initialState: obj
    abstract setOptions: (obj -> obj) -> unit
    abstract getFlatHeaders: unit -> Header array
    abstract getRowModel: unit -> RowModel
```

table-core does not own any state. The sorting lives in a Solid signal, and a memo hands it to the table with
`setOptions` whenever it changes. Everything that reads the table reads that memo first, so it always sees the
current sort. Click a header to sort by it; shift-click to add a second sort key.

```fsharp solid render=PlanetTable jsx
[<SolidComponent>]
let PlanetTable () =
    let sorting, setSorting = createSignal<ColumnSort array> [||]

    let column key header numeric =
        {| accessorKey = key; header = header; meta = {| numeric = numeric |} |}

    // table-core passes either the new sorting or a function of the old one.
    let onSortingChange (updater: obj) =
        if jsTypeof updater = "function" then
            let update = unbox<ColumnSort array -> ColumnSort array> updater
            setSorting (update (sorting ()))
        else
            setSorting (unbox updater)

    let tbl: Table =
        createTable
            {| data = planets
               columns =
                   [| column "name" "Planet" false
                      column "moons" "Moons" true
                      column "diameter" "Diameter (km)" true
                      column "au" "Distance (AU)" true |]
               state = createObj []
               onStateChange = ignore
               onSortingChange = onSortingChange
               renderFallbackValue = null
               getCoreRowModel = getCoreRowModel ()
               getSortedRowModel = getSortedRowModel () |}
        |> unbox

    // Hand the signal to table-core. Returns the sorting, so every reader depends on it.
    let synced =
        createMemo (fun (_: ColumnSort array option) ->
            let current = sorting ()
            let state = JS.Constructors.Object.assign (createObj [], tbl.initialState, {| sorting = current |})
            tbl.setOptions (fun prev -> JS.Constructors.Object.assign (createObj [], prev, {| state = state |}))
            current)

    let headers () = synced () |> ignore; tbl.getFlatHeaders ()
    let rows () = synced () |> ignore; tbl.getRowModel().rows

    let arrow (column: Column) =
        synced () |> ignore
        match string (column.getIsSorted ()) with
        | "asc" -> "▲"
        | "desc" -> "▼"
        | _ -> ""

    let cellStyle (column: Column) =
        let align =
            if column.columnDef.meta.numeric then "text-align: right; font-variant-numeric: tabular-nums;"
            else "font-weight: 500;"
        let tint = if arrow column <> "" then " background: var(--nacara-primary-subtle);" else ""
        align + tint

    let summary () =
        match synced () |> Array.toList with
        | [] -> "Unsorted. Click a header."
        | keys ->
            keys
            |> List.map (fun key -> key.id + (if key.desc then " descending" else " ascending"))
            |> String.concat ", then "
            |> sprintf "Sorted by %s."

    let sortButton =
        "background: none; border: 0; padding: 0; font: inherit; color: inherit; cursor: pointer;"
        + " display: inline-flex; gap: .375rem; align-items: center"
    let footer =
        "display: flex; align-items: center; justify-content: space-between; gap: 1rem;"
        + " margin-top: .75rem; font-size: .875rem; color: var(--nacara-text-muted)"

    div () {
        table (style = "background: var(--nacara-bg)") {
            thead () {
                tr () {
                    For.Keyed(each = headers ()) {
                        yield fun header _ ->
                            let col = header.column
                            th (style = if col.columnDef.meta.numeric then "text-align: right" else "") {
                                button (
                                    class' = "planet-sort",
                                    style = sortButton,
                                    onClick = fun e -> col.getToggleSortingHandler () e
                                ) {
                                    col.columnDef.header
                                    span (style = "font-size: .7em; width: 1em; color: var(--nacara-primary)") {
                                        arrow col
                                    }
                                }
                            }
                    }
                }
            }
            tbody () {
                For.KeyedFn(each = rows (), keyed = fun (row: Row) -> box row.id) {
                    yield fun row _ ->
                        tr () {
                            For.Keyed(each = row().getVisibleCells ()) {
                                yield fun cell _ ->
                                    td (style = cellStyle cell.column) { string (cell.getValue ()) }
                            }
                        }
                }
            }
        }
        div (style = footer) {
            span () { summary () }
            button (class' = "p-btn p-btn--secondary", onClick = fun _ -> setSorting [||]) { "Clear sorting" }
        }
    }
```

## Setting up the 2.x project

The rest of this walkthrough needs a Partas.Solid 2.x project on Solid 1.9, set up as in the
[2.x documentation](https://partas-solid.vercel.app/partas-solid), with Tailwind 4 and the `shadcn` colour variables
from [solid-ui](https://www.solid-ui.com/). On top of that, it uses these packages:

```json
{
  "dependencies": {
    "@kobalte/core": "^0.13.8",
    "@tanstack/solid-table": "^8.21.2",
    "clsx": "^2.1.1",
    "lucide-solid": "^0.474.0",
    "tailwind-merge": "^2.6.0"
  }
}
```

```bash
dotnet add package Partas.Solid.TanStack.Table
dotnet add package Partas.Solid.Kobalte
dotnet add package Partas.Solid.Lucide
```

Next: [Utilities](utils.md).
