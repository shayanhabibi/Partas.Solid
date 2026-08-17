module Partas.Solid.Tests.AttributeCases.Pojo

open Partas.Solid
open Fable.Core

[<JS.Pojo>]
type Poj(?testAttr: string, ?otherAttr: string) =
    member val testAttr: string = JS.undefined with get, set
    member val testGrok: string = JS.undefined with get, set
    member val testBok: string = JS.undefined with get, set

[<SolidComponent>]
let Bock () =
    [|
        Poj (testGrok = "test", testBok = "bok", testAttr = "chok")
        Poj (testAttr = "tes")
        Poj (testGrok = "dfs", otherAttr = "sdf")
        Poj (testGrok = "dfs", otherAttr = "sdf")
    |]
