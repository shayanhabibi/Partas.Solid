import {describe, it, expect} from "vitest";
import {createSignal} from "solid-js";
import {mount, click, input, flush, text} from "../../helpers/index.js";
import {
    PortalLabel, PortalCounter, PortalBubble, Modal, PortalCleanup, TwoPortals, SwitchMount,
    PortalList, NestedPortals, PortalManyChildren, PortalRef, SiblingLayer, PortalAttrs,
    PortalExplicitBody, PortalInput
} from "./A-Portal.fs.jsx";

/** A fresh mount node appended to body (removed by the global afterEach body reset). */
function makeTarget(id = "target") {
    const el = document.createElement("div");
    el.id = id;
    document.body.appendChild(el);
    return el;
}

describe("Dom/Portals Portal: placement", () => {
    it("renders portal content into the mount node and nothing but a marker in place", () => {
        const target = makeTarget();
        const {root} = mount(PortalLabel, {target, label: "hello"});
        expect(root.className).toBe("pl-host");
        expect(root.children.length).toBe(0);
        expect(root.textContent).toBe("");
        const content = target.querySelector(".pl-content");
        expect(content).not.toBeNull();
        expect(content.textContent).toBe("hello");
        expect(content.title).toBe("hello");
    });

    it("does not leak the mount prop onto any element", () => {
        const target = makeTarget();
        const {root} = mount(PortalLabel, {target, label: "x"});
        expect(root.hasAttribute("mount")).toBe(false);
        expect(root.hasAttribute("target")).toBe(false);
        expect(target.querySelector("[mount]")).toBeNull();
    });

    it("renders several children in source order", () => {
        const target = makeTarget();
        mount(PortalManyChildren, {target});
        expect([...target.children].map(c => c.className)).toEqual(["c1", "c2", "c3"]);
        expect(text(target)).toBe("abc");
    });

    it("mount = document.body explicitly renders into body", () => {
        const {root, container} = mount(PortalExplicitBody);
        const aside = document.getElementById("eb-aside");
        expect(aside).not.toBeNull();
        expect(aside.parentNode).toBe(document.body);
        expect(container.contains(aside)).toBe(false);
        expect(root.children.length).toBe(0);
    });

    it("two portals into the same node keep source order", () => {
        const target = makeTarget();
        mount(TwoPortals, {target, showFirst: true});
        expect([...target.querySelectorAll("p")].map(p => p.className)).toEqual(["first", "second"]);
    });

    it("nested portals put each level in its own mount node", () => {
        const outer = makeTarget("outer");
        const inner = makeTarget("inner");
        const {root} = mount(NestedPortals, {outer, inner});
        const lvl1 = outer.querySelector(".lvl1");
        expect(lvl1).not.toBeNull();
        expect(text(lvl1)).toBe("level 1");
        expect(lvl1.querySelector(".lvl2")).toBeNull();
        expect(inner.querySelector(".lvl2").textContent).toBe("level 2");
        expect(root.textContent).toBe("");
    });

    it("a ref inside the portal receives the element living in the mount node", () => {
        const target = makeTarget();
        let got;
        mount(PortalRef, {target, got: el => (got = el)});
        expect(got).toBeDefined();
        expect(got.id).toBe("portal-input");
        expect(got.parentNode).toBe(target);
    });
});

describe("Dom/Portals Portal: reactivity", () => {
    it("a reactive prop updates the portalled text and attribute", () => {
        const target = makeTarget();
        const [label, setLabel] = createSignal("first");
        mount(PortalLabel, {
            target, get label() {
                return label();
            }
        });
        const content = target.querySelector(".pl-content");
        expect(content.textContent).toBe("first");
        setLabel("second");
        flush();
        expect(content.textContent).toBe("second");
        expect(content.title).toBe("second");
        // updated in place, not re-created
        expect(target.querySelector(".pl-content")).toBe(content);
    });

    it("a signal owned outside the portal drives content inside it", () => {
        const target = makeTarget();
        const {root} = mount(PortalCounter, {target});
        click(root.querySelector("#pc-out"));
        click(root.querySelector("#pc-out"));
        expect(target.querySelector("#pc-value").textContent).toBe("2");
        expect(root.querySelector("#pc-mirror").textContent).toBe("2");
    });

    it("a click on a button inside the portal updates state rendered outside it", () => {
        const target = makeTarget();
        const {root} = mount(PortalCounter, {target});
        click(target.querySelector("#pc-in"));
        expect(root.querySelector("#pc-mirror").textContent).toBe("10");
        expect(target.querySelector("#pc-value").textContent).toBe("10");
    });

    it("reactive class and attribute on portal content toggle", () => {
        const target = makeTarget();
        const {root} = mount(PortalAttrs, {target});
        const box = target.querySelector("#pa-box");
        expect(box.className).toBe("box");
        expect(box.title).toBe("no");
        expect(box.textContent).toBe("off");
        click(root.querySelector("#toggle-active"));
        expect(box.className).toBe("box active");
        expect(box.title).toBe("yes");
        expect(box.textContent).toBe("on");
    });

    it("a For list inside the portal grows and clears", () => {
        const target = makeTarget();
        const {root} = mount(PortalList, {target});
        const list = () => [...target.querySelectorAll(".portal-list li")].map(li => li.textContent);
        expect(list()).toEqual(["one"]);
        click(root.querySelector("#add"));
        click(root.querySelector("#add"));
        expect(list()).toEqual(["one", "item2", "item3"]);
        click(root.querySelector("#clear"));
        expect(list()).toEqual([]);
        expect(target.querySelector(".portal-list")).not.toBeNull();
    });

    it("an input inside a body portal feeds a signal displayed outside", () => {
        const {root} = mount(PortalInput);
        const field = document.getElementById("pi-in");
        expect(root.contains(field)).toBe(false);
        input(field, "typed");
        expect(root.querySelector("#pi-out").textContent).toBe("typed");
    });

    it("a reactive mount moves the content between nodes", () => {
        const a = makeTarget("a");
        const b = makeTarget("b");
        const [useB, setUseB] = createSignal(false);
        mount(SwitchMount, {
            a, b, get useB() {
                return useB();
            }
        });
        expect(a.querySelector(".moving")).not.toBeNull();
        expect(b.querySelector(".moving")).toBeNull();
        setUseB(true);
        flush();
        expect(a.querySelector(".moving")).toBeNull();
        expect(b.querySelector(".moving")).not.toBeNull();
        setUseB(false);
        flush();
        expect(a.querySelectorAll(".moving").length).toBe(1);
        expect(b.querySelector(".moving")).toBeNull();
    });

    it("portal into a sibling layer captured by ref", () => {
        const {root} = mount(SiblingLayer);
        flush();
        const layer = root.querySelector(".layer");
        expect(layer).not.toBeNull();
        const tip = layer.querySelector(".tip");
        expect(tip).not.toBeNull();
        expect(tip.textContent).toBe("tooltip");
    });
});

describe("Dom/Portals Portal: events", () => {
    it("delegated clicks inside the portal run the handler of the portalled element", () => {
        const target = makeTarget();
        const log = [];
        mount(PortalBubble, {target, log: n => log.push(n)});
        click(target.querySelector("#pb-btn"));
        expect(log[0]).toBe("btn");
        expect(log).toContain("wrap");
    });

    it("delegated clicks bubble from portal content to component-tree ancestors", () => {
        const target = makeTarget();
        const log = [];
        mount(PortalBubble, {target, log: n => log.push(n)});
        click(target.querySelector("#pb-btn"));
        expect(log).toEqual(["btn", "wrap", "outer"]);
    });

    it("stopPropagation inside the portal stops the logical bubbling", () => {
        const target = makeTarget();
        const log = [];
        mount(PortalBubble, {target, log: n => log.push(n)});
        click(target.querySelector("#pb-stop"));
        expect(log).toEqual(["stop"]);
    });

    it("clicks on the mount node itself do not reach the component-tree ancestors", () => {
        const target = makeTarget();
        const log = [];
        mount(PortalBubble, {target, log: n => log.push(n)});
        click(target);
        expect(log).toEqual([]);
    });
});

describe("Dom/Portals Portal: lifecycle", () => {
    it("modal opens into the mount node and closes from inside the portal", () => {
        const target = makeTarget();
        const {root} = mount(Modal, {target});
        expect(target.querySelector(".dialog")).toBeNull();
        expect(root.querySelector("#state").textContent).toBe("closed");
        click(root.querySelector("#open"));
        const dialog = target.querySelector(".dialog");
        expect(dialog).not.toBeNull();
        expect(dialog.getAttribute("role")).toBe("dialog");
        expect(dialog.querySelector("h2").textContent).toBe("Dialog title");
        expect(root.querySelector("#state").textContent).toBe("open");
        click(dialog.querySelector("#close"));
        expect(target.querySelector(".dialog")).toBeNull();
        expect(root.querySelector("#state").textContent).toBe("closed");
    });

    it("the modal can be reopened after closing, with a single dialog", () => {
        const target = makeTarget();
        const {root} = mount(Modal, {target});
        click(root.querySelector("#open"));
        click(target.querySelector("#close"));
        click(root.querySelector("#open"));
        expect(target.querySelectorAll(".dialog").length).toBe(1);
    });

    it("hiding the owning Show disposes portal content and runs its cleanup", () => {
        const target = makeTarget();
        const disposed = [];
        const [visible, setVisible] = createSignal(true);
        mount(PortalCleanup, {
            target, get visible() {
                return visible();
            }, onDispose: n => disposed.push(n)
        });
        expect(target.querySelector(".tracked").textContent).toBe("inner");
        setVisible(false);
        flush();
        expect(target.querySelector(".tracked")).toBeNull();
        expect(disposed).toEqual(["inner"]);
    });

    it("disposing the root disposes portal content, runs its cleanup and empties the mount node", () => {
        const target = makeTarget();
        const disposed = [];
        const {dispose} = mount(PortalCleanup, {target, visible: true, onDispose: n => disposed.push(n)});
        expect(target.childNodes.length).toBeGreaterThan(0);
        dispose();
        expect(disposed).toEqual(["inner"]);
        expect(target.childNodes.length).toBe(0);
    });

    it("removing one of two portals leaves the other in the shared node", () => {
        const target = makeTarget();
        const [showFirst, setShowFirst] = createSignal(true);
        mount(TwoPortals, {
            target, get showFirst() {
                return showFirst();
            }
        });
        setShowFirst(false);
        flush();
        expect([...target.querySelectorAll("p")].map(p => p.className)).toEqual(["second"]);
        setShowFirst(true);
        flush();
        expect(target.querySelectorAll("p.first").length).toBe(1);
        expect(target.querySelectorAll("p.second").length).toBe(1);
    });

    it("disposing a body portal removes its content from body", () => {
        const {dispose} = mount(PortalInput);
        expect(document.getElementById("pi-in")).not.toBeNull();
        dispose();
        expect(document.getElementById("pi-in")).toBeNull();
    });
});
