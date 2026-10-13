module Partas.Solid.Tests.SolidCases.ImportedClassInEffect

open Partas.Solid
open Fable.Core
open Fable.Core.TS.Dom
open Fable.Core.TS
[<Import("Draggable", "@neodrag/vanilla")>]
type Draggable(node: HTMLElement, options: obj) =
    member _.destroy() : unit = jsNative

[<SolidComponent>]
let ImportedClassInEffect () =
    let mutable card: HTMLDivElement = JS.undefined
    let mutable drag: Draggable = JS.undefined

    onSettled (fun () ->
        drag <- Draggable(card, {| bounds = "parent" |})
        fun () -> drag.destroy ())

    div().ref (card) { "Drag me" }
