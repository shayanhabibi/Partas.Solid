---
title: Plugin structure
---

:::note
The plugin's source is documented heavily, and the comments explain how each piece is used. These pages are an
overview to read before the source.
:::

Partas.Solid ships as two NuGet packages that always share a version:

- **Partas.Solid** is the DSL: tags, attributes, the style spec and the Solid bindings. Almost all of it is erased or
  imported. The types exist to give you a typed way to write components, and the plugin erases them.
- **Partas.Solid.FablePlugin** is a Fable compiler plugin that rewrites the F# AST into JSX. This is where the
  behaviour lives. Without it, the library produces nothing useful.

Because the plugin rewrites the AST rather than running anything at runtime, the JSX it writes is the specification.
You see a change in behaviour by comparing the generated JSX. See [Contributing](../index.md) for how the snapshot
tests do that.

## Files

The plugin project targets `net6.0` and references `Fable.AST` 5.0. Its files compile in this order, and each one
depends only on the ones before it:

```bash
Partas.Solid.FablePlugin
|-- Utils.fs       # string and AST active patterns, AST and JSX constructors
|-- Types.fs       # PluginContext, ComponentFlag, IdentType and the other plugin types
|-- Spec.fs        # what input is legal, prebaked expressions, recognisers for AST nodes
|-- Plugin.fs      # the transformation tree and the two public attributes
|__ Storybook.fs   # a separate attribute that writes Storybook CSF output
```

| File | Page |
| --- | --- |
| `Utils.fs` | [Utilities](utilities.md) |
| `Types.fs` | [Types](types.md) |
| `Spec.fs` | [Spec](spec.md) |
| `Plugin.fs` | [Patterns](patterns.md) and [Transformation](transformation.md) |

`Storybook.fs` holds `PartasStorybookAttribute`, which turns a story definition into a Storybook CSF module. It has
its own recursion over the story type, and is covered in [Storybook](../../guide/storybook.md).

## Rules for the plugin project

- Warnings 3239 and 0025 (incomplete pattern matches) are errors. An inexhaustive `match` in the plugin fails the
  build.
- `Plugin.fs` is excluded from fantomas. Do not format it.
- The plugin requires Fable 5 and only runs when the target is JavaScript and the file extension is `.js` or `.jsx`.
  The docs and tests compile with `-e .fs.jsx`.
