#!/usr/bin/env node
// Runtime test runner for Partas.Solid.
//
//   node run.mjs [primitives|dom|integration|all] [--no-compile] [vitest args...]
//
// 1. (unless --no-compile) takes a cross-process lock and compiles the requested suite(s) with
//    Fable + the Partas.Solid plugin. The lock matters: every suite references the shared
//    Partas.Solid and Partas.Solid.FablePlugin projects, and concurrent dotnet builds of those
//    collide on obj/ and bin/. Compiles are serialised; vitest runs are not.
// 2. runs vitest over the suite folder(s). Extra args go straight to vitest, e.g.
//      node run.mjs dom -t "renders"           (filter by test name)
//      node run.mjs integration Counter        (filter by file path)
//
// Env: PARTAS_RUNTIME_LOCK_STALE_MS  heartbeat age after which a lock is considered abandoned
//      (default 120000). PARTAS_RUNTIME_LOCK_TIMEOUT_MS  give up waiting (default 1800000).
import {spawn} from "node:child_process";
import fs from "node:fs";
import os from "node:os";
import path from "node:path";
import {fileURLToPath} from "node:url";

const here = path.dirname(fileURLToPath(import.meta.url));
const repoRoot = path.resolve(here, "..");
const lockDir = path.join(here, ".compile-lock");
const ownerFile = path.join(lockDir, "owner.json");

const SUITES = {
    primitives: "Primitives",
    dom: "Dom",
    integration: "Integration"
};

const STALE_MS = Number(process.env.PARTAS_RUNTIME_LOCK_STALE_MS ?? 120_000);
const WAIT_TIMEOUT_MS = Number(process.env.PARTAS_RUNTIME_LOCK_TIMEOUT_MS ?? 30 * 60_000);
const HEARTBEAT_MS = 5_000;

const FABLE_ARGS = [
    "fable",
    "--exclude",
    "Partas.Solid.FablePlugin",
    "--noCache",
    "-e",
    ".fs.jsx",
    "-c",
    "Release",
    // Same as the plugin snapshot tests (Partas.Solid.Tests.Plugin/Common.fs), so runtime tests
    // exercise the same JSX the snapshots pin.
    "--optimize",
    // Output next to the sources. Referenced projects (Partas.Solid) then land in <Suite>/Partas.Solid/
    // with a suite-local fable_modules, instead of being written next to the shared library sources
    // where every suite (and the snapshot tests) would overwrite each other's import paths.
    "-o",
    "."
];

function log(msg) {
    console.log(`[run.mjs ${process.pid}] ${msg}`);
}

function parseArgs(argv) {
    let suite = "all";
    let compile = true;
    const rest = [];
    let i = 0;
    if (argv[0] && !argv[0].startsWith("-") && (argv[0] in SUITES || argv[0] === "all")) {
        suite = argv[0];
        i = 1;
    }
    for (; i < argv.length; i++) {
        if (argv[i] === "--no-compile") compile = false;
        else rest.push(argv[i]);
    }
    return {suite, compile, vitestArgs: rest};
}

function run(cmd, args, cwd) {
    return new Promise((resolve, reject) => {
        const child = spawn(cmd, args, {
            cwd,
            stdio: "inherit",
            // dotnet/npm are .cmd shims on Windows for npm; dotnet is an .exe but shell keeps PATH lookup uniform.
            shell: process.platform === "win32"
        });
        child.on("error", reject);
        child.on("exit", code => resolve(code ?? 1));
    });
}

// ---------------------------------------------------------------------------------------------
// Cross-process lock: atomic mkdir of .compile-lock, owner.json with pid/host, heartbeat by
// rewriting owner.json. A lock is stale when its heartbeat is older than STALE_MS, or when the
// owner is on this host and its pid is gone.
// ---------------------------------------------------------------------------------------------
function pidAlive(pid) {
    try {
        process.kill(pid, 0);
        return true;
    } catch (e) {
        return e.code === "EPERM";
    }
}

function readOwner() {
    try {
        return JSON.parse(fs.readFileSync(ownerFile, "utf8"));
    } catch {
        return undefined;
    }
}

function lockAgeMs() {
    for (const p of [ownerFile, lockDir]) {
        try {
            return Date.now() - fs.statSync(p).mtimeMs;
        } catch {
        }
    }
    return 0;
}

function isStale() {
    const owner = readOwner();
    const age = lockAgeMs();
    if (owner && owner.host === os.hostname() && !pidAlive(owner.pid)) return `owner pid ${owner.pid} is gone`;
    // owner.json not yet written: only stale if the directory itself is old.
    if (age > STALE_MS) return `no heartbeat for ${Math.round(age / 1000)}s`;
    return undefined;
}

function writeOwner(token) {
    const tmp = `${ownerFile}.${process.pid}.tmp`;
    fs.writeFileSync(tmp, JSON.stringify({
        pid: process.pid,
        host: os.hostname(),
        token,
        since: new Date().toISOString()
    }));
    fs.renameSync(tmp, ownerFile);
}

const sleep = ms => new Promise(r => setTimeout(r, ms));

async function acquireLock() {
    const token = `${os.hostname()}:${process.pid}:${Date.now()}:${Math.random()}`;
    const started = Date.now();
    let announced = false;
    for (; ;) {
        try {
            fs.mkdirSync(lockDir);
            writeOwner(token);
            const heartbeat = setInterval(() => {
                try {
                    if (readOwner()?.token === token) writeOwner(token);
                } catch {
                }
            }, HEARTBEAT_MS);
            heartbeat.unref();
            let released = false;
            const release = () => {
                if (released) return;
                released = true;
                clearInterval(heartbeat);
                try {
                    if (readOwner()?.token === token) fs.rmSync(lockDir, {recursive: true, force: true});
                } catch {
                }
            };
            process.once("exit", release);
            for (const sig of ["SIGINT", "SIGTERM", "SIGHUP"]) process.once(sig, () => (release(), process.exit(130)));
            return release;
        } catch (e) {
            if (e.code !== "EEXIST") throw e;
        }
        const why = isStale();
        if (why) {
            log(`removing stale compile lock (${why})`);
            // Rename first so two waiters cannot both delete-and-recreate the same lock.
            const grave = `${lockDir}.stale.${process.pid}.${Date.now()}`;
            try {
                fs.renameSync(lockDir, grave);
                fs.rmSync(grave, {recursive: true, force: true});
            } catch {
            }
            continue;
        }
        if (!announced) {
            const o = readOwner();
            log(`waiting for compile lock held by ${o ? `pid ${o.pid} on ${o.host} since ${o.since}` : "another process"}...`);
            announced = true;
        }
        if (Date.now() - started > WAIT_TIMEOUT_MS) throw new Error(`timed out waiting for ${lockDir}`);
        await sleep(500 + Math.random() * 500);
    }
}

// ---------------------------------------------------------------------------------------------

async function ensureNodeModules() {
    if (fs.existsSync(path.join(here, "node_modules", "vitest", "package.json"))) return 0;
    log("node_modules missing: npm ci");
    return run("npm", ["ci", "--no-audit", "--no-fund"], here);
}

async function compile(suiteDirs) {
    let code = await run("dotnet", ["tool", "restore"], repoRoot);
    if (code !== 0) return code;
    for (const dir of suiteDirs) {
        const cwd = path.join(here, dir);
        log(`fable: compiling ${dir}`);
        const t = Date.now();
        code = await run("dotnet", FABLE_ARGS, cwd);
        if (code !== 0) {
            log(`fable: ${dir} FAILED (exit ${code})`);
            return code;
        }
        log(`fable: ${dir} done in ${((Date.now() - t) / 1000).toFixed(1)}s`);
    }
    return 0;
}

async function main() {
    const {suite, compile: doCompile, vitestArgs} = parseArgs(process.argv.slice(2));
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

    const vitest = path.join(here, "node_modules", "vitest", "vitest.mjs");
    const dirArgs = suite === "all" ? [] : ["--dir", suiteDirs[0]];
    code = await new Promise((resolve, reject) => {
        const child = spawn(process.execPath, [vitest, "run", ...dirArgs, ...vitestArgs], {
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
