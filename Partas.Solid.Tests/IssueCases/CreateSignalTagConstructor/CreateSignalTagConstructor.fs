module Partas.Solid.Tests.IssueCases.CreateSignalTagConstructor

// #2
// createSignal was being converted into <createSignal /> due to a nested tagConstructor
// combined with the fact that it was a user import at the top level. Or something along those lines.
// Fixed by making the tag constructors match for ".ctor" in the member info

open Partas.Solid
open Fable.Core
open CreateSignalTagConstructor.Types

[<JS.Pojo>]
type MenuItem(title: string, url: string, icon: RegularNode) =
    member val title = title
    member val url = url
    member val icon = icon
module App =
    [<Import("fakeTag", "fakeLibrary")>]
    type [<Erase>] MyItemizer() =
        interface RegularNode
        member val items: MenuItem array = unbox null with get,set
        [<SolidTypeComponent>]
        member props.constructor =
            div()
    
    [<SolidComponent>]
    let App() =
        let initialItems = [|
            MenuItem("Home", "#", div())
        |]
        let (items, setItems) = createSignal(initialItems)
        let (item, addItem) = createSignal<MenuItem>(Unchecked.defaultof<_>)
        div() {
            MyItemizer(items = items())
            Imported()
            Match()
        }
