---
title: Media
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/media`.

```fsharp
let makeMediaQueryListener (query: string) (handler: MediaQueryEvent -> unit): DisposeCallback
let createMediaQuery(query: string, ?serverFallback: bool): MediaQuery
let createBreakpoints(queryMonitor: 'T): BreakpointMonitor<'T>
let sortBreakpoints(breakpoints: 'T): 'T
let createPrefersDark(?fallback: bool): MediaQuery
let usePrefersDark(): MediaQuery          // rootless: shared by every caller

type MediaQuery = unit -> bool
type MediaQueryEvent =
    member matches: bool
    member media: string
```

:::caution
`BreakpointMonitor<'T>` and the `createBreakpoints` binding need fixing.
:::
