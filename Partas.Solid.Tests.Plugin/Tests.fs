module Partas.Solid.Tests.Plugin.IssueTests

open Expecto
open Partas.Solid.Tests.Plugin.Common

let built  = lazy buildCases()
let runIssueCase caseName = fun _ ->
    if not built.IsValueCreated then built.Force()
    let folderName = "IssueCases"
    runCase folderName caseName
let runSolidCase name caseName =
    let runSolidCase' caseName = fun _ ->
        if not built.IsValueCreated then built.Force()
        let folderName = "SolidCases"
        runCase folderName caseName
    testCase name <| runSolidCase' caseName
let runAttributeCase name caseName =
    let runAttributeCase' caseName = fun _ ->
        if not built.IsValueCreated then built.Force()
        let folderName = "AttributeCases"
        runCase folderName caseName
    testCase name <| runAttributeCase' caseName


[<Tests>]
let IssueCases =
    testList "IssueCases" [
        testCase "#2 createSignal getting converted into Tag" <| runIssueCase "CreateSignalTagConstructor"
        testCase "#9 Index access is rendered" <| runIssueCase "IndexAccess"
        testCase "#11 String interpolation is transformed" <| runIssueCase "TransformInsideStringInterpolation"
        testCase "#13 Getter extensions are transformed" <| runIssueCase "TransformGetterExtensions"
        testCase "#14 Object Expressions are transformed" <| runIssueCase "ObjectExpressions"
        testCase "#15 props.words.ToCharArray() |> Array.map string" <| runIssueCase "CharArrayMapping"
        testCase "#16 val mutable overloads render" <| runIssueCase "ValMutableOverloads"
        testCase "#18 indexed identifiers spread" <| runIssueCase "IndexedPropSpreading"
        testCase "#28 ThisArg is transformed" <| runIssueCase "ThisArgTransforms"
        testCase "#29 val mutable overloads render 2" <| runIssueCase "InheritedProperty"
    ]

[<Tests>]
let SolidCases =
    testList "SolidCases" [
        runSolidCase "TagsNoChildren" "TagsNoChildren"
        runSolidCase "Library Imports and User Imports" "LibraryImport"
        "TagExtensions" |> runSolidCase "Tag Extensions"
        "ImportedTagsWithExtensions" |> runSolidCase "Imported Tags with Extensions"
        "ChildrenSimple" |> runSolidCase "Tags with child elements"
        "MergeProps" |> runSolidCase "Default property setting"
        "SplitProps" |> runSolidCase "Property accessing with split props"
        "CombinedSpread" |> runSolidCase "mergeProps splitProps and property spreading"
        "OperatorsInProps" |> runSolidCase "Property getters mixed with Operands are transformed"
        "FieldGettersInComputations" |> runSolidCase "Field getters and records are transformed"
        "TagsAsValuesSimple" |> runSolidCase "Tags can be used as values"
        "FieldGetExpressionsTransformed" |> runSolidCase "FieldGets like props.words.Length are transformed"
        "SignalSetterInvoke" |> runSolidCase "Signal Setters can be invoked with a handler"
        "ExperimentalBuilders" |> runSolidCase "Experimental Builders compile correct output"
        "CssStyles" |> runSolidCase "CssStyle definitions compiles correct output"
        "ChildLambdaProvider" |> runSolidCase "ChildLambdaProvider interfaces"
    ]

[<Tests>]
let AttributeCases =
    testList "AttributeCases" [
        "PartasImportAttr" |> runAttributeCase "PartasImport Attribute"
        "Pojo" |> runAttributeCase "Pojo Optimisation"
    ]
