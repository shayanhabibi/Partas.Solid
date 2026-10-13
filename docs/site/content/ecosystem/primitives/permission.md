---
title: Permission
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/permission`.

```fsharp
let createPermission(name: U2<PermissionDescriptor, PermissionName>): unit -> PermissionState
```

Queries the Permissions API and returns an accessor of the state.

| Type | Kind | Values |
| --- | --- | --- |
| `PermissionName` | `StringEnum` | `Geolocation`, `Midi`, `Notifications`, `PersistentStorage`, `Push`, `ScreenWakeLock`, `StorageAccess`, `Microphone`, `Camera` |
| `PermissionState` | `StringEnum` | `Granted`, `Denied`, `Prompt`, `Unknown` |
| `PermissionDescriptor` | interface | `name: PermissionName` |
