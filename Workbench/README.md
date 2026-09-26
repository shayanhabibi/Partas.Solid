# Workbench

A [SageFs](https://github.com/WillEhrendreich/SageFs) session that compiles Partas.Solid projects with Fable
in-process, for working on the plugin and the bindings. It keeps Fable's F# checker warm, so after the first
compile the loop is:

| Change | What to run | Time (all 50 snapshot cases) |
| --- | --- | --- |
| A binding in `Partas.Solid/`, or a test input | `Workbench.checkAll ();;` | under 1 s |
| The plugin (`Partas.Solid.FablePlugin/`) | `Workbench.reloadPlugin ();;` then `Workbench.checkAll ();;` | about 2.5 s; 5.5 s if its public surface changed |

The first compile in a session takes about 10 s, because it cracks the project. A `dotnet run --project
partas-solid.fsproj -- test` stays the final gate.

It uses [Fable.SageFs](https://github.com/shayanhabibi/Fable.SageFs) to load Fable in the session: Fable ships its
own fork of FSharp.Compiler.Service, which clashes with the FCS that SageFs is built on, so Fable.SageFs renames the
fork (`Fable.FSharp.Compiler.Service`).

## Setup

```powershell
dotnet fsi workbench.fsx
```

It clones Fable.SageFs into `.workbench/` (gitignored), points it at the Fable version in
`.config/dotnet-tools.json`, and builds the workbench. Rerun it after changing that Fable version. Fable.SageFs's
setup ends by building its own sample session, which targets a newer Fable; with 5.13 that step fails after
`vendor/` is written, and the script says so and carries on.

Then create a SageFs session on `Workbench/Partas.Solid.Workbench.fsproj` with working directory `Workbench/`
(`create_project_session` over MCP).

## Use

```fsharp
Workbench.selfCheck ();;                 // Fable's FCS is the renamed fork, and MergeProps matches its snapshot
Workbench.cases ();;                     // every snapshot case: Name "SolidCases.MergeProps", Test its Expecto name
Workbench.check "MergeProps";;           // one case: Passed, Diff (line diff against .expected), Errors
Workbench.checkAll ();;                  // prints "N/M cases pass"; returns the failures
Workbench.jsx "MergeProps";;             // the JSX a case compiles to now
Workbench.accept "MergeProps";;          // write that JSX to the case's .expected
Workbench.reloadPlugin ();;              // rebuild the plugin and use the new build from the next compile
Workbench.compile "<.fsproj>" "<.fs>";;  // any file of any project, e.g. a ScratchTests file
```

A case is found by its name or by its Expecto test name, whole or by a dotted suffix, case-insensitively:
`"MergeProps"`, `"SolidCases.MergeProps"` and `"Setting default properties"` are the same case. An ambiguous or
unknown name fails with the candidates.

When a reload changes the plugin's public surface, `reloadPlugin` also recreates each project's checker on its
next compile, because the checker holds the plugin's metadata: a new `ComponentFlag` or attribute is then seen
without a `reset ()`. `reset ()` is for `.fsproj` edits and new source files.

### Runtime tests

```fsharp
Workbench.emit Workbench.Suite.Dom;;     // write the suite's .fs.jsx files; returns the error logs
```

then, from `Partas.Solid.Tests.Runtime/`, `node run.mjs dom --no-compile`. `emit` writes what `dotnet fable -o .`
writes: the fixtures, and the `Partas.Solid` sources under `<Suite>/Partas.Solid/`, so binding edits reach the
runtime tests too. The first `emit` of a suite in a session resets its `fable_modules` and copies `fable-library`
in, as the CLI does. For all three suites the files are byte-identical to the CLI's.

### Watch

```fsharp
Workbench.watchFiles [ "<.fs or .fs.jsx>" ];; // emit just these files now and after every save
Workbench.watchCases [ "MergeProps" ];;  // check now and after every save; rewrites each case's .fs.jsx
Workbench.watchSuite Workbench.Suite.Dom;; // emit now and after every save
Workbench.unwatch ();;
```

A watcher reacts to saved `.fs` files in `Partas.Solid.FablePlugin/`, `Partas.Solid/` and the test inputs. After
a plugin save it runs `reloadPlugin` first. `watchFiles` is the tightest loop for a plugin fix: it takes the
files you have open, a snapshot case, a runtime fixture or a ScratchTests file, and compiles only those, each
with its own project. With one of them open in the editor, a plugin edit shows up there about 3 s after saving
(2 s of that is the plugin build). A binding or test-input edit shows up in about 1 s.

The checker only holds the plugin's public surface (its attributes, `ComponentFlag`), so `reloadPlugin` keeps
it warm unless that surface changed; an edit to the plugin's `internal` code, which is nearly all of it, never
costs a re-type-check. When the surface does change, the next compile type-checks the project again, about 3.5 s.
The checker reads a copy of the plugin DLL from `.workbench/plugin-ref/<hash>/`, one per surface. Creating a
checker deletes the copies of the other surfaces, so one is normally all there is; a copy that is still open
stays until a later run. `Workbench.cleanPluginRefs ()` does that cleanup on demand. Saves within 300 ms run once. A failed
build or compile is printed, and the watcher keeps going. Starting a watcher stops the previous one. `Workbench.watch (fun () -> ...)` runs your own
action instead.

To rerun the runtime specs on every save, run `watchSuite` together with `npx vitest --dir Dom` from
`Partas.Solid.Tests.Runtime/`.

`reloadPlugin`, the first compile of a project, and `emit` take `run.mjs`'s cross-process compile lock
(`Partas.Solid.Tests.Runtime/.compile-lock`). They wait for a running `run.mjs` or `watch.mjs` compile, and those
wait for them.

## How it works

`Workbench.fs` runs Fable's pipeline (type-check, `FSharp2Fable`, `FableTransforms`, `Fable2Babel`, print)
with the same options the tests pass `dotnet fable`: `-c Release --optimize -e .fs.jsx --exclude
Partas.Solid.FablePlugin`. Fable loads plugins through a `getPlugin` callback, which the workbench redirects to its
current copy of `Partas.Solid.FablePlugin`. `reloadPlugin` builds the plugin in Release and loads the result into a
new collectible `AssemblyLoadContext`. That context resolves nothing itself, so the plugin shares Fable.AST and
FSharp.Core with the session. Fable looks for `fable-library` next to its host process, which is SageFs here, so
the workbench copies it from the Fable tool package in the NuGet cache (`dotnet tool restore` puts it there).

### Why not SageFs hot reload for the plugin

SageFs hot-patches a function by detouring its method to a redefinition sent as an eval. That does not reach the
plugin:

- Almost all of the plugin is `internal` (`module internal rec AST`, `Baked`, `PluginContext`), and SageFs declines
  to patch a module that is not public.
- An eval redefinition of a public plugin function did not detour either, whether the plugin was referenced or its
  sources were compiled into the workbench.
- Detours also need an unoptimized build. A Debug build of the plugin from the SDK 10.0.4xx or 11 fsc throws
  `InvalidProgramException` in `PropCollector`, so the plugin is built in Release, as the CLI builds it.

Rebuilding the plugin takes about 2 s. The project stays cracked, which is the expensive part. SageFs hot reload
still works for `Workbench.fs` itself.

## Files

| Path | |
| --- | --- |
| `workbench.fsx` (repo root) | Setup. Pins the Fable.SageFs commit. |
| `Workbench/Partas.Solid.Workbench.fsproj` | The session project. Imports `.workbench/Fable.SageFs/vendor/Fable.SageFs.props`, so it builds only after setup. |
| `Workbench/Workbench.fs` | Everything above. A top-level module, because SageFs rejects `namespace` in evals. |
| `.workbench/` | Gitignored. The Fable.SageFs clone. |
| `Workbench/.SageFs/` | Gitignored. SageFs's session cache. |
