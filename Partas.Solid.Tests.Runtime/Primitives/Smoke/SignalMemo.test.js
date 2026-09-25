import { describe, it, expect } from "vitest";
import { flush } from "../../helpers/index.js";
import { makeCounter } from "./SignalMemo.fs.jsx";

describe("Primitives smoke: createSignal + createMemo", () => {
  it("memo recomputes after setter + flush", () => {
    const c = makeCounter(2);
    try {
      expect(c.count()).toBe(2);
      expect(c.doubled()).toBe(4);
      const runsBefore = c.memoRuns();

      c.setCount(5);
      flush();

      expect(c.count()).toBe(5);
      expect(c.doubled()).toBe(10);
      expect(c.memoRuns()).toBe(runsBefore + 1);
    } finally {
      c.dispose();
    }
  });

  it("memo does not recompute when nothing it reads changed", () => {
    const c = makeCounter(1);
    try {
      c.doubled();
      const runs = c.memoRuns();
      flush();
      expect(c.doubled()).toBe(2);
      expect(c.memoRuns()).toBe(runs);
    } finally {
      c.dispose();
    }
  });
});
