module DerivedDepVals

open Partas.Solid.TanStack.Store

let count = new Store<int> (1)

let sumWithPrev =
    new Derived<int> (
        DerivedOptions (
            (fun props ->
                let prevDep =
                    props.prevDepVals
                    |> Option.map (fun arr -> unbox<int> arr[0])
                    |> Option.defaultValue 0

                let currDep = unbox<int> props.currDepVals[0]

                prevDep
                + currDep),
            [| count |]
        )
    )

let unmount = sumWithPrev.mount ()
