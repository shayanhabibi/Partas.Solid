---
title: Migrating to Solid 2
---

Partas.Solid 3.0 targets Solid 2.0 (`2.0.0-rc.9`). Partas.Solid 2.x targeted Solid 1.9. Solid 2 renamed,
removed or reworked a large part of its runtime API, and the bindings follow it. Expect to touch
every component that does more than render markup.

This page goes through the changes in the order you are likely to hit them. Each section shows the
2.x code and what replaces it. For the full 3.0 API, see [Solid-JS](../guide/solid-js.md).

## Checklist

1. Update the packages: Partas.Solid and Partas.Solid.FablePlugin 3.0, `solid-js` and `@solidjs/web`
   2.0. The plugin needs Fable 5.
2. Rename `.classList` to `.class'`. Replace `.on (...)`, `.prop (...)` and `.use' (...)` calls.
3. Rename the control-flow components: `ErrorBoundary` to `Errored`, `Suspense` to `Loading`,
   `SuspenseList` to `Reveal`, `For` to `For.Keyed` or `For.NonKeyed`. Drop `Index`.
4. Replace `splitProps` and `mergeProps` with `omit` and `merge`.
5. Split each `createEffect` into a compute function and an effect function. Return cleanups from the
   effect instead of calling `onCleanup` in it.
6. Rework code built on `createResource`, `batch`, `onMount`, `createSelector`, `createComputed`,
   `startTransition`, `useTransition`, `produce` or `unwrap`.
7. Pass store setters an updater: `setStore (fun s -> ...)`. Change `solid-js/store` imports to
   `solid-js`.
8. Write two-argument callbacks (`mapArray`, `createErrorBoundary`) as curried lambdas.
9. Add `open Partas.Solid.Web` where you use `render`, `hydrate`, `renderToString`, `isServer`,
   `Portal` or `Dynamic`.
10. Rewrite `batch { }` and `selector { }` blocks. Give each `effect { }` block a `let!`.
11. Regenerate any committed JSX snapshots.

## Props: omit and merge

Solid 2 replaced `splitProps` and `mergeProps` with `omit` and `merge`. `omit` returns only the rest
of the props, so there is no longer a "local" half to read from.

For `[<SolidTypeComponent>]` members, your F# does not change. The plugin generates the calls, and
only the JSX differs:

| 2.x output | 3.0 output |
| --- | --- |
| `const [PARTAS_LOCAL, PARTAS_OTHERS] = splitProps(props, ["class"])` | `const PARTAS_OTHERS = omit(props, "class")` |
| `PARTAS_LOCAL.class` | `props.class` |
| `props = mergeProps({ class: "x" }, props)` | `props = merge({ class: "x" }, props)` |
| `import { splitProps, mergeProps } from "solid-js"` | `import { omit, merge } from "solid-js"` |

When a component reads no props, the plugin drops the `omit` call and its import:

```jsx
export function MyTag(props) {
    const PARTAS_OTHERS = props;
    return <div />;
}
```

If you called `splitProps` or `mergeProps` yourself, call `omit` and `merge` instead:

```fsharp
// 2.x
let local, others = splitProps (props, [| "class"; "size" |])
let withDefaults = mergeProps ({| size = "md" |}, props)

// 3.0
let others = omit (props, "class", "size")
let withDefaults = merge ({| size = "md" |}, props)
```

Read the omitted props straight from `props`.

## The self identifier can have any name

In 2.x the self identifier of a `[<SolidTypeComponent>]` member had to be `props`. In 3.0 any name
works, and it appears in the JSX as written:

```fsharp
[<Erase>]
type Button() =
    interface RegularNode
    [<Erase>] member val size: string = unbox null with get, set
    [<Erase>] member val variant: string = unbox null with get, set

    [<SolidTypeComponent>]
    member this.constructor =
        button (class' = $"btn {this.size} {this.variant}").spread this
```

```jsx
export function Button(this$) {
    const PARTAS_OTHERS = omit(this$, "size", "variant");
    return <button class={`btn ${this$.size} ${this$.variant}`} {...PARTAS_OTHERS} n$={false} />;
}
```

The member must still be an instance member with a single unit parameter, on a type in the
`Partas.Solid` namespace. See [SolidTypeComponent](../guide/solid-type-attribute.md).

## New component flags

Two new `ComponentFlag`s control the props preamble:

- `ComponentFlag.SkipOmit` leaves out the `omit` call.
- `ComponentFlag.SpreadProps` makes `.spread` spread the self identifier instead of `PARTAS_OTHERS`.

With either flag, `.spread props` compiles to `{...props}`. A wrapper can pass its props on unchanged
without paying for `omit`:

```fsharp
[<SolidTypeComponent(ComponentFlag.SkipOmit ||| ComponentFlag.SpreadProps)>]
member props.comp = For(keyed = !^true).spread props
```

See [Attribute flags](../guide/attribute-flags.md). The existing flags are unchanged.

## The spread marker

A spread used to emit a `bool:`-prefixed marker prop. The prefix is gone:

```diff
- <div {...PARTAS_OTHERS} bool:n$={false}>
+ <div {...PARTAS_OTHERS} n$={false}>
```

This only matters if you post-process the generated JSX.

## Extension methods

| 2.x | 3.0 |
| --- | --- |
| `.classList (obj)` | `.class' (obj)` |
| `.on (name, handler)` | Removed |
| `.prop (name, value)` | Removed |
| `.use' (name, value)` | Removed. Solid 2 has no `use:` directives. |
| `.bool (name, value)`, emitting `bool:name` | `.bool (name, value)`, emitting `name` |
| `.spread` on `#HtmlTag` | `.spread` on `#HtmlElement` |

`.attr`, `.data`, `.ref`, `.style'`, `.class'`, `.bool` and `.spread` remain. Of those, only `.data`
still adds a prefix (`data-name`).

```fsharp
// 2.x
div().classList {| active = isActive () |}

// 3.0
div().class' {| active = isActive () |}
```

There is no replacement for `.on` or `.use'` yet. Use the typed event attributes (`onClick = ...`),
or an `OnHandler` for `once`, `passive` and `capture`. For a directive, use `createDirectiveFactory`
and `.ref`:

```fsharp
// 2.x
input().use' ("autofocus", true)

// 3.0
let autofocus = createDirectiveFactory (fun (el: Browser.Types.HTMLElement) ->
    el.focus ()
    ignore)

input().ref autofocus
```

See [Extension methods](../guide/extension-methods.md).

## Control flow

| 2.x | 3.0 |
| --- | --- |
| `ErrorBoundary` | `Errored` |
| `Suspense` | `Loading` |
| `SuspenseList` | `Reveal` |
| `For<'T>` | `For.Keyed<'T>`, `For.NonKeyed<'T>` or `For.KeyedFn<'T>` (`For.Component<'T>` is `For.Keyed`) |
| `Index<'T>` | `For.NonKeyed<'T>` |
| `Show`, `Switch`, `Match` | Unchanged, plus `Show.Keyed`, `Show.NonKeyed`, `Match.Keyed` and `Match.NonKeyed` |
| none | `Repeat`, which renders by count instead of by array |
| `Portal`, `Dynamic` in `Partas.Solid` | The same names in `Partas.Solid.Web` |

### For and Index

```fsharp
// 2.x
For(each = items ()) {
    yield fun item index -> li () { item.name }
}

Index(each = names ()) {
    yield fun name index -> li () { name () }
}

// 3.0
For.Keyed(each = items ()) {
    yield fun item index -> li () { item.name }
}

For.NonKeyed(each = names ()) {
    yield fun name index -> li () { name () }
}
```

`For.Keyed` gives the row the item and an index accessor. `For.NonKeyed` gives it an item accessor
and a plain index.

### ErrorBoundary to Errored

The fallback function now gets the error as an accessor, and `fallback` is a union of an element and
a function, so the function needs `!^`. `plainFallback` is gone.

```fsharp
// 2.x
ErrorBoundary(fallback = ErrorBoundary.Fallback(fun err reset -> div () { string err })) {
    Risky ()
}

// 3.0
Errored(
    fallback =
        !^(ErrorBoundary.Fallback(fun err reset ->
            div () { (err () :?> exn).Message }))
) {
    Risky ()
}
```

`Errored` also has `fallbackFn` and `fallbackEle` setters, but they do not work yet. Set `fallback`
with `!^` as above.

### Suspense and SuspenseList

```fsharp
// 2.x
SuspenseList(revealOrder = SuspenseList.RevealOrder.Forwards) {
    Suspense(fallback = Spinner ()) { Profile () }
    Suspense(fallback = Spinner ()) { Posts () }
}

// 3.0
Reveal(order = Reveal.Order.Sequential) {
    Loading(fallback = Spinner ()) { Profile () }
    Loading(fallback = Spinner ()) { Posts () }
}
```

## Removed and replaced primitives

Solid 2 removed these. The bindings for them are gone too.

| 2.x | 3.0 |
| --- | --- |
| `createResource` and its types | An async `createMemo`, under `Loading`. `refresh` re-runs it. |
| `createComputed` | A memo, or a writable derived signal (`createSignal (fun () -> ...)`) |
| `createDeferred` | None |
| `createSelector` | `createProjection` |
| `batch` | `flush`. Writes are always batched. |
| `startTransition`, `useTransition` | `action`, with `isPending` |
| `onMount` | `onSettled` |
| `on` | The compute half of `createEffect` |
| `catchError` | `createErrorBoundary` |
| `indexArray` | `mapArrayUnkeyed` |
| `produce` | The store setter: mutate the draft and return it |
| `unwrap` | `snapshot` |
| `SolidStoreSetter`, `SolidStorePath` | `StoreSetter<'T>` |
| `importComponent` | `importDynamic` with `lazy'` |
| `getListener` | `getObserver` |

### createResource

```fsharp
// 2.x
let user, manager = createResource (userId, fetchUser)

Suspense(fallback = p () { "Loading..." }) {
    p () { user.current.name }
}

// 3.0
let user = createMemo (fun (_: User option) -> fetchUser (userId ()))

Loading(fallback = p () { "Loading..." }) {
    p () { user().name }
}
```

An async memo suspends its readers until the promise settles, so you read the value directly.
`refresh user` replaces `manager.refetch ()`. `isPending (fun () -> box (user ()))` tells you a refetch is in flight.

### onMount and batch

```fsharp
// 2.x
onMount (fun () -> inputRef.focus ())

batch (fun () ->
    setFirst "Ada"
    setLast "Lovelace")

// 3.0
onSettled (fun () -> inputRef.focus ())

// Writes are batched anyway. Use flush to apply them now.
flush (fun () ->
    setFirst "Ada"
    setLast "Lovelace")
```

## Effects have two phases

Solid 2 has no single-function `createEffect`. An effect is a tracked compute function, which
returns a value, and an untracked effect function, which gets it.

```fsharp
// 2.x
createEffect (fun () ->
    console.log $"count is {count ()}")

// 3.0
createEffect (
    (fun (_: int option) -> count ()),
    fun (n: int) -> console.log $"count is {n}"
)
```

Return a cleanup from the effect function instead of calling `onCleanup` inside it. `createEffect`
has overloads for an effect function that returns a cleanup:

```fsharp
// 2.x
createEffect (fun () ->
    let id = JS.setInterval tick (delayMs ())
    onCleanup (fun () -> JS.clearInterval id))

// 3.0
createEffect (
    (fun (_: int option) -> delayMs ()),
    fun (ms: int) ->
        let id = JS.setInterval tick ms
        fun () -> JS.clearInterval id
)
```

`on (deps, fn)` is gone as well: the compute function already names what the effect depends on.
When you cannot separate the two phases, `createTrackedEffect (fun () -> ...)` runs one function
that both tracks and acts.

## Stores

Stores moved into `solid-js`. There is no `solid-js/store` module.

The store setter takes a single updater, `'T -> 'T`. Path setters are gone, and so is `produce`,
because mutating the draft is now the default. Mutate the draft and return it, or return a new
value:

```fsharp
// 2.x
setState.Path.Map(_.todos).Update (fun todos -> Array.append todos [| todo |])
setState.Update (produce (fun s -> s.count <- s.count + 1))

// 3.0
setState (fun s -> {| s with todos = Array.append s.todos [| todo |] |})

setState (fun s ->
    s.count <- s.count + 1
    s)
```

`createStore` now returns a `Store<'T>` rather than a bare `'T`. Read it through `.Value`:
`state.Value.todos`. `unwrap state` is now `snapshot state`. `reconcile next` returns an updater, so
`setState (reconcile next)` still works.

## Curried callbacks

The two-argument callbacks of `mapArray` and `createErrorBoundary` are now typed as `Func<_, _, _>`.
Write them as curried lambdas, `fun item index -> ...`, which F# converts to a two-argument JS
function. The 2.x signatures compiled to a one-argument JS function, so Solid never passed the index
or the reset function.

```fsharp
// map an array, with the index
let labels = mapArray (items, fun item index -> $"{index ()}: {item.name}")

// an error boundary, with its reset function
let view = createErrorBoundary ((fun () -> Risky ()), fun err reset -> Fallback (err ()) reset)
```

## lazy'

`lazy'` follows Solid 2's `lazy (fn, options?, moduleUrl?)`. `importComponent` is gone. Use Fable's
`importDynamic`:

```fsharp
// 2.x
let Settings = lazy' (fun () -> importComponent "./Settings.fs.jsx")

// 3.0
let Settings = lazy' (fun () -> importDynamic "./Settings.fs.jsx")

// a named export instead of the default one
let Panel = lazy' ((fun () -> importDynamic "./Panels.fs.jsx"), LazyOptions (``export`` = "Panel"))
```

## The web runtime

`solid-js/web` is now `@solidjs/web`. Its bindings are in the `Partas.Solid.Web` namespace:
`render`, `hydrate`, `renderToString`, `renderToStream`, `isServer`, `isDev`, `clientOnly`,
`httpHeader`, `httpStatus`, `Portal`, `Dynamic` and `HeadTag`.

```fsharp
// 2.x
open Partas.Solid

render ((fun () -> App ()), document.getElementById "root")

// 3.0
open Partas.Solid
open Partas.Solid.Web

render ((fun () -> App ()), document.getElementById "root")
```

Install `@solidjs/web` next to `solid-js`.

## Experimental builders

The builders in `Partas.Solid.Experimental` follow the new primitives:

- `effect { }` needs exactly one `let!`, which names the tracked source. The rest of the block is the
  effect.
- `mount { }` wraps `onSettled`.
- `lazyload { }` takes `importDynamic`.
- `batch { }` and `selector { }` are gone. Call `flush` and `createProjection` directly.

```fsharp
// 2.x
effect {
    console.log $"count is {count ()}"
}

// 3.0
effect {
    let! n = count
    console.log $"count is {n}"
}
```

See [Experimental features](../guide/experimental.md).

## Snapshots and generated JSX

If you commit the generated JSX, or compare against it in tests, regenerate it. Expect these
changes:

- `PARTAS_LOCAL` is gone. Props are read from the self identifier.
- `splitProps` and `mergeProps` become `omit` and `merge`, and the imports change to match.
- `bool:n$` becomes `n$`.
- Control-flow imports change: `KeyedFor`, `Errored`, `Loading`, `Reveal`.
- Anything from the web runtime is imported from `@solidjs/web`.

See [JSX output](../about/jsx-output.md) for how the plugin output is laid out.
