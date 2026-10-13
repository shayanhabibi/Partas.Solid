/// Writes the tag types of `Tags.fs` (after its `// <generated>` line) and the marker interfaces they need, from
/// `@solidjs/web`'s tag maps. SVG stays in the hand-written `Svg.fs`.
module Partas.Solid.JsxGen.TagsEmit

open System.IO
open System.Text
open System.Text.RegularExpressions
open Partas.Solid.JsxGen.Surface
open Partas.Solid.JsxGen.Emit

/// HTML void elements (HTML Living Standard, "Void elements"), plus the deprecated `param` and `keygen`. The
/// declarations do not say which elements are void.
let voidElements =
    set [ "area"; "base"; "br"; "col"; "embed"; "hr"; "img"; "input"; "link"; "meta"; "source"; "track"; "wbr"; "param"; "keygen" ]

/// Interfaces every tag reaches through `HtmlTag`, or that are not attribute contracts.
let private ambient =
    set [ "HTMLAttributes"; "ElementAttributes"; "CustomAttributes"; "EventHandlersElement"; "AriaAttributes"; "<anonymous>"; "__type" ]

let generatedTags (overrides: Overrides) (surface: Surface) =
    surface.Tags
    |> Array.filter (fun t -> t.Group <> "svg" && not (overrides.SkipTags.Contains t.Name))

/// A tag's attribute interfaces: its own and everything they extend, short of the ambient ones.
let tagInterfaces (overrides: Overrides) (surface: Surface) (tag: Tag) =
    let byName = surface.Interfaces |> Array.map (fun i -> i.Name, i) |> Map.ofArray

    let rec closure (name: string) =
        if ambient.Contains name then
            []
        else
            name
            :: (byName.TryFind name
                |> Option.map (fun i -> i.Bases |> Array.toList |> List.collect closure)
                |> Option.defaultValue [])

    tag.Interfaces
    |> Array.toList
    |> List.collect closure
    |> List.distinct
    |> List.map (fun n -> overrides.RenameInterfaces.TryFind n |> Option.defaultValue n)

/// Marker interfaces the tags need that `Builder.fs` does not declare, with their non-ambient bases.
let missingMarkers (overrides: Overrides) (surface: Surface) (builder: string) =
    let declared =
        Regex.Matches(builder, @"type (\w+) =\s*(?:interface end|\r?\n\s*inherit)")
        |> Seq.map (fun m -> m.Groups[1].Value)
        |> Set.ofSeq

    let byName = surface.Interfaces |> Array.map (fun i -> i.Name, i) |> Map.ofArray
    let renamedBack = overrides.RenameInterfaces |> Map.toSeq |> Seq.map (fun (a, b) -> b, a) |> Map.ofSeq

    generatedTags overrides surface
    |> Array.toList
    |> List.collect (tagInterfaces overrides surface)
    |> List.distinct
    |> List.filter (fun n -> not (declared.Contains n))
    |> List.sort
    |> List.map (fun n ->
        let upstream = renamedBack.TryFind n |> Option.defaultValue n

        let bases =
            byName.TryFind upstream
            |> Option.map (fun i ->
                i.Bases
                |> Array.filter (fun b -> not (ambient.Contains b))
                |> Array.map (fun b -> overrides.RenameInterfaces.TryFind b |> Option.defaultValue b)
                |> Array.toList)
            |> Option.defaultValue []

        n, bases)

let renderMarkers (markers: (string * string list) list) =
    let out = StringBuilder()
    let line (text: string) = out.Append(text).Append('\n') |> ignore

    // Bases first: a marker can only inherit one declared above it.
    let byName = Map.ofList markers

    let ordered =
        let rec visit (seen: string list) (name: string) =
            if List.contains name seen || not (byName.ContainsKey name) then
                seen
            else
                name :: (byName[name] |> List.fold visit seen)

        markers |> List.map fst |> List.fold visit [] |> List.rev

    for name in ordered do
        let bases = byName[name]
        line "    [<EB(EBState.Never); AllowNullLiteral; Interface>]"

        match bases with
        | [] -> line $"    type {name} = interface end"
        | _ ->
            line $"    type {name} ="

            for b in bases do
                line $"        inherit {b}"

        line ""

    out.ToString()

/// The new text of `Tags.fs`. Tags it already declares keep their identifier, so user code keeps compiling. Element
/// types (`asDomElement`) are the ones `Fable.Core.TS.Dom` declares: an existing tag keeps its type if it is one,
/// otherwise every tag takes upstream's, falling back to `HTMLElement`.
let renderTags (overrides: Overrides) (surface: Surface) (domTypes: Set<string>) (tagsFile: string) =
    let text = File.ReadAllText(tagsFile).Replace("\r\n", "\n")
    let marker = "    // <generated>"
    let cut = text.IndexOf marker

    if cut < 0 then
        failwith $"{tagsFile} has no '{marker}' line"

    let head = text.Substring(0, text.IndexOf('\n', cut) + 1)

    let existing =
        Regex.Matches(text.Substring cut, @"    type ([\w'`-]+)\(\) =\n((?:        .*\n?)+)")
        |> Seq.map (fun m ->
            let identifier = m.Groups[1].Value
            let dom = Regex.Match(m.Groups[2].Value, @"asDomElement: (\w+)")
            identifier.Trim('`').TrimEnd('\''), (identifier, (if dom.Success then Some dom.Groups[1].Value else None)))
        |> Map.ofSeq

    let out = StringBuilder(head)
    let mutable indent = ""

    let line (text: string) =
        out.Append(if text = "" then "" else indent + text).Append('\n') |> ignore

    let mutable inMathML = false

    for tag in generatedTags overrides surface do
        // MathML names (`math`, `mi`, `mo`) are short and common, so they sit behind `MathML.` as SVG's sit
        // behind `Svg.`. The tag maps list MathML last, so the nested module runs to the end of the file.
        if tag.Group = "mathml" && not inMathML then
            inMathML <- true
            line ""
            line "    /// <summary>MathML elements: <c>MathML.math () { MathML.mi () { \"x\" } }</c>.</summary>"
            line "    module MathML ="
            indent <- "    "

        let upstreamDom =
            if tag.Group = "mathml" then None
            elif domTypes.Contains tag.DomType then Some tag.DomType
            else Some "HTMLElement"

        let identifier, domType =
            match existing.TryFind tag.Name with
            // A generic HTMLElement gives way to upstream's specific type (HTMLSlotElement); a ref typed HTMLElement still fits.
            | Some(identifier, Some dom) when tag.Group <> "mathml" && domTypes.Contains dom && dom <> "HTMLElement" ->
                identifier, Some dom
            | Some(identifier, _) when tag.Group <> "mathml" -> identifier, upstreamDom
            | _ -> memberIdentifier tag.Name, upstreamDom

        line ""

        if tag.Group = "deprecated" then
            line $"    [<Erase; System.Obsolete(\"<{tag.Name}> is deprecated in HTML.\")>]"
        else
            line "    [<Erase>]"

        line $"    type {identifier}() ="

        // MathML elements are not HTMLElements, so like SVG they skip the typed `asDomElement` ref path.
        if tag.Group = "mathml" then line "        interface RegularNode"
        elif voidElements.Contains tag.Name then line "        interface IntrinsicVoidNode"
        else line "        interface IntrinsicParentNode"

        for i in tagInterfaces overrides surface tag do
            line $"        interface {i}"

        match domType with
        | Some dom -> line $"        member inline this.asDomElement: {dom} = unbox this"
        | None -> ()

    out.ToString()
