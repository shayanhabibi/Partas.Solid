import {describe, it, expect, vi} from "vitest";
import {mount, click, text} from "../../helpers/index.js";
import {Counter, Toggle, ListOf, CounterHost, ToggleHost, StatefulHost, ListOfHost} from "./C-RenderProps.fs.jsx";

describe("Composition: render props and function children", () => {
    it("calls a named render prop with an accessor, once, and stays reactive", () => {
        const {container} = mount(CounterHost);
        const shown = container.querySelector(".shown");
        expect(text(shown)).toBe("value=5");
        click(container.querySelector(".inc"));
        click(container.querySelector(".inc"));
        expect(text(shown)).toBe("value=7");
        expect(container.querySelector(".shown")).toBe(shown);
    });

    it("accepts a render prop supplied from JS", () => {
        let calls = 0;
        const {container} = mount(Counter, {
            start: 1,
            render: c => {
                calls++;
                const el = document.createElement("i");
                el.className = "js-render";
                el.textContent = "js";
                return [el, () => String(c())];
            }
        });
        expect(calls).toBe(1);
        expect(text(container.querySelector(".counter"))).toBe("+js1");
        click(container.querySelector(".inc"));
        expect(text(container.querySelector(".counter"))).toBe("+js2");
        expect(calls).toBe(1);
    });

    it("calls function-as-children with the component's own state", () => {
        const {container} = mount(ToggleHost);
        const state = container.querySelector(".state");
        expect(text(state)).toBe("OFF");
        click(container.querySelector(".flip"));
        expect(text(state)).toBe("ON");
        click(container.querySelector(".flip"));
        expect(text(state)).toBe("OFF");
        expect(container.querySelector(".state")).toBe(state);
    });

    it("accepts function children from JS", () => {
        const {container} = mount(Toggle, {children: on => () => (on() ? "yes" : "no")});
        expect(text(container.querySelector(".toggle"))).toBe("flipno");
        click(container.querySelector(".flip"));
        expect(text(container.querySelector(".toggle"))).toBe("flipyes");
    });

    it("passes two arguments (value, setter) to a two-arg function child returning a fragment", () => {
        const {container} = mount(StatefulHost);
        expect(text(container.querySelector(".value"))).toBe("a");
        click(container.querySelector(".set-b"));
        expect(text(container.querySelector(".value"))).toBe("b");
        // the fragment's two roots sit directly in the wrapper
        expect([...container.querySelector(".stateful").children].map(e => e.tagName)).toEqual(["SPAN", "BUTTON"]);
    });

    // BUG: a curried F# function prop (string -> int -> HtmlElement) is passed uncurried `(item, i) =>` at the call site but invoked curried `props.renderItem(item)(index())` inside the component
    it.fails("renders list rows through a two-argument render prop authored in F#", () => {
        const {container} = mount(ListOfHost);
        expect([...container.querySelectorAll(".row .item")].map(text)).toEqual(["0-x", "1-y"]);
        click(container.querySelector(".add"));
        expect([...container.querySelectorAll(".row .item")].map(text)).toEqual(["0-x", "1-y", "2-z"]);
    });

    it("renders one row per item through a render prop supplied from JS", () => {
        // Accept either calling convention, so this test does not pin the curried/uncurried choice
        // that the two-argument render prop bug above has to settle.
        const renderItem = vi.fn((item, i) => (i === undefined ? j => `${j}:${item}` : `${i}:${item}`));
        const {container} = mount(ListOf, {items: ["p", "q"], renderItem});
        expect([...container.querySelectorAll(".row")].map(text)).toEqual(["0:p", "1:q"]);
        expect(renderItem.mock.calls.map(c => c[0])).toEqual(["p", "q"]);
    });
});
