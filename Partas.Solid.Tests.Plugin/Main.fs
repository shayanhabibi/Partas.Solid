module Partas.Solid.Tests.Plugin.Main

open Expecto
open Partas.Solid.Tests.Plugin.Common
open Partas.Solid.Tests.Plugin.IssueTests

[<EntryPoint>]
let main argv =
    Tests.runTestsInAssemblyWithCLIArgs [] argv
