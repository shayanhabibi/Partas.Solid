# Partas JSX defects: missing information versus discarded information

The failures found in the differential corpus should be fixed. None has been shown to require information missing from Fable's AST. Eighteen additional `DebugMode` probes on Fable 5.21.0 confirm that the relevant distinctions survive into the plugin and are discarded by Partas. Expressions whose results F# discards must still be retained in JSX children under Partas's rendering contract; that behavior is not another defect.

Keep documentation-only restrictions narrow: an opaque callee's unavailable implementation cannot be reconstructed from a caller's AST; the current builder's supported syntax and reserved marker conventions also need an explicit contract. Do not describe ordinary matches, multiple branch children, or local bindings as inherently unsupported.

An impossibility claim would require two semantically different inputs reaching the plugin as indistinguishable complete ASTs, including relevant metadata and enclosing context. Similar generated JavaScript or similar AST subtrees are insufficient. No such complete-AST collision was demonstrated here. The investigation changes no production sources.

## The three observed failure families are fixable candidates

### Union payload access

The stress output transforms `Beta text -> b() { text }` into a child containing the union object, and `text.Length` into the union object's length. The input accessor need not be reconstructed from source spelling. The actual `UnionPayload` AST dump contains `UnionField { Entity = AstTriage.Kind; GenericArgs = []; CaseIndex = 1; FieldIndex = 0 }`, with result type `String`. `UnionLength` retains that node underneath the `length` field access. Fable 5.21.0 models exactly this information in `GetKind.UnionField`. [Pinned Fable AST](https://github.com/fable-compiler/Fable/blob/c1264365a84e462eae491fa3984087816931b7b2/src/Fable.AST/Fable.fs#L703-L731).

Partas' [BuilderCollector](../../src/Partas.Solid.FablePlugin/Plugin.fs) has preservation cases for field and tuple gets around lines 801–819, then a general `Get` case that retains only the transformed receiver. That fallback is a strong candidate for dropping `UnionField` and other accessors. Preserve each `GetKind` unless there is a narrowly proven builder artifact, recursively transforming the receiver and expression index while retaining the original result type and range. `UnionTag`, list head/tail and option access deserve the same adversarial tests. Do not document union payload access as an upstream limitation.

### Several yielded children under one branch or local binding

Partas already holds several collected expressions before its conditional, decision-tree and local-binding cases feed them to `AstUtils.Sequential`. [AstUtils.Sequential](../../src/Partas.Solid.FablePlugin/Utils.fs) explicitly returns a `Sequential` node for a list with multiple members. JavaScript sequencing returns the last member. Thus branch children disappear during Partas' own collection/reassembly; the original problem is not an absence of a second expression.

The implementation must preserve every expression belonging to the JSX child sequence, including expressions whose results F# discards. For example, `log(); span()` can emit both `{log()}` and the span as children; there is no requirement to move the call into a separate statement. A candidate internal representation should preserve child sequences, branch structure and lexical bindings until final JSX assembly. It must preserve evaluation order and binding scope, while keeping ordinary sequencing in guards, property values and other non-child contexts intact.

The `BranchChildren` dump retains `PARTAS_FIRST`, `PARTAS_SECOND`, `PARTAS_ELEMENT`, and `PARTAS_BUILDER` structures; explicit-yield and implicit-yield probes both retain the intended combination. The discarded-element probe instead has a `Sequential` containing `TypeCast(tagExpression, Unit)` before the remaining yielded child. The text-only pair also differs in its enclosing builder lambda/application structure. These are structurally distinct incoming programs, even though current output collapses them. No source-text parsing is needed for these cases.

Fix this with a collector representation that preserves child-sequence context before calling the general `transform`; `BuilderCollectorFeedback` currently transforms first. Preserve lexical `let` scope as well as child order. Grouping child sequences in fragments or arrays is a candidate; a blanket replacement of every `Sequential`, including guards and other ordinary expressions, is not justified. F# discard annotations alone must not exclude an expression from children.

### Empty sequence versus null array entry

In the attribute array-comprehension case, Partas' `ValueUnroller` itself turns a `Seq.empty` call into a null expression and materializes an array. This changes cardinality: `String.concat ":"` sees an extra slot. [Plugin.fs](../../src/Partas.Solid.FablePlugin/Plugin.fs), lines 155–169.

The original sequence operation is available and can simply be preserved if static materialization is not proven safe. A more ambitious optimizer needs explicit zero/one/many-item semantics for conditionals and sequence append. `null` cannot stand for “no item” in an ordinary array. This is an optimization bug, not missing AST information; preserving Fable's original collection expression is a valid correctness-first route.

The `OmittedArrayItem` dump still contains `Seq.delay`, `append`, `singleton`, and `empty`. The matching `OmittedArrayItemSkip` probe, with `ComponentFlag.SkipCEOptimisation`, preserves those operations in emitted JSX. Thus a conservative fallback exists; the compiler does not need to infer the number of runtime items in order to preserve the original expression.

### Intentionally discarded results remain children by design

The new pair is:

```fsharp
div() {
    do produce() |> ignore
    "tail"
}
// versus
div() {
    yield produce()
    "tail"
}
```

Both currently emit `<div>{produce()}tail</div>`, which is expected under Partas's JSX contract. In the incoming `DiscardedCall` dump, the call is wrapped in `TypeCast(..., Unit)` inside a `Sequential`. `YieldedCall` instead binds `PARTAS_ELEMENT` and places it beneath a `PARTAS_YIELD` lambda. This demonstrates available AST distinctions, but those distinctions do not imply different child inclusion here. Preserve the current inclusion behavior when fixing multi-child loss; do not introduce filtering based on F# `do`, `ignore`, or unit casts.

## Information that may require a contract or better markers

### Yield provenance after inlining

The ordinary [Builder.fs](../../src/Partas.Solid/Builder.fs) methods are inline/erased scaffolding. `Yield` returns `fun PARTAS_YIELD -> ignore PARTAS_ELEMENT` (or `PARTAS_TEXT`). `Combine` invokes two container functions in sequence; `Delay` invokes its thunk; `Run` invokes the container function and returns the tag. Explicit and implicit yield both reach these methods, so their original spelling is not inherently a distinction the plugin needs to recover. Their intended child behavior should be identical.

Fable's expression model does not contain dedicated CE `Yield`, `Combine` or `do` variants. Identifier and call metadata exist, but are not a promise that inlining preserves source syntax. [Pinned Fable Expr definitions](https://github.com/fable-compiler/Fable/blob/c1264365a84e462eae491fa3984087816931b7b2/src/Fable.AST/Fable.fs#L789-L848).

Partas intentionally relies on `PARTAS_*` parameter names. [Spec.fs](../../src/Partas.Solid.FablePlugin/Spec.fs), lines 388–429, documents that imported types can yield default parameter names instead. Some recognizers test only name prefixes, others also test a Partas type; generatedness is not uniformly required. This is a fragility boundary that needs DebugMode evidence on the actual Fable tool version, not a universal claim that provenance is absent.

Those paired probes have now been compiled: yielded tag versus `do ignore (tag)`, yielded text versus `do ignore text`, ordinary effects, and implicit versus explicit yields. The differences described above involve AST structure and owned builder markers, not only source ranges. There is no evidence from these probes that a syntax restriction is necessary. If a future compiler inlines away those markers too, change the builder to retain explicit provenance or reject the exact unsupported shape; do not guess from source line positions or result type alone.

### User functions and no-op builder functions

Partas already documents a concrete shape collision in `transformValue`: a user `ignore` function can have the same lambda/cast shape as a disposable builder husk. The attribute-value path deliberately preserves that function, while the general transform removes the husk. [Plugin.fs](../../src/Partas.Solid.FablePlugin/Plugin.fs), lines 893–905 and 978–984. This shows why lexical context matters. It does not show the two complete programs are indistinguishable: an attribute value is meaningful precisely because its enclosing setter/constructor identifies its role.

Similarly, preserve a `unit -> string` callback supplied to a user function. A unit-argument lambda is not necessarily a CE `Delay`. The existing `UnitThunk` and `DelayThunk` recognizers should remain constrained to the operation/context they are normalizing. The user-thunk stress control currently passes, but more nested callbacks should accompany any collector rewrite.

## Restrictions worth documenting

### Attribution does not propagate through opaque helpers

The additional helper-file probe compares an ordinary `OpaqueHelper` with an otherwise similar `[<SolidComponent>] AnnotatedHelper`, both called by attributed functions in another file. The ordinary helper's output still contains erased `Run`/tag-constructor calls and an empty continuation. The annotated helper emits its span; its attributed caller emits an `untrack` call to it.

The caller sees a function call and metadata, not an arbitrary callee's implementation embedded in that expression. Same-file analysis or inline expansion may expose some bodies, but cannot establish a general guarantee for functions in other files, assemblies, JavaScript imports, or supplied callbacks. Document that every helper which directly authors the DSL must itself be transformed, or have its body exposed through a supported inline pattern. Merely returning `HtmlElement` does not prove correct JSX generation. An external function that already returns real JSX needs no reconstruction.

This is a transformation-scope contract, not a restriction on passing ordinary function values. Where metadata proves a suspicious direct call, a diagnostic may help; do not try to reconstruct arbitrary JSX from its return type.

### Builder capabilities and marker conventions

The absence of `For`, `While`, `TryWith`, or collection `Yield` overloads in an ordinary builder is a type-level DSL capability boundary. Document supported syntax; adding those capabilities would be a separate feature, rather than recovery of lost AST information. Likewise document reserved `PARTAS_*` identifiers and supported compiler versions. Missing reliable provenance in manually spoofed or custom builders calls for explicit markers, validation or a narrow restriction.

Dynamic values alone also cannot reveal whether a function is intended as a component constructor, a callback, or already-created JSX. Prefer the existing explicit APIs/markers where that intent is required. This is a design principle, not another reproduced failure in this corpus.

## Semantic contracts that fixes must uphold

- Explicit and implicit yields have the same rendering semantics; several yielded values in a branch must all survive in order.
- Preserve effectful and F#-discarded expressions in JSX children, in source order. Do not filter children based on `do`, `ignore`, or a unit cast. A genuinely unit-valued expression may render no content, but it must remain present and execute.
- Local bindings retain lexical scope and do not discard following sibling children.
- Arbitrary attribute values retain ordinary F# semantics, including collection cardinality and callable values. CE cleanup must not treat those values as builder scaffolding.
- Reserve `PARTAS_*` names as compiler scaffolding; do not promise correctness for manually spoofed builder internals. Prefer stable markers/member identity over name-only heuristics in future designs.
- Direct `for`, `while`, `try/with` and collection yielding inside an ordinary element CE require matching builder members; absence of such members is a DSL capability boundary, not malformed emitted JSX. Equivalent ordinary F# constructs outside the CE are a different supported compilation question.
- If a genuine indistinguishable AST pair is demonstrated, document that exact narrow syntax combination and compiler version. A builder change that retains provenance is an alternative to declaring the behavior permanently unsupported.

## Fix order and temporary workarounds

1. Preserve `GetKind`, especially `UnionField`. This is a narrow, well-evidenced data-loss fix; cover nested access, union case/field indices, and guard/attribute variants.
2. Stop unsafe attribute collection materialization when zero/one/many cardinality is not preserved. Keeping the original sequence expression is preferable to a speculative rewrite. Test true and false branches, explicit null items, multiple and zero yielded items, and side effects.
3. Redesign builder collection around child sequences, bindings and branches. Fix multi-child loss while retaining effectful and F#-discarded expressions as children. Test effects before/between/after children, explicit `do`/`ignore`, text and element pairs, nested scopes, decision trees, and existing render-function children. Keep reactive evaluation behavior under test too; this investigation only establishes the sampled expression behavior.

The compiled workaround probes show that a single explicit `Fragment() { ... }` containing the branch's siblings emits both children, and moving a local binding outside the CE avoids the local-binding failure. `SkipCEOptimisation` preserves the array-comprehension expression. These are narrow temporary workarounds, not reasons to declare the underlying constructs unsupported. Guarded matches/attribute use happen to avoid some union failures, but should not be advertised as a stable workaround for a dropped accessor.

## Reproducing the AST investigation

With the prerequisites from [tools/JsxStress/README.md](../../tools/JsxStress/README.md), run `rtk proxy node tools/JsxStress/triage.mjs`. It compiles eighteen `DebugMode` probes from [triage.fs.template](../../tools/JsxStress/triage.fs.template) plus a helper file, records full pre-transform dumps under `.research/ast-triage`, and writes [AST excerpts and emitted evidence](solid-jsx-ast-evidence.json). The recorded compilation exits zero. Excerpts preserve nearby metadata but are not a complete-AST equivalence checker.
