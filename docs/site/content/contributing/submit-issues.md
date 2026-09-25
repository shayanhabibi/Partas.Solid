---
title: Submitting issues
---

If something is not going right, open an issue on
[GitHub](https://github.com/shayanhabibi/Partas.Solid/issues). The plugin rewrites your F# into JSX, so the most
useful report shows three things:

1. **The input**: the smallest F# that shows the problem.
2. **The output**: the `.fs.jsx` file Fable wrote for it.
3. **The AST**: what the plugin was given, printed with `DebugMode`.

Say what you expected the JSX to be, too. Because the JSX is readable, you can often point at the exact line that is
wrong.

## DebugMode

Pass `ComponentFlag.DebugMode` to the attribute on the component in question:

```fsharp
[<SolidComponent(ComponentFlag.DebugMode)>]
let MyComponent () =
    // ...

[<Erase>]
type MyTypeComponent() =
    inherit div()

    [<SolidTypeComponent(ComponentFlag.DebugMode)>]
    member props.View =
        // ...
```

When Fable compiles that component, the plugin prints the Fable AST of its body to the console, before any
transformation, between `START MEMBER DECL!!!` and `END MEMBER DECL!!!`. Copy that block into your issue along with
the input and the JSX.

## When output goes missing

Sometimes an expression is not wrong in the output, but simply not there. The plugin drops expressions it has no use
for during a transformation, and `PrintDisposals` tells you which ones. Each dropped expression is logged as a
Fable warning, with the stage it was dropped in and its source location where there is one:

```fsharp
[<SolidComponent(ComponentFlag.PrintDisposals)>]
let MyComponent () =
    // ...
```

It is noisy. `ComponentFlag.VerboseDebugMode` turns on `DebugMode` and `PrintDisposals` together.

## Ruling out an optimisation

Some flags switch off one of the plugin's optimisations. If the problem goes away with one of them set, say so in the
issue, because it narrows down where the bug is:

| Flag | What it switches off |
| --- | --- |
| `SkipPojoOptimisation` | Folding setters used when building a `[<Pojo>]` object into its object literal |
| `SkipCEOptimisation` | Unrolling computation expressions, such as list and array builders, into plain values |
| `SkipOmit` | The `const PARTAS_OTHERS = omit(props, ...)` binding in a `[<SolidTypeComponent>]` |
| `ComponentFlag.None` | `SkipPojoOptimisation` and `SkipCEOptimisation` together |

Flags combine with `|||`:

```fsharp
[<SolidComponent(ComponentFlag.DebugMode ||| ComponentFlag.SkipCEOptimisation)>]
let MyComponent () =
    // ...
```

See [Attribute flags](../guide/attribute-flags.md) for the full list.

## Before you file

- Check that you are on Fable 5 (the repository tests against 5.13.0) and that `Partas.Solid` and `Partas.Solid.FablePlugin` have the same
  version.
- If you are coming from Partas.Solid 2.x, check [Migrating to Solid 2](../guide/migrating-to-solid-2.md). Several
  APIs were renamed or removed.
- If you can, add the case to the snapshot tests and send it as a pull request. See
  [Contributing](index.md).
