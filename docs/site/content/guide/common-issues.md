---
title: Common issues
---

The problems people run into most often, and what to check first.

## The component is not in a `Partas.Solid` namespace

The plugin only treats types whose full name starts with `Partas.Solid` as tags and components. This keeps it from
rewriting code it should leave alone. A `[<SolidTypeComponent>]` type declared anywhere else gets a warning and is not
transformed into a component, and a custom tag type is not emitted as a JSX tag.

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

## Statements before the element in a `For` child

In a `For.Keyed` or `For.NonKeyed` child function, a statement before the element (a side effect such as a function
call) currently makes the plugin drop the element. A `let` binding before the element works:

```fsharp
// The <li> is lost
yield fun item _ ->
    log item
    li () { item }

// Works
yield fun item _ ->
    let label = item.ToUpper()
    li () { label }
```

## Typed arrays

Fable compiles numeric arrays to typed arrays, so `[| 1; 2; 3 |]` becomes an `Int32Array`. JavaScript libraries that
expect a plain array can fail on it. Compile with `--typedArrays false`, or `box` the values. See
[Installation](installation.md).

## Compiling in Debug

The plugin expects the Release AST. Always pass `-c Release` to Fable, including under `dotnet fable watch`.

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

## Known bugs in 3.0

These are known and tracked. Work around them until they are fixed:

- **Context providers.** `Ctx value { ... }` emits `<Ctx.Provider>`, which does not exist in Solid 2. See
  [API differences](api-differences.md).
- **ARIA properties.** `ariaLabel`, `ariaExpanded` and the rest are emitted in camelCase instead of `aria-label` and
  so on. Use `.attr("aria-label", ...)` instead. See [ARIA attributes](aria-attributes.md).
- **Raw string children.** String literals are written into the JSX unescaped, so `<`, `>`, `{` and `}` are read as
  JSX, and leading or trailing spaces next to elements can be lost.
- **`Errored` helpers.** `fallbackEle` and `fallbackFn` emit props of those literal names. Set `fallback` with `!^`.
- **`InvokeAndGet`** returns `undefined`.
- **`[<SolidComponent>]` let-bindings** are emitted as plain calls, so Solid tracks their body.
- **`spellcheck = false`** emits `spellcheck={false}`, which Solid 2 removes. Use `.attr("spellcheck", "false")`.
