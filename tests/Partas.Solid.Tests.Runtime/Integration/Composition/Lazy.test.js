import {describe, it, expect, vi} from "vitest";
import {mount, click, deferred, settle, waitFor, text} from "../../helpers/index.js";
import {LazyHost, NamedLazyHost, BuilderLazyHost, ToggleLazyHost, makeLazy} from "./G-Lazy.fs.jsx";
import * as target from "./F-LazyTarget.fs.jsx";

describe("Composition: lazy components under <Loading>", () => {
    it("loads a lazy' component with a named export (dynamic import)", async () => {
        const {container} = mount(NamedLazyHost);
        const panel = await waitFor(() => container.querySelector(".lazy-panel"));
        expect(text(panel.querySelector(".lazy-title"))).toBe("named");
        expect(text(panel.querySelector(".lazy-body"))).toBe("lazy body");
        expect(container.querySelector(".lazy-fallback")).toBeNull();
    });

    it("loads a lazyload { } builder component mapped to a default export", async () => {
        const {container} = mount(BuilderLazyHost);
        const panel = await waitFor(() => container.querySelector(".lazy-panel"));
        expect(text(panel.querySelector(".lazy-title"))).toBe("builder");
    });

    it("shows the Loading fallback until the module resolves", async () => {
        const d = deferred();
        const comp = makeLazy(() => d.promise);
        const {container} = mount(LazyHost, {comp, title: "t"});
        expect(text(container.querySelector(".lazy-fallback"))).toBe("loading...");
        expect(container.querySelector(".lazy-panel")).toBeNull();
        d.resolve(target);
        await settle();
        expect(container.querySelector(".lazy-fallback")).toBeNull();
        expect(text(container.querySelector(".lazy-title"))).toBe("t");
    });

    it("does not call the loader until the lazy component is first rendered", async () => {
        const loader = vi.fn(() => Promise.resolve(target));
        const comp = makeLazy(loader);
        const {container} = mount(() => ToggleLazyHost(comp), undefined, {thunk: true});
        expect(loader).not.toHaveBeenCalled();
        click(container.querySelector(".show"));
        expect(loader).toHaveBeenCalledTimes(1);
        await waitFor(() => container.querySelector(".lazy-panel"));
        expect(text(container.querySelector(".lazy-title"))).toBe("toggled");
    });

    it("reuses the loaded module on remount without loading again or showing the fallback", async () => {
        const loader = vi.fn(() => Promise.resolve(target));
        const comp = makeLazy(loader);
        const {container} = mount(() => ToggleLazyHost(comp), undefined, {thunk: true});
        click(container.querySelector(".show"));
        await waitFor(() => container.querySelector(".lazy-panel"));
        click(container.querySelector(".show"));
        expect(container.querySelector(".lazy-panel")).toBeNull();
        click(container.querySelector(".show"));
        expect(container.querySelector(".lazy-panel")).not.toBeNull();
        expect(container.querySelector(".lazy-fallback")).toBeNull();
        expect(loader).toHaveBeenCalledTimes(1);
    });

    it("exposes preload, which starts the import ahead of rendering", async () => {
        const loader = vi.fn(() => Promise.resolve(target));
        const comp = makeLazy(loader);
        expect(typeof comp.preload).toBe("function");
        await comp.preload();
        expect(loader).toHaveBeenCalledTimes(1);
        const {container} = mount(LazyHost, {comp, title: "pre"});
        await waitFor(() => container.querySelector(".lazy-panel"));
        expect(loader).toHaveBeenCalledTimes(1);
    });

    it("shares one import between two instances", async () => {
        const d = deferred();
        const loader = vi.fn(() => d.promise);
        const comp = makeLazy(loader);
        const a = mount(LazyHost, {comp, title: "a"});
        const b = mount(LazyHost, {comp, title: "b"});
        expect(loader).toHaveBeenCalledTimes(1);
        d.resolve(target);
        await settle();
        expect(text(a.container.querySelector(".lazy-title"))).toBe("a");
        expect(text(b.container.querySelector(".lazy-title"))).toBe("b");
    });
});
