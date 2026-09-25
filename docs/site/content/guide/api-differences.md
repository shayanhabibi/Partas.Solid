---
title: API differences
---

:::info
These pages do not cover how to use Solid itself. See the [Solid documentation](https://docs.solidjs.com/) for that.
This page lists where the F# API differs noticeably from Solid's JavaScript API.
:::

Partas.Solid 3.0 binds Solid 2.0. If you know Solid 1.x, also read [Migrating to Solid 2](migrating-to-solid-2.md):
several APIs this page used to describe, such as `createResource` and store paths, no longer exist.

## Option objects

Where a Solid function takes an options object, the Partas.Solid binding usually turns the options into optional
parameters. Fable's `ParamObject` attribute packs them back into an object when it compiles the call. You can tell
from the signature. `createMemo`, for example:

```fsharp
[<ImportMember("solid-js"); ParamObject(1)>]
static member createMemo<'T>(
    compute: 'T option -> 'T,
    ?name: string,
    ?transparent: bool,
    ?equals: EqualityFunc<'T>,
    ?unobserved: unit -> unit,
    ?``lazy``: bool,
    ?sync: bool,
    ?loadingValue: 'T
) : Accessor<'T> = jsNative
```

```fsharp
let doubled = createMemo ((fun _ -> count () * 2), name = "doubled")
```

compiles to

```js
const doubled = createMemo((_arg) => count() * 2, { name: "doubled" });
```

Most of these functions also have an overload that takes the options as a typed object, such as `MemoOptions<'T>` or
`EffectOptions`.

## Special JSX attributes

_Adapted from [Oxpecker.Solid](https://lanayx.github.io/Oxpecker/src/Oxpecker.Solid/#special-jsx-attributes). Credit
to [Lanayx](https://github.com/Lanayx)._

Attributes that do not fit a typed constructor parameter are extension methods on the element:

| Method | Emits |
| --- | --- |
| `.attr(name, value)` | `name={value}`, for any attribute |
| `.data(name, value)` | `data-name="value"` |
| `.bool(name, value)` | `name={value}` |
| `.ref(element)` | `ref={element}` |
| `.style'(obj)` | `style={obj}`, a style object |
| `.class'(obj)` | `class={obj}`, a class object |
| `.spread(obj)` | `{...obj}` |

The `style` and `class'` constructor parameters take a string. For an object, use the extension methods. `.style'`
also takes a list of style pairs:

```fsharp
open Partas.Solid.Style

div().style' [ Style.backgroundColor Color.Red; "--my-var" ==> "12px" ] { "Styled" }
```

Solid 1's `on:`, `prop:` and `use:` escape hatches (`.on`, `.prop` and `.use'`) were removed in 3.0 and have no
replacement yet. Solid 2 merged `classList` into `class`, so `.classList` is now `.class'`. See
[extension methods](extension-methods.md) for details.

When you pass a variable to `.ref`, make it `mutable`:

```fsharp
open Browser.Types

[<SolidComponent>]
let RefVariable () =
    let mutable inputRef: HTMLInputElement = JS.undefined

    div () {
        input().ref (inputRef)
        button (onClick = fun _ -> inputRef.value <- "filled via ref") { "Fill" }
    }
```

`.ref` also takes a callback, `.ref (fun (el: HTMLDivElement) -> ...)`.

## Stores

In Solid 2, `createStore` is exported from `solid-js`. There is no `solid-js/store` any more. The binding returns a
`Store<'T> * StoreSetter<'T>`.

- Read the current value through `.Value`, or let the implicit conversion to `'T` do it.
- The setter takes an **updater**, never a plain value. Mutate the draft and return it, or return a new value.

```fsharp
type Counter = { mutable count: int }

let state, setState = createStore { count = 0 }

let increment () =
    setState (fun s ->
        s.count <- s.count + 1
        s)

p () { state.Value.count }
```

`reconcile next` returns an updater too, so it reads `setState (reconcile next)`.

The Oxpecker-style store path helper (`setStore.Path.Map(...).Update(...)`) was removed along with `SolidStorePath`.
Use an updater instead.

## Async data

Solid 2 removed `createResource`, so the old `resource.current` helper is gone too. An async value is now a memo whose
computation returns a promise. Read it like any accessor, and put a `Loading` boundary above the reader:

```fsharp
let user = createMemo (fun (_: User option) -> fetchUser (userId ()))

Loading(fallback = p () { "Loading..." }) {
    p () { user().Name }
}
```

See [Building the DOM](building-the-dom.md).

## Router

_Adapted from [Oxpecker.Solid](https://lanayx.github.io/Oxpecker/src/Oxpecker.Solid/#router). Credit to
[Lanayx](https://github.com/Lanayx)._

The `@solidjs/router` bindings live in the `Partas.Solid.Router` namespace. The function that renders the router
needs `[<SolidComponent>]` like any other component. Pass components to routes as a `TagValue` with `!@`:

```fsharp
open Partas.Solid.Router
open Partas.Solid.Web
open Browser.Dom

[<SolidComponent>]
let MyRouter () =
    Router() {
        Route(path = "/", component' = !@Home)
        Route(path = "/about", component' = !@About)
    }

render ((fun () -> MyRouter ()), document.getElementById "root") |> ignore
```

You still need `@solidjs/router` in `package.json`. The router bindings have not been checked against a Solid 2
release of the router yet. See [Solid Router](solid-router.md).

## Lazy components

`lazy'` binds Solid's `lazy(fn, options?, moduleUrl?)`. `importComponent` was removed. Use Fable's `importDynamic`
(from `Fable.Core.JsInterop`) for the import, and name the export with `LazyOptions`:

```fsharp
open Fable.Core.JsInterop

let LazyPanel: LazyComponent<HtmlElement> =
    lazy' ((fun () -> importDynamic "./Panel.fs.jsx"), LazyOptions(``export`` = "Panel"))
```

That compiles to

```js
const LazyPanel = lazy(() => import("./Panel.fs.jsx"), { export: "Panel" });
```

Render it with `Dynamic` under a `Loading` boundary. `lazyload { ... }` in `Partas.Solid.Experimental` is a builder
alternative. See [Experimental](experimental.md).

## Signal setters

A signal is a getter and setter pair. In F# their types are:

```fsharp
type Accessor<'T> = unit -> 'T
type Setter<'T> = 'T -> unit
```

A Solid setter can also take a function of the previous value. In F#, call it through `Invoke`:

```fsharp solid
let clicks, setClicks = createSignal 0

button (onClick = fun _ -> setClicks.Invoke(fun previous -> previous + 1)) {
    $"Clicked {clicks ()} times"
}
```

`setter.Invoke(value)` is the same as `setter value`.

:::warning
`InvokeAndGet`, which was meant to return the value that was set, currently returns `undefined`. Read the accessor
after setting instead.
:::

## Context

A context is created with `createContext` and read with `useContext`:

```fsharp
let ThemeContext = createContext<string> "light"

[<SolidComponent>]
let ThemedLabel () =
    let theme = useContext ThemeContext
    span () { theme }
```

`tryUseContext` returns a `Result` instead of throwing when no provider or default exists.

To provide a value, call the context with it and pass children:

```fsharp
[<SolidComponent>]
let ThemeApp () =
    ThemeContext "dark" {
        ThemedLabel()
    }
```

:::danger
The provider syntax is broken on Solid 2. The plugin still emits `<ThemeContext.Provider value="dark">`, the Solid 1
form. In Solid 2 the context object is itself the provider and has no `.Provider` member, so this renders an undefined
component. `useContext` and context defaults work. Providing a value does not, until the plugin is fixed.
:::
