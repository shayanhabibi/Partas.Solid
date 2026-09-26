---
title: Ecosystem
---

Bindings to Solid libraries, and component libraries built with Partas.Solid. Most of them live in
[Partas.Solid.Bindings](https://github.com/shayanhabibi/Partas.Solid.Bindings).

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

The libraries they bind are built on Solid 1.x, so they cannot run next to Partas.Solid 3.0 and Solid 2 until both the
library and the binding are ported. The pages keep their 2.x usage notes for reference.

| Area | Bindings |
| --- | --- |
| Headless UI | [Kobalte](kobalte.md), Corvu and others |
| Charts | [ApexCharts](apexcharts.md) |
| Tables | [TanStack Table](tan-stack-table.md) |
| Primitives | [solid-primitives](primitives/index.md): keyboard, RAF, event bus, mouse and more |
| Animation | [Motion](motion.md), animejs |
| Forms | [Modular Forms](modular-forms.md) |
| Drag and drop | [NeoDrag](neodrag.md), animejs |
| Command menu | [cmdk](cmdk.md) |
| Icons | [Lucide](lucide.md) |
| Stories | [Storybook](storybook.md) |
| Component libraries | [Partas.Solid.UI](partas-solid-ui.md): components built on headless UI and styled with Tailwind CSS |

Two pages here are about Partas.Solid itself and are current for 3.0:

- [Writing bindings](bindings.md): how to bind a Solid library, and the conventions the prebuilt bindings follow.
- [Polymorphism](polymorphism.md): the plugin's support for `as`-style polymorphic components.
