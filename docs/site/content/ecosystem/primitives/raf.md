---
title: RAF
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/raf`.

```fsharp
type FrameRequestCallback = float -> unit

let createRAF(callback: FrameRequestCallback): running: Accessor<bool> * start: VoidFunction * stop: VoidFunction
let targetFPS(callback: FrameRequestCallback, fps: float | Accessor<float>): FrameRequestCallback
let createMs(fps: float | Accessor<float>, ?limit: float | Accessor<float>): MsCounter
```

- `createRAF` is a `requestAnimationFrame` loop that is disposed with its owner. It returns a signal of whether it is
  running, and functions that start and stop it.
- `targetFPS` caps a frame callback at `fps` frames per second by skipping frames, so frame durations can be uneven.
- `createMs` counts up milliseconds at a given frame rate, to base animations on.

| `MsCounter` member | Desc |
| --- | --- |
| `current: int` | The current value. Compiles to `msCounter()`; upstream reads it with `counter()`. |
| `reset(): unit` | Resets the counter. |
| `running(): bool` | Whether the counter is running. |
| `start(): unit` | Restarts the counter if it is stopped. |
| `stop(): unit` | Stops the counter if it is running. |
