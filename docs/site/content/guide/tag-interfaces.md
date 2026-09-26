---
title: Tag Interfaces
---

Every tag in Partas.Solid is a type, and what a tag can do comes from the interfaces it implements. A type that
implements `HtmlContainer` takes children. A type that implements `HtmlTag` takes the global HTML attributes and the
[extension methods](extension-methods.md). You use the same interfaces when you write your own components.

In most cases you do not implement the interfaces yourself. You inherit a tag that already has them:

```fsharp
[<Erase>]
type MyButton() =
    inherit button() // every button attribute, children, and the extension methods
```

## Foundation interfaces

| Interface | What it gives you |
| --- | --- |
| `HtmlElement` | The base. Anything that implements it can be a child of another element. |
| `HtmlTag` | Inherits `HtmlElement`. Adds the global HTML attributes, events and the extension methods. |
| `HtmlContainer` | Inherits `HtmlElement`. Lets the type take children in a `{ }` block, and adds a `children` property. |
| `FlowContainer<'A>` | Inherits `HtmlElement`. Like `HtmlContainer`, but children must be of type `'A`. |

## Derived interfaces

| Interface | Inherits | Used for |
| --- | --- | --- |
| `RegularNode` | `HtmlTag`, `HtmlContainer` | An element with attributes and children, such as `div`. |
| `VoidNode` | `HtmlTag` | An element with attributes and no children, such as `input` or `br`. |
| `FlowNode<'A>` | `HtmlTag`, `FlowContainer<'A>` | An element with attributes whose children are limited to `'A`. |
| `FragmentNode` | `HtmlContainer` | Renders as a JSX fragment, `<></>`. `Fragment()` implements it. |

`Switch`, for example, is a `FlowContainer<IMatch>`, so the compiler only accepts `Match` children inside it.

## Child lambda providers

Some components take a function as their child. `For` passes you each item and its index, and your function returns
what to render for it. The child lambda provider interfaces type that function:

| Interface | The child is |
| --- | --- |
| `ChildLambdaProvider<'P1>` | `'P1 -> #HtmlElement` |
| `ChildLambdaProvider2<'P1, 'P2>` | `'P1 -> 'P2 -> #HtmlElement` |
| `ChildLambdaProvider3<'P1, 'P2, 'P3>` | a function of three arguments |
| `ChildLambdaProvider4<'P1, 'P2, 'P3, 'P4>` | a function of four arguments |
| `ChildLambdaProviderStrict<'P1, 'Children>` | `'P1 -> 'Children` |
| `ChildLambdaProviderStrict2` to `4` | the same, with two to four arguments |

The strict variants fix the return type as well as the arguments. `For.Keyed` implements
`ChildLambdaProvider2<'T, Accessor<int>>`, so its child is `fun item index -> ...`. `Repeat<'T>` implements
`ChildLambdaProviderStrict<int, 'T>`.

You pass the function with `yield`:

```fsharp
For.Keyed(each = items) {
    yield fun item index -> li () { item }
}
```

Implement one of these interfaces on your own component to take a function child. Inside the component,
`props.children` holds the function. Unbox it to its F# type and call it:

```fsharp solid jsx
[<Erase>]
type Toggle() =
    inherit div()
    interface ChildLambdaProvider<Accessor<bool>>

    [<SolidTypeComponent>]
    member props.View =
        let on, setOn = createSignal false
        let renderChild: Accessor<bool> -> HtmlElement = unbox props.children

        div (style = "display: flex; gap: .5rem; align-items: center") {
            button (onClick = fun _ -> setOn (not (on ()))) { "Flip" }
            renderChild on
        }
```

```fsharp solid
Toggle() {
    yield fun on -> span () { if on () then "ON" else "OFF" }
}
```

For a function of two or more arguments, unbox the children to a `System.Func`, because Solid calls it with all of its
arguments at once:

```fsharp
let renderChild: System.Func<Accessor<string>, (string -> unit), HtmlElement> = unbox props.children
renderChild.Invoke(value, setValue)
```

:::note
Solid 2 removed `Index`. Use `For.NonKeyed`, which passes you an accessor for each item. See
[Migrating to Solid 2](migrating-to-solid-2.md).
:::

## Polymorph

`Polymorph` lives in `Partas.Solid.Polymorphism` and inherits `HtmlTag`. It is for components whose rendered element
the caller picks, such as Kobalte's. It adds an `.as'` extension that takes a tag or a [tag value](tag-values.md).

See [Polymorphism](../ecosystem/polymorphism.md) and [Kobalte](../ecosystem/kobalte.md).
