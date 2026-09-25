---
title: Autofocus
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/autofocus`.

## createAutofocus

```fsharp
/// <summary>
/// Reactively autofocuses an element passed in as a signal
/// </summary>
[<ImportMember(path)>]
static member createAutofocus (ref: unit -> #HtmlElement): unit = jsNative
```

Pass an accessor that returns the element to focus. It can read a `ref` you captured, or a signal that the element's
`ref` sets.

The upstream JavaScript usage:

```jsx
import { createAutofocus } from "@solid-primitives/autofocus";

// Using ref
let ref;
createAutofocus(() => ref);

<button ref={ref}>Autofocused</button>;

// Using ref signal
const [ref, setRef] = createSignal();
createAutofocus(ref);

<button ref={setRef}>Autofocused</button>;
```
