module Partas.Solid.Tests.Runtime.Integration.Smoke.AsyncLoading

open Partas.Solid
open Fable.Core

/// An async memo read under <Loading>: proves the helpers' settle()/waitFor() drive async resolution.
[<Erase>]
type AsyncGreeting() =
    inherit div()

    [<Erase>]
    member val load: unit -> JS.Promise<string> = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let greeting = createMemo (fun (_: string option) -> props.load ())

        div (class' = "async") {
            Loading(fallback = span (class' = "fallback") { "loading" }) {
                span (class' = "data") { greeting () }
            }
        }
