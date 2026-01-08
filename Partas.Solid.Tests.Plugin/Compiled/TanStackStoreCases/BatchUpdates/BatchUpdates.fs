module BatchUpdates

open Partas.Solid.TanStack.Store

let store = new Store<int> (0)

let performBatchUpdate () =
    batch (fun () ->
        store.setState (fun _ -> 1)
        store.setState (fun _ -> 2)
        store.setState (fun s -> s + 1))
