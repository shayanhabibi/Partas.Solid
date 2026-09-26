---
title: Solid Primitives
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

These are bindings for the community packages published under `@solid-primitives`.

Each page lists the signatures of the bound functions and types. For usage, read the upstream
`@solid-primitives` documentation for the package: the bindings follow it closely.

`@solid-primitives` itself depends on Solid 1.x, so none of these packages work with Partas.Solid 3.0 on Solid 2
until they are ported upstream and here. There are no live examples on these pages for that reason.

## Packages

| Page | Package |
| --- | --- |
| [Active Element](active-element.md) | `@solid-primitives/active-element` |
| [Autofocus](autofocus.md) | `@solid-primitives/autofocus` |
| [Bounds](bounds.md) | `@solid-primitives/bounds` |
| [Broadcast Channel](broadcast-channel.md) | `@solid-primitives/broadcast-channel` |
| [Clipboard](clipboard.md) | `@solid-primitives/clipboard` |
| [Devices](devices.md) | `@solid-primitives/devices` |
| [Event Bus](event-bus.md) | `@solid-primitives/event-bus` |
| [Event Listener](event-listener.md) | `@solid-primitives/event-listener` |
| [Idle](idle.md) | `@solid-primitives/idle` |
| [Input Mask](input-mask.md) | `@solid-primitives/input-mask` |
| [JSX Tokenizer](jsx-tokenizer.md) | `@solid-primitives/jsx-tokenizer` |
| [Keyboard](keyboard.md) | `@solid-primitives/keyboard` |
| [Media](media.md) | `@solid-primitives/media` |
| [Mouse](mouse.md) | `@solid-primitives/mouse` |
| [Permission](permission.md) | `@solid-primitives/permission` |
| [RAF](raf.md) | `@solid-primitives/raf` |
| [Rootless](rootless.md) | `@solid-primitives/rootless` |
| [Scheduled](scheduled.md) | `@solid-primitives/scheduled` |
| [Scroll](scroll.md) | `@solid-primitives/scroll` |
| [Spring](spring.md) | `@solid-primitives/spring` |
| [Storage](storage.md) | `@solid-primitives/storage` |
| [Timer](timer.md) | `@solid-primitives/timer` |
| [Trigger](trigger.md) | `@solid-primitives/trigger` |
| [Tween](tween.md) | `@solid-primitives/tween` |
| [WebSocket](websocket.md) | `@solid-primitives/websocket` (not bound) |

## Package organisation

Each primitive is published as its own NuGet package, such as `Partas.Solid.Primitives.Mouse`. Whichever packages you
install, you reach all of them through the namespace `Partas.Solid.Primitives`.

:::note
The package `Partas.Solid.Primitives` installs the latest version of every primitive. Do not depend on it when you
publish a library.
:::

All packages share a dependency on `Partas.Solid.Primitives.Common`.

Before `0.2.0`, each primitive had its own namespace, and they all depended on a shared package called
`Partas.Solid.Primitives`.

## Femto

You can install every primitive with Femto:

```bash
femto install Partas.Solid.Primitives.Mouse
```

## Use, make and create

The bindings keep the `@solid-primitives` naming scheme:

- `make` primitives are not reactive, but they clean up their resources when their owner is disposed.
- `use` primitives are usually *rootless*: there is one instance of the primitive, shared by every consumer.
- `create` primitives are reactive, and clean up their resources when their owner is disposed.

## Disposal callbacks

Some primitives return a `DisposeCallback`, a `unit -> unit` function that cleans up the resource. Call it when you
need to release the resource before its owner is disposed.

`DisposeCallback` comes from `Partas.Solid.Primitives.Common`. Core Partas.Solid 3.0 has its own `DisposalFunc` type,
which is what `createRoot` passes to its callback.
