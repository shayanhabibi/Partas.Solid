---
title: Writing bindings
---

[Partas.Solid.Bindings](https://github.com/shayanhabibi/Partas.Solid.Bindings) has prebuilt bindings for popular
Solid libraries.

:::warning
The prebuilt bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet. What this page
says about writing bindings is current for Partas.Solid 3.0.
:::

## Binding a component

A Solid component is a type. Its constructor parameters and mutable fields are the props, and an attribute tells the
plugin where to import it from.

- `[<Import("Name", "path")>]` is the plain Fable import. Partas.Solid uses it for `Show`, `Loading`, `Repeat` and most
  of the `solid-js` flow components.
- `[<PartasImport("Name", "path")>]` comes from `Partas.Solid`. Fable sees an ordinary local type, and the plugin adds
  the import when it turns a constructor call into a JSX tag. Use it when Fable's import would get in the way, for
  example on a type that carries its own builder members. `Router`, `Route`, `Switch` and `Dynamic` are bound this way.

The interfaces decide what the tag accepts:

| Interface | Gives the tag |
| --- | --- |
| `HtmlElement` | Nothing but its own props. The base of everything. |
| `HtmlTag` | All the standard HTML attributes and events. |
| `HtmlContainer` | Children, through a `{ ... }` block. |
| `RegularNode` | `HtmlTag` and `HtmlContainer` together. |
| `VoidNode` | `HtmlTag` without children. |

Props are `[<Erase; DefaultValue>] val mutable` fields, or `[<Erase>] member val` properties. `Erase` keeps them out of
the compiled output: the plugin reads them as JSX attributes. A trailing `'` is dropped, so `when'` becomes `when`.

Here `solid-js`'s `Show` is bound again under another name:

```fsharp solid render=GateDemo jsx
[<PartasImport("Show", "solid-js"); Erase>]
type Gate() =
    interface HtmlContainer
    [<Erase; DefaultValue>]
    val mutable when': bool
    [<Erase; DefaultValue>]
    val mutable fallback: HtmlElement

[<SolidComponent>]
let GateDemo () =
    let shown, setShown = createSignal false
    div () {
        button (onClick = fun _ -> setShown (not (shown ()))) { "Toggle" }
        Gate(when' = shown (), fallback = p () { "Hidden" }) {
            p () { "Now you see me" }
        }
    }
```

The JSX tab shows `<Show>`, imported from `solid-js`. `Gate` exists only in F#.

### Wrapping a bound component

To give a bound component defaults or styling, inherit it and add a `[<SolidTypeComponent>]` member that renders the
original with the props spread onto it:

```fsharp solid render=QuietGateDemo jsx
[<Erase>]
type QuietGate() =
    inherit Gate()
    [<SolidTypeComponent>]
    member props.View =
        Gate(fallback = em () { "Nothing to show" }).spread props

[<SolidComponent>]
let QuietGateDemo () =
    let shown, setShown = createSignal false
    div () {
        button (onClick = fun _ -> setShown (not (shown ()))) { "Toggle" }
        QuietGate(when' = shown ()) { strong () { "Shown" } }
    }
```

`QuietGate` accepts every prop `Gate` does. A `fallback` passed by the caller wins over the default, because the spread
comes after it. See [SolidTypeComponent](../guide/solid-type-attribute.md) for the rules on these members.

## Naming

`[<SolidTypeComponent>]` members must be declared inside the `Partas.Solid` namespace, or a namespace or module under
it, and the plugin checks for that prefix when it decides whether a type is one of its own. So start binding
namespaces with `Partas.Solid`, and name them after the library: `Partas.Solid.Kobalte`, not `Partas.Solid.Fabalte`.

## Specs

Put the constant literals, such as the library's import paths, in a module named `Spec` (it can be `Erase`), with a
public string literal holding the binding's version. If the file is small, put the `Enums` module in it too.

## Enums

Collect the enums under an auto-opened `Enums` module in the `Spec` or `Enums` file. Typing the library name and
`.Enums` then lists every enum, even when libraries have conflicting names such as `Orientation`. Name each enum in
full; for shorter names, add submodules with aliases to the originals, with or without `RequireQualifiedAccess`.

```fsharp
namespace Partas.Solid.Kobalte

[<Erase; AutoOpen>]
module Enums =
    [<StringEnum; RequireQualifiedAccess>]
    type TextFieldInputOrientation =
        | Ltr
        | Rtl

    [<RequireQualifiedAccess; Erase>]
    module TextFieldInput =
        type Orientation = TextFieldInputOrientation

    [<StringEnum>]
    type PopperOrientation =
        | Start
        | End

    [<RequireQualifiedAccess>]
    module Popper =
        type Orientation = PopperOrientation
```

- Use PascalCase. Use `CompiledName` and `CompiledValue` where the JS value differs.
- Use `RequireQualifiedAccess` as much as you can.
- You can also add aliases to an enum as static members of the type it belongs to.

## Options objects

Many functions take their optional arguments as an object:

```js
someMethod(someparam, { someotherparam, someotherotherparam })
```

Give the binding a signature that flattens them:

```fsharp
[<ImportMember(path); ParamObject(1)>]
static member someMethod(someparam: 'T, ?someotherparam: 'T, ?someotherotherparam: 'T): 'T = jsNative
[<ImportMember(path)>]
static member someMethod(someparam: 'T): 'T = jsNative
```

`ParamObject(1)` collects the arguments from index 1 on into an object, which is empty when no optional argument is
given; the second overload avoids that. Partas.Solid binds `createSignal` and `createEffect` this way.

When an option must itself be an object, or has to be a named argument, go through a `JS.Pojo` type, and flatten its
fields into an inline overload so users do not build the options object themselves:

```fsharp
[<JS.Pojo>]
type SomeMethodOptions(?someotherparam: 'T, ?someotherotherparam: 'T) =
    member val someotherparam = someotherparam with get,set
    member val someotherotherparam = someotherotherparam with get,set
// ...
[<ImportMember(path)>]
static member someMethod(someparam: 'T, ?options: obj): 'T = jsNative
static member inline someMethod(someparam: 'T, ?someotherparam: 'T, ?someotherotherparam: 'T): 'T =
    TYPE.someMethod(someparam, options = SomeMethodOptions(?someotherparam = someotherparam, ?someotherotherparam = someotherotherparam))
```

### Pojo constructors

A `JS.Pojo` constructor call compiles to a plain object literal, without the optional arguments you leave out.

You can also set a property that is not a constructor parameter in the same call, as `size` is set below. F# compiles
that to a constructor call followed by a setter; inside a `[<SolidComponent>]` the plugin folds the setters into the
literal (`ComponentFlag.SkipPojoOptimisation` turns that off):

```fsharp solid render=BadgeDemo jsx
[<JS.Pojo>]
type BadgeOptions(label: string, ?tone: string) =
    member val label: string = label with get, set
    member val tone: string = JS.undefined with get, set
    member val size: string = JS.undefined with get, set

[<SolidComponent>]
let BadgeDemo () =
    let options = BadgeOptions("new", tone = "green", size = "small")
    code () { JS.JSON.stringify options }
```

Prebuilt bindings such as ApexCharts and TanStack Table use Pojo constructors for all their options.
