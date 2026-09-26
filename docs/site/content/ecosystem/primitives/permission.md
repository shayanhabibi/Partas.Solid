---
title: Permission
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/permission`.

## PermissionName

Type: `StringEnum`

- Geolocation
- Midi
- Notifications
- PersistentStorage
- Push
- ScreenWakeLock
- StorageAccess
- Microphone
- Camera

## PermissionState

Type: `StringEnum`

- Granted
- Denied
- Prompt
- Unknown

## PermissionDescriptor

Type: `interface`

- name: `PermissionName`

## Bindings

```fsharp
[<Erase; AutoOpen>]
type Permission =
    /// <summary>
    /// Queries the permission API
    /// </summary>
    [<ImportMember(Spec.path)>]
    static member createPermission(name: U2<PermissionDescriptor, PermissionName>): unit -> PermissionState = jsNative
```
