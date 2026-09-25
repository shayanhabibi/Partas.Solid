---
title: Event Listener
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/event-listener`.

Every method that takes options has two overloads:

- One flattens the option properties into the method signature, and uses `ParamObject` to turn them back into an
  object in JavaScript.
- One takes an object of the options type.

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
