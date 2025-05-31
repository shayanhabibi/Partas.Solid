module Partas.Solid.Tests.SolidCases.ExperimentalBuilders

open Partas.Solid
open Partas.Solid.Experimental
open Fable.Core
open Fable.Core.JS

[<JS.Pojo>]
type MyObject(value: int, label: string) =
    member val value = value with get,set
    member val label = label with get,set

let getter,setter = createSignal(10)
let data,store = createStore<MyObject ResizeArray>(ResizeArray([||]))

effect {
    if getter() = 10 then
        store.Update(ResizeArray([||]))
    else
        setter 10
}
let getGetter = lambda {
    let mutable check = 10
    match getter() with
    | 10 -> check <- 5
    | 5 -> check <- 10
    check
}

let getSetter = lambda {
    store.Update(fun o -> o.Add(MyObject(5, "Test")); o)
    if data.Count > 0 then
        setter
}

[<SolidComponent>]
let ComponentWrap () =
    effect {
        if getter() = 10 then
            store.Update(ResizeArray([||]))
        else
            setter 10
    }
    let getGetter = lambda {
        let mutable check = 10
        match getter() with
        | 10 -> check <- 5
        | 5 -> check <- 10
        check
    }

    let getSetter = lambda {
        store.Update(fun o -> o.Add(MyObject(5, "Test")); o)
        if data.Count > 0 then
            setter
    }
    
    if getter() = 0 then getGetter
    else unbox getSetter
    
[<Erase>]
type TestComponent() =
    interface RegularNode
    [<SolidTypeComponent>]
    member props.__ =
        let childs = children { props.children }
        effect {
            printfn "effect"
        }
        cleanup {
            printfn "cleanup"
        }
        mount {
            printfn "mount"
        }
        let x = memo {
            printfn "memo"
            "memo"
        }
        let submission = batch {
            printfn ""
            childs()
        }
        let isSelected = selector {
            false
        }
        
        div() { childs() }
    
