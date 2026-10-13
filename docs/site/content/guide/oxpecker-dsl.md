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

## Elements

Each HTML element is a class, and its attributes are optional constructor parameters. Attribute names that are F#
keywords get an apostrophe: `class'`, `type'`, `for'`, `when'`. An attribute with a fixed set of values takes a case
of a union in `Attr`, and the JSX gets its string:

```fsharp solid jsx
button (class' = "btn", type' = Attr.ButtonType.Button, title = "A plain button") { "Button" }
```

Event handlers receive the event type of their event. `onClick` takes a `Dom.MouseEvent -> unit`, `onInput` a
`Dom.InputEvent -> unit`; `Dom` is in `Fable.Core.TS`.

## Children

The computation expression after an element holds its children: strings, numbers and other elements, in any mix.

```fsharp solid jsx
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

`JSX.Element` collides with `HtmlElement` in the F# compiler, so the builder does not take Fable's `JSX.jsx`
directly. `unbox` it. `JSX.jsx` only takes a string literal:

```fsharp solid jsx
div () {
    unbox<HtmlElement> (JSX.jsx "<button>Raw JSX button</button>")
}
```

## Lambda children

Some components, such as `For`, take a function as their child instead of elements.

:::warning
To pass a function as a child, you must `yield` it. If your IDE cannot infer the function's parameter types, you have
probably left out the `yield`.
:::

```fsharp solid jsx
ul () {
    For.Keyed(each = [| "one"; "two"; "three" |]) {
        yield fun item _ -> li () { $"Item {item}" }
    }
}
```

To write a component that takes a function child, see [tag interfaces](tag-interfaces.md#child-lambda-providers).

## Extension methods

Spreads, custom and `data-*` attributes, style objects and refs are chained after the constructor:
`div(class' = "card").data("id", "42")`. See [extension methods](extension-methods.md).
