---
title: Experimental Features
---

The `Partas.Solid.Experimental` namespace holds syntax sugar that has seen less real-world use than
the rest of the library. Use it with care: some builders are better tested than others, and the
known problems are listed [below](#known-problems).

## Computation expressions

Many `solid-js` functions take a lambda with no parameters, such as `onSettled` or `children`. The
builders in this namespace write that lambda and the call for you. They take no options: when you
need an option or a different overload, call the function directly.

To use them, open the namespace:

```fsharp
open Partas.Solid.Experimental
```

```fsharp solid setup
open Partas.Solid.Experimental
```

Here are three of them working together. `memo` derives a value, `effect` reacts to it, and `mount`
runs once after the first render.

```fsharp solid render=BuilderCounter
[<SolidComponent>]
let BuilderCounter () =
    let count, setCount = createSignal 1
    let lastSeen, setLastSeen = createSignal 0
    let settled, setSettled = createSignal false

    let doubled = memo { count () * 2 }

    effect {
        let! d = doubled
        setLastSeen d
    }

    mount { setSettled true }

    div () {
        button (onClick = fun _ -> setCount (count () + 1)) { "Add one" }
        p () { $"doubled = {doubled ()}, the effect last saw {lastSeen ()}, settled = {settled ()}" }
    }
```

### lambda

Wraps the computation in `fun () -> ...`.

```fsharp
let config = lambda { Data.config.data }
// Same as
let config = fun () -> Data.config.data

console.log (config ())
```

### effect

Wraps the computation in `createEffect (compute, effect)`, the two-phase effect of Solid 2. The
`let!` names the tracked source: it becomes the compute function. The rest of the body is the
effect function, and gets the value.

```fsharp
effect {
    let! value = count
    console.log $"count is {value}"
}
// Same as
createEffect ((fun (_: int option) -> count ()), fun (value: int) -> console.log $"count is {value}")
```

- The `let!` is required, and there can only be one. It must bind an accessor (`unit -> 'T`).
- To track several sources, bind a `lambda` that reads them all:

  ```fsharp
  effect {
      let! first, last = lambda { firstName (), lastName () }
      console.log $"{first} {last}"
  }
  ```

- Statements written before the `let!` run immediately, once, when the builder runs. They are not
  part of the effect.
- The effect body is untracked. Signals it reads do not re-run it.

:::note
In Partas.Solid 2.x, `effect { ... }` wrapped a single-function `createEffect (fun () -> ...)`.
Solid 2 has no single-function effect, so a 2.x `effect` block without a `let!` no longer compiles.
Move the reads that should trigger it into a `let!`.
:::

### mount

Wraps the computation in `onSettled (fun () -> ...)`. `onSettled` replaces `onMount` in Solid 2.

```fsharp
mount {
    inputRef.focus ()
}
// Same as
onSettled (fun () -> inputRef.focus ())
```

### cleanup

Wraps the computation in `onCleanup (fun () -> ...)`.

```fsharp
cleanup {
    thishere.available <- false
}
// Same as
onCleanup (fun () -> thishere.available <- false)
```

A `cleanup` inside a `memo`, or inside the `lambda` of an effect's `let!`, runs before each
recomputation, and when the owner is disposed.

### memo

Wraps the computation in `createMemo (fun _ -> ...)`. The last expression is the value.

```fsharp
let label =
    memo {
        let v = value ()
        if v > 0 then "positive" elif v < 0 then "negative" else "zero"
    }
// Same as
let label =
    createMemo (fun (_: string option) ->
        let v = value ()
        if v > 0 then "positive" elif v < 0 then "negative" else "zero")
```

Write the memo as a plain body that ends in its value. Do not use `let!` and `return` in it (see
[Known problems](#known-problems)). The builder does not give you the previous value. Call
`createMemo` directly when you need it.

### lazyload

Wraps the computation in `lazy' (fun () -> ...)`.

```fsharp
let Settings = lazyload { importDynamic "./Settings.fs.jsx" }
// Same as
let Settings = lazy' (fun () -> importDynamic "./Settings.fs.jsx")
```

`importComponent` was removed in 3.0. Use Fable's `importDynamic`.

### children

Wraps the computation in `children (fun () -> ...)`.

```fsharp
let resolved = children { props.children }
// Same as
let resolved = children (fun () -> props.children)

let hasChildren = lambda { resolved.toArray().Length > 0 }
// ...
if hasChildren () then resolved.Invoke ()
```

### reaction

Wraps the computation in `createReaction (fun () -> ...)`, and returns its `track` function. Call
`track` with the source to watch. The body runs once, the next time that source changes. Call
`track` again to watch for the next change.

```fsharp
let isOpen, setOpen = createSignal false

let onFirstOpen =
    reaction {
        console.log "opened for the first time"
    }

onFirstOpen (fun () -> box (isOpen ()))
// Same as
let onFirstOpen = createReaction (fun () -> console.log "opened for the first time")
```

An `if ... then` without an `else`, followed by another statement, does not type-check inside a
`reaction` block.

### Removed builders

`batch { }` and `selector { }` were removed along with `batch` and `createSelector`, which Solid 2
dropped. Use `flush (fun () -> ...)` and `createProjection` instead. See
[Solid-JS](../guide/solid-js.md#removed-apis).

## Known problems

:::warning
These builders emit wrong code in some shapes. The runtime tests track each one as a known bug.

- `memo { let! v = source; return v * 2 }` caches a function instead of the value, and never tracks
  `source`. Write `memo { source () * 2 }` instead.
- `mount { }` and `cleanup { }` drop every statement after an `if ... then` with no `else`, or after
  a `match`. Put the branch last, give the `if` an `else`, or call `onSettled` or `onCleanup`
  directly.
:::

## Erased union implicit casting

Bindings often accept several types for one argument, typed as an erased union such as
`U2<string, int>`. With Fable's unions you cast a value into the union with the `!^` operator.

The `U` module in `Partas.Solid.Experimental` redefines `U2` to `U9` with implicit conversions, so a
plain value converts to the union without `!^`. It is most useful when writing bindings, because
calls to them read with less noise.

:::tip
Open `Partas.Solid.Experimental.U` AFTER `Fable.Core`, so its union types shadow Fable's.
:::

```fsharp
open Fable.Core
open Fable.Core.JsInterop
open Partas.Solid.Experimental.U

[<Import("somefunction", "somelibrary")>]
let numberOrString (value: U2<string, int>) : obj = jsNative

// With Fable's unions:
numberOrString !^"Something"
numberOrString "Something" // error
numberOrString !^3. // error

// With the U module:
numberOrString "Something"
numberOrString 2
numberOrString 3. // still an error: float is not in the union
```

Depending on your settings, F# may warn about each implicit conversion. Using `!^` there silences
the warning, and the call works either way.

:::tip
The implicit conversion does not apply to array or list literals. Give the value its type
explicitly, or use `!^` as before.
:::

```fsharp
let numberOrList (value: U2<int, int list>) : obj = jsNative

numberOrList 5 // works
numberOrList [ 5; 6 ] // error: expected U2<int, int list>, got 'a list
numberOrList ([ 5; 6 ]: int list) // works
numberOrList !^[ 5; 6 ] // works
numberOrList !^[ "5"; "6" ] // error, as expected
```
