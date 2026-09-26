---
title: Storybook
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

`Partas.Solid.Storybook` is a thin binding to [Storybook](https://storybook.js.org/), through
[Kachurun's solid-storybook plugin](https://github.com/kachurun/create-solid-storybook).

It works, but it will not get much more work. Stories are meant to move to a data-contract style abstraction inside
the Partas.Solid plugin itself: that is the `[<PartasStorybook>]` attribute described in
[Storybook support](../guide/storybook.md).

## Installation

Unlike the other binding packages, this one has no Femto helpers. Follow the steps below to set everything up before you
add the package.

### Create a solid-storybook project

```bash
npx create-solid-storybook [path/project]

cd [path/project]

npm install

npm run storybook
```

If that works, set up your Fable project in the same directory.

## Running Fable and Storybook

Compile into the `stories/` directory, so styling and the rest resolve as Storybook expects. Storybook only picks up
files with the `.stories.jsx` extension, so the other compiled files do not get in the way.

```bash
fable watch -c Release -o stories -e .jsx --run storybook dev -p 6006
```

## Making a story

See Storybook's [Component Story Format](https://storybook.js.org/docs/api/csf).

Create a source file in the `stories/` directory with a `.stories` suffix:

```
Button.stories.fs
```

Define the component meta, and make it the default export.

:::note
Make the meta binding private, or Storybook will treat it as a story too. Storybook's docs explain how it picks up
stories.
:::

`[<SolidComponent>]` is required for `!@` to render correctly:

```fsharp
[<SolidComponent>]
[<ExportDefault>]
let private meta: Meta<Button> = Meta.make [
    Meta.component' !@Button
    Meta.args [
        Args.make "children" "Button"
        Args.make "variant" Button.Variant.Default
        Args.make "size" Button.Size.Default
    ]
    Meta.parameters [
        Parameters.layout StorybookLayout.Centered
    ]
    Meta.argTypes [
        ArgTypes.make "variant" <| ArgType(
            control = Control(
                ``type`` = ControlType.Select,
                labels = createObj [
                    !!Button.Variant.Default ==> nameof Button.Variant.Default
                    // ...
                ]
            ),
            table = ArgType.Table(
                category = "variants",
                defaultValue = ArgType.Table.Value(summary = nameof Button.Variant.Default)
            ),
            options = [| Button.Variant.Default; Button.Variant.Secondary |]
        )
    ]
]

// A trick to get completion for attributes and args
[<Global>]
let btn = Button()

let Default = meta.make [
    Story.args [
        Args.make (nameof btn.variant) Button.Variant.Default
    ]
    Story.render (fun props -> div() { Button().spread props })
    // ^-- when you use render, spread the props into
    // the tested component
]
```

Once you have written at least one story, its page comes up in Storybook.

## Example

To see Partas.Solid and Storybook together, build the storybook in the
[Partas.Solid.UI](https://github.com/shayanhabibi/Partas.Solid.UI) repository.
