module Partas.Solid.Tests.IssueCases.CharArrayMapping

open Partas.Solid
open Fable.Core

[<Erase>]
type WordRotate() =
    inherit div()

    [<Erase>]
    member val words: string = unbox null with get, set

    [<SolidTypeComponentAttribute>]
    member props.constructor =
        For (
            each =
                (props.words.ToCharArray ()
                 |> Array.map string)
        )
