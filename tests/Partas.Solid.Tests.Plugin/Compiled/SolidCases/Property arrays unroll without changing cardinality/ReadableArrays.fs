module Partas.Solid.Tests.ReadableArrays

open Partas.Solid

[<SolidComponent>]
let Simple (flag: bool) =
    div (
        title =
            String.concat
                ":"
                [| yield "head"
                   if flag then yield "yes" else yield "no"
                   yield "tail" |]
    ) {
        "body"
    }

[<SolidComponent>]
let Optional (flag: bool) =
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
