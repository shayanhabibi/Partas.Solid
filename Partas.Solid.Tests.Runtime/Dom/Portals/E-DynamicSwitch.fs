module Partas.Solid.Tests.Runtime.Dom.Portals.DynamicSwitch

// Isolated: the emitted JSX imports op_BangAt from Builder.fs.jsx, which does not exist, so this
// module cannot be loaded. Kept separate so the other Dynamic cases still load.

open Partas.Solid
open Partas.Solid.Web
open Fable.Core
open Fable.Core.JsInterop
open Partas.Solid.Tests.Runtime.Dom.Portals.Dynamic

/// Dynamic switching between a tag name and a component.
[<Erase>]
type TagOrComponent() =
    inherit div()

    [<Erase>]
    member val onDispose: string -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let fancy, setFancy = createSignal false
        let fancyProps = createObj [ "label", box "hi"; "onDispose", box props.onDispose ]

        div (class' = "toc") {
            button (id = "toggle-fancy", onClick = fun _ -> setFancy (not (fancy ()))) { "toggle" }
            Dynamic<obj>(component' = (if fancy () then !@Fancy else unbox "em")).spread (fancyProps)
        }

