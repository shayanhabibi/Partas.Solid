---
title: Installation
---

Partas.Solid has two parts that you install together:

- **Partas.Solid**, the NuGet package with the DSL and the bindings.
- **Partas.Solid.FablePlugin**, the Fable compiler plugin that turns your F# into JSX. `Partas.Solid` depends on it,
  so it comes in with the package.

Fable writes `.jsx` files. A JavaScript bundler such as Vite then runs the Solid JSX compiler over them. You need both
a .NET toolchain and a Node toolchain.

:::warning
These docs cover **Partas.Solid 3.0**, which targets **Solid 2.0.0-rc.9**. 3.0 is a prerelease and is **not on NuGet
yet**. The latest version on nuget.org is 2.x, which targets Solid 1.9 and does not match these pages. Until 3.0 is
published, build the packages from source and install them from a local feed. The steps are below.
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

Create a tool manifest if your solution does not have one, then install Fable 5.

```bash
dotnet new tool-manifest
dotnet tool install fable --version 5.13.0
```

### 2. Build the 3.0 packages

Clone the repository and check out the Solid 2 branch. `dotnet pack` writes both packages to a folder of your choice.

```bash
git clone https://github.com/shayanhabibi/Partas.Solid.git
cd Partas.Solid
git checkout solid2rc
dotnet pack Partas.Solid.FablePlugin/Partas.Solid.FablePlugin.fsproj -c Release -o ../partas-feed
dotnet pack Partas.Solid/Partas.Solid.fsproj -c Release -o ../partas-feed
```

### 3. Add the local feed

Tell NuGet about the folder with a `nuget.config` next to your solution.

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="partas-local" value="../partas-feed" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
```

### 4. Reference Partas.Solid

```bash
dotnet add package Partas.Solid --version 3.0.0
```

If you use Paket, add `nuget Partas.Solid 3.0.0` to `paket.dependencies` along with a `source` line for the feed
folder, then run `paket install`.

`Partas.Solid` brings in `Partas.Solid.FablePlugin`, `Fable.Core` 5.2.0, `Fable.Browser.Dom` 2.20.0,
`Xantham.Fable.Core.TS` and `FSharp.Core` 10.

## The npm side

Solid 2 is a release candidate, so pin the exact versions. These are the versions Partas.Solid 3.0 is tested against.

```bash
npm install solid-js@2.0.0-rc.9 @solidjs/web@2.0.0-rc.9
npm install --save-dev vite@8.3.1 @solidjs/vite-plugin@3.0.0-next.44 @solidjs/compiler@2.0.0-rc.9 @solidjs/babel-plugin@2.0.0-rc.9
```

Or in `package.json`:

```json
{
  "type": "module",
  "dependencies": {
    "@solidjs/web": "2.0.0-rc.9",
    "solid-js": "2.0.0-rc.9"
  },
  "devDependencies": {
    "@solidjs/babel-plugin": "2.0.0-rc.9",
    "@solidjs/compiler": "2.0.0-rc.9",
    "@solidjs/vite-plugin": "3.0.0-next.44",
    "vite": "8.3.1"
  },
  "overrides": {
    "@solidjs/compiler": "2.0.0-rc.9",
    "@solidjs/babel-plugin": "2.0.0-rc.9",
    "solid-js": "2.0.0-rc.9",
    "@solidjs/web": "2.0.0-rc.9"
  }
}
```

The `overrides` block stops npm from pulling in a second, different Solid release (of `solid-js`, `@solidjs/web` or
the compiler) through the Vite plugin.

In Solid 2 the web runtime moved from `solid-js/web` to its own package, `@solidjs/web`. That package is where `render`,
`hydrate`, `Portal` and `Dynamic` come from. In F# they are in the `Partas.Solid.Web` namespace.

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

Mount your root component with `render` from `Partas.Solid.Web`:

```fsharp
module App

open Partas.Solid
open Partas.Solid.Web
open Browser.Dom

[<SolidComponent>]
let App () =
    h1 () { "Hello from F#" }

render ((fun () -> App ()), document.getElementById "root") |> ignore
```

`render` returns a function that disposes the app.

### Bindings and Femto

Some binding packages carry npm metadata for [Femto](https://github.com/Zaid-Ajaj/Femto), which can then install
their npm dependencies for you. Bindings that target Solid 1.x will not work with Solid 2. See the
[ecosystem](../ecosystem/index.md) for what has been ported.

## Compiling

Pass these flags to Fable:

```bash
dotnet fable -e .fs.jsx -c Release [--typedArrays false]
```

| Flag | Why |
| --- | --- |
| `-e .fs.jsx` | The plugin emits JSX, so Fable must write files with a `.jsx` extension. The Solid compiler then reads them. |
| `-c Release` | F# and Fable produce a different AST in Debug and Release. The plugin was built against the Release AST. Pass `-c Release` every time, including under `dotnet fable watch`, which otherwise compiles in Debug. |
| `--typedArrays false` | Fable compiles numeric arrays to typed arrays, so an `int[]` becomes an `Int32Array`. Some libraries expect a plain array and break on typed arrays. Turn them off with this flag, or `box` the values (`[\| box 5 \|]`) where it matters. |

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
