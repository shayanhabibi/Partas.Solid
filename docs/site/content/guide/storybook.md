---
title: Storybook Support
---

The plugin's second attribute, `PartasStorybook`, turns a `storybook<'T> { ... }` expression into a Storybook CSF
module: a default-exported `meta` object and one named export per story. Setting up a Storybook project that picks up
Fable's output is on the [Storybook ecosystem page](../ecosystem/storybook.md).

```fsharp
open Partas.Solid
open Partas.Solid.Storybook

[<PartasStorybook>]
let private meta = storybook<Button> { () }
```

:::warning
Declare the binding `private` or `internal`, or Fable emits its own export next to the plugin's.

No test covers `PartasStorybook`, and Storybook's Solid renderer targets Solid 1.x: it is not verified with Solid 2
components.
:::

## Operations

| Operation | Sets |
| --- | --- |
| `args (fun c -> c.prop <- value)` | Args for every story, read from the assignments |
| `render (fun props -> Comp().spread props)` | The render function. Spread the props, or the controls do nothing |
| `decorator (fun story -> div () { story () })` | A decorator. One per story for now |
| `cases (fun c -> match c.prop with ...)` | The options of a `[<StringEnum>]` property ([Cases](#cases)) |
| `tags [\| "autodocs" \|]` | The meta object's tags |

Put a story name first (`args "Default" (fun c -> ...)`) and `args`, `render` and `decorator` apply to that story
alone. Each story name becomes a named export.

## ArgTypes

The plugin infers `argTypes` from the component type, picking a control from each property's type:

| Property type | Control | Table summary |
| --- | --- | --- |
| `bool` | `boolean` | `bool` |
| `string`, `char`, `Regex` | `text` | the type name |
| `[<StringEnum>]` with `cases` | `radio` under 6 options, else `select` | `[<StringEnum>]` |
| integer and float types | `number`, with `min`/`max`/`step` from the type's range | the type name |
| tuples, lists, arrays, records, unions, anonymous records | `object` | the type shape |
| functions, generic parameters | hidden | `function`, or `GenericParam` and the parameter's name |
| option, nullable | the wrapped type's control | |

Properties inherited from the native tag bindings get no argType, and args set on them are dropped: `btn.about <-
"test"` in the example below does not reach `args`.

### Cases

A `[<StringEnum>]` union reaches the plugin as a plain string, so without help it gets a `text` control. List its
cases by matching on the property inside `cases`. The right-hand sides do not matter, and Rider can generate the
match:

```fsharp
cases (fun btn ->
    match btn.variant with
    | Black -> failwith "todo"
    | Red -> failwith "todo"
    | Blue when btn.variant = Black -> failwith "todo"
    | _ -> ())
```

:::warning
Guard the last case with a reference back to the first, as `Blue` does. Without it the compiler can fold the last case
into the `else` branch, and the plugin loses it.
:::

### XML docs

XML docs on a `member val` steer generation. Fable does not pass plugins the XML docs of `val mutable` fields.

- `<summary>` is the property's description.
- `<defaultValue>` is the default value in the docs table.
- `<storybook />` takes attributes:

| Attribute | Effect |
| --- | --- |
| `spy="true"` | On a function property, sets the arg to `fn()` from `storybook/test`, so calls show in the Actions panel. Function properties named `on...` get this anyway |
| `controlType="..."` | Overrides the control type (`radio`, `select`, `color`, `date`, `range`). Ignored on numbers. Experimental |
| `controlType="false"` | Hides the control, as does any value starting with `hide` |

## Example

```fsharp
open Fable.Core.TS

[<StringEnum>]
type Variant =
    | Brown2
    | Brown
    | Black

[<Erase>]
type Button() =
    interface RegularNode

    [<DefaultValue>]
    val mutable color: string

    [<DefaultValue>]
    val mutable variant: Variant

    /// <summary>A flavour.</summary>
    [<Erase>]
    member val chocolate: string = unbox null with get, set

    /// <summary>Fired when the button is chosen.</summary>
    /// <storybook spy="true"/>
    [<Erase>]
    member val choosey: Dom.Event -> unit = unbox null with get, set

    [<SolidTypeComponent>]
    member props.__ =
        props.variant <- Variant.Black

        div (
            class' =
                match props.variant with
                | Brown -> "brown"
                | Black -> "black"
                | Brown2 -> "cho"
        )

[<PartasStorybook>]
let private meta =
    storybook<Button> {
        cases (fun btn ->
            match btn.variant with
            | Brown -> failwith "todo"
            | Black when btn.variant = Brown -> failwith "todo"
            | _ -> ())

        args (fun btn ->
            btn.variant <- Variant.Black
            btn.about <- "test")

        decorator (fun story -> div () { story () })
        decorator "brown" (fun story -> div () { div () { story () } })
        args "brown" (fun btn -> btn.variant <- Variant.Brown)
        render (fun btn -> Button(about = "test").spread btn { "Test" })
        render "brown" (fun btn -> Button(about = "brown").spread btn { "brown test" })
        args "choc" (fun btn -> ())
        render "choc" (fun btn -> Button())
    }
```

The plugin emits (the component itself omitted):

```jsx
import { fn } from "storybook/test";

const meta = {
    args: {
        variant: "black",
        choosey: fn(),
    },
    argTypes: {
        color: {
            control: { type: "text" },
            table: { type: { summary: "string" } },
        },
        variant: {
            control: { type: "radio" },
            options: ["brown", "black"],
            table: { type: { summary: "[<StringEnum>]" } },
        },
        chocolate: {
            control: { type: "text" },
            description: "A flavour.",
            table: { type: { summary: "string" } },
        },
        choosey: {
            control: { type: false },
            description: "Fired when the button is chosen.",
            table: { type: { summary: "function" } },
        },
    },
    decorators: [(story) => <div>{story()}</div>],
    render: (btn_3) => <Button about="test" {...btn_3} n$={false}>Test</Button>,
    component: Button
};

export default meta;

export const brown = {
    args: { variant: "brown" },
    render: (PARTAS_RENDER_BUILDER) => <Button about="brown" {...PARTAS_RENDER_BUILDER} n$={false}>brown test</Button>,
    decorators: [(PARTAS_DECORATOR_BUILDER) => <div><div>{PARTAS_DECORATOR_BUILDER()}</div></div>],
}
export const choc = {
    args: {},
    render: (PARTAS_RENDER_BUILDER_2) => <Button />,
}
```
