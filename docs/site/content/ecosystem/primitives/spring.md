---
title: Spring
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/spring`.

```fsharp
let createSpring(
    initialValue: 'T when 'T :> (float | float seq | Accessor<float>),
    ?stiffness: float,
    ?damping: float
    ): Signal<'T>

let createDerivedSpring(
    target: Accessor<'T when 'T :> (float | float seq)>,
    ?stiffness: float,
    ?damping: float
    ): Accessor<'T>
```

`createSpring` is a signal that moves to each new value with spring physics instead of at a steady rate; `stiffness`
and `damping` configure the physics. It works best for values that can be interpolated, such as numbers, `Date` and
arrays. `createDerivedSpring` follows `target`, like [`createTween`](tween.md).

The package also adds an `Invoke(newValue: float, options: StringSetterOptions)` extension to `Setter<float>`, under
`open Partas.Solid.Primitives.Spring.SetterExtensions`. Its return type is not documented, and it needs review.
