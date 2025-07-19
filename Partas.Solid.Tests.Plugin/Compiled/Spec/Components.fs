namespace Partas.Solid.Tests.Spec

open Partas.Solid
open Fable.Core
open Browser.Dom

module Section1 =
    /// Let bindings compile to functions
    [<SolidComponent>]
    let Component () = div ()

    /// SolidComponent functions can be implemented within
    /// normal functions.
    let WrappingComponent () =
        Component ()

    /// However native tags or SolidTypeComponents will not
    let FailsToWrap () =
        div () { Component () }

    /// Reactivity would not work outside a function
    [<SolidComponent>]
    let NotAFunction = Component ()

    [<SolidComponent>]
    let WrapNotAFunction () =
        NotAFunction

    /// Parameters can be passed
    [<SolidComponent>]
    let ParameterComponent someInt someString someHandler =
        let thisInt = someInt
        let thisString = someString
        let thisHandler = someHandler
        div ()

    /// And since they are functions without special JSX compilation,
    /// can be used outside a SolidComponent or SolidTypeComponent
    let WrapParameterComponent () =
        ParameterComponent 5 "HeyThere" (fun () -> ())

    /// Native components use constructor parameters for attributes
    [<SolidComponent>]
    let NativeComponentAttributes () =
        div (id = "MyDiv", tabindex = 5, onClick = (fun _ -> console.log "Clicked"))

    /// Reserved keywords are apostrophised, but compile correctly
    [<SolidComponent>]
    let ApostrophisedAttribute () =
        div (class' = "myclass")

    /// Native components use computation expression syntax to receive children
    [<SolidComponent>]
    let NativeComponentChildren () =
        div (class' = "group") {
            button (class' = "bg_black text-white") { "Button" }
            div () { select () }
        }

    /// New components can be typed and used to interop with libraries
    /// And they will compile correctly in a solid component
    /// But they will not generate a function (since they are an import)
    [<Import("LibraryImport", "@some/library")>]
    type FakeImport() =
        interface RegularNode

        [<Erase>]
        member val myAttribute: string = "" with get, set

    /// New components can be typed and used to interop with libraries
    /// And they will compile correctly in a solid component
    [<SolidComponent>]
    let ComponentTypeWrap () =
        div () { FakeImport (myAttribute = "Something") }

    /// But not outside of a solid component
    let FailComponentTypeWrap () =
        div () { FakeImport (myAttribute = "something") }

namespace Banana

open Partas.Solid
open Fable.Core

/// However, the components will not compile correctly
/// if defined outside of a namespace starting with Partas.Solid
[<Import("LibraryImport", "@some/library")>]
type FakeImport() =
    interface RegularNode

    [<Erase>]
    member val myAttribute: string = "" with get, set

module Section2 =
    /// However, the components will not compile correctly
    /// if defined outside of a namespace starting with Partas.Solid
    let Component () =
        FakeImport (myAttribute = "something")

    /// Regardless of with/without the SolidComponent attribute
    [<SolidComponent>]
    let SolidCmp () =
        FakeImport (myAttribute = "seomthing")
