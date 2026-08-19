module Partas.Solid.Tests

open Browser.Types
open Fable.Core

[<Erase>]
type TransformGetterExtensions() =
    interface VoidNode

    [<Erase>]
    member val data: int[] = unbox null with get, set

    [<SolidTypeComponent(ComponentFlag.DebugMode)>]
    member props.constructor =
        let mutable element = JS.undefined<HTMLElement>
        let cbref = Ref<HTMLElement>.Callback(fun x -> ())
        let cb = fun (ele: HTMLElement) -> ()
        div() {
            div().ref([| Ref.cast cbref |])
            div().ref(cb)
        }
