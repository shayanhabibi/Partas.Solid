# Partas.Solid

> [!WARNING]
> This is not compatible with Fable 4.0, as the
> Plugin is not detected correctly.
> 
> Use the latest 5.0 alpha, or wait for full release.

> [!IMPORTANT]
> This is an opinionated fork of [Oxpecker.Solid](https://github.com/lanayx/Oxpecker) that
> keeps the original DSL style, but more aggressively
> transforms F# input to produce correct JSX.
> 
> As I progress with changes in this fork, I am
> growing more pessimistic of the chances of this
> being merged upstream; these changes are fundamentally
> personal, and do not fit the up-stream vision or intention.

## Features

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

### Custom Tags & Components in Oxpecker Syntax

Feel free to design a component system completely within F# that uses the same syntax, and offers the same flexibility and style variation that any other component framework would offer, but with all the advantages of F# compiler safety.

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

> [!NOTE]
> The tag name is dependent on the type; the member definition that you add the `SolidTypeComponent` attribute to has no influence.
> 
> It is advantageous to make it a private field.

> [!CAUTION]
> Like in Oxpecker, defining custom tags without the
> special attribute member will not generate a component
> function. You will still be able to transpile, but the
> output would be invalid on runtime.

Any accesses to your properties are automatically split, and you can spread whatever other properties into a tag to allow prop drilling, or to integrate with other component libraries that might use providers etc.

As an added benefit, there is sugar for setting the defaults of any property using `<-`.

```fsharp
// -- Program.fs
type [<Erase>] CustomTag() =
    inherit RegularNode()
    
    [<SolidTypeComponent>]
    member private props.typeDef =
    
        props.class' <- "DefaultClass" // setting properties will set the 'default'
        
        div(class' = props.class') {
        
            button().spread(props)
            
        }
```

```jsx
function CustomTag(props) {
    
    props = mergeProps({
        class: "DefaultClass",
    }, props);
    
    const [PARTAS_LOCAL, PARTAS_OTHERS] = splitProps(props, ["class"]);
    
    return <div class={PARTAS_LOCAL.class}>
        
        <button {...PARTAS_OTHERS} bool:n$={false} />
        
    </div>;
}
```

> [!NOTE]
> There is a pull in discussion on Fable that will prevent the need for the noop `bool:n$={false}` after the spread.

You can use those tags like any other import from other files, and from the standard attribute `SolidComponent`:

```fsharp
// -- Test.fs
open Program

[<SolidComponent>]
let SomeTag () =

    let this = button() { "some boiler plate" }
    
    CustomTag(class'="MyButton") {
    
        if show then this else ()
        
    }
```

```jsx
import { createAtom } from "./fable_modules/fable-library-js.5.0.0-alpha.11/Util.js";
import { CustomTag } from "./Program.fs.jsx";

export let show = createAtom(true);

export function SomeTag() {
    
    const this$ = <button>
        some boiler plate
    </button>;
    
    return <CustomTag class="MyButton">
        {show() ? this$ : undefined}
    </CustomTag>;
}
```

You can set those defaults in a deeply nested tree where you actually make use of them.

```fsharp
type [<Erase>] CustomTag() =
    inherit RegularNode()
    [<SolidTypeComponent>]
    member private props.typeDef =
        div(){
            div(class' = props.class') {
                button().spread(props)
                props.class' <- "ProximalDefaultDefinition"            
            }
        }
```

```jsx
function CustomTag(props) {
    props = mergeProps({
        class: "ProximalDefaultDefinition",
    }, props);
    const [PARTAS_LOCAL, PARTAS_OTHERS] = splitProps(props, ["class"]);
    return <div>
        <div class={PARTAS_LOCAL.class}>
            <button {...PARTAS_OTHERS} bool:n$={false} />
        </div>
    </div>;
}
```

If you try to apply two defaults to the same property, you will receive an error on compilation.

## createContext & useContext

Frequently described as an anti-pattern, it doesn't mean it has _no_ place to be used. Especially in the context of creating a component library.

The plugin supports you creating a context, and then using it as a tag to generate the provider.

```fsharp
// context defined in another module/file
let context = createContext<int>()
// local context definition
let SomeContext = Bindings.createContext<int>()
// Adding the context providers
[<SolidComponent>]
let MyComponent () =
    context(5) {
        context(3) {
            "provider 1"
        }
        SomeContext(10) {
            "provider 2"
        }
    }
```

```jsx
export const SomeContext = createContext();

export function MyComponent() {
    return <context.Provider value={5}>
        <context.Provider value={3}>
            provider 1
        </context.Provider>
        <SomeContext.Provider value={10}>
            provider 2
        </SomeContext.Provider>
    </context.Provider>;
}
```

### Flattened Arrays in attribute value positions

To allow a pattern such as:

```fsharp
div(class' = clsx [| "a1"; "a2"; if someCondition then "a3" else "a4" |])
```

We aggressively unroll the Fable AST in the value position to prevent the formation of toList/toArray, delay and singleton chains.

A comprehensive component and example output:

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

## Conclusion

If you like this kind of thing DSL, let me know!

I'll  be developing a component library that will make heavy usage of the type definition feature to allow you to copy paste and change what you want.

### Install

`dotnet add package Partas.Solid`