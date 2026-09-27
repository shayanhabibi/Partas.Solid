---
title: Solid Meta
---

Bindings for [`@solidjs/meta`](https://docs.solidjs.com/solid-meta/). They live in the `Partas.Solid.Meta` namespace,
which ships inside the `Partas.Solid` package.

```fsharp
open Partas.Solid
open Partas.Solid.Meta
```

Partas.Solid does not install the npm package:

```bash
npm install @solidjs/meta
```

:::warning
These bindings were written for `@solidjs/meta` on Solid 1.x and have not changed for Partas.Solid 3.0. No test covers
them, and they are not verified against Solid 2. For a tested alternative, use `useHead` from `@solidjs/web`, below.
:::

## Components

Each component inherits the matching HTML tag, so it takes the same attributes.

| Component | Inherits | Upstream docs |
| --- | --- | --- |
| `MetaProvider` | `FragmentNode` | [MetaProvider](https://docs.solidjs.com/solid-meta/reference/meta/metaprovider) |
| `Title` | `title` | [Title](https://docs.solidjs.com/solid-meta/reference/meta/title) |
| `Style` | `style` | [Style](https://docs.solidjs.com/solid-meta/reference/meta/style) |
| `Link` | `link` | [Link](https://docs.solidjs.com/solid-meta/reference/meta/link) |
| `Meta` | `meta` | [Meta](https://docs.solidjs.com/solid-meta/reference/meta/meta) |
| `Base` | `base'` | [Base](https://docs.solidjs.com/solid-meta/reference/meta/base) |

```fsharp
[<SolidComponent>]
let App () =
    MetaProvider() {
        Title() { "My page" }
        Meta().attr("name", "description").attr("content", "A page about things")
        div () { "Hello" }
    }
```

:::warning
The `meta` tag binding has no `name`, `content` or `charset` properties: in `HtmlAttributes.fs` they (and an `http`
property meant as `http-equiv`) are declared on the `menu` tag's attributes instead. `Meta(name = ...)` does not
compile. Set them with `.attr`, as above, until the binding is fixed.
:::

## useHead from @solidjs/web

Bound in `Partas.Solid.Web`. It needs no extra package and no provider. The runtime tests cover it: tags reach
`document.head`, a reactive title updates, and the tags are removed when the component is disposed.

```fsharp
open Partas.Solid.Web

useHead(tag: HeadTag): unit
useHead(tag: HeadTag[]): unit
useHead(tag: unit -> HeadTag): unit
useHead(tag: unit -> HeadTag[]): unit
```

`HeadTag` is a `[<Pojo>]`: `HeadTag(tag: HeadTag.Tag, props: obj, ?key: U2<string, unit -> string>)`. `HeadTag.Tag`
is `Title`, `Meta`, `Link`, `Style`, `Script` or `Base`. Put the element's children, such as a title's text, in
`props` under `"children"`.

```fsharp solid show=code jsx
open Partas.Solid.Web

[<SolidComponent>]
let ReactiveTitle () =
    let count, setCount = createSignal 0
    useHead (fun () -> HeadTag(HeadTag.Tag.Title, createObj [ "children", box ("Count " + string (count ())) ]))
    useHead (HeadTag(HeadTag.Tag.Meta, createObj [ "name", box "description"; "content", box "A counter" ]))
    button (onClick = fun _ -> setCount (count () + 1)) { count () }
```

Pass a function when a tag depends on reactive state, as the title does here.
