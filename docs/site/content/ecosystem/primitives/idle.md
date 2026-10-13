---
title: Idle
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/idle`, which tracks whether the user is idle.

```fsharp
let createIdleTimer(
    ?idleTimeout: int,
    ?promptTimeout: int,
    ?onIdle: Event -> unit,
    ?onPrompt: Event -> unit,
    ?onActive: Event -> unit,
    ?startManually: bool,
    ?events: Event[],
    ?element: HtmlElement
    ): IdleTimer
```

| `IdleTimer` member | Type | Desc |
| --- | --- | --- |
| `isIdle` | `Accessor<bool>` | Whether the user is idle. |
| `isPrompted` | `Accessor<bool>` | Whether the user is in the prompt period. |
| `start` | `unit -> unit` | Starts the timer. |
| `stop` | `unit -> unit` | Stops the timer. |
| `reset` | `unit -> unit` | Resets the timer. |
| `triggerIdle` | `unit -> unit` | Sets `isIdle` to true and calls `onIdle` with a custom `manualidle` event. |
