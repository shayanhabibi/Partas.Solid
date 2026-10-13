import {describe, it, expect} from "vitest";
import {mount, click, flush} from "../../helpers/index.js";
import {createSignal} from "solid-js";
import {
    cn, StaticClass, ArrayClass, MultiTokenKeys, RecordClassMap, Badge, ClassProp, ClassMapProp, Card, ToggleClass,
    SignalClassMap, SignalClassArray, StringThenMap, Tabs, ClassAndStyle, SvgClass
} from "./B-ClassAttr.fs.jsx";

const $ = (root, sel) => root.querySelector(sel);
const $$ = (root, sel) => [...root.querySelectorAll(sel)];
const classes = el => [...el.classList].sort();

describe("Dom/Styles static class values", () => {
    it("a static class string is kept verbatim and tokenised by the DOM", () => {
        const {root} = mount(StaticClass);
        expect(classes(root)).toEqual(["a", "b", "c"]);
    });

    it("a clsx-style array (nested arrays and object entries) applies truthy classes", () => {
        const {root} = mount(ArrayClass);
        expect(classes(root)).toEqual(["a", "b", "c", "d"]);
    });

    it("object keys holding several space-separated names toggle each name", () => {
        const {root} = mount(MultiTokenKeys);
        expect(classes(root)).toEqual(["p-2", "rounded"]);
    });

    it("an anonymous record class map with a quoted key applies the key name", () => {
        const {root} = mount(RecordClassMap);
        expect(classes(root)).toEqual(["is-open"]);
    });

    it("SVG elements take class strings", () => {
        const {container} = mount(SvgClass);
        const svg = $(container, "#svgc");
        expect(svg.getAttribute("class")).toBe("icon icon-lg");
        expect($(svg, "rect").getAttribute("class")).toBe("fill");
    });
});

describe("Dom/Styles class from props", () => {
    it("cn drops empty and undefined entries", () => {
        expect(cn(["a", "", undefined, null, "b"])).toBe("a b");
    });

    it("Badge applies the default tone and appends the consumer class", () => {
        const {root} = mount(Badge, {class: "extra", children: "New"});
        expect(root.className).toBe("badge badge-neutral extra");
        expect(root.textContent).toBe("New");
    });

    it("Badge without a consumer class has no trailing token", () => {
        const {root} = mount(Badge, {tone: "danger", children: "x"});
        expect(root.className).toBe("badge badge-danger");
    });

    it("Badge re-computes its class when tone and class props change", () => {
        const [tone, setTone] = createSignal("info");
        const [cls, setCls] = createSignal("a");
        const {root} = mount(Badge, {
            get tone() {
                return tone();
            }, get class() {
                return cls();
            }
        });
        expect(root.className).toBe("badge badge-info a");
        setTone("warn");
        setCls("b");
        flush();
        expect(root.className).toBe("badge badge-warn b");
    });

    it("a class string prop updates the class attribute", () => {
        const [c, setC] = createSignal("one");
        const {root} = mount(ClassProp, {
            get cls() {
                return c();
            }
        });
        expect(root.className).toBe("one");
        setC("two three");
        flush();
        expect(classes(root)).toEqual(["three", "two"]);
    });

    it("a class prop set to undefined removes the class attribute", () => {
        const [c, setC] = createSignal("one");
        const {root} = mount(ClassProp, {
            get cls() {
                return c();
            }
        });
        setC(undefined);
        flush();
        expect(root.hasAttribute("class")).toBe(false);
    });

    it("a class object map from boolean props toggles individual classes", () => {
        const [sel, setSel] = createSignal(false);
        const [dis, setDis] = createSignal(true);
        const {root} = mount(ClassMapProp, {
            get selected() {
                return sel();
            }, get disabled() {
                return dis();
            }
        });
        expect(classes(root)).toEqual(["disabled", "item"]);
        setSel(true);
        setDis(false);
        flush();
        expect(classes(root)).toEqual(["item", "selected"]);
    });

    it("Card merges its own class with the consumer class and spreads other props", () => {
        const {root} = mount(Card, {class: "wide", title: "hello", "data-x": "1"});
        expect(root.className).toBe("card wide");
        expect(root.title).toBe("hello");
        expect(root.getAttribute("data-x")).toBe("1");
        expect(root.id).toBe("card");
    });

    it("Card follows a reactive consumer class", () => {
        const [c, setC] = createSignal("a");
        const {root} = mount(Card, {
            get class() {
                return c();
            }
        });
        expect(root.className).toBe("card a");
        setC("b");
        flush();
        expect(root.className).toBe("card b");
    });
});

describe("Dom/Styles class driven by signals", () => {
    it("a class toggled to null removes the attribute and restores it", () => {
        const {root} = mount(ToggleClass);
        const t = root.firstElementChild;
        expect(t.className).toBe("active");
        click($(root, ".toggle"));
        expect(t.hasAttribute("class")).toBe(false);
        click($(root, ".toggle"));
        expect(t.className).toBe("active");
    });

    it("a signal-driven class map only toggles the classes that changed", () => {
        const {root} = mount(SignalClassMap);
        const p = root.firstElementChild;
        expect(classes(p)).toEqual(["panel"]);
        // a class added from outside survives the object-map diff
        p.classList.add("external");
        click($(root, ".open-btn"));
        expect(classes(p)).toEqual(["external", "open", "panel"]);
        click($(root, ".err-btn"));
        expect(classes(p)).toEqual(["error", "external", "open", "panel"]);
        click($(root, ".open-btn"));
        expect(classes(p)).toEqual(["error", "external", "panel"]);
    });

    it("a signal-driven class array replaces the variant class and adds the conditional one", () => {
        const {root} = mount(SignalClassArray);
        const b = root.firstElementChild;
        expect(classes(b)).toEqual(["btn", "btn-sm"]);
        click($(root, ".lg"));
        expect(classes(b)).toEqual(["big", "btn", "btn-lg"]);
    });

    it("switching a class from a string to an object map replaces the string classes", () => {
        const {root} = mount(StringThenMap);
        const t = root.firstElementChild;
        expect(classes(t)).toEqual(["s1", "s2"]);
        click($(root, ".toggle"));
        expect(classes(t)).toEqual(["m1"]);
        click($(root, ".toggle"));
        expect(classes(t)).toEqual(["s1", "s2"]);
    });

    it("tabs mark exactly one tab as selected", () => {
        const {root} = mount(Tabs);
        const tabs = $$(root, "button");
        const selected = () => tabs.map(t => t.classList.contains("selected"));
        expect(tabs.map(t => t.textContent)).toEqual(["One", "Two", "Three"]);
        expect(selected()).toEqual([true, false, false]);
        click(tabs[2]);
        expect(selected()).toEqual([false, false, true]);
        click(tabs[1]);
        expect(selected()).toEqual([false, true, false]);
        expect(tabs.every(t => t.classList.contains("tab"))).toBe(true);
    });

    it("class and style driven by the same signal update together", () => {
        const {root} = mount(ClassAndStyle);
        const a = root.firstElementChild;
        expect(a.className).toBe("alert");
        expect(a.style.color).toBe("black");
        click($(root, ".toggle"));
        expect(a.className).toBe("alert danger");
        expect(a.style.color).toBe("red");
    });
});
