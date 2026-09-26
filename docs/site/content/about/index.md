---
title: About Partas.Solid
---

:::tip
If you have not used [Fable](https://fable.io) or are [new to F#](https://fsharp.org/), start with their websites.
The rest of these docs assume you know both a little.
:::

## Introduction

Partas.Solid is a plugin for Fable, the F# to JavaScript compiler. It turns F# into JSX for
[Solid](https://www.solidjs.com/). Partas.Solid 3.0 targets Solid 2.0 (currently `2.0.0-rc.9`).

It has one overarching goal that sets it apart from other F# front-end libraries:

:::note
Define one DSL that builds HTML/JSX trees for native elements, imported components, **and** the components you write
yourself.
:::

## What this means

- You **define** a component and use it **in the same DSL** as a native tag. A component you wrote looks the same at
  the call site as `div` or a component imported from a library. Every tree reads the same way, which makes it easier
  to read, refactor and reuse.
- A component inherits its shape from one type. Inherit `button()` and your component accepts children, classes and
  every button attribute and event. You do not write overloads or optional parameters by hand.
- Optional properties and default values look like they do in the JavaScript source you are porting from.

## How these docs are organised

- **About**: where the plugin came from, and why its output is JSX.
- **Guide**: the plugin's features, an introduction to Solid, and the bindings with their signatures.
- **Contributing**: how to build and test the repository, and notes on the plugin internals.
- **Examples**: longer walkthroughs.
- **Ecosystem**: bindings for other libraries. See [the ecosystem index](../ecosystem/index.md).

## Where to go next

- [Installation](../guide/installation.md): set up a project.
- [Benefits of JSX output](jsx-output.md): what the compiled output looks like and why it matters.
- [The Oxpecker fork](oxpecker-fork.md): the history of the project.
- [Migrating to Solid 2](../guide/migrating-to-solid-2.md): if you are coming from Partas.Solid 2.x.

:::details Isn't this Oxpecker.Solid?
Partas.Solid is an opinionated fork of Lanayx's [Oxpecker.Solid](https://github.com/lanayx/oxpecker). The plugin
transforms the Fable AST aggressively to produce Solid-compatible **JSX**, with a lot of syntactic sugar on top.

The two plugins have different goals, which is why the fork exists. Where possible, work on Partas.Solid also
supports and contributes back to [Oxpecker.Solid](https://github.com/lanayx/oxpecker).
:::
