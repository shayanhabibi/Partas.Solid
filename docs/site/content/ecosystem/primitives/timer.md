---
title: Timer
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/timer`.

```fsharp
type IntervalOrTimeout = (unit -> unit) -> int -> int     // setInterval or setTimeout

let makeTimer(callback: unit -> unit, timespan: int, policy: IntervalOrTimeout): DisposeCallback
let createTimer(callback: unit -> unit, timespan: int | Accessor<U2<bool, int>> | Accessor<bool> | Accessor<int>, policy: IntervalOrTimeout): unit
let createTimeoutLoop(callback: unit -> unit, timespan: int | Accessor<int>): unit
let createPolled(callback: unit -> 'T, timespan: int | Accessor<int>): Accessor<'T>
let createIntervalCounter(timespan: int | Accessor<int>): Accessor<int>
```

| Member | Desc |
| --- | --- |
| `makeTimer` | A timer that is cleaned up with its owner. |
| `createTimer` | `makeTimer` with a reactive delay. Set the delay to `false` to turn the timer off. |
| `createTimeoutLoop` | Like an interval from `createTimer`, but a new delay only takes effect after the callback runs. |
| `createPolled` | Calls `callback` periodically and returns an accessor of its latest result. |
| `createIntervalCounter` | A counter that goes up by one on each interval. |
