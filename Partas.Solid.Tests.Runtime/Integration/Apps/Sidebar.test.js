import {describe, it, expect, vi, beforeAll, afterAll} from "vitest";
import {mount, click, text} from "../../helpers/index.js";
import {createComponent} from "@solidjs/web";
import {
    SidebarApp,
    SidebarProvider,
    Sidebar,
    SidebarMenuButton,
    OrphanTrigger,
    menuButtonVariants,
    cn,
    Context_SidebarContext
} from "./D-Sidebar.fs.jsx";

const $ = (root, sel) => root.querySelector(sel);
const $$ = (root, sel) => [...root.querySelectorAll(sel)];

describe("Apps: Kobalte-style sidebar - context-free pieces", () => {
    it("cn drops empty and undefined classes", () => {
        expect(cn(["a", "", undefined, "b"])).toBe("a b");
    });

    it("menuButtonVariants resolves every variant/size combination", () => {
        expect(menuButtonVariants("sidebar", "md")).toBe("menu-button mb-default h-8 text-sm");
        expect(menuButtonVariants("floating", "sm")).toBe("menu-button mb-floating h-7 text-xs");
        expect(menuButtonVariants("inset", "lg")).toBe("menu-button mb-inset h-12 text-base");
    });

    it("SidebarMenuButton applies merged defaults, variant classes and data-active", () => {
        const {container} = mount(SidebarMenuButton, {children: "Home"});
        const b = $(container, "button");
        expect(b.className).toBe("menu-button mb-default h-8 text-sm");
        expect(b.getAttribute("data-active")).toBe("false");
        expect(text(b)).toBe("Home");
    });

    it("SidebarMenuButton appends a user class and spreads unknown props onto the button", () => {
        const onClick = vi.fn();
        const {container} = mount(SidebarMenuButton, {
            variant: "floating",
            size: "lg",
            isActive: true,
            class: "mine",
            id: "btn",
            title: "tip",
            "data-item": "x",
            onClick,
            children: "Go"
        });
        const b = $(container, "button");
        expect(b.className).toBe("menu-button mb-floating h-12 text-base mine");
        expect(b.getAttribute("data-active")).toBe("true");
        expect(b.id).toBe("btn");
        expect(b.getAttribute("title")).toBe("tip");
        expect(b.getAttribute("data-item")).toBe("x");
        // Omitted component props must not leak as attributes.
        expect(b.hasAttribute("variant")).toBe(false);
        expect(b.hasAttribute("isActive")).toBe(false);
        expect(b.hasAttribute("size")).toBe(false);
        click(b);
        expect(onClick).toHaveBeenCalledTimes(1);
    });

    it("SidebarMenuButton reacts to a getter-backed isActive prop", async () => {
        const {createSignal} = await import("solid-js");
        const {act} = await import("../../helpers/index.js");
        const [active, setActive] = createSignal(false);
        const {container} = mount(SidebarMenuButton, {
            get isActive() {
                return active();
            },
            children: "x"
        });
        const b = $(container, "button");
        expect(b.getAttribute("data-active")).toBe("false");
        act(() => setActive(true));
        expect(b.getAttribute("data-active")).toBe("true");
    });

    it("a consumer with no provider throws (default-less context)", () => {
        // ContextNotFoundError is not exported by solid-js rc.9; match its dev-mode message.
        expect(() => mount(OrphanTrigger)).toThrow(/Context must either be created with a default value/);
    });
});

// All tests below need <SidebarProvider>, which is emitted as <Context_SidebarContext.Provider value=...>.
describe("Apps: Kobalte-style sidebar - provider + consumers", () => {
    // BUG: provider emitted as <Ctx.Provider value=...>; Solid 2 contexts are themselves the provider.
    it.fails("SidebarProvider renders its wrapper with the default open state", () => {
        const {container: c} = mount(SidebarProvider, {class: "x", children: "body"});
        const wrapper = $(c, ".sidebar-wrapper");
        expect(wrapper.className).toBe("sidebar-wrapper x");
        expect(wrapper.dataset.state).toBe("expanded");
        expect(text(wrapper)).toBe("body");
    });

    // BUG: provider emitted as <Ctx.Provider value=...>; undefined component in Solid 2.
    it.fails("renders the provider wrapper, sidebar and menu with context-derived state", () => {
        const {container: c} = mount(SidebarApp, {
            startOpen: true, look: "floating", onToggle: () => {
            }
        });
        const wrapper = $(c, ".sidebar-wrapper");
        expect(wrapper.className).toBe("sidebar-wrapper app");
        expect(wrapper.dataset.state).toBe("expanded");
        const sb = $(c, "#main-sidebar");
        expect(sb.className).toBe("sidebar right raised");
        expect(sb.dataset.variant).toBe("floating");
        expect(sb.dataset.collapsible).toBe("");
        expect($$(c, ".sidebar-inner button.menu-button").map(b => b.dataset.item)).toEqual(["home", "inbox", "settings"]);
        expect($(c, '[data-item="settings"]').className).toBe("menu-button mb-default h-7 text-xs");
    });

    // BUG: provider emitted as <Ctx.Provider value=...>; undefined component in Solid 2.
    it.fails("the trigger toggles shared open state and reports each change", () => {
        const onToggle = vi.fn();
        const {container: c} = mount(SidebarApp, {startOpen: true, look: "sidebar", onToggle});
        const trigger = $(c, "button.sidebar-trigger");
        expect(trigger.className).toBe("sidebar-trigger extra");
        click(trigger);
        expect($(c, ".sidebar-wrapper").dataset.state).toBe("collapsed");
        expect($(c, "#main-sidebar").dataset.state).toBe("collapsed");
        expect($(c, "#main-sidebar").dataset.collapsible).toBe("icon");
        click(trigger);
        expect($(c, ".sidebar-wrapper").dataset.state).toBe("expanded");
        expect(onToggle.mock.calls).toEqual([[false], [true]]);
    });

    // BUG: provider emitted as <Ctx.Provider value=...>; undefined component in Solid 2.
    it.fails("menu buttons drive the active item through a spread onClick", () => {
        const {container: c} = mount(SidebarApp, {
            startOpen: false, look: "inset", onToggle: () => {
            }
        });
        expect(text($(c, ".active-item"))).toBe("home");
        expect($(c, '[data-item="home"]').dataset.active).toBe("true");
        click($(c, '[data-item="inbox"]'));
        expect(text($(c, ".active-item"))).toBe("inbox");
        expect($$(c, "[data-item]").map(b => b.dataset.active)).toEqual(["false", "true", "false"]);
    });
});

// Control group: the same scenarios with the one missing piece supplied at runtime
// (`Ctx.Provider = Ctx`, which is what Solid 2's context object already is). Everything below
// passing proves the fixture is sound and that `.Provider` is the only defect behind the it.fails
// above. When the plugin is fixed, the shim is a harmless no-op.
describe("Apps: Kobalte-style sidebar - with a runtime .Provider shim", () => {
    let hadProvider;
    beforeAll(() => {
        hadProvider = Object.prototype.hasOwnProperty.call(Context_SidebarContext, "Provider");
        if (!hadProvider) Context_SidebarContext.Provider = Context_SidebarContext;
    });
    afterAll(() => {
        if (!hadProvider) delete Context_SidebarContext.Provider;
    });

    it("renders the wrapper, sidebar and menu with context-derived state", () => {
        const {container: c} = mount(SidebarApp, {startOpen: true, look: "floating", onToggle: () => {}});
        const wrapper = $(c, ".sidebar-wrapper");
        expect(wrapper.className).toBe("sidebar-wrapper app");
        expect(wrapper.dataset.state).toBe("expanded");
        const sb = $(c, "#main-sidebar");
        expect(sb.className).toBe("sidebar right raised");
        expect(sb.dataset.state).toBe("expanded");
        expect(sb.dataset.variant).toBe("floating");
        expect(sb.dataset.collapsible).toBe("");
        // Omitted component props must not leak onto the DOM through the spread.
        expect(sb.hasAttribute("side")).toBe(false);
        expect(sb.hasAttribute("collapsible")).toBe(false);
        expect($$(c, ".sidebar-inner button.menu-button").map(b => b.dataset.item)).toEqual(["home", "inbox", "settings"]);
        expect($(c, '[data-item="settings"]').className).toBe("menu-button mb-default h-7 text-xs");
        expect(wrapper.hasAttribute("defaultOpen")).toBe(false);
    });

    it("startOpen=false starts collapsed and exposes the collapsible mode", () => {
        const {container: c} = mount(SidebarApp, {startOpen: false, look: "sidebar", onToggle: () => {}});
        expect($(c, ".sidebar-wrapper").dataset.state).toBe("collapsed");
        expect($(c, "#main-sidebar").dataset.state).toBe("collapsed");
        expect($(c, "#main-sidebar").dataset.collapsible).toBe("icon");
        expect($(c, "#main-sidebar").className).toBe("sidebar right flat");
    });

    it("the trigger toggles shared open state and reports each change", () => {
        const onToggle = vi.fn();
        const {container: c} = mount(SidebarApp, {startOpen: true, look: "sidebar", onToggle});
        const trigger = $(c, "button.sidebar-trigger");
        expect(trigger.className).toBe("sidebar-trigger extra");
        click(trigger);
        expect($(c, ".sidebar-wrapper").dataset.state).toBe("collapsed");
        expect($(c, "#main-sidebar").dataset.state).toBe("collapsed");
        expect($(c, "#main-sidebar").dataset.collapsible).toBe("icon");
        click(trigger);
        expect($(c, ".sidebar-wrapper").dataset.state).toBe("expanded");
        expect($(c, "#main-sidebar").dataset.collapsible).toBe("");
        expect(onToggle.mock.calls).toEqual([[false], [true]]);
    });

    it("menu buttons drive the active item through a spread onClick", () => {
        const {container: c} = mount(SidebarApp, {startOpen: false, look: "inset", onToggle: () => {}});
        expect(text($(c, ".active-item"))).toBe("home");
        expect($$(c, "[data-item]").map(b => b.dataset.active)).toEqual(["true", "false", "false"]);
        click($(c, '[data-item="inbox"]'));
        expect(text($(c, ".active-item"))).toBe("inbox");
        expect($$(c, "[data-item]").map(b => b.dataset.active)).toEqual(["false", "true", "false"]);
    });

    it("collapsible='none' takes the static Match branch", () => {
        const {container: c} = mount(SidebarProvider, {
            get children() {
                return createComponent(Sidebar, {collapsible: "none", class: "mine", id: "s", children: "static body"});
            }
        });
        const sb = $(c, "#s");
        expect(sb.className).toBe("sidebar static mine");
        expect(sb.querySelector(".sidebar-inner")).toBeNull();
        expect(text(sb)).toBe("static body");
        expect(sb.hasAttribute("data-state")).toBe(false);
    });

    it("Sidebar defaults (left, flat, offcanvas) come from merge when no props are given", () => {
        const {container: c} = mount(SidebarProvider, {
            defaultOpen: false,
            get children() {
                return createComponent(Sidebar, {id: "s"});
            }
        });
        const sb = $(c, "#s");
        expect(sb.className).toBe("sidebar left flat");
        expect(sb.dataset.variant).toBe("sidebar");
        expect(sb.dataset.collapsible).toBe("offcanvas");
    });

    // BUG: ariaExpanded is emitted verbatim as `ariaExpanded=` instead of `aria-expanded=`.
    it.fails("the trigger reflects the open state in aria-expanded", () => {
        const {container: c} = mount(SidebarApp, {startOpen: true, look: "sidebar", onToggle: () => {}});
        const trigger = $(c, "button.sidebar-trigger");
        expect(trigger.getAttribute("aria-expanded")).toBe("true");
        click(trigger);
        expect(trigger.getAttribute("aria-expanded")).toBe("false");
    });
});
