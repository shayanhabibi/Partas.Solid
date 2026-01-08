module MountUnmount

open Partas.Solid
open Partas.Solid.TanStack.Store
open Fable.Core.JS

let store = new Store<int> (0)

let derived =
    new Derived<int> (
        DerivedOptions (
            (fun _ ->
                store.state
                * 2),
            [| store |]
        )
    )

let effect =
    new Effect (EffectOptions ((fun () -> console.log (store.state)), [| store |]))

[<SolidComponent>]
let Component () =
    let derivedUnmount = derived.mount ()
    let effectUnmount = effect.mount ()

    onCleanup (fun () ->
        derivedUnmount ()
        effectUnmount ())

    div () { string derived.state }
