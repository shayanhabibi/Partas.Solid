---
title: Extension Methods
---

Tags have extension methods for the attributes that do not fit a typed property: arbitrary attributes, `data-*`,
refs, style objects and spreads. You chain them after the constructor, before the children:

```fsharp
div(class' = "card").data("id", "42").attr("custom-attr", "yes") { "..." }
```

Write the call without a space between the constructor and the dot, as above. With a space, as in
`div (class' = "card").data(...)`, F# applies `.data` to the argument list instead of the tag.

| Method | On | Emits |
| --- | --- | --- |
| `.attr(name, value)` | `#HtmlTag` | `name={value}` |
| `.data(name, value)` | `#HtmlTag` | `data-name={value}` |
| `.bool(name, value)` | `#HtmlTag` | `name={value}` |
| `.ref(...)` | tags | `ref={...}` |
| `.style'(...)` | `#HtmlTag` | `style={...}` |
| `.class'(...)` | `#HtmlTag` | `class={...}` |
| `.spread(value)` | `#HtmlElement` | `{...value}` |

## Changes in 3.0

| 2.x on Solid 1.9 | 3.0 on Solid 2 |
| --- | --- |
| `.on(event, handler)` | Removed. Solid 2 dropped the `on:` namespace. Use the typed `on*` event properties. |
| `.prop(name, value)` | Removed, with no replacement binding yet. |
| `.use'(directive)` | Removed. Solid 2 dropped `use:` directives. Pass the directive to `.ref` as a callback. |
| `.classList(obj)` | Renamed to `.class'(obj)`, because Solid 2's `class` takes an object map. On HTML tags the extension is hidden by the `class'` property; see `class'` below. |
| `.bool` emitted `bool:name` | `.bool` emits the plain attribute name. |
| `.spread` was on `#HtmlTag` | `.spread` is on `#HtmlElement`, so components that only implement `HtmlElement` have it too. |

See [Migrating to Solid 2](migrating-to-solid-2.md) for the rest of the changes.

## attr

Sets any attribute by name. Use it for attributes that have no typed property, including custom attributes and
attributes with characters F# does not allow in a name.

```fsharp
attr(name: string, value: obj)
```

## data

Sets a `data-*` attribute. `name` is the part after `data-`.

```fsharp
data(name: string, value: string)
```

## bool

Sets a boolean attribute by name. `true` adds the attribute and `false` removes it.

```fsharp
bool(name: string, value: bool)
```

```fsharp solid jsx
div(style = "font-family: monospace").data("user-id", "42").attr("custom-attr", "yes").bool("itemscope", true) {
    "Inspect this element to see its attributes."
}
```

## ref

Gives you the DOM element once Solid has created it. `.ref` takes a callback, a variable, an option, an array of
callbacks, or a `Ref`. Solid calls a callback after it has applied the element's attributes and children.

```fsharp solid render=FocusDemo jsx
[<SolidComponent>]
let FocusDemo () =
    let mutable inputRef: Browser.Types.HTMLInputElement = JS.undefined

    div () {
        input(placeholder = "Filled by the button").ref (inputRef)
        button (onClick = fun _ -> inputRef.value <- "filled via ref"; inputRef.focus ()) { "Fill" }
    }
```

The same with a callback:

```fsharp
div().ref (fun (el: Browser.Types.HTMLDivElement) -> JS.console.log el) { "..." }
```

For HTML tags, the element type follows from the tag: `input().ref` takes an `HTMLInputElement`, `div().ref` an
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

Libraries that work on real elements need refs. The tooltip below uses
[Floating UI](https://floating-ui.com/) from npm. Bind each export with `[<Import>]` and tupled parameters, so
Fable emits a plain `f(a, b, c)` call:

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

Both refs are set during render, so `onSettled` can pass the two elements to `autoUpdate`. It returns its own
stop function, and returning that from `onSettled` runs it when the component is disposed. Options are
anonymous records, which compile to plain objects.

```fsharp solid render=TooltipDemo jsx
[<SolidComponent>]
let TooltipDemo () =
    let mutable anchor: Browser.Types.HTMLButtonElement = JS.undefined
    let mutable tip: Browser.Types.HTMLDivElement = JS.undefined

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

```fsharp
style'(styleObj: obj)
style'(styles: (string * obj) list)
```

```fsharp solid render=StyledBox jsx
open Partas.Solid.Style

[<SolidComponent>]
let StyledBox () =
    div().style' [ Style.backgroundColor Color.Lavender; "padding" ==> "1em"; "--accent" ==> "navy" ] {
        span(style = "color: var(--accent)") { "Styled with a list, including a custom property." }
    }
```

A plain string works as well, through the `style` property: `div (style = "color: blue")`.

## class'

In Solid 2, `class` takes a string, an object map, or an array. The object map form replaces the old `classList`: each
key whose value is true is added.

```fsharp
class'(classListObj: obj)
```

:::warning
On HTML tags, the typed `class'` string property hides this extension, so `div().class'(obj)` does not compile.
:::

Pass the object through the property instead, with `!!` to convert it:

```fsharp solid
let isOpen, setOpen = createSignal true

div () {
    button (onClick = fun _ -> setOpen (not (isOpen ()))) { "Toggle" }
    span (class' = !!(createObj [ "is-open", box (isOpen ()); "closed", box (not (isOpen ())) ])) {
        "Inspect my class"
    }
}
```

## spread

Spreads the properties of an object onto the element.

```fsharp
spread(value: obj)
```

```fsharp solid render=SpreadLocal jsx
[<SolidComponent>]
let SpreadLocal () =
    let extra = createObj [ "id", box "spread-demo"; "title", box "from object"; "data-x", box "1" ]
    div(style = "text-decoration: underline dotted").spread (extra) { "Hover me" }
```

In a [SolidTypeComponent](solid-type-attribute.md), `.spread props` spreads the props the component does not read
itself, the `PARTAS_OTHERS` binding. This is how a component passes the attributes it does not use on to the element
it renders:

```fsharp
namespace Partas.Solid.MyCenteredButton

open Partas.Solid

[<Erase>]
type MyCenteredButton() =
    inherit button()

    [<SolidTypeComponent>]
    member props.View =
        props.class' <- "centered"
        div (style = "display: flex; justify-content: center") {
            button(class' = props.class').spread props { props.children }
        }
```

:::note
A prop that the body reads is left out of the spread. Above, the body reads `props.class'` for the button, so the
spread does not carry `class`, and the button has to set `class' = props.class'` itself. If another element read
`props.class'` and the button only had `.spread props`, the button would get no class at all. Setting a default does
not count as a read: `props.class' <- "centered"` only adds `class` to the `merge` call.
:::

To spread every prop, reads included, see `SpreadProps` in [Attribute Flags](attribute-flags.md).

## OnHandler

`OnHandler` is a record with `handleEvent`, `once`, `passive` and `capture` fields. It was the argument of the removed
`.on` method, and nothing takes it now. To pass listener options in Solid 2, attach the listener yourself in a `.ref`
callback with `addEventListener`.
