module Partas.Solid.Tests.IssueCases.IndexedPropSpreading

open Partas.Solid
open Fable.Core

[<Import("splitProps", "solid-js")>]
let splitProps (o: 'T, arr1: string[], arr2: string[]) : 'T * 'T = jsNative

[<Erase>]
type Select() =
    inherit select()

    [<SolidTypeComponent>]
    member props.__ =
        let rootProps, selectProps =
            splitProps (props, [| "name"; "placeholder"; "required"; "disabled" |], [| "placeholder"; "ref"; "onInput"; "onChange"; "onBlur" |])

        div().spread rootProps { select().spread selectProps }
