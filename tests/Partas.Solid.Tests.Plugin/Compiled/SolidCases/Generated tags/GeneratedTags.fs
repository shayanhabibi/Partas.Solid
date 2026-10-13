module Partas.Solid.Tests.SolidCases.GeneratedTags

open Partas.Solid
open Fable.Core

/// Elements added by generating Tags.fs from Solid's tag maps.
[<SolidComponent>]
let NewElements () =
    div () {
        slot (name = "header")
        template (shadowrootmode = Attr.Shadowrootmode.Open, shadowrootdelegatesfocus = true)
        select () {
            button () { selectedcontent () }
            option' (value = "a") { "A" }
        }
    }

/// Elements whose generated declarations fixed their attributes or void-ness.
[<SolidComponent>]
let FixedElements () =
    div () {
        base' (href = "/app/", target = "_self")
        meta (name = "description", content = "generated", ``http-equiv`` = Attr.HttpEquiv.ContentSecurityPolicy)
        video () { track (kind = Attr.Kind.Subtitles, src = "en.vtt", srclang = "en", default' = true) }
        p () {
            "long"
            wbr ()
            "word"
        }
        bdo (dir = Attr.Dir.Rtl) { "abc" }
        table () { caption () { "Scores" } }
    }

/// MathML elements, behind the MathML module.
[<SolidComponent>]
let Formula () =
    MathML.math (display = Attr.Display.Block) {
        MathML.mrow () {
            MathML.mi () { "x" }
            MathML.mo () { "=" }
            MathML.mfrac (linethickness = "2px") {
                MathML.mn () { "1" }
                MathML.mn () { "2" }
            }
        }
    }

/// A hyphenated MathML element name.
[<SolidComponent>]
let Annotated () =
    MathML.semantics () {
        MathML.mi () { "y" }
        MathML.``annotation-xml`` (encoding = "MathML-Content") { MathML.mi () { "y" } }
    }
