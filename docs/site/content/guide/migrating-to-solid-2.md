---
title: Migrating to Solid 2
---

Partas.Solid 3.0 targets Solid `2.0.0-rc.14`; 2.x targeted Solid 1.9. Expect to touch every component that does more
than render markup. Each section shows the 2.x code and what replaces it. The full 3.0 API is in
[Solid-JS](solid-js.md).

## Checklist

1. Update the packages: Partas.Solid 3.0, Fable 5, `solid-js` and `@solidjs/web` 2.0 (see
   [Installation](installation.md)). Compile with `--typedArrays false`.
2. Replace `Fable.Browser.Dom` with `open Fable.Core.TS` and `Dom.X` ([DOM types](#dom-types)).
3. Replace string values of closed attributes with `Attr.*` cases ([Attributes](#attributes-and-events)).
4. Rename `.classList` to `.class'`; replace `.on`, `.prop` and `.use'` ([Extension methods](#extension-methods)).
5. Rename the control flow: `Errored`, `Loading`, `Reveal`, `For.Keyed` / `For.NonKeyed` ([Control flow](#control-flow)).
6. Replace `splitProps` / `mergeProps` with `omit` / `merge` ([Props](#props-omit-and-merge)).
7. Split each `createEffect` into compute and effect functions ([Effects](#effects-have-two-phases)).
8. Rework `createResource`, `batch`, `onMount`, `createSelector`, store setters and the removed primitives
   ([table](#removed-and-replaced-primitives)).
9. Add `open Partas.Solid.Web` for `render`, `Portal`, `Dynamic` and the rest of the [web runtime](#the-web-runtime).
10. Regenerate any committed JSX snapshots.

## Packages and bindings that moved

| 2.x | 3.0 |
| --- | --- |
| `Fable.Browser.Dom` | `Xantham.Fable.Core.TS`, from its own Cloudsmith feed ([Installation](installation.md)) |
| `solid-js/web` | `@solidjs/web`, bound in `Partas.Solid.Web` |
| `solid-js/store` | `solid-js` |
| `@solidjs/meta` with `MetaProvider` | `@solidjs/meta` 1.0, no provider ([Solid Meta](solid-meta.md)) |
| `Router`, `Route`, `A`, `Navigate` | `@solidjs/router` 2: `createRouter` with `route` definitions, plain `a` links ([Solid Router](solid-router.md)) |
| SolidStart bindings | Removed |

## DOM types

DOM types come from `Fable.Core.TS.Dom`. Write them as `Dom.X` after `open Fable.Core.TS`; opening
`Fable.Core.TS.Dom` would shadow Partas's `Attr` module with DOM's `Attr` type. `document`, `window` and
`globalThis` are bound by Partas.Solid and come in with `open Fable.Core`. Lookups return options:

```fsharp
// 2.x
open Browser.Types
let el: HTMLElement = Browser.Dom.document.getElementById "root"

// 3.0
open Fable.Core.TS
let el: Dom.HTMLElement = (document.getElementById "root").Value
```

## Attributes and events

The HTML, ARIA and SVG attributes are generated from Solid's JSX declarations:

- A closed set of values is a `[<StringEnum>]` union in `Attr`: `type' = Attr.ButtonType.Button`,
  `role = Attr.AriaRole.Dialog`.
- An attribute that is only `"true"`/`"false"` is a `bool`.
- Names follow Solid's lowercase spelling. The old camelCase spellings still compile, as `[<Obsolete>]` aliases.
- Event handlers take their event type: `onClick: Dom.MouseEvent -> unit`.
- `<input>`'s `min`, `max` and `step` take a number or a string.

```fsharp solid jsx render=TypedAttributes
[<SolidComponent>]
let TypedAttributes () =
    let position, setPosition = createSignal "not clicked"

    div () {
        button (type' = Attr.ButtonType.Button, onClick = fun e -> setPosition $"{e.clientX}, {e.clientY}") {
            "Click me"
        }
        p () { $"Last click: {position ()}" }
    }
```

See [Oxpecker DSL](oxpecker-dsl.md), [ARIA attributes](aria-attributes.md) and [SVG elements](svg-elements.md).

## Props: omit and merge

Solid 2 replaced `splitProps` and `mergeProps` with `omit` and `merge`. `omit` returns only the rest of the props,
so there is no `PARTAS_LOCAL` any more: read props straight from the self identifier.

A `[<SolidTypeComponent>]` needs no change. The JSX tab shows the new preamble: `merge` for the default set in the
body, `omit` for the props it reads.

```fsharp solid jsx render=ChipRow
[<Erase>]
type Chip() =
    inherit span()

    [<Erase>]
    member val tone: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        props.tone <- "info"
        span(class' = "chip chip-" + props.tone).spread props { props.children }

[<SolidComponent>]
let ChipRow () =
    div () {
        Chip () { "Default tone" }
        Chip (tone = "warning") { "Warning tone" }
    }
```

A component that reads no props gets `const PARTAS_OTHERS = props` and no import. If you called the functions
yourself:

```fsharp
// 2.x
let local, others = splitProps (props, [| "class"; "size" |])
let withDefaults = mergeProps ({| size = "md" |}, props)

// 3.0
let others = omit (props, "class", "size")
let withDefaults = merge ({| size = "md" |}, props)
```

The self identifier no longer has to be `props`: `member this.View` compiles to `function Chip(this$)`. Two new
flags control the preamble: `ComponentFlag.SkipOmit` drops the `omit`, and `ComponentFlag.SpreadProps` makes
`.spread` spread the self identifier. See [Attribute flags](attribute-flags.md).

The marker after a spread lost its prefix: `bool:n$={false}` is now `n$={false}`.

## Extension methods

| 2.x | 3.0 |
| --- | --- |
| `.classList (obj)` | `.classObj (obj)` |
| `.on (name, handler)` | Removed. Use the event attribute, or an `OnHandler` for `once`, `passive` and `capture` |
| `.prop (name, value)` | Removed |
| `.use' (name, value)` | Removed. Solid 2 has no `use:` directives. Pass `createDirectiveFactory` to `.ref` |
| `.bool (name, value)`, emitting `bool:name` | `.bool (name, value)`, emitting `name` |
| `.spread` on `#HtmlTag` | `.spread` on `#HtmlElement` |

```fsharp
// 2.x
input().use' ("autofocus", true)

// 3.0
let autofocus = createDirectiveFactory (fun (el: Dom.HTMLInputElement) ->
    el.focus ()
    ignore)

input().ref autofocus
```

See [Extension methods](extension-methods.md).

## Control flow

| 2.x | 3.0 |
| --- | --- |
| `ErrorBoundary` | `Errored` |
| `Suspense` | `Loading` |
| `SuspenseList` | `Reveal` |
| `For<'T>` | `For.Keyed<'T>`, `For.NonKeyed<'T>` or `For.KeyedFn<'T>` |
| `Index<'T>` | `For.NonKeyed<'T>` |
| `Show`, `Switch`, `Match` | Unchanged, plus `Show.Keyed`, `Show.NonKeyed`, `Match.Keyed` and `Match.NonKeyed` |
| none | `Repeat`, which renders by count |
| `Portal`, `Dynamic` in `Partas.Solid` | The same names in `Partas.Solid.Web` |

`For.Keyed` gives the row the item and an index accessor. `For.NonKeyed` gives it an item accessor and a plain index:

```fsharp solid jsx render=Lists
[<SolidComponent>]
let Lists () =
    let names = [| "Ada"; "Grace"; "Edsger" |]

    div () {
        ul () {
            For.Keyed(each = names) {
                yield fun name index -> li () { $"{index ()}: {name}" }
            }
        }
        ul () {
            For.NonKeyed(each = names) {
                yield fun name index -> li () { $"{index}: {name ()}" }
            }
        }
    }
```

`Errored`'s fallback gets the error as an accessor, and `fallback` is a union, so a function needs `!^` (or set
`fallbackFn` instead). `plainFallback` is gone.

```fsharp
// 2.x
ErrorBoundary(fallback = ErrorBoundary.Fallback(fun err reset -> div () { string err })) {
    Risky ()
}
SuspenseList(revealOrder = SuspenseList.RevealOrder.Forwards) {
    Suspense(fallback = Spinner ()) { Profile () }
}

// 3.0
Errored(fallback = !^(ErrorBoundary.Fallback(fun err reset -> div () { (err () :?> exn).Message }))) {
    Risky ()
}
Reveal(order = Reveal.Order.Sequential) {
    Loading(fallback = Spinner ()) { Profile () }
}
```

## Removed and replaced primitives

| 2.x | 3.0 |
| --- | --- |
| `createResource` | An async `createMemo`, under `Loading`. `refresh` re-runs it |
| `createComputed` | A memo, or a writable derived signal (`createSignal (fun () -> ...)`) |
| `createDeferred` | None |
| `createSelector` | `createProjection` |
| `batch` | `flush`. Writes are always batched |
| `startTransition`, `useTransition` | `action`, with `isPending` |
| `onMount` | `onSettled` |
| `on` | The compute half of `createEffect` |
| `catchError` | `Errored` |
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
Suspense(fallback = p () { "Loading..." }) { p () { user.current.name } }

// 3.0
let user = createMemo (fun (_: User option) -> fetchUser (userId ()))
Loading(fallback = p () { "Loading..." }) { p () { user().name } }
```

An async memo suspends its readers until the promise settles. `refresh user` replaces `manager.refetch ()`.
[Async memos](solid-js.md#async-memos) has a live example.

## Effects have two phases

An effect is a tracked compute function, which returns a value, and an untracked effect function, which gets it.
Return a cleanup from the effect function instead of calling `onCleanup` in it:

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

[Effects](solid-js.md#effects) has live examples. When the phases cannot be separated, `createTrackedEffect (fun () -> ...)` runs one function that tracks and acts.
In `Partas.Solid.Experimental`, `effect { }` needs exactly one `let!` naming the tracked source, and `batch { }` and
`selector { }` are gone ([Experimental](experimental.md)).

## Stores

The setter takes one updater, `'T -> 'T`. Path setters and `produce` are gone: mutate the draft and return it, or
return a new value. `createStore` returns a `Store<'T>`; read it through `.Value`.

```fsharp
// 2.x
setState.Update (produce (fun s -> s.count <- s.count + 1))

// 3.0
setState (fun s ->
    s.count <- s.count + 1
    s)
```

`setState (reconcile next)` still works. [Stores](solid-js.md#stores) has a live example.

## Curried callbacks

`mapArray`'s two-argument callback is a `Func<_, _, _>`: write it as a curried lambda. In 2.x it compiled to a
one-argument JS function, so Solid never passed the index.

```fsharp
let labels = mapArray (items, fun item index -> $"{index ()}: {item.name}")
```

## The web runtime

`Partas.Solid.Web` binds `@solidjs/web`: `render`, `hydrate`, `renderToString`, `renderToStream`, `isServer`, `isDev`,
`clientOnly`, `httpHeader`, `httpStatus`, `useHead`, `Portal` and `Dynamic`.

```fsharp
open Partas.Solid
open Partas.Solid.Web

render ((fun () -> App ()), (document.getElementById "root").Value)
```

## Snapshots

Regenerated JSX shows `omit`/`merge` instead of `splitProps`/`mergeProps`, no `PARTAS_LOCAL`, `n$` instead of
`bool:n$`, `Errored`/`Loading`/`Reveal` from `solid-js`, `KeyedFor`/`NonKeyedFor` from Partas.Solid's compiled
bindings, and web imports from `@solidjs/web`. See [JSX output](../about/jsx-output.md).
