---
title: Solid Primitives
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9. `@solid-primitives` depends on Solid 1.x, so none of these
packages work with Partas.Solid 3.0 until they are ported upstream and here.
:::

Bindings for the community packages published under `@solid-primitives`. Each page lists the bound signatures; for
usage, read the upstream documentation, which the bindings follow closely. The signatures are the 2.x packages', so
their DOM types (`HtmlElement`, `Event`) come from Fable.Browser.Dom; a Solid 2 port will use Xantham's `Dom.X` types.

Some pages show the primitive's behaviour live, written in plain Partas.Solid 3.0. That code works today, so you can
use it until the package is ported.

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

## Packages and namespaces

Each primitive is its own NuGet package, such as `Partas.Solid.Primitives.Mouse`. All of them share the namespace
`Partas.Solid.Primitives` and depend on `Partas.Solid.Primitives.Common`. Femto installs a package together with its
npm dependency:

```bash
femto install Partas.Solid.Primitives.Mouse
```

:::note
The package `Partas.Solid.Primitives` installs the latest version of every primitive. Do not depend on it when you
publish a library.
:::

## Use, make and create

The bindings keep the `@solid-primitives` naming scheme:

- `make` primitives are not reactive, but they clean up their resources when their owner is disposed.
- `use` primitives are usually *rootless*: one instance, shared by every consumer.
- `create` primitives are reactive, and clean up their resources when their owner is disposed.

Some primitives return a `DisposeCallback`, a `unit -> unit` from `Partas.Solid.Primitives.Common`. Call it to release
the resource before its owner is disposed. (Core Partas.Solid 3.0 calls the same shape `DisposalFunc`.)
