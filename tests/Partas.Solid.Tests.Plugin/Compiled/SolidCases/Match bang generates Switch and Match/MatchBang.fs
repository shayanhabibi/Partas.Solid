module Partas.Solid.Tests.MatchBang

open Partas.Solid

type State =
    | Waiting
    | Ready of string
    | Failed of int * string

[<SolidComponent>]
let View (state: unit -> State) (expanded: unit -> bool) =
    let heading = "Status"

    div () {
        h1 () { heading }

        match! state () with
        | Waiting -> span () { "Waiting" }
        | Ready text when text.Length > 2 ->
            section () {
                match! expanded () with
                | true -> strong () { text }
                | false -> i () { text }
            }
        | Ready text -> b () { text }
        | Failed (code, message) ->
            span () { code }
            span () { message }
    }

[<SolidComponent>]
let OrdinaryMatch (state: unit -> State) =
    div () {
        match state () with
        | Waiting -> span () { "Waiting" }
        | Ready text -> b () { text }
        | Failed (code, message) ->
            span () { code }
            span () { message }
    }
