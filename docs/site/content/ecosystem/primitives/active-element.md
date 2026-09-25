---
title: Active Element
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/active-element`.

```fsharp
[<Erase; AutoOpen>]
type ActiveElement =
    /// <summary>
    /// Listen for changes to the <c>document.activeElement</c>
    /// </summary>
    /// <remarks>
    /// non reactive
    /// </remarks>
    [<ImportMember(path)>]
    static member makeActiveElementListener (handler: #HtmlElement -> unit): DisposeCallback = jsNative

    /// <summary>
    /// Attaches "blur" and "focus" event listeners to the element
    /// </summary>
    [<ImportMember(path)>]
    static member makeFocusListener (target: #HtmlElement, callBack: bool -> unit, ?useCapture: bool): DisposeCallback = jsNative

    /// <summary>
    /// Provides a reactive signal of <c>document.activeElement</c>. Check which element is currently focused.
    /// </summary>
    [<ImportMember(path)>]
    static member createActiveElement (): Accessor<#HtmlElement> = jsNative

    /// <summary>
    /// Provides a signal representing element's focus state
    /// </summary>
    [<ImportMember(path)>]
    static member createFocusSignal(target: #HtmlElement): Accessor<bool> = jsNative

    /// <summary>
    /// Provides a signal representing element's focus state
    /// </summary>
    [<ImportMember(path)>]
    static member createFocusSignal(target: Accessor<HtmlElement>): Accessor<bool> = jsNative
```

| Member | Reactive | Returns |
| --- | --- | --- |
| `makeActiveElementListener` | No | A `DisposeCallback` that removes the listener |
| `makeFocusListener` | No | A `DisposeCallback` that removes the `blur` and `focus` listeners |
| `createActiveElement` | Yes | The currently focused element |
| `createFocusSignal` | Yes | Whether the target element has focus |
