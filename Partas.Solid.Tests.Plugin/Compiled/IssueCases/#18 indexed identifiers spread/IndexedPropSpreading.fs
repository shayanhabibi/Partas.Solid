module Partas.Solid.Tests.IssueCases.IndexedPropSpreading

open Partas.Solid
open Fable.Core

/// Opaque to Fable, so the returned tuple cannot be destructured at compile time
/// and each spread must read the tuple by index (`patternInput[0]`).
/// Solid 2 removed `splitProps`, and nothing left in solid-js returns a props tuple, so this stands in for it.
/// `./partitionProps.js` deliberately does not exist: the case is only compiled and snapshot-compared, never run.
[<Import("partitionProps", "./partitionProps.js")>]
let partitionProps (o: 'T, local: string[], others: string[]) : 'T * 'T = jsNative

[<Erase>]
type Select() =
    inherit select()

    [<SolidTypeComponent>]
    member props.__ =
        let rootProps, selectProps =
            partitionProps (props, [| "name"; "placeholder"; "required"; "disabled" |], [| "ref"; "onInput"; "onChange"; "onBlur" |])

        div().spread rootProps { select().spread selectProps }
