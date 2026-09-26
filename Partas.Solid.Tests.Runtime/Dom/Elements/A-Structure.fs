module Partas.Solid.Tests.Runtime.Dom.Elements.Structure

open Partas.Solid
open Fable.Core

/// Nested tags with mixed text and element children.
[<SolidComponent>]
let Nested () =
    article (id = "post") {
        h1 () { "Title" }
        p (class' = "lead") {
            "Hello "
            strong () { "world" }
            "!"
        }
        ul () {
            li () { "one" }
            li () { "two" }
            li () { "three" }
        }
    }

/// Numeric children (int and float) render as text.
[<SolidComponent>]
let Numbers () =
    div (class' = "nums") {
        span (class' = "i") { 42 }
        span (class' = "f") { 3.5 }
        span (class' = "neg") { -7 }
    }

/// Interpolated strings computed at render time.
[<SolidComponent>]
let Interpolated () =
    let name = "Partas"
    let n = 3
    p (class' = "interp") { $"Hi {name}, you have {n} items" }

/// A fragment root yields sibling nodes without a wrapper element.
[<SolidComponent>]
let Frag () =
    Fragment () {
        span (class' = "a") { "A" }
        span (class' = "b") { "B" }
        "tail"
    }

/// Void elements and a childless element.
[<SolidComponent>]
let Voids () =
    div (class' = "voids") {
        input ()
        br ()
        hr ()
        img ()
        div (class' = "empty")
    }

/// Text with HTML-significant characters must be escaped (text node, not markup).
[<SolidComponent>]
let EscapedText () =
    div (class' = "esc") { "<b>not bold</b> & \"quoted\"" }

/// Deep nesting of tags.
[<SolidComponent>]
let Deep () =
    section () {
        div (class' = "l1") {
            div (class' = "l2") {
                div (class' = "l3") {
                    div (class' = "l4") { span () { "deep" } }
                }
            }
        }
    }

/// A table with implicit-tbody-sensitive structure.
[<SolidComponent>]
let Table () =
    table () {
        thead () { tr () { th () { "k" }; th () { "v" } } }
        tbody () {
            tr () {
                td () { "a" }
                td () { "1" }
            }
            tr () {
                td () { "b" }
                td () { "2" }
            }
        }
    }

/// A type component whose text children come from props.
[<Erase>]
type Greeting() =
    inherit div()

    [<Erase>]
    member val name: string = unbox null with get, set

    [<Erase>]
    member val count: int = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        div (class' = "greeting") {
            span (class' = "name") { props.name }
            span (class' = "count") { props.count }
        }

/// Component composition: a type component used as a child tag, with props.
[<SolidComponent>]
let Composed () =
    main () {
        Greeting (name = "Ann", count = 1)
        Greeting (name = "Bob", count = 2)
    }

/// Text containing JSX-significant braces.
[<SolidComponent>]
let BraceText () =
    div (class' = "brace") { "a {1 + 1} b" }

/// Whitespace between inline text and an element on the next line.
[<SolidComponent>]
let InlineSpacing () =
    p (class' = "spacing") {
        "Hello "
        strong () { "world" }
        " again"
    }

/// Markup characters inside an interpolated string (a JS expression, not JSX text).
[<SolidComponent>]
let InterpolatedMarkup () =
    let who = "you"
    div (class' = "imk") { $"<i>{who}</i> & {{x}}" }

/// A string child with an embedded newline and indentation, inside <pre>.
[<SolidComponent>]
let MultilineText () =
    pre (class' = "ml") { "line one\n  line two" }
