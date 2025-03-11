<div id="top"></div>

<br />

<div align="center">
  <a href="https://github.com/shayanhabibi/Partas.Solid" target="_blank">
    <img src="data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHdpZHRoPSIyNCIgaGVpZ2h0PSIyNCIgdmlld0JveD0iMCAwIDI0IDI0IiBmaWxsPSJub25lIiBzdHJva2U9IiMwYzA0ODEiIHN0cm9rZS13aWR0aD0iMiIgc3Ryb2tlLWxpbmVjYXA9InJvdW5kIiBzdHJva2UtbGluZWpvaW49InJvdW5kIiBjbGFzcz0ibHVjaWRlIGx1Y2lkZS1yZXBsYWNlLWFsbCI+PHBhdGggZD0iTTE0IDE0YTIgMiAwIDAgMSAyIDJ2NGEyIDIgMCAwIDEtMiAyIi8+PHBhdGggZD0iTTE0IDRhMiAyIDAgMCAxIDItMiIvPjxwYXRoIGQ9Ik0xNiAxMGEyIDIgMCAwIDEtMi0yIi8+PHBhdGggZD0iTTIwIDE0YTIgMiAwIDAgMSAyIDJ2NGEyIDIgMCAwIDEtMiAyIi8+PHBhdGggZD0iTTIwIDJhMiAyIDAgMCAxIDIgMiIvPjxwYXRoIGQ9Ik0yMiA4YTIgMiAwIDAgMS0yIDIiLz48cGF0aCBkPSJtMyA3IDMgMyAzLTMiLz48cGF0aCBkPSJNNiAxMFY1YSAzIDMgMCAwIDEgMy0zaDEiLz48cmVjdCB4PSIyIiB5PSIxNCIgd2lkdGg9IjgiIGhlaWdodD0iOCIgcng9IjIiLz48L3N2Zz4=" height="24px" style="border-radius:8px;"/>
  </a>

<h3 align="center">Partas.Solid</h3>
  <p align="center">
    <kbd>Solid-JS wrapper in Oxpecker style.</kbd>
  </p>
</div>

---

# Install

```shell
dotnet add package Partas.Solid
```

```shell
paket install Partas.Solid
```

### Related Repositories

Solid-ui (Shadcn port) [Partas.Solid.UI](https://github.com/shayanhabibi/Partas.Solid.UI)

Bindings for different libraries [Partas.Solid.Bindings](https://github.com/shayanhabibi/Partas.Solid.Bindings)

---

## Fable

> [!WARNING]
> This is not compatible with Fable 4.0, as the
> Plugin is not detected correctly.
> 
> Use the latest 5.0 alpha, or wait for full release.

## Oxpecker.Solid

> [!IMPORTANT]
> This is an opinionated fork of [Oxpecker.Solid](https://github.com/lanayx/Oxpecker) that
> keeps the original DSL style, but more aggressively
> transforms F# input to produce correct JSX.
> 
> Please support the original release.

### Differences

The most significant difference to Oxpecker.Solid is:


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

# Usage

Usage by principle is the same as [Oxpecker.Solid](https://github.com/lanayx/Oxpecker).

### SolidTypeComponentAttribute

An extra Attribute is provided which can be applied to a member of a type definition for a custom tag.

The custom tag must be defined within a namespace that begins with `Partas.Solid`.

Any type which the attribute is applied to, must have no arguments, and be defined with `props` as the self identifier.

```fsharp
[<SolidTypeComponent>]
member private props.constructor = div(class' = "MyClass").spread props
```

The name of the member has no influence on the output. The compiled name is determined by the name of the type to which the member is attached to.

```fsharp
[<Erase>]
type MyComponent() =
    inherit RegularNode()
    [<SolidTypeComponent>]
    member private props.uncleIro = div()
```

```jsx
export function MyComponent(props) {
    return <div />
}
```

> [!NOTE]
> If you were to apply an Import attribute to the type, and also provide a SolidTypeComponent member, then you would have the generated function, but any use of the tag would be rerouted to the Import selector, making the constructor useless.

### Properties

Defining custom properties is simple:

```fsharp
[<Erase>]
type MyComponent() =
    inherit RegularNode()
    
    [<Erase>] member val bordered: bool = unbox null with get,set
```

> [!NOTE]
> Ensure you apply Erase to prevent compilation of an empty functor
> 
> jsNative sometimes provides warnings, for this reason I instead bind to `unbox null`. Doesn't make a difference in the end.

### Aliasing properties

If you want to for example: define an alias to `class`, you can do this by defining an inlined member. You can use this to provide named overloads.

```fsharp
[<Erase>]
type MyComponent() =
    inherit RegularNode()
    member this.className
        with inline set(value: string) = this.class' <- value
        and inline get(): string = this.class'
```

### Accessing Properties in definitions

Just use the attached members of the self identifier.

### Spreading properties in descendants

The extension method `.spread` will spread the identifier within the tag.

> [!NOTE]
> There is a pull currently being discussed to perform this action without having to use a stub/noop (Null operation).

If you know Solid-js you might ask:

> Won't this also spread properties we've used elsewhere in the definition?

All property access is automatically split for you. The times where you INTEND to pass the same properties, are far fewer than not. When you do, you can still just manually assign the few properties you've spread anyway.

See the next section for an example.

### Setting defaults, mergeProps

In combination with automatic property splitting, you can also assign properties to generate a `mergeProps`

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

#### Risky Business

For code clarity, you can even define the default in PROXIMITY to the usage, even in a nested tree:

```fsharp
type [<Erase>] CustomTag() =
    inherit RegularNode()
    [<SolidTypeComponent>]
    member private props.typeDef =
        div(){
            div(class' = props.class') {
                props.class' <- "ProximalDefaultDefinition"            
                button().spread(props)
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

However, this can also be counter productive to readability etc. If you try to assign the same property twice, then you will receive an error on compilation.

## Misc magics

### createContext & useContext

Frequently described as an anti-pattern, it doesn't mean it shouldn't be supported, as it is frequently used in online examples. The freedom to essentially copy JSX code without too much cognitive load is important.

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

We aggressively unroll the Fable AST in the value position to prevent the formation of toList/toArray, delay and singleton chains. If you know, then you know!

### Passing tags as values

You can pass tags as values in jsx using `!@` prefix operator and then `%` in combination with an anonymous record as a POJO or with a constructor to build the element at the call site.

```fsharp
[<Erase>]
type CustomTag() =
    inherit RegularNode()
    [<Erase>]
    member val icon: TagValue<_> = unbox null with get,set
    [<SolidTypeComponent>]
    member props.constructor =
        props.icon <- !@button
        div() {
            props.icon % {| class' = "KeyVal" |}
            props.icon % {| class' = "KeyVal2" |}
            props.icon % div(class' = "constructor") { button() { "internal" } }
            
        }

[<SolidComponent>]
let Rock () =
    let Comp = !@Imported
    CustomTag(icon = unbox !@a) {
        Comp % Imported(other = unbox !@ModuleTag )
    }
```

```jsx
import { splitProps, mergeProps } from "solid-js";
import { FakeImportedTag } from "fakeLibrary";
import { ModuleTag } from "./TagsAsValuesSimpleTypes.fs.jsx";

export function CustomTag(props) {
    props = mergeProps({
        icon: button,
    }, props);
    const [PARTAS_LOCAL, PARTAS_OTHERS] = splitProps(props, ["icon"]);
    return <div>
        <PARTAS_LOCAL.icon class="KeyVal" />
        <PARTAS_LOCAL.icon class="KeyVal2" />
        <PARTAS_LOCAL.icon class="constructor">
            <button>
                internal
            </button>
        </PARTAS_LOCAL.icon>
    </div>;
}

export function Rock() {
    const Comp = FakeImportedTag;
    return <CustomTag icon={a}>
        <Comp other={ModuleTag} />
    </CustomTag>;
}
```

### SolidComponent Attribute

You can still use the SolidComponent attribute in the same way you would in Oxpecker.Solid. This means no mergeProps/splitProps will be generated by default. You can still spread objects etc, but you will have to manage splitting yourself.

## Example:

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

If you like this, please let me know!

I will heavily develop bindings and what not to work with this, but am not motivated to provide documentation/examples without knowing it might help someone.

_Please support the original release [Oxpecker.Solid](https://github.com/lanayx/Oxpecker)_

# Dev

To develop the plugin, ensure you exclude the plugin on compilation:

`fable --exclude Partas.Solid.FablePlugin --noCache -o output -e .fs.jsx --run dotnet restore`

There are a suite of tests to run to help inform if any changes have broken something else.

I've done my best to heavily document the plugin and the method of transformations.

If you are finding a property is not being included in compilation, it likely has not been recognized by the property collection loop. if you add the `--verbose` flag to Fable compilation, you will have a bunch of warnings about what expressions were disposed of in property collection becuase they weren't recognized. This is a good place to start.

When utilising computations/builders, providing parameter names which are exceedingly distinct is advantageous in ensuring targetted transformation.

Similarly, when changing/generating JSX code, use distinct names to prevent ident collisions. This is why I use PARTAS_LOCAL and PARTAS_OTHERS instead of local/others. The names are distinct, they are recognizable, unlikely to collide, and do not obfuscate the output code with meaningless identifiers.