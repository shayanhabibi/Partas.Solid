---
title: Utilities
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Start with a file of utilities you will use later. Create `Utils.fs`:

```fsharp
namespace Partas.Solid.Example

open Partas.Solid
open Fable.Core

type [<Erase>] Lib =
    [<Import("twMerge", "tailwind-merge")>]
    static member twMerge (classes: string) : string = jsNative
    [<Import("clsx", "clsx")>]
    static member clsx(classes: obj): string = jsNative
    static member cn (classes: string array): string = classes |> Lib.clsx |> Lib.twMerge
```

`Lib.cn` joins a list of classes with `clsx`, then lets `tailwind-merge` resolve any Tailwind classes that clash, so a
class passed to a component overrides the component's own.

:::note
Nothing here depends on Solid. `clsx` and `tailwind-merge` are plain npm packages, so this file works unchanged on
Partas.Solid 3.0. The `AccordionItem` in [Benefits of JSX output](../../about/jsx-output.md) uses the same helper.
:::

Next: [Table components](table-component.md).
