module Partas.Solid.Tests.IssueCases.IndexAccess

open Partas.Solid
open Fable.Core

[<JS.Pojo>]
type Container (value: Accessor<string>) = member val value: Accessor<string>

[<Erase>]
type TestStringAccess() =
    inherit VoidNode()
    [<Erase>]
    member val index: int = jsNative with get,set
    [<SolidTypeComponent>]
    member props.constructor =
        let context: Container = unbox null
        let character(): string = context.value()[props.index] |> string
        div() {
            character()
        }


