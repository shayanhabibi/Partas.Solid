import {describe, it, expect, vi} from "vitest";
import {mount, click, text} from "../../helpers/index.js";
import {TermDef, Pair, PersonRow, Glossary, PairHost, PeopleList, RepeatHost} from "./E-Lists.fs.jsx";

describe("Composition: components returning fragments", () => {
    it("renders both roots of a SolidTypeComponent fragment with no wrapper", () => {
        const {container} = mount(TermDef, {term: "t", definition: "d"});
        expect(container.innerHTML).toBe("<dt>t</dt><dd>d</dd>");
    });

    it("renders let-bound fragment components side by side", () => {
        const {root} = mount(PairHost);
        // Solid leaves empty comment markers after inserted fragments; compare elements only
        expect([...root.children].map(e => e.className + ":" + e.textContent)).toEqual([
            "pa:left", "pb:right", "pa:l2", "pb:r2"
        ]);
    });

    it("renders a fragment component called from JS", () => {
        const {container} = mount(() => Pair("p", "q"), undefined, {thunk: true});
        expect([...container.children].map(e => e.className)).toEqual(["pa", "pb"]);
    });

    it("renders fragment rows inside a <dl> and appends new pairs in order", () => {
        const {container} = mount(Glossary);
        const dl = container.querySelector("dl");
        expect([...dl.children].map(e => e.tagName + ":" + e.textContent)).toEqual([
            "DT:css", "DD:styles", "DT:js", "DD:scripts"
        ]);
        const firstDt = dl.firstElementChild;
        click(container.querySelector(".add"));
        expect([...dl.children].map(e => e.textContent)).toEqual(["css", "styles", "js", "scripts", "html", "markup"]);
        expect(dl.firstElementChild).toBe(firstDt);
    });

    it("renders fragment components from Repeat and removes the trailing ones", () => {
        const {container} = mount(RepeatHost);
        const host = container.querySelector(".repeat-host");
        expect(host.querySelectorAll(".pa").length).toBe(3);
        expect([...host.querySelectorAll(".pa")].map(text)).toEqual(["r0", "r1", "r2"]);
        click(container.querySelector(".less"));
        expect([...host.querySelectorAll(".pa")].map(text)).toEqual(["r0", "r1"]);
        expect(host.querySelectorAll(".pb").length).toBe(2);
    });
});

describe("Composition: components used inside lists", () => {
    const rows = container => [...container.querySelectorAll("li.person .pname")].map(text);

    it("creates one row component per item", () => {
        const onCreate = vi.fn();
        const {container} = mount(() => PeopleList(onCreate), undefined, {thunk: true});
        expect(rows(container)).toEqual(["Ada", "Grace"]);
        expect(onCreate.mock.calls.map(c => c[0])).toEqual([1, 2]);
        expect(container.querySelectorAll("li.person.selected").length).toBe(0);
    });

    it("selects a row through a callback prop and highlights only that row", () => {
        const {container} = mount(() => PeopleList(() => {
        }), undefined, {thunk: true});
        const [ada, grace] = container.querySelectorAll("li.person");
        click(grace);
        expect(text(container.querySelector("span.selected"))).toBe("2");
        expect(grace.className).toBe("person selected");
        expect(ada.className).toBe("person");
        click(ada);
        expect(ada.className).toBe("person selected");
        expect(grace.className).toBe("person");
    });

    it("reorders row components without recreating them", () => {
        const onCreate = vi.fn();
        const {container} = mount(() => PeopleList(onCreate), undefined, {thunk: true});
        const [ada, grace] = container.querySelectorAll("li.person");
        click(container.querySelector(".reverse"));
        expect(rows(container)).toEqual(["Grace", "Ada"]);
        const after = container.querySelectorAll("li.person");
        expect(after[0]).toBe(grace);
        expect(after[1]).toBe(ada);
        expect(onCreate).toHaveBeenCalledTimes(2);
    });

    it("creates only the new row component when an item is appended", () => {
        const onCreate = vi.fn();
        const {container} = mount(() => PeopleList(onCreate), undefined, {thunk: true});
        click(container.querySelector(".add"));
        expect(rows(container)).toEqual(["Ada", "Grace", "Linus"]);
        expect(onCreate.mock.calls.map(c => c[0])).toEqual([1, 2, 3]);
    });

    it("re-renders a row whose item was replaced by a new record", () => {
        const {container} = mount(() => PeopleList(() => {
        }), undefined, {thunk: true});
        click(container.querySelector(".rename"));
        expect(rows(container)).toEqual(["Ada L.", "Grace"]);
        expect(text(container.querySelector("li.person .prole"))).toBe("eng");
    });

    it("renders a row component directly from JS props", () => {
        const onSelect = vi.fn();
        const {root} = mount(PersonRow, {
            person: {id: 7, name: "N", role: "R"},
            selected: true,
            onSelect,
            onCreate: () => {
            }
        });
        expect(root.outerHTML).toBe('<li class="person selected"><span class="pname">N</span><span class="prole">R</span></li>');
        click(root);
        expect(onSelect).toHaveBeenCalledWith(7);
    });
});
