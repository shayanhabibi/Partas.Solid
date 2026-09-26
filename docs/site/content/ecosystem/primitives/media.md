---
title: Media
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/media`.

## makeMediaQueryListener

```fsharp
let makeMediaQueryListener
    (query: string)
    (handler: MediaQueryEvent -> unit)
    : DisposeCallback
```

### MediaQueryEvent

```fsharp
type MediaQueryEvent = interface
```

```fsharp
member matches: bool
member media: string
```

## createMediaQuery

```fsharp
let createMediaQuery(
    query: string,
    ?serverFallback: bool
    ): MediaQuery
```

### MediaQuery

```fsharp
type MediaQuery = unit -> bool
```

## createBreakpoints

```fsharp
let createBreakpoints(queryMonitor: 'T): BreakpointMonitor<'T>
```

### BreakpointMonitor

:::caution
This type and its binding need fixing.
:::

```fsharp
type BreakpointMonitor<'T>
```

## sortBreakpoints

```fsharp
let sortBreakpoints(breakpoints: 'T): 'T
```

## createPrefersDark

```fsharp
let createPrefersDark(?fallback: bool): MediaQuery
```

## usePrefersDark

```fsharp
let usePrefersDark(): MediaQuery
```

Uses a rootless primitive that every caller shares.
