---
title: "match! expression"
---

Inside an element builder, `match!` turns F# pattern matching into Solid control flow. The plugin generates a
`Switch` with a non-keyed `Match` for each arm, so changing a payload within the selected arm updates its content
while retaining its DOM.

Use it inside a [`[<SolidComponent>]`](solid-component-attribute.md) function or a
[`[<SolidTypeComponent>]`](solid-type-attribute.md) member.

## Try it

Type into the input, then switch between Alice and Bob. Both values select the `Some` arm, so the name updates
and the input keeps what you typed. Choose Clear to select the `None` arm; choosing a name again creates a fresh
input.

```fsharp solid render=MatchExample jsx
[<SolidComponent>]
let MatchExample () =
    let user, setUser = createSignal (Some "Alice")

    div () {
        button (onClick = fun _ -> setUser (Some "Alice")) { "Alice" }
        button (onClick = fun _ -> setUser (Some "Bob")) { "Bob" }
        button (onClick = fun _ -> setUser None) { "Clear" }
        div () {
            match! user () with
            | Some name ->
                p () { $"Hello, {name}" }
                input (placeholder = "Type something to keep between names")
            | None -> p () { "Choose a name" }
        }
    }
```

The JSX tab shows the generated `Switch`, `Match` arms, and accessor reads for `name`.

## Patterns and guards

Use ordinary F# patterns, including union cases, tuples, active patterns, and guards. Arms can contain several
children; the plugin groups them in JSX fragments. The first matching arm wins.

```fsharp solid jsx
[<SolidComponent>]
let NameBadge (user: unit -> string option) =
    div () {
        match! user () with
        | Some name when name.Length > 3 -> strong () { name }
        | Some name -> span () { name }
        | None -> i () { "Anonymous" }
    }
```

A guard is part of the arm selection. For example, changing `"Alice"` to `"Bob"` in `NameBadge` selects a different
arm and replaces its content, even though both values are `Some`.

## Reactivity and arm lifetime

Supply a reactive expression such as `user ()`; a static value stays static. The generated code tracks the
dependencies your expression and guards read. It does not turn ordinary values into signals.

While the same arm remains selected, its pattern-bound values stay live through the selected Match's accessor.
Reads in JSX and event callbacks see the current payload. Switching arms disposes the old arm and constructs
the new one.

An ordinary `match` keeps its existing expression-based output. Choose `match!` when you want Solid's `Switch` and
`Match` arm lifetime. This element-builder syntax is synchronous; it does not await asynchronous computations.

## Shared selection in the output

The plugin evaluates the source, guards, and pattern captures through one generated selection memo. This shares
the F# pattern decision across the Match arms instead of repeating it in each `when` expression.

The memo is declared before the JSX in the surrounding component or render-callback scope. It is lazy, so moving
the declaration there does not evaluate the source early. Payload reads remain inside JSX expressions or event
callbacks, where they can use the selected arm's accessor.
