---
title: SolidStart
---

Light bindings for [`@solidjs/start`](https://docs.solidjs.com/solid-start/). They live in the `Partas.Solid.Start`
namespace, which ships inside the `Partas.Solid` package. They do not cover the whole library.

```fsharp
open Partas.Solid
open Partas.Solid.Start
```

Add the npm package yourself:

```bash
npm install @solidjs/start
```

:::warning
These bindings were written for SolidStart on Solid 1.x and have not changed for Partas.Solid 3.0. SolidStart is a
separate repository that is not vendored here, and no test covers these bindings. Nothing on this page is verified
against Solid 2.
:::

Much of SolidStart lives in configuration and entry files with fixed names and extensions, such as `app.config.ts`
and `entry-server.tsx`. Fable writes `.fs.jsx` files, so several of these bindings are of limited use. Write those files
in JavaScript and keep your components in F#.

## Solid 2 changes that affect SolidStart code

Solid 2 moved several server helpers into `@solidjs/web`. Partas.Solid 3.0 binds them in `Partas.Solid.Web`:

| SolidStart binding | `@solidjs/web` binding |
| --- | --- |
| `HttpStatusCode` component | `httpStatus(code: System.Net.HttpStatusCode, ?text: string): unit` |
| `HttpHeader` component | `httpHeader(name: string, value: string, ?append: bool): unit` |
| `clientOnly(importFunc: unit -> Promise<HtmlElement>): TagValue` | `clientOnly(fn: unit -> Promise<'T>, ?lazy: bool): unit -> 'T` |

Both `clientOnly`s are in scope when you open `Partas.Solid.Start` and `Partas.Solid.Web` together. They import from
different packages and return different types, so qualify the one you mean.

`isServer`, `renderToString` and `renderToStream` are in `Partas.Solid.Web` as well. `isServer` and `renderToString`
are no longer on the core `Bindings` type. See [Migrating to Solid 2](migrating-to-solid-2.md).

## Functions

### useServer

Emits a `"use server"` directive.

::::tabs
:::tab F#
```fsharp
useServer
```
:::
:::tab JSX
```jsx
"use server";
```
:::
::::

`useServer` is a static property, not a function, so write it without `()`.

### getServerFunctionMeta

Call it inside a server function to read the function's metadata. It returns the `id` field directly, as a `string`.

### defineConfig

```fsharp
defineConfig(config: obj): obj
defineConfig(objList: (string * obj) list): obj
```

Imports from `@solidjs/start/config`. The list overload builds the object with `createObj`. It is rarely useful,
because the config file needs a specific name and extension.

### Client.mount

```fsharp
Client.mount(fn: unit -> HtmlElement, el: obj)
```

Imports from `@solidjs/start/client`, for the client entry:

```fsharp
Client.mount ((fun () -> StartClient()), document.getElementById "app")
```

### clientOnly

```fsharp
clientOnly(importFunc: unit -> JS.Promise<HtmlElement>): TagValue
```

Takes a lambda that dynamically imports a component, so the component only renders on the client. See the table
above for the `@solidjs/web` version.

### createHandler

```fsharp
createHandler(fn: unit -> StartServer, ?mode: string): unit
```

Imports from `@solidjs/start/server`. `mode` is passed in an options object.

## Components

| Component | Imported from | Properties |
| --- | --- | --- |
| `StartServer` | `@solidjs/start/server` | `document` (see below) |
| `StartClient` | `@solidjs/start/client` | |
| `HttpStatusCode` | `@solidjs/start` | `code: HttpStatusCode` |
| `HttpHeader` | `@solidjs/start` | `name: string`, `value: string` |
| `FileRoutes` | `@solidjs/start/router` | |

`StartServer.document` renders the HTML shell:

```fsharp
val mutable document: {| children: HtmlElement; assets: link; scripts: script |} -> html
```

`HttpStatusCode` the component takes `code` as the `Partas.Solid.Start.HttpStatusCode` enum, a copy of
`System.Net.HttpStatusCode` that shares the component's name.

`FileRoutes` has a `ToRoute()` member that casts it to a router `Route`, so you can yield it inside a `Router`. This
type-checks, but no test pins the JSX it produces:

```fsharp
open Partas.Solid.Router

[<SolidComponent>]
let App () =
    Router() {
        FileRoutes().ToRoute()
    }
```
