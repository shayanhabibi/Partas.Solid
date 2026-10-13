#i "nuget: https://nuget.cloudsmith.io/shayanhabibi/shayanhabibi-partas-build/v3/index.json"
#r "nuget: Partas.TypeProvider.BuildHelper, 0.2.5"
#r "nuget: Partas.Build, 0.8.1"
#r "nuget: Partas.Build.EasyBuild.ShipIt, 0.1.0"
#r "nuget: Partas.Build.Baked, 0.2.0"
#r "nuget: Fake.IO.FileSystem"
#r "nuget: Fake.DotNet.AssemblyInfoFile"
open Partas.Build
open Partas.Build.Baked
open Partas.TypeProvider.BuildHelper

[<Literal>]
let __REPO_ROOT__ = __SOURCE_DIRECTORY__ + "/.."
[<Literal>]
let __VIRTUAL_ROOT__ = "
    bin/
"

type Repo = BuildHelperProvider<
    __REPO_ROOT__,
    __VIRTUAL_ROOT__,
    capabilityFullOverride = true
    >

module Project =
    /// The two shipped packages. They are versioned in lockstep by the one changelog in src/CHANGELOG.md.
    let src = [ Repo.Project.``Partas.Solid``.Path; Repo.Project.``Partas.Solid.FablePlugin``.Path ]
    /// The Expecto suites. The vitest runtime suites are driven by their own run.mjs.
    let tests = [ Repo.Project.``Partas.Solid.Tests.Core``.Path; Repo.Project.``Partas.Solid.Tests.Plugin``.Path ]
    /// The docs' live-examples plugin, released in lockstep with src (docs/site references the project), and its tests.
    let docsPlugin = Repo.Project.``Partas.Nacara.Plugins.Solid``.Path
    let docsPluginTests = Repo.Project.``Partas.Nacara.Plugins.Solid.Tests``.Path
    /// Everything `pack` and `publish` ship.
    let packages = src @ [ docsPlugin ]

module Options =
    let extension =
        Input.option<string> "--extension"
        |> Input.alias "-e"
        |> Input.description "The extension to use for fable compiled files."
        |> Input.def ".fs.jsx"
        |> Input.helpName "EXT"
    let nugetSource =
        Input.option<string> "--nuget-source"
        |> Input.def "https://api.nuget.org/v3/index.json"
        |> Input.description "NuGet feed URL for authenticated publishing"
        |> Input.helpName "URL"

let restore = Stages.restore Repo.Project.SolutionFile
// npm packages ship their own bin/ folders, and solid/ is a submodule this build must never touch.
let clean = Stages.clean [ "**/bin"; "!**/node_modules/**"; "!solid/**"; "temp" ] [ "bin/*.nupkg" ]
let fableClean = input {
    let! quick = Common.quick
    and! skipTests = Common.skipTests
    and! extension = Options.extension
    return stage "fable-clean" {
        when' (not (quick || skipTests))
        Project.tests @ Project.src
        |> List.map (
            System.IO.FileInfo
            >> _.Directory.FullName
            >> fun project ->
                stage $"clean-{project}" { run $"dotnet fable clean --cwd {project} -e {extension} --yes" }
            )
    }
}

let fantomasPaths = [
    "**/*.fs"
    "**/*.fsx"
    "!packages/**/*.*"
    "!**/obj/**/*.*"
    "!**/IndexAccess/IndexAccess.fs"
    "!src/Partas.Solid.FablePlugin/Plugin.fs"
]

let fantomas = Stages.fantomas fantomasPaths
let fantomasCheck = Stages.fantomasWith (InputSpec.ofInput Common.quick) true fantomasPaths

// Sequential: Partas.Solid references the plugin, so parallel builds race on its deps.json.
let build = Stages.build Project.packages |> InputSpec.map (StageContext.toggleParallel false)

let tests = input {
    let! config = Dotnet.configOrRelease
    and! skipTests = Common.skipTests
    and! ci = Common.isCI
    and! fableClean = fableClean
    return stage "run-tests" {
        when' (not skipTests)
        continueStepsOnFailure
        stage "run-tests" {
            for test in Project.tests @ [ Project.docsPluginTests ] do
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
        // Compiles the fixtures with Fable, then runs vitest; run.mjs runs `npm ci` itself when node_modules is missing.
        stage "runtime-tests" {
            run "node tests/Partas.Solid.Tests.Runtime/run.mjs all"
        }
        fableClean
    }
}

let private packages = Repo.VirtualFileSystem.bin.ToString()
let pack = Stages.pack packages Project.packages

let docs = input {
    let! watch = Common.watch
    and! config = Dotnet.configOrRelease
    let mode = if watch then "watch" else "build"
    return stage "docs" {
        run $"dotnet run --project docs/site/docs.fsproj -c {config} -- {mode}"
    }
}

/// Pushes to --nuget-source with the key from --nuget-key or NUGET_API_KEY, or to the `local` source without one.
/// CI points --nuget-source at the temporary Cloudsmith feed while nuget.org ownership is pending.
let publish = input {
    let! source = Options.nugetSource
    and! apiKey = NuGet.apiKey.option
    return stage "push" {
        let packageGlob = System.IO.Path.Combine(packages, "*.nupkg")
        match apiKey with
        | Some key ->
            stage "push to source" {
                run (cmd $"dotnet nuget push {packageGlob} --source {source}"
                     |> Cmd.secretOption "--api-key" key
                     |> Cmd.arg "--skip-duplicate")
            }
        | None -> stage "push to local feed" { run $"dotnet nuget push {packageGlob} --source local --skip-duplicate" }
    }
}

module ShipIt =
    open Partas.Build.EasyBuild.ShipIt
    /// This repository releases from master, and its history before ShipIt has merge and non-conventional commits.
    let private repoDefaults (options: ReleaseOptions) =
        { options with
            AllowedBranches = if options.AllowedBranches = ReleaseOptions.defaults.AllowedBranches then [ "master" ] else options.AllowedBranches
            SkipInvalidCommit = true
            SkipMergeCommit = true }
    /// ShipIt treats every CHANGELOG.md under its working directory as a project, and the solid/ submodule has a
    /// dozen of them. Running from src/ scopes it to ours.
    let private inSrc = InputSpec.map (StageContext.setWorkingDir (ValueSome (System.IO.Path.Combine(__REPO_ROOT__, "src"))))
    /// Local mode: updates src/CHANGELOG.md and both package versions, without committing.
    let bump = Stages.bumpWith (Inputs.bump |> InputSpec.map repoDefaults) |> inSrc
    /// Opens or updates the release pull request on GitHub.
    let release = Stages.githubWith (Inputs.release |> InputSpec.map repoDefaults) Inputs.token |> inSrc
    let commands = Commands.shipit

rootCommandOfScript {
    description "Partas.Solid build scripts"
    command "format" {
        description "Formats the code."
        restore
        fantomas
        command "check" {
            description "Checks style for errors."
            restore
            fantomasCheck
        }
    }
    command "build" {
        description "Builds the project"
        restore
        build
    }
    command "test" {
        description "Runs tests."
        command "scratch" {
            description "Run the scratch project."
            restore
            input {
                let! quick = Common.quick
                and! extension = Options.extension
                return stage "clean scratch" {
                    when' (not quick)
                    run $"dotnet fable clean -e {extension} --yes --cwd {Repo.Project.ScratchTests.Directory}"
                }
            }
            input {
                let! watch = Common.watch
                and! extension = Options.extension
                return stage "run scratch" {
                    if watch then
                        stage "watch" {
                            run $"dotnet fable watch -o output --optimize --typedArrays false -c Release -e {extension} --cwd {Repo.Project.ScratchTests.Directory} --exclude Partas.Solid.FablePlugin"
                        }
                    else
                        stage "run" {
                            run $"dotnet fable -o output --optimize --typedArrays false -c Release -e {extension} --cwd {Repo.Project.ScratchTests.Directory} --exclude Partas.Solid.FablePlugin"
                        }
                }
            }
        }
        restore
        fableClean
        build
        tests
    }
    command "publish" {
        description "Builds, tests, packs and pushes the three packages (--nuget-source, --nuget-key)."
        restore
        fableClean
        build
        tests
        pack
        publish
    }
    command "docs" {
        description "Builds the documentation site in docs/site (--watch to serve it)"
        docs
    }
    command "bump" {
        description "Calculates the next version from conventional commits and writes it to src/CHANGELOG.md and the three .fsproj files (--dry-run to preview)."
        restore
        ShipIt.bump
    }
    command "release" {
        description "Opens or updates the ShipIt release pull request on GitHub (--token or GITHUB_TOKEN)."
        restore
        ShipIt.release
    }
    command "clean" {
        description "Cleans the build."
        restore
        clean
        fableClean
    }
    addCommand ShipIt.commands
}
// Without this the script exits 0 whatever happened, and CI can never go red.
|> exit
