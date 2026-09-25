import { describe, it, expect } from "vitest";
import { createSignal } from "solid-js";
import { mount, act, text } from "../../helpers/index.js";
import { SnapshotHost } from "./E-Abstract.fs.jsx";

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

  // BUG: a [<SolidComponent>] let-binding is emitted as a plain call `{Snapshot(...)}` inside a JSX insert, so its body is tracked and the whole subtree is rebuilt on every signal read in the body.
  it.fails("SolidComponent let-binding: body runs once; a body read is a snapshot, a JSX read is live", () => {
    const { container, log, setValue } = setup(false);
    const el = container.querySelector(".snapshot");
    expect(text(el)).toBe("1/1");
    act(() => setValue(2));
    expect(text(container.querySelector(".snapshot"))).toBe("1/2");
    expect(container.querySelector(".snapshot")).toBe(el);
    expect(log).toEqual(["body"]);
  });
});
