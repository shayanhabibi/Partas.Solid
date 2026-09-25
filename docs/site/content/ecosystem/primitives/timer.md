---
title: Timer
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/timer`.

## IntervalOrTimeout

```fsharp
type IntervalOrTimeout = (unit -> unit) -> int -> int
```

## makeTimer

```fsharp
let makeTimer(
    callback: unit -> unit,
    timespan: int,
    policy: IntervalOrTimeout
    ): DisposeCallback
```

Makes a timer that is cleaned up automatically. It takes a callback, the timespan, and either `setInterval` or
`setTimeout` as the base.

## createTimer

```fsharp
let createTimer(
    callback: unit -> unit,
    timespan: int | Accessor<U2<bool, int>> | Accessor<bool> | Accessor<int>,
    policy: IntervalOrTimeout
    ): unit
```

`makeTimer` with a fully reactive delay. Set the delay to `false` to turn the timer off.

## createTimeoutLoop

```fsharp
let createTimeoutLoop(
    callback: unit -> unit,
    timespan: int | Accessor<int>
    ): unit
```

Like an interval made with `createTimer`, but a new delay only takes effect after the callback runs.

## createPolled

```fsharp
let createPolled(
    callback: unit -> 'T,
    timespan: int | Accessor<int>
    ): Accessor<'T>
```

Calls a function periodically, and returns an accessor of its latest return value.

## createIntervalCounter

```fsharp
let createIntervalCounter(timespan: int | Accessor<int>): Accessor<int>
```

A counter that goes up by one on each interval.
