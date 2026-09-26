---
title: Table components
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Next, create the styled base table components. They are opinionated and styled, but you can extend them, and the data
table builds on them.

Put them in a `Table.fs` file in the `Partas.Solid.Example` namespace:

```fsharp
namespace Partas.Solid.Example

open Partas.Solid
open Fable.Core
```

You need styled versions of `table`, `thead`, `tbody`, `tfoot`, `tr`, `th`, `td` and `caption`. Change the styles or
write your own versions if you like.

Start with `TableCaption`:

```fsharp
[<Erase>]
type TableCaption() =
    inherit caption()
    [<SolidTypeComponent>]
    member props.__ =
        caption(class' = Lib.cn [|
                "mt-4 text-sm text-muted-foreground"
                props.class'
            |]).spread props
```

- It inherits every attribute of the native `caption` element, and returns a `caption`.
- `Lib.cn`, from [Utilities](utils.md), merges the preset classes with any passed to the component. The passed ones
  win.
- `.spread props` passes every other attribute through to the element.

Try the next one on your own before you open the answer.

`Table` inherits from `table`. It is a `div` with the classes `"relative w-full overflow-auto"`, wrapping a `table`
with the classes `"w-full caption-bottom text-sm"` plus any classes passed to `Table`. The inner `table` also gets
every other attribute passed to `Table`.

:::details Table component
```fsharp
[<Erase>]
type Table() =
    inherit table()
    [<SolidTypeComponent>]
    member props.__ =
        div(class' = "relative w-full overflow-auto") {
            table(class' = Lib.cn [|
                    "w-full caption-bottom text-sm"
                    props.class'
                |]).spread props
        }
```
:::

Then finish the set:

```fsharp
[<Erase>]
type TableHeader() =
    inherit thead()
    [<SolidTypeComponent>]
    member props.constructor =
        thead(class' = Lib.cn [| "[&_tr]:border-b"
                                 props.class' |])
            .spread props

[<Erase>]
type TableBody() =
    inherit tbody()
    [<SolidTypeComponent>]
    member props.constructor =
        tbody(class' = Lib.cn [|
            "[&_tr:last-child]:border-0"
            props.class'
        |]).spread props

[<Erase>]
type TableFooter() =
    inherit tfoot()
    [<SolidTypeComponent>]
    member props.constructor =
        tfoot(class' = Lib.cn [|
            "bg-primary font-medium text-primary-foreground"
            props.class'
        |]).spread props

[<Erase>]
type TableRow() =
    inherit tr()
    [<SolidTypeComponent>]
    member props.constructor =
        tr(
            class' = Lib.cn [|
                "border-b transition-colors hover:bg-muted/50 data-[state=selected]:bg-muted"
                props.class'
            |]
        ).spread props

[<Erase>]
type TableHead() =
    inherit th()
    [<SolidTypeComponent>]
    member props.constructor =
        th(
            class' = Lib.cn [|
                "h-10 px-2 text-left align-middle font-medium
                text-muted-foreground [&:has([role=checkbox])]:pr-0"
                props.class'
            |]
        ).spread props

[<Erase>]
type TableCell() =
    inherit td()
    [<SolidTypeComponentAttribute>]
    member props.constructor =
        td(class' = Lib.cn [| "p-2 align-middle [&:has([role=checkbox])]:pr-0"; props.class' |]).spread props
```

:::note
These components use only native elements, so they compile unchanged on Partas.Solid 3.0. What changes is the
output: each becomes `const PARTAS_OTHERS = omit(props, "class")` and `{...PARTAS_OTHERS} n$={false}`, where 2.x wrote
a `splitProps`. See [Benefits of JSX output](../../about/jsx-output.md).
:::

Next: [The DataTable component](datatable-component.md).
