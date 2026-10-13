import {describe, it, expect} from "vitest";
import {mount} from "../../helpers/index.js";

const load = () => import("./H-WebDynamicChildren.fs.jsx");

describe("Dom/Elements Dynamic with builder children", () => {
    it("Dynamic with a string tag renders it with text children and spread props", async () => {
        const {DynamicString} = await load();
        const {root} = mount(DynamicString);
        expect(root.tagName).toBe("H2");
        expect(root.id).toBe("dyn");
        expect(root.textContent).toBe("dynamic heading");
    });

    it("the plainest Dynamic string tag with a text child renders", async () => {
        const {DynamicPlain} = await load();
        const {root} = mount(DynamicPlain);
        expect(root.outerHTML).toBe("<h3>plain</h3>");
    });

    it("Dynamic with a component from a prop renders it with children", async () => {
        const {DynamicProp} = await load();
        const {root} = mount(DynamicProp, {tag: "nav"});
        expect(root.tagName).toBe("NAV");
        expect(root.className).toBe("dp");
        expect(root.textContent).toBe("dyn");
    });
});
