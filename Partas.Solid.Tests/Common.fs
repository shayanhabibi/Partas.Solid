module Partas.Solid.Tests.Common

open Fli
open FsUnitTyped
open System.IO

let runCase folderName caseName =
    let dir = $"{__SOURCE_DIRECTORY__}/{folderName}/{caseName}"
    cli {
        Shell CMD
        WorkingDirectory dir
        Command "fable --exclude Partas.Solid.FablePlugin --noCache -e .fs.jsx"
    }
    |> Command.execute
    |> fun output ->
        output
        |> Output.printError
        output
        |>Output.toExitCode
        |> shouldEqual 0
    let readLinesOf = fun (s: string) -> File.ReadAllLines($"{dir}/{caseName}{s}") |> Array.filter ((<>) "")
    let result = readLinesOf ".fs.jsx"
    let expected = readLinesOf ".expected"
    result |> shouldEqual expected