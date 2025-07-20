#r "nuget: Fake.Core.Target"
#r "nuget: Fake.Core.Process"
#r "nuget: Fake.Core.ReleaseNotes"
#r "nuget: Fake.IO.FileSystem"
#r "nuget: Fake.DotNet.Cli"
#r "nuget: Fake.DotNet.MSBuild"
#r "nuget: Fake.DotNet.AssemblyInfoFile"
#r "nuget: Fake.Tools.Git"
#r "nuget: Fake.Api.GitHub"
#r "nuget: Fake.DotNet.Testing.Expecto"

System.Environment.GetCommandLineArgs ()
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

Target.initEnvironment ()

module Ops =
    [<Literal>]
    let Clean = "Clean"

    [<Literal>]
    let RestoreTools = "RestoreTools"

    [<Literal>]
    let Nuget = "NuGet"

    [<Literal>]
    let Publish = "Publish"

    [<Literal>]
    let Build = "Build"

    [<Literal>]
    let AssemblyInfo = "AssemblyInfo"

    [<Literal>]
    let Test = "RunTests"

    [<Literal>]
    let GitCliff = "GitCliff"

    [<Literal>]
    let PublishLocal = "PublishLocal"

    [<Literal>]
    let Format = "Format"

    [<Literal>]
    let CheckFormat = "CheckFormat"

    [<Literal>]
    let ReleaseNotes = "ReleaseNotes"

let description =
    "F# Fable front-end framework; derived from Oxpecker.Solid; built on top of Solid.js"

let gitOwner = "shayanhabibi"
let gitName = "Partas.Solid"
let release = lazy ReleaseNotes.load "docs/RELEASE_NOTES.md"

let apiKey =
    Target.getArguments ()
    |> Option.bind (fun args ->
        let idx =
            args
            |> (Array.tryFindIndex ((=) "--nuget-api-key")
                >> Option.map ((+) 1))

        idx
        |> Option.map (Array.get args))

let sourceFiles =
    !!"**/*.fs"
    ++ "**/*.fsx"
    -- "packages/**/*.*"
    -- "paket-files/**/*.*"
    -- ".fake/**/*.*"
    -- "**/obj/**/*.*"
    -- "**/AssemblyInfo.fs"
    -- "**/IndexAccess/IndexAccess.fs"
    -- "Partas.Solid.FablePlugin/Plugin.fs"

Target.create Ops.Format (fun _ ->
    let result =
        sourceFiles
        |> Seq.map (sprintf "\"%s\"")
        |> String.concat " "
        |> DotNet.exec id "fantomas"

    if not result.OK then
        Trace.log $"Errors while formatting all files: %A{result.Messages}")

Target.create Ops.CheckFormat (fun _ ->
    let errorAction =
        if Git.Information.getBranchName "." = "master" then
            Trace.traceImportant
        else
            failwith

    let result =
        sourceFiles
        |> Seq.map (sprintf "\"%s\"")
        |> String.concat " "
        |> sprintf "%s --check"
        |> DotNet.exec id "fantomas"

    if result.ExitCode = 0 then
        Trace.log "No files need formatting"
    elif result.ExitCode = 99 then
        errorAction "Some files need formatting, run `dotnet fsi build.fsx target Format` to format them."
    else
        Trace.logf $"Errors while formatting: %A{result.Errors}"
        errorAction "Unknown errors while formatting")

Target.create Ops.GitCliff (fun _ ->
    { ExecParams.Empty with
        Program = "git-cliff" }
    |> Process.shellExec
    |> function
        | 0 -> ()
        | code -> failwith $"Git-cliff failed with code: {code}")

// Generate assembly info file with versioning and up-to-date info
Target.create Ops.AssemblyInfo (fun _ ->
    let fileName = "Common/AssemblyInfo.fs"

    AssemblyInfoFile.createFSharp
        fileName
        [ AssemblyInfo.Title gitName
          AssemblyInfo.Product gitName
          AssemblyInfo.Version release.Value.AssemblyVersion
          AssemblyInfo.FileVersion release.Value.AssemblyVersion ])

Target.create Ops.Clean (fun _ ->
    !!"**/**/bin"
    |> Shell.cleanDirs

    Shell.cleanDirs [ "bin"; "temp" ])

let makeArgs: string seq -> string = String.concat " "

let dotnet cmd args =
    match DotNet.exec id cmd (makeArgs args) with
    | result when not result.OK -> failwith $"Failed: {result.Errors}"
    | _ -> ()

Target.create Ops.Build (fun _ ->
    "Partas.Solid.sln"
    |> DotNet.build (fun p ->
        { p with
            Configuration = DotNet.BuildConfiguration.Release
            DotNet.BuildOptions.MSBuildParams.DisableInternalBinLog = true
            DotNet.BuildOptions.MSBuildParams.Properties =
                [ "PackageVersion", release.Value.AssemblyVersion
                  "Version", release.Value.AssemblyVersion ] }))

Target.create Ops.Test (fun _ ->
    !!"**/bin/**/*.Tests.Plugin.dll"
    |> Testing.Expecto.run (fun p ->
        { p with
            Summary = true
            CustomArgs =
                [ "--colours 256" ]
                @ p.CustomArgs }))

Target.create Ops.RestoreTools (fun _ ->
    let result = DotNet.exec id "tool" "restore"

    result.Messages
    |> Trace.logItems "Tool Restore"

    if not result.OK then
        failwith "Failed to restore dotnet tools")

Target.create Ops.Nuget (fun _ ->
    [ "Partas.Solid"; "Partas.Solid.FablePlugin" ]
    |> List.iter (
        DotNet.pack (fun p ->
            { p with
                NoRestore = true
                OutputPath = Some "bin"
                DotNet.PackOptions.MSBuildParams.DisableInternalBinLog = true
                DotNet.PackOptions.MSBuildParams.Properties =
                    [ "PackageVersion", release.Value.AssemblyVersion
                      "Version", release.Value.AssemblyVersion ] })
    ))

Target.create Ops.Publish (fun _ ->
    !!"bin/*.nupkg"
    |> Seq.iter (
        DotNet.nugetPush (fun p ->
            { p with
                DotNet.NuGetPushOptions.PushParams.ApiKey = apiKey
                DotNet.NuGetPushOptions.PushParams.Source = Some "https://api.nuget.org/v3/index.json"
                DotNet.NuGetPushOptions.Common.CustomParams = Some "--skip-duplicate" })
    ))

Target.create Ops.PublishLocal (fun _ ->
    !!"bin/*.nupkg"
    |> Seq.iter (
        DotNet.nugetPush (fun p ->
            { p with
                DotNet.NuGetPushOptions.PushParams.Source = Some "local"
                DotNet.NuGetPushOptions.PushParams.PushTrials = 1 })
    ))

Target.create Ops.ReleaseNotes (fun _ ->
    Git.FileStatus.getAllFiles "./docs"
    |> Seq.iter (function
        | Git.FileStatus.FileStatus.Modified, "RELEASE_NOTES.md" -> Git.Commit.execExtended "./docs" "[skip ci]" "docs: Update RELEASE_NOTES.md"
        | _ -> ()))

Ops.GitCliff
==> Ops.AssemblyInfo
?=> Ops.Build

Ops.AssemblyInfo
==> Ops.Nuget

Ops.Test
==> Ops.Nuget
==> Ops.Publish

Ops.GitCliff
==> Ops.ReleaseNotes
==> Ops.Publish

Ops.Test
==> Ops.Nuget
==> Ops.PublishLocal

Ops.Clean
==> Ops.Build
==> Ops.Test

Ops.RestoreTools
==> Ops.Test

Ops.CheckFormat
==> Ops.Build

Target.runOrDefaultWithArguments Ops.Test
