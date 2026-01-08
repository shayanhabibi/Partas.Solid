module UseStoreFull

open Partas.Solid
open Partas.Solid.TanStack.Store

let store = new Store<int> (42)

[<SolidComponent>]
let Component () =
    let state = useStoreFull store

    div () { string (state ()) }
