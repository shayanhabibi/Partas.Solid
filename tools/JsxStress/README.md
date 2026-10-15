# Differential JSX stress corpus

Read the [findings](../../docs/research/solid-jsx-stress-test.md). This investigation
harness changes no framework sources or tool pins.

From the repository root, install prerequisites once:

```powershell
rtk proxy git clone --depth 1 https://github.com/Lanayx/Oxpecker.git .research/Oxpecker
rtk proxy git -C .research/Oxpecker checkout 369cda210b630ba7f85ea56bf6e84d20510ab943
rtk proxy dotnet tool install fable --tool-path .research/tools --version 5.21.0
rtk proxy npm ci --prefix tests/Partas.Solid.Tests.Runtime
```

The recorded run used the newest stable Fable resolved by an unversioned tool
installation, 5.21.0. The command above pins that version to reproduce the run.
Skip clone/install commands when those prerequisites already exist.

```powershell
rtk proxy node tools/JsxStress/run.mjs
rtk proxy node tests/Partas.Solid.Tests.Runtime/node_modules/vitest/vitest.mjs run --config tools/JsxStress/vitest.config.mjs
rtk proxy node tools/JsxStress/snapshot.mjs
```

`run.mjs` compiles identical templates with each library, records complete logs,
and executes the independent Babel/tree oracle. Its exit code indicates whether
the survey ran, not whether every library probe passed. Inspect `results.json` and
the printed failures. Oxpecker's aggregate compilation exits 1 because it explicitly
rejects five isolated files; other emitted files remain usable for the survey.
Case output is cleared first to prevent stale successes. Generated work is under
`.research/jsx-stress`; the snapshot command writes durable evidence to `docs/research`.

The DOM checks use the real Solid 2 rc.14 compiler/runtime for shared intrinsic JSX.
They do not claim Oxpecker binds Solid 2 APIs. Known defects retain correct assertions
with `it.fails`; five expected failures and five control checks are the recorded result.

`check.mjs` reruns the semantic oracle without compilation. Its JSX lowering uses
inert trees and actual Fable collection helpers, independently of Solid versions.
Function children are rejected strictly: an empty leaked continuation may be harmless
under a particular runtime. Reactive scheduling, hydration, custom component semantics
and the complete shared language surface are outside this corpus.
