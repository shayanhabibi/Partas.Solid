module Partas.Solid.Tests.Runtime.Dom.Elements.GeneratedTags

open Partas.Solid
open Fable.Core

/// <slot>, added by generating Tags.fs from Solid's tag maps.
[<SolidComponent>]
let NewSlot () =
    div (id = "new-slot") { slot (id = "slot", name = "header") }

/// <template>. Solid's compiler rejects a template with static children as malformed: the browser moves them into
/// `.content`, so the rendered childNodes never match the JSX.
[<SolidComponent>]
let NewTemplate () =
    div (id = "new-template") { template (id = "tpl", shadowrootmode = Attr.Shadowrootmode.Open) }

/// <selectedcontent>. It belongs in a customizable select's button (the snapshot case pins that shape), but jsdom's
/// parser predates customizable selects and drops a button inside a select, so it renders standalone here.
[<SolidComponent>]
let NewSelectedContent () =
    div (id = "sc-host") { selectedcontent (id = "sc") }

/// Elements whose generated declarations fixed their attributes or void-ness.
[<SolidComponent>]
let FixedElements () =
    div (id = "fixed") {
        base' (id = "base", href = "/app/", target = "_self")
        meta (id = "meta", name = "description", content = "generated", ``http-equiv`` = Attr.HttpEquiv.ContentSecurityPolicy)
        video (id = "vid") { track (id = "trk", kind = Attr.Kind.Subtitles, src = "en.vtt", srclang = "en", default' = true) }
        p (id = "wrap") {
            "long"
            wbr ()
            "word"
        }
        bdo (id = "bdo", dir = Attr.Dir.Rtl) { "abc" }
        table () { caption (id = "cap") { "Scores" } }
    }

/// MathML elements, behind the MathML module.
[<SolidComponent>]
let Formula () =
    MathML.math (id = "math", display = Attr.Display.Block) {
        MathML.mrow () {
            MathML.mi () { "x" }
            MathML.mo () { "=" }
            MathML.mfrac (id = "frac", linethickness = "2px") {
                MathML.mn () { "1" }
                MathML.mn () { "2" }
            }
        }
        MathML.``annotation-xml`` (id = "ann", encoding = "MathML-Content") { MathML.mi () { "y" } }
    }
