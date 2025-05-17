module Partas.Solid.Tests.IssueCases.ThisArgTransforms

open Partas.Solid.Tests.IssueCases.ThisArgTransformsTypes
open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop
open Fable.Core.JS

[<Erase>]
type PartasTable<'Data>() =
    inherit RegularNode()
    [<Erase>]
    member val dataStack: DataStack<'Data> = undefined with get,set
    [<SolidTypeComponent(PartasCompileFlag.DebugMode)>]
    member props.__ =
        
        let options =
            TableOptions<'Data>(
            )   .data(fun () -> props.dataStack.data)
        div()