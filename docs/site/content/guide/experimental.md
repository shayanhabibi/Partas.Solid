---
title: Experimental Features
---

`Partas.Solid.Experimental` holds syntax sugar that has seen less real-world use than the rest of the library.

## Computation expressions

Many `solid-js` functions take a lambda with no parameters, such as `onSettled` or `children`. These builders write
the lambda and the call for you. They take no options: for an option or another overload, call the function
directly.

```fsharp solid setup
open Partas.Solid.Experimental
```

```fsharp
open Partas.Solid.Experimental
```

`memo` derives a value, `effect` reacts to it, and `mount` runs once after the first render. The JSX tab shows the
calls they become:

```fsharp solid render=BuilderCounter jsx
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

| Builder | Same as |
| --- | --- |
| `lambda { e }` | `fun () -> e` |
| `memo { e }` | `createMemo (fun _ -> e)` |
| `mount { e }` | `onSettled (fun () -> e)` |
| `cleanup { e }` | `onCleanup (fun () -> e)` |
| `children { e }` | `children (fun () -> e)` |
| `lazyload { e }` | `lazy' (fun () -> e)`, with `importDynamic` |
| `reaction { e }` | `createReaction (fun () -> e)`, returning its `track` function |

### effect

`effect` is Solid 2's two-phase effect. The `let!` names the tracked source and becomes the compute function; the
rest of the body is the effect function, and gets the value.

- The `let!` is required, and there can only be one. It binds an accessor (`unit -> 'T`).
- To track several sources, bind a `lambda` that reads them all:

  ```fsharp
  effect {
      let! first, last = lambda { firstName (), lastName () }
      console.log $"{first} {last}"
  }
  ```

- Statements before the `let!` run once, when the builder runs. They are not part of the effect.
- The effect body is untracked: signals it reads do not re-run it.

A 2.x `effect { }` without a `let!` no longer compiles. Move the reads that should trigger it into a `let!`.

### memo

The last expression is the value. The builder does not give you the previous value; call `createMemo` directly when
you need it. A `cleanup` inside a `memo`, or inside the `lambda` of an effect's `let!`, runs before each
recomputation and when the owner is disposed.

### reaction

`reaction` returns a `track` function. Call it with the source to watch: the body runs once, the next time that
source changes. Call `track` again to watch for the next change. Here the second toggle changes nothing:

```fsharp solid render=FirstOpen jsx
[<SolidComponent>]
let FirstOpen () =
    let isOpen, setOpen = createSignal false
    let note, setNote = createSignal "Not opened yet"

    let track = reaction { setNote "Opened for the first time" }
    track (fun () -> box (isOpen ()))

    div () {
        button (onClick = fun _ -> setOpen (not (isOpen ()))) { "Toggle" }
        p () { $"Open: {isOpen ()}. {note ()}" }
    }
```

An `if ... then` without an `else`, followed by another statement, does not type-check inside a `reaction` block.

### children

```fsharp
let resolved = children { props.children }
let hasChildren = lambda { resolved.toArray().Length > 0 }
```

### Removed builders

`batch { }` and `selector { }` went with `batch` and `createSelector`, which Solid 2 dropped. Use
`flush (fun () -> ...)` and `createProjection`.

## Erased union implicit casting

Bindings often type an argument as an erased union such as `U2<string, int>`, and Fable's unions need `!^` to cast a
value into it. The `U` module in `Partas.Solid.Experimental` redefines `U2` to `U9` with implicit conversions, so a
plain value converts without `!^`. Partas.Solid's HTML, SVG and style bindings are typed with it.

:::tip
Open `Partas.Solid.Experimental.U` after `Fable.Core`, so its union types shadow Fable's.
:::

```fsharp
open Fable.Core
open Partas.Solid.Experimental.U

[<Import("somefunction", "somelibrary")>]
let numberOrString (value: U2<string, int>) : obj = jsNative

numberOrString "Something"
numberOrString 2
numberOrString 3. // error: float is not in the union
```

F# may warn about each implicit conversion; `!^` silences it. The conversion does not apply to array or list
literals: give the literal its type, or use `!^`:

```fsharp
let numberOrList (value: U2<int, int list>) : obj = jsNative

numberOrList [ 5; 6 ] // error: expected U2<int, int list>, got 'a list
numberOrList ([ 5; 6 ]: int list) // works
numberOrList !^[ 5; 6 ] // works
```
