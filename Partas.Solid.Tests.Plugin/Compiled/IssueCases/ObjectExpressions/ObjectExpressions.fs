module Partas.Solid.Tests.IssueCases.ObjectExpressions

open Partas.Solid
open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop

[<AllowNullLiteral; Interface>]
type AbstractInterface =
    abstract member delay: int with get, set

[<Erase>]
type WordRotate() =
    inherit div()

    [<Erase>]
    member val words: string = unbox null with get, set

    [<Erase>]
    member val duration: int = unbox null with get, set

    [<SolidTypeComponentAttribute(ComponentFlag.DebugMode)>]
    member props.constructor =
        let index, setIndex = createSignal (0)

        let transition =
            jsOptions<AbstractInterface> (fun o ->
                o.delay <-
                    props.duration
                    * index ())

        For (each = props.words.Split (' '))
