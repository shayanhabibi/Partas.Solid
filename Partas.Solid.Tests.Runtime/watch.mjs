// Watch mode for the runtime tests, reached through `node run.mjs [suite] --watch`.
//
//   F# fixture / Partas.Solid binding edit  -> `dotnet fable watch` recompiles the suite's .fs.jsx
//   .fs.jsx / .test.js edit                 -> `vitest watch` reruns the affected specs
//   Partas.Solid.FablePlugin edit           -> the plugin is rebuilt and the Fable watchers restart
//                                              (Fable loads the plugin dll once, at startup)
//
// With --no-compile only vitest watches: use it when you are editing specs against existing output.
//
// The compile lock (lib.mjs) is held while the watchers start and while the plugin rebuilds, which
// is when dotnet builds run. It is released in between, so do not also `node run.mjs` a suite that
// is being watched: both would write the same .fs.jsx.
import {spawn, spawnSync} from "node:child_process";
import fs from "node:fs";
import path from "node:path";
import {SUITES, FABLE_ARGS, here, repoRoot, log, run, acquireLock, ensureNodeModules, vitestPath} from "./lib.mjs";

const pluginDir = path.join(repoRoot, "Partas.Solid.FablePlugin");
const pluginProject = path.join(pluginDir, "Partas.Solid.FablePlugin.fsproj");
const DEBOUNCE_MS = 800;

const children = new Set();

function killTree(child) {
    if (child.exitCode !== null || child.pid === undefined) return;
    if (process.platform === "win32") spawnSync("taskkill", ["/pid", String(child.pid), "/T", "/F"], {stdio: "ignore"});
    else {
        try {
            process.kill(-child.pid, "SIGTERM");
        } catch {
        }
    }
}

function killAll() {
    for (const c of children) killTree(c);
    children.clear();
}

function track(child) {
    children.add(child);
    child.on("exit", () => children.delete(child));
    return child;
}

// Forwards a Fable watcher's output, keeping only what matters while iterating: compile starts,
// finishes and errors. Resolves `ready` on the first "Watching" line, i.e. after the initial compile.
function startFable(dir) {
    const child = track(spawn("dotnet", ["fable", "watch", ...FABLE_ARGS.slice(1)], {
        cwd: path.join(here, dir),
        detached: process.platform !== "win32"
    }));
    const tag = `[fable ${dir}]`;
    let resolveReady, rejectReady;
    const ready = new Promise((res, rej) => (resolveReady = res, rejectReady = rej));
    let buffered = "";
    let errors = 0;
    const onData = data => {
        buffered += data.toString();
        const lines = buffered.split(/\r?\n/);
        buffered = lines.pop();
        for (const line of lines) {
            if (/Started Fable compilation/.test(line)) {
                errors = 0;
                console.log(`${tag} compiling...`);
            } else if (/Fable compilation finished/.test(line)) console.log(`${tag} ${line.trim()}`);
            else if (/\berror\b/i.test(line) && !/\b0 Error\(s\)/.test(line)) {
                errors++;
                console.log(`${tag} ${line.trim()}`);
            } else if (/^Watching /.test(line.trim())) {
                if (errors) console.log(`${tag} ${errors} error line(s); vitest keeps the previous output`);
                resolveReady();
            }
        }
    };
    child.stdout.on("data", onData);
    child.stderr.on("data", onData);
    child.on("exit", code => rejectReady(new Error(`${tag} exited with code ${code}`)));
    return {child, ready};
}

async function startWatchers(suiteDirs, {restore = true} = {}) {
    const release = await acquireLock();
    try {
        if (restore) {
            const code = await run("dotnet", ["tool", "restore"], repoRoot);
            if (code !== 0) throw new Error("dotnet tool restore failed");
        }
        const fables = [];
        // One at a time: each watcher's startup runs a dotnet build of the shared projects.
        for (const dir of suiteDirs) {
            log(`fable watch: starting ${dir}`);
            const f = startFable(dir);
            await f.ready;
            fables.push(f.child);
        }
        return fables;
    } finally {
        release();
    }
}

function watchPlugin(onChange) {
    let timer;
    const watcher = fs.watch(pluginDir, {recursive: true}, (_event, file) => {
        if (!file || !/\.(fs|fsproj)$/.test(file) || /(^|[\\/])(bin|obj)[\\/]/.test(file)) return;
        clearTimeout(timer);
        timer = setTimeout(() => onChange(file), DEBOUNCE_MS);
    });
    return watcher;
}

export async function watch({suite, compile, vitestArgs}) {
    const suiteDirs = suite === "all" ? Object.values(SUITES) : [SUITES[suite]];
    process.on("exit", killAll);
    for (const sig of ["SIGINT", "SIGTERM", "SIGHUP"]) process.on(sig, () => process.exit(130));

    {
        const release = await acquireLock();
        let code;
        try {
            code = await ensureNodeModules();
        } finally {
            release();
        }
        if (code !== 0) process.exit(code);
    }

    let fables = compile ? await startWatchers(suiteDirs) : [];

    if (compile) {
        let restarting = false;
        let pending = false;
        const restart = async file => {
            if (restarting) return void (pending = true);
            restarting = true;
            try {
                do {
                    pending = false;
                    log(`plugin changed (${file}): rebuilding plugin and restarting Fable watchers`);
                    for (const f of fables) killTree(f);
                    fables = [];
                    const release = await acquireLock();
                    let code;
                    try {
                        code = await run("dotnet", ["build", pluginProject, "-c", "Release", "--nologo", "-v", "q"], repoRoot);
                    } finally {
                        release();
                    }
                    if (code !== 0) {
                        log("plugin build FAILED; fix it and save again (Fable watchers stay stopped)");
                        continue;
                    }
                    fables = await startWatchers(suiteDirs, {restore: false});
                    log("Fable watchers restarted");
                } while (pending);
            } catch (e) {
                log(String(e));
            } finally {
                restarting = false;
            }
        };
        watchPlugin(restart);
        log(`watching ${path.relative(repoRoot, pluginDir)} for plugin changes`);
    }

    const dirArgs = suite === "all" ? [] : ["--dir", suiteDirs[0]];
    const vitest = track(spawn(process.execPath, [vitestPath, "watch", ...dirArgs, ...vitestArgs], {
        cwd: here,
        stdio: "inherit"
    }));
    vitest.on("exit", code => process.exit(code ?? 0));
}
