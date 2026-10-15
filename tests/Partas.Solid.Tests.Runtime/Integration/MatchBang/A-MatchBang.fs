module Partas.Solid.Tests.Runtime.Integration.MatchBang.Fixtures

open Partas.Solid
open Fable.Core

type State =
    | Loading
    | Loaded of string
    | Failed of int * string

[<SolidComponent>]
let View (state: unit -> State) (mounted: string -> unit) =
    div () {
        span (class' = "before") { "before" }

        match! state () with
        | Loading -> p (class' = "loading") { "Loading..." }
        | Loaded name when name.Length > 2 ->
            mounted name
            input (class' = "name", value = name)
            span (class' = "mirror") { name.ToUpper () }
        | Loaded name -> b (class' = "short") { name }
        | Failed (code, message) ->
            b (class' = "code") { code }
            i (class' = "message") { message }

        span (class' = "after") { "after" }
    }

[<SolidComponent>]
let BooleanMatch (flag: unit -> bool) =
    div () {
        match! flag () with
        | true -> b () { "yes" }
        | false -> i () { "no" }
    }

[<SolidComponent>]
let Falsy (value: unit -> int) =
    div () {
        match! value () with
        | 0 -> span () { "zero" }
        | number -> b () { number }
    }

[<SolidComponent>]
let Nested (state: unit -> State) (flag: unit -> bool) =
    div () {
        match! state () with
        | Loaded name ->
            section () {
                match! flag () with
                | true -> span () { name }
                | false -> b () { name }
            }
        | _ -> "empty"
    }

[<SolidComponent>]
let Tuples (value: unit -> int * string) =
    div () {
        match! value () with
        | 0, text -> span () { text }
        | number, text ->
            b () {
                number
                text
            }
    }

let (|Positive|_|) number =
    if number > 0 then Some (string number) else None

[<SolidComponent>]
let ActivePattern (value: unit -> int) =
    div () {
        match! value () with
        | Positive text -> span () { text }
        | _ -> b () { "non-positive" }
    }

[<SolidComponent>]
let Callbacks (state: unit -> State) (report: string -> unit) =
    div () {
        match! state () with
        | Loaded name -> button (onClick = fun _ -> report name) { name }
        | _ -> "empty"
    }

[<SolidComponent>]
let SharedTarget (value: unit -> int * string) =
    div () {
        match! value () with
        | (1 | 2), text -> span () { text }
        | _, text -> b () { text }
    }

[<Erase>]
type GuardedProps() =
    inherit div()

    [<Erase>]
    member val readState: unit -> State = fun () -> Loading with get, set

    [<Erase>]
    member val enabled: bool = false with get, set

    [<SolidTypeComponent>]
    member props.View =
        div () {
            match! props.readState () with
            | Loaded name when props.enabled -> span () { name }
            | Loaded name -> b () { name }
            | _ -> "empty"
        }

[<SolidComponent>]
let TwoMatches (first: unit -> bool) (second: unit -> bool) =
    div () {
        match! first () with
        | true -> span () { "first" }
        | false -> span () { "none" }

        match! second () with
        | true -> b () { "second" }
        | false -> b () { "none" }
    }

type ArrayState =
    | NoItems
    | Items of string array

[<SolidComponent>]
let IndexedBinding (state: unit -> ArrayState) (nextIndex: unit -> int) =
    div () {
        match! state () with
        | Items items ->
            let picked: string = Fable.Core.JsInterop.(?) items (nextIndex ())
            span () { picked }
            b () { picked }
        | NoItems -> "empty"
    }

let (|PositiveValue|OtherValue|) value =
    if value > 0 then PositiveValue value else OtherValue

[<SolidComponent>]
let TotalActivePattern (value: unit -> int) =
    div () {
        match! value () with
        | PositiveValue number -> span () { number }
        | OtherValue -> b () { "none" }
    }
