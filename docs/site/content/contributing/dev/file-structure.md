---
title: Plugin structure
---

:::note
The plugin's source is documented heavily, and the comments explain how each piece is used. These pages are an
overview to read before the source.
:::

Partas.Solid ships as two NuGet packages that always share a version. **Partas.Solid**, in `src/Partas.Solid/`, is
the DSL: tags, attributes and the Solid bindings, almost all of it erased or imported. **Partas.Solid.FablePlugin**, in
`src/Partas.Solid.FablePlugin/`, rewrites the F# AST into JSX, and is where the behaviour lives. Nothing of it runs at
runtime, so the JSX it writes is the specification: a change in behaviour shows as a change in the
[snapshot tests](../index.md#snapshot-tests).

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

The repository's [conventions](../index.md#conventions) apply: an inexhaustive `match` fails the build, and `Plugin.fs`
is never formatted.
