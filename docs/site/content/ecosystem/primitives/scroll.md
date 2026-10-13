---
title: Scroll
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/scroll`.

```fsharp
type ScrollPosition =
    member x: int     // will likely become float
    member y: int

let createScrollPosition(?element: #HtmlElement | Accessor<#HtmlElement>): Accessor<ScrollPosition>
let useWindowScrollPosition(): ScrollPosition
let getScrollPosition(): ScrollPosition
```

- `createScrollPosition` tracks an element's scroll position. The target defaults to `window`.
- `useWindowScrollPosition` tracks the window's. Its signals and listeners are shared by every caller, so it is cheap
  to use in many places.
- `getScrollPosition` reads the current position once, without tracking.
