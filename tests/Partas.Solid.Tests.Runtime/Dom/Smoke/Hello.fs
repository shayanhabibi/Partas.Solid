module Partas.Solid.Tests.Runtime.Dom.Smoke.Hello

open Partas.Solid
open Fable.Core

[<SolidComponent>]
let Hello () = div (class' = "x") { "hello" }
