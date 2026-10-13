namespace Nacara.Plugins.Internal

open System
open System.Text
open System.Text.RegularExpressions
open Nacara.Plugins

/// <summary>What scanning a page's body found.</summary>
type SolidScan =
    {
        /// <summary>The body with every solid fence rewritten to what the page shows.</summary>
        Body: string
        Cells: SolidCell list
        /// <summary>Problems with how a fence was written, by body line.</summary>
        Problems: (int * string) list
    }

/// <summary>
/// Finds solid fences in markdown and rewrites them to a plain fence and a placeholder.
/// </summary>
/// <remarks>Line based rather than a markdown parse: all it has to know is where fences open and
/// close, and it runs before the markdown plugin has seen the page.</remarks>
[<RequireQualifiedAccess>]
module SolidScan =

    let private opening =
        Regex(@"^(?<indent> {0,3})(?<fence>`{3,}|~{3,})\s*(?<info>.*)$", RegexOptions.Compiled)

    let private languages = set [ "fsharp"; "fs"; "f#" ]

    let private closes (fence: string) (line: string) =
        let trimmed = line.Trim()

        trimmed.Length >= fence.Length
        && trimmed |> Seq.forall (fun c -> c = fence[0])

    /// <summary>Whether code is declarations rather than an expression to render.</summary>
    /// <remarks>Decided by the last line at column zero, so bindings that lead up to an expression
    /// still make an expression.</remarks>
    let declares (code: string) =
        code.Split('\n')
        |> Array.map _.TrimEnd('\r')
        |> Array.filter (fun line ->
            line <> ""
            && not (Char.IsWhiteSpace line[0])
            && not (line.StartsWith "//")
            && not (line.StartsWith "(*")
            // A closing bracket at column zero ends something above it rather than starting anything.
            && not (")]}|".Contains line[0])
        )
        |> Array.tryLast
        |> Option.map (fun line ->
            [ "let "; "type "; "open "; "module "; "[<"; "#nowarn"; "exception "; "and " ]
            |> List.exists line.StartsWith
        )
        |> Option.defaultValue true

    /// <summary>The placeholder a cell mounts into.</summary>
    /// <remarks>A use sits in a paragraph, where a <c>div</c> would end it, so it gets a <c>span</c>.</remarks>
    let placeholder (pageKey: string) (cell: SolidCell) =
        let data = $"data-partas-page=\"%s{pageKey}\" data-partas-cell=\"%s{cell.Id}\""

        match cell.Kind, cell.Show with
        | SolidCellKind.Use _, _ -> $"""<span class="partas-solid partas-solid--inline" %s{data}></span>"""
        | _, SolidShow.Inline -> $"""<div class="partas-solid partas-solid--inline" %s{data}></div>"""
        | _ -> $"""<div class="partas-solid" %s{data}></div>"""

    let private isUse (cell: SolidCell) =
        match cell.Kind with
        | SolidCellKind.Use _ -> true
        | _ -> false

    /// <summary>A code span in prose that holds an expression to render.</summary>
    type SolidUse =
        {
            /// <summary>Where the span starts in its line, counting from zero, backticks included.</summary>
            Start: int
            Length: int
            Code: string
            /// <summary>The column the expression starts on, counting from one.</summary>
            Column: int
        }

    /// <summary>The code spans in a line that start with the fence token and a colon.</summary>
    /// <remarks>
    /// Written <c>`solid: Badge "new"`</c>, so a viewer that knows nothing of the plugin still shows
    /// the code. Spans follow CommonMark: a run of backticks closes at the next run as long. A space
    /// before the token makes a span plain code, which is how a page shows the syntax.
    /// </remarks>
    let uses (fenceToken: string) (line: string) =
        let prefix = fenceToken + ":"

        let runAt at =
            let mutable finish = at

            while finish < line.Length && line[finish] = '`' do
                finish <- finish + 1

            finish - at

        [
            let mutable at = 0

            while at < line.Length do
                if line[at] <> '`' || (at > 0 && line[at - 1] = '\\') then
                    at <- at + 1
                else
                    let run = runAt at
                    let mutable close = at + run
                    let mutable found = -1

                    while found < 0 && close < line.Length do
                        if line[close] = '`' then
                            let length = runAt close

                            if length = run then found <- close else close <- close + length
                        else
                            close <- close + 1

                    if found < 0 then
                        // An unclosed run is literal backticks.
                        at <- at + run
                    else
                        let inner = line.Substring(at + run, found - at - run)

                        // Only hard against the backticks: `` `solid: x` `` shows the syntax itself.
                        // `solid:` alone names the syntax rather than using it.
                        if inner.StartsWith prefix && inner.Substring(prefix.Length).Trim() <> "" then
                            let afterPrefix = at + run + prefix.Length
                            let code = line.Substring(afterPrefix, found - afterPrefix)
                            let skipped = code.Length - code.TrimStart().Length

                            {
                                Start = at
                                Length = found + run - at
                                Code = code.Trim()
                                Column = afterPrefix + skipped + 1
                            }

                        at <- found + run
        ]

    /// <summary>Where the JSX Fable made of a cell goes, as a highlighted block, once it has compiled.</summary>
    /// <remarks>The markdown is rendered before Fable runs, so the block is put in the rendered page.</remarks>
    let jsxMarker (pageKey: string) (cell: SolidCell) =
        $"""<div data-partas-jsx-page="%s{pageKey}" data-partas-jsx-cell="%s{cell.Id}"></div>"""

    type private Tokens =
        {
            Setup: bool
            Jsx: bool
            Render: string option
            Show: SolidShow option
            Id: string option
            Rest: string list
            Problems: string list
        }

    let private readTokens (fenceToken: string) (tokens: string list) =
        let value (token: string) =
            token.Substring(token.IndexOf '=' + 1).Trim('"', '\'')

        let start =
            {
                Setup = false
                Jsx = false
                Render = None
                Show = None
                Id = None
                Rest = []
                Problems = []
            }

        (start, tokens)
        ||> List.fold (fun state token ->
            match token with
            | token when token = fenceToken -> state
            | "setup" -> { state with Setup = true }
            | "jsx" -> { state with Jsx = true }
            | token when token.StartsWith "render=" -> { state with Render = Some(value token) }
            | token when token.StartsWith "id=" -> { state with Id = Some(value token) }
            | token when token.StartsWith "show=" ->
                match value token with
                | "both" -> { state with Show = Some SolidShow.Both }
                | "code" -> { state with Show = Some SolidShow.Code }
                | "output" -> { state with Show = Some SolidShow.Output }
                | "inline" -> { state with Show = Some SolidShow.Inline }
                | other ->
                    { state with
                        Problems = state.Problems @ [ $"show=%s{other} is not one of both, code, output or inline" ]
                    }
            | token -> { state with Rest = state.Rest @ [ token ] }
        )

    /// <summary>Scan a page's body.</summary>
    /// <param name="transforms">Fence transforms, by the token that marks a fence for one.</param>
    /// <param name="fenceToken">The word that marks a fence as a solid example.</param>
    /// <param name="showJsx">Show the JSX of every cell, not only of fences marked <c>jsx</c>.</param>
    /// <param name="pageKey">Written into each placeholder, so the loader knows which bundle to load.</param>
    /// <param name="body">The page's markdown.</param>
    let scanWith
        (transforms: (string * (SolidTransformInput -> SolidTransformOutput)) list)
        (fenceToken: string)
        (showJsx: bool)
        (pageKey: string)
        (body: string)
        =
        let lines = body.Split('\n') |> Array.map _.TrimEnd('\r')
        let output = StringBuilder()
        let cells = ResizeArray<SolidCell>()
        let problems = ResizeArray<int * string>()
        let emit (line: string) = output.Append(line).Append('\n') |> ignore
        let mutable index = 0

        while index < lines.Length do
            let line = lines[index]
            let matched = opening.Match line

            if not matched.Success then
                let found = uses fenceToken line

                if found.IsEmpty then
                    emit line
                else
                    let rewritten = StringBuilder()
                    let mutable copied = 0

                    for used in found do
                        let id = $"u%d{(cells |> Seq.filter isUse |> Seq.length) + 1}"

                        if cells |> Seq.exists (fun cell -> cell.Id = id) then
                            problems.Add(index + 1, $"id=%s{id} is taken by a fence, so an inline use cannot have it")

                        let cell =
                            {
                                Id = id
                                Kind = SolidCellKind.Use used.Column
                                Show = SolidShow.Inline
                                Code = used.Code
                                Line = index + 1
                                // Tabs after a span would split the paragraph it sits in.
                                Jsx = false
                            }

                        cells.Add cell

                        rewritten.Append(line, copied, used.Start - copied).Append(placeholder pageKey cell)
                        |> ignore

                        copied <- used.Start + used.Length

                    emit (rewritten.Append(line, copied, line.Length - copied).ToString())

                index <- index + 1
            else
                let fence = matched.Groups["fence"].Value
                let info = matched.Groups["info"].Value.Trim()

                let words =
                    info.Split([| ' '; '\t' |], StringSplitOptions.RemoveEmptyEntries) |> List.ofArray

                let closing =
                    seq { index + 1 .. lines.Length - 1 }
                    |> Seq.tryFind (fun at -> closes fence lines[at])
                    |> Option.defaultValue lines.Length

                let isSolid =
                    match words with
                    | language :: rest ->
                        languages.Contains(language.ToLowerInvariant()) && List.contains fenceToken rest
                    | [] -> false

                let transform =
                    match words with
                    | language :: rest when not isSolid && languages.Contains(language.ToLowerInvariant()) ->
                        transforms |> List.tryFind (fun (token, _) -> List.contains token rest)
                    | _ -> None

                let nextId () =
                    $"c%d{(cells |> Seq.filter (isUse >> not) |> Seq.length) + 1}"

                match transform with
                | Some(token, apply) ->
                    let language = List.head words
                    let tokens = readTokens token (List.tail words)
                    let code = lines[index + 1 .. closing - 1] |> String.concat "\n"
                    let bodyLine = index + 2

                    for problem in tokens.Problems do
                        problems.Add(index + 1, problem)

                    let id = tokens.Id |> Option.defaultWith nextId

                    if cells |> Seq.exists (fun cell -> cell.Id = id) then
                        problems.Add(index + 1, $"id=%s{id} is already used on this page")

                    let show = tokens.Show |> Option.defaultValue SolidShow.Both

                    // The page shows the fence as plain code, without the words the transform read.
                    let emitCode () =
                        emit (matched.Groups["indent"].Value + fence + language)

                        for at in index + 1 .. closing - 1 do
                            emit lines[at]

                        emit (matched.Groups["indent"].Value + fence)

                    let input =
                        {
                            Code = code
                            Flags = tokens.Rest
                            PageKey = pageKey
                            CellId = id
                        }

                    match apply input with
                    | SolidTransformOutput.Rejected found ->
                        for line, problem in found do
                            problems.Add(bodyLine + line - 1, problem)

                        emitCode ()
                    | SolidTransformOutput.Compiled transformed ->
                        let cell =
                            {
                                Id = id
                                Kind = SolidCellKind.Transformed transformed
                                Show = show
                                Code = code
                                Line = bodyLine
                                Jsx = false
                            }

                        cells.Add cell
                        let shown = show = SolidShow.Both || show = SolidShow.Code

                        if cell.Mounts then
                            emit $"""<div class="partas-solid-card partas-solid-card--%s{token}">"""
                            emit ""

                            if shown then
                                emitCode ()
                                emit ""

                            emit (placeholder pageKey cell)
                            emit ""
                            emit "</div>"
                        elif shown then
                            emitCode ()

                    index <- closing + 1
                | None ->

                if not isSolid then
                    for at in index .. min closing (lines.Length - 1) do
                        emit lines[at]

                    index <- closing + 1
                else
                    let language = List.head words
                    let tokens = readTokens fenceToken (List.tail words)
                    let code = lines[index + 1 .. closing - 1] |> String.concat "\n"
                    let bodyLine = index + 2

                    for problem in tokens.Problems do
                        problems.Add(index + 1, problem)

                    let kind =
                        match tokens.Setup, tokens.Render with
                        | true, _ -> SolidCellKind.Setup
                        | false, Some name -> SolidCellKind.Declarations(Some name)
                        | false, None when declares code -> SolidCellKind.Declarations None
                        | false, None -> SolidCellKind.Expression

                    let id = tokens.Id |> Option.defaultWith nextId

                    if cells |> Seq.exists (fun cell -> cell.Id = id) then
                        problems.Add(index + 1, $"id=%s{id} is already used on this page")

                    let cell =
                        {
                            Id = id
                            Kind = kind
                            Show = tokens.Show |> Option.defaultValue SolidShow.Both
                            Code = code
                            Line = bodyLine
                            Jsx = (tokens.Jsx || showJsx) && not tokens.Setup
                        }

                    cells.Add cell

                    let shown =
                        match cell.Kind, cell.Show with
                        | SolidCellKind.Setup, _
                        | _, SolidShow.Output
                        | _, SolidShow.Inline -> false
                        | _ -> true

                    // With its JSX, the code is a tab beside it, so the two read as one listing.
                    if cell.Jsx then
                        emit "::::tabs"

                    if shown then
                        if cell.Jsx then
                            emit ":::tab F#"

                        emit (
                            matched.Groups["indent"].Value
                            + fence
                            + String.concat " " (language :: tokens.Rest)
                        )

                        for at in index + 1 .. closing - 1 do
                            emit lines[at]

                        emit (matched.Groups["indent"].Value + fence)

                        if cell.Jsx then
                            emit ":::"

                    if cell.Jsx then
                        emit ":::tab JSX"
                        emit ""
                        emit (jsxMarker pageKey cell)
                        // A blank line ends the html block, or it would swallow the directive's close.
                        emit ""
                        emit ":::"
                        emit "::::"

                    if cell.Mounts then
                        emit ""
                        emit (placeholder pageKey cell)
                        emit ""
                    elif not shown then
                        // A hidden fence leaves blank lines behind, so what follows stays on the line
                        // it was written on.
                        for _ in index .. min closing (lines.Length - 1) do
                            emit ""

                    index <- closing + 1

        {
            Body = output.ToString().TrimEnd('\n') + (if body.EndsWith "\n" then "\n" else "")
            Cells = List.ofSeq cells
            Problems = List.ofSeq problems
        }

    /// <summary>Scan a page's body, with no fence transforms.</summary>
    let scan (fenceToken: string) (showJsx: bool) (pageKey: string) (body: string) =
        scanWith [] fenceToken showJsx pageKey body
