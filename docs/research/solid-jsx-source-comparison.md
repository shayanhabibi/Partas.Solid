# JSX transformation comparison: source reconnaissance

This note records source evidence and candidate differential tests. It does **not** label an uncompiled candidate a reproduced defect. The accompanying stress harness is responsible for compiling with the same Fable version and checking the emitted JSX.

## Sources and scope

Oxpecker checkout: `Lanayx/Oxpecker`, `develop`, commit [`369cda210b630ba7f85ea56bf6e84d20510ab943`](https://github.com/Lanayx/Oxpecker/tree/369cda210b630ba7f85ea56bf6e84d20510ab943). Partas base checkout: `a2b9f9aeed1a41606b34896e13dcf481b396380a`, plus the working tree. The comparison concerns expression-to-JSX transformation, excluding Solid 1/2 binding differences.

The user asked for latest Fable on both sides. The parent investigation resolved and installed stable Fable **5.21.0** for the shared harness; this note itself runs no builds.

## What the sources establish

### Simple conditionals are already covered upstream

Oxpecker's committed `Branches` fixture contains nested pattern matches, a guard, and an `if/else` inside a tag CE. Its expected output has JSX branches, including the final conditional child. Thus the historical conditional limitation cannot honestly be described as “Oxpecker cannot compile if/else inside CEs.” The interesting boundary is a branch combined with siblings, statements, local bindings, or a different enclosing AST node. [Fixture](https://github.com/Lanayx/Oxpecker/blob/369cda210b630ba7f85ea56bf6e84d20510ab943/tests/Oxpecker.Solid.Tests/Cases/Branches/Branches.fs), [snapshot](https://github.com/Lanayx/Oxpecker/blob/369cda210b630ba7f85ea56bf6e84d20510ab943/tests/Oxpecker.Solid.Tests/Cases/Branches/Branches.expected).

### Oxpecker traverses substantially fewer AST shapes

Oxpecker `AST.transform` handles tag construction, `Let` **body only**, the **last** member of `Sequential`, `Call` **arguments only**, text, branches, and casts to declared types. It returns all other expressions unchanged. This is direct source evidence, not proof that every such node survives Fable optimizations in a reproducer. [Library.fs lines 506–535](https://github.com/Lanayx/Oxpecker/blob/369cda210b630ba7f85ea56bf6e84d20510ab943/src/Oxpecker.Solid.FablePlugin/Library.fs#L506-L535).

Partas' main traversal additionally descends into lambdas/delegates, curried applications, `Let` values, every sequential member, call receivers/callees, arrays/lists/options/tuples/records/unions/templates, operations, indexed/field access, object members, tests, loops, `try/catch/finally`, recursive bindings, and assignments. It also descends into branch guards and decision-tree bound values. [Plugin.fs](../../src/Partas.Solid.FablePlugin/Plugin.fs), main transform around lines 935–1114. These differences motivate tests placing JSX **inside** each shape rather than merely around it.

### CE collection is a separate transformation boundary

Oxpecker `getChildren` matches a finite set of inlined builder shapes using names such as `element`, `first`, `second`, `cont`, `builder`, `text`, and `matchValue`. Its branch cases call `transform` on branch bodies rather than recursively applying the CE collector. Unknown nodes can silently contribute no children, while unrecognized `let` nodes can throw. [Library.fs lines 296–413](https://github.com/Lanayx/Oxpecker/blob/369cda210b630ba7f85ea56bf6e84d20510ab943/src/Oxpecker.Solid.FablePlugin/Library.fs#L296-L413).

Partas' `BuilderCollector` recursively removes builder lets, lambdas, applications and casts. Its `IfThenElse` and decision-tree target cases collect their branch bodies; a general `Let` preserves the value and collected body. It has special treatment for statementful iterator callbacks. [Plugin.fs](../../src/Partas.Solid.FablePlugin/Plugin.fs), lines 711–846. Its committed `BuilderStatements` fixture explicitly covers a callback local binding and logging before a returned tag, plus logging before a match-branch tag. [Fixture](../../tests/Partas.Solid.Tests.Plugin/Compiled/SolidCases/Statements%20before%20children%20in%20builders%20are%20kept/BuilderStatements.fs).

### Local tag bindings are intentionally rejected upstream

Oxpecker has an `ExplicitLetBinding` recognizer that fails with “let binding to HtmlElement can't be converted to JSX” and asks the author to move the value into a separate `SolidComponent`. This is an explicit unsupported input, distinct from a successful compile that emits malformed JSX. [Library.fs lines 490–508](https://github.com/Lanayx/Oxpecker/blob/369cda210b630ba7f85ea56bf6e84d20510ab943/src/Oxpecker.Solid.FablePlugin/Library.fs#L490-L508). Partas has committed fixtures named “Local tag values render through the tag” and “TagsAsValuesSimple.”

### Attribute extension arguments are another traversal boundary

Oxpecker `collectAttributes` directly retains handlers/values for `.on`, `.bool`, `.data`, `.attr`, `.ref`, `.style'`, `.classList`, and mutable field setters. Property setters transform only a value cast to `HtmlElement` or a `Delegate` body; other values are retained. Compare JSX passed directly to a property, nested in a callback, and supplied via `.attr`, using an imported component with a callback prop where possible. [Library.fs lines 240–287](https://github.com/Lanayx/Oxpecker/blob/369cda210b630ba7f85ea56bf6e84d20510ab943/src/Oxpecker.Solid.FablePlugin/Library.fs#L240-L287).

## Suggested stress corpus

1. `div() { if flag then span(){"yes"} else i(){"no"} }` as a control, followed by the same condition before, after, and between static tag/text siblings.
2. `if flag then ...` without `else`, with both flag values and siblings on either side. Check empty children become a valid empty value rather than a leaked function/CE continuation.
3. `match` with a guard, nested match, and shared targets; add a logging statement and a local string binding before a branch's tag. Verify both tag presence and side-effect order.
4. A local non-tag binding inside a CE, then a child using it. Distinguish explicit unsupported input from silent disappearance.
5. JSX inside `Array.map (fun x -> span(){x})`, returned callback, local function, tuple, option, record/anonymous record, array, and list. Verify element construction becomes JSX throughout the container.
6. JSX inside a `try/with` expression or an object-expression method **outside the element CE**. Verify both success and catch/method branches are transformed.
7. Attribute/callback values containing tags; compare constructor property with an extension method. Use callbacks that are semantically expected to produce JSX, rather than event handlers discarding a tag.
8. A local tag value later returned/rendered. Record upstream's explicit rejection separately from malformed successful output.
9. Empty CE, deeply nested tags, explicit yields, branch expressions containing text versus tags, and names similar to builder temporaries. These test recognizer fragility with latest Fable inlining.
10. Static attribute strings containing double quotes, ampersands, braces and line breaks. Parse the resulting JSX and check decoded values; a snapshot alone can ratify bad escaping.

Neither shared ordinary element builder defines `For`, `While`, `TryWith`, or `TryFinally`; tests placing those constructs directly in the element CE would measure unsupported F# syntax instead of plugin correctness. Oxpecker's supported builder methods are `Combine`, `Zero`, `Delay`, `Yield`, and `Run`. [Builder.fs](https://github.com/Lanayx/Oxpecker/blob/369cda210b630ba7f85ea56bf6e84d20510ab943/src/Oxpecker.Solid/Builder.fs).

## Oracle guidance

A CE continuation such as `cont => {}`, `builder => ...`, ignored tag casts, or null `HtmlElement` values in a rendered child is suspicious. Parse success is insufficient: JSX can contain legal JavaScript returning a function rather than an element. For reproduced cases, inspect normalized JSX structure and evaluate representative inputs, recording retained text/tags and side-effect order. Normalize Fable helper names/import locations only; do not normalize away extra wrappers, lost siblings or changed evaluation order.

No source-only claim here establishes a reverse case where Oxpecker succeeds and Partas fails. The larger traversal surface in Partas is a reason to stress semantics, especially preservation of user thunks, `ignore`, list conversions, and statements. Partas' committed regression names for thunk arguments, list conversions and decision trees make those useful adversarial controls, not evidence of a current defect.
