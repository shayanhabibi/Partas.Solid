---
title: Autofocus
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/autofocus`.

```fsharp
let createAutofocus(ref: unit -> #HtmlElement): unit
```

Pass an accessor that returns the element to focus: one that reads a captured `ref`, or a signal that the element's
`ref` sets. Upstream usage:

```jsx
let ref;
createAutofocus(() => ref);
<button ref={ref}>Autofocused</button>;

const [ref, setRef] = createSignal();
createAutofocus(ref);
<button ref={setRef}>Autofocused</button>;
```
