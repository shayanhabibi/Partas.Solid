---
title: Scheduled
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/scheduled`.

## Schedule

```fsharp
type Schedule<'T> = interface
```

```fsharp
member exec: 'T -> unit
```

Stands in for calling `schedule(_)` in JavaScript.

```fsharp
member clear: unit -> unit
```

## debounce

```fsharp
let debounce(
    callback: 'T -> unit,
    timespan: int
    ): Schedule<'T>
```

## throttle

```fsharp
let throttle(
    callback: 'T -> unit,
    timespan: int
    ): Schedule<'T>
```

## scheduleIdle

```fsharp
let scheduleIdle(
    callback: 'T -> unit,
    timespan: int
    ): Schedule<'T>
```

## DebounceOrThrottle

```fsharp
type DebounceOrThrottle<'T> = ('T -> unit) * int -> Schedule<'T>
```

## leading

```fsharp
let leading(
    debOrThrot: DebounceOrThrottle<'T>,
    callback: 'T -> unit,
    timespan: unit
    ): Schedule<'T>
```

:::caution
`timespan` is typed `unit` here, where it should be `int` as in `leadingAndTrailing`.
:::

## leadingAndTrailing

```fsharp
let leadingAndTrailing(
    debOrThrot: DebounceOrThrottle<'T>,
    callback: 'T -> unit,
    timespan: int
    ): Schedule<'T>
```

## createScheduled

Not bound.
