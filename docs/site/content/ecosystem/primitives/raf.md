---
title: RAF
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/raf`.

## FrameRequestCallback

```fsharp
type FrameRequestCallback = float -> unit
```

## createRAF

```fsharp
let createRAF(
    callback: FrameRequestCallback
    ): running: Accessor<bool> * start: VoidFunction * stop: VoidFunction
```

| Param | Desc |
| --- | --- |
| `callback` | The callback to run on each frame. |

| Returns | Desc |
| --- | --- |
| `running` | Signal of whether it is running. |
| `start` | Starts the loop. |
| `stop` | Stops the loop. |

A reactive `window.requestAnimationFrame` loop that is disposed automatically when its owner is cleaned up.

## targetFPS

```fsharp
let targetFPS(
    callback: FrameRequestCallback,
    fps: float | Accessor<float>
    ): FrameRequestCallback
```

Wraps a `window.requestAnimationFrame` callback so it runs at most the given number of frames per second.

It limits the frame rate by skipping the callback on frames above the limit, so frame durations can be uneven.

## createMs

```fsharp
let createMs(
    fps: float | Accessor<float>,
    ?limit: float | Accessor<float>
    ): MsCounter
```

A signal that counts up milliseconds at a given frame rate, to base animations on.

:::note
Unlike upstream, the binding reads the counter's current value with [`counter.current`](#mscounter), not
`counter()`.
:::

### MsCounter

```fsharp
type MsCounter = interface
```

| Member | Desc |
| --- | --- |
| `current: int` | The current value. Compiles to `msCounter()`. |
| `reset(): unit` | Resets the counter. |
| `running(): bool` | Whether the counter is running. |
| `start(): unit` | Restarts the counter if it is stopped. |
| `stop(): unit` | Stops the counter if it is running. |
