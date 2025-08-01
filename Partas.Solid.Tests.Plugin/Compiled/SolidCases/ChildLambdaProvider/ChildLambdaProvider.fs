﻿module Partas.Solid.Tests.SolidCases.ChildLambdaProvider

open Fable.Core
open Partas.Solid

[<Import("SomeComponent", "FakeLibrary")>]
type CustomComponent() =
    interface VoidNode
    interface ChildLambdaProvider3<int, Accessor<string>, float>

[<Import("OnlyTakesButtons", "FakeLibrary")>]
type OnlyButtons() =
    interface VoidNode
    interface ChildLambdaProviderStrict<int, button>

[<SolidComponent>]
let TestProvider () =
    CustomComponent () {
        yield
            fun i s f ->
                Fragment () {
                    div (tabindex = i) { s () }
                    OnlyButtons () { yield fun b -> button (tabindex = b) }
                }
    }
