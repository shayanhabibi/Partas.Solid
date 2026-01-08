module DerivedPrevVal

open Partas.Solid.TanStack.Store

let count = new Store<int> (1)

let runningTotal =
    new Derived<int> (
        DerivedOptions (
            (fun props ->
                let prev =
                    props.prevVal
                    |> Option.defaultValue 0

                count.state
                + prev),
            [| count |]
        )
    )

let unmount = runningTotal.mount ()
