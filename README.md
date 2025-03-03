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

# Current Progress

I have undertaken to completely rewrite the plugin, but keeping much of the Oxpecker.Solid library.

I intend my version to be more aggressive in transformation, and to be more sensible in active recognition.

As an example, there is only one TagConstructor/Builder/Tag recognizer. This pattern must successfully capture ALL cases of tag builders regardless of method chains, children etc. The pattern seeks the constructors when nested in chains, and lifts them to the top, to also simplify library import/member import patterns.

> Previous patterns to recognize constructors

```fsharp
let (|ConstructorBuilder|_|) (expr: Expr) =
    let condition =
        fun importInfo ->
            [ "HtmlContainerExtensions_Run"; "BindingsModule_Extensions_run" ]
            |> List.map importInfo.Selector.StartsWith
            |> List.exists ((=) true)

    match expr with
    | CallTag condition tagCallInfo -> Some tagCallInfo
    | _ -> None
let (|Constructor|_|) (expr: Expr) =
    let condition = _.Selector.EndsWith("_$ctor")

    match expr with
    | CallTag condition (_, _, range) & LibraryTagImport(tagName, _) -> Some(LibraryImport tagName, range)
    | CallTag condition (tagName, _, range) -> Some(tagName, range)
    | _ -> None
let (|TextNoSiblings|_|) =
    function
    | Lambda({ Name = cont }, TypeCast(textBody, Unit), None) when cont.StartsWith("cont") -> Some textBody
    | _ -> None

module Let =
    let (|NoChildrenNoProps|NoChildrenWithProps|WithChildren|InvalidExpr|) =
        function
        | Let(_, Tags.ConstructorBuilder(withChildrenTag), _) ->
            WithChildren(TagInfo.WithChildren withChildrenTag)
        | Let(_, Tags.Constructor(tagName, range), Sequential exprs) ->
            NoChildrenWithProps(TagInfo.NoChildren(tagName, exprs, range))
        | Let(_, Tags.Constructor(tagName, range), _) ->
            NoChildrenNoProps(TagInfo.NoChildren(tagName, [], range))
        | _ as expr -> InvalidExpr(expr)

module Call =
    let (|NoChildrenWithHandler|_|) =
        function
        | Call(Import(ImportInfo.HtmlElementExtension _, _, _),
               {
                   Args = arg :: _
               },
               _,
               range) as expr ->
            match arg with
            | Tags.Constructor(tagName, _) -> TagInfo.NoChildren(tagName, [ expr ], range) |> Some
            | LibraryTagImport(imp, _) -> TagInfo.NoChildren(LibraryImport imp, [expr], range) |> Some
            | Tags.Let.NoChildrenWithProps(TagInfo.NoChildren(tagName, props, _)) -> TagInfo.NoChildren(tagName, expr :: props, range) |> Some
            | Let(_, LibraryTagImport(imp, _), Sequential exprs) -> TagInfo.NoChildren(LibraryImport imp, expr :: exprs, range) |> Some 
            | NoChildrenWithHandler(TagInfo.NoChildren(tagName, props, _)) -> TagInfo.NoChildren(tagName, expr :: props, range) |> Some
            | _ -> None
            
            
        | _ -> None
```

> Redesigned recogniz**er**

```fsharp
let (|TagConstructor|_|) (ctx: PluginContext) = function
    | Call(
        (IdentExpr(Ident.IdentIs ctx IdentType.Constructor)),
        (CallInfo.Constructor ctx as callInfo),
        ((Type.PartasName ctx (Helpers.EndsWithTrimmed "_$ctor" typeName)) | (Type.PartasName ctx typeName)),
        range) ->
        TagInfo.Constructor( TagSource.AutoImport typeName, callInfo.Args, range)
        |> Some
    | Call(
        (Expr.ImportedConstructor ctx & Import(importee, t, r) ),
        callInfo,
        (Type.PartasName ctx typeName),
        range) ->
        TagInfo.Constructor (TagSource.LibraryImport (Import({ importee with Selector = typeName }, t, r)), callInfo.Args, range)
        |> Some
    // WITH PROPS - TODO
    | Let(Ident.IdentIs ctx IdentType.ReturnVal, TagConstructor ctx tagInfo, body) ->
        match tagInfo with
        | TagInfo.Combined(tagSource, props, propsAndChildren, range) ->
            TagInfo.Combined(tagSource, body :: props, propsAndChildren, range)
        | TagInfo.Constructor(tagSource, props, range) ->
            TagInfo.Constructor(tagSource, body :: props, range)
        | TagInfo.WithBuilder(tagSource, propsAndChildren, range) ->
            TagInfo.Combined(tagSource, [ body ], propsAndChildren, range)
        |> Some
    // WITH PROPS & EXTENSION - TODO
    | Call(Import({ Kind = MemberImport(MemberRef({ FullName = Helpers.EndsWith "HtmlElementExtensions" }, _)) }, _, _) as callee, ({
            Args = TagConstructor ctx tagInfo :: rest 
        } as callInfo), _, _) ->
        match tagInfo with
        | TagInfo.Combined(tagSource, props, propsAndChildren, range) ->
            TagInfo.Combined(tagSource, props, { propsAndChildren with Args = rest @ propsAndChildren.Args }, range)
        | TagInfo.Constructor(tagSource, props, range) ->
            TagInfo.Combined(tagSource, props, { callInfo with Args = rest }, range)
        | TagInfo.WithBuilder(tagSource, propsAndChildren, range) ->
            TagInfo.WithBuilder(tagSource, { propsAndChildren with Args = rest @ propsAndChildren.Args }, range)
        |> Some
        
    | _ -> None
```

It also incorporates a context pattern to allow the use of the Fable warning/errors logger etc, and to allow behaviours like lifting property accesses and assignments. 