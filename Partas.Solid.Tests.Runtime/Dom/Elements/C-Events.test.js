import { describe, it, expect, beforeEach } from "vitest";
import { mount, click, input, act } from "../../helpers/index.js";
import { EventLog, PassThrough } from "./C-Events.fs.jsx";

// EventLog's handlers call the curried F# prop `log name event`.
let calls;
const log = name => e => calls.push([name, e]);
const names = () => calls.map(c => c[0]);

beforeEach(() => { calls = []; });

describe("Dom/Elements event handlers", () => {
  it("onClick fires once per click with the MouseEvent targeting the button", () => {
    const { container } = mount(EventLog, { log });
    const btn = container.querySelector("#btn");
    click(btn);
    expect(names()).toEqual(["click"]);
    const [, e] = calls[0];
    expect(e).toBeInstanceOf(MouseEvent);
    expect(e.type).toBe("click");
    expect(e.target).toBe(btn);
    click(btn);
    click(btn);
    expect(calls).toHaveLength(3);
  });

  it("onInput receives the input event after the value changed", () => {
    let valueSeen;
    const logWithValue = name => e => { valueSeen = e.target.value; calls.push([name, e]); };
    const { container } = mount(EventLog, { log: logWithValue });
    const txt = container.querySelector("#txt");
    input(txt, "abc");
    expect(names()).toEqual(["input"]);
    expect(calls[0][1]).toBeInstanceOf(Event);
    expect(calls[0][1].type).toBe("input");
    expect(calls[0][1].target).toBe(txt);
    expect(valueSeen).toBe("abc");
  });

  it("onKeyDown receives a KeyboardEvent carrying key and modifiers", () => {
    const { container } = mount(EventLog, { log });
    const keys = container.querySelector("#keys");
    act(() => keys.dispatchEvent(new KeyboardEvent("keydown", { key: "Enter", code: "Enter", shiftKey: true, bubbles: true })));
    expect(names()).toEqual(["keydown"]);
    const e = calls[0][1];
    expect(e).toBeInstanceOf(KeyboardEvent);
    expect(e.key).toBe("Enter");
    expect(e.shiftKey).toBe(true);
    // keyup is not handled
    act(() => keys.dispatchEvent(new KeyboardEvent("keyup", { key: "Enter", bubbles: true })));
    expect(calls).toHaveLength(1);
  });

  it("non-delegated onFocus/onBlur fire in order on the element", () => {
    const { container } = mount(EventLog, { log });
    const foc = container.querySelector("#foc");
    act(() => foc.focus());
    expect(document.activeElement).toBe(foc);
    act(() => foc.blur());
    expect(names()).toEqual(["focus", "blur"]);
    expect(calls[0][1].type).toBe("focus");
    expect(calls[1][1].type).toBe("blur");
  });

  it("onChange on a select fires with the new value", () => {
    const { container } = mount(EventLog, { log });
    const sel = container.querySelector("#sel");
    act(() => {
      sel.value = "b";
      sel.dispatchEvent(new Event("change", { bubbles: true }));
    });
    expect(names()).toEqual(["change"]);
    expect(calls[0][1].target.value).toBe("b");
  });

  it("bubbles delegated clicks from inner to outer handler, inner first", () => {
    const { container } = mount(EventLog, { log });
    click(container.querySelector("#inner"));
    expect(names()).toEqual(["inner", "outer"]);
    // same event object reaches both handlers
    expect(calls[0][1]).toBe(calls[1][1]);
    expect(calls[1][1].target).toBe(container.querySelector("#inner"));
  });

  it("clicking the outer element alone only fires the outer handler", () => {
    const { container } = mount(EventLog, { log });
    click(container.querySelector("#outer"));
    expect(names()).toEqual(["outer"]);
  });

  it("stopPropagation inside a handler stops the delegated walk", () => {
    const { container } = mount(EventLog, { log });
    click(container.querySelector("#stop"));
    expect(names()).toEqual(["stop"]);
    // control: without stopPropagation the outer stopper handler does fire
    calls = [];
    click(container.querySelector("#stopper"));
    expect(names()).toEqual(["stopper-outer"]);
  });

  it("onSubmit fires on the form when its submit button is clicked; preventDefault holds", () => {
    const { container } = mount(EventLog, { log });
    click(container.querySelector("#submit"));
    expect(names()).toEqual(["submit"]);
    const e = calls[0][1];
    expect(e.type).toBe("submit");
    expect(e.defaultPrevented).toBe(true);
    expect(e.target).toBe(container.querySelector("#frm"));
  });

  it("onDblClick, onMouseDown and onMouseUp fire for their own event types only", () => {
    const { container } = mount(EventLog, { log });
    act(() => container.querySelector("#dbl").dispatchEvent(new MouseEvent("dblclick", { bubbles: true })));
    const md = container.querySelector("#md");
    act(() => md.dispatchEvent(new MouseEvent("mousedown", { bubbles: true, button: 0 })));
    act(() => md.dispatchEvent(new MouseEvent("mouseup", { bubbles: true, button: 0 })));
    expect(names()).toEqual(["dblclick", "mousedown", "mouseup"]);
    // a plain click on the dblclick button does not trigger the dblclick handler
    calls = [];
    click(container.querySelector("#dbl"));
    expect(names()).toEqual([]);
  });

  it("does not render handlers as attributes", () => {
    const { container } = mount(EventLog, { log });
    for (const el of container.querySelectorAll("*")) {
      for (const n of el.getAttributeNames()) expect(n.startsWith("on"), `${el.id}: ${n}`).toBe(false);
    }
    expect(container.querySelector(".events").hasAttribute("log")).toBe(false);
  });

  it("a handler passed through as a prop is attached and called with the event", () => {
    const seen = [];
    const { root } = mount(PassThrough, { handler: e => seen.push(e) });
    click(root);
    expect(seen).toHaveLength(1);
    expect(seen[0]).toBeInstanceOf(MouseEvent);
    expect(seen[0].target).toBe(root);
  });
});
