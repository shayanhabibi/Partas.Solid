module Partas.Solid.Tests.Runtime.Integration.FlowAdvanced.BoundaryCases

open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop

/// Throws synchronously from the component body when `shouldFail()` is true.
[<Erase>]
type RenderThrower() =
    inherit span()

    [<Erase>]
    member val shouldFail: unit -> bool = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        if props.shouldFail () then
            failwith "render failed"

        span (class' = "ok") { "fine" }

/// Errored around a component that throws while rendering; the fallback exposes reset.
[<Erase>]
type RenderGuard() =
    inherit div()

    [<Erase>]
    member val shouldFail: unit -> bool = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "guard") {
            span (class' = "sibling") { "outside" }

            Errored(
                fallback =
                    !^(ErrorBoundary.Fallback(fun err reset ->
                        div (class' = "error") {
                            span (class' = "message") { (err () :?> exn).Message }
                            button (class' = "retry", onClick = fun _ -> reset ())
                        }))
            ) {
                RenderThrower(shouldFail = props.shouldFail)
            }
        }

/// Reads an async memo created inside the boundary.
[<Erase>]
type AsyncChild() =
    inherit span()

    [<Erase>]
    member val load: unit -> JS.Promise<string> = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let data = createMemo (fun (_: string option) -> props.load ())
        span (class' = "data") { data () }

/// Errored > Loading > async child: rejected promises reach the error boundary.
[<Erase>]
type AsyncGuard() =
    inherit div()

    [<Erase>]
    member val load: unit -> JS.Promise<string> = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "async-guard") {
            Errored(
                fallback =
                    !^(ErrorBoundary.Fallback(fun err reset ->
                        div (class' = "error") {
                            span (class' = "message") { (err () :?> exn).Message }
                            button (class' = "retry", onClick = fun _ -> reset ())
                        }))
            ) {
                Loading(fallback = span (class' = "spinner") { "loading" }) { AsyncChild(load = props.load) }
            }
        }

/// Two nested Errored boundaries: the inner one catches first; a throwing inner fallback escalates.
[<Erase>]
type NestedGuards() =
    inherit div()

    [<Erase>]
    member val innerFails: unit -> bool = unbox null with get, set

    [<Erase>]
    member val fallbackFails: bool = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "nested-guards") {
            Errored(
                fallback =
                    !^(ErrorBoundary.Fallback(fun err _ ->
                        div (class' = "outer-error") { (err () :?> exn).Message }))
            ) {
                span (class' = "outer-content") { "outer" }

                Errored(
                    fallback =
                        !^(ErrorBoundary.Fallback(fun err _ ->
                            if props.fallbackFails then
                                failwith "fallback failed"

                            div (class' = "inner-error") { (err () :?> exn).Message }))
                ) {
                    RenderThrower(shouldFail = props.innerFails)
                }
            }
        }

/// Errored whose child memo throws based on a reactive signal, inside a list row.
[<Erase>]
type RowGuard() =
    inherit li()

    [<Erase>]
    member val value: int = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let checkedValue =
            createMemo (fun (_: int option) ->
                if props.value < 0 then
                    failwith ("negative " + string props.value)

                props.value)

        li (class' = "cell") {
            Errored(fallback = !^(ErrorBoundary.Fallback(fun err _ -> b (class' = "bad") { (err () :?> exn).Message }))) {
                span (class' = "good") { checkedValue () }
            }
        }

[<Erase>]
type GuardedList() =
    inherit ul()

    [<Erase>]
    member val values: int[] = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        ul (class' = "guarded") { For.Keyed(each = props.values) { yield fun v _ -> RowGuard(value = v) } }

/// Nested Loading: the outer waits on `outer`, the inner on `inner`.
[<Erase>]
type NestedLoading() =
    inherit div()

    [<Erase>]
    member val outer: unit -> JS.Promise<string> = unbox null with get, set

    [<Erase>]
    member val inner: unit -> JS.Promise<string> = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let a = createMemo (fun (_: string option) -> props.outer ())
        let b = createMemo (fun (_: string option) -> props.inner ())

        div (class' = "nl") {
            Loading(fallback = span (class' = "outer-fb") { "outer loading" }) {
                span (class' = "outer") { a () }

                Loading(fallback = span (class' = "inner-fb") { "inner loading" }) {
                    span (class' = "inner") { b () }
                }
            }
        }

/// isPending around a keyed async fetch; the indicator sits inside the boundary so first load is caught by Loading.
[<Erase>]
type PendingIndicator() =
    inherit div()

    [<Erase>]
    member val id: int = unbox null with get, set

    [<Erase>]
    member val fetch: int -> JS.Promise<string> = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let detail = createMemo (fun (_: string option) -> props.fetch props.id)

        div (class' = "pi") {
            Loading(fallback = span (class' = "spinner") { "loading" }) {
                Show(when' = isPending (fun () -> box (detail ()))) { span (class' = "busy") { "refreshing" } }
                span (class' = "content") { detail () }
            }
        }

/// Loading with an `on` key: writes to `on` bring the fallback back.
[<Erase>]
type OnLoading() =
    inherit div()

    [<Erase>]
    member val id: int = unbox null with get, set

    [<Erase>]
    member val fetch: int -> JS.Promise<string> = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let detail = createMemo (fun (_: string option) -> props.fetch props.id)

        div (class' = "ol") {
            Loading(fallback = span (class' = "spinner") { "loading" }, on = props.id) {
                span (class' = "content") { detail () }
            }
        }

/// Reveal with `collapsed`: tail fallbacks past the frontier are suppressed.
[<Erase>]
type CollapsedReveal() =
    inherit div()

    [<Erase>]
    member val first: unit -> JS.Promise<string> = unbox null with get, set

    [<Erase>]
    member val second: unit -> JS.Promise<string> = unbox null with get, set

    [<Erase>]
    member val third: unit -> JS.Promise<string> = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let a = createMemo (fun (_: string option) -> props.first ())
        let b = createMemo (fun (_: string option) -> props.second ())
        let c = createMemo (fun (_: string option) -> props.third ())

        div (class' = "cr") {
            Reveal(order = Reveal.Order.Sequential, collapsed = true) {
                Loading(fallback = span (class' = "fb-a") { "..." }) { span (class' = "a") { a () } }
                Loading(fallback = span (class' = "fb-b") { "..." }) { span (class' = "b") { b () } }
                Loading(fallback = span (class' = "fb-c") { "..." }) { span (class' = "c") { c () } }
            }
        }

/// RowGuard with the prop bound to a local before it is formatted.
[<Erase>]
type RowGuardLocal() =
    inherit li()

    [<Erase>]
    member val value: int = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let checkedValue =
            createMemo (fun (_: int option) ->
                let v = props.value

                if v < 0 then
                    failwith ("negative " + string v)

                v)

        li (class' = "cell") {
            Errored(fallback = !^(ErrorBoundary.Fallback(fun err _ -> b (class' = "bad") { (err () :?> exn).Message }))) {
                span (class' = "good") { checkedValue () }
            }
        }

[<Erase>]
type GuardedListLocal() =
    inherit ul()

    [<Erase>]
    member val values: int[] = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        ul (class' = "guarded") { For.Keyed(each = props.values) { yield fun v _ -> RowGuardLocal(value = v) } }

/// PendingIndicator with isPending computed inside a memo.
[<Erase>]
type PendingIndicatorLocal() =
    inherit div()

    [<Erase>]
    member val id: int = unbox null with get, set

    [<Erase>]
    member val fetch: int -> JS.Promise<string> = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let detail = createMemo (fun (_: string option) -> props.fetch props.id)
        let busy = createMemo (fun (_: bool option) -> isPending (fun () -> box (detail ())))

        div (class' = "pi") {
            Loading(fallback = span (class' = "spinner") { "loading" }) {
                Show(when' = busy ()) { span (class' = "busy") { "refreshing" } }
                span (class' = "content") { detail () }
            }
        }
