module Partas.Solid.Tests.Runtime.Integration.Apps.AsyncSearch

open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop

/// Search box backed by an async (Promise-returning) memo, rendered under <Loading>.
/// The input sits outside the boundary so typing never unmounts it.
[<Erase>]
type SearchBox() =
    inherit div()

    [<Erase>]
    member val search: string -> JS.Promise<string array> = unbox null with get, set

    [<Erase>]
    member val initialQuery: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        props.initialQuery <- ""
        let query, setQuery = createSignal props.initialQuery

        let results =
            createMemo (fun (_: string array option) -> props.search (query ()))

        // Hoisted: an isPending thunk written inline in an attribute value is unwrapped by the
        // plugin (see PendingBadge), so the realistic app computes it in a helper.
        // (A plain `let stale () = ...` helper gets inlined by Fable and then unwrapped too.)
        let stale =
            createMemo (fun (_: bool option) -> isPending (fun () -> box (results ())))

        div (class' = "search") {
            input (
                class' = "q",
                placeholder = "Search",
                value = query (),
                onInput = fun e -> setQuery (!!e.currentTarget?value)
            )

            span (class' = "typed") { query () }

            Loading (fallback = p (class' = "loading") { "Searching..." }) {
                div (class' = (if stale () then "results stale" else "results")) {
                    Show (when' = (results().Length > 0), fallback = p (class' = "no-results") { "No results" }) {
                        ul (class' = "hits") {
                            For.Keyed (each = results ()) { yield fun hit _ -> li (class' = "hit") { hit } }
                        }
                    }

                    span (class' = "count") { $"{results().Length} results" }
                }
            }
        }

/// Same search, wrapped in <Errored> with a reset-capable fallback.
[<Erase>]
type SafeSearchBox() =
    inherit div()

    [<Erase>]
    member val search: string -> JS.Promise<string array> = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let query, setQuery = createSignal ""

        let results =
            createMemo (fun (_: string array option) -> props.search (query ()))

        div (class' = "safe-search") {
            input (class' = "q", value = query (), onInput = fun e -> setQuery (!!e.currentTarget?value))

            Errored (
                fallbackFn =
                    ErrorBoundary.Fallback(fun err reset ->
                        div (class' = "error") {
                            span (class' = "message") { $"Search failed: {err ()}" }
                            button (class' = "retry", onClick = fun _ -> reset ()) { "Retry" }
                        })
            ) {
                Loading (fallback = p (class' = "loading") { "Searching..." }) {
                    ul (class' = "hits") {
                        For.Keyed (each = results ()) { yield fun hit _ -> li (class' = "hit") { hit } }
                    }
                }
            }
        }

/// isPending written inline inside an attribute value (the natural way to write it).
[<Erase>]
type PendingBadge() =
    inherit div()

    [<Erase>]
    member val load: string -> JS.Promise<string> = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let key, setKey = createSignal "a"
        let value = createMemo (fun (_: string option) -> props.load (key ()))

        div (class' = "badge") {
            button (class' = "next", onClick = fun _ -> setKey (key () + "a"))

            Loading (fallback = span (class' = "loading") { "..." }) {
                span (class' = (if isPending (fun () -> box (value ())) then "value pending" else "value")) { value () }
            }
        }

/// SafeSearchBox using the raw `fallback` U2 property instead of the `fallbackFn` helper.
[<Erase>]
type SafeSearchBoxRaw() =
    inherit div()

    [<Erase>]
    member val search: string -> JS.Promise<string array> = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let query, setQuery = createSignal ""

        let results =
            createMemo (fun (_: string array option) -> props.search (query ()))

        div (class' = "safe-search") {
            input (class' = "q", value = query (), onInput = fun e -> setQuery (!!e.currentTarget?value))

            Errored(
                fallback =
                    !^(ErrorBoundary.Fallback(fun err reset ->
                        div (class' = "error") {
                            span (class' = "message") { $"Search failed: {err ()}" }
                            button (class' = "retry", onClick = fun _ -> reset ()) { "Retry" }
                        }))
            ) {
                Loading (fallback = p (class' = "loading") { "Searching..." }) {
                    ul (class' = "hits") {
                        For.Keyed (each = results ()) { yield fun hit _ -> li (class' = "hit") { hit } }
                    }
                }
            }
        }

/// Control for PendingBadge: the same isPending check hoisted into a memo, which the plugin
/// leaves intact. Proves the PendingBadge expectation is reachable once the thunk survives.
[<Erase>]
type PendingBadgeMemo() =
    inherit div()

    [<Erase>]
    member val load: string -> JS.Promise<string> = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let key, setKey = createSignal "a"
        let value = createMemo (fun (_: string option) -> props.load (key ()))
        let pending = createMemo (fun (_: bool option) -> isPending (fun () -> box (value ())))

        div (class' = "badge") {
            button (class' = "next", onClick = fun _ -> setKey (key () + "a"))

            Loading (fallback = span (class' = "loading") { "..." }) {
                span (class' = (if pending () then "value pending" else "value")) { value () }
            }
        }
