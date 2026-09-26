module Partas.Solid.Tests.Runtime.Dom.Portals.DynamicFn

open Partas.Solid
open Partas.Solid.Web
open Fable.Core
open Fable.Core.JsInterop

/// The `dynamic()` factory: a component whose tag comes from a signal, used as a TagValue.
[<SolidComponent>]
let FieldSwitch () =
    let multiline, setMultiline = createSignal false
    let Field: TagValue = unbox (dynamic (fun () -> if multiline () then unbox<HtmlElement> "textarea" else unbox<HtmlElement> "input"))

    div (class' = "field-switch") {
        button (id = "toggle-multi", onClick = fun _ -> setMultiline (not (multiline ()))) { "toggle" }
        Field % {| id = "field"; placeholder = "type" |}
    }

/// `dynamic()` with the static option, used as a TagValue.
[<SolidComponent>]
let StaticDynamic () =
    let Nav: TagValue = unbox (dynamic ((fun () -> unbox<HtmlElement> "nav"), ``static`` = true))
    div (class' = "dyn-static") { Nav % {| id = "static-nav" |} }

/// `dynamic()` resolving to a Partas component, with props passed through the TagValue.
[<Erase>]
type Shout() =
    inherit div()

    [<Erase>]
    member val word: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View = b (class' = "shout") { props.word + "!" }

/// The component bound through `!@` first, then returned from the `dynamic()` source.
[<SolidComponent>]
let DynamicToTagValue () =
    let ShoutTag = !@Shout
    let Comp: TagValue = unbox (dynamic (fun () -> unbox<HtmlElement> ShoutTag))
    div (class' = "dyn-comp2") { Comp % {| word = "yo" |} }

/// The `dynamic()` result used the way its binding type (`unit -> 'T`) suggests: called as a function.
[<SolidComponent>]
let DynamicCall () =
    let Tag = dynamic (fun () -> unbox<HtmlElement> "section")
    div (class' = "dyn-call") { Tag() }
