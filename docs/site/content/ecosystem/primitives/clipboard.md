---
title: Clipboard
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/clipboard`, which wraps the MDN `clipboard` API.

## ClipboardItem

```fsharp
type ClipboardItem = interface
```

An interface to the MDN `clipboard` API's `ClipboardItem`. To create one, see [`newClipboardItem`](#newclipboarditem).

```fsharp
member presentationStyle: PresentationStyle with get
```

See [`PresentationStyle`](#presentationstyle).

```fsharp
member types: string[] with get
```

```fsharp
member getType(``type``: string): JS.Promise<obj>
```

Resolves to the item's data for one MIME type; rejects if the item has none.

### PresentationStyle

```fsharp
type PresentationStyle = StringEnum
```

```fsharp
| Attachment
| Inline
| Unspecified
```

## newClipboardItem

```fsharp
let newClipboardItem(``type``: string, data: obj): ClipboardItem
```

Creates a [`ClipboardItem`](#clipboarditem) from a MIME type and a data object.

## readClipboard

```fsharp
let readClipboard(): JS.Promise<ClipboardItem[]>
```

Reads the clipboard. Returns a promise that resolves to an array of [`ClipboardItem`](#clipboarditem).

## writeClipboard

```fsharp
let writeClipboard(input: string): unit
```

```fsharp
let writeClipboard(input: ClipboardItem[]): unit
```

Writes to the clipboard. To build a `ClipboardItem`, see [`newClipboardItem`](#newclipboarditem).

:::note
Writing is asynchronous. Use the version with an apostrophe when you need the returned `Promise`.
:::

```fsharp
let writeClipboard'(input: string): JS.Promise<unit>
```

```fsharp
let writeClipboard'(input: ClipboardItem[]): JS.Promise<unit>
```

## createClipboard

```fsharp
let createClipboard(
    ?data: Accessor<string | ClipboardItem[]>,
    ?deferInitial: bool
    ): ClipboardResult
```

:::details Version with an apostrophe
```fsharp
let createClipboard'(
    ?data: Accessor<string | ClipboardItem[]>,
    ?deferInitial: bool
    ):
    SolidResource<ClipboardItem[]> *
    (unit -> unit) *
    (string -> JS.Promise<unit>)
```
:::

`createClipboard` reads and writes. Write with the `write` member or through the input signal; reads come back as a
Solid resource. Use the named members of [`ClipboardResult`](#clipboardresult), or destructure the apostrophe
version's tuple.

:::caution
`SolidResource` and `createResource` were removed in Partas.Solid 3.0, because Solid 2.0 removed resources, so this
binding cannot be ported as it stands. In Solid 2.0, async data comes from a `createMemo` whose compute function
returns a `JS.Promise`, read under a `Loading` boundary. See the
[migration guide](../../guide/migrating-to-solid-2.md#createresource).
:::

### ClipboardResult

```fsharp
type ClipboardResult = interface
```

Named access to what [`createClipboard`](#createclipboard) returns.

```fsharp
member resourceItems: SolidResource<ClipboardItem[]>
```

```fsharp
member refetch: (unit -> unit)
```

```fsharp
member write: (string -> JS.Promise<unit>)
```
