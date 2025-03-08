module SolidTests

open Xunit
open Partas.Solid.Tests.Common

let runSolidCase caseName =
    let folderName = "SolidCases"
    runCase folderName caseName

[<Fact>]
let ``TagsNoChildren`` () =
    runSolidCase "TagsNoChildren"

[<Fact>]
let ``Library Imports And User Imports``() =
    runSolidCase "LibraryImport"

[<Fact>]
let ``Tag Extensions``() =
    runSolidCase "TagExtensions"

[<Fact>]
let ``Imported Tags With Extensions`` () =
    runSolidCase "ImportedTagsWithExtensions"

[<Fact>]
let ``Tags with child elements`` () =
    runSolidCase "ChildrenSimple"

[<Fact>]
let ``Default property setting`` () =
    runSolidCase "MergeProps"

[<Fact>]
let ``Property accessing with splitProps`` () =
    runSolidCase "SplitProps"

[<Fact>]
let ``mergeProps splitProps and property spreading`` () =
    runSolidCase "CombinedSpread"
    
[<Fact>]
let ``Property getters mixed with Operands are transformed`` () =
    runSolidCase "OperatorsInProps"