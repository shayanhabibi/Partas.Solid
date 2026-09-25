module Partas.Solid.Tests.Runtime.Dom.Portals.WebApi

open Partas.Solid
open Partas.Solid.Web
open Fable.Core
open Fable.Core.JsInterop

/// Build-time flags, read through the bindings.
let flags () = {| isServer = isServer; isDev = isDev |}

[<SolidComponent>]
let Hello () = p (class' = "hello") { "hello from render" }

/// `render` from the bindings, mounting a component into a node; returns the dispose function.
let renderHello (el: Browser.Types.Element) : unit -> unit =
    let d = render ((fun () -> Hello()), el)
    fun () -> d ()

/// A component that guards browser-only work on isServer.
[<SolidComponent>]
let ClientOnlyBranch () =
    span (id = "branch") { if isServer then "server" else "client" }

/// useHead with a constant title tag.
[<SolidComponent>]
let TitleSetter () =
    useHead (HeadTag(HeadTag.Tag.Title, createObj [ "children", box "Portals page" ]))
    div (class' = "title-setter") { "titled" }

/// useHead with a reactive tag accessor driven by a signal.
[<SolidComponent>]
let ReactiveTitle () =
    let count, setCount = createSignal 0
    useHead (fun () -> HeadTag(HeadTag.Tag.Title, createObj [ "children", box ("Count " + string (count ())) ]))
    button (id = "bump", onClick = fun _ -> setCount (count () + 1)) { count () }

/// useHead with a meta description.
[<SolidComponent>]
let MetaSetter () =
    useHead (HeadTag(HeadTag.Tag.Meta, createObj [ "name", box "description"; "content", box "portal tests" ]))
    div (class' = "meta-setter") { "meta" }

/// useHead with a group (array) of tags.
[<SolidComponent>]
let HeadGroup () =
    useHead [|
        HeadTag(HeadTag.Tag.Title, createObj [ "children", box "Grouped" ])
        HeadTag(HeadTag.Tag.Meta, createObj [ "name", box "keywords"; "content", box "a,b" ])
    |]
    div (class' = "head-group") { "group" }
