module Partas.Solid.Tests.Runtime.Integration.Apps.DataTable

open Partas.Solid
open Partas.Solid.Aria
open Fable.Core
open Fable.Core.JsInterop

type Person =
    {| id: int
       name: string
       age: int
       city: string |}

[<StringEnum>]
type SortKey =
    | Name
    | Age
    | City

[<StringEnum>]
type SortDir =
    | Asc
    | Desc

/// Sortable, filterable table. Sorting cycles none -> ascending -> descending -> none per column.
[<Erase>]
type DataTable() =
    inherit div()

    [<Erase>]
    member val rows: Person array = unbox null with get, set

    /// Invoked each time the sorted-rows memo recomputes (to count recomputations).
    [<Erase>]
    member val onSortComputed: unit -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let sortKey, setSortKey = createSignal<SortKey option> None
        let dir, setDir = createSignal Asc
        let query, setQuery = createSignal ""

        let clickHeader (key: SortKey) =
            match sortKey () with
            | Some current when current = key ->
                if dir () = Asc then
                    setDir Desc
                else
                    setSortKey None
                    setDir Asc
            | _ ->
                setSortKey (Some key)
                setDir Asc

        let filtered =
            createMemo (fun (_: Person array option) ->
                let q = query().Trim().ToLower ()

                if q = "" then
                    props.rows
                else
                    props.rows
                    |> Array.filter (fun r -> r.name.ToLower().Contains q || r.city.ToLower().Contains q))

        let sorted =
            createMemo (fun (_: Person array option) ->
                if not (isNull (box props.onSortComputed)) then
                    props.onSortComputed ()

                let rows = filtered ()

                match sortKey () with
                | None -> rows
                | Some key ->
                    let cmp (a: Person) (b: Person) =
                        match key with
                        | Name -> compare a.name b.name
                        | Age -> compare a.age b.age
                        | City -> compare a.city b.city

                    rows
                    |> Array.sortWith (fun a b -> if dir () = Asc then cmp a b else cmp b a))

        let ariaSortFor (key: SortKey) =
            match sortKey () with
            | Some k when k = key -> if dir () = Asc then "ascending" else "descending"
            | _ -> "none"

        div (class' = "datatable") {
            input (
                class' = "filter",
                placeholder = "Filter",
                value = query (),
                onInput = fun e -> setQuery (!!e.currentTarget?value)
            )

            table () {
                thead () {
                    tr () {
                        For.Keyed (each = [| Name; Age; City |]) {
                            yield
                                fun key _ ->
                                    th(ariaSort = ariaSortFor key).data ("key", unbox<string> key) {
                                        button (class' = "sort", onClick = fun _ -> clickHeader key) {
                                            unbox<string> key
                                        }
                                    }
                        }
                    }
                }

                tbody () {
                    For.Keyed (
                        each = sorted (),
                        fallback =
                            tr (class' = "empty") {
                                td (colspan = 4) { "No rows" }
                            }
                    ) {
                        yield
                            fun row index ->
                                tr().data ("id", string row.id) {
                                    td (class' = "pos") { index () + 1 }
                                    td (class' = "name") { row.name }
                                    td (class' = "age") { row.age }
                                    td (class' = "city") { row.city }
                                }
                    }
                }
            }

            p (class' = "summary") { $"{sorted().Length} of {props.rows.Length} rows" }
        }
