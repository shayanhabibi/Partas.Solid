---
title: Building the DOM
---

You can compose the DOM from `[<SolidComponent>]` functions, `[<SolidTypeComponent>]` types, or both. Mix them as it
suits you.

:::tip
If you build components for others to use, prefer `[<SolidTypeComponent>]`.

- F# callers use the same DSL as for built-in tags, and every prop is optional.
- JavaScript callers get a normal Solid component: all props arrive in one object, so using it as a tag never drops an
  argument.
:::

## Large and small components

Solid does not re-render components. A component runs once, and fine-grained reactivity updates only the DOM that
depends on a changed signal. So there is no performance reason to split a large component into small ones, or to
merge small ones. Split them where it makes the code easier to read.

## Reactivity

Solid's reactivity is pleasant to work with, but it can surprise you if you come from React. The
[Solid documentation](https://docs.solidjs.com/) covers it better than this page can, so read it there. The one rule
that matters most in Partas.Solid:

**A component body runs once. A value read in the body is a snapshot. A value read inside the JSX stays live.**

The two components below differ only in where they read `props.active`.

```fsharp solid render=ReactivityExample jsx
[<Erase>]
type SnapshotLabel() =
    inherit span()

    [<Erase>]
    member val active: bool = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        // Read once, when the component is created.
        let text = if props.active then "snapshot: on" else "snapshot: off"
        span () { text }

[<Erase>]
type LiveLabel() =
    inherit span()

    [<Erase>]
    member val active: bool = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        // Read in the JSX, so it updates.
        span () { if props.active then "live: on" else "live: off" }

[<SolidComponent>]
let ReactivityExample () =
    let active, setActive = createSignal false

    div () {
        button (onClick = fun _ -> setActive (not (active ()))) { "Toggle" }
        p () { SnapshotLabel(active = active ()) }
        p () { LiveLabel(active = active ()) }
    }
```

Press the button. The live label follows the signal. The snapshot label keeps the value it had when it was created.
In the JSX tab, `SnapshotLabel` stores `props.active ? ... : ...` in a `const`, while `LiveLabel` puts the same
expression in the JSX.

To keep a derived value live in the body, make it a function (`let text () = ...`) and call it in the JSX, or wrap it
in `createMemo`.

:::warning
`[<SolidComponent>]` let-bindings are currently emitted as plain function calls inside the JSX. Solid then tracks
their body, so a signal read in the body rebuilds that component's DOM instead of taking a snapshot. Use
`[<SolidTypeComponent>]` when you rely on the snapshot behaviour described above.
:::

## Conditional rendering

An `if` in a children block becomes a ternary in the JSX. `Show` is the explicit form. It also takes a `fallback`
for when the condition is false.

```fsharp solid render=Conditional jsx
[<SolidComponent>]
let Conditional () =
    let truthy, setTruthy = createSignal false

    div () {
        button (onClick = fun _ -> setTruthy (not (truthy ()))) { "Click me!" }
        if truthy () then
            p () { "Boo!" }
        Show(when' = not (truthy ()), fallback = p () { "It went quiet." }) {
            p () { "Do you hear that?" }
        }
    }
```

`Show.Keyed` and `Show.NonKeyed` take a child function instead of plain children. The function receives the value,
or an accessor for it. See [solid-js](solid-js.md).

### Switch and Match

For more than two cases, use `Switch` with a `Match` per case. The first `Match` whose `when'` is true renders.
`Switch` shows its `fallback` when none do.

```fsharp solid render=TrafficLight
[<SolidComponent>]
let TrafficLight () =
    let state, setState = createSignal "red"

    div () {
        button (
            onClick =
                fun _ ->
                    setState (
                        match state () with
                        | "red" -> "green"
                        | "green" -> "amber"
                        | _ -> "red"
                    )
        ) {
            "Next"
        }
        Switch(fallback = p () { "unknown" }) {
            Match(when' = (state () = "red")) { p (style = "color: #dc2626") { "Stop" } }
            Match(when' = (state () = "amber")) { p (style = "color: #d97706") { "Wait" } }
            Match(when' = (state () = "green")) { p (style = "color: #16a34a") { "Go" } }
        }
    }
```

## Lists

`For` renders a list. In Solid 2 you choose how rows are kept when the list changes:

| Form | Rows are keyed by | The child function receives |
| --- | --- | --- |
| `For.Keyed` | item identity | the item, and an `Accessor<int>` for its index |
| `For.NonKeyed` | position | an `Accessor` for the item, and the index as an `int` |

`For.NonKeyed` replaces Solid 1's `Index`, which no longer exists. The child is a function, so you must `yield` it.

```fsharp solid render=Shopping jsx
[<SolidComponent>]
let Shopping () =
    let items, setItems = createSignal [| "Milk"; "Bread" |]

    div () {
        button (onClick = fun _ -> setItems (Array.append (items ()) [| "Item " + string (items().Length + 1) |])) {
            "Add"
        }
        button (onClick = fun _ -> setItems (Array.rev (items ()))) { "Reverse" }
        ol () {
            For.Keyed(each = items (), fallback = li () { "Nothing yet" }) {
                yield fun item index -> li () { item + " at " + string (index ()) }
            }
        }
    }
```

:::warning
Do not put a statement before the element in a `For` child function. A side effect such as a function call, followed
by the element, currently makes the plugin drop the element. A `let` binding before the element is fine. Move side
effects into a helper component or into a function called from the expression.
:::

`Repeat` renders a row per number instead of per item: `Repeat(count = 3) { yield fun i -> ... }`.

## Async and errors

Solid 2 renamed its boundaries:

| Solid 1 | Solid 2 | Purpose |
| --- | --- | --- |
| `Suspense` | `Loading` | Shows `fallback` while async values under it are pending. |
| `ErrorBoundary` | `Errored` | Shows `fallback` when something under it throws. |
| `SuspenseList` | `Reveal` | Orders how sibling `Loading` boundaries reveal. |

`createResource` is gone. In Solid 2 an async value is a memo or store whose computation returns a promise, and
`Loading` waits for it.

```fsharp
// fetchUser: int -> JS.Promise<User>
let user = createMemo (fun (_: User option) -> fetchUser (userId ()))

Errored(fallback = !^(p () { "Something broke." })) {
    Loading(fallback = p () { "Loading..." }) {
        p () { user().Name }
    }
}
```

To get the error and a reset function, pass an `ErrorBoundary.Fallback`:

```fsharp
Errored(
    fallback =
        !^(ErrorBoundary.Fallback(fun err reset ->
            div () {
                p () { (err () :?> exn).Message }
                button (onClick = fun _ -> reset ()) { "Retry" }
            }))
) {
    RiskyWidget()
}
```

:::warning
`Errored` also has `fallbackEle` and `fallbackFn` setters. They currently compile to props of those literal names,
which Solid ignores. Set `fallback` with `!^` as above.
:::

The Solid documentation on async data and error handling is recommended reading. The [solid-js](solid-js.md) page
lists the bindings.
