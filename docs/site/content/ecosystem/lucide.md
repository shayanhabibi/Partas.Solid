---
title: Lucide
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

`Partas.Solid.Lucide` binds [Lucide](https://lucide.dev/guide/) icons. See its docs for usage.

## Usage

Every standard icon is a component in the `Partas.Solid.Lucide` namespace. For Lucide Lab icons, use the `Icon`
component and pass the icon node:

```fsharp
open Partas.Solid.Lucide

Check() // a standard icon

// Icon component for Lucide Lab icons
Icon(iconNode = LucideLab.yinYang)
```

The binding is generated from the React library, with a generator written for Feliz. The Solid library is organised less
cleanly, but the icon names are the same in both.

## Without a binding: vanilla `lucide` on Solid 2

Until the binding is ported, you can use the framework-free [`lucide`](https://lucide.dev/guide/packages/lucide) package
directly. Each icon is a named export holding an icon node, a plain array that describes the SVG. `createElement` turns a
node into an `<svg>` element, and takes optional attributes such as `width` or `stroke-width`. Bind both with
`[<Import>]`, and import only the icons you use: the `icons` export holds the whole set and defeats tree shaking.

```fsharp solid
type IconNode = interface end

[<Import("createElement", "lucide")>]
let createLucideIcon (node: IconNode, attrs: obj) : Browser.Types.Element = jsNative

module LucideIcons =
    [<Import("Heart", "lucide")>]
    let heart: IconNode = jsNative

    [<Import("Star", "lucide")>]
    let star: IconNode = jsNative

    [<Import("Zap", "lucide")>]
    let zap: IconNode = jsNative

    [<Import("Sparkles", "lucide")>]
    let sparkles: IconNode = jsNative

    [<Import("Rocket", "lucide")>]
    let rocket: IconNode = jsNative

    [<Import("Coffee", "lucide")>]
    let coffee: IconNode = jsNative
```

`createElement` returns a DOM node that Solid does not know about, so give it a place to live with a ref. A ref
callback is enough when the icon never changes, as in the buttons below. The large preview changes with the selection,
so a split effect reads the signals in its compute function and swaps the `<svg>` in its effect function. Pick an icon, a colour and a stroke width below. Colour needs
no effect at all: Lucide strokes with `currentColor`, so the wrapper's CSS `color` sets it.

```fsharp solid setup
// Inline styles for the demo. They only use the site's CSS variables.
let lucideCard =
    "display: flex; align-items: center; gap: 1.25rem; padding: 1rem; border: 1px solid var(--nacara-border);"
    + " border-radius: var(--nacara-radius); background: var(--nacara-bg-subtle)"

let lucideStage (colour: string) =
    $"color: {colour}; display: grid; place-items: center; width: 7rem; height: 7rem; flex: none;"
    + " border-radius: var(--nacara-radius-sm); background: var(--nacara-bg); border: 1px solid var(--nacara-border)"

let lucideSwatch (swatch: string) (selected: bool) =
    let ring = if selected then $"0 0 0 2px {swatch}" else "0 0 0 1px var(--nacara-border)"
    $"width: 1.5rem; height: 1.5rem; padding: 0; cursor: pointer; border-radius: 999px; background: {swatch};"
    + $" border: 2px solid var(--nacara-bg); box-shadow: {ring}"

let lucideButtonClass (selected: bool) =
    if selected then "p-btn p-btn--accent" else "p-btn p-btn--secondary"

let lucideIconButton = "width: 2.5rem; padding: 0; justify-content: center"
```

```fsharp solid render=LucidePicker jsx
let lucideChoices =
    [| "Heart", LucideIcons.heart
       "Star", LucideIcons.star
       "Zap", LucideIcons.zap
       "Sparkles", LucideIcons.sparkles
       "Rocket", LucideIcons.rocket
       "Coffee", LucideIcons.coffee |]

let lucideColours =
    [| "var(--nacara-primary)"; "#f28b5b"; "#b845fc"; "#1d8fe0"; "#16a34a" |]

[<SolidComponent>]
let LucidePicker () =
    let picked, setPicked = createSignal lucideChoices[0]
    let colour, setColour = createSignal lucideColours[0]
    let stroke, setStroke = createSignal 2.0
    let mutable stage: Browser.Types.HTMLDivElement = JS.undefined

    // Compute reads the signals; the effect swaps the <svg> whenever they change.
    createEffect (
        (fun (_: (IconNode * float) option) -> snd (picked ()), stroke ()),
        fun (node: IconNode, width: float) ->
            let attrs = createObj [ "width" ==> 80; "height" ==> 80; "stroke-width" ==> width ]
            let svg = createLucideIcon (node, attrs)
            stage?replaceChildren (svg) |> ignore
    )

    div (style = "display: grid; gap: 1rem; width: min(100%, 30rem)") {
        div (style = lucideCard) {
            div(style = lucideStage (colour ())).ref (stage)

            div (style = "display: grid; gap: .75rem") {
                code () { $"{fst (picked ())}, stroke-width {stroke ()}" }

                div (style = "display: flex; gap: .5rem") {
                    For.Keyed(each = lucideColours) {
                        yield fun swatch _ ->
                            button(
                                title = swatch,
                                style = lucideSwatch swatch (swatch = colour ()),
                                onClick = fun _ -> setColour swatch
                            )
                                .attr ("aria-pressed", string (swatch = colour ()))
                    }
                }

                input (
                    type' = "range",
                    min = 1,
                    max = 3,
                    step = "0.25",
                    value = string (stroke ()),
                    onInput = fun e -> setStroke (!!e.currentTarget?valueAsNumber)
                )
            }
        }

        div (style = "display: flex; flex-wrap: wrap; justify-content: center; gap: .5rem") {
            For.Keyed(each = lucideChoices) {
                yield fun (name, node) _ ->
                    // This icon never changes, so a ref callback is all it needs.
                    button(
                        class' = lucideButtonClass (name = fst (picked ())),
                        title = name,
                        style = lucideIconButton,
                        onClick = fun _ -> setPicked (name, node)
                    )
                        .ref (fun (el: Browser.Types.HTMLButtonElement) ->
                            let icon = createLucideIcon (node, {| width = 18; height = 18 |})
                            el.appendChild icon |> ignore)
            }
        }
    }
```
