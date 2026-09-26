import {describe, it, expect, vi} from "vitest";
import {createSignal} from "solid-js";
import {mount, click, act, text} from "../../helpers/index.js";
import {
    Counted, WrapEach, OptionalFrame, CountedHost, StaticCounted, StaticWrapEach,
    StaticBuilderCounted, TwiceHost, FrameHost
} from "./B-Children.fs.jsx";

describe("Composition: children helper", () => {
    it("resolves static children and counts them with toArray", () => {
        const {container} = mount(StaticCounted);
        expect(text(container.querySelector(".count"))).toBe("3");
        const items = container.querySelector(".items");
        expect([...items.children].map(c => c.className)).toEqual(["a", "b", "c"]);
    });

    it("tracks a For-generated child list as it grows and shrinks", () => {
        const {container} = mount(CountedHost);
        const count = container.querySelector(".count");
        expect(text(count)).toBe("2");
        expect(container.querySelectorAll(".items b.item").length).toBe(2);
        click(container.querySelector(".more"));
        expect(text(count)).toBe("3");
        expect(text(container.querySelector(".items"))).toBe("012");
        click(container.querySelector(".less"));
        click(container.querySelector(".less"));
        expect(text(count)).toBe("1");
        expect(container.querySelectorAll(".items b.item").length).toBe(1);
    });

    it("wraps each resolved child in its own <li>", () => {
        const {root} = mount(StaticWrapEach);
        expect(root.outerHTML).toBe(
            '<ul class="wrap-each"><li class="wrapped"><span>one</span></li><li class="wrapped"><span>two</span></li></ul>'
        );
    });

    it("resolves with the experimental children { } builder like children()", () => {
        const {container} = mount(StaticBuilderCounted);
        expect(text(container.querySelector(".count"))).toBe("2");
        expect(container.querySelectorAll(".items i").length).toBe(2);
    });

    it("creates child components once even when the resolved children are read several times", () => {
        const onCreate = vi.fn();
        const {container} = mount(() => TwiceHost(onCreate), undefined, {thunk: true});
        expect(onCreate.mock.calls.map(c => c[0])).toEqual(["t1", "t2"]);
        expect(text(container.querySelector(".n"))).toBe("2");
        expect(text(container.querySelector(".n2"))).toBe("2");
        expect(container.querySelectorAll(".body .tracked").length).toBe(2);
    });

    it("shows a fallback when the resolved children are empty and the frame when they are not", () => {
        const {container} = mount(FrameHost);
        expect(container.querySelector(".empty")).not.toBeNull();
        expect(container.querySelector(".frame")).toBeNull();
        click(container.querySelector(".toggle"));
        expect(container.querySelector(".empty")).toBeNull();
        expect(text(container.querySelector(".frame .content"))).toBe("content");
        click(container.querySelector(".toggle"));
        expect(container.querySelector(".empty")).not.toBeNull();
        expect(container.querySelector(".content")).toBeNull();
    });

    it("counts an array of DOM nodes passed from JS", () => {
        const a = document.createElement("u");
        const b = document.createElement("u");
        const {container} = mount(Counted, {children: [a, b]});
        expect(text(container.querySelector(".count"))).toBe("2");
        expect(container.querySelector(".items").firstChild).toBe(a);
    });

    it("follows a reactive children getter", () => {
        const [list, setList] = createSignal(["x"]);
        const {container} = mount(WrapEach, {
            get children() {
                return list();
            }
        });
        expect(container.querySelectorAll("li.wrapped").length).toBe(1);
        act(() => setList(["x", "y", "z"]));
        expect(text(container.querySelector("ul"))).toBe("xyz");
        expect(container.querySelectorAll("li.wrapped").length).toBe(3);
    });

    it("renders the heading and treats a single text child as non-empty", () => {
        const {container} = mount(OptionalFrame, {heading: "H", children: "text"});
        expect(text(container.querySelector("h3"))).toBe("H");
        expect(text(container.querySelector(".frame"))).toBe("text");
    });
});
