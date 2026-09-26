import {describe, it, expect} from "vitest";
import {mount, text} from "../../helpers/index.js";
import {
    Nested, Numbers, Interpolated, Frag, Voids, EscapedText, Deep, Table, Greeting, Composed,
    BraceText, InlineSpacing, InterpolatedMarkup, MultilineText
} from "./A-Structure.fs.jsx";

describe("Dom/Elements structure: tags, text, nesting", () => {
    it("renders nested tags with mixed text and element children in order", () => {
        const {container, root} = mount(Nested);
        expect(root.tagName).toBe("ARTICLE");
        expect(root.id).toBe("post");
        expect([...root.children].map(c => c.tagName)).toEqual(["H1", "P", "UL"]);
        expect(root.querySelector("h1").textContent).toBe("Title");
        const p = root.querySelector("p.lead");
        // text, element, text: exactly three child nodes, whitespace from F# layout trimmed away
        expect([...p.childNodes].map(n => n.nodeType)).toEqual([3, 1, 3]);
        expect(p.innerHTML).toBe("Hello <strong>world</strong>!");
        expect([...root.querySelectorAll("li")].map(li => li.textContent)).toEqual(["one", "two", "three"]);
        expect(container.children).toHaveLength(1);
    });

    it("keeps the trailing space of a string child that precedes an element", () => {
        const {root} = mount(InlineSpacing);
        expect(root.firstChild.nodeValue).toBe("Hello ");
    });

    // BUG: a string child with a leading space (" again") is emitted as raw JSX text on its own
    // line, and JSX line-trimming drops the space.
    it.fails("keeps the leading space of a string child that follows an element", () => {
        const {root} = mount(InlineSpacing);
        expect(root.textContent).toBe("Hello world again");
    });

    it("renders int, float and negative children as text", () => {
        const {root} = mount(Numbers);
        expect(root.querySelector(".i").textContent).toBe("42");
        expect(root.querySelector(".f").textContent).toBe("3.5");
        expect(root.querySelector(".neg").textContent).toBe("-7");
    });

    it("renders an interpolated string child", () => {
        const {root} = mount(Interpolated);
        expect(root.textContent).toBe("Hi Partas, you have 3 items");
    });

    it("renders a Fragment root as siblings without a wrapper element", () => {
        const {container} = mount(Frag);
        expect(container.innerHTML).toBe('<span class="a">A</span><span class="b">B</span>tail');
        expect(container.childNodes).toHaveLength(3);
    });

    it("renders void elements with no children and an empty non-void element", () => {
        const {root} = mount(Voids);
        expect([...root.children].map(c => c.tagName)).toEqual(["INPUT", "BR", "HR", "IMG", "DIV"]);
        expect(root.innerHTML).toBe('<input><br><hr><img><div class="empty"></div>');
    });

    // BUG: string children are emitted raw into JSX text, so "<b>" becomes a real element.
    it.fails("escapes HTML-significant characters in a string child (text node, not markup)", () => {
        const {root} = mount(EscapedText);
        expect(root.querySelector("b")).toBeNull();
        expect(root.textContent).toBe('<b>not bold</b> & "quoted"');
    });

    // BUG: string children are emitted raw into JSX text, so "{1 + 1}" becomes a JSX expression.
    it.fails("renders braces in a string child literally", () => {
        const {root} = mount(BraceText);
        expect(root.textContent).toBe("a {1 + 1} b");
    });

    it("renders deeply nested elements", () => {
        const {root} = mount(Deep);
        expect(root.tagName).toBe("SECTION");
        const span = root.querySelector(".l1 > .l2 > .l3 > .l4 > span");
        expect(span).not.toBeNull();
        expect(span.textContent).toBe("deep");
    });

    it("renders a table keeping thead/tbody structure", () => {
        const {root} = mount(Table);
        expect(root.tHead.rows).toHaveLength(1);
        expect(root.tBodies).toHaveLength(1);
        const cells = [...root.tBodies[0].rows].map(r => [...r.cells].map(c => c.textContent));
        expect(cells).toEqual([["a", "1"], ["b", "2"]]);
    });

    it("renders a type component with props as text children", () => {
        const {root} = mount(Greeting, {name: "Zed", count: 9});
        expect(root.outerHTML).toBe('<div class="greeting"><span class="name">Zed</span><span class="count">9</span></div>');
    });

    it("composes type components as child tags, each with its own props", () => {
        const {root} = mount(Composed);
        expect(root.tagName).toBe("MAIN");
        const names = [...root.querySelectorAll(".greeting .name")].map(e => e.textContent);
        const counts = [...root.querySelectorAll(".greeting .count")].map(e => e.textContent);
        expect(names).toEqual(["Ann", "Bob"]);
        expect(counts).toEqual(["1", "2"]);
        // consumed props must not appear as attributes on the component's root
        for (const g of root.querySelectorAll(".greeting")) {
            expect(g.getAttributeNames()).toEqual(["class"]);
        }
    });

    it("keeps markup characters of an interpolated string as literal text", () => {
        // Interpolation is emitted as a JS expression child, so this is the working counterpart of
        // the raw-string-child escaping bug above.
        const {root} = mount(InterpolatedMarkup);
        expect(root.querySelector("i")).toBeNull();
        expect(root.childNodes).toHaveLength(1);
        expect(root.textContent).toBe("<i>you</i> & {x}");
    });

    // BUG: a string child containing a newline is emitted as raw multi-line JSX text; JSX whitespace
    // rules strip the second line's indentation and collapse the newline, even inside <pre>.
    it.fails("preserves newlines and indentation of a string child inside <pre>", () => {
        const {root} = mount(MultilineText);
        expect(root.textContent).toBe("line one\n  line two");
    });
});
