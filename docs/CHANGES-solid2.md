# Usage changes on `solid2rc`

Changes in **usage** between the last release (`2.1.3`, 2025-10-10) and the `solid2rc` branch.

This branch retargets Partas.Solid from **solid-js 1.x to solid-js 2.0**. Solid 2.0 renamed, removed, and
restructured a large part of its own runtime API, so most of what follows is that change surfacing through the
bindings — it is not gratuitous churn. Expect to touch every non-trivial component.

> **Status:** the branch is a release candidate built from `wip:` commits. Several plugin test cases are currently
> stubbed with `failwith "redo"` (effects/resources in `OperatorsInProps`, `SignalSetterInvoke`,
> `ThisArgTransforms`, `ExperimentalBuilders`) pending rework against the new API. Treat anything reactive beyond
> signals/memos as still in flux.

---

## 1. `SolidTypeComponent` self identifier is no longer restricted to `props`

Previously the plugin hard-matched the literal name `props` and errored out on anything else:

> "The self identifier must be named `props`, no arguments must be provided"

The self identifier is now captured from the declaration and threaded through the transformation via
`PluginContext.SelfIdentifier`. **Any name works**, and the name you choose is the name that appears in the emitted
JSX.

```fsharp
[<Erase>]
type Button() =
    interface RegularNode
    member val size: size = unbox null with get, set
    member val variant: variant = unbox null with get, set

    [<SolidTypeComponent>]
    member someProps.constructor =        // <- was required to be `props`
        button(class' = button.variants {| size = someProps.size; variant = someProps.variant |})
            .spread someProps
```

```jsx
export function Button(someProps) {
    const PARTAS_OTHERS = omit(someProps, "size", "variant");
    return <button class={button_variants({ size: someProps.size, variant: someProps.variant })}
        {...PARTAS_OTHERS} n$={false} />;
}
```

`this` works too, and is now used throughout the test suite (`member this.CombinedSpread = ...` →
`export function SimpleCombination(this$)`). The single unit parameter and the `Partas.Solid`-namespaced declaring
entity are still required; only the *name* restriction is lifted.

## 2. `splitProps`/`mergeProps` → `omit`/`merge`, and `PARTAS_LOCAL` is gone

This is the change with the widest blast radius on generated output. Solid 2.0 replaced `splitProps` and
`mergeProps` with `omit` and `merge`, and because `omit` returns only the *rest* object, there is no longer a "local"
half to destructure into.

**Property reads now go through the self identifier directly.** `PARTAS_LOCAL` no longer exists.

| Before (2.1.3) | After (`solid2rc`) |
| --- | --- |
| `const [PARTAS_LOCAL, PARTAS_OTHERS] = splitProps(props, ["class"])` | `const PARTAS_OTHERS = omit(props, "class")` |
| `PARTAS_LOCAL.class` | `props.class` |
| `props = mergeProps({ class: "x" }, props)` | `props = merge({ class: "x" }, props)` |
| `import { splitProps, mergeProps } from "solid-js"` | `import { omit, merge } from "solid-js"` |

When a component reads no properties at all, the `omit` call and its import are elided entirely — the binding
degrades to a plain alias:

```jsx
export function MyTag(props) {
    const PARTAS_OTHERS = props;     // was: splitProps(props, [])
    return <div />;
}
```

The F# source for all of this is unchanged. If you only ever read `props.foo` and spread `props`, your components
recompile as-is; the difference is entirely in the emitted JSX (and therefore in any snapshot tests you keep).

`Bindings.splitProps` and `Bindings.mergeProps` are removed from the library surface. Call `omit` and `merge`
instead, which are bound as `Bindings.omit(obj, [<ParamArray>] props)` and `Bindings.merge([<ParamArray>] sources)`.

## 3. New `ComponentFlag`s: `SkipOmit` and `SpreadProps`

Two new bits control the props preamble:

- **`ComponentFlag.SkipOmit`** — suppress the generated `omit` binding entirely.
- **`ComponentFlag.SpreadProps`** — `.spread` emits the self identifier rather than `PARTAS_OTHERS`.

With either flag set, `.spread self` compiles to `{...self} n$={false}` instead of `{...PARTAS_OTHERS} n$={false}`.
This is what lets a wrapper forward its props verbatim without paying for an `omit` — the pattern the new `For`
bindings are built on:

```fsharp
[<SolidTypeComponent(ComponentFlag.SkipOmit ||| ComponentFlag.SpreadProps)>]
member props.comp = ForComponent(keyed = !^true).spread props
```

Existing flags (`DebugMode`, `PrintDisposals`, `SkipPojoOptimisation`, `SkipCEOptimisation`) are unchanged.

## 4. The spread marker changed: `bool:n$` → `n$`

Spreads previously emitted a `bool:`-namespaced sentinel prop. That namespace is gone:

```diff
- <div {...PARTAS_OTHERS} bool:n$={false}>
+ <div {...PARTAS_OTHERS} n$={false}>
```

Only relevant if you diff or post-process generated JSX.

## 5. Tag extension methods: removals and a rename

`HtmlElementExtensions` lost three members and renamed one.

| 2.1.3 | `solid2rc` |
| --- | --- |
| `.classList(obj)` | **`.class'(obj)`** — renamed |
| `.on(name, handler)` | **removed** |
| `.prop(name, value)` | **removed** |
| `.use'(name, value)` | **removed** |
| `.bool(name, value)` → `bool:name` | `.bool(name, value)` → plain `name` |
| `.spread` on `#HtmlTag` | `.spread` on `#HtmlElement` |

Surviving: `.attr`, `.data`, `.ref`, `.style'`, `.class'`, `.bool`, `.spread`.

The removals matter most if you leaned on `.on(...)` as the escape hatch for events the typed attribute surface
doesn't cover, or `.use'(...)` for Solid directives — there is currently no replacement binding for either. Typed
`onClick`-style attributes and the `OnHandler` POJO (for `once`/`passive`/`capture`) are unaffected.

The plugin's namespacing changed to match: `bool`/`data`/`attr` are the only prefix-eligible extensions now, and
only `data` still prefixes (`data-{name}`). `bool` and `attr` both emit the bare property name.

## 6. Control-flow components restructured

Solid 2.0 reorganised control flow, and the bindings follow it. This is a source-breaking rename set.

| 2.1.3 | `solid2rc` |
| --- | --- |
| `ErrorBoundary()` | `Errored()` — `fallback` is now `U2<HtmlElement, Fallback>`, with `.fallbackEle` / `.fallbackFn` setters; the `Fallback` delegate's `err` is now an `Accessor<obj>` |
| `For<'T>()` | `For.Keyed<'T>` / `For.NonKeyed<'T>` / `For.KeyedFn<'T>` (`For.Component<'T>` aliases `Keyed`) |
| `Index<'T>()` | **removed** — use `For.NonKeyed` (child lambda receives `Accessor<'T> * int`) |
| `Suspense()` | `Loading()` — gains an `on` property |
| `SuspenseList()` / `RevealOrder` | `Reveal()` with `Reveal.Order` (`Sequential` / `Together` / `Natural`) |
| `Show()` / `Show<'T>()` | `Show.Base` / `Show.Base<'T,'A>`, normally constructed via the overloaded `Show(when', ?fallback, ?keyed)` factory |
| — | **`Repeat<'T>`** (new): `count`, `from`, `fallback` |
| `Portal()`, `Dynamic<'T>()` | moved to `SolidWebBindings` |

`Match`, `Switch`, `Hydration`, `NoHydration`, `Fragment` are unchanged.

`For` usage now picks keying explicitly at the type level:

```fsharp
For.Component(each = [| 1; 2; 3 |]) {
    yield fun item index -> Fragment() { item }
}
```

## 7. Reactive primitives: substantial removals and additions

`SolidBindings` was largely rewritten. **Removed** (no direct replacement in the bindings today):

`createResource` and the whole `SolidResource` / `SolidResourceManager` / `ResourceFetcher` / `ResourceFetcherInfo`
family · `createComputed` · `createDeferred` · `createSelector` · `startTransition` · `useTransition` · `batch` ·
`onMount` · `indexArray` · `produce` · `unwrap` · `catchError` · `createUniqueId`'s old overloads ·
`importComponent` · `mapArray`'s old shape · `SolidStorePath` and its extensions · `SolidStoreSetter` ·
`ComparisonFunc`

**Added**, tracking Solid 2.0: `createOptimistic` / `createOptimisticStore` · `createProjection` ·
`createErrorBoundary` · `createLoadingBoundary` · `createTrackedEffect` · `createRevealOrder` · `action` ·
`affects` · `latest` · `isPending` · `isDisposed` · `flush` · `snapshot` · `deep` · `refresh` · `repeat` ·
`resolve` · `flatten` · `onSettled` · `getObserver` · `tryUseContext` · `enableExternalSource` ·
`mapArrayKeyed` / `mapArrayKeyedFn` / `mapArrayUnkeyed`

New supporting types: `Store<'T>`, `StoreSetter<'T>`, `StoreReturn<'T>`, `Refreshable<'T>`,
`RefreshableStore<'T>`, `EffectOptions`, `MemoOptions<'T>`, `ProjectionOptions<'T>`, plus a `DiagnosticEvent` /
`DiagnosticCode` / `DiagnosticKind` / `DiagnosticSeverity` set for dev diagnostics.

Two specifics worth calling out:

- **`createEffect` changed shape.** Alongside `createEffect(fun () -> ...)` there is now the Solid 2.0
  compute/effect split: `createEffect(compute: 'T option -> 'T, effectFn: 'T -> unit, ?defer, ?schedule, ?sync,
  ?transparent)`, also available taking an `EffectOptions`.
- **`createStore` moved from `solid-js/store` to `solid-js`** and returns the new `Store<'T> * StoreSetter<'T>`.
  `Store<'T>` carries an implicit conversion to `'T`, so reads stay ergonomic. `reconcile` remains.

## 8. `Partas.Solid.Experimental` computation expressions removed

The entire `Builders` module is gone — `effect`, `mount`, `cleanup`, `memo`, `batch`, `lazyload`, `selector`,
`children`, `reaction`, and `lambda` no longer exist, along with their builder types. Only the base builder types
(`NullLambdaBuilder`, `BaseLambdaBuilder`, `LambdaBuilder`) remain; nothing is instantiated for you.

Any `effect { ... }` / `memo { ... }` / `lambda { ... }` blocks must be rewritten as direct calls. Several of these
depended on primitives that Solid 2.0 itself removed (`batch`, `createSelector`, `onMount`), so they cannot simply
be reinstated as-is.

## 9. New `SolidWebBindings` module

`solid-js/web` now has its own file (compiled between `SolidBindings` and `SolidRouterBindings`), holding what was
previously scattered or missing:

- `Portal()`, `Dynamic<'T>()` (moved out of `SolidBindings`), `HeadTag()`
- `render`, `hydrate`, `renderToString`, `renderToStream`
- `isServer`, `isDev`
- `clientOnly` (sync and `Promise`-returning overloads)
- `httpHeader`, `httpStatus` (typed against `System.Net.HttpStatusCode`)

`Bindings.render` / `renderToString` / `isServer` / `DEV` are no longer on the core `Bindings` type — update your
`open`s accordingly.

## 10. Toolchain

Relevant if you build from source or pin transitively:

- Fable.AST `5.0.0-beta.2` → **`5.0.0`**; Fable.Core `5.0.0-beta.1` → **`5.2.0`**; FSharp.Core pinned to **10**.
  The `Ident` AST node gained `IsInlineIfLambda`, which is why the plugin's `AstUtils.Ident`/`IdentExpr` helpers
  took a new optional parameter.
- Solution file replaced: `Partas.Solid.sln` → **`Partas.Solid.slnx`**.
- New build CLI (`partas-solid.fsproj`) supersedes `build.fsx`:
  `dotnet run --project partas-solid.fsproj -- test|build|format|lint|publish`.
- Test discovery is now directory-driven — cases are found by walking `Compiled/*Cases/**` for `.expected` files
  rather than being listed in `Tests.fs`. Adding a case means adding a folder.

---

## Migration checklist

1. Rename `.classList` → `.class'`.
2. Replace `.on(...)`, `.prop(...)`, `.use'(...)` call sites — no direct replacement exists yet.
3. Rename control-flow components: `ErrorBoundary` → `Errored`, `Suspense` → `Loading`, `SuspenseList` → `Reveal`,
   `For` → `For.Component`/`For.Keyed`/`For.NonKeyed`, drop `Index`.
4. Rewrite every `Partas.Solid.Experimental` CE block as direct calls.
5. Replace `splitProps`/`mergeProps` calls with `omit`/`merge`.
6. Rework anything built on `createResource`, `batch`, `onMount`, `createSelector`, `createComputed`,
   `startTransition`, `useTransition`, `produce`, or `unwrap`.
7. Fix `open`s for `render`/`renderToString`/`isServer` (now `SolidWebBindings`).
8. Regenerate any committed JSX snapshots — `PARTAS_LOCAL` disappears, `bool:n$` becomes `n$`, and imports change.
9. Optionally, drop the `props` naming convention on `SolidTypeComponent` members now that `this` (or anything
   else) is legal.
