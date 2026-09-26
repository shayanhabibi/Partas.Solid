import {describe, it, expect} from "vitest";
import {mount, click} from "../../helpers/index.js";
import {FieldSwitch, StaticDynamic, DynamicToTagValue, DynamicCall} from "./D-DynamicFn.fs.jsx";

describe("Dom/Portals dynamic() factory", () => {
    it("renders the tag its source returns, with the props given at the use site", () => {
        const {root} = mount(FieldSwitch);
        const field = root.querySelector("#field");
        expect(field.tagName).toBe("INPUT");
        expect(field.getAttribute("placeholder")).toBe("type");
    });

    it("swaps the element when the source signal changes, keeping use-site props", () => {
        const {root} = mount(FieldSwitch);
        click(root.querySelector("#toggle-multi"));
        const area = root.querySelector("#field");
        expect(area.tagName).toBe("TEXTAREA");
        expect(area.getAttribute("placeholder")).toBe("type");
        expect(root.querySelectorAll("#field").length).toBe(1);
        click(root.querySelector("#toggle-multi"));
        expect(root.querySelector("#field").tagName).toBe("INPUT");
    });

    it("static: true renders the constant tag", () => {
        const {root} = mount(StaticDynamic);
        expect(root.innerHTML).toBe('<nav id="static-nav"></nav>');
    });

    it("a source returning a component renders it with the use-site props", () => {
        const {root} = mount(DynamicToTagValue);
        expect(root.innerHTML).toBe('<b class="shout">yo!</b>');
    });

    // BUG: SolidWebBindings `dynamic` returns `unit -> 'T`, so `Tag()` emits `{Tag()}` (component called with no props, throws) instead of a TagValue rendered as `<Tag />`.
    it.fails("the dynamic() result used as its binding type suggests renders the element", () => {
        const {root} = mount(DynamicCall);
        expect(root.innerHTML).toBe("<section></section>");
    });
});
