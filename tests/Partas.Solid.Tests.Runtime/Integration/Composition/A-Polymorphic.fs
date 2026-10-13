module Partas.Solid.Tests.Runtime.Integration.Composition.Polymorphic

open Partas.Solid
open Partas.Solid.Web
open Fable.Core
open Fable.Core.JsInterop
open Partas.Solid.Tests.Runtime.Integration.Composition.PolymorphicExt

/// A Kobalte-style polymorphic primitive: renders whatever `as` holds through <Dynamic>, spreading
/// the rest of its props onto it. `as` defaults to "div".
[<Erase>]
type Box() =
    inherit div()
    interface Polymorph

    [<Erase>]
    member val ``as``: obj = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        props.``as`` <- box "div"
        Dynamic<obj>(component' = unbox props.``as``).spread props

/// A plain SolidTypeComponent used as the polymorphic target.
[<Erase>]
type Link() =
    inherit a()

    [<Erase>]
    member val tone: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        a(class' = "link link-" + props.tone).spread props { props.children }

/// A polymorphic primitive whose polymorphic prop is named `render`.
[<Erase>]
type Slot() =
    inherit div()
    interface Polymorph

    [<Erase>]
    member val render: obj = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        Dynamic<obj>(component' = unbox props.render).spread props

/// Box rendered as a native <button>, with an event handler on the morph target.
[<SolidComponent>]
let BoxAsButton () =
    let count, setCount = createSignal 0

    Box(class' = "boxed", id = "poly-btn").as'(button (type' = "button", onClick = fun _ -> setCount (count () + 1))) {
        "Pressed " + string (count ())
    }

/// Box with no `as`: the default "div".
[<SolidComponent>]
let BoxDefault () = Box(class' = "plain", id = "poly-default") { "just a div" }

/// Box rendered as a native <span>.
[<SolidComponent>]
let BoxAsSpan () = Box(class' = "as-span").as'(span ()) { "span text" }

/// Box rendered as another SolidTypeComponent; props given to the morph target and to Box both arrive.
[<SolidComponent>]
let BoxAsLink () =
    Box(id = "poly-link", title = "go home").as'(Link(tone = "primary", href = "/home")) { "Home" }

/// Box whose class is reactive and whose morph is a native element.
[<SolidComponent>]
let ReactiveBox () =
    let active, setActive = createSignal false

    div (class' = "rb-host") {
        button (class' = "toggle", onClick = fun _ -> setActive (not (active ()))) { "toggle" }
        Box(class' = (if active () then "on" else "off")).as'(section ()) { if active () then "ACTIVE" else "idle" }
    }

/// The custom polymorphic attribute.
[<SolidComponent>]
let SlotAsArticle () = Slot(id = "slot").__PARTAS_POLYMORPHIC__render(article (class' = "art")) { "slotted" }

