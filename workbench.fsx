// Sets up the plugin/bindings workbench: a SageFs session that compiles Partas.Solid projects with Fable in-process.
// See Workbench/README.md.
//
//   dotnet fsi workbench.fsx
//
// 1. Clones Fable.SageFs (pinned below) into .workbench/Fable.SageFs.
// 2. Points its versions.json at the Fable version in .config/dotnet-tools.json.
// 3. Runs its setup, which renames Fable's FCS fork so it loads next to SageFs's own FCS (vendor/).
// 4. Builds Workbench/Partas.Solid.Workbench.fsproj.
//
// Idempotent. It writes the gitignored .workbench/, and step 4 the usual (gitignored) bin/ and obj/.

open System
open System.Diagnostics
open System.IO
open System.Text.Json.Nodes

let fableSageFsUrl = "https://github.com/shayanhabibi/Fable.SageFs.git"
let fableSageFsCommit = "53ed66275a267d35bee2044a30550bbb5b2ffd6c"

let repoRoot = __SOURCE_DIRECTORY__
let workDir = Path.Join(repoRoot, ".workbench")
let fableSageFs = Path.Join(workDir, "Fable.SageFs")
let workbenchProject = Path.Join(repoRoot, "Workbench", "Partas.Solid.Workbench.fsproj")

let step (s: string) = printfn $"\n==> {s}"
let fail (s: string) =
    eprintfn $"FAIL {s}"
    exit 1

/// Runs a process, echoing its output. Returns the exit code.
let run (dir: string) (file: string) (args: string list) =
    let psi = ProcessStartInfo(file, args, WorkingDirectory = dir)
    use p = Process.Start psi
    p.WaitForExit()
    p.ExitCode

/// Runs git and returns its trimmed output, or fails.
let git (dir: string) (args: string list) =
    let psi = ProcessStartInfo("git", args, WorkingDirectory = dir, RedirectStandardOutput = true)
    use p = Process.Start psi
    let out = p.StandardOutput.ReadToEnd()
    p.WaitForExit()
    if p.ExitCode <> 0 then fail $"""git {String.concat " " args} failed in {dir}"""
    out.Trim()

// ---------------------------------------------------------------- the Fable version

let fableVersion =
    let tools = JsonNode.Parse(File.ReadAllText(Path.Join(repoRoot, ".config", "dotnet-tools.json")))
    match tools["tools"] |> Option.ofObj |> Option.bind (fun t -> Option.ofObj t["fable"]) with
    | None -> fail ".config/dotnet-tools.json has no fable tool."
    | Some fable -> string fable["version"]

step $"Fable {fableVersion} (.config/dotnet-tools.json)"

// ---------------------------------------------------------------- Fable.SageFs

step $"Fable.SageFs at {fableSageFsCommit.Substring(0, 7)} in .workbench/Fable.SageFs"
Directory.CreateDirectory workDir |> ignore
if not (Directory.Exists(Path.Join(fableSageFs, ".git"))) then
    if run workDir "git" [ "clone"; "--quiet"; fableSageFsUrl; "Fable.SageFs" ] <> 0 then
        fail $"git clone {fableSageFsUrl} failed."
if git fableSageFs [ "rev-parse"; "HEAD" ] <> fableSageFsCommit then
    // versions.json is the one file this script changes; everything else setup writes is ignored there.
    git fableSageFs [ "checkout"; "--quiet"; "--"; "versions.json" ] |> ignore
    if git fableSageFs [ "status"; "--porcelain" ] <> "" then
        fail ".workbench/Fable.SageFs has local changes. Commit or discard them, or delete the folder."
    git fableSageFs [ "fetch"; "--quiet"; "origin" ] |> ignore
    git fableSageFs [ "-c"; "advice.detachedHead=false"; "checkout"; "--quiet"; fableSageFsCommit ] |> ignore
printfn "    OK"

step "versions.json: fableCompiler"
let versionsFile = Path.Join(fableSageFs, "versions.json")
let versions = JsonNode.Parse(File.ReadAllText versionsFile)
if string versions["fableCompiler"] = fableVersion then
    printfn $"    OK   already {fableVersion}"
else
    printfn $"""    {versions["fableCompiler"]} -> {fableVersion}"""
    versions["fableCompiler"] <- JsonValue.Create fableVersion
    File.WriteAllText(versionsFile, versions.ToJsonString())

step "Fable.SageFs setup (vendor/)"
// Setup ends by building its own sample session and self-check against the Fable version it was written for;
// with another Fable version those steps can fail after vendor/ is written. The workbench needs only vendor/.
let props = Path.Join(fableSageFs, "vendor", "Fable.SageFs.props")
// Setup leaves an unchanged props file alone, so remove it first: only this run's setup can then pass the check below.
File.Delete props
let setupExit = run fableSageFs "dotnet" [ "fsi"; "setup.fsx" ]
if not (File.Exists props && File.ReadAllText(props).Contains $"<FableCompilerVersion>{fableVersion}</FableCompilerVersion>") then
    fail $"Fable.SageFs setup did not produce vendor/ for Fable {fableVersion}. See its output above."
if setupExit <> 0 then
    printfn $"    Setup stopped after vendor/ (exit {setupExit}); its sample session is not used here."
printfn $"    OK   vendor/Fable.SageFs.props for Fable {fableVersion}"

// ---------------------------------------------------------------- the workbench

step "Build Workbench/Partas.Solid.Workbench.fsproj"
if run repoRoot "dotnet" [ "build"; workbenchProject; "--nologo"; "-v:q"; "-clp:ErrorsOnly" ] <> 0 then
    fail "The workbench does not build. See the errors above."
printfn "    OK"

printfn
    """
Ready. Open a SageFs session on the workbench (project Workbench/Partas.Solid.Workbench.fsproj, working
directory Workbench/), then:

    Workbench.selfCheck ();;
    Workbench.checkAll ();;           // every snapshot case against its .expected
    Workbench.reloadPlugin ();;       // after editing Partas.Solid.FablePlugin

See Workbench/README.md."""
