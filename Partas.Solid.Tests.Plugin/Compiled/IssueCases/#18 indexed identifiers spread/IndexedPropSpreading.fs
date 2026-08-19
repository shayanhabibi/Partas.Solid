module Partas.Solid.Tests.IssueCases.IndexedPropSpreading

open Partas.Solid
open Fable.Core

[<Erase>]
type Select() =
    inherit select()

    [<SolidTypeComponent>]
    member props.__ =
        let rootProps, selectProps =
            {||}, omit(props, "name", "placeholder", "required", "disabled", "placeholder", "ref", "onInput", "onChange", "onBlur")

        div().spread rootProps { select().spread selectProps }
