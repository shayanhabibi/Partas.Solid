module Build

open Fake.DotNet
open Spec
open Fake.Core
open Fake.IO
open Fake.IO.Globbing.Operators
open Partas.Build

// disable warning of implicit conversion of ops to string
#nowarn 3391

initializeContext ()

let private root = Repo.FileSystem.``.``.FullName


let release = lazy ReleaseNotes.load (Repo.FileSystem.docs.``RELEASE_NOTES.md``.ToString())
let cleanAndRestore = input {
    let! quick = Options.quick
    return pipeline "restore_clean" {
        stage "clean" {
            when' (not quick)
            workingDir root
            run (fun _ -> !! "**/**/bin" ++ "temp" -- "bin" |> Shell.cleanDirs)
        }
        stage "restore" {
            when' (not quick)
            workingDir root; parallel'; quiet
            run "dotnet tool restore --verbosity q"
            run $"dotnet restore {Repo.Project.SolutionFile}"
        }
    }
}
let fableClean = input {
    let! quick = Options.quick
    let directories = [
        Repo.Project.``Partas.Solid.FablePlugin``.Directory
        Repo.Project.``Partas.Solid``.Directory
        Repo.Project.``Partas.Solid.Tests.Core``.Directory
        Repo.Project.``Partas.Solid.Tests.Plugin``.Directory
    ]
    return stage "fable_clean" {
        parallel'
        for directory in directories do
            stage directory {
                workingDir directory
                run "dotnet fable clean -e .fs.jsx --yes"
            }
    }
}
let format check =
    let formatFiles =
        sourceFiles
        -- Repo.FileSystem.``Partas.Solid.FablePlugin``.``Plugin.fs``.ToString()
        -- "**/IndexAccess/IndexAccess.fs"
        |> Seq.map (sprintf "\"%s\"")
        |> String.concat " "
    let formatCmd =
        if check
        then $"dotnet fantomas --check {formatFiles}"
        else $"dotnet fantomas {formatFiles}"
    stage "format" {
        captureOutput
        continueOnStepFailure (not check)
        run formatCmd
    }
let build project = stage "build" {
    run $"dotnet build {project} -c Release"
}
let pack project = stage "pack" {
    run $"dotnet pack {project} -c Release --no-restore -o bin"
}
let publish = input {
    let! apiKey = Baked.Input.NuGet.apiKeyOrEnv
    and! ci = Baked.Input.CI.isCI
    let packages = System.IO.Path.Combine(Repo.FileSystem.bin.``.``.ToString(), "*.nupkg")
    return
        match apiKey with
        | Some key -> pipeline "publish" {
            stage "publish" {
                runSensitive $"dotnet nuget push {packages} --source https://api.nuget.org/v3/index.json --skip-duplicate --api-key {key}"
            }
            }
        | None -> pipeline "local-publish" {
            stage "publish" {
                when' (not ci)
                run $"dotnet nuget push {packages} --source local --skip-duplicate"
            }
            }
}
let runTests = input {
    let! skipTests = Options.skipTests
    and! fableClean = fableClean
    return pipeline "tests" {
        description "Running tests"
        stage "tests" {
            when' (not skipTests)
            fableClean
            build Repo.Project.``Partas.Solid.Tests.Core``.Path
            build Repo.Project.``Partas.Solid.Tests.Plugin``.Path
            run (fun _ ->
                !! "**/bin/**/*.Tests.Plugin.dll"
                |> Testing.Expecto.run (fun p -> { p with Summary = true; CustomArgs = "--colours 256" :: p.CustomArgs })
            )
        }
    }
}
let runScratch = input {
    let! watch = Options.watch
    return stage "run scratch" {
        run (fun _ -> fable [ "--exclude"; "Partas.Solid.FablePlugin"; "--noCache"; "-e"; ".fs.jsx"; if watch then "--watch"; "--optimize" ] Repo.Project.ScratchTests.Directory)
    }
}

[<EntryPoint>]
let main argsv = rootCommand argsv {
    description ""
    addCommands [
        command "build" {
            cleanAndRestore
            Command.pipeline {
                for project in [ Repo.Project.``Partas.Solid.FablePlugin``.Path; Repo.Project.``Partas.Solid``.Path ] do
                    build project
            }
        }
        command "test" {
            cleanAndRestore
            runTests
            Command.pipeline {
                fableClean
            }
        }
        command "publish" {
            cleanAndRestore
            runTests
            Command.pipeline {
                fableClean
                for project in [ Repo.Project.``Partas.Solid.FablePlugin``.Path; Repo.Project.``Partas.Solid``.Path ] do
                    stage project {
                        build project
                        pack project
                    }
            }
            publish
        }
        command "bump" {
            Command.pipeline {
                input {
                    let! bump = Baked.Argument.Versioning.bump
                    and! ci = Baked.Input.CI.isCI
                    return stage "bump" {
                        when' (not ci)
                        run (fun _ ->
                            [ Repo.Project.``Partas.Solid``.Path; Repo.Project.``Partas.Solid.FablePlugin``.Path ]
                            |> List.map (fun project ->
                                match Baked.IO.bumpVersion project bump with
                                | Ok (previous, next) ->
                                    printfn "Bumping %s from %s to %s" project previous next
                                    Ok()
                                | Error error -> Error $"%s{project}: %s{error.Message}"
                                )
                            |> List.tryPick (function Error error -> Some (Error error) | Ok () -> None)
                            |> Option.defaultValue (Ok())
                            )
                    }

                }
            }
        }
    ]
}
