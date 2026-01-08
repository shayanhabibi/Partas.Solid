module RecordState

open Partas.Solid
open Partas.Solid.TanStack.Store

type AppState = { count: int; name: string }

let store = new Store<AppState> ({ count = 0; name = "Test" })

[<SolidComponent>]
let Component () =
    let count = useStore store (fun s -> s.count)
    let name = useStore store (fun s -> s.name)

    div () {
        span () { name () }
        span () { string (count ()) }

        button()
            .on (
                "click",
                fun _ ->
                    store.setState (fun s ->
                        { s with
                            count =
                                s.count
                                + 1 })
            ) {
            "Increment"
        }
    }
