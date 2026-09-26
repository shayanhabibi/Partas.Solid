import {describe, it, expect, vi} from "vitest";
import {createSignal} from "solid-js";
import {mount, act, text} from "../../helpers/index.js";
import {click, input, key} from "./guard.js";
import {VerticalTabs} from "./B-Tabs.fs.jsx";

const $ = (root, sel) => root.querySelector(sel);
const $$ = (root, sel) => [...root.querySelectorAll(sel)];

const tabs = [
    {id: "profile", label: "Profile", disabled: false},
    {id: "billing", label: "Billing", disabled: true},
    {id: "security", label: "Security", disabled: false},
    {id: "alerts", label: "Alerts", disabled: false}
];

const tab = (c, id) => $(c, `#vtab-${id}`);
const selectedTabs = c => $$(c, 'button[role="tab"]').filter(b => b.classList.contains("selected")).map(b => b.id);
const visiblePanels = c => $$(c, '[role="tabpanel"]').filter(p => !p.hidden).map(p => p.id);

function setup(props = {}) {
    const onChange = vi.fn();
    const {container} = mount(VerticalTabs, {tabs, onChange, ...props});
    return {c: container, onChange};
}

describe("AppsMore: vertical tabs with disabled tabs and persistent panels", () => {
    it("selects the first enabled tab and keeps every panel mounted, hiding inactive ones", () => {
        const {c} = setup();
        expect(selectedTabs(c)).toEqual(["vtab-profile"]);
        expect($$(c, '[role="tabpanel"]')).toHaveLength(4);
        expect(visiblePanels(c)).toEqual(["vpanel-profile"]);
        expect(text($(c, ".vtabs-current"))).toBe("Current: profile");
    });

    it("skips a leading disabled tab when choosing the initial selection", () => {
        const {c} = setup({tabs: [{id: "x", label: "X", disabled: true}, ...tabs]});
        expect(selectedTabs(c)).toEqual(["vtab-profile"]);
    });

    it("renders disabled tabs with the disabled attribute and a roving tabindex", () => {
        const {c} = setup();
        expect(tab(c, "billing").disabled).toBe(true);
        expect(tab(c, "profile").disabled).toBe(false);
        expect($$(c, 'button[role="tab"]').map(b => b.getAttribute("tabindex"))).toEqual(["0", "-1", "-1", "-1"]);
    });

    it("clicking a tab selects it, shows its panel, focuses it and reports the change", () => {
        const {c, onChange} = setup();
        click(tab(c, "security"));
        expect(selectedTabs(c)).toEqual(["vtab-security"]);
        expect(visiblePanels(c)).toEqual(["vpanel-security"]);
        expect(tab(c, "security").getAttribute("tabindex")).toBe("0");
        expect(document.activeElement).toBe(tab(c, "security"));
        expect(onChange.mock.calls).toEqual([["security"]]);
    });

    it("clicking a disabled tab does nothing (disabled buttons receive no delegated click)", () => {
        const {c, onChange} = setup();
        click(tab(c, "billing"));
        expect(selectedTabs(c)).toEqual(["vtab-profile"]);
        expect(onChange).not.toHaveBeenCalled();
    });

    it("panel DOM state survives switching because panels are hidden, not unmounted", () => {
        const {c} = setup();
        const field = $(c, ".field-profile");
        input(field, "draft text");
        click(tab(c, "alerts"));
        expect(field.closest('[role="tabpanel"]').hidden).toBe(true);
        click(tab(c, "profile"));
        expect($(c, ".field-profile")).toBe(field);
        expect(field.value).toBe("draft text");
    });

    it("controlled mode: the value prop wins, and clicks only request a change", () => {
        const [value, setValue] = createSignal("alerts");
        const onChange = vi.fn();
        const {container: c} = mount(VerticalTabs, {
            tabs,
            onChange,
            get value() {
                return value();
            }
        });
        expect(selectedTabs(c)).toEqual(["vtab-alerts"]);
        click(tab(c, "security"));
        expect(onChange.mock.calls).toEqual([["security"]]);
        // Parent did not accept the change yet.
        expect(selectedTabs(c)).toEqual(["vtab-alerts"]);
        act(() => setValue("security"));
        expect(selectedTabs(c)).toEqual(["vtab-security"]);
        expect(visiblePanels(c)).toEqual(["vpanel-security"]);
    });

    // BUG: `props.tabs[i]` in a `while` guard is emitted as undefined `VerticalTabs__get_tabs(props)` instead of `props.tabs`.
    it.fails("ArrowDown moves to the next enabled tab, skipping disabled ones", () => {
        const {c, onChange} = setup();
        key(tab(c, "profile"), "ArrowDown");
        expect(selectedTabs(c)).toEqual(["vtab-security"]);
        expect(document.activeElement).toBe(tab(c, "security"));
        expect(onChange.mock.calls).toEqual([["security"]]);
    });

    // BUG: `props.tabs[i]` in a `while` guard is emitted as undefined `VerticalTabs__get_tabs(props)` instead of `props.tabs`.
    it.fails("ArrowUp / ArrowDown wrap around both ends", () => {
        const {c} = setup();
        key(tab(c, "profile"), "ArrowUp");
        expect(selectedTabs(c)).toEqual(["vtab-alerts"]);
        key(tab(c, "alerts"), "ArrowDown");
        expect(selectedTabs(c)).toEqual(["vtab-profile"]);
        key(tab(c, "profile"), "ArrowUp");
        key(tab(c, "alerts"), "ArrowUp");
        expect(selectedTabs(c)).toEqual(["vtab-security"]);
        key(tab(c, "security"), "ArrowUp");
        // billing is disabled: skipped
        expect(selectedTabs(c)).toEqual(["vtab-profile"]);
    });

    it("keys other than the arrows are ignored", () => {
        const {c, onChange} = setup();
        key(tab(c, "profile"), "Enter");
        key(tab(c, "profile"), "ArrowRight");
        expect(selectedTabs(c)).toEqual(["vtab-profile"]);
        expect(onChange).not.toHaveBeenCalled();
    });

    // BUG: ariaSelected / ariaOrientation are emitted verbatim instead of aria-selected / aria-orientation.
    it.fails("exposes aria-selected on tabs and aria-orientation on the tablist", () => {
        const {c} = setup();
        expect($(c, '[role="tablist"]').getAttribute("aria-orientation")).toBe("vertical");
        expect(tab(c, "profile").getAttribute("aria-selected")).toBe("true");
        expect(tab(c, "security").getAttribute("aria-selected")).toBe("false");
    });
});
