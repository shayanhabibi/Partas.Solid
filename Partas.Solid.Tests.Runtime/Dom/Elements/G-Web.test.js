import {describe, it, expect} from "vitest";
import {mount} from "../../helpers/index.js";
import {Portaled, PortalBody, DynamicNoChildren, DynamicTagProp, DynamicTagField} from "./G-Web.fs.jsx";

describe("Dom/Elements @solidjs/web Portal and Dynamic", () => {
    it("Portal renders its children into the given mount node, not in place", () => {
        const target = document.createElement("div");
        target.id = "portal-target";
        document.body.appendChild(target);
        try {
            const {root} = mount(Portaled, {target});
            expect([...root.children].map(c => c.className)).toEqual(["before", "after"]);
            expect(root.querySelector(".modal")).toBeNull();
            const modal = target.querySelector(".modal");
            expect(modal).not.toBeNull();
            expect(modal.textContent).toBe("in portal");
            expect(root.hasAttribute("target")).toBe(false);
        } finally {
            target.remove();
        }
    });

    it("Portal content is removed from the mount node on dispose", () => {
        const target = document.createElement("div");
        document.body.appendChild(target);
        try {
            const {dispose} = mount(Portaled, {target});
            expect(target.querySelector(".modal")).not.toBeNull();
            dispose();
            expect(target.querySelector(".modal")).toBeNull();
        } finally {
            target.remove();
        }
    });

    it("Portal without mount renders into document.body", () => {
        const {root} = mount(PortalBody);
        expect(root.querySelector("#body-portal")).toBeNull();
        const p = document.getElementById("body-portal");
        expect(p).not.toBeNull();
        expect(p.textContent).toBe("to body");
        expect(root.contains(p)).toBe(false);
    });

    it("Dynamic with a string component renders that tag with the spread props", () => {
        const {root, container} = mount(DynamicNoChildren);
        expect(root.tagName).toBe("SECTION");
        expect(root.id).toBe("dyn-empty");
        expect(root.title).toBe("t");
        expect(root.hasAttribute("component")).toBe(false);
        expect(container.innerHTML).not.toContain("n$");
    });

    // BUG: `Dynamic(componentAsString = props.tag)` is emitted as a bare `<Dynamic />`; the inline
    // componentAsString setter reading a prop is disposed by the plugin (the raw component' field works, below).
    it.fails("Dynamic with a component taken from a prop renders that tag", () => {
        const {root} = mount(DynamicTagProp, {tag: "aside"});
        expect(root.querySelector("aside")).not.toBeNull();
    });

    it("Dynamic with component' taken from a prop renders that tag, per mount", () => {
        const {root} = mount(DynamicTagField, {tag: "aside"});
        expect(root.className).toBe("dyn-field");
        expect(root.innerHTML).toBe("<aside></aside>");
        // the consumed prop is not leaked onto the host
        expect(root.getAttributeNames()).toEqual(["class"]);
        const {root: other} = mount(DynamicTagField, {tag: "nav"});
        expect(other.firstElementChild.tagName).toBe("NAV");
    });
});
