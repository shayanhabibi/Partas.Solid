---
title: Keyboard
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/keyboard`.

```fsharp
let useKeyDownEvent(): Accessor<KeyboardEvent>       // the last keydown event
let useKeyDownList(): Accessor<string[]>             // the keys held now, in press order
let useCurrentlyHeldKey(): Accessor<string | null>   // the single held key, or null for none or several
let useKeyDownSequence(): Accessor<string[][]>       // the held-key lists since all keys were last released
let createKeyHold(key: string, ?preventDefault: bool): Accessor<bool>
let createShortcut(keys: string[], handler: unit -> unit, ?preventDefault: bool, ?requireReset: bool): unit
```

`createShortcut` fires when `keys` are pressed in the order given. The `use` primitives listen on `window` and are
shared by every caller.

## On Solid 2 today

`useKeyDownList` is a signal of the held keys, updated from `keydown` and `keyup`. Here it is in plain Partas.Solid,
scoped to one element; the handlers are typed `Dom.KeyboardEvent -> unit`. Click the box and hold a few keys:

```fsharp solid render=HeldKeys jsx
[<SolidComponent>]
let HeldKeys () =
    let held, setHeld = createSignal ([||]: string array)

    div (
        tabindex = 0,
        style = "display: flex; gap: .4rem; min-height: 3rem; padding: .75rem; border: 1px dashed var(--nacara-border); border-radius: var(--nacara-radius-sm)",
        onKeyDown = (fun e ->
            e.preventDefault ()
            if not (Array.contains e.key (held ())) then setHeld (Array.append (held ()) [| e.key |])),
        onKeyUp = (fun e -> setHeld (held () |> Array.filter (fun k -> k <> e.key))),
        onBlur = fun _ -> setHeld [||]
    ) {
        Show(when' = ((held ()).Length > 0), fallback = span () { "Click here, then hold some keys" }) {
            For.Keyed(each = held ()) { yield fun key _ -> kbd () { if key = " " then "Space" else key } }
        }
    }
```
