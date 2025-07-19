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
    let tog = Poj (testGrok = "test", testBok = "bok", testAttr = "chok")
    let bop = Poj (testAttr = "tes")
    let chog = Poj (testGrok = "dfs", otherAttr = "sdf")
    Poj (testGrok = "dfs", otherAttr = "sdf")
