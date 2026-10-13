import { describe, it, expect } from "vitest";
import { mount, deferred, settle, waitFor, text } from "../../helpers/index.js";
import { AsyncGreeting } from "./AsyncLoading.fs.jsx";

describe("Integration smoke: async memo under <Loading>", () => {
  it("shows the fallback, then the resolved value (settle)", async () => {
    const d = deferred();
    const { container } = mount(AsyncGreeting, { load: () => d.promise });
    expect(container.querySelector(".fallback")).not.toBeNull();
    expect(container.querySelector(".data")).toBeNull();

    d.resolve("hi");
    await settle();

    expect(container.querySelector(".fallback")).toBeNull();
    expect(text(container.querySelector(".data"))).toBe("hi");
  });

  it("resolves with waitFor", async () => {
    const { container } = mount(AsyncGreeting, { load: () => Promise.resolve("there") });
    const el = await waitFor(() => container.querySelector(".data"));
    expect(text(el)).toBe("there");
  });
});
