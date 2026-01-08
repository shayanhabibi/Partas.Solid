module Subscribe

open Partas.Solid.TanStack.Store
open Fable.Core.JS

let store = new Store<int> (0)

let unsub = store.subscribe (fun () -> console.log ("State changed:", store.state))

let cleanup () = unsub ()
