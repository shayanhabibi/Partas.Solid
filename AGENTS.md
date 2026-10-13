# AGENTS.md

This file provides guidance to coding agents (Codex, Claude Code, ...) when working with code in this repository.
Claude Code reads it through `CLAUDE.md`, which imports it; edit this file, not that one.

## What this is

Partas.Solid is an F#/Fable front-end framework for [solid-js](https://www.solidjs.com/), an opinionated fork of
Oxpecker.Solid. It ships two NuGet packages that are always versioned in lockstep:

- **Partas.Solid** — the DSL/bindings library (tags, HTML/ARIA/SVG attributes, CSS-in-F# style spec, solid-js /
  solid-router / solid-meta bindings). Almost all of it is `[<Erase>]`/import surface: the types exist to
  give F# a typed authoring experience and are erased by the plugin.
- **Partas.Solid.FablePlugin** — a Fable compiler plugin (`MemberDeclarationPluginAttribute`) that rewrites the F# AST
  into JSX. This is where the real behaviour lives; the library alone produces nothing useful.

Because the plugin rewrites the AST rather than emitting from a runtime, **the compiled `.jsx` output is the
specification**. Behaviour changes are observed by diffing generated JSX, not by running the framework.

Layout: the two shipped projects live in `src/`; the snapshot, runtime and unit test projects and `ScratchTests` in
`tests/`; the build script (`tools/build.fsx`) and the SageFs workbench (`tools/Workbench/`,
set up by `tools/workbench.fsx`) in `tools/`. `Partas.Solid.slnx`, `docs/` and the `solid/` submodule stay at the root.

## `solid/` — vendored upstream, NOT part of this project

`solid/` is a **git submodule** of [`solidjs/solid`](https://github.com/solidjs/solid) (branch `next`), pinned at tag
`solid-js@2.0.0-rc.14` (`8d23a5a1`). It is upstream TypeScript/JavaScript/Rust reference material, checked out purely so
the Solid 2 API can be consulted while porting the bindings. **It is not built, tested, packaged, formatted, or shipped
by anything in this repository, and no F# source depends on it.**

As of rc.9 the monorepo has absorbed dom-expressions: `solid/packages/` holds `solid`, `signals`, `web`, `compiler`,
`babel-plugin`, `element`, `h`, `html`, `universal` and `diagnostics`. The web runtime is published as `@solidjs/web`
(`solid/packages/web`), the JSX/DOM types are authored in `solid/packages/web/jsx/`, the JSX compiler is Rust
(`solid/packages/compiler/src`), and the store API is exported from `solid-js` itself (there is no `solid-js/store`).

**Do not search it by default.** Glob/Grep/find over the repo root will otherwise drown in tens of thousands of
irrelevant `.ts`, `.js`, `.rs`, `.md` and `node_modules` files. Scope searches to the F# projects — `src/Partas.Solid/`,
`src/Partas.Solid.FablePlugin/`, `tests/Partas.Solid.Tests.*/`, `tests/ScratchTests/`, `docs/` — or pass `--glob '!solid/**'`.

Read from it **only** when the question is explicitly about upstream Solid behaviour that cannot be answered from this
repo, e.g.:

- confirming a Solid 2 export's name, signature, or module
- checking what JSX shape the Solid 2 compiler/runtime actually expects, to justify a plugin output change
- reading the upstream migration notes when deciding how a binding must change

**Check [`docs/API-COVERAGE-solid2.md`](docs/API-COVERAGE-solid2.md) first** — it already maps the upstream
surface to our bindings with clickable `file:line` anchors, and records what this checkout *cannot* answer. Most
binding questions are answered there without opening `solid/` at all.

Anchors when you do need to look (package dirs are `web` and `signals`, not `solid-web` / `solid-signals`):

| Question | File |
| --- | --- |
| What does `solid-js` export? What was **removed** in 2.0 and why? | [`solid/packages/solid/src/index.ts`](solid/packages/solid/src/index.ts) (removals: [`:255-313`](solid/packages/solid/src/index.ts#L255-L313)) |
| Migration notes | [`solid/documentation/solid-2.0/`](solid/documentation/solid-2.0/) |
| Reactivity + store primitives | [`signals/src/index.ts`](solid/packages/signals/src/index.ts), [`src/store/index.ts`](solid/packages/signals/src/store/index.ts) |
| `For`/`Show`/`Switch`/`Match`/`Errored`/`Loading`/`Repeat`/`Reveal` | [`solid/packages/solid/src/client/flow.ts`](solid/packages/solid/src/client/flow.ts) |
| `createSignal`/`createMemo`/`createEffect`/`createStore` impls | [`solid/packages/solid/src/client/hydration.ts`](solid/packages/solid/src/client/hydration.ts) |
| `render`/`hydrate` | [`web/src/client.ts`](solid/packages/web/src/client.ts) (`render` [`:294`](solid/packages/web/src/client.ts#L294), `hydrate` [`:2012`](solid/packages/web/src/client.ts#L2012)) |
| `Portal`/`Dynamic`/`clientOnly` | [`solid/packages/web/src/index.ts`](solid/packages/web/src/index.ts) |
| SSR entry points | [`web/src/server-mock.ts`](solid/packages/web/src/server-mock.ts), [`solid/packages/solid/src/server/`](solid/packages/solid/src/server/) |
| Attribute / event / tag typings | [`web/jsx/jsx.d.ts`](solid/packages/web/jsx/jsx.d.ts) |
| How `prop:*` keys are derived | [`web/jsx/jsx-properties.d.ts`](solid/packages/web/jsx/jsx-properties.d.ts) |
| What the compiler actually emits | [`compiler/src/dom/`](solid/packages/compiler/src/dom/) (Rust; SSR in [`compiler/src/ssr/`](solid/packages/compiler/src/ssr/)) |

Coverage of the JSX/DOM surface is tracked in [`docs/API-COVERAGE-solid2.md`](docs/API-COVERAGE-solid2.md) §5 — read
that before re-deriving a diff by hand.

Not in this checkout, so don't search for them: **`@solidjs/router` / `@solidjs/meta`** (separate
repos).

Never edit files under `solid/`. Changes there are upstream's, and committing inside the submodule would move the
gitlink for everyone. To update the pin deliberately: `git -C solid fetch --tags origin`, check out the new
`solid-js@<version>` tag, then commit the changed gitlink in this repo.

> **If `solid/` vanishes** (a since-removed build CLI's `clean` wiped it), restore with
> `git submodule add --force -b next https://github.com/solidjs/solid.git solid` — the `.git/modules` cache
> survives, so this reactivates the local clone without re-downloading. Then check out the pinned tag again.
> (The former `dom-expressions/` submodule was removed after rc.9 absorbed it; use `solid/packages/web/jsx/` and
> `solid/packages/compiler/` instead.)

## Build & test

The entry point is `tools/build.fsx`, a Partas.Build script; CI runs nothing else. `dotnet fsi tools/build.fsx -- --help`
lists the commands, and `<command> --help` or `--explain` shows a command's flags and stage tree without running it:

```powershell
dotnet fsi tools/build.fsx -- test      # restore, fable clean, build, Expecto suites, then the runtime (vitest) suites
dotnet fsi tools/build.fsx -- build     # build Partas.Solid + FablePlugin
dotnet fsi tools/build.fsx -- docs      # build the Nacara site in docs/site (--watch to serve it)
```

Run it from the repository root: its globs resolve against the current directory. `-q/--quick` skips restores, cleans and
`fable clean`, and is the flag to reach for during iteration. `--skip-tests` skips the suites. The script restores its
Partas.Build packages from the Cloudsmith feed named by its `#i` line.

After Expecto, `test` runs `node tests/Partas.Solid.Tests.Runtime/run.mjs all`, which runs `npm ci` itself when
`node_modules` is missing. It needs Node >= 22.12 on `PATH`.

Format with `dotnet fantomas <files>` directly, and compile ScratchTests with `dotnet fable --exclude
Partas.Solid.FablePlugin --noCache -e .fs.jsx --optimize --typedArrays false [--watch]` inside `tests/ScratchTests/` (or `build.fsx test scratch`).

### Generated JSX surface (`tools/JsxGen`)

**Generated, never hand-edited:** `HtmlAttributes.fs`, `AriaAttributes.fs`, `Svg.fs`, and everything after the
`// <generated>` line of `Tags.fs`. Change `tools/JsxGen/overrides.json` or the generator instead. `tools/JsxGen`
(ported from Ranvier's DOM generator) reads `@solidjs/web`'s `namespace JSX` through the pinned TypeScript 7 compiler
(Xantham.TypeScript.Wire), so interfaces, `extends` chains, member types and event types come from the compiler.
Regenerate with `npm ci` in `tools/JsxGen`, then `dotnet run -c Release --project tools/JsxGen`.
`tools/JsxGen/out/diff.md` reports HTML drift against the file being replaced.

- Closed literal sets become `[<StringEnum>]` unions in `module Attr` (`dir = Attr.Dir.Rtl`; ARIA's take an `Aria`
  prefix, SVG's `Svg`). A boolean spelled only `""`/`"true"`/`"false"` is `bool`. HTML `string | number` is `int`,
  SVG's `U2<float, string>`. Open strings are `string` with a Rider `LanguageInjection`.
- Event handlers are typed by their event: `onClick: Dom.MouseEvent -> unit`, checked against `Fable.Core.TS.Dom`.
- ARIA is `module Aria` on `HtmlTag`. Its camelCase names come from the existing file (they cannot be derived);
  a new ARIA attribute needs an `ariaNames` entry in `overrides.json`, or generation stops.
- SVG gives each attribute several interfaces declare one provider interface (`strokeAttributeInterfaceProvider`)
  that those interfaces inherit, so the property is never ambiguous. Hyphenated attributes are a hidden
  ``` ``stroke-width`` ``` member plus a camelCase alias.
- A per-element member that redeclares a global one (`dir` on `<bdo>`, `id` on SVG) is dropped: it would be ambiguous.
- `overrides.json` holds what the declarations cannot say: interface renames, per-member types, skipped members, ARIA
  names, and `aliases` (legacy camelCase HTML spellings kept as `[<Obsolete>]` members).
- `HtmlEvents.fs` keeps the hand-written members: `children`, `style` and `inlist`. Marker interfaces `Builder.fs`
  lacks (MathML, newer HTML elements) are generated at the top of `HtmlAttributes.fs`.
- Tags come from the tag maps. Existing tags keep their identifier (`option'`, `select`). Element types are the ones
  `Fable.Core.TS.Dom` declares, falling back to `HTMLElement`. Void elements are a list in `TagsEmit.fs`, because the
  declarations do not say. MathML tags sit behind `MathML.`, SVG's behind `Svg.`.
- Solid's compiler rejects a `<template>` with static children (the browser moves them into `.content`), and
  jsdom drops a `<button>` inside `<select>`, so the runtime tests render both without those children.
- Upstream bumps are a `package.json` change in `tools/JsxGen`; regenerate and read `diff.md`.

### DOM types: Xantham only

Partas.Solid does not reference `Fable.Browser.Dom`. DOM types come from `Xantham.Fable.Core.TS`
(`open Fable.Core.TS`, then `Dom.HTMLElement`). Its published releases bind no global values, so `src/Partas.Solid/Prelude.fs`
(first in compile order) binds `globalThis`, `document` and `window` in an auto-opened module of Xantham's own
`Fable.Core` namespace. Its `globalThis` shadows the one Xantham's unreleased `TSExtensions` adds: when that ships, the
binding can be deleted or left, as both are the same global. Its lookups return options (`getElementById`), and its
`addEventListener` overloads are typed: call the string form through `EventTarget` with an
`EventListenerOrEventListenerObject`. Write DOM types as `Dom.X` after `open Fable.Core.TS`, rather than opening `Fable.Core.TS.Dom`: DOM has an `Attr` type that would
shadow Partas's `Attr` module, which keeps its name (as usual in F#, the qualified access is the fix).

### Releases (ShipIt)

[EasyBuild.ShipIt](https://github.com/easybuild-org/EasyBuild.ShipIt) owns the version. `src/CHANGELOG.md` is the one
changelog for both packages: its front matter registers both `.fsproj` `<Version>` elements as updaters, so they stay in
lockstep. ShipIt calculates the next version from the **conventional commits** touching `src/` since
`last_commit_released`, so commit messages must be conventional (`feat:`, `fix:`, `feat!:` ...). Edit only that front
matter; ShipIt rewrites the body. Releases up to 2.1.3 are in `docs/RELEASE_NOTES.md`, frozen.

- `build.fsx -- bump --dry-run` previews the next version; without `--dry-run` it writes the changelog and both
  versions locally. Add `--allow-branch <branch>` off `master`.
- `build.fsx -- release` opens the release pull request on GitHub; CI runs it.
- The ShipIt stages run from `src/`: from the root, ShipIt would treat every `solid/**/CHANGELOG.md` as a project.

`.github/workflows/release.yml` drives it. Each push to `master` runs `dotnet.yml` (the CI tests), then ShipIt opens or
updates the `chore: release Partas.Solid@x.y.z` pull request. Merging it pushes that commit, which runs `publish` to the
**temporary Cloudsmith feed** (`https://nuget.cloudsmith.io/shayanhabibi/partas-solid/v3/index.json`, secret
`CLOUDSMITH_API_KEY`) and creates the GitHub release and tag (`x.y.z`, no `v`). nuget.org publishing is paused while
package ownership is transferred; the README banner and the docs-site installation page say so, and change together
when it resumes.

Tools are pinned in `.config/dotnet-tools.json` (`fantomas` 7.0.5, `fable` 5.13.0) — run `dotnet tool restore` first if
invoking `dotnet fable` directly.

### Running a single test

Tests are Expecto. Test names are derived from directory structure (see below), formatted `<Category>.<CaseName>` or
`<Category>.<CaseName>.<file>.fs.jsx` when a case has multiple files:

```powershell
dotnet run --project tests/Partas.Solid.Tests.Plugin/Partas.Solid.Tests.Plugin.fsproj -- --filter "Plugin/SolidCases.MergeProps"
dotnet run --project tests/Partas.Solid.Tests.Plugin/Partas.Solid.Tests.Plugin.fsproj -- --filter-test-list SolidCases
```

The test assembly first runs `buildCases()`, which shells out to
`dotnet fable --exclude Partas.Solid.FablePlugin --noCache -e .fs.jsx -c Release --optimize --typedArrays false` inside
`tests/Partas.Solid.Tests.Plugin/Compiled/`. That step must succeed or every test fails.

Runtime tests (vitest) are run through `run.mjs` from `tests/Partas.Solid.Tests.Runtime/`:

```powershell
node run.mjs dom                                   # Fable-compile the Dom suite, then run its specs
node run.mjs integration Apps -t "sorts by name"   # extra args go to vitest: path filter, -t name filter
node run.mjs primitives --no-compile               # rerun specs without recompiling (fast, when only .test.js changed)
node run.mjs all                                   # what build.fsx test runs
node run.mjs integration --watch                   # fable watch + vitest watch; plugin edits rebuild the plugin and restart
```

## Test architecture (snapshot tests over generated JSX)

`tests/Partas.Solid.Tests.Plugin/Compiled/` is a **separate Fable project** (`Partas.Solid.Tests.Plugin.Compiled.fsproj`,
globbing `*Cases\**\*.fs`) that references the DSL and the plugin. Layout:

```
Compiled/<Category>Cases/<Human readable case name>/<CaseName>.fs        <- F# input
                                                    <CaseName>.fs.jsx    <- generated (gitignored? committed)
                                                    <CaseName>.expected  <- committed snapshot
```

Categories currently: `IssueCases` (regression per GitHub issue; folder names carry the issue number),
`SolidCases` (feature/DSL behaviour), `AttributeCases` (`PartasImport`, `Pojo`).

`Common.fs` discovers cases by walking for `*.expected`, pairing each with the sibling `.fs.jsx`, and grouping by
directory. `Tests.fs` turns each pair into a `testCase` that string-compares the trimmed files. Consequences worth
knowing:

- **Adding a test = adding a folder.** Write `Foo.fs` under a `*Cases` directory, run the fable compile, inspect the
  produced `Foo.fs.jsx`, and — once correct — copy it to `Foo.expected`. No test registration code to edit.
- A `.expected` with no matching `.fs.jsx` is silently dropped (partitioned into an ignored `_sourceNotFound`), so a
  case that fails to compile disappears rather than failing loudly. If a test vanishes, check the fable output.
- The commented-out explicit test lists at the bottom of `Tests.fs` are the pre-discovery scheme; they document the
  intent of each case name and are worth reading when naming a new case.

## Runtime tests (`tests/Partas.Solid.Tests.Runtime/`)

The snapshot tests pin *what* the plugin emits. The runtime tests check *what that output does*. F# fixtures are
compiled by Fable and the plugin (same flags as the snapshots, plus `-o .`), then compiled again by the real Solid
2 JSX compiler (`@solidjs/vite-plugin` / `@solidjs/compiler`, pinned to rc.14 with the other Solid packages in
`package.json`). vitest runs the result in jsdom against the browser development build of `solid-js` and `@solidjs/web`.
`README.md` there is the full guide. It covers the helpers and Solid 2 scheduling: setters only queue a microtask, so
call `flush()` or use `click`/`input`/`act` before you assert.

Three suites, each its own Fable project (`<Suite>/Partas.Solid.Tests.Runtime.<Suite>.fsproj`, globbing `**\*.fs`):
`Primitives` (reactivity and stores, no DOM), `Dom` (elements, attributes, events, refs, SVG, `@solidjs/web`), and
`Integration` (components, control flow, context, async, small apps).

```
<Suite>/<CaseFolder>/A-Foo.fs        <- F# fixture; module Partas.Solid.Tests.Runtime.<Suite>.<CaseFolder>.<File>
                     A-Foo.fs.jsx    <- generated, gitignored
                     Foo.test.js(x)  <- vitest spec importing ./A-Foo.fs.jsx
```

- **Adding a test = adding a folder** with a fixture and a spec. Module names must be unique within a suite, and
  files compile in path order (`A-`/`B-` prefixes for dependencies). `Partas.Solid`, `fable_modules`, `bin`, `obj`
  and `node_modules` are reserved folder names in a suite, because Fable writes its output there.
- **One broken fixture breaks the whole suite.** A compile error fails the suite's Fable step, so every spec in it
  fails too. Run `node run.mjs <suite>` before finishing.
- **Known bugs are `it.fails`.** When runtime behaviour is wrong because of Partas.Solid, keep the correct assertion,
  mark the test `it.fails(...)`, and put `// BUG: <one line>` next to it. Never weaken the assertion. When the bug is
  fixed, vitest reports the `it.fails` as failing; change it to `it` then. `grep -rn "BUG:"` over the suites is
  the current list of known defects. A defect in Solid itself takes `// UPSTREAM: <one line>` instead, once the same
  failure is reproduced with hand-written JSX; recheck those on every Solid bump.
- `run.mjs` serialises Fable compiles across processes with `.compile-lock/`. It does not coordinate with a
  concurrent snapshot-test build, and both of them build `Partas.Solid` and the plugin.

`tests/ScratchTests/` is an unversioned playground for the same loop (`dotnet fable ... --watch`, see above) when you
want to eyeball JSX for input that isn't yet a test case. `Partas.Solid.Tests.Core` is a small unit-test project.

### Workbench (SageFs)

`tools/Workbench/` is a SageFs session project that runs Fable in-process with a warm checker, for closing plugin and
binding bugs quickly. Set it up with `dotnet fsi tools/workbench.fsx`, which clones Fable.SageFs into the gitignored
`.workbench/`. Then create a SageFs session on `tools/Workbench/Partas.Solid.Workbench.fsproj`.

- `Workbench.checkAll ()` recompiles every snapshot case and diffs it, in under a second once the session is warm.
- After a plugin edit, `Workbench.reloadPlugin ()` rebuilds and reloads the plugin; with the next `checkAll` that
  takes about 2.5 s. The checker stays warm unless the plugin's public surface changed.
- `Workbench.emit <Suite>` writes a runtime suite's `.fs.jsx` files as `dotnet fable -o .` does, for
  `node run.mjs <suite> --no-compile`. The workbench takes `run.mjs`'s `.compile-lock` while it builds or writes.
- `Workbench.watchFiles [ "<.fs or .fs.jsx>" ]` / `watchCases [ "<case>" ]` / `watchSuite <Suite>` redo that on
  every `.fs` save in the plugin, bindings or test inputs, reloading the plugin first when it changed. An open
  `.fs.jsx` follows along, about 3 s after a plugin save with `watchFiles`. `Workbench.unwatch ()` stops it.

SageFs eval hot-patching does not reach the plugin, whose code is almost all `internal`. `tools/Workbench/README.md` has
the details. The workbench isn't in `Partas.Solid.slnx`, because it only builds after setup.

## Plugin architecture

Compilation order in `Partas.Solid.FablePlugin.fsproj` reflects the dependency chain:

| File | Role |
| --- | --- |
| `Utils.fs` | String/AST active patterns (`Utils.StartsWith`, `EndsWith`, `StartsWithTrimmed`), `AstUtils` constructors |
| `Types.fs` | `PluginContext`, `ComponentFlag`, `IdentType`, `TransformationKind`, `MemberRefType` |
| `Spec.fs` | `FableRequirements` (min Fable 5.0, JS + `.js`/`.jsx` only) and `SchemaRules` — what shapes are legal input |
| `Plugin.fs` | The transformation tree and the two public attributes. Excluded from fantomas formatting. |
| `Storybook.fs` | Separate `MemberDeclarationPluginAttribute` for Storybook CSF output |

**Entry points** (`Plugin.fs`, bottom of file):

- `[<SolidComponent>]` — applies to `let` bindings. Just runs `AST.transform ctx` over the body.
- `[<SolidTypeComponent>]` — applies to type members (`member props.View = ...`). Requires a strict shape checked by
  `SchemaRules.ValidMemberRef`: instance member, self-identifier literally named `props`, single unit parameter,
  declaring entity under the `Partas.Solid` namespace. On match it renames the member to the declaring type's
  `DisplayName` and prepends a `const PARTAS_OTHERS = omit(props, ...)` binding (from collected getters; plain
  `= props` when none were collected, and omitted entirely under `ComponentFlag.SkipOmit`) and a `props = merge({...}, props)` default-props
  assignment (from collected setters). Both `omit` and `merge` are imported from `solid-js`.
  On mismatch it warns and falls back to plain transformation.

**`PluginContext` is the spine.** It threads `PluginHelper` (for `LogWarning`/`LogError`/`GetEntity`/`GetMember`)
through every pattern, and carries two mutable accumulators — `GetterArray` and `SetterArray` — that transformation
patterns push into as they encounter `props.foo` reads and property assignments. `getGetters`/`getSetters` *drain*
these; `peekGetters`/`peekSetters` don't. Duplicate setters are a hard `LogError`.

**`ComponentFlag`** is a bit flag passed to either attribute (`[<SolidComponent(ComponentFlag.DebugMode)>]`) to opt out
of optimisations or turn on diagnostics: `DebugMode` (dump the pre-transform AST — ask for this in bug reports),
`PrintDisposals` (log every expression discarded during transformation),
`SkipPojoOptimisation`, `SkipCEOptimisation`, `SkipOmit`, `SpreadProps`. `ComponentFlag.None` = all skippable
optimisations off; `ComponentFlag.VerboseDebugMode` = `DebugMode ||| PrintDisposals`.

When a transformation silently drops output, `PluginContext.debugDisposal` is the mechanism that will tell you where —
compile that component with `PrintDisposals`.

## Conventions

- **Fantomas is configured but not currently enforced.** `.editorconfig` drives it. `src/Partas.Solid.FablePlugin/Plugin.fs`
  and any `**/IndexAccess/IndexAccess.fs` are excluded from formatting — don't run fantomas over them manually. No build
  command runs fantomas, and most sources (including `HEAD`'s bindings) do not pass `fantomas --check`, so a repo-wide
  run produces a large reformatting diff. Format only the files you touch, and match their existing layout.
- `WarningsAsErrors` includes `3239` and `0025` (incomplete pattern matches) in both shipped projects: an inexhaustive
  match in the plugin is a build failure, not a warning.
- Target frameworks intentionally differ: `Partas.Solid` `net8.0`, `Partas.Solid.FablePlugin` `net6.0`, plugin test host
  `net8.0`, the fable test project and `Partas.Solid.Tests.Core` `net9.0`, ScratchTests `net10.0`.
- Versions are duplicated in both `.fsproj`s and written by ShipIt (see *Releases*); never bump them by hand.
- `.fs.js` / `.fs.jsx` files sitting next to sources in `src/Partas.Solid/` and the plugin are fable build artifacts, not
  inputs.
