---
name: partas-solid
description: "Use when building or debugging applications with Partas.Solid: F# components, reactive UI, match! expressions, Solid control flow, DOM events, or npm component bindings. For library consumers, rather than developing the Partas.Solid compiler plugin."
---

# Partas.Solid

Build applications with the public Partas.Solid DSL. Target the user's application and its existing architecture,
package manager, and build scripts. This skill is self-contained when installed outside the framework repository.

## Establish the application's version

Inspect its `.fsproj`, Fable tool manifest, `package.json`, and Vite configuration before choosing APIs.
Partas.Solid 3 targets Solid 2; Partas.Solid 2 targets Solid 1.9. Preserve the existing major version unless a
migration is requested. For a 2.x app, use the matching documentation instead of applying the 3.x examples here.
`match!` requires Partas.Solid 3.0.0-rc.2 or later.

For a new app or a compilation/setup problem, read [setup.md](references/setup.md). Check the linked installation
guide before selecting package versions; prerelease npm packages need a compatible version set.

## Choose the relevant reference

- When writing components, props, event handlers, refs, or imported tags, read
  [components.md](references/components.md). It includes a complete reusable component and view.
- When changing signals, stores, conditional UI, lists, effects, or async UI, read
  [reactivity.md](references/reactivity.md). It covers the Solid 2 callback and lifetime rules.
- When configuring Fable, mounting the app, or resolving incompatible dependencies, read
  [setup.md](references/setup.md).

## Authoring rules

- Put `[<SolidComponent>]` on each function that constructs DSL tags, including helpers in other files.
  A function component is a positional function call; reactive arguments can cause the caller to invoke it again.
- For a reusable tag with named reactive props, use an erased type under `Partas.Solid.*`, with a public,
  parameterless property such as `[<SolidTypeComponent>] member props.View = ...` in the same file as the type.
  A method written as `View()` is a different shape.
- Read signals and props in JSX, a derived accessor, or a memo. Component bodies and control-flow child callbacks
  build structure; copying a reactive value into a local variable there captures a snapshot.
- Use `yield fun ... -> ...` for function children. Pick `For.Keyed`, `For.NonKeyed`, or `For.KeyedFn` according to
  row identity; their item and index callback arguments differ.
- Use `match! source ()` for Switch/Match arm lifetime with live pattern-bound payloads. The source must supply
  reactive dependencies. Ordinary `match` remains an ordinary expression; `match!` here does not await tasks.
- Use `.spread props` explicitly to forward unused props from a type component. Read props are omitted from that
  spread; pass any consumed class or event prop explicitly when it must also reach the root element.
- Use `Fable.Core.TS` and qualified `Dom.X` types. DOM's `Attr` type can shadow Partas's `Attr` module if its
  namespace is opened directly. Fixed attribute values use the appropriate `Attr` union, not guessed strings.

## Check the result in the application

Run the application's Fable compilation and bundler/build command. Inspect generated `.fs.jsx` when a tag,
prop, or child disappears: it should contain JSX rather than erased DSL constructors or builder calls.
For behavior changes, exercise the relevant interaction: payload updates, arm changes, list reordering, event
handling, or cleanup. A successful .NET type-check alone does not establish correct JSX or DOM behavior.

For missing output, first check the attribute scope, type namespace, `yield`, and Release-mode Fable flags.
If those are correct, `ComponentFlag.DebugMode` or `ComponentFlag.PrintDisposals` can help produce a focused
library bug report. Keep application fixes in the application; editing the compiler dependency is a separate task.

## Documentation

[Partas.Solid docs](https://shayanhabibi.github.io/Partas.Solid/) are the public reference. Follow the pages for the
installed major version. For 3.x, start with the [component overview](https://shayanhabibi.github.io/Partas.Solid/guide/overview/),
[`match!` guide](https://shayanhabibi.github.io/Partas.Solid/guide/match-expression/), and
[Solid API bindings](https://shayanhabibi.github.io/Partas.Solid/guide/solid-js/).
