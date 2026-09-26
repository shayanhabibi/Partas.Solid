---
title: Plugin utilities
---

:::note
The plugin's source is documented heavily. Only short notes are kept here.
:::

`Utils.fs` is the first file in the plugin. It sits in the `Fable.AST.Fable` namespace, so the rest of the plugin
can use it next to Fable's own AST types.

Everything in it is built on primitives: strings, and Fable's `Expr`, `Type` and `PluginHelper`. It does not know
about `PluginContext`. That is why it now compiles before `Types.fs`; anything that needs the context belongs in
`Spec.fs` or `Plugin.fs`.

## Constructors

- `AstUtils` builds common expressions: constants, identifiers, `Let`, `Sequential`, calls, imports, property gets and
  sets, and objects. Use it instead of spelling out Fable AST records by hand.
- `JsxUtils` builds JSX. `JsxUtils.CreateElement` makes the call to Fable's JSX `create` that Fable prints as a JSX
  element, from a tag name (or tag expression), a list of properties and a list of children.

## Patterns

The `Patterns` module holds the active patterns used everywhere else. `Spec.fs` and `Plugin.fs` alias it as
`Utils`, so in the source you see `Utils.StartsWith` and so on.

String patterns:

```fsharp
let inline (|StartsWith|_|) (value: string): string -> bool = _.StartsWith(value)
let inline (|EndsWith|_|) (value: string): string -> bool = _.EndsWith(value)
```

`StartsWithTrimmed` and `EndsWithTrimmed` match the same way and return the string with the match cut off. For
example, `EndsWithTrimmed "_$ctor"` turns a constructor name into its type name.

AST patterns:

- `ExprTypeCastDrill` looks through any number of `TypeCast` wrappers to the expression inside.
- `GetDeclaredType` digs through lambda, tuple, option, list and array types to the declared type inside, if there is
  one.
- `EntityRefHasAttribute` and `TypeHasAttribute` match an entity or type that carries an attribute with a given full
  name. They need the `PluginHelper` to look the entity up.

## Other helpers

- `Expr.findAndDiscardElse` walks an expression and keeps only the parts that match a predicate.
- `StringUtils.TrimReservedIdentifiers` strips the suffixes F# uses to dodge reserved words, such as the `'` in
  `class'` or a generic arity marker. It is applied to every attribute key, so `class'` is written as `class`.
