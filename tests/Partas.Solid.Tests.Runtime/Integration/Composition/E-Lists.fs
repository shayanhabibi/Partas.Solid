module Partas.Solid.Tests.Runtime.Integration.Composition.Lists

open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop

type Person = { id: int; name: string; role: string }

/// A component returning a fragment with two roots.
[<Erase>]
type TermDef() =
    inherit div()

    [<Erase>]
    member val term: string = unbox null with get, set

    [<Erase>]
    member val definition: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        Fragment() {
            dt () { props.term }
            dd () { props.definition }
        }

/// A let-bound component returning a fragment.
[<SolidComponent>]
let Pair (a: string) (b: string) =
    Fragment() {
        span (class' = "pa") { a }
        span (class' = "pb") { b }
    }

/// Row component used inside For.
[<Erase>]
type PersonRow() =
    inherit li()

    [<Erase>]
    member val person: Person = unbox null with get, set

    [<Erase>]
    member val selected: bool = unbox null with get, set

    [<Erase>]
    member val onSelect: int -> unit = unbox null with get, set

    [<Erase>]
    member val onCreate: int -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        props.onCreate props.person.id

        li (class' = (if props.selected then "person selected" else "person"), onClick = fun _ -> props.onSelect props.person.id) {
            span (class' = "pname") { props.person.name }
            span (class' = "prole") { props.person.role }
        }

/// Glossary: fragments inside a <dl> list.
[<SolidComponent>]
let Glossary () =
    let entries, setEntries = createSignal [| "css", "styles"; "js", "scripts" |]

    div (class' = "glossary") {
        button (class' = "add", onClick = fun _ -> setEntries (Array.append (entries ()) [| "html", "markup" |])) { "add" }
        dl () {
            For.Keyed(each = entries ()) { yield fun (t, d) _ -> TermDef(term = t, definition = d) }
        }
    }

[<SolidComponent>]
let PairHost () =
    div (class' = "pair-host") {
        Pair "left" "right"
        Pair "l2" "r2"
    }

/// Real usage: a people list with selection, rename and reorder, rows are components.
[<SolidComponent>]
let PeopleList (onCreate: int -> unit) =
    let people, setPeople =
        createSignal [| { id = 1; name = "Ada"; role = "eng" }; { id = 2; name = "Grace"; role = "admiral" } |]

    let selected, setSelected = createSignal 0

    div (class' = "people") {
        button (class' = "rename", onClick = fun _ -> setPeople (people () |> Array.map (fun p -> if p.id = 1 then { p with name = "Ada L." } else p))) { "rename" }
        button (class' = "reverse", onClick = fun _ -> setPeople (Array.rev (people ()))) { "reverse" }
        button (class' = "add", onClick = fun _ -> setPeople (Array.append (people ()) [| { id = 3; name = "Linus"; role = "kernel" } |])) { "add" }
        ul () {
            For.Keyed(each = people ()) {
                yield fun p _ -> PersonRow(person = p, selected = (selected () = p.id), onSelect = (fun id -> setSelected id), onCreate = onCreate)
            }
        }
        span (class' = "selected") { string (selected ()) }
    }

/// Components rendered by Repeat, count driven by a signal.
[<SolidComponent>]
let RepeatHost () =
    let n, setN = createSignal 3

    div (class' = "repeat-host") {
        button (class' = "less", onClick = fun _ -> setN (n () - 1)) { "-" }
        Repeat(count = n ()) { yield fun i -> Pair ("r" + string i) "x" }
    }
