/// Turns the extracted surface into Partas.Solid's attribute layout: `[<Erase>]` extension members on the marker
/// interfaces of `Builder.fs`, with closed literal sets as `[<StringEnum>]` unions in `module Attr`.
module Partas.Solid.JsxGen.Emit

open System
open System.IO
open System.Text
open System.Text.Json
open System.Text.RegularExpressions
open Partas.Solid.JsxGen.Surface

type Overrides =
    {
        RenameInterfaces: Map<string, string>
        Types: Map<string, string>
        SkipMembers: Set<string>
        SkipInterfaces: Set<string>
        /// `<PartasInterface>.<legacyName>` -> upstream name, kept as `[<Obsolete>]` members.
        Aliases: Map<string, string>
    }

let loadOverrides (path: string) =
    use document = JsonDocument.Parse(File.ReadAllBytes path)
    let root = document.RootElement

    let map (name: string) =
        root.GetProperty(name).EnumerateObject()
        |> Seq.map (fun p -> p.Name, p.Value.GetString())
        |> Map.ofSeq

    let set (name: string) =
        root.GetProperty(name).EnumerateArray() |> Seq.map _.GetString() |> Set.ofSeq

    {
        RenameInterfaces = map "renameInterfaces"
        Types = map "types"
        SkipMembers = set "skipMembers"
        SkipInterfaces = set "skipInterfaces"
        Aliases = map "aliases"
    }

/// F# keywords and reserved identifiers. A member named after one takes a trailing apostrophe, which the plugin trims.
let private keywords =
    set
        [
            "abstract"; "and"; "as"; "assert"; "base"; "begin"; "class"; "default"; "delegate"; "do"; "done"
            "downcast"; "downto"; "elif"; "else"; "end"; "exception"; "extern"; "false"; "finally"; "fixed"; "for"
            "fun"; "function"; "global"; "if"; "in"; "inherit"; "inline"; "interface"; "internal"; "lazy"; "let"
            "match"; "member"; "module"; "mutable"; "namespace"; "new"; "not"; "null"; "of"; "open"; "or"
            "override"; "private"; "public"; "rec"; "return"; "select"; "sig"; "static"; "struct"; "then"; "to"
            "true"; "try"; "type"; "upcast"; "use"; "val"; "void"; "when"; "while"; "with"; "yield"; "const"
            "break"; "checked"; "component"; "constraint"; "continue"; "event"; "external"; "include"; "mixin"
            "parallel"; "process"; "protected"; "pure"; "sealed"; "tailcall"; "trait"; "virtual"
        ]

let memberIdentifier (name: string) =
    if keywords.Contains name then name + "'"
    elif Regex.IsMatch(name, "^[A-Za-z_][A-Za-z0-9_]*$") then name
    else $"``{name}``"

let private pascal (text: string) =
    Regex.Split(text, "[^A-Za-z0-9]+")
    |> Array.filter ((<>) "")
    |> Array.map (fun part -> string (Char.ToUpperInvariant part[0]) + part.Substring 1)
    |> String.concat ""

/// Case names for a literal set. `""` is `Empty`; a leading digit takes `N`; `a`/`A` collisions split by case.
let caseNames (literals: string array) =
    let baseName (literal: string) =
        match pascal literal with
        | "" -> "Empty"
        | name when Char.IsDigit name[0] -> "N" + name
        | name -> name

    let names = literals |> Array.map baseName

    literals
    |> Array.mapi (fun i literal ->
        let name = names[i]

        if (names |> Array.filter ((=) name) |> Array.length) > 1 then
            if literal.Length > 0 && Char.IsLower literal[0] then "Lower" + name
            elif literal.Length > 0 && Char.IsUpper literal[0] then "Upper" + name
            else $"{name}{i}"
        else
            name)

let private stem (interfaceName: string) =
    interfaceName
        .Replace("HTMLAttributes", "")
        .Replace("HtmlAttributes", "")
        .Replace("ElementAttributes", "")
        .Replace("Attributes", "")
        .Replace("HTML", "")

let private xml (text: string) =
    text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;")

/// The interfaces whose members land on a given Partas marker interface. Partas folds upstream's
/// `ElementAttributes` and `CustomAttributes` into `HTMLAttributes`, which every tag inherits through `HtmlTag`.
let htmlTargets (overrides: Overrides) (surface: Surface) =
    let isHtml (name: string) =
        (name.EndsWith "HTMLAttributes" || name.EndsWith "HtmlAttributes" || (name.StartsWith "HTML" && name.EndsWith "ElementAttributes"))
        && name <> "HTMLAttributes"
        && not (overrides.SkipInterfaces.Contains name)

    let byName = surface.Interfaces |> Array.map (fun i -> i.Name, i) |> Map.ofArray

    [
        yield "HTMLAttributes", [ byName["CustomAttributes"]; byName["ElementAttributes"]; byName["HTMLAttributes"] ]
        yield "AmbiguousNamespaceAttributes", [ byName["AmbiguousNamespaceAttributes"] ]
        for i in surface.Interfaces do
            if isHtml i.Name then
                yield (overrides.RenameInterfaces.TryFind i.Name |> Option.defaultValue i.Name), [ i ]
    ]

type EnumType =
    {
        Name: string
        Literals: string array
    }

type EmittedMember =
    {
        Target: string
        Upstream: string
        Name: string
        Type: string
        Injection: string option
        Docs: string
    }

type Emission =
    {
        Enums: EnumType list
        Members: EmittedMember list
        /// Members left for hand-written bindings, with the reason.
        Omitted: (string * string) list
    }

let plan (overrides: Overrides) (surface: Surface) : Emission =
    let targets = htmlTargets overrides surface
    let omitted = ResizeArray()

    // A tag that owns each interface, for the injection prefix; global attributes use `div`.
    let tagFor =
        let owners =
            surface.Tags
            |> Array.filter (fun t -> t.Group = "html" || t.Group = "deprecated")
            |> Array.collect (fun t -> t.Interfaces |> Array.map (fun i -> i, t.Name))
            |> Array.groupBy fst
            |> Array.map (fun (i, tags) -> i, (tags |> Array.map snd |> Array.sort |> Array.head))
            |> Map.ofArray

        fun (upstream: string) -> owners.TryFind upstream |> Option.defaultValue "div"

    let candidates =
        [
            for target, sources in targets do
                for source in sources do
                    for m in source.Members do
                        let key = $"{source.Name}.{m.Name}"

                        if overrides.SkipMembers.Contains key then
                            omitted.Add(key, "skipped by overrides.json")
                        elif m.Method || m.Name.StartsWith "on" then
                            omitted.Add(key, "event or method: hand-written")
                        elif
                            m.Value.Object
                            && not (m.Value.String || m.Value.Number || m.Value.Boolean || m.Value.Literals.Length > 0)
                            && not (overrides.Types.ContainsKey key)
                        then
                            omitted.Add(key, "callback or object value: hand-written")
                        else
                            yield target, source, m
        ]

    // Enum names: the attribute's own name, unless two interfaces give the same attribute different literal sets.
    let enumLiterals (m: Member) =
        m.Value.Literals |> Array.filter (fun l -> not (m.Value.Boolean && l = ""))

    let isEnum (m: Member) =
        not m.Value.String && not m.Value.Number && (enumLiterals m).Length > 0

    let enumNames =
        candidates
        |> List.filter (fun (_, source, m) -> isEnum m && not (overrides.Types.ContainsKey $"{source.Name}.{m.Name}"))
        |> List.groupBy (fun (_, _, m) -> m.Name)
        |> List.collect (fun (name, uses) ->
            let sets =
                uses
                |> List.groupBy (fun (_, _, m) -> enumLiterals m |> Array.sort |> String.concat "|")

            match sets with
            | [ (key, _) ] -> [ key, name, pascal name ]
            | _ ->
                sets
                |> List.map (fun (key, setUses) ->
                    let _, source, _ = setUses.Head
                    key, name, stem source.Name + pascal name))

    let enumFor (m: Member) =
        let key = enumLiterals m |> Array.sort |> String.concat "|"

        enumNames
        |> List.find (fun (k, name, _) -> k = key && name = m.Name)
        |> fun (_, _, typeName) -> typeName

    let members =
        candidates
        |> List.map (fun (target, source, m) ->
            let key = $"{source.Name}.{m.Name}"
            let v = m.Value

            let typ =
                match overrides.Types.TryFind key with
                | Some t -> t
                | None when isEnum m && v.Boolean -> $"U2<bool, Attr.{enumFor m}>"
                | None when isEnum m -> $"Attr.{enumFor m}"
                // `number | string` (width, span, tabindex) stays `int` as before; `ClassValue` also admits
                // arrays and records, so its object arm keeps it a string.
                | None when v.Number && not v.Object && v.Literals.Length = 0 -> "int"
                | None when v.String -> "string"
                | None when v.Number -> "int"
                | None when v.Boolean -> "bool"
                | None -> "string"

            let injection =
                if m.Name = "innerHTML" then Some "InjectedLanguage.HTML"
                elif typ = "string" then
                    let tag = if target = "HTMLAttributes" then "div" else tagFor source.Name
                    Some $"\"jsx\", Prefix = \"<{tag} {m.Name}='\", Suffix = \"' >\""
                else None

            {
                Target = target
                Upstream = source.Name
                Name = m.Name
                Type = typ
                Injection = injection
                Docs = m.Docs
            })

    {
        Enums =
            enumNames
            |> List.map (fun (key, _, typeName) ->
                {
                    Name = typeName
                    Literals = if key = "" then [||] else key.Split '|'
                })
            |> List.distinctBy _.Name
            |> List.sortBy _.Name
        Members = members
        Omitted = List.ofSeq omitted
    }

let render (overrides: Overrides) (emission: Emission) =
    let out = StringBuilder()
    let line (text: string) = out.Append(text).Append('\n') |> ignore

    line "// <auto-generated> tools/JsxGen from @solidjs/web's JSX declarations. Regenerate; do not edit. </auto-generated>"
    line "namespace Partas.Solid"
    line ""
    line "open JetBrains.Annotations"
    line "open Fable.Core"
    line "open Partas.Solid.Experimental.U"
    line ""
    line "/// Closed value sets of JSX attributes, as string enums: `dir = Attr.Dir.Rtl`."
    line "module Attr ="

    for e in emission.Enums do
        line "    [<StringEnum; RequireQualifiedAccess>]"
        line $"    type {e.Name} ="

        for literal, case in Array.zip e.Literals (caseNames e.Literals) do
            line $"        | [<CompiledName \"{literal}\">] {case}"

        line ""

    line "[<AutoOpen>]"
    line "module HtmlAttributes ="

    for target, members in emission.Members |> List.groupBy _.Target do
        line $"    type {target} with"

        for m in members |> List.distinctBy _.Name do
            if m.Docs <> "" then
                line "        /// <summary>"

                for docLine in m.Docs.Replace("\r", "").Split('\n') do
                    line ($"        /// {xml docLine}".TrimEnd())

                line "        /// </summary>"

            match m.Injection with
            | Some args -> line $"        [<Erase; LanguageInjection({args})>]"
            | None -> line "        [<Erase>]"

            line $"        member _.{memberIdentifier m.Name}"
            line $"            with set (_: {m.Type}) = ()"
            line $"            and [<Erase>] get (): {m.Type} = unbox ()"
            line ""

    // Legacy camelCase spellings. HTML attribute names are case-insensitive, so these still render correctly.
    line "/// Earlier spellings of attributes that upstream now names in lowercase."
    line "[<AutoOpen>]"
    line "module HtmlAttributeAliases ="

    let generated =
        emission.Members |> List.map (fun m -> (m.Target, m.Name), m) |> Map.ofList

    for target, aliases in
        overrides.Aliases
        |> Map.toList
        |> List.map (fun (key, upstream) ->
            let i = key.IndexOf '.'
            key.Substring(0, i), key.Substring(i + 1), upstream)
        |> List.groupBy (fun (t, _, _) -> t) do
        line $"    type {target} with"

        for _, legacy, upstream in aliases do
            match generated.TryFind(target, upstream) with
            | Some m ->
                line $"        [<Erase; System.Obsolete(\"Use '{memberIdentifier upstream}'.\")>]"
                line $"        member _.{memberIdentifier legacy}"
                line $"            with set (_: {m.Type}) = ()"
                line $"            and [<Erase>] get (): {m.Type} = unbox ()"
                line ""
            | None -> failwith $"Alias {target}.{legacy} names {upstream}, which is not generated"

    out.ToString()

/// Members of the hand-written file, per `type X with` block: name -> setter type.
let private parseHandWritten (path: string) =
    // The aliases module repeats members under legacy names; drift is about the canonical ones.
    let text = (File.ReadAllText path).Split("module HtmlAttributeAliases")[0]
    let blocks = Regex.Split(text, @"\n    type (\w+(?:<'?\w+>)?) with")
    // Split yields [preamble; name; body; name; body; ...].
    [
        for i in 1..2 .. blocks.Length - 2 do
            let name = blocks[i]

            for m in Regex.Matches(blocks[i + 1], @"member _\.(``[^`]+``|[\w']+)\s*\n\s*with set \((?:value|_): ([^)]*)\)") do
                let memberName = m.Groups[1].Value.Trim('`').TrimEnd('\'')
                yield name, memberName, m.Groups[2].Value.Trim()
    ]

let report (emission: Emission) (handWritten: string) =
    let ours = parseHandWritten handWritten
    let oursByKey = ours |> List.map (fun (t, n, ty) -> (t, n.ToLowerInvariant()), (n, ty)) |> Map.ofList

    let generated =
        emission.Members |> List.distinctBy (fun m -> m.Target, m.Name)

    let generatedKeys =
        generated |> List.map (fun m -> m.Target, m.Name.ToLowerInvariant()) |> Set.ofList

    let added =
        generated |> List.filter (fun m -> not (oursByKey.ContainsKey(m.Target, m.Name.ToLowerInvariant())))

    let changed =
        generated
        |> List.choose (fun m ->
            match oursByKey.TryFind(m.Target, m.Name.ToLowerInvariant()) with
            | Some(ourName, ourType) when ourType <> m.Type || ourName <> m.Name -> Some(m, ourName, ourType)
            | _ -> None)

    let removed =
        ours
        |> List.filter (fun (t, n, _) -> not (n.StartsWith "on") && not (generatedKeys.Contains(t, n.ToLowerInvariant())))

    let out = StringBuilder()
    let line (text: string) = out.Append(text).Append('\n') |> ignore
    line "# JSX attribute diff: generated vs src/Partas.Solid/HtmlAttributes.fs"
    line ""
    line $"Generated {generated.Length} members on {generated |> List.map _.Target |> List.distinct |> List.length} interfaces, {emission.Enums.Length} enums. Events are compared separately."
    line ""
    line $"## Added ({added.Length}): upstream has them, we don't"
    line ""

    for m in added do
        line $"- `{m.Target}.{m.Name}`: `{m.Type}`"

    line ""
    line $"## Changed ({changed.Length}): name casing or type differs"
    line ""

    for m, ourName, ourType in changed do
        line $"- `{m.Target}.{ourName}`: `{ourType}` -> `{m.Name}`: `{m.Type}`"

    line ""
    line $"## Ours only ({removed.Length}): not in upstream rc.14 under this interface"
    line ""

    for t, n, ty in removed do
        line $"- `{t}.{n}`: `{ty}`"

    line ""
    line $"## Omitted from generation ({emission.Omitted.Length})"
    line ""

    for key, reason in emission.Omitted do
        line $"- `{key}`: {reason}"

    out.ToString()
