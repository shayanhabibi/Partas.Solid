---
title: Storybook Support
---

The plugin ships a second attribute, `PartasStorybook`, that turns a `storybook<'T> { ... }` expression into a
Storybook CSF module: a default-exported `meta` object and one named export per story.

```fsharp
open Partas.Solid
open Partas.Solid.Storybook

[<PartasStorybook>]
let private meta = storybook<Button> { () }
```

:::warning
Declare the binding `private` or `internal`. Otherwise Fable emits its own export for it next to the one the plugin
generates. The plugin exports the meta object and every story for you.
:::

For setting up a Storybook project that picks up Fable's output, see the [Storybook ecosystem
page](../ecosystem/storybook.md).

:::warning
The `PartasStorybook` attribute and the `storybook` builder are unchanged in Partas.Solid 3.0, and no test in this
repository covers them. The Solid renderer for Storybook targets Solid 1.x. Whether it runs Solid 2 components is not
verified here.
:::

## ArgTypes

The plugin infers `argTypes` from the component type. It reads each property's type and picks a control:

| Property type | Control | Table summary |
| --- | --- | --- |
| `bool` | `boolean` | `bool` |
| `string` | `text` | `string` |
| `[<StringEnum>]` with `cases` | `radio` under 6 options, else `select` | `[<StringEnum>]` |
| `char` | `text` | `char` |
| `Regex` | `text` | `Regex` |
| integer and float types | `number`, with `min`/`max`/`step` set from the type's range (for example `int` and `byte`) | the type name |
| tuples, lists, arrays, records, unions, anonymous records | `object` | the type shape |
| functions, generic parameters | hidden | `function`, or `GenericParam` and the parameter's name |
| option, nullable | the wrapped type's control | |

To cut noise, properties inherited from the native tag bindings are ignored. They get no argType, and a value you give
one in `args` is dropped too: the plugin only emits args for properties it collected. In the example below,
`btn.about <- "test"` does not appear in the generated `args`.

`[<StringEnum>]` unions need extra help: see [Cases](#cases). XML docs can steer generation: see [XML docs](#xml-docs).

### Cases

A `[<StringEnum>]` union reaches the plugin as a plain string. There is no type information left to list its cases, so
you declare them with the `cases` operation.

Take this component:

```fsharp
[<StringEnum>]
type Variant =
    | Black
    | Red
    | Blue

[<Erase>]
type Button() =
    interface RegularNode

    [<DefaultValue>]
    val mutable variant: Variant
```

By default `variant` gets a `text` control. To get the options, match on the property inside `cases`. Rider and most
F# IDEs can generate the full match for you, and the values on the right-hand side do not matter:

```fsharp
[<PartasStorybook>]
let private meta =
    storybook<Button> {
        cases (fun btn ->
            match btn.variant with
            | Black -> failwith "todo"
            | Red -> failwith "todo"
            | Blue when btn.variant = Black -> failwith "todo"
            | _ -> ())
    }
```

:::warning
Put a guard on the last case that refers back to the first case, as `Blue` does above. Without it, the compiler can
fold the last case into the `else` branch, and the plugin loses it.
:::

The plugin now has every case, but not the union's name, since that was erased. The generated argType:

```jsx
variant: {
    control: {
        type: "radio",
    },
    options: ["blue", "red", "black"],
    table: {
        type: {
            summary: "[<StringEnum>]",
        },
    },
},
```

### XML docs

:::warning
Fable does not give plugins the XML docs of `val mutable` fields. To steer generation with XML docs, declare the
property with `member val`.
:::

- `<summary>` becomes the property's description.
- `<defaultValue>` becomes the default value shown in the docs table.
- `<storybook />` is a closed tag whose attributes set options:

| Attribute | Effect |
| --- | --- |
| `spy="true"` | On a function-typed property, sets the arg to `fn()` from `storybook/test`, so calls show in the Actions panel. Function properties whose names start with `on` get this without the attribute. |
| `controlType="..."` | Overrides the control type, for example `radio`, `select`, `color`, `date`, `range`. Ignored on number properties. Experimental. |
| `controlType="false"` | Hides the control. |

Older docs listed a `hideControl` attribute. The plugin does not read it: use `controlType="false"`.

```fsharp
/// <summary>Fired when the button is chosen.</summary>
/// <storybook spy="true"/>
[<Erase>]
member val choosey: Browser.Types.Event -> unit = unbox null with get, set
```

## Args

`args` sets args for every story. The plugin reads them from the assignments in the lambda:

```fsharp
args (fun btn ->
    btn.variant <- Variant.Black)
```

Put a story name first to set args for that story alone. Each story name becomes a named export:

```fsharp
args "Default" (fun btn ->
    btn.variant <- Variant.Red)
```

## Render

`render` sets the render function. Spread the props you are given into the component, or the controls do nothing:

```fsharp
render (fun props -> Button().spread props)
```

With a story name first, it sets the render function for that story:

```fsharp
render "Default" (fun props -> Button().spread props)
```

## Decorators

`decorator` sets a decorator. Only one per story is supported for now. Call the story you are given:

```fsharp
decorator (fun story ->
    div () {
        "decorated:"
        story ()
    })
```

With a story name first, it applies to that story:

```fsharp
decorator "Default" (fun story ->
    div () {
        "default story decoration:"
        story ()
    })
```

## Tags

`tags` sets the meta object's Storybook tags:

```fsharp
tags [| "autodocs" |]
```

## Example

```fsharp
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

    /// <summary>
    /// Test
    /// </summary>
    [<Erase>]
    member val chocolate: string = unbox null with get, set

    /// <summary>
    /// sd
    /// </summary>
    /// <storybook spy="true"/>
    [<Erase>]
    member val choosey: Browser.Types.Event -> unit = unbox null with get, set

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
```

```fsharp
[<PartasStorybook(ComponentFlag.DebugMode)>]
let private meta =
    storybook<Button> {
        cases (fun btn ->
            match btn.variant with
            | Brown -> failwith "todo"
            | Black when btn.variant = Brown -> failwith "todo"
            | _ -> ())

        args (fun btn ->
            btn.variant <- Variant.Black
            btn.chocolate <- "Some value"
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

The output below was generated by Partas.Solid 2.x. The only change 3.0 makes to it is that the spread marker
`bool:n$={false}` is now `n$={false}`.

```jsx
const meta = {
    args: {
        chocolate: "Some value",
        choosey: fn(),
        variant: "black",
    },
    argTypes: {
        color: {
            control: {
                type: "text",
            },
            table: {
                type: {
                    summary: "string",
                },
            },
        },
        chocolate: {
            control: {
                type: "text",
            },
            description: "Test",
            table: {
                type: {
                    summary: "string",
                },
            },
        },
        choosey: {
            control: {
                type: false,
            },
            description: "sd",
            table: {
                type: {
                    summary: "function",
                },
            },
        },
        variant: {
            control: {
                type: "radio",
            },
            options: ["brown", "black"],
            table: {
                type: {
                    summary: "[<StringEnum>]",
                },
            },
        },
    },
    decorators: [(story) => <div>
        {story()}
    </div>],
    render: (btn_3) => <Button about="test"
        {...btn_3} n$={false}>
        Test
    </Button>,
    component: Button_1
 };

export default meta;

export const brown = {
        args: {
            variant: "brown",
        },
        render: (PARTAS_RENDER_BUILDER) => <Button about="brown"
            {...PARTAS_RENDER_BUILDER} n$={false}>
            brown test
        </Button>,
        decorators: [(PARTAS_DECORATOR_BUILDER) => <div>
            <div>
                {PARTAS_DECORATOR_BUILDER()}
            </div>
        </div>],
    }
export const choc = {
        args: {},
        render: (PARTAS_RENDER_BUILDER_2) => <Button />,
    }

const $PARTAS_DISCARD = { $discard: true,
};
```
