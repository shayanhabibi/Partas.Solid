module Partas.Solid.Tests.Plugin.Compiled.IssueCases.ObjectEventHandler.ObjectEventHandler

open Partas.Solid
open Fable.Core

[<SolidComponent>]
let test () =
    div().on("event", OnHandler((fun e -> JS.console.log(e)), passive = true))

