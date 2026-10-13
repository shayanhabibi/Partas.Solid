namespace Nacara.Plugins

open System

/// <summary>What a solid fence puts on the page.</summary>
[<RequireQualifiedAccess>]
type SolidShow =
    /// <summary>The code, then what it renders.</summary>
    | Both
    /// <summary>The code alone, although it still compiles with the page.</summary>
    | Code
    /// <summary>What it renders, without the code.</summary>
    | Output
    /// <summary>What it renders, without the code or the box around it, as though it were part of the page.</summary>
    | Inline

/// <summary>A run of generated lines that came from a page's body.</summary>
type SolidLineSpan =
    {
        /// <summary>First generated line, counting from one.</summary>
        Generated: int
        Length: int
        /// <summary>The body line the first generated line came from.</summary>
        Body: int
        /// <summary>Columns the generator moved the code right by: negative when it moved left.</summary>
        Indent: int
    }

/// <summary>What a fence transform is given.</summary>
type SolidTransformInput =
    {
        /// <summary>The fence's code, as the page shows it.</summary>
        Code: string
        /// <summary>
        /// The words of the info string after the language, less the transform's token and the ones every solid fence
        /// reads (<c>id=</c>, <c>show=</c>): bare words and <c>key=value</c> pairs.
        /// </summary>
        Flags: string list
        PageKey: string
        CellId: string
    }

/// <summary>The F# a fence transform compiles in place of the fence's code.</summary>
type SolidTransformed =
    {
        /// <summary>Module-level declarations, from column zero.</summary>
        Code: string
        /// <summary>The expression the cell renders.</summary>
        Render: string
        /// <summary>
        /// Where the lines of <c>Code</c> came from. <c>Generated</c> counts from one within <c>Code</c>, <c>Body</c>
        /// from one within the fence's code; <c>Indent = Int32.MaxValue</c> blames the fence's opening line.
        /// </summary>
        Spans: SolidLineSpan list
    }

/// <summary>What a fence transform made of a fence.</summary>
[<RequireQualifiedAccess>]
type SolidTransformOutput =
    | Compiled of SolidTransformed
    /// <summary>The fence stays plain code, and each problem is reported at a line of the fence's code, counting from one.</summary>
    | Rejected of problems: (int * string) list

/// <summary>How the code of a solid fence is read.</summary>
[<RequireQualifiedAccess>]
type SolidCellKind =
    /// <summary>
    /// Module-level declarations. Nothing renders unless the fence names a component with
    /// <c>render=</c>.
    /// </summary>
    | Declarations of render: string option
    /// <summary>A single expression, rendered as the body of a component.</summary>
    | Expression
    /// <summary>Declarations the page needs but the reader does not: compiled, never shown.</summary>
    | Setup
    /// <summary>An expression written in the prose as a code span, rendered where it stands.</summary>
    | Use of column: int
    /// <summary>A fence a transform rewrote: the page shows the fence's code, and <c>Code</c> is what compiles.</summary>
    | Transformed of SolidTransformed

/// <summary>One solid fence, as the scanner found it.</summary>
type SolidCell =
    {
        /// <summary>Unique within its page: <c>c1</c>, <c>c2</c>, or what the fence named with <c>id=</c>.</summary>
        Id: string
        Kind: SolidCellKind
        Show: SolidShow
        Code: string
        /// <summary>The line of the body the code starts on, counting from one.</summary>
        Line: int
        /// <summary>Whether the page shows the JSX Fable made of the cell, under it.</summary>
        Jsx: bool
    }

    /// <summary>Whether a placeholder is mounted for it.</summary>
    member this.Mounts =
        match this.Kind with
        | SolidCellKind.Expression
        | SolidCellKind.Declarations(Some _) -> this.Show <> SolidShow.Code
        | SolidCellKind.Transformed _ -> this.Show <> SolidShow.Code
        | SolidCellKind.Use _ -> true
        | SolidCellKind.Declarations None
        | SolidCellKind.Setup -> false

/// <summary>Everything the compile step needs of one page.</summary>
type SolidPageUnit =
    {
        /// <summary>Names the module, the entry and the bundle: <c>p1a2b3c4d</c>.</summary>
        Key: string
        /// <summary>The page's source, which diagnostics point at.</summary>
        Source: string option
        /// <summary>Added to a body line to reach the line of the file.</summary>
        BodyLine: int
        Cells: SolidCell list
        /// <summary>The generated F# module.</summary>
        Code: string
        Spans: SolidLineSpan list
    }

/// <summary>Configures the solid examples plugin.</summary>
type SolidExamplesOptions =
    {
        /// <summary>The Partas.Solid package the examples compile against.</summary>
        /// <remarks>Defaults to <c>"3.*"</c>.</remarks>
        PartasVersion: string
        /// <summary>
        /// A <c>Partas.Solid.fsproj</c> the examples reference in place of the package, by path. A relative path
        /// resolves against the site root.
        /// </summary>
        /// <remarks>
        /// When set, <c>PartasVersion</c> is not used, and the project's sources are part of the build
        /// fingerprint. The Partas.Solid repository's own docs use it to compile against its sources.
        /// Defaults to <c>None</c>.
        /// </remarks>
        PartasProject: string option
        /// <summary>Extra NuGet sources, tried alongside nuget.org. A local folder feed works.</summary>
        /// <remarks>Defaults to <c>[]</c>.</remarks>
        Feeds: string list
        /// <summary>
        /// More NuGet packages the examples compile against, as (id, version): F# bindings, say.
        /// </summary>
        /// <remarks>An entry for <c>Partas.Solid</c> is ignored: <c>PartasVersion</c> sets it.
        /// Defaults to <c>[]</c>.</remarks>
        NuGetPackages: (string * string) list
        /// <summary>
        /// More npm packages the examples can import, as (name, version): a framework-free library
        /// such as <c>animejs</c>, say.
        /// </summary>
        /// <remarks>
        /// Entries for the packages the plugin installs itself are ignored. <c>solid-js</c> and
        /// <c>@solidjs/web</c> stay pinned to <c>SolidVersion</c> for every package, so a library
        /// built on Solid 1 cannot bring a second, older Solid with it, and does not work.
        /// <para>Defaults to <c>[]</c>.</para>
        /// </remarks>
        NpmPackages: (string * string) list
        /// <summary>F# projects the examples reference, by path. A relative path resolves against the site root.</summary>
        /// <remarks>Their project files and sources are part of the build fingerprint. Defaults to <c>[]</c>.</remarks>
        Projects: string list
        /// <summary>MSBuild properties, as (name, value), for every project Fable loads.</summary>
        /// <remarks>
        /// Set in the environment of the Fable process, so they reach referenced projects too, including
        /// conditions on their <c>Compile</c> items. Defaults to <c>[]</c>.
        /// </remarks>
        Properties: (string * string) list
        /// <summary>The target framework of the generated project.</summary>
        /// <remarks>Defaults to <c>"net9.0"</c>.</remarks>
        TargetFramework: string
        /// <summary>The Fable the examples compile with.</summary>
        /// <remarks>Defaults to <c>"5.13.0"</c>.</remarks>
        FableVersion: string
        /// <summary>The version of <c>solid-js</c>, <c>@solidjs/web</c> and <c>@solidjs/compiler</c>.</summary>
        /// <remarks>Defaults to <c>"2.0.0-rc.14"</c>.</remarks>
        SolidVersion: string
        /// <summary>The version of <c>rolldown</c>, which bundles each page's examples.</summary>
        /// <remarks>Defaults to <c>"1.2.11"</c>.</remarks>
        RolldownVersion: string
        /// <summary>The word in a fence's info string that makes it a solid example.</summary>
        /// <remarks>Defaults to <c>"solid"</c>.</remarks>
        FenceToken: string
        /// <summary>Lines at the top of every page's module.</summary>
        /// <remarks>Defaults to <c>[ "open Partas.Solid"; "open Fable.Core"; "open Fable.Core.JsInterop" ]</c>.</remarks>
        Prelude: string list
        /// <summary>Where the bundles are written, relative to the output directory.</summary>
        /// <remarks>Defaults to <c>"_partas/solid"</c>.</remarks>
        OutputPath: string
        /// <summary>Where the generated project lives, relative to the project root.</summary>
        /// <remarks>
        /// A dotted directory, so watch mode does not rebuild when the plugin writes it. Defaults to <c>".nacara/partas-solid"</c>.
        /// </remarks>
        WorkspacePath: string
        /// <summary>Whether the bundles are minified.</summary>
        /// <remarks>Defaults to <c>true</c>.</remarks>
        Minify: bool
        /// <summary>
        /// Show the JSX Fable made of every example, not only of fences marked <c>jsx</c>.
        /// </summary>
        /// <remarks>Defaults to <c>false</c>.</remarks>
        ShowJsx: bool
        /// <summary>
        /// Fence transforms, by token: an <c>fsharp</c> fence whose info string holds the token compiles as the
        /// transform's output.
        /// </summary>
        /// <remarks>Defaults to <c>[]</c>.</remarks>
        Transforms: (string * (SolidTransformInput -> SolidTransformOutput)) list
        /// <summary>How long one tool run may take before it is abandoned.</summary>
        /// <remarks>Defaults to five minutes.</remarks>
        Timeout: TimeSpan
    }
