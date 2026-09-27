module Partas.Solid.Tests.SolidCases.ThunkArguments

open Partas.Solid

[<SolidComponent>]
let ThunkArguments (value: Accessor<string>) =
    div (class' = (if isPending (fun () -> box (value ())) then "pending" else "idle")) {
        Show(when' = isPending (fun () -> box (value ()))) { span () { "refreshing" } }
        span (title = latest (fun () -> value ())) { value () }
    }
