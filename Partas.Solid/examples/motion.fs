module Partas.Solid.examples.motion
open Partas.Solid
open Partas.Solid.Style
open Partas.Solid.Motion
open Fable.Core.JsInterop
open Fable.Core
open Fable.Core.JS

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

[<SolidComponent>]
let WordRotateExample () =
    div() {
        WordRotate(style = "font-size: 4rem",
                   words = [| "Word" ; "Rotate" |])
    }
    
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
