# Solid 2.0 surface API coverage

Working map for the `solid2rc` port: what the upstream surface **is**, where each piece **lives**, what is
**bound**, and what is **left**. Companion to [`CHANGES-solid2.md`](CHANGES-solid2.md) (which records usage-level
breaking changes); this file is the coverage ledger.

All `solid/...` paths are the pinned submodule (`solidjs/solid` @ tag `solid-js@2.0.0-rc.9`, HEAD `9a29b1a0`).
File references are relative links — they resolve in Rider's and VS Code's Markdown preview, and `Ctrl/Cmd+Click`
jumps to the line. They will **not** resolve on GitHub for paths under `solid/`, since that is a submodule.

---

## 1. Package map — what we import from, and where it is defined

| Import specifier | Upstream source | Bound in | Notes |
| --- | --- | --- | --- |
| `solid-js` | [`solid/packages/solid/src/index.ts`](../solid/packages/solid/src/index.ts) (barrel) | [`SolidBindings.fs`](../Partas.Solid/SolidBindings.fs) | the bulk of the surface |
| ↳ re-exports `@solidjs/signals` | [`signals/src/index.ts`](../solid/packages/signals/src/index.ts) | same | reactivity + store actually live here |
| `@solidjs/web` | [`web/src/index.ts`](../solid/packages/web/src/index.ts) | [`SolidWebBindings.fs`](../Partas.Solid/SolidWebBindings.fs) | **renamed** from `solid-js/web`; already correct |
| `@solidjs/router` | *not in submodule* | [`SolidRouterBindings.fs`](../Partas.Solid/SolidRouterBindings.fs) (446 ln, 27 imports) | separate repo — see [§7](#7-blind-spots--things-these-submodules-cannot-answer) |
| `@solidjs/meta` | *not in submodule* | [`SolidMetaBindings.fs`](../Partas.Solid/SolidMetaBindings.fs) (35 ln) | separate repo |
| `@solidjs/start` | *not in submodule* | [`SolidStartBindings.fs`](../Partas.Solid/SolidStartBindings.fs) (182 ln) | separate repo |

**`solid-js/store` no longer exists.** [`solid/packages/solid/package.json`](../solid/packages/solid/package.json#L30)
declares only `.`, `./refresh`, `./attribution`, `./internal`, `./package.json`. The store API lives in
[`signals/src/store/index.ts`](../solid/packages/signals/src/store/index.ts), is folded into the `@solidjs/signals`
barrel (`export * from "./store/index.js"`), and is re-exported from `"solid-js"`. Our bindings already import
everything from `"solid-js"`, so nothing to change — just don't reintroduce the subpath.

Since rc.9 the monorepo also carries what used to be dom-expressions: the web runtime (`@solidjs/web`,
[`web/src/`](../solid/packages/web/src/)), the JSX types ([`web/jsx/`](../solid/packages/web/jsx/)) and a Rust JSX
compiler ([`compiler/src/`](../solid/packages/compiler/src/)). The former `dom-expressions/` submodule has
been removed.

### Where upstream definitions actually are

| Area | Anchors |
| --- | --- |
| Core barrel | [`index.ts:1`](../solid/packages/solid/src/index.ts#L1) — **authoritative removal list** at [`:255-313`](../solid/packages/solid/src/index.ts#L255-L313) |
| Signals barrel | [`signals/src/index.ts:1-120`](../solid/packages/signals/src/index.ts#L1-L120) |
| Store | [`store/index.ts`](../solid/packages/signals/src/store/index.ts) · [`createStore:54`](../solid/packages/signals/src/store/index.ts#L54) · [`merge`/`omit` impl](../solid/packages/signals/src/store/utils.ts#L1094) |
| Control-flow components | [`For:63`](../solid/packages/solid/src/client/flow.ts#L63) · [`Repeat:126`](../solid/packages/solid/src/client/flow.ts#L126) · [`Show:164`](../solid/packages/solid/src/client/flow.ts#L164) · [`Switch:264`](../solid/packages/solid/src/client/flow.ts#L264) · [`Match:375`](../solid/packages/solid/src/client/flow.ts#L375) · [`Errored:415`](../solid/packages/solid/src/client/flow.ts#L415) · [`Loading:466`](../solid/packages/solid/src/client/flow.ts#L466) · [`Reveal:525`](../solid/packages/solid/src/client/flow.ts#L525) |
| Component types + `lazy` | [`createComponent:80`](../solid/packages/solid/src/client/component.ts#L80) · [`lazy:127`](../solid/packages/solid/src/client/component.ts#L127) · [`createUniqueId:223`](../solid/packages/solid/src/client/component.ts#L223) |
| Context / children | [`createContext:109`](../solid/packages/solid/src/client/core.ts#L109) · [`useContext:154`](../solid/packages/solid/src/client/core.ts#L154) · [`children:180`](../solid/packages/solid/src/client/core.ts#L180) |
| Reactive primitives (client build) | [`createMemo:1466`](../solid/packages/solid/src/client/hydration.ts#L1466) · [`createSignal:1519`](../solid/packages/solid/src/client/hydration.ts#L1519) · [`createStore:1736`](../solid/packages/solid/src/client/hydration.ts#L1736) · [`createEffect:1933`](../solid/packages/solid/src/client/hydration.ts#L1933) · [`sharedConfig:196`](../solid/packages/solid/src/client/hydration.ts#L196) · [`enableHydration:1327`](../solid/packages/solid/src/client/hydration.ts#L1327) |
| Web runtime | [`render:294`](../solid/packages/web/src/client.ts#L294) · [`hydrate:2012`](../solid/packages/web/src/client.ts#L2012) · [`Portal:131`](../solid/packages/web/src/index.ts#L131) · [`dynamic:318`](../solid/packages/web/src/index.ts#L318) · [`Dynamic:481`](../solid/packages/web/src/index.ts#L481) · [`clientOnly:540`](../solid/packages/web/src/index.ts#L540) · [`httpStatus:639`](../solid/packages/web/src/index.ts#L639) · [`httpHeader:660`](../solid/packages/web/src/index.ts#L660) |
| JSX types | [`web/jsx/jsx.d.ts`](../solid/packages/web/jsx/jsx.d.ts) · [`jsx-properties.d.ts`](../solid/packages/web/jsx/jsx-properties.d.ts) — see [§5](#5-jsx--dom-attribute-surface) |
| SSR | [`renderToString:159`](../solid/packages/web/src/server-mock.ts#L159) · [`renderToStream:210`](../solid/packages/web/src/server-mock.ts#L210) · [`ssr* helpers:356-416`](../solid/packages/web/src/server-mock.ts#L356-L416) |

[`index.ts:255-313`](../solid/packages/solid/src/index.ts#L255-L313) is the single most useful file in the
checkout: upstream left the removed 1.x exports in a comment block with the reason for each (`batch → flush`,
`createSelector → createProjection`, `onMount → onSettled`, `Suspense → Loading`, `unwrap → snapshot`, …). Treat
it as the don't-bind list.

---

## 2. Core `solid-js` — coverage

Bound surface is [`SolidBindings.fs`](../Partas.Solid/SolidBindings.fs) (`type Bindings`) plus the
component types above it.

**Done.** All reactivity and store primitives that a component author touches:

`createSignal` `createMemo` `createEffect` `createRenderEffect` `createTrackedEffect` `createReaction`
`createRoot` `createStore` `createProjection` `createOptimistic` `createOptimisticStore`
`createErrorBoundary` `createLoadingBoundary` `createRevealOrder` `createContext` `useContext`
`tryUseContext`¹ `children` `createUniqueId` `lazy'` `action` `affects` `deep` `snapshot` `merge` `omit`
`reconcile` `refresh` `repeat` `resolve` `flatten` `flush` `latest` `isPending` `isDisposed` `untrack`
`onCleanup` `onSettled` `getOwner` `getObserver` `runWithOwner` `enableExternalSource` `mapArray`
(+ `mapArray'` / `mapArrayKeyed` / `mapArrayUnkeyed` / `mapArrayKeyedFn` ergonomic wrappers¹) `DEV`

Components: `For` `Show` `Switch` `Match` `Errored` `Loading` `Repeat` `Reveal` `Hydration` `NoHydration`.

**rc.9 additions (bound).** New or reshaped upstream between rc.0 and rc.9. Anchors on our side name the binding
rather than a line, since [`SolidBindings.fs`](../Partas.Solid/SolidBindings.fs) and
[`SolidWebBindings.fs`](../Partas.Solid/SolidWebBindings.fs) are still moving.

| Upstream | Defined at | Binding |
| --- | --- | --- |
| `isStatic(o, key)` | [`store/utils.ts:260`](../solid/packages/signals/src/store/utils.ts#L260) | `Bindings.isStatic(o: obj, key: string): bool` |
| `until(fn, options?)` → `Promise<Truthy<T>>` | [`signals.ts:958`](../solid/packages/signals/src/signals.ts#L958) | `Bindings.until` overloads (bare / `UntilOptions` / `timeout` + `signal`), plus `UntilOptions` |
| `TimeoutError` | [`core/error.ts:71`](../solid/packages/signals/src/core/error.ts#L71) | `TimeoutError` (`Import` class, inherits `exn`) — the rejection value of `until` on timeout |
| `configureClientErrors(config)` | [`core/error-hooks.ts:69`](../solid/packages/signals/src/core/error-hooks.ts#L69) | `Bindings.configureClientErrors`, with `ClientErrorHook` / `ClientErrorContext` |
| `OBSERVE` (observe-tier diagnostics; `undefined` outside dev/observe builds) | [`index.ts:177`](../solid/packages/solid/src/index.ts#L177) | `Bindings.OBSERVE`, alongside `DEV` |
| `omit(props, predicate)` overload | [`store/utils.ts:1094`](../solid/packages/signals/src/store/utils.ts#L1094) | `Bindings.omit<'T>(obj, hidden: string -> bool)` beside the `ParamArray` key form |
| `refresh(target)` now returns `Promise<T>` | [`signals.ts:793`](../solid/packages/signals/src/signals.ts#L793) | `Bindings.refresh` returns `JS.Promise<'T>` (was `unit`); fire-and-forget callers `ignore` it |
| `render` / `hydrate` `onError` option | [`web/src/client.ts:294`](../solid/packages/web/src/client.ts#L294) / [`:2012`](../solid/packages/web/src/client.ts#L2012) | `render` / `hydrate` overloads taking `onError: ClientErrorHook` in [`SolidWebBindings.fs`](../Partas.Solid/SolidWebBindings.fs) |
| `createStore(fn, seed, options?)` derived form; `StoreOptions.shallow` | [`store/index.ts:54-66`](../solid/packages/signals/src/store/index.ts#L54-L66) | `Bindings.createStore` seed overloads (returning `RefreshableStoreReturn`), `StoreOptions(?name, ?shallow)` |
| `createEffect(compute, { effect, error })` bundle | [`signals.ts:495`](../solid/packages/signals/src/signals.ts#L495), type [`:154`](../solid/packages/signals/src/signals.ts#L154) | `EffectBundle<'T, 'R>(effect, error)` + `Bindings.createEffect` overloads taking it |
| `EffectOptions.name` (from `BaseEffectOptions`); `createReaction(effectFn, options?)` | [`signals.ts:170-177`](../solid/packages/signals/src/signals.ts#L170-L177), [`:635`](../solid/packages/signals/src/signals.ts#L635) | `EffectOptions(?name)` and `?name` on every `ParamObject` effect overload; `Bindings.createReaction` overloads taking `EffectOptions` |
| `action(genFn)` returns a callable, not a promise | [`core/action.ts:112`](../solid/packages/signals/src/core/action.ts#L112) | `Bindings.action(genFn: 'Args -> 'Gen): 'Args -> JS.Promise<'R>`; `genFn` must return a JS generator |
| `reconcile(value, key)` with a per-item key fn or property name | [`store/index.ts:68`](../solid/packages/signals/src/store/index.ts#L68) | `Bindings.reconcile` overloads taking `key: string` or `key: 'Item -> objnull` |

**Removed upstream by rc.9** — do not bind: `$REFRESH` (still in the signals barrel, no longer re-exported by
`solid-js`), `storePath` (the value is gone from `solid-js`; only its `PathSetter`/`Part`/`StorePathRange` types
remain), and `NoHydrateContext` (now internal to [`hydration.ts:120`](../solid/packages/solid/src/client/hydration.ts#L120);
use the `NoHydration` component).

¹ Partas-only conveniences, not upstream names.

**Gaps.** Nothing load-bearing, but these are genuinely exported and currently unbindable from F#:

| Done          | Defined at | Why you'd want it | Priority |
|---------------| --- | --- | --- |
| `createOwner` | [signals barrel `:9`](../solid/packages/signals/src/index.ts#L9) | build a detached owner to pair with `runWithOwner` — we bind the consumer but not the producer | **high** (asymmetric API) |

| Ignore                                          | Defined at | Why you'd want it | Priority | Why ignored                                                                                             |
|--------------------------------------------------| --- | --- | --- |---------------------------------------------------------------------------------------------------------|
| `storePath` (removed from `solid-js` in rc.9) | [`store/index.ts:83`](../solid/packages/signals/src/store/index.ts#L83) | typed deep-path setters (`PathSetter`, `Part`, `StorePathRange`) | medium | The draft mutaters/new value returns are more idiomatic for F#; no reason to use this deprecated method |
| `$PROXY` `$TRACK` (`$REFRESH` `$TARGET` no longer re-exported by `solid-js`) | [`index.ts:2-3`](../solid/packages/solid/src/index.ts#L2-L3) | store introspection symbols | low | Unless we can quickly identify whether something is a store, or get other information from some type, then this is not useful |

| Missing                                          | Defined at | Why you'd want it | Priority |
|--------------------------------------------------| --- | --- | --- |
| `isWrappable` | [`store/index.ts:14`](../solid/packages/signals/src/store/index.ts#L14) | guard before `createStore`; already flagged `// todo - isWrappable` | medium |
| `isEqual`                                        | [signals barrel `:20`](../solid/packages/signals/src/index.ts#L20) | default comparator, useful as an explicit `equals` argument | medium |
| `NotReadyError`                                  | [signals barrel `:5`](../solid/packages/signals/src/index.ts#L5) | catchable async-suspend sentinel — needed to write correct error boundaries | medium |
| `enforceLoadingBoundary`                         | [signals barrel `:29`](../solid/packages/signals/src/index.ts#L29) | used by upstream `render` itself ([`web/src/client.ts:323`](../solid/packages/web/src/client.ts#L323)) | low |
| `sharedConfig` / `enableHydration`               | [`:196`](../solid/packages/solid/src/client/hydration.ts#L196) / [`:1327`](../solid/packages/solid/src/client/hydration.ts#L1327) | hydration control for custom renderers | low |
| `materializeContainerTrace` | [`hydration.ts:911`](../solid/packages/solid/src/client/hydration.ts#L911) (exported at [`index.ts:120`](../solid/packages/solid/src/index.ts#L120)) | advanced hydration | low |
| `getNextChildId`                                 | [signals barrel `:13`](../solid/packages/signals/src/index.ts#L13) | SSR id generation | low |

**Deliberately not bound** (compiler-facing — the Babel/Partas plugin emits these, users never write them):
`$DEVCOMP` and the `ssrHandleError` / `ssrScope` / `runInServerComponentScope` /
`creationStamp` / `inServerComponentScope` / `ssrSanitizeError` / `reportServerError` / `getProjectionTrace` stubs at
[`index.ts:118-170`](../solid/packages/solid/src/index.ts#L118-L170) (all marked `@internal`). `createComponent` is
also compiler-facing, but *is* bound (`Bindings.createComponent`) for custom JSX factories and renderers.

---

## 3. `@solidjs/web` — coverage

[`SolidWebBindings.fs`](../Partas.Solid/SolidWebBindings.fs) (`type Bindings`). **This is the healthiest area.**

Done: `render` `hydrate` `Portal` `Dynamic` (component,
[`SolidWebBindings.fs`](../Partas.Solid/SolidWebBindings.fs) `Dynamic<'T>`) `dynamic` (function) `clientOnly`
`isServer` `isDev` `httpStatus` `httpHeader` `renderToString` `renderToStream`, plus a Partas-only
`useHead`/`HeadTag`.

Gaps:

| Missing | Defined at | Verdict |
| --- | --- | --- |
| Request/response plumbing — `createRequestEvent` `createResponseStub` `createSSRResponse` `commitEventResponse` `composeMiddleware` `getExpectedRedirectStatus` | [`server-mock.ts:254-342`](../solid/packages/web/src/server-mock.ts#L254-L342) | **belongs in [`SolidStartBindings.fs`](../Partas.Solid/SolidStartBindings.fs)**, not web — decide placement |
| Asset types — `AssetManifest` `AssetResolver` `AssetResolverFn` `ResolvedAssets` `InlineStyleAsset` | [`server-mock.ts:56-142`](../solid/packages/web/src/server-mock.ts#L56-L142) | only if we bind SSR asset handling |
| Response helpers — `redirect` `reload` `respond` `markSafeError` `isSafeError` `ResponseEnvelope` `HREF` `REVALIDATE_HEADER` … (`export * from "./response.js"`) | [`response.ts:33-288`](../solid/packages/web/src/response.ts#L33-L288) | now in-tree (was an unverifiable `@dom-expressions/runtime` re-export); server-function plumbing — same placement question as the request/response row |
| `export * from "./client.js"` | [`client.ts`](../solid/packages/web/src/client.ts) (in-tree since rc.9) | compiler-facing DOM runtime (`insert`, `spread`, `template`, `delegateEvents`) — do not bind; only `render` / `hydrate` are user-facing |
| `mergeProps` | [`client.ts:243-249`](../solid/packages/web/src/client.ts#L243-L249) | compiler-emitted prop-spread helper, marked `@internal` — do not bind; use `merge` from `solid-js` |
| `ssr` `ssrElement` `ssrAttribute` `ssrClassName` `ssrStyle` `ssrStyleProperty` `ssrGroup` `ssrHydrationKey` `resolveSSRNode` `escape` | [`server-mock.ts:356-416`](../solid/packages/web/src/server-mock.ts#L356-L416) | compiler-facing — do not bind |

---

## 4. Types — the largest remaining gap

Upstream exports ~45 type aliases; we bind a fraction (`Store` `StoreSetter` `StoreReturn` `Refreshable`
`RefreshableStore` `EffectOptions` `MemoOptions` `ProjectionOptions` `StoreOptions` `SignalOptions` `EffectBundle`
`EffectErrorHandler` `LazyOptions` `UntilOptions` `ClientError*` `Accessor` `Setter` `Owner` `Context` `Diagnostic*`). Not yet surfaced, from [`index.ts:44-82`](../solid/packages/solid/src/index.ts#L44-L82) and
[`store/index.ts`](../solid/packages/signals/src/store/index.ts):

- **Store**: `StoreNode` `SolidStore` `ProjectionStoreReturn` `NotWrappable` `PathSetter` `Part`
  `StorePathRange` `ArrayFilterFn` `CustomPartial` `Merge` `Omit`
- **Reactivity**: `ComputeFunction` `EffectFunction` `SourceAccessor` `NoInfer` `ExternalSource`
  `ExternalSourceFactory` `Truthy`
- **Components** ([`component.ts:10-66`](../solid/packages/solid/src/client/component.ts#L10-L66)): `Component`
  `VoidComponent` `ParentComponent` `FlowComponent` `VoidProps` `ParentProps` `FlowProps` `ComponentProps` `Ref`
- **Children** ([`core.ts`](../solid/packages/solid/src/client/core.ts)): `ChildrenReturn` `ResolvedChildren`
  `ResolvedElement` `ContextProviderComponent`; `ArrayElement` / `Element` from
  [`types.ts`](../solid/packages/solid/src/types.ts)

Most matter only if a user writes a generic helper over Solid types. The **component types are the exception** —
`ParentProps`/`FlowProps`/`Ref` shape how consumers type their own components, so they're worth doing early.

---

## 5. JSX / DOM attribute surface

This is the **largest binding surface in the repo**. It was diffed name-by-name against the pre-rc.9
dom-expressions `jsx.d.ts` — see [Reconciliation](#reconciliation-diffed-against-the-pinned-checkout) below, including
the deltas rc.9 introduced.

### Where it comes from

Since rc.9 **Solid authors its own JSX types**, in [`solid/packages/web/jsx/`](../solid/packages/web/jsx/). The
dom-expressions copy step is gone: [`web/package.json:306`](../solid/packages/web/package.json#L306) keeps the
`types:copy-jsx` script name, but it now just aliases `jsx-sync`
([`scripts/jsx-sync.mjs`](../solid/packages/web/scripts/jsx-sync.mjs)), which regenerates `jsx.d.ts` in place from
the in-tree hyperscript source:

```
jsx-h.d.ts  --(jsx-sync --compile --element "SolidElement | Node | ArrayElement")-->  jsx.d.ts
```

Consequences:

- [`jsx.d.ts`](../solid/packages/web/jsx/jsx.d.ts) and
  [`jsx-properties.d.ts`](../solid/packages/web/jsx/jsx-properties.d.ts) are committed and readable;
  [`web/src/index.ts:38`](../solid/packages/web/src/index.ts#L38) re-exports `JSX` from `../jsx/jsx.js`. `jsx.d.ts`
  is generated, so read it — but upstream edits land in
  [`jsx-h.d.ts`](../solid/packages/web/jsx/jsx-h.d.ts).
- **`solid/packages/web/jsx/` is the source of truth** for element attributes, ARIA, SVG, MathML and event handler
  names. The `dom-expressions/` submodule that used to hold these types has been removed.
- The compiler that consumes these types is Rust: [`compiler/src/dom/`](../solid/packages/compiler/src/dom/)
  (client) and [`compiler/src/ssr/`](../solid/packages/compiler/src/ssr/) (server).

### What we currently bind

| File | Surface |
| --- | --- |
| [`HtmlAttributes.fs`](../Partas.Solid/HtmlAttributes.fs) | `HTMLAttributes` base: **91** event handlers + ~60 global attributes; **47** per-element interfaces (`AnchorHTMLAttributes` … `VideoHTMLAttributes`) |
| [`AriaAttributes.fs`](../Partas.Solid/AriaAttributes.fs) | **49** members |
| [`Svg.fs`](../Partas.Solid/Svg.fs) | **145** members |

The 47 element interfaces are: `Anchor` `Area` `Base` `Blockquote` `Button` `Canvas` `Col` `Colgroup` `Data`
`Details` `Dialog` `Embed` `Fieldset` `Form` `Iframe` `Img` `Input` `Ins` `Keygen` `Label` `Li` `Link` `Map`
`Media` `Menu` `Meter` `Quote` `Object` `Ol` `Optgroup` `Option` `Output` `Param` `Progress` `Script` `Select`
`HTMLSlotElement` `Source` `Style` `Td` `Template` `Textarea` `Th` `Time` `Track` `Video`.

### Reconciliation: diffed against the pinned checkout

Every count below is a name-level `comm` diff between the dom-expressions `0.50.0-next.42` `jsx.d.ts` and our
sources, normalised for casing and for our `aria*`/trailing-apostrophe naming conventions. It has **not** been re-run
against rc.9's [`jsx.d.ts`](../solid/packages/web/jsx/jsx.d.ts). A file-level diff of dom-expressions `0.50.0-next.44`
(`e97e4290`, the last pin before the submodule was removed) against rc.9 shows only these surface changes, none reflected in the counts yet:

- **New tag `selectedcontent`** (`HTMLAttributes<HTMLElement>`) — HTML tags are now 118 upstream; not bound.
- **`AmbiguousNamespaceAttributes`** ([`jsx.d.ts:1118`](../solid/packages/web/jsx/jsx.d.ts#L1118)) adds `xmlns` to
  `<a>`, `<script>`, `<style>` and `<title>` (new `TitleHTMLAttributes`); we only carry `xmlns` on SVG.
- `$key` (SSR entity identity) is **gone** from the element attribute surface.
- `HTMLFetchPriority` / `HTMLPreloadAs` aliases factored out — no name-level change.

| Surface | Upstream | Ours | Verdict |
| --- | --- | --- | --- |
| ARIA attributes | 53 | 52 | **2 missing**, 1 stray |
| SVG tags | 59 | 59 | ✅ complete |
| HTML tags | 117 | 111 | **6 missing** (all obsolete/non-standard) |
| MathML tags | 32 | 0 | ❌ **no coverage at all** |
| Global HTML attributes | 45 | 129 | **7 missing** (we are otherwise a superset) |
| Event handlers | 136 | 91 | **45 missing** (24 window-scoped, 22 element-scoped) |
| Element attribute interfaces | 49 | 45 | **3 missing**, 1 renamed, 1 non-standard |

**ARIA** — [`AriaAttributes.fs`](../Partas.Solid/AriaAttributes.fs) is effectively complete. Missing:
`aria-colindextext`, `aria-rowindextext`. Note [`AriaAttributes.fs:35`](../Partas.Solid/AriaAttributes.fs#L35)
declares `ariaDescendant`, which matches no upstream key and is not in the ARIA spec — almost certainly a stray
next to `ariaActiveDescendant` at [line 11](../Partas.Solid/AriaAttributes.fs#L11).

**SVG** — [`Svg.fs`](../Partas.Solid/Svg.fs) declares exactly the 59 upstream SVG tags, none missing, none extra.
The earlier worry that SVG was "the most likely under-binding" was wrong at *tag* level; the SVG **attribute**
space (145 members on our side) is still undiffed, as upstream spreads it across 65 `*SVGAttributes` interfaces.

**HTML tags** — missing: `big` `keygen` `menuitem` `param` `slot` `template` `webview`. All but `slot` and
`template` are obsolete or non-standard (`webview` is Electron). `slot` and `template` are the two worth adding.

**MathML** — 32 tags and a `MathMLAttributes` interface
([`jsx.d.ts:925`](../solid/packages/web/jsx/jsx.d.ts#L925),
[`:3988`](../solid/packages/web/jsx/jsx.d.ts#L3988)) with **zero** counterpart here. This is the
single largest untouched surface. It is also entirely additive — no existing binding changes.

**Global attributes** — we are a superset (129 vs 45) because our `HTMLAttributes` block also carries events and
convenience members. Genuinely absent: `autocorrect` `autofocus` `elementtiming` `enterkeyhint` `nonce`, plus the
two upstream marks `@experimental` (`virtualkeyboardpolicy`, `writingsuggestions`).

**Events** — the largest real gap. The 24 window-scoped ones (`beforeunload`, `hashchange`, `popstate`, `storage`,
`unload`, the `device*`/`gamepad*` family, …) come from
[`EventHandlersWindow`](../solid/packages/web/jsx/jsx.d.ts#L693) and only apply to `<body>`/`<html>`,
so skipping them is defensible. The 22 element-scoped ones are not: `beforematch` `beforexrselect` `command`
`contentvisibilityautostatechange` `contextlost` `contextrestored` `cuechange` `formdata` `fullscreenchange`
`fullscreenerror` `pointerrawupdate` `resize` `scrollsnapchange` `scrollsnapchanging` `securitypolicyviolation`
`selectionchange` `selectstart` `slotchange` `animationcancel` `beforecopy` `beforecut` `beforepaste`. We also
bind `onEncrypted`, which upstream does not.

**Element interfaces** — missing `Bdo`, `Body`, `Caption`. Upstream `ModHTMLAttributes` (for `<ins>`/`<del>`) is
our `InsHTMLAttributes` — a naming difference, not a gap. `WebView` is Electron-only and can be ignored.

### `jsx-properties.d.ts` is not a plugin concern

[`jsx-properties.d.ts`](../solid/packages/web/jsx/jsx-properties.d.ts) (93 lines) is **pure
type-level machinery** for deriving the `prop:*` key set — `SkipPropsFrom`, `PropValue`, `WidenPropValue`,
`IfEquals`, `IsReadonlyKey`, `PropKey`. `IsReadonlyKey` compares `Pick<T,K>` against `Readonly<Pick<T,K>>` purely
to exclude readonly DOM properties from the typed surface. It does **not** describe a property-vs-attribute split
that the compiler emits, so it imposes no requirement on our plugin and needs no F# analogue.

### Namespaces: only `prop:` survives

On this branch `prop:` is the sole reserved JSX namespace —
[`set_attr.rs:39`](../solid/packages/compiler/src/dom/set_attr.rs#L39) guards on
`prefix == "prop"` alone, and its own comment notes that `style:`/`class:` are synthetic markers produced by the
object splitters rather than user-facing namespaces. `attr:`, `bool:`, `on:`, `oncapture:` and `use:` are gone.

This is **not** a regression for us: `attr` and `bool` are escape hatches in our own bindings and were never
forwarded to the upstream `attr:`/`bool:` namespaces, so their disappearance changes nothing on our side.

`prop:` is the one to watch. It survived upstream, but per [`CHANGES-solid2.md`](CHANGES-solid2.md) `.prop(name,
value)` was removed from our `HtmlElementExtensions`. That looks like a genuine loss of capability and should be
confirmed against the plugin before it is written off.

---

## 6. Plugin & tests — status

Per [`CHANGES-solid2.md`](CHANGES-solid2.md), plugin-side migration is largely **done**: `splitProps`/`mergeProps`
→ `omit`/`merge` (no more `PARTAS_LOCAL`), spread marker `bool:n$` → `n$`, self-identifier no longer forced to
`props`, new `SkipOmit` / `SpreadProps` flags, `.classList` → `.class'`, tag-extension removals.

The test inputs that were stubbed with `failwith "redo"` / `"REDO"` — `IndexedPropSpreading`, `OperatorsInProps`,
`SignalSetterInvoke`, `ThisArgTransforms`, `ExperimentalBuilders` — are restored against the Solid 2 API and their
`.expected` snapshots regenerated. [`ExperimentalBuilders.fs`](../Partas.Solid.Tests.Plugin/Compiled/SolidCases/Experimental%20builders%20compile%20correct%20output/ExperimentalBuilders.fs)
exercises the re-ported [`Experimental.fs`](../Partas.Solid/Experimental.fs) builders (`effect` → two-phase
`createEffect`, `mount` → `onSettled`, `batch`/`selector` dropped). All 31 plugin cases pass.

---

## 7. Blind spots — things these submodules cannot answer

Flagging these so nobody burns time searching for them:

1. **The JSX / DOM attribute surface is now in the checkout** (rc.9, [`web/jsx/`](../solid/packages/web/jsx/)) —
   no longer a blind spot. What remains open is the re-diff: [§5](#5-jsx--dom-attribute-surface)'s counts predate
   rc.9, so [`HtmlAttributes.fs`](../Partas.Solid/HtmlAttributes.fs),
   [`AriaAttributes.fs`](../Partas.Solid/AriaAttributes.fs), and [`Svg.fs`](../Partas.Solid/Svg.fs) are reconciled
   against `0.50.0-next.42` plus the listed deltas, not against rc.9 itself.
2. **Router / meta / start are separate repositories.** `solid/packages/` has no `router`, `meta`, or `start`.
   [`SolidRouterBindings.fs`](../Partas.Solid/SolidRouterBindings.fs) (27 imports) is the second-largest binding
   file and is entirely unverified by this pin. If router coverage matters, add `solidjs/solid-router` as a
   second submodule.
3. **Server vs client builds diverge.** [`solid/packages/solid/src/server/`](../solid/packages/solid/src/server/)
   is a distinct entry from `src/client/`. Anything SSR-sensitive should be checked against the server entry, not
   just the client one.

---

## 8. Suggested order of work

1. ~~**Land the restored `failwith "redo"` cases**~~ — done ([§6](#6-plugin--tests--status)).
2. ~~**`createOwner`**~~ — done (`Bindings.createOwner`); the owner/`runWithOwner` pair is now symmetric.
3. **Component type aliases** (`Component` `ParentProps` `FlowProps` `Ref` …) — user-facing typing ergonomics.
4. **`isWrappable` `isEqual` `NotReadyError`** + store path types (`storePath` itself is gone in rc.9) — small,
   mechanical, removes the existing `// todo - isWrappable`.
5. **Decide where SSR request/response plumbing lives** (web vs start bindings) before binding it.
6. **Re-run the JSX diff against rc.9** ([§5](#5-jsx--dom-attribute-surface)) using
   [`solid/packages/web/jsx/jsx.d.ts`](../solid/packages/web/jsx/jsx.d.ts) — no extra submodule needed. Start with
   the known deltas (`selectedcontent`, `xmlns` on `a`/`script`/`style`/`title`), then audit
   [`Svg.fs`](../Partas.Solid/Svg.fs) (most likely under-bound at 145 members),
   [`HtmlAttributes.fs`](../Partas.Solid/HtmlAttributes.fs), and
   [`AriaAttributes.fs`](../Partas.Solid/AriaAttributes.fs).
7. Optional: second submodule for `solid-router` if router bindings need verification.
