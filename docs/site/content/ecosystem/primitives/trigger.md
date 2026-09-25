---
title: Trigger
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/trigger`.

Alias types:

```fsharp
type [<Erase>] Track<'T> = 'T -> unit
type [<Erase>] Dirty<'T> = 'T -> unit
type [<Erase>] DirtyAll = unit -> unit
type [<Erase>] TriggerSignal<'T> = Track<'T> * Dirty<'T>
type [<Erase>] TriggerCacheSignal<'T> = Track<'T> * Dirty<'T> * DirtyAll
```

## createTrigger

```fsharp
let createTrigger(): TriggerSignal<unit>
```

Track the trigger in reactive computations, then fire it when you want.

:::details title="Example"
```fsharp
let track, dirty = createTrigger()

createEffect (
    (fun _ -> track ()),                     // the trigger is now a dependency
    (fun () -> JS.console.log "Triggered!")
)
// ...
dirty () // "Triggered!"
```
:::

## createTriggerCache

```fsharp
let createTriggerCache<'T>(): TriggerCacheSignal<'T>
```

Creates a cache of triggers, so you can mark only specific keys as dirty.

The cache is a `Map` or a `WeakMap`, depending on the upstream `mapConstructor` argument. It defaults to `Map`. With
`WeakMap` the cache is weak, and keys are garbage collected once nothing references them. The binding above takes no
arguments, so you always get a `Map`.

Triggers are added to the cache only when a computation tracks them, and are removed when nothing tracks them any
more.

```fsharp
[<Extension>]
static member track (triggerCache: TriggerCacheSignal<'T>, key: 'T): unit
[<Extension>]
static member dirty (triggerCache: TriggerCacheSignal<'T>, key: 'T): unit
```

:::details title="Example"
```fsharp
let map = createTriggerCache<int>()

createEffect (
    (fun _ -> map.track 1),                  // adds key 1 to the dependencies
    (fun () -> JS.console.log "Triggered!")
)
// ...
map.dirty 1 // "Triggered!"
```
:::

:::note
Partas.Solid 3.0 removed the single-callback `createEffect`. The examples use the two-phase form,
`createEffect(compute, effectFn)`: the compute function tracks, and the effect function runs the side effect with its
result. See the [migration guide](../../guide/migrating-to-solid-2.md).
:::
