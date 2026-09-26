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

Both tell the Partas.Solid Fable plugin to rewrite the body into JSX. Without one of them the plugin leaves the code
alone, and the DSL compiles to nothing useful.

## SolidComponent

`[<SolidComponent>]` works on any `let` binding. The binding's parameters stay ordinary F# parameters.

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

Click the last button and the "Responsive" button switches variant.

Look at the JSX tab. A `[<SolidComponent>]` with parameters is called as a plain function, `Button("Primary",
"primary")`, not used as a `<Button />` tag. That is fine for small helpers. For a component other code will use,
reach for `[<SolidTypeComponent>]`: it becomes a real tag with named props, for F# and for JavaScript callers alike.
See [SolidComponent](solid-component-attribute.md) for the details.

## SolidTypeComponent

The second attribute defines a custom tag. You declare a type, give it properties, and put the component's body in a
member marked `[<SolidTypeComponent>]`.

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

A few things happen in that code:

- `inherit div()` gives `Greeting` every attribute a `div` has, as well as its own `name` and `loud`.
- `props.name <- "world"` sets a default. The plugin turns assignments to props into a `merge({...}, props)` call.
- `.spread props` passes on every prop the component did not read itself. The plugin builds that set with `omit`.
- The member becomes a function named after the type. The member name, here `View`, does not matter.

`Greeting` is now used like any built-in tag: `Greeting(name = "Partas") { ... }`.

:::note
The type must live in a namespace or module that starts with `Partas.Solid`. The plugin only transforms types under
that prefix. Put your components in something like `namespace Partas.Solid.MyApp`.
:::

A type does not have to have a body. A type with no `[<SolidTypeComponent>]` member is a way to describe a component
that already exists in JavaScript, so you can use it from the DSL. Import it with `[<Import>]` and declare its
properties:

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

## What the plugin does for you

Beyond turning the DSL into JSX, the plugin handles the Solid boilerplate you would otherwise write by hand:

- It collects the props your component reads and emits `const PARTAS_OTHERS = omit(props, ...)` for spreading the
  rest.
- It turns default assignments into `props = merge({ ... }, props)`.
- It keeps prop reads as `props.x` in the JSX, so they stay reactive.

The [JSX output](../about/jsx-output.md) page shows the exact shapes.
