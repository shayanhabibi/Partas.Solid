---
title: Broadcast Channel
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/broadcast-channel`.

:::caution
Nobody has used this binding yet, and it needs review. `CreateBroadcastChannelResult.message`, for example, is typed
`Accessor<'T> -> unit` where upstream returns a plain accessor.
:::

```fsharp
let makeBroadcastChannel<'T>(name: string): MakeBroadcastChannelResult<'T>
let createBroadcastChannel<'T>(name: string): CreateBroadcastChannelResult<'T>
```

- `makeBroadcastChannel` opens a channel for cross-tab communication. Its result has `onMessage`, `postMessage`,
  `close`, `channelName` and `instance` (the underlying `BroadcastChannel`).
- `createBroadcastChannel` returns a `message` signal in place of `onMessage`. The signal updates when another context
  calls `postMessage`.

Channels are shared by name: asking for an existing name returns that channel, and it closes when its last owner is
cleaned up.
