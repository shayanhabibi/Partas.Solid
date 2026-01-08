module DerivedStore

open Partas.Solid
open Partas.Solid.TanStack.Store

let countStore = new Store<int> (5)

let doubledOptions =
    DerivedOptions (
        (fun props ->
            let current = countStore.state

            current
            * 2),
        [| countStore |]
    )

let doubled = new Derived<int> (doubledOptions)

[<SolidComponent>]
let Component () =
    let count = useStore countStore (fun s -> s)

    div () {
        span () { $"Count: {count ()}" }
        span () { $"Doubled: {doubled.state}" }
        button().on ("click", fun _ -> countStore.setState (fun s -> s + 1)) { "Increment" }
    }
