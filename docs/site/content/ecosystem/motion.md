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

## Live example on Solid 2

The bindings above are not ported yet, but you can still animate from Partas.Solid 3.0 by importing a framework-free
library. `motion-dom` is the DOM engine underneath Motion. Its `animateElement` takes an element, a keyframes object and
a transition object. `[<Import>]` binds it as a plain function. The parameters are tupled so Fable emits a normal
call, and anonymous records compile to the plain objects Motion expects.

```fsharp solid jsx render=WordRotateDemo
type PlaybackControls =
    abstract stop: unit -> unit

[<Import("animateElement", "motion-dom")>]
let animateElement (element: obj, keyframes: obj, transition: obj): PlaybackControls array =
    jsNative

[<SolidComponent>]
let WordRotateDemo () =
    let words = [| "typed"; "reactive"; "fine-grained"; "just F#" |]
    let index, setIndex = createSignal 0
    let mutable word: Browser.Types.HTMLSpanElement = JS.undefined

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

The interval starts in `onSettled` and its cleanup is returned from the same lambda. The effect tracks `index` and
replays the enter animation on the `span` each time the word changes. `animateElement` returns one set of playback
controls per animated value, and the effect returns a cleanup that stops them. Return a real cleanup here. Solid 2 calls
whatever the effect function returns, and `animateElement (...) |> ignore` still compiles to an arrow that returns the
controls array, which then fails as "not a function". `y` is one of Motion's transform shorthands, so it
animates `translateY` without touching the layout.

The keyframes use `ResizeArray` rather than `[| ... |]`. Fable compiles an F# `float[]` or `int[]` to a typed array such
as `Float64Array`, and Motion only treats a real JS array as a list of keyframes or a cubic-bezier `ease`. A
`ResizeArray` compiles to a plain array.

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
