---
title: Devices
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/devices`: reactive lists of media devices, and device sensors.

```fsharp
let createDevices(): Accessor<MediaDeviceInfo[]>      // every media device
let createMicrophones(): Accessor<MediaDeviceInfo[]>
let createSpeakers(): Accessor<MediaDeviceInfo[]>
let createCameras(): Accessor<MediaDeviceInfo[]>
```

## createAccelerometer

```fsharp
let createAccelerometer(?includeGravity: bool, ?interval: float): Accessor<DeviceAcceleration option>
```

The device's acceleration. `includeGravity` defaults to `false`, and `interval` (in ms) to `100`.

## createGyroscope

```fsharp
let createGyroscope(?interval: float): Accessor<GyroscopeValue>

type GyroscopeValue =
    member alpha: float
    member beta: float
    member gamma: float
```

`interval` (in ms) defaults to `100`.
