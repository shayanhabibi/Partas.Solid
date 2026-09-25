import {describe, it, expect} from "vitest";
import {mount} from "../../helpers/index.js";

// Loaded dynamically so that, should the emitted import ever become a hard ESM link error, only
// these cases fail rather than the whole file. Today the module loads: the imported extension is
// simply undefined, and the call throws "is not a function" at render time.
const load = () => import("./H-WebDynamicChildren.fs.jsx");

describe("Dom/Elements Dynamic with builder children", () => {
    it("the module loads, but the emitted Run extension it imports does not exist at runtime", async () => {
        const mod = await load();
        expect(typeof mod.DynamicPlain).toBe("function");
        expect(() => mount(mod.DynamicPlain)).toThrow(/not a function/);
    });

    // BUG: `Dynamic(...) { "text" }` emits a call to the erased extension
    // Partas_Solid_Web_BindingsModule_Extensions_Run_368C95E3 (imported from ../Partas.Solid/SolidWebBindings.fs.jsx,
    // which does not export it) instead of rewriting the builder body into JSX children.
    it.fails("Dynamic with a string tag renders it with text children and spread props", async () => {
        const {DynamicString} = await load();
        const {root} = mount(DynamicString);
        expect(root.tagName).toBe("H2");
        expect(root.id).toBe("dyn");
        expect(root.textContent).toBe("dynamic heading");
    });

    // BUG: same Run-extension emission as above.
    it.fails("the plainest Dynamic string tag with a text child renders", async () => {
        const {DynamicPlain} = await load();
        const {root} = mount(DynamicPlain);
        expect(root.outerHTML).toBe("<h3>plain</h3>");
    });

    // BUG: same Run-extension emission, and additionally componentAsString = props.tag is dropped
    // (emitted `<Dynamic {...dynProps} />` has no component at all).
    it.fails("Dynamic with a component from a prop renders it with children", async () => {
        const {DynamicProp} = await load();
        const {root} = mount(DynamicProp, {tag: "nav"});
        expect(root.tagName).toBe("NAV");
        expect(root.className).toBe("dp");
        expect(root.textContent).toBe("dyn");
    });
});
