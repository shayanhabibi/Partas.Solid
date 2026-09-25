---
title: Solid-JS
---

This page covers the `solid-js` bindings in Partas.Solid 3.0, which target Solid 2.0 (`2.0.0-rc.9`). It
lists the types and functions, shows how each one reads in F#, and runs small examples where they
compile.

Solid 2.0 removed or renamed a large part of the 1.x API. If you are upgrading, read
[Migrating to Solid 2](../guide/migrating-to-solid-2.md) first. The table at the
[end of this page](#removed-apis) maps each removed API to its replacement.

## How the bindings are laid out

- Everything imported from `solid-js` sits on the `Bindings` type in the `Partas.Solid` namespace.
  It is `[<AutoOpen>]`, so `open Partas.Solid` is all you need. The functions are `static member`s,
  which is how they get overloads.
- The web runtime (`@solidjs/web`) lives in the `Partas.Solid.Web` namespace: `render`, `hydrate`,
  `renderToString`, `renderToStream`, `isServer`, `isDev`, `clientOnly`, `Portal` and `Dynamic`.
  Add `open Partas.Solid.Web` to use them. `DEV` stays in `Partas.Solid`.
- Stores are part of `solid-js` now. There is no `solid-js/store` module.
- A control-flow component that takes children implements `HtmlContainer`. One that takes a
  function as its child implements a child-lambda interface, and you pass the function with
  `yield fun ... -> ...`.

:::tip
See something missing? Open an issue. Missing bindings are usually quick to add.
:::

The examples below run on the page. Each one is compiled with Fable and the Partas.Solid plugin
when the site builds.

```fsharp solid setup
let resolveAfter (ms: int) (value: 'T) : JS.Promise<'T> =
    emitJsExpr (ms, value) "new Promise(resolve => setTimeout(() => resolve($1), $0))"
```

## Signals

```fsharp
type Accessor<'T> = unit -> 'T
type Setter<'T> = 'T -> unit
type Signal<'T> = Accessor<'T> * Setter<'T>
```

`createSignal` returns an accessor and a setter. Call the accessor to read the value. Reading it
inside a tracking scope (a memo, the compute half of an effect, or JSX) subscribes that scope to it.

```fsharp solid render=SignalCounter
[<SolidComponent>]
let SignalCounter () =
    let count, setCount = createSignal 0

    button (onClick = fun _ -> setCount (count () + 1)) {
        $"Clicked {count ()} times"
    }
```

Solid 2 batches writes. A setter queues the change, and the DOM updates on the next microtask, not
when the setter returns. Call `flush ()` when you need the update applied immediately.

### createSignal

| Call | Returns | |
| --- | --- | --- |
| `createSignal value` | `Signal<'T>` | A plain signal |
| `createSignal<'T> ()` | `Signal<'T option>` | Starts as `None` (`undefined` in JS) |
| `createSignal<'T> (fun () -> ...)` | `Signal<'T>` | A writable derived signal: it recomputes from its sources, and you can still set it |
| `createSignal (value, equals = ..., name = ..., ownedWrite = ..., unobserved = ...)` | `Signal<'T>` | Options as named arguments |
| `createSignal (value, SignalOptions(...))` | `Signal<'T>` | Options as an object |

```fsharp
let name, setName = createSignal<string> ()          // Signal<string option>
let doubled, setDoubled = createSignal<int> (fun () -> count () * 2)
let point, setPoint =
    createSignal ({ x = 0; y = 0 }, equals = EqualityFunc (fun prev next -> prev = next))
```

`equals` takes an `EqualityFunc<'T>`, a delegate of `prev * next -> bool`. Return `true` to treat the
new value as unchanged, so nothing downstream runs.

A function passed as the argument to `createSignal` makes a derived signal. The type argument is
there because the value and function overloads are otherwise ambiguous for a lambda (FS0041).

### Setting with the previous value

A setter is typed `'T -> unit`, so F# will not let you pass it an updater function directly. Use the
`Invoke` extension:

```fsharp
let index, setIndex = createSignal 0

setIndex 5                         // replace the value
setIndex.Invoke 5                  // same thing
setIndex.Invoke (fun i -> i + 1)   // compute the next value from the previous one
```

:::warning
Solid treats any function passed to a setter as an updater. To store a function in a signal, wrap it
in an updater that returns it, even when you are only replacing the value:

```fsharp
let myFunc () = console.log "myFunc"
let handler, setHandler = createSignal (unbox<unit -> (unit -> unit)> myFunc)
let myNewFunc () = console.log "newFunc"
setHandler (fun () -> myNewFunc)
```
:::

`InvokeAndGet` also exists, with the same two overloads. It is meant to set the value and return it,
but it currently returns `undefined`. Do not rely on its result.

## Memos

```fsharp
createMemo (fun (prev: 'T option) -> ...) : Accessor<'T>
```

A memo is a cached, read-only derived value. The compute function tracks what it reads, runs again
when any of it changes, and only notifies its own readers when the result is different. It gets the
previous result as an option: `None` on the first run.

```fsharp solid render=MemoDemo
[<SolidComponent>]
let MemoDemo () =
    let count, setCount = createSignal 1
    let doubled = createMemo (fun (_: int option) -> count () * 2)
    let runningTotal = createMemo (fun (prev: int option) -> defaultArg prev 0 + count ())

    div () {
        button (onClick = fun _ -> setCount (count () + 1)) { "Add one" }
        p () { $"count = {count ()}, doubled = {doubled ()}, running total = {runningTotal ()}" }
    }
```

Annotate the parameter (`fun (_: int option) -> ...`). It tells F# which overload you mean, and it
keeps the result type from being inferred as a promise.

Options go in as named arguments: `name`, `equals`, `transparent`, `unobserved`, `lazy`, `sync` and
`loadingValue`.

```fsharp
let expensive = createMemo ((fun (_: int option) -> heavyWork (input ())), ``lazy`` = true)
```

A `lazy` memo does not compute until something reads it.

:::warning
`createMemo (compute, MemoOptions (...))` emits its options in the wrong shape at the moment. Pass
options as named arguments instead.
:::

### Async memos

A memo whose compute returns a `JS.Promise<'T>` gives you an `Accessor<'T>`. Reading it before the
promise settles suspends the reader, and the nearest [`Loading`](#loading) boundary shows its
fallback. There is no `createResource` in Solid 2: an async memo is its replacement.

```fsharp
let user = createMemo (fun (_: User option) -> fetchUser (userId ()))
```

`createMemo (compute, loadingValue)` gives the memo a value to hold while the first promise is
pending.

## Effects

Solid 2 splits an effect into two functions:

1. **compute** runs in a tracking scope. It reads signals and returns a value. It gets the previous
   value as an option.
2. **effect** gets that value and does the side effect. Nothing it reads is tracked.

```fsharp
createEffect (
    (fun (_: int option) -> count ()),
    fun (n: int) -> console.log $"count is {n}"
)
```

Annotate the compute function's parameter, as with a memo, so F# can pick the overload.

The effect runs after the first render, then again each time the compute result changes. The
single-function `createEffect (fun () -> ...)` from Solid 1 no longer exists.

```fsharp solid render=EffectDemo
[<SolidComponent>]
let EffectDemo () =
    let count, setCount = createSignal 0
    let message, setMessage = createSignal "The effect has not run yet"

    createEffect ((fun (_: int option) -> count ()), (fun (n: int) -> setMessage $"The effect last saw {n}"))

    div () {
        button (onClick = fun _ -> setCount (count () + 1)) { "Click" }
        p () { message () }
    }
```

This example writes a signal from an effect to show when the effect runs. In real code, derive the
value with a memo instead.

### Cleanup

The effect function can return a cleanup, a `unit -> unit`. It runs before the effect runs again,
and when the owner is disposed. `createEffect` has overloads for an effect function that returns a
cleanup, so return it from the lambda:

```fsharp
createEffect (
    (fun (_: int option) -> delayMs ()),
    fun (ms: int) ->
        let id = JS.setInterval tick ms
        fun () -> JS.clearInterval id
)
```

`createRenderEffect`, `createTrackedEffect`, `createReaction` and `onSettled` accept a
cleanup-returning function in the same way. (The primed imports such as `createEffect'` are hidden
implementation details of these overloads. Do not call them.)

### Options and errors

Pass options as named arguments (`defer`, `schedule`, `sync`, `transparent`, `name`) or as an
`EffectOptions` object. `defer = true` skips the first run of the effect function, so it first runs
on the first change.

```fsharp
createEffect ((fun (_: int option) -> value ()), (fun (v: int) -> save v), defer = true)
```

To handle an error thrown by the compute function, add an error handler as the third argument:

```fsharp
createEffect (
    (fun (_: int option) -> parse (input ())),
    effect = (fun (v: int) -> show v),
    error = (fun (err: obj) -> console.error err)
)
```

`EffectBundle (effect, error)` passes the two as one object. Its handler is an `EffectErrorHandler`,
which also receives the effect's cleanup:

```fsharp
createEffect (
    (fun (_: int option) -> parse (input ())),
    EffectBundle (
        (fun (v: int) -> show v),
        EffectErrorHandler (fun err cleanup ->
            console.error err
            cleanup ())
    )
)
```

### Other effect primitives

| Function | Use |
| --- | --- |
| `createRenderEffect (compute, effect)` | Same two phases, but the first effect run happens immediately, during rendering. Refs are not attached yet. Within a flush, render effects run before user effects. No error overload. |
| `createTrackedEffect (fun () -> ...)` | A single function that both tracks and does the side effect. `createTrackedEffect'` may return a cleanup. Prefer `createEffect`; this is for cases where the two phases cannot be separated. |
| `createReaction effect` | Returns a `track` function. Call `track (fun () -> box (source ()))` to name what to watch; the next time it changes, `effect` runs once. Call `track` again to re-arm it. |
| `onSettled (fun () -> ...)` | Runs once after the owner's first render has settled. It replaces `onMount`. `onSettled'` may return a cleanup. |
| `onCleanup (fun () -> ...)` | Runs when the current owner is disposed or re-runs. |

```fsharp
let isOpen, setOpen = createSignal false
let track = createReaction (fun () -> console.log "opened for the first time")
track (fun () -> box (isOpen ()))
```

### flush and untrack

`flush ()` applies all queued writes now. `flush (fun () -> ...)` runs the function, then flushes,
and returns what it returned. It replaces `batch`: writes are always batched in Solid 2, and `flush`
is how you ask for them to land.

```fsharp
flush (fun () ->
    setFirst "Ada"
    setLast "Lovelace")
```

`untrack (fun () -> ...)` reads signals without subscribing to them:

```fsharp
let total = createMemo (fun (_: int option) -> value () + untrack other)
```

## Stores

```fsharp
type Store<'T>                                // read with .Value, or let it convert to 'T
type StoreSetter<'T> = ('T -> 'T) -> unit
type StoreReturn<'T> = Store<'T> * StoreSetter<'T>
```

A store is a deeply reactive object. Every property you read through it is tracked on its own, so
changing one field only updates what read that field. `createStore` is imported from `solid-js`.

- Read through `.Value`: `state.Value.todos`. `Store<'T>` also converts implicitly to `'T`.
- The setter takes an **updater, never a value**. Either mutate the draft and return it, or return a
  new object.

```fsharp
type Settings = { mutable theme: string; mutable fontSize: int }

let settings, setSettings = createStore { theme = "light"; fontSize = 14 }

// mutate the draft in place
setSettings (fun s ->
    s.fontSize <- 16
    s)

// or return a replacement
setSettings (fun s -> { s with theme = "dark" })
```

Mark the fields you mutate as `mutable`. The draft is a proxy, so writing to it records exactly
which properties changed.

```fsharp solid render=StoreCart
type CartLine = { name: string; mutable qty: int }

[<SolidComponent>]
let StoreCart () =
    let cart, setCart =
        createStore<CartLine array> [| { name = "Apples"; qty = 1 }; { name = "Pears"; qty = 0 } |]

    let total =
        createMemo (fun (_: int option) -> cart.Value |> Array.sumBy (fun l -> l.qty))

    let addOne (name: string) =
        setCart (fun lines ->
            for l in lines do
                if l.name = name then
                    l.qty <- l.qty + 1

            lines)

    div () {
        ul () {
            For.Keyed(each = cart.Value) {
                yield fun line _ ->
                    li () {
                        button (onClick = fun _ -> addOne line.name) { "+1" }
                        span () { $"{line.name}: {line.qty}" }
                    }
            }
        }

        p () { $"Total items: {total ()}" }
    }
```

The rows are keyed by the line objects. Adding one updates the count in that row without
re-creating it.

`createStore` has a few overloads that can clash for a value like an array literal. When F# reports
FS0041, give the type argument explicitly, as above: `createStore<CartLine array> [| ... |]`.
`createStore (value, name = ..., shallow = ...)` and `createStore (value, StoreOptions (...))` take
options.

### reconcile, snapshot and deep

| Function | Use |
| --- | --- |
| `reconcile next` | Returns an updater that diffs `next` into the store, so only changed fields notify. Use it as `setStore (reconcile next)`. `reconcile (next, "id")` or `reconcile (next, fun item -> item.id)` sets how array items are matched. |
| `snapshot store` | A plain, non-reactive copy of the store's current value. It replaces `unwrap`. |
| `deep store` | Reads every nested property, so a tracking scope that calls it re-runs on any change inside the store. |

```fsharp
setTodos (reconcile freshFromServer)

createEffect ((fun _ -> deep settings), (fun s -> localStorage.setItem ("settings", JSON.stringify s)))
```

### Derived stores and projections

`createStore (fn, seed)` makes a writable store derived from other state. The function gets the
current draft. Return `Some` new value, or mutate the draft and return `None`. It may return a promise, in
which case reads suspend like an async memo. `createProjection (fn, seed)` does the same with
projection options. It is the Solid 2 replacement for `createSelector`.

```fsharp
type Row = { id: int; mutable active: bool }

let row, setRow =
    createStore (
        (fun (draft: Row) ->
            draft.active <- draft.id = selectedId ()
            U2.Case1 None),
        { id = 1; active = false }
    )
```

A seed type with an `id` member uses it as the key. Otherwise pass a key function as the third
argument.

The result is a `RefreshableStoreReturn<'T>`. Its store half converts to a `Store<'T>` with
`.AsStore`, and to a `Refreshable<'T>` for [`refresh`](#refresh) with `.AsRefreshable`.

:::warning
`createProjection` is typed as returning a store and setter tuple, but Solid rc.9 returns a single
refreshable store. Destructuring its result does not work at the moment.
:::

## Control flow

Control-flow components are imported from `solid-js`. Each one takes its props as named arguments
and its children in braces.

### For

`For` makes you choose how rows are keyed. The raw `For` is hidden, and you use one of three
variants:

| Component | Child function | Rows are keyed by |
| --- | --- | --- |
| `For.Keyed<'T>` | `fun (item: 'T) (index: Accessor<int>) -> ...` | Item identity. A row is created once per item and moved when the list reorders. |
| `For.NonKeyed<'T>` | `fun (item: Accessor<'T>) (index: int) -> ...` | Position. Rows stay put and their `item` accessor updates. This replaces `Index`. |
| `For.KeyedFn<'T>` | `fun (item: Accessor<'T>) (index: Accessor<int>) -> ...` | The result of `keyed = fun item -> ...`, for example an id. |

`For.Component` is an alias of `For.Keyed`. Each takes `each` (an array) and an optional `fallback`,
shown when the array is empty.

```fsharp solid render=FruitList jsx
type Fruit = { id: int; name: string }

[<SolidComponent>]
let FruitList () =
    let fruits, setFruits =
        createSignal [| { id = 1; name = "Apple" }; { id = 2; name = "Pear" } |]

    let nextId, setNextId = createSignal 3

    div () {
        button (
            onClick =
                fun _ ->
                    let id = nextId ()
                    setNextId (id + 1)
                    setFruits (Array.append (fruits ()) [| { id = id; name = $"Fruit {id}" } |])
        ) {
            "Add"
        }

        button (onClick = fun _ -> setFruits (Array.rev (fruits ()))) { "Reverse" }
        button (onClick = fun _ -> setFruits [||]) { "Clear" }

        ol () {
            For.Keyed(each = fruits (), fallback = li () { "No fruit" }) {
                yield fun fruit index -> li () { $"{fruit.name} (row {index ()})" }
            }
        }
    }
```

```fsharp
For.NonKeyed(each = names ()) {
    yield fun name index -> li () { string index + ": " + name () }
}

For.KeyedFn(each = users (), keyed = (fun u -> box u.id)) {
    yield fun user index -> li () { user().name }
}
```

:::warning
Keep the child function a single expression that returns the element. If it starts with a statement
(`fun item _ -> log item; li () { ... }`), the element is dropped from the output. Move the statement
into the element's handlers, or compute it before the `For`.
:::

### Show

`Show` renders its children while `when'` is true, and `fallback` otherwise.

```fsharp solid render=ShowDemo
[<SolidComponent>]
let ShowDemo () =
    let isOpen, setOpen = createSignal false

    div () {
        button (onClick = fun _ -> setOpen (not (isOpen ()))) { "Toggle" }

        Show(when' = isOpen (), fallback = p () { "Closed" }) {
            p () { "Open" }
        }
    }
```

To use the value that `when'` checked, pass a child function. `Show.Keyed` gives the child the value
itself, and re-creates the child when the value changes. `Show.NonKeyed` gives it an accessor, and
keeps the child while the value stays truthy.

```fsharp
Show.Keyed(when' = selectedUser (), fallback = span () { "Nobody selected" }) {
    yield fun (user: User option) -> span () { user.Value.name }
}

Show.NonKeyed(when' = selectedUser ()) {
    yield fun (user: Accessor<User option>) -> span () { user().Value.name }
}
```

`Show<'T>` is the generic form, with a `keyed` flag. Prefer the two variants above, which type the
child for you.

### Switch and Match

`Switch` renders the first `Match` whose `when'` is true, or its `fallback`.

```fsharp solid render=TrafficLightDemo
[<SolidComponent>]
let TrafficLightDemo () =
    let light, setLight = createSignal "red"

    let next () =
        match light () with
        | "red" -> setLight "green"
        | "green" -> setLight "amber"
        | _ -> setLight "red"

    div () {
        button (onClick = fun _ -> next ()) { "Next light" }

        Switch(fallback = span () { "Unknown" }) {
            Match(when' = (light () = "red")) { span (style = "color: crimson") { "Stop" } }
            Match(when' = (light () = "amber")) { span (style = "color: orange") { "Wait" } }
            Match(when' = (light () = "green")) { span (style = "color: green") { "Go" } }
        }
    }
```

`Match<'T>` takes a child function like `Show<'T>`, and has a `when'option` setter for option values.
`Match.Keyed` and `Match.NonKeyed` mirror the `Show` variants.

:::warning
`Match.Keyed(when'option = ...)` loses its condition in the output at the moment. Use
`Match.Keyed(when' = ...)` or a plain `Match`.
:::

### Errored

`Errored` replaces `ErrorBoundary`. When something inside it throws, it renders `fallback` instead.
The fallback is either an element, or an `ErrorBoundary.Fallback` delegate that gets the error (as an
`Accessor<obj>`) and a `reset` function.

```fsharp
type ErrorBoundary.Fallback = delegate of err: Accessor<obj> * reset: (unit -> unit) -> HtmlElement
```

```fsharp solid render=ErroredDemo
[<SolidComponent>]
let Fragile (value: Accessor<int>) =
    let checkedValue =
        createMemo (fun (_: int option) ->
            let v = value ()

            if v > 2 then
                failwith ("Too big: " + string v)

            v)

    span () { checkedValue () }

[<SolidComponent>]
let ErroredDemo () =
    let value, setValue = createSignal 0

    div () {
        button (onClick = fun _ -> setValue (value () + 1)) { "Increase" }

        Errored(
            fallback =
                !^(ErrorBoundary.Fallback(fun err reset ->
                    div () {
                        span () { (err () :?> exn).Message }

                        button (
                            onClick =
                                fun _ ->
                                    setValue 0
                                    reset ()
                        ) {
                            "Reset"
                        }
                    }))
        ) {
            Fragile value
        }
    }
```

Press **Increase** until the value passes 2. The memo throws, and the boundary shows the message.
**Reset** sets the value back and re-renders the children.

:::warning
`Errored` also has `fallbackFn` and `fallbackEle` setters, meant as typed shortcuts for the two
kinds of fallback. They are emitted as literal `fallbackFn=` and `fallbackEle=` props, which
`Errored` ignores. Set `fallback` with `!^` as above until this is fixed.
:::

### Loading

`Loading` replaces `Suspense`. While any async read inside it is pending, it shows `fallback`. Once
everything has settled the first time, later changes keep the old content on screen until the new
data arrives. Use [`isPending`](#ispending-and-latest) to show that a refresh is in progress.

```fsharp solid render=GreetingLoader
[<SolidComponent>]
let GreetingLoader () =
    let name, setName = createSignal "Ada"

    let greeting =
        createMemo (fun (_: string option) -> resolveAfter 800 ("Hello, " + name ()))

    let stale =
        createMemo (fun (_: bool option) -> isPending (fun () -> box (greeting ())))

    div () {
        button (onClick = fun _ -> setName "Ada") { "Ada" }
        button (onClick = fun _ -> setName "Grace") { "Grace" }

        Loading(fallback = p () { "Loading..." }) {
            p (style = (if stale () then "opacity: 0.5" else "")) { greeting () }
        }
    }
```

The first load shows the fallback. After that, switching names dims the old greeting until the new
one arrives.

Put `Loading` around the part that depends on the data, not around the whole page. Anything inside
it is replaced by the fallback on the first load.

`on` limits the boundary to one source. When `on` is set, the boundary only shows its fallback for
changes caused by writes to that source. Other changes keep the old content.

```fsharp
Loading(fallback = Skeleton (), on = route ()) { Page () }
```

### Repeat

`Repeat` renders its child function `count` times, passing the index. It needs no array. `from`
sets the first index, and `fallback` shows when `count` is 0.

```fsharp solid render=RepeatDemo
[<SolidComponent>]
let RepeatDemo () =
    let count, setCount = createSignal 3

    div () {
        button (onClick = fun _ -> setCount (count () + 1)) { "More" }
        button (onClick = fun _ -> setCount (max 0 (count () - 1))) { "Fewer" }

        p () {
            Repeat(count = count (), fallback = em () { "No stars" }) {
                yield fun i -> b (title = string i) { "*" }
            }
        }
    }
```

### Reveal

`Reveal` coordinates when sibling `Loading` boundaries show their content. It replaces
`SuspenseList`.

| `order` | Behaviour |
| --- | --- |
| `Reveal.Order.Sequential` (default) | Boundaries reveal in order. A later one waits for the ones before it. |
| `Reveal.Order.Together` | Nothing reveals until every boundary is ready, then all reveal at once. |
| `Reveal.Order.Natural` | Each boundary reveals when its own data is ready. Useful when nested in another `Reveal`. |

`collapsed = true` hides the fallbacks of boundaries that are still waiting their turn. It only
applies to `Sequential`.

```fsharp
Reveal(order = Reveal.Order.Sequential) {
    Loading(fallback = span () { "..." }) { Profile () }
    Loading(fallback = span () { "..." }) { Posts () }
}
```

### Hydration and NoHydration

`Hydration(id = ...)` and `NoHydration()` are unchanged from Solid 1. `NoHydration` renders its
children on the server and skips them when hydrating.

### Portal and Dynamic

`Portal` and `Dynamic` are part of `@solidjs/web`, so they are in `Partas.Solid.Web`. `Portal(mount =
element) { ... }` renders its children into another part of the document. `Dynamic` renders a
component chosen at run time.

:::warning
`Dynamic` has open bugs: `componentAsString = ...` is dropped from the output, and children passed in
braces are not emitted correctly. Check the generated JSX when you use it.
:::

## Context

```fsharp
type Context<'T> = 'T -> ContextProvider
```

`createContext` makes a context, optionally with a default value. `useContext` reads the nearest
value. `tryUseContext` returns a `Result<'T, ContextNotFoundError>` instead of throwing when there is
no provider and no default.

```fsharp
let ThemeContext = createContext<string> "light"

[<SolidComponent>]
let ThemedLabel () =
    let theme = useContext ThemeContext
    span () { theme }

[<SolidComponent>]
let ThemeApp () =
    div () {
        ThemedLabel ()          // "light", the default

        ThemeContext "dark" {   // provides "dark" to its children
            ThemedLabel ()
        }
    }
```

To share state, put accessors and functions in the context value, for example a `[<JS.Pojo>]` type
with a `count: Accessor<int>` and an `increment: unit -> unit`.

:::danger
Providing a value does not work yet. The plugin emits `<ThemeContext.Provider value=...>`, but a
Solid 2 context is itself the provider component, and has no `.Provider`. Consumers only ever see
the default value. `createContext` and `useContext` are fine. Only the provider syntax is broken.
:::

## Async

In Solid 2, async is built in. An async memo, an async derived store and a `lazy'` component all
suspend their readers, and `Loading` catches them. These functions help around that.

### isPending and latest

`isPending (fun () -> box (source ()))` is `true` while the source is refreshing after it first
settled. It does not trigger `Loading`, so you can use it to dim old content, as in the
[`Loading` example](#loading).

`latest (fun () -> source ())` reads the most recent value, even while a newer one is pending.

:::warning
Put `isPending` in a memo, as in the example above, and read the memo in JSX. Written inline in an
attribute or child, the plugin strips the `fun () -> ...` wrapper, and `isPending` receives a value
instead of a function.
:::

### refresh

`refresh target` re-runs an async memo or derived store, and returns a promise of the new value. The
target is an `Accessor<'T>` (a memo) or a `Refreshable<'T>`.

```fsharp
button (onClick = fun _ -> refresh user |> ignore) { "Reload" }
```

### until

`until (fun () -> condition)` returns a promise that resolves once the condition is truthy.
`timeout` rejects it with a `TimeoutError` after that many milliseconds, and `signal` takes an
`AbortSignal`. Do not call it inside a tracking scope.

```fsharp
promise {
    let! _ = until ((fun () -> count () >= 3), timeout = 5000)
    console.log "reached 3"
}
```

### action

`action` wraps a JS generator function in a transition. Each `yield` waits on a promise. Writes to
optimistic state inside it show immediately, and are replaced by the real values when it finishes.
It returns a function that returns a promise.

```fsharp
action (genFn: 'Args -> 'Gen) : 'Args -> JS.Promise<'R>
```

F# has no generator syntax, so the generator must come from JS or be written by hand as an object
with `next` and `throw`. This makes `action` awkward to use from F# today.

### createOptimistic and createOptimisticStore

`createOptimistic` makes a signal whose writes are temporary. Inside an `action`, a write shows at
once and is reverted when the action ends, unless the real source has changed to match.
`createOptimistic<'T> (fun () -> source ())` tracks a source. The type argument is required.

```fsharp
let likes, setLikes = createSignal 10
let shownLikes, setShownLikes = createOptimistic<int> (fun () -> likes ())
```

`createOptimisticStore` is the store version, with the same overloads as `createStore`.

### affects and resolve

- `affects source` tells Solid that the current action or effect writes to `source`, so readers show
  as pending. For a store, `affects (store, "key")` or `affects (store, fun s -> s.key)` names one
  property.
- `resolve (fun () -> ...)` returns a promise of the function's result once everything it reads has
  settled.

## Owners and roots

| Function | Use |
| --- | --- |
| `createRoot (fun dispose -> ...)` | Makes an owner that is not disposed with its parent. Call `dispose` to clean it up. `createRoot (fun () -> ...)` works when you do not need it. |
| `getOwner ()` | The current owner, as an `Owner option`. |
| `runWithOwner (owner, fun () -> ...)` | Runs the function under that owner, for example after an `await`. |
| `createOwner ()` | Makes a new owner under the current one. |
| `isDisposed owner` | Whether the owner has been disposed. |
| `getObserver ()` | The current tracking scope, if any. It was `getListener` in Solid 1. |

## Utilities

### children

`children (fun () -> props.children)` resolves a component's children once and memoises them. It
returns a `ChildrenReturn`: call `.Invoke ()` to render them, or `.toArray ()` to inspect them.

```fsharp
let resolved = children (fun () -> props.children)
let hasChildren = fun () -> resolved.toArray().Length > 0
```

### merge and omit

`merge (a, b, ...)` combines prop objects, with later sources winning. `omit (props, "a", "b")`
returns the props without those keys. They replace `mergeProps` and `splitProps`. You rarely call
them yourself, because the plugin generates them for `[<SolidTypeComponent>]` members. See
[SolidTypeComponent](../guide/solid-type-attribute.md).

### lazy'

```fsharp
lazy' (fn: unit -> JS.Promise<'T>, ?options: LazyOptions, ?moduleUrl: string) : LazyComponent<'T>
```

`lazy'` loads a component on first render. `LazyOptions (export = "Name")` picks a named export
instead of the default. `.preload ()` starts loading early. Rendering it before it loads suspends,
like an async memo.

```fsharp
let Settings = lazy' (fun () -> importDynamic "./Settings.fs.jsx")
```

`importComponent` is gone. Use Fable's `importDynamic`.

### createUniqueId

`createUniqueId ()` returns an id that matches between server and client, for `id`/`for` pairs.

### mapArray and repeat

These are the functions behind `For` and `Repeat`, for use outside JSX. Each returns an accessor of
the mapped array.

| Function | Map function |
| --- | --- |
| `mapArray (list, fun item index -> ...)` or `mapArrayKeyed` | `item: 'Item`, `index: Accessor<int>` |
| `mapArrayUnkeyed (list, fun item index -> ...)` | `item: Accessor<'Item>`, `index: int` |
| `mapArrayKeyedFn (list, (fun item index -> ...), keyed)` | `item: Accessor<'Item>`, `index: Accessor<int>` |
| `repeat (count, fun i -> ...)` | `i: int` |

The map functions are curried F# lambdas (`fun item index -> ...`), compiled to a two-argument JS
function. `mapArray` replaces both `mapArray` and `indexArray` from Solid 1.

### Boundaries as functions

`createErrorBoundary`, `createLoadingBoundary` and `createRevealOrder` are what `Errored`, `Loading`
and `Reveal` are built on. Use them to build your own boundary components.

```fsharp
let content =
    createErrorBoundary ((fun () -> riskyView ()), fun err reset -> errorView (err ()) reset)
```

### Directives

Solid 2 has no `use:` directives. `createDirectiveFactory` turns a function of the element into a
ref, which you attach with `.ref`. The function runs once the element has settled, and returns a
cleanup (`ignore` when there is nothing to clean up).

```fsharp
let autofocus = createDirectiveFactory (fun (el: Browser.Types.HTMLElement) ->
    el.focus ()
    ignore)

input().ref autofocus
```

## Removed APIs

These Solid 1 APIs are gone from Solid 2, and from the bindings. Use the replacement instead.

| Removed | Replacement |
| --- | --- |
| `createResource`, `SolidResource`, `ResourceFetcher`, ... | An async `createMemo`, read under `Loading`. `refresh` re-runs it. |
| `Suspense` | `Loading` |
| `SuspenseList` | `Reveal`, or `createRevealOrder` |
| `ErrorBoundary` | `Errored` |
| `catchError` | `createErrorBoundary`, or an error handler on `createEffect` |
| `Index` | `For.NonKeyed` |
| `indexArray` | `mapArrayUnkeyed` |
| `For` with a single child signature | `For.Keyed`, `For.NonKeyed` or `For.KeyedFn` |
| `onMount` | `onSettled` |
| `createEffect (fun () -> ...)` | `createEffect (compute, effect)`, or `createTrackedEffect` |
| `createComputed` | A memo, or a writable derived signal (`createSignal (fun () -> ...)`) |
| `createDeferred` | None. Derive the value outside the reactive graph. |
| `createSelector` | `createProjection` |
| `batch` | `flush`. Writes are always batched. |
| `startTransition`, `useTransition` | `action`, with `isPending` for the pending state |
| `on` | The compute half of `createEffect` names the sources |
| `solid-js/store` | `createStore` from `solid-js` |
| `produce` | The store setter itself: mutate the draft and return it |
| `unwrap` | `snapshot` |
| `SolidStoreSetter`, `SolidStorePath` | `StoreSetter<'T>`, an updater |
| `mergeProps`, `splitProps` | `merge`, `omit` |
| `importComponent` | `importDynamic` with `lazy'` |
| `getListener` | `getObserver` |
| `render`, `renderToString`, `isServer` on `Partas.Solid` | The same names in `Partas.Solid.Web` |

For the full list of what changed in Partas.Solid itself, see
[Migrating to Solid 2](../guide/migrating-to-solid-2.md).
