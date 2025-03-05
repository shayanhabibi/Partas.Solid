# Partas.Solid

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

The feature set contained within this is a modified transformation or alternate transformation
which can be applied to the type definitions of your tags/elements. This allows a UserDefined custom tag rather than only allowing Library Imported tags.

```fsharp
namespace Partas.Solid.Test

open Partas.Solid
open Fable.Core

type [<Erase>] CustomTag() =
    inherit RegularNode()
    [<SolidTypeComponent>]
    member private props.typeDef () =
        div() { "Hi" }
```

Compiles to

```jsx
function CustomTag(props) {
    return <div> Hi </div>;
}
```

We can now utilise this like any other tag:

```fsharp
module App
open Partas.Solid.Test

[<SolidTypeComponent>]
let App () =
    CustomTag()
```
```jsx
import { CustomTag } from "./Program.fs.jsx"

export function App() {
    return <CustomTag />;
}
```

Which would render the Html

```html
<div>Hi</div>
```

Ontop of the above, we also can pass properties down to nested children while maintaining reactivity:

> [!TIP]
> Setting default properties and splitting properties/spreading now has all the mergeProps and splitProps abstracted away

```fsharp
type [<Erase>] CustomTag() =
    inherit RegularNode()
    [<SolidTypeComponent>]
    member private props.typeDef () =
        props.class' <- "DefaultClass" // setting properties will set the 'default'
        div(class' = props.class') {
            button(spreadObj = props)
        }
```
```jsx
function CustomTag(props) {
    props = mergeProps({
        class: "DefaultClass",
    }, props);
    const [local, others] = splitProps(props, ["class"]);
    return <div class={local.class}>
        <button {...others} />
    </div>;
}
```

## Rough Edges

Oxpecker.Solid is a great DSL and works for most use cases. However, I've encountered many rough edges when it comes to LibraryImports especially. In the process of developing these behaviours, I've fixed several of those issues. But in the process, perhaps I've created more.

If you do decide to play with this and find some odd behaviours, please let me know.

# Usage

## Installation

Will be uploaded to nuget soon(tm)

## Attribute

Purposeful attribute change to allow simultaneous use from mainstream by changing `[<SolidComponent>]` to `[<SolidTypeComponent>]`.

# REWRITE

I have undertaken to rewrite the plugin to be more aggressive in transformation so that things such as tags within if/else statements with extensions etc etc are supported by default, not because of adding extra patterns, but because the transformations are recursively applied to all expressions. This will likely increase compile times, and this will be measured when a sizeable example is prepared.

For me, the native support of some type of 'context' within the transformations also allows support of features like automatically
producing mergeProps (to set defaults at the top level) and splitProps, while keeping type safety. This has already been implemented.

As I progress, I will also look at for/loops to see about automatically producing the relevant Solid construct.
Naturally, these behaviours will have opt-out flags when finalised.

As it deviates further from Oxpecker.Solid, the chance of a merge might decrease as my intention differs from Oxpecker.Solid, in that I intend to allow full creation of components that are usable and flexible with the same syntax, completely within F#.

This allows me to write my components in F#, create a component library with the flexibility of jsx element attributes/properties, and use that library in the Oxpecker.Solid style DSL/Syntax within subsequent functions, with the freedom to choose whether end usage is entirely by f# functions, or in Oxpecker.Solid DSL.

This probably doesn't make sense but I'm really tired :)
