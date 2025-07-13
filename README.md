<div id="top"></div>

<br />


<div align="center">
  <a href="https://github.com/shayanhabibi/Partas.Solid" target="_blank">
  <img src="https://github.com/shayanhabibi/Partas.Solid/blob/master/Public/Partas_d00b_00a%20icon.png" height="42px"/>
  </a>
<h3 align="center">Partas.Solid</h3>
  <p align="center">
    <img src="https://www.solidjs.com/img/logo/without-wordmark/logo.svg" height="24px" style="border-radius:8px;" />
    <kbd>Solid-JS wrapper in Oxpecker style.</kbd>
    <img src="https://fsharp.org/img/logo/fsharp256.png" height="24px" />
  </p>
</div>

<div align="center">

[![Scc Count Badge](https://sloc.xyz/github/shayanhabibi/Partas.Solid/?category=code&badge-bg-color=9100FF)](https://github.com/shayanhabibi/Partas.Solid/)
[![Scc Count Badge](https://sloc.xyz/github/shayanhabibi/Partas.Solid/?category=comments&badge-bg-color=5E00B5)](https://github.com/shayanhabibi/Partas.Solid/)
[![Scc Count Badge](https://sloc.xyz/github/shayanhabibi/Partas.Solid/?category=cocomo&badge-bg-color=3B0086)](https://github.com/shayanhabibi/Partas.Solid/)

</div>

---

## Getting Started

[See the docs to get started!](partas-solid.vercel.app/partas-solid)

~~[Legacy Docs](http://shayanhabibi.github.io/partas-solid-docs/)~~

# Related Repositories

Solid-ui (Shadcn port) [Partas.Solid.UI](https://github.com/shayanhabibi/Partas.Solid.UI)

Bindings for different libraries [Partas.Solid.Bindings](https://github.com/shayanhabibi/Partas.Solid.Bindings)

## Oxpecker.Solid

> This is an opinionated fork of [Oxpecker.Solid](https://github.com/lanayx/Oxpecker) that
> keeps the original DSL style, but more aggressively
> transforms F# input to produce correct JSX.
> 
> Please support the original release.


### Differences

[See the docs](partas-solid.vercel.app/partas-solid)

~~[Legacy Docs](http://shayanhabibi.github.io/partas-solid-docs/)~~


<details>
    <summary>Aggressive transformation of AST</summary>
<p>

### Reduce undefined behaviour

I feel this iteration has less specific pattern matchers, which prevents what might some deem as undocumented behaviour.

As an example, currently Oxpecker would perform the following conversion:

```fsharp
let mutable show = true

[<SolidComponent>]
let Button () =
    let this = button() {
        "some boiler plate"
    }
    div(class'="MyButton") {
        if show then this else ()
    }
```
```jsx
export let show = createAtom(true);

export function Button() {
    return <button>
        some boiler plate
    </button>;
}
```

As opposed to Partas:

```jsx
export let show = createAtom(true);

export function Button() {
    const this$ = <button>
        some boiler plate
    </button>;
    return <div class="MyButton">
        {show() ? this$ : undefined}
    </div>;
}
```

Using ternary conditional expressions in solid-js works, although there is
also the `<Match>` or `<Show>` tags.
</p>

<p align="right">(<a href="#top">back to top</a>)</p>
</details>

<details>
    <summary>
    Ability to define custom components/tags and use them in the same DSL style
    </summary>

<p>
Partas.Solid provides an extra attribute which can be applied to members of a Tag type definition using `props` as the self identifier. The self identifier, props, allows type safe access to your defined properties which can be set in Oxpecker style.

```fsharp
[<Erase>]
type MyCustomDiv() =
    inherit div()
    [<Erase>]
    member val bordered: bool = jsNative with get,set
    [<SolidTypeComponent>]
    member props.constructor =
    // the props self identifier is a requirement
    // the member name has no influence on output
        div(class' = if props.bordered then "border border-border" else "") { props.children }

[<SolidComponent>]
let App() =
    MyCustomDiv(bordered = true) {
        "Hello world!"
    }
```
</p>

<p align="right">(<a href="#top">back to top</a>)</p>

</details>

---

## Example

See the [Partas.Solid.UI.Playground](https://github.com/shayanhabibi/Partas.Solid.UI) for a comprehensive example of a component library built _ENTIRELY_ in F# using <kbd>Partas.Solid</kbd>, <kbd>Tailwind</kbd> and a host of other libraries like <kbd>TanStack Table</kbd> and <kbd>Kobalte</kbd>.

<details>

<summary>
A comprehensive component and example output
</summary>

```fsharp
[<Erase>]
type Sidebar() =
    inherit div()
    member val side: sidebar.Side = unbox null with get,set
    member val variant: sidebar.Variant = unbox null with get,set
    member val collapsible: sidebar.Collapsible = unbox null with get,set
    [<SolidTypeComponentAttribute>]
    member props.constructor =
        props.side <- Left
        props.variant <- sidebar.Sidebar
        props.collapsible <- Offcanvas
        let ctx = Context.useSidebar()
        let (isMobile, state, openMobile, setOpenMobile) = (ctx.isMobile, ctx.state, ctx.openMobile, ctx.openMobile)
        Switch() {
            Match(when' = (props.collapsible = sidebar.None)) {
                
                div(class' = Lib.cn [|
                    "test flex h-full w-[--sidebar-width] flex-col bg-sidebar text-sidebar-foreground"
                    props.class'
                |]).spread props
                    { props.children }
                
            }
            Match(when' = isMobile()) {
                
                Sheet( open' = openMobile(), onOpenChange = !!setOpenMobile )
                    .spread props {
                        SheetContent(
                            class' = "w-[--sidebar-width] bg-sidebar p-0 text-sidebar-foreground [&>button]:hidden",
                            position = !!props.side
                            ).data("sidebar", !!sidebar.Sidebar)
                            .data("mobile", "true")
                            .style'(createObj [ "--sidebar-width" ==> sidebarWidthMobile ])
                            { div(class' = "flex size-full flex-col") { props.children } }
                    }
                
            }
            Match(when' = (isMobile() |> not)) {
                // gap handler on desktop
                div(
                class' = Lib.cn [|
                    "relative h-svh w-[--sidebar-width] bg-transparent transition-[width] duration-200 ease-linear"
                    "group-data-[collapsible=offcanvas]:w-0"
                    "group-data-[side=right]:rotate-180"
                    if (props.variant = sidebar.Floating || props.variant = sidebar.Inset) then
                        "group-data-[collapsible=icon]:w-[calc(var(--sidebar-width-icon)_+_theme(spacing.4))]"
                    else "group-data-[collapsible=icon]:w-[--sidebar-width-icon]"
                |]
                )
                
                div(
                class' = Lib.cn [|
                    "fixed inset-y-0 z-10 hidden h-svh w-[--sidebar-width] transition-[left,right,width] duration-200 ease-linear md:flex"
                    if props.side = sidebar.Left then
                        "left-0 group-data-[collapsible=offcanvas]:left-[calc(var(--sidebar-width)*-1)]"
                    else "right-0 group-data-[collapsible=offcanvas]:right-[calc(var(--sidebar-width)*-1)]"
                    // Adjust the padding for floating and inset variants.
                    if props.variant = sidebar.Floating || props.variant = sidebar.Inset then
                        "p-2 group-data-[collapsible=icon]:w-[calc(var(--sidebar-width-icon)_+_theme(spacing.4)_+2px)]"
                    else "group-data-[collapsible=icon]:w-[--sidebar-width-icon] group-data-[side=left]:border-r group-data-[side=right]:border-l"
                    props.class' 
                |]
                    ).spread props
                    {
                        div(
                            class' = "flex size-full flex-col bg-sidebar group-data-[variant=floating]:rounded-lg group-data-[variant=floating]:border group-data-[variant=floating]:border-sidebar-border group-data-[variant=floating]:shadow"
                        ).data("sidebar", !!sidebar.Sidebar)
                            { props.children }
                    }
            }
            
        }
```

```jsx

export function Sidebar(props) {
    props = mergeProps({
        side: "left", variant: "sidebar", collapsible: "offcanvas",
    }, props);
    const [PARTAS_LOCAL, PARTAS_OTHERS] = splitProps(props, ["collapsible", "class", "children", "side", "variant"]);
    const ctx = Context_useSidebar();
    const isMobile = ctx.isMobile;
    return <Switch>
        <Match when={PARTAS_LOCAL.collapsible === "none"}>
            <div
                class={twMerge(clsx(["test flex h-full w-[--sidebar-width] flex-col bg-sidebar text-sidebar-foreground", PARTAS_LOCAL.class]))}
                {...PARTAS_OTHERS} bool:n $={false}>
                {PARTAS_LOCAL.children}
            </div>
        </Match>
        <Match when={isMobile()}>
            <Sheet open={ctx.openMobile()}
                   onOpenChange={ctx.openMobile}
                   {...PARTAS_OTHERS} bool:n $={false}>
                <SheetContent class="w-[--sidebar-width] bg-sidebar p-0 text-sidebar-foreground [&>button]:hidden"
                              position={PARTAS_LOCAL.side}
                              data-sidebar="sidebar"
                              data-mobile="true"
                              style={{
                                  "--sidebar-width": sidebar_sidebarWidthMobile,
                              }}>
                    <div class="flex size-full flex-col">
                        {PARTAS_LOCAL.children}
                    </div>
                </SheetContent>
            </Sheet>
        </Match>
        <Match when={!isMobile()}>
            <div
                class={twMerge(clsx(["relative h-svh w-[--sidebar-width] bg-transparent transition-[width] duration-200 ease-linear", "group-data-[collapsible=offcanvas]:w-0", "group-data-[side=right]:rotate-180", ((PARTAS_LOCAL.variant === "floating") ? (true) : (PARTAS_LOCAL.variant === "inset")) ? ("group-data-[collapsible=icon]:w-[calc(var(--sidebar-width-icon)_+_theme(spacing.4))]") : ("group-data-[collapsible=icon]:w-[--sidebar-width-icon]")]))}/>
            <div
                class={twMerge(clsx(["fixed inset-y-0 z-10 hidden h-svh w-[--sidebar-width] transition-[left,right,width] duration-200 ease-linear md:flex", (PARTAS_LOCAL.side === "left") ? ("left-0 group-data-[collapsible=offcanvas]:left-[calc(var(--sidebar-width)*-1)]") : ("right-0 group-data-[collapsible=offcanvas]:right-[calc(var(--sidebar-width)*-1)]"), ((PARTAS_LOCAL.variant === "floating") ? (true) : (PARTAS_LOCAL.variant === "inset")) ? ("p-2 group-data-[collapsible=icon]:w-[calc(var(--sidebar-width-icon)_+_theme(spacing.4)_+2px)]") : ("group-data-[collapsible=icon]:w-[--sidebar-width-icon] group-data-[side=left]:border-r group-data-[side=right]:border-l"), PARTAS_LOCAL.class]))}
                {...PARTAS_OTHERS} bool:n $={false}>
                <div
                    class="flex size-full flex-col bg-sidebar group-data-[variant=floating]:rounded-lg group-data-[variant=floating]:border group-data-[variant=floating]:border-sidebar-border group-data-[variant=floating]:shadow"
                    data-sidebar="sidebar">
                    {PARTAS_LOCAL.children}
                </div>
            </div>
        </Match>
    </Switch>;
}

```
</details>

# Dev

To develop the plugin, ensure you exclude the plugin on compilation:

`fable --exclude Partas.Solid.FablePlugin --noCache -o output -e .fs.jsx --run dotnet restore`

There are a suite of tests to run to help inform if any changes have broken something else.

I've done my best to heavily document the plugin and the method of transformations.
