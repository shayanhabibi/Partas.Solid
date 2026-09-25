import {describe, it, expect} from "vitest";
import {mount, click} from "../../helpers/index.js";

// Loaded lazily: the module's emitted import of op_BangAt cannot be resolved, so a static import
// would fail this whole file at collection time.
const load = () => import("./E-DynamicSwitch.fs.jsx");

describe("Dom/Portals Dynamic switching between a tag and a component chosen in F#", () => {
    // BUG: `!@Fancy` inside an if/else emits `op_BangAt(<Fancy />)` plus an unresolvable Builder.fs.jsx import instead of the bare `Fancy` identifier.
    it.fails("switches between a tag name and a component, forwarding props and disposing the old instance", async () => {
        const {TagOrComponent} = await load();
        const disposed = [];
        const {root} = mount(TagOrComponent, {onDispose: n => disposed.push(n)});
        const em = root.querySelector("em");
        expect(em).not.toBeNull();
        expect(em.getAttribute("label")).toBe("hi");
        click(root.querySelector("#toggle-fancy"));
        expect(root.querySelector("em")).toBeNull();
        expect(root.querySelector("strong.fancy").textContent).toBe("*hi*");
        click(root.querySelector("#toggle-fancy"));
        expect(root.querySelector("strong.fancy")).toBeNull();
        expect(root.querySelector("em")).not.toBeNull();
        expect(disposed).toEqual(["fancy"]);
    });
});
