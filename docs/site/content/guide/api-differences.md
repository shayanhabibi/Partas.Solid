---
title: API differences
---

Where the F# API differs noticeably from Solid's JavaScript API. For Solid itself, see the
[Solid documentation](https://docs.solidjs.com/).

## Option objects

Where a Solid function takes an options object, the binding usually exposes the options as optional parameters, and
Fable's `ParamObject` attribute packs them back into an object at the call site:

```fsharp
[<ImportMember("solid-js"); ParamObject(1)>]
static member createMemo<'T>(compute: 'T option -> 'T, ?name: string, ?equals: EqualityFunc<'T>, ...) : Accessor<'T>
```

The JSX tab shows the object Fable builds from `name = "doubled"`:

```fsharp solid jsx
let count, setCount = createSignal 2
let doubled = createMemo ((fun (_: int option) -> count () * 2), name = "doubled")

button (onClick = fun _ -> setCount (count () + 1)) { $"doubled = {doubled ()}" }
```

Most of these functions also have an overload that takes the options as a typed object, such as `MemoOptions<'T>` or
`EffectOptions`.

## Special JSX attributes

_Adapted from [Oxpecker.Solid](https://lanayx.github.io/Oxpecker/src/Oxpecker.Solid/#special-jsx-attributes). Credit
to [Lanayx](https://github.com/Lanayx)._

Attributes that do not fit a typed constructor parameter are extension methods on the element:

| Method | Emits |
| --- | --- |
| `.attr(name, value)` | `name={value}`, for any attribute |
| `.data(name, value)` | `data-name="value"` |
| `.bool(name, value)` | `name={value}` |
| `.ref(element)` | `ref={element}` |
| `.style'(obj)` | `style={obj}`, a style object |
| `.classObj(obj)` | `class={obj}`, a class object map |
| `.spread(obj)` | `{...obj}` |

```fsharp solid jsx
div () {
    span().data("state", "open").attr("aria-busy", false) { "data and attr " }
    span().classObj({| card = true; muted = false |}).style'({| border = "1px dashed" |}) {
        "class and style objects"
    }
}
```

`style` takes a string, and `.style'` an object; likewise `class'` and `.classObj`. Solid 1's `on:`, `prop:` and `use:` escape
hatches (`.on`, `.prop` and `.use'`) were removed in 3.0. See [extension methods](extension-methods.md).

When you pass a variable to `.ref`, make it `mutable`:

```fsharp solid render=RefVariable jsx
open Fable.Core.TS

[<SolidComponent>]
let RefVariable () =
    let mutable inputRef: Dom.HTMLInputElement = JS.undefined

    div () {
        input().ref (inputRef)
        button (onClick = fun _ -> inputRef.value <- "filled via ref") { "Fill" }
    }
```

`.ref` also takes a callback, `.ref (fun (el: Dom.HTMLDivElement) -> ...)`.

## Bindings that read differently in F#

- A store setter takes an updater, never a value: [Stores](solid-js.md#stores).
- A signal setter takes an updater through `.Invoke`: [Setting with the previous value](solid-js.md#setting-with-the-previous-value).
- An async value is a memo whose computation returns a promise, read under `Loading`. `createResource` is gone:
  [Async memos](solid-js.md#async-memos).
- A context is provided by calling it with the value, `ThemeContext "dark" { ... }`: [Context](solid-js.md#context).
- `lazy'` takes Fable's `importDynamic` in place of `importComponent`: [lazy'](solid-js.md#lazy).

## Router

`@solidjs/router` 2 has no JSX components: `createRouter` takes a tree of `route` definitions and returns the
router to render, and links are plain `a` elements. `createAsync` is gone; read a `query` with `createMemo`. See
[Solid Router](solid-router.md).
