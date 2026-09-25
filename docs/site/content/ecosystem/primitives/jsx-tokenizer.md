---
title: JSX Tokenizer
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/jsx-tokenizer`.

This page lists the binding's types and API. For usage, see the `@solid-primitives` documentation.

## JSXTokenizer

Type: `interface`

```fsharp
type JSXTokenizer<'Data>
```

- `$TOKENIZER`: `obj`
- `$TYPE`: `'Data`

A tokenizer creates several token components with the same id, and resolves their data from the JSX element
structure.

## TokenElement

Type: `interface`

```fsharp
type TokenElement<'Data>
```

- `data`: `'Data`
- `$TOKENIZER`: `obj`

A resolved token, as returned by `resolveTokens`. Read its `data` property to get the data it carries.

## TokenComponent

Type: `interface`

```fsharp
type TokenComponent<'Props, 'Data> =
    inherit HtmlElement
    inherit JSXTokenizer<'Data>
type TokenComponent<'Props> = TokenComponent<'Props, 'Props>
```

The `createToken` overloads return this type. A class can implement it so that the plugin compiles the class as a JSX
tag. `'Props` is meant to be your class type.

This lets the binding use the plugin's tag construction. You do not have to write POJOs with parameters in both the
constructor and the properties, and you can use interfaces more naturally.

## Bindings

| Member | Desc |
| --- | --- |
| `createTokenizer` | Creates a tokenizer. `name` is used for debugging. |
| `createToken` | Creates a token component that passes custom data through the JSX structure. `resolveTokens` returns its data. Resolved normally, for example with `children`, it renders the fallback from `render`, or nothing (with a warning in development). |
| `resolveTokens` | Like Solid's `children`. Resolves the JSX structure from `fn` and returns an accessor of the tokens that belong to the tokenizer. With `includeJSXElements = true`, other JSX elements are included too. |
| `isToken` | Whether a value is a `TokenElement` of the tokenizer. |

`createToken` parameters:

| Param | Desc |
| --- | --- |
| `tokenizer` | Identity object returned by `createTokenizer`, or another `TokenComponent`. If you leave it out, a new tokenizer id is created. |
| `tokenData` | Function that returns the token's data. If you leave it out, the props are the data. |
| `render` | Function that returns the fallback JSX element. If you leave it out, the token renders nothing and warns in development. |

`resolveTokens` and `isToken` also accept an array of tokenizers.

:::details title="Bindings"
```fsharp
[<AutoOpen; Erase>]
type JsxTokenizer =
    [<ImportMember(Spec.path); ParamObject(0)>]
    static member createTokenizer<'Data>(name: string): JSXTokenizer<'Data> = jsNative
    [<ImportMember(Spec.path)>]
    static member createTokenizer<'Data>(): JSXTokenizer<'Data> = jsNative

    [<ImportMember(Spec.path)>]
    static member createToken<'Props, 'Data>(tokenizer: JSXTokenizer<'Data>, tokenData: 'Props -> 'Data, ?render: 'Props -> HtmlElement): TokenComponent<'Props, 'Data> = jsNative
    static member inline createToken<'Data>(tokenizer: JSXTokenizer<'Data>, ?render: 'Data -> HtmlElement): TokenComponent<'Data> =
        JsxTokenizer.createToken<'Data, 'Data>(tokenizer, undefined, ?render = render)
    static member inline createToken<'Props, 'Data>(tokenData: 'Props -> 'Data, ?render: 'Props -> HtmlElement): TokenComponent<'Props, 'Data> =
        JsxTokenizer.createToken<'Props, 'Data>(undefined, tokenData, ?render = render)
    static member inline createToken<'Data>(?render: 'Data -> HtmlElement): TokenComponent<'Data, 'Data> =
        JsxTokenizer.createToken<'Data, 'Data>(undefined, ?render = render)

    [<ImportMember(Spec.path); ParamObject(2)>]
    static member resolveTokens<'Data>(tokenizer: JSXTokenizer<'Data>, fn: Accessor<HtmlElement>, ?includeJSXElements: bool): Accessor<TokenElement<'Data>[]> = jsNative
    [<ImportMember(Spec.path); ParamObject(2)>]
    static member resolveTokens<'Data>(tokenizer: JSXTokenizer<'Data>[], fn: Accessor<HtmlElement>, ?includeJSXElements: bool): Accessor<TokenElement<'Data>[]> = jsNative

    [<ImportMember(Spec.path)>]
    static member isToken<'Data>(tokenizer: JSXTokenizer<'Data>, value: obj): bool = jsNative
    [<ImportMember(Spec.path)>]
    static member isToken<'Data>(tokenizer: JSXTokenizer<'Data>[], value: obj): bool = jsNative
```
:::
