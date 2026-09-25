---
title: Lucide
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

`Partas.Solid.Lucide` binds the [Lucide](https://lucide.dev/guide/) icons. See its docs for usage.

## Usage

Every standard icon is a component in the `Partas.Solid.Lucide` namespace. For the Lucide Lab icons, use the `Icon`
component and pass the icon node:

```fsharp
open Partas.Solid.Lucide

Check() // a standard icon

// Icon component for Lucide Lab icons
Icon(iconNode = LucideLab.yinYang)
```

The binding is generated from the React library, with a generator written for Feliz. The Solid library is organised less
cleanly, and the icon names are the same in both.
