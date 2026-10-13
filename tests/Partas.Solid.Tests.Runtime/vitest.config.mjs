import { defineConfig } from "vitest/config";
import solid from "@solidjs/vite-plugin";

// One config for all three suites; run.mjs scopes a run with `--dir <Suite>`.
export default defineConfig({
  plugins: [
    solid({
      // Fable emits JSX into *.fs.jsx next to each F# fixture. Hand-written specs may be .jsx too.
      // Patterns are relative to this folder (the vite root).
      include: ["**/*.jsx"],
      exclude: ["node_modules/**", "**/fable_modules/**"],
      // Use the solid-js / @solidjs/web development builds (better errors, dev diagnostics).
      dev: true
    })
  ],
  resolve: {
    // The plugin already adds these in test mode; stated explicitly so the intent survives
    // plugin changes. Never let the SSR ("node") build of @solidjs/web resolve here.
    conditions: ["browser", "development"]
  },
  test: {
    environment: "jsdom",
    setupFiles: ["./helpers/setup.js"],
    include: ["**/*.test.{js,jsx}"],
    exclude: ["**/node_modules/**", "**/fable_modules/**", "**/bin/**", "**/obj/**"],
    // Solid's DOM runtime must be inlined so the conditions above apply to it.
    server: { deps: { inline: [/solid-js/, /@solidjs\//] } },
    testTimeout: 10000
  }
});
