---
title: Storage
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/storage`.

## SyncStorage

Type: `interface`

- getItem: `key: string -> string option`
- setItem: `key: string * value: string -> unit`
- removeItem: `key: string -> unit`

## AsyncStorage

Type: `interface`

- getItem: `key: string -> Promise<string option>`
- setItem: `key: string * value: string -> Promise<obj>`
- removeItem: `key: string -> Promise<unit>`

## SyncStorageWithOptions

Type: `interface`

Type parameter: `'Options`

- getItem: `key: string * ?options: 'Options -> string option`
- setItem: `key: string * value: string * ?options: 'Options -> unit`
- removeItem: `key: string * ?options: 'Options -> unit`

## AsyncStorageWithOptions

Type: `interface`

Type parameter: `'Options`

- getItem: `key: string * ?options: 'Options -> Promise<string option>`
- setItem: `key: string * value: string * ?options: 'Options -> Promise<obj>`
- removeItem: `key: string * ?options: 'Options -> Promise<unit>`

## PersistenceSyncData

Type: `interface`

- key: `string`
- newValue: `string option`
- timeStamp: `float`
- url: `string option`

## PersistenceSyncCallback

```fsharp
PersistenceSyncData -> unit
```

## PersistenceSyncSubscribe

```fsharp
PersistenceSyncCallback -> unit
```

## PersistenceSyncUpdate

```fsharp
key: string * value: string option -> unit
```

## PersistenceSyncAPI

```fsharp
PersistenceSyncSubscribe * PersistenceSyncUpdate
```

## PersistenceOptions

Type: `POJO`

Type parameters: `'T`, `'Options`

- name: `string`
- serialize: `'T -> string`
- deserialize: `string -> 'T`
- sync: `PersistenceSyncAPI`
- storage: one of the four storages above, as a `U4`
- storageOptions: `'Options`

## PersistedState

Type parameter: `'T`

```fsharp
Accessor<'T> * Setter<'T> * obj
```

`makePersisted` takes a signal, such as the result of `createSignal`, and returns it with its value persisted.

:::note
In Partas.Solid 3.0, `Signal<'T>` is still `Accessor<'T> * Setter<'T>`. Stores are different: `createStore` now comes
from `solid-js` and returns `Store<'T> * StoreSetter<'T>`, where the setter takes an updater. `makePersisted` is bound
for signals only.
:::

## Bindings

:::details Bindings
```fsharp
[<Erase; AutoOpen>]
type Storage =
    [<ImportMember(Spec.path)>]
    static member makePersisted<'T>(signal: Signal<'T>, ?options: PersistenceOptions<'T, _>): PersistedState<'T> = jsNative
    [<ImportMember(Spec.path)>]
    static member storageSync: PersistenceSyncAPI = jsNative
    // TODO broadcast channel overload
    [<ImportMember(Spec.path)>]
    static member messageSync(?channel: Browser.Types.Window): PersistenceSyncAPI = jsNative
    // TODO
    // [<ImportMember(Spec.path)>]
    // static member wsSync(ws: WebSocket, ?warnOnError: bool): PersistenceSyncAPI = jsNative
    [<ImportMember(Spec.path)>]
    static member multiplexSync([<ParamArray>] syncAPIs: PersistenceSyncAPI[]): PersistenceSyncAPI = jsNative
```
:::

`messageSync` has no `BroadcastChannel` overload yet, and `wsSync` is not bound.
