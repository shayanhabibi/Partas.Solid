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

[<Fact>]
let ``Field getters and records are transformed`` () =
    runSolidCase "FieldGettersInComputations"

[<Fact>]
let ``Tags can be used as values`` () =
    runSolidCase "TagsAsValuesSimple"

[<Fact>]
let ``FieldGets like props.words.Length are transformed`` () =
    runSolidCase "FieldGetExpressionsTransformed"

[<Fact>]
let ``Signal Setters can be invoked with a handler`` () =
    runSolidCase "SignalSetterInvoke"

[<Fact>]
let ``Experimental Builders compile correct output`` () =
    runSolidCase "ExperimentalBuilders"

[<Fact>]
let ``CssStyle definitions compile correct output`` () =
    runSolidCase "CssStyles"

[<Fact>]
let ``ChildLambdaProvider interfaces`` () =
    runSolidCase "ChildLambdaProvider"