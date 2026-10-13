import {describe, it, expect, vi} from "vitest";
import {createSignal} from "solid-js";
import {mount, click, act, flush, text} from "../../helpers/index.js";
import {AccessorChain, Middle, Swapper, Tabs, Lifecycle, HoverUser, LifecycleList, LetSwapper} from "./E-Abstract.fs.jsx";

const leaves = container => [...container.querySelectorAll(".leaf")].map(text);

describe("Components: nested components passing accessors", () => {
    it("threads an accessor through two component layers and derives from it once per change", () => {
        const onCompute = vi.fn();
        const {container} = mount(() => AccessorChain(onCompute), undefined, {thunk: true});
        expect(leaves(container)).toEqual(["1", "2"]);
        expect(onCompute.mock.calls).toEqual([[2]]);
        const [l1, l2] = container.querySelectorAll(".leaf");

        click(container.querySelector(".inc"));
        expect(leaves(container)).toEqual(["2", "4"]);
        click(container.querySelector(".inc"));
        expect(leaves(container)).toEqual(["3", "6"]);
        expect(onCompute.mock.calls).toEqual([[2], [4], [6]]);
        // the leaves were updated in place, not re-created
        expect(container.querySelectorAll(".leaf")[0]).toBe(l1);
        expect(container.querySelectorAll(".leaf")[1]).toBe(l2);
    });

    it("accepts an accessor from outside and batches several writes into one derivation", () => {
        const [n, setN] = createSignal(5);
        const onCompute = vi.fn();
        const {container} = mount(Middle, {value: n, onCompute});
        expect(leaves(container)).toEqual(["5", "10"]);
        act(() => {
            setN(6);
            setN(7);
            setN(8);
        });
        expect(leaves(container)).toEqual(["8", "16"]);
        expect(onCompute.mock.calls).toEqual([[10], [16]]);
    });

    it("calls a render-prop with the owner's accessor", () => {
        const {container} = mount(HoverUser);
        const host = container.querySelector(".hover");
        const state = container.querySelector(".state");
        expect(text(state)).toBe("idle");
        act(() => host.dispatchEvent(new MouseEvent("mouseenter")));
        expect(text(state)).toBe("hovering");
        act(() => host.dispatchEvent(new MouseEvent("mouseleave")));
        expect(text(state)).toBe("idle");
    });
});

describe("Components: lifecycle and disposal", () => {
    it("runs the component body once, the effect after the render flush, and cleanup on dispose", () => {
        const log = [];
        const [tick, setTick] = createSignal(0);
        const {dispose} = mount(Lifecycle, {name: "solo", tick, log: m => log.push(m)});
        expect(log).toEqual(["mount solo", "effect solo 0"]);

        act(() => setTick(1));
        act(() => setTick(2));
        expect(log).toEqual(["mount solo", "effect solo 0", "effect solo 1", "effect solo 2"]);

        dispose();
        expect(log.at(-1)).toBe("cleanup solo");
        act(() => setTick(3));
        expect(log.filter(l => l.startsWith("effect"))).toHaveLength(3);
    });

    it("disposes the hidden branch of a Show swap: onCleanup fires and its effect stops", () => {
        const log = [];
        const [showA, setShowA] = createSignal(true);
        const [tick, setTick] = createSignal(0);
        const {container} = mount(Swapper, {
            get showA() {
                return showA();
            },
            tick,
            log: m => log.push(m)
        });
        expect(text(container.querySelector(".panel"))).toBe("A");
        expect(log).toEqual(["mount A", "effect A 0"]);

        log.length = 0;
        act(() => setShowA(false));
        expect(text(container.querySelector(".panel"))).toBe("B");
        // exactly: A cleaned up once, B mounted and its effect ran; A's effect did not re-run
        expect([...log].sort()).toEqual(["cleanup A", "effect B 0", "mount B"]);

        log.length = 0;
        act(() => setTick(1));
        // only the live branch reacts
        expect(log).toEqual(["effect B 1"]);

        log.length = 0;
        act(() => setShowA(true));
        expect([...log].sort()).toEqual(["cleanup B", "effect A 1", "mount A"]);
    });

    it("disposes every row of a list branch when Switch leaves it", () => {
        const log = [];
        const [tab, setTab] = createSignal("one");
        const [tick, setTick] = createSignal(0);
        const {container} = mount(Tabs, {
            get tab() {
                return tab();
            },
            tick,
            log: m => log.push(m)
        });
        expect([...container.querySelectorAll(".panel")].map(text)).toEqual(["one"]);

        log.length = 0;
        act(() => setTab("many"));
        expect([...container.querySelectorAll(".panel")].map(text)).toEqual(["x", "y"]);
        expect(log).toContain("cleanup one");
        expect(log.filter(l => l.startsWith("mount")).sort()).toEqual(["mount x", "mount y"]);

        log.length = 0;
        act(() => setTick(5));
        expect(log.sort()).toEqual(["effect x 5", "effect y 5"]);

        log.length = 0;
        act(() => setTab("none"));
        expect(container.querySelector(".panel")).toBeNull();
        expect(log.sort()).toEqual(["cleanup x", "cleanup y"]);

        log.length = 0;
        act(() => setTick(6));
        expect(log).toEqual([]);
    });

    it("disposes everything when the root is disposed", () => {
        const log = [];
        const [tick, setTick] = createSignal(0);
        const {dispose, container} = mount(Tabs, {tab: "many", tick, log: m => log.push(m)});
        log.length = 0;
        dispose();
        expect(log.sort()).toEqual(["cleanup x", "cleanup y"]);
        expect(container.isConnected).toBe(false);
        act(() => setTick(1));
        expect(log).toHaveLength(2);
        flush();
    });
});

describe("Components: per-row and let-binding disposal", () => {
    it("removing one keyed row disposes only that row; surviving rows keep their node and effect", () => {
        const log = [];
        const [names, setNames] = createSignal(["x", "y", "z"]);
        const [tick, setTick] = createSignal(0);
        const {container} = mount(LifecycleList, {
            get names() {
                return names();
            },
            tick,
            log: m => log.push(m)
        });
        const panels = () => [...container.querySelectorAll(".panel")];
        expect(panels().map(text)).toEqual(["x", "y", "z"]);
        const [px, , pz] = panels();

        log.length = 0;
        act(() => setNames(["x", "z"]));
        expect(panels().map(text)).toEqual(["x", "z"]);
        expect(panels()).toEqual([px, pz]);
        expect(log).toEqual(["cleanup y"]);

        log.length = 0;
        act(() => setTick(1));
        expect([...log].sort()).toEqual(["effect x 1", "effect z 1"]);

        log.length = 0;
        act(() => setNames(["z", "x", "w"]));
        expect(panels().map(text)).toEqual(["z", "x", "w"]);
        expect(panels()[0]).toBe(pz);
        expect(panels()[1]).toBe(px);
        // reorder does not remount; only the new row mounts
        expect([...log].sort()).toEqual(["effect w 1", "mount w"]);
    });

    it("a let-bound component in a Show branch runs onCleanup when the branch is swapped out", () => {
        const log = [];
        const [showA, setShowA] = createSignal(true);
        const {container} = mount(LetSwapper, {
            get showA() {
                return showA();
            },
            log: m => log.push(m)
        });
        expect(text(container.querySelector(".let-panel"))).toBe("A");
        expect(log).toEqual(["mount A"]);

        log.length = 0;
        act(() => setShowA(false));
        expect(text(container.querySelector(".let-panel"))).toBe("B");
        expect([...log].sort()).toEqual(["cleanup A", "mount B"]);

        log.length = 0;
        act(() => setShowA(true));
        expect([...log].sort()).toEqual(["cleanup B", "mount A"]);
    });
});
