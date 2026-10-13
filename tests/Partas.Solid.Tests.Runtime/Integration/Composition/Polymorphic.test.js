import {describe, it, expect} from "vitest";
import {createSignal} from "solid-js";
import {mount, click, act, text} from "../../helpers/index.js";
import {
    Box,
    Link,
    Slot,
    BoxAsButton,
    BoxDefault,
    BoxAsSpan,
    BoxAsLink,
    ReactiveBox,
    SlotAsArticle
} from "./A-Polymorphic.fs.jsx";

describe("Composition: polymorphic `as'` components", () => {
    it("renders the morph target (native button) with the host's props and children", () => {
        const {container} = mount(BoxAsButton);
        const btn = container.querySelector("#poly-btn");
        expect(btn).not.toBeNull();
        expect(btn.tagName).toBe("BUTTON");
        expect(btn.getAttribute("class")).toBe("boxed");
        expect(btn.getAttribute("type")).toBe("button");
        expect(text(btn)).toBe("Pressed 0");
    });

    it("wires events declared on the morph target and keeps the element across updates", () => {
        const {container} = mount(BoxAsButton);
        const btn = container.querySelector("#poly-btn");
        click(btn);
        click(btn);
        expect(text(btn)).toBe("Pressed 2");
        expect(container.querySelector("#poly-btn")).toBe(btn);
    });

    it("does not leak the polymorphic `as` prop or the spread marker onto the DOM", () => {
        const {container} = mount(BoxAsButton);
        const btn = container.querySelector("#poly-btn");
        expect(btn.hasAttribute("as")).toBe(false);
        expect(btn.hasAttribute("n$")).toBe(false);
        expect(container.innerHTML).not.toContain("PARTAS");
    });

    it("falls back to the default `as` (merge default) when none is given", () => {
        const {root} = mount(BoxDefault);
        expect(root.tagName).toBe("DIV");
        expect(root.id).toBe("poly-default");
        expect(root.className).toBe("plain");
        expect(text(root)).toBe("just a div");
    });

    it("morphs into an inline element", () => {
        const {root} = mount(BoxAsSpan);
        expect(root.outerHTML).toBe('<span class="as-span">span text</span>');
    });

    it("morphs into another SolidTypeComponent, merging both prop sets", () => {
        const {root} = mount(BoxAsLink);
        expect(root.tagName).toBe("A");
        expect(root.id).toBe("poly-link");
        expect(root.getAttribute("title")).toBe("go home");
        expect(root.getAttribute("href")).toBe("/home");
        expect(root.getAttribute("class")).toBe("link link-primary");
        expect(root.hasAttribute("tone")).toBe(false);
        expect(text(root)).toBe("Home");
    });

    it("keeps host props reactive through the morph", () => {
        const {container} = mount(ReactiveBox);
        const sec = container.querySelector("section");
        expect(sec.className).toBe("off");
        expect(text(sec)).toBe("idle");
        click(container.querySelector(".toggle"));
        expect(sec.className).toBe("on");
        expect(text(sec)).toBe("ACTIVE");
        expect(container.querySelector("section")).toBe(sec);
    });

    it("supports a custom polymorphic attribute declared with __PARTAS_POLYMORPHIC__", () => {
        const {root} = mount(SlotAsArticle);
        expect(root.tagName).toBe("ARTICLE");
        expect(root.id).toBe("slot");
        expect(root.className).toBe("art");
        expect(root.hasAttribute("render")).toBe(false);
        expect(text(root)).toBe("slotted");
    });

    it("accepts a string tag as `as` from JS callers", () => {
        const {root} = mount(Box, {as: "em", id: "e", children: "emph"});
        expect(root.outerHTML).toBe('<em id="e">emph</em>');
    });

    it("switches the rendered tag when `as` changes reactively", () => {
        const [tag, setTag] = createSignal("em");
        const {container} = mount(Box, {
            get as() {
                return tag();
            },
            children: "x"
        });
        expect(container.firstElementChild.tagName).toBe("EM");
        act(() => setTag("strong"));
        expect(container.firstElementChild.tagName).toBe("STRONG");
        expect(text(container)).toBe("x");
    });

    it("renders a component passed as `as` from JS with forwarded props", () => {
        const {root} = mount(Box, {as: Link, tone: "muted", href: "/a", children: "A"});
        expect(root.tagName).toBe("A");
        expect(root.getAttribute("class")).toBe("link link-muted");
        expect(root.getAttribute("href")).toBe("/a");
        expect(text(root)).toBe("A");
    });

    it("renders a Slot with a JS-provided render target", () => {
        const {root} = mount(Slot, {render: "aside", id: "s", children: "c"});
        expect(root.outerHTML).toBe('<aside id="s">c</aside>');
    });
});
