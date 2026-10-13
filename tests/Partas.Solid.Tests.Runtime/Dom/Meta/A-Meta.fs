module Partas.Solid.Tests.Runtime.Dom.Meta.MetaTags

open Partas.Solid
open Partas.Solid.Meta
open Fable.Core

/// Head tags from @solidjs/meta 1.0: no provider, rendered straight into document.head.
[<SolidComponent>]
let PageHead () =
    Head () {
        Title (key = "title") { "Page" }
        Meta (key = "description", name = "description", content = "A page")
        Base (href = "/app/")
    }

/// A stylesheet: Solid 2 warms it with a preload and holds the update until it loads.
[<SolidComponent>]
let SiteStylesheet () = Stylesheet (href = "/site.css")

/// A keyed title, and a second keyed title that replaces it while shown.
[<Erase>]
type Layered() =
    inherit div()

    [<Erase>]
    member val inner: bool = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div () {
            Title (key = "title") { "Outer" }
            Show (when' = props.inner) { Head () { Title (key = "title") { "Inner" } } }
        }
