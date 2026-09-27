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
Components are plain functions. Give one a unit parameter to use it on its own, as in `ClickCounter ()`.
:::

## Components with parameters are untracked calls

A `[<SolidComponent>]` binding compiles to a JavaScript function, and a call to it compiles to a function call inside
the parent's JSX, not to a `<Label />` element:

```jsx
export function Label(text) {
    return <span style="font-weight: bold">
        {text}
    </span>;
}
```

The parent renders `{untrack(() => Label("Plain"))}`. The arguments are positional, so the call cannot become a tag,
but `untrack` gives it the same semantics as `<Label />`, which Solid compiles to an untracked `createComponent`. The
body runs once: a signal read in the body is a snapshot, and a signal read in the returned JSX stays live, just as in
a [SolidTypeComponent](solid-type-attribute.md).

## How it works

The plugin walks the body of the binding and replaces each tag, with its properties and children, by a JSX element.
On the way it removes computation expression plumbing around lists and children, so that it does not hide values from
Solid's reactivity, and optimises `[<Pojo>]` constructors (see [Attribute Flags](attribute-flags.md)).

To see what the plugin made of your code, read [JSX output](../about/jsx-output.md).

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

## Signatures

```fsharp
type SolidComponentAttribute(flag: int)
new()
new(compileOptions: ComponentFlag)
```

See [Attribute Flags](attribute-flags.md) for the available `ComponentFlag`s.
