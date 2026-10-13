module Partas.Solid.Tests.Runtime.Integration.Components.Abstract

open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop

/// Leaf that receives an accessor (not a value) and reads it lazily.
[<Erase>]
type Leaf() =
    inherit span()

    [<Erase>]
    member val value: Accessor<int> = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View = span (class' = "leaf") { props.value () }

/// Middle layer derives a new accessor from the one it receives and forwards it.
[<Erase>]
type Middle() =
    inherit div()

    [<Erase>]
    member val value: Accessor<int> = unbox null with get, set

    [<Erase>]
    member val onCompute: int -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let doubled =
            createMemo (fun _ ->
                let v = props.value () * 2
                props.onCompute v
                v)

        div (class' = "middle") {
            Leaf(value = props.value)
            Leaf(value = doubled)
        }

/// Root that owns the signal and passes the accessor two levels down.
[<SolidComponent>]
let AccessorChain (onCompute: int -> unit) =
    let n, setN = createSignal 1

    div (class' = "chain") {
        button (class' = "inc", onClick = fun _ -> setN (n () + 1))
        Middle(value = n, onCompute = onCompute)
    }

/// A child that logs its lifecycle: effect runs and cleanup, through a callback prop.
[<Erase>]
type Lifecycle() =
    inherit div()

    [<Erase>]
    member val name: string = unbox null with get, set

    [<Erase>]
    member val tick: Accessor<int> = unbox null with get, set

    [<Erase>]
    member val log: string -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let name = props.name
        props.log ("mount " + name)
        onCleanup (fun () -> props.log ("cleanup " + name))

        createEffect ((fun _ -> props.tick ()), fun (t: int) -> props.log ("effect " + name + " " + string t))

        div (class' = "panel " + name) { name }

/// Swaps between two stateful children; the hidden one must be disposed.
[<Erase>]
type Swapper() =
    inherit div()

    [<Erase>]
    member val showA: bool = unbox null with get, set

    [<Erase>]
    member val tick: Accessor<int> = unbox null with get, set

    [<Erase>]
    member val log: string -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "swapper") {
            Show(when' = props.showA, fallback = Lifecycle(name = "B", tick = props.tick, log = props.log)) {
                Lifecycle(name = "A", tick = props.tick, log = props.log)
            }
        }

/// Swapping via Switch/Match between a component tree and a keyed list.
[<Erase>]
type Tabs() =
    inherit div()

    [<Erase>]
    member val tab: string = unbox null with get, set

    [<Erase>]
    member val tick: Accessor<int> = unbox null with get, set

    [<Erase>]
    member val log: string -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "tabs") {
            Switch() {
                Match(when' = (props.tab = "one")) { Lifecycle(name = "one", tick = props.tick, log = props.log) }
                Match(when' = (props.tab = "many")) {
                    For.Keyed(each = [| "x"; "y" |]) {
                        yield fun name _ -> Lifecycle(name = name, tick = props.tick, log = props.log)
                    }
                }
            }
        }

/// A component that takes a render callback (render prop) and calls it with its own state.
[<Erase>]
type Hover() =
    inherit div()

    [<Erase>]
    member val render: Accessor<bool> -> HtmlElement = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let hovered, setHovered = createSignal false

        div (class' = "hover", onMouseEnter = (fun _ -> setHovered true), onMouseLeave = fun _ -> setHovered false) {
            props.render hovered
        }

[<SolidComponent>]
let HoverUser () =
    Hover(render = fun hovered -> span (class' = "state") { if hovered () then "hovering" else "idle" })

/// A let-bound component whose body reads an accessor once (a snapshot) and once reactively in JSX.
[<SolidComponent>]
let Snapshot (value: Accessor<int>) (log: string -> unit) =
    log "body"
    let initial = value ()
    span (class' = "snapshot") { string initial + "/" + string (value ()) }

/// The same shape as a SolidTypeComponent, for comparison.
[<Erase>]
type TypedSnapshot() =
    inherit span()

    [<Erase>]
    member val value: Accessor<int> = unbox null with get, set

    [<Erase>]
    member val log: string -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        props.log "body"
        let initial = props.value ()
        span (class' = "snapshot") { string initial + "/" + string (props.value ()) }

[<Erase>]
type SnapshotHost() =
    inherit div()

    [<Erase>]
    member val value: Accessor<int> = unbox null with get, set

    [<Erase>]
    member val log: string -> unit = unbox null with get, set

    [<Erase>]
    member val typed: bool = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "snapshot-host") {
            if props.typed then
                TypedSnapshot(value = props.value, log = props.log)
            else
                Snapshot props.value props.log
        }

/// Keyed list of stateful rows: removing one item must dispose only that row.
[<Erase>]
type LifecycleList() =
    inherit div()

    [<Erase>]
    member val names: string[] = unbox null with get, set

    [<Erase>]
    member val tick: Accessor<int> = unbox null with get, set

    [<Erase>]
    member val log: string -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "lifecycle-list") {
            For.Keyed(each = props.names) {
                yield fun name _ -> Lifecycle(name = name, tick = props.tick, log = props.log)
            }
        }

/// A let-bound component that registers a cleanup.
[<SolidComponent>]
let LetPanel (name: string) (log: string -> unit) =
    log ("mount " + name)
    onCleanup (fun () -> log ("cleanup " + name))
    div (class' = "let-panel") { name }

/// Show swapping two let-bound components: the hidden one must be cleaned up.
[<Erase>]
type LetSwapper() =
    inherit div()

    [<Erase>]
    member val showA: bool = unbox null with get, set

    [<Erase>]
    member val log: string -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "let-swapper") {
            Show(when' = props.showA, fallback = LetPanel "B" props.log) { LetPanel "A" props.log }
        }
