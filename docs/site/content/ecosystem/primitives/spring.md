---
title: Spring
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/spring`.

The package also provides an extension for `float` setters:

```fsharp
open Partas.Solid.Primitives.Spring.SetterExtensions
```

```fsharp
[<Extension>]
static member inline Invoke(
    setter: Setter<float>,
    newValue: float,
    options: StringSetterOptions
    ) = ...
```

:::caution
The return type of this extension is not documented, and the binding needs review.
:::

## createSpring

```fsharp
let createSpring(
    initialValue: 'T when 'T :> (float | float seq | Accessor<float>),
    ?stiffness: float,
    ?damping: float
    ): Signal<'T>
```

| Param | Desc |
| --- | --- |
| `initialValue` | The initial value of the signal. |
| `stiffness` | Configures the physics of the spring. |
| `damping` | Configures the physics of the spring. |

Creates a signal that uses spring physics to move from one value to the next.

When the value changes, it does not move at a steady rate. It bounces like a spring, depending on the physics
parameters. This makes transitions feel more natural.

It works best for types that can be interpolated, such as numbers, `Date` and arrays.

## createDerivedSpring

```fsharp
let createDerivedSpring(
    target: Accessor<'T when 'T :> (float | float seq)>,
    ?stiffness: float,
    ?damping: float
    ): Accessor<'T>
```

Creates a spring that follows changes to the signal you pass in. It works like [`createTween`](tween.md).
