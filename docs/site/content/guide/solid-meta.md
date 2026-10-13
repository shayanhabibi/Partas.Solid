---
title: Solid Meta
---

Bindings for [`@solidjs/meta`](https://docs.solidjs.com/solid-meta/). They live in the `Partas.Solid.Meta` namespace,
which ships inside the `Partas.Solid` package.

```fsharp
open Partas.Solid
open Partas.Solid.Meta
```

Partas.Solid does not install the npm package. Use the Solid 2 line:

```bash
npm install @solidjs/meta@1.0.0-next.2
```

No provider is needed: render the tag components anywhere, and they manage `document.head`. Tags reach the head in a
microtask after the update that renders them, and are removed when their component is disposed. The runtime tests
cover the components below.

## Components

Each component inherits the matching HTML tag, so it takes the same attributes. Every tag but `Base` also takes a
`key`: a later tag with the same key replaces the earlier one until it is disposed.

| Component | Inherits | Notes |
| --- | --- | --- |
| `Head` | `FragmentNode` | Groups its child tags into one replacement set |
| `Title` | `title` | |
| `Meta` | `meta` | |
| `Link` | `link` | |
| `Stylesheet` | `link` | `rel="stylesheet"` preset. Solid 2 preloads it and holds the update until it loads |
| `Style` | `style` | |
| `Script` | `script` | |
| `Base` | `base'` | Shell-only: applied in server rendering, ignored on the client |

```fsharp
open Partas.Solid
open Partas.Solid.Meta

[<SolidComponent>]
let PageHead () =
    Head () {
        Title (key = "title") { "My page" }
        Meta (key = "description", name = "description", content = "A page about things")
        Stylesheet (href = "/site.css")
    }
```

A `Head` inside another `Head` forms its own group. To add tags to the surrounding group, render bare tags instead.

`MetaProvider` is gone: `@solidjs/meta` 1.0 has none.

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
