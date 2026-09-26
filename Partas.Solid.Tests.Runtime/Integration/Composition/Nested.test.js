import {describe, it, expect, vi} from "vitest";
import {createSignal} from "solid-js";
import {mount, click, input, act, text} from "../../helpers/index.js";
import {
    OuterLayer, BaseButton, DangerButton, ToolbarButton, Profile, Status, Nick, KindLabel, KindIcon,
    ThreeLevelHost, ToolbarHost, ProfileHost
} from "./D-Nested.fs.jsx";

describe("Composition: reactive prop forwarding across three levels", () => {
    it("renders the value decorated by the middle layer", () => {
        const onRender = vi.fn();
        const {container} = mount(() => ThreeLevelHost(onRender), undefined, {thunk: true});
        expect(text(container.querySelector("section.outer > div.middle > span.inner"))).toBe("[a]");
        expect(onRender.mock.calls.map(c => c[0])).toEqual(["outer", "middle", "inner"]);
    });

    it("propagates input changes to the leaf without re-running any component body", () => {
        const onRender = vi.fn();
        const {container} = mount(() => ThreeLevelHost(onRender), undefined, {thunk: true});
        const inner = container.querySelector(".inner");
        input(container.querySelector(".src"), "hello");
        expect(text(inner)).toBe("[hello]");
        input(container.querySelector(".src"), "");
        expect(text(inner)).toBe("[]");
        expect(onRender).toHaveBeenCalledTimes(3);
        expect(container.querySelector(".inner")).toBe(inner);
    });

    it("forwards a JS getter prop through all three levels", () => {
        const [v, setV] = createSignal("x");
        const {container} = mount(OuterLayer, {
            get value() {
                return v();
            },
            onRender: () => {
            }
        });
        expect(text(container.querySelector(".inner"))).toBe("[x]");
        act(() => setV("y"));
        expect(text(container.querySelector(".inner"))).toBe("[y]");
    });
});

describe("Composition: defaults (merge) and rest spreading (omit) across wrappers", () => {
    it("applies the primitive's defaults when nothing is passed", () => {
        const {root} = mount(BaseButton, {children: "ok"});
        expect(root.outerHTML).toBe('<button class="btn btn-md btn-solid">ok</button>');
    });

    it("lets explicit props override the defaults and spreads unknown props", () => {
        const {root} = mount(BaseButton, {size: "lg", variant: "ghost", id: "b", "aria-pressed": "true"});
        expect(root.getAttribute("class")).toBe("btn btn-lg btn-ghost");
        expect(root.id).toBe("b");
        expect(root.getAttribute("aria-pressed")).toBe("true");
        expect(root.hasAttribute("size")).toBe(false);
        expect(root.hasAttribute("variant")).toBe(false);
    });

    it("layers a wrapper's own default on top of the primitive's defaults", () => {
        const {root} = mount(DangerButton, {children: "rm"});
        expect(root.getAttribute("class")).toBe("btn btn-md btn-danger");
        expect(root.getAttribute("title")).toBe("Sure?");
        expect(root.hasAttribute("confirmText")).toBe(false);
        expect(root.hasAttribute("confirmtext")).toBe(false);
        expect(text(root)).toBe("rm");
    });

    it("overrides the wrapper's default reactively", () => {
        const [confirm, setConfirm] = createSignal("Really?");
        const {root} = mount(DangerButton, {
            get confirmText() {
                return confirm();
            }
        });
        expect(root.getAttribute("title")).toBe("Really?");
        act(() => setConfirm("Positive?"));
        expect(root.getAttribute("title")).toBe("Positive?");
    });

    it("forwards rest props and children through three wrappers onto the native button", () => {
        const onClick = vi.fn();
        const {container} = mount(() => ToolbarHost(onClick), undefined, {thunk: true});
        const btn = container.querySelector("#tb");
        expect(btn.tagName).toBe("BUTTON");
        expect(btn.getAttribute("class")).toBe("btn btn-sm btn-danger");
        expect(btn.getAttribute("title")).toBe("Sure?");
        expect(text(btn.querySelector("i.icon"))).toBe("x");
        expect(text(btn)).toBe("xDelete");
        expect(btn.hasAttribute("icon")).toBe(false);
        click(btn);
        expect(onClick).toHaveBeenCalledTimes(1);
        expect(text(btn)).toBe("xDeleted");
        expect(container.querySelector("#tb")).toBe(btn);
    });

    it("lets a caller override the size a wrapper sets explicitly", () => {
        const {root} = mount(ToolbarButton, {icon: "+", size: "xl", children: "Add"});
        // ToolbarButton sets size="sm" before spreading the rest, so the caller's size wins
        expect(root.getAttribute("class")).toBe("btn btn-xl btn-danger");
        expect(text(root)).toBe("+Add");
    });
});

describe("Composition: optional props", () => {
    it("reads an optional prop through defaultArg", () => {
        const {root} = mount(Nick, {});
        expect(text(root)).toBe("anon");
    });

    it("reads a provided optional prop through defaultArg, reactively", () => {
        const [n, setN] = createSignal("Bob");
        const {root} = mount(Nick, {
            get nickname() {
                return n();
            }
        });
        expect(text(root)).toBe("Bob");
        act(() => setN(undefined));
        expect(text(root)).toBe("anon");
    });

    // BUG: `props.age.IsSome` on an option prop compiles to an undefined getter call `Profile__get_age(props)` (ReferenceError) instead of `props.age != null`
    it.fails("renders a profile with every optional prop missing", () => {
        const {container} = mount(Profile, {name: "Ada"});
        expect(text(container.querySelector(".name"))).toBe("Ada");
        expect(text(container.querySelector(".nick"))).toBe("none");
        expect(container.querySelector(".age-unknown")).not.toBeNull();
    });

    // BUG: `props.age.IsSome` on an option prop compiles to an undefined getter call `Profile__get_age(props)`
    it.fails("renders a profile with the optional props provided", () => {
        const {container} = mount(Profile, {name: "Ada", nickname: "Addy", age: 36});
        expect(text(container.querySelector(".nick"))).toBe("Addy");
        expect(text(container.querySelector(".age"))).toBe("36");
    });

    // BUG: `props.age.IsSome` on an option prop compiles to an undefined getter call `Profile__get_age(props)`
    it.fails("toggles an optional prop from the parent", () => {
        const {container} = mount(ProfileHost);
        expect(text(container.querySelector(".nick"))).toBe("none");
        click(container.querySelector(".set-nick"));
        expect(text(container.querySelector(".nick"))).toBe("Addy");
        click(container.querySelector(".clear-nick"));
        expect(text(container.querySelector(".nick"))).toBe("none");
    });

    // BUG: `match props.message with Some m -> "..." | None -> "..."` emits an undefined getter `Status__get_message(props)` and yields `(PARTAS_YIELD) => {}` lambdas instead of the strings
    it.fails("matches on an optional prop to pick the text child", () => {
        const [m, setM] = createSignal(undefined);
        const {root} = mount(Status, {
            get message() {
                return m();
            }
        });
        expect(text(root)).toBe("idle");
        act(() => setM("saved"));
        expect(text(root)).toBe("msg:saved");
    });

    it("matches on a plain string prop to pick text children", () => {
        const [k, setK] = createSignal("a");
        const {root} = mount(KindLabel, {
            get kind() {
                return k();
            }
        });
        expect(text(root)).toBe("Alpha");
        act(() => setK("z"));
        expect(text(root)).toBe("Other");
    });

    it("matches on a plain string prop to pick element children", () => {
        const [k, setK] = createSignal("a");
        const {root} = mount(KindIcon, {
            get kind() {
                return k();
            }
        });
        expect(root.querySelector("b.alpha")).not.toBeNull();
        act(() => setK("q"));
        expect(root.querySelector("b.alpha")).toBeNull();
        expect(text(root.querySelector("i.other"))).toBe("?");
    });
});
