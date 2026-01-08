module EffectEager

open Partas.Solid.TanStack.Store
open Fable.Core.JS

let store = new Store<int> (0)

let eagerEffect =
    let opts =
        EffectOptions ((fun () -> console.log ("Immediate effect:", store.state)), [| store |])

    opts.eager <- true
    new Effect (opts)

let unmount = eagerEffect.mount ()
