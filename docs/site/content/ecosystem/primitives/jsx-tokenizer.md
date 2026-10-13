---
title: JSX Tokenizer
---

:::warning
These bindings target Partas.Solid 2.x on Solid 1.9 and have not been ported to Solid 2 yet.
:::

Bindings for `@solid-primitives/jsx-tokenizer`. A tokenizer creates token components that share an id, and resolves
their data from the JSX structure.

## Types

```fsharp
type JSXTokenizer<'Data>          // $TOKENIZER: obj; $TYPE: 'Data
type TokenElement<'Data>          // a resolved token: data: 'Data; $TOKENIZER: obj

type TokenComponent<'Props, 'Data> =
    inherit HtmlElement
    inherit JSXTokenizer<'Data>
type TokenComponent<'Props> = TokenComponent<'Props, 'Props>
```

`createToken` returns a `TokenComponent`. A class can implement it, with `'Props` as the class itself, so the plugin
compiles it as a JSX tag; props then come from the plugin's tag construction instead of hand-written POJOs.

## Bindings

```fsharp
let createTokenizer<'Data>(?name: string): JSXTokenizer<'Data>
let createToken<'Props, 'Data>(?tokenizer: JSXTokenizer<'Data>, ?tokenData: 'Props -> 'Data, ?render: 'Props -> HtmlElement)
    : TokenComponent<'Props, 'Data>
let resolveTokens<'Data>(tokenizer: JSXTokenizer<'Data> | JSXTokenizer<'Data>[], fn: Accessor<HtmlElement>, ?includeJSXElements: bool)
    : Accessor<TokenElement<'Data>[]>
let isToken<'Data>(tokenizer: JSXTokenizer<'Data> | JSXTokenizer<'Data>[], value: obj): bool
```

| Member | Desc |
| --- | --- |
| `createTokenizer` | Creates a tokenizer. `name` is used for debugging. |
| `createToken` | Creates a token component that passes data through the JSX structure. Without `tokenizer`, it gets a new tokenizer id; without `tokenData`, the props are the data. Resolved normally, for example with `children`, it renders `render`'s fallback, or nothing (with a warning in development). |
| `resolveTokens` | Like Solid's `children`. Resolves the JSX from `fn` and returns the tokens that belong to the tokenizer; with `includeJSXElements = true`, other JSX elements too. |
| `isToken` | Whether a value is a `TokenElement` of the tokenizer. |
