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
        /// Tags left out of `Tags.fs`, e.g. Electron's `webview`.
        SkipTags: Set<string>
        /// `<PartasInterface>.<legacyName>` -> upstream name, kept as `[<Obsolete>]` members.
        Aliases: Map<string, string>
        /// camelCase member names for ARIA attributes the existing file does not have yet.
        AriaNames: Map<string, string>
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
        SkipTags = set "skipTags"
        Aliases = map "aliases"
        AriaNames = map "ariaNames"
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

/// Which file a member goes to, and which conventions it follows.
type Profile =
    /// `HtmlAttributes.fs`: HTML and MathML attributes and events on the marker interfaces.
    | Html
    /// `AriaAttributes.fs`: `module Aria`, extending `HtmlTag`. The plugin turns `ariaLabel` into `aria-label`.
    | Aria
    /// `Svg.fs`: attributes shared by several interfaces go through one provider interface each.
    | Svg

/// The interfaces whose members land on a given Partas marker interface. Partas folds upstream's
/// `ElementAttributes`, `CustomAttributes` and `EventHandlersElement` into `HTMLAttributes`, which every tag inherits
/// through `HtmlTag`.
let htmlTargets (overrides: Overrides) (surface: Surface) =
    let skip (name: string) = overrides.SkipInterfaces.Contains name

    let isHtml (name: string) =
        (name.EndsWith "HTMLAttributes"
         || name.EndsWith "HtmlAttributes"
         || (name.StartsWith "HTML" && name.EndsWith "ElementAttributes")
         || (name.StartsWith "MathML" && name.EndsWith "Attributes"))
        && name <> "HTMLAttributes"
        && not (skip name)

    let isSvg (name: string) =
        name.Contains "SVG" && name.EndsWith "Attributes" && not (skip name)

    let byName = surface.Interfaces |> Array.map (fun i -> i.Name, i) |> Map.ofArray

    [
        yield
            "HTMLAttributes",
            Html,
            [ byName["CustomAttributes"]; byName["ElementAttributes"]; byName["HTMLAttributes"]; byName["EventHandlersElement"] ]
        yield "AmbiguousNamespaceAttributes", Html, [ byName["AmbiguousNamespaceAttributes"] ]
        yield "EventHandlersWindow", Html, [ byName["EventHandlersWindow"] ]
        yield "HtmlTag", Aria, [ byName["AriaAttributes"] ]
        for i in surface.Interfaces do
            if isHtml i.Name then
                yield (overrides.RenameInterfaces.TryFind i.Name |> Option.defaultValue i.Name), Html, [ i ]
            elif isSvg i.Name then
                yield i.Name, Svg, [ i ]
    ]

type EnumType =
    {
        Name: string
        Literals: string array
    }

type EmittedMember =
    {
        Profile: Profile
        Target: string
        Upstream: string
        /// The JSX attribute name.
        Name: string
        /// The F# member name, before escaping.
        Identifier: string
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

/// The existing ARIA members, by attribute name: the camelCase identifier and the value type. The names cannot be
/// derived (`aria-activedescendant` is `ariaActiveDescendant`), and the numeric ones keep their int/float split.
let existingAria (path: string) =
    if not (File.Exists path) then
        Map.empty
    else
        Regex.Matches(File.ReadAllText(path).Replace("\r\n", "\n"), @"member (?:_|this)\.(aria\w+|role)\s*\n\s*with set \(_: ([^)]*)\)")
        |> Seq.map (fun m ->
            let identifier = m.Groups[1].Value

            let attribute =
                if identifier = "role" then "role" else "aria-" + identifier.Substring(4).ToLowerInvariant()

            attribute, (identifier, m.Groups[2].Value.Trim()))
        |> Map.ofSeq

let plan (overrides: Overrides) (surface: Surface) (aria: Map<string, string * string>) (domTypes: Set<string>) : Emission =
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

    // Every tag reaches HTMLAttributes through HtmlTag. A same-named member on another interface (`dir` on
    // `<bdo>`, `id` on SVG) would make the property ambiguous, so the global one stands.
    let globalNames =
        targets
        |> List.filter (fun (target, _, _) -> target = "HTMLAttributes")
        |> List.collect (fun (_, _, sources) -> sources |> List.collect (fun s -> s.Members |> Array.toList))
        |> List.map _.Name
        |> Set.ofList

    let candidates =
        [
            for target, profile, sources in targets do
                for source in sources do
                    for m in source.Members do
                        let key = $"{source.Name}.{m.Name}"

                        if profile <> Aria && target <> "HTMLAttributes" && globalNames.Contains m.Name then
                            omitted.Add(key, "redeclares a global HTMLAttributes member")
                        elif overrides.SkipMembers.Contains key then
                            omitted.Add(key, "skipped by overrides.json")
                        elif m.Method then
                            omitted.Add(key, "method")
                        elif m.EventType.IsSome then
                            yield target, profile, source, m
                        elif
                            m.Value.Object
                            && not (m.Value.String || m.Value.Number || m.Value.Boolean || m.Value.Literals.Length > 0)
                            && not (overrides.Types.ContainsKey key)
                        then
                            omitted.Add(key, "callback or object value: hand-written")
                        else
                            yield target, profile, source, m
        ]

    // SVG shares one member per attribute (through a provider), so its value type is the union over every interface.
    let svgMerged =
        candidates
        |> List.filter (fun (_, profile, _, _) -> profile = Svg)
        |> List.groupBy (fun (_, _, _, m) -> m.Name)
        |> List.map (fun (name, uses) ->
            let values = uses |> List.map (fun (_, _, _, m) -> m.Value)

            name,
            {
                Literals = values |> List.collect (fun v -> List.ofArray v.Literals) |> List.distinct |> List.toArray
                String = values |> List.exists _.String
                Number = values |> List.exists _.Number
                Boolean = values |> List.exists _.Boolean
                Object = values |> List.exists _.Object
            })
        |> Map.ofList

    let valueOf profile (m: Member) =
        match profile with
        | Svg -> svgMerged[m.Name]
        // ARIA's "true"/"false" states are booleans to Partas: the plugin writes a `bool` as those strings.
        | Aria when m.Value.Literals |> Array.exists (fun l -> l = "true" || l = "false") ->
            { m.Value with Boolean = true }
        | _ -> m.Value

    // A boolean whose only spellings are ""/"true"/"false" is just `bool`. When other values sit beside them
    // (contenteditable's "plaintext-only", aria-checked's "mixed") the explicit cases stay: an explicit "false" is
    // not the same as leaving the attribute out.
    let enumLiterals (v: ValueType) =
        let booleanSpelling (l: string) = l = "" || l = "true" || l = "false"

        if v.Boolean && Array.forall booleanSpelling v.Literals then [||]
        else v.Literals |> Array.filter (fun l -> not (v.Boolean && l = ""))

    let isEnum (v: ValueType) =
        not v.String && not v.Number && (enumLiterals v).Length > 0

    let setKey (v: ValueType) = enumLiterals v |> Array.sort |> String.concat "|"

    // Enum names: HTML uses the attribute's name, prefixed with the interface when two interfaces give it different
    // sets; ARIA and SVG take an `Aria`/`Svg` prefix, so they cannot collide with HTML's.
    let enumNames =
        candidates
        |> List.filter (fun (_, profile, source, m) ->
            isEnum (valueOf profile m) && not (overrides.Types.ContainsKey $"{source.Name}.{m.Name}"))
        |> List.groupBy (fun (_, profile, _, m) -> profile, m.Name)
        |> List.collect (fun ((profile, name), uses) ->
            let _, _, _, first = uses.Head

            match profile with
            | Aria -> [ (profile, name, setKey (valueOf profile first)), "Aria" + pascal (name.Replace("aria-", "")) ]
            | Svg -> [ (profile, name, setKey svgMerged[name]), "Svg" + pascal name ]
            | Html ->
                let sets = uses |> List.groupBy (fun (_, _, _, m) -> setKey m.Value)

                match sets with
                | [ (key, _) ] -> [ (profile, name, key), pascal name ]
                | _ ->
                    sets
                    |> List.map (fun (key, setUses) ->
                        let _, _, source, _ = setUses.Head
                        (profile, name, key), stem source.Name + pascal name))
        |> Map.ofList

    let enumFor profile (m: Member) =
        enumNames[(profile, m.Name, setKey (valueOf profile m))]

    let identifierFor profile (m: Member) =
        match profile with
        | Aria ->
            match aria.TryFind m.Name, overrides.AriaNames.TryFind m.Name with
            | Some(identifier, _), _ -> identifier
            | None, Some identifier -> identifier
            | None, None when m.Name = "role" -> "role"
            | None, None -> failwith $"New ARIA attribute {m.Name}: name its camelCase member in overrides.json ariaNames"
        | _ -> m.Name

    let members =
        candidates
        |> List.map (fun (target, profile, source, m) ->
            let key = $"{source.Name}.{m.Name}"
            let v = valueOf profile m

            let typ =
                match overrides.Types.TryFind key, m.EventType with
                | Some t, _ -> t
                | None, Some event when domTypes.Contains event -> $"Dom.{event} -> unit"
                | None, Some _ -> "Dom.Event -> unit"
                | None, None when isEnum v && v.Boolean -> $"U2<bool, Attr.{enumFor profile m}>"
                | None, None when isEnum v -> $"Attr.{enumFor profile m}"
                | None, None when v.Number && not v.Object && v.Literals.Length = 0 ->
                    match profile with
                    // `number | string` (width, span, tabindex) stays `int`; SVG coordinates are fractional.
                    | Html -> "int"
                    | Svg -> if v.String then "U2<float, string>" else "float"
                    | Aria -> aria.TryFind m.Name |> Option.map snd |> Option.defaultValue "U2<int, string>"
                // `ClassValue` also admits arrays and records, so its object arm keeps it a string.
                | None, None when v.String -> "string"
                | None, None when v.Number -> (if profile = Svg then "float" else "int")
                | None, None when v.Boolean -> "bool"
                | None, None -> "string"

            let injection =
                match profile with
                | _ when m.Name = "innerHTML" -> Some "InjectedLanguage.HTML"
                | Html when typ = "string" ->
                    let tag = if target = "HTMLAttributes" then "div" else tagFor source.Name
                    Some $"\"jsx\", Prefix = \"<{tag} {m.Name}='\", Suffix = \"' >\""
                | Aria when typ = "string" -> Some $"InjectedLanguage.HTML, Prefix = \"<div {m.Name}='\", Suffix = \"'>\""
                | _ -> None

            {
                Profile = profile
                Target = target
                Upstream = source.Name
                Name = m.Name
                Identifier = identifierFor profile m
                Type = typ
                Injection = injection
                Docs = m.Docs
            })

    {
        Enums =
            enumNames
            |> Map.toList
            |> List.map (fun ((_, _, key), typeName) ->
                {
                    Name = typeName
                    Literals = if key = "" then [||] else key.Split '|'
                })
            |> List.distinctBy _.Name
            |> List.sortBy _.Name
        Members = members
        Omitted = List.ofSeq omitted
    }

let render (overrides: Overrides) (markers: string) (emission: Emission) =
    let out = StringBuilder()
    let line (text: string) = out.Append(text).Append('\n') |> ignore

    line "// <auto-generated> tools/JsxGen from @solidjs/web's JSX declarations. Regenerate; do not edit. </auto-generated>"
    line "namespace Partas.Solid"
    line ""
    line "open JetBrains.Annotations"
    line "open Fable.Core"
    line "open Fable.Core.TS"
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

    if markers <> "" then
        line "    // Marker interfaces Builder.fs does not declare: MathML, and HTML elements added since."
        out.Append(markers) |> ignore

    let html = emission.Members |> List.filter (fun m -> m.Profile = Html)

    for target, members in html |> List.groupBy _.Target do
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
        html |> List.map (fun m -> (m.Target, m.Name), m) |> Map.ofList

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
        emission.Members
        |> List.filter (fun m -> m.Profile = Html)
        |> List.distinctBy (fun m -> m.Target, m.Name)

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
