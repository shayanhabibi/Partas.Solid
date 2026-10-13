module Partas.Solid.Tests.Runtime.Integration.Smoke.Counter

open Partas.Solid
open Fable.Core

[<Erase>]
type Counter() =
    inherit div()

    [<Erase>]
    member val initial: int = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let count, setCount = createSignal props.initial

        div (class' = "counter") {
            span (class' = "value") { count () }
            button (class' = "inc", onClick = fun _ -> setCount (count () + 1)) { "+" }
        }
