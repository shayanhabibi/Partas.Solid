# Solid 2.0 surface API coverage

Working map for the `solid2rc` port: what the upstream surface **is**, where each piece **lives**, what is
**bound**, and what is **left**. Companion to [`CHANGES-solid2.md`](CHANGES-solid2.md) (which records usage-level
breaking changes); this file is the coverage ledger.

All `solid/...` paths are the pinned submodule (`solidjs/solid` @ `next`, `solid-js` 2.0.0-rc.0, HEAD `58ef5f0f`).
File references are relative links — they resolve in Rider's and VS Code's Markdown preview, and `Ctrl/Cmd+Click`
jumps to the line. They will **not** resolve on GitHub for paths under `solid/`, since that is a submodule.

---

## 1. Package map — what we import from, and where it is defined

| Import specifier | Upstream source | Bound in | Notes |
| --- | --- | --- | --- |
| `solid-js` | [`solid/packages/solid/src/index.ts`](../solid/packages/solid/src/index.ts) (barrel) | [`SolidBindings.fs`](../Partas.Solid/SolidBindings.fs) | 125 references — the bulk of the surface |
| ↳ re-exports `@solidjs/signals` | [`solid-signals/src/index.ts`](../solid/packages/solid-signals/src/index.ts) | same | reactivity + store actually live here |
| `@solidjs/web` | [`solid-web/src/index.ts`](../solid/packages/solid-web/src/index.ts) | [`SolidWebBindings.fs`](../Partas.Solid/SolidWebBindings.fs) | **renamed** from `solid-js/web`; already correct |
| `@solidjs/router` | *not in submodule* | [`SolidRouterBindings.fs`](../Partas.Solid/SolidRouterBindings.fs) (446 ln, 27 imports) | separate repo — see [§6](#7-blind-spots--things-these-submodules-cannot-answer) |
| `@solidjs/meta` | *not in submodule* | [`SolidMetaBindings.fs`](../Partas.Solid/SolidMetaBindings.fs) (35 ln) | separate repo |
| `@solidjs/start` | *not in submodule* | [`SolidStartBindings.fs`](../Partas.Solid/SolidStartBindings.fs) (182 ln) | separate repo |

**`solid-js/store` no longer exists.** [`solid/packages/solid/package.json`](../solid/packages/solid/package.json#L28)
declares only `.`, `./refresh`, `./types/*`, `./package.json`. Store lives in `@solidjs/signals/store`
([`store/index.ts`](../solid/packages/solid-signals/src/store/index.ts)) and is re-exported through the core
barrel. Our bindings already import everything from `"solid-js"`, so nothing to change — just don't reintroduce
the subpath.

### Where upstream definitions actually are

| Area | Anchors |
| --- | --- |
| Core barrel | [`index.ts:1`](../solid/packages/solid/src/index.ts#L1) — **authoritative removal list** at [`:153-211`](../solid/packages/solid/src/index.ts#L153-L211) |
| Signals barrel | [`solid-signals/src/index.ts:1-87`](../solid/packages/solid-signals/src/index.ts#L1-L87) |
| Store | [`store/index.ts`](../solid/packages/solid-signals/src/store/index.ts) |
| Control-flow components | [`For:55`](../solid/packages/solid/src/client/flow.ts#L55) · [`Repeat:106`](../solid/packages/solid/src/client/flow.ts#L106) · [`Show:144`](../solid/packages/solid/src/client/flow.ts#L144) · [`Switch:244`](../solid/packages/solid/src/client/flow.ts#L244) · [`Match:355`](../solid/packages/solid/src/client/flow.ts#L355) · [`Errored:395`](../solid/packages/solid/src/client/flow.ts#L395) · [`Loading:446`](../solid/packages/solid/src/client/flow.ts#L446) · [`Reveal:505`](../solid/packages/solid/src/client/flow.ts#L505) |
| Component types + `lazy` | [`createComponent:74`](../solid/packages/solid/src/client/component.ts#L74) · [`lazy:111`](../solid/packages/solid/src/client/component.ts#L111) · [`createUniqueId:187`](../solid/packages/solid/src/client/component.ts#L187) |
| Context / children | [`createContext:101`](../solid/packages/solid/src/client/core.ts#L101) · [`useContext:145`](../solid/packages/solid/src/client/core.ts#L145) · [`children:171`](../solid/packages/solid/src/client/core.ts#L171) |
| Reactive primitives (client build) | [`createMemo:1170`](../solid/packages/solid/src/client/hydration.ts#L1170) · [`createSignal:1223`](../solid/packages/solid/src/client/hydration.ts#L1223) · [`createStore:1440`](../solid/packages/solid/src/client/hydration.ts#L1440) · [`createEffect:1613`](../solid/packages/solid/src/client/hydration.ts#L1613) · [`sharedConfig:182`](../solid/packages/solid/src/client/hydration.ts#L182) · [`enableHydration:1033`](../solid/packages/solid/src/client/hydration.ts#L1033) |
| Web runtime | [`render:148`](../solid/packages/solid-web/src/index.ts#L148) · [`hydrate:197`](../solid/packages/solid-web/src/index.ts#L197) · [`Portal:225`](../solid/packages/solid-web/src/index.ts#L225) · [`dynamic:364`](../solid/packages/solid-web/src/index.ts#L364) · [`Dynamic:479`](../solid/packages/solid-web/src/index.ts#L479) · [`clientOnly:522`](../solid/packages/solid-web/src/index.ts#L522) · [`httpStatus:610`](../solid/packages/solid-web/src/index.ts#L610) · [`httpHeader:630`](../solid/packages/solid-web/src/index.ts#L630) |
| SSR | [`renderToString:70`](../solid/packages/solid-web/src/server-mock.ts#L70) · [`renderToStream:120`](../solid/packages/solid-web/src/server-mock.ts#L120) · [`ssr* helpers:264-323`](../solid/packages/solid-web/src/server-mock.ts#L264-L323) |

[`index.ts:153-211`](../solid/packages/solid/src/index.ts#L153-L211) is the single most useful file in the
checkout: upstream left the removed 1.x exports in a comment block with the reason for each (`batch → flush`,
`createSelector → createProjection`, `onMount → onSettled`, `Suspense → Loading`, `unwrap → snapshot`, …). Treat
it as the don't-bind list.

---

## 2. Core `solid-js` — coverage

Bound surface is [`SolidBindings.fs:496`](../Partas.Solid/SolidBindings.fs#L496) (`type Bindings`) plus the
component types above it.

**Done (52 members).** All reactivity and store primitives that a component author touches:

`createSignal` `createMemo` `createEffect` `createRenderEffect` `createTrackedEffect` `createReaction`
`createRoot` `createStore` `createProjection` `createOptimistic` `createOptimisticStore`
`createErrorBoundary` `createLoadingBoundary` `createRevealOrder` `createContext` `useContext`
`tryUseContext`¹ `children` `createUniqueId` `lazy'` `action` `affects` `deep` `snapshot` `merge` `omit`
`reconcile` `refresh` `repeat` `resolve` `flatten` `flush` `latest` `isPending` `isDisposed` `untrack`
`onCleanup` `onSettled` `getOwner` `getObserver` `runWithOwner` `enableExternalSource` `mapArray`
(+ `mapArray'` / `mapArrayKeyed` / `mapArrayUnkeyed` / `mapArrayKeyedFn` ergonomic wrappers¹) `DEV`

Components: `For` `Show` `Switch` `Match` `Errored` `Loading` `Repeat` `Reveal` `Hydration` `NoHydration`.

¹ Partas-only conveniences, not upstream names.

**Gaps.** Nothing load-bearing, but these are genuinely exported and currently unbindable from F#:

| Done          | Defined at | Why you'd want it | Priority |
|---------------| --- | --- | --- |
| `createOwner` | [signals barrel `:8`](../solid/packages/solid-signals/src/index.ts#L8) | build a detached owner to pair with `runWithOwner` — we bind the consumer but not the producer | **high** (asymmetric API) |

| Ignore                                          | Defined at | Why you'd want it | Priority | Why ignored                                                                                             |
|--------------------------------------------------| --- | --- | --- |---------------------------------------------------------------------------------------------------------|
| `storePath`                                      | [`store/index.ts`](../solid/packages/solid-signals/src/store/index.ts) | typed deep-path setters (`PathSetter`, `Part`, `StorePathRange`) | medium | The draft mutaters/new value returns are more idiomatic for F#; no reason to use this deprecated method |
| `$PROXY` `$TRACK` `$REFRESH` `$TARGET`           | [`store/index.ts`](../solid/packages/solid-signals/src/store/index.ts) | store introspection symbols | low | Unless we can quickly identify whether something is a store, or get other information from some type, then this is not useful |

| Missing                                          | Defined at | Why you'd want it | Priority |
|--------------------------------------------------| --- | --- | --- |
| `isWrappable`                                    | [`store/index.ts`](../solid/packages/solid-signals/src/store/index.ts) | guard before `createStore`; already flagged [`// todo`](../Partas.Solid/SolidBindings.fs#L749) | medium |
| `isEqual`                                        | [signals barrel `:19`](../solid/packages/solid-signals/src/index.ts#L19) | default comparator, useful as an explicit `equals` argument | medium |
| `NotReadyError`                                  | [signals barrel `:31`](../solid/packages/solid-signals/src/index.ts#L31) | catchable async-suspend sentinel — needed to write correct error boundaries | medium |
| `enforceLoadingBoundary`                         | [signals barrel `:34`](../solid/packages/solid-signals/src/index.ts#L34) | used by upstream `render` itself ([`solid-web/src/index.ts:155`](../solid/packages/solid-web/src/index.ts#L155)) | low |
| `sharedConfig` / `enableHydration`               | [`:182`](../solid/packages/solid/src/client/hydration.ts#L182) / [`:1033`](../solid/packages/solid/src/client/hydration.ts#L1033) | hydration control for custom renderers | low |
| `NoHydrateContext` / `materializeContainerTrace` | [`:118`](../solid/packages/solid/src/client/hydration.ts#L118) / [`:737`](../solid/packages/solid/src/client/hydration.ts#L737) | advanced hydration | low |
| `getNextChildId`                                 | [signals barrel `:12`](../solid/packages/solid-signals/src/index.ts#L12) | SSR id generation | low |

**Deliberately not bound** (compiler-facing — the Babel/Partas plugin emits these, users never write them):
`createComponent`, `$DEVCOMP`, and the `ssrHandleError` / `ssrScope` / `runInServerComponentScope` /
`creationStamp` / `inServerComponentScope` / `getProjectionTrace` stubs at
[`index.ts:110-133`](../solid/packages/solid/src/index.ts#L110-L133) (all marked `@internal`).

---

## 3. `@solidjs/web` — coverage

[`SolidWebBindings.fs:206`](../Partas.Solid/SolidWebBindings.fs#L206). **This is the healthiest area.**

Done: `render` `hydrate` `Portal` `Dynamic` (component,
[`SolidWebBindings.fs:131`](../Partas.Solid/SolidWebBindings.fs#L131)) `dynamic` (function) `clientOnly`
`isServer` `isDev` `httpStatus` `httpHeader` `renderToString` `renderToStream`, plus a Partas-only
`useHead`/`HeadTag`.

Gaps:

| Missing | Defined at | Verdict |
| --- | --- | --- |
| `mergeProps` (alias of `merge`) | [`index.ts:65`](../solid/packages/solid-web/src/index.ts#L65) | trivial back-compat alias; bind or ignore |
| Request/response plumbing — `createRequestEvent` `createResponseStub` `createSSRResponse` `commitEventResponse` `composeMiddleware` `getExpectedRedirectStatus` | [`server-mock.ts:174-262`](../solid/packages/solid-web/src/server-mock.ts#L174-L262) | **belongs in [`SolidStartBindings.fs`](../Partas.Solid/SolidStartBindings.fs)**, not web — decide placement |
| Asset types — `AssetManifest` `AssetResolver` `AssetResolverFn` `ResolvedAssets` `InlineStyleAsset` | [`server-mock.ts:11-57`](../solid/packages/solid-web/src/server-mock.ts#L11-L57) | only if we bind SSR asset handling |
| `export * from "./response.js"` | [`response.ts:5`](../solid/packages/solid-web/src/response.ts#L5) → `@dom-expressions/runtime` | **unverifiable** — see [§6](#7-blind-spots--things-these-submodules-cannot-answer) |
| `export * from "./client.js"` | [`client.ts:1`](../solid/packages/solid-web/src/client.ts#L1) → `@dom-expressions/runtime` | compiler-facing DOM runtime (`insert`, `spread`, `template`, `delegateEvents`) — do not bind |
| `ssr` `ssrElement` `ssrAttribute` `ssrClassName` `ssrStyle` `ssrStyleProperty` `ssrGroup` `ssrHydrationKey` `resolveSSRNode` `escape` | [`server-mock.ts:264-323`](../solid/packages/solid-web/src/server-mock.ts#L264-L323) | compiler-facing — do not bind |

---

## 4. Types — the largest remaining gap

Upstream exports ~45 type aliases; we bind a fraction (`Store` `StoreSetter` `StoreReturn` `Refreshable`
`RefreshableStore` `EffectOptions` `MemoOptions` `ProjectionOptions` `Accessor` `Setter` `Owner` `Context`
`Diagnostic*`). Not yet surfaced, from [`index.ts:40-84`](../solid/packages/solid/src/index.ts#L40-L84) and
[`store/index.ts`](../solid/packages/solid-signals/src/store/index.ts):

- **Store**: `StoreNode` `StoreOptions` `SolidStore` `ProjectionStoreReturn` `NotWrappable` `PathSetter` `Part`
  `StorePathRange` `ArrayFilterFn` `CustomPartial` `Merge` `Omit`
- **Reactivity**: `ComputeFunction` `EffectBundle` `EffectFunction` `SignalOptions` `SourceAccessor` `NoInfer`
  `ExternalSource` `ExternalSourceFactory`
- **Components** ([`component.ts:10-66`](../solid/packages/solid/src/client/component.ts#L10-L66)): `Component`
  `VoidComponent` `ParentComponent` `FlowComponent` `VoidProps` `ParentProps` `FlowProps` `ComponentProps` `Ref`
- **Children** ([`core.ts`](../solid/packages/solid/src/client/core.ts)): `ChildrenReturn` `ResolvedChildren`
  `ResolvedElement` `ContextProviderComponent`; `ArrayElement` / `Element` from
  [`types.ts`](../solid/packages/solid/src/types.ts)

Most matter only if a user writes a generic helper over Solid types. The **component types are the exception** —
`ParentProps`/`FlowProps`/`Ref` shape how consumers type their own components, so they're worth doing early.

---

## 5. JSX / DOM attribute surface

This is the **largest binding surface in the repo**. It has now been diffed name-by-name against the pinned
dom-expressions checkout — see [Reconciliation](#reconciliation-diffed-against-the-pinned-checkout) below.

### Where it comes from

Solid does not author its JSX types. [`solid-web/package.json:383`](../solid/packages/solid-web/package.json#L383)
(`types:copy-jsx`) copies them out of the dom-expressions runtime and rewrites the element type:

```
ncp ../../node_modules/@dom-expressions/runtime/src/jsx.d.ts            ./src/jsx.d.ts
ncp ../../node_modules/@dom-expressions/runtime/src/jsx-properties.d.ts ./src/jsx-properties.d.ts
dom-expressions-jsx-types --input ./src/jsx.d.ts \
  --element "SolidElement | Node | ArrayElement" \
  --import 'import type { Element as SolidElement } from "solid-js";'
```

Consequences:

- `jsx.d.ts` / `jsx-properties.d.ts` **do not exist** in the solid checkout's `solid-web/src` — they are build
  artifacts. That is why [`solid-web/src/index.ts:36`](../solid/packages/solid-web/src/index.ts#L36) imports from
  a `./jsx.js` you cannot open.
- **dom-expressions is the source of truth** for element attributes, ARIA, SVG, and event handler names.
- Solid's only edit is the *element return type*; the attribute surface passes through unchanged. So diffing our
  bindings against dom-expressions is legitimate — no Solid-specific filter to account for.

Pinned version: **`@dom-expressions/runtime` `0.50.0-next.42`**
([`solid/package.json:36`](../solid/package.json#L36)) — a `next`-line prerelease, matching the Solid 2 RC.

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

Every count below is a name-level `comm` diff between the pinned
[`jsx.d.ts`](../dom-expressions/packages/runtime/src/jsx.d.ts) and our sources, normalised for casing and for our
`aria*`/trailing-apostrophe naming conventions.

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
([`jsx.d.ts:932`](../dom-expressions/packages/runtime/src/jsx.d.ts#L932),
[`:3989`](../dom-expressions/packages/runtime/src/jsx.d.ts#L3989)) with **zero** counterpart here. This is the
single largest untouched surface. It is also entirely additive — no existing binding changes.

**Global attributes** — we are a superset (129 vs 45) because our `HTMLAttributes` block also carries events and
convenience members. Genuinely absent: `autocorrect` `autofocus` `elementtiming` `enterkeyhint` `nonce`, plus the
two upstream marks `@experimental` (`virtualkeyboardpolicy`, `writingsuggestions`).

**Events** — the largest real gap. The 24 window-scoped ones (`beforeunload`, `hashchange`, `popstate`, `storage`,
`unload`, the `device*`/`gamepad*` family, …) come from
[`EventHandlersWindow`](../dom-expressions/packages/runtime/src/jsx.d.ts#L700) and only apply to `<body>`/`<html>`,
so skipping them is defensible. The 22 element-scoped ones are not: `beforematch` `beforexrselect` `command`
`contentvisibilityautostatechange` `contextlost` `contextrestored` `cuechange` `formdata` `fullscreenchange`
`fullscreenerror` `pointerrawupdate` `resize` `scrollsnapchange` `scrollsnapchanging` `securitypolicyviolation`
`selectionchange` `selectstart` `slotchange` `animationcancel` `beforecopy` `beforecut` `beforepaste`. We also
bind `onEncrypted`, which upstream does not.

**Element interfaces** — missing `Bdo`, `Body`, `Caption`. Upstream `ModHTMLAttributes` (for `<ins>`/`<del>`) is
our `InsHTMLAttributes` — a naming difference, not a gap. `WebView` is Electron-only and can be ignored.

### `jsx-properties.d.ts` is not a plugin concern

[`jsx-properties.d.ts`](../dom-expressions/packages/runtime/src/jsx-properties.d.ts) (93 lines) is **pure
type-level machinery** for deriving the `prop:*` key set — `SkipPropsFrom`, `PropValue`, `WidenPropValue`,
`IfEquals`, `IsReadonlyKey`, `PropKey`. `IsReadonlyKey` compares `Pick<T,K>` against `Readonly<Pick<T,K>>` purely
to exclude readonly DOM properties from the typed surface. It does **not** describe a property-vs-attribute split
that the compiler emits, so it imposes no requirement on our plugin and needs no F# analogue.

### Namespaces: only `prop:` survives

On this branch `prop:` is the sole reserved JSX namespace —
[`set_attr.rs:39`](../dom-expressions/packages/compiler/src/dom/set_attr.rs#L39) guards on
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

Outstanding, verified by grep — **2** snapshot cases are stubbed with `failwith "redo"`:

- [`SolidCases/…/OperatorsInProps.fs`](../Partas.Solid.Tests.Plugin/Compiled/SolidCases/Property%20getters%20mixed%20with%20Operands%20are%20transformed/OperatorsInProps.fs)
- [`SolidCases/…/SignalSetterInvoke.fs`](../Partas.Solid.Tests.Plugin/Compiled/SolidCases/Signal%20Setters%20can%20be%20invoked%20with%20a%20handler/SignalSetterInvoke.fs)

([`CHANGES-solid2.md`](CHANGES-solid2.md) also lists `ThisArgTransforms` and `ExperimentalBuilders` as pending;
they no longer carry the marker, so that note is stale — confirm they pass and correct the doc.)

---

## 7. Blind spots — things these submodules cannot answer

Flagging these so nobody burns time searching for them:

1. **The JSX / DOM attribute surface is not in the `solid/` checkout** — see [§5](#5-jsx--dom-attribute-surface)
   for the full explanation. `jsx.d.ts` is a build artifact copied from `@dom-expressions/runtime`, an npm
   dependency that is not installed. Resolving this needs `ryansolid/dom-expressions` (pin
   `v0.50.0-next.42`) added as a second submodule; **that add is still outstanding**, so
   [`HtmlAttributes.fs`](../Partas.Solid/HtmlAttributes.fs),
   [`AriaAttributes.fs`](../Partas.Solid/AriaAttributes.fs), and [`Svg.fs`](../Partas.Solid/Svg.fs) remain
   unreconciled.
2. **Router / meta / start are separate repositories.** `solid/packages/` has no `router`, `meta`, or `start`.
   [`SolidRouterBindings.fs`](../Partas.Solid/SolidRouterBindings.fs) (27 imports) is the second-largest binding
   file and is entirely unverified by this pin. If router coverage matters, add `solidjs/solid-router` as a
   second submodule.
3. **Server vs client builds diverge.** [`solid/packages/solid/src/server/`](../solid/packages/solid/src/server/)
   is a distinct entry from `src/client/`. Anything SSR-sensitive should be checked against the server entry, not
   just the client one.

---

## 8. Suggested order of work

1. **Correct the stale note** in [`CHANGES-solid2.md`](CHANGES-solid2.md) about
   `ThisArgTransforms`/`ExperimentalBuilders`; land the two real `failwith "redo"` cases. *(unblocks the test
   suite as a signal)*
2. **`createOwner`** — closes the only asymmetric primitive pair in the bound surface.
3. **Component type aliases** (`Component` `ParentProps` `FlowProps` `Ref` …) — user-facing typing ergonomics.
4. **`isWrappable` `isEqual` `NotReadyError` `storePath`** + store path types — small, mechanical, removes the
   existing [`// todo`](../Partas.Solid/SolidBindings.fs#L749).
5. **Decide where SSR request/response plumbing lives** (web vs start bindings) before binding it.
6. **Add the `dom-expressions` submodule and run the JSX diff** ([§5](#5-jsx--dom-attribute-surface)). Start with
   `jsx-properties.d.ts` — it may change plugin emission, not just bindings — then audit
   [`Svg.fs`](../Partas.Solid/Svg.fs) (most likely under-bound at 145 members),
   [`HtmlAttributes.fs`](../Partas.Solid/HtmlAttributes.fs), and
   [`AriaAttributes.fs`](../Partas.Solid/AriaAttributes.fs). This is the largest unknown-unknown in the port.

   ```
   git submodule add -b main https://github.com/ryansolid/dom-expressions.git dom-expressions
   git -C dom-expressions checkout v0.50.0-next.42   # match solid/package.json
   ```
7. Optional: second submodule for `solid-router` if router bindings need verification.
