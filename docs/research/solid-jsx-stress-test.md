# Partas.Solid and Oxpecker.Solid: differential JSX stress test

Partas handles substantially more of this corpus correctly, but the comparison also found concrete cases where Oxpecker generates correct JSX and Partas changes the program's meaning. The most serious Partas defects are lost children in multi-child branch/local-binding bodies, lost discriminated-union payload access, and an extra array entry introduced by attribute-expression optimization.

## Reproducible result

The run on 15 October 2026 used **Fable 5.21.0 for both libraries**, with identical optimization and JSX-output flags. It tested **57 F# cases and 112 input scenarios per library**. A case passes only when every scenario passes.

- Partas: **42/57 cases**, **94/112 scenarios** pass.
- Oxpecker: **27/57 cases**, **67/112 scenarios** pass.
- Both pass 23 cases; Partas alone passes 19; Oxpecker alone passes four; both fail 11.
- These are targeted stress-test counts, not a representative estimate of application compatibility.

“Pass” requires valid parsable JSX, the expected inert element tree and attribute values, expected side-effect logs, and no recognized CE plumbing left in the emitted source. Oxpecker has ten failing scenarios whose selected tree/logs are correct but whose file still contains CE markers. Those are kept separate from observed loss of content below. Partas has no marker-only failures in this run.

The recorded baseline hashes are Partas [`a2b9f9aeed1a41606b34896e13dcf481b396380a`](https://github.com/shayanhabibi/Partas.Solid/tree/a2b9f9aeed1a41606b34896e13dcf481b396380a) and Oxpecker `develop` [`369cda210b630ba7f85ea56bf6e84d20510ab943`](https://github.com/Lanayx/Oxpecker/tree/369cda210b630ba7f85ea56bf6e84d20510ab943). Node was `v26.7.0`; metadata timestamp was `2026-10-15T02:53:13.744Z`. The comparison uses checked-out source projects, so the Partas hash identifies the baseline, not a guarantee of a clean working tree. Partas compilation exited zero; Oxpecker compilation exited one because five isolated case files were explicitly rejected by its plugin. Other emitted files were checked individually.

## What the test does and does not establish

The same templates are compiled after changing only the opened library namespace and project reference. The cases use shared tags, ordinary F# expressions, and shared attributes; they do not use version-specific Solid APIs. Array/list/sequence cases return collections rather than attempting to yield collections into an unsupported ordinary element CE.

The checker parses generated JSX with Babel, replaces tags with inert tree construction, resolves actual Fable helpers, executes representative inputs, and checks trees, attributes and logs. It rejects remaining erased-DSL imports and child functions/objects. This deliberately avoids making either Solid version the comparison's oracle. It establishes output syntax and ordinary expression semantics; it does not establish reactive scheduling or hydration behavior.

Ten additional checks compile selected emitted files with the real Solid 2 rc.14 JSX compiler and render in jsdom: **five controls pass and five known-bug assertions fail as expected** (`it.fails`). These confirm Partas' conditional-child loss, local-binding child loss, incorrect union payload, and extra array entry, plus Oxpecker's discarded statement/child. Controls confirm hand-written JSX, Oxpecker's correct payload and array-comprehension behavior, and Partas' retained statement. Oxpecker's leaked empty false-branch function renders empty under this runtime, confirming that this particular leakage is not visible DOM damage. This uses only shared intrinsic JSX, not either library's version-specific Solid bindings. See [runtime.test.jsx](../../tools/JsxStress/runtime.test.jsx).

Initial exploratory cases that were invalid shared F# input were discarded or corrected before the final corpus. In particular, arrays/collections are not ordinary `HtmlElement` CE children, and a one-line `if ... then child; sibling` can put the semicolon-separated sibling inside the conditional branch. The final sibling probes use multiline indentation. A curried-return-function probe is interpreted using Fable's parameter flattening. Stale emitted files are deleted before compilation, so rejected cases cannot accidentally reuse earlier output.

## Where Oxpecker succeeds and Partas fails

### Union payloads become the union object

Minimal form, with `type Kind = Alpha | Beta of string | Other`:

```fsharp
[<SolidComponent>]
let MatchOnly (kind: Kind) =
    div() {
        match kind with
        | Alpha -> span() { "a" }
        | Beta text -> b() { text }
        | Other -> i() { "o" }
    }
```

The relevant Partas output is `<b>{kind}</b>`; Oxpecker emits `<b>{kind.fields[0]}</b>`. For `Beta "hello"`, the former passes the union object as the child instead of its string payload. The independent oracle rejects that object.

`UnionPayloadNested` repeats the error through nested tags. `UnionPayloadLength` emits `kind.length` rather than the payload's length, yielding an empty span instead of `2` for `Beta "xy"`. Oxpecker passes all scenarios for these three cases.

This is context-sensitive: `MatchGuard`, `UnionPayloadAttribute`, and the simple attribute union match pass in both libraries. Giving the payload a title attribute causes Partas to retain `text = kind.fields[0]`, while the plain child access is lost. The likely source boundary is Partas' builder `Get` reduction: its fallback drops an accessor and retains the underlying expression. This is a source-based diagnosis to verify while fixing, not a complete traced-AST proof. See [Plugin.fs](../../src/Partas.Solid.FablePlugin/Plugin.fs), `BuilderCollector` around lines 801–819.

### An omitted array-comprehension item becomes a null item

```fsharp
[<SolidComponent>]
let AttributeArrayComprehension (flag: bool) =
    div(title = String.concat ":" [|
        yield "head"
        if flag then yield "yes"
        yield "tail"
    |]) { "body" }
```

Partas emits the equivalent of:

```jsx
<div title={join(":", ["head", flag ? "yes" : null, "tail"])}>body</div>
```

With `false`, the title is `head::tail`; the correct title is `head:tail`. Oxpecker preserves the sequence computation and produces the correct result for both inputs. Partas' value unroller replaces `Seq.empty` with a null expression while materializing the array; an omitted item and a present null item have different array semantics. See [Plugin.fs](../../src/Partas.Solid.FablePlugin/Plugin.fs), `ValueUnroller` around lines 155–169. Direct conditional array elements and union matches inside arrays pass; this failure specifically involves conditional omission in a comprehension.

These are the four complete-case reverse wins: `MatchOnly`, `UnionPayloadNested`, `UnionPayloadLength`, and `AttributeArrayComprehension`.

## Where both fail: several children collapse to the final child

Partas emits clean, parsable JSX for these cases, but uses JavaScript sequence/comma expressions where the program needs several children:

```fsharp
[<SolidComponent>]
let IfTwoElements (flag: bool) =
    div() {
        if flag then
            span() { "one" }
            b() { "two" }
        else
            i() { "three" }
            strong() { "four" }
    }
```

Partas emits the equivalent of:

```jsx
<div>{flag ? (<span>one</span>, <b>two</b>)
           : (<i>three</i>, <strong>four</strong>)}</div>
```

Only `b` or `strong` becomes the child. The earlier node is evaluated and discarded. Oxpecker leaves inlined CE constructor/continuation code in the same branch bodies instead of transforming them into JSX.

The same Partas loss occurs in `IfElementText`, `IfTextElement`, `IfLocalTwoElements`, `MatchTwoElements`, `UnionPayloadSiblings`, `UnionPayloadLocal`, and the long-payload branch of `UnionPayloadGuarded`. A local binding can trigger it without any conditional:

```fsharp
div() {
    "head"
    let upper = text.ToUpper()
    span() { upper }
    "tail"
}
```

Partas emits a child like `(upper = text.toLocaleUpperCase(), (<span>{upper}</span>, "tail"))`, leaving `headtail` and losing the span. `LetThreeChildren` loses both earlier children; `NestedLocalTwoChildren` loses an inner child. Oxpecker explicitly rejects those local-binding cases.

The Partas source explains the common shape: branch and local-binding collection wraps a list of collected expressions in `AstUtils.Sequential`. Fable then emits that as a sequence expression whose value is its last item. A correct fix must preserve the complete JSX child sequence, including effectful expressions and results F# discards: `log(); element` can emit `{log()}` followed by the element, and two yielded elements must both survive. Keep ordinary sequencing intact in non-child contexts. See [Plugin.fs](../../src/Partas.Solid.FablePlugin/Plugin.fs), `BuilderCollector` conditional and `Let`/decision-tree handling around lines 787–845.

The 11 complete cases failing in both libraries are `LetSibling`, `UnionPayloadSiblings`, `UnionPayloadLocal`, `UnionPayloadGuarded`, `MatchTwoElements`, `IfTwoElements`, `IfElementText`, `IfTextElement`, `IfLocalTwoElements`, `LetThreeChildren`, and `NestedLocalTwoChildren`.

## Where Partas succeeds and Oxpecker fails

### JSX inside collections and protected expressions

For `items |> Array.map (fun text -> span() { text })`, Partas emits a callback returning `<span>{text}</span>`. Oxpecker leaves `HtmlContainerExtensions_Run_...` and `span_$ctor()` in the callback, with an empty continuation body. The same failure family covers list mapping, array/list literals, sequence expressions, array comprehensions, and an array with conditional JSX elements. `try/with` and `try/finally` around tags also retain erased DSL construction in Oxpecker; Partas emits the expected JSX and preserves finalizer logging.

This matches Oxpecker's main traversal: it transforms `Let` bodies but not their values, only the final expression of `Sequential`, `Call` arguments but not general lambdas/collections, and a small set of branches/casts. Partas traverses these additional AST structures. [Oxpecker transform](https://github.com/Lanayx/Oxpecker/blob/369cda210b630ba7f85ea56bf6e84d20510ab943/src/Oxpecker.Solid.FablePlugin/Library.fs#L506-L535).

### Statements silently disappear

`div() { log "before"; span() { "yes" } }` becomes `<div />` in Oxpecker, losing both the side effect and tag. Putting a log between children retains only the first child; putting it after the child retains the child but drops the log. Partas preserves all three scenarios. Statementful single-result conditional and match branches pass in both libraries, so this is specifically a CE collection boundary rather than all sequencing.

### Attribute quotes produce invalid JSX

For a title containing a double quote, line break, `<` and `&`, Oxpecker writes the string directly into a quoted JSX attribute. The embedded quote terminates the attribute; Babel rejects the output. Partas emits a JavaScript string expression, preserving the value safely. This is a syntax failure independent of Solid's version or runtime.

### Explicitly rejected local bindings

Oxpecker rejects `LetText`, `LetSibling`, `LetElement`, `LetThreeChildren`, and `NestedLocalTwoChildren`. Its diagnostic asks authors of explicit tag bindings to move them to a separate `SolidComponent`. These are supported-input gaps, recorded separately from silently wrong successful emission. Partas passes `LetText` and `LetElement`; the other three compile but lose children as described above. [Upstream explicit rejection](https://github.com/Lanayx/Oxpecker/blob/369cda210b630ba7f85ea56bf6e84d20510ab943/src/Oxpecker.Solid.FablePlugin/Library.fs#L490-L508).

### False-branch CE husks

Oxpecker's `IfOnly`, `IfBeforeSibling`, `IfAfterSibling`, and `ExplicitYield` leave a false branch such as `(value) => {}`. Partas uses `undefined`. The strict checker rejects function children or any recognized CE markers in the file, including unselected branches. This records the requested CE leakage, but does **not** establish visible content loss for a false branch: Solid may evaluate an empty function child to an empty result. The substantial content-loss cases above do not depend on this stricter criterion.

Simple `if/else`, nested conditionals, guarded matches and tuple matches already work in Oxpecker. Its existing [Branches fixture and expected JSX](https://github.com/Lanayx/Oxpecker/tree/369cda210b630ba7f85ea56bf6e84d20510ab943/tests/Oxpecker.Solid.Tests/Cases/Branches) also demonstrate that history should not be summarized as a blanket inability to compile conditional CE children.

## Running it again

The retained harness is [tools/JsxStress](../../tools/JsxStress). It requires the .NET 10 SDK, Node, RTK, the Oxpecker checkout at `.research/Oxpecker`, and the local Fable tool at `.research/tools/fable`. Babel is resolved from `tests/Partas.Solid.Tests.Runtime/node_modules` using that project's dependency versions. With those prerequisites installed, run from the repository root:

```powershell
rtk proxy dotnet tool install fable --tool-path .research/tools --version 5.21.0
rtk proxy npm ci --prefix tests/Partas.Solid.Tests.Runtime
rtk proxy node tools/JsxStress/run.mjs
```

For a fresh upstream checkout, clone `https://github.com/Lanayx/Oxpecker.git` to `.research/Oxpecker` and check out the pinned hash above. Skip tool installation when that version is already installed. To recheck existing JSX without compilation, run `rtk proxy node tools/JsxStress/check.mjs`.

`prepare.mjs` generates separate files from `cases.fs.template` and `extra.fs.template`, isolating plugin exceptions to individual files. `run.mjs` clears old case output, invokes the shared Fable tool for both projects with `--noCache -e .fs.jsx --optimize --typedArrays false`, records exit codes and hashes, and invokes the checker. Generated JSX, complete compile logs, metadata and per-input results are under `.research/jsx-stress/`; that directory is local generated evidence, not a shipped dependency. The retained templates/checker preserve the reproducible corpus.

The complete recorded inputs, outputs, per-scenario assertions, compile logs and grouping are retained in [solid-jsx-stress-evidence.json](solid-jsx-stress-evidence.json). Recreate this snapshot with `rtk proxy node tools/JsxStress/snapshot.mjs`. Run the DOM confirmation with `rtk proxy node tests/Partas.Solid.Tests.Runtime/node_modules/vitest/vitest.mjs run --config tools/JsxStress/vitest.config.mjs`.

The highest-priority Partas regression tests are union payload access, multiple yielded children under a branch or local binding, and conditional omission inside an attribute array comprehension. Fixes should preserve both children and side-effect order; removing visible CE markers alone would miss the demonstrated Partas failures.
