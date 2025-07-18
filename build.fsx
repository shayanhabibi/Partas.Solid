#r "nuget: Fake.Core.Target"
#r "nuget: Fake.Core.Process"
#r "nuget: Fake.Core.ReleaseNotes"
#r "nuget: Fake.IO.FileSystem"
#r "nuget: Fake.DotNet.Cli"
#r "nuget: Fake.DotNet.MSBuild"
#r "nuget: Fake.DotNet.AssemblyInfoFile"
#r "nuget: Fake.Tools.Git"
#r "nuget: Fake.Api.GitHub"
#r "nuget: Fake.DotNet.Testing.Coverlet"

System.Environment.GetCommandLineArgs()
|> Array.skip 2
|> Array.toList
|> Fake.Core.Context.FakeExecutionContext.Create false __SOURCE_FILE__
|> Fake.Core.Context.RuntimeContext.Fake
|> Fake.Core.Context.setExecutionContext

open Fake
open Fake.Core.TargetOperators
open Fake.Core
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.DotNet
open Fake.DotNet.Testing
open Fake.Tools
open System.IO

Target.initEnvironment()

let description = "F# Fable front-end framework; derived from Oxpecker.Solid; built on top of Solid.js"
let gitOwner = "shayanhabibi"
let gitName = "Partas.Solid"
let release =
    try
        ReleaseNotes.load "docs/RELEASE_NOTES.md"
    with e ->
        ReleaseNotes.ReleaseNotes.New("0.1.0", "0.1.0", [
            "Initialising Build Scripts"
        ])
let version = release.AssemblyVersion

module Ops =
    let Clean = "Clean"
    let Build = "Build"
    let AssemblyInfo = "AssemblyInfo"
    let Test = "RunTests"

// Generate assembly info file with versioning and up-to-date info
Target.create Ops.AssemblyInfo (fun _ ->
    let fileName = "Common/AssemblyInfo.fs"
    AssemblyInfoFile.createFSharp
        fileName
        [
            AssemblyInfo.Title gitName
            AssemblyInfo.Product gitName
            AssemblyInfo.Version version
            AssemblyInfo.FileVersion version
        ]
    )

Target.create Ops.Clean (fun _ ->
    !! "**/**/bin" |> Shell.cleanDirs
    Shell.cleanDirs [
        "bin"
        "temp"
    ]
    )

let makeArgs: string seq -> string = String.concat " "

let dotnet cmd args =
    match DotNet.exec id cmd (makeArgs args) with
    | result when not result.OK ->
        failwith $"Failed: {result.Errors}"
    | _ -> ()
Target.create Ops.Build (fun _ -> dotnet "build" [
    "Partas.Solid.sln"
    "-c Release"
])

Target.create Ops.Test (fun _ ->
    "Partas.Solid.Tests"
    |> DotNet.test (fun opts ->

        Coverlet.withDotNetTestOptions
            (id)
            { opts with
                NoBuild = true
                NoRestore = true }
        )
    // |> DotNet.test id
    )


Ops.Clean
==> Ops.AssemblyInfo
==> Ops.Build
==> Ops.Test

Target.runOrDefault Ops.Test
