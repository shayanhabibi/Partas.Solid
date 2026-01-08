module BasicStore

open Partas.Solid
open Partas.Solid.TanStack.Store

let store = new Store<int> (0)

[<SolidComponent>]
let Component () =
    let count = useStore store (fun state -> state)

    div().on ("click", fun _ -> store.setState (fun s -> s + 1)) { string (count ()) }
