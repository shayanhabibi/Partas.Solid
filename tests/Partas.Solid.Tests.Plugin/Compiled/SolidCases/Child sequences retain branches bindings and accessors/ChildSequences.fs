module Partas.Solid.Tests.ChildSequences

open Partas.Solid

type Kind =
    | Empty
    | Text of string
    | Pair of int * string

[<SolidComponent>]
let Payload (kind: Kind) =
    div () {
        match kind with
        | Text text -> b () { text.Length }
        | Pair (number, text) ->
            span () {
                number
                text
            }
        | Empty -> "empty"
    }

[<SolidComponent>]
let Branches (flag: bool) (log: string -> unit) =
    div () {
        if flag then
            log "before"
            yield span () { "one" }
            log "between"
            yield b () { "two" }
            log "after"
        else
            "one"
            "two"
    }

[<SolidComponent>]
let Binding (text: string) =
    div () {
        "head"
        let upper = text.ToUpper ()
        span () { upper }
        "middle"
        b () { text }
        "tail"
    }

[<SolidComponent>]
let Matched (kind: Kind) =
    div () {
        match kind with
        | Text text when text.Length > 2 ->
            let upper = text.ToUpper ()
            b () { upper }
            i () { text }
        | _ ->
            "no"
            span () { "payload" }
    }

[<SolidComponent>]
let AttributeArray (flag: bool) =
    div (
        title =
            String.concat
                ":"
                [| yield "head"
                   if flag then
                       yield "one"
                       yield "two"
                   yield "tail" |]
    ) {
        "body"
    }
