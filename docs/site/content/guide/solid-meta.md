---
title: Solid Meta
---

Bindings for [`@solidjs/meta`](https://docs.solidjs.com/solid-meta/) 1.0, in the `Partas.Solid.Meta` namespace of the
`Partas.Solid` package. Install the npm package yourself:

```bash
npm install @solidjs/meta@1.0.0-next.2
```

No provider is needed (1.0 removed `MetaProvider`). Render the tag components anywhere: they reach `document.head` in
a microtask after the update that renders them, and leave it when their component is disposed.

## Components

Each component inherits the matching HTML tag, so it takes the same attributes. Every tag but `Base` also takes a
`key`: a later tag with the same key replaces the earlier one, which comes back when the later one is disposed.

| Component | Inherits | Notes |
| --- | --- | --- |
| `Head` | `FragmentNode` | Groups its child tags into one replacement set |
| `Title` | `title` | |
| `Meta` | `meta` | |
| `Link` | `link` | |
| `Stylesheet` | `link` | `rel="stylesheet"` preset. Solid preloads it and holds the update until it loads |
| `Style` | `style` | |
| `Script` | `script` | |
| `Base` | `base'` | Shell-only: applied in server rendering, ignored on the client |

```fsharp
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

## useHead

`useHead`, from `@solidjs/web`, is bound in `Partas.Solid.Web` and needs no extra package:

```fsharp
useHead(tag: HeadTag): unit
useHead(tag: HeadTag[]): unit
useHead(tag: unit -> HeadTag): unit
useHead(tag: unit -> HeadTag[]): unit
```

`HeadTag(tag: HeadTag.Tag, props: obj, ?key: U2<string, unit -> string>)` is a `[<Pojo>]`. `HeadTag.Tag` is `Title`,
`Meta`, `Link`, `Style`, `Script` or `Base`. A title's text goes in `props` under `"children"`. Pass a function when
the tag depends on reactive state:

```fsharp solid show=code jsx
open Partas.Solid.Web

[<SolidComponent>]
let ReactiveTitle () =
    let count, setCount = createSignal 0
    useHead (fun () -> HeadTag(HeadTag.Tag.Title, createObj [ "children", box ("Count " + string (count ())) ]))
    useHead (HeadTag(HeadTag.Tag.Meta, createObj [ "name", box "description"; "content", box "A counter" ]))
    button (onClick = fun _ -> setCount (count () + 1)) { count () }
```
