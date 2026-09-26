---
title: Plugin types
---

:::note
The plugin's source is documented heavily. Only short notes are kept here.
:::

`Types.fs` holds the unions, records and functions the transformation passes around.

## Unions that name what a pattern found

Many patterns exist to drill through wrappers, such as a type cast, to find the type or identifier inside. Their
results are unions, so the patterns built on top of them can match on a case instead of repeating the drilling.

- `MemberRefType` says whether a member reference is a `Setter`, `Getter`, `Constructor`, something `Generated`, or
  `None` of these.
- `IdentType` names the identifiers that change how the transformation flows. The DSL's builders name their
  identifiers deliberately oddly, like `PARTAS_YIELD`, `PARTAS_ELEMENT` and `PARTAS_BUILDER`, so they cannot clash
  with yours. `IdentType` hides those names from the patterns that consume them: they match `IdentType.Yield`, not a
  string. It also covers Fable's `returnVal`, a Partas type's `_$ctor`, the component's self identifier (`Props`) and
  context providers.
- `TransformationKind` records whether the transformation started from a `let` binding or a type member. It is
  mostly unused.

## `ComponentFlag`

A bit flag passed to either attribute. Each flag turns on a diagnostic or turns off an optimisation:

| Flag | Value | Effect |
| --- | --- | --- |
| `Default` | 0 | All transformations and optimisations |
| `DebugMode` | 1 | Print the component's AST before transformation |
| `SkipPojoOptimisation` | 2 | Do not fold `[<Pojo>]` setters into the object literal |
| `PrintDisposals` | 4 | Warn about every expression dropped during transformation |
| `SkipCEOptimisation` | 8 | Do not unroll computation expressions into plain values |
| `SkipOmit` | 16 | Do not bind `PARTAS_OTHERS` in a type component. `.spread props` spreads the whole props object |
| `SpreadProps` | 32 | `.spread props` spreads the whole props object instead of `PARTAS_OTHERS`. The `omit` binding is still written |

The `ComponentFlag` module adds two combinations: `ComponentFlag.None` (`SkipPojoOptimisation ||| SkipCEOptimisation`)
and `ComponentFlag.VerboseDebugMode` (`DebugMode ||| PrintDisposals`).

## `PluginContext`

This record is threaded through every transformation, and it is the first argument of most patterns and functions in
the plugin.

```fsharp
type PluginContext =
    { Helper: PluginHelper
      Kind: TransformationKind
      SetterArray: ResizeArray<string * Expr>
      GetterArray: ResizeArray<string>
      SetterCollector: (string * Expr) -> unit
      GetterCollector: string -> unit
      Flags: ComponentFlag
      mutable SelfIdentifier: string }
```

- `Helper` gives every pattern the Fable `PluginHelper`, so it can look up entities and members and log warnings and
  errors.
- `Flags` lets any pattern check a `ComponentFlag` with `ctx.HasFlag`.
- `SelfIdentifier` is the name of the component's self identifier, such as `props` or `this`. `[<SolidTypeComponent>]`
  sets it before the transformation starts.
- `GetterArray` and `SetterArray` collect what the transformation finds. Each `props.foo` read adds `foo` to the
  getters. Each `props.foo <- value` adds `(foo, value)` to the setters. At the end, the getters become the `omit`
  call and the setters become the `merge` call.

The `PluginContext` module has the functions to work with it. The important ones:

- `create` makes a context for a member declaration.
- `addGetter` and `addSetter` push into the arrays. `addSetter` logs an **error** if the same property is given a
  default twice.
- `peekGetters` and `peekSetters` read the arrays. `getGetters` and `getSetters` read them **and clear them**.
- `logWarning` and `logError` report through Fable. Errors are collected and reported when the transformation ends.
- `debugDisposal` logs a dropped expression when `PrintDisposals` is set. Call it wherever you drop an expression, so
  a missing attribute can be traced.

## `ElementBuilder`

This record is where every tag transformation ends up:

```fsharp
type ElementBuilder =
    { TagSource: TagSource
      Properties: PropList
      Children: Expr list
      Range: SourceLocation option }
```

`TagSource` is either `AutoImport name`, a tag known by name (a native element, or a component in scope), or
`LibraryImport expr`, a tag with an import to inject. `PropList` is a list of `PropInfo`, which is a
`string * Expr` pair of attribute name and value.

Once a tag's pieces are collected into an `ElementBuilder`, they are all rendered the same way into a JSX element.
See [Spec](spec.md).
