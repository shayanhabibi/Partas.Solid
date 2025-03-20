module Partas.Solid.Tests.Common

open Fli
open FsUnitTyped
open System.IO
open Xunit

// let START_TEST (name: string) = name; ()
// let END_TEST (name: string) = name; ()
//
// let (|StartsWithTrimmed|_|) (comp: string) : string -> string option = function
//     | value when value.StartsWith comp -> value.Substring(comp.Length, value.Length) |> Some
//     | _ -> None
// let (|Until|_|) (comp: char): string -> string option = function
//     | line when line.Contains(comp) ->
//         line
//         |> _.Split(comp)
//         |> Array.tryHead
//     | _ -> None
// let (|TestStart|_|): string -> string option = function
//     | StartsWithTrimmed "START_TEST(" (Until ')' name) ->
//         Some name
//     | _ -> None
// let (|TestEnd|_|): string -> unit option = function
//     | StartsWithTrimmed "END_TEST" _ -> Some()
//     | _ -> None
//
// let mutable testName = "None"
//
// let groupingFunctor: string -> string = function
//     | TestStart name ->
//         testName <- name
//         "Dispose"
//     | TestEnd ->
//         testName <- "None"
//         "Dispose"
//     | _ -> testName
//     
// let getTestBlocks = File.ReadAllLines("") |> Array.groupBy groupingFunctor

// type TestFolderFixture(folder: string) =
//     let dir = $"{__SOURCE_DIRECTORY__}/{folder}"
//     do
//         cli {
//             Shell CMD
//             WorkingDirectory dir
//             Command "dotnet fable --exclude Partas.Solid.FablePlugin --noCache -e .fs.jsx"
//         }
//         |> Command.execute
//         |> Output.toExitCode
//         |> shouldEqual 1
//
// [<CollectionDefinition("Build Collection")>]
// type TestFolderCollection = ICollectionFixture<TestFolderFixture>


let runCase folderName caseName =
    let dir = $"{__SOURCE_DIRECTORY__}/{folderName}/{caseName}"
    cli {
        Shell CMD
        WorkingDirectory dir
        Command "dotnet fable --exclude Partas.Solid.FablePlugin --noCache -e .fs.jsx"
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