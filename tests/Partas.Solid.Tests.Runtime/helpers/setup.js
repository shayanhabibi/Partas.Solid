// Vitest setupFile: registers auto-cleanup and asserts we are running Solid's browser DEV builds.
import "./index.js";
import { beforeAll } from "vitest";
import { DEV } from "solid-js";
import { isServer, isDev } from "@solidjs/web";

beforeAll(() => {
  if (isServer)
    throw new Error("@solidjs/web resolved to its SERVER build; vitest must resolve the 'browser' condition.");
  if (!isDev || !DEV)
    throw new Error("solid-js/@solidjs/web resolved to a production build; vitest must resolve 'development'.");
});
