module Spec

open EasyBuild.FileSystemProvider
open FSharp.SystemCommandLine
open Fake.Core
open Fake.Core.Context
open Fake.JavaScript
open FsToolkit.ErrorHandling.Operator.Option

[<Literal>]
let __REPOSITORY_DIRECTORY__ =
    __SOURCE_DIRECTORY__
    + "/.."

type Root = AbsoluteFileSystem<__REPOSITORY_DIRECTORY__>

let inline funApply value fn = fn value

type ActionPath = ActionContext -> ActionContext

module ActionPath =
    let inline create ([<InlineIfLambda>] fn: ActionContext -> unit) : ActionPath =
        fun ctx ->
            fn ctx
            ctx

    type SRTPHelper =
        static member inline createFor([<InlineIfLambda>] fn: 'A -> unit, a1: ActionInput<'A>) =
            fun ctx ->
                a1.GetValue (ctx.ParseResult)
                |> fn

                ctx

        static member inline createFor([<InlineIfLambda>] fn: 'A * 'B -> unit, (a1: ActionInput<_>, a2: ActionInput<_>)) =
            fun ctx ->
                (a1.GetValue (ctx.ParseResult), a2.GetValue (ctx.ParseResult))
                |> fn

                ctx

        static member inline createFor([<InlineIfLambda>] fn: 'A * 'B * 'C -> unit, (a1: ActionInput<_>, a2: ActionInput<_>, a3: ActionInput<_>)) =
            fun ctx ->
                (a1.GetValue (ctx.ParseResult), a2.GetValue (ctx.ParseResult), a3.GetValue (ctx.ParseResult))
                |> fn

                ctx

        static member inline createFor
            ([<InlineIfLambda>] fn: 'A * 'B * 'C * 'D -> unit, (a1: ActionInput<_>, a2: ActionInput<_>, a3: ActionInput<_>, a4: ActionInput<_>))
            =
            fun ctx ->
                (a1.GetValue (ctx.ParseResult), a2.GetValue (ctx.ParseResult), a3.GetValue (ctx.ParseResult), a4.GetValue (ctx.ParseResult))
                |> fn

                ctx

        static member inline createFor
            (
                [<InlineIfLambda>] fn: 'A * 'B * 'C * 'D * 'E -> unit,
                (a1: ActionInput<_>, a2: ActionInput<_>, a3: ActionInput<_>, a4: ActionInput<_>, a5: ActionInput<_>)
            ) =
            fun ctx ->
                (a1.GetValue (ctx.ParseResult),
                 a2.GetValue (ctx.ParseResult),
                 a3.GetValue (ctx.ParseResult),
                 a4.GetValue (ctx.ParseResult),
                 a5.GetValue (ctx.ParseResult))
                |> fn

                ctx

        static member inline createFor
            (
                [<InlineIfLambda>] fn: 'A * 'B * 'C * 'D * 'E * 'F -> unit,
                (a1: ActionInput<_>, a2: ActionInput<_>, a3: ActionInput<_>, a4: ActionInput<_>, a5: ActionInput<_>, a6: ActionInput<_>)
            ) =
            fun ctx ->
                (a1.GetValue (ctx.ParseResult),
                 a2.GetValue (ctx.ParseResult),
                 a3.GetValue (ctx.ParseResult),
                 a4.GetValue (ctx.ParseResult),
                 a5.GetValue (ctx.ParseResult),
                 a6.GetValue (ctx.ParseResult))
                |> fn

                ctx

        static member inline createFor
            (
                [<InlineIfLambda>] fn: 'A * 'B * 'C * 'D * 'E * 'F * 'G -> unit,
                (a1: ActionInput<_>,
                 a2: ActionInput<_>,
                 a3: ActionInput<_>,
                 a4: ActionInput<_>,
                 a5: ActionInput<_>,
                 a6: ActionInput<_>,
                 a7: ActionInput<_>)
            ) =
            fun ctx ->
                (a1.GetValue (ctx.ParseResult),
                 a2.GetValue (ctx.ParseResult),
                 a3.GetValue (ctx.ParseResult),
                 a4.GetValue (ctx.ParseResult),
                 a5.GetValue (ctx.ParseResult),
                 a6.GetValue (ctx.ParseResult),
                 a7.GetValue (ctx.ParseResult))
                |> fn

                ctx

        static member inline createFor
            (
                [<InlineIfLambda>] fn: 'A * 'B * 'C * 'D * 'E * 'F * 'G * 'H -> unit,
                (a1: ActionInput<_>,
                 a2: ActionInput<_>,
                 a3: ActionInput<_>,
                 a4: ActionInput<_>,
                 a5: ActionInput<_>,
                 a6: ActionInput<_>,
                 a7: ActionInput<_>,
                 a8: ActionInput<_>)
            ) =
            fun ctx ->
                (a1.GetValue (ctx.ParseResult),
                 a2.GetValue (ctx.ParseResult),
                 a3.GetValue (ctx.ParseResult),
                 a4.GetValue (ctx.ParseResult),
                 a5.GetValue (ctx.ParseResult),
                 a6.GetValue (ctx.ParseResult),
                 a7.GetValue (ctx.ParseResult),
                 a8.GetValue (ctx.ParseResult))
                |> fn

                ctx

        static member inline createWith([<InlineIfLambda>] fn: ActionContext -> 'A -> unit, a1: ActionInput<'A>) =
            fun ctx ->
                a1.GetValue (ctx.ParseResult)
                |> fn ctx

                ctx

        static member inline createWith([<InlineIfLambda>] fn: ActionContext -> 'A * 'B -> unit, (a1: ActionInput<_>, a2: ActionInput<_>)) =
            fun ctx ->
                (a1.GetValue (ctx.ParseResult), a2.GetValue (ctx.ParseResult))
                |> fn ctx

                ctx

        static member inline createWith
            ([<InlineIfLambda>] fn: ActionContext -> 'A * 'B * 'C -> unit, (a1: ActionInput<_>, a2: ActionInput<_>, a3: ActionInput<_>))
            =
            fun ctx ->
                (a1.GetValue (ctx.ParseResult), a2.GetValue (ctx.ParseResult), a3.GetValue (ctx.ParseResult))
                |> fn ctx

                ctx

        static member inline createWith
            (
                [<InlineIfLambda>] fn: ActionContext -> 'A * 'B * 'C * 'D -> unit,
                (a1: ActionInput<_>, a2: ActionInput<_>, a3: ActionInput<_>, a4: ActionInput<_>)
            ) =
            fun ctx ->
                (a1.GetValue (ctx.ParseResult), a2.GetValue (ctx.ParseResult), a3.GetValue (ctx.ParseResult), a4.GetValue (ctx.ParseResult))
                |> fn ctx

                ctx

        static member inline createWith
            (
                [<InlineIfLambda>] fn: ActionContext -> 'A * 'B * 'C * 'D * 'E -> unit,
                (a1: ActionInput<_>, a2: ActionInput<_>, a3: ActionInput<_>, a4: ActionInput<_>, a5: ActionInput<_>)
            ) =
            fun ctx ->
                (a1.GetValue (ctx.ParseResult),
                 a2.GetValue (ctx.ParseResult),
                 a3.GetValue (ctx.ParseResult),
                 a4.GetValue (ctx.ParseResult),
                 a5.GetValue (ctx.ParseResult))
                |> fn ctx

                ctx

        static member inline createWith
            (
                [<InlineIfLambda>] fn: ActionContext -> 'A * 'B * 'C * 'D * 'E * 'F -> unit,
                (a1: ActionInput<_>, a2: ActionInput<_>, a3: ActionInput<_>, a4: ActionInput<_>, a5: ActionInput<_>, a6: ActionInput<_>)
            ) =
            fun ctx ->
                (a1.GetValue (ctx.ParseResult),
                 a2.GetValue (ctx.ParseResult),
                 a3.GetValue (ctx.ParseResult),
                 a4.GetValue (ctx.ParseResult),
                 a5.GetValue (ctx.ParseResult),
                 a6.GetValue (ctx.ParseResult))
                |> fn ctx

                ctx

        static member inline createWith
            (
                [<InlineIfLambda>] fn: ActionContext -> 'A * 'B * 'C * 'D * 'E * 'F * 'G -> unit,
                (a1: ActionInput<_>,
                 a2: ActionInput<_>,
                 a3: ActionInput<_>,
                 a4: ActionInput<_>,
                 a5: ActionInput<_>,
                 a6: ActionInput<_>,
                 a7: ActionInput<_>)
            ) =
            fun ctx ->
                (a1.GetValue (ctx.ParseResult),
                 a2.GetValue (ctx.ParseResult),
                 a3.GetValue (ctx.ParseResult),
                 a4.GetValue (ctx.ParseResult),
                 a5.GetValue (ctx.ParseResult),
                 a6.GetValue (ctx.ParseResult),
                 a7.GetValue (ctx.ParseResult))
                |> fn ctx

                ctx

        static member inline createWith
            (
                [<InlineIfLambda>] fn: ActionContext -> 'A * 'B * 'C * 'D * 'E * 'F * 'G * 'H -> unit,
                (a1: ActionInput<_>,
                 a2: ActionInput<_>,
                 a3: ActionInput<_>,
                 a4: ActionInput<_>,
                 a5: ActionInput<_>,
                 a6: ActionInput<_>,
                 a7: ActionInput<_>,
                 a8: ActionInput<_>)
            ) =
            fun ctx ->
                (a1.GetValue (ctx.ParseResult),
                 a2.GetValue (ctx.ParseResult),
                 a3.GetValue (ctx.ParseResult),
                 a4.GetValue (ctx.ParseResult),
                 a5.GetValue (ctx.ParseResult),
                 a6.GetValue (ctx.ParseResult),
                 a7.GetValue (ctx.ParseResult),
                 a8.GetValue (ctx.ParseResult))
                |> fn ctx

                ctx

    let inline createFor (action: ActionInput<'T>) ([<InlineIfLambda>] fn: 'T -> unit) : ActionPath =
        fun ctx ->
            action.GetValue (ctx.ParseResult)
            |> fn

            ctx

    let inline createFor' actions fn : ActionPath =
        ((^T or SRTPHelper): (static member createFor: ^Fn * ^T -> ActionPath) (fn, actions))

    /// <summary>Runs the function if the given action returns true.</summary>
    let inline createCond (action: ActionInput<bool>) ([<InlineIfLambda>] fn: unit -> unit) : ActionPath =
        createFor action (function
            | true -> fn ()
            | _ -> ())

    let inline createOverrideCond (ignoreCond: bool) (action: ActionInput<bool>) ([<InlineIfLambda>] fn: unit -> unit) : ActionPath =
        if ignoreCond then
            create (
                ignore
                >> fn
            )
        else
            createCond action fn

    /// <summary>Run the function if the given action returns false.</summary>
    let inline createCondNot (action: ActionInput<bool>) ([<InlineIfLambda>] fn: unit -> unit) : ActionPath =
        createFor action (function
            | false -> fn ()
            | _ -> ())

    let inline createOverrideCondNot (ignoreCond: bool) (action: ActionInput<bool>) ([<InlineIfLambda>] fn: unit -> unit) : ActionPath =
        if ignoreCond then
            create (
                ignore
                >> fn
            )
        else
            createCondNot action fn

    let inline createWith (action: ActionInput<'T>) ([<InlineIfLambda>] fn: ActionContext -> 'T -> unit) : ActionPath =
        fun ctx ->
            action.GetValue (ctx.ParseResult)
            |> fn ctx

            ctx

    let inline createWith' actions fn : ActionPath =
        ((^T or SRTPHelper): (static member createWith: ^Fn * ^T -> ActionPath) (fn, actions))

    let inline batch (paths: ActionPath list) : ActionPath =
        paths
        |> List.fold
            (fun prev next ->
                prev
                >> next)
            id

    let terminate: ActionContext -> unit = ignore

    let doBatch (paths: ActionPath list) =
        batch paths
        >> terminate

[<AutoOpen>]
module DirectoryManagement =
    open Fake.IO.Globbing.Operators

    let sourceFiles =
        !!"**/*.fs"
        -- "**/obj/**/*.*"
        -- "**/AssemblyInfo.fs"

    module Projects =
        module Directory =
            type Solid = Root.``Partas.Solid``
            type Plugin = Root.``Partas.Solid.FablePlugin``

        module FsProj =
            [<Literal>]
            let Solid = Directory.Solid.``Partas.Solid.fsproj``

            [<Literal>]
            let Plugin = Directory.Plugin.``Partas.Solid.FablePlugin.fsproj``

    module Tests =
        module Directory =
            type Core = Root.``Partas.Solid.Tests.Core``
            type Plugin = Root.``Partas.Solid.Tests.Plugin``
            type Scratch = Root.ScratchTests

        module FsProj =
            [<Literal>]
            let Core = Directory.Core.``Partas.Solid.Tests.Core.fsproj``

            [<Literal>]
            let Plugin = Directory.Plugin.``Partas.Solid.Tests.Plugin.fsproj``

            [<Literal>]
            let Scratch = Directory.Scratch.``ScratchTests.fsproj``

    module Solutions =
        [<Literal>]
        let Partas = Root.``Partas.Solid.slnx``

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
        let config =
            Input.option<string> "--configuration"
            |> Input.alias "-c"
            |> Input.desc "Build/pack configuration"
            |> Input.def "Release"
            |> Input.arity Arity.ExactlyOne
            |> Input.helpName "Debug|Release"
            |> Input.acceptOnlyFromAmong [ "Debug"; "Release" ]

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

        let interactive =
            Input.option<bool> "--interactive"
            |> Input.alias "-i"
            |> Input.desc "Runs the operation in interactive mode."

        module Npm =
            let cleanInstall =
                Input.option<bool> "--clean-install"
                |> Input.alias "--ci"
                |> Input.desc "Npm commands are run with --ci (clean install)"

        module NuGet =
            let key =
                Input.optionMaybe<string> "--nuget-key"
                |> Input.alias "--nuget"
                |> Input.arity Arity.ExactlyOne
                |> Input.desc "NuGet API key"
                |> Input.helpName "APIKEY"

        module GitHub =
            let key =
                Input.optionMaybe<string> "--github-key"
                |> Input.alias "--github"
                |> Input.arity Arity.ExactlyOne
                |> Input.desc "GitHub API key"
                |> Input.helpName "APIKEY"

    let buildOptions: ActionInput list = [ Options.config ]

    let publishingOptions: ActionInput list =
        buildOptions
        @ [ Options.NuGet.key; Options.GitHub.key ]

    let npmOptions: ActionInput list = [ Options.Npm.cleanInstall ]

    let testOptions: ActionInput list =
        [ Options.quick; Options.skipTests; Options.Npm.cleanInstall; Options.watch ]

    let globalOptions: ActionInput list =
        [ Options.quick; Options.format; Options.dryFormat ]

[<AutoOpen>]
module FakeInitializationAndUtilities =
    let private root = Root.``.``
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
