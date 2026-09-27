---
title: Common issues
---

The problems people run into most often, and what to check first.

## The component is not in a `Partas.Solid` namespace

The plugin only treats types whose full name starts with `Partas.Solid` as tags and components, so it leaves other
code alone. A `[<SolidTypeComponent>]` declared anywhere else gets a warning and is not transformed, and a custom tag
type is not emitted as a JSX tag.

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

## Typed arrays

Fable compiles numeric arrays to typed arrays, so `[| 1; 2; 3 |]` becomes an `Int32Array`. JavaScript libraries that
expect a plain array can fail on it. Compile with `--typedArrays false`, or `box` the values. See
[Installation](installation.md).

## Compiling in Debug

The plugin expects the Release AST. Always pass `-c Release` to Fable, including under `dotnet fable watch`.

## Children of a `<template>`

Solid 2's development build rejects any JSX child of `<template>` with "The HTML provided is malformed", because its
template check re-parses the markup and cannot see template contents. Set the content with `innerHTML` instead.

## Nothing updates when a signal changes

A value read in a component body is read once. Read it inside the JSX, or make it a function, to keep it live. See
[Building the DOM](building-the-dom.md).

## Output that is silently missing

If the plugin drops part of a component, compile that component with `ComponentFlag.PrintDisposals` to log every
expression it discards, or `ComponentFlag.DebugMode` to dump the AST it received:

```fsharp
[<SolidComponent(ComponentFlag.VerboseDebugMode)>]
let Broken () = ...
```

Include that output when you [submit an issue](../contributing/submit-issues.md). See
[attribute flags](attribute-flags.md) for the other flags.

