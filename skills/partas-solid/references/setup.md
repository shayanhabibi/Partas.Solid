# Application setup and compilation

Use the application's existing toolchain and scripts. Inspect package references and lockfiles before changing
versions. The examples in this skill are for Partas.Solid 3 with Solid 2; 2.x/Solid 1.9 applications need their
own versioned guide. `match!` first ships in Partas.Solid 3.0.0-rc.2.

## NuGet

`Partas.Solid` supplies the DSL/bindings and depends on `Partas.Solid.FablePlugin`. An ordinary application does
not need the Nacara documentation plugin. New Partas releases use a temporary public Cloudsmith feed;
Xantham's DOM types use a separate feed. Preserve nuget.org for other dependencies:

```xml
<configuration>
  <packageSources>
    <add key="partas-solid" value="https://nuget.cloudsmith.io/shayanhabibi/partas-solid/v3/index.json" />
    <add key="xantham" value="https://nuget.cloudsmith.io/shayanhabibi/xantham/v3/index.json" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
```

These feeds restore publicly without credentials. In configurations using package-source mapping, map
`Partas.Solid*` and `Xantham.*` to their corresponding feeds. Retain the application's other sources and mappings.
Select a published compatible package version; do not assume an unqualified `3.0.0` stable release exists.

## Fable and the Solid compiler

The DSL needs Fable's plugin transformation and then a Solid JSX compiler. A normal .NET build alone is not the
application build. For Fable 5, use the application's project path with these flags:

```sh
dotnet fable -e .fs.jsx -c Release --typedArrays false
```

Pass Release configuration in watch mode too. The plugin expects the Release AST. `.jsx` output lets the Solid
compiler process the generated code; plain `.js` output with JSX is not sufficient for a bundler that only
includes JSX files. Plain numeric arrays also matter to stores and npm libraries.

For a new 3.x app, the documented compatibility set uses Solid `2.0.0-rc.14`: keep `solid-js`, `@solidjs/web`,
`@solidjs/compiler`, and `@solidjs/babel-plugin` aligned. The installation guide pairs these with
`@solidjs/vite-plugin` `3.0.0-next.49`. Treat this as a verified set, not a direction to replace a different working
set; consult the guide and installed packages for upgrades. Pin compatible prereleases and use overrides when
necessary to prevent the Vite plugin from introducing a second Solid runtime.

```js
import { defineConfig } from "vite";
import solid from "@solidjs/vite-plugin";

export default defineConfig({
  plugins: [solid({
    include: ["**/*.jsx"],
    exclude: ["node_modules/**", "**/fable_modules/**"]
  })]
});
```

The web runtime lives in `@solidjs/web`; its F# bindings require `open Partas.Solid.Web`:

```fsharp
module App

open Fable.Core
open Fable.Core.TS
open Partas.Solid
open Partas.Solid.Web

[<SolidComponent>]
let App () = h1 () { "Hello from F#" }

match document.getElementById "root" with
| Some root -> render ((fun () -> App ()), root) |> ignore
| None -> failwith "Missing root element"
```

Keep the disposer returned by `render` when the host application needs to unmount explicitly. Use Xantham's
`Fable.Core.TS` DOM types and the globals bound by Partas; adding `Fable.Browser.Dom` creates a competing DOM type
surface. Partas.Solid 3 has no SolidStart bindings; choose hosting independently of obsolete 2.x templates.

Sources: [installation](https://shayanhabibi.github.io/Partas.Solid/guide/installation/),
[Solid 2 migration](https://shayanhabibi.github.io/Partas.Solid/guide/migrating-to-solid-2/),
[common issues](https://shayanhabibi.github.io/Partas.Solid/guide/common-issues/).
