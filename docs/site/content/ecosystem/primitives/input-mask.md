---
title: Input Mask
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/input-mask`.

## InputMask

An erased union of the four ways to describe a mask:

```fsharp
type InputMask =
    | Fn of InputMaskFn          // delegate of value: string * selection: Selection -> string
    | Array of InputMaskArray    // InputMaskArray of U2<string, Regex>[]
    | Regex of InputMaskRegex    // InputMaskRegex of regex: Regex * replacer: (obj -> string)
    | String of string

type Selection = Selection of start: float * finish: float
```

## Bindings

```fsharp
let stringMaskToArray(mask: string, ?regexps: Map<string, Regex>): InputMaskArray
let regexMaskToFn(regex: Regex, replacer: obj): InputMaskFn
let maskArrayToFn(maskArray: InputMaskArray): InputMaskFn
let anyMaskToFn(mask: InputMask, ?regexps: Map<string, Regex>): InputMaskFn
let createInputMask<'MaskEvent>(mask: InputMask, ?regexps: Map<string, Regex>): 'MaskEvent -> string
let createMaskPattern<'MaskEvent>(inputMask: 'MaskEvent -> string, pattern: string -> string): 'MaskEvent -> string
```

`createKeyboardInputMask`, `createInputInputMask` and `createClipboardInputMask` are inline helpers that fix
`'MaskEvent` of `createInputMask` to `KeyboardEvent`, `InputEvent` or `ClipboardEvent`. `createKeyboardMaskPattern`,
`createInputMaskPattern` and `createClipboardMaskPattern` do the same for `createMaskPattern`.
