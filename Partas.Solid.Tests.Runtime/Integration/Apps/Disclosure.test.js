import {describe, it, expect, vi} from "vitest";
import {mount, click, flush, act, text} from "../../helpers/index.js";
import {Tabs, Accordion} from "./C-Disclosure.fs.jsx";

const $ = (root, sel) => root.querySelector(sel);
const $$ = (root, sel) => [...root.querySelectorAll(sel)];

const items = [
    {id: "a", label: "Alpha", content: "First panel"},
    {id: "b", label: "Beta", content: "Second panel"},
    {id: "c", label: "Gamma", content: "Third panel"}
];

/** keydown is delegated by Solid, so it must bubble from a descendant of the tablist. */
const key = (el, k) => flush(() => el.dispatchEvent(new KeyboardEvent("keydown", {key: k, bubbles: true})));

const tab = (c, id) => $(c, `#tab-${id}`);
const activeTab = c => $$(c, 'button[role="tab"]').filter(b => b.classList.contains("active")).map(b => b.id);
const panels = c => $$(c, '[role="tabpanel"]').map(p => p.id);

function setupTabs(props = {}) {
    const onTabChange = vi.fn();
    const {container} = mount(Tabs, {items, onTabChange, ...props});
    return {c: container, onTabChange};
}

describe("Apps: tabs widget", () => {
    it("selects the first tab by default with a roving tabindex and a single mounted panel", () => {
        const {c} = setupTabs();
        expect($$(c, 'button[role="tab"]').map(text)).toEqual(["Alpha", "Beta", "Gamma"]);
        expect(activeTab(c)).toEqual(["tab-a"]);
        expect($$(c, 'button[role="tab"]').map(b => b.getAttribute("tabindex"))).toEqual(["0", "-1", "-1"]);
        expect(panels(c)).toEqual(["panel-a"]);
        expect(text($(c, "#panel-a .panel-content"))).toBe("First panel");
        expect($(c, ".tablist").getAttribute("role")).toBe("tablist");
    });

    it("honours defaultValue", () => {
        const {c} = setupTabs({defaultValue: "c"});
        expect(activeTab(c)).toEqual(["tab-c"]);
        expect(panels(c)).toEqual(["panel-c"]);
    });

    it("clicking a tab swaps the panel, moves tabindex, and reports the change once", () => {
        const {c, onTabChange} = setupTabs();
        click(tab(c, "b"));
        expect(activeTab(c)).toEqual(["tab-b"]);
        expect(panels(c)).toEqual(["panel-b"]);
        expect(tab(c, "b").getAttribute("tabindex")).toBe("0");
        expect(tab(c, "a").getAttribute("tabindex")).toBe("-1");
        expect(onTabChange.mock.calls).toEqual([["b"]]);

        // Re-selecting the current tab is not a change.
        click(tab(c, "b"));
        expect(onTabChange).toHaveBeenCalledTimes(1);
    });

    it("tab buttons are stable keyed nodes across selection changes", () => {
        const {c} = setupTabs();
        const before = $$(c, 'button[role="tab"]');
        click(tab(c, "c"));
        // toEqual would accept re-created but structurally equal nodes (isEqualNode); check identity.
        const after = $$(c, 'button[role="tab"]');
        expect(after).toHaveLength(3);
        after.forEach((b, i) => expect(b).toBe(before[i]));
        expect(before.map(b => b.className)).toEqual(["tab", "tab", "tab active"]);
    });

    it("ArrowRight / ArrowLeft move the selection and wrap at both ends", () => {
        const {c, onTabChange} = setupTabs();
        key(tab(c, "a"), "ArrowRight");
        expect(activeTab(c)).toEqual(["tab-b"]);
        key(tab(c, "b"), "ArrowRight");
        key(tab(c, "c"), "ArrowRight");
        expect(activeTab(c)).toEqual(["tab-a"]);
        key(tab(c, "a"), "ArrowLeft");
        expect(activeTab(c)).toEqual(["tab-c"]);
        expect(onTabChange.mock.calls.map(x => x[0])).toEqual(["b", "c", "a", "c"]);
    });

    it("Home / End jump to the ends; other keys are ignored", () => {
        const {c, onTabChange} = setupTabs({defaultValue: "b"});
        key(tab(c, "b"), "End");
        expect(activeTab(c)).toEqual(["tab-c"]);
        key(tab(c, "c"), "Home");
        expect(activeTab(c)).toEqual(["tab-a"]);
        key(tab(c, "a"), "Enter");
        expect(activeTab(c)).toEqual(["tab-a"]);
        // Home on the first tab is not a change.
        key(tab(c, "a"), "Home");
        expect(onTabChange.mock.calls.map(x => x[0])).toEqual(["c", "a"]);
    });

    it("panel-local component state is discarded when its panel unmounts", () => {
        const {c} = setupTabs();
        click($(c, "#panel-a .panel-counter"));
        click($(c, "#panel-a .panel-counter"));
        expect(text($(c, "#panel-a .panel-counter"))).toBe("clicked 2");
        click(tab(c, "b"));
        expect(text($(c, "#panel-b .panel-counter"))).toBe("clicked 0");
        click(tab(c, "a"));
        expect(text($(c, "#panel-a .panel-counter"))).toBe("clicked 0");
    });

    it("two selection changes in one flush settle on the last one", () => {
        const {c, onTabChange} = setupTabs();
        act(() => {
            tab(c, "b").click();
            tab(c, "c").click();
        });
        // Both handlers run before the flush; each sees a differing id, so both report a change,
        // and the DOM settles on the last write.
        expect(activeTab(c)).toEqual(["tab-c"]);
        expect(panels(c)).toEqual(["panel-c"]);
        expect(onTabChange.mock.calls.map(x => x[0])).toEqual(["b", "c"]);
    });

    // BUG: ariaSelected / ariaControls are emitted verbatim instead of aria-selected / aria-controls.
    it.fails("exposes aria-selected and aria-controls on each tab", () => {
        const {c} = setupTabs();
        expect(tab(c, "a").getAttribute("aria-selected")).toBe("true");
        expect(tab(c, "b").getAttribute("aria-selected")).toBe("false");
        expect(tab(c, "a").getAttribute("aria-controls")).toBe("panel-a");
    });
});

function setupAccordion(props = {}) {
    const {container} = mount(Accordion, {items, ...props});
    return container;
}

const states = c => $$(c, ".acc-item").map(i => i.dataset.state);

describe("Apps: accordion widget", () => {
    it("starts fully collapsed with the merged defaults", () => {
        const c = setupAccordion();
        expect(states(c)).toEqual(["closed", "closed", "closed"]);
        expect($$(c, ".acc-panel")).toEqual([]);
        expect(text($(c, ".open-count"))).toBe("0 open");
        expect($$(c, "h3.acc-header > button.acc-trigger").map(text)).toEqual(["Alpha", "Beta", "Gamma"]);
    });

    it("single mode: opening one item closes the other", () => {
        const c = setupAccordion();
        const [ta, tb] = $$(c, ".acc-trigger");
        click(ta);
        expect(states(c)).toEqual(["open", "closed", "closed"]);
        expect($(c, "#acc-a").getAttribute("role")).toBe("region");
        expect(text($(c, "#acc-a"))).toBe("First panel");
        click(tb);
        expect(states(c)).toEqual(["closed", "open", "closed"]);
        expect($$(c, ".acc-panel").map(p => p.id)).toEqual(["acc-b"]);
        expect(text($(c, ".open-count"))).toBe("1 open");
    });

    it("clicking an open item closes it", () => {
        const c = setupAccordion();
        const ta = $(c, ".acc-trigger");
        click(ta);
        click(ta);
        expect(states(c)).toEqual(["closed", "closed", "closed"]);
        expect(text($(c, ".open-count"))).toBe("0 open");
    });

    it("multiple mode keeps several items open and closes them individually", () => {
        const c = setupAccordion({multiple: true});
        const [ta, tb, tc] = $$(c, ".acc-trigger");
        click(ta);
        click(tc);
        expect(states(c)).toEqual(["open", "closed", "open"]);
        expect(text($(c, ".open-count"))).toBe("2 open");
        click(ta);
        expect(states(c)).toEqual(["closed", "closed", "open"]);
        click(tb);
        expect($$(c, ".acc-panel").map(p => p.id)).toEqual(["acc-b", "acc-c"]);
    });

    it("defaultOpen seeds the open set", () => {
        const c = setupAccordion({defaultOpen: ["b"]});
        expect(states(c)).toEqual(["closed", "open", "closed"]);
        expect(text($(c, ".open-count"))).toBe("1 open");
    });

    // BUG: ariaExpanded / ariaControls are emitted verbatim instead of aria-expanded / aria-controls.
    it.fails("reflects expansion in aria-expanded", () => {
        const c = setupAccordion();
        const ta = $(c, ".acc-trigger");
        expect(ta.getAttribute("aria-expanded")).toBe("false");
        expect(ta.getAttribute("aria-controls")).toBe("acc-a");
        click(ta);
        expect(ta.getAttribute("aria-expanded")).toBe("true");
    });
});
