module Partas.Solid.Tests

open Fable.Core

[<SolidComponent(ComponentFlag.DebugMode)>]
let x () =
    Dynamic<button> (componentAsString = "div") {
        For (
            each = [| 1; 2; 3 |],
            fallback =
                (Decorators.once
                 div ())
        )

        Show (when' = true) { "flesh" }
        Show<_> (when' = "stringger") { yield fun o -> Fragment () { o } }
    }
