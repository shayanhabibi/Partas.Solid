---
title: Kobalte
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

`Partas.Solid.Kobalte` binds [Kobalte](https://kobalte.dev), a library of headless UI components. See its docs for
usage.

Install the npm package with Femto, or by hand:

```bash
npm install @kobalte/core
```

Each component binds its `Root` under the component's name, and its parts as qualified names under it. For example,
`Combobox()` and `Combobox.Input()`.

## Polymorphism

Kobalte's `as` prop is the `.as'` extension method from Partas.Solid's [polymorphism](polymorphism.md) support. Pass
a built element, so F# checks the morph target's props in its own constructor:

```fsharp
SidebarMenuButton(
    tooltip = item.Title,
    class' = "group/mbutton disabled:cursor-default"
    ) //---- Polymorphism
    .as' (
        A(href = item.Path)
    )
```

## Example component

A tooltip built from Kobalte's headless parts:

```fsharp
[<Erase>]
type TooltipTrigger() =
    inherit Tooltip.Trigger()
    [<SolidTypeComponent>]
    member props.constructor = Tooltip.Trigger().spread props

[<Erase>]
type Tooltip() =
    inherit Kobalte.Tooltip()
    [<SolidTypeComponent>]
    member props.constructor = Kobalte.Tooltip(gutter = 4).spread props

[<Erase>]
type TooltipContent() =
    inherit Tooltip.Content()
    [<SolidTypeComponent>]
    member props.constructor =
        Tooltip.Portal() {
            Tooltip.Content(
                class' =
                    Lib.cn [|
                        "z-50 origin-[var(--kb-popover-content-transform-origin)]
                        overflow-hidden rounded-md border bg-popover px-3 py-1.5
                        text-sm text-popover-foreground shadow-md animate-in fade-in-0 zoom-in-95"
                        props.class'
                    |]
                ).spread props
        }
```

`Lib.cn` is a class-merging helper from [Partas.Solid.UI](partas-solid-ui.md), not part of the binding.

## Example: color area

```fsharp
[<SolidComponent>]
let ColorAreaExample() =
    let color,setColor = createSignal<Color>(JS.undefined)
    let colorLabel format =
        if color() |> unbox then color().toString(format)
        else $"{format}"
    ColorArea(
        class' = "relative flex flex-col
                align-items-center w-[200px]"
        ,value = color()
        ,onChange = setColor
        ) {
        div(class' = "flex flex-col pb-2") {
            ColorArea.Label() { colorLabel Color.ToString.Format.Hsl }
            ColorArea.Label() { colorLabel Color.ToString.Format.Rgb }
            ColorArea.Label() { colorLabel Color.ToString.Format.Hex }
            ColorArea.Label() { colorLabel Color.ToString.Format.Hsb }
        }
        ColorArea.Background(
            class' = "relative rounded-sm
                    h-[150px] w-[150px]"
            ) {
            ColorArea.Thumb(
                class' = "block w-[16px] h-[16px]
                rounded-full border-1 border-border
                bg-(--kb-color-current)"
                ) {
                ColorArea.HiddenInputX()
                ColorArea.HiddenInputY()
            }
        }
    }
```

## Contexts

Most Kobalte components expose an undocumented context from their root component, which is useful in dialogs and
similar components. The binding only has the contexts that were needed so far.

## ColorMode

Kobalte's undocumented `ColorModeProvider` stores the theme mode (light or dark) locally and exposes it as a reactive
signal. See the binding's source for its options.
