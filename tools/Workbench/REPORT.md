# Workbench report: what the SageFs workbench bought Partas.Solid

Measured 2026-09-27. Figures come from the Claude Code session transcripts (tool-call and tool-result timestamps,
and timings printed by Fable and the workbench) and from runs taken for this report on the same machine.

## Summary

- The workbench cuts the snapshot check from about 14 s (CLI) or 18 s (Expecto) to about 0.33 s once warm.
  Checking a plugin edit takes about 1 to 3 s with `reloadPlugin` plus `checkAll`.
- Its output matches the CLI byte for byte: all 58 snapshot `.fs.jsx` files and all 28 Dom-suite files.
- `Workbench.ast` prints a member's AST in about 0.1 s. The `dotnet fsi` script it replaced took about 60 s.
- With the workbench, all 60 `it.fails` runtime specs, one per `// BUG:` marker, were closed in one afternoon
  (commit e08ad85). About 46 of them took 46 minutes in the main session.
- It did not remove every slow step. The first compile in a session still takes 6 to 19 s. A runtime suite still
  needs vitest (2.5 to 5 s). The workbench saved no time for agents that were not allowed to use it.

## Measures

| Measure | CLI loop | Workbench | Source / method |
| --- | --- | --- | --- |
| Snapshot compile, all cases, fresh | 14.1 s wall (Fable reports 7.8 to 8.4 s; 4.2 s of that is parsing) | n/a | Fresh: `dotnet fable --exclude Partas.Solid.FablePlugin --noCache -e .fs.jsx -c Release --optimize` in `Compiled/`, 2 runs |
| Snapshot test run | 17.6 s wall, Expecto (38 tests) | `checkAll`: 0.32 to 0.35 s warm (3 runs); 1.2 s on the first call after 6 h idle | Fresh: timed `dotnet run --project Partas.Solid.Tests.Plugin ...` vs `Stopwatch` around `checkAll` |
| Same, from transcripts | Expecto median 21 s (n=8); CLI snapshot compile median 12.5 s (n=40), session ee07882c | `checkAll` median 0.39 s (n=92, IQR 0.29 to 1.0 s), session 5a137065 | Bash durations from tool timestamps; `N/M cases pass (X ms)` lines |
| Plugin edit picked up | Rebuild inside the CLI compile above | `reloadPlugin` 0.84 to 0.99 s fresh; median 3.1 s (n=28, 1.2 to 5.4 s) in 5a137065 | Fresh: Stopwatch; transcripts: printed ms |
| Cold / after `reset ()` | n/a | `checkAll` 9.3 s fresh; median 15.4 s (n=20) in transcripts | Re-type-checks the cases project |
| Runtime suite (Dom, 243 specs) | `node run.mjs dom` 17.0 s (Fable 7.3 s) | `emit Dom` 0.81 s warm (9.9 s first) + `run.mjs dom --no-compile` 2.5 s = 3.3 s | Fresh, timed |
| Same, from transcripts | `run.mjs <suite>` with compile median 22.5 s (n=131) | `run.mjs --no-compile` median 5.3 s (n=29) | ee07882c vs 5a137065 |
| AST of one member | ~60 s per `dotnet fsi printer-test.fsx` run; 18 runs during diagnosis | `Workbench.ast` 0.11 s fresh (87-line tree); 0.44 s when introduced | 5a137065 log at 17:17 and 17:20; fresh Stopwatch |
| Plugin edit to verified result | median 41 s (n=8, IQR 28 to 164 s) | median 11 s (n=19, IQR 8 to 26 s) | Time from a `Plugin.fs` edit to the end of the next verifying call. ee07882c vs 5a137065. Small samples, rough |
| Output parity | reference | 58/58 snapshot files, 28/28 Dom files byte-identical | Fresh: byte compare of workbench output with CLI files; `sha1sum -c` after `emit` |

Two caveats about the transcript figures. MCP round trips add 1 to 3 s to each workbench call: a `checkAll` that
prints 0.4 s shows up as a 2 to 4 s tool call. The two baselines also did different work: ee07882c mostly wrote
runtime tests and has only 8 plugin-edit loops.

## Bug-fix throughput (e08ad85)

Before the commit the runtime specs held 60 `it.fails` tests, each with a `// BUG:` marker. After it, none were
left. The commit added seven snapshot cases and changed 67 files (+771/−298).

| Phase (2026-09-26, UTC) | Duration | Specs closed | Notes |
| --- | --- | --- | --- |
| Main session using the workbench directly (15:49 to 16:35) | 46 min | ~46 (aria, getters, string children, Provider, Errored, builders, Dynamic) | 14 plugin edits, 36 workbench calls (10.6 min in total, including session start), 19 `run.mjs` runs (8.9 min) |
| Workflow of subagents (16:36 to 18:12) | ~96 min | ~17 | Diagnosis took 40 min, because agents were barred from the shared session and ran the cold fsi AST script 18 times. `Workbench.ast` was added at 17:20 in response. Fixing ran one agent at a time |
| Leftover Dom specs (18:13 to 18:21) | 8 min | 3, plus one real plugin bug (attribute quote escaping) | 10 workbench calls |

Per-phase counts come from the transcripts and are approximate; the commit total (60) is exact.

For comparison, one pre-workbench fix, the `List.toArray` unroll fork in ee07882c, took 7 minutes, 3.1 of which
went to 8 CLI verify runs. Single small fixes were not slow before. The gain shows up across many iterations.

**Estimate:** the main-session phase ran about 55 verify steps. Each would have cost about 15 to 20 s more through
the CLI, because every runtime run recompiles and every snapshot check is a full Fable run. That is about 15
minutes, a third of the phase. The AST script alone cost the workflow about 18 minutes (18 runs × ~60 s), where
`Workbench.ast` would have taken seconds.

## Qualitative benefits

- **AST printer (`Workbench.ast`, `AstPrinter`).** It prints the post-`FableTransforms` tree the plugin actually
  receives, labelled with the fields the plugin matches on (`@none` ranges, member refs, `{Pojo}`). Diagnosis
  stopped depending on guesswork and `DebugMode` dumps. It reads binding and fixture edits from disk without a
  reload.
- **Warm checker survives plugin edits.** Almost all of the plugin is `internal`, so `reloadPlugin` only
  re-type-checks when the public surface (attributes, `ComponentFlag`) changes.
- **Byte-identical emit.** `emit` writes the same files as `dotnet fable -o .`, so `run.mjs --no-compile` tests
  exactly what CI would build.
- **Watch mode.** `watchFiles`, `watchCases` and `watchSuite` rerun on save. A plugin edit shows up in an open
  `.fs.jsx` about 3 s later.
- **Compile lock.** `reloadPlugin`, a project's first compile and `emit` take `run.mjs`'s `.compile-lock`, so the
  workbench and CLI runs do not overwrite each other's output.
- **Accept loop.** `Workbench.accept` writes `.expected` from the current output, and new cases are found without
  registration.

## Costs and limits

- **Build cost.** The workbench was built on 2026-09-26 from about 11:37 to 14:38 UTC (session e1080b56, which
  also did other work), producing c4ac389 (1,041 lines). The AST printer and `Workbench.ast` (ada88c5, 625 lines)
  took roughly another 30 min.
- **Setup.** `dotnet fsi tools/workbench.fsx` clones and builds Fable.SageFs into `.workbench/`. Its sample-session step
  failed on Fable 5.13 and the script carried on past it (now skipped with `--vendor-only`). After that you
  create a SageFs session.
- **First compile is not fast.** Cracking a project takes 6 to 19 s (9.3 s fresh after `reset`, 9.9 s for the first
  Dom `emit`). A `.fsproj` edit or a new file needs `reset ()`. Transcripts show several `32/33` results caused by
  stale workbench state, which a `reset` cleared.
- **No hot reload for the plugin.** SageFs detours methods redefined in an eval, but the plugin's `internal` code
  is not reachable that way. Plugin edits need `reloadPlugin` (about 1 to 5 s).
- **Shared session.** One session serialises plugin edits, so the workflow ran fixers one at a time. Agents barred
  from it fell back to minute-long tools.
- **vitest is unchanged.** Runtime specs still take 2.5 to 5 s per suite, now the largest part of a warm loop.
- **Not the gate.** `dotnet fsi tools/build.fsx -- test` is still the final check.

SageFs problems met along the way are logged in `.workbench/Fable.SageFs/docs/sagefs-issues.md`.

## Conclusion

The workbench turned the plugin loop from roughly 15 to 25 s per verify into roughly 1 to 5 s, with output identical
to the CLI. Its AST printer removed the slowest diagnostic step, about 60 s per look. All 60 tracked runtime bugs
closed in one afternoon. Most of the gain came from direct use (about 46 specs in 46 min); the phase kept away from
the workbench took twice as long for about a third as many. The remaining costs are the one-off setup, cold first
compiles, `reset` after project changes, and vitest time.
