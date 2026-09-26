---
title: NeoDrag
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

`Partas.Solid.NeoDrag` binds [@neodrag/solid](https://www.neodrag.dev/docs/solid), for drag and drop. See its docs for
usage.

Several drag and drop libraries were considered. NeoDrag was by far the simplest to bind: the others needed tens of
TypeScript definition files read to find the shape of every option and event.

The binding has not been tested much.

## Vanilla NeoDrag on Solid 2

[@neodrag/vanilla](https://www.neodrag.dev/docs/vanilla) has no framework dependency, so it works on Solid 2 today.
Its whole API is one class, `Draggable`, and binding it takes a few lines.

`[<Import>]` on a class type imports the named export, and calling the constructor emits `new Draggable(...)`. The
options are an anonymous record, which compiles to a plain object. The interface types the data NeoDrag passes to its
drag callbacks.

```fsharp solid
open Browser.Types

[<Import("Draggable", "@neodrag/vanilla")>]
type Draggable(node: HTMLElement, options: obj) =
    member _.updateOptions(options: obj) : unit = jsNative
    member _.destroy() : unit = jsNative

type DragEventData =
    abstract offsetX: float
    abstract offsetY: float
```

```fsharp solid setup
let dragArea =
    "position: relative; width: 100%; height: 14rem; overflow: hidden; border: 1px solid var(--nacara-border); border-radius: var(--nacara-radius); background: var(--nacara-bg)"

let dragCard =
    "position: absolute; top: 1rem; left: 1rem; width: 9.5rem; padding: .75rem .9rem; border: 1px solid var(--nacara-border); border-radius: var(--nacara-radius-sm); background: linear-gradient(90deg, #f28b5b, #b845fc 50%, #1d8fe0) top / 100% 2px no-repeat, var(--nacara-bg-raised); box-shadow: 0 1px 2px rgb(0 0 0 / 6%), 0 10px 28px -14px rgb(0 0 0 / 35%); cursor: grab; user-select: none"

let dragTitle = "font-weight: 600; color: var(--nacara-heading)"
let dragReadout = "font: .8125rem var(--nacara-font-mono); color: var(--nacara-text-muted)"
```

The component creates the `Draggable` in `onSettled`, once the ref is set, and returns `destroy` as the cleanup.
`onDrag` writes the offset into two signals. `bounds = "parent"` keeps the card inside its box. Passing a `position`
makes the position controlled, so Reset can move the card back with `updateOptions`. The style strings live in a
hidden setup block.

```fsharp solid render=DragDemo jsx
[<SolidComponent>]
let DragDemo () =
    let x, setX = createSignal 0
    let y, setY = createSignal 0
    let mutable card: HTMLDivElement = JS.undefined
    let mutable drag: Draggable = JS.undefined

    onSettled (fun () ->
        drag <-
            Draggable(
                card,
                {| bounds = "parent"
                   position = {| x = 0; y = 0 |}
                   onDrag =
                    fun (data: DragEventData) ->
                        setX (int data.offsetX)
                        setY (int data.offsetY) |}
            )
        fun () -> drag.destroy ())

    let reset () =
        drag.updateOptions {| position = {| x = 0; y = 0 |} |}
        setX 0
        setY 0

    div (style = "display: grid; gap: .75rem; justify-items: start; width: 100%") {
        div (style = dragArea) {
            div(style = dragCard).ref (card) {
                div (style = dragTitle) { "Drag me" }
                div (style = dragReadout) { $"x {x ()} · y {y ()}" }
            }
        }
        button (class' = "p-btn p-btn--secondary", onClick = fun _ -> reset ()) { "Reset" }
    }
```
