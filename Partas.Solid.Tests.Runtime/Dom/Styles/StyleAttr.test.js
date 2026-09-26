import {describe, it, expect} from "vitest";
import {mount, click, flush} from "../../helpers/index.js";
import {createSignal} from "solid-js";
import {
    TypedEnums, TypedStrings, Shorthands, GlobalKeyword, Units, Numbers, CustomProps, SharedList, StringWithVar,
    EmptyList, SvgStyled, MixedWithAttrs, ColorBox, CssText, StyleObjProp, PassThrough, Themed, Progress,
    RemovableProp, ConditionalList, SwapObject, MemoList, StoreDriven, StringToNull, StringThenObject
} from "./A-StyleAttr.fs.jsx";

const $ = (root, sel) => root.querySelector(sel);

describe("Dom/Styles static StyleSpec lists", () => {
    it("typed members with StringEnum values emit kebab-cased property names and values", () => {
        const {root} = mount(TypedEnums);
        expect(root.style.display).toBe("inline-flex");
        expect(root.style.position).toBe("absolute");
        expect(root.style.cursor).toBe("pointer");
        expect(root.style.textAlign).toBe("center");
        expect(root.style.fontWeight).toBe("bold");
        expect(root.style.visibility).toBe("hidden");
        expect(root.style.length).toBe(6);
    });

    it("typed members accept plain strings through the implicit union conversion", () => {
        const {root} = mount(TypedStrings);
        expect(root.style.width).toBe("50%");
        expect(root.style.marginTop).toBe("1.5em");
        expect(root.style.lineHeight).toBe("20px");
        expect(root.style.transform).toBe("translateX(10px)");
        expect(root.style.gridTemplateColumns).toBe("1fr 2fr");
        expect(root.style.gap).toBe("8px");
        expect(root.style.zIndex).toBe("3");
        expect(root.style.opacity).toBe("0.25");
    });

    it("shorthand properties expand into their longhands", () => {
        const {root} = mount(Shorthands);
        expect(root.style.marginTop).toBe("0px");
        expect(root.style.marginLeft).toBe("auto");
        expect(root.style.marginRight).toBe("auto");
        expect(root.style.paddingTop).toBe("1px");
        expect(root.style.paddingRight).toBe("2px");
        expect(root.style.paddingBottom).toBe("3px");
        expect(root.style.paddingLeft).toBe("4px");
    });

    it("CSS-wide keywords (Globals / Color.Inherit) are emitted verbatim", () => {
        const {root} = mount(GlobalKeyword);
        expect(root.style.opacity).toBe("inherit");
        expect(root.style.color).toBe("inherit");
    });

    it("keeps units and CSS functions from ==> values", () => {
        const {root} = mount(Units);
        expect(root.style.width).toBe("calc(100% - 10px)");
        expect(root.style.height).toBe("50vh");
        expect(root.style.fontSize).toBe("1.25rem");
        expect(root.style.minWidth).toBe("12ch");
        expect(root.style.maxWidth).toBe("80vw");
        expect(root.style.borderRadius).toBe("4px 8px");
    });

    it("applies numeric values for unitless properties", () => {
        const {root} = mount(Numbers);
        expect(root.style.opacity).toBe("0.5");
        expect(root.style.zIndex).toBe("7");
        expect(root.style.flexGrow).toBe("2");
        expect(root.style.lineHeight).toBe("1.5");
    });

    it("sets CSS custom properties and keeps var() references", () => {
        const {root} = mount(CustomProps);
        expect(root.style.getPropertyValue("--brand")).toBe("rgb(255, 0, 0)");
        expect(root.style.getPropertyValue("--gap-size")).toBe("6px");
        expect(root.style.color).toBe("var(--brand)");
        expect(root.style.gap).toBe("var(--gap-size, 2px)");
    });

    it("a let-bound StyleSpec list can be applied to several elements", () => {
        const {container} = mount(SharedList);
        for (const id of ["#s1", "#s2"]) {
            const el = $(container, id);
            expect(el.style.display).toBe("block");
            expect(el.style.borderTopWidth).toBe("1px");
            expect(el.style.borderTopStyle).toBe("solid");
        }
    });

    it("a plain string style keeps custom properties declared inside it", () => {
        const {root} = mount(StringWithVar);
        expect(root.style.getPropertyValue("--x")).toBe("3px");
        expect(root.style.color).toBe("red");
        expect(root.style.padding).toBe("var(--x)");
    });

    it("an empty StyleSpec list leaves no inline declarations", () => {
        const {root} = mount(EmptyList);
        expect(root.style.length).toBe(0);
        expect(root.getAttribute("style") ?? "").toBe("");
    });

    it("applies SvgStyle members on an SVG element", () => {
        const {container} = mount(SvgStyled);
        const c = $(container, "#circ");
        expect(c.namespaceURI).toBe("http://www.w3.org/2000/svg");
        expect(c.style.fill).toBe("red");
        expect(c.style.strokeWidth).toBe("2");
    });

    it("combines style' with class and other attributes on the same element", () => {
        const {root} = mount(MixedWithAttrs);
        expect(root.className).toBe("tag");
        expect(root.title).toBe("t");
        expect(root.style.color).toBe("green");
        expect(root.textContent).toBe("mixed");
    });
});

describe("Dom/Styles reactive style driven by props", () => {
    it("typed members read props reactively", () => {
        const [fg, setFg] = createSignal("red");
        const [w, setW] = createSignal("10px");
        const {root} = mount(ColorBox, {
            get fg() {
                return fg();
            }, get width() {
                return w();
            }
        });
        expect(root.style.color).toBe("red");
        expect(root.style.width).toBe("10px");
        expect(root.style.display).toBe("block");
        setFg("blue");
        setW("25%");
        flush();
        expect(root.style.color).toBe("blue");
        expect(root.style.width).toBe("25%");
        // consumed props do not leak as attributes
        expect(root.hasAttribute("fg")).toBe(false);
        expect(root.hasAttribute("width")).toBe(false);
    });

    it("removes a property whose prop value becomes undefined, keeping the others", () => {
        const [fg, setFg] = createSignal("red");
        const {root} = mount(ColorBox, {
            get fg() {
                return fg();
            }, width: "5px"
        });
        expect(root.style.color).toBe("red");
        setFg(undefined);
        flush();
        expect(root.style.color).toBe("");
        expect(root.style.width).toBe("5px");
        expect(root.style.display).toBe("block");
    });

    it("a string style prop replaces the whole cssText when it changes", () => {
        const [css, setCss] = createSignal("color: red; margin-top: 2px");
        const {root} = mount(CssText, {
            get css() {
                return css();
            }
        });
        expect(root.style.color).toBe("red");
        expect(root.style.marginTop).toBe("2px");
        setCss("padding-left: 3px");
        flush();
        expect(root.style.color).toBe("");
        expect(root.style.marginTop).toBe("");
        expect(root.style.paddingLeft).toBe("3px");
    });

    it("an object style prop is diffed: dropped keys are removed, new keys added", () => {
        const [s, setS] = createSignal({color: "red", "font-size": "10px"});
        const {root} = mount(StyleObjProp, {
            get styles() {
                return s();
            }
        });
        expect(root.style.color).toBe("red");
        expect(root.style.fontSize).toBe("10px");
        setS({"font-size": "12px", "--k": "1"});
        flush();
        expect(root.style.color).toBe("");
        expect(root.style.fontSize).toBe("12px");
        expect(root.style.getPropertyValue("--k")).toBe("1");
    });

    it("a style object passed by the consumer reaches the root through the spread", () => {
        const {root} = mount(PassThrough, {style: {color: "purple", "--pt": "9px"}});
        expect(root.id).toBe("pt");
        expect(root.style.color).toBe("purple");
        expect(root.style.getPropertyValue("--pt")).toBe("9px");
    });

    it("a style string passed by the consumer reaches the root through the spread", () => {
        const {root} = mount(PassThrough, {style: "color: teal"});
        expect(root.style.color).toBe("teal");
    });

    it("a custom property follows its prop and is removed when the prop is cleared", () => {
        const [accent, setAccent] = createSignal("orange");
        const {root} = mount(Themed, {
            get accent() {
                return accent();
            }
        });
        expect(root.style.getPropertyValue("--accent")).toBe("orange");
        expect(root.style.color).toBe("var(--accent)");
        setAccent("navy");
        flush();
        expect(root.style.getPropertyValue("--accent")).toBe("navy");
        setAccent(undefined);
        flush();
        expect(root.style.getPropertyValue("--accent")).toBe("");
        expect(root.style.color).toBe("var(--accent)");
    });
});

describe("Dom/Styles reactive style driven by signals", () => {
    it("a progress bar width tracks a numeric signal with a unit suffix", () => {
        const {root} = mount(Progress);
        const bar = $(root, ".bar");
        expect(bar.style.width).toBe("10%");
        expect(bar.style.height).toBe("4px");
        click($(root, ".inc"));
        expect(bar.style.width).toBe("25%");
        click($(root, ".inc"));
        expect(bar.style.width).toBe("40%");
    });

    it("null and undefined property values remove the property and re-add it later", () => {
        const {root} = mount(RemovableProp);
        const t = $(root, ".target");
        expect(t.style.color).toBe("red");
        expect(t.style.backgroundColor).toBe("blue");
        click($(root, ".toggle"));
        expect(t.style.color).toBe("");
        expect(t.style.backgroundColor).toBe("");
        expect(t.style.padding).toBe("2px");
        click($(root, ".toggle"));
        expect(t.style.color).toBe("red");
        expect(t.style.backgroundColor).toBe("blue");
    });

    it("a conditional StyleSpec list comprehension adds and removes declarations", () => {
        const {root} = mount(ConditionalList);
        const t = $(root, ".target");
        expect(t.style.display).toBe("block");
        expect(t.style.color).toBe("");
        click($(root, ".toggle"));
        expect(t.style.color).toBe("red");
        expect(t.style.getPropertyValue("outline")).toBe("1px solid red");
        click($(root, ".toggle"));
        expect(t.style.color).toBe("");
        expect(t.style.getPropertyValue("outline")).toBe("");
        expect(t.style.display).toBe("block");
    });

    it("swapping style objects removes keys missing from the new object", () => {
        const {root} = mount(SwapObject);
        const t = $(root, ".target");
        expect(t.style.color).toBe("red");
        expect(t.style.paddingTop).toBe("5px");
        click($(root, ".toggle"));
        expect(t.style.color).toBe("blue");
        expect(t.style.marginLeft).toBe("3px");
        expect(t.style.paddingTop).toBe("");
        click($(root, ".toggle"));
        expect(t.style.marginLeft).toBe("");
        expect(t.style.paddingTop).toBe("5px");
    });

    it("a memoised StyleSpec list re-applies when the memo changes", () => {
        const {root} = mount(MemoList);
        const t = $(root, ".target");
        expect(t.style.fontSize).toBe("1em");
        expect(t.style.getPropertyValue("--size")).toBe("1");
        click($(root, ".grow"));
        click($(root, ".grow"));
        expect(t.style.fontSize).toBe("3em");
        expect(t.style.getPropertyValue("--size")).toBe("3");
    });

    it("style follows store fields updated through the store setter", () => {
        const {root} = mount(StoreDriven);
        const t = $(root, ".target");
        expect(t.style.color).toBe("red");
        expect(t.style.visibility).toBe("visible");
        click($(root, ".recolor"));
        expect(t.style.color).toBe("blue");
        click($(root, ".hide"));
        expect(t.style.visibility).toBe("hidden");
        expect(t.style.color).toBe("blue");
    });

    it("a string style set to null removes the style attribute", () => {
        const {root} = mount(StringToNull);
        const t = $(root, ".target");
        expect(t.style.color).toBe("red");
        click($(root, ".toggle"));
        expect(t.style.color).toBe("");
        expect(t.hasAttribute("style")).toBe(false);
        click($(root, ".toggle"));
        expect(t.style.color).toBe("red");
    });

    it("switching between the string and object forms clears the previous form", () => {
        const {root} = mount(StringThenObject);
        const t = $(root, ".target");
        expect(t.style.color).toBe("red");
        expect(t.style.padding).toBe("1px");
        click($(root, ".toggle"));
        expect(t.style.color).toBe("");
        expect(t.style.padding).toBe("");
        expect(t.style.marginTop).toBe("7px");
        click($(root, ".toggle"));
        expect(t.style.marginTop).toBe("");
        expect(t.style.color).toBe("red");
    });
});
