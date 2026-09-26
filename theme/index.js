import { resolve } from "node:path";
import { pathToFileURL } from "node:url";
import defaultTheme from "@kobalte/solidbase/default-theme";
import { defineTheme } from "@kobalte/solidbase/config";

// The default theme, with a version switcher in the header. Only Layout.jsx lives here;
// every other component still comes from the default theme. The path is taken from the
// working directory because the config bundler rewrites import.meta.url.
export default defineTheme({
    componentsPath: pathToFileURL(resolve("theme/Layout.jsx")).href,
    extends: defaultTheme,
});
