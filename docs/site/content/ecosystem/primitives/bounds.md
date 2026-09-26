---
title: Bounds
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/bounds`.

## ElementBounds

Type: `interface`

- width: `int`
- height: `int`
- top: `int`
- left: `int`
- right: `int`
- bottom: `int`

## Bindings

`createElementBounds` returns a reactive, store-like object with the element's position on the screen and its size.
It updates on scroll, on resize, and when the DOM changes.

Each kind of tracking is on by default. Turning all three on can be more than you need, so set the tracking parameters
to `false` and turn on only the ones you use.

| Parameter | Listens to |
| --- | --- |
| `trackScroll` | Window scroll events |
| `trackMutation` | Changes to the DOM structure and styles |
| `trackResize` | The element's resize events |

:::details Bindings
```fsharp
[<Erase; AutoOpen>]
type Bounds =
    /// <summary>
    /// Creates a reactive store-like object of current element bounds — position on the screen, and size dimensions. Bounds will be automatically updated on scroll, resize events and updates to the DOM.
    /// </summary>
    /// <param name="target">Ref or reactive ref element</param>
    /// <param name="trackScroll">Listen to window scroll events</param>
    /// <param name="trackMutation">Listen to changes to the dom structure/styles</param>
    /// <param name="trackResize">Listen to changes to the element's resize events</param>
    /// <remarks>All options are 'truthy' by default</remarks>
    [<ImportMember(path); ParamObject(1)>]
    static member createElementBounds(target: #HTMLElement,
                                      ?trackScroll: bool,
                                      ?trackMutation: bool,
                                      ?trackResize: bool): ElementBounds = jsNative
    /// <summary>
    /// Creates a reactive store-like object of current element bounds — position on the screen, and size dimensions. Bounds will be automatically updated on scroll, resize events and updates to the DOM.
    /// </summary>
    /// <param name="target">Ref or reactive ref element</param>
    [<ImportMember(path)>]
    static member createElementBounds(target: #HTMLElement): ElementBounds = jsNative
    /// <summary>
    /// Creates a reactive store-like object of current element bounds — position on the screen, and size dimensions. Bounds will be automatically updated on scroll, resize events and updates to the DOM.
    /// </summary>
    /// <param name="target">Ref or reactive ref element</param>
    /// <param name="trackScroll">Listen to window scroll events</param>
    /// <param name="trackMutation">Listen to changes to the dom structure/styles</param>
    /// <param name="trackResize">Listen to changes to the element's resize events</param>
    /// <remarks>All options are 'truthy' by default</remarks>
    [<ImportMember(path); ParamObject(1)>]
    static member createElementBounds(target: Accessor<#HTMLElement>,
                                      ?trackScroll: bool,
                                      ?trackMutation: bool,
                                      ?trackResize: bool): ElementBounds = jsNative
    /// <summary>
    /// Creates a reactive store-like object of current element bounds — position on the screen, and size dimensions. Bounds will be automatically updated on scroll, resize events and updates to the DOM.
    /// </summary>
    /// <param name="target">Ref or reactive ref element</param>
    [<ImportMember(path)>]
    static member createElementBounds(target: Accessor<#HTMLElement>): ElementBounds = jsNative
```
:::
