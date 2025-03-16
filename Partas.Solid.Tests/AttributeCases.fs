module AttributeCases

open Xunit
open Partas.Solid.Tests.Common

let runAttributeCase caseName =
    let folderName = "AttributeCases"
    runCase folderName caseName

[<Fact>]
let ``PartasImport Attribute`` () =
    runAttributeCase "PartasImportAttr"