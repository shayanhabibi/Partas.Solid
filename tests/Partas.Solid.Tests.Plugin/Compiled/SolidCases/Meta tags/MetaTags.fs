module Partas.Solid.Tests.SolidCases.MetaTags

open Partas.Solid
open Partas.Solid.Meta
open Fable.Core

/// @solidjs/meta 1.0 tags, keyed, grouped by Head.
[<SolidComponent>]
let PageHead () =
    Head () {
        Title (key = "title") { "Page" }
        Meta (key = "description", name = "description", content = "A page")
        Link (rel = "icon", href = "/favicon.ico")
        Stylesheet (href = "/site.css")
        Script (src = "/analytics.js", async = true)
        Base (href = "/app/")
    }
