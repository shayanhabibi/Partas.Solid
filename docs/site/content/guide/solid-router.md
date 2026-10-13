---
title: Solid Router
---

Bindings for [`@solidjs/router`](https://github.com/solidjs/solid-router) 2, in the `Partas.Solid.Router` namespace of
the `Partas.Solid` package. Install the npm package yourself:

```bash
npm install @solidjs/router@2.0.0-next.38
```

The Solid 2 router is configured, not composed. There are no `Router`, `Route` or `A` components: you declare the
route tree as data, create the router once, and render the router it returns. Links are plain `a` elements.

## Routes and the router

`route` declares a route. Its component is a component that takes `RouteSectionProps`, or `unit`, such as a
`[<SolidComponent>]` let binding. `createRouter` takes the tree and returns the router.

```fsharp
open Partas.Solid
open Partas.Solid.Router

[<SolidComponent>]
let Home () = h1 () { "Home" }

[<SolidComponent>]
let User (props: RouteSectionProps) =
    h1 () { "User " + (props.``params``["id"] |> Option.defaultValue "") }

[<SolidComponent>]
let Users (props: RouteSectionProps) =
    section () {
        h2 () { "Users" }
        props.children
    }

let Router =
    createRouter (
        RouterConfig(
            [| route ("/", Home)
               route ("/users", Users, children = [| route ("/:id", User, matchFilters = {| id = intFilter |}) |])
               route ("*404", Home) |]
        )
    )
```

Create the router at module level, and give it a capitalised name, in a capitalised module: the router is the component
you render, and the Solid compiler treats a lowercase tag as an HTML element. The plugin reports a lowercase one as an
error.

A parent with a component wraps its children, which render where it places `props.children`. `routePaths` matches
several paths with one route, and `routeGroup` groups children under a path without a component of its own.

| `route` argument | |
| --- | --- |
| `path` | The path. `:id` is a parameter, `:id?` an optional one, `*rest` the remainder of the path |
| `component` | The component to render |
| `children` | Nested routes |
| `preload` | Runs on navigation and link preloading. Its result reaches the component as `props.data` |
| `matchFilters` | An object of filters per parameter. `intFilter` accepts integers, an array lists the allowed values |
| `info` | Metadata, read back through `useRouteMatches` |

`RouterConfig` takes the routes, then `history` (`browserHistory ()` by default, `hashHistory ()`, or
`memoryHistory "/start"` for tests), `base`, `preload`, `links`, `preloadLinks` and the router's other options.

## Rendering the router

`Router.render ()` renders the matched route. To keep a layout mounted around it, render the router with a
`RouterRoot`, whose child lambda receives the matched content as `props.children`:

```fsharp
[<SolidComponent>]
let App () =
    Router % RouterRoot() {
        yield fun props ->
            div () {
                nav () {
                    a (href = "/") { "Home" }
                    a (href = "/users/1") { "User 1" }
                }
                main () { props.children }
            }
    }
```

```jsx
export function App() {
    return <Router>
        {(props) => <div>
            <nav>
                <a href="/">
                    Home
                </a>
                <a href="/users/1">
                    User 1
                </a>
            </nav>
            <main>
                {props.children}
            </main>
        </div>}
    </Router>;
}
```

The hooks below read the router's context, so call them in a component rendered under the router: a route, or a
component inside the root layout. `App` itself is outside it.

## Links

The router handles clicks on any same-origin `a`. Anchor attributes change how it navigates:

| Attribute | |
| --- | --- |
| `replace = true` | Replace the history entry instead of pushing one |
| `noscroll = true` | Do not scroll to the top |
| `state = "..."` | A JSON string, pushed as `location.state` |
| `preload = "false"` | Never preload this link. `"viewport"` or `"eager"` opts it into that strategy |
| `link = true` | Marks a router link when `explicitLinks = true` |

The router marks links for CSS: `aria-current="page"` on an exact match, `data-active` on the link's path and the paths
under it, and `data-pending` on the target of an in-flight navigation when the router is created with
`links = pendingLinks`. `useLinkState` gives the same state to code.

Link preloading is off by default. Turn it on with `preloadLinks = [| intentPreload () |]` (hover, focus and touch),
`tapPreload ()`, `viewportPreload ()` or `eagerPreload ()`.

`Router.paths` builds URLs from the route tree: `Router.paths["users"].Invoke(2).href` is `"/users/2"`.

## Hooks

| Hook | Returns |
| --- | --- |
| `useParams ()` | The route's parameters: `ps["id"]` is a `string option` |
| `useNavigate ()` | `navigate.Invoke "/path"`, `navigate.Invoke("/path", NavigateOptions(replace = true))`, `navigate.Invoke -1` |
| `useLocation ()` | The reactive location: `pathname`, `search`, `hash`, `query`, `state` |
| `useSearchParams ()` | The query string, and a setter that merges keys: `setSearch.Invoke {| page = 2 |}` |
| `useIsRouting ()` | `true` while a navigation settles |
| `useMatch (fun () -> "/users/:id")` | The match of a pattern against the location, or `None` |
| `useRouteMatches ()` | The routes matching the location, outermost first |
| `useLinkState (fun () -> "/users")` | `active`, `current` and `pending` for a link |
| `useBeforeLeave listener` | Runs before leaving the route; `e.preventDefault ()` blocks the navigation |
| `usePreloadRoute ()` | Preloads a route by hand |

## Data

`query` caches a fetching function under a key. Read it with Solid's own primitives: the router has no async wrapper of
its own in Solid 2.

```fsharp
let getUser = query ((fun (id: string) -> fetchUser id), "user")

[<SolidComponent>]
let UserName (props: RouteSectionProps) =
    let user = createMemo (fun (_: User option) -> getUser.Invoke(props.``params``["id"].Value))
    Loading (fallback = p () { "Loading" }) { h1 () { user().name } }
```

`getUser.key` and `getUser.keyFor id` name its cache entries for `revalidate`. Pass several values as a tuple or a
record.

`action` is a mutation with a URL. A form posts its `FormData` to it, and `useAction` calls it directly. Settled
submissions are listed by `useSubmissions`. In-flight and optimistic state belongs to Solid's optimistic primitives,
fed by the action's `onSubmit` hook.

```fsharp
let saveTitle =
    action ((fun (id: int) (data: obj) -> save id (unbox<string> (data?get "title"))), "save-title")

[<SolidComponent>]
let Editor () =
    form (action = (saveTitle.with' 3).url, method = Attr.Method.Post) {
        input (name = "title")
        button (type' = Attr.ButtonType.Submit) { "Save" }
    }
```

`with'` binds an action's first argument, in place of a hidden form field. An action that is not a server function
needs a stable name, the second argument of `action`.

## Not bound

- **Deferred until Partas.Solid has a server-functions story:** server rendering helpers (`@solidjs/router/server`)
  and server component routes. They consume `"use server"` functions, which nothing in Partas.Solid produces yet.
- **Deferred until there is a design for F# route files:** file-system routes (`@solidjs/router/fs`).
- **Not bound yet:** `liveQuery`, the `query` cache helpers (`get`, `set`, `clear`), lazy route subtrees and
  `RouterContext`. Until then, bind one yourself with `[<Import>]`.
- **Not needed:** `defineRoute` and `defineRoutes` only add TypeScript types.

The 2.x router bindings, for Solid 1.9, are documented on the [2.x site](https://partas-solid.vercel.app/partas-solid).
