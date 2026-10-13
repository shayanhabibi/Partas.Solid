namespace Nacara.Plugins.Internal

open System
open System.Security.Cryptography
open System.Text
open System.Text.Json
open System.Text.RegularExpressions
open Nacara.Plugins

/// <summary>Turns a page's cells into an F# module and the entry that mounts them.</summary>
[<RequireQualifiedAccess>]
module SolidGenerate =

    /// <summary>Lowercase hex of a SHA-256, cut to the length asked for.</summary>
    let hash (length: int) (text: string) =
        SHA256.HashData(Encoding.UTF8.GetBytes text)
        |> Convert.ToHexString
        |> _.ToLowerInvariant().Substring(0, length)

    /// <summary>The key a page's module, entry and bundle are named by.</summary>
    /// <remarks>Taken from the page's id, so it survives edits to the page.</remarks>
    let pageKey (pageId: string) = "p" + hash 10 pageId

    let moduleName (key: string) =
        $"Partas.Solid.Docs.P%s{key.Substring 1}"

    let fileName (key: string) = $"P%s{key.Substring 1}.fs"

    let wrapper (cell: SolidCell) = $"Cell_%s{cell.Id.Replace('-', '_')}"

    /// <summary>The page's module, and where each run of it came from.</summary>
    /// <remarks>
    /// No <c>#line</c> directives: Fable drops an error it cannot place in a project file and
    /// compiles the expression to a <c>throw</c>, so diagnostics are mapped back here instead.
    /// </remarks>
    let fsharp (prelude: string list) (key: string) (cells: SolidCell list) =
        let lines = ResizeArray<string>()
        let spans = ResizeArray<SolidLineSpan>()
        let add (line: string) = lines.Add line

        let addCode (indent: int) (cell: SolidCell) =
            let code = cell.Code.Split('\n') |> Array.map _.TrimEnd('\r')

            spans.Add
                {
                    Generated = lines.Count + 1
                    Length = code.Length
                    Body = cell.Line
                    Indent = indent
                }

            let pad = String(' ', indent)

            for line in code do
                add (if line.Trim() = "" then "" else pad + line)

        // A line the generator wrote for a cell is blamed on the fence that asked for it.
        let pinned (cell: SolidCell) (line: string) =
            spans.Add
                {
                    Generated = lines.Count + 1
                    Length = 1
                    Body = cell.Line - 1
                    Indent = Int32.MaxValue
                }

            add line

        add $"module %s{moduleName key}"
        add ""

        for line in prelude do
            add line

        // Uses come last, so prose can use a component before the fence that declares it.
        let uses, fences =
            cells
            |> List.partition (fun cell ->
                match cell.Kind with
                | SolidCellKind.Use _ -> true
                | _ -> false
            )

        for cell in fences @ uses do
            add ""

            match cell.Kind with
            | SolidCellKind.Setup
            | SolidCellKind.Declarations None -> addCode 0 cell
            | SolidCellKind.Declarations(Some render) ->
                addCode 0 cell
                add ""
                pinned cell "[<Partas.Solid.SolidComponent>]"
                pinned cell $"let %s{wrapper cell} () = %s{render} ()"
            | SolidCellKind.Expression ->
                pinned cell "[<Partas.Solid.SolidComponent>]"
                pinned cell $"let %s{wrapper cell} () ="
                addCode 4 cell
            | SolidCellKind.Transformed transformed ->
                let start = lines.Count

                for span in transformed.Spans do
                    spans.Add
                        { span with
                            Generated = start + span.Generated
                            Body =
                                if span.Indent = Int32.MaxValue then
                                    cell.Line - 1
                                else
                                    cell.Line + span.Body - 1
                        }

                for line in transformed.Code.Split('\n') do
                    add (line.TrimEnd('\r'))

                add ""
                pinned cell "[<Partas.Solid.SolidComponent>]"
                pinned cell $"let %s{wrapper cell} () = %s{transformed.Render}"
            | SolidCellKind.Use column ->
                // Blamed on the use itself: it has no fence line above it.
                let mapped indent =
                    spans.Add
                        {
                            Generated = lines.Count + 1
                            Length = 1
                            Body = cell.Line
                            Indent = indent
                        }

                mapped Int32.MaxValue
                add "[<Partas.Solid.SolidComponent>]"
                mapped Int32.MaxValue
                add $"let %s{wrapper cell} () ="
                // Four columns in here, and column in its line on the page.
                mapped (4 - (column - 1))
                add ("    " + cell.Code)

        (String.concat "\n" lines + "\n"), List.ofSeq spans

    /// <summary>The bundle entry for a page: one <c>mount</c> that knows every cell.</summary>
    let entry (key: string) (cells: SolidCell list) =
        let mounted =
            cells
            |> List.filter _.Mounts
            |> List.map (fun cell -> $"    \"%s{cell.Id}\": m.%s{wrapper cell},")
            |> String.concat "\n"

        $"""import {{ render, createComponent }} from "@solidjs/web";
import * as m from "../%s{fileName key}.jsx";

const cells = {{
%s{mounted}
}};

export function mount(cell, element) {{
    const component = cells[cell];
    if (!component) throw new Error(`No cell called '${{cell}}'`);
    return render(() => createComponent(component, {{}}), element);
}}
"""

    /// <summary>Map a line of a generated module back to its page.</summary>
    /// <returns>The body line and column, when the line came from a cell.</returns>
    let locate (spans: SolidLineSpan list) (line: int) (column: int) =
        spans
        |> List.tryFind (fun span -> line >= span.Generated && line < span.Generated + span.Length)
        |> Option.map (fun span ->
            let column =
                if span.Indent = Int32.MaxValue then 1 else max 1 (column - span.Indent)

            span.Body + line - span.Generated, column
        )

    let private declared =
        Regex(
            @"^(?:let|and|type)\s+(?:(?:rec|inline|private|internal|public|mutable)\s+)*(?<name>[A-Za-z_]\w*)",
            RegexOptions.Compiled
        )

    /// <summary>The names whose JSX a cell's JSX tab shows.</summary>
    /// <remarks>
    /// An expression is its wrapper. Declarations are what the cell declares at column zero, or the
    /// component it renders when it declares nothing.
    /// </remarks>
    let jsxNames (cell: SolidCell) =
        match cell.Kind with
        | SolidCellKind.Expression
        | SolidCellKind.Transformed _
        | SolidCellKind.Use _ -> [ wrapper cell ]
        | SolidCellKind.Setup -> []
        | SolidCellKind.Declarations render ->
            let names =
                cell.Code.Split('\n')
                |> Array.map (fun line -> declared.Match(line.TrimEnd('\r')))
                |> Array.filter _.Success
                |> Array.map _.Groups["name"].Value
                |> Array.distinct
                |> List.ofArray

            match names, render with
            | [], Some render -> [ render ]
            | names, _ -> names

    /// <summary>A top-level declaration of the module's JSX, cut out by name.</summary>
    /// <remarks>
    /// Fable writes each one from column zero and closes a block with a brace at column zero, so the
    /// first such brace ends it. A declaration on one line ends with it.
    /// </remarks>
    let jsxDeclaration (jsx: string) (name: string) =
        let lines = jsx.Split('\n') |> Array.map _.TrimEnd('\r')

        let starts (line: string) =
            [ "export function "; "function "; "export class "; "class "; "export const "; "const "; "export let "; "let " ]
            |> List.exists (fun keyword ->
                line.StartsWith(keyword + name)
                && (let rest = line.Substring((keyword + name).Length) in rest.StartsWith "(" || rest.StartsWith " ")
            )

        lines
        |> Array.tryFindIndex starts
        |> Option.map (fun first ->
            let last =
                if lines[first].EndsWith ";" then
                    first
                else
                    seq { first + 1 .. lines.Length - 1 }
                    |> Seq.tryFind (fun at -> lines[at].StartsWith "}")
                    |> Option.defaultValue (lines.Length - 1)

            lines[first..last] |> String.concat "\n"
        )

    /// <summary>The JSX tab of each cell marked <c>jsx</c>, by cell id.</summary>
    /// <param name="cells">The page's cells; only those marked <c>jsx</c> are kept.</param>
    /// <param name="jsx">The <c>.fs.jsx</c> Fable wrote for the page's module.</param>
    let jsxCode (cells: SolidCell list) (jsx: string) =
        [
            for cell in cells |> List.filter _.Jsx do
                let found =
                    match jsxNames cell |> List.choose (jsxDeclaration jsx) with
                    // Nothing declared by a name Fable kept: the wrapper at least shows what mounts.
                    | [] -> jsxDeclaration jsx (wrapper cell) |> Option.toList
                    | found -> found

                cell.Id, String.concat "\n\n" found
        ]
        |> Map.ofList
