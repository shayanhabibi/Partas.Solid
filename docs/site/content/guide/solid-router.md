---
title: Solid Router
---

:::warning
**Partas.Solid 3.0 has no router bindings yet.** `@solidjs/router` 2 replaced the JSX components the 2.x bindings
wrapped (`Router`, `Route`, `A`, `Navigate`) with `createRouter` and route data. The `Partas.Solid.Router` namespace
is out of the package while it is rewritten for that API, so `open Partas.Solid.Router` does not compile.
:::

Until the bindings return, bind what you use from `@solidjs/router` 2 yourself with `[<Import>]`, as with any other
library (see [Bindings](../ecosystem/bindings.md)), and follow the router's own documentation for the release you
install.

The 2.x router bindings, for Solid 1.9, are documented on the
[2.x site](https://partas-solid.vercel.app/partas-solid).
