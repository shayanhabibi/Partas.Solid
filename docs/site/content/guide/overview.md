---
title: Overview
---

:::info
These pages cover how Partas.Solid maps F# onto Solid. They do not teach Solid itself. For signals, effects, stores
and the reactive model, read the [Solid documentation](https://docs.solidjs.com/).
:::

In Solid, a component is a function that runs once and returns DOM. Partas.Solid gives you two ways to write that
function in F#:

1. [`[<SolidComponent>]`](#solidcomponent) on a `let` binding.
2. [`[<SolidTypeComponent>]`](#solidtypecomponent) on a member of a type. The type's properties become the
   component's props.

Both tell the Fable plugin to rewrite the body into JSX. Without one, the DSL compiles to nothing useful.

## SolidComponent

`[<SolidComponent>]` works on any `let` binding. Its parameters stay ordinary F# parameters.

```fsharp solid render=ButtonExample jsx
[<StringEnum>]
type ButtonVariant =
    | Primary
    | Ghost

[<SolidComponent>]
let Button (title: string) (variant: ButtonVariant) =
    button (
        style =
            (match variant with
             | Primary -> "background: #2563eb; color: white; margin-right: .5em"
             | Ghost -> "background: transparent; margin-right: .5em")
    ) {
        title
    }

[<SolidComponent>]
let ButtonExample () =
    let variant, setVariant = createSignal Ghost

    div () {
        Button "Primary" Primary
        Button "Ghost" Ghost
        Button "Responsive" (variant ())
        button (onClick = fun _ -> setVariant (if variant () = Primary then Ghost else Primary)) {
            "Click me"
        }
    }
```

In the JSX tab, a `[<SolidComponent>]` with parameters is called as a plain function, `Button("Primary", "primary")`,
not used as a `<Button />` tag. That is fine for small helpers. For a component other code will use, reach for
`[<SolidTypeComponent>]`: it becomes a real tag with named props, for F# and JavaScript callers alike. See
[SolidComponent](solid-component-attribute.md) for the details.

## SolidTypeComponent

This attribute defines a custom tag: a type whose properties are the props, with the body in a member marked
`[<SolidTypeComponent>]`.

```fsharp solid render=GreetingExample jsx
[<Erase>]
type Greeting() =
    inherit div()

    [<Erase>]
    member val name: string = unbox null with get, set

    [<Erase>]
    member val loud: bool = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        props.name <- "world"

        div(style = (if props.loud then "font-weight: bold" else "")).spread props {
            $"Hello, {props.name}!"
        }

[<SolidComponent>]
let GreetingExample () =
    let loud, setLoud = createSignal false

    div () {
        Greeting()
        Greeting(name = "Partas", loud = loud ())
        button (onClick = fun _ -> setLoud (not (loud ()))) { "Toggle loud" }
    }
```

- `inherit div()` gives `Greeting` every attribute a `div` has, as well as its own `name` and `loud`.
- `props.name <- "world"` sets a default. The plugin turns assignments to props into `props = merge({...}, props)`.
- `.spread props` passes on every prop the body does not read. The plugin collects the reads into
  `const PARTAS_OTHERS = omit(props, ...)` and spreads that.
- Prop reads stay as `props.x` in the JSX, so they stay reactive.
- The member becomes a function named after the type. The member name, here `View`, does not matter.

`Greeting` is now used like any built-in tag. The [JSX output](../about/jsx-output.md) page shows the exact shapes.

:::note
The type must live in a namespace or module that starts with `Partas.Solid`. The plugin only transforms types under
that prefix. Put your components in something like `namespace Partas.Solid.MyApp`.
:::

A type without a `[<SolidTypeComponent>]` member describes a component that already exists in JavaScript. Import it
with `[<Import>]` and declare its properties:

```fsharp
namespace Partas.Solid.MyBindings

open Fable.Core
open Partas.Solid

[<Import("Tooltip", "some-js-library")>]
[<Erase>]
type Tooltip() =
    inherit div()

    [<Erase>]
    member val placement: string = unbox null with get, set
```

```fsharp
Tooltip(placement = "top", class' = "tip") { "More info" }
```

See [SolidTypeComponent](solid-type-attribute.md) for the rules the member must follow, and
[tag interfaces](tag-interfaces.md) for the interfaces a custom tag can implement instead of inheriting.
