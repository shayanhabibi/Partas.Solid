---
title: Input Mask
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/input-mask`. The package offers several kinds of input mask.

This page lists the binding's API and types. For usage, see the `@solid-primitives` documentation.

## InputMask

Type: erased DU

```fsharp
type InputMask
```

### `Fn of InputMaskFn`

```fsharp
type InputMaskFn = delegate of value: string * selection: Selection -> value: string
```

```fsharp
type Selection = Selection of start: float * finish: float
```

### `Array of InputMaskArray`

```fsharp
type InputMaskArray = InputMaskArray of U2<string, Regex>[]
```

### `Regex of InputMaskRegex`

```fsharp
type InputMaskRegex = InputMaskRegex of regex: Regex * replacer: (obj -> string)
```

### `String of string`

## Bindings

The `createKeyboard*`, `createInput*` and `createClipboard*` members are inline helpers that fix the event type of
`createInputMask` and `createMaskPattern`.

:::details title="Bindings"
```fsharp
[<AutoOpen; Erase>]
type InputMask =
    [<ImportMember(Spec.path)>]
    static member stringMaskToArray(mask: string, ?regexps: Map<string, Regex>): InputMask.InputMaskArray = jsNative
    [<ImportMember(Spec.path)>]
    static member regexMaskToFn(regex: Regex, replacer: obj): InputMask.InputMaskFn = jsNative
    [<ImportMember(Spec.path)>]
    static member maskArrayToFn(maskArray: InputMask.InputMaskArray): InputMask.InputMaskFn = jsNative
    [<ImportMember(Spec.path)>]
    static member anyMaskToFn(mask: InputMask.InputMask, ?regexps: Map<string, Regex>): InputMask.InputMaskFn = jsNative
    [<ImportMember(Spec.path)>]
    static member createInputMask<'MaskEvent>(mask: InputMask.InputMask, ?regexps: Map<string, Regex>): 'MaskEvent -> string = jsNative
    static member inline createKeyboardInputMask(mask: InputMask.InputMask, ?regexps: Map<string, Regex>) =
        InputMask.createInputMask<KeyboardEvent>(mask, ?regexps = regexps)
    static member inline createInputInputMask(mask: InputMask.InputMask, ?regexps: Map<string, Regex>) =
        InputMask.createInputMask<InputEvent>(mask, ?regexps = regexps)
    static member inline createClipboardInputMask(mask: InputMask.InputMask, ?regexps: Map<string, Regex>) =
        InputMask.createInputMask<ClipboardEvent>(mask, ?regexps = regexps)
    [<ImportMember(Spec.path)>]
    static member createMaskPattern<'MaskEvent>(inputMask: 'MaskEvent -> string, pattern: string -> string): 'MaskEvent -> string = jsNative
    static member inline createKeyboardMaskPattern(inputMask: KeyboardEvent -> string, pattern: string -> string) =
        InputMask.createMaskPattern<KeyboardEvent>(inputMask,pattern)
    static member inline createInputMaskPattern(inputMask: InputEvent -> string, pattern: string -> string) =
        InputMask.createMaskPattern<InputEvent>(inputMask,pattern)
    static member inline createClipboardMaskPattern(inputMask: ClipboardEvent -> string, pattern: string -> string) =
        InputMask.createMaskPattern<ClipboardEvent>(inputMask,pattern)
```
:::
