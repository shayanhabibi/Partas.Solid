module Partas.Solid.Tests.SolidCases.SignalSetterInvoke

open Partas.Solid
open Fable.Core
open Fable.Core.JS

[<Erase>]
type WordRotate() =
    inherit div()
    [<Erase>] member val words: string[] = unbox null with get,set
    [<Erase>] member val duration: int = unbox null with get,set
    [<SolidTypeComponentAttribute>]
    member props.constructor =
        let index,setIndex = createSignal(0)
        createEffect(
            fun () ->
                let interval = setInterval (fun () -> setIndex.Invoke(fun prevIndex -> (prevIndex + 1) % (props.words.Length))) props.duration
                onCleanup(fun () -> clearInterval(interval))
        )
        div()