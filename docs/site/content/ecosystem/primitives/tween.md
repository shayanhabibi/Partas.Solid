---
title: Tween
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/tween`.

## Easing

```fsharp
type Easing = float -> float
```

## createTween

```fsharp
let createTween(
    target: Accessor<'T>,
    ?duration: int,
    ?easing: Easing
    ): Accessor<'T>
```

Creates a derived signal that moves smoothly from the target's previous value to its next value whenever it changes.

The target can be any reactive value: a signal, a memo, or a function that reads them. To tween a component prop,
pass `fun () -> props.value`.

| Param | Desc |
| --- | --- |
| `target` | The reactive value to follow. |
| `duration` | How many milliseconds the transition from one value to the next takes. Defaults to `100`. |
| `easing` | A function that maps a number between `0.` and `1.`, to speed up or slow down parts of the transition. Defaults to linear. |

`createTween` uses `requestAnimationFrame` to update the value at the display's refresh rate. Once the tweened value
reaches the target, it stops requesting frames.

## Easings

The binding ships a set of easing functions written in F#, with no dependencies:

| In | Out | In and out |
| --- | --- | --- |
| `easeInSine` | `easeOutSine` | `easeInOutSine` |
| `easeInQuad` | `easeOutQuad` | `easeInOutQuad` |
| `easeInCubic` | `easeOutCubic` | `easeInOutCubic` |
| `easeInQuart` | `easeOutQuart` | `easeInOutQuart` |
| `easeInQuint` | `easeOutQuint` | `easeInOutQuint` |
| `easeInExpo` | `easeOutExpo` | `easeInOutExpo` |
| `easeInCirc` | `easeOutCirc` | `easeInOutCirc` |
| `easeInBack` | `easeOutBack` | `easeInOutBack` |
| `easeInElastic` | `easeOutElastic` | `easeInOutElastic` |
| `easeInBounce` | `easeOutBounce` | `easeInOutBounce` |
