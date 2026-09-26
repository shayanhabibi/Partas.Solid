import {describe, it, expect} from "vitest";
import {createSignal} from "solid-js";
import {mount, click, flush, text} from "../../helpers/index.js";
import {
    DynamicComponent, DynamicTagSwitch, DynamicFromProp, Badge, DynamicMaybe, DynamicButton, DynamicSvg, Heading
} from "./B-Dynamic.fs.jsx";

describe("Dom/Portals Dynamic (component' field)", () => {
    it("renders a Partas component with props passed through spread", () => {
        const {container} = mount(DynamicComponent);
        const badge = container.querySelector("span.badge");
        expect(badge).not.toBeNull();
        expect(badge.textContent).toBe("new");
        expect(container.innerHTML).toBe('<span class="badge">new</span>');
    });

    it("switches intrinsic tags when the component signal changes", () => {
        const {root} = mount(DynamicTagSwitch);
        expect(root.querySelector("h1")).not.toBeNull();
        click(root.querySelector("#next-level"));
        expect(root.querySelector("h1")).toBeNull();
        expect(root.querySelector("h2")).not.toBeNull();
        click(root.querySelector("#next-level"));
        expect(root.querySelector("h3")).not.toBeNull();
        // exactly one dynamic element next to the button
        expect([...root.children].map(c => c.tagName)).toEqual(["BUTTON", "H3"]);
    });

    it("switches between a tag name and a component taken from a reactive prop", () => {
        const [comp, setComp] = createSignal("em");
        const {root} = mount(DynamicFromProp, {get comp() { return comp(); }});
        const em = root.querySelector("em");
        expect(em).not.toBeNull();
        // an intrinsic tag receives the forwarded prop as an attribute, and nothing else
        expect(em.getAttribute("label")).toBe("hi");
        expect(em.getAttributeNames()).toEqual(["label"]);
        setComp(() => Badge);
        flush();
        expect(root.querySelector("em")).toBeNull();
        expect(root.innerHTML).toBe('<span class="badge">hi</span>');
        setComp("em");
        flush();
        expect(root.querySelector("span.badge")).toBeNull();
        expect(root.querySelector("em")).not.toBeNull();
        // the consumed prop is not leaked onto the host
        expect(root.getAttributeNames()).toEqual(["class"]);
    });

    it("a falsy component renders nothing; setting it renders the tag; clearing removes it", () => {
        const {root} = mount(DynamicMaybe);
        expect([...root.children].map(c => c.tagName)).toEqual(["BUTTON", "BUTTON"]);
        click(root.querySelector("#set-tag"));
        expect(root.querySelector("article")).not.toBeNull();
        click(root.querySelector("#unset-tag"));
        expect(root.querySelector("article")).toBeNull();
    });

    it("forwards event handlers, attributes and children to an intrinsic element", () => {
        let presses = 0;
        const {root} = mount(DynamicButton, {onPress: () => presses++});
        const btn = root.querySelector("#dyn-btn");
        expect(btn).not.toBeNull();
        expect(btn.tagName).toBe("BUTTON");
        expect(btn.getAttribute("type")).toBe("button");
        expect(btn.textContent).toBe("press");
        click(btn);
        click(btn);
        expect(presses).toBe(2);
    });

    it("creates SVG-namespaced elements for SVG tag names", () => {
        const {root} = mount(DynamicSvg);
        expect(root.namespaceURI).toBe("http://www.w3.org/2000/svg");
        const circle = root.querySelector("circle");
        expect(circle).not.toBeNull();
        expect(circle.namespaceURI).toBe("http://www.w3.org/2000/svg");
        expect(circle.getAttribute("r")).toBe("5");
    });

    it("polymorphic heading picks its tag from a prop", () => {
        const {container} = mount(Heading, {level: 3, text: "Section"});
        expect(container.innerHTML).toBe('<h3 class="heading">Section</h3>');
        const {container: c2} = mount(Heading, {level: 5, text: "Deep"});
        expect(c2.firstElementChild.tagName).toBe("H5");
    });

    it("polymorphic heading follows a reactive level prop", () => {
        const [level, setLevel] = createSignal(1);
        const {container} = mount(Heading, {
            get level() {
                return level();
            }, text: "T"
        });
        expect(container.firstElementChild.tagName).toBe("H1");
        setLevel(4);
        flush();
        expect(container.firstElementChild.tagName).toBe("H4");
        expect(text(container)).toBe("T");
    });
});
