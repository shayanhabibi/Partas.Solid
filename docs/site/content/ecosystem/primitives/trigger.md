---
title: Trigger
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/trigger`.

```fsharp
type Track<'T> = 'T -> unit
type Dirty<'T> = 'T -> unit
type DirtyAll = unit -> unit
type TriggerSignal<'T> = Track<'T> * Dirty<'T>
type TriggerCacheSignal<'T> = Track<'T> * Dirty<'T> * DirtyAll

let createTrigger(): TriggerSignal<unit>
let createTriggerCache<'T>(): TriggerCacheSignal<'T>
```

## createTrigger

Track the trigger in a computation, then fire it when you want:

```fsharp
let track, dirty = createTrigger()

createEffect (
    (fun _ -> track ()),                     // the trigger is now a dependency
    (fun () -> JS.console.log "Triggered!")
)
dirty () // "Triggered!"
```

The examples use Solid 2's two-phase `createEffect(compute, effectFn)`: the compute function tracks, and the effect
function runs the side effect. See the [migration guide](../../guide/migrating-to-solid-2.md#effects-have-two-phases).

## createTriggerCache

A cache of triggers keyed by `'T`, so you can mark single keys dirty. A key's trigger exists only while a computation
tracks it. Upstream's `mapConstructor` argument is not bound, so the cache is always a `Map`, never a `WeakMap`. The
binding adds `track` and `dirty` extension methods that take the key:

```fsharp
let map = createTriggerCache<int>()

createEffect (
    (fun _ -> map.track 1),                  // adds key 1 to the dependencies
    (fun () -> JS.console.log "Triggered!")
)
map.dirty 1 // "Triggered!"
```
