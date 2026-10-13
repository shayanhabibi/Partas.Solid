import {describe, it, expect} from "vitest";
import {mount} from "../../helpers/index.js";
import {SpreadButton, DefaultedSpread, LocalSpread, EmptySpread} from "./F-Spread.fs.jsx";

// The plugin emits `{...PARTAS_OTHERS} n$={false}`; n$ is a compiler marker and must never become DOM.
const noMarker = container => {
    for (const el of [container, ...container.querySelectorAll("*")]) {
        for (const n of el.getAttributeNames()) expect(n.toLowerCase(), `${el.tagName}[${n}]`).not.toBe("n$");
    }
    expect(container.innerHTML).not.toContain("n$");
};

describe("Dom/Elements prop spreads", () => {
    it("spreads unconsumed props as attributes and keeps consumed ones off the element", () => {
        const {root, container} = mount(SpreadButton, {
            label: "Save",
            id: "sb",
            title: "tip",
            "data-k": "v",
            type: "button"
        });
        expect(root.tagName).toBe("BUTTON");
        expect(root.textContent).toBe("Save");
        expect(root.id).toBe("sb");
        expect(root.getAttribute("title")).toBe("tip");
        expect(root.getAttribute("data-k")).toBe("v");
        expect(root.type).toBe("button");
        expect(root.hasAttribute("label")).toBe(false);
        expect(root.className).toBe("sp");
        noMarker(container);
    });

    it("spread boolean false removes the attribute; true sets it empty", () => {
        const {root: off} = mount(SpreadButton, {label: "x", disabled: false});
        expect(off.hasAttribute("disabled")).toBe(false);
        const {root: on} = mount(SpreadButton, {label: "x", disabled: true});
        expect(on.getAttribute("disabled")).toBe("");
        expect(on.disabled).toBe(true);
    });

    it("spread onClick is attached as a handler, not an attribute", () => {
        const seen = [];
        const {root} = mount(SpreadButton, {label: "x", onClick: e => seen.push(e.type)});
        root.click();
        expect(seen).toEqual(["click"]);
        expect(root.hasAttribute("onclick")).toBe(false);
    });

    it("a defaulted prop falls back to its default and is omitted from the spread", () => {
        const {container} = mount(DefaultedSpread, {title: "r"});
        expect(container.querySelector("#cls").className).toBe("default-class");
        const rest = container.querySelector("#rest");
        expect(rest.getAttribute("title")).toBe("r");
        expect(rest.hasAttribute("class")).toBe(false);
        noMarker(container);
    });

    it("a supplied value overrides the default and still is not spread", () => {
        const {container} = mount(DefaultedSpread, {class: "given", "aria-label": "rest"});
        expect(container.querySelector("#cls").className).toBe("given");
        const rest = container.querySelector("#rest");
        expect(rest.hasAttribute("class")).toBe(false);
        expect(rest.getAttribute("aria-label")).toBe("rest");
    });

    it("spreads a locally built object literal", () => {
        const {root, container} = mount(LocalSpread);
        expect(root.id).toBe("local");
        expect(root.title).toBe("from object");
        expect(root.dataset.x).toBe("1");
        expect(root.className).toBe("ls");
        noMarker(container);
    });

    it("a component with no own props spreads everything it receives", () => {
        const {root, container} = mount(EmptySpread, {title: "all", "data-a": "b"});
        expect(root.id).toBe("empty-spread");
        expect(root.getAttribute("title")).toBe("all");
        expect(root.getAttribute("data-a")).toBe("b");
        noMarker(container);
    });

    it("an empty props object renders only the static attributes", () => {
        const {root} = mount(EmptySpread, {});
        expect(root.getAttributeNames()).toEqual(["id"]);
    });
});
