---
title: Active Element
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/active-element`.

```fsharp
let makeActiveElementListener(handler: #HtmlElement -> unit): DisposeCallback
let makeFocusListener(target: #HtmlElement, callBack: bool -> unit, ?useCapture: bool): DisposeCallback
let createActiveElement(): Accessor<#HtmlElement>
let createFocusSignal(target: #HtmlElement | Accessor<HtmlElement>): Accessor<bool>
```

| Member | Reactive | Returns |
| --- | --- | --- |
| `makeActiveElementListener` | No | A `DisposeCallback` that removes the `document.activeElement` listener |
| `makeFocusListener` | No | A `DisposeCallback` that removes the `blur` and `focus` listeners |
| `createActiveElement` | Yes | The currently focused element |
| `createFocusSignal` | Yes | Whether the target element has focus |
