---
title: Workbench
---

The workbench is a [SageFs](https://github.com/WillEhrendreich/SageFs) session that runs Fable in-process. It keeps
Fable's F# checker warm between compiles, so the loop for a plugin or binding fix takes seconds rather than a full
test run:

| Change | What to run | Time (all snapshot cases) |
| --- | --- | --- |
| a binding in `Partas.Solid/`, or a test input | `Workbench.checkAll ()` | under 1 s |
| the plugin (`Partas.Solid.FablePlugin/`) | `Workbench.reloadPlugin ()`, then `Workbench.checkAll ()` | about 2.5 s |

The first compile in a session takes about 10 s, because it cracks the project. The build CLI's `test` command stays
the final gate.

:::note
`Workbench/README.md` is the full reference. This page covers what you need to start.
:::

## Setup

From the repository root:

```bash
dotnet fsi workbench.fsx
```

This clones [Fable.SageFs](https://github.com/shayanhabibi/Fable.SageFs) into the gitignored `.workbench/`, points it
at the Fable version in `.config/dotnet-tools.json`, and builds the workbench. Rerun it after changing the Fable
version. Fable ships its own fork of the F# compiler service, which clashes with the one SageFs is built on, and
Fable.SageFs renames the fork so both can load.

Then create a SageFs session on `Workbench/Partas.Solid.Workbench.fsproj`, with `Workbench/` as the working
directory. The workbench is not in `Partas.Solid.slnx`, because it only builds after setup.

## Snapshot cases

```fsharp
Workbench.checkAll ();;              // compile every snapshot case and diff it; prints "N/M cases pass"
Workbench.check "MergeProps";;       // one case: Passed, Diff (against .expected) and Errors
Workbench.jsx "MergeProps";;         // the JSX the case compiles to now
Workbench.accept "MergeProps";;      // write that JSX to the case's .expected
Workbench.reloadPlugin ();;          // rebuild the plugin; the next compile uses it
Workbench.compile "<.fsproj>" "<.fs>";;  // any file in any project, e.g. a ScratchTests file
Workbench.reset ();;                 // after an .fsproj edit or a new source file
```

A case is found by its Expecto name or its folder name, by dotted suffix and case-insensitively, so
`"MergeProps"`, `"SolidCases.MergeProps"` and `"Setting default properties"` are the same case.

`reloadPlugin` keeps the checker warm unless the plugin's public surface (its attributes and `ComponentFlag`)
changed. Nearly all of the plugin is `internal`, so most edits never cost a re-type-check.

## Printing the AST

The plugin sees the AST after Fable's own passes (beta reduction, uncurrying), not the one you might guess from the
F# source. `Workbench.ast` prints what the plugin receives for one member, without running the plugin:

```fsharp
Workbench.ast "<path to .fs>" "PendingBadge__get_View" 8;;   // 8 levels deep; 0 prints the whole tree
Workbench.ast "<path to .fs>" "?" 0;;                        // an unknown name lists the file's members
Workbench.astWith { AstPrinter.Options.Default with Detail = AstPrinter.Full } "<path to .fs>" "Foo";;
```

Type components are named `<Type>__get_View`. Each line is one node, labelled with the field it sits in, so a line
reads as the pattern that reaches it. This is usually faster than compiling with `ComponentFlag.DebugMode`.

## Runtime tests

```fsharp
Workbench.emit Workbench.Suite.Dom;;   // write the suite's .fs.jsx files; returns the error logs
```

Then run `node run.mjs dom --no-compile` from `Partas.Solid.Tests.Runtime/`. `emit` writes the same files as
`dotnet fable -o .`, including the `Partas.Solid` sources, so binding edits reach the runtime tests too.

## Watching

```fsharp
Workbench.watchFiles [ "<.fs or .fs.jsx>" ];;   // emit just these files now, and after every save
Workbench.watchCases [ "MergeProps" ];;         // check these cases now, and after every save
Workbench.watchSuite Workbench.Suite.Dom;;      // emit the suite now, and after every save
Workbench.unwatch ();;
```

The watcher reacts to saved `.fs` files in the plugin, the bindings and the test inputs, and reloads the plugin first
when it changed. `watchFiles` is the tightest loop: keep a `.fs.jsx` open in your editor, and it follows a plugin edit
about 3 s after you save. To rerun specs on every save as well, run `watchSuite` alongside `npx vitest --dir Dom` in
`Partas.Solid.Tests.Runtime/`.

:::caution
`reloadPlugin`, a project's first compile, and `emit` take the same `.compile-lock` as `run.mjs`. They wait for a
running `run.mjs` compile, and it waits for them.
:::

## Why not hot reload?

SageFs can hot-patch a function you redefine in the session, but that does not reach the plugin. Almost all of it is
`internal`, which SageFs declines to patch, and the plugin must be a Release build. Rebuilding it takes about 2 s, and
the project stays cracked, which is the expensive part.
