---
title: Polymorphism
---

Libraries such as Kobalte and ArkUI let a component render as another component or tag, passing its props on. In
Kobalte this is the `as` prop:

```jsx
<Button as={a} href={"/"} />
```

The plugin supports this for Kobalte and ArkUI, and lets you enable it for other libraries. The F# side lives in the
auto-opened `Partas.Solid.Polymorphism` module.

:::note
The Kobalte and ArkUI bindings themselves have not been ported to Solid 2 yet, so the examples below use a small
component of their own.
:::

## Making a component polymorphic

A component opts in by implementing the `Polymorph` interface. That gives it the `.as'` extension method, which takes
either a built element or a `TagValue`. Pass an element when you can: F# cannot inherit the morph target's props
dynamically, but it does type-check them in the target's own constructor.

Here `Box` renders whatever its `as` prop holds through `Dynamic`, and defaults to a `div`:

```fsharp solid render=BoxAsButton jsx
open Partas.Solid.Web

[<Erase>]
type Box() =
    inherit div()
    interface Polymorph

    [<Erase>]
    member val ``as``: obj = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        props.``as`` <- box "div"
        Dynamic<obj>(component' = unbox props.``as``).spread props

[<SolidComponent>]
let BoxAsButton () =
    let count, setCount = createSignal 0
    Box(class' = "boxed")
        .as'(button (type' = "button", onClick = fun _ -> setCount (count () + 1))) {
        $"Pressed {count ()} times"
    }
```

The element passed to `.as'` becomes a function that spreads the parent's props onto the morph target:

```jsx
as={(PARTAS_POLYPROPS) => <button {...PARTAS_POLYPROPS} n$={false} type="button" onClick={...} />}
```

So the rendered `<button>` gets `Box`'s `class` and children, and keeps the `type` and `onClick` set on it.

The morph target can be another component. Its own props arrive alongside the parent's:

```fsharp solid render=BoxAsLink jsx
[<Erase>]
type ToneLink() =
    inherit a()

    [<Erase>]
    member val tone: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        a(style = (if props.tone = "loud" then "font-weight: bold" else "")).spread props { props.children }

[<SolidComponent>]
let BoxAsLink () =
    Box(title = "Go to the top").as'(ToneLink(tone = "loud", href = "#")) { "A Box rendered as a ToneLink" }
```

A `TagValue` passed to `.as'` goes through unchanged, so the morph target's props are untyped and set with `.attr`.

## Polymorphic attributes

The plugin treats these extension methods as polymorphic attributes:

| Method | JSX prop | For |
| --- | --- | --- |
| `.as'` | `as` | Kobalte |
| `.asChild` | `asChild` | ArkUI |
| any name starting `__PARTAS_POLYMORPHIC__` | the name with the prefix cut off | anything else |

Only `.as'` comes with Partas.Solid. The plugin recognises `asChild`, but you declare that extension yourself, the same
way as a custom one below. This is the built-in declaration:

```fsharp
[<Erase; Extension>]
type PolymorphicExtensions =
    [<Erase; Extension>]
    static member as'<'Base when 'Base :> Polymorph>(this: 'Base, morph: #HtmlTag) : 'Base = this
    [<Erase; Extension>]
    static member as'<'Base when 'Base :> Polymorph>(this: 'Base, morph: TagValue) : 'Base = this
```

### Custom polymorphic attributes

For a library whose polymorphic prop has another name, declare an extension method with the magic prefix. This one
makes `render` polymorphic:

```fsharp
module MyLibrary.PolymorphicExt

open Partas.Solid
open Fable.Core

[<Erase; System.Runtime.CompilerServices.Extension>]
type CustomPolymorphicExtensions =
    [<Erase; System.Runtime.CompilerServices.Extension>]
    static member __PARTAS_POLYMORPHIC__render<'Base when 'Base :> Polymorph>(this: 'Base, morph: #HtmlTag) : 'Base = this
```

```fsharp
Slot(id = "slot").__PARTAS_POLYMORPHIC__render(article (class' = "art")) { "slotted" }
```

The plugin cuts the prefix off the name, turns the element into a function as above, and emits
`render={(PARTAS_POLYPROPS) => <article ... />}`.

:::warning
The plugin only looks for polymorphic attributes on extension method calls, and only when the type that declares the
extension has a name ending in `PolymorphicExtensions` (or `HtmlElementExtensions`). A prefixed property setter is not
wrapped in a function.

Declare the extension in an earlier file than the code that calls it. When the extension and the call are in the same
module, the plugin does not recognise the call and the component compiles to a plain function call instead of JSX.
:::

## `.attr`

Without any of this, you can pass a tag value to the library's polymorphic prop and chain `.attr` for the props the
parent does not have, at the cost of type checking.
