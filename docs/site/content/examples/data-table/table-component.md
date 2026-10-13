---
title: Table components
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Next, create the styled base table components. They are opinionated and styled, but you can extend them, and the data
table builds on them.

Put them in a `Table.fs` file in the `Partas.Solid.Example` namespace, opening `Partas.Solid` and `Fable.Core`. You
need styled versions of `table`, `thead`, `tbody`, `tfoot`, `tr`, `th`, `td` and `caption`.

These components use only native elements, so they compile on Partas.Solid 3.0, and this page compiles them live. The
Tailwind classes are not loaded here, so the table at the bottom renders unstyled.

```fsharp solid setup
// A stand-in for Lib.cn from the previous page, without the npm packages.
type Lib =
    static member cn(classes: string array) =
        classes |> Array.filter (fun c -> not (isNull c)) |> String.concat " "
```

Start with `TableCaption`. Its JSX tab shows what 3.0 makes of it:

```fsharp solid jsx
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
- `.spread props` passes every other attribute through to the element. The JSX spreads `PARTAS_OTHERS`, the props
  minus the `class` the component read; 2.x wrote a `splitProps` here.

Try the next one on your own before you open the answer.

`Table` inherits from `table`. It is a `div` with the classes `"relative w-full overflow-auto"`, wrapping a `table`
with the classes `"w-full caption-bottom text-sm"` plus any classes passed to `Table`. The inner `table` also gets
every other attribute passed to `Table`.

:::details Table component
```fsharp solid
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

```fsharp solid
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

Used together:

```fsharp solid render=TableDemo
[<SolidComponent>]
let TableDemo () =
    Table() {
        TableCaption() { "Two planets" }
        TableHeader() {
            TableRow() {
                TableHead() { "Planet" }
                TableHead() { "Moons" }
            }
        }
        TableBody() {
            TableRow() {
                TableCell() { "Earth" }
                TableCell() { "1" }
            }
            TableRow() {
                TableCell() { "Mars" }
                TableCell() { "2" }
            }
        }
    }
```

Next: [The DataTable component](datatable-component.md).
