---
title: Devices
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/devices`: reactive lists of media devices, and device sensors.

## createDevices

```fsharp
let createDevices(): Accessor<MediaDeviceInfo[]>
```

Lists all media devices.

## createMicrophones

```fsharp
let createMicrophones(): Accessor<MediaDeviceInfo[]>
```

Lists the media devices that are microphones.

## createSpeakers

```fsharp
let createSpeakers(): Accessor<MediaDeviceInfo[]>
```

Lists the media devices that are speakers.

## createCameras

```fsharp
let createCameras(): Accessor<MediaDeviceInfo[]>
```

Lists the media devices that are cameras.

## createAccelerometer

```fsharp
let createAccelerometer(
    ?includeGravity: bool,
    ?interval: float
    ): Accessor<DeviceAcceleration option>
```

| Param | Desc |
| --- | --- |
| `?includeGravity` | `bool`. Defaults to `false`. |
| `?interval` | Number of ms. Defaults to `100`. |

The device's acceleration.

## createGyroscope

```fsharp
let createGyroscope(
    ?interval: float
    ): Accessor<GyroscopeValue>
```

| Param | Desc |
| --- | --- |
| `?interval` | Number of ms. Defaults to `100`. |

### GyroscopeValue

```fsharp
type GyroscopeValue = interface
```

```fsharp
member alpha: float
member beta: float
member gamma: float
```
