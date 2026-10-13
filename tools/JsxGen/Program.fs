/// `dotnet run --project tools/JsxGen [-- <report-dir>]`: reads Solid's JSX declarations, regenerates
/// `src/Partas.Solid/HtmlAttributes.fs`, and writes the surface and a drift report to the report directory.
module Partas.Solid.JsxGen.Program

open System
open System.IO
open System.Text
open System.Text.Json
open System.Text.RegularExpressions

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

        let src = Path.GetFullPath(Path.Combine(root, "..", "..", "src", "Partas.Solid"))
        let target = Path.Combine(src, "HtmlAttributes.fs")

        let markers =
            TagsEmit.missingMarkers overrides surface (File.ReadAllText(Path.Combine(src, "Builder.fs")))

        // Fable.Browser.Dom at the version Partas.Solid references: the element types tags can name.
        let browser =
            let fsproj = File.ReadAllText(Path.Combine(src, "Partas.Solid.fsproj"))
            let version = Regex.Match(fsproj, "\"Fable.Browser.Dom\" Version=\"([^\"]+)\"").Groups[1].Value

            let packages =
                Environment.GetEnvironmentVariable "NUGET_PACKAGES"
                |> Option.ofObj
                |> Option.defaultValue (Path.Combine(Environment.GetFolderPath Environment.SpecialFolder.UserProfile, ".nuget", "packages"))

            Directory.GetFiles(Path.Combine(packages, "fable.browser.dom", version), "Browser.Dom.dll", SearchOption.AllDirectories)
            |> Array.head
            |> TagsEmit.browserTypes

        // Report drift against the file being replaced before replacing it.
        write (Path.Combine(output, "diff.md")) (Emit.report emission target)
        write target (Emit.render overrides (TagsEmit.renderMarkers markers) emission)

        let tagsFile = Path.Combine(src, "Tags.fs")
        write tagsFile (TagsEmit.renderTags overrides surface browser tagsFile)

        printfn
            "JSX: %d interfaces, %d tags (%d generated, %d new markers); generated %d attribute members, %d enums, %d omitted -> %s"
            surface.Interfaces.Length
            surface.Tags.Length
            (TagsEmit.generatedTags overrides surface).Length
            markers.Length
            emission.Members.Length
            emission.Enums.Length
            emission.Omitted.Length
            output

        0
    with error ->
        eprintfn "%s" (string error)
        1
