import {describe, it, expect, vi} from "vitest";
import {createSignal} from "solid-js";
import {mount, click, flush, act, text} from "../../helpers/index.js";
import {Greeting, Badge, FancyButton, Card, Divider, Pill, GreetingHost, CardHost} from "./A-Props.fs.jsx";

describe("Components: SolidTypeComponent props", () => {
    it("reads getter props reactively when the parent's signal changes", () => {
        const [name, setName] = createSignal("Ada");
        const [count, setCount] = createSignal(2);
        const {container} = mount(Greeting, {
            get name() {
                return name();
            },
            get count() {
                return count();
            }
        });
        const nameEl = container.querySelector(".name");
        expect(text(nameEl)).toBe("Ada");
        expect(text(container.querySelector(".count"))).toBe("2");
        expect(text(container.querySelector(".double"))).toBe("4");

        setName("Grace");
        setCount(5);
        // Solid 2: writes are batched until a flush.
        expect(text(nameEl)).toBe("Ada");
        flush();
        expect(text(nameEl)).toBe("Grace");
        expect(text(container.querySelector(".count"))).toBe("5");
        expect(text(container.querySelector(".double"))).toBe("10");
        // the component body did not re-run: same element instance was updated in place
        expect(container.querySelector(".name")).toBe(nameEl);
    });

    it("does not leak consumed props onto the root element (omit is not spread when unused)", () => {
        const {root} = mount(Greeting, {name: "x", count: 1});
        expect(root.outerHTML).toBe(
            '<div class="greeting"><span class="name">x</span><span class="count">1</span><span class="double">2</span></div>'
        );
    });

    it("applies default props via merge when the parent omits them", () => {
        const {root} = mount(Badge, {});
        expect(root.className).toBe("badge info");
        expect(text(root)).toBe("default-label");
    });

    it("lets explicit props override the defaults", () => {
        const {root} = mount(Badge, {label: "custom", tone: "warn"});
        expect(root.className).toBe("badge warn");
        expect(text(root)).toBe("custom");
    });

    it("keeps overridden defaults reactive", () => {
        const [tone, setTone] = createSignal("warn");
        const {root} = mount(Badge, {
            get tone() {
                return tone();
            }
        });
        expect(root.className).toBe("badge warn");
        expect(text(root)).toBe("default-label");
        act(() => setTone("danger"));
        expect(root.className).toBe("badge danger");
    });

    it("spreads the rest props onto the child element but not the consumed ones", () => {
        const onClick = vi.fn();
        const {root} = mount(FancyButton, {
            variant: "primary",
            id: "go",
            title: "Go!",
            "aria-label": "go button",
            "data-kind": "cta",
            onClick,
            children: "Go"
        });
        expect(root.tagName).toBe("BUTTON");
        expect(root.getAttribute("class")).toBe("fancy fancy-primary");
        expect(root.id).toBe("go");
        expect(root.getAttribute("title")).toBe("Go!");
        expect(root.getAttribute("aria-label")).toBe("go button");
        expect(root.getAttribute("data-kind")).toBe("cta");
        expect(root.hasAttribute("variant")).toBe(false);
        expect(root.hasAttribute("n$")).toBe(false);
        expect(text(root)).toBe("Go");
        click(root);
        expect(onClick).toHaveBeenCalledTimes(1);
    });

    it("keeps spread rest props reactive", () => {
        const [title, setTitle] = createSignal("one");
        const [variant, setVariant] = createSignal("a");
        const {root} = mount(FancyButton, {
            get variant() {
                return variant();
            },
            get title() {
                return title();
            }
        });
        expect(root.getAttribute("title")).toBe("one");
        expect(root.getAttribute("class")).toBe("fancy fancy-a");
        act(() => {
            setTitle("two");
            setVariant("b");
        });
        expect(root.getAttribute("title")).toBe("two");
        expect(root.getAttribute("class")).toBe("fancy fancy-b");
    });

    it("renders the children prop inside the wrapper", () => {
        const child = document.createElement("em");
        child.textContent = "inner";
        const {root} = mount(Card, {heading: "Hello", children: child});
        expect(text(root.querySelector(".card-title"))).toBe("Hello");
        expect(root.querySelector(".card-body").firstChild).toBe(child);
    });

    it("renders a reactive children getter", () => {
        const [label, setLabel] = createSignal("first");
        const {root} = mount(Card, {
            heading: "H",
            get children() {
                return label();
            }
        });
        expect(text(root.querySelector(".card-body"))).toBe("first");
        act(() => setLabel("second"));
        expect(text(root.querySelector(".card-body"))).toBe("second");
    });
});

describe("Components: SolidComponent let-bindings", () => {
    it("renders a zero-argument component", () => {
        const {root} = mount(Divider);
        expect(root.outerHTML).toBe('<hr class="divider">');
    });

    it("renders a curried-argument component called as a function", () => {
        const {root} = mount(() => Pill("hi", "ok"), undefined, {thunk: true});
        expect(root.outerHTML).toBe('<span class="pill pill-ok">hi</span>');
    });
});

describe("Components: parent -> child prop flow (real usage)", () => {
    it("propagates the parent's signals into the child through props", () => {
        const {container} = mount(GreetingHost);
        const nameEl = container.querySelector(".name");
        expect(text(nameEl)).toBe("Ada");
        expect(text(container.querySelector(".count"))).toBe("1");
        click(container.querySelector(".rename"));
        expect(text(nameEl)).toBe("Grace");
        click(container.querySelector(".bump"));
        click(container.querySelector(".bump"));
        expect(text(container.querySelector(".count"))).toBe("3");
        expect(text(container.querySelector(".double"))).toBe("6");
        expect(container.querySelector(".name")).toBe(nameEl);
        expect(container.querySelector("hr.divider")).not.toBeNull();
        expect(container.querySelector(".pill.pill-ok").textContent).toBe("static");
    });

    it("closes the event -> signal -> DOM loop across a composed card and spread button", () => {
        const {container} = mount(CardHost);
        const btn = container.querySelector("#fb");
        expect(btn.getAttribute("class")).toBe("fancy fancy-primary");
        expect(btn.getAttribute("title")).toBe("clicked 0 times");
        expect(text(container.querySelector(".card-title"))).toBe("Clicks: 0");
        click(btn);
        click(btn);
        expect(btn.getAttribute("title")).toBe("clicked 2 times");
        expect(text(container.querySelector(".card-title"))).toBe("Clicks: 2");
        expect(text(container.querySelector(".echo"))).toBe("2");
        expect(text(btn)).toBe("Click me");
        // button node survived the updates
        expect(container.querySelector("#fb")).toBe(btn);
    });
});
