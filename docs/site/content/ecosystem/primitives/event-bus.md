---
title: Event Bus
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/event-bus`.

## createEventBus

```fsharp
let createEventBus<'T>(): EventBus<'T>
```

See [`EventBus`](#eventbus).

## createEmitter

```fsharp
let createEmitter<'T>(): Emitter<'T>
```

See [`Emitter`](#emitter).

## createMappedEmitter

```fsharp
let createMappedEmitter<'MessageSchema>(): MappedEmitter<'MessageSchema>
```

See [`MappedEmitter`](#mappedemitter), a binding made for F#.

## createEventHub

```fsharp
let createEventHub<'T> (?channels: 'T): EventHub<'T>
```

See [`EventHub`](#eventhub).

## EventBus

Created by [`createEventBus`](#createeventbus).

```fsharp
type EventBus<'T> = interface
```

A single-event emitter with listener management.

```fsharp
member listen: ('T -> unit) -> DisposeCallback
```

The listener is removed automatically on cleanup. Call the returned callback to remove it earlier.

```fsharp
member emit: 'T -> unit
```

```fsharp
member clear: unit -> unit
```

## Emitter

Created by [`createEmitter`](#createemitter). For a typed version, see [`createMappedEmitter`](#createmappedemitter)
and [`MappedEmitter`](#mappedemitter).

```fsharp
type Emitter<'MessageTyper> = interface
```

An emitter for multiple named events.

```fsharp
member on: (string * (obj -> unit)) -> DisposeCallback
```

The subscription is removed automatically on cleanup. Call the returned callback to remove it earlier.

```fsharp
member emit: (string * obj) -> unit
```

```fsharp
member clear: unit -> unit
```

## MappedEmitter

Created by [`createMappedEmitter`](#createmappedemitter).

```fsharp
type MappedEmitter<'MessageMapper> = interface
```

A type-safe `Emitter`, made for F#. The event key is the path to a member of `'MessageMapper`, so the message has
that member's type.

```fsharp
member on
    (mapping: 'MessageMapper -> 'MessageType)
    (callback: 'MessageType -> unit)
    : DisposeCallback
```

```fsharp
member emit
    (mapping: 'MessageMapper -> 'MessageType)
    (message: 'MessageType)
    : unit
```

```fsharp
member clear: unit -> unit
```

## GlobalEmitter

```fsharp
type GlobalEmitter<'T> = inherit Emitter<'T>
```

An emitter that can also listen to every event.

```fsharp
member listen: (obj -> unit) -> DisposeCallback
```

## EventHub

Created by [`createEventHub`](#createeventhub).

```fsharp
type EventHub<'T> = inherit GlobalEmitter<'T>
```

Helpers for using a group of event buses. Works with `createEventBus`, `createEventStack`, or any emitter with the same
API.

:::caution
Little else is bound for this type.
:::
