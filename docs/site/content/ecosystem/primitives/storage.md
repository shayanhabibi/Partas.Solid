---
title: Storage
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/storage`.

## makePersisted

```fsharp
let makePersisted<'T>(signal: Signal<'T>, ?options: PersistenceOptions<'T, 'Options>): PersistedState<'T>

type PersistedState<'T> = Accessor<'T> * Setter<'T> * obj
```

`makePersisted` takes a signal, such as the result of `createSignal`, and returns it with its value persisted. It is
bound for signals only. In Partas.Solid 3.0, `Signal<'T>` is still `Accessor<'T> * Setter<'T>`, but `createStore`
returns `Store<'T> * StoreSetter<'T>`, so a store cannot be passed.

`PersistenceOptions` is a POJO:

| Field | Type |
| --- | --- |
| `name` | `string` |
| `serialize` | `'T -> string` |
| `deserialize` | `string -> 'T` |
| `sync` | `PersistenceSyncAPI` |
| `storage` | one of the four storage interfaces below, as a `U4` |
| `storageOptions` | `'Options` |

## Storage interfaces

Each has `getItem`, `setItem` and `removeItem`:

| Interface | `getItem` returns | Options |
| --- | --- | --- |
| `SyncStorage` | `string option` | none |
| `AsyncStorage` | `Promise<string option>` | none |
| `SyncStorageWithOptions<'Options>` | `string option` | `?options: 'Options` on each method |
| `AsyncStorageWithOptions<'Options>` | `Promise<string option>` | `?options: 'Options` on each method |

## Sync

```fsharp
type PersistenceSyncData = { key: string; newValue: string option; timeStamp: float; url: string option }
type PersistenceSyncCallback = PersistenceSyncData -> unit
type PersistenceSyncSubscribe = PersistenceSyncCallback -> unit
type PersistenceSyncUpdate = key: string * value: string option -> unit
type PersistenceSyncAPI = PersistenceSyncSubscribe * PersistenceSyncUpdate

let storageSync: PersistenceSyncAPI
let messageSync(?channel: Window): PersistenceSyncAPI
let multiplexSync([<ParamArray>] syncAPIs: PersistenceSyncAPI[]): PersistenceSyncAPI
```

`messageSync` has no `BroadcastChannel` overload yet, and `wsSync` is not bound.
