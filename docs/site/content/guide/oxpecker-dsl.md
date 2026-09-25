---
title: Oxpecker DSL
---

Partas.Solid is a fork of Oxpecker.Solid and keeps its DSL. Attributes go in a constructor call, children go in a
computation expression after it:

```fsharp
tag (attribute = value, ...) {
    child
    child
}
```

:::note
The DSL makes attributes easy to tell apart from children, and it reads much like XML, with a clear start and end to
each element.
:::

A common objection is that attributes are hard to build up programmatically, for example from a list. That does not
apply to Partas.Solid: you can spread any object into an element's attributes with
[`.spread`](extension-methods.md), so you can build attributes however you like and pass them to native elements.

## Elements

Each HTML element is a class. Its attributes are optional constructor parameters.

```fsharp
[<SolidComponent>]
let Example () =
    button (onClick = fun _ -> Browser.Dom.console.log "Clicked!")
```

Attribute names that are F# keywords get an apostrophe: `class'`, `type'`, `for'`, `when'`.

```fsharp solid jsx
button (class' = "btn", type' = "button", title = "A plain button") { "Button" }
```

## Children

The computation expression after an element holds its children. You can put strings, integers, floats and other
elements in it, in any mix.

```fsharp solid
div () {
    "Button label"
    button () { "Button" }
    span () { "Count:" }
    5
}
```

An element with no children does not need the braces: `input (type' = "checkbox")`. Void elements such as `input`,
`br` and `img` do not accept any.

Ordinary F# works around and inside the block. An `if` becomes a ternary in the JSX, and an interpolated string becomes
a template literal:

```fsharp solid jsx
let name = "Solid"

div () {
    p () { $"Hello, {name}!" }
    if name.Length > 3 then
        p () { "That is a long name." }
}
```

:::warning
String literals in a children block are currently written into the JSX as raw text, unescaped. That is a known bug.
Characters JSX treats as markup, such as `<`, `>`, `{` and `}`, are read as JSX rather than shown, and a space at the
start or end of a string next to an element can be lost. Avoid those characters in literal children for now, and put
spacing in CSS rather than in the text.
:::

## Embedded JSX

You can embed raw JSX through a small helper:

```fsharp
let inline toHtmlElement (func: string -> JSX.Element) (value: string) : HtmlElement =
    unbox (func value)
```

```fsharp
div () {
    toHtmlElement JSX.jsx "<button>Button</button>"
}
```

:::note
The F# compiler treats `JSX.Element` as a type that collides with `HtmlElement`, so the builder cannot accept both
`#HtmlElement` and `JSX.Element`. Until that is resolved, convert with a helper as above.
:::

## Lambda children

Some components take a function as their child instead of elements. `For` is one: it calls the function for each item
to build that item's DOM.

:::warning
To pass a function as a child, you must `yield` it. If your IDE cannot infer the function's parameter types, you have
probably left out the `yield`.
:::

```fsharp solid
ul () {
    For.Keyed(each = [| "one"; "two"; "three" |]) {
        yield fun item _ -> li () { $"Item {item}" }
    }
}
```

:::tip
If you write bindings for a library component that takes a function child, implement one of the `ChildLambdaProvider`
interfaces on its type. The builder then accepts a yielded function with the right parameter types. See
[tag interfaces](tag-interfaces.md).
:::

## Extension methods

Extension methods cover what constructor parameters cannot: spreading an object into the attributes, adding custom
attributes or `data-*` attributes, style and class objects, and refs.

```fsharp solid jsx
div(style = "padding: .5em; border: 1px dashed gray").data("state", "ready").attr("aria-live", "polite") {
    "This div has data-state and aria-live attributes."
}
```

See [extension methods](extension-methods.md) for the full list.
