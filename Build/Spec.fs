module Spec

open Fake.Core
open Fake.Core.Context
open Fake.JavaScript
open FsToolkit.ErrorHandling.Operator.Option
open Partas.TypeProvider.BuildHelper
open Partas.Build

[<Literal>]
let __REPOSITORY_DIRECTORY__ =
    __SOURCE_DIRECTORY__
    + "/.."

type Repo = BuildHelperProvider<__REPOSITORY_DIRECTORY__, capabilityFullOverride = true>
let inline funApply value fn = fn value

[<AutoOpen>]
module DirectoryManagement =
    open Fake.IO.Globbing.Operators
    let sourceFiles =
        !!"**/*.fs"
        -- "**/obj/**/*.*"
        -- "**/AssemblyInfo.fs"

[<AutoOpen>]
module GitManagement =
    [<Literal>]
    let githubUsername = "GitHub Action"

    [<Literal>]
    let githubEmail = "41898282+github-actions[bot]@users.noreply.github.com"

    [<Literal>]
    let gitCiPrefix =
        "-c user.name=\""
        + githubUsername
        + "\" -c user.email=\""
        + githubEmail
        + "\""

    [<Literal>]
    let gitCiCommand =
        "git "
        + gitCiPrefix

    let gitCiArgs =
        [ "-c"
          $"user.name=\"{githubUsername}\""
          "-c"
          $"user.email=\"{githubEmail}\"" ]

#nowarn 3391

[<AutoOpen>]
module CliApiManagement =
    module Options =
        let quick =
            Input.option<bool> "--quick"
            |> Input.alias "-q"
            |> Input.desc "Skips installations, linting, and other checks"

        let format =
            Input.option<bool> "--format"
            |> Input.alias "-f"
            |> Input.desc "Formats the code"

        let dryFormat =
            Input.option<bool> "--dry-format"
            |> Input.desc "Checks style for errors"

        let skipTests =
            Input.option<bool> "--skip-tests"
            |> Input.desc "Skips running tests"

        let watch =
            Input.option<bool> "--watch"
            |> Input.desc "Runs the operation in watch mode."

        module Npm =
            let cleanInstall =
                Input.option<bool> "--clean-install"
                |> Input.alias "--ci"
                |> Input.desc "Npm commands are run with --ci (clean install)"

        module GitHub =
            let key =
                Input.optionMaybe<string> "--github-key"
                |> Input.alias "--github"
                |> Input.arity Arity.ExactlyOne
                |> Input.desc "GitHub API key"
                |> Input.helpName "APIKEY"

[<AutoOpen>]
module FakeInitializationAndUtilities =
    let private root = Repo.FileSystem.``.``.FullName
    // Credit SAFE STACK
    let initializeContext () =
        let execContext = FakeExecutionContext.Create false "build.fsx" []
        setExecutionContext (RuntimeContext.Fake execContext)

    let private createProcess exe args dir =
        CreateProcess.fromRawCommand exe args
        |> CreateProcess.withWorkingDirectory dir
        |> CreateProcess.ensureExitCode

    let dotnet args dir =
        createProcess "dotnet" args dir
        |> Proc.run
        |> ignore

    let fable args dir =
        dotnet
            ("fable"
             :: args)
            dir

    let mocha args dir =
        createProcess
            Npm.defaultNpmParams.NpmFilePath
            ("exec"
             :: "--"
             :: "mocha"
             :: args)
            dir
        |> Proc.run
        |> ignore

    let private gitCi args dir =
        createProcess gitCiCommand args dir
        |> Proc.run
        |> ignore

    module Npm =
        let private setDir dir =
            fun p ->
                { p with
                    Npm.NpmParams.WorkingDirectory = dir }

        let cleanInstall =
            setDir
            >> Npm.cleanInstall

        let install =
            setDir
            >> Npm.install

        let test =
            setDir
            >> Npm.runTest "test"

        let runScript command =
            setDir root
            |> Npm.run command

    module Git =
        open Fake.Tools.Git

        let inline private run command =
            CommandHelper.directRunGitCommandAndFail root command

        let pushTags pass =
            run $"{gitCiPrefix} push --tags origin"
            pass

        let pushBranch branchName pass =
            run $"{gitCiPrefix} push origin {branchName}"
            pass

        let pushBranchAndTags branchName pass =
            pushBranch branchName pass
            |> pushTags

        let branchName () =
            Information.getBranchName root

        let pushCurrentBranch pass =
            branchName ()
            |> pushBranch
            |> funApply pass

        let pushCurrentBranchAndTags pass =
            branchName ()
            |> pushBranchAndTags
            |> funApply pass

        let commitFiles msg files =
            files
            |> List.iter (
                Staging.stageFile root
                >> ignore
            )

            Commit.exec root msg

        let tagBranch tag =
            Branches.tag root tag
