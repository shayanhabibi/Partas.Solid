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

I found it hard to take in at a glance. Add some obvious React performance problems in fairly small single-page apps,
and I was in for a bad ride.

:::caution
Before you reach for the pitchforks: yes, there are many ways to optimise a React app, and yes, it was probably
because I was a rookie React and JavaScript developer. That is exactly the point. I am not a JavaScript developer, and
I never intend to be. And yet here I am, writing about a plugin for an F# to JavaScript compiler.
:::

## Discovering Oxpecker

Around then, the [Amplifying F#](https://amplifyingfsharp.io/) group ran a session with Vladimir Shchur, the author
of Oxpecker. That is where I found Oxpecker.Solid.

It was the first DSL I had seen that was not list based, and to me it was more intuitive. It was not the first time I
had seen Solid either, but it came just as I was fed up with trying to stop massive re-renders from eating my
performance. It was a match made in heaven.

I loved working with the DSL. It looked much more like the component examples I was finding online. I also loved that
the compiled output was **readable**.

:::info
I had never wanted to touch web development. I preferred working closer to the metal, or making desktop apps with Qt
and Nim. The project I had in mind made me change my ways.
:::

I was surprised that the compiled output mattered to me. But since I had set out to make a web app without touching
JavaScript, I depended on examples and guides written in JSX and TSX.

A lot of the bindings I found were also out of date. My project had particular needs, so I often had to write or update
a binding. Every problem I hit left me scratching my head, because it was hard to tell whether my code was even close
to right from its output. With all the black magic React needs to render quickly, I could not work out what was going
right and what was going wrong.

With readable output, I could search for an issue and compare my code against it, or even file an issue with an
example. I did not need a pile of prints or tests to see whether my code had compiled correctly. I could just look,
and copy from the examples online.

## The fork

Oxpecker let me stop needing to know JavaScript. I could write everything in F#, and if something went wrong on the
JavaScript side, I could use the huge amount of material online with source code in hand that I could read.

Except for one thing: I wanted to make a **component library**.

No amount of functions, static parameters or optional method parameters got around one problem. As with Feliz, I
could not write a component and then use it through the Oxpecker DSL. The DSL was designed for native elements. It
also worked for imported library types, but if I wanted a Shadcn-style component library on top of a headless
component package, that was not supported.

Part of why that mattered goes back to resembling JSX, so I could use all the front-end material out there. Part of it
was simply that this was **so close** to being exactly what I wanted.

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

I am happy to make Oxpecker versions of bindings I write, and I check with Lanayx about the ones I think he would want.
Oxpecker.Solid.MotionOne and Oxpecker.Solid.Primitives are two examples.

:::warning
I do not want to bring early bindings under the Oxpecker umbrella until they have seen some real-world use. I change
things quickly, and tracking and testing those changes for two plugins at once gets laborious. Once a binding's API is
stable, I finish the Oxpecker version. That is a matter of respect for Vladimir Shchur, and of not handing him
something half-baked.
:::

I spend a lot of time writing bindings and putting them through their paces. I am happy to contribute any of them
back, with better public APIs, while I carry on with other bindings and my own project.

As I told Lanayx at the start, I am open at any time to bringing the plugin logic under Oxpecker and his ownership.
For now, keeping the plugins forked lets me iterate on features quickly without needing much oversight.
