import {describe, it, expect} from "vitest";
import {flush, settle, deferred} from "../../helpers/index.js";
import {createProjection} from "solid-js";
import {
    makeDerivedStore,
    makeDerivedReturning,
    makeAsyncDerived,
    makeProjectionIdiomatic,
    makeProjectionRaw,
    makeOptimisticSignal,
    makeOptimisticStore
} from "./C-Derived.fs.jsx";

describe("fn-form createStore(fn, seed): writable derived store", () => {
    it("derives from the seed by mutating the draft", () => {
        const h = makeDerivedStore([1, 2, 3, 4]);
        try {
            expect([...h.derived.items]).toEqual([3, 4]);
            expect(h.derived.total).toBe(7);
            expect(h.derived.id).toBe("filtered");
            expect(h.runs()).toBe(1);
        } finally {
            h.dispose();
        }
    });

    it("re-derives when a signal it read changes", () => {
        const h = makeDerivedStore([1, 2, 3, 4]);
        try {
            h.derived.total;
            h.setThreshold(0);
            flush();
            expect([...h.derived.items]).toEqual([1, 2, 3, 4]);
            expect(h.derived.total).toBe(10);
            expect(h.runs()).toBe(2);
        } finally {
            h.dispose();
        }
    });

    it("a manual write through its setter sticks until the next re-derive overwrites it", () => {
        const h = makeDerivedStore([1, 2, 3, 4]);
        try {
            h.derived.total;
            h.overrideTotal(-1);
            flush();
            expect(h.derived.total).toBe(-1);
            expect(h.runs()).toBe(1);
            h.setThreshold(3);
            flush();
            expect(h.derived.total).toBe(4);
            expect(h.runs()).toBe(2);
        } finally {
            h.dispose();
        }
    });

    it("refresh() on the derived store re-runs the derive and resolves", async () => {
        const h = makeDerivedStore([5, 6]);
        try {
            expect(h.derived.total).toBe(11);
            expect(h.runs()).toBe(1);
            const p = h.refreshIt();
            flush();
            await settle();
            await p;
            expect(h.runs()).toBe(2);
            expect(h.derived.total).toBe(11);
        } finally {
            h.dispose();
        }
    });

    it("a derive that returns a new record (Some) replaces the state", () => {
        const h = makeDerivedReturning();
        try {
            expect([...h.derived.items]).toEqual([1, 2]);
            expect(h.derived.total).toBe(3);
            h.setN(4);
            flush();
            expect([...h.derived.items]).toEqual([4, 8]);
            expect(h.derived.total).toBe(12);
        } finally {
            h.dispose();
        }
    });

    it("an async derive (Promise of Some record) lands after the promise resolves", async () => {
        const loads = new Map();
        const load = v => {
            const d = deferred();
            loads.set(v, d);
            return d.promise;
        };
        const h = makeAsyncDerived(load);
        try {
            flush();
            expect(h.log).toEqual([]);
            loads.get(1).resolve(10);
            await settle();
            expect(h.log).toEqual([10]);
            expect(h.derived.total).toBe(10);
            expect([...h.derived.items]).toEqual([1]);

            h.setN(2);
            flush();
            // the previous value is held while the next derive is in flight
            expect(h.log).toEqual([10]);
            loads.get(2).resolve(20);
            await settle();
            expect(h.log).toEqual([10, 20]);
            expect(h.derived.total).toBe(20);
            expect([...h.derived.items]).toEqual([2]);
        } finally {
            h.dispose();
        }
    });
});

describe("createProjection", () => {
    it("upstream createProjection returns the store itself, not a [store, setter] tuple", () => {
        const h = makeProjectionRaw();
        try {
            expect(Array.isArray(h.raw)).toBe(false);
            expect(h.raw.a).toBe(true);
            expect(h.raw.b).toBe(false);
            expect(h.raw[0]).toBeUndefined();
            expect(h.raw[1]).toBeUndefined();
            expect(typeof createProjection).toBe("function");
        } finally {
            h.dispose();
        }
    });

    // BUG: Bindings.createProjection is typed RefreshableStoreReturn<'T> (store * setter), but
    // solid-js 2.0.0-rc.9 createProjection returns Refreshable<Store<T>> (no tuple; see
    // solid/packages/signals/src/store/next/projection.ts:214-219). Idiomatic
    // `let sel, _ = createProjection(...)` emits `patternInput[0]`, which is undefined on the store.
    it.fails("idiomatic tuple destructuring of createProjection reads the projected store", () => {
        const h = makeProjectionIdiomatic();
        try {
            expect(h.readA()).toBe(true);
        } finally {
            h.dispose();
        }
    });

    it("selection projection: only the keys whose value flips notify their readers", () => {
        const h = makeProjectionRaw();
        try {
            expect([h.memoA(), h.memoB(), h.memoC()]).toEqual([true, false, false]);
            expect([h.runsA(), h.runsB(), h.runsC()]).toEqual([1, 1, 1]);

            h.setSelected("b");
            flush();
            expect([h.memoA(), h.memoB(), h.memoC()]).toEqual([false, true, false]);
            expect([h.runsA(), h.runsB(), h.runsC()]).toEqual([2, 2, 1]);

            h.setSelected("c");
            flush();
            expect([h.memoA(), h.memoB(), h.memoC()]).toEqual([false, false, true]);
            expect([h.runsA(), h.runsB(), h.runsC()]).toEqual([2, 3, 2]);

            h.setSelected("none");
            flush();
            expect([h.memoA(), h.memoB(), h.memoC()]).toEqual([false, false, false]);
            expect([h.runsA(), h.runsB(), h.runsC()]).toEqual([2, 3, 3]);
        } finally {
            h.dispose();
        }
    });
});

describe("createOptimistic / createOptimisticStore (ambient writes, no action)", () => {
    it("an optimistic signal write outside an action is shown by the flush then reverted", () => {
        const h = makeOptimisticSignal();
        try {
            flush();
            expect(h.log).toEqual([1]);
            h.setValue(5);
            expect(h.value()).toBe(1);
            flush();
            expect(h.log).toEqual([1, 5, 1]);
            expect(h.value()).toBe(1);
        } finally {
            h.dispose();
        }
    });

    it("an optimistic store write outside an action is shown by the flush then reverted", () => {
        const h = makeOptimisticStore();
        try {
            flush();
            h.rename("Jake");
            expect(h.profile.name).toBe("John");
            flush();
            expect(h.log).toEqual(["John", "Jake", "John"]);
            expect(h.profile.name).toBe("John");
        } finally {
            h.dispose();
        }
    });
});
