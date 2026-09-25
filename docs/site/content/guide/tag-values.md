---
title: Tag Values
---

A tag value lets you pass a component around as a value, and render it later as the tag of an element. Use one when
the caller of your component picks what it renders: an icon, a wrapper, a list item.

## Making a tag value

Wrap a component constructor in `TagValue`, or use the `!@` operator, which does the same:

```fsharp
let Icon = TagValue(Pill)
let Icon = !@Pill
```

`!@` is rewritten by the plugin, so it only works inside a `[<SolidComponent>]` or `[<SolidTypeComponent>]`. See
[SolidComponent](solid-component-attribute.md) for what that means for code outside a component.

A prop that holds a tag value has the type `TagValue`.

## Rendering a tag value

`%` renders a tag value. On the right, give it the props, either as an anonymous record or as an element:

```fsharp
Icon % {| class' = "large" |}             // <Icon class="large" />
Icon % span (class' = "large") { "text" } // <Icon class="large">text</Icon>
```

With an element on the right, the plugin keeps its attributes and children and swaps its tag for the tag value. The
`span` is only there to carry them. `Icon.render(...)` is the long form of `%`, and `Icon.render()` renders it with no
props.

## Example

`Labelled` takes a `wrapper` prop, and uses `Pill` when the caller does not pass one:

```fsharp solid jsx
[<Erase>]
type Pill() =
    inherit span()

    [<SolidTypeComponent>]
    member props.View =
        span(style = "padding: 0 .5em; border-radius: 1em; background: lavender").spread props {
            props.children
        }

[<Erase>]
type Shouty() =
    inherit span()

    [<SolidTypeComponent>]
    member props.View =
        strong () {
            props.children
            "!"
        }

[<Erase>]
type Labelled() =
    inherit div()

    [<Erase>]
    member val wrapper: TagValue = unbox null with get, set

    [<Erase>]
    member val label: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        props.wrapper <- !@Pill
        div () {
            "Label: "
            props.wrapper % span () { props.label }
        }
```

```fsharp solid
div () {
    Labelled(label = "default wrapper")
    Labelled(label = "custom wrapper", wrapper = !@Shouty)
}
```

A tag value in a local binding is meant to work the same way:

```fsharp
[<SolidComponent>]
let LocalTag () =
    let Wrapper = !@Pill
    Wrapper % span (title = "rendered through a local tag value") { "local" }
```

:::danger
This does not compile to valid JSX yet. Fable inlines the local binding, so the tag comes out as
`<op_BangAt(Pill_$ctor) ...>`, which the Solid JSX compiler rejects. Until that is fixed, pass the tag value in through
a prop, as `Labelled` does above, or use `Dynamic` (below).
:::

:::warning
Give a local tag value a name that starts with a capital letter. The Solid JSX compiler reads `<wrapper>` as an HTML
element called `wrapper`, and only `<Wrapper>` as a component.
:::

## HTML tags

Use `!@` with components only. There are two known problems with it:

- `!@button` or `!@div` compiles to a bare `button` or `div` identifier (see the `TagsAsValuesSimple` snapshot test),
  which does not exist at runtime.
- `!@Comp` inside an `if`/`else` does not compile to a valid tag. The runtime tests record this as a known bug.

To let the caller choose between HTML tags, or to pick a tag from a signal, use `Dynamic` from `Partas.Solid.Web`.
It takes the tag name as a string:

```fsharp solid render=HeadingLevels
open Partas.Solid.Web

[<SolidComponent>]
let HeadingLevels () =
    let level, setLevel = createSignal 3
    let headingProps = createObj [ "children", box "A heading of any level" ]

    div () {
        button (onClick = fun _ -> setLevel (if level () = 6 then 1 else level () + 1)) { "Next level" }
        Dynamic<obj>(component' = unbox ("h" + string (level ()))).spread (headingProps)
    }
```

`Dynamic` takes components too: `Dynamic<obj>(component' = !@Pill)`.

## Signatures

```fsharp
type TagValue(tag: FSharpFunc<_, #HtmlElement>) =
    member render: PARTAS_CONSTRUCTOR: 'T -> 'T
    member render: PARTAS_PROPERTIES: obj -> RegularNode
    member render: unit -> RegularNode
    static member (%): TagValue * 'T -> 'T
    static member (%): TagValue * obj -> RegularNode

val (!@): FSharpFunc<_, #HtmlElement> -> TagValue
```
