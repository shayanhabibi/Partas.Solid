import {describe, it, expect} from "vitest";
import {createSignal} from "solid-js";
import {mount, act, click} from "../../helpers/index.js";
import {State, View, BooleanMatch, Falsy, Nested, Tuples, ActivePattern, Callbacks, SharedTarget, GuardedProps, TwoMatches} from "./A-MatchBang.fs.jsx";

const render = fn => mount(fn, undefined, {thunk: true}).root;

import {ArrayState, IndexedBinding, TotalActivePattern} from "./A-MatchBang.fs.jsx";

describe("match! user bindings", () => {
    it("keeps total active-pattern payloads live and switches cases", () => {
        const [value, setValue] = createSignal(1);
        const root = render(() => TotalActivePattern(value));
        const node = root.querySelector("span");
        expect(node.textContent).toBe("1");
        act(() => setValue(2));
        expect(root.querySelector("span")).toBe(node);
        expect(node.textContent).toBe("2");
        act(() => setValue(-1));
        expect(root.querySelector("b").textContent).toBe("none");
    });
    it("evaluates an indexed user binding once per arm mount", () => {
        const [state, setState] = createSignal(new ArrayState(1, [["first", "second", "third"]]));
        let reads = 0;
        const root = render(() => IndexedBinding(state, () => reads++));
        const span = root.querySelector("span");
        expect(root.textContent).toBe("firstfirst");
        expect(reads).toBe(1);
        act(() => setState(new ArrayState(1, [["new", "values", "here"]])));
        expect(root.querySelector("span")).toBe(span);
        expect(root.textContent).toBe("firstfirst");
        expect(reads).toBe(1);
    });
});

describe("match! Switch/Match lowering", () => {
    it("keeps active-pattern results live while retaining the arm", () => {
        const [value, setValue] = createSignal(1);
        const root = render(() => ActivePattern(value));
        const node = root.querySelector("span");
        expect(node.textContent).toBe("1");
        act(() => setValue(2));
        expect(root.querySelector("span")).toBe(node);
        expect(node.textContent).toBe("2");
        act(() => setValue(0));
        expect(root.querySelector("b").textContent).toBe("non-positive");
    });

    it("reads updated bound values from event callbacks", () => {
        const [state, setState] = createSignal(new State(1, ["Alice"]));
        const reports = [];
        const root = render(() => Callbacks(state, name => { reports.push(name); }));
        const node = root.querySelector("button");
        act(() => setState(new State(1, ["Bobby"])));
        expect(root.querySelector("button")).toBe(node);
        click(node);
        expect(reports).toEqual(["Bobby"]);
    });

    it("keeps OR-pattern targets and tuple bindings reactive", () => {
        const [value, setValue] = createSignal([1, "one"]);
        const root = render(() => SharedTarget(value));
        const node = root.querySelector("span");
        act(() => setValue([2, "two"]));
        expect(root.querySelector("span")).toBe(node);
        expect(node.textContent).toBe("two");
        act(() => setValue([3, "three"]));
        expect(root.querySelector("b").textContent).toBe("three");
    });

    it("tracks props read by source expressions and pattern guards", () => {
        const [state, setState] = createSignal(new State(1, ["Alice"]));
        const [enabled, setEnabled] = createSignal(false);
        const {root} = mount(GuardedProps, {readState: state, get enabled() { return enabled(); }});
        expect(root.querySelector("b").textContent).toBe("Alice");
        act(() => setEnabled(true));
        expect(root.querySelector("span").textContent).toBe("Alice");
        act(() => setState(new State(1, ["Bobby"])));
        expect(root.querySelector("span").textContent).toBe("Bobby");
    });

    it("updates two sibling matches independently", () => {
        const [first, setFirst] = createSignal(false);
        const [second, setSecond] = createSignal(false);
        const root = render(() => TwoMatches(first, second));
        expect(root.textContent).toBe("nonenone");
        act(() => setFirst(true));
        expect(root.textContent).toBe("firstnone");
        act(() => setSecond(true));
        expect(root.textContent).toBe("firstsecond");
    });
    it("keeps outer bindings live through a nested match!", () => {
        const [state, setState] = createSignal(new State(1, ["Alice"]));
        const [flag, setFlag] = createSignal(true);
        const root = render(() => Nested(state, flag));
        const section = root.querySelector("section");
        expect(section.textContent).toBe("Alice");
        act(() => setState(new State(1, ["Bobby"])));
        expect(root.querySelector("section")).toBe(section);
        expect(section.textContent).toBe("Bobby");
        act(() => setFlag(false));
        expect(section.querySelector("b").textContent).toBe("Bobby");
    });

    it("preserves tuple fields and updates a wildcard arm", () => {
        const [value, setValue] = createSignal([0, "zero"]);
        const root = render(() => Tuples(value));
        expect(root.textContent).toBe("zero");
        act(() => setValue([1, "one"]));
        const node = root.querySelector("b");
        expect(node.textContent).toBe("1one");
        act(() => setValue([2, "two"]));
        expect(root.querySelector("b")).toBe(node);
        expect(node.textContent).toBe("2two");
    });
    it("retains a selected arm while its payload updates, and switches guarded arms", () => {
        const [state, setState] = createSignal(new State(1, ["Alice"]));
        const mounts = [];
        const root = render(() => View(state, name => { mounts.push(name); }));
        const before = root.querySelector(".before");
        const after = root.querySelector(".after");
        const input = root.querySelector(".name");
        expect(input.value).toBe("Alice");
        expect(root.querySelector(".mirror").textContent).toBe("ALICE");
        expect(mounts).toEqual(["Alice"]);
        act(() => setState(new State(1, ["Bobby"])));
        expect(root.querySelector(".name")).toBe(input);
        expect(input.value).toBe("Bobby");
        expect(root.querySelector(".mirror").textContent).toBe("BOBBY");
        expect(mounts).toEqual(["Alice", "Bobby"]);
        act(() => setState(new State(1, ["Bo"])));
        expect(input.isConnected).toBe(false);
        expect(root.querySelector(".short").textContent).toBe("Bo");
        act(() => setState(new State(2, [0, "failed"])));
        expect(root.querySelector(".code").textContent).toBe("0");
        expect(root.querySelector(".message").textContent).toBe("failed");
        act(() => setState(new State(0, [])));
        expect(root.querySelector(".loading").textContent).toBe("Loading...");
        act(() => setState(new State(1, ["Carol"])));
        expect(root.querySelector(".name")).not.toBe(input);
        expect(mounts).toEqual(["Alice", "Bobby", "Carol"]);
        expect(root.querySelector(".before")).toBe(before);
        expect(root.querySelector(".after")).toBe(after);
    });

    it("evaluates the matched source once per reactive selection", () => {
        const [state, setState] = createSignal(new State(1, ["Alice"]));
        let reads = 0;
        const root = render(() => View(() => { reads++; return state(); }, () => {}));
        expect(root.querySelector(".name").value).toBe("Alice");
        expect(reads).toBe(1);
        act(() => setState(new State(1, ["Bobby"])));
        expect(root.querySelector(".name").value).toBe("Bobby");
        expect(reads).toBe(2);
    });

    it("handles boolean patterns and false values", () => {
        const [flag, setFlag] = createSignal(false);
        const root = render(() => BooleanMatch(flag));
        expect(root.textContent).toBe("no");
        act(() => setFlag(true));
        expect(root.textContent).toBe("yes");
    });

    it("handles zero payloads and preserves a wildcard arm", () => {
        const [value, setValue] = createSignal(0);
        const root = render(() => Falsy(value));
        expect(root.textContent).toBe("zero");
        act(() => setValue(1));
        const node = root.querySelector("b");
        expect(node.textContent).toBe("1");
        act(() => setValue(2));
        expect(root.querySelector("b")).toBe(node);
        expect(node.textContent).toBe("2");
    });
});
