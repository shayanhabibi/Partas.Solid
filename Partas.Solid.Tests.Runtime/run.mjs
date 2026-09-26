#!/usr/bin/env node
// Runtime test runner for Partas.Solid.
//
//   node run.mjs [primitives|dom|integration|all] [--no-compile] [--watch] [vitest args...]
//
// 1. (unless --no-compile) takes a cross-process lock and compiles the requested suite(s) with
//    Fable + the Partas.Solid plugin. The lock matters: every suite references the shared
//    Partas.Solid and Partas.Solid.FablePlugin projects, and concurrent dotnet builds of those
//    collide on obj/ and bin/. Compiles are serialised; vitest runs are not.
// 2. runs vitest over the suite folder(s). Extra args go straight to vitest, e.g.
//      node run.mjs dom -t "renders"           (filter by test name)
//      node run.mjs integration Counter        (filter by file path)
//
// --watch hands over to watch.mjs: Fable and vitest both stay running, and plugin edits rebuild.
//
// Env: PARTAS_RUNTIME_LOCK_STALE_MS  heartbeat age after which a lock is considered abandoned
//      (default 120000). PARTAS_RUNTIME_LOCK_TIMEOUT_MS  give up waiting (default 1800000).
import {spawn} from "node:child_process";
import {SUITES, here, acquireLock, ensureNodeModules, compile, vitestPath} from "./lib.mjs";

function parseArgs(argv) {
    let suite = "all";
    let compile = true;
    let watch = false;
    const rest = [];
    let i = 0;
    if (argv[0] && !argv[0].startsWith("-") && (argv[0] in SUITES || argv[0] === "all")) {
        suite = argv[0];
        i = 1;
    }
    for (; i < argv.length; i++) {
        if (argv[i] === "--no-compile") compile = false;
        else if (argv[i] === "--watch") watch = true;
        else rest.push(argv[i]);
    }
    return {suite, compile, watch, vitestArgs: rest};
}

async function main() {
    const args = parseArgs(process.argv.slice(2));
    if (args.watch) {
        const {watch} = await import("./watch.mjs");
        return watch(args);
    }
    const {suite, compile: doCompile, vitestArgs} = args;
    const suiteDirs = suite === "all" ? Object.values(SUITES) : [SUITES[suite]];

    const release = await acquireLock();
    let code;
    try {
        code = await ensureNodeModules();
        if (code === 0 && doCompile) code = await compile(suiteDirs);
    } finally {
        release();
    }
    if (code !== 0) process.exit(code);

    const dirArgs = suite === "all" ? [] : ["--dir", suiteDirs[0]];
    code = await new Promise((resolve, reject) => {
        const child = spawn(process.execPath, [vitestPath, "run", ...dirArgs, ...vitestArgs], {
            cwd: here,
            stdio: "inherit"
        });
        child.on("error", reject);
        child.on("exit", c => resolve(c ?? 1));
    });
    process.exit(code);
}

main().catch(e => {
    console.error(e);
    process.exit(1);
});
