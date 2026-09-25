import { describe, it, expect } from "vitest";
import { createSignal } from "solid-js";
import { mount, click, input, act, text } from "../../helpers/index.js";
import { StatusChip, MirrorField, ToggleCounter } from "./D-Reactive.fs.jsx";

function chipProps() {
  const [status, setStatus] = createSignal("ok");
  const [active, setActive] = createSignal(true);
  const [color, setColor] = createSignal("green");
  const [size, setSize] = createSignal(12);
  return {
    props: {
      get status() { return status(); },
      get active() { return active(); },
      get color() { return color(); },
      get size() { return size(); }
    },
    setStatus,
    setActive,
    setColor,
    setSize
  };
}

describe("Components: reactive attributes", () => {
  it("renders the initial attribute, class and style values", () => {
    const { props } = chipProps();
    const { root } = mount(StatusChip, props);
    expect(root.getAttribute("class")).toBe("chip chip-ok");
    expect(root.getAttribute("title")).toBe("status: ok");
    const dot = root.querySelector(".dot");
    expect(dot.style.color).toBe("green");
    expect(dot.style.fontSize).toBe("12px");
    expect(root.querySelector(".text").style.color).toBe("green");
    expect(root.querySelector(".act").disabled).toBe(false);
    expect(root.querySelector(".field").value).toBe("ok");
  });

  it("updates class and title attributes on signal change without replacing the element", () => {
    const { props, setStatus } = chipProps();
    const { root, container } = mount(StatusChip, props);
    setStatus("down");
    expect(root.getAttribute("class")).toBe("chip chip-ok"); // not flushed yet
    act(() => {});
    expect(root.getAttribute("class")).toBe("chip chip-down");
    expect(root.getAttribute("title")).toBe("status: down");
    expect(text(root.querySelector(".text"))).toBe("down");
    expect(root.querySelector(".field").value).toBe("down");
    expect(container.firstElementChild).toBe(root);
  });

  it("updates object and string styles", () => {
    const { props, setColor, setSize } = chipProps();
    const { root } = mount(StatusChip, props);
    const dot = root.querySelector(".dot");
    act(() => {
      setColor("red");
      setSize(20);
    });
    expect(dot.style.color).toBe("red");
    expect(dot.style.fontSize).toBe("20px");
    expect(root.querySelector(".text").style.color).toBe("red");
  });

  it("toggles class-object entries and boolean attributes", () => {
    const { props, setActive } = chipProps();
    const { root } = mount(StatusChip, props);
    const flag = root.querySelectorAll("span")[2];
    expect(flag.getAttribute("class")).toBe("on");
    act(() => setActive(false));
    expect(flag.getAttribute("class")).toBe("off");
    expect(root.querySelector(".act").disabled).toBe(true);
    expect(root.querySelector(".act").hasAttribute("disabled")).toBe(true);
    act(() => setActive(true));
    expect(flag.getAttribute("class")).toBe("on");
    expect(root.querySelector(".act").hasAttribute("disabled")).toBe(false);
  });
});

describe("Components: event -> signal -> DOM loop", () => {
  it("mirrors typed input into text, derived length and a derived class", () => {
    const { container } = mount(MirrorField);
    const src = container.querySelector(".src");
    const preview = container.querySelector(".preview");
    expect(preview.getAttribute("class")).toBe("preview short");
    expect(text(container.querySelector(".len"))).toBe("0");

    input(src, "abc");
    expect(text(preview)).toBe("abc");
    expect(text(container.querySelector(".len"))).toBe("3");
    expect(preview.getAttribute("class")).toBe("preview short");

    input(src, "abcd");
    expect(preview.getAttribute("class")).toBe("preview long");
    expect(container.querySelector(".preview")).toBe(preview);
  });

  it("gates a counter behind a toggle; custom attr/data attributes follow the signals", () => {
    const { container } = mount(ToggleCounter);
    const toggle = container.querySelector(".toggle");
    const inc = container.querySelector(".inc");
    const out = container.querySelector(".count");
    expect(toggle.getAttribute("aria-pressed")).toBe("false");
    expect(text(toggle)).toBe("OFF");
    expect(inc.disabled).toBe(true);
    expect(out.getAttribute("data-count")).toBe("0");

    click(inc); // disabled buttons don't fire click
    expect(text(out)).toBe("0");

    click(toggle);
    expect(toggle.getAttribute("aria-pressed")).toBe("true");
    expect(text(toggle)).toBe("ON");
    expect(inc.disabled).toBe(false);

    click(inc);
    click(inc);
    click(inc);
    expect(text(out)).toBe("3");
    expect(out.getAttribute("data-count")).toBe("3");

    click(toggle);
    click(inc);
    expect(text(out)).toBe("3");
  });
});
