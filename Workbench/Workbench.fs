/// Compile Partas.Solid projects with Fable from inside a SageFs session.
///
/// The session keeps Fable's F# checker warm, so a compile of the snapshot suite takes seconds rather than the
/// minute a `dotnet fable` run spends starting up. Bindings and test inputs are F# sources that Fable re-reads
/// on every compile, so edits to them need no reload at all. The plugin is a DLL: after editing it, call
/// `Workbench.reloadPlugin ()`, which rebuilds it and loads the new copy next to the old one.
///
/// In the session (working directory Workbench/):
///     Workbench.selfCheck ();;
///     Workbench.cases ();;                                  // every snapshot case, as "SolidCases.MergeProps"
///     Workbench.check "MergeProps";;                        // compile the case, diff against its .expected
///     Workbench.checkAll ();;                               // the whole snapshot suite, in one type-check
///     Workbench.accept "MergeProps";;                       // write the case's output to its .expected
///     Workbench.reloadPlugin ();;                           // after editing Partas.Solid.FablePlugin
///     Workbench.compile "<path to .fsproj>" "<path to .fs>";;
///     Workbench.emit Workbench.Suite.Dom;;                  // write .fs.jsx for a runtime suite, then
///                                                           // `node run.mjs dom --no-compile`
///     Workbench.watchCases [ "MergeProps" ];;               // on every save: reload if needed, re-check
///     Workbench.watchSuite Workbench.Suite.Dom;;            // on every save: reload if needed, re-emit
///     Workbench.watchFiles [ "<path to .fs or .fs.jsx>" ];; // on every save: reload if needed, re-emit those
///     Workbench.unwatch ();;
///     Workbench.cleanPluginRefs ();;                         // delete unused plugin DLL copies (also automatic)
///
/// A top-level module on purpose: SageFs rejects 'namespace' in evals.
module Workbench

open System
open System.IO
open System.Diagnostics
open Fable
open Fable.Compiler.Util
open Fable.Compiler.ProjectCracker
open Fable.Transforms.State
open FSharp.Compiler.SourceCodeServices

let repoRoot = Path.GetFullPath(Path.Join(__SOURCE_DIRECTORY__, ".."))

/// The snapshot test project, and the root its case names are relative to.
let casesProject =
    Path.Join(repoRoot, "Partas.Solid.Tests.Plugin", "Compiled", "Partas.Solid.Tests.Plugin.Compiled.fsproj")

let casesRoot = Path.GetDirectoryName casesProject

/// The runtime test suites (Partas.Solid.Tests.Runtime/<Suite>).
type Suite =
    | Primitives
    | Dom
    | Integration

    member this.Project =
        let name = string this
        Path.Join(repoRoot, "Partas.Solid.Tests.Runtime", name, $"Partas.Solid.Tests.Runtime.{name}.fsproj")

/// Serialises the session's own compiles and plugin reloads: a watcher runs them on a background thread.
let private gate = obj ()

// ---------------------------------------------------------------- the compile lock

/// Runs `f` holding run.mjs's cross-process compile lock (Partas.Solid.Tests.Runtime/lib.mjs, same protocol), so
/// the session's plugin builds and suite writes never race run.mjs, watch.mjs or another session.
let private withCompileLock (f: unit -> 'T) : 'T =
    let lockDir = Path.Join(repoRoot, "Partas.Solid.Tests.Runtime", ".compile-lock")
    let ownerFile = Path.Join(lockDir, "owner.json")
    let host = Net.Dns.GetHostName()
    let token = $"{host}:{Environment.ProcessId}:{Guid.NewGuid()}"
    let owner () =
        try
            let o = Text.Json.Nodes.JsonNode.Parse(File.ReadAllText ownerFile)
            Some(string o["host"], int o["pid"], string o["token"])
        with _ -> None
    let isStale () =
        let alive pid = try not (Process.GetProcessById(pid).HasExited) with _ -> false
        let age =
            [ ownerFile; lockDir ]
            |> List.tryFind (fun p -> File.Exists p || Directory.Exists p)
            |> Option.map (fun p -> DateTime.UtcNow - File.GetLastWriteTimeUtc p)
            |> Option.defaultValue TimeSpan.Zero
        match owner () with
        | Some(h, pid, _) when h = host && not (alive pid) -> true
        | _ -> age > TimeSpan.FromMinutes 2.
    // mkdir is not atomic in .NET, so build the lock under another name and move it into place.
    let tryAcquire () =
        let staging = $"{lockDir}.{Environment.ProcessId}.tmp"
        Directory.CreateDirectory staging |> ignore
        let json =
            Text.Json.JsonSerializer.Serialize
                {| pid = Environment.ProcessId; host = host; token = token; since = DateTime.UtcNow.ToString "o" |}
        File.WriteAllText(Path.Join(staging, "owner.json"), json)
        try
            Directory.Move(staging, lockDir)
            true
        with :? IOException ->
            Directory.Delete(staging, true)
            false
    let deadline = DateTime.UtcNow.AddMinutes 30.
    let mutable announced = false
    while not (tryAcquire ()) do
        if isStale () then
            let grave = $"{lockDir}.stale.{Environment.ProcessId}"
            try
                Directory.Move(lockDir, grave)
                Directory.Delete(grave, true)
            with _ -> ()
        else
            if not announced then
                printfn "Waiting for the compile lock (Partas.Solid.Tests.Runtime/.compile-lock)..."
                announced <- true
            if DateTime.UtcNow > deadline then failwith $"Timed out waiting for {lockDir}"
            Threading.Thread.Sleep 500
    // The heartbeat: run.mjs treats a lock whose owner.json is 2 min old as abandoned.
    use _heartbeat =
        new Threading.Timer((fun _ -> try File.SetLastWriteTimeUtc(ownerFile, DateTime.UtcNow) with _ -> ()), null, 5000, 5000)
    try
        f ()
    finally
        match owner () with
        | Some(_, _, t) when t = token -> try Directory.Delete(lockDir, true) with _ -> ()
        | _ -> ()

// ---------------------------------------------------------------- the plugin

/// The plugin's project, rebuilt by reloadPlugin.
let pluginProject =
    Path.Join(repoRoot, "Partas.Solid.FablePlugin", "Partas.Solid.FablePlugin.fsproj")

/// A collectible context for one reloaded copy of the plugin. It resolves nothing itself, so Fable.AST and
/// FSharp.Core come from the session and the plugin's types stay compatible with Fable's.
type private PluginLoadContext(name: string) =
    inherit System.Runtime.Loader.AssemblyLoadContext(name, isCollectible = true)
    override _.Load(_: System.Reflection.AssemblyName) : System.Reflection.Assembly = null

/// What a project referencing the plugin can see of it: its public types and members, attributes and enum values.
/// A hash of it tells whether a rebuild changed anything the checker holds.
let private publicSurface (a: System.Reflection.Assembly) =
    let flags =
        Reflection.BindingFlags.Public ||| Reflection.BindingFlags.Instance
        ||| Reflection.BindingFlags.Static ||| Reflection.BindingFlags.DeclaredOnly
    let lines =
        // SageFs adds a __SageFsCoverage type to the copy of the plugin it loads with the session.
        [ for t in a.GetExportedTypes() |> Array.filter (fun t -> not (t.Name.StartsWith "__SageFs")) |> Array.sortBy _.FullName do
              yield $"{t.FullName} {t.Attributes} : {t.BaseType}"
              for i in t.GetInterfaces() |> Array.map string |> Array.sort do yield $"  : {i}"
              for c in t.GetCustomAttributesData() |> Seq.map string |> Seq.sort do yield $"  [{c}]"
              for m in t.GetMembers flags |> Array.map (fun m ->
                           match m with
                           | :? Reflection.FieldInfo as f when f.IsLiteral -> $"{f} = {f.GetRawConstantValue()}"
                           | _ -> string m)
                       |> Array.sort do
                  yield $"  {m}" ]
    let bytes = Text.Encoding.UTF8.GetBytes(String.concat "\n" lines)
    Convert.ToHexString(Security.Cryptography.SHA256.HashData bytes).Substring(0, 16)

let mutable private plugin = typeof<Partas.Solid.SolidComponentAttribute>.Assembly
let mutable private pluginContext: PluginLoadContext option = None
let mutable private reloads = 0
let mutable private surface = publicSurface plugin
/// Advances when a reload changes the plugin's public surface; checkers created before then are stale.
let mutable private surfaceVersion = 0

/// The plugin assembly the next compile uses: the one the session loaded, or the last reloadPlugin copy.
let pluginAssembly () = plugin

/// Rebuilds Partas.Solid.FablePlugin (Release, as the CLI builds it) and makes the next compile use the new
/// build. Warm projects keep their cracked options. When the plugin's public surface changed (ComponentFlag, the
/// attributes), their checkers are recreated on next use, which costs a full type-check; otherwise they stay
/// warm. Fails with the build output when the plugin does not compile.
let reloadPlugin () =
    let sw = Stopwatch.StartNew()
    lock gate <| fun () ->
    withCompileLock (fun () ->
        let psi =
            ProcessStartInfo("dotnet", [ "build"; pluginProject; "-c"; "Release"; "--nologo"; "-v"; "q"; "-clp:NoSummary" ])
        psi.RedirectStandardOutput <- true
        psi.RedirectStandardError <- true
        use p = Process.Start psi
        let out = p.StandardOutput.ReadToEndAsync()
        let err = p.StandardError.ReadToEndAsync()
        p.WaitForExit()
        if p.ExitCode <> 0 then
            failwithf "Plugin build failed:\n%s%s" out.Result err.Result
        let dll =
            Path.Join(Path.GetDirectoryName pluginProject, "bin", "Release", "net6.0", "Partas.Solid.FablePlugin.dll")
        let context = new PluginLoadContext($"Partas.Solid.FablePlugin #{reloads + 1}")
        // From bytes, so the DLL stays unlocked for the next build.
        let pdb = Path.ChangeExtension(dll, ".pdb")
        use dllStream = new MemoryStream(File.ReadAllBytes dll)
        use pdbStream = if File.Exists pdb then new MemoryStream(File.ReadAllBytes pdb) else null
        plugin <- context.LoadFromStream(dllStream, pdbStream)
        reloads <- reloads + 1
        pluginContext |> Option.iter _.Unload()
        pluginContext <- Some context
        let s = publicSurface plugin
        if s <> surface then
            surface <- s
            surfaceVersion <- surfaceVersion + 1
            printfn "Plugin's public surface changed: checkers are recreated on next compile.")
    printfn $"Plugin reloaded (#%d{reloads}, %.0f{sw.Elapsed.TotalMilliseconds} ms)"

/// Fable's plugin loader, redirected to pluginAssembly (). Other plugins load as Fable would.
let private getPlugin (cliArgs: CliArgs) (r: PluginRef) : System.Type =
    if Path.GetFileNameWithoutExtension r.DllPath = "Partas.Solid.FablePlugin" then
        plugin.GetTypes()
        |> Array.tryFind (fun t -> t.FullName.Replace("+", ".") = r.TypeFullName)
        |> Option.defaultWith (fun () -> failwith $"The plugin assembly has no type {r.TypeFullName}")
    else
        Reflection.loadType cliArgs r

// ---------------------------------------------------------------- projects

/// The Fable version in use, e.g. "5.13.0". workbench.fsx sets it up from .config/dotnet-tools.json.
let fableVersion = typeof<CrackerResponse>.Assembly.GetName().Version.ToString 3

/// fable_modules/fable-library-js.<version> of a project, as the CLI names it.
let private fableLibraryTarget (projDir: string) =
    Path.Join(projDir, "fable_modules", $"fable-library-js.{fableVersion}")

/// Copies fable-library-js from the Fable tool package (restored by `dotnet tool restore`) into a project's
/// fable_modules, as the CLI does on every --noCache run.
let private copyFableLibrary (projDir: string) =
    let packages =
        match Environment.GetEnvironmentVariable "NUGET_PACKAGES" with
        | null | "" -> Path.Join(Environment.GetFolderPath Environment.SpecialFolder.UserProfile, ".nuget", "packages")
        | dir -> dir
    let source = Path.Join(packages, "fable", fableVersion, "fable-library-js")
    if not (Directory.Exists source) then
        failwith $"No fable-library-js at {source}. Run `dotnet tool restore` in the repo root."
    let target = fableLibraryTarget projDir
    for file in Directory.EnumerateFiles(source, "*", SearchOption.AllDirectories) do
        let dest = Path.Join(target, Path.GetRelativePath(source, file))
        Directory.CreateDirectory(Path.GetDirectoryName dest) |> ignore
        File.Copy(file, dest, true)

/// What `dotnet fable --exclude Partas.Solid.FablePlugin --noCache -e .fs.jsx -c Release --optimize` passes: as
/// the snapshot tests run it, or with `-o .` as the runtime suites do (Partas.Solid.Tests.Runtime/lib.mjs).
/// Cracking with these options resets the project's fable_modules, as the CLI does.
let private cliArgsFor (projFile: string) =
    let projDir = Path.GetDirectoryName projFile
    let isSuite = Path.GetFileName(projFile).StartsWith("Partas.Solid.Tests.Runtime.", StringComparison.Ordinal)
    { CliArgs.ProjectFile = projFile
      // Where the CLI puts it. An explicit path, because Fable looks for its copy of the library next to the
      // running process, and that is SageFs here; warm copies it in from the Fable tool package instead.
      FableLibraryPath = Some(fableLibraryTarget projDir)
      RootDir = projDir
      Configuration = "Release"
      OutDir = if isSuite then Some projDir else None
      IsWatch = false
      Precompile = false
      PrecompiledLib = None
      PrintAst = false
      SourceMaps = false
      SourceMapsRoot = None
      NoRestore = false
      NoCache = true
      NoGitignore = false
      NoParallelTypeCheck = false
      Exclude = [ "Partas.Solid.FablePlugin" ]
      Replace = Map.empty
      RunProcess = None
      CompilerOptions =
        CompilerOptionsHelper.Make(
            define = [ "FABLE_COMPILER"; "FABLE_COMPILER_5"; "FABLE_COMPILER_JAVASCRIPT" ],
            debugMode = false,
            fileExtension = ".fs.jsx",
            optimizeFSharpAst = true
        )
      Verbosity = Verbosity.Silent }

/// The Fable CLI's path resolver (Fable.Cli/Main.fs), one per project: with an outDir, each source directory
/// maps to one target directory.
let private pathResolver () =
    let targetDirs = Collections.Concurrent.ConcurrentDictionary<string, string>()
    { new PathResolver with
        member _.TryPrecompiledOutPath(_sourceDir, _relativePath) = None
        member _.GetOrAddDeduplicateTargetDir(importDir, addTargetDir) =
            targetDirs.GetOrAdd(importDir.ToLower(), fun _ -> set targetDirs.Values |> addTargetDir) }

/// Where the Fable CLI writes a source's JavaScript (Fable.Cli/Main.fs, getOutPath).
let private outPath (cliArgs: CliArgs) (resolver: PathResolver) (file: string) =
    let ext = cliArgs.CompilerOptions.FileExtension
    let path =
        match cliArgs.OutDir with
        | Some outDir -> Imports.getTargetAbsolutePath resolver file (Path.GetDirectoryName cliArgs.ProjectFile) outDir
        | None -> file
    File.changeExtensionButUseDefaultExtensionInFableModules JavaScript (Naming.isInFableModules file) path ext

/// The Fable CLI's writer (Fable.Cli/Pipeline.fs), printing to memory.
type private MemoryWriter(com: Compiler, cliArgs: CliArgs, resolver: PathResolver, targetPath: string) =
    let sourcePath = com.CurrentFile
    let buffer = Text.StringBuilder()

    member _.Content = buffer.ToString()

    interface Fable.Transforms.Printer.Writer with
        member _.Dispose() = ()
        member _.Write(str) = async { buffer.Append(str: string) |> ignore }

        member _.MakeImportPath(path) =
            let projDir = Path.GetDirectoryName cliArgs.ProjectFile
            let path = Imports.getImportPath resolver sourcePath targetPath projDir cliArgs.OutDir path
            if path.EndsWith(".fs", StringComparison.Ordinal) then
                let isInFableModules = Path.Combine(Path.GetDirectoryName targetPath, path) |> Naming.isInFableModules
                File.changeExtensionButUseDefaultExtensionInFableModules
                    JavaScript isInFableModules path cliArgs.CompilerOptions.FileExtension
            else
                path

        member _.AddLog(msg, severity, ?range) = com.AddLog(msg, severity, ?range = range, fileName = com.CurrentFile)
        member _.AddSourceMapping(_, _, _, _, _, _) = ()

/// Fable.Compiler.CodeServices.compileFileToJs, which it does not expose.
let private compileFileToJs (com: Compiler) (cliArgs: CliArgs) (resolver: PathResolver) (outPath: string) =
    async {
        let babel =
            Fable.Transforms.FSharp2Fable.Compiler.transformFile com
            |> Fable.Transforms.FableTransforms.transformFile com
            |> Fable.Transforms.Fable2Babel.Compiler.transformFile com
        // The CLI writes no file for a source with no output (only erased types, say).
        if babel.IsEmpty then
            return None
        else
            let writer = new MemoryWriter(com, cliArgs, resolver, outPath)
            do! Fable.Transforms.BabelPrinter.run writer babel
            return Some writer.Content
    }

/// A cracked project with its warm checker.
type WarmProject =
    { CliArgs: CliArgs
      Cracked: CrackerResponse
      Checker: InteractiveChecker
      /// The plugin surface version the checker was created at: it holds the plugin's metadata from then.
      PluginSurface: int
      PathResolver: PathResolver }

let private projects = Collections.Concurrent.ConcurrentDictionary<string, WarmProject>()

/// A checker for the cracked project that references a copy of the plugin DLL, one per public surface under
/// .workbench/plugin-ref/, rather than bin/Release, which the next reloadPlugin overwrites while the checker
/// is still in use.
let private pluginRefRoot = Path.Join(repoRoot, ".workbench", "plugin-ref")

/// Deletes the plugin DLL copies of every surface but the current one. A copy a checker still has open (a
/// project not compiled since the surface changed, or another session) stays, and goes on a later run.
/// Returns the number deleted and the number left.
let private prunePluginRefs () =
    if not (Directory.Exists pluginRefRoot) then
        0, 0
    else
        let results =
            [ for dir in Directory.GetDirectories pluginRefRoot do
                  if Path.GetFileName dir <> surface then
                      yield
                          try
                              Directory.Delete(dir, true)
                              true
                          with
                          | :? IOException
                          | :? UnauthorizedAccessException -> false ]
        results |> List.filter id |> List.length, results |> List.filter not |> List.length

/// Deletes the plugin DLL copies in .workbench/plugin-ref/ that no current checker uses. Runs by itself whenever a
/// checker is created; this is for doing it now.
let cleanPluginRefs () =
    let deleted, left = prunePluginRefs ()
    printfn $"plugin-ref: deleted %d{deleted}, %d{left} still in use"

let private createChecker (cracked: CrackerResponse) =
    let copy =
        lazy
            let dir = Path.Join(pluginRefRoot, surface)
            let dll = Path.Join(dir, "Partas.Solid.FablePlugin.dll")
            if not (File.Exists dll) then
                Directory.CreateDirectory dir |> ignore
                let built = Path.Join(Path.GetDirectoryName pluginProject, "bin", "Release", "net6.0", "Partas.Solid.FablePlugin.dll")
                File.Copy(built, dll + ".tmp", true)
                File.Move(dll + ".tmp", dll, true)
            prunePluginRefs () |> ignore
            Path.normalizePath dll
    let options = cracked.ProjectOptions
    let otherOptions =
        options.OtherOptions
        |> Array.map (fun o ->
            if o.StartsWith "-r:" && Path.GetFileName(o.Substring 3) = "Partas.Solid.FablePlugin.dll" then
                "-r:" + copy.Value
            else
                o)
    InteractiveChecker.Create { options with OtherOptions = otherOptions }

/// Cracks the project (an MSBuild design-time build) and creates its checker, once per project. After a
/// reloadPlugin, the checker is recreated from the cracked options.
let warm (fsproj: string) =
    let fsproj = Path.normalizeFullPath fsproj
    let w =
        projects.GetOrAdd(fsproj, fun p ->
            let cliArgs = cliArgsFor p
            let resolver: ProjectCrackerResolver = Fable.Compiler.MSBuildCrackerResolver()
            // Cracking builds the referenced projects and rewrites fable_modules, which run.mjs also does.
            let cracked =
                withCompileLock (fun () ->
                    let cracked = CrackerOptions(cliArgs, false) |> getFullProjectOpts resolver
                    copyFableLibrary (Path.GetDirectoryName p)
                    cracked)
            { CliArgs = cliArgs
              Cracked = cracked
              Checker = createChecker cracked
              PluginSurface = surfaceVersion
              PathResolver = pathResolver () })
    if w.PluginSurface = surfaceVersion then
        w
    else
        let w = { w with Checker = createChecker w.Cracked; PluginSurface = surfaceVersion }
        projects[fsproj] <- w
        w

/// Forget cracked projects. Needed after editing an .fsproj or adding a source file (a new case).
let reset () = projects.Clear()

type Compiled =
    { /// Source file (normalized full path) -> JSX.
      Js: Map<string, string>
      Logs: LogEntry[]
      ElapsedMs: float }

    member this.Errors = this.Logs |> Array.filter (fun l -> l.Severity = Severity.Error)

let private formatLog (l: LogEntry) =
    let file = l.FileName |> Option.map (fun f -> Path.GetRelativePath(repoRoot, f)) |> Option.defaultValue ""
    let line = l.Range |> Option.map (fun r -> $"({r.start.line},{r.start.column})") |> Option.defaultValue ""
    $"{l.Severity} {l.Tag} {file}{line}: {l.Message}"

/// An F# diagnostic as a Fable log entry, tagged "FSHARP" as Fable's CLI does.
let private fsharpLog (d: FSharp.Compiler.Diagnostics.FSharpDiagnostic) =
    let severity =
        match d.Severity with
        | FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity.Error -> Severity.Error
        | FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity.Warning -> Severity.Warning
        | _ -> Severity.Info
    let range =
        Fable.AST.SourceLocation.Create(
            start = { line = d.StartLine; column = d.StartColumn + 1 },
            ``end`` = { line = d.EndLine; column = d.EndColumn + 1 })
    LogEntry.Make(severity, $"{d.Message} (code {d.ErrorNumber})", fileName = d.FileName, range = range, tag = "FSHARP")

/// Type-checks the project and compiles `files` (default: every source outside fable_modules) to JSX, keyed by
/// source file. A source with no output (only erased types, say) has no entry, as the CLI writes no file for it. Sources are re-read on every call, so edits are always seen.
let compileProjectFiles (fsproj: string) (files: string list option) =
    lock gate <| fun () ->
    let sw = Stopwatch.StartNew()
    let w = warm fsproj
    let projDir = Path.normalizeFullPath (Path.GetDirectoryName w.CliArgs.ProjectFile)
    let _, sourceReader =
        w.Cracked.ProjectOptions.SourceFiles
        |> Array.map Fable.Compiler.File
        |> Fable.Compiler.File.MakeSourceReader
    let files =
        match files with
        | Some fs -> fs |> List.map Path.normalizeFullPath
        | None ->
            w.Cracked.ProjectOptions.SourceFiles
            |> Array.filter (fun f -> f.StartsWith(projDir + "/") && not (Naming.isInFableModules f))
            |> List.ofArray
    let result =
        async {
            let! typeChecked = Fable.Compiler.CodeServices.typeCheckProject sourceReader w.Checker w.CliArgs w.Cracked
            let fableProj =
                Project.From(
                    w.CliArgs.ProjectFile,
                    w.Cracked.ProjectOptions,
                    typeChecked.ProjectCheckResults.AssemblyContents.ImplementationFiles,
                    typeChecked.Assemblies,
                    Log.log,
                    getPlugin = getPlugin w.CliArgs
                )
            let opts = w.CliArgs.CompilerOptions
            let! results =
                files
                |> List.map (fun currentFile ->
                    async {
                        let fableLibDir = Path.getRelativePath currentFile w.Cracked.FableLibDir
                        let compiler =
                            CompilerImpl(currentFile, fableProj, opts, fableLibDir, w.Cracked.OutputType, ?outDir = w.CliArgs.OutDir)
                        let! js =
                            compileFileToJs (compiler :> Compiler) w.CliArgs w.PathResolver (outPath w.CliArgs w.PathResolver currentFile)
                        return (currentFile, js), compiler.Logs
                    })
                |> Async.Parallel
            let compiled, logs = Array.unzip results
            let fsharpLogs = typeChecked.ProjectCheckResults.Diagnostics |> Array.map fsharpLog
            return compiled |> Array.choose (fun (f, js) -> js |> Option.map (fun js -> f, js)) |> Map.ofArray, Array.append fsharpLogs (Array.concat logs)
        }
        |> Async.RunSynchronously
    sw.Stop()
    { Js = fst result; Logs = snd result; ElapsedMs = sw.Elapsed.TotalMilliseconds }

/// Compiles one source file of a project and returns its JSX. Fails with the logs when there is none.
let compile (fsproj: string) (file: string) =
    let file = Path.normalizeFullPath file
    let c = compileProjectFiles fsproj (Some [ file ])
    match c.Js |> Map.tryFind file with
    | Some js when c.Errors.Length = 0 -> js
    | _ -> failwithf "Fable failed on %s:\n%s" file (c.Errors |> Array.map formatLog |> String.concat "\n")

/// Warnings and errors of the last type-check and compile of `files`, readable.
let logs (c: Compiled) =
    c.Logs |> Array.filter (fun l -> l.Severity <> Severity.Info) |> Array.map formatLog

// ---------------------------------------------------------------- snapshot cases

/// Line endings and surrounding whitespace are not significant, as in Partas.Solid.Tests.Plugin.
let normalize (s: string) = s.Replace("\r\n", "\n").Trim()

/// A snapshot test file: <Category>Cases/<folder>/<Name>.fs next to <Name>.expected.
type Case =
    { /// "SolidCases.MergeProps": the category and the file name.
      Name: string
      /// "SolidCases.Setting default properties": the Expecto test's name, from the folder (plus ".<Name>.fs.jsx"
      /// when the folder holds more than one case).
      Test: string
      Source: string
      Expected: string }

let cases () =
    let found =
        Directory.EnumerateFiles(casesRoot, "*.expected", SearchOption.AllDirectories)
        |> Seq.filter (fun e -> not (e.Contains "fable_modules") && not (e.Contains $"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}"))
        |> List.ofSeq
    let perFolder = found |> List.countBy Path.GetDirectoryName |> Map.ofList
    found
    |> List.map (fun expected ->
        let source = expected.Substring(0, expected.Length - ".expected".Length) + ".fs"
        let parts = Path.GetRelativePath(casesRoot, expected).Split([| '/'; '\\' |])
        let stem = Path.GetFileNameWithoutExtension expected
        { Name = $"{parts[0]}.{stem}"
          Test =
            if perFolder[Path.GetDirectoryName expected] > 1 then $"{parts[0]}.{parts[1]}.{stem}.fs.jsx"
            else $"{parts[0]}.{parts[1]}"
          Source = Path.normalizeFullPath source
          Expected = expected })
    |> List.sortBy _.Name

/// Finds a case by (a suffix of) its name or its Expecto test name, case-insensitively: "MergeProps",
/// "SolidCases.MergeProps", "Setting default properties".
let findCase (name: string) =
    let all = cases ()
    let is (n: string) =
        n.Equals(name, StringComparison.OrdinalIgnoreCase) || n.EndsWith("." + name, StringComparison.OrdinalIgnoreCase)
    match all |> List.filter (fun c -> is c.Name || is c.Test) with
    | [ c ] -> c
    | [] ->
        let near =
            all |> List.filter (fun c ->
                c.Name.Contains(name, StringComparison.OrdinalIgnoreCase)
                || c.Test.Contains(name, StringComparison.OrdinalIgnoreCase))
        failwithf "No case %s.%s" name (if near.IsEmpty then "" else " Did you mean: " + (near |> List.map _.Name |> String.concat ", "))
    | many -> failwithf "Case %s is ambiguous: %s" name (many |> List.map _.Name |> String.concat ", ")

/// A line diff of expected against actual; empty when they match.
let diff (expected: string) (actual: string) =
    let e, a = (normalize expected).Split '\n', (normalize actual).Split '\n'
    [ for i in 0 .. max e.Length a.Length - 1 do
          let el, al = Array.tryItem i e, Array.tryItem i a
          if el <> al then
              match el with Some l -> yield $"{i + 1,4} - {l}" | None -> ()
              match al with Some l -> yield $"{i + 1,4} + {l}" | None -> () ]

type CaseResult =
    { Case: string
      Passed: bool
      Diff: string list
      Errors: string list }

let private resultOf (c: Case) (compiled: Compiled) =
    let errors =
        compiled.Errors |> Array.filter (fun l -> l.FileName = Some c.Source) |> Array.map formatLog |> List.ofArray
    match compiled.Js |> Map.tryFind c.Source with
    | Some js ->
        let d = diff (File.ReadAllText c.Expected) js
        { Case = c.Name; Passed = d.IsEmpty && errors.IsEmpty; Diff = d; Errors = errors }
    | None -> { Case = c.Name; Passed = false; Diff = []; Errors = "no output" :: errors }

/// Compiles one case (with pluginAssembly ()) and diffs it against its .expected.
let check (name: string) =
    let c = findCase name
    resultOf c (compileProjectFiles casesProject (Some [ c.Source ]))

/// The JSX a case compiles to now.
let jsx (name: string) = compile casesProject (findCase name).Source

/// Every snapshot case in one type-check. Returns the failures; prints a one-line summary.
let checkAll () =
    let all = cases ()
    let compiled = compileProjectFiles casesProject (Some(all |> List.map _.Source))
    let results = all |> List.map (fun c -> resultOf c compiled)
    let failed = results |> List.filter (not << _.Passed)
    printfn $"%d{results.Length - failed.Length}/%d{results.Length} cases pass (%.0f{compiled.ElapsedMs} ms)"
    failed

/// Writes a case's current output to its .expected (and its .fs.jsx), accepting it as the new snapshot.
let accept (name: string) =
    let c = findCase name
    let js = compile casesProject c.Source
    File.WriteAllText(c.Expected, js)
    File.WriteAllText(Path.ChangeExtension(c.Source, ".fs.jsx"), js)
    c.Expected

// ---------------------------------------------------------------- runtime suites

/// Compiles a runtime suite with pluginAssembly () and writes its .fs.jsx files where `dotnet fable -o .` does: each
/// fixture next to its source, the referenced Partas.Solid sources under <Suite>/Partas.Solid/. Then
/// `node run.mjs <suite> --no-compile` (from Partas.Solid.Tests.Runtime/) tests them. Returns the error logs.
///
/// The first emit of a suite in a session cracks it, which resets its fable_modules and copies fable-library
/// in, as the CLI does. Package sources in fable_modules are not compiled, so run `node run.mjs <suite>`
/// once if the suite has never been compiled.
let emit (suite: Suite) =
    let w = warm suite.Project
    let sources =
        w.Cracked.ProjectOptions.SourceFiles |> Array.filter (fun f -> not (Naming.isInFableModules f)) |> List.ofArray
    let compiled = compileProjectFiles suite.Project (Some sources)
    withCompileLock (fun () ->
        for KeyValue(file, js) in compiled.Js do
            let target = outPath w.CliArgs w.PathResolver file
            Directory.CreateDirectory(Path.GetDirectoryName target) |> ignore
            File.WriteAllText(target, js))
    printfn $"%s{string suite}: wrote %d{compiled.Js.Count} files (%.0f{compiled.ElapsedMs} ms)"
    compiled.Errors |> Array.map formatLog

/// The .fs source for a path to a source or to its .fs.jsx output next to it.
let private sourceOf (path: string) =
    let path = Path.normalizeFullPath path
    if path.EndsWith ".fs.jsx" then path.Substring(0, path.Length - 4) else path

/// The nearest .fsproj above a source file.
let private projectOf (file: string) =
    let rec up (dir: string) =
        match Directory.GetFiles(dir, "*.fsproj") with
        | [| p |] -> p
        | [||] when not (isNull (Path.GetDirectoryName dir)) -> up (Path.GetDirectoryName dir)
        | ps -> failwith $"No single .fsproj above {file}: {ps}"
    up (Path.GetDirectoryName file)

/// Compiles just these files, each with its own project (a snapshot case, a runtime fixture, a ScratchTests
/// file), and writes their .fs.jsx where the CLI writes them. Takes sources or their .fs.jsx. Returns the error
/// logs.
let emitFiles (paths: string list) =
    [| for proj, files in paths |> List.map sourceOf |> List.groupBy projectOf do
           let w = warm proj
           let compiled = compileProjectFiles proj (Some files)
           withCompileLock (fun () ->
               for KeyValue(file, js) in compiled.Js do
                   let target = outPath w.CliArgs w.PathResolver file
                   Directory.CreateDirectory(Path.GetDirectoryName target) |> ignore
                   File.WriteAllText(target, js))
           printfn $"  %s{Path.GetFileName proj}: wrote %d{compiled.Js.Count} files (%.0f{compiled.ElapsedMs} ms)"
           yield! compiled.Errors |> Array.map formatLog |]

// ---------------------------------------------------------------- watch

let mutable private watcher: IDisposable option = None

/// Stops the watcher, if one is running.
let unwatch () =
    watcher |> Option.iter _.Dispose()
    watcher <- None

/// The folders a watcher reacts to, and whether a save there needs a plugin reload.
let private watchedDirs =
    [ Path.GetDirectoryName pluginProject, true
      Path.Join(repoRoot, "Partas.Solid"), false
      casesRoot, false
      Path.Join(repoRoot, "Partas.Solid.Tests.Runtime"), false ]

let private ignoredDirs = set [ "bin"; "obj"; "fable_modules"; "node_modules" ]

/// Runs `onChange` after every save of a .fs file in the plugin, the bindings or the test inputs, reloading the
/// plugin first when a plugin source changed. Saves within 300 ms run once. It replaces any running watcher;
/// unwatch () stops it. A failed build or compile is printed, and the watcher keeps going.
let watch (onChange: unit -> unit) =
    unwatch ()
    let mutable pluginChanged = false
    let flag = obj ()
    let run () =
        let reload = lock flag (fun () -> let r = pluginChanged in pluginChanged <- false; r)
        try
            if reload then reloadPlugin ()
            onChange ()
        with e -> printfn $"Watch: {e.Message}"
    let timer = new Threading.Timer((fun _ -> run ()), null, Threading.Timeout.Infinite, Threading.Timeout.Infinite)
    let watchers =
        watchedDirs
        |> List.map (fun (dir, isPlugin) ->
            let w = new FileSystemWatcher(dir, "*.fs", IncludeSubdirectories = true)
            let onEvent (e: FileSystemEventArgs) =
                let dirs = Path.GetRelativePath(dir, e.FullPath).Split([| '/'; '\\' |])
                // Editors often save through a temporary file, which shows up as a rename.
                if Path.GetExtension e.FullPath = ".fs" && not (dirs |> Array.exists ignoredDirs.Contains) then
                    if isPlugin then lock flag (fun () -> pluginChanged <- true)
                    timer.Change(300, Threading.Timeout.Infinite) |> ignore
            w.Changed.Add onEvent
            w.Created.Add onEvent
            w.Renamed.Add(fun e -> onEvent e)
            w.EnableRaisingEvents <- true
            w)
    watcher <-
        Some
            { new IDisposable with
                member _.Dispose() =
                    for w in watchers do w.Dispose()
                    timer.Dispose() }
    printfn "Watching the plugin, Partas.Solid and the test inputs. Workbench.unwatch () stops it."

/// Checks the named snapshot cases now and after every save (see watch). Each case's .fs.jsx is rewritten, so an
/// open one follows along, and each case prints whether it matches its .expected.
let watchCases (names: string list) =
    let cs = names |> List.map findCase
    let once () =
        let compiled = compileProjectFiles casesProject (Some(cs |> List.map _.Source))
        for c in cs do
            compiled.Js
            |> Map.tryFind c.Source
            |> Option.iter (fun js -> File.WriteAllText(Path.ChangeExtension(c.Source, ".fs.jsx"), js))
            let r = resultOf c compiled
            if r.Passed then printfn $"  ok    %s{c.Name}"
            else printfn $"  DIFFS %s{c.Name}: %d{r.Diff.Length} lines differ, %d{r.Errors.Length} errors"
            r.Errors |> List.iter (printfn "        %s")
    once ()
    watch once

/// Emits the suite now and after every save (see watch). With vitest watching it too, from
/// Partas.Solid.Tests.Runtime/ (`npx vitest --dir Dom`), the specs rerun on every save.
let watchSuite (suite: Suite) =
    let once () = emit suite |> Array.iter (printfn "  %s")
    once ()
    watch once

/// Emits just these files now and after every save (see watch and emitFiles): the fastest loop for the one
/// fixture or case a plugin fix is about. Takes sources or their .fs.jsx, from any project.
let watchFiles (paths: string list) =
    let once () = emitFiles paths |> Array.iter (printfn "  %s")
    once ()
    watch once

// ---------------------------------------------------------------- self-check

/// The session sees the right Fable and the plugin, and a known case compiles to its snapshot.
let selfCheck () =
    let fcs = typeof<InteractiveChecker>.Assembly.GetName().Name
    printfn $"Fable's FCS:  %s{fcs} (must be Fable.FSharp.Compiler.Service)"
    printfn $"Plugin:       %s{plugin.GetName().Name}, %s{string (Runtime.Loader.AssemblyLoadContext.GetLoadContext plugin)}"
    let r = check "SolidCases.MergeProps"
    printfn $"""MergeProps:   %s{if r.Passed then "matches its snapshot" else "DIFFERS"}"""
    fcs = "Fable.FSharp.Compiler.Service" && r.Passed
