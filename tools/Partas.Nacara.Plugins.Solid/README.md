# Partas.Nacara.Plugins.Solid

`Partas.Nacara.Plugins.Solid` turns an F# fence into a working
[Partas.Solid](https://github.com/shayanhabibi/Partas.Solid) example. When the
site builds, the plugin compiles the fence with Fable and the Partas.Solid
compiler plugin. It then passes the result through the Solid JSX compiler and
mounts what it renders under the code.

For example:

```fsharp solid render=Counter
[<SolidComponent>]
let Counter () =
    let count, setCount = createSignal 0

    button (onClick = fun _ -> setCount (count () + 1)) {
        $"Clicked {count ()} times"
    }
```

Nothing above runs in F# in the browser. The page loads a small bundle of what
Fable and Solid made of the fence.

## Registering it

```fsharp
open Nacara.Plugins

let site =
    Site.create "My site"
    |> Markdown.register
    |> SolidExamples.registerWith (
        SolidExamples.partasVersion "3.0.0"
    )
```

Partas.Solid's own docs (`docs/site`) reference this project and compile the examples against the repository's
`src/Partas.Solid` with `SolidExamples.partasProject`, so the site always shows the current sources.

The plugin needs `dotnet` and Node 22.12 or later on the `PATH`. On the first
build it restores Fable and installs `solid-js`, `@solidjs/web`,
`@solidjs/compiler` and `rolldown`. It keeps them in `.nacara/partas-solid`,
next to your site, so later builds reuse them. When no example on the site has
changed, a build skips the compilers entirely. Under `nacara watch` the plugin
keeps Fable running, so saving a page recompiles only that page's examples.
Fable runs with `--typedArrays false`, as Partas.Solid requires, so an `int[]`
is a plain array that Solid stores can wrap.

## Writing examples

Put `solid` after the language of an `fsharp` fence. Every fence on a page
compiles into one module, in page order, so a later fence can use what an
earlier one declared. `Partas.Solid`, `Fable.Core` and `Fable.Core.JsInterop` are
already open.

What the fence renders depends on what it contains:

- **An expression** renders as it stands. `Counter ()` below reuses the
  component from the top of the page.
- **Declarations** render nothing on their own. Add `render=Name` to mount the
  component called `Name`.
- **`setup`** compiles the fence but leaves it off the page. Use it for
  helpers the reader does not need to see.

```fsharp solid
div (class' = "flex gap-2") {
    Counter ()
    Counter ()
}
```

Each counter keeps its own signal, as it would in any Solid app.

```fsharp solid setup
type Todo = { Id: int; Text: string; Done: bool }
```

The fence above declared a `Todo` record. It compiled with the page but does not
appear on it. The next fence uses it and names the component to mount:

```fsharp solid render=TodoList jsx
[<SolidComponent>]
let TodoList () =
    let todos, setTodos =
        createSignal [ { Id = 1; Text = "Write docs"; Done = true }
                       { Id = 2; Text = "Ship it"; Done = false } ]
    let draft, setDraft = createSignal ""

    let add () =
        if draft () <> "" then
            setTodos (todos () @ [ { Id = todos().Length + 1; Text = draft (); Done = false } ])
            setDraft ""

    let toggle id =
        todos ()
        |> List.map (fun t -> if t.Id = id then { t with Done = not t.Done } else t)
        |> setTodos

    div () {
        form (onSubmit = fun e -> e.preventDefault (); add ()) {
            input (value = draft (), onInput = fun e -> setDraft !!e.currentTarget?value)
            button (type' = "submit") { "Add" }
        }
        ul () {
            For.Keyed(each = List.toArray (todos ())) {
                yield fun todo _ ->
                    li (
                        onClick = (fun _ -> toggle todo.Id),
                        style = (if todo.Done then "text-decoration: line-through" else "")
                    ) { todo.Text }
            }
        }
    }
```

### Choosing what shows

`show=output` keeps the code off the page and renders only the result.
`show=code` does the opposite: the fence still compiles, so a broken example
still fails the build, but nothing is mounted.

```fsharp solid show=output
p () { "Only the output of this fence is on the page." }
```

`show=inline` renders only the result too, and drops the box around it, so
what it renders reads as part of the page:

```fsharp solid show=inline
p (style = "font-style: italic") { "No box, no code: this paragraph came from F#." }
```

### Showing the JSX

Add `jsx` to a fence to show its code in two tabs: **F#**, as written, and
**JSX**, what Fable and the Partas.Solid plugin made of it before the Solid
compiler turned it into DOM code. The JSX is coloured like any `jsx` fence. Use
it to show readers what Partas.Solid does with a piece of F#.

```fsharp solid jsx
let name = "Solid"

p (class' = "greeting") { $"Hello, {name}!" }
```

The `TodoList` fence above is marked `jsx` too, so its JSX tab shows the
declared component rather than a wrapper.

The JSX tab holds only the cell's own functions:

- For an expression, that is the wrapper component the plugin generated for it.
- For declarations, it is each name the fence declares at column zero (`let`,
  `type` and `and`). When Fable wrote none of them, the tab shows the
  `render=` wrapper instead.

Imports, and anything Fable generated beside a declaration (such as a record's
`_$reflection` function), are left out. `setup` fences never get the tabs, and
a `show=output` fence gets only the JSX one.

`SolidExamples.showJsx true` gives every fence on the site the tabs. Fable runs
after the markdown is rendered, so the plugin puts the coloured JSX into each
page as it is written out: the page carries it, and nothing is fetched.

## Components in the prose

A code span that starts with `solid:` renders the expression after the colon
where the span stands, in the middle of a sentence or a list item. Declare a
component in a fence, often a `setup` one, and use it anywhere on the page:

```fsharp solid setup
[<SolidComponent>]
let Kbd (key: string) =
    kbd (style = "padding: 0 .35em; border: 1px solid currentColor; border-radius: .25em; font-size: .85em") {
        key
    }

[<SolidComponent>]
let Clicks () =
    let count, setCount = createSignal 0
    button (onClick = fun _ -> setCount (count () + 1)) { $"clicked {count ()}" }
```

Press `solid: Kbd "Ctrl"` + `solid: Kbd "C"` to copy. This button,
`solid: Clicks ()`, keeps its own count inside the sentence.

On GitHub, or anywhere else that knows nothing of the plugin, a use still reads
as the code it runs. A use can name a component declared further down the
page, because uses compile after every fence. Set `fenceToken` and the prefix
follows it.

### Naming a cell

Cells are numbered `c1`, `c2` and so on. Add `id=name` to give one a stable
name. The name appears on the placeholder element, which is the handle for
styling one example on its own. Uses in the prose are numbered `u1`, `u2` and
so on.

## When an example is wrong

Fable's errors point at the line of the markdown file they came from, not at
the generated module. `nacara build` fails on them, like it does on a broken
link. `nacara watch` reports them as warnings and keeps serving, so you can fix
the fence and save again.

## Using other packages

Examples compile against Partas.Solid and nothing else. To use more, name the
packages when you register the plugin. `nuget` adds a NuGet package, such as F#
bindings for a library. `npm` adds an npm package for the bundle to import.

```fsharp
let site =
    Site.create "My site"
    |> Markdown.register
    |> SolidExamples.registerWith (
        SolidExamples.nuget "Fable.Browser.Dom" "2.18.0"
        >> SolidExamples.npm "animejs" "3.2.2"
    )
```

Each call adds one package. Call either again with the same name and the later
version replaces the earlier one. Adding, changing or removing a package makes
the next build install and compile again.

An example then imports from the npm package as any Fable code does:

```fsharp
[<Import("default", "animejs")>]
let anime: obj -> unit = jsNative
```

The examples run on Solid 2. A library built on Solid 1.x will not work: the
plugin holds `solid-js` and `@solidjs/web` at `solidVersion` for every package,
so such a library gets Solid 2 in place of the Solid it expects. Libraries that
do not use Solid at all, like `animejs`, are fine.

## Options

| Option | Default | |
| --- | --- | --- |
| `partasVersion` | `"3.*"` | The Partas.Solid package the examples compile against |
| `partasProject` | none | A `Partas.Solid.fsproj` to compile against in place of the package |
| `feed` | none | Adds a NuGet source; a local folder works |
| `nuget` | none | Adds a NuGet package the examples compile against |
| `npm` | none | Adds an npm package the examples can import |
| `fableVersion` | `"5.13.0"` | |
| `solidVersion` | `"2.0.0-rc.14"` | `solid-js`, `@solidjs/web` and `@solidjs/compiler` |
| `fenceToken` | `"solid"` | The word that marks a fence, and the prefix of a use in the prose |
| `prelude` | the three `open`s above | Lines at the top of every page's module |
| `outputPath` | `"_partas/solid"` | Where the bundles go in the output |
| `workspacePath` | `".nacara/partas-solid"` | Where the generated project lives |
| `minify` | `true` | |
| `showJsx` | `false` | Adds a JSX tab to every fence, as if each were marked `jsx` |
