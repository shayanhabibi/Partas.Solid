module Partas.Solid.Tests.Runtime.Integration.Composition.Nested

open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop

/// Level 3: renders the value and reports each body run.
[<Erase>]
type Inner() =
    inherit span()

    [<Erase>]
    member val value: string = unbox null with get, set

    [<Erase>]
    member val onRender: string -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        props.onRender "inner"
        span (class' = "inner") { props.value }

/// Level 2: decorates the value and forwards it.
[<Erase>]
type MiddleLayer() =
    inherit div()

    [<Erase>]
    member val value: string = unbox null with get, set

    [<Erase>]
    member val onRender: string -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        props.onRender "middle"

        div (class' = "middle") { Inner(value = "[" + props.value + "]", onRender = props.onRender) }

/// Level 1: forwards untouched.
[<Erase>]
type OuterLayer() =
    inherit section()

    [<Erase>]
    member val value: string = unbox null with get, set

    [<Erase>]
    member val onRender: string -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        props.onRender "outer"
        section (class' = "outer") { MiddleLayer(value = props.value, onRender = props.onRender) }

/// Button primitive: consumes `size`/`variant` (defaults via setters), spreads the rest onto <button>.
[<Erase>]
type BaseButton() =
    inherit button()

    [<Erase>]
    member val size: string = unbox null with get, set

    [<Erase>]
    member val variant: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        props.size <- "md"
        props.variant <- "solid"

        button(class' = "btn btn-" + props.size + " btn-" + props.variant).spread props { props.children }

/// Wrapper that consumes `danger`, forces a variant, and spreads the rest into BaseButton.
[<Erase>]
type DangerButton() =
    inherit BaseButton()

    [<Erase>]
    member val confirmText: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        props.confirmText <- "Sure?"
        BaseButton(variant = "danger", title = props.confirmText).spread props { props.children }

/// Outer wrapper that forwards everything (rest spread at two levels).
[<Erase>]
type ToolbarButton() =
    inherit DangerButton()

    [<Erase>]
    member val icon: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        DangerButton(size = "sm").spread props {
            i (class' = "icon") { props.icon }
            props.children
        }

/// Optional props via F# options.
[<Erase>]
type Profile() =
    inherit div()

    [<Erase>]
    member val name: string = unbox null with get, set

    [<Erase>]
    member val nickname: string option = unbox null with get, set

    [<Erase>]
    member val age: int option = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "profile") {
            span (class' = "name") { props.name }
            span (class' = "nick") { defaultArg props.nickname "none" }
            Show(when' = props.age.IsSome, fallback = span (class' = "age-unknown") { "?" }) {
                span (class' = "age") { string props.age.Value }
            }
        }

/// Optional prop matched with `match`.
[<Erase>]
type Status() =
    inherit span()

    [<Erase>]
    member val message: string option = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        span (class' = "status") {
            match props.message with
            | Some m -> "msg:" + m
            | None -> "idle"
        }

/// Optional prop read only through `defaultArg`.
[<Erase>]
type Nick() =
    inherit span()

    [<Erase>]
    member val nickname: string option = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View = span (class' = "nick-only") { defaultArg props.nickname "anon" }

/// `match` on a plain string prop, yielding text children.
[<Erase>]
type KindLabel() =
    inherit span()

    [<Erase>]
    member val kind: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        span (class' = "kind") {
            match props.kind with
            | "a" -> "Alpha"
            | _ -> "Other"
        }

/// `match` on a plain string prop, yielding elements.
[<Erase>]
type KindIcon() =
    inherit span()

    [<Erase>]
    member val kind: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        span (class' = "kind-icon") {
            match props.kind with
            | "a" -> b (class' = "alpha") { "A" }
            | _ -> i (class' = "other") { "?" }
        }

/// Real usage: three levels, driven by an input at the top.
[<SolidComponent>]
let ThreeLevelHost (onRender: string -> unit) =
    let text, setText = createSignal "a"

    div (class' = "three-host") {
        input (class' = "src", value = text (), onInput = fun e -> setText e.target?value)
        OuterLayer(value = text (), onRender = onRender)
    }

[<SolidComponent>]
let ToolbarHost (onClick: unit -> unit) =
    let label, setLabel = createSignal "Delete"

    div (class' = "toolbar") {
        ToolbarButton(icon = "x", id = "tb", onClick = fun _ ->
            onClick ()
            setLabel "Deleted") {
            label ()
        }
    }

[<SolidComponent>]
let ProfileHost () =
    let nick, setNick = createSignal (None: string option)

    div (class' = "profile-host") {
        button (class' = "set-nick", onClick = fun _ -> setNick (Some "Addy")) { "nick" }
        button (class' = "clear-nick", onClick = fun _ -> setNick None) { "clear" }
        Profile(name = "Ada", nickname = nick ())
    }
