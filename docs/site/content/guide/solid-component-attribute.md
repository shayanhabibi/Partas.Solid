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

```fsharp solid jsx
[<SolidComponent>]
let Label (text: string) =
    span (style = "font-weight: bold") { text }

[<SolidComponent>]
let Framed (text: string) =
    div (style = "border: 1px solid currentColor; padding: .5rem") { Label text }
```

```fsharp solid jsx
div () {
    Label "Plain"
    Framed "Framed"
}
```

A binding that only calls another component, and builds no tags itself, does not need the attribute:

```fsharp
let Render () = "Button" |> Framed
```

## Calls are untracked

A `[<SolidComponent>]` binding compiles to a JavaScript function, and a call to it compiles to a function call inside
the parent's JSX, not to a `<Label />` element. The JSX tab above shows `untrack(Label.bind(undefined, "Plain"))`. The
arguments are positional, so the call cannot become a tag, but `untrack` gives the body the same semantics as
`<Label />`: a signal read in the body is a snapshot, and a signal read in the returned JSX stays live, as in a
[SolidTypeComponent](solid-type-attribute.md).

The arguments are evaluated outside the `untrack`. `Label (name ())` reads `name` in the parent's JSX, so when it
changes the call runs again with the new value and the body runs again with it.

## How it works

The plugin walks the body of the binding and replaces each tag, with its properties and children, by a JSX element.
It removes element-builder plumbing and optimises `[<Pojo>]` constructors. Recognized attribute array computations
become readable array literals and conditional spreads while retaining ordinary F# collection and function semantics.
[Attribute Flags](attribute-flags.md) control optional transformations and diagnostics.

## Children and builder scope

Explicit and implicit yields render the same children. Conditional and matched branches can contain several children,
and local bindings keep their scope across following siblings. The plugin groups these children in JSX fragments.
Expressions discarded with `do`, `ignore`, or a unit cast still contribute their values as children; unit-valued
effects execute in source order and render no content.

Every helper that constructs DSL tags needs its own `[<SolidComponent>]` attribute, including helpers in other files.
The caller's attribute transforms the call site; it cannot reconstruct a helper's unavailable implementation.
Ordinary function values and external functions that already return JSX can be passed as usual.

Ordinary element builders support `Yield`, `Combine`, `Zero`, `Delay`, `Run`, and `Bind` for `match!`. Use `For.Keyed` or `For.NonKeyed`
for iteration. Direct `for`, `while`, `try/with`, `try/finally`, and collection yielding require builder members the
ordinary element builder does not provide. Ordinary F# collections and protected expressions outside the element CE
remain supported. Reserve identifiers beginning with `PARTAS_` for compiler scaffolding.

The plugin requires Fable 5 or newer, JavaScript output, and a `.js` or `.jsx` extension. The regression suite uses
Fable 5.13.0; the differential JSX corpus also verifies Fable 5.21.0.

## Reactive pattern matching

Inside an element builder, `match!` generates a Solid `Switch` with a non-keyed `Match` for each arm:

```fsharp solid jsx
[<SolidComponent>]
let Status () =
    let message, setMessage = createSignal (Some "Ready")
    div () {
        match! message () with
        | Some text -> span () { text }
        | None -> b () { "Waiting" }
    }
```

Supply a reactive expression such as `message ()`; a static value stays static. Changing the value within the same
arm updates its pattern-bound values while retaining that arm's DOM. Selecting another arm replaces its content.
Pattern-bound reads in JSX and event callbacks use the selected Match's accessor.

The plugin shares the source evaluation, guards, and pattern captures through a generated selection memo declared
before the JSX in the surrounding component or render-callback scope. This avoids repeating a pattern decision
for every Match. The memo tracks dependencies read by your expression; it does not turn ordinary values into signals.
An ordinary `match` keeps its existing expression-based output. `match!` here is synchronous control flow, not an
asynchronous computation.

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
