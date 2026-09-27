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

:::details Example
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

A cache of triggers keyed by `'T`, so you can mark specific keys dirty. A key's trigger exists only while a
computation tracks it. The binding does not expose upstream's `mapConstructor` argument, so the cache is always a
`Map`, never a `WeakMap`.

```fsharp
[<Extension>]
static member track (triggerCache: TriggerCacheSignal<'T>, key: 'T): unit
[<Extension>]
static member dirty (triggerCache: TriggerCacheSignal<'T>, key: 'T): unit
```

:::details Example
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
result. See the [migration guide](../../guide/migrating-to-solid-2.md#effects-have-two-phases).
:::
