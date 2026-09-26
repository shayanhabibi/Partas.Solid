module Partas.Solid.Tests.Runtime.Integration.FlowAdvanced.ConversionCases

open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop

let private items = [ "a"; "b"; "c" ]

[<SolidComponent>]
let ListToArray () =
    let xs, _ = createSignal items
    ul (class' = "list-to-array") { For.Keyed(each = List.toArray (xs ())) { yield fun x _ -> li () { x } } }

[<SolidComponent>]
let ArrayOfList () =
    let xs, _ = createSignal items
    ul (class' = "array-of-list") { For.Keyed(each = Array.ofList (xs ())) { yield fun x _ -> li () { x } } }

[<SolidComponent>]
let SeqToArray () =
    let xs, _ = createSignal items
    ul (class' = "seq-to-array") { For.Keyed(each = Seq.toArray (xs ())) { yield fun x _ -> li () { x } } }

[<SolidComponent>]
let PipedToArray () =
    let xs, _ = createSignal items
    ul (class' = "piped-to-array") { For.Keyed(each = (xs () |> List.toArray)) { yield fun x _ -> li () { x } } }

[<SolidComponent>]
let ListToArrayLiteral () =
    ul (class' = "literal") { For.Keyed(each = List.toArray [ "a"; "b"; "c" ]) { yield fun x _ -> li () { x } } }

[<SolidComponent>]
let ArrayMapped () =
    let xs, _ = createSignal [| "a"; "b"; "c" |]
    ul (class' = "array-map") { For.Keyed(each = Array.map id (xs ())) { yield fun x _ -> li () { x } } }

[<SolidComponent>]
let ListMappedToArray () =
    let xs, _ = createSignal items
    ul (class' = "list-map") { For.Keyed(each = (xs () |> List.map id |> List.toArray)) { yield fun x _ -> li () { x } } }

[<SolidComponent>]
let ToArrayInLet () =
    let xs, _ = createSignal items
    let arr () = List.toArray (xs ())
    ul (class' = "in-let") { For.Keyed(each = arr ()) { yield fun x _ -> li () { x } } }
