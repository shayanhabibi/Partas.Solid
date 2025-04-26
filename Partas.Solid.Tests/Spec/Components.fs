module Partas.Solid.Tests.Spec.Components

open Partas.Solid

/// Let bindings compile to functions
[<SolidComponent>]
let Component () =
    div()
[<SolidComponentAttribute>]
let WrappingComponent () =
    Component()
/// They do not compile when not used within another
/// SolidComponent or SolidTypeComponent
let FailsToWrap () =
    Component()
/// They do not work without being a function within f#
[<SolidComponent>]
let NotAFunction =
    Component()

[<SolidComponent>]
let WrapNotAFunction () =
    NotAFunction
/// Parameters can be passed
[<SolidComponent>]
let ParameterComponent someInt someString someHandler =
    let thisInt = someInt
    let thisString = someString
    let thisHandler = someHandler
    div()
[<SolidComponent>]
let WrapParameterComponent () =
    ParameterComponent 5 "HeyThere" (fun () -> ())
