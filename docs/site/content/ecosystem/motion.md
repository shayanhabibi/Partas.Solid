---
title: Motion
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

`Partas.Solid.Motion` binds the [solid-motionone](https://github.com/solidjs-community/solid-motionone) library, and the
`animate` function from `motion`.

## solid-motionone

See [the docs](https://github.com/solidjs-community/solid-motionone) for usage.

`solid-motionone` uses different prop names from `motion/react`, so keep that in mind when you port an example. For
instance, keyframes in `transition` use `offset`, not `times`. See the keyframes example below.

### Differences

Each Motion-specific prop has two forms:

- The plain name, such as `initial`, takes a Feliz-style list of props, with type checking and completion for every CSS
  style prop.
- The `JSX`-suffixed name, such as `initialJSX`, takes a string and injects the JSX language, so you write the value as
  JSX.

For the CSS props such as `animate` and `initial`, use the `MotionStyle` qualifier to get at the props, Feliz style.

For `transition`, use the `MotionTransition` qualifier. It enforces the `AnimationOptions` Pojo when you set a CSS
property inside the transition.

## motion

See [the docs](https://motion.dev/docs/animate) for usage. Only the `animate` function is bound.

## Example

A component that cycles through words:

```fsharp
[<Erase>]
type WordRotate() =
    inherit div()
    interface OptionKeys
    [<Erase>] member val words: string[] = unbox null with get,set
    [<Erase>] member val duration: int = unbox null with get,set
    [<SolidTypeComponent>]
    member props.constructor =
        props.duration <- 2500 // set a default duration if none is set
        let index,setIndex = createSignal(0)
        createEffect(
            fun () ->
                let interval = setInterval (fun () -> setIndex.Invoke(fun prevIndex -> (prevIndex + 1) % (props.words.Length))) props.duration
                onCleanup(fun () -> clearInterval(interval))
        )
        Presence(exitBeforeEnter = true) {
            Show(when' = !!(index() + 1), keyed = true) {
                Motion(
                    initial = [
                        MotionStyle.opacity 0
                        MotionStyle.y -50
                    ],
                    animate = [
                        MotionStyle.opacity 1
                        MotionStyle.y 0
                    ],
                    exit = [
                        MotionStyle.opacity 0
                        MotionStyle.y 50
                    ],
                    transition = [
                        MotionTransition.duration 0.25
                        MotionTransition.easing Easing.EaseOut
                    ]
                ).spread props {
                    props.words[index()]
                }
            }
        }
```

```fsharp
[<SolidComponent>]
let WordRotateExample () =
    div() {
        WordRotate(style = "font-size: 4rem",
                   words = [| "Word" ; "Rotate" |])
    }
```

The word fades and slides out downwards, and the next one fades in from above, every 2.5 seconds.

:::warning
This example is written for Partas.Solid 2.x. In 3.0 the single-callback `createEffect` is gone. Solid 2 splits an
effect into a tracked compute function and an untracked effect function, and the effect function returns its cleanup
instead of calling `onCleanup`. The `OptionKeys` interface used above does not exist in Partas.Solid 3.0 either;
drop it. The 3.0 effect looks like this:

```fsharp
createEffect(
    (fun _ -> props.duration),
    fun (duration: int) ->
        let interval =
            setInterval (fun () -> setIndex.Invoke(fun prevIndex -> (prevIndex + 1) % props.words.Length)) duration
        fun () -> clearInterval interval
)
```

See [Migrating to Solid 2](../guide/migrating-to-solid-2.md).
:::

## Keyframes

```fsharp
[<SolidComponent>]
let KeyframeExample () =
    let inline percent value = $"{value}%%"
    Motion.div(
        animate = [
            MotionStyle.scale !^[|1;2;2;1;1|]
            MotionStyle.rotate !^[|0;0;180;180;0|]
            MotionStyle.borderRadius !^[|percent 0; percent 0; percent 50; percent 50; percent 0|]
        ],
        transition = [
            MotionTransition.duration 2
            MotionTransition.easing Easing.EaseInOut
            MotionTransition.offset [|0.;0.2;0.5;0.8;1|]
            MotionTransition.repeat JS.Infinity
            MotionTransition.endDelay 1.
        ],
        style="width: 100px; height: 100px; background-color: green"
        )
```

A green square scales up, turns, rounds into a circle, and returns, on repeat.

## Computation expression helpers

In Partas.Solid 2.x, `createEffect` and the other functions that take a void function could also be written with the
experimental computation expressions:

::::tabs
:::tab Normal
```fsharp
createEffect(
    fun () ->
        let interval =
            setInterval (fun () -> setIndex.Invoke(fun prevIndex -> (prevIndex + 1) % (props.words.Length))) props.duration
        onCleanup(fun () -> clearInterval(interval))
)
```
:::
:::tab open Partas.Solid.Experimental
```fsharp
effect {
    let interval = setInterval (fun _ -> setIndex.Invoke(fun prevIndex -> (prevIndex + 1) % props.words.Length)) props.duration
    cleanup { clearInterval(interval) }
}
```
:::
::::

In 3.0 the `effect { }` builder targets the two-phase `createEffect` and needs exactly one `let!` for the tracked value.
See [Experimental](../guide/experimental.md).

## Language injection

The `JSX`-suffixed props give you JSX language injection in JetBrains IDEs:

```fsharp
Presence(exitBeforeEnter = true) {
    Show(when' = !!(index() + 1), keyed = true) {
        Motion(
            initialJSX = "{ opacity: 0, y: -50 }",
            animateJSX = "{ opacity: 1, y: 0 }",
            exitJSX = "{ opacity: 0, y: 50 }",
            transitionJSX = "{ duration: 0.25, easing: 'ease-out' }"
        ).spread props {
            props.words[index()]
        }
    }
}
```

:::note
The injection only worked with a project reference, not with the NuGet package. The issue was reported to JetBrains
and was due to be fixed in a 2025 Rider release. You can tell it works when the IDE suggests values for string-enum HTML
attributes such as `dir`.
:::
