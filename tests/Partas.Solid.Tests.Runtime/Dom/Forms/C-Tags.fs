module Partas.Solid.Tests.Runtime.Dom.Forms.Tags

open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop

/// A table with caption, colgroup, thead/tbody/tfoot, colspan/rowspan (both spellings), th scope.
[<SolidComponent>]
let Table () =
    table (id = "tbl") {
        caption () { "Totals" }
        colgroup () {
            col (span = 2, class' = "c-a")
            col (class' = "c-b")
        }
        thead () {
            tr () {
                th (scope = "col", id = "h-name") { "Name" }
                th (scope = "col", colspan = 2) { "Scores" }
            }
        }
        tbody () {
            tr () {
                td (id = "rs", rowspan = 2, headers = "h-name") { "Ann" }
                td () { "1" }
                td () { "2" }
            }
            tr () {
                td () { "3" }
                td () { "4" }
            }
        }
        tfoot () {
            tr () {
                td (id = "cs", colSpan = 3) { "sum 10" }
            }
        }
    }

/// rowSpan (camelCase) on th.
[<SolidComponent>]
let CamelRowSpan () =
    table () {
        tbody () {
            tr () {
                th (id = "th-rs", rowSpan = 2) { "row" }
                td () { "a" }
            }
            tr () { td () { "b" } }
        }
    }

/// Dynamic colspan driven by a signal.
[<SolidComponent>]
let DynamicColspan () =
    let span', setSpan = createSignal 1

    div (class' = "dc") {
        table () {
            tbody () {
                tr () {
                    td (id = "dc-td", colspan = span' ()) { "wide" }
                }
            }
        }
        button (id = "dc-grow", onClick = fun _ -> setSpan (span' () + 1)) { "grow" }
    }

/// details/summary with a static open and a signal-driven open, plus onToggle.
[<Erase>]
type Disclosure() =
    inherit div()

    [<Erase>]
    member val log: string -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        let isOpen, setOpen = createSignal false

        div (class' = "disc") {
            details (id = "d-static", open' = true) {
                summary () { "always" }
                p () { "body" }
            }
            details (id = "d-closed", open' = false) {
                summary () { "closed" }
                p () { "hidden body" }
            }
            details(id = "d-dyn", open' = isOpen ()).attr ("onToggle", fun (_: obj) -> props.log "toggle") {
                summary () { "dynamic" }
                p () { "content" }
            }
            button (id = "d-btn", onClick = fun _ -> setOpen (not (isOpen ()))) { "flip" }
        }

/// dialog open attribute driven by a signal.
[<SolidComponent>]
let DialogOpen () =
    let shown, setShown = createSignal false

    div (class' = "dlg") {
        dialog (id = "dlg-el", open' = shown ()) {
            p () { "in dialog" }
            form (method = "dialog") { button (id = "dlg-ok", value = "ok") { "OK" } }
        }
        button (id = "dlg-show", onClick = fun _ -> setShown true) { "show" }
        button (id = "dlg-hide", onClick = fun _ -> setShown false) { "hide" }
    }

/// Common attributes of img / a / iframe / media.
[<SolidComponent>]
let Media () =
    div (class' = "media") {
        img (id = "m-img", src = "/a.png", alt = "", width = 64, height = 32, loading = "lazy", decoding = "async", srcset = "/a.png 1x, /a@2x.png 2x", sizes = "64px", crossorigin = "anonymous", referrerpolicy = "no-referrer")
        a (id = "m-a", href = "/file.pdf", download = "report.pdf", hreflang = "en", type' = "application/pdf", ping = "/ping") { "download" }
        iframe (id = "m-if", src = "about:blank", title = "frame", width = 300, height = 150, loading = "lazy", sandbox = "allow-scripts", allow = "fullscreen", name = "fr")
        video (id = "m-vid", src = "/v.mp4", poster = "/p.png", width = 320, controls = true, muted = true, loop = true, playsinline = true, preload = "none")
        audio (id = "m-aud", src = "/a.mp3", controls = false, autoplay = false)
    }

/// Void elements render without children or closing tags and keep their attributes.
[<SolidComponent>]
let Voids () =
    div (id = "voids") {
        "a"
        br ()
        "b"
        hr (class' = "rule")
        wbr ()
        img (src = "/x.png", alt = "x")
        input (value = "v")
        embed (src = "/e.swf", type' = "application/x-shockwave-flash")
        source (src = "/s.webm", type' = "video/webm")
        area (shape = "rect", coords = "0,0,1,1", href = "#a", alt = "area")
        meta ()
        link (rel = "stylesheet", href = "/s.css")
    }

/// The ol reversed/start attributes, meter/progress values, time datetime.
[<SolidComponent>]
let Misc () =
    div (class' = "misc") {
        ol (id = "ol", start = "3", reversed = "") {
            li () { "x" }
            li () { "y" }
        }
        meter (id = "met", min = 0, max = 10, low = 2, high = 8, optimum = 5, value = 7) { "7" }
        progress (id = "prog", max = 100, value = "40") { "40%" }
        time (id = "tm", dateTime = "2024-03-15") { "March 15" }
    }

/// Boolean attributes set false on less-common tags must be removed, not stringified.
[<SolidComponent>]
let BoolsFalseMisc () =
    div (id = "bfm") {
        details (id = "bf-details", open' = false) { summary () { "s" } }
        dialog (id = "bf-dialog", open' = false) { "d" }
        select (id = "bf-select", multiple = false, disabled = false, required = false) { option' (value = "a") { "a" } }
        option' (id = "bf-option", disabled = false) { "o" }
        video (id = "bf-video", controls = false, loop = false, autoplay = false)
        form (id = "bf-form", noValidate = false) { "f" }
        fieldset (id = "bf-fs", disabled = false) { "fs" }
        textarea (id = "bf-ta", required = false, readonly = false)
    }

/// dialog close/cancel handlers.
[<Erase>]
type DialogEvents() =
    inherit div()

    [<Erase>]
    member val log: string -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        dialog (id = "de", open' = true, onClose = (fun _ -> props.log "close"), onCancel = fun _ -> props.log "cancel") { "x" }

/// track default (bound as string) and ol reversed (bound as string).
[<SolidComponent>]
let StringTypedBools () =
    div (class' = "stb") {
        video () { track (id = "trk", kind = "subtitles", src = "/t.vtt", srclang = "en", label = "English", default' = "") }
        iframe (id = "ifs", allowfullscreen = "")
    }
