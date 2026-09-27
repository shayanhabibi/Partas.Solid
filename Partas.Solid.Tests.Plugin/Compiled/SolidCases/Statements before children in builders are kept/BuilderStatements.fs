module Partas.Solid.Tests.SolidCases.BuilderStatements

open Partas.Solid

type Kind =
    | Alpha
    | Other

[<SolidComponent>]
let LetBeforeChildren (items: string[]) (log: Accessor<int> -> unit) =
    ul () {
        For.Keyed(each = items) {
            yield fun item index ->
                let title = item.ToUpper()
                log index
                li () { title }
        }
    }

[<SolidComponent>]
let MatchBranchStatements (kind: Kind) (log: string -> unit) =
    div () {
        match kind with
        | Alpha ->
            log "alpha"
            b () { "A" }
        | Other -> i () { "?" }
    }
