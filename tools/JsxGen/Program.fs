/// `dotnet run --project tools/JsxGen [-- <report-dir>]`: reads Solid's JSX declarations, regenerates
/// `src/Partas.Solid/HtmlAttributes.fs`, and writes the surface and a drift report to the report directory.
module Partas.Solid.JsxGen.Program

open System
open System.IO
open System.Text
open System.Text.Json

let private write (path: string) (text: string) =
    Directory.CreateDirectory(Path.GetDirectoryName path) |> ignore
    File.WriteAllText(path, text, UTF8Encoding(false))

[<EntryPoint>]
let main args =
    try
        let root = __SOURCE_DIRECTORY__

        let output =
            if args.Length > 0 then args[0] else Path.Combine(root, "out")

        let surface = Surface.read root

        write
            (Path.Combine(output, "jsx-surface.json"))
            (JsonSerializer.Serialize(surface, JsonSerializerOptions(WriteIndented = true)) + "\n")

        let overrides = Emit.loadOverrides (Path.Combine(root, "overrides.json"))
        let emission = Emit.plan overrides surface

        let target =
            Path.GetFullPath(Path.Combine(root, "..", "..", "src", "Partas.Solid", "HtmlAttributes.fs"))

        // Report drift against the file being replaced before replacing it.
        write (Path.Combine(output, "diff.md")) (Emit.report emission target)
        write target (Emit.render overrides emission)

        printfn
            "JSX: %d interfaces, %d tags; generated %d attribute members, %d enums, %d omitted -> %s"
            surface.Interfaces.Length
            surface.Tags.Length
            emission.Members.Length
            emission.Enums.Length
            emission.Omitted.Length
            output

        0
    with error ->
        eprintfn "%s" (string error)
        1
