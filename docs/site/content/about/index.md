---
title: About Partas.Solid
---

:::tip
If you have not used [Fable](https://fable.io) or are [new to F#](https://fsharp.org/), start with their websites.
The rest of these docs assume you know both a little.
:::

## Introduction

Partas.Solid is a plugin for Fable, the F# to JavaScript compiler. It turns F# into JSX for
[Solid](https://www.solidjs.com/). Partas.Solid 3.0 targets Solid 2.0 (currently `2.0.0-rc.14`).

It has one overarching goal that sets it apart from other F# front-end libraries:

:::note
Define one DSL that builds HTML/JSX trees for native elements, imported components, **and** the components you write
yourself.
:::

## What this means

A component you write is called the same way as `div` or a component imported from a library. It inherits its shape
from one type: inherit `button()` and it accepts children, classes and every button attribute and event, with no
overloads written by hand. Defaults are set in the body, as they would be in the JavaScript you are porting from:

```fsharp solid jsx render=CardDemo
[<Erase>]
type Card() =
    inherit button()

    [<Erase>]
    member val label: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        props.label <- "Untitled"
        button(style = "padding: .25rem .75rem").spread props { props.label }

[<SolidComponent>]
let CardDemo () =
    div (style = "display: flex; gap: .5rem") {
        Card()
        Card(label = "Disabled", disabled = true)
        button (style = "padding: .25rem .75rem") { "Native button" }
    }
```

## Where to go next

- [Installation](../guide/installation.md): set up a project.
- [Benefits of JSX output](jsx-output.md): what the compiled output looks like and why it matters.
- [Comparing JSX with Oxpecker](oxpecker-comparison.md): tested F# inputs and generated output from both libraries.
- [The Oxpecker fork](oxpecker-fork.md): why this is a fork of Lanayx's [Oxpecker.Solid](https://github.com/lanayx/oxpecker),
  and how the two differ.
- [Migrating to Solid 2](../guide/migrating-to-solid-2.md): if you are coming from Partas.Solid 2.x.
