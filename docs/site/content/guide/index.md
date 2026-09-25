---
title: Partas.Solid
---

Partas.Solid is a front-end framework for F#. It binds [Solid](https://www.solidjs.com/) and compiles your F# to the
JSX Solid expects, through a [Fable](https://fable.io/) compiler plugin.

- **F#.** Write your front end in F#, next to an F# back end, with types checked end to end.
- **Fable.** Fable compiles your code to JavaScript. The Partas.Solid plugin turns your components into JSX on the
  way.
- **Solid.** The output is ordinary Solid code. You get its fine-grained reactivity and small bundles, and you can
  use it from JavaScript.

```fsharp solid render=Counter jsx
[<SolidComponent>]
let Counter () =
    let count, setCount = createSignal 0

    button (onClick = fun _ -> setCount (count () + 1)) {
        $"Clicked {count ()} times"
    }
```

The example above is live. It was compiled from the F# on this page when the site was built.

:::warning
These docs are for **Partas.Solid 3.0 on Solid 2.0.0-rc.9**. 3.0 is a prerelease and is not on NuGet yet. If you use
Partas.Solid 2.x on Solid 1.9, read [Migrating to Solid 2](migrating-to-solid-2.md) for what changed.
:::

Partas.Solid is an opinionated fork of [Oxpecker.Solid](https://github.com/lanayx/Oxpecker). It keeps Oxpecker's DSL
and transforms the F# AST more aggressively to produce correct JSX. Please support the original project too. See
[the Oxpecker fork](../about/oxpecker-fork.md) for the differences.

## Where to start

- [Installation](installation.md): set up Fable, the packages and Vite.
- [Overview](overview.md): the two ways to write a component.
- [Oxpecker DSL](oxpecker-dsl.md): elements, attributes and children.
- [Building the DOM](building-the-dom.md): reactivity, control flow and lists.
- [API differences](api-differences.md): where the F# API differs from Solid's.
- [Common issues](common-issues.md): what to check when something goes wrong.

For the plugin itself, see [About](../about/index.md) and [JSX output](../about/jsx-output.md).
