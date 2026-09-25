import {describe, it, expect} from "vitest";
import {mount, click, settle, text} from "../../helpers/index.js";
import {
    flags, renderHello, ClientOnlyBranch, TitleSetter, ReactiveTitle, MetaSetter, HeadGroup
} from "./C-WebApi.fs.jsx";

describe("Dom/Portals @solidjs/web flags and render", () => {
    it("isServer is false and isDev is true in the browser development build", () => {
        expect(flags()).toEqual({isServer: false, isDev: true});
    });

    it("an isServer branch picks the client side", () => {
        const {root} = mount(ClientOnlyBranch);
        expect(root.textContent).toBe("client");
    });

    it("render from the bindings mounts a component into a node and disposes it", () => {
        const el = document.createElement("div");
        document.body.appendChild(el);
        const dispose = renderHello(el);
        expect(el.innerHTML).toBe('<p class="hello">hello from render</p>');
        dispose();
        expect(el.innerHTML).toBe("");
    });
});

describe("Dom/Portals useHead", () => {
    it("a constant title tag sets document.title", async () => {
        mount(TitleSetter);
        await settle();
        expect(document.title).toBe("Portals page");
    });

    it("a reactive tag accessor updates document.title", async () => {
        const {root} = mount(ReactiveTitle);
        await settle();
        expect(document.title).toBe("Count 0");
        click(root);
        click(root);
        await settle();
        expect(root.textContent).toBe("2");
        expect(document.title).toBe("Count 2");
    });

    it("a meta tag is added to head and removed on dispose", async () => {
        const {dispose} = mount(MetaSetter);
        await settle();
        const meta = document.head.querySelector('meta[name="description"]');
        expect(meta).not.toBeNull();
        expect(meta.getAttribute("content")).toBe("portal tests");
        dispose();
        await settle();
        expect(document.head.querySelector('meta[name="description"]')).toBeNull();
    });

    it("an array of tags registers every tag in the group", async () => {
        const {root} = mount(HeadGroup);
        await settle();
        expect(text(root)).toBe("group");
        expect(document.title).toBe("Grouped");
        expect(document.head.querySelector('meta[name="keywords"]').getAttribute("content")).toBe("a,b");
    });
});
