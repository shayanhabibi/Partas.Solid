import {describe, it, expect} from "vitest";
import {mount} from "../../helpers/index.js";
import {
    Globals, DataAttrs, AriaAttrs, AriaHiddenFalse, AriaViaAttr, BoolsTrue, BoolsFalse, BoolProps, BoolExt,
    StyleSpecEl, StyleString, StyleObj, ClassList, PerElement, InnerHtml, HtmlProp, ValueProp, TabIndexes
} from "./B-Attributes.fs.jsx";

const attrs = el => Object.fromEntries([...el.attributes].map(a => [a.name, a.value]));

describe("Dom/Elements global attributes", () => {
    it("renders id, class, title, lang, dir, tabindex, draggable and accesskey", () => {
        const {root} = mount(Globals);
        expect(attrs(root)).toEqual({
            id: "g",
            class: "a b",
            title: "tip",
            lang: "en",
            dir: "rtl",
            tabindex: "3",
            draggable: "true",
            accesskey: "k"
        });
        // the attributes reflect into the matching DOM properties
        expect(root.tabIndex).toBe(3);
        expect(root.accessKey).toBe("k");
        expect([...root.classList]).toEqual(["a", "b"]);
        expect(root.textContent).toBe("globals");
    });

    it("renders tabindex 0 and -1 as attributes", () => {
        const {container} = mount(TabIndexes);
        expect(container.querySelector("#t0").getAttribute("tabindex")).toBe("0");
        expect(container.querySelector("#tneg").getAttribute("tabindex")).toBe("-1");
        expect(container.querySelector("#tneg").tabIndex).toBe(-1);
    });

    it("renders .data(name, value) as data-* attributes (readable through dataset) and .attr verbatim", () => {
        const {root} = mount(DataAttrs);
        expect(root.getAttribute("data-user-id")).toBe("42");
        expect(root.getAttribute("data-role")).toBe("admin");
        expect({...root.dataset}).toEqual({userId: "42", role: "admin"});
        expect(root.getAttribute("custom-attr")).toBe("yes");
    });

    it("renders aria-* attributes passed through .attr", () => {
        const {root} = mount(AriaViaAttr);
        expect(root.getAttribute("aria-label")).toBe("Close");
        expect(root.getAttribute("aria-pressed")).toBe("true");
    });

    // BUG: Aria module properties (ariaLabel, ariaExpanded, ...) are emitted verbatim as camelCase JSX
    // attributes instead of aria-*; the HTML parser lowercases them to "arialabel" etc.
    it.fails("renders Aria module properties as aria-* attributes", () => {
        const {root} = mount(AriaAttrs);
        expect(root.getAttribute("aria-label")).toBe("Close dialog");
        expect(root.getAttribute("aria-expanded")).toBe("false");
        expect(root.getAttribute("aria-controls")).toBe("menu-1");
        expect(root.getAttribute("aria-hidden")).toBe("true");
    });

    // BUG: ariaHidden = false is emitted as ariaHidden={false}. ARIA needs the string "false";
    // even a correctly named aria-hidden={false} would remove the attribute in Solid 2 (upstream jsx.d.ts
    // types aria-hidden as "true" | "false" | false-to-remove; the Aria binding types it as bool).
    it.fails("renders ariaHidden = false as aria-hidden=\"false\"", () => {
        const {root} = mount(AriaHiddenFalse);
        expect(root.getAttribute("aria-hidden")).toBe("false");
    });
});

describe("Dom/Elements boolean attributes", () => {
    it("sets boolean attributes (empty value) and the matching properties when true", () => {
        const {container} = mount(BoolsTrue);
        const q = s => container.querySelector(s);
        expect(q("#dis").getAttribute("disabled")).toBe("");
        expect(q("#dis").disabled).toBe(true);
        expect(q("#chk").checked).toBe(true);
        expect(q("#ro").hasAttribute("readonly")).toBe(true);
        expect(q("#ro").readOnly).toBe(true);
        expect(q("#req").required).toBe(true);
        expect(q("#btn").disabled).toBe(true);
        expect(q("#hid").hasAttribute("hidden")).toBe(true);
        expect(q("#hid").hidden).toBe(true);
    });

    it("omits boolean attributes when false (Solid 2 removes the attribute on false)", () => {
        const {container} = mount(BoolsFalse);
        const q = s => container.querySelector(s);
        for (const [sel, name] of [["#dis", "disabled"], ["#ro", "readonly"], ["#btn", "disabled"], ["#hid", "hidden"]]) {
            expect(q(sel).hasAttribute(name), `${sel}[${name}]`).toBe(false);
        }
        expect(q("#dis").disabled).toBe(false);
        expect(q("#chk").checked).toBe(false);
        expect(q("#chk").hasAttribute("checked")).toBe(false);
        expect(q("#hid").hidden).toBe(false);
        // never stringified to "false"
        expect(container.innerHTML).not.toContain('="false"');
    });

    it("applies a runtime true prop to attribute and property", () => {
        const {container} = mount(BoolProps, {on: true});
        expect(container.querySelector("#dis").hasAttribute("disabled")).toBe(true);
        expect(container.querySelector("#btn").disabled).toBe(true);
        // checked is DOM state: Solid sets the property
        expect(container.querySelector("#chk").checked).toBe(true);
    });

    it("applies a runtime false prop by removing the attribute", () => {
        const {container} = mount(BoolProps, {on: false});
        expect(container.querySelector("#dis").hasAttribute("disabled")).toBe(false);
        expect(container.querySelector("#btn").disabled).toBe(false);
        expect(container.querySelector("#chk").checked).toBe(false);
        // the consumed prop "on" must not leak onto the root
        expect(container.querySelector(".bp").getAttributeNames()).toEqual(["class"]);
    });

    it(".bool(name, true) sets an empty attribute; .bool(name, false) omits it", () => {
        const {container} = mount(BoolExt);
        expect(container.querySelector("#t").getAttribute("itemscope")).toBe("");
        expect(container.querySelector("#f").hasAttribute("itemscope")).toBe(false);
    });
});

describe("Dom/Elements style and class", () => {
    it("applies a StyleSpec list (typed properties and a custom property) as inline style", () => {
        const {root} = mount(StyleSpecEl);
        expect(root.style.backgroundColor).toBe("red");
        expect(root.style.display).toBe("flex");
        expect(root.style.getPropertyValue("--my-var")).toBe("12px");
        expect(root.className).toBe("styled");
    });

    it("applies a plain string style", () => {
        const {root} = mount(StyleString);
        expect(root.style.color).toBe("blue");
        expect(root.style.marginTop).toBe("4px");
    });

    it("applies a createObj style object", () => {
        const {root} = mount(StyleObj);
        expect(root.style.color).toBe("green");
        expect(root.style.paddingLeft).toBe("2px");
    });

    it("applies a class object map: truthy keys only", () => {
        const {root} = mount(ClassList);
        expect([...root.classList].sort()).toEqual(["also-on", "on"]);
    });
});

describe("Dom/Elements per-element attributes", () => {
    it("renders form, anchor, img, label, input, textarea and select attributes with browser semantics", () => {
        const {container} = mount(PerElement);
        const f = container.querySelector("form");
        expect(f.getAttribute("action")).toBe("/submit");
        expect(f.getAttribute("method")).toBe("post");
        expect(f.method).toBe("post");

        const a = container.querySelector("#lnk");
        expect(attrs(a)).toEqual({id: "lnk", href: "https://example.com/x?y=1", target: "_blank", rel: "noopener"});
        expect(a.href).toBe("https://example.com/x?y=1");
        expect(a.textContent).toBe("link");

        const img = container.querySelector("#pic");
        expect(img.getAttribute("src")).toBe("/pic.png");
        expect(img.alt).toBe("A picture");
        expect(img.getAttribute("width")).toBe("20");

        const lbl = container.querySelector("#lbl");
        expect(lbl.getAttribute("for")).toBe("name");
        expect(lbl.htmlFor).toBe("name");
        expect(lbl.control).toBe(container.querySelector("#name"));

        const inp = container.querySelector("#name");
        expect(inp.type).toBe("text");
        expect(inp.value).toBe("initial");
        expect(inp.placeholder).toBe("Your name");
        expect(inp.name).toBe("name");
        expect(inp.maxLength).toBe(10);

        const ta = container.querySelector("#ta");
        expect(ta.value).toBe("prefilled");
        expect(ta.placeholder).toBe("Type here");
        expect(ta.rows).toBe(3);
        expect(ta.cols).toBe(20);

        const sel = container.querySelector("#sel");
        expect([...sel.options].map(o => [o.value, o.textContent])).toEqual([["a", "Alpha"], ["b", "Beta"], ["c", "Gamma"]]);
        expect(sel.value).toBe("b");
        expect(sel.selectedIndex).toBe(1);
        expect(sel.options[2].disabled).toBe(true);
        // form ownership through the real DOM
        expect(inp.form).toBe(f);
        const data = new FormData(f);
        expect(data.get("pick")).toBe("b");
        expect(data.get("name")).toBe("initial");
    });

    it("innerHTML parses markup; textContent inserts literal text", () => {
        const {container} = mount(InnerHtml);
        const ih = container.querySelector("#ih");
        expect(ih.innerHTML).toBe("<em>raw</em><i>html</i>");
        expect(ih.children).toHaveLength(2);
        const tc = container.querySelector("#tc");
        expect(tc.children).toHaveLength(0);
        expect(tc.textContent).toBe("<em>not html</em>");
        // neither is left behind as an attribute
        expect(ih.getAttributeNames()).toEqual(["id"]);
        expect(tc.getAttributeNames()).toEqual(["id"]);
    });

    it("innerHTML from a prop becomes the element's markup", () => {
        const {root} = mount(HtmlProp, {html: "<b>bold</b> text"});
        expect(root.innerHTML).toBe("<b>bold</b> text");
        expect(root.querySelector("b").textContent).toBe("bold");
        expect(root.getAttributeNames()).toEqual(["id"]);
    });

    it("input value from a prop sets the value property", () => {
        const {root} = mount(ValueProp, {v: "hello"});
        expect(root.value).toBe("hello");
    });
});
