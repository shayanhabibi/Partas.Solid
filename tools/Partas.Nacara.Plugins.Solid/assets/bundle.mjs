// Written into the workspace by Partas.Nacara.Plugins.Solid, and run there: node bundle.mjs manifest.json
//
// Fable has already turned each page's module into JSX. This hands the JSX to the Solid compiler
// and bundles one entry per page, with what the pages share split into chunks of its own.
import fs from "node:fs";
import path from "node:path";
import { rolldown } from "rolldown";
import { transform } from "@solidjs/compiler";

const manifest = JSON.parse(fs.readFileSync(process.argv[2], "utf8"));

// The pages come out of Fable as .fs.jsx. Packages come out as .fs.js under fable_modules, and a
// package built on Partas.Solid (Partas.Solid itself, for one) has JSX in them all the same.
const solid = {
    name: "partas-solid-jsx",
    transform: {
        filter: { id: { include: [/\.jsx$/, /[\\/]fable_modules[\\/].*\.js$/], exclude: [/[\\/]fable-library-js[^\\/]*[\\/]/] } },
        handler(code, id) {
            // The compiler reads a .js filename as plain JavaScript and rejects the JSX in it.
            const filename = id.endsWith(".js") ? id + "x" : id;
            const out = transform(code, { filename, generate: "dom", hydratable: false, sourceMap: true });
            return { code: out.code, map: out.map ?? null };
        },
    },
};

const bundle = await rolldown({
    input: manifest.entries,
    plugins: [solid],
    platform: "browser",
    resolve: {
        conditionNames: ["browser", "import", "default"],
        // Packages resolve from the workspace's node_modules, for a referenced project's sources outside it too.
        modules: [path.resolve("node_modules"), "node_modules"],
    },
    logLevel: "warn",
});

await bundle.write({
    dir: manifest.outDir,
    format: "esm",
    entryFileNames: "[name].js",
    chunkFileNames: "shared-[hash].js",
    minify: manifest.minify,
    sourcemap: false,
});

await bundle.close();
