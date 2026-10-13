/// Reads the `JSX` namespace of `@solidjs/web` through the pinned TypeScript 7 compiler: every interface, its
/// `extends` chain, its own members with their value types, and the tag maps. Ported from Ranvier's
/// `tools/Ranvier.Dom.Generate`, which drives the same Wire API over `lib.dom.d.ts`.
module Partas.Solid.JsxGen.Surface

open System.IO
open System.Text.Json
open Xantham.TypeScript.Wire
open Xantham.TypeScript.Wire.Proto

let fail message = invalidOp message

let require condition message =
    if not condition then
        fail message

let compilerVersion = "7.1.0-dev.20260902.1"
let compilerRevision = "43a90f4c105bc9db7cb7aa299beddafbabe1d23e"

/// What a member's value type admits, after dropping `undefined`/`null` and upstream's `RemoveAttribute` marker
/// (a lone `false`).
type ValueType =
    {
        /// String literals of the union, verbatim. `""` included.
        Literals: string array
        /// An open `string` (or template literal) widens the literals.
        String: bool
        Number: bool
        /// Both `true` and `false`.
        Boolean: bool
        /// A callback, object or anything else without an attribute spelling.
        Object: bool
    }

type Member =
    {
        Name: string
        Optional: bool
        Readonly: bool
        Method: bool
        Value: ValueType
        Docs: string
    }

type Interface =
    {
        Name: string
        Bases: string array
        Members: Member array
        Docs: string
    }

type Tag =
    {
        Name: string
        /// The `JSX` interfaces the tag's type intersects, e.g. `AnchorHTMLAttributes`.
        Interfaces: string array
        /// `html`, `svg`, `mathml` or `deprecated`: the tag map it came from.
        Group: string
    }

type Surface =
    {
        Interfaces: Interface array
        TypeAliases: string array
        Tags: Tag array
    }

let private hasFlag (flag: SymbolFlags) (symbol: SymbolResponse) = symbol.Flags &&& flag = flag

let private str (name: string) (value: JsonElement) = value.GetProperty(name).GetString()

let read (root: string) : Surface =
    use package =
        JsonDocument.Parse(File.ReadAllBytes(Path.Combine(root, "node_modules", "typescript", "package.json")))

    require
        (str "version" package.RootElement = compilerVersion
         && str "gitHead" package.RootElement = compilerRevision)
        "Installed compiler version/revision mismatch; run npm ci in tools/JsxGen"

    let exe =
        Tsc.locate root
        |> Option.defaultWith (fun () -> fail "Pinned TypeScript native compiler not found; run npm ci in tools/JsxGen")

    let jsxFile =
        Path.Combine(root, "node_modules", "@solidjs", "web", "types", "jsx.d.ts")

    require (File.Exists jsxFile) "@solidjs/web is not installed; run npm ci in tools/JsxGen"

    use channel = new TscChannel(exe, root)
    Api.initialize channel |> ignore

    let program =
        Api.createProgram
            channel
            { CreateProgramParams.Default with
                RootFiles = ValueSome [| DocumentIdentifier.FileName jsxFile |]
            }

    let project = program.Project.Value.Id
    let snapshot = program.Snapshot

    let exportsOf (symbol: SymbolResponse) =
        Api.getExportsOfSymbol
            channel
            {
                Snapshot = snapshot
                Project = project
                ObjectId = symbol.Id
            }
        |> ValueOption.defaultValue [||]

    let membersOf (symbol: SymbolResponse) =
        Api.getMembersOfSymbol
            channel
            {
                Snapshot = snapshot
                Project = project
                ObjectId = symbol.Id
            }
        |> ValueOption.defaultValue [||]
        |> Array.filter (fun m -> not (hasFlag SymbolFlags.TypeParameter m))

    let docs (symbol: SymbolResponse) =
        Api.getDocumentationComment
            channel
            {
                Snapshot = snapshot
                Project = project
                Symbol = symbol.Id
            }

    let readonly (symbol: SymbolResponse) =
        Api.isReadonlySymbol
            channel
            {
                Snapshot = snapshot
                Project = project
                Symbol = symbol.Id
            }

    let typeOf (symbol: SymbolResponse) =
        Api.getTypeOfSymbol
            channel
            {
                Snapshot = snapshot
                Project = project
                Symbol = symbol.Id
            }

    let constituents (t: TypeResponse) =
        Api.getTypesOfType
            channel
            {
                Snapshot = snapshot
                Project = project
                ObjectId = t.Id
            }
        |> ValueOption.defaultValue [||]

    let symbolName (t: TypeResponse) =
        Api.getSymbolOfType
            channel
            {
                Snapshot = snapshot
                Project = project
                ObjectId = t.Id
            }
        |> ValueOption.map _.Name

    let has (flag: TypeFlags) (t: TypeResponse) = t.Flags &&& flag = flag

    let rec flatten (t: TypeResponse) : TypeResponse list =
        if has TypeFlags.Union t then
            constituents t |> Array.toList |> List.collect flatten
        else
            [ t ]

    // `"_blank" | (string & {})` is upstream's open string that keeps its literals as editor suggestions.
    let isOpenString (t: TypeResponse) =
        has TypeFlags.String t
        || has TypeFlags.TemplateLiteral t
        || (has TypeFlags.Intersection t
            && constituents t |> Array.exists (fun c -> has TypeFlags.String c))

    let valueType (symbol: SymbolResponse) =
        let parts =
            flatten (typeOf symbol)
            |> List.filter (fun t -> not (has TypeFlags.Undefined t || has TypeFlags.Null t))

        let literals =
            parts
            |> List.filter (has TypeFlags.StringLiteral)
            |> List.map (fun t -> t.Value.GetValue<string>())
            |> List.distinct
            |> List.toArray

        {
            Literals = literals
            String = parts |> List.exists isOpenString
            Number = parts |> List.exists (fun t -> has TypeFlags.Number t || has TypeFlags.NumberLiteral t)
            // `boolean` reaches a union as its two literals; a lone `false` is upstream's `RemoveAttribute`.
            Boolean =
                (parts |> List.filter (has TypeFlags.BooleanLiteral) |> List.length) >= 2
                || parts |> List.exists (has TypeFlags.Boolean)
            Object =
                parts
                |> List.exists (fun t ->
                    not (
                        has TypeFlags.StringLiteral t
                        || isOpenString t
                        || has TypeFlags.Number t
                        || has TypeFlags.NumberLiteral t
                        || has TypeFlags.BooleanLiteral t
                        || has TypeFlags.Boolean t
                    ))
        }

    let baseNames (symbol: SymbolResponse) =
        let declared =
            Api.getDeclaredTypeOfSymbol
                channel
                {
                    Snapshot = snapshot
                    Project = project
                    Symbol = symbol.Id
                }

        Api.getBaseTypes
            channel
            {
                Snapshot = snapshot
                Project = project
                Type = declared.Id
            }
        |> ValueOption.defaultValue [||]
        |> Array.map (symbolName >> ValueOption.defaultValue "<anonymous>")

    let moduleSymbol =
        Api.getSymbolOfSourceFile
            channel
            {
                Snapshot = snapshot
                Project = project
                File = DocumentIdentifier.FileName jsxFile
            }
        |> ValueOption.defaultWith (fun () -> fail "No module symbol for jsx.d.ts")

    let jsx =
        exportsOf moduleSymbol
        |> Array.tryFind (fun symbol -> symbol.Name = "JSX")
        |> Option.defaultWith (fun () -> fail "jsx.d.ts does not export JSX")

    let jsxExports = exportsOf jsx

    let interfaces =
        jsxExports
        |> Array.filter (hasFlag SymbolFlags.Interface)
        |> Array.sortBy _.Name
        |> Array.map (fun symbol ->
            {
                Name = symbol.Name
                Bases = baseNames symbol
                Docs = docs symbol
                Members =
                    membersOf symbol
                    |> Array.map (fun memberSymbol ->
                        {
                            Name = memberSymbol.Name
                            Optional = hasFlag SymbolFlags.Optional memberSymbol
                            Readonly = readonly memberSymbol
                            Method = hasFlag SymbolFlags.Method memberSymbol
                            Value = valueType memberSymbol
                            Docs = docs memberSymbol
                        })
            })

    let tagMap group name =
        let map =
            jsxExports
            |> Array.tryFind (fun symbol -> symbol.Name = name)
            |> Option.defaultWith (fun () -> fail $"JSX.{name} is missing")

        membersOf map
        |> Array.map (fun tag ->
            let parts =
                let t = typeOf tag

                if has TypeFlags.Intersection t then
                    constituents t |> Array.toList
                else
                    [ t ]

            {
                Name = tag.Name
                Group = group
                Interfaces =
                    parts
                    |> List.choose (symbolName >> ValueOption.toOption)
                    |> List.filter (fun n -> n.EndsWith "Attributes")
                    |> List.toArray
            })

    {
        Interfaces = interfaces
        TypeAliases =
            jsxExports
            |> Array.filter (hasFlag SymbolFlags.TypeAlias)
            |> Array.map _.Name
            |> Array.sort
        Tags =
            Array.concat
                [
                    tagMap "html" "HTMLElementTags"
                    tagMap "deprecated" "HTMLElementDeprecatedTags"
                    tagMap "svg" "SVGElementTags"
                    tagMap "mathml" "MathMLElementTags"
                ]
    }
