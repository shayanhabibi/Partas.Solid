import {describe, it, expect} from "vitest";
import {mount, click} from "../../helpers/index.js";
import {
    StyleForward, ClassForward, StyledConsumers, ReactiveConsumer, AppendedLists, ComputedKey, Swatches,
    AttrEscapeHatch, SvgNumbers, DuplicateKeys, HelperValues, Defaults, SpreadRecord, ListPropConsumer, RecordStyle,
    PanelConsumer, PanelDefault, PanelConditional
} from "./C-Composition.fs.jsx";

const $ = (root, sel) => root.querySelector(sel);
const $$ = (root, sel) => [...root.querySelectorAll(sel)];

describe("Dom/Styles F# consumers styling components", () => {
    it("style' on a spreading component reaches its root; class' merges; style and class forward", () => {
        const {root} = mount(StyledConsumers);
        const pt = $(root, "#pt");
        expect(pt.style.color).toBe("red");
        expect(pt.style.getPropertyValue("--pt")).toBe("2px");

        const card = $(root, "#card");
        expect(card.className).toBe("card wide");
        expect(card.title).toBe("card-title");

        const badge = $(root, ".badge");
        expect(badge.className).toBe("badge badge-ok extra");
        expect(badge.textContent).toBe("B");

        expect($(root, "#sf").style.color).toBe("blue");
        expect($(root, "#cf").className).toBe("forwarded");
    });

    it("StyleForward and ClassForward render without a consumer style/class", () => {
        const a = mount(StyleForward);
        expect(a.root.id).toBe("sf");
        expect(a.root.textContent).toBe("fwd");
        expect(a.root.hasAttribute("style")).toBe(false);
        const b = mount(ClassForward);
        expect(b.root.id).toBe("cf");
        expect(b.root.textContent).toBe("fwd");
        expect(b.root.hasAttribute("class")).toBe(false);
    });

    it("a consumer signal drives a child component's style props", () => {
        const {root} = mount(ReactiveConsumer);
        const cb = $(root, "#cb");
        const ct = $(root, "#ct");
        expect(cb.style.color).toBe("black");
        expect(cb.style.width).toBe("10px");
        expect(ct.style.fontSize).toBe("10px");
        click($(root, ".toggle"));
        expect(cb.style.color).toBe("red");
        expect(cb.style.width).toBe("100px");
        expect(ct.style.fontSize).toBe("20px");
    });

    it("default class and style set through prop setters apply, and consumer values override them", () => {
        const d = mount(Defaults);
        expect(d.root.className).toBe("default-class");
        expect(d.root.style.color).toBe("green");
        const o = mount(Defaults, {class: "mine", style: "color: red"});
        expect(o.root.className).toBe("mine");
        expect(o.root.style.color).toBe("red");
    });

    it("a let-bound record spread applies class and style", () => {
        const {root} = mount(SpreadRecord);
        expect(root.id).toBe("sr");
        expect(root.textContent).toBe("sr");
        expect(root.className).toBe("from-spread");
        expect(root.style.color).toBe("maroon");
    });

    // BUG: component prop `[ a; if on () then b ]` is emitted as JS array `[a, on() ? b : null]` instead of an F# list, so createObj throws on the null entry.
    it.fails("a StyleSpec list prop with a conditional element applies only the yielded declarations", () => {
        const {root} = mount(ListPropConsumer);
        const lp = $(root, "#lp");
        expect(lp.style.display).toBe("grid");
        expect(lp.style.color).toBe("");
        click($(root, ".toggle"));
        expect(lp.style.color).toBe("red");
        expect(lp.style.display).toBe("grid");
    });

    it("a component appends consumer StyleSpec overrides to its base list", () => {
        const {root} = mount(PanelConsumer);
        const p = $(root, "#panel");
        expect(p.style.display).toBe("block");
        expect(p.style.color).toBe("red");
        expect(p.style.getPropertyValue("--pad")).toBe("3px");
        expect(p.textContent).toBe("2 overrides");
    });

    it("the default (empty) overrides list leaves the base styles", () => {
        const {root} = mount(PanelDefault);
        expect(root.style.display).toBe("block");
        expect(root.style.color).toBe("black");
        expect(root.textContent).toBe("0 overrides");
    });

    // BUG: component prop `[ a; if warn () then b ]` is emitted as JS array `[a, warn() ? b : null]` instead of an F# list, so `@`/List.length break.
    it.fails("a conditional overrides list is an F# list the component can append and measure", () => {
        const {root} = mount(PanelConditional);
        const p = $(root, "#panel");
        expect(p.style.color).toBe("orange");
        expect(p.style.getPropertyValue("--pad")).toBe("3px");
        expect(p.textContent).toBe("2 overrides");
        click($(root, ".toggle"));
        expect(p.style.color).toBe("black");
        expect(p.textContent).toBe("1 overrides");
    });
});

describe("Dom/Styles style composition", () => {
    it("appended StyleSpec lists: later declarations override the base ones", () => {
        const {root} = mount(AppendedLists);
        const t = $(root, ".target");
        expect(t.style.display).toBe("flex");
        expect(t.style.color).toBe("black");
        click($(root, ".toggle"));
        expect(t.style.color).toBe("red");
        expect(t.style.display).toBe("flex");
        click($(root, ".toggle"));
        expect(t.style.color).toBe("black");
    });

    it("a signal-dependent key moves the value from one property to another", () => {
        const {root} = mount(ComputedKey);
        const t = $(root, ".target");
        expect(t.style.width).toBe("10px");
        expect(t.style.height).toBe("");
        click($(root, ".toggle"));
        expect(t.style.width).toBe("");
        expect(t.style.height).toBe("10px");
    });

    it("per-item styles in a keyed For use the item and index", () => {
        const {root} = mount(Swatches);
        const sw = $$(root, ".swatch");
        expect(sw.map(s => s.style.backgroundColor)).toEqual(["red", "green", "blue"]);
        expect(sw.map(s => s.style.getPropertyValue("--i"))).toEqual(["0", "1", "2"]);
        expect(sw.map(s => s.title)).toEqual(["red", "green", "blue"]);
    });

    it(".attr(\"style\"/\"class\") sets the raw attributes", () => {
        const {root} = mount(AttrEscapeHatch);
        expect(root.style.color).toBe("red");
        expect(root.className).toBe("via-attr");
    });

    it("numeric SvgStyle members apply to SVG elements", () => {
        const {container} = mount(SvgNumbers);
        const r = $(container, "#rn");
        expect(r.style.opacity).toBe("0.5");
        expect(r.style.strokeOpacity).toBe("0.25");
    });

    it("duplicate keys: the last declaration wins", () => {
        const {root} = mount(DuplicateKeys);
        expect(root.style.color).toBe("blue");
    });

    it("values computed by let-bound helpers stay reactive", () => {
        const {root} = mount(HelperValues);
        const t = $(root, ".target");
        expect(t.style.padding).toBe("2px");
        expect(t.style.margin).toBe("4px");
        click($(root, ".inc"));
        expect(t.style.padding).toBe("3px");
        expect(t.style.margin).toBe("6px");
    });

    it("an anonymous record style (with a custom property field) updates reactively", () => {
        const {root} = mount(RecordStyle);
        const t = $(root, ".target");
        expect(t.style.width).toBe("5px");
        expect(t.style.getPropertyValue("--depth")).toBe("5");
        click($(root, ".inc"));
        expect(t.style.width).toBe("10px");
        expect(t.style.getPropertyValue("--depth")).toBe("10");
    });
});
