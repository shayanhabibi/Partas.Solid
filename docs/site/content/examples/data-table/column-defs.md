---
title: Column definitions
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Now for the tedious part: defining the columns, which is the table's structure.

Start with the type that fills the table. Create `Model.fs`:

```fsharp
module Partas.Solid.Example.TestColumnDefs

open Fable.Core
open Partas.Solid.TanStack.Table
open Partas.Solid

type User = {
    Code: string
    Name: string
    Color: string
}
```

Now define the columns. You can organise this several ways. Here each column is defined on its own, and they are put
into an array at the end.

`ColumnDef<'T>` from `Partas.Solid.TanStack.Table` is close to a copy of the TypeScript type. In Fable, the best fit
for a type like that is a `Pojo` class with optional constructor parameters.

Start with the column for the user's `Code`. In TypeScript you would usually name the field:

```fsharp
let codeColumn = ColumnDef<User>(
    accessor = "Code"
)
```

TanStack also has `accessorFn`, which is better: it is type safe. The second parameter of the `accessorFn` lambda is
the row's index.

```fsharp
let codeColumn = ColumnDef<User>(
    accessorFn = fun user _ -> user.Code
)
```

Then render the cell however you like, and add a header and anything else you need:

```fsharp
[<SolidComponent>]
let codeColumn = ColumnDef<User>(
    accessorFn = fun user _ -> user.Code
    ,header = !!"Code"
    ,cell = fun props ->
        div(class' = "w-14 hover:scale-102 flex justify-center bg-black text-white") {
                props.getValue() :?> string
            }
)
```

:::warning
Now that the `let` renders tags, it needs `[<SolidComponent>]`, so that the plugin transforms it.
:::

Do the same for the other columns, styled however you like, and put them in an array:

```fsharp
let columnDefs = [|
    codeColumn
    nameColumn
    colorColumn
|]
```

Next: [Rendering the table](table-render.md).
