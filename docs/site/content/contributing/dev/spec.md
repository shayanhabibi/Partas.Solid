---
title: Plugin spec
---

:::note
The plugin's source is documented heavily. Only short notes are kept here.
:::

`Spec.fs` says what input the plugin accepts, holds the expressions the plugin builds over and over, and holds the
patterns that recognise particular AST nodes. The patterns are grouped in modules named after what they match:
`MemberRef`, `Expr`, `Type`, `EntityRef`, `Ident` and `CallInfo`.

## `FableRequirements`

The plugin declares Fable 5.0 as its minimum version. Fable 4 does not detect it. The repository pins Fable 5.13.0 in
`.config/dotnet-tools.json`, and that is the version the tests run on. The `Language` and `FileExtension` patterns
check that the target is JavaScript and the extension ends in `.js` or `.jsx`.

## `SchemaRules`

`ValidMemberRef` decides whether a member can take `[<SolidTypeComponent>]`. It matches when:

- the declaring type's full name starts with `Partas.Solid`
- the member is an instance member
- it takes only unit, so its arguments are the self identifier and a unit

On a match it returns the declaring type's `DisplayName`, which becomes the component's name, and the name of the self
identifier. The identifier can have any name: `props`, `this`, `someProps`. On a mismatch it logs a warning and
the attribute falls back to the plain transformation.

## `Baked`

Prebuilt expressions that are verbose or used in several places.

- `convertGettersToObject` binds `PARTAS_OTHERS`. With keys, it is `omit(props, "a", "b")`. With no keys, it is the
  props themselves.
- `convertSettersToObject` prepends `props = merge({ ... }, props)` when there are defaults. Keys have their reserved
  suffixes trimmed.
- `renderElement` turns an `ElementBuilder` into a JSX element. It also does one last rewrite: a tag named
  `Fragment` loses its name, so it is written as `<>...</>`.

`omit` and `merge` are imported from `solid-js`. They replaced `splitProps` and `mergeProps` in Solid 2. Here is what
the two produce together, from the `CombinedSpread` snapshot test:

```jsx
export function SimpleCombination(this$) {
    this$ = merge({
        class: "ClassDefault",
    }, this$);
    const PARTAS_OTHERS = omit(this$, "class");
    return <div>
        Some text
        <span class={this$.class}>
            More text
        </span>
        <div {...PARTAS_OTHERS} n$={false}>
            All but class have been spread into this tag!
        </div>
    </div>;
}
```

The member's self identifier was `this`. Fable writes it as `this$`, and the plugin uses that name throughout.

## Recognisers

A few examples of what the pattern modules do:

- `MemberRef.PartasName` returns a member's name from its compiled name, with `get_`, `set_`, `_$ctor` and the
  reserved suffixes cleaned off. It matches any non-generated member reference.
- `MemberRef.MemberRefIs` classifies a member reference as a `MemberRefType`.
- `Expr.ImportedConstructor`, `ImportedSetter` and `ImportedGetter` recognise calls to members declared in other
  modules, which Fable represents as imports.
- `Type.HasPartasImport` and `Type.HasPartasProxyImport` find the attributes that tell the plugin to import a tag
  from a library. `Type.HasPojo` finds `[<Pojo>]`.
- `Ident.IdentIs` classifies an identifier as an `IdentType`, including the component's self identifier.
- `CallInfo.PropertySetter` and `CallInfo.Pojo` recognise property assignments and pojo constructors.
