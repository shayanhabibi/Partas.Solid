---
title: Polymorphism
---

Libraries such as Kobalte and ArkUI let a component render as another component or tag. The parent's props are passed
on to the component it renders as. In Kobalte this is the `as` prop:

```jsx
<Button as={a} href={"/"} />
```

Partas.Solid supports this in the plugin, and lets you enable it for other libraries. The support is part of
Partas.Solid 3.0 itself, in the auto-opened `Partas.Solid.Polymorphism` module.

:::note
The Kobalte and ArkUI bindings themselves target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
The examples below use a small component of their own instead.
:::

## Making a component polymorphic

A component opts in by implementing the `Polymorph` interface. That gives it the `.as'` extension method, which takes
either a built element or a `TagValue`.

The plugin handles `.as'` specially. Passing a whole element, rather than a tag value, is what keeps F#'s type checking:
you set the morph target's own props in its constructor, where the compiler checks them. F# cannot inherit props
dynamically, so this is how you get to them.

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

The JSX tab shows what the plugin does with the element passed to `.as'`. It becomes a function that takes the
parent's props and spreads them onto the morph target:

```jsx
as={(PARTAS_POLYPROPS) => <button {...PARTAS_POLYPROPS} n$={false} type="button" onClick={...} />}
```

So the rendered `<button>` gets `Box`'s `class` and children, and keeps the `type` and `onClick` set on it.

The morph target can be another component. Its own props arrive alongside the parent's:

```fsharp solid render=BoxAsLink
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

You can also pass a `TagValue` to `.as'`. The plugin passes it through unchanged, so you lose the typed props of the
morph target and have to set them with `.attr`.

## Polymorphic attributes

The plugin treats these extension methods as polymorphic attributes:

| Method | JSX prop | For |
| --- | --- | --- |
| `.as'` | `as` | Kobalte |
| `.asChild` | `asChild` | ArkUI |
| any name starting `__PARTAS_POLYMORPHIC__` | the name with the prefix cut off | anything else |

Only `.as'` comes with Partas.Solid, for any type that implements `Polymorph`. The plugin recognises `asChild`, but you
declare that extension yourself, the same way as a custom one below.

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
extension has a name ending in `PolymorphicExtensions` (or `HtmlElementExtensions`). `CustomPolymorphicExtensions`
above qualifies. A prefixed property setter, as the 2.x docs showed, is not wrapped in a function.

Declare the extension in an earlier file than the code that calls it. When the extension and the call are in the same
module, the plugin does not recognise the call and the component compiles to a plain function call instead of JSX.
:::

## `.attr`

You can skip all of this: pass a tag value to the library's polymorphic prop, then chain `.attr` for the props the
parent does not have. You lose the type checking that is the reason to use F# and Fable in the first place.
