---
title: Comparing JSX with Oxpecker
---

Partas.Solid builds on Oxpecker.Solid's DSL. One practical difference is how much ordinary F# the plugin can
translate inside and around element builders. The examples below show the actual input and generated output
from a differential run on 15 October 2026.

Both libraries used **Fable 5.21.0**, with `--noCache -e .fs.jsx --optimize --typedArrays false`. Oxpecker was
pinned to [commit 369cda2](https://github.com/Lanayx/Oxpecker/tree/369cda210b630ba7f85ea56bf6e84d20510ab943);
Partas used this repository's working sources for the unreleased 3.0 version. These findings describe those
sources and compiler settings, rather than every release of either library.

The same 57 F# cases, with 112 input scenarios per library, produced:

- **Partas: 57/57 cases and 112/112 scenarios passed.**
- **Oxpecker: 27/57 cases and 67/112 scenarios passed.**
- Both passed 27 cases; Partas alone passed the other 30.

This is a targeted correctness corpus, not a performance benchmark or an estimate of application compatibility.
A pass checks JSX syntax, element trees, attribute values, side-effect logs, and remaining builder plumbing.
The corpus uses shared intrinsic tags and F# expressions, without version-specific Solid APIs. It does not test
reactive scheduling or hydration. Separate checks with the Solid 2 rc.14 compiler and jsdom pass nine assertions,
with one expected Oxpecker failure.

## Keep statements and the children after them

A statement before a child should execute and leave that child in the output. In this case, Partas preserves
both; Oxpecker emits an empty element.

::::tabs
:::tab F# input
```fsharp
[<SolidComponent>]
let StatementBefore (log: string -> unit) = div() { log "before"; span() { "yes" } }
```
:::
:::tab Partas output
```jsx
export function StatementBefore(log) {
    return <div>
        {log("before")}
        <span>
            yes
        </span>
    </div>;
}
```
:::
:::tab Oxpecker output
```jsx
export function StatementBefore(log) {
    return <div />;
}
```
:::
::::

The oracle observes `<div><span>yes</span></div>` and the log `["before"]` from Partas. Oxpecker produces
`<div></div>` and no log. The Solid 2 DOM check confirms the missing content and effect in this example.

## Build elements inside collection callbacks

An ordinary `Array.map` can return JSX. Partas transforms the callback's element builder into a `span`;
Oxpecker leaves the erased DSL constructor and builder call in the output.

::::tabs
:::tab F# input
```fsharp
[<SolidComponent>]
let ArrayMap (items: string[]) = items |> Array.map (fun s -> span() { s })
```
:::
:::tab Partas output
```jsx
export function ArrayMap(items) {
    return map((s) => <span>
        {s}
    </span>, items);
}
```
:::
:::tab Oxpecker output
```jsx
export function ArrayMap(items) {
    return map((s) => HtmlContainerExtensions_Run_606A7C2E(span_$ctor(), (cont) => {
    }), items);
}
```
:::
::::

For `[| "one"; "two" |]`, Partas returns `<span>one</span><span>two</span>`. The independent checker rejects
Oxpecker's remaining DSL import. This comparison checks ordinary array construction; for reactive list rendering,
use Solid's [For bindings](../guide/solid-js.md).

## Preserve every child in a branch

A branch can yield several siblings. Partas groups them in JSX fragments, keeping the complete selected branch.
The captured Oxpecker output retains builder continuations and constructor calls instead of those JSX children.

::::tabs
:::tab F# input
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
:::
:::tab Partas output
```jsx
export function IfTwoElements(flag) {
    return <div>
        {flag ? (<>
            <span>
                one
            </span>
            <b>
                two
            </b>
        </>) : (<>
            <i>
                three
            </i>
            <strong>
                four
            </strong>
        </>)}
    </div>;
}
```
:::
:::tab Oxpecker output
```jsx
export function IfTwoElements(flag) {
    let first, element, second, element_1, first_1, element_2, second_1, element_3;
    return <div>
        {flag ? ((first = ((element = HtmlContainerExtensions_Run_606A7C2E(span_$ctor(), (cont) => {
        }), (cont_1) => {
        })), (second = ((element_1 = HtmlContainerExtensions_Run_606A7C2E(b_$ctor(), (cont_2) => {
        }), (cont_3) => {
        })), (builder) => {
            first(builder);
            second(builder);
        }))) : ((first_1 = ((element_2 = HtmlContainerExtensions_Run_606A7C2E(i_$ctor(), (cont_4) => {
        }), (cont_5) => {
        })), (second_1 = ((element_3 = HtmlContainerExtensions_Run_606A7C2E(strong_$ctor(), (cont_6) => {
        }), (cont_7) => {
        })), (builder_1) => {
            first_1(builder_1);
            second_1(builder_1);
        })))}
    </div>;
}
```
:::
::::

Partas produces `<div><span>one</span><b>two</b></div>` for `true`, and
`<div><i>three</i><strong>four</strong></div>` for `false`. Oxpecker fails this case's shared-output checks.
Simple single-child conditionals and guarded matches do pass in both libraries; this example concerns multiple
children within a branch.

## Keep local bindings with following siblings

Local values can be calculated between children without extracting another component. Partas keeps the binding
and all three children. The pinned Oxpecker plugin explicitly rejects this input.

::::tabs
:::tab F# input
```fsharp
[<SolidComponent>]
let LetSibling (text: string) =
    div() {
        "head"
        let upper = text.ToUpper()
        span() { upper }
        "tail"
    }
```
:::
:::tab Partas output
```jsx
export function LetSibling(text) {
    let upper;
    return <div>
        head
        {(upper = text.toLocaleUpperCase(), <>
            <span>
                {upper}
            </span>
            {"tail"}
        </>)}
    </div>;
}
```
:::
:::tab Oxpecker diagnostic
```text
`let` binding inside HtmlElement can't be converted to JSX:line 11
```
:::
::::

For `"hello"`, Partas produces `<div>head<span>HELLO</span>tail</div>`. Oxpecker emits no case file after this
diagnostic. The Solid 2 DOM check also confirms that Partas retains the local-binding example's children.

## Escape attribute values as JavaScript strings

An attribute containing quotes, a newline, `<`, and `&` needs to survive JSX parsing with its value intact.
Partas emits a JavaScript string expression. Oxpecker places the value directly inside JSX quotes, producing
invalid JSX in this case.

::::tabs
:::tab F# input
```fsharp
[<SolidComponent>]
let AttributeQuotes () = div(title = "a\"b\n<&") { "body" }
```
:::
:::tab Partas output
```jsx
export function AttributeQuotes() {
    return <div title={"a\"b\n<&"}>
        body
    </div>;
}
```
:::
:::tab Oxpecker output
```jsx
export function AttributeQuotes() {
    return <div title="a"b
<&">
        body
    </div>;
}
```
:::
::::

The checker preserves Partas's exact title value and rejects Oxpecker's output during JSX parsing. This failure
occurs before a Solid runtime is involved.

## Inspect or reproduce the comparison

The F# blocks are shared inputs: open `Partas.Solid` or `Oxpecker.Solid` for the respective compilation. Output
blocks reproduce the emitted case functions; imports and unused union/reflection declarations are omitted.
Helpers such as `map` remain actual Fable imports. The diagnostic is the captured compiler message, without its
machine-specific file prefix.

The repository retains the [harness and commands](https://github.com/shayanhabibi/Partas.Solid/tree/master/tools/JsxStress)
to regenerate the comparison. Inputs live in the harness templates; generated JSX, compiler logs, per-scenario
results, and optional evidence snapshots stay in the gitignored `.research/` directory.

Partas also supports reactive [`match!`](../guide/match-expression.md), with
separate snapshot and runtime coverage. It is outside this shared comparison corpus and is not included in the
counts above.
