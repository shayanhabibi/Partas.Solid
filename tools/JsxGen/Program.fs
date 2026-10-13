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
        let src = Path.GetFullPath(Path.Combine(root, "..", "..", "src", "Partas.Solid"))
        let target = Path.Combine(src, "HtmlAttributes.fs")
        let fsproj = File.ReadAllText(Path.Combine(src, "Partas.Solid.fsproj"))

        let packages =
            Environment.GetEnvironmentVariable "NUGET_PACKAGES"
            |> Option.ofObj
            |> Option.defaultValue (Path.Combine(Environment.GetFolderPath Environment.SpecialFolder.UserProfile, ".nuget", "packages"))

        // A package Partas.Solid references, at the version it pins.
        let referenced (id: string) (dll: string) =
            let version = Regex.Match(fsproj, $"\"{id}\" Version=\"([^\"]+)\"").Groups[1].Value
            Directory.GetFiles(Path.Combine(packages, id.ToLowerInvariant(), version), dll, SearchOption.AllDirectories) |> Array.head

        let domTypes = ProfilesEmit.domTypes (referenced "Xantham.Fable.Core.TS" "Xantham.Fable.Core.TS.dll")
        let ariaFile = Path.Combine(src, "AriaAttributes.fs")
        let emission = Emit.plan overrides surface (Emit.existingAria ariaFile) domTypes

        let markers =
            TagsEmit.missingMarkers overrides surface (File.ReadAllText(Path.Combine(src, "Builder.fs")))


        // Report drift against the file being replaced before replacing it.
        write (Path.Combine(output, "diff.md")) (Emit.report emission target)
        write target (Emit.render overrides (TagsEmit.renderMarkers markers) emission)

        let tagsFile = Path.Combine(src, "Tags.fs")
        write tagsFile (TagsEmit.renderTags overrides surface domTypes tagsFile)
        write ariaFile (ProfilesEmit.renderAria emission)
        let svgFile = Path.Combine(src, "Svg.fs")
        write svgFile (ProfilesEmit.renderSvg surface emission (File.ReadAllText svgFile))

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
