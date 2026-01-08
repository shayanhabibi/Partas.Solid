module StoreOptions

open Partas.Solid.TanStack.Store
open Fable.Core.JS
open Fable.Core.JsInterop

let store =
    new Store<int> (
        12,
        !!{| updateFn =
            fun prev updater ->
                updater prev
                + prev |}
    )

let storeWithOnUpdate =
    new Store<int> (0, !!{| onUpdate = fun () -> console.log ("Updated!") |})
