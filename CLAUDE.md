# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

Partas.Solid is an F#/Fable front-end framework for [solid-js](https://www.solidjs.com/), an opinionated fork of
Oxpecker.Solid. It ships two NuGet packages that are always versioned in lockstep:

- **Partas.Solid** — the DSL/bindings library (tags, HTML/ARIA/SVG attributes, CSS-in-F# style spec, solid-js /
  solid-router / solid-meta / solid-start bindings). Almost all of it is `[<Erase>]`/import surface: the types exist to
  give F# a typed authoring experience and are erased by the plugin.
- **Partas.Solid.FablePlugin** — a Fable compiler plugin (`MemberDeclarationPluginAttribute`) that rewrites the F# AST
  into JSX. This is where the real behaviour lives; the library alone produces nothing useful.

Because the plugin rewrites the AST rather than emitting from a runtime, **the compiled `.jsx` output is the
specification**. Behaviour changes are observed by diffing generated JSX, not by running the framework.

## `solid/` — vendored upstream, NOT part of this project

`solid/` is a **git submodule** of [`solidjs/solid`](https://github.com/solidjs/solid) (branch `next`), pinned at tag
`solid-js@2.0.0-rc.9` (`9a29b1a0`). It is upstream TypeScript/JavaScript/Rust reference material, checked out purely so
the Solid 2 API can be consulted while porting the bindings. **It is not built, tested, packaged, formatted, or shipped
by anything in this repository, and no F# source depends on it.**

As of rc.9 the monorepo has absorbed dom-expressions: `solid/packages/` holds `solid`, `signals`, `web`, `compiler`,
`babel-plugin`, `element`, `h`, `html`, `universal` and `diagnostics`. The web runtime is published as `@solidjs/web`
(`solid/packages/web`), the JSX/DOM types are authored in `solid/packages/web/jsx/`, the JSX compiler is Rust
(`solid/packages/compiler/src`), and the store API is exported from `solid-js` itself (there is no `solid-js/store`).

**Do not search it by default.** Glob/Grep/find over the repo root will otherwise drown in tens of thousands of
irrelevant `.ts`, `.js`, `.rs`, `.md` and `node_modules` files. Scope searches to the F# projects — `Partas.Solid/`,
`Partas.Solid.FablePlugin/`, `Partas.Solid.Tests.*/`, `ScratchTests/`, `docs/` — or pass `--glob '!solid/**'`.

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

Not in this checkout, so don't search for them: **`@solidjs/router` / `@solidjs/meta` / `@solidjs/start`** (separate
repos).

Never edit files under `solid/`. Changes there are upstream's, and committing inside the submodule would move the
gitlink for everyone. To update the pin deliberately: `git -C solid fetch --tags origin`, check out the new
`solid-js@<version>` tag, then commit the changed gitlink in this repo.

> **The build CLI's `clean` target wipes `solid/`.** If it vanishes, restore with
> `git submodule add --force -b next https://github.com/solidjs/solid.git solid` — the `.git/modules` cache
> survives, so this reactivates the local clone without re-downloading. Then check out the pinned tag again.
> (The former `dom-expressions/` submodule was removed after rc.9 absorbed it; use `solid/packages/web/jsx/` and
> `solid/packages/compiler/` instead.)

## Build & test

The current entry point is the `partas-solid.fsproj` build CLI (System.CommandLine + FAKE):

```powershell
dotnet run --project partas-solid.fsproj -- test      # clean, restore, build test projects, run Expecto tests
dotnet run --project partas-solid.fsproj -- build     # build Partas.Solid + FablePlugin (Release)
dotnet run --project partas-solid.fsproj -- publish --nuget <APIKEY>
dotnet run --project partas-solid.fsproj -- bump <bump>   # bump the version in both .fsproj files (skipped in CI)
```

Those four are the only registered commands. Useful flags: `-q/--quick` (skip tool restore, clean, solution restore)
and `--skip-tests`. Builds are always Release; there is no configuration flag. `--quick` is the flag to reach for during
iteration — a full `test` run cleans every `bin`, runs `fable clean`, and re-restores.

`Build/Program.fs` still defines a `format` stage and a `runScratch` input, but neither is wired to a command, and
`Build/Spec.fs` declares `--format`/`--dry-format` options that nothing reads. Format with `dotnet fantomas <files>`
directly, and compile ScratchTests with `dotnet fable --exclude Partas.Solid.FablePlugin --noCache -e .fs.jsx
--optimize [--watch]` inside `ScratchTests/`.

`build.fsx` is the older FAKE script still invoked by `.github/workflows/dotnet.yml` (`dotnet fsi ./build.fsx`). Note
its `Build` target targets `Partas.Solid.sln`, which no longer exists — the solution is now `Partas.Solid.slnx`. Prefer
the build CLI locally.

Tools are pinned in `.config/dotnet-tools.json` (`fantomas` 7.0.5, `fable` 5.13.0) — run `dotnet tool restore` first if
invoking `dotnet fable` directly.

### Running a single test

Tests are Expecto. Test names are derived from directory structure (see below), formatted `<Category>.<CaseName>` or
`<Category>.<CaseName>.<file>.fs.jsx` when a case has multiple files:

```powershell
dotnet run --project Partas.Solid.Tests.Plugin/Partas.Solid.Tests.Plugin.fsproj -- --filter "Plugin/SolidCases.MergeProps"
dotnet run --project Partas.Solid.Tests.Plugin/Partas.Solid.Tests.Plugin.fsproj -- --filter-test-list SolidCases
```

The test assembly first runs `buildCases()`, which shells out to
`dotnet fable --exclude Partas.Solid.FablePlugin --noCache -e .fs.jsx -c Release --optimize` inside
`Partas.Solid.Tests.Plugin/Compiled/`. That step must succeed or every test fails.

## Test architecture (snapshot tests over generated JSX)

`Partas.Solid.Tests.Plugin/Compiled/` is a **separate Fable project** (`Partas.Solid.Tests.Plugin.Compiled.fsproj`,
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

`ScratchTests/` is an unversioned playground for the same loop (`dotnet fable ... --watch`, see above) when you
want to eyeball JSX for input that isn't yet a test case. `Partas.Solid.Tests.Core` is a small unit-test project.

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

- **Fantomas is configured but not currently enforced.** `.editorconfig` drives it. `Partas.Solid.FablePlugin/Plugin.fs`
  and any `**/IndexAccess/IndexAccess.fs` are excluded from formatting — don't run fantomas over them manually. No build
  command runs fantomas, and most sources (including `HEAD`'s bindings) do not pass `fantomas --check`, so a repo-wide
  run produces a large reformatting diff. Format only the files you touch, and match their existing layout.
- `WarningsAsErrors` includes `3239` and `0025` (incomplete pattern matches) in both shipped projects: an inexhaustive
  match in the plugin is a build failure, not a warning.
- Target frameworks intentionally differ: `Partas.Solid` `net8.0`, `Partas.Solid.FablePlugin` `net6.0`, plugin test host
  `net8.0`, the fable test project and `Partas.Solid.Tests.Core` `net9.0`, build CLI and ScratchTests `net10.0`.
- Versions are duplicated in both `.fsproj`s and driven from `docs/RELEASE_NOTES.md` at pack time; `docs/RELEASE_NOTES.md`
  is generated by git-cliff (`cliff.toml`, `.cliffignore`), so commit messages should follow conventional-commit style.
- `.fs.js` / `.fs.jsx` files sitting next to sources in `Partas.Solid/` and the plugin are fable build artifacts, not
  inputs.
