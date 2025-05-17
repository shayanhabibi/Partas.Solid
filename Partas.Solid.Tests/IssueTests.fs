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

[<Fact>]
let ``#11 String interpolation is transformed`` () =
    runIssueCase "TransformInsideStringInterpolation"

[<Fact>]
let ``#13 Getter extensions are transformed`` () =
    runIssueCase "TransformGetterExtensions"

[<Fact>]
let ``#14 Object Expressions are transformed`` () =
    runIssueCase "ObjectExpressions"

[<Fact>]
let ``#15 props.words.ToCharArray() |> Array.map string`` () =
    runIssueCase "CharArrayMapping"

[<Fact>]
let ``#16 val mutable overloads render`` () =
    runIssueCase "ValMutableOverloads"

[<Fact>]
let ``#18 indexed identifiers spread`` () =
    runIssueCase "IndexedPropSpreading"

[<Fact>]
let ``#27 list CE 'empty' rendered out`` () =
    runIssueCase "EmptyCE"

[<Fact>]
let ``#28 ThisArg is transformed`` () =
    runIssueCase "ThisArgTransforms"