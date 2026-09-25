module Partas.Solid.Tests.Runtime.Integration.Composition.ChildrenHelper

open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop

/// `children` helper: resolve once, render the resolved value, and count the resolved nodes.
[<Erase>]
type Counted() =
    inherit div()

    [<SolidTypeComponent>]
    member props.View =
        let resolved = children (fun () -> props.children)

        div (class' = "counted") {
            span (class' = "count") { resolved.toArray().Length }
            div (class' = "items") { resolved.Invoke() }
        }

/// Wraps every resolved child in its own <li>.
[<Erase>]
type WrapEach() =
    inherit ul()

    [<SolidTypeComponent>]
    member props.View =
        let resolved = children (fun () -> props.children)

        ul (class' = "wrap-each") {
            For.Keyed(each = resolved.toArray()) { yield fun child _ -> li (class' = "wrapped") { child } }
        }

/// Only renders its frame when it actually has children.
[<Erase>]
type OptionalFrame() =
    inherit div()

    [<Erase>]
    member val heading: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let resolved = children (fun () -> props.children)

        div (class' = "optional-frame") {
            h3 () { props.heading }
            Show(when' = (resolved.toArray().Length > 0), fallback = p (class' = "empty") { "no content" }) {
                div (class' = "frame") { resolved.Invoke() }
            }
        }

/// Experimental `children { }` builder.
[<Erase>]
type BuilderCounted() =
    inherit div()

    [<SolidTypeComponent>]
    member props.View =
        let resolved = Partas.Solid.Experimental.Builders.children { props.children }

        div (class' = "builder-counted") {
            span (class' = "count") { resolved.toArray().Length }
            div (class' = "items") { resolved.Invoke() }
        }

/// Reads the resolved children twice: the child components must be created once.
[<Erase>]
type Twice() =
    inherit div()

    [<SolidTypeComponent>]
    member props.View =
        let resolved = children (fun () -> props.children)

        div (class' = "twice") {
            span (class' = "n") { resolved.toArray().Length }
            span (class' = "n2") { resolved.toArray().Length }
            div (class' = "body") { resolved.Invoke() }
        }

/// A child that reports when it is created.
[<Erase>]
type Tracked() =
    inherit span()

    [<Erase>]
    member val onCreate: string -> unit = unbox null with get, set

    [<Erase>]
    member val label: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        props.onCreate props.label
        span (class' = "tracked") { props.label }

/// Real usage: a signal adds/removes children of a Counted.
[<SolidComponent>]
let CountedHost () =
    let n, setN = createSignal 2

    div (class' = "counted-host") {
        button (class' = "more", onClick = fun _ -> setN (n () + 1)) { "+" }
        button (class' = "less", onClick = fun _ -> setN (max 0 (n () - 1))) { "-" }
        Counted() {
            For.Keyed(each = Array.init (n ()) id) { yield fun i _ -> b (class' = "item") { string i } }
        }
    }

/// Static children of different shapes.
[<SolidComponent>]
let StaticCounted () =
    Counted() {
        span (class' = "a") { "a" }
        span (class' = "b") { "b" }
        span (class' = "c") { "c" }
    }

[<SolidComponent>]
let StaticWrapEach () =
    WrapEach() {
        span () { "one" }
        span () { "two" }
    }

[<SolidComponent>]
let StaticBuilderCounted () =
    BuilderCounted() {
        i () { "x" }
        i () { "y" }
    }

[<SolidComponent>]
let TwiceHost (onCreate: string -> unit) =
    Twice() {
        Tracked(label = "t1", onCreate = onCreate)
        Tracked(label = "t2", onCreate = onCreate)
    }

[<SolidComponent>]
let FrameHost () =
    let show, setShow = createSignal false

    div (class' = "frame-host") {
        button (class' = "toggle", onClick = fun _ -> setShow (not (show ()))) { "toggle" }
        OptionalFrame(heading = "Frame") { Show(when' = show ()) { em (class' = "content") { "content" } } }
    }
