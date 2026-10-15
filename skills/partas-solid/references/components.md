# Components and interop

These examples target Partas.Solid 3 and Solid 2. Copy namespace and import declarations with the code; the
`Partas.Solid` prefix on custom tag types is required by the plugin.

## Reusable tag and reactive view

```fsharp
namespace Partas.Solid.MyApp

open Fable.Core
open Partas.Solid

[<Erase>]
type StyledButton() =
    inherit button()

    [<Erase>]
    member val tone: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        props.tone <- "primary"
        button(
            class' = "button button-" + props.tone + " " + defaultArg (Option.ofObj props.class') "",
            type' = Attr.ButtonType.Button
        ).spread props { props.children }

module Views =
    [<SolidComponent>]
    let App () =
        let selected, setSelected = createSignal (Some "Alice")
        let rows, setRows = createSignal [| "First"; "Second" |]

        div () {
            StyledButton(onClick = fun _ -> setSelected (Some "Bob")) { "Change name" }
            StyledButton(onClick = fun _ -> setSelected None) { "Clear" }
            div () {
                match! selected () with
                | Some name ->
                    p () { $"Hello, {name}" }
                    input (placeholder = "This draft survives a name change")
                | None -> p () { "Choose a name" }
            }
            StyledButton(onClick = fun _ -> setRows (Array.rev (rows ()))) { "Reverse" }
            ul () {
                For.Keyed(each = rows ()) {
                    yield fun item index -> li () { $"{index ()}: {item}" }
                }
            }
        }
```

The type's public `View` property is in the same file as its declaration. `tone` is a custom erased prop.
Assigning it once in `View` supplies a default; the caller's prop wins. Reading `props.class'` and passing it
explicitly preserves the caller's classes alongside the component's classes.

`.spread props` forwards unused attributes and handlers. The plugin omits consumed props (`tone`, `class`, and
`children`) and renders the remaining props on the root button. An explicit default prop on that button can be
overridden by the later spread. The inherited typed HTML attributes remain available to callers.

Each tag-building function needs its own `[<SolidComponent>]`. Function helpers take ordinary positional
arguments; use a type component when the caller needs a stable component with reactive named props.

## Events, attributes, and refs

`class'`, `type'`, `for'`, and `when'` escape F# keywords. Closed attribute values use `Attr` unions, for example
`type' = Attr.ButtonType.Button`. Void elements such as `input` have no children block.

Events have DOM event types. Open `Fable.Core.TS` and qualify DOM types. Xantham exposes `currentTarget` as an
option: unwrap it before casting to the corresponding input element. Use the application's typed or interop
convention rather than assuming every event target has a `value` field.

```fsharp
open Fable.Core.TS

[<SolidComponent>]
let Search () =
    let query, setQuery = createSignal ""
    let mutable inputRef: Dom.HTMLInputElement = JS.undefined

    div () {
        input(
            value = query (),
            onInput = fun event ->
                match event.currentTarget with
                | Some target -> setQuery ((target :?> Dom.HTMLInputElement).value)
                | None -> ()
        ).ref (inputRef)
        button (onClick = fun _ -> inputRef.focus ()) { "Focus" }
        p () { query () }
    }
```

Chain extension methods directly after the constructor, as in `div(class' = "card").data("id", "42")`.
`div (class' = "card").data(...)` can bind the dot to the argument expression instead.
Use `.attr` for arbitrary attributes, `.data` for data attributes, `.style'` for style objects or pairs,
and `.classObj` for conditional class maps. Start DOM-dependent integrations in `onSettled` and return their
cleanup function when the integration needs disposal.

## Imported npm components

Bind the actual export and its props; inherited interfaces determine supported HTML attributes and children.
Keep custom tag/binding types under `Partas.Solid.*` too.

```fsharp
[<Import("Widget", "some-solid-package"); Erase>]
type Widget() =
    interface HtmlContainer

    [<Erase; DefaultValue>]
    val mutable label: string
```

`HtmlContainer` supplies children; `HtmlTag` supplies HTML attributes/events; `RegularNode` supplies both.
Use `[<PartasImport("Name", "package")>]` when Fable's normal import interferes with a type's own builder
members. For imported JavaScript functions, tupled parameters produce a plain multi-argument JS call.
Check the npm package's Solid major-version compatibility before installing bindings.

Sources: [type components](https://shayanhabibi.github.io/Partas.Solid/guide/solid-type-attribute/),
[extension methods](https://shayanhabibi.github.io/Partas.Solid/guide/extension-methods/),
[writing bindings](https://shayanhabibi.github.io/Partas.Solid/ecosystem/bindings/).
