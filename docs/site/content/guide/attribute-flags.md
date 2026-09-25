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
| `ComponentFlag.SkipCEOptimisation` | `0b1000` | Turns off the optimisation of computation expressions and lists. |
| `ComponentFlag.SkipOmit` | `0b0001_0000` | Leaves out the `PARTAS_OTHERS = omit(...)` binding. Type components only. |
| `ComponentFlag.SpreadProps` | `0b0010_0000` | Makes `.spread` on the self identifier spread the whole props object. Type components only. |

Two shortcuts combine flags for you:

| Shortcut | Equals |
| --- | --- |
| `ComponentFlag.None` | `SkipPojoOptimisation ||| SkipCEOptimisation`: the minimal set of transformations |
| `ComponentFlag.VerboseDebugMode` | `DebugMode ||| PrintDisposals` |

:::details title="DebugMode"
Prints the AST of the component, before the plugin transforms it, to the console during the Fable build.

If you open an issue about the plugin, include a minimal example compiled with this flag and its output. See
[Submitting Issues](../contributing/submit-issues.md).
:::

:::details title="PrintDisposals"
While the plugin transforms a component, it throws some expressions away: plumbing from computation expressions,
unit values, and so on. This flag logs each one.

Use it when part of your component is missing from the output. The log tells you where the plugin dropped it. It is
very noisy, so turn it on for one component at a time.
:::

:::details title="SkipPojoOptimisation"
When you set properties on a `[<Pojo>]` object outside its primary constructor, the plugin normally moves those
assignments into the object literal, so the object is built in one step instead of being mutated after it is built.

This flag turns that off. Use it if the optimisation breaks a component.
:::

:::details title="SkipCEOptimisation"
The plugin removes computation expression plumbing around lists and children, so the output is a plain array or
plain JSX children rather than a chain of builder calls.

This flag turns that off. Fable 5 handles most of these shapes itself. Use the flag if the optimisation produces
wrong code, and report it.
:::

## SkipOmit and SpreadProps

These two flags only change `[<SolidTypeComponent>]` output. By default, the plugin binds the props your component
does not read and spreads those (see [SolidTypeComponent](solid-type-attribute.md)):

```jsx
const PARTAS_OTHERS = omit(props, "class");
return <div class={props.class} {...PARTAS_OTHERS} n$={false} />;
```

- `SkipOmit` leaves out the `const PARTAS_OTHERS = ...` line.
- `SpreadProps` makes `.spread props` spread the self identifier itself: `{...props}` in place of
  `{...PARTAS_OTHERS}`.

`SkipOmit` also makes `.spread` use the self identifier, since there is no `PARTAS_OTHERS` to spread. Use them together
for a thin wrapper that passes all of its props on, reads included. Partas.Solid's own `Match.Keyed` is one. It
renders Solid's `Match` with `keyed` set, and hands every prop it was given straight through:

```fsharp
[<SolidTypeComponent(ComponentFlag.SkipOmit ||| ComponentFlag.SpreadProps)>]
member props.comp = Match<'T>(keyed = true).spread(props)
```

The body compiles to a `<Match keyed={true} {...props} n$={false} />` element, with no `omit` before it.

:::note
`.spread` on any value other than the self identifier spreads that value, whatever the flags are.
:::

## Signatures

```fsharp
type ComponentFlag =
    | Default = 0b0000
    | DebugMode = 0b0001
    | SkipPojoOptimisation = 0b0010
    | PrintDisposals = 0b0100
    | SkipCEOptimisation = 0b1000
    | SkipOmit = 0b0001_0000
    | SpreadProps = 0b0010_0000

module ComponentFlag =
    [<Literal>]
    let None = ComponentFlag.SkipPojoOptimisation ||| ComponentFlag.SkipCEOptimisation
    [<Literal>]
    let VerboseDebugMode = ComponentFlag.DebugMode ||| ComponentFlag.PrintDisposals
```
