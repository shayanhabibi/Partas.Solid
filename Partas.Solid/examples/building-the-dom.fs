module Partas.Solid.examples.building_the_dom

open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop
open Fable.Core.JS

[<Erase>]
type ReactiveComponent() =
    interface RegularNode
    [<DefaultValue>]
    val mutable myAttribute: bool
    [<SolidTypeComponent>]
    member props.__ =
        let buttonText () =
            if props.myAttribute then
                "MyComponent"
            else "Button"
        button() { buttonText() }

[<Erase>]
type UnreactiveComponent() =
    interface RegularNode
    [<DefaultValue>]
    val mutable myAttribute: bool
    [<SolidTypeComponent>]
    member props.__ =
        let buttonText =
            if props.myAttribute then
                "MyComponent"
            else "Button"
        button() { buttonText }

[<SolidComponent>]
let MyComponentExample() =
    let sign,setSign = createSignal false
    div() {
        button(onClick = (fun _ -> sign() |> not |> setSign)) { "Click me!" }
        ReactiveComponent(myAttribute = sign())
        UnreactiveComponent(myAttribute = sign())
    }

[<SolidComponent>]
let Conditional () =
    let truthy,setTruthy = createSignal false
    div() {
        button(onClick = fun _ -> setTruthy(truthy() |> not)) { "Click me!" }
        if truthy() then
            "Boo!"
        Show(when'=not (truthy())) {
            "Do you hear that?"
        }
    }
