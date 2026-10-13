---
title: Tween
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/tween`.

```fsharp
type Easing = float -> float

let createTween(target: Accessor<'T>, ?duration: int, ?easing: Easing): Accessor<'T>
```

A derived signal that moves smoothly from the target's previous value to its new one whenever it changes. The target
can be any accessor; to tween a component prop, pass `fun () -> props.value`. `duration` is in ms and defaults to
`100`. `easing` maps progress from `0.` to `1.`, to speed up or slow down parts of the transition, and defaults to
linear. Updates run on `requestAnimationFrame`, and stop once the value reaches the target.

## Easings

The binding ships these easing functions, written in F# with no dependencies:

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

## On Solid 2 today

`createTween` is an effect that watches the target and steps a signal towards it once per frame. Written out in plain
Partas.Solid:

```fsharp solid
[<Global>]
let requestAnimationFrame (callback: float -> unit) : int = jsNative

[<Global>]
let cancelAnimationFrame (handle: int) : unit = jsNative

let tween (target: unit -> float) (duration: float) (easing: float -> float) =
    let value, setValue = createSignal (target ())
    createEffect (
        (fun (_: float option) -> target ()),
        fun (goal: float) ->
            let from = value ()
            let mutable start = -1.
            let mutable handle = 0
            let rec step (now: float) =
                if start < 0. then start <- now
                let t = min 1. ((now - start) / duration)
                setValue (from + (goal - from) * easing t)
                if t < 1. then handle <- requestAnimationFrame step
            handle <- requestAnimationFrame step
            fun () -> cancelAnimationFrame handle
    )
    value

let linear (t: float) = t
let easeOutCubic (t: float) = 1. - (1. - t) ** 3.
let easeOutBack (t: float) = 1. + 2.70158 * (t - 1.) ** 3. + 1.70158 * (t - 1.) ** 2.
```

Each row below tweens the same target over 800 ms with a different easing. Press **Move**:

```fsharp solid render=TweenDemo jsx
[<SolidComponent>]
let TweenRow (name: string) (easing: float -> float) (target: unit -> float) =
    let x = tween target 800. easing
    div (style = "display: grid; grid-template-columns: 7.5rem 1fr; align-items: center; gap: .75rem") {
        code () { name }
        div (style = "position: relative; height: 1rem; border-radius: 999px; background: var(--nacara-bg-subtle)") {
            div (
                style =
                    "position: absolute; top: 0; width: 1rem; height: 1rem; border-radius: 999px; background: var(--nacara-primary); left: calc((100% - 1rem) * "
                    + string (x () / 100.)
                    + ")"
            )
        }
    }

[<SolidComponent>]
let TweenDemo () =
    let target, setTarget = createSignal 0.
    div (style = "display: grid; gap: .6rem; width: 100%") {
        TweenRow "linear" linear target
        TweenRow "easeOutCubic" easeOutCubic target
        TweenRow "easeOutBack" easeOutBack target
        button (
            class' = "p-btn p-btn--secondary",
            style = "justify-self: start",
            onClick = fun _ -> setTarget (if target () = 0. then 100. else 0.)
        ) { "Move" }
    }
```
