module Partas.Nacara.Plugins.Solid.Tests.Solid

open Expecto
open Nacara.Plugins
open Nacara.Plugins.Internal

let private fence (info: string) (code: string) = $"```fsharp %s{info}\n%s{code}\n```"

let private scan (body: string) = SolidScan.scan "solid" false "pkey" body

let private cell (body: string) = List.exactlyOne (scan body).Cells

let private unitOf (cells: SolidCell list) =
    let code, spans = SolidGenerate.fsharp [ "open Partas.Solid" ] "pkey" cells

    {
        Key = "pkey"
        Source = Some "page.md"
        BodyLine = 1
        Cells = cells
        Code = code
        Spans = spans
    }

/// <summary>A temporary directory, deleted on dispose.</summary>
type private TempDir() =
    let root = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName())

    /// <summary>Writes <c>text</c> at <c>relative</c> under the directory and returns the full path.</summary>
    member _.Write (relative: string) (text: string) =
        let path = System.IO.Path.Combine(root, relative)
        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName path) |> ignore
        System.IO.File.WriteAllText(path, text)
        path

    interface System.IDisposable with
        member _.Dispose() =
            if System.IO.Directory.Exists root then
                System.IO.Directory.Delete(root, true)

/// <summary>An F# project compiling <c>files</c> and referencing <c>projects</c>.</summary>
let private projectText (files: string list) (projects: string list) =
    let items tag values =
        values |> List.map (fun value -> $"""<%s{tag} Include="%s{value}" />""") |> String.concat ""

    $"""<Project Sdk="Microsoft.NET.Sdk"><ItemGroup>%s{items "Compile" files}</ItemGroup><ItemGroup>%s{items "ProjectReference" projects}</ItemGroup></Project>"""

/// <summary>
/// A transform's output: a module that indents the author's second line by four columns, with the author's
/// lines mapped, and a pinned header.
/// </summary>
let private mapped =
    {
        Code = "module Map_c1 =\n    let x = 1\n    let y = oops"
        Render = "Render.map ()"
        Spans =
            [
                {
                    Generated = 1
                    Length = 1
                    Body = 0
                    Indent = System.Int32.MaxValue
                }
                {
                    Generated = 2
                    Length = 2
                    Body = 1
                    Indent = 4
                }
            ]
    }

let private scanMap transform (body: string) =
    SolidScan.scanWith [ "map", transform ] "solid" false "pkey" body

/// <summary>The generated line that holds <c>text</c>, counting from one.</summary>
let private lineOf (text: string) (code: string) =
    code.Split('\n') |> Array.findIndex (fun line -> line.Contains text) |> (+) 1

[<Tests>]
let tests =
    testList
        "solid examples"
        [
            testList
                "scan"
                [
                    test "an expression mounts" {
                        let found = cell (fence "solid" "Counter ()")
                        Expect.equal found.Kind SolidCellKind.Expression "kind"
                        Expect.isTrue found.Mounts "mounts"
                    }

                    test "declarations mount nothing without render=" {
                        let found = cell (fence "solid" "let x = 1")
                        Expect.equal found.Kind (SolidCellKind.Declarations None) "kind"
                        Expect.isFalse found.Mounts "mounts"
                    }

                    test "bindings that lead to an expression are an expression" {
                        let found = cell (fence "solid" "let x = 1\ndiv () { string x }")
                        Expect.equal found.Kind SolidCellKind.Expression "kind"
                    }

                    test "a closing bracket at column zero does not decide" {
                        let found = cell (fence "solid" "let xs = [\n    1\n]")
                        Expect.equal found.Kind (SolidCellKind.Declarations None) "kind"
                    }

                    test "render= names the component to mount" {
                        let found = cell (fence "solid render=Counter" "let Counter () = div () { }")
                        Expect.equal found.Kind (SolidCellKind.Declarations(Some "Counter")) "kind"
                        Expect.isTrue found.Mounts "mounts"
                    }

                    test "a plain fsharp fence is left alone" {
                        let body = fence "" "let x = 1"
                        let result = scan body
                        Expect.isEmpty result.Cells "cells"
                        Expect.equal result.Body body "body"
                    }

                    test "the fence keeps its code and gains a placeholder" {
                        let result = scan (fence "solid id=hello" "Counter ()")
                        Expect.stringContains result.Body "```fsharp\nCounter ()\n```" "code"
                        Expect.stringContains result.Body "data-partas-page=\"pkey\" data-partas-cell=\"hello\"" "placeholder"
                        Expect.isFalse (result.Body.Contains "```fsharp solid") "token removed"
                    }

                    test "setup leaves blank lines, so later lines keep their place" {
                        let body = fence "solid setup" "type T = { A: int }" + "\nafter"
                        let result = scan body
                        Expect.equal (result.Body.Split('\n').Length) (body.Split('\n').Length) "line count"
                        Expect.isFalse (result.Body.Contains "type T") "hidden"
                    }

                    test "show=output hides the code but mounts" {
                        let result = scan (fence "solid show=output" "p () { \"hi\" }")
                        Expect.isFalse (result.Body.Contains "```") "no fence"
                        Expect.stringContains result.Body "data-partas-cell=\"c1\"" "placeholder"
                    }

                    test "show=code compiles but does not mount" {
                        Expect.isFalse (cell (fence "solid show=code" "Counter ()")).Mounts "mounts"
                    }

                    test "cells are numbered and start on the line after the fence" {
                        let result = scan (fence "solid" "A ()" + "\n\n" + fence "solid" "B ()")
                        Expect.equal (result.Cells |> List.map _.Id) [ "c1"; "c2" ] "ids"
                        Expect.equal (result.Cells |> List.map _.Line) [ 2; 6 ] "lines"
                    }

                    test "jsx puts the code and its JSX in tabs, above the placeholder" {
                        let result = scan (fence "solid jsx" "Counter ()")
                        Expect.isTrue (List.exactlyOne result.Cells).Jsx "marked"

                        let tabs =
                            String.concat
                                "\n"
                                [
                                    "::::tabs"
                                    ":::tab F#"
                                    "```fsharp"
                                    "Counter ()"
                                    "```"
                                    ":::"
                                    ":::tab JSX"
                                    ""
                                    "<div data-partas-jsx-page=\"pkey\" data-partas-jsx-cell=\"c1\"></div>"
                                    ""
                                    ":::"
                                    "::::"
                                ]

                        Expect.stringContains result.Body tabs "tabs"
                        let placeholder = result.Body.IndexOf "data-partas-cell=\"c1\""
                        Expect.isGreaterThan placeholder (result.Body.IndexOf "::::tabs") "placeholder after tabs"
                    }

                    test "without jsx there are no tabs" {
                        let result = scan (fence "solid" "Counter ()")
                        Expect.isFalse (List.exactlyOne result.Cells).Jsx "marked"
                        Expect.isFalse (result.Body.Contains "::::tabs") "tabs"
                    }

                    test "showJsx marks every fence but setup" {
                        let body = fence "solid" "A ()" + "\n" + fence "solid setup" "let x = 1"
                        let result = SolidScan.scan "solid" true "pkey" body
                        Expect.equal (result.Cells |> List.map _.Jsx) [ true; false ] "marked"
                    }

                    test "show=code still gets its tabs" {
                        let result = scan (fence "solid show=code jsx" "Counter ()")
                        Expect.stringContains result.Body "```\n:::\n:::tab JSX" "tabs"
                    }

                    test "show=output with jsx has only the JSX tab" {
                        let result = scan (fence "solid show=output jsx" "Counter ()")
                        Expect.isFalse (result.Body.Contains ":::tab F#") "no F# tab"
                        Expect.stringContains result.Body "::::tabs\n:::tab JSX" "JSX tab"
                    }

                    test "a bad show= and a repeated id are problems" {
                        let result = scan (fence "solid show=nope id=a" "A ()" + "\n" + fence "solid id=a" "B ()")
                        Expect.equal (result.Problems |> List.map fst) [ 1; 4 ] "lines"
                    }
                ]

            testList
                "generate"
                [
                    test "an expression is wrapped in a component, indented" {
                        let unit = unitOf (scan (fence "solid" "Counter ()")).Cells
                        Expect.stringContains unit.Code "let Cell_c1 () =\n    Counter ()" "wrapper"
                    }

                    test "render= adds a wrapper that calls the component" {
                        let unit = unitOf (scan (fence "solid render=Counter" "let Counter () = 1")).Cells
                        Expect.stringContains unit.Code "let Cell_c1 () = Counter ()" "wrapper"
                    }

                    test "a line of a cell maps back to its body line and column" {
                        let unit = unitOf (scan ("text\n" + fence "solid" "div () {\n    Counter ()\n}")).Cells
                        let line = lineOf "    Counter ()" unit.Code
                        // Four columns of wrapper indent come off; the body line is the fence's third.
                        Expect.equal (SolidGenerate.locate unit.Spans line 9) (Some(4, 5)) "located"
                    }

                    test "a wrapper line is blamed on the fence line, column one" {
                        let unit = unitOf (scan (fence "solid" "Counter ()")).Cells
                        let line = lineOf "let Cell_c1" unit.Code
                        Expect.equal (SolidGenerate.locate unit.Spans line 5) (Some(1, 1)) "located"
                    }

                    test "prelude lines map nowhere" {
                        let unit = unitOf (scan (fence "solid" "Counter ()")).Cells
                        Expect.isNone (SolidGenerate.locate unit.Spans (lineOf "open Partas.Solid" unit.Code) 1) "prelude"
                    }

                    test "the entry mounts only the cells that mount" {
                        let cells = (scan (fence "solid" "A ()" + "\n" + fence "solid setup" "let x = 1")).Cells
                        let entry = SolidGenerate.entry "pkey" cells
                        Expect.stringContains entry "\"c1\": m.Cell_c1," "c1"
                        Expect.isFalse (entry.Contains "Cell_c2") "c2"
                        Expect.stringContains entry "../Pkey.fs.jsx" "module"
                    }
                ]

            testList
                "jsx"
                [
                    let jsx =
                        String.concat
                            "\n"
                            [
                                "import { createSignal } from \"solid-js\";"
                                ""
                                "export function Counter() {"
                                "    return <button>"
                                "        {count()}"
                                "    </button>;"
                                "}"
                                ""
                                "export class Todo extends Record {"
                                "    constructor(Id) {"
                                "        super();"
                                "    }"
                                "}"
                                ""
                                "export const limit = 3;"
                                ""
                                "export function Cell_c1() {"
                                "    return Counter();"
                                "}"
                                ""
                            ]

                    test "an expression shows its wrapper" {
                        Expect.equal (SolidGenerate.jsxNames (cell (fence "solid" "Counter ()"))) [ "Cell_c1" ] "names"
                    }

                    test "declarations show what they declare at column zero" {
                        let code =
                            "[<SolidComponent>]\nlet Counter () =\n    let inner = 1\n    inner\ntype private Todo = { Id: int }"

                        let names = SolidGenerate.jsxNames (cell (fence "solid render=Counter" code))
                        Expect.equal names [ "Counter"; "Todo" ] "names"
                    }

                    test "a function is cut out up to its closing brace" {
                        Expect.equal
                            (SolidGenerate.jsxDeclaration jsx "Counter")
                            (Some "export function Counter() {\n    return <button>\n        {count()}\n    </button>;\n}")
                            "Counter"
                    }

                    test "a class and a one-line const are cut out" {
                        Expect.stringStarts (Option.get (SolidGenerate.jsxDeclaration jsx "Todo")) "export class Todo" "class"
                        Expect.equal (SolidGenerate.jsxDeclaration jsx "limit") (Some "export const limit = 3;") "const"
                    }

                    test "a name is matched whole" {
                        Expect.isNone (SolidGenerate.jsxDeclaration jsx "Count") "prefix"
                    }

                    test "only the cells marked jsx get their JSX" {
                        let cells = (scan (fence "solid jsx" "Counter ()" + "\n" + fence "solid" "B ()")).Cells
                        let parsed = SolidGenerate.jsxCode cells jsx
                        Expect.equal (parsed |> Map.keys |> List.ofSeq) [ "c1" ] "keys"
                        Expect.stringContains parsed["c1"] "return Counter();" "c1"
                    }

                    test "declarations Fable left no trace of fall back to the wrapper" {
                        let cells = (scan (fence "solid render=Counter jsx" "let private helper = 1")).Cells
                        let parsed = SolidGenerate.jsxCode cells jsx
                        Expect.stringStarts parsed["c1"] "export function Cell_c1" "wrapper"
                    }

                    test "marking a cell jsx changes the fingerprint" {
                        let plain = unitOf (scan (fence "solid" "Counter ()")).Cells
                        let marked = unitOf (scan (fence "solid jsx" "Counter ()")).Cells
                        let options = SolidExamples.defaults ()

                        Expect.notEqual
                            (SolidCompile.fingerprint options [ plain ])
                            (SolidCompile.fingerprint options [ marked ])
                            "fingerprint"
                    }

                    test "Fable compiles numeric arrays to plain arrays" {
                        let flag =
                            SolidCompile.fableArguments
                            |> List.pairwise
                            |> List.contains ("--typedArrays", "false")

                        Expect.isTrue flag "--typedArrays false"
                    }
                ]

            testList
                "packages"
                [
                    let defaults = SolidExamples.defaults ()

                    test "there are no extra packages by default" {
                        Expect.isEmpty defaults.NpmPackages "npm"
                        Expect.isEmpty defaults.NuGetPackages "nuget"
                    }

                    test "npm appends, and a later call for the same name replaces the earlier one" {
                        let options =
                            defaults
                            |> SolidExamples.npm "animejs" "3.2.2"
                            |> SolidExamples.npm "canvas-confetti" "1.9.3"
                            |> SolidExamples.npm "animejs" "4.0.0"

                        Expect.equal
                            options.NpmPackages
                            [ "canvas-confetti", "1.9.3"; "animejs", "4.0.0" ]
                            "packages"
                    }

                    test "nuget appends, and a later call for the same id replaces the earlier one" {
                        let options =
                            defaults
                            |> SolidExamples.nuget "Fable.Browser.Dom" "2.18.0"
                            |> SolidExamples.nuget "Thoth.Json" "10.2.0"
                            |> SolidExamples.nuget "fable.browser.dom" "2.19.0"

                        Expect.equal
                            options.NuGetPackages
                            [ "Thoth.Json", "10.2.0"; "fable.browser.dom", "2.19.0" ]
                            "packages"
                    }

                    test "npm packages are dependencies, and Solid stays overridden" {
                        let json =
                            defaults
                            |> SolidExamples.npm "animejs" "^3.2.2"
                            |> SolidExamples.npm "odd\"name" "1.0.0"
                            |> SolidWorkspace.packageJson

                        use document = System.Text.Json.JsonDocument.Parse json
                        let root = document.RootElement
                        let dependencies = root.GetProperty "dependencies"
                        Expect.equal (dependencies.GetProperty("animejs").GetString()) "^3.2.2" "animejs"
                        Expect.equal (dependencies.GetProperty("odd\"name").GetString()) "1.0.0" "escaped"
                        Expect.equal (dependencies.GetProperty("solid-js").GetString()) defaults.SolidVersion "solid-js"
                        let overrides = root.GetProperty "overrides"
                        Expect.equal (overrides.GetProperty("solid-js").GetString()) defaults.SolidVersion "override"
                        Expect.equal (overrides.GetProperty("@solidjs/web").GetString()) defaults.SolidVersion "web override"
                    }

                    test "an npm package cannot replace one the plugin installs" {
                        let json = defaults |> SolidExamples.npm "solid-js" "1.9.0" |> SolidWorkspace.packageJson
                        use document = System.Text.Json.JsonDocument.Parse json
                        let dependencies = document.RootElement.GetProperty "dependencies"
                        let solid = dependencies.EnumerateObject() |> Seq.filter (fun p -> p.Name = "solid-js") |> List.ofSeq
                        Expect.equal (solid |> List.map _.Value.GetString()) [ defaults.SolidVersion ] "one solid-js"
                    }

                    test "without extra packages package.json is what it was" {
                        let json = SolidWorkspace.packageJson defaults
                        use document = System.Text.Json.JsonDocument.Parse json
                        let names = document.RootElement.GetProperty("dependencies").EnumerateObject() |> Seq.map _.Name |> List.ofSeq
                        Expect.equal names [ "@solidjs/compiler"; "@solidjs/web"; "solid-js"; "rolldown" ] "names"
                    }

                    test "nuget packages are package references, escaped" {
                        let project =
                            defaults
                            |> SolidExamples.nuget "Fable.Browser.Dom" "2.18.0"
                            |> SolidExamples.nuget "Odd&Id" "[1.0,2.0)"
                            |> fun options -> SolidWorkspace.projectFile options [ "Pkey.fs" ]

                        let document = System.Xml.Linq.XDocument.Parse project

                        let references =
                            document.Descendants(System.Xml.Linq.XName.Get "PackageReference")
                            |> Seq.map (fun element -> element.Attribute(System.Xml.Linq.XName.Get "Include").Value, element.Attribute(System.Xml.Linq.XName.Get "Version").Value)
                            |> List.ofSeq

                        Expect.equal
                            references
                            [ "Partas.Solid", defaults.PartasVersion; "Fable.Browser.Dom", "2.18.0"; "Odd&Id", "[1.0,2.0)" ]
                            "references"
                    }

                    test "a nuget entry for Partas.Solid is left to partasVersion" {
                        let project =
                            defaults
                            |> SolidExamples.nuget "partas.solid" "1.0.0"
                            |> fun options -> SolidWorkspace.projectFile options []

                        Expect.isFalse (project.Contains "1.0.0") "ignored"
                    }

                    test "a Partas.Solid project replaces the package" {
                        let project =
                            defaults
                            |> SolidExamples.partasProject "C:/repo/src/Partas.Solid/Partas.Solid.fsproj"
                            |> SolidExamples.project "C:/site/maps/Maps.fsproj"
                            |> fun options -> SolidWorkspace.projectFile options [ "Pkey.fs" ]

                        let document = System.Xml.Linq.XDocument.Parse project
                        let includes (name: string) =
                            document.Descendants(System.Xml.Linq.XName.Get name)
                            |> Seq.map (fun element -> element.Attribute(System.Xml.Linq.XName.Get "Include").Value)
                            |> List.ofSeq

                        Expect.isEmpty (includes "PackageReference") "no package"
                        Expect.equal
                            (includes "ProjectReference")
                            [ "C:/repo/src/Partas.Solid/Partas.Solid.fsproj"; "C:/site/maps/Maps.fsproj" ]
                            "references"
                    }

                    test "an edit to the Partas.Solid project changes the fingerprint" {
                        use dir = new TempDir()
                        let partas = dir.Write "Partas.Solid/Partas.Solid.fsproj" (projectText [ "Tags.fs" ] [])
                        let source = dir.Write "Partas.Solid/Tags.fs" "module Tags\nlet a = 1"
                        let units = [ unitOf (scan (fence "solid" "Counter ()")).Cells ]
                        let options = defaults |> SolidExamples.partasProject partas
                        let before = SolidCompile.fingerprint options units
                        System.IO.File.WriteAllText(source, "module Tags\nlet a = 2")

                        Expect.notEqual (SolidCompile.fingerprint options units) before "changed"
                        Expect.notEqual (SolidCompile.fingerprint defaults units) before "package differs"
                    }

                    test "adding, changing or removing a package changes the fingerprint" {
                        let units = [ unitOf (scan (fence "solid" "Counter ()")).Cells ]
                        let print options = SolidCompile.fingerprint options units
                        let withNpm = defaults |> SolidExamples.npm "animejs" "3.2.2"
                        let withNuget = defaults |> SolidExamples.nuget "Thoth.Json" "10.2.0"

                        let prints =
                            [
                                print defaults
                                print withNpm
                                print (withNpm |> SolidExamples.npm "animejs" "4.0.0")
                                print withNuget
                                print (withNuget |> SolidExamples.nuget "Thoth.Json" "10.3.0")
                            ]

                        Expect.equal (List.distinct prints) prints "all differ"
                        Expect.equal (print { withNpm with NpmPackages = [] }) (print defaults) "removed"
                    }

                    test "projects are project references, and the target framework is set" {
                        let project =
                            defaults
                            |> SolidExamples.project "C:/site/maps/Maps.fsproj"
                            |> SolidExamples.targetFramework "net10.0"
                            |> fun options -> SolidWorkspace.projectFile options [ "Pkey.fs" ]

                        let document = System.Xml.Linq.XDocument.Parse project
                        let named (name: string) = document.Descendants(System.Xml.Linq.XName.Get name) |> List.ofSeq

                        let references =
                            named "ProjectReference" |> List.map (fun element -> element.Attribute(System.Xml.Linq.XName.Get "Include").Value)

                        Expect.equal references [ "C:/site/maps/Maps.fsproj" ] "references"
                        Expect.equal (named "TargetFramework" |> List.map _.Value) [ "net10.0" ] "target framework"
                    }

                    test "the target framework defaults to net9.0 and no project is referenced" {
                        let project = SolidWorkspace.projectFile defaults []
                        Expect.stringContains project "<TargetFramework>net9.0</TargetFramework>" "target framework"
                        Expect.isFalse (project.Contains "ProjectReference") "no references"
                    }

                    test "project inputs cover a referenced project's sources" {
                        use dir = new TempDir()
                        let lib = dir.Write "lib/Lib.fsproj" (projectText [ "Lib.fs" ] [])
                        dir.Write "lib/Lib.fs" "module Lib\nlet answer = 42" |> ignore
                        let app = dir.Write "app/App.fsproj" (projectText [ "App.fs" ] [ "../lib/Lib.fsproj" ])
                        dir.Write "app/App.fs" "module App\nlet run () = Lib.answer" |> ignore

                        let inputs = SolidCompile.projectInputs app

                        Expect.contains inputs "module Lib\nlet answer = 42" "referenced source"
                        Expect.contains inputs "module App\nlet run () = Lib.answer" "own source"
                        Expect.contains inputs (System.IO.File.ReadAllText lib) "referenced project"
                    }

                    test "a referenced source or a property changes the fingerprint" {
                        use dir = new TempDir()
                        dir.Write "lib/Lib.fsproj" (projectText [ "Lib.fs" ] []) |> ignore
                        dir.Write "lib/Lib.fs" "module Lib\nlet answer = 42" |> ignore
                        let app = dir.Write "app/App.fsproj" (projectText [ "App.fs" ] [ "../lib/Lib.fsproj" ])
                        dir.Write "app/App.fs" "module App" |> ignore
                        let units = [ unitOf (scan (fence "solid" "Counter ()")).Cells ]
                        let options = defaults |> SolidExamples.project app
                        let before = SolidCompile.fingerprint options units
                        dir.Write "lib/Lib.fs" "module Lib\nlet answer = 43" |> ignore
                        let edited = SolidCompile.fingerprint options units
                        let traced = SolidCompile.fingerprint (options |> SolidExamples.property "RanvierTrace" "true") units

                        Expect.notEqual edited before "source edit"
                        Expect.notEqual traced edited "property"
                    }
                ]

            testList
                "inline"
                [
                    test "show=inline mounts without the code, in an unboxed div" {
                        let result = scan (fence "solid show=inline" "p () { \"hi\" }")
                        let found = List.exactlyOne result.Cells
                        Expect.equal found.Show SolidShow.Inline "show"
                        Expect.isTrue found.Mounts "mounts"
                        Expect.isFalse (result.Body.Contains "```") "no fence"
                        Expect.stringContains result.Body "<div class=\"partas-solid partas-solid--inline\"" "placeholder"
                    }

                    test "a use in prose becomes a span where it stood" {
                        let result = scan "Press `solid: Kbd \"Ctrl\"` to copy."
                        let found = List.exactlyOne result.Cells
                        Expect.equal found.Kind (SolidCellKind.Use 15) "kind"
                        Expect.equal found.Code "Kbd \"Ctrl\"" "code"
                        Expect.equal found.Id "u1" "id"
                        Expect.isTrue found.Mounts "mounts"

                        Expect.equal
                            result.Body
                            "Press <span class=\"partas-solid partas-solid--inline\" data-partas-page=\"pkey\" data-partas-cell=\"u1\"></span> to copy."
                            "body"
                    }

                    test "several uses on a line are each found" {
                        let found = SolidScan.uses "solid" "`solid: A ()` and ``solid: B `x` ()`` then `solid:C()`"
                        Expect.equal (found |> List.map _.Code) [ "A ()"; "B `x` ()"; "C()" ] "codes"
                        Expect.equal (found |> List.map _.Column) [ 9; 28; 51 ] "columns"
                    }

                    test "plain code spans are left alone" {
                        let body = "Write `solid` or `Kbd \"x\"` or `solidly: x` or \`solid: x`."
                        let result = scan body
                        Expect.isEmpty result.Cells "cells"
                        Expect.equal result.Body body "body"
                    }

                    test "a space before the token shows the syntax as code" {
                        let body = "Write `` `solid: Kbd \"x\"` `` in the prose."
                        let result = scan body
                        Expect.isEmpty result.Cells "cells"
                        Expect.equal result.Body body "body"
                    }

                    test "a use inside a fence is code, not a use" {
                        let body = fence "" "let s = \"`solid: A ()`\""
                        let result = scan body
                        Expect.isEmpty result.Cells "cells"
                        Expect.equal result.Body body "body"
                    }

                    test "a use takes the token the site set" {
                        let result = SolidScan.scan "live" false "pkey" "`live: A ()` but not `solid: B ()`"
                        Expect.equal (result.Cells |> List.map _.Code) [ "A ()" ] "codes"
                    }

                    test "showJsx leaves uses without a panel" {
                        let result = SolidScan.scan "solid" true "pkey" "Press `solid: Kbd \"C\"` now."
                        Expect.isFalse (List.exactlyOne result.Cells).Jsx "jsx"
                        Expect.isFalse (result.Body.Contains "::::tabs") "no tabs"
                    }

                    test "uses do not shift the numbering of fences" {
                        let result = scan ("`solid: A ()`\n\n" + fence "solid" "B ()")
                        Expect.equal (result.Cells |> List.map _.Id) [ "u1"; "c1" ] "ids"
                    }

                    test "the token alone names the syntax and stays as written" {
                        let result = scan "x `solid:` or `solid: ` y"
                        Expect.isEmpty result.Cells "cells"
                        Expect.isEmpty result.Problems "problems"
                        Expect.equal result.Body "x `solid:` or `solid: ` y" "body"
                    }

                    test "a use is wrapped after every fence, so it can come first" {
                        let cells = (scan ("`solid: Badge ()`\n\n" + fence "solid" "let Badge () = span () { }")).Cells
                        let unit = unitOf cells
                        Expect.isLessThan (lineOf "let Badge" unit.Code) (lineOf "let Cell_u1" unit.Code) "order"
                        Expect.stringContains unit.Code "let Cell_u1 () =\n    Badge ()" "wrapper"
                        Expect.stringContains (SolidGenerate.entry "pkey" cells) "\"u1\": m.Cell_u1," "entry"
                    }

                    test "an error in a use points at its line and column" {
                        let unit = unitOf (scan "text\nSee `solid: Badge ()` here.").Cells
                        let line = lineOf "    Badge ()" unit.Code
                        // Badge starts at column 5 of the generated line and column 13 of the prose.
                        Expect.equal (SolidGenerate.locate unit.Spans line 5) (Some(2, 13)) "start"
                        Expect.equal (SolidGenerate.locate unit.Spans line 11) (Some(2, 19)) "later"
                        Expect.equal (SolidGenerate.locate unit.Spans (lineOf "let Cell_u1" unit.Code) 5) (Some(2, 1)) "wrapper"
                    }
                ]

            testList
                "fable messages"
                [
                    let unit = unitOf (scan (fence "solid" "Counter ()")).Cells
                    let line = lineOf "    Counter ()" unit.Code
                    let file = SolidGenerate.fileName unit.Key

                    test "an error in a cell is placed on the page" {
                        let output =
                            $"C:\\ws\\%s{file}(%d{line},5): (%d{line},12) error FSHARP: The value 'Counter' is not defined. (code 39)"

                        let messages = SolidCompile.fableMessages [ unit ] output
                        let message = List.exactlyOne messages
                        Expect.isTrue message.IsError "error"
                        Expect.equal message.At (Some(2, 1)) "at"
                        Expect.equal message.Message "The value 'Counter' is not defined." "message"
                    }

                    test "a warning from a generated line is dropped" {
                        let output = $"C:\\ws\\%s{file}(1,1): (1,5) warning FSHARP: Something generated. (code 1)"
                        Expect.isEmpty (SolidCompile.fableMessages [ unit ] output) "dropped"
                    }

                    test "an error from elsewhere is kept, unplaced" {
                        let output = "C:\\ws\\Other.fs(3,1): (3,5) error FSHARP: Broken. (code 1)"
                        let message = List.exactlyOne (SolidCompile.fableMessages [ unit ] output)
                        Expect.isNone message.Page "page"
                        Expect.isNone message.At "at"
                    }
                ]

            testList
                "fable watch"
                [
                    let started = "Started Fable compilation..."
                    let watching = "Watching ."
                    let first = [ "Parsing Docs.fsproj..."; started; "Fable compilation finished in 6922ms"; watching ]

                    test "a change made while idle waits for a compilation of its own" {
                        Expect.isNone (SolidFableCycle.finished first first.Length) "nothing yet"
                    }

                    test "the compilation a change started is returned once it finishes" {
                        let lines = first @ [ started; "an error"; watching ]
                        let cycle = SolidFableCycle.finished lines first.Length
                        Expect.equal cycle (Some [ started; "an error"; watching ]) "cycle"
                    }

                    test "a compilation still running is waited for" {
                        let lines = first @ [ started; "an error" ]
                        Expect.isNone (SolidFableCycle.finished lines first.Length) "running"
                    }

                    test "a change made mid-compilation may be folded into it" {
                        let mark = 2
                        let cycle = SolidFableCycle.finished first mark
                        Expect.equal cycle (Some [ "Fable compilation finished in 6922ms"; watching ]) "cycle"
                    }

                    test "a compilation queued after the first is waited for" {
                        let lines = first @ [ started ]
                        Expect.isNone (SolidFableCycle.finished lines 2) "queued"
                    }

                    test "of several finished compilations, the last is returned" {
                        let lines = first @ [ started; "old error"; watching; started; "new error"; watching ]
                        let cycle = SolidFableCycle.finished lines first.Length
                        Expect.equal cycle (Some [ started; "new error"; watching ]) "cycle"
                    }
                ]

            testList
                "transforms"
                [
                    test "a transform fence becomes a transformed cell with its flags" {
                        let mutable seen = None

                        let transform (input: SolidTransformInput) =
                            seen <- Some input
                            SolidTransformOutput.Compiled mapped

                        let found = scanMap transform (fence "map timeline" "let x = 1\ncontrols []")
                        let cell = List.exactlyOne found.Cells

                        Expect.equal cell.Kind (SolidCellKind.Transformed mapped) "kind"
                        Expect.equal cell.Code "let x = 1\ncontrols []" "code is the author's"
                        Expect.isTrue cell.Mounts "mounts"

                        Expect.equal
                            seen
                            (Some
                                {
                                    Code = "let x = 1\ncontrols []"
                                    Flags = [ "timeline" ]
                                    PageKey = "pkey"
                                    CellId = "c1"
                                })
                            "input"
                    }

                    test "the body wraps the plain fence and the placeholder in a card" {
                        let found = scanMap (fun _ -> SolidTransformOutput.Compiled mapped) (fence "map timeline" "let x = 1")
                        let cell = List.exactlyOne found.Cells

                        let expected =
                            [
                                """<div class="partas-solid-card partas-solid-card--map">"""
                                ""
                                "```fsharp"
                                "let x = 1"
                                "```"
                                ""
                                SolidScan.placeholder "pkey" cell
                                ""
                                "</div>"
                            ]
                            |> String.concat "\n"

                        Expect.equal found.Body expected "body"
                    }

                    test "show=code mounts nothing" {
                        let found = scanMap (fun _ -> SolidTransformOutput.Compiled mapped) (fence "map show=code" "let x = 1")
                        let cell = List.exactlyOne found.Cells
                        Expect.isFalse cell.Mounts "mounts"
                        Expect.isFalse (found.Body.Contains "data-partas-cell") "no placeholder"
                        Expect.stringContains found.Body "let x = 1" "code shown"
                    }

                    test "a rejected fence reports its problems at body lines and stays plain code" {
                        let body = "intro\n\n" + fence "map" "let x = 1\nlet y = 2"
                        let found = scanMap (fun _ -> SolidTransformOutput.Rejected [ 2, "no controls" ]) body
                        Expect.isEmpty found.Cells "no cell"
                        Expect.equal found.Problems [ 5, "no controls" ] "problem at the fence's second line"
                        Expect.stringContains found.Body "let y = 2" "code shown"
                    }

                    test "a plain fsharp fence is left to other plugins" {
                        let body = fence "" "let x = 1"
                        let found = scanMap (fun _ -> failwith "not a map") body
                        Expect.isEmpty found.Cells "no cell"
                    }

                    test "generated lines of transformed code map to the fence, and the wrapper is pinned" {
                        let body = "intro\n\n" + fence "map" "let x = 1\nlet y = oops"
                        let found = scanMap (fun _ -> SolidTransformOutput.Compiled mapped) body
                        let unit = unitOf found.Cells
                        let generated = lineOf "let y = oops" unit.Code
                        Expect.equal (SolidGenerate.locate unit.Spans generated 13) (Some(5, 9)) "author's line"
                        let wrapperLine = lineOf "let Cell_c1 () = Render.map ()" unit.Code
                        Expect.equal (SolidGenerate.locate unit.Spans wrapperLine 5) (Some(3, 1)) "fence line"
                        Expect.equal (SolidGenerate.jsxNames (List.exactlyOne found.Cells)) [ "Cell_c1" ] "jsx names"
                    }
                ]
        ]
