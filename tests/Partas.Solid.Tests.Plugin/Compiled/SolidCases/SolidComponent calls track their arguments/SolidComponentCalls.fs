module Partas.Solid.Tests.SolidCases.SolidComponentCalls

open Partas.Solid
open Fable.Core

[<SolidComponent>]
let NoArgs () = span () { "no args" }

[<SolidComponent>]
let Label (title: string) (count: int) = span () { title + string count }

/// The body runs untracked; `count ()` is read in the caller, so a change re-runs the call.
[<SolidComponent>]
let Host () =
    let count, setCount = createSignal 0

    div () {
        NoArgs ()
        Label "static" 1
        Label "live" (count ())
        button (onClick = fun _ -> setCount (count () + 1)) { "+" }
    }
