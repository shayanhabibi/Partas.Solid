---
title: Attribute Flags
---

`[<SolidComponent>]` and `[<SolidTypeComponent>]` both take an optional `ComponentFlag`. A flag turns off one of the
plugin's optimisations, or turns on a diagnostic. Combine flags with `|||`:

```fsharp
[<SolidTypeComponent(ComponentFlag.DebugMode ||| ComponentFlag.SkipPojoOptimisation)>]
member props.View = div () { "..." }
```

## The flags

| Flag | Value | What it does |
| --- | --- | --- |
| `ComponentFlag.Default` | `0b0000` | The default transformations. The same as passing no flag. |
| `ComponentFlag.DebugMode` | `0b0001` | Prints the component's AST before the plugin transforms it. |
| `ComponentFlag.SkipPojoOptimisation` | `0b0010` | Turns off the `[<Pojo>]` constructor optimisation. |
| `ComponentFlag.PrintDisposals` | `0b0100` | Logs every expression the plugin discards. Noisy. |
| `ComponentFlag.SkipCEOptimisation` | `0b1000` | Keeps attribute array computations instead of unrolling them into array literals. |
| `ComponentFlag.SkipOmit` | `0b0001_0000` | Leaves out the `PARTAS_OTHERS = omit(...)` binding. Type components only. |
| `ComponentFlag.SpreadProps` | `0b0010_0000` | Makes `.spread` on the self identifier spread the whole props object. Type components only. |

Two shortcuts combine flags for you:

| Shortcut | Equals |
| --- | --- |
| `ComponentFlag.None` | `SkipPojoOptimisation ||| SkipCEOptimisation`: the minimal set of transformations |
| `ComponentFlag.VerboseDebugMode` | `DebugMode ||| PrintDisposals` |

:::details DebugMode
The AST is printed to the console during the Fable build. If you open an issue about the plugin, include a minimal example compiled with this flag and its output. See
[Submitting Issues](../contributing/submit-issues.md).
:::

:::details PrintDisposals
The plugin throws some expressions away: plumbing from computation expressions, unit values, and so on. Use this flag
when part of your component is missing from the output; the log tells you where the plugin dropped it. Turn it on for
one component at a time.
:::

:::details SkipPojoOptimisation
When you set properties on a `[<Pojo>]` object outside its primary constructor, the plugin normally moves those
assignments into the object literal, so the object is built in one step instead of being mutated after it is built.

This flag turns that off. Use it if the optimisation breaks a component.
:::

:::details SkipCEOptimisation
Recognized attribute array computations compile to array literals. A branch yielding one item on each side becomes
a ternary item; optional or multiple items become spreads such as `...(flag ? ["one", "two"] : [])`.
Bindings and side effects keep their scope and evaluation order. Unrecognized sequence shapes retain their original
Fable helper calls, and F# lists remain lists.

This flag disables attribute array unrolling. Element builders always compile to JSX children.
:::

## SkipOmit and SpreadProps

These two flags only change `[<SolidTypeComponent>]` output. By default, the plugin binds the props your component
does not read to `PARTAS_OTHERS`, and `.spread props` spreads those (see [rest props](solid-type-attribute.md#rest-props)).

- `SkipOmit` leaves out the `const PARTAS_OTHERS = ...` binding, so `.spread props` spreads the self identifier.
- `SpreadProps` makes `.spread props` spread the self identifier, `{...props}` in place of `{...PARTAS_OTHERS}`.

Together they make a thin wrapper that passes all of its props on, reads included. The two components below differ
only in their flags; compare their JSX:

```fsharp solid jsx
[<Erase>]
type Filtered() =
    inherit div()

    [<SolidTypeComponent>]
    member props.View = div(class' = "box " + props.class').spread props { props.children }

[<Erase>]
type PassThrough() =
    inherit div()

    [<SolidTypeComponent(ComponentFlag.SkipOmit ||| ComponentFlag.SpreadProps)>]
    member props.View = div(class' = "box " + props.class').spread props { props.children }
```

In `PassThrough`, the spread comes after `class`, so the caller's `class` replaces `"box ..."`. Partas.Solid's own
`Match.Keyed` is built this way: it renders Solid's `Match` with `keyed` set and hands every prop straight through.

:::note
`.spread` on any value other than the self identifier spreads that value, whatever the flags are.
:::
