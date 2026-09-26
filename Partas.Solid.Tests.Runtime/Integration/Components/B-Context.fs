module Partas.Solid.Tests.Runtime.Integration.Components.Context

open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop

/// Context with a static default, and one carrying a reactive accessor.
let ThemeContext = createContext<string> "light"

[<JS.Pojo>]
type CounterStore(count: Accessor<int>, increment: unit -> unit) =
    member val count = count with get, set
    member val increment = increment with get, set

let CounterContext = createContext<CounterStore> ()

[<SolidComponent>]
let ThemedLabel () =
    let theme = useContext ThemeContext
    span (class' = "theme") { theme }

/// Idiomatic provider syntax: `Context(value) { children }` -> `<Context.Provider value=...>`.
[<SolidComponent>]
let ThemeApp () =
    div (class' = "app") {
        ThemedLabel()
        ThemeContext "dark" {
            ThemedLabel()
            ThemeContext "nested" { ThemedLabel() }
        }
    }

/// Consumers far from the provider share one store; one mutates, the other re-renders.
[<SolidComponent>]
let CounterDisplay () =
    let store = useContext CounterContext
    span (class' = "display") { store.count () }

[<SolidComponent>]
let CounterButton () =
    let store = useContext CounterContext
    button (class' = "inc", onClick = fun _ -> store.increment ()) { "+" }

[<SolidComponent>]
let CounterApp () =
    let count, setCount = createSignal 0
    let store = CounterStore(count, (fun () -> setCount (count () + 1)))

    CounterContext store {
        div (class' = "counter-app") {
            section () { CounterDisplay() }
            footer () { CounterButton() }
        }
    }
