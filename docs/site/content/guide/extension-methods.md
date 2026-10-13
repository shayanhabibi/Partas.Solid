---
title: Extension Methods
---

Extension methods cover what does not fit a typed property: arbitrary attributes, `data-*`, refs, style objects and
spreads. Chain them after the constructor, before the children:

```fsharp
div(class' = "card").data("id", "42").attr("custom-attr", "yes") { "..." }
```

Write the call without a space between the constructor and the dot, as above. With a space, as in
`div (class' = "card").data(...)`, F# applies `.data` to the argument list instead of the tag.

| Method | On | Emits |
| --- | --- | --- |
| `.attr(name: string, value: obj)` | `#HtmlTag` | `name={value}` |
| `.data(name: string, value: string)` | `#HtmlTag` | `data-name={value}` |
| `.bool(name: string, value: bool)` | `#HtmlTag` | `name={value}` |
| `.ref(...)` | tags | `ref={...}` |
| `.style'(obj)` or `.style'((string * obj) list)` | `#HtmlTag` | `style={...}` |
| `.classObj(obj)` | `#HtmlTag` | `class={...}` |
| `.spread(value: obj)` | `#HtmlElement` | `{...value}` |

Solid 2 dropped `on:`, `prop:` and `use:`, so `.on`, `.prop` and `.use'` are gone, and `.classList` is now
[`.classObj`](#classobj). See [Migrating to Solid 2](migrating-to-solid-2.md#extension-methods).

## attr, data and bool

`.attr` sets any attribute by name, including names F# does not allow as identifiers. `.data` sets a `data-*`
attribute; `name` is the part after `data-`. `.bool` adds the attribute when `true` and removes it when `false`.

```fsharp solid jsx
div(style = "font-family: monospace").data("user-id", "42").attr("custom-attr", "yes").bool("itemscope", true) {
    "Inspect this element to see its attributes."
}
```

## ref

Gives you the DOM element once Solid has created it. `.ref` takes a variable, an option, a callback, an array of
callbacks, a `Ref` or an array of `Ref`s.

```fsharp solid render=FocusDemo jsx
open Fable.Core.TS
[<SolidComponent>]
let FocusDemo () =
    let mutable inputRef: Dom.HTMLInputElement = JS.undefined

    div () {
        input(placeholder = "Filled by the button").ref (inputRef)
        button (onClick = fun _ -> inputRef.value <- "filled via ref"; inputRef.focus ()) { "Fill" }
    }
```

A callback works too: `div().ref (fun (el: Dom.HTMLDivElement) -> JS.console.log el)`. For HTML tags, the element type follows from the tag: `input().ref` takes an `HTMLInputElement`, `div().ref` an
`HTMLDivElement`. If you have a more general type, cast it: `div().ref (myRef :?> _)`.

The `Ref<'T>` union describes every shape a ref can take, so a component can take a ref prop and pass it on:

```fsharp
type Ref<'DomType> =
    | Singleton of 'DomType option
    | Callback of ('DomType -> unit)
    | Array of Ref<'DomType> array
```

`Ref.cast` converts a `Ref` to another element type.

### Refs and DOM libraries

The tooltip below drives [Floating UI](https://floating-ui.com/) from npm through refs. Bind each export with
`[<Import>]` and tupled parameters, so Fable emits a plain `f(a, b, c)` call:

```fsharp solid show=code
[<Import("computePosition", "@floating-ui/dom")>]
let computePosition (reference: obj, floating: obj, options: obj): JS.Promise<obj> = jsNative

[<Import("autoUpdate", "@floating-ui/dom")>]
let autoUpdate (reference: obj, floating: obj, update: unit -> unit): (unit -> unit) = jsNative

[<Import("offset", "@floating-ui/dom")>]
let offset (distance: int): obj = jsNative

[<Import("flip", "@floating-ui/dom")>]
let flip (options: obj): obj = jsNative

[<Import("shift", "@floating-ui/dom")>]
let shift (options: obj): obj = jsNative
```

Both refs are set during render, so `onSettled` can pass the two elements to `autoUpdate`. `autoUpdate` returns a
stop function, and returning that from `onSettled` runs it when the component is disposed. Anonymous records compile to
plain option objects.

```fsharp solid render=TooltipDemo jsx
open Fable.Core.TS
[<SolidComponent>]
let TooltipDemo () =
    let mutable anchor: Dom.HTMLButtonElement = JS.undefined
    let mutable tip: Dom.HTMLDivElement = JS.undefined

    let place () =
        // padding keeps the tooltip clear of the sticky navbar
        let middleware = [| offset 10; flip {| padding = 64 |}; shift {| padding = 8 |} |]
        computePosition(anchor, tip, {| placement = "top"; middleware = middleware |})
            .``then``(fun pos ->
                tip?style?left <- $"{pos?x}px"
                tip?style?top <- $"{pos?y}px")
        |> ignore

    onSettled (fun () -> autoUpdate (anchor, tip, place))

    let show visible = tip?style?opacity <- (if visible then "1" else "0")

    let tipStyle =
        "position: absolute; top: 0; left: 0; width: max-content; opacity: 0; "
        + "transition: opacity .15s; pointer-events: none; padding: .375rem .625rem; "
        + "border-radius: var(--nacara-radius-sm); font-size: .8125rem; "
        + "background: var(--nacara-heading); color: var(--nacara-bg)"

    div (style = "display: flex; justify-content: center; padding: 3rem 0 1rem") {
        button(
            class' = "p-btn p-btn--secondary",
            onMouseEnter = (fun _ -> show true),
            onMouseLeave = (fun _ -> show false),
            onFocus = (fun _ -> show true),
            onBlur = (fun _ -> show false)
        ).attr("aria-describedby", "floating-tip").ref (anchor) { "Hover or focus me" }

        div(style = tipStyle).attr("id", "floating-tip").attr("role", "tooltip").ref (tip) {
            "Placed by Floating UI"
        }
    }
```

Scroll until the button nears the top of the window. `flip` moves the tooltip below it, and `autoUpdate`
keeps it attached while the page moves.

## style'

Sets the `style` attribute from an object, or from a list of name and value pairs. The `Style` module in
`Partas.Solid.Style` has typed helpers for the pairs.

```fsharp solid render=StyledBox jsx
open Partas.Solid.Style

[<SolidComponent>]
let StyledBox () =
    div().style' [ Style.backgroundColor Color.Lavender; "padding" ==> "1em"; "--accent" ==> "navy" ] {
        span(style = "color: var(--accent)") { "Styled with a list, including a custom property." }
    }
```

A plain string works as well, through the `style` property: `div (style = "color: blue")`.

## classObj

In Solid 2, `class` takes a string, an object map, or an array. The object map form replaces the old `classList`: each
key whose value is true is added. The `class'` property takes a string, so pass a map with `.classObj`:

```fsharp solid jsx
let isOpen, setOpen = createSignal true

div () {
    button (onClick = fun _ -> setOpen (not (isOpen ()))) { "Toggle" }
    span().classObj({| ``is-open`` = isOpen (); closed = not (isOpen ()) |}) {
        "Inspect my class"
    }
}
```

## spread

Spreads the properties of an object onto the element.

```fsharp solid render=SpreadLocal jsx
[<SolidComponent>]
let SpreadLocal () =
    let extra = createObj [ "id", box "spread-demo"; "title", box "from object"; "data-x", box "1" ]
    div(style = "text-decoration: underline dotted").spread (extra) { "Hover me" }
```

In a [SolidTypeComponent](solid-type-attribute.md), `.spread props` spreads the props the body does not read (the
`PARTAS_OTHERS` binding), passing unused attributes on to the rendered element:

```fsharp solid jsx
[<Erase>]
type CenteredButton() =
    inherit button()

    [<SolidTypeComponent>]
    member props.View =
        props.class' <- "centered"
        div (style = "display: flex; justify-content: center") {
            button(class' = props.class').spread props { props.children }
        }
```

```fsharp solid
CenteredButton(title = "title came through the spread") { "Hover me" }
```

A prop that the body reads is left out of the spread: the JSX tab shows `class` in the `omit` call. So the button
sets `class' = props.class'` itself; with only `.spread props` it would get no class at all. Setting a default does
not count as a read: `props.class' <- "centered"` only adds `class` to the `merge` call.

To spread every prop, reads included, see [SkipOmit and SpreadProps](attribute-flags.md#skipomit-and-spreadprops).
