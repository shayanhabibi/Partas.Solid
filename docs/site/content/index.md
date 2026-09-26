---
title: Partas
layout: splash
---

<section class="p-hero">
<img class="p-hero__logo" src="/Partas.Solid/logo.png" alt="" width="88" height="88">
<a class="p-hero__eyebrow" href="/Partas.Solid/guide/migrating-to-solid-2/"><span class="p-hero__eyebrow-tag">3.0</span> Partas.Solid now targets Solid 2 <span aria-hidden="true">→</span></a>
<h1>Solid components,<br><span class="p-hero__gradient">written in F#</span></h1>
<p>A typed F# DSL and a Fable compiler plugin that turns your components into the JSX Solid expects, with no runtime of its own.</p>
<div class="p-hero__actions">
<a class="p-btn p-btn--primary" href="/Partas.Solid/guide/">Get started <svg aria-hidden="true" viewBox="0 0 24 24" width="16" height="16" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M5 12h14"/><path d="m12 5 7 7-7 7"/></svg></a>
<a class="p-btn p-btn--secondary" href="/Partas.Solid/about/jsx-output/">See the JSX output</a>
<a class="p-btn p-btn--secondary" href="https://github.com/shayanhabibi/Partas.Solid"><svg aria-hidden="true" viewBox="0 0 24 24" width="16" height="16" fill="currentColor"><path d="M12 .5a11.5 11.5 0 0 0-3.64 22.41c.58.1.79-.25.79-.56v-2c-3.2.7-3.88-1.37-3.88-1.37-.52-1.33-1.28-1.69-1.28-1.69-1.05-.72.08-.7.08-.7 1.16.08 1.77 1.19 1.77 1.19 1.03 1.77 2.7 1.26 3.36.96.1-.75.4-1.26.73-1.55-2.55-.29-5.24-1.28-5.24-5.69 0-1.26.45-2.28 1.19-3.09-.12-.29-.52-1.46.11-3.05 0 0 .97-.31 3.17 1.18a11 11 0 0 1 5.77 0c2.2-1.49 3.17-1.18 3.17-1.18.63 1.59.23 2.76.11 3.05.74.81 1.19 1.83 1.19 3.09 0 4.42-2.7 5.39-5.26 5.68.41.36.78 1.06.78 2.14v3.17c0 .31.21.67.8.56A11.5 11.5 0 0 0 12 .5Z"/></svg> GitHub</a>
</div>
<div class="p-install"><code>dotnet add package Partas.Solid <span class="p-install__nowrap">--version 3.0.0</span></code></div>
<p class="p-install__note">3.0 is a prerelease and is not on NuGet yet. <a href="/Partas.Solid/guide/installation/">Install it from a local feed</a>.</p>
</section>

## Try it

Every example on this site is compiled from its F# when the site is built. Fable and the Partas plugin produce JSX, Solid's compiler turns it into DOM code, and it runs in your browser. This one also uses anime.js from npm.

```fsharp solid setup
open Browser.Types

[<Import("animate", "animejs")>]
let animate (targets: obj, parameters: obj): obj = jsNative

[<Import("stagger", "animejs")>]
let stagger (step: int, parameters: obj): obj = jsNative
```

<div class="p-demo">

```fsharp solid render=HeroCounter jsx
[<SolidComponent>]
let HeroCounter () =
    let count, setCount = createSignal 0
    let mutable number: HTMLSpanElement = JS.undefined
    let mutable bars: HTMLDivElement = JS.undefined

    let pop = {| scale = {| from = 1.4; ``to`` = 1 |}; ease = "outElastic(1, .5)" |}
    let wave =
        {| scaleY = {| from = 1; ``to`` = 2.6 |}; duration = 240
           alternate = true; loop = 1
           delay = stagger (30, {| from = "center" |}) |}

    let bump () =
        setCount (count () + 1)
        animate (number, pop) |> ignore
        animate (bars.children, wave) |> ignore

    div (class' = "p-demo__stage") {
        span(class' = "p-demo__count").ref (number) { $"{count ()}" }
        div(class' = "p-demo__bars").ref (bars) {
            Repeat(count = 13) { yield fun _ -> span () }
        }
        button (class' = "p-btn p-btn--accent", onClick = fun _ -> bump ()) {
            "Increment"
        }
    }
```

</div>

## Why Partas.Solid

Write the whole front end in F#, and ship ordinary Solid code.

<div class="p-cards">
<a class="p-card" href="/Partas.Solid/guide/oxpecker-dsl/"><span class="p-card__icon"><svg aria-hidden="true" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M8 3H7a2 2 0 0 0-2 2v5a2 2 0 0 1-2 2 2 2 0 0 1 2 2v5c0 1.1.9 2 2 2h1"/><path d="M16 21h1a2 2 0 0 0 2-2v-5c0-1.1.9-2 2-2a2 2 0 0 1-2-2V5a2 2 0 0 0-2-2h-1"/></svg></span><strong class="p-card__title">Typed DSL</strong><p>Tags, attributes, ARIA and SVG are typed F#. A misspelled attribute is a compile error.</p></a>
<a class="p-card" href="/Partas.Solid/about/jsx-output/"><span class="p-card__icon"><svg aria-hidden="true" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="16 18 22 12 16 6"/><polyline points="8 6 2 12 8 18"/></svg></span><strong class="p-card__title">Compiles to JSX</strong><p>The plugin rewrites the F# AST into JSX, and Solid's compiler does the rest.</p></a>
<a class="p-card" href="/Partas.Solid/guide/solid-js/"><span class="p-card__icon"><svg aria-hidden="true" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M13 2 3 14h9l-1 8 10-12h-9l1-8z"/></svg></span><strong class="p-card__title">Solid 2 APIs</strong><p>Signals, memos, split effects, stores, Loading, Errored and actions are all bound.</p></a>
<a class="p-card" href="/Partas.Solid/guide/solid-type-attribute/"><span class="p-card__icon"><svg aria-hidden="true" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21 8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16Z"/><path d="m3.3 7 8.7 5 8.7-5"/><path d="M12 22V12"/></svg></span><strong class="p-card__title">Components as types</strong><p>With [&lt;SolidTypeComponent&gt;], props get defaults, and the plugin writes the merge and omit for you.</p></a>
<a class="p-card" href="/Partas.Solid/ecosystem/bindings/"><span class="p-card__icon"><svg aria-hidden="true" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="m7.5 4.27 9 5.15"/><path d="M21 8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16Z"/><path d="m3.3 7 8.7 5 8.7-5"/><path d="M12 22V12"/></svg></span><strong class="p-card__title">Any npm library</strong><p>Bind a package with [&lt;Import&gt;] and drive it through a ref, as the demo above does.</p></a>
<a class="p-card" href="/Partas.Solid/about/oxpecker-fork/"><span class="p-card__icon"><svg aria-hidden="true" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="18" r="3"/><circle cx="6" cy="6" r="3"/><circle cx="18" cy="6" r="3"/><path d="M18 9v2c0 .6-.4 1-1 1H7c-.6 0-1-.4-1-1V9"/><path d="M12 12v3"/></svg></span><strong class="p-card__title">Oxpecker heritage</strong><p>An opinionated fork of Oxpecker.Solid. Components you write use the same DSL as native tags.</p></a>
</div>

## The Partas family

Open source F# front-end projects, built on Fable and Solid.

<div class="p-cards p-cards--family">
<a class="p-card" href="/Partas.Solid/guide/"><span class="p-card__icon"><svg aria-hidden="true" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="m12.83 2.18a2 2 0 0 0-1.66 0L2.6 6.08a1 1 0 0 0 0 1.83l8.58 3.91a2 2 0 0 0 1.66 0l8.58-3.9a1 1 0 0 0 0-1.83Z"/><path d="m22 17.65-9.17 4.16a2 2 0 0 1-1.66 0L2 17.65"/><path d="m22 12.65-9.17 4.16a2 2 0 0 1-1.66 0L2 12.65"/></svg></span><strong class="p-card__title">Partas.Solid</strong><p>The DSL and the compiler plugin. Version 3.0 targets Solid 2.</p></a>
<a class="p-card" href="/Partas.Solid/ecosystem/"><span class="p-card__icon"><svg aria-hidden="true" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="m7.5 4.27 9 5.15"/><path d="M21 8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16Z"/><path d="m3.3 7 8.7 5 8.7-5"/><path d="M12 22V12"/></svg></span><strong class="p-card__title">Ecosystem</strong><p>Bindings for Solid libraries, and a port of shadcn/ui.</p><span class="p-card__tag">Not on Solid 2 yet</span></a>
<a class="p-card" href="https://github.com/shayanhabibi/partas.animejs"><span class="p-card__icon"><svg aria-hidden="true" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M9.94 15.5A2 2 0 0 0 8.5 14.06l-6.14-1.58a.5.5 0 0 1 0-.96L8.5 9.94A2 2 0 0 0 9.94 8.5l1.58-6.14a.5.5 0 0 1 .96 0L14.06 8.5A2 2 0 0 0 15.5 9.94l6.14 1.58a.5.5 0 0 1 0 .96L15.5 14.06a2 2 0 0 0-1.44 1.44l-1.58 6.14a.5.5 0 0 1-.96 0z"/></svg></span><strong class="p-card__title">Partas.AnimeJs</strong><p>F# bindings for anime.js, the library animating the demo above.</p></a>
</div>

<section class="p-cta">
<strong class="p-cta__title">Write your first component</strong>
<p>Set up Fable, the packages and Vite, then learn the two ways to write a component.</p>
<div class="p-hero__actions">
<a class="p-btn p-btn--primary" href="/Partas.Solid/guide/installation/">Installation</a>
<a class="p-btn p-btn--secondary" href="/Partas.Solid/guide/overview/">Overview</a>
</div>
</section>
