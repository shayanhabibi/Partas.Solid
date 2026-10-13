import {describe, it, expect} from "vitest";
import {mount, click} from "../../helpers/index.js";

const load = () => import("./E-DynamicSwitch.fs.jsx");

describe("Dom/Portals Dynamic switching between a tag and a component chosen in F#", () => {
    it("switches between a tag name and a component, forwarding props and disposing the old instance", async () => {
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
