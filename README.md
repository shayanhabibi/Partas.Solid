# Partas.Solid

> [!WARNING]
> This is not compatible with Fable 4.0, as the
> Plugin is not detected correctly.
> 
> Use the latest 5.0 alpha, or wait for full release.

> [!IMPORTANT]
> This is an opinionated fork of [Oxpecker.Solid](https://github.com/lanayx/Oxpecker) that
> enables experimental behaviours and patterns which I use in personal projects.
> 
> Until such a time as these types of behaviours/patterns are sought out by users, the behaviours
> will not be supported on the mainstream.
> 
> For this reason, I publish this as a means of allowing users to experiment or observe the patterns
> enabled and submit requests for it to be considered in the mainline.

## Features

### Reduce undefined behaviour

Hugely opinionated, but I feel that reducing all transformation
to a few pathways which expressions must all traverse simplifies
the cognitive load for developing the plugin. The bulk of the code is
boiler plate reduction for Expr pattern matches. 

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

As opposed to the proposed iteration:

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

Currently, Oxpecker doesn't support the ability to create components and then use them in the Oxpecker style.

Naturally, this is because a functional approach would not use the DSL to construct final pages, and use functions instead. But creating a permissive and flexible component library wouldn't work in this manner without being extremely verbose.

As an alternative, the current iteration provides an extra attribute which can be applied to members of your type definition for the tag using `props` as the self identifier. The self identifier, props, allows type safe access to your defined properties which can be set in Oxpecker style.

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

You can set those defaults in a deeply nested tree where you actually make use of them. As an example of where/what undefined behaviour looks like currently, I've included this example despite it producing unsatisfactory output. This would be the first time I've actually tested this capability!

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
            {(value) => {
            }}
        </div>
    </div>;
}
```

To prevent undefined behaviour further, I can easily add an error when a property is set twice, for safety.

## Conclusion

If you like this kind of thing DSL, let me know!

I'll  be developing a component library that will make heavy usage of the type definition feature to allow you to copy paste and change what you want.

### Install

This will be published as a package to test this when I've completed a comprehensive testing suite.