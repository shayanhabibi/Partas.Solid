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

Attributes are easy to tell apart from children, and each element has a clear start and end, much like XML. To build
attributes programmatically, for example from a list, spread an object onto the element with
[`.spread`](extension-methods.md#spread).

## Elements

Each HTML element is a class, and its attributes are optional constructor parameters. Attribute names that are F#
keywords get an apostrophe: `class'`, `type'`, `for'`, `when'`.

```fsharp solid jsx
button (class' = "btn", type' = "button", title = "A plain button") { "Button" }
```

## Children

The computation expression after an element holds its children: strings, numbers and other elements, in any mix.

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

Ordinary F# works around and inside the block. An `if` becomes a ternary in the JSX:

```fsharp solid jsx
let name = "Solid"

div () {
    p () { $"Hello, {name}!" }
    if name.Length > 3 then
        p () { "That is a long name." }
}
```

## Embedded JSX

Embed raw JSX (Fable's `JSX.jsx`) through a small helper:

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
`JSX.Element` collides with `HtmlElement` in the F# compiler, so the builder cannot accept both. Convert with a helper
as above.
:::

## Lambda children

Some components, such as `For`, take a function as their child instead of elements.

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

Extension methods cover what constructor parameters cannot: spreads, custom and `data-*` attributes, style objects and
refs.

```fsharp solid jsx
div(style = "padding: .5em; border: 1px dashed gray").data("state", "ready").attr("aria-live", "polite") {
    "This div has data-state and aria-live attributes."
}
```

See [extension methods](extension-methods.md) for the full list.
