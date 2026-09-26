#r "nuget: Partas.Build, 0.2.3"
#r "nuget: Partas.TypeProvider.BuildHelper, 0.2.5"
#r "nuget: Fake.IO.FileSystem"
#r "nuget: Fake.DotNet.AssemblyInfoFile"
open Partas.Build
open Partas.TypeProvider.BuildHelper
open Fake.IO.Globbing.Operators
open Fake.IO

[<Literal>]
let root = __SOURCE_DIRECTORY__

type Repo = BuildHelperProvider<
    root,
    "bin/",
    capabilityFullOverride = true
    >

module Project =
    let all = [
        Repo.Project.``Partas.Solid``.Path
        Repo.Project.``Partas.Solid.FablePlugin``.Path
    ]
    let tests = [
        Repo.Project.``Partas.Solid.Tests.Plugin``.Path
        Repo.Project.``Partas.Solid.Tests.Core``.Path
    ]

module Options =
    let extension =
        Input.option<string> "--extension"
        |> Input.alias "-e"
        |> Input.desc "The extension to use for fable compiled files."
        |> Input.def ".fs.jsx"
        |> Input.helpName "EXT"
    let quick =
        Input.option<bool> "--quick"
        |> Input.alias "-q"
        |> Input.desc "Skips installations, linting, and other checks"
    let skipTests =
        Input.option<bool> "--skip-tests"
        |> Input.desc "Skips running tests"
    let watch =
        Input.option<bool> "--watch"
        |> Input.alias "-w"
        |> Input.desc "Runs the operation in watch mode."
    let config =
        Baked.Input.DotNet.configString
        |> InputSpec.ofInput
        |> InputSpec.map (Option.defaultValue "Release")
    let format =
        Input.option<bool> "--format"
        |> Input.alias "-f"
        |> Input.desc "Formats the code"
    let dryFormat =
        Input.option<bool> "--dry-format"
        |> Input.desc "Checks style for errors"



let restore = input {
    let! quick = Options.quick
    return stage "restore" {
        when' (not quick)
        quiet
        parallel'
        run "dotnet tool restore -v q"
        run $"dotnet restore {Repo.Project.SolutionFile} -v q"
    }
}
let clean = input {
    let! quick = Options.quick
    return stage "clean" {
        when' (not quick)
        run (fun _ ->
            !! "**/**/bin"
            ++ "bin"
            ++ "temp"
            |> Shell.cleanDirs
            )
    }
}
let fableClean = input {
    let! quick = Options.quick
    and! skipTests = Options.skipTests
    and! extension = Options.extension
    return stage "fable-clean" {
        when' (not (quick || skipTests))
        Project.tests @ Project.all
        |> List.map (
            System.IO.FileInfo
            >> _.Directory.FullName
            >> fun project ->
                stage $"clean-{project}" { run $"dotnet fable clean --cwd {project} -e {extension} --yes" }
            )
    }
}
let formatOption (format: Internal.InputSpec<bool>) (dryFormat: Internal.InputSpec<bool>) = input {
    let! dryFormat = dryFormat
    and! format = format
    return (
        let formatSuffix = if dryFormat then "--check" else ""
        let sourceFiles =
            !! "**/*.fs"
            ++ "**/*.fsx"
            -- "packages/**/*.*"
            -- "**/obj/**/*.*"
            -- "**/IndexAccess/IndexAccess.fs"
            -- "Partas.Solid.FablePlugin/Plugin.fs"
            |> Seq.map (sprintf "\"%s\"")
            |> String.concat " "
        stage "format" {
            when' (format || dryFormat)
            quiet
            captureOutput
            run $"dotnet fantomas {sourceFiles} {formatSuffix}"
        }
        )
}

let buildTarget (target: string) (config: string) = stage $"build-{target}" {
    quiet
    run $"dotnet build {target} -c {config} -v q"
}

let build = input {
    let! config = Options.config
    return stage "build" {
        quiet
        parallel'
        buildTarget Repo.Project.``Partas.Solid``.Path config
        buildTarget Repo.Project.``Partas.Solid.FablePlugin``.Path config
    }
}

let tests = input {
    let! config = Options.config
    and! skipTests = Options.skipTests
    and! ci = Baked.Input.CI.isCI
    and! fableClean = fableClean
    return stage "run-tests" {
        when' (not skipTests)
        continueStepsOnFailure
        stage "run-tests" {
            for test in Project.tests do
                buildTarget test config
                stage "run-target" {
                    if ci then
                        stage "run" {
                            quiet
                            run $"dotnet run --project {test} -c {config} -- --colours 256 --summary"
                        }
                    else
                        stage "run" {
                            quiet
                            run $"dotnet run --project {test} -c {config} -- --colours 256"
                        }
                }
        }
        fableClean
    }
}

let pack = input {
    let! config = Options.config
    return stage "pack" {
        quiet
        for project in Project.all do
        stage "pack" {
            run $"dotnet pack {project} -c {config} -o {Repo.VirtualFileSystem.bin.ToString()}"
        }
    }
}

let docs = input {
    let! watch = Options.watch
    and! config = Options.config
    let mode = if watch then "watch" else "build"
    return stage "docs" {
        run $"dotnet run --project docs/site/docs.fsproj -c {config} -- {mode}"
    }
}

let publish = input {
    let! apiKey = Baked.Input.NuGet.apiKeyOrEnv
    let path =
        System.IO.Path.Combine(
            Repo.VirtualFileSystem.bin.ToString(),
            "*.nupkg"
            )
    return stage "publish" {
        when' apiKey.IsSome
        failIfIgnored
        run $"dotnet nuget push {path} --source https://api.nuget.org/v3/index.json --skip-duplicate --api-key {Cmd.sensitive apiKey.Value}"
    }
}
rootCommand fsi.CommandLineArgs[1..] {
    description "Partas.Solid build scripts"
    command "build" {
        description "Builds the project"
        Command.pipeline {
            noPrefixForStep
            restore
            formatOption (InputSpec.ofInput Options.format) (InputSpec.ofInput Options.dryFormat)
            build
        }
    }
    command "test" {
        description "Runs tests."
        command "scratch" {
            description "Run the scratch project."
            restore
            input {
                let! quick = Options.quick
                and! extension = Options.extension
                return stage "clean scratch" {
                    when' (not quick)
                    run $"dotnet fable clean -e {extension} --yes --cwd {Repo.Project.ScratchTests.Directory}"
                }
            }
            input {
                let! watch = Options.watch
                and! extension = Options.extension
                return stage "run scratch" {
                    if watch then
                        stage "watch" {
                            run $"dotnet fable watch -o output --optimize -c Release -e {extension} --cwd {Repo.Project.ScratchTests.Directory} --exclude Partas.Solid.FablePlugin"
                        }
                    else
                        stage "run" {
                            run $"dotnet fable -o output --optimize -c Release -e {extension} --cwd {Repo.Project.ScratchTests.Directory} --exclude Partas.Solid.FablePlugin"
                        }
                }
            }
        }
        Command.pipeline {
            noPrefixForStep
            restore
            fableClean
            build
            tests
        }
    }
    command "publish" {
        description "Publishes the project."
        Command.pipeline {
            noPrefixForStep
            restore
            fableClean
            formatOption (InputSpec.ofInput Options.format) (InputSpec.ofInput Options.dryFormat)
            build
            tests
            pack
            publish
        }
    }
    command "docs" {
        description "Builds the documentation site in docs/site (--watch to serve it)"
        docs
    }
    command "format" {
        description "Formats the code."
        restore
        formatOption (InputSpec.ret true) (InputSpec.ofInput Options.dryFormat)
    }
    command "bump" {
        description "Bumps the version."
        restore
        Baked.Pipelines.bumpArgument Project.all (InputSpec.ret Project.all)
    }
    command "clean" {
        description "Cleans the build."
        restore
        clean
        fableClean
    }
    command "changelog" {
        description "Generates the changelog"
        stage "git-cliff" {
            run "git-cliff"
            run "git add -u"
        }
        let commitMessage = "[skip ci]\n\nUpdate changelog."
        let commitAuthor = "GitHub Action <41898282+github-actions[bot]@users.noreply.github.com>"
        stage "commit" {
            when' (Repo.Git.IsDirty())
            whenBranch "master"
            run (cmd $"git commit -m {commitMessage} --author={commitAuthor}")
            run (cmd $"git push origin HEAD:master")
        }
    }
}
