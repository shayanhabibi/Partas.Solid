---
title: Scheduled
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/scheduled`.

```fsharp
type Schedule<'T> =
    member exec: 'T -> unit      // schedule(x) in JavaScript
    member clear: unit -> unit

let debounce(callback: 'T -> unit, timespan: int): Schedule<'T>
let throttle(callback: 'T -> unit, timespan: int): Schedule<'T>
let scheduleIdle(callback: 'T -> unit, timespan: int): Schedule<'T>

type DebounceOrThrottle<'T> = ('T -> unit) * int -> Schedule<'T>
let leading(debOrThrot: DebounceOrThrottle<'T>, callback: 'T -> unit, timespan: unit): Schedule<'T>
let leadingAndTrailing(debOrThrot: DebounceOrThrottle<'T>, callback: 'T -> unit, timespan: int): Schedule<'T>
```

:::caution
`leading`'s `timespan` is typed `unit`; it should be `int`. `createScheduled` is not bound.
:::

## On Solid 2 today

`debounce` delays a call until `timespan` ms pass without another one, so a burst of calls runs once. The same thing
in plain Partas.Solid, with `JS.setTimeout`; type quickly and the runs counter stays behind the keystrokes:

```fsharp solid render=DebounceDemo jsx
[<SolidComponent>]
let DebounceDemo () =
    let typed, setTyped = createSignal ""
    let settled, setSettled = createSignal ""
    let keys, setKeys = createSignal 0
    let runs, setRuns = createSignal 0
    let mutable timer = 0

    let debounced (value: string) =
        JS.clearTimeout timer
        timer <- JS.setTimeout (fun () -> setSettled value; setRuns (runs () + 1)) 400

    onCleanup (fun () -> JS.clearTimeout timer)

    div (style = "display: grid; gap: .5rem; width: min(100%, 24rem)") {
        input (
            placeholder = "Type here",
            onInput = fun e ->
                let value: string = !!e.currentTarget?value
                setTyped value
                setKeys (keys () + 1)
                debounced value
        )
        code () { $"typed:     {typed ()} ({keys ()} inputs)" }
        code () { $"debounced: {settled ()} ({runs ()} runs)" }
    }
```
