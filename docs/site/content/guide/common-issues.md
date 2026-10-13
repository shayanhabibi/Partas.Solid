---
title: Common issues
---

## The component is not in a `Partas.Solid` namespace

The plugin only treats types whose full name starts with `Partas.Solid` as tags and components. A
`[<SolidTypeComponent>]` declared anywhere else gets a warning and is not transformed, and a custom tag type is not
emitted as a JSX tag.

```fsharp
// Works
namespace Partas.Solid.MyApp

// Is not picked up
namespace MyApp
```

## A function child without `yield`

Components such as `For`, `Repeat`, `Show.Keyed` and `Show.NonKeyed` take a function as their child. You must `yield`
it:

```fsharp
// Wrong: the function is not passed to For
For.Keyed(each = [| "1"; "2" |]) {
    fun item index -> li () { item }
}

// Right
For.Keyed(each = [| "1"; "2" |]) {
    yield fun item index -> li () { item }
}
```

If your IDE cannot infer the types of `item` and `index`, the `yield` is probably missing.

## Nothing updates when a signal changes

A signal read in the component body is read once, because the body runs once. Read it inside the JSX to keep it live:

```fsharp solid render=ReadOnce jsx
[<SolidComponent>]
let ReadOnce () =
    let count, setCount = createSignal 0
    let readInBody = count ()

    div () {
        button (onClick = fun _ -> setCount (count () + 1)) { "Add one" }
        p () { $"Read in the body: {readInBody}" }
        p () { $"Read in the JSX: {count ()}" }
    }
```

See [Building the DOM](building-the-dom.md).

## Store items in a number array do not update

Compile with `--typedArrays false`. Without it, `[| 1; 2; 3 |]` is an `Int32Array`, which stores do not make reactive.
See [Compiling](installation.md#compiling).

## The output looks wrong in Debug

The plugin expects the Release AST. Always pass `-c Release` to Fable, including under `dotnet fable watch`.

## `Attr.Dir` or another `Attr` member is not defined

`open Fable.Core.TS.Dom` brings in DOM's `Attr` type, which hides Partas.Solid's `Attr` module. Open `Fable.Core.TS`
instead and write `Dom.HTMLElement`.

## Children of a `<template>`

Solid 2's development build rejects any JSX child of `<template>` with "The HTML provided is malformed", because its
template check cannot see template contents. Set the content with `innerHTML` instead.

## Output that is silently missing

If the plugin drops part of a component, compile that component with `ComponentFlag.PrintDisposals` to log every
expression it discards, or `ComponentFlag.DebugMode` to dump the AST it received:

```fsharp
[<SolidComponent(ComponentFlag.VerboseDebugMode)>]
let Broken () = ...
```

Include that output when you [submit an issue](../contributing/submit-issues.md). See
[attribute flags](attribute-flags.md) for the other flags.
