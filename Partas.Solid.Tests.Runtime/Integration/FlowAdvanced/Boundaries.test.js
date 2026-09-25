import {describe, it, expect, vi, beforeEach, afterEach} from "vitest";
import {createSignal} from "solid-js";
import {mount, click, act, settle, deferred, waitFor, text} from "../../helpers/index.js";
import {
    RenderGuard,
    AsyncGuard,
    NestedGuards,
    GuardedList,
    GuardedListLocal,
    NestedLoading,
    PendingIndicator,
    PendingIndicatorLocal,
    OnLoading,
    CollapsedReveal
} from "./C-Boundaries.fs.jsx";

// Errored logs caught errors in dev only for non-callback fallbacks, but keep the output quiet anyway.
let errSpy;
beforeEach(() => {
    errSpy = vi.spyOn(console, "error").mockImplementation(() => {
    });
});
afterEach(() => errSpy.mockRestore());

describe("FlowAdvanced: Errored with render-time throws", () => {
    it("catches a throw from a component body and keeps siblings outside the boundary", () => {
        const {container} = mount(RenderGuard, {shouldFail: () => true});
        expect(text(container.querySelector(".sibling"))).toBe("outside");
        expect(container.querySelector(".ok")).toBeNull();
        expect(text(container.querySelector(".error .message"))).toBe("render failed");
    });

    it("renders children normally when nothing throws", () => {
        const {container} = mount(RenderGuard, {shouldFail: () => false});
        expect(text(container.querySelector(".ok"))).toBe("fine");
        expect(container.querySelector(".error")).toBeNull();
    });

    it("reset() re-renders the children, which may throw again or recover", () => {
        let fail = true;
        const shouldFail = vi.fn(() => fail);
        const {container} = mount(RenderGuard, {shouldFail});
        expect(container.querySelector(".error")).not.toBeNull();

        click(container.querySelector(".retry"));
        expect(text(container.querySelector(".error .message"))).toBe("render failed");

        fail = false;
        click(container.querySelector(".retry"));
        expect(container.querySelector(".error")).toBeNull();
        expect(text(container.querySelector(".ok"))).toBe("fine");
        expect(shouldFail).toHaveBeenCalledTimes(3);
    });
});

describe("FlowAdvanced: Errored with async errors", () => {
    it("routes a rejected async memo to the boundary", async () => {
        const d = deferred();
        const {container} = mount(AsyncGuard, {load: () => d.promise});
        expect(text(container.querySelector(".spinner"))).toBe("loading");

        d.reject(new Error("net down"));
        await waitFor(() => container.querySelector(".error"));
        expect(text(container.querySelector(".error .message"))).toBe("net down");
        expect(container.querySelector(".spinner")).toBeNull();
    });

    it("reset() retries the async child, which can then succeed", async () => {
        let attempt = 0;
        const load = vi.fn(() => (++attempt === 1 ? Promise.reject(new Error("first fails")) : Promise.resolve("second works")));
        const {container} = mount(AsyncGuard, {load});
        await waitFor(() => container.querySelector(".error"));
        expect(text(container.querySelector(".error .message"))).toBe("first fails");

        click(container.querySelector(".retry"));
        const data = await waitFor(() => container.querySelector(".data"));
        expect(text(data)).toBe("second works");
        expect(container.querySelector(".error")).toBeNull();
        expect(load).toHaveBeenCalledTimes(2);
    });

    it("resolves normally when the promise fulfils", async () => {
        const {container} = mount(AsyncGuard, {load: () => Promise.resolve("ok")});
        const data = await waitFor(() => container.querySelector(".data"));
        expect(text(data)).toBe("ok");
        expect(container.querySelector(".spinner")).toBeNull();
        expect(container.querySelector(".error")).toBeNull();
    });
});

describe("FlowAdvanced: nested Errored", () => {
    it("the inner boundary catches first and the outer content survives", () => {
        const {container} = mount(NestedGuards, {innerFails: () => true, fallbackFails: false});
        expect(text(container.querySelector(".outer-content"))).toBe("outer");
        expect(text(container.querySelector(".inner-error"))).toBe("render failed");
        expect(container.querySelector(".outer-error")).toBeNull();
    });

    it("an error thrown by the inner fallback escalates to the outer boundary", () => {
        const {container} = mount(NestedGuards, {innerFails: () => true, fallbackFails: true});
        expect(container.querySelector(".outer-content")).toBeNull();
        expect(container.querySelector(".inner-error")).toBeNull();
        expect(text(container.querySelector(".outer-error"))).toBe("fallback failed");
    });
});

describe("FlowAdvanced: Errored per list row", () => {
    const rowIsolation = Comp => () => {
        const [values, setValues] = createSignal([1, -2, 3]);
        const {container} = mount(Comp, {
            get values() {
                return values();
            }
        });
        const cells = () => [...container.querySelectorAll("li.cell")].map(text);
        expect(cells()).toEqual(["1", "negative -2", "3"]);
        act(() => setValues([1, 2, 3]));
        expect(cells()).toEqual(["1", "2", "3"]);
        act(() => setValues([-1]));
        expect(cells()).toEqual(["negative -1"]);
    };

    it("isolates failures to the offending row and heals reactively", rowIsolation(GuardedListLocal));

    // BUG: `string props.value` inside `failwith` in a memo emits undefined `RowGuard__get_value(props)` instead of `props.value`.
    it.fails("formats a prop read directly in the thrown message", rowIsolation(GuardedList));
});

describe("FlowAdvanced: nested Loading", () => {
    it("the outer waits for its own data, then the inner shows its fallback until ready", async () => {
        const outer = deferred();
        const inner = deferred();
        const {container} = mount(NestedLoading, {outer: () => outer.promise, inner: () => inner.promise});
        expect(container.querySelector(".outer-fb")).not.toBeNull();
        expect(container.querySelector(".inner-fb")).toBeNull();

        outer.resolve("O");
        await settle();
        expect(container.querySelector(".outer-fb")).toBeNull();
        expect(text(container.querySelector(".outer"))).toBe("O");
        expect(container.querySelector(".inner-fb")).not.toBeNull();

        inner.resolve("I");
        await settle();
        expect(container.querySelector(".inner-fb")).toBeNull();
        expect(text(container.querySelector(".inner"))).toBe("I");
    });

    it("inner data resolving first does not reveal the outer boundary early", async () => {
        const outer = deferred();
        const inner = deferred();
        const {container} = mount(NestedLoading, {outer: () => outer.promise, inner: () => inner.promise});
        inner.resolve("I");
        await settle();
        expect(container.querySelector(".outer-fb")).not.toBeNull();
        expect(container.querySelector(".inner")).toBeNull();

        outer.resolve("O");
        await settle();
        expect(container.querySelector(".outer-fb")).toBeNull();
        expect(container.querySelector(".inner-fb")).toBeNull();
        expect(text(container.querySelector(".outer"))).toBe("O");
        expect(text(container.querySelector(".inner"))).toBe("I");
    });
});

describe("FlowAdvanced: isPending", () => {
    const pendingFlow = Comp => async () => {
        const pending = new Map();
        const fetch = id => {
            const d = deferred();
            pending.set(id, d);
            return d.promise;
        };
        const [id, setId] = createSignal(1);
        const {container} = mount(Comp, {
            get id() {
                return id();
            },
            fetch
        });
        expect(container.querySelector(".spinner")).not.toBeNull();
        expect(container.querySelector(".busy")).toBeNull();

        pending.get(1).resolve("one");
        await settle();
        expect(text(container.querySelector(".content"))).toBe("one");
        expect(container.querySelector(".busy")).toBeNull();

        act(() => setId(2));
        await settle();
        expect(text(container.querySelector(".busy"))).toBe("refreshing");
        expect(text(container.querySelector(".content"))).toBe("one");
        expect(container.querySelector(".spinner")).toBeNull();

        pending.get(2).resolve("two");
        await waitFor(() => text(container.querySelector(".content")) === "two");
        expect(container.querySelector(".busy")).toBeNull();
    };

    it("is false on first load and true while a refetch is in flight", pendingFlow(PendingIndicatorLocal));

    // BUG: (known ValueUnroller issue) inline `isPending (fun () -> box (detail ()))` emits `isPending(detail())` instead of a thunk.
    it.fails("inline isPending thunk shows the indicator during a refetch", pendingFlow(PendingIndicator));
});

describe("FlowAdvanced: Loading on", () => {
    it("shows the fallback again when the `on` key changes", async () => {
        const pending = new Map();
        const fetch = id => {
            const d = deferred();
            pending.set(id, d);
            return d.promise;
        };
        const [id, setId] = createSignal(1);
        const {container} = mount(OnLoading, {
            get id() {
                return id();
            },
            fetch
        });
        expect(container.querySelector(".spinner")).not.toBeNull();
        pending.get(1).resolve("one");
        await settle();
        expect(text(container.querySelector(".content"))).toBe("one");
        expect(container.querySelector(".spinner")).toBeNull();

        act(() => setId(2));
        await settle();
        expect(container.querySelector(".spinner")).not.toBeNull();

        pending.get(2).resolve("two");
        await waitFor(() => text(container.querySelector(".content")) === "two");
        expect(container.querySelector(".spinner")).toBeNull();
    });
});

describe("FlowAdvanced: Reveal collapsed", () => {
    it("sequential + collapsed shows only the frontier fallback", async () => {
        const a = deferred();
        const b = deferred();
        const c = deferred();
        const {container} = mount(CollapsedReveal, {
            first: () => a.promise,
            second: () => b.promise,
            third: () => c.promise
        });
        expect(container.querySelector(".fb-a")).not.toBeNull();
        expect(container.querySelector(".fb-b")).toBeNull();
        expect(container.querySelector(".fb-c")).toBeNull();

        a.resolve("A");
        await settle();
        expect(text(container.querySelector(".a"))).toBe("A");
        expect(container.querySelector(".fb-b")).not.toBeNull();
        expect(container.querySelector(".fb-c")).toBeNull();

        c.resolve("C");
        await settle();
        expect(container.querySelector(".c")).toBeNull();
        expect(container.querySelector(".fb-b")).not.toBeNull();

        b.resolve("B");
        await settle();
        expect(text(container.querySelector(".b"))).toBe("B");
        expect(text(container.querySelector(".c"))).toBe("C");
        expect(container.querySelector(".fb-b")).toBeNull();
    });
});
