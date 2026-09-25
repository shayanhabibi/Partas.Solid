---
title: Keyboard
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/keyboard`.

## useKeyDownEvent

```fsharp
let useKeyDownEvent(): Accessor<KeyboardEvent>
```

## useKeyDownList

```fsharp
let useKeyDownList(): Accessor<string[]>
```

## useCurrentlyHeldKey

```fsharp
let useCurrentlyHeldKey(): Accessor<string | null>
```

## useKeyDownSequence

```fsharp
let useKeyDownSequence(): Accessor<string[][]>
```

## createKeyHold

```fsharp
let createKeyHold(
    key: string,
    ?preventDefault: bool
    ): Accessor<bool>
```

## createShortcut

```fsharp
let createShortcut(
    keys: string[],
    handler: unit -> unit,
    ?preventDefault: bool,
    ?requireReset: bool
    ): unit
```

The keys must be pressed in the order given.
