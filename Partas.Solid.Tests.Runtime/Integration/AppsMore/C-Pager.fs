module Partas.Solid.Tests.Runtime.Integration.AppsMore.Pager

open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop

type Product =
    {| id: int
       name: string
       category: string |}

/// UI state for the list lives in one store; every derived view is a memo over it.
type ListState =
    { mutable query: string
      mutable category: string
      mutable page: int
      mutable pageSize: int }

/// Paginated, filterable product list.
[<Erase>]
type PagedList() =
    inherit div()

    [<Erase>]
    member val products: Product array = unbox null with get, set

    [<Erase>]
    member val initialPageSize: int = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        props.initialPageSize <- 5

        let state, setState =
            createStore
                { query = ""
                  category = "all"
                  page = 1
                  pageSize = props.initialPageSize }

        let categories =
            createMemo (fun (_: string array option) ->
                props.products |> Array.map (fun p -> p.category) |> Array.distinct |> Array.sort)

        let filtered =
            createMemo (fun (_: Product array option) ->
                let q = state.Value.query.Trim().ToLower ()
                let cat = state.Value.category

                props.products
                |> Array.filter (fun p ->
                    (cat = "all" || p.category = cat) && (q = "" || p.name.ToLower().Contains q)))

        let pageCount =
            createMemo (fun (_: int option) ->
                let n = filtered().Length
                if n = 0 then 1 else (n + state.Value.pageSize - 1) / state.Value.pageSize)

        let visible =
            createMemo (fun (_: Product array option) ->
                let size = state.Value.pageSize
                let start = (state.Value.page - 1) * size
                let all = filtered ()
                let stop = min all.Length (start + size)
                if start >= stop then [||] else all[start .. stop - 1])

        let rangeLabel =
            createMemo (fun (_: string option) ->
                let total = filtered().Length

                if total = 0 then
                    "No results"
                else
                    let first = (state.Value.page - 1) * state.Value.pageSize + 1
                    let last = min total (state.Value.page * state.Value.pageSize)
                    $"Showing {first}-{last} of {total}")

        let goTo (page: int) =
            let clamped = max 1 (min (pageCount ()) page)
            setState (fun s -> s.page <- clamped; s)

        div (class' = "paged") {
            div (class' = "controls") {
                input (
                    class' = "search",
                    placeholder = "Search",
                    value = state.Value.query,
                    onInput =
                        fun e ->
                            let v: string = !!e.currentTarget?value
                            // Changing the filter always resets to the first page.
                            setState (fun s ->
                                s.query <- v
                                s.page <- 1
                                s)
                )

                select (
                    class' = "category",
                    value = state.Value.category,
                    onChange =
                        fun e ->
                            let v: string = !!e.currentTarget?value

                            setState (fun s ->
                                s.category <- v
                                s.page <- 1
                                s)
                ) {
                    option' (value = "all") { "All" }

                    For.Keyed (each = categories ()) { yield fun c _ -> option' (value = c) { c } }
                }

                select (
                    class' = "page-size",
                    value = string state.Value.pageSize,
                    onChange =
                        fun e ->
                            let v: string = !!e.currentTarget?value

                            setState (fun s ->
                                s.pageSize <- int v
                                s.page <- 1
                                s)
                ) {
                    option' (value = "2") { "2" }
                    option' (value = "5") { "5" }
                    option' (value = "10") { "10" }
                }
            }

            ul (class' = "results") {
                For.Keyed (each = visible (), fallback = li (class' = "empty") { "Nothing found" }) {
                    yield fun p _ -> li(class' = "result").data ("id", string p.id) { p.name }
                }
            }

            nav (class' = "pager") {
                button (class' = "prev", disabled = (state.Value.page <= 1), onClick = fun _ -> goTo (state.Value.page - 1)) {
                    "Prev"
                }

                For.Keyed (each = Array.init (pageCount ()) (fun i -> i + 1)) {
                    yield
                        fun n _ ->
                            button(
                                class' = (if state.Value.page = n then "page current" else "page"),
                                onClick = fun _ -> goTo n
                            )
                                .data ("page", string n) {
                                string n
                            }
                }

                button (
                    class' = "next",
                    disabled = (state.Value.page >= pageCount ()),
                    onClick = fun _ -> goTo (state.Value.page + 1)
                ) {
                    "Next"
                }
            }

            p (class' = "range") { rangeLabel () }
            p (class' = "page-of") { $"Page {state.Value.page} of {pageCount ()}" }
        }
