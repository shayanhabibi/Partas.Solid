---
title: Benefits of JSX output
---

Partas.Solid does not compile your components to plain JavaScript. It compiles them to JSX, and the Solid JSX
compiler takes it from there. The JSX it writes is meant to be read. Here is a component from the plugin's snapshot
tests, next to what it compiles to:

```fsharp
[<Erase>]
type AccordionItem() =
    inherit button()

    [<SolidTypeComponent>]
    member props.constructor =
        button(class' = Lib.cn [| "border-b"; props.class' |]).spread props
```

```jsx
import { omit } from "solid-js";
import { twMerge } from "tailwind-merge";
import { clsx } from "clsx";

export function AccordionItem(props) {
    const PARTAS_OTHERS = omit(props, "class");
    return <button class={twMerge(clsx(["border-b", props.class]))}
        {...PARTAS_OTHERS} n$={false} />;
}
```

The component keeps its name. Props are read as `props.class`, so Solid can track them. The props you read are left
out of the spread with `omit` from `solid-js`. `n$={false}` is the marker the plugin puts after a spread; it does
nothing at runtime.

The rest of this page is about why that matters.

## Onboarding JavaScript developers to F#

The Oxpecker style of DSL is already close to what a JavaScript developer knows. A constructor is the opening tag, its
arguments are the attributes, and the computation expression after it is what sits between the opening and closing
tags.

Add `jsx` to a fence on this site and you get a second tab with the JSX. Try it here:

```fsharp solid jsx
div (class' = "flex gap-2") {
    button (onClick = fun _ -> JS.console.log "Clicked!") { "Log to console" }
}
```

A JavaScript developer can learn the quirks of F# and Fable by checking what their code turns into, in a language
they already read.

Components you write compile the same way. A default value set in the body turns into a `merge` from `solid-js`:

```fsharp solid jsx render=BadgeDemo
[<Erase>]
type Badge() =
    inherit span()

    [<Erase>]
    member val tone: string = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        props.tone <- "info"
        span(class' = "badge badge-" + props.tone).spread props { props.children }

[<SolidComponent>]
let BadgeDemo () =
    div () {
        Badge() { "Default tone" }
        Badge(tone = "warning") { "Warning tone" }
    }
```

The self identifier does not have to be `props`. Name it `this` or anything else and the compiled function uses
that name (Fable writes `this` as `this$`, since `this` is reserved in JavaScript).

### Dropping a dependency

F# can also replace a JavaScript dependency outright. Class-variance helpers such as CVA exist because JavaScript
has no pattern matching. F# does, so a variant is a union and a `match`:

```fsharp solid jsx render=ButtonRow
[<StringEnum; RequireQualifiedAccess>]
type Variant =
    | Primary
    | Outline
    | Ghost

let variantStyle (variant: Variant) =
    let colours =
        match variant with
        | Variant.Primary -> "background: #2563eb; color: white; border: 1px solid #2563eb"
        | Variant.Outline -> "background: transparent; border: 1px solid currentColor"
        | Variant.Ghost -> "background: transparent; border: 1px solid transparent"

    "padding: .25rem .75rem; border-radius: .375rem; " + colours

[<Erase>]
type VariantButton() =
    inherit button()

    [<Erase>]
    member val variant: Variant = unbox null with get, set

    [<SolidTypeComponent>]
    member props.View =
        props.variant <- Variant.Primary
        button(style = variantStyle props.variant).spread props { props.children }

[<SolidComponent>]
let ButtonRow () =
    div (style = "display: flex; gap: .5rem") {
        VariantButton() { "Primary" }
        VariantButton(variant = Variant.Outline) { "Outline" }
        VariantButton(variant = Variant.Ghost) { "Ghost" }
    }
```

The compiler checks every variant. Add a case to `Variant` and each `match` that misses it warns.

## Enterprise migrations to F# and Fable

You can move components to Fable one at a time, or write new ones in F#, and the output is still something the rest
of the team recognises and can use. Storybook support (see [Storybook](../guide/storybook.md)) helps here too.

## Publishing as a JavaScript package

In enterprise and open-source work alike, a readable source can decide whether people use your package. When users
can read the output, they can send issues and pull requests against it.

The DSL is close to JSX, so it takes little effort to carry a change from a JSX pull request back to the F# source.

## Solid's optimisations and helpers

Solid does its optimisation in its JSX compiler: it turns templates into cloned DOM nodes, and wraps each dynamic
expression so that only that expression updates. Partas.Solid hands its output to that compiler, so your F# gets the
same treatment as hand-written Solid. Tools and helpers built to work on JSX work on it too.

## Fewer surprises while debugging

Sooner or later something in your UI goes wrong. When the output is clean JSX, you can find the cause much faster.

You are not limited to the size of the F# or Fable communities, either. You can take a reproducible JSX example to
the much larger pool of front-end developers and ask for help.

It also helps you tell a bug in your code from a bug in Fable, F# or the plugin: read the output and check whether it
says what you meant. If it does not, [submit an issue](../contributing/submit-issues.md).

## F# and JavaScript developers working together

This follows from the sections above, but it is worth saying on its own. Teams push back less on F# and Fable, with
the safety and domain modelling they bring, when the JavaScript developers can read and use the output. The same is
true when you work with a design team.

## Other thoughts

Plenty of development tools work on JSX source. Plenty of tools also produce JSX, and the gap between JSX and the
Oxpecker style of DSL is small, whether a person or an LLM is doing the translating.

## Comparing Fable/Feliz output with Partas.Solid

Here is a sidebar, compiled with Feliz:

```js
function AppSideBar() {
    let el, el_2, el_4, el_6, children_1_4, children_1_2, children_1_1, children_1, children_1_3;
    const items = ofArray([{
        icon: (el = House, createElement(el, {})),
        title: "Home",
        url: "#",
    }, {
        icon: (el_2 = Inbox, createElement(el_2, {})),
        title: "Inbox",
        url: "#",
    }, {
        icon: (el_4 = Search, createElement(el_4, {})),
        title: "Calendar",
        url: "#",
    }, {
        icon: (el_6 = Settings, createElement(el_6, {})),
        title: "Settings",
        url: "#",
    }]);
    return createElement(Sidebar_1, {
        collapsible: "icon",
        children: (children_1_4 = ofArray([(children_1_2 = ofArray([createElement(SidebarGroupLabel_1, {
            children: "Application",
        }), (children_1_1 = singleton((children_1 = toList(delay(() => map((item) => {
            let elems_1, elems;
            return createElement(SidebarMenuItem_1, createObj(ofArray([["key", item.title], (elems_1 = [createElement(SidebarMenuButton_1, {
                asChild: true,
                tooltip: item.title,
                children: createElement("a", createObj(ofArray([["href", item.url], (elems = [item.icon, createElement("span", {
                    children: [item.title],
                })], ["children", reactApi.Children.toArray(Array.from(elems))])]))),
            })], ["children", reactApi_1.Children.toArray(Array.from(elems_1))])])));
        }, items))), createElement(SidebarMenu_1, {
            children: reactApi.Children.toArray(Array.from(children_1)),
        }))), createElement(SidebarGroupContent_1, {
            children: reactApi.Children.toArray(Array.from(children_1_1)),
        }))]), createElement(SidebarGroup_1, {
            children: reactApi.Children.toArray(Array.from(children_1_2)),
        })), (children_1_3 = singleton(defaultOf()), createElement(SidebarRail, {
            children: reactApi.Children.toArray(Array.from(children_1_3)),
        }))]), createElement(SidebarContent_1, {
            children: reactApi.Children.toArray(Array.from(children_1_4)),
        })),
    });
}
```

And here is a smaller navigation list in Partas.Solid. Click an item, then open the JSX tab:

```fsharp solid jsx render=NavList
[<SolidComponent>]
let NavList () =
    let items = [| "Home"; "Inbox"; "Calendar"; "Settings" |]
    let selected, setSelected = createSignal "Home"

    ul () {
        For.Keyed(each = items) {
            yield fun item _ ->
                li (
                    onClick = (fun _ -> setSelected item),
                    style = (if selected () = item then "font-weight: bold" else "")
                ) { item }
        }
    }
```

If you suspected the compiled code rather than your own logic, you know which of the two you would rather debug.

:::note
This is not a criticism of Feliz, its authors, contributors or users. Work is under way on a Feliz that uses Fable's
newer JSX helpers, which would give it clean output too. The comparison only shows what Fable 5 and Partas.Solid
offer.
:::
