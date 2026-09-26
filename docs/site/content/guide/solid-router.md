---
title: Solid Router
---

Bindings for [`@solidjs/router`](https://docs.solidjs.com/solid-router/). They live in the `Partas.Solid.Router`
namespace, which ships inside the `Partas.Solid` package.

```fsharp
open Partas.Solid
open Partas.Solid.Router
```

Add the npm package yourself. Partas.Solid does not install it:

```bash
npm install @solidjs/router
```

:::warning
These bindings were written for `@solidjs/router` on Solid 1.x and carried over to Partas.Solid 3.0 unchanged. They
compile, and the plugin emits the right imports for `Router` and `Route`, but nothing in this repository checks them
against a Solid 2 build of the router. The router is a separate repository that is not vendored here, and the runtime
test suite installs only `solid-js` and `@solidjs/web`. Treat every signature on this page as unverified on Solid 2,
and check the router's own docs for the release you install.
:::

## An example

```fsharp
[<SolidComponent>]
let Home () = h1 () { "Home" }

[<SolidComponent>]
let About () = h1 () { "About" }

[<Erase>]
type Layout() =
    inherit div()

    [<SolidTypeComponent>]
    member props.View =
        div () {
            nav () {
                A(href = "/") { "Home" }
                A(href = "/about", activeClass = "active") { "About" }
            }
            props.children
        }

[<SolidComponent>]
let App () =
    Router(root = !@Layout) {
        Route(path = "/", component' = !@Home)
        Route(path = "/about", component' = !@About)
    }
```

`Router` and `Route` are builders. A `Route` can hold nested `Route`s. `component'` and `root` take a `TagValue`:
use `!@Component` to pass a component rather than call it. The router passes the matched route to `root` as
`children`, which is why `Layout` is a type component that inherits `div` and so has a `children` property. Like
every `[<SolidTypeComponent>]`, `Layout` must be declared in a namespace or module whose name starts with
`Partas.Solid` (see [SolidTypeComponent](solid-type-attribute.md)).

What the plugin emits for `Router () { Route () }`:

```jsx
import { Route, Router } from "@solidjs/router";

<Router>
    <Route />
</Router>
```

## Components

### Router

```fsharp
type Router() = interface HtmlElement
```

| Property | Type |
| --- | --- |
| `root` | `TagValue` |
| `base'` | `string` |
| `actionBase` | `string` |
| `preload` | `bool` |
| `explicitLinks` | `bool` |
| `url` | `string` |

The body accepts `Route` children, or a `RootConfig[]` for config-based routing.

### HashRouter

```fsharp
type HashRouter() = inherit Router()
```

Same properties as `Router`.

### MemoryRouter

```fsharp
type MemoryRouter() = inherit Router()
```

Adds `history: MemoryHistory`. Create one with `createMemoryHistory ()`.

### MemoryHistory

```fsharp
member get(): string
member set(value: string, ?scroll: bool, ?replace: bool): unit   // emitted as one options object
member back()
member forward()
member go(n: int): unit
member listen(listener: string -> unit): unit -> unit
```

`listen` returns the function that removes the listener.

### Route

```fsharp
type Route() = interface HtmlElement
```

| Property | Type | Notes |
| --- | --- | --- |
| `path` | `string` | |
| `paths` | `string array` | Sets `path` to an array of paths. |
| `component'` | `TagValue` | |
| `matchFilters` | `obj` | |
| `preload` | `RoutePreloadFunc` | |

### RootConfig

```fsharp
[<Pojo>]
type RootConfig(path: string, ``component``: HtmlElement)
```

`component'` is an alias for ``` ``component`` ```.

### A

```fsharp
type A() = interface RegularNode
```

| Property | Type |
| --- | --- |
| `href` | `string` |
| `noScroll` | `bool` |
| `replace` | `bool` |
| `state` | `obj` |
| `activeClass` | `string` |
| `inactiveClass` | `string` |
| `end'` | `bool` |

### Navigate

```fsharp
type Navigate() = interface RegularNode
```

Properties: `href: string`, `state: obj`.

## Navigation

### useNavigate

```fsharp
useNavigate(): Navigator
```

`Navigator` has three ways to call it:

```fsharp
abstract Invoke: ``to``: string * ?options: NavigateOptions -> unit
abstract InvokeOptions: ``to``: string * ?resolve: bool * ?replace: bool * ?scroll: bool * ?state: obj -> unit
abstract Invoke: delta: float -> unit
```

`InvokeOptions` builds the options object for you, so you do not need a `NavigateOptions`:

```fsharp
let navigate = useNavigate ()
navigate.InvokeOptions ("/login", replace = true)
navigate.Invoke(-1.)
```

`NavigateOptions` is a `[<Pojo>]` with the optional arguments `resolve`, `replace`, `scroll` and `state`.

### useLocation

```fsharp
useLocation(): Location
```

```fsharp
type Path =
    abstract pathname: string
    abstract search: string
    abstract hash: string

type Location =
    inherit Path
    abstract query: obj
    abstract state: obj option
    abstract key: string
```

### useParams

```fsharp
useParams(): obj
```

### useSearchParams

```fsharp
useSearchParams(): Signal<obj>
```

See [Solid-js](solid-js.md) for `Signal<'T>`.

### useIsRouting

```fsharp
useIsRouting(): unit -> bool
```

### useMatch

```fsharp
useMatch(fn: unit -> string, ?matchFilters: obj): unit -> bool
```

### useBeforeLeave

```fsharp
useBeforeLeave(listener: BeforeLeaveEventArgs -> unit): unit
```

```fsharp
type BeforeLeaveEventArgs =
    abstract from: Location
    abstract ``to``: U2<string, float>
    abstract options: NavigateOptions option
    abstract defaultPrevented: bool
    abstract preventDefault: unit -> unit
    abstract retry: ?force: bool -> unit
```

### useCurrentMatches

```fsharp
useCurrentMatches(): unit -> RouteMatch[]
```

```fsharp
type PathMatch =
    abstract ``params``: obj
    abstract path: string

type RouteMatch =
    inherit PathMatch
    abstract route: RouteDescription

type RouteDescription =
    abstract key: obj
    abstract originalPath: string
    abstract pattern: string
    abstract preload: RoutePreloadFunc option
    abstract matcher: (string -> PathMatch option)
    abstract matchFilters: obj option
    abstract info: obj option
```

### usePreloadRoute

```fsharp
usePreloadRoute(): (string -> PreloadData) -> unit
```

`PreloadData` is a `[<Pojo>]` with one field, `preloadData: bool`.

:::warning
This return type does not match the router, whose `usePreloadRoute` returns a function taking a URL and an options
object. As bound, you cannot call the result with a path. Use `Fable.Core.JsInterop` until the binding is fixed.
:::

## Preloading

```fsharp
type RoutePreloadFunc = RoutePreloadFuncArgs -> unit

type RoutePreloadFuncArgs =
    abstract ``params``: obj
    abstract location: Location
    abstract intent: Intent
```

`Intent` is a `[<StringEnum>]` with `[<RequireQualifiedAccess>]`: `Intent.Initial`, `Intent.Native`,
`Intent.Navigate`, `Intent.Preload`.

## Data

### query

```fsharp
query<'Input, 'Output>(fn: 'Input -> 'Output, ?name: string): 'Input -> 'Output
query'<'Input, 'Output>(fn: 'Input -> 'Output, ?name: string): Query<'Input, 'Output>
```

`query` hands back a plain F# function. `query'` hands back a `Query`, which exposes the cache key:

```fsharp
type Query<'Input, 'Output> =
    abstract Invoke: 'Input -> 'Output
    abstract key: string
    abstract keyFor: 'Input -> string
```

If you used `query`, the extension members `.key` and `.keyFor value` on the returned function do the same thing.

### revalidate

```fsharp
revalidate(key: string, ?force: bool): unit
```

### createAsync

```fsharp
createAsync<'T>(
    fn: 'T option -> Promise<'T>,
    ?name: string,
    ?initialValue: 'T,
    ?deferStream: bool,
    ?onHydrated: unit -> unit,
    ?ssrLoadFrom: string,
    ?storage: unit -> Signal<'T>
    ): Accessor<'T>
```

`createAsyncWithLatest` takes the same arguments and returns an `AsyncAccessor<'T>`, which adds `latest: 'T`. On a
plain accessor the `.latest` extension member reads the same field.

### createAsyncStore

```fsharp
createAsyncStore<'T>(
    fn: 'T option -> Promise<'T>,
    ?name: string,
    ?initialValue: 'T,
    ?deferStream: bool,
    ?reconcile: obj,
    ?onHydrated: unit -> unit,
    ?ssrLoadFrom: string
    ): Accessor<'T>
```

`createAsyncStoreWithLatest` is the `AsyncAccessor<'T>` form.

:::warning
The router takes these options as one object: `createAsync(fn, { name, initialValue, ... })`. The bindings do not
mark the optional arguments as an options object, so Fable passes them positionally. Pass only `fn` until this is
fixed.
:::

:::note
Solid 2 removed `createResource`. In core Solid, async data now comes from `createMemo` or `createStore` given an async
function, with `Loading` and `isPending` to show progress. See [Migrating to Solid 2](migrating-to-solid-2.md). Whether
a Solid 2 router still ships `createAsync`, and in what shape, is not checked here.
:::

## Actions

:::note
`solid-js` 2.0 has its own `action`, bound on `Partas.Solid.Bindings`. With both `Partas.Solid` and
`Partas.Solid.Router` open, both are in scope under the same name. Qualify the one you mean:
`Partas.Solid.Router.Bindings.action`.
:::

### action

Actions only work with `POST` requests.

```fsharp
action<'Input, 'Output>(
    handler: 'Input -> Promise<'Output>,
    ?name: string,
    ?onComplete: Submission<'Input, 'Output> -> unit
    ): SolidAction<'Input, 'Output>
```

`name` and `onComplete` are passed as one options object.

```fsharp
type SolidAction<'Input, 'Result> =
    abstract url: string
    abstract with': 'Input -> string
```

### useAction

```fsharp
useAction<'Input, 'Result>(action: SolidAction<'Input, 'Result>): 'Input -> 'Result
```

This avoids `FormData`, but needs client-side JavaScript, so it is not progressively enhanced the way a form is.

### useSubmission, useSubmissions

```fsharp
useSubmission<'Input>(action: SolidAction<'Input, unit>, ?filter: 'Input -> bool): Submission<'Input, unit>
useSubmissions<'Input>(action: SolidAction<'Input, unit>, ?filter: 'Input -> bool): Submission<'Input, unit>[]
```

```fsharp
type Submission<'Input, 'Result> =
    abstract input: 'Input
    abstract result: 'Result option
    abstract error: obj
    abstract pending: bool
    abstract url: string
    abstract clear: (unit -> unit)
    abstract retry: (unit -> unit)
```

An array of submissions has a `.pending` extension member. It has no implementation (it compiles to `undefined`), so
check `Array.exists _.pending` yourself.

## Not bound

The bindings file ends with a note that `preload`, `json`, `redirect` and `reload` are not bound yet. Import them
with `Fable.Core.JsInterop.import` if you need them.
