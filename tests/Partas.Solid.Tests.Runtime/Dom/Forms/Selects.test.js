import {describe, it, expect, vi} from "vitest";
import {mount, click, flush, text, settle} from "../../helpers/index.js";
import {
    ControlledSelect, MultiSelect, Groups, LateOptions, DataList, Labels, FieldsetDisabled, ButtonTypes,
    FormDataSubmit, ResetInvalid
} from "./B-Selects.fs.jsx";

const choose = (sel, value) => flush(() => {
    sel.value = value;
    sel.dispatchEvent(new Event("change", {bubbles: true}));
});
const selectedValues = sel => [...sel.selectedOptions].map(o => o.value);

describe("Dom/Forms select", () => {
    it("a controlled select shows the signal's option after options render", async () => {
        const {container} = mount(ControlledSelect);
        const sel = container.querySelector("#cs-sel");
        await settle(1); // select.value is also re-applied in a microtask
        expect(sel.value).toBe("pear");
        expect([...sel.options].map(o => o.textContent.trim())).toEqual(["apple", "pear", "plum"]);
    });

    it("choosing an option writes back; setting the signal moves the selection", async () => {
        const {container} = mount(ControlledSelect);
        const sel = container.querySelector("#cs-sel");
        await settle(1);
        choose(sel, "apple");
        expect(text(container.querySelector("#cs-out"))).toBe("apple");
        click(container.querySelector("#cs-plum"));
        await settle(1);
        expect(sel.value).toBe("plum");
        expect(sel.selectedIndex).toBe(2);
    });

    it("a select value set before its options exist still selects the option", async () => {
        const {root} = mount(LateOptions);
        await settle(1);
        expect(root.value).toBe("z");
    });

    it("multi-select: per-option selected follows a signal", () => {
        const {container} = mount(MultiSelect);
        const sel = container.querySelector("#ms-sel");
        expect(sel.multiple).toBe(true);
        expect(sel.size).toBe(4);
        expect(sel.name).toBe("letters");
        expect(selectedValues(sel)).toEqual(["b", "c"]);
        expect([...sel.options].map(o => o.textContent.trim())).toEqual(["A", "B", "C", "D"]);
        click(container.querySelector("#ms-only-a"));
        expect(selectedValues(sel)).toEqual(["a"]);
    });

    it("optgroup label/disabled and option label render and disable their options", () => {
        const {root} = mount(Groups);
        const groups = [...root.querySelectorAll("optgroup")];
        expect(groups.map(g => g.label)).toEqual(["Fruit", "Veg"]);
        expect(groups[1].disabled).toBe(true);
        expect(groups[0].disabled).toBe(false);
        expect(root.querySelector('option[value="pear"]').label).toBe("Pear (ripe)");
        expect(root.querySelector('option[value="pear"]').text.trim()).toBe("Pear");
        expect(root.value).toBe("apple");
    });

    it("an input's list attribute points at a datalist of valueless-child options", () => {
        const {container} = mount(DataList);
        const inp = container.querySelector("#dl-in");
        expect(inp.getAttribute("list")).toBe("dl-opts");
        const dl = container.querySelector("#dl-opts");
        expect([...dl.querySelectorAll("option")].map(o => o.value)).toEqual(["red", "green"]);
        expect(inp.list).toBe(dl);
    });
});

describe("Dom/Forms labels and fieldsets", () => {
    it("label for' becomes the for attribute and labels its control; wrapping labels their child", () => {
        const {container} = mount(Labels);
        const forLbl = container.querySelector("#lb-for");
        const target = container.querySelector("#lb-target");
        expect(forLbl.htmlFor).toBe("lb-target");
        expect(forLbl.control).toBe(target);
        expect([...target.labels]).toEqual([forLbl]);
        click(forLbl);
        expect(target.checked).toBe(true);

        const wrap = container.querySelector("#lb-wrap");
        const inner = container.querySelector("#lb-inner");
        expect(wrap.control).toBe(inner);
        click(wrap);
        expect(inner.checked).toBe(true);
    });

    it("output for' is a token list of ids", () => {
        const {container} = mount(Labels);
        const out = container.querySelector("#lb-out");
        expect([...out.htmlFor]).toEqual(["lb-target", "lb-inner"]);
        expect(out.name).toBe("result");
    });

    it("fieldset disabled disables descendants and toggles with a signal", () => {
        const {container} = mount(FieldsetDisabled);
        const fs = container.querySelector("#fd-set");
        const inp = container.querySelector("#fd-in");
        expect(fs.disabled).toBe(true);
        expect(inp.matches(":disabled")).toBe(true);
        expect(container.querySelector("#fd-btn").matches(":disabled")).toBe(true);
        click(container.querySelector("#fd-toggle"));
        expect(fs.hasAttribute("disabled")).toBe(false);
        expect(inp.matches(":disabled")).toBe(false);
        expect(fs.name).toBe("grp");
        expect(fs.elements.length).toBe(2);
    });
});

describe("Dom/Forms submission", () => {
    it("a button without type defaults to submit; type=button does not submit", () => {
        const onSubmitted = vi.fn();
        const {container} = mount(ButtonTypes, {onSubmitted});
        expect(container.querySelector("#bt-default").type).toBe("submit");
        expect(container.querySelector("#bt-button").type).toBe("button");
        click(container.querySelector("#bt-button"));
        expect(onSubmitted).not.toHaveBeenCalled();
        click(container.querySelector("#bt-default"));
        expect(onSubmitted).toHaveBeenCalledTimes(1);
        expect(onSubmitted).toHaveBeenLastCalledWith("bt-default");
        click(container.querySelector("#bt-submit"));
        expect(onSubmitted).toHaveBeenLastCalledWith("bt-submit");
        expect(container.querySelector("#bt-submit").name).toBe("action");
        expect(container.querySelector("#bt-submit").value).toBe("save");
    });

    it("type=reset restores fields without submitting", () => {
        const onSubmitted = vi.fn();
        const {container} = mount(ButtonTypes, {onSubmitted});
        const inp = container.querySelector("#bt-in");
        inp.value = "edited";
        click(container.querySelector("#bt-reset"));
        expect(inp.value).toBe("initial");
        expect(onSubmitted).not.toHaveBeenCalled();
    });

    it("onSubmit with preventDefault reads the form through FormData (checked, selected, textarea, disabled skipped)", () => {
        const onData = vi.fn();
        const {container} = mount(FormDataSubmit, {onData});
        const f = container.querySelector("#fds");
        expect(f.noValidate).toBe(true);
        expect(f.method).toBe("post");
        click(container.querySelector("#fds-go"));
        expect(onData).toHaveBeenCalledTimes(1);
        expect(onData.mock.calls[0][0]).toEqual({user: "ann", agree: "on", tier: "pro", note: "hi"});
    });

    it("onReset fires on form.reset(); onInvalid fires on checkValidity of an empty required field", () => {
        const log = vi.fn();
        const {container} = mount(ResetInvalid, {log});
        const f = container.querySelector("#ri");
        flush(() => f.reset());
        expect(log).toHaveBeenCalledWith("reset");
        const ok = container.querySelector("#ri-req").checkValidity();
        expect(ok).toBe(false);
        expect(log).toHaveBeenCalledWith("invalid");
    });
});
