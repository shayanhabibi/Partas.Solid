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

Provides the base functions of an event emitter, plus functions for managing listeners. A config object can customise
its behaviour, which is useful for advanced cases.

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

An emitter you can listen to and emit different events on.

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

A type-safe version of `Emitter`, made for F# and Fable.

It uses the path from the type to one of its members as the key of the event. Because the path is typed, the message
has the type of that member.

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

A wrapper around `createEmitter`. It is an emitter that can also listen to every event.

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
