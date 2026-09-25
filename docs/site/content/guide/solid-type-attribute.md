---
title: SolidTypeComponent
---

`[<SolidTypeComponent>]` goes on a member of a type. The type's properties become the component's props, and the
member's body becomes the component.

```fsharp solid jsx
[<Erase>]
type Tag() =
    inherit span()

    [<Erase>]
    member val tone: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        props.tone <- "lightblue"

        span(style = "padding: 0 .4em; border-radius: .3em; background: " + props.tone).spread props {
            props.children
        }
```

```fsharp solid
div (style = "display: flex; gap: .5rem") {
    Tag() { "default" }
    Tag(tone = "palegreen", title = "Any span attribute works") { "green" }
}
```

The type inherits `span`, so `Tag` accepts every attribute a `span` does, as well as its own `tone`. It is used like
any other tag.

## Requirements

The plugin checks three things before it transforms the member. The member:

- is an **instance member** of the type,
- takes **no parameters**: write it as a property, `member props.View =`, and
- is declared in a type whose full name **starts with `Partas.Solid`**: put it in a `Partas.Solid.*` namespace or
  module.

The plugin does not check the next two, but the component only works when they hold. The member:

- is **public**, because the compiled component function takes the member's accessibility, and a private one is not
  exported for other files to use, and
- is in the **same file as the type declaration**.

The self identifier can be any name. `member props.View`, `member this.View` and `member x.View` all work, and the
generated code keeps the name you chose (`this` becomes `this$`). The member name does not matter either. The plugin
renames the compiled function after the type, so the member above compiles to `export function Tag(props)`.

If a member does not pass, the plugin warns:

> This is an invalid member declaration for this attribute. Ensure it follows the following pattern:
> `member props.typeDef =`. Ensure it is declared within a namespace starting with `Partas.Solid`

It then transforms the body as if it were a `[<SolidComponent>]`. There is no renaming, no defaults, and no rest
props, so the component will not work as one.

:::details title="Why does the type need to be in a Partas.Solid namespace?"
The plugin only treats types whose full name starts with `Partas.Solid` as tags and components. The prefix keeps it
from rewriting code it should leave alone. A type anywhere else fails the check above.
:::

:::details title="Why does the member need to be in the same file as the type?"
Every use of the type's constructor imports the component from the file that declares the type. Say the type is
declared in `MyComponent.fs` and the member is an extension in `Extensions.fs`. Partas.Solid compiles the component
function into `Extensions.fs.jsx`, but every use imports it from `MyComponent.fs.jsx`, which has no function of that
name. You get a runtime error.
:::

## Properties

A property on the type is a prop of the component. The erased shapes below all work. The plugin reads them as prop
names, not as F# properties.

::::tabs
:::tab member val
```fsharp
[<Erase>]
type Greeting() =
    inherit div()

    [<Erase>]
    member val name: string = unbox null with get, set
```
:::
:::tab val mutable
```fsharp
[<Erase>]
type Greeting() =
    inherit div()

    [<DefaultValue>]
    val mutable name: string
```
:::
:::tab Explicit get/set
```fsharp
[<Erase>]
type Greeting() =
    inherit div()

    [<Erase>]
    member _.name
        with get (): string = JS.undefined
        and set (value: string) = ()
```
:::
::::

`unbox null` and `JS.undefined` are only placeholders. The property is erased, so the value never reaches
JavaScript.

### Aliasing names

Some prop names are F# keywords, or clash with a member you inherited. Declare the property under the real name in
backticks, then add an alias with the explicit get/set shape and `inline` accessors, so callers can both read and set
it:

```fsharp
[<Erase>]
type Field() =
    inherit div()

    [<Erase>]
    member val ``type``: string = unbox null with get, set

    member this.type'
        with inline get (): string = this.``type``
        and inline set (value: string) = this.``type`` <- value
```

## Default values

Assign to a prop in the body to give it a default:

```fsharp
[<SolidTypeComponent>]
member props.View =
    props.tone <- "lightblue"
    // ...
```

Every assignment is collected into one `merge` call at the start of the component. A prop that the caller passes
wins over the default. The JSX tab of the `Tag` example at the top of the page shows the result, which starts:

```jsx
export function Tag(props) {
    props = merge({
        tone: "lightblue",
    }, props);
    const PARTAS_OTHERS = omit(props, "tone", "children");
    // ...
}
```

`merge` comes from `solid-js`. It keeps the props reactive: `props.tone` still reads the caller's current value.
Setting the same prop twice is a compile error ("Multiple defaults for the same property").

## Rest props

The plugin collects every prop the body reads (`props.tone`, `props.children`) and emits an `omit` call that leaves
them out:

```jsx
const PARTAS_OTHERS = omit(props, "tone", "children");
```

`.spread props` spreads `PARTAS_OTHERS` onto the element, not the whole of `props`. The props your component uses
itself are not passed on, and everything else is: `title`, event handlers, `id` and so on. If the body reads no props,
the binding is just `const PARTAS_OTHERS = props;`.

Here is a component that uses `variant` itself, and passes every other `button` attribute through:

```fsharp solid jsx
[<Erase>]
type FancyButton() =
    inherit button()

    [<Erase>]
    member val variant: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        button(class' = "fancy-" + props.variant).spread props { props.children }
```

```fsharp solid render=FancyHost
[<SolidComponent>]
let FancyHost () =
    let clicks, setClicks = createSignal 0

    FancyButton(
        variant = "primary",
        title = "I came through the spread",
        onClick = fun _ -> setClicks (clicks () + 1)
    ) {
        $"Clicked {clicks ()} times"
    }
```

`title` and `onClick` are not props of `FancyButton`. They reach the `button` through the spread.

:::note
A prop you read in the body is **not** in `PARTAS_OTHERS`. If you read `props.class'` and also want it on the element
you spread onto, pass it explicitly: `button(class' = props.class').spread props`. See
[Extension Methods](extension-methods.md).
:::

### Changes from Partas.Solid 2.x

Partas.Solid 3.0 uses the Solid 2 primitives. `splitProps` and `mergeProps` no longer exist.

| 2.x on Solid 1.9 | 3.0 on Solid 2 |
| --- | --- |
| `const [PARTAS_LOCAL, PARTAS_OTHERS] = splitProps(props, ["class"])` | `const PARTAS_OTHERS = omit(props, "class")` |
| reads went through `PARTAS_LOCAL.class` | reads go through `props.class` |
| `props = mergeProps({...}, props)` | `props = merge({...}, props)` |
| self identifier had to be `props` | any self identifier |

You can turn the `omit` binding off with `ComponentFlag.SkipOmit`, and make `.spread` spread the whole self
identifier with `ComponentFlag.SpreadProps`. See [Attribute Flags](attribute-flags.md). To call `omit` or `merge`
yourself, use `Bindings.omit` and `Bindings.merge` (see [Solid-JS](solid-js.md)).

:::danger
Do not name anything in your component `PARTAS_OTHERS`. The plugin declares that name in every type component, and
yours would clash with it.
:::

## Known issues

:::warning
The runtime tests record these as known bugs:

- An option-typed prop read with `props.age.IsSome` or `match props.message with ...` compiles to an undefined getter
  call. Reading it with `defaultArg props.nickname "anon"` works.
- Indexing a prop inside a `while` condition (`props.tabs[i]`) has the same problem.
- A default whose value is a function, such as `props.onChange <- ignore`, is left out of `merge`.
:::

## Signatures

```fsharp
type SolidTypeComponentAttribute(flag: int)
new()
new(compileOptions: ComponentFlag)
```
