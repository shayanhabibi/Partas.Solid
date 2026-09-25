import {describe, it, expect} from "vitest";
import {mount, click, input, flush, text} from "../../helpers/index.js";
import {
    InputTypes, ControlledCheckbox, RadioGroup, LinkedRange, ValueSemantics, ControlledTextarea, MaybeValue,
    ToggledInputFlags, SpellcheckOff, SpellcheckOn, PropNamespace, Uncontrolled, Inert
} from "./A-Inputs.fs.jsx";

const change = el => flush(() => el.dispatchEvent(new Event("change", {bubbles: true})));

describe("Dom/Forms input types", () => {
    it("renders each input type with its type-specific attributes and properties", () => {
        const {container} = mount(InputTypes);
        const q = s => container.querySelector(s);

        const t = q("#t-text");
        expect(t.type).toBe("text");
        expect(t.value).toBe("hello");
        expect(t.maxLength).toBe(8);
        expect(t.minLength).toBe(2);
        expect(t.pattern).toBe("[a-z]+");
        expect(t.getAttribute("autocomplete")).toBe("off");

        const n = q("#t-num");
        expect(n.type).toBe("number");
        expect(n.valueAsNumber).toBe(5);
        expect(n.min).toBe("0");
        expect(n.max).toBe("10");
        expect(n.step).toBe("0.5");

        const r = q("#t-range");
        expect(r.type).toBe("range");
        expect(r.value).toBe("30");
        expect(r.max).toBe("200");

        const d = q("#t-date");
        expect(d.type).toBe("date");
        expect(d.value).toBe("2024-03-15");
        expect(d.min).toBe("2024-01-01");
        expect(d.max).toBe("2024-12-31");

        expect(q("#t-email").multiple).toBe(true);
        expect(q("#t-email").value).toBe("a@b.co");
        // minLength (camelCase in the binding) lands on the lower-case attribute
        expect(q("#t-pass").getAttribute("minlength")).toBe("4");
        expect(q("#t-pass").type).toBe("password");
        expect(q("#t-hidden").type).toBe("hidden");
        expect(q("#t-hidden").name).toBe("token");
        expect(q("#t-color").value).toBe("#ff0000");
        expect(q("#t-file").accept).toBe("image/*");
        expect(q("#t-file").multiple).toBe(true);
        expect(q("#t-search").getAttribute("inputmode")).toBe("search");
        expect(q("#t-search").getAttribute("enterkeyhint")).toBe("search");
    });
});

describe("Dom/Forms checkbox and radio", () => {
    it("a signal-driven checkbox reflects the signal as the checked property, not an attribute", () => {
        const {container} = mount(ControlledCheckbox);
        const cb = container.querySelector("#cb");
        expect(cb.checked).toBe(false);
        click(container.querySelector("#cb-set"));
        expect(cb.checked).toBe(true);
        expect(cb.hasAttribute("checked")).toBe(false);
        expect(text(container.querySelector("#cb-state"))).toBe("on");
        click(container.querySelector("#cb-clear"));
        expect(cb.checked).toBe(false);
    });

    it("user clicks feed back into the signal through onChange", () => {
        const {container} = mount(ControlledCheckbox);
        const cb = container.querySelector("#cb");
        click(cb);
        expect(cb.checked).toBe(true);
        expect(text(container.querySelector("#cb-state"))).toBe("on");
        click(cb);
        expect(text(container.querySelector("#cb-state"))).toBe("off");
    });

    it("a radio group checks exactly the option equal to the signal", () => {
        const {container} = mount(RadioGroup);
        const radios = [...container.querySelectorAll('input[type="radio"]')];
        expect(radios.map(r => r.value)).toEqual(["s", "m", "l"]);
        expect(radios.map(r => r.checked)).toEqual([false, true, false]);
        expect(radios.every(r => r.name === "size")).toBe(true);
        expect(text(container.querySelector("#rg-out"))).toBe("m");
        expect(container.querySelector("legend").textContent).toBe("Size");
    });

    it("clicking a radio updates the signal; setting the signal moves the check", () => {
        const {container} = mount(RadioGroup);
        const radios = [...container.querySelectorAll('input[type="radio"]')];
        click(radios[0]);
        expect(text(container.querySelector("#rg-out"))).toBe("s");
        expect(radios.map(r => r.checked)).toEqual([true, false, false]);
        click(container.querySelector("#rg-large"));
        expect(radios.map(r => r.checked)).toEqual([false, false, true]);
        expect(text(container.querySelector("#rg-out"))).toBe("l");
    });
});

describe("Dom/Forms range and number", () => {
    it("range and number inputs are two views of one numeric signal", () => {
        const {container} = mount(LinkedRange);
        const range = container.querySelector("#lr-range");
        const num = container.querySelector("#lr-num");
        expect(range.value).toBe("40");
        expect(num.value).toBe("40");
        input(range, "70");
        expect(text(container.querySelector("#lr-out"))).toBe("70");
        expect(num.value).toBe("70");
        input(num, "15");
        expect(range.value).toBe("15");
        expect(range.valueAsNumber).toBe(15);
    });
});

describe("Dom/Forms value semantics", () => {
    it("a literal value is the value attribute (default value); a dynamic value is the property only", () => {
        const {container} = mount(ValueSemantics);
        const st = container.querySelector("#vs-static");
        const dyn = container.querySelector("#vs-dyn");
        expect(st.getAttribute("value")).toBe("static");
        expect(st.defaultValue).toBe("static");
        expect(dyn.value).toBe("dynamic");
        expect(dyn.hasAttribute("value")).toBe(false);
    });

    it("typing updates the signal; a programmatic write replaces what the user typed", () => {
        const {container} = mount(ValueSemantics);
        const dyn = container.querySelector("#vs-dyn");
        input(dyn, "typed");
        expect(text(container.querySelector("#vs-echo"))).toBe("typed");
        click(container.querySelector("#vs-reset"));
        expect(dyn.value).toBe("reset");
        expect(text(container.querySelector("#vs-echo"))).toBe("reset");
    });

    it("typing into a static-value input leaves the attribute alone", () => {
        const {container} = mount(ValueSemantics);
        const st = container.querySelector("#vs-static");
        input(st, "changed");
        expect(st.value).toBe("changed");
        expect(st.getAttribute("value")).toBe("static");
    });

    it("an undefined value prop yields an empty input, never the string \"undefined\"", () => {
        const {root} = mount(MaybeValue, {});
        expect(root.value).toBe("");
        const {root: r2} = mount(MaybeValue, {v: "given"});
        expect(r2.value).toBe("given");
    });

    it("a controlled textarea takes its value from the signal and writes back on input", () => {
        const {container} = mount(ControlledTextarea);
        const ta = container.querySelector("#ct-ta");
        expect(ta.value).toBe("first line");
        expect(ta.rows).toBe(4);
        expect(ta.cols).toBe(30);
        expect(ta.getAttribute("wrap")).toBe("soft");
        expect(text(container.querySelector("#ct-len"))).toBe("10");
        input(ta, "abc\ndef");
        expect(text(container.querySelector("#ct-len"))).toBe("7");
        click(container.querySelector("#ct-clear"));
        expect(ta.value).toBe("");
        expect(text(container.querySelector("#ct-len"))).toBe("0");
    });

    it(".attr(\"prop:value\", ...) writes the DOM property reactively, without an attribute", () => {
        const {container} = mount(PropNamespace);
        const inp = container.querySelector("#pn-in");
        expect(inp.value).toBe("a");
        expect(inp.hasAttribute("prop:value")).toBe(false);
        expect(inp.hasAttribute("value")).toBe(false);
        click(container.querySelector("#pn-b"));
        expect(inp.value).toBe("b");
        const dv = container.querySelector("#pn-dv");
        expect(dv.defaultValue).toBe("dv");
        expect(dv.value).toBe("dv");
    });

    it("uncontrolled defaultValue / defaultChecked (through .attr) seed the controls and survive reset", () => {
        const {container} = mount(Uncontrolled);
        const t = container.querySelector("#unc-text");
        const cb = container.querySelector("#unc-cb");
        const ta = container.querySelector("#unc-ta");
        expect(t.value).toBe("start");
        expect(cb.checked).toBe(true);
        expect(ta.value).toBe("body");
        input(t, "edited");
        click(cb);
        expect(cb.checked).toBe(false);
        container.querySelector("#unc").reset();
        expect(t.value).toBe("start");
        expect(cb.checked).toBe(true);
    });
});

describe("Dom/Forms boolean input flags", () => {
    it("disabled / readonly / required follow a signal; autofocus={false} is omitted", () => {
        const {container} = mount(ToggledInputFlags);
        const inp = container.querySelector("#tf-in");
        const ta = container.querySelector("#tf-ta");
        expect(inp.disabled).toBe(true);
        expect(inp.readOnly).toBe(true);
        expect(inp.required).toBe(true);
        expect(inp.hasAttribute("autofocus")).toBe(false);
        expect(ta.disabled).toBe(true);
        expect(ta.readOnly).toBe(true);
        click(container.querySelector("#tf-toggle"));
        expect(inp.getAttributeNames().sort()).toEqual(["id"]);
        expect(ta.getAttributeNames().sort()).toEqual(["id"]);
        expect(ta.readOnly).toBe(false);
        click(container.querySelector("#tf-toggle"));
        expect(inp.getAttribute("disabled")).toBe("");
        expect(ta.hasAttribute("readonly")).toBe(true);
    });

    it("inert true sets the attribute; inert false omits it", () => {
        const {container} = mount(Inert);
        expect(container.querySelector("#inert-on").getAttribute("inert")).toBe("");
        expect(container.querySelector("#inert-off").hasAttribute("inert")).toBe(false);
    });

    it("spellcheck = true renders an enabling spellcheck attribute", () => {
        const {root} = mount(SpellcheckOn);
        // "" and "true" are both the true state of the enumerated attribute
        expect(["", "true"]).toContain(root.getAttribute("spellcheck"));
        expect(root.getAttribute("contenteditable")).toBe("true");
    });

    // BUG: HTMLAttributes.spellcheck is typed bool, so spellcheck = false emits spellcheck={false}, which Solid 2 removes; it must emit the string "false".
    it.fails("spellcheck = false renders spellcheck=\"false\" (disables spell checking)", () => {
        const {root} = mount(SpellcheckOff);
        expect(root.getAttribute("spellcheck")).toBe("false");
    });
});
