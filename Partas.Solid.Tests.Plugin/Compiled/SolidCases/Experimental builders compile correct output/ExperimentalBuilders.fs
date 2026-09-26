module Partas.Solid.Tests.SolidCases.ExperimentalBuilders

open Partas.Solid
open Partas.Solid.Experimental
open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop

[<JS.Pojo>]
type MyObject(value: int, label: string) =
    member val value = value with get, set
    member val label = label with get, set

let getter, setter = createSignal (10)
let data, store = createStore<MyObject ResizeArray> (ResizeArray ([||]))

effect {
    let! value = getter

    if value = 10 then
        store (fun _ -> ResizeArray ([||]))
    else
        setter 10
}

let getGetter =
    lambda {
        let mutable check = 10

        match getter () with
        | 10 -> check <- 5
        | 5 -> check <- 10
        | _ -> ()

        check
    }

let getSetter =
    lambda {
        store (fun o ->
            o.Add (MyObject (5, "Test"))
            o)

        if data.Value.Count > 0 then
            setter
    }

let LazyTest: LazyComponent<HtmlElement> =
    lazyload { importDynamic "./ExperimentalBuilders.fs.jsx" }

[<SolidComponent>]
let ComponentWrap () =
    effect {
        let! value = getter

        if value = 10 then
            store (fun _ -> ResizeArray ([||]))
        else
            setter 10
    }

    let getGetter =
        lambda {
            let mutable check = 10

            match getter () with
            | 10 -> check <- 5
            | 5 -> check <- 10
            | _ -> ()

            check
        }

    let getSetter =
        lambda {
            store (fun o ->
                o.Add (MyObject (5, "Test"))
                o)

            if data.Value.Count > 0 then
                setter
        }

    if getter () = 0 then getGetter else unbox getSetter

[<Erase>]
type TestComponent() =
    interface RegularNode

    [<SolidTypeComponent>]
    member props.__ =
        let childs = children { props.children }

        effect {
            let! count, length = lambda { getter (), data.Value.Count }
            console.log ("effect", count, length)
        }

        cleanup { printfn "cleanup" }
        mount { printfn "mount" }

        let x =
            memo {
                printfn "memo"
                "memo"
            }

        let track = reaction { printfn "reaction" }
        track (fun () -> box (getter ()))

        div () { childs.Invoke () }
