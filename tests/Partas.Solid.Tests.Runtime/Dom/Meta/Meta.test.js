import { describe, it, expect, afterEach } from "vitest";
import { createSignal, flush } from "solid-js";
import { mount } from "../../helpers/index.js";
import { PageHead, SiteStylesheet, Layered } from "./A-Meta.fs.jsx";

// Solid 2 applies head tags in a microtask after the flush that registers them.
const settleHead = async () => {
  flush();
  await new Promise(r => setTimeout(r, 0));
};

afterEach(() => {
  document.head.innerHTML = "";
  document.title = "";
});

describe("Dom/Meta @solidjs/meta 1.0", () => {
  it("renders head tags into document.head without a provider", async () => {
    mount(PageHead);
    await settleHead();
    expect(document.title).toBe("Page");
    expect(document.head.querySelector('meta[name="description"]').getAttribute("content")).toBe("A page");
  });

  it("ignores <Base> on the client: Solid 2 treats it as shell-only", async () => {
    mount(PageHead);
    await settleHead();
    expect(document.head.querySelector("base")).toBeNull();
  });

  it("warms a Stylesheet with a style preload (Solid 2 holds the update until the sheet loads)", () => {
    // jsdom never loads CSS, so the gated rel="stylesheet" link itself does not land here.
    mount(SiteStylesheet);
    flush();
    expect(document.head.querySelector('link[rel="preload"][as="style"][href="/site.css"]')).not.toBeNull();
  });

  it("removes its tags when unmounted", async () => {
    const h = mount(PageHead);
    await settleHead();
    expect(document.head.querySelector('meta[name="description"]')).not.toBeNull();
    h.dispose();
    await settleHead();
    expect(document.head.querySelector('meta[name="description"]')).toBeNull();
  });

  it("a later keyed title replaces the earlier one, which comes back when it is disposed", async () => {
    const [inner, setInner] = createSignal(false);
    mount(Layered, { get inner() { return inner(); } });
    await settleHead();
    expect(document.title).toBe("Outer");
    setInner(true);
    await settleHead();
    expect(document.title).toBe("Inner");
    expect(document.head.querySelectorAll("title").length).toBe(1);
    setInner(false);
    await settleHead();
    expect(document.title).toBe("Outer");
  });
});
