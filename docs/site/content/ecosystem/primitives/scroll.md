---
title: Scroll
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/scroll`.

## ScrollPosition

```fsharp
type ScrollPosition = interface
```

```fsharp
member x: int
member y: int
```

:::note
These will likely become `float` in a later version of the binding.
:::

## createScrollPosition

```fsharp
let createScrollPosition(
    ?element: #HtmlElement | Accessor<#HtmlElement>
    ): Accessor<ScrollPosition>
```

The target defaults to `window`.

## useWindowScrollPosition

```fsharp
let useWindowScrollPosition(): ScrollPosition
```

Returns a reactive object with the current window scroll position. Its signals and event listeners are shared between
everything that uses it, so it is cheaper to use in many places at once.

## getScrollPosition

```fsharp
let getScrollPosition(): ScrollPosition
```

Gets the current `ScrollPosition`.
