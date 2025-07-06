module Partas.Solid.examples.overview

open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop

[<Erase>]
type ButtonVariant =
    | Primary
    | Ghost

[<SolidComponent>]
let Button (title: string) (variant: ButtonVariant) =
    button(style = match variant with
                    | Primary -> "background-color: var(--sb-important-background-color)"
                    | Ghost -> "") { title }

[<SolidComponent>]
let ButtonExample () =
    let variant, setVariant = createSignal(Ghost)
    div() {
        Button "Primary" Primary
        Button "Ghost" Ghost
        Button "Responsive" (variant())
        button(onClick = fun _ ->
            if variant() = Primary
            then Ghost
            else Primary
            |> setVariant) { "Click me" }
    }

[<Erase>]
type MyComponent() =
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

[<SolidComponent>]
let MyComponentExample() =
    let sign,setSign = createSignal false
    div() {
        button(onClick = (fun _ -> sign() |> not |> setSign)) { "Click me!" }
        MyComponent(myAttribute = sign())
    }
