module IssueTests

open Xunit
open Partas.Solid.Tests.Common

let runIssueCase caseName =
    let folderName = "IssueCases"
    runCase folderName caseName

[<Fact>]
let ``#2 createSignal getting converted into Tag`` () =
    runIssueCase "CreateSignalTagConstructor"

[<Fact>]
let ``#9 Index access is rendered`` () =
    runIssueCase "IndexAccess"