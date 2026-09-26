---
title: SolidComponent
---

`[<SolidComponent>]` goes on a `let` binding. It tells the Partas.Solid plugin to turn the tags in the binding's body
into JSX.

```fsharp solid render=ClickCounter jsx
[<SolidComponent>]
let ClickCounter () =
    let count, setCount = createSignal 0

    button (onClick = fun _ -> setCount (count () + 1)) {
        $"Clicked {count ()} times"
    }
```

## When you need it

You need the attribute on any binding whose body builds elements with the DSL. Without it, the tags stay as F#
constructor calls and compile to nothing useful.

A component can take parameters. Other components can call it like any F# function:

```fsharp solid
[<SolidComponent>]
let Label (text: string) =
    span (style = "font-weight: bold") { text }

[<SolidComponent>]
let Framed (text: string) =
    div (style = "border: 1px solid currentColor; padding: .5rem") { Label text }
```

```fsharp solid
div () {
    Label "Plain"
    Framed "Framed"
}
```

A binding that only calls another component, and builds no tags itself, does not need the attribute:

```fsharp
let Render () = "Button" |> Framed
```

:::note
Solid works on the idea that everything is a function. Your bindings can be used as components **because they are
functions**. Give a component a unit parameter if you want to use it on its own, as in `ClickCounter ()`.
:::

## Components with parameters are plain calls

A `[<SolidComponent>]` binding compiles to a JavaScript function, and a call to it compiles to a function call inside
the parent's JSX, not to a `<Label />` element:

```jsx
export function Label(text) {
    return <span style="font-weight: bold">
        {text}
    </span>;
}
```

The parent renders `{Label("Plain")}`. Solid 2 tracks that expression, so the component's body runs inside the
parent's tracking scope.

:::warning
Because of this, a signal read in the **body** of a let-bound component (outside the returned JSX) re-runs the whole
body and rebuilds its DOM when the signal changes. A `[<SolidTypeComponent>]` body runs once, as Solid intends. The
runtime tests record this as a known bug. Until it is fixed, read signals inside the JSX, or use a
[SolidTypeComponent](solid-type-attribute.md) for components that hold state or read accessors in their body.
:::

## How it works

The attribute makes the plugin walk the body of the binding. It finds the tags, collects each one with its
properties and children into an element, and emits a JSX element in its place.

The plugin does a few other things on the way. It removes computation expression plumbing around lists and children,
so that it does not hide values from Solid's reactivity. It also optimises `[<Pojo>]` constructors (see
[Attribute Flags](attribute-flags.md)).

If you want to see what the plugin made of your code, read [JSX output](../about/jsx-output.md), or add `jsx` to a
fence on this site.

## Tag values need a plugin scope

The `!@` operator turns a component into a [tag value](tag-values.md). The plugin rewrites it, so it only works in code
the plugin transforms: the body of a `[<SolidComponent>]` binding or a `[<SolidTypeComponent>]` member.

A common case is a list of records or tuples that carry an icon for the UI to render:

```fsharp
[<Erase>]
type SportsIcon() =
    inherit span()

    [<SolidTypeComponent>]
    member props.View = span () { "S" }

[<SolidComponent>]
let NewsIcon () = span () { "N" }
```

::::tabs
:::tab Invalid
```fsharp
// Not in a plugin scope: !@ is not rewritten
let items =
    [ for item in titles do
        match item with
        | "Sports" -> item, Some !@SportsIcon
        | _ -> item, None ]
```
:::
:::tab Valid
```fsharp
// A let-bound component is already a function value
let items =
    [ for item in titles do
        match item with
        | "News" -> item, Some NewsIcon
        | _ -> item, None ]
```
:::
:::tab Also valid
```fsharp
[<SolidComponent>] // <-- plugin scope
let makeItems () =
    [ for item in titles do
        match item with
        | "Sports" -> item, Some !@SportsIcon
        | _ -> item, None ]
```
:::
::::

If the component is a let binding rather than a type component, you can refer to it directly and skip `!@`.

## Signatures

```fsharp
type SolidComponentAttribute(flag: int)
new()
new(compileOptions: ComponentFlag)
```

See [Attribute Flags](attribute-flags.md) for the available `ComponentFlag`s.
