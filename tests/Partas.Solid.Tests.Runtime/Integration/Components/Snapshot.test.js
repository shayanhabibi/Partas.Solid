import { describe, it, expect } from "vitest";
import { createSignal } from "solid-js";
import { mount, act, text } from "../../helpers/index.js";
import { SnapshotHost, LabelledHost } from "./E-Abstract.fs.jsx";

function setup(typed) {
  const log = [];
  const [value, setValue] = createSignal(1);
  const r = mount(SnapshotHost, { typed, value, log: m => log.push(m) });
  return { ...r, log, setValue };
}

describe("Components: component bodies are untracked", () => {
  it("SolidTypeComponent: body runs once; a body read is a snapshot, a JSX read is live", () => {
    const { container, log, setValue } = setup(true);
    const el = container.querySelector(".snapshot");
    expect(text(el)).toBe("1/1");
    act(() => setValue(2));
    expect(text(container.querySelector(".snapshot"))).toBe("1/2");
    expect(container.querySelector(".snapshot")).toBe(el);
    expect(log).toEqual(["body"]);
  });

  it("SolidComponent let-binding: body runs once; a body read is a snapshot, a JSX read is live", () => {
    const { container, log, setValue } = setup(false);
    const el = container.querySelector(".snapshot");
    expect(text(el)).toBe("1/1");
    act(() => setValue(2));
    expect(text(container.querySelector(".snapshot"))).toBe("1/2");
    expect(container.querySelector(".snapshot")).toBe(el);
    expect(log).toEqual(["body"]);
  });
});

describe("Components: SolidComponent arguments are tracked", () => {
  it("re-renders a let-bound component when a positional argument's signal changes", () => {
    const log = [];
    const [label, setLabel] = createSignal("first");
    const { container } = mount(LabelledHost, { label, log: m => log.push(m) });
    expect(text(container.querySelector(".labelled"))).toBe("first");
    act(() => setLabel("second"));
    expect(text(container.querySelector(".labelled"))).toBe("second");
    expect(log).toEqual(["body first", "body second"]);
  });
});
