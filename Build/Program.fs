module Build

open Fake.DotNet
open Spec
open Fake.Core
open Fake.IO
open Fake.Tools
open Fake.IO.Globbing.Operators

// disable warning of implicit conversion of ops to string
#nowarn 3391

initializeContext ()

let private root = Root.``.``

open FSharp.SystemCommandLine

let release = lazy ReleaseNotes.load "docs/RELEASE_NOTES.md"

module Prelude =
    let restoreTools =
        ActionPath.createCondNot Options.quick
        <| fun _ -> dotnet [ "tool"; "restore"; "--verbosity"; "q" ] root

    let restoreSolution =
        ActionPath.createCondNot Options.quick
        <| fun _ ->
            DotNet.restore
                (fun p ->
                    { p with
                        DotNet.RestoreOptions.MSBuildParams.DisableInternalBinLog = true })
                Solutions.Partas

module HouseKeeping =
    let clean: ActionPath =
        ActionPath.createCondNot Options.quick
        <| fun _ ->
            !!"**/**/bin"
            ++ "temp"
            -- "bin"
            |> Shell.cleanDirs

    let fableClean: ActionPath =
        ActionPath.createCondNot Options.quick
        <| fun _ ->
            let func = fable [ "clean"; "-e"; ".fs.jsx"; "--yes" ]

            [| Projects.Directory.Plugin.``.``
               Projects.Directory.Solid.``.``
               Tests.Directory.Plugin.``.``
               Tests.Directory.Core.``.`` |]
            |> Array.Parallel.iter func

    let private formatImpl () =
        sourceFiles
        -- "Partas.Solid.FablePlugin/Plugin.fs"
        -- "**/IndexAccess/IndexAccess.fs"
        |> Seq.map (sprintf "\"%s\"")
        |> String.concat " "
        |> DotNet.exec id "fantomas"
        |> function
            | result when result.OK -> ()
            | result -> Trace.log $"Errors while formatting all files: %A{result.Messages}"

    let private formatCheckImpl () =
        sourceFiles
        -- "Partas.Solid.FablePlugin/Plugin.fs"
        -- "**/IndexAccess/IndexAccess.fs"
        |> Seq.map (sprintf "\"%s\"")
        |> String.concat " "
        |> sprintf "%s --check"
        |> DotNet.exec id "fantomas"
        |> function
            | result when result.OK -> ()
            | result when result.ExitCode = 99 -> failwith "Some files need formatting"
            | result -> failwith $"Errors while checking formatting of all files: %A{result.Messages}"

    let formatCommand: ActionPath =
        ActionPath.createFor Options.dryFormat (function
            | true -> formatCheckImpl ()
            | _ -> formatImpl ())

    let format: ActionPath = ActionPath.createCond Options.format formatImpl

    let dryFormatCommand: ActionPath =
        ActionPath.create (
            ignore
            >> formatCheckImpl
        )

    let dryFormat: ActionPath =
        ActionPath.createFor' (Options.format, Options.dryFormat)
        <| function
            | true, _
            | _, false -> ()
            | _, true -> formatCheckImpl ()

let private getConfig: string -> DotNet.BuildConfiguration =
    function
    | "Debug" -> DotNet.BuildConfiguration.Debug
    | _ -> DotNet.BuildConfiguration.Release

module ProjectManagement =
    let build: ActionPath =
        ActionPath.createFor
            Options.config
            (getConfig
             >> fun config ->
                 [ Projects.FsProj.Plugin; Projects.FsProj.Solid ]
                 |> List.iter (
                     DotNet.build (fun p ->
                         { p with
                             Configuration = config
                             DotNet.BuildOptions.MSBuildParams.DisableInternalBinLog = true
                             DotNet.BuildOptions.MSBuildParams.Properties =
                                 [ "PackageVersion", release.Value.AssemblyVersion
                                   "Version", release.Value.AssemblyVersion ] })
                 ))

    let pack: ActionPath =
        ActionPath.create
        <| fun _ ->
            [ Projects.FsProj.Plugin; Projects.FsProj.Solid ]
            |> List.iter (
                DotNet.pack (fun p ->
                    { p with
                        NoRestore = true
                        OutputPath = Some "bin"
                        DotNet.PackOptions.MSBuildParams.DisableInternalBinLog = true
                        DotNet.PackOptions.MSBuildParams.Properties =
                            [ "PackageVersion", release.Value.AssemblyVersion
                              "Version", release.Value.AssemblyVersion ] })
            )

    let publish: ActionPath =
        let inline publishToSourceWithKey apiKey source =
            !!"bin/*.nupkg"
            |> Seq.iter (
                DotNet.nugetPush (fun p ->
                    { p with
                        DotNet.NuGetPushOptions.PushParams.ApiKey = apiKey
                        DotNet.NuGetPushOptions.PushParams.Source = Some source
                        DotNet.NuGetPushOptions.Common.CustomParams = Some "--skip-duplicate" })
            )

        let inline publishToSource source =
            publishToSourceWithKey None source

        ActionPath.createFor Options.NuGet.key
        <| function
            | None ->
                Trace.traceImportant "No NuGet API key provided. Publishing to local feed if it exists."

                "local"
                |> publishToSource
            | Some _ as apiKey ->
                "https://api.nuget.org/v3/index.json"
                |> publishToSourceWithKey apiKey

module Tests =
    let build = ActionPath.createFor' (Options.skipTests, Options.config) <| function
        | true, _ -> ()
        | _, config ->
            let config = getConfig config
            [ Tests.FsProj.Plugin; Tests.FsProj.Core ]
            |> List.iter (
                DotNet.build (fun p ->
                    { p with
                        Configuration = config
                        DotNet.BuildOptions.MSBuildParams.DisableInternalBinLog = true })
            )
    let run = ActionPath.createCondNot Options.skipTests <| fun _ ->
        !! "**/bin/**/*.Tests.*.dll"
        |> Testing.Expecto.run (fun p -> { p with Summary = true; CustomArgs = "--colours 256" :: p.CustomArgs })

    let scratch = ActionPath.createFor Options.watch <| function
        | false -> fable [ "--exclude"; "Partas.Solid.FablePlugin"; "--noCache"; "-e"; ".fs.jsx"; "--optimize" ] Tests.Directory.Scratch.``.``
        | true -> fable [ "--exclude"; "Partas.Solid.FablePlugin"; "--noCache"; "-e"; ".fs.jsx"; "--watch"; "--optimize" ] Tests.Directory.Scratch.``.``

module Commands =
    let format =
        command "format" {
            description "Formats all source files"
            inputs Input.context
            addInput Options.dryFormat
            setAction (ActionPath.doBatch [ Prelude.restoreTools; HouseKeeping.formatCommand ])
        }

    let lint =
        command "lint" {
            description "Checks formatting of all source files"
            inputs Input.context
            setAction (ActionPath.doBatch [ Prelude.restoreTools; HouseKeeping.dryFormatCommand ])
        }

    let build =
        command "build" {
            description "Builds the solution"
            inputs Input.context
            addInputs buildOptions

            setAction (
                ActionPath.doBatch
                    [ Prelude.restoreTools
                      Prelude.restoreSolution
                      HouseKeeping.clean
                      HouseKeeping.format
                      HouseKeeping.dryFormat
                      ProjectManagement.build ]
            )
        }

    let publish =
        command "publish" {
            description "Publish the package to nuget"
            inputs Input.context
            addInputs publishingOptions

            setAction (
                ActionPath.doBatch
                    [ Prelude.restoreTools
                      Prelude.restoreSolution
                      HouseKeeping.clean
                      HouseKeeping.format
                      HouseKeeping.dryFormat
                      ProjectManagement.build
                      ProjectManagement.pack
                      ProjectManagement.publish ]
            )
        }

    let test = command "test" {
        description "Test the plugin"
        inputs Input.context
        addInputs testOptions
        setAction (ActionPath.doBatch [
            Prelude.restoreTools
            Prelude.restoreSolution
            HouseKeeping.fableClean
            HouseKeeping.clean
            HouseKeeping.format
            HouseKeeping.dryFormat
            ProjectManagement.build
            Tests.build
            Tests.run
        ])
    }

    let scratch = command "scratch" {
        hidden
        inputs Input.context
        addInputs testOptions
        addInput Options.watch
        setAction (ActionPath.doBatch [
            Prelude.restoreTools
            Prelude.restoreSolution
            HouseKeeping.fableClean
            HouseKeeping.clean
            HouseKeeping.format
            HouseKeeping.dryFormat
            ProjectManagement.build
            Tests.scratch
        ])
    }

let rec mainBuilder _ argsv =
    rootCommand argsv {
        description "Partas.Solid"
        inputs Input.context
        helpAction
        addInputs globalOptions
        addCommands [ Commands.format; Commands.lint; Commands.build; Commands.publish; Commands.test; Commands.scratch ]
    }

[<EntryPoint>]
let main argsv =
    mainBuilder false argsv
