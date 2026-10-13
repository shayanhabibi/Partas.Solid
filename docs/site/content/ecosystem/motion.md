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

Keyframes in `transition` use `offset`, not `motion/react`'s `times` (see [Keyframes](#keyframes)). Each Motion-specific
prop has two forms:

- The plain name, such as `initial`, takes a Feliz-style list of props. Build it with the `MotionStyle` qualifier for
  CSS props such as `animate` and `initial`, and `MotionTransition` for `transition`, which enforces the
  `AnimationOptions` Pojo when you set a CSS property inside it.
- The `JSX`-suffixed name, such as `initialJSX`, takes the value as a JSX string (see
  [Language injection](#language-injection)).

From `motion`, only [`animate`](https://motion.dev/docs/animate) is bound.

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

This example is written for Partas.Solid 2.x: its single-callback `createEffect` and `OptionKeys` are gone in 3.0. The
live example below shows the Solid 2 effect.

## On Solid 2 today

Until the bindings are ported, import the framework-free `motion-dom`, the DOM engine underneath Motion. Its
`animateElement` takes an element, a keyframes object and a transition object. The tupled parameters make Fable emit a
normal call, and anonymous records compile to the plain objects Motion expects.

```fsharp solid jsx render=WordRotateDemo
open Fable.Core.TS
type PlaybackControls =
    abstract stop: unit -> unit

[<Import("animateElement", "motion-dom")>]
let animateElement (element: obj, keyframes: obj, transition: obj): PlaybackControls array =
    jsNative

[<SolidComponent>]
let WordRotateDemo () =
    let words = [| "typed"; "reactive"; "fine-grained"; "just F#" |]
    let index, setIndex = createSignal 0
    let mutable word: Dom.HTMLSpanElement = JS.undefined

    onSettled (fun () ->
        let id = JS.setInterval (fun () -> setIndex ((index () + 1) % words.Length)) 2000
        fun () -> JS.clearInterval id)

    createEffect (
        (fun (_: int option) -> index ()),
        fun (_: int) ->
            let running =
                animateElement (
                    word,
                    {| opacity = ResizeArray [ 0.; 1. ]
                       y = ResizeArray [ 14; 0 ]
                       filter = ResizeArray [ "blur(6px)"; "blur(0px)" ] |},
                    {| duration = 0.45; ease = ResizeArray [ 0.22; 1.; 0.36; 1. ] |}
                )
            fun () -> running |> Array.iter (fun a -> a.stop ())
    )

    div (style = "font-size: 1.75rem; font-weight: 600; color: var(--nacara-heading)") {
        "Partas.Solid is "
        span(style = "display: inline-block; color: var(--nacara-primary)").ref (word) {
            words[index ()]
        }
    }
```

The effect tracks `index` and replays the enter animation each time the word changes, returning a cleanup that stops
the playback controls `animateElement` returns. Return a real cleanup: Solid 2 calls whatever the effect function
returns, and `animateElement (...) |> ignore` still compiles to an arrow that returns the controls array, which then
fails as "not a function".

The keyframes use `ResizeArray`, because this site compiles its examples without `--typedArrays false`, and Motion only
treats a real JS array as a list of keyframes or a cubic-bezier `ease`. With the flag, as
[installation](../guide/installation.md#compiling) recommends, `[| ... |]` works too.

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
The injection may only work with a project reference, not with the NuGet package, on older Rider releases. It works
when the IDE suggests values for string-enum HTML attributes such as `dir`.
:::
