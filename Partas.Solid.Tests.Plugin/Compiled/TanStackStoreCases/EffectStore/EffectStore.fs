module EffectStore

open Partas.Solid
open Partas.Solid.TanStack.Store
open Fable.Core

let store = new Store<int> (0)

let effectOptions =
    EffectOptions ((fun () -> JS.console.log ("Store changed:", store.state)), [| store |])

let storeEffect = new Effect (effectOptions)

[<SolidComponent>]
let Component () =
    let count = useStore store (fun s -> s)

    div () {
        span () { string (count ()) }
        button().on ("click", fun _ -> store.setState (fun s -> s + 1)) { "Increment" }
    }
