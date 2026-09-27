---
title: Event Listener
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/event-listener`.

Every method that takes options has two overloads: one takes an options object, the other flattens the options into
optional parameters (`ParamObject` rebuilds the object in JavaScript).

## makeEventListener

```fsharp
let makeEventListener(
    target: #Element,
    ``type``: string,
    handler: Event -> unit,
    options: AddEventListenerOptions
    ): DisposeCallback
```

```fsharp
let makeEventListener(
    target: #Element,
    ``type``: string,
    handler: Event -> unit,
    ?capture: bool,
    ?once: bool,
    ?passive: bool
    ): DisposeCallback
```

## createEventListener

```fsharp
let createEventListener(
    target: #Element | #Element[] | Accessor<#Element> | Accessor<#Element[]>,
    ``type``: U4<string, string[], Accessor<string>, Accessor<string[]>>,
    handler: Event -> unit,
    options: AddEventListenerOptions
    ): unit
```

```fsharp
let createEventListener(
    target: #Element | #Element[] | Accessor<#Element> | Accessor<#Element[]>,
    ``type``: U4<string, string[], Accessor<string>, Accessor<string[]>>,
    handler: Event -> unit,
    ?capture: bool,
    ?once: bool,
    ?passive: bool
    ): unit
```

## createEventSignal

:::caution
Not implemented.
:::

```fsharp
let createEventSignal([<ParamArray>] args: obj[]): obj
```
