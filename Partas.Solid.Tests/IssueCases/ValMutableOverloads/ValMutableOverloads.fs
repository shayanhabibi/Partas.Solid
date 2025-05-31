module Partas.Solid.Tests.IssueCases.ValMutableOverloads

open Partas.Solid
open Fable.Core

[<Erase>]
type MemberVal() =
    interface RegularNode
    [<Erase>]
    member val index: int = unbox null with get,set 
    member this.overloaded
        with inline set(value: string) = this.index <- unbox value

[<Erase>]
type ValMutable() =
    interface RegularNode
    [<DefaultValue>]
    val mutable index: int
    member this.overloaded
        with inline set(value: string) = this.index <- unbox value

[<SolidComponent>]
let TestCase() =
    Fragment() {
        MemberVal(overloaded = "test")
        ValMutable(overloaded = "test")
    }
