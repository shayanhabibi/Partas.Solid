namespace Nacara.Plugins.Internal

open System
open System.Diagnostics
open System.Runtime.InteropServices
open System.Text
open System.Threading

/// <summary>Where Fable's watch output stands, read from its lines.</summary>
[<RequireQualifiedAccess>]
module SolidFableCycle =

    let isStarted (line: string) = line.StartsWith "Started Fable compilation"
    let isWatching (line: string) = line.StartsWith "Watching "

    /// <summary>Whether Fable was compiling after <c>count</c> lines.</summary>
    let busyAt (lines: string list) (count: int) =
        lines
        |> List.truncate count
        |> List.filter (fun line -> isStarted line || isWatching line)
        |> List.tryLast
        |> Option.exists isStarted

    /// <summary>The lines of the last compilation that covers a change, once it has finished.</summary>
    /// <param name="lines">Everything Fable has printed.</param>
    /// <param name="mark">How many lines there were when the change was written.</param>
    /// <returns>
    /// The lines from the last <c>Started</c> to the <c>Watching</c> that closed it, or nothing while a
    /// compilation that could have seen the change is still to finish.
    /// </returns>
    /// <remarks>
    /// A change written while Fable was compiling may be folded into that compilation or queue another,
    /// so either one finishing counts. Written while Fable was idle, it has to start one of its own.
    /// </remarks>
    let finished (lines: string list) (mark: int) =
        let after = lines |> List.skip (min mark lines.Length) |> Array.ofList
        let lastWatching = after |> Array.tryFindIndexBack isWatching
        let startedAfter = after |> Array.exists isStarted

        match lastWatching with
        | Some last when busyAt lines mark || startedAfter ->
            // A Started after the last Watching is a compilation still running.
            if after[last + 1 ..] |> Array.exists isStarted then
                None
            else
                let first =
                    after[.. last] |> Array.tryFindIndexBack isStarted |> Option.defaultValue 0

                Some(List.ofArray after[first .. last])
        | _ -> None

[<Struct; StructLayout(LayoutKind.Sequential)>]
type internal BasicLimits =
    val mutable PerProcessUserTimeLimit: int64
    val mutable PerJobUserTimeLimit: int64
    val mutable LimitFlags: uint32
    val mutable MinimumWorkingSetSize: unativeint
    val mutable MaximumWorkingSetSize: unativeint
    val mutable ActiveProcessLimit: uint32
    val mutable Affinity: unativeint
    val mutable PriorityClass: uint32
    val mutable SchedulingClass: uint32

[<Struct; StructLayout(LayoutKind.Sequential)>]
type internal IoCounters =
    val mutable ReadOperationCount: uint64
    val mutable WriteOperationCount: uint64
    val mutable OtherOperationCount: uint64
    val mutable ReadTransferCount: uint64
    val mutable WriteTransferCount: uint64
    val mutable OtherTransferCount: uint64

[<Struct; StructLayout(LayoutKind.Sequential)>]
type internal ExtendedLimits =
    val mutable Basic: BasicLimits
    val mutable Io: IoCounters
    val mutable ProcessMemoryLimit: unativeint
    val mutable JobMemoryLimit: unativeint
    val mutable PeakProcessMemoryUsed: unativeint
    val mutable PeakJobMemoryUsed: unativeint

/// <summary>Ties child processes to this one's lifetime on Windows.</summary>
/// <remarks>
/// A process exit handler does not run when the site's process is killed, by an IDE's stop button
/// for one, and Windows does not end a process's children with it. A job object that kills what it
/// holds when its last handle closes does: this process holds the only handle, and the system closes
/// it however the process ends.
/// </remarks>
module internal KillOnClose =

    [<DllImport("kernel32.dll", CharSet = CharSet.Unicode)>]
    extern nativeint CreateJobObject(nativeint attributes, string name)

    [<DllImport("kernel32.dll")>]
    extern bool SetInformationJobObject(nativeint job, int infoClass, nativeint info, uint32 length)

    [<DllImport("kernel32.dll")>]
    extern bool AssignProcessToJobObject(nativeint job, nativeint proc)

    let private extendedLimitInformation = 9
    let private killOnJobClose = 0x2000u

    let private job =
        lazy
            if not (OperatingSystem.IsWindows()) then
                None
            else
                let handle = CreateJobObject(0n, null)

                if handle = 0n then
                    None
                else
                    let mutable limits = ExtendedLimits()
                    limits.Basic.LimitFlags <- killOnJobClose
                    let size = Marshal.SizeOf<ExtendedLimits>()
                    let buffer = Marshal.AllocHGlobal size

                    try
                        Marshal.StructureToPtr(limits, buffer, false)

                        if SetInformationJobObject(handle, extendedLimitInformation, buffer, uint32 size) then
                            Some handle
                        else
                            None
                    finally
                        Marshal.FreeHGlobal buffer

    /// <summary>End a process, and what it starts afterwards, when this one ends.</summary>
    /// <returns>Whether it could be tied: always false off Windows.</returns>
    let tie (proc: Process) =
        match job.Value with
        | Some handle -> AssignProcessToJobObject(handle, proc.Handle)
        | None -> false

/// <summary>A <c>dotnet fable watch</c> kept running over a workspace, for <c>nacara watch</c>.</summary>
type SolidFableWatcher(directory: string, environment: (string * string) list, arguments: string list) =
    let lines = ResizeArray<string>()
    let gate = obj ()
    let changed = new AutoResetEvent(false)

    let proc =
        let info = ProcessStartInfo("dotnet", "fable" :: "watch" :: arguments)
        info.WorkingDirectory <- directory
        info.RedirectStandardOutput <- true
        info.RedirectStandardError <- true
        info.UseShellExecute <- false
        info.StandardOutputEncoding <- Encoding.UTF8
        info.StandardErrorEncoding <- Encoding.UTF8
        info.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] <- "1"
        info.Environment["NO_COLOR"] <- "1"

        for name, value in environment do
            info.Environment[name] <- value

        new Process(StartInfo = info)

    let collect (args: DataReceivedEventArgs) =
        if not (isNull args.Data) then
            lock gate (fun () -> lines.Add args.Data)
            changed.Set() |> ignore

    let snapshot () = lock gate (fun () -> List.ofSeq lines)

    let stop () =
        try
            if not proc.HasExited then
                proc.Kill true
        with _ ->
            ()

    // The watcher must not outlive the site's process, however that ends.
    let onExit = EventHandler(fun _ _ -> stop ())

    do
        proc.OutputDataReceived.Add collect
        proc.ErrorDataReceived.Add collect
        proc.Exited.Add(fun _ -> changed.Set() |> ignore)
        proc.EnableRaisingEvents <- true
        proc.Start() |> ignore
        // Tied before `dotnet` starts Fable itself, so Fable joins the job too.
        KillOnClose.tie proc |> ignore
        proc.BeginOutputReadLine()
        proc.BeginErrorReadLine()
        AppDomain.CurrentDomain.ProcessExit.AddHandler onExit

    member _.HasExited = proc.HasExited

    /// <summary>How many lines Fable has printed: take it before writing a change.</summary>
    member _.Mark() = lock gate (fun () -> lines.Count)

    /// <summary>Wait for the compilation that covers a change written after <c>mark</c>.</summary>
    /// <param name="mark">What <c>Mark</c> returned before the change was written.</param>
    /// <param name="settle">How long Fable may stay quiet before a finished compilation counts.</param>
    /// <param name="timeout">How long to wait in all.</param>
    /// <returns>That compilation's output, or an error when Fable exited or took too long.</returns>
    member _.Wait(mark: int, settle: TimeSpan, timeout: TimeSpan) : Result<string, string> =
        let deadline = Stopwatch.StartNew()

        let rec loop () =
            match SolidFableCycle.finished (snapshot ()) mark with
            | Some cycle ->
                // Several files written at once can reach Fable as more than one change.
                if changed.WaitOne settle then
                    loop ()
                else
                    Ok(String.concat "\n" cycle)
            | None when proc.HasExited ->
                let all = snapshot ()
                let output = all |> List.skip (min mark all.Length) |> String.concat "\n"
                Error $"Fable's watch exited:\n%s{output}"
            | None when deadline.Elapsed > timeout -> Error $"Fable did not finish within %O{timeout}"
            | None ->
                changed.WaitOne(TimeSpan.FromMilliseconds 250.) |> ignore
                loop ()

        loop ()

    interface IDisposable with
        member _.Dispose() =
            AppDomain.CurrentDomain.ProcessExit.RemoveHandler onExit
            stop ()
            proc.Dispose()
            changed.Dispose()
