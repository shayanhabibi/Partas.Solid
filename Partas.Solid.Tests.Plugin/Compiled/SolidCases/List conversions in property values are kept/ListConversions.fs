module Partas.Solid.Tests.SolidCases.ListConversions

open Partas.Solid

[<SolidComponent>]
let ListToArray () =
    let xs, _ = createSignal [ "a"; "b" ]
    For.Keyed(each = List.toArray (xs ())) { yield fun x _ -> li () { x } }

[<SolidComponent>]
let ArrayCollection () =
    For.Keyed(each = [| "a"; "b" |]) { yield fun x _ -> li () { x } }
