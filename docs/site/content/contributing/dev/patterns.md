---
title: Plugin patterns
---

:::note
The plugin's source is documented heavily. Every pattern has a comment on what it matches and why. This page is a
map.
:::

`Plugin.fs` is one recursive module, `AST`. Its active patterns recognise the pieces of a tag: its constructor, its
attributes and its children. Order matters in several places, and the comments say where.

## Attributes and properties

The `AttributesAndProperties` module deals with everything that ends up as an attribute.

- `PropertyGetter` recognises a read like `props.label` and returns the property name.
- `PropertySetter` recognises `props.label <- value`, in all the shapes Fable produces for it: a setter call, a
  `member val` field set, or a `[<DefaultValue>] val mutable` field set.
- `PropsGetterOrSetter` combines the two. A getter is recorded with `addGetter` and rewritten to a plain property
  read on the self identifier. A setter is recorded with `addSetter` and removed from the body; it comes back as the
  `merge`.

:::warning
Match `PropertyGetter` **before** `PropertySetter`. The setter pattern is greedy, and it will swallow attribute
expressions that happen to read from the props.
:::

- `AttributeExpression` recognises one attribute and returns its name and value. It handles the special cases:
  - `.style'`, `.class'` and `ref`
  - `.data`, `.attr` and `.bool`, which build `data-*` and other attribute names from a string
  - `.spread`, written as `{...PARTAS_OTHERS} n$` when you spread the component's own props, or `{...x} n$` for any
    other identifier or field. `n$={false}` is the marker that closes a spread.
  - polymorphic attributes (`as'`, `asChild`, or any alias of `__PARTAS_POLYMORPHIC__`), whose value becomes a
    function of `PARTAS_POLYPROPS` that spreads them onto the tag
  - `aria` setters, renamed to `aria-*`
- `ValueUnroller` and `MatchValueReplacer` flatten the `delay`/`toArray` wrappers Fable puts around list and array
  values in attributes. They only unroll `toArray`/`toList` over a delayed sequence; `List.toArray xs` is a real
  conversion and stays.
- `PropCollector` runs over a list of expressions and collects every attribute. Anything it does not recognise is
  dropped, with `debugDisposal`.

## Tags

`TagConstructor` recognises the expression that creates a tag, and returns an `ElementBuilder`. It tries, in order:

1. constructors of types with `[<PartasImport>]` or `[<PartasProxyImport>]`, which need an import injected. These must
   come before the local tag case, or they are never reached.
2. a tag declared in this project (a Partas type, matched through its `_$ctor`)
3. a native element
4. a library import, and then a user-defined import
5. a tag with properties, which Fable writes as a `let` of the element followed by setters
6. a tag with an extension call such as `.spread` or `.data`
7. a tag with children: a call to the builder's `Run` with the tag as its first argument
8. a context provider, imported or local, which becomes `<Context.Provider value={...}>`

## Children

`BuilderCollector` walks the expressions inside a tag's computation expression and returns the children. It is the
most recursive part of the plugin. It:

- unwraps the builder's `let` and lambda bindings (`PARTAS_ELEMENT`, `PARTAS_BUILDER`, `PARTAS_YIELD` and the rest)
- turns the lambdas of `ChildLambdaProvider` types, such as the child function of `For.Keyed`, into JSX functions
  with the right number of parameters
- transforms conditionals, and lifts property getters and setters
- keeps field and tuple accessors whole, so a child like `item.Title` is not thrown away

`collectTagInfo` does the final clean-up of an `ElementBuilder`, trimming reserved suffixes off the attribute names.

## Other patterns

- `SpecialAttributeTransformation.Pojo` rewrites a `[<Pojo>]` constructor and its setters into one object literal.
  `SkipPojoOptimisation` turns it off.
- `TagValue.TagValue` and `TagValue.TagRender` handle tag values: a tag stored as a value with `!@`, and rendered later
  with `.render`.
- `Polymorphism.PolymorphicAttribute` decides whether an attribute name is polymorphic.

See [Transformation](transformation.md) for how these are put together.
