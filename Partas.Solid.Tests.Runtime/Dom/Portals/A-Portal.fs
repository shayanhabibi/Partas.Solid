module Partas.Solid.Tests.Runtime.Dom.Portals.Portal

open Partas.Solid
open Partas.Solid.Web
open Fable.Core
open Fable.Core.JsInterop
open Browser.Types
open Partas.Solid.Aria

/// Portal content reads a prop: updates to the prop reach the portalled DOM.
[<Erase>]
type PortalLabel() =
    inherit div()

    [<Erase>]
    member val target: Element = unbox null with get, set

    [<Erase>]
    member val label: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "pl-host") {
            Portal(mount = props.target) {
                div (class' = "pl-content", title = props.label) { props.label }
            }
        }

/// A counter whose value is shown inside the portal; buttons live on both sides.
[<Erase>]
type PortalCounter() =
    inherit div()

    [<Erase>]
    member val target: Element = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let count, setCount = createSignal 0

        div (class' = "pc-host") {
            button (id = "pc-out", onClick = fun _ -> setCount (count () + 1)) { "outside +1" }
            span (id = "pc-mirror") { count () }
            Portal(mount = props.target) {
                div (class' = "pc-portal") {
                    span (id = "pc-value") { count () }
                    button (id = "pc-in", onClick = fun _ -> setCount (count () + 10)) { "inside +10" }
                }
            }
        }

/// Delegated events from portalled content bubble to handlers on the component-tree ancestors.
[<Erase>]
type PortalBubble() =
    inherit div()

    [<Erase>]
    member val target: Element = unbox null with get, set

    [<Erase>]
    member val log: string -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (id = "pb-outer", onClick = fun _ -> props.log "outer") {
            Portal(mount = props.target) {
                div (id = "pb-wrap", onClick = fun _ -> props.log "wrap") {
                    button (id = "pb-btn", onClick = fun _ -> props.log "btn") { "click" }
                }
                button (
                    id = "pb-stop",
                    onClick =
                        fun e ->
                            e.stopPropagation ()
                            props.log "stop"
                ) {
                    "stop"
                }
            }
        }

/// Real usage: a modal dialog opened from a button, rendered through a Portal under Show,
/// closed from a button inside the portal.
[<Erase>]
type Modal() =
    inherit div()

    [<Erase>]
    member val target: Element = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let isOpen, setOpen = createSignal false

        div (class' = "modal-app") {
            button (id = "open", onClick = fun _ -> setOpen true) { "open" }
            span (id = "state") { if isOpen () then "open" else "closed" }
            Show (when' = isOpen ()) {
                Portal(mount = props.target) {
                    div (class' = "dialog", role = "dialog") {
                        h2 () { "Dialog title" }
                        button (id = "close", onClick = fun _ -> setOpen false) { "close" }
                    }
                }
            }
        }

/// A child whose cleanup is reported: used to check portal content is disposed with its owner.
[<Erase>]
type Tracked() =
    inherit div()

    [<Erase>]
    member val name: string = unbox null with get, set

    [<Erase>]
    member val onDispose: string -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        onCleanup (fun () -> props.onDispose props.name)
        span (class' = "tracked") { props.name }

/// Portal whose content is a component with an onCleanup, under a Show toggled by a prop.
[<Erase>]
type PortalCleanup() =
    inherit div()

    [<Erase>]
    member val target: Element = unbox null with get, set

    [<Erase>]
    member val visible: bool = unbox null with get, set

    [<Erase>]
    member val onDispose: string -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "cleanup-host") {
            Show (when' = props.visible) {
                Portal(mount = props.target) { Tracked(name = "inner", onDispose = props.onDispose) }
            }
        }

/// Two portals into the same mount node keep their order and are removed independently.
[<Erase>]
type TwoPortals() =
    inherit div()

    [<Erase>]
    member val target: Element = unbox null with get, set

    [<Erase>]
    member val showFirst: bool = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "two") {
            Show (when' = props.showFirst) {
                Portal(mount = props.target) { p (class' = "first") { "first" } }
            }
            Portal(mount = props.target) { p (class' = "second") { "second" } }
        }

/// Portal whose mount node is chosen reactively from a prop.
[<Erase>]
type SwitchMount() =
    inherit div()

    [<Erase>]
    member val a: Element = unbox null with get, set

    [<Erase>]
    member val b: Element = unbox null with get, set

    [<Erase>]
    member val useB: bool = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "switch") {
            Portal(mount = (if props.useB then props.b else props.a)) { em (class' = "moving") { "moving" } }
        }

type Item = { id: int; name: string }

/// A For list rendered inside the portal, driven by a signal owned outside it.
[<Erase>]
type PortalList() =
    inherit div()

    [<Erase>]
    member val target: Element = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let items, setItems = createSignal [| { id = 1; name = "one" } |]
        let nextId, setNextId = createSignal 2

        div (class' = "plist") {
            button (
                id = "add",
                onClick =
                    fun _ ->
                        let n = nextId ()
                        setItems (Array.append (items ()) [| { id = n; name = "item" + string n } |])
                        setNextId (n + 1)
            ) {
                "add"
            }
            button (id = "clear", onClick = fun _ -> setItems [||]) { "clear" }
            Portal(mount = props.target) {
                ul (class' = "portal-list") {
                    For.Keyed(each = items ()) { yield fun item _ -> li () { item.name } }
                }
            }
        }

/// Nested portals: the inner Portal renders into the second mount from inside the first portal.
[<Erase>]
type NestedPortals() =
    inherit div()

    [<Erase>]
    member val outer: Element = unbox null with get, set

    [<Erase>]
    member val inner: Element = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "nested") {
            Portal(mount = props.outer) {
                div (class' = "lvl1") {
                    span () { "level 1" }
                    Portal(mount = props.inner) { div (class' = "lvl2") { "level 2" } }
                }
            }
        }

/// Several children inside one portal, in order.
[<Erase>]
type PortalManyChildren() =
    inherit div()

    [<Erase>]
    member val target: Element = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "many") {
            Portal(mount = props.target) {
                span (class' = "c1") { "a" }
                span (class' = "c2") { "b" }
                span (class' = "c3") { "c" }
            }
        }

/// A ref inside the portal receives the element that lives in the mount node.
[<Erase>]
type PortalRef() =
    inherit div()

    [<Erase>]
    member val target: Element = unbox null with get, set

    [<Erase>]
    member val got: HTMLElement -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "pref") {
            Portal(mount = props.target) {
                input(id = "portal-input").ref (fun (el: HTMLInputElement) -> props.got (unbox el))
            }
        }

/// Real usage: portal into a sibling element captured by ref (tooltip layer inside the same app).
[<SolidComponent>]
let SiblingLayer () =
    let layer, setLayer = createSignal (unbox<Element> null)

    div (class' = "sib") {
        Show (when' = (not (isNull (box (layer ()))))) {
            Portal(mount = layer ()) { span (class' = "tip") { "tooltip" } }
        }
        div(class' = "layer").ref (fun (el: HTMLDivElement) -> setLayer (unbox el))
    }

/// Reactive attributes and classes on portal content.
[<Erase>]
type PortalAttrs() =
    inherit div()

    [<Erase>]
    member val target: Element = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let active, setActive = createSignal false

        div (class' = "pa") {
            button (id = "toggle-active", onClick = fun _ -> setActive (not (active ()))) { "toggle" }
            Portal(mount = props.target) {
                div (id = "pa-box", class' = (if active () then "box active" else "box"), title = (if active () then "yes" else "no")) {
                    if active () then "on" else "off"
                }
            }
        }

/// Portal with the mount given as document.body explicitly.
[<SolidComponent>]
let PortalExplicitBody () =
    div (class' = "eb") { Portal(mount = unbox Browser.Dom.document.body) { aside (id = "eb-aside") { "to body" } } }

/// Input inside a body portal feeds a signal displayed outside it.
[<SolidComponent>]
let PortalInput () =
    let value, setValue = createSignal ""

    div (class' = "pi") {
        output (id = "pi-out") { value () }
        Portal() { input (id = "pi-in", onInput = fun e -> setValue (e.target?value)) }
    }
