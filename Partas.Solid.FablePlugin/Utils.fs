/// Variety of useful Pattern Recognizers for dealing with strings within pattern matchers
[<RequireQualifiedAccess>]
module internal Partas.Solid.Utils

let (|StartsWith|_|) (value: string) = function
    | (s: string) when s.StartsWith(value) -> Some()
    | _ -> None
let (|EndsWith|_|) (value: string) = function
    | (s: string) when s.EndsWith(value) -> Some()
    | _ -> None
let (|StartsWithTrimmed|_|) (value: string) = function
    | StartsWith value as s ->
        s.Substring(value.Length)
        |> Some
    | _ -> None
let (|EndsWithTrimmed|_|) (value: string) = function
    | EndsWith value as s ->
        s.Substring(0, s.Length - value.Length)
        |> Some
    | _ -> None
let rec trimReservedIdentifiers = function
    | EndsWithTrimmed "'" s -> trimReservedIdentifiers s
    | EndsWithTrimmed "`1" s -> trimReservedIdentifiers s
    | s when s.Length > 2 && s.Substring(0, s.Length - 2).StartsWith('`') ->
        s.Substring(0, s.Length - 2)
        |> trimReservedIdentifiers
    | s -> s
