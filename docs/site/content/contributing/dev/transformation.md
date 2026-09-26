---
title: Plugin transformation
---

:::note
The plugin's source is documented heavily. Every pattern has a comment on what it does and why.
:::

`Plugin.fs` holds the transformation logic and the two public attributes. It is the heart of the package.

## Entry points

Both attributes are Fable `MemberDeclarationPluginAttribute`s. Fable hands each one the declaration it is attached
to, and the attribute returns a rewritten one. Each has a constructor taking a `ComponentFlag`, and one taking an
`int`.

### `[<SolidComponent>]`

For `let` bindings. It creates a `PluginContext`, prints the AST if `DebugMode` is set, and runs `AST.transform` over
the body. Nothing else: there are no props to `omit` or `merge`.

### `[<SolidTypeComponent>]`

For members of a type, like `member props.View = ...`. It:

1. prints the AST if `DebugMode` is set
2. checks the member with `SchemaRules.ValidMemberRef` (see [Spec](spec.md)), and stores the self identifier on the
   context
3. runs `AST.transform` over the body, which collects every getter and setter it finds on the self identifier
4. unless `SkipOmit` is set, wraps the body in `const PARTAS_OTHERS = omit(props, ...)` with the distinct getters
5. puts `props = merge({ ... }, props)` in front, if there were any setters
6. renames the member to the declaring type's `DisplayName`, so `Badge.View` is written as `function Badge(props)`

The `merge` goes first in the output, so defaults are in place before `omit` reads the props. If the member does not
match the schema, the attribute warns that you should use `[<SolidComponent>]`, and only transforms the body.

## `transform`

`transform` is the first, and main, pass. Its job: reduce the AST where that is reasonable, find tags and expand them,
and drop expressions only when there is no other choice. It matches, in order:

1. `Emit` expressions, whose arguments are transformed before they are injected. Fable uses these for operators like
   `&&=`.
2. tag values and their `.render`
3. special attribute transformations, such as pojo constructors
4. `TagConstructor`, which is collected with `collectTagInfo` and rendered with `Baked.renderElement`
5. `Sequential` expressions, transformed one by one
6. `PropsGetterOrSetter`, which records a props read or default
7. calls to imported getters, rewritten to field reads
8. `IfThenElse` and `DecisionTree`, transformed branch by branch
9. an `AttributeExpression` at the top level. This should not happen, so it is logged as a warning and reduced to its
   value.
10. everything else that can contain expressions: lambdas, delegates, `let`s, calls, applications, values (arrays,
    lists, records, tuples, unions, string templates), operations, `Get`s and object expressions. Each is rebuilt
    with its parts transformed.

Most of this is picking up an expression that contains other expressions, transforming those, and putting the
structure back together.

## Following a component through

Take this component:

```fsharp
[<Erase>]
type Badge() =
    inherit span()

    [<Erase>]
    member val tone: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        props.tone <- "info"
        span(class' = "badge " + props.tone).spread props { props.children }
```

- `props.tone <- "info"` matches `PropsGetterOrSetter` as a setter. `("tone", "info")` is recorded and the line is
  removed.
- `span(...)...{ ... }` is a `Run` call with a tag as its first argument, so `TagConstructor` matches it.
  `PropCollector` collects `class` from the constructor. `AttributeExpression` turns `.spread props` into
  `{...PARTAS_OTHERS} n$`. `BuilderCollector` collects `props.children` as the child.
- The reads of `props.tone` and `props.children` are recorded as getters.
- Last, the attribute wraps the result with `omit(props, "tone", "children")` and `merge({ tone: "info" }, props)`,
  and renames the function to `Badge`.

The result:

```jsx
export function Badge(props) {
    props = merge({
        tone: "info",
    }, props);
    const PARTAS_OTHERS = omit(props, "tone", "children");
    return <span class={"badge " + props.tone}
        {...PARTAS_OTHERS} n$={false}>
        {props.children}
    </span>;
}
```

## When something goes missing

Wherever the transformation drops an expression, it calls `PluginContext.debugDisposal`. Compile the component with
`ComponentFlag.PrintDisposals` and each drop is logged as a warning with the stage it happened in. See
[Submitting issues](../submit-issues.md).
