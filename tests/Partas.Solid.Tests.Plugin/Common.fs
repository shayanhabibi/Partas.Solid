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
        Command "dotnet fable --exclude Partas.Solid.FablePlugin --noCache -e .fs.jsx -c Release --optimize"
    }
    |> Command.execute
    |> fun output ->
        shouldEqual output.Error None
        shouldEqual output.ExitCode 0

type ExpectedFile = FileInfo
type SourceFile = FileInfo
type TestFilePair = ExpectedFile * SourceFile
type TestCase = {
    Path: string array
    TestFiles: TestFilePair array
}

[<Literal>]
let caseFolderSuffix = "Cases"

let findTestCases () =
    let expectedFiles, _errors =
        DirectoryInfo($"{__SOURCE_DIRECTORY__}/Compiled/")
            .EnumerateFiles("*.expected", SearchOption.AllDirectories)
        |> Seq.toArray
        |> Array.partition _.Exists
    expectedFiles.Length |> printfn "%i"
    let expectedRoots, _sourceNotFound =
        expectedFiles
        |> Array.map (fun file ->
            file.FullName
            |> fun s ->
                file,
                s.Substring(0, s.Length - ".expected".Length) + ".fs.jsx"
                |> FileInfo
            )
        |> Array.partition (snd >> _.Exists)
    let cases, _caseErrors =
        let rootLength = DirectoryInfo($"{__SOURCE_DIRECTORY__}/Compiled/").FullName.Length
        let removeRoot (dir: string) = dir.Substring rootLength
        expectedRoots
        |> Array.groupBy (fst >> _.DirectoryName)
        |> Array.map (fun (dir, testFilePairs) ->
            {
                Path = dir |> removeRoot |> _.Split(Path.DirectorySeparatorChar)
                TestFiles = testFilePairs
            })
        |> Array.partition (_.Path.Length >> (<) 1)
    cases

let createTestCases (case: TestCase) =
    let testCategory, testCaseName = case.Path[0], case.Path[1]
    match case.TestFiles with
    | [| expected, source |] ->
        ($"{testCategory}.{testCaseName}", fun () ->
            let source = File.ReadAllText source.FullName |> _.Trim()
            let expected = File.ReadAllText expected.FullName |> _.Trim()
            Expecto.Expect.equalWithDiffPrinter Expecto.Expect.defaultDiffPrinter source expected "")
        |> Array.singleton
    | cases ->
        cases
        |> Array.map (fun (expected, source) ->
            $"{testCategory}.{testCaseName}.{source.Name}", fun _ ->
                let source = File.ReadAllText source.FullName |> _.Trim()
                let expected = File.ReadAllText expected.FullName |> _.Trim()
                Expecto.Expect.equalWithDiffPrinter Expecto.Expect.defaultDiffPrinter source expected "")

let runCase folderName caseName =
    let dir = $"{__SOURCE_DIRECTORY__}/Compiled/{folderName}"
    let readLinesOf =
        fun (s: string) ->
            File.ReadAllLines $"{dir}/{caseName}/{caseName}{s}"
            |> Array.filter ((<>) "")
            |> String.concat "\n"

    let result = readLinesOf ".fs.jsx"
    let expected = readLinesOf ".expected"

    let printer = Expecto.Expect.defaultDiffPrinter
    Expecto.Expect.equalWithDiffPrinter printer result expected ""
