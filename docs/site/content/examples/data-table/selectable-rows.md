---
title: Selectable rows
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

As an example, make the rows selectable, and add a "select all" control too.

First you need something to select a row with: a checkbox. This page uses `@kobalte/core`, an accessible headless UI
library, and `lucide` for icons. It shows how a headless UI library is used from Partas.Solid, since Kobalte is the
main one with bindings.

## Checkbox

Unstyled, a Kobalte checkbox is a root, a hidden input, a control and an indicator:

```jsx
<Root class="size-8 bg-muted">
    <Input />
    <Control class="size-8">
        <Indicator />
    </Control>
</Root>
```

:::tip
In a running app, inspect the element and click it, and you will see the `checked` data attribute appear.
:::

The `Checkbox` component inherits `Kobalte.Checkbox`:

```fsharp
open Partas.Solid.Kobalte

[<Erase>]
type Checkbox() =
    inherit Kobalte.Checkbox()
```

Add the implementation:

```fsharp
[<SolidTypeComponent>]
member props.__ =
    Kobalte.Checkbox(
        class' = Lib.cn [|
                "items-top group relative flex space-x-2"
                props.class'
            |]
        ).spread(props)
```

The checkbox needs a few accessibility parts inside it: `Input`, `Control` and `Indicator`. If you add them directly,
you may hit a type error. `Kobalte.Checkbox` expects its child to be a function.

:::tip
The tooltip for `Kobalte.Checkbox` shows:

```fsharp
type Checkbox =
  interface ChildLambdaProvider<CheckboxRenderProp>
  interface Polymorph
  interface HtmlTag
```

So the child is a lambda that takes one parameter, a `CheckboxRenderProp`.
:::

```fsharp
open Partas.Solid.Lucide

[<SolidTypeComponent>]
member props.checkbox =
    Kobalte.Checkbox(
        indeterminate = props.indeterminate,
        class' = Lib.cn [| "items-top group relative flex space-x-2"; props.class' |]
        ).spread(props) { yield fun _ -> Fragment() {

        Checkbox.Input(class'="peer")
        Checkbox.Control(
            class' = "size-4 shrink-0 rounded-sm border border-primary
            ring-offset-background disabled:cursor-not-allowed disabled:opacity-50
            peer-focus-visible:outline-none peer-focus-visible:ring-2 peer-focus-visible:ring-ring
            peer-focus-visible:ring-offset-2 data-[checked]:border-none
            data-[indeterminate]:border-none data-[checked]:bg-primary
            data-[indeterminate]:bg-primary data-[checked]:text-primary-foreground
            data-[indeterminate]:text-primary-foreground"
            ) {
            Checkbox.Indicator() {
                if props.indeterminate then
                    Minus(class' = "size-4", strokeWidth = 2)
                else
                    Check(class' = "size-4", strokeWidth = 2)
            }
        }
    }
    }
```

:::note
You want `indeterminate` on the root component, and you also read it to decide what to render. Reading it removes it
from the spread props, so you have to pass it to the root by name as well.
:::

:::details title="Using the CheckboxRenderProp instead"
The checkbox passes its state to its child function. Read `indeterminate` from there, and you no longer need to pass
the property by name:

```fsharp
[<SolidTypeComponent>]
member props.checkbox =
    Kobalte.Checkbox(
        class' = Lib.cn [| "items-top group relative flex space-x-2"; props.class' |]
        ).spread(props) { yield fun rprops -> Fragment() {

        Checkbox.Input(class'="peer")
        Checkbox.Control(
            class' = "size-4 shrink-0 rounded-sm border border-primary
            ring-offset-background disabled:cursor-not-allowed disabled:opacity-50
            peer-focus-visible:outline-none peer-focus-visible:ring-2 peer-focus-visible:ring-ring
            peer-focus-visible:ring-offset-2 data-[checked]:border-none
            data-[indeterminate]:border-none data-[checked]:bg-primary
            data-[indeterminate]:bg-primary data-[checked]:text-primary-foreground
            data-[indeterminate]:text-primary-foreground"
            ) {
            Checkbox.Indicator() {
                if rprops.indeterminate then
                    Minus(class' = "size-4", strokeWidth = 2)
                else
                    Check(class' = "size-4", strokeWidth = 2)
            }
        }
    }
    }
```
:::

## Column definition

Add a column definition for the selection column, which holds the checkbox for each row.

This time, use the full signatures of `ColumnDef`'s `header` and `cell` properties. They pass the row, or the table,
into the definition.

```fsharp
open Partas.Solid.Aria

[<SolidComponent>]
let selectColumn =
    ColumnDef<User>(
        id = "select"
        ,enableHiding = false
        ,cell = fun props ->
            Checkbox(
                checked' = props.row.getIsSelected(),
                onChange = fun value -> props.row.toggleSelected(!!value)
                ,ariaLabel = "Select row",
                class' = "translate-y-[2px]"
                )
        ,header = fun props ->
            Checkbox(
                checked' = (props.table.getIsAllPageRowsSelected()),
                indeterminate = (props.table.getIsSomePageRowsSelected()),
                onChange = fun value -> props.table.toggleAllPageRowsSelected(!!value)
                ,ariaLabel = "Select all"
                ,class' = "translate-y-[2px]"
                )
    )

let columnDefs = [|
    selectColumn
    codeColumn
    nameColumn
    colorColumn
|]
```

:::note
The header is not a plain string, so the column needs a unique `id` to identify it.
:::

## Adding selection to the table

Change the table options to hold the selection state. First, make a signal for it:

```fsharp
[<SolidComponent>]
let TestSelectableTable () =
    let selection,setSelection =
        createSignal <| RowSelectionState.init()
    let table = createTable(
            TableOptions<User>(
                getCoreRowModel = getCoreRowModel()
            )   .data(fun _ -> userData)
                .columns(fun _ -> columnDefs)
        )
    DataTable(table = table)
```

Then plug it into the `TableOptions` passed to `createTable`:

```fsharp
[<SolidComponent>]
let TestSelectableTable () =
    let selection,setSelection =
        createSignal <| RowSelectionState.init()
    let table = createTable(
            TableOptions<User>(
                getCoreRowModel = getCoreRowModel(),
                enableRowSelection = !!true,
                onRowSelectionChange = !!setSelection
            )   .data(fun _ -> userData)
                .columns(fun _ -> columnDefs)
                .stateFn(fun state ->
                    state.rowSelection(selection)
                    )
        )
    DataTable(table = table)
```

:::details title="What is stateFn?"
Like `data` and `columns`, the row selection has to be a `get` property of the options object to be reactive. `.stateFn`
hands you an object that makes that easy to do from Fable.
:::

Render `TestSelectableTable` and try selecting rows.

## Conclusion

That is the end of this exercise. The idea was to try some of the binding libraries and get familiar with
Partas.Solid.

For data tables, `@tanstack/table` is one of the best known libraries around. Its API, and this large binding, are
worth exploring.
