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

- `makeBroadcastChannel` opens a channel for cross-tab communication and returns `onMessage`, `postMessage`, `close`,
  `channelName` and `instance`.
- `createBroadcastChannel` does the same, but returns a `message` signal instead of `onMessage`. The signal updates
  when another context calls `postMessage`.

If a channel with the same name already exists, you get it back instead of a new one. The channel tries to close
when its owner is cleaned up. If several instances are connected, it stays open until the last owner goes.

```fsharp
[<AllowNullLiteral; Interface>]
type MessageEvent<'T> =
    inherit MessageEvent
    abstract member data: 'T with get

[<AllowNullLiteral; Interface>]
type BroadcastChannelResult = interface end

[<AllowNullLiteral; Interface>]
type MakeBroadcastChannelResult<'T> =
    inherit BroadcastChannelResult
    /// A function to subscribe to messages from other tabs on the same channel
    abstract member onMessage: event: MessageEvent<'T> -> unit with get
    /// A function to send messages to other tabs
    abstract member postMessage: 'T -> unit with get
    /// A function to close the channel
    abstract member close: unit -> unit with get
    /// The name of the channel
    abstract member channelName: string with get
    /// The underlying BroadcastChannel instance
    abstract member instance: BroadcastChannel<'T> with get

[<AllowNullLiteral; Interface>]
type CreateBroadcastChannelResult<'T> =
    inherit BroadcastChannelResult
    /// An accessor that updates when postMessage is fired from other contexts
    abstract member message: Accessor<'T> -> unit with get
    /// A function to send messages to other tabs
    abstract member postMessage: 'T -> unit with get
    /// A function to close the channel
    abstract member close: unit -> unit with get
    /// The name of the channel
    abstract member channelName: string with get
    /// The underlying BroadcastChannel instance
    abstract member instance: BroadcastChannel<'T> with get

[<Erase>]
type BroadcastChannel<'T> =
    /// <summary>
    /// Creates a new BroadcastChannel instance for cross-tab communication.
    /// </summary>
    /// <param name="name">Channel name to listen/broadcast on</param>
    /// <returns>onMessage, postMessage, close, channelName, instance</returns>
    [<ImportMember("@solid-primitives/broadcast-channel")>]
    static member makeBroadcastChannel<'T> (name: string): MakeBroadcastChannelResult<'T> = jsNative
    /// <summary>
    /// Provides the same functionality as <c>makeBroadcastChannel</c> but instead of returning <c>onMessage</c>, it
    /// returns a <c>message</c> signal accessor that updates when postMessage is fired from other contexts.
    /// </summary>
    /// <param name="name">Channel name to listen/broadcast on</param>
    /// <returns>message, postMessage, close, channelName, instance</returns>
    [<ImportMember("@solid-primitives/broadcast-channel")>]
    static member createBroadcastChannel<'T> (name: string): CreateBroadcastChannelResult<'T> = jsNative
```
