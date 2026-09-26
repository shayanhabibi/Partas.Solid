---
title: Data tables
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

This walkthrough builds a data table with `@tanstack/solid-table`, through `Partas.Solid.TanStack.Table`, styled
with `tailwindcss` and a few small utilities. It follows the `solid-ui` port of the `shadcn-ui` data table.

The code on these pages is kept as it was written for Partas.Solid 2.x. TanStack Table, Kobalte and Lucide have not
been ported to Solid 2, so you cannot run it against Partas.Solid 3.0 yet. Notes point out what changes in 3.0 where
it matters.

:::tip
If you want a table on Partas.Solid 3.0 today, the runtime tests in the Partas.Solid repository include a sortable,
filterable data table written with plain Partas.Solid and no TanStack:
`Partas.Solid.Tests.Runtime/Integration/Apps/E-DataTable.fs`. [Try it on Partas.Solid 3.0](#try-it-on-partas.solid-3.0),
below, sorts a table with `@tanstack/table-core` instead, live on this page.
:::

The pages, in order:

1. [Getting started](index.md) (this page)
2. [Utilities](utils.md)
3. [Table components](table-component.md)
4. [The DataTable component](datatable-component.md)
5. [Column definitions](column-defs.md)
6. [Rendering the table](table-render.md)
7. [Selectable rows](selectable-rows.md)

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

## Getting started

Set up either a SolidStart project or a plain Vite project.

## Partas.SolidStart

See [Installation](../../guide/installation.md) and start from the tailwind template. Then install the npm
dependencies:

```bash
npm install @kobalte/core @tanstack/solid-table clsx tailwind-merge lucide-solid
```

and the bindings:

```bash
dotnet add package Partas.Solid.TanStack.Table
dotnet add package Partas.Solid.Lucide
dotnet add package Partas.Solid.Kobalte
```

Start the dev server and check that everything works:

```bash
dotnet run dev
```

## Vite

Create a console `.fsproj` and set up a Vite environment inside it. You can do this by hand, with a
`vite.config.mts`, an `index.html` and an `index.css`. The dependencies come next.

::::tabs
:::tab vite.config.mts
```ts
import { defineConfig } from 'vite'
import solidPlugin from 'vite-plugin-solid';
import tailwindcss from "@tailwindcss/vite";

// https://vitejs.dev/config/
export default defineConfig({
    server: {
        watch: {
            ignored: [
                "**/*.md" , // Don't watch markdown files
                "**/*.fs" , // Don't watch F# files
                "**/*.fsx"  // Don't watch F# script files
            ]
        }
    },
    plugins: [
        solidPlugin(),
        tailwindcss(),
    ],
})
```
:::
:::tab index.html
```xml
<!DOCTYPE html>
<html lang="en">
<head>
    <link rel="icon" href="./favicon.ico" type="image/x-icon" />
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Partas.Solid Playground</title>
    <script type="module" src="/Program.fs.jsx"></script>
    <!-- Adjust the src to point at your compiled entry file -->
</head>
<body>
    <div id="root"></div>
</body>
</html>
```
:::
:::tab index.css
```css
@import 'tailwindcss';

/* set default border color due to tailwindcss v4 */
@layer base {
    *,
    ::after,
    ::before,
    ::backdrop,
    ::file-selector-button {
        border-color: var(--color-gray-200, currentColor);
    }
}

/* Base color palette */
@layer base {
    :root {
        --background: hsl(0 0% 100%);
        --foreground: hsl(240 10% 3.9%);

        --muted: hsl(240 4.8% 95.9%);
        --muted-foreground: hsl(240 3.8% 46.1%);

        --popover: hsl(0 0% 100%);
        --popover-foreground: hsl(240 10% 3.9%);

        --border: hsl(240 5.9% 90%);
        --input: hsl(240 5.9% 90%);

        --card: hsl(0 0% 100%);
        --card-foreground: hsl(240 10% 3.9%);

        --primary: hsl(240 5.9% 10%);
        --primary-foreground: hsl(0 0% 98%);

        --secondary: hsl(240 4.8% 95.9%);
        --secondary-foreground: hsl(240 5.9% 10%);

        --accent: hsl(240 4.8% 95.9%);
        --accent-foreground: hsl(240 5.9% 10%);

        --destructive: hsl(0 84.2% 60.2%);
        --destructive-foreground: hsl(0 0% 98%);

        --info: hsl(204 94% 94%);
        --info-foreground: hsl(199 89% 48%);

        --success: hsl(149 80% 90%);
        --success-foreground: hsl(160 84% 39%);

        --warning: hsl(48 96% 89%);
        --warning-foreground: hsl(25 95% 53%);

        --error: hsl(0 93% 94%);
        --error-foreground: hsl(0 84% 60%);

        --ring: hsl(240 5.9% 10%);

        --radius: 0.5rem;
    }

    .dark,
    [data-kb-theme="dark"] {
        --background: hsl(240 10% 3.9%);
        --foreground: hsl(0 0% 98%);

        --muted: hsl(240 3.7% 15.9%);
        --muted-foreground: hsl(240 5% 64.9%);

        --accent: hsl(240 3.7% 15.9%);
        --accent-foreground: hsl(0 0% 98%);

        --popover: hsl(240 10% 3.9%);
        --popover-foreground: hsl(0 0% 98%);

        --border: hsl(240 3.7% 15.9%);
        --input: hsl(240 3.7% 15.9%);

        --card: hsl(240 10% 3.9%);
        --card-foreground: hsl(0 0% 98%);

        --primary: hsl(0 0% 98%);
        --primary-foreground: hsl(240 5.9% 10%);

        --secondary: hsl(240 3.7% 15.9%);
        --secondary-foreground: hsl(0 0% 98%);

        --destructive: hsl(0 62.8% 30.6%);
        --destructive-foreground: hsl(0 0% 98%);

        --info: hsl(204 94% 94%);
        --info-foreground: hsl(199 89% 48%);

        --success: hsl(149 80% 90%);
        --success-foreground: hsl(160 84% 39%);

        --warning: hsl(48 96% 89%);
        --warning-foreground: hsl(25 95% 53%);

        --error: hsl(0 93% 94%);
        --error-foreground: hsl(0 84% 60%);

        --ring: hsl(240 4.9% 83.9%);

        --radius: 0.5rem;
    }
}

/* Sidebar colour palette */
@layer base {
    :root {
        --sidebar-background: hsl(0 0% 98%);
        --sidebar-foreground: hsl(240 5.3% 26.1%);
        --sidebar-primary: hsl(240 5.9% 10%);
        --sidebar-primary-foreground: hsl(0 0% 98%);
        --sidebar-accent: hsl(240 4.8% 95.9%);
        --sidebar-accent-foreground: hsl(240 5.9% 10%);
        --sidebar-border: hsl(220 13% 91%);
        --sidebar-ring: hsl(217.2 91.2% 59.8%);
    }

    .dark,
    [data-kb-theme="dark"] {
        --sidebar-background: hsl(240 5.9% 10%);
        --sidebar-foreground: hsl(240 4.8% 95.9%);
        --sidebar-primary: hsl(224.3 76.3% 48%);
        --sidebar-primary-foreground: hsl(0 0% 100%);
        --sidebar-accent: hsl(240 3.7% 15.9%);
        --sidebar-accent-foreground: hsl(240 4.8% 95.9%);
        --sidebar-border: hsl(240 3.7% 15.9%);
        --sidebar-ring: hsl(217.2 91.2% 59.8%);
    }
}

@theme inline {
    /* Color utils */
    --color-border: var(--border);
    --color-input: var(--input);
    --color-ring: var(--ring);

    --color-background: var(--background);
    --color-foreground: var(--foreground);

    --color-primary: var(--primary);
    --color-primary-foreground: var(--primary-foreground);

    --color-secondary: var(--secondary);
    --color-secondary-foreground: var(--secondary-foreground);

    --color-destructive: var(--destructive);
    --color-destructive-foreground: var(--destructive-foreground);

    --color-muted: var(--muted);
    --color-muted-foreground: var(--muted-foreground);

    --color-info: var(--info);
    --color-info-foreground: var(--info-foreground);

    --color-success: var(--success);
    --color-success-foreground: var(--success-foreground);

    --color-warning: var(--warning);
    --color-warning-foreground: var(--warning-foreground);

    --color-error: var(--error);
    --color-error-foreground: var(--error-foreground);

    --color-accent: var(--accent);
    --color-accent-foreground: var(--accent-foreground);

    --color-popover: var(--popover);
    --color-popover-foreground: var(--popover-foreground);

    --color-card: var(--card);
    --color-card-foreground: var(--card-foreground);

    /* Sidebar */
    --color-sidebar: var(--sidebar-background);
    --color-sidebar-foreground: var(--sidebar-foreground);

    --color-sidebar-primary: var(--sidebar-primary);
    --color-sidebar-primary-foreground: var(--sidebar-primary-foreground);

    --color-sidebar-accent: var(--sidebar-accent);
    --color-sidebar-accent-foreground: var(--sidebar-accent-foreground);

    --color-sidebar-border: var(--sidebar-border);
    --color-sidebar-ring: var(--sidebar-ring);

    /* Radius */
    --border-radius-sm: calc(var(--radius) - 4px);
    --border-radius-md: calc(var(--radius) - 2px);
    --border-radius-lg: calc(var(--radius));
    --border-radius-xl: calc(var(--radius) + 4px);
}

```
:::
::::

Here is a `package.json` to match:

```json
{
  "private": "true",
  "scripts": {
    "start": "dotnet tool run fable watch -c Release --optimize --typedArrays false -e .fs.jsx --run vite"
  },
  "dependencies": {
    "@kobalte/core": "^0.13.8",
    "@tailwindcss/vite": "^4.0.1",
    "@tanstack/solid-table": "^8.21.2",
    "clsx": "^2.1.1",
    "lucide-solid": "^0.474.0",
    "solid-js": "^1.9.2",
    "tailwind-merge": "^2.6.0",
    "tailwindcss": "^4.0.1"
  },
  "devDependencies": {
    "vite": "^5.4.9",
    "vite-plugin-solid": "^2.10.2"
  }
}
```

:::note
`solid-js ^1.9` and `vite-plugin-solid` are Solid 1. A Partas.Solid 3.0 project uses Solid 2 (`solid-js`,
`@solidjs/web` and the Solid 2 Vite plugin, `@solidjs/vite-plugin`) instead, which is why the TanStack, Kobalte and Lucide bindings cannot be
used with it yet. See [Installation](../../guide/installation.md) for a Solid 2 setup.
:::

Install the npm dependencies while you sort out the .NET ones:

```bash
npm install
```

Then install these packages, and Fable 5 or later:

::::tabs
:::tab dotnet
```bash
dotnet add package Partas.Solid
dotnet add package Partas.Solid.TanStack.Table
dotnet add package Partas.Solid.Kobalte
dotnet add package Partas.Solid.Lucide
```
:::
:::tab paket
```bash
paket install Partas.Solid
paket install Partas.Solid.TanStack.Table
paket install Partas.Solid.Kobalte
paket install Partas.Solid.Lucide
```
:::
::::

That gives you some headless components and icons to work with.

Now create a file called `Root.fs` that compiles before `Program.fs`, and add this:

```fsharp
// Remember: your namespaces must start with Partas.Solid
module Partas.Solid.Example.Root
open Partas.Solid
open Fable.Core

[<SolidComponent>]
let Root () =
    div() {
        "Hello World!"
    }
```

Add this to `Program.fs`:

```fsharp
module Partas.Solid.Example.Program

open Partas.Solid.Example.Root
open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop

importSideEffects "./index.css"

render(Root, Browser.Dom.document.getElementById "root")
```

Keeping `Root` in its own file avoids glitches with hot module reloading.

:::note
In Partas.Solid 3.0, `render` lives in the `Partas.Solid.Web` namespace, so `Program.fs` also needs
`open Partas.Solid.Web`.
:::

Now run the npm script, and check that the browser shows "Hello World!":

```bash
npm run start
```

Next: [Utilities](utils.md).
