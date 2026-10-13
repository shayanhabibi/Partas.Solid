---
title: Mouse
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/mouse`.

## Types

All are POJOs. The positions and sizes are typed `int`; they should be `float`.

| Type | Fields |
| --- | --- |
| `Position` | `x`, `y` |
| `MousePosition` | `x`, `y`, `sourceType: MouseSourceType option` |
| `MousePositionInside` | `x`, `y`, `sourceType: MouseSourceType`, `isInside: bool` |
| `PositionRelativeToElement` | `x`, `y`, `isInside`, `top`, `left`, `width`, `height` |

`MouseSourceType` is a `StringEnum`: `Mouse` or `Touch`.

## Bindings

| Member | Desc |
| --- | --- |
| `makeMousePositionListener` | Listens for changes to the mouse or touch position on the target. Returns a `DisposeCallback`. |
| `makeMouseInsideListener` | Listens for the mouse or touch entering or leaving the target. Returns a `DisposeCallback`. |
| `getPositionToElement` | Turns a page position into a position relative to an element. |
| `getPositionInElement` | Like `getPositionToElement`, but clamped to the element's bounds. |
| `getPositionToScreen` | Turns a page position into a screen position. |
| `createMousePosition` | A reactive `MousePositionInside` on the page. The target defaults to `window`, and can be an accessor. |
| `useMousePosition` | `createMousePosition` on `window`, as a singleton root shared by every caller. |
| `createPositionToElement` | A position relative to an element, updated from an accessor of a page position, plus the element's bounds. |

| Param | Desc |
| --- | --- |
| `target` | `SVGSVGElement`, `HTMLElement`, `Window` or `Document`. |
| `touch` | Listen to touch events, updating on `touchstart`. |
| `followTouch` | Update on `touchmove`. |
| `initialValues` | The initial values. |

:::caution
The `createMousePosition` overload that takes an element, not an accessor, has no `ParamObject`, so `touch` and
`followTouch` are passed as positional arguments rather than as an options object.
:::

## On Solid 2 today

`createMousePosition` on an element is a store of `x`, `y` and `isInside`, kept up to date by pointer events. In plain
Partas.Solid it is a signal written from `onPointerMove`, whose handler is typed `Dom.PointerEvent -> unit`:

```fsharp solid render=PointerReadout jsx
[<SolidComponent>]
let PointerReadout () =
    let x, setX = createSignal 0.
    let y, setY = createSignal 0.
    let inside, setInside = createSignal false

    div (
        style = "position: relative; height: 10rem; border: 1px dashed var(--nacara-border); border-radius: var(--nacara-radius-sm); cursor: crosshair; overflow: hidden",
        onPointerMove = (fun e -> setX e.offsetX; setY e.offsetY),
        onPointerEnter = (fun _ -> setInside true),
        onPointerLeave = fun _ -> setInside false
    ) {
        code (style = "position: absolute; top: .5rem; left: .5rem; pointer-events: none") {
            $"x {int (x ())} · y {int (y ())} · isInside {inside ()}"
        }
        div (
            style =
                $"position: absolute; width: .75rem; height: .75rem; border-radius: 999px; background: var(--nacara-primary); pointer-events: none; transform: translate({x () - 6.}px, {y () - 6.}px); opacity: {if inside () then 1 else 0}"
        )
    }
```
