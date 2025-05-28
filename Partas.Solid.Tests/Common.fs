module Partas.Solid.Tests.Common

open Fli
open FsUnitTyped
open System.IO
open System
open Xunit

let platformShell =
    match Environment.OSVersion.Platform with
    | PlatformID.Unix | PlatformID.MacOSX -> ZSH
    | _ -> CMD

let runCase folderName caseName =
    let dir = $"{__SOURCE_DIRECTORY__}/{folderName}/{caseName}"
    let checkOutput = fun output ->
        output |> Output.printError
        output |> Output.toExitCode
    cli {
        Shell platformShell
        WorkingDirectory dir
        Command "dotnet fable --exclude Partas.Solid.FablePlugin --noCache -e .fs.jsx"
    }
    |> Command.execute
    |> checkOutput
    |> shouldEqual 0
    
    let readLinesOf = fun (s: string) -> File.ReadAllLines($"{dir}/{caseName}{s}") |> Array.filter ((<>) "")
    let result = readLinesOf ".fs.jsx"
    let expected = readLinesOf ".expected"
    result |> shouldEqual expected
