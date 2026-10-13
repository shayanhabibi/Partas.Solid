namespace Nacara.Plugins.Internal

open System
open System.IO
open System.Text.RegularExpressions
open Nacara.Plugins

/// <summary>Something a compile had to say, placed on a page when it can be.</summary>
type SolidMessage =
    {
        Page: SolidPageUnit option
        /// <summary>The body line and column, when the message came from a cell.</summary>
        At: (int * int) option
        IsError: bool
        Message: string
    }

/// <summary>What compiling every page's examples produced.</summary>
type SolidCompiled =
    {
        /// <summary>Output files, relative to the plugin's output path, with their text.</summary>
        Files: (string * string) list
        Messages: SolidMessage list
        /// <summary>False when the output is what an earlier build left, because this one could not run.</summary>
        Fresh: bool
    }

/// <summary>Runs Fable, then the Solid compiler and rolldown, over the generated workspace.</summary>
[<RequireQualifiedAccess>]
module SolidCompile =

    let private readResource =
        Nacara.Core.Resource.text (Reflection.Assembly.GetExecutingAssembly())

    let bundleScript = lazy readResource "bundle.mjs"

    let private fableLine =
        Regex(
            @"^(?<file>.+?)\((?<line>\d+),(?<column>\d+)\): \(\d+,\d+\) (?<severity>error|warning) (?<code>\w+): (?<message>.*?)(?: \(code \d+\))?$",
            RegexOptions.Compiled
        )

    /// <summary>Read Fable's diagnostics and place each on the page it came from.</summary>
    /// <remarks>Warnings from lines the generator wrote rather than a reader are dropped.</remarks>
    let fableMessages (units: SolidPageUnit list) (output: string) =
        [
            for line in output.Split('\n') do
                let matched = fableLine.Match(line.TrimEnd('\r'))

                if matched.Success then
                    // Split by hand: Path.GetFileName on Linux leaves a Windows path whole.
                    let file = matched.Groups["file"].Value.Split([| '/'; '\\' |]) |> Array.last
                    let isError = matched.Groups["severity"].Value = "error"
                    let page = units |> List.tryFind (fun unit -> SolidGenerate.fileName unit.Key = file)

                    let at =
                        page
                        |> Option.bind (fun unit ->
                            SolidGenerate.locate
                                unit.Spans
                                (int matched.Groups["line"].Value)
                                (int matched.Groups["column"].Value)
                        )

                    if isError || at.IsSome then
                        {
                            Page = page
                            At = at
                            IsError = isError
                            Message = matched.Groups["message"].Value
                        }
        ]
        |> List.distinct

    let private general isError message =
        {
            Page = None
            At = None
            IsError = isError
            Message = message
        }

    let private tail (text: string) =
        let lines = text.Trim().Split('\n')
        lines[max 0 (lines.Length - 30) ..] |> String.concat "\n"

    let private readOutput (directory: string) =
        if Directory.Exists directory then
            Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories)
            |> Seq.map (fun file ->
                Path.GetRelativePath(directory, file).Replace('\\', '/'), File.ReadAllText file
            )
            |> Seq.sortBy fst
            |> List.ofSeq
        else
            []

    /// <summary>
    /// The text of a project file, of each file it compiles, and of each project it references, recursively.
    /// </summary>
    /// <remarks>Reads the paths that name existing files and hold no MSBuild property.</remarks>
    let projectInputs (fsproj: string) : string list =
        let visited = Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase)

        let rec inputs (path: string) =
            let path = Path.GetFullPath path

            if not (File.Exists path) || not (visited.Add path) then
                []
            else

            let text = File.ReadAllText path
            let directory = Path.GetDirectoryName path
            let document = Xml.Linq.XDocument.Parse text

            let includes (item: string) =
                document.Descendants()
                |> Seq.filter (fun element -> element.Name.LocalName = item)
                |> Seq.choose (fun element -> Option.ofObj (element.Attribute(Xml.Linq.XName.Get "Include")))
                |> Seq.map _.Value
                |> Seq.filter (fun value -> not (value.Contains "$("))
                |> Seq.map (fun value -> Path.GetFullPath(Path.Combine(directory, value.Replace('\\', '/'))))
                |> List.ofSeq

            [
                text
                for file in includes "Compile" do
                    if File.Exists file then
                        File.ReadAllText file
                for project in includes "ProjectReference" do
                    yield! inputs project
            ]

        inputs fsproj

    /// <summary>The arguments after <c>dotnet fable</c> (or <c>dotnet fable watch</c>) for the workspace.</summary>
    /// <remarks>
    /// <c>--typedArrays false</c> is what Partas.Solid requires: otherwise a numeric array compiles
    /// to a typed array, which Solid stores do not wrap.
    /// </remarks>
    let fableArguments =
        [
            "Docs.fsproj"
            "-e"
            ".fs.jsx"
            "-c"
            "Release"
            "--optimize"
            "--typedArrays"
            "false"
            "--exclude"
            "Partas.Solid.FablePlugin"
        ]

    /// <summary>Everything that decides the output, so an unchanged site skips the tools.</summary>
    let fingerprint (options: SolidExamplesOptions) (units: SolidPageUnit list) =
        [
            options.PartasVersion
            yield! options.Feeds
            match options.PartasProject with
            | Some project ->
                $"partas:%s{project}"
                yield! projectInputs project
            | None -> ()
            // Packages change the project and package.json, and so what Fable and rolldown produce.
            for id, version in options.NuGetPackages do
                $"nuget:%s{id}@%s{version}"
            for name, version in options.NpmPackages do
                $"npm:%s{name}@%s{version}"
            options.TargetFramework
            for name, value in options.Properties do
                $"property:%s{name}=%s{value}"
            // A referenced project's sources are compiled into the bundle, so an edit to one rebuilds it.
            for project in options.Projects do
                $"project:%s{project}"
                yield! projectInputs project
            options.FableVersion
            // A changed flag changes every module Fable writes, as --typedArrays does.
            yield! fableArguments
            options.SolidVersion
            options.RolldownVersion
            string options.Minify
            bundleScript.Value
            for unit in units do
                unit.Key
                unit.Code
                SolidGenerate.entry unit.Key unit.Cells

                for cell in unit.Cells do
                    if cell.Jsx then
                        cell.Id
        ]
        |> String.concat "\u0000"
        |> SolidGenerate.hash 16

    /// <summary>The JSX Fable made of each cell marked <c>jsx</c>, by page key and cell id.</summary>
    /// <remarks>
    /// Read from Fable's output rather than the bundle, which the Solid compiler has already
    /// rewritten. A page Fable has not written yet has none.
    /// </remarks>
    let jsx (options: SolidExamplesOptions) (root: string) (units: SolidPageUnit list) =
        let workspace = Path.GetFullPath(Path.Combine(root, options.WorkspacePath))

        [
            for unit in units do
                let file = Path.Combine(workspace, SolidGenerate.fileName unit.Key + ".jsx")

                if unit.Cells |> List.exists _.Jsx && File.Exists file then
                    unit.Key, SolidGenerate.jsxCode unit.Cells (File.ReadAllText file)
        ]
        |> Map.ofList

    /// <summary>A watcher, and the output of the last compilation it finished.</summary>
    type private Watching =
        {
            Watcher: SolidFableWatcher
            /// <summary>The environment the watcher was started with.</summary>
            Properties: (string * string) list
            mutable Last: string
        }

    /// <summary>Watchers by workspace. Only <c>nacara watch</c> starts one, and it lives as long as the process.</summary>
    let private watchers = Collections.Generic.Dictionary<string, Watching>()

    let private stopWatching (workspace: string) =
        match watchers.TryGetValue workspace with
        | true, watching ->
            (watching.Watcher :> IDisposable).Dispose()
            watchers.Remove workspace |> ignore
        | _ -> ()

    /// <summary>The running watcher for a workspace, started when there is none or its properties differ.</summary>
    /// <returns>The watcher, and whether it was started just now.</returns>
    let private watcherFor (workspace: string) (properties: (string * string) list) =
        match watchers.TryGetValue workspace with
        | true, watching when not watching.Watcher.HasExited && watching.Properties = properties -> watching, false
        | _ ->
            stopWatching workspace

            let watching =
                {
                    Watcher = new SolidFableWatcher(workspace, properties, fableArguments)
                    Properties = properties
                    Last = ""
                }

            watchers[workspace] <- watching
            watching, true

    /// <summary>Bring the workspace up to date and compile it.</summary>
    /// <param name="options">The plugin's options.</param>
    /// <param name="root">The site's project root.</param>
    /// <param name="watch">
    /// Keep a <c>dotnet fable watch</c> running between calls, so an edit recompiles one module rather
    /// than starting Fable cold.
    /// </param>
    /// <param name="log">Told what is taking the time.</param>
    /// <param name="units">Every page with examples.</param>
    let compile
        (options: SolidExamplesOptions)
        (root: string)
        (watch: bool)
        (log: string -> unit)
        (units: SolidPageUnit list)
        =
        let resolve (path: string) = Path.GetFullPath(Path.Combine(root, path))

        let options =
            { options with
                Projects = options.Projects |> List.map resolve
                PartasProject = options.PartasProject |> Option.map resolve }

        let workspace = Path.GetFullPath(Path.Combine(root, options.WorkspacePath))
        let outDir = Path.Combine(workspace, "out")
        let stampFile = Path.Combine(workspace, ".stamp")
        let key = fingerprint options units
        let at (path: string) = Path.Combine(workspace, path)
        let run = SolidWorkspace.run options.Timeout [] workspace
        let previous () = readOutput outDir

        let failed (messages: SolidMessage list) =
            {
                Files = previous ()
                Messages = messages
                Fresh = false
            }

        if File.Exists stampFile && File.ReadAllText stampFile = key && Directory.Exists outDir then
            // Started now, the watcher has its first compilation done before the first edit arrives.
            if watch then
                watcherFor workspace options.Properties |> ignore

            {
                Files = previous ()
                Messages = []
                Fresh = true
            }
        else

        Directory.CreateDirectory workspace |> ignore
        // A stamp that outlives a failed run would let the next build skip it.
        if File.Exists stampFile then
            File.Delete stampFile

        let sources = units |> List.map (fun unit -> SolidGenerate.fileName unit.Key)

        // Taken before anything is written, so the compilation the writes cause is the one waited for.
        let mark =
            match watchers.TryGetValue workspace with
            | true, watching -> watching.Watcher.Mark()
            | _ -> 0

        let configChanged = SolidWorkspace.write (at "nuget.config") (SolidWorkspace.nugetConfig options)
        SolidWorkspace.write (at "bundle.mjs") bundleScript.Value |> ignore
        let projectChanged = SolidWorkspace.write (at "Docs.fsproj") (SolidWorkspace.projectFile options sources)
        let toolsChanged = SolidWorkspace.write (at ".config/dotnet-tools.json") (SolidWorkspace.toolManifest options)
        let packagesChanged = SolidWorkspace.write (at "package.json") (SolidWorkspace.packageJson options)

        // A watcher reads the project once; one with a stale view of it is started again.
        if configChanged || projectChanged || toolsChanged then
            stopWatching workspace

        let mutable sourcesChanged = false

        for unit in units do
            if SolidWorkspace.write (at (SolidGenerate.fileName unit.Key)) unit.Code then
                sourcesChanged <- true

            SolidWorkspace.write (at $"entries/%s{unit.Key}.js") (SolidGenerate.entry unit.Key unit.Cells) |> ignore

        // Pages that no longer have examples leave their module behind otherwise.
        for file in Directory.EnumerateFiles(workspace, "P*.fs*") do
            let name = Path.GetFileName file

            if not (sources |> List.exists (fun source -> name = source || name = source + ".jsx")) then
                File.Delete file

        if Directory.Exists(at "entries") then
            for file in Directory.EnumerateFiles(at "entries") do
                if not (units |> List.exists (fun unit -> Path.GetFileName file = unit.Key + ".js")) then
                    File.Delete file

        let npm =
            if packagesChanged || not (Directory.Exists(at "node_modules")) then
                log "installing solid-js, @solidjs/compiler and rolldown"
                run "npm" [ "install"; "--no-audit"; "--no-fund"; "--loglevel=error" ]
            else
                { ExitCode = 0; Output = ""; TimedOut = false }

        if npm.ExitCode <> 0 then
            failed [ general true $"npm install failed:\n%s{tail npm.Output}" ]
        else

        let tools =
            if toolsChanged || not (File.Exists(at ".config/.restored")) then
                let restored = run "dotnet" [ "tool"; "restore" ]

                if restored.ExitCode = 0 then
                    File.WriteAllText(at ".config/.restored", options.FableVersion)

                restored
            else
                { ExitCode = 0; Output = ""; TimedOut = false }

        if tools.ExitCode <> 0 then
            failed [ general true $"dotnet tool restore failed:\n%s{tail tools.Output}" ]
        else

        let fable =
            if watch then
                let watching, started = watcherFor workspace options.Properties

                if started then
                    log $"starting Fable's watch over %d{units.Length} page(s) of examples"

                let finished (output: string) =
                    let failed = fableMessages units output |> List.exists _.IsError

                    {
                        ExitCode = (if failed then 1 else 0)
                        Output = output
                        TimedOut = false
                    }

                if started || sourcesChanged then
                    let mark = if started then 0 else mark

                    match watching.Watcher.Wait(mark, TimeSpan.FromMilliseconds 300., options.Timeout) with
                    | Ok output ->
                        watching.Last <- output
                        finished output
                    | Error problem ->
                        stopWatching workspace

                        {
                            ExitCode = -1
                            Output = problem
                            TimedOut = false
                        }
                else
                    // Nothing Fable reads changed, so its last compilation stands.
                    finished watching.Last
            else
                log $"compiling %d{units.Length} page(s) of examples with Fable"
                SolidWorkspace.run options.Timeout options.Properties workspace "dotnet" ("fable" :: fableArguments)

        let messages = fableMessages units fable.Output

        if fable.TimedOut then
            failed [ general true $"Fable did not finish within %O{options.Timeout}" ]
        elif fable.ExitCode <> 0 then
            let messages =
                if messages |> List.exists _.IsError then
                    messages
                else
                    messages @ [ general true $"Fable failed:\n%s{tail fable.Output}" ]

            failed messages
        else

        if Directory.Exists outDir then
            Directory.Delete(outDir, true)

        let manifest =
            let entries =
                units
                |> List.map (fun unit ->
                    let path = (at $"entries/%s{unit.Key}.js").Replace('\\', '/')
                    $"\"%s{unit.Key}\": \"%s{path}\""
                )
                |> String.concat ", "

            let outPath = outDir.Replace('\\', '/')
            let minify = if options.Minify then "true" else "false"
            $"""{{ "outDir": "%s{outPath}", "minify": %s{minify}, "entries": {{ %s{entries} }} }}"""

        File.WriteAllText(at "manifest.json", manifest)
        let bundled = run "node" [ "bundle.mjs"; "manifest.json" ]

        if bundled.ExitCode <> 0 then
            failed (messages @ [ general true $"bundling failed:\n%s{tail bundled.Output}" ])
        else

        File.WriteAllText(stampFile, key)

        {
            Files = readOutput outDir
            Messages = messages
            Fresh = true
        }
