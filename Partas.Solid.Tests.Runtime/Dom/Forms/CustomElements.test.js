import {describe, it, expect, vi, beforeAll} from "vitest";
import {mount, click, text} from "../../helpers/index.js";
import {
    UsesCustomElement, CustomElementProps, GlobalsForComponents, TemplateAndSlot, CustomEventHost
} from "./D-CustomElements.fs.jsx";

/** A small web component: mirrors its `label`/`count` attributes into a shadow root, exposes an `items` property. */
class XCounter extends HTMLElement {
    static observedAttributes = ["label", "count"];

    constructor() {
        super();
        this.attachShadow({mode: "open"}).innerHTML = `<span part="out"></span><slot></slot>`;
        this.changes = [];
        this._items = undefined;
    }

    attributeChangedCallback(name, _old, value) {
        this.changes.push([name, value]);
        this.shadowRoot.querySelector("span").textContent =
            `${this.getAttribute("label") ?? ""}:${this.getAttribute("count") ?? ""}`;
    }

    get items() {
        return this._items;
    }

    set items(v) {
        this._items = v;
    }
}

beforeAll(() => {
    if (!customElements.get("x-counter")) customElements.define("x-counter", XCounter);
});

describe("Dom/Forms custom elements", () => {
    it("a user-declared custom element tag renders as that element and is upgraded", () => {
        const {container} = mount(UsesCustomElement);
        const el = container.querySelector("#xc");
        expect(el.localName).toBe("x-counter");
        expect(el).toBeInstanceOf(XCounter);
        expect(el.shadowRoot.querySelector("span").textContent).toBe("Clicks:1");
    });

    it("static and dynamic values reach the custom element as attributes (Solid 2 default)", () => {
        const {container} = mount(UsesCustomElement);
        const el = container.querySelector("#xc");
        expect(el.getAttribute("label")).toBe("Clicks");
        expect(el.getAttribute("count")).toBe("1");
        click(container.querySelector("#xc-inc"));
        expect(el.getAttribute("count")).toBe("2");
        expect(el.shadowRoot.querySelector("span").textContent).toBe("Clicks:2");
        expect(el.changes.filter(([n]) => n === "count").map(([, v]) => v)).toEqual(["1", "2"]);
    });

    it("light-DOM children are rendered inside the custom element (slotted)", () => {
        const {container} = mount(UsesCustomElement);
        const el = container.querySelector("#xc");
        expect(el.querySelector("span.light").textContent.trim()).toBe("child");
        const slot = el.shadowRoot.querySelector("slot");
        expect(slot.assignedNodes().some(n => n.nodeType === 1 && n.classList.contains("light"))).toBe(true);
    });

    it(".attr(\"prop:items\", ...) assigns a DOM property (non-string value), reactively", () => {
        const {container} = mount(CustomElementProps);
        const el = container.querySelector("#xp");
        expect(el.items).toEqual(["a", "b"]);
        expect(el.hasAttribute("items")).toBe(false);
        expect(el.hasAttribute("prop:items")).toBe(false);
        expect(el.getAttribute("data-kind")).toBe("k");
        click(container.querySelector("#xp-set"));
        expect(el.items).toEqual(["c"]);
    });

    it("delegated events fire from a custom element host", () => {
        const log = vi.fn();
        const {container} = mount(CustomEventHost, {log});
        click(container.querySelector("#xe"));
        expect(log).toHaveBeenCalledWith("click");
    });
});

describe("Dom/Forms component-facing globals, template and slot", () => {
    it("is / slot / part / exportparts render as attributes", () => {
        const {container} = mount(GlobalsForComponents);
        expect(container.querySelector("#is-btn").getAttribute("is")).toBe("fancy-button");
        const s = container.querySelector("#slotted");
        expect(s.slot).toBe("title");
        expect(s.getAttribute("part")).toBe("label");
        expect(container.querySelector("#exp").getAttribute("exportparts")).toBe("label: title-label");
    });

    it("template children go into template.content, not the live DOM", () => {
        const {container} = mount(TemplateAndSlot);
        const tpl = container.querySelector("#tpl");
        expect(tpl).toBeInstanceOf(HTMLTemplateElement);
        expect(tpl.children).toHaveLength(0);
        expect(container.querySelector("li.row")).toBeNull();
        const li = tpl.content.querySelector("li.row");
        expect(li).not.toBeNull();
        expect(li.textContent.trim()).toBe("template row");
    });

    it("slot renders as an HTMLSlotElement with its name and fallback content", () => {
        const {container} = mount(TemplateAndSlot);
        const sl = container.querySelector("#sl");
        expect(sl).toBeInstanceOf(HTMLSlotElement);
        expect(sl.name).toBe("footer");
        expect(text(sl)).toBe("fallback");
    });
});
