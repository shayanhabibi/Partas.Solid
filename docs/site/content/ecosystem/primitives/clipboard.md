---
title: Clipboard
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/clipboard`, which wraps the browser's clipboard API.

## ClipboardItem

```fsharp
type ClipboardItem =
    member presentationStyle: PresentationStyle // Attachment | Inline | Unspecified
    member types: string[]
    member getType(``type``: string): JS.Promise<obj>

let newClipboardItem(``type``: string, data: obj): ClipboardItem
```

`getType` resolves to the item's data for one MIME type, and rejects if the item has none.

## readClipboard and writeClipboard

```fsharp
let readClipboard(): JS.Promise<ClipboardItem[]>
let writeClipboard(input: string | ClipboardItem[]): unit
let writeClipboard'(input: string | ClipboardItem[]): JS.Promise<unit>
```

Writing is asynchronous. Use the version with an apostrophe when you need the returned `Promise`.

## createClipboard

:::caution
`createClipboard` returns a Solid resource. Solid 2.0 removed resources, and Partas.Solid 3.0 removed `SolidResource`
and `createResource` with them, so this binding cannot be ported as it stands. In Solid 2.0, async data comes from a
`createMemo` whose compute function returns a `JS.Promise`, read under a `Loading` boundary. See the
[migration guide](../../guide/migrating-to-solid-2.md#createresource).
:::

```fsharp
let createClipboard(?data: Accessor<string | ClipboardItem[]>, ?deferInitial: bool): ClipboardResult

type ClipboardResult =
    member resourceItems: SolidResource<ClipboardItem[]>
    member refetch: unit -> unit
    member write: string -> JS.Promise<unit>
```

Write with `write` or through the `data` signal; reads come back as the resource. `createClipboard'` returns the same
three values as a tuple.
