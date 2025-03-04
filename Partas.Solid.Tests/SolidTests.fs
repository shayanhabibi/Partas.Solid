﻿module SolidTests

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