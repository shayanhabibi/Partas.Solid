module Partas.Solid.Tests.Plugin.Common

open Fli
open System
open System.IO
open FsUnitTyped

let platformShell =
    match Environment.OSVersion.Platform with
    | PlatformID.Unix -> BASH
    | PlatformID.MacOSX -> ZSH
    | _ -> CMD

let buildCases () =
    let dir = $"{__SOURCE_DIRECTORY__}/Compiled/"

    cli {
        Shell platformShell
        WorkingDirectory dir
        Command "dotnet fable --exclude Partas.Solid.FablePlugin --noCache -e .fs.jsx"
    }
    |> Command.execute
    |> fun output ->
        shouldEqual output.Error None
        shouldEqual output.ExitCode 0

let runCase folderName caseName =
    let dir = $"{__SOURCE_DIRECTORY__}/Compiled/{folderName}"

    let readLinesOf =
        fun (s: string) ->
            File.ReadAllLines ($"{dir}/{caseName}/{caseName}{s}")
            |> Array.filter ((<>) "")

    let result = readLinesOf ".fs.jsx"
    let expected = readLinesOf ".expected"

    result
    |> shouldEqual expected
