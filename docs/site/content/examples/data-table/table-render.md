---
title: Rendering the table
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Put it all together and render a first, basic table before you add anything else. Create `TableTest.fs`:

```fsharp
module Partas.Solid.Example.TableTest

open Partas.Solid
open Partas.Solid.Example.TestColumnDefs
open Partas.Solid.TanStack.Table
open Fable.Core

let userData = [|
        { Name = "Name"; Color = "Color"; Code = "Code" }
        { Name = "Name1"; Color = "Color1"; Code = "Code1" }
        { Name = "Name2"; Color = "Color2"; Code = "Code2" }
    |]

[<SolidComponent>]
let Test () =
    let table = createTable(
            TableOptions<User>(
                getCoreRowModel = getCoreRowModel()
            )   .data(fun _ -> userData)
                .columns(fun _ -> columnDefs)
        )
    DataTable(table = table)
```

:::details title="Why are data and columns methods?"
TanStack needs `data` and `columns` to be `get` properties of the `TableOptions` object passed to `createTable`. The
`.data` and `.columns` methods set them up that way for you.
:::

Render `Test` from your `Root` component to see the table.

:::note
The old version of this page ran the example in the browser. It is not live here, because the examples on this site
run on Solid 2 and TanStack Table's bindings still need Solid 1.
:::

Next: [Selectable rows](selectable-rows.md).
