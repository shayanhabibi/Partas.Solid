---
title: The Oxpecker fork
---

:::note
This page is the project's history, told by its author. It is personal, and a bit of a ramble.
:::

## Oxpecker

[Oxpecker](https://github.com/lanayx/oxpecker) is a full-stack framework for F#. Its front-end DSL uses computation
expressions to template the DOM, and Oxpecker.Solid uses that DSL to compose native elements for Solid. I found
Oxpecker.Solid when a private project of mine reached a crossroads.

If you like Partas.Solid, [go and support Oxpecker](https://github.com/lanayx/oxpecker) too.

## Starting out with Feliz

I was building an app with the SAFE stack, which uses Elmish, React and Vite with a Feliz-style front end. I got to a
point where I found it harder and harder to see where things were happening in my DOM elements. The list style meant I
either made components too small or ended up deep in nested indentation.

This is some of my early Fable/Feliz code:

```fsharp
let Default () =
    // ...
    Html.div [
        prop.className "flex flex-row gap-5 p-2"
        prop.children [
            for size,label in sizes do
                UI.Button [
                    size
                    if label = "Icon" then
                        props.children (Icon.Image [])
                    else props.text label
                ]
            UI.Button [
                props.text "Disabled"
                props.disabled true
            ]
           // ...
        ]
    ]
```

I found it hard to take in at a glance. Add some React performance problems in fairly small single-page apps, which
a seasoned React developer would have avoided, and I was in for a bad ride. I am not a JavaScript developer, and never
intend to be.

## Discovering Oxpecker

Around then, the [Amplifying F#](https://amplifyingfsharp.io/) group ran a session with Vladimir Shchur, the author
of Oxpecker. That is where I found Oxpecker.Solid.

It was the first DSL I had seen that was not list based, and to me it was more intuitive. It was not the first time I
had seen Solid either, but it came just as I was fed up with trying to stop massive re-renders from eating my
performance. It was a match made in heaven.

I loved working with the DSL. It looked much more like the component examples I was finding online, and the compiled
output was **readable**. That mattered more than I expected. I depended on examples written in JSX and TSX, and often
had to write or update a binding. With readable output I could compare my code against those examples, or file an issue
with one, instead of guessing from a pile of prints. [Benefits of JSX output](jsx-output.md) covers why.

## The fork

Oxpecker let me write everything in F#, except for one thing: I wanted to make a **component library**.

No amount of functions, static parameters or optional method parameters got around one problem. As with Feliz, I
could not write a component and then use it through the Oxpecker DSL. The DSL was designed for native elements. It
also worked for imported library types, but if I wanted a Shadcn-style component library on top of a headless
component package, that was not supported.

### First contributions

So I started contributing. It was my first time working with the F# or Fable AST, and it took a while to get my head
around. I had complained about the nesting in Feliz; if you have seen a Fable AST printed to the console, you will
understand.

After a few clumsy attempts, I realised I *could* get what I wanted. Like an imported library tag, you would define
the properties on a type. Then you would write the component function with access to those properties, and the two
would be linked. That is a job for a member method.

I tried it on my fork, made some headway, and talked it over with Lanayx (Vladimir), along with some other features I
wanted. He liked the enthusiasm, but felt the larger API surface was not the direction he wanted for Oxpecker.Solid,
at least while he kept actively maintaining and supporting it. Compare the two plugins and you will see how much the
API and the plugin logic grew.

He gave his blessing to a harder fork. I am always grateful for his advice and opinions, and he still patiently
answers my sometimes absurd questions. I still contribute to Oxpecker.Solid, and will keep doing so.

The plugin logic and API have changed a great deal, but the bindings underneath started out very close to
Oxpecker's. The biggest difference was that Partas.Solid uses property getters and setters on elements and
components, where Oxpecker uses setters only. Partas.Solid 3.0 has since moved to Solid 2, while Oxpecker.Solid
targets Solid 1, so the two have drifted further apart.

## Continued support for Oxpecker

I make Oxpecker versions of bindings I write, such as Oxpecker.Solid.MotionOne and Oxpecker.Solid.Primitives, once
their API has seen real-world use and settled. Tracking fast changes across two plugins at once is laborious, and I
would rather not hand Vladimir Shchur something half-baked.

As I told Lanayx at the start, I am open at any time to bringing the plugin logic under Oxpecker and his ownership.
For now, keeping the plugins forked lets me iterate on features quickly without needing much oversight.
