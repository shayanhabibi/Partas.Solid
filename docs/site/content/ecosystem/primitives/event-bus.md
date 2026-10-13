---
title: Event Bus
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/event-bus`.

```fsharp
let createEventBus<'T>(): EventBus<'T>
let createEmitter<'T>(): Emitter<'T>
let createMappedEmitter<'MessageSchema>(): MappedEmitter<'MessageSchema>
let createEventHub<'T>(?channels: 'T): EventHub<'T>
```

Every subscription is removed automatically when its owner is cleaned up. Call the returned `DisposeCallback` to remove
it earlier.

## EventBus

A single-event emitter.

```fsharp
type EventBus<'T> =
    member listen: ('T -> unit) -> DisposeCallback
    member emit: 'T -> unit
    member clear: unit -> unit
```

## Emitter

An emitter for multiple named events, with untyped messages.

```fsharp
type Emitter<'MessageTyper> =
    member on: (string * (obj -> unit)) -> DisposeCallback
    member emit: (string * obj) -> unit
    member clear: unit -> unit
```

## MappedEmitter

A typed `Emitter`, made for F#. The event key is a member of `'MessageMapper`, so the message has that member's type.

```fsharp
type MappedEmitter<'MessageMapper> =
    member on: (mapping: 'MessageMapper -> 'MessageType) -> (callback: 'MessageType -> unit) -> DisposeCallback
    member emit: (mapping: 'MessageMapper -> 'MessageType) -> (message: 'MessageType) -> unit
    member clear: unit -> unit
```

## GlobalEmitter and EventHub

`GlobalEmitter<'T>` is an `Emitter` that can also listen to every event, through
`listen: (obj -> unit) -> DisposeCallback`.

`EventHub<'T>` inherits `GlobalEmitter<'T>` and groups several buses: `createEventBus`, `createEventStack`, or any
emitter with the same API. Little else is bound for it.
