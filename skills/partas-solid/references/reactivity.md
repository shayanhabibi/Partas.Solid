# Reactivity and control flow

## Tracked reads and writes

A component body runs once. Read an accessor or prop in JSX to keep the expression live. For a derived value
used several places, use an accessor function or a memo rather than copying a reactive value into a body-local
variable. Control-flow function children also build structure; keep their changing accessor reads in JSX.

```fsharp
let count, setCount = createSignal 0
let doubled () = count () * 2
let cached = createMemo (fun (_: int option) -> count () * 2)

button (onClick = fun _ -> setCount.Invoke(fun previous -> previous + 1)) {
    $"{count ()}, {doubled ()}, {cached ()}"
}
```

The signal setter takes a value; use `.Invoke(fun previous -> ...)` for an updater function.
Annotate a memo's previous-value parameter as an option to resolve its overload. A function passed to
`createSignal` is a derived computation; storing a function value needs a wrapper that returns the function.

Signal writes are batched to a microtask. Use `flush ()` only when imperative code or a test needs the update
applied immediately. Write from event handlers or the appropriate effect side-effect callback, rather than
from memo computations or JSX evaluation.

## Pattern selection with match!

In an element builder, `match! source () with ...` generates a Solid `Switch` and non-keyed `Match` arms.
It supports F# patterns and guards; the first matching arm wins. Same-arm payload changes retain the arm's DOM
and update pattern-bound reads in JSX and event callbacks. Selecting another arm replaces its content.
The source must supply reactive dependencies; a static value remains static.

The plugin shares source evaluation, guards, and captures in one lazy selection memo before the surrounding
JSX. Use the syntax directly; the application does not need a second selection memo or manual Match wrappers.
An ordinary `match` keeps expression-based output. Element-builder `match!` is synchronous, not async binding.
See the [complete example](components.md) and the [match! guide](https://shayanhabibi.github.io/Partas.Solid/guide/match-expression/).

## Lists and function children

Pass arrays to `each` and explicitly `yield` the function child:

```fsharp
For.Keyed(each = items ()) {
    yield fun item index -> li () { $"{index ()}: {item}" }
}
```

- `For.Keyed`: item is the raw value, index is an accessor. Rows are keyed by item identity. Preserve object
  identity across reorders when retaining rows is intended.
- `For.NonKeyed`: item is an accessor, index is a plain integer. Rows are retained by position; this replaces
  Solid 1's `Index`.
- `For.KeyedFn`: item and index are accessors; supply `keyed = fun item -> item.id` to retain rows by an explicit
  key when item objects are replaced.
- `Repeat(count = n) { yield fun index -> ... }`: index is a plain integer.

An ordinary HTML element builder does not provide direct `for`/`while` builder operations or collection yielding.
Use these flow components for UI iteration. Array mapping outside the element builder remains ordinary F#.

`Show(when' = condition (), fallback = ...)` accepts element children. For function children, `Show.Keyed`
passes a raw value and recreates on value changes; `Show.NonKeyed` passes an accessor and retains content while
the condition stays truthy. Manual `Match.Keyed` and `Match.NonKeyed` use the same raw-value/accessor distinction.
Write `yield fun value -> ...` in each function-child form.

## Effects, async values, and stores

Solid 2 effects split tracked computation from side effects:

```fsharp
createEffect (
    (fun (_: int option) -> count ()),
    fun (value: int) -> JS.console.log value
)
```

The effect callback can return a cleanup function, which runs before the next effect and on disposal.
`onSettled` is useful for attaching DOM integrations after refs are populated; return their teardown callback.

For async data, use a promise-returning memo with a typed previous-value parameter. Its accessor exposes the
resolved value; read it inside `Loading` so pending reads show the fallback. Use `Errored` for error boundaries
and `Reveal` for coordination. Solid 1's `createResource`, `Suspense`, `ErrorBoundary`, and `SuspenseList` recipes
need the corresponding Solid 2 APIs rather than copied imports.

Stores come from `solid-js` through `open Partas.Solid`, rather than `solid-js/store`.
Read `store.Value.property`. The store setter takes an updater function that returns the mutated draft or a
replacement object; it does not take a plain replacement value. Numeric arrays need Fable's `--typedArrays false`
for per-item store reactivity.

Sources: [Solid API bindings](https://shayanhabibi.github.io/Partas.Solid/guide/solid-js/),
[building the DOM](https://shayanhabibi.github.io/Partas.Solid/guide/building-the-dom/).
