module Partas.Solid.Tests.Runtime.Integration.JsxCollection.Collection

open Partas.Solid
open Fable.Core

type Payload =
    | Empty
    | Text of string
    | Pair of int * string

[<SolidComponent>]
let UnionChild (value: Payload) =
    div () {
        match value with
        | Empty -> "empty"
        | Text text ->
            span () {
                b () { text }
                i () { text.Length }
            }
        | Pair (number, text) ->
            span (title = text) {
                number
                text
            }
    }

[<SolidComponent>]
let Accessors (items: string list) (value: string option) (pair: string * string) =
    div () {
        span () { items.Head }
        b () { items.Tail.Head }
        i () { value.Value }

        strong () {
            fst pair
            snd pair
        }
    }

[<SolidComponent>]
let Conditional (flag: unit -> bool) =
    div () {
        "head"

        if flag () then
            span () { "one" }
            b () { "two" }
        else
            i () { "three" }
            strong () { "four" }

        "tail"
    }

[<SolidComponent>]
let BoundChildren (text: unit -> string) =
    div () {
        "head"
        let upper = (text ()).ToUpper ()
        span () { upper }
        "middle"
        b () { text () }
        "tail"
    }

[<SolidComponent>]
let MatchedChildren (value: Payload) =
    div () {
        match value with
        | Text text when text.Length > 2 ->
            let upper = text.ToUpper ()
            span () { upper }
            b () { text }
        | Text text ->
            i () { text }
            "short"
        | _ ->
            "no"
            strong () { "payload" }
    }

[<SolidComponent>]
let NestedChildren (text: string) =
    div () {
        span () {
            let upper = text.ToUpper ()
            b () { upper }
            i () { text }
        }

        "tail"
    }

[<SolidComponent>]
let DiscardedChildren (flag: bool) (log: string -> unit) (produce: unit -> HtmlElement) =
    div () {
        if flag then
            log "before"

            do
                span (
                    title =
                        (log "span"
                         "one")
                ) {
                    "one"
                }
                |> ignore

            log "between"

            do
                produce ()
                |> ignore

            do
                "text"
                |> ignore

            b () { "two" }
            log "after"
        else
            "empty"
    }

[<SolidComponent>]
let TextChildren (flag: bool) =
    div () {
        if flag then
            yield "one"
            yield "two"
        else
            yield "empty"
    }

[<SolidComponent>]
let ArrayProp (flag: bool) (log: string -> string) =
    div (
        title =
            String.concat
                ":"
                [| yield log "head"
                   if flag then
                       yield log "one"
                       yield log "two"
                   yield log "tail" |]
    ) {
        "body"
    }

[<SolidComponent>]
let ArrayCardinality (flag: bool) =
    div (
        title =
            string
                [| if flag then
                       yield "one"
                   yield null |]
                    .Length
    ) {
        "body"
    }

[<SolidComponent>]
let EmptyArray (flag: bool) =
    div (
        title =
            string
                [| if flag then
                       yield "one" |]
                    .Length
    ) {
        "body"
    }

[<SolidComponent>]
let Produced () =
    em () { "produced" }

[<SolidComponent>]
let AccessorValues () =
    Accessors [ "a"; "b" ] (Some "c") ("d", "e")

[<Erase>]
type MatchedArray() =
    inherit div()

    [<Erase>]
    member val label: string = "" with get, set

    [<Erase>]
    member val enabled: bool = false with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (
            title =
                (match props.label with
                 | "skip" -> ""
                 | text ->
                     String.concat
                         ":"
                         [| yield text
                            if props.enabled then
                                yield text |])
        )

[<SolidComponent>]
let MatchedArrayHost (label: string) (enabled: bool) =
    MatchedArray (label = label, enabled = enabled)

[<SolidComponent>]
let RenderFunction (log: string -> unit) =
    div () {
        For.Keyed (each = [| "one"; "two" |]) {
            yield
                fun text _ ->
                    log text
                    span () { text }
                    b () { text }
        }
    }

[<SolidComponent>]
let ScopedArray (flag: bool) (log: string -> string) =
    div (
        title =
            String.concat
                ":"
                [| yield log "head"
                   let text = log "bound"

                   if flag then
                       yield text
                       yield log "branch"

                   yield log "tail" |]
    )

[<SolidComponent>]
let DelayedEffects (flag: bool) (log: string -> string) =
    div (
        title =
            String.concat
                ":"
                [| log "before"
                   |> ignore
                   if flag then yield log "one" else yield log "two"
                   log "between"
                   |> ignore
                   yield log "tail" |]
    )

[<SolidComponent>]
let OpaqueSequence (log: string -> string) =
    div (title = String.concat ":" [| yield! Seq.append (seq { yield log "left" }) (Seq.singleton (log "right")) |])

[<SolidComponent>]
let LoopSequence (items: string array) (log: string -> string) =
    div (
        title =
            String.concat
                ":"
                [| for item in items do
                       if item <> "" then
                           yield log item |]
    )

[<SolidComponent(ComponentFlag.SkipCEOptimisation)>]
let SkippedArray (flag: bool) =
    div (
        title =
            String.concat
                ":"
                [| yield "head"
                   if flag then yield "one" else yield "two"
                   yield "tail" |]
    )

[<SolidComponent>]
let MatchedItems (value: Payload) (log: string -> string) =
    div (
        title =
            String.concat
                ":"
                [| yield log "head"
                   match value with
                   | Empty -> ()
                   | Text text -> yield log text
                   | Pair (number, text) ->
                       yield log (string number)
                       yield log text
                   yield log "tail" |]
    )
