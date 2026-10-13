---
title: Installation
---

Partas.Solid has two NuGet packages:

- **Partas.Solid**, the DSL and the bindings.
- **Partas.Solid.FablePlugin**, the Fable compiler plugin that turns your F# into JSX. `Partas.Solid` depends on it.

Fable writes `.jsx` files, and a bundler such as Vite runs the Solid JSX compiler over them, so you need both a .NET and
a Node toolchain.

:::warning
These docs cover **Partas.Solid 3.0**, which targets **Solid 2.0.0-rc.14**.

**New releases are on a temporary NuGet feed.** The nuget.org account is inaccessible while NuGet support transfers
package ownership, so new releases are published to **Cloudsmith**, under the same package IDs:
`https://nuget.cloudsmith.io/shayanhabibi/partas-solid/v3/index.json`. Restoring from it needs no account or API key.
The latest version on nuget.org is 2.x, which targets Solid 1.9 and does not match these pages. Once ownership is
restored, releases return to nuget.org and this notice will be updated.
:::

## Requirements

| Tool | Version |
| --- | --- |
| .NET SDK | 10 recommended. `Partas.Solid` targets `net8.0`, but it references FSharp.Core 10 |
| Fable | 5.x. The repository pins the tool at `5.13.0` |
| Fable.Core | `5.2.0`, a dependency of `Partas.Solid` |
| Node | 22.12 or later |

## The .NET side

### 1. Install Fable

```bash
dotnet new tool-manifest
dotnet tool install fable --version 5.13.0
```

### 2. Add the Cloudsmith feed

Partas.Solid's DOM types come from `Xantham.Fable.Core.TS`, which is published on its own Cloudsmith feed, so add both
feeds. Add a `nuget.config` next to your solution, keeping nuget.org for the other dependencies:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="partas-solid-temporary" value="https://nuget.cloudsmith.io/shayanhabibi/partas-solid/v3/index.json" />
    <add key="xantham" value="https://nuget.cloudsmith.io/shayanhabibi/xantham/v3/index.json" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
```

Or add it to your user configuration:

```bash
dotnet nuget add source https://nuget.cloudsmith.io/shayanhabibi/partas-solid/v3/index.json --name partas-solid-temporary
dotnet nuget add source https://nuget.cloudsmith.io/shayanhabibi/xantham/v3/index.json --name xantham
```

If your configuration uses package source mapping, map `Partas.Solid*` to the first source and `Xantham.*` to the
second.

### 3. Reference Partas.Solid

```bash
dotnet add package Partas.Solid --version 3.0.0
```

If you use Paket, add `nuget Partas.Solid 3.0.0` to `paket.dependencies` along with
`source https://nuget.cloudsmith.io/shayanhabibi/partas-solid/v3/index.json` and `source https://nuget.cloudsmith.io/shayanhabibi/xantham/v3/index.json`, then run
`paket install`.

`Partas.Solid` brings in `Partas.Solid.FablePlugin`, `Fable.Core` 5.2.0, `Xantham.Fable.Core.TS` and `FSharp.Core` 10.

DOM types come from Xantham's `Fable.Core.TS.Dom`, not `Fable.Browser.Dom`: `open Fable.Core.TS` and write
`Dom.HTMLElement`, `Dom.MouseEvent` and so on. Lookups such as `getElementById` return options. Partas.Solid binds
the globals `globalThis`, `document` and `window`; they come in with `open Fable.Core`.

## The npm side

Solid 2 is a release candidate, so pin the exact versions Partas.Solid 3.0 is tested against:

```bash
npm install solid-js@2.0.0-rc.14 @solidjs/web@2.0.0-rc.14
npm install --save-dev vite@8.3.1 @solidjs/vite-plugin@3.0.0-next.49 @solidjs/compiler@2.0.0-rc.14 @solidjs/babel-plugin@2.0.0-rc.14
```

Or in `package.json`:

```json
{
  "type": "module",
  "dependencies": {
    "@solidjs/web": "2.0.0-rc.14",
    "solid-js": "2.0.0-rc.14"
  },
  "devDependencies": {
    "@solidjs/babel-plugin": "2.0.0-rc.14",
    "@solidjs/compiler": "2.0.0-rc.14",
    "@solidjs/vite-plugin": "3.0.0-next.49",
    "vite": "8.3.1"
  },
  "overrides": {
    "@solidjs/compiler": "2.0.0-rc.14",
    "@solidjs/babel-plugin": "2.0.0-rc.14",
    "solid-js": "2.0.0-rc.14",
    "@solidjs/web": "2.0.0-rc.14"
  }
}
```

The `overrides` block stops npm from pulling in a second, different Solid release (of `solid-js`, `@solidjs/web` or
the compiler) through the Vite plugin.

In Solid 2 the web runtime moved from `solid-js/web` to `@solidjs/web`, which provides `render`, `hydrate`, `Portal`
and `Dynamic`. In F# they are in the `Partas.Solid.Web` namespace.

A minimal `vite.config.js` that picks up Fable's output:

```js
import { defineConfig } from "vite";
import solid from "@solidjs/vite-plugin";

export default defineConfig({
  plugins: [
    solid({
      include: ["**/*.jsx"],
      exclude: ["node_modules/**", "**/fable_modules/**"]
    })
  ]
});
```

### Mounting the app

Mount the root component with `render`. It returns a function that disposes the app.

```fsharp
module App

open Partas.Solid
open Partas.Solid.Web
open Fable.Core
open Fable.Core.TS

[<SolidComponent>]
let App () =
    h1 () { "Hello from F#" }

render ((fun () -> App ()), (document.getElementById "root").Value) |> ignore
```

### Bindings and Femto

Some binding packages carry npm metadata for [Femto](https://github.com/Zaid-Ajaj/Femto), which can then install
their npm dependencies for you. Bindings that target Solid 1.x will not work with Solid 2. See the
[ecosystem](../ecosystem/index.md) for what has been ported.

## Compiling

Pass these flags to Fable:

```bash
dotnet fable -e .fs.jsx -c Release --typedArrays false
```

| Flag | Why |
| --- | --- |
| `-e .fs.jsx` | The plugin emits JSX, so Fable must write files with a `.jsx` extension. The Solid compiler then reads them. |
| `-c Release` | F# and Fable produce a different AST in Debug and Release. The plugin was built against the Release AST. Pass `-c Release` every time, including under `dotnet fable watch`, which otherwise compiles in Debug. |
| `--typedArrays false` | Without it, Fable compiles numeric arrays to typed arrays, so an `int[]` becomes an `Int32Array`. Solid's stores do not wrap typed arrays (`createStore [\| 1; 2 \|]` is not reactive per item), and libraries that expect a plain array can break on them. Partas.Solid is tested with this flag, so pass it. |

:::note
The Partas.SolidStart `dotnet new` templates (`bare-devtools`, `tailwind-devtools`, `solidbase-devtools`,
`mdx-devtools`) set up Partas.Solid 2.x on Solid 1.9 and SolidStart 1. They have not been updated for 3.0, so do not
use them with these docs.
:::

## Next

- [Overview](overview.md): the two ways to write a component.
- [Oxpecker DSL](oxpecker-dsl.md): how elements, attributes and children are written.
- [Common issues](common-issues.md): what to check when the output looks wrong.
- [Migrating to Solid 2](migrating-to-solid-2.md): if you are coming from Partas.Solid 2.x.
