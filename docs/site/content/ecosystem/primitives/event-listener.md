---
title: Event Listener
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/event-listener`.

```fsharp
let makeEventListener(
    target: #Element,
    ``type``: string,
    handler: Event -> unit,
    ?capture: bool,
    ?once: bool,
    ?passive: bool
    ): DisposeCallback

let createEventListener(
    target: #Element | #Element[] | Accessor<#Element> | Accessor<#Element[]>,
    ``type``: U4<string, string[], Accessor<string>, Accessor<string[]>>,
    handler: Event -> unit,
    ?capture: bool,
    ?once: bool,
    ?passive: bool
    ): unit
```

Each also has an overload that takes an `AddEventListenerOptions` object in place of the three flags.

`createEventSignal` is declared as `obj[] -> obj` but not implemented.
