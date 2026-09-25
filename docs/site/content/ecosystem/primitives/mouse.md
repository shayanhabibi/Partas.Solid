---
title: Mouse
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/mouse`.

:::caution
The position and size properties below are typed `int`. They should be `float`.
:::

## MouseSourceType

Type: `StringEnum`

- `Mouse`
- `Touch`

## Position

Type: `POJO`

- x: `int`
- y: `int`

## MousePosition

Type: `POJO`

- x: `int`
- y: `int`
- sourceType: `MouseSourceType option`

## MousePositionInside

Type: `POJO`

- x: `int`
- y: `int`
- sourceType: `MouseSourceType`
- isInside: `bool`

## PositionRelativeToElement

Type: `POJO`

- x: `int`
- y: `int`
- isInside: `bool`
- top: `int`
- left: `int`
- width: `int`
- height: `int`

## Bindings

| Member | Desc |
| --- | --- |
| `makeMousePositionListener` | Listens for changes to the mouse or touch position on the target. Returns a function that removes the listeners. |
| `makeMouseInsideListener` | Listens for the mouse or touch entering or leaving the target. Returns a function that removes the listeners. |
| `getPositionToElement` | Turns a position relative to the page into a position relative to an element. |
| `getPositionInElement` | Like `getPositionToElement`, but clamped to the element's bounds. |
| `getPositionToScreen` | Turns a position relative to the page into a position relative to the screen. |
| `createMousePosition` | A reactive object with the current mouse position on the page. The target defaults to `window`, and can be an accessor. |
| `useMousePosition` | `createMousePosition` on `window`, as a singleton root shared by every caller. |
| `createPositionToElement` | A position relative to an element, updated from a reactive page position. Also returns the element's current bounds. |

Common parameters:

| Param | Desc |
| --- | --- |
| `target` | `SVGSVGElement`, `HTMLElement`, `Window` or `Document`. |
| `touch` | Listen to touch events. If on, the position updates on `touchstart`. |
| `followTouch` | If on, the position updates on `touchmove`. |
| `initialValues` | The initial values. |
| `pos` | For `createPositionToElement`: an accessor of the page position (relative to the page, not the window). |

:::details title="Bindings"
```fsharp
[<Erase; AutoOpen>]
type Mouse =
    [<Import("makeMousePositionListener", path); ParamObject(2)>]
    static member makeMousePositionListener (target: U4<HtmlElement, Element, Document, Window>, callback: (MousePosition -> unit), ?touch: bool, ?followTouch: bool) : DisposeCallback = nativeOnly

    [<Import("makeMouseInsideListener", path); ParamObject(2)>]
    static member makeMouseInsideListener (target: U4<HtmlElement, Element, Document, Window>, callback: (bool -> unit), ?touch: bool) : DisposeCallback = nativeOnly

    [<Import("getPositionToElement", path)>]
    static member getPositionToElement(pageX: int, pageY: int, el: U4<HtmlElement, Element, Document, Window>): PositionRelativeToElement = nativeOnly

    [<Import("getPositionInElement", path)>]
    static member getPositionInElement(pageX: int, pageY: int, el: U4<HtmlElement, Element, Document, Window>): PositionRelativeToElement = nativeOnly

    [<Import("getPositionToScreen", path)>]
    static member getPositionToScreen(pageX: int, pageY: int): Position = nativeOnly

    [<Import("createMousePosition", path)>]
    static member createMousePosition (?target: U4<HtmlElement, Element, Document, Window>, ?initialValues: MousePositionInside, ?touch: bool, ?followTouch: bool) : MousePositionInside = nativeOnly

    [<Import("createMousePosition", path); ParamObject(1)>]
    static member createMousePosition (?target: U4<Accessor<HtmlElement>, Accessor<Element>, Accessor<Document>, Accessor<Window>>, ?initialValues: MousePositionInside, ?touch: bool, ?followTouch: bool) : MousePositionInside = nativeOnly

    [<Import("useMousePosition", path)>]
    static member inline useMousePosition: (unit -> MousePositionInside) = nativeOnly

    [<Import("createPositionToElement", path); ParamObject(2)>]
    static member createPositionToElement (element: U4<HtmlElement, Element, Accessor<HtmlElement>, Accessor<Element>>, pos: Accessor<Position>, ?initialValues: PositionRelativeToElement, ?touch: bool, ?followTouch: bool) : PositionRelativeToElement = nativeOnly
```
:::

:::caution
The first `createMousePosition` overload has no `ParamObject`, so `touch` and `followTouch` are passed as positional
arguments rather than as an options object.
:::
