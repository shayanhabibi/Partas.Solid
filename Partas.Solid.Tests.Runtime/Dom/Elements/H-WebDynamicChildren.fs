module Partas.Solid.Tests.Runtime.Dom.Elements.WebDynamicChildren

// Kept in its own module: the emitted JSX imports an erased extension that does not exist at
// runtime, so importing this module fails. Isolated so it cannot break the other Web cases.

open Partas.Solid
open Partas.Solid.Web
open Fable.Core
open Fable.Core.JsInterop

/// Dynamic with a string tag name.
[<SolidComponent>]
let DynamicString () =
    let dynProps = createObj [ "id", box "dyn" ]
    Dynamic<obj>(componentAsString = "h2").spread (dynProps) { "dynamic heading" }

/// Dynamic with a tag name from a prop.
[<Erase>]
type DynamicProp() =
    inherit div()

    [<Erase>]
    member val tag: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let dynProps = createObj [ "class", box "dp" ]
        Dynamic<obj>(componentAsString = props.tag).spread (dynProps) { "dyn" }

/// The plainest form: a string tag with a text child and no other props.
[<SolidComponent>]
let DynamicPlain () =
    Dynamic<obj>(componentAsString = "h3") { "plain" }
