module Partas.Solid.Tests.Runtime.Integration.Components.Props

open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop

/// Plain getter props: every read goes through `props.x` so it stays reactive.
[<Erase>]
type Greeting() =
    inherit div()

    [<Erase>]
    member val name: string = unbox null with get, set

    [<Erase>]
    member val count: int = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "greeting") {
            span (class' = "name") { props.name }
            span (class' = "count") { props.count }
            span (class' = "double") { props.count * 2 }
        }

/// Default props through property setters (compiled to `merge`).
[<Erase>]
type Badge() =
    inherit span()

    [<Erase>]
    member val label: string = unbox null with get, set

    [<Erase>]
    member val tone: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        props.label <- "default-label"
        props.tone <- "info"
        span (class' = "badge " + props.tone) { props.label }

/// Rest spreading: `variant` is consumed, everything else is spread onto the <button>.
[<Erase>]
type FancyButton() =
    inherit button()

    [<Erase>]
    member val variant: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        button(class' = "fancy fancy-" + props.variant).spread props { props.children }

/// Children prop, rendered inside a wrapper, next to a reactive title.
[<Erase>]
type Card() =
    inherit div()

    [<Erase>]
    member val heading: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "card") {
            h2 (class' = "card-title") { props.heading }
            div (class' = "card-body") { props.children }
        }

/// A SolidComponent let-binding with no arguments.
[<SolidComponent>]
let Divider () = hr (class' = "divider")

/// A SolidComponent let-binding that takes arguments (compiled as a plain function call).
[<SolidComponent>]
let Pill (text: string) (tone: string) = span (class' = "pill pill-" + tone) { text }

/// Real usage: a parent owns the signal and passes it down through props.
[<SolidComponent>]
let GreetingHost () =
    let name, setName = createSignal "Ada"
    let count, setCount = createSignal 1

    div (class' = "host") {
        Greeting(name = name (), count = count ())
        button (class' = "rename", onClick = fun _ -> setName "Grace")
        button (class' = "bump", onClick = fun _ -> setCount (count () + 1))
        Divider()
        Pill "static" "ok"
    }

/// Composition: FancyButton inside Card, with a signal feeding both the rest props and the children.
[<SolidComponent>]
let CardHost () =
    let clicks, setClicks = createSignal 0

    Card(heading = "Clicks: " + string (clicks ())) {
        FancyButton(variant = "primary", id = "fb", title = "clicked " + string (clicks ()) + " times", onClick = fun _ ->
            setClicks (clicks () + 1)) {
            "Click me"
        }
        span (class' = "echo") { clicks () }
    }
