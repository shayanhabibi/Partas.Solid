---
title: Bounds
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/bounds`.

```fsharp
let createElementBounds(
    target: #HTMLElement | Accessor<#HTMLElement>,
    ?trackScroll: bool,
    ?trackMutation: bool,
    ?trackResize: bool
    ): ElementBounds
```

Returns a reactive, store-like object with the element's position on the screen and its size. All three kinds of
tracking are on by default; set the ones you don't need to `false`.

| Parameter | Listens to |
| --- | --- |
| `trackScroll` | Window scroll events |
| `trackMutation` | Changes to the DOM structure and styles |
| `trackResize` | The element's resize events |

## ElementBounds

An interface with `width`, `height`, `top`, `left`, `right` and `bottom`, each an `int`.
