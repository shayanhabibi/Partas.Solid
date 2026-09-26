import {describe, it, expect} from "vitest";
import {flush, settle, deferred} from "../../helpers/index.js";
import {TimeoutError} from "solid-js";
import {
    makeAsyncMemo,
    makeAsyncMemoWithLoadingValue,
    makeUntil,
    makeRefreshSync,
    makeRefreshAsync,
    refreshPlainFunction,
    makeLikeAction,
    syncSumAction,
    makeOptimisticListAction
} from "./D-Async.fs.jsx";

const tick = () => new Promise(r => setTimeout(r, 0));

function gatedLoader() {
    const gates = new Map();
    const load = id => {
        const d = deferred();
        gates.set(id, d);
        return d.promise;
    };
    return {gates, load};
}

describe("async createMemo (Promise compute), isPending, latest", () => {
    it("effects wait for the first value, then see it", async () => {
        const {gates, load} = gatedLoader();
        const h = makeAsyncMemo(load);
        try {
            flush();
            expect(h.loads()).toBe(1);
            expect(h.values).toEqual([]);
            gates.get(1).resolve("user-1");
            await settle();
            expect(h.values).toEqual(["user-1"]);
            expect(h.user()).toBe("user-1");
        } finally {
            h.dispose();
        }
    });

    it("a source change refetches; the old value is held until the new one lands", async () => {
        const {gates, load} = gatedLoader();
        const h = makeAsyncMemo(load);
        try {
            flush();
            gates.get(1).resolve("user-1");
            await settle();

            h.setId(2);
            flush();
            expect(h.loads()).toBe(2);
            expect(h.values).toEqual(["user-1"]);

            gates.get(2).resolve("user-2");
            await settle();
            expect(h.values).toEqual(["user-1", "user-2"]);
            expect(h.user()).toBe("user-2");
        } finally {
            h.dispose();
        }
    });

    it("isPending(memo) is false when settled, true during a refetch, false again after", async () => {
        const {gates, load} = gatedLoader();
        const h = makeAsyncMemo(load);
        try {
            flush();
            gates.get(1).resolve("user-1");
            await settle();
            expect(h.pending.at(-1)).toBe(false);

            h.setId(2);
            flush();
            expect(h.pending.at(-1)).toBe(true);

            gates.get(2).resolve("user-2");
            await settle();
            expect(h.pending.at(-1)).toBe(false);
        } finally {
            h.dispose();
        }
    });

    it("latest(memo) suspends on the initial load, then tracks the landed value", async () => {
        const {gates, load} = gatedLoader();
        const h = makeAsyncMemo(load);
        try {
            flush();
            expect(h.latestValues).toEqual([]);
            gates.get(1).resolve("user-1");
            await settle();
            expect(h.latestValues.at(-1)).toBe("user-1");

            h.setId(2);
            flush();
            // never regresses to undefined while the refetch is in flight
            expect(h.latestValues.every(v => v !== undefined)).toBe(true);
            gates.get(2).resolve("user-2");
            await settle();
            expect(h.latestValues.at(-1)).toBe("user-2");
        } finally {
            h.dispose();
        }
    });

    it("createMemo(asyncFn, loadingValue) serves the loading value until the promise lands", async () => {
        const d = deferred();
        const h = makeAsyncMemoWithLoadingValue(() => d.promise);
        try {
            flush();
            expect(h.values).toEqual(["loading…"]);
            d.resolve("ready");
            await settle();
            expect(h.values).toEqual(["loading…", "ready"]);
            expect(h.read()).toBe("ready");
        } finally {
            h.dispose();
        }
    });
});

describe("until", () => {
    it("resolves immediately (with the predicate's truthy value) when already true", async () => {
        const h = makeUntil();
        try {
            await expect(h.waitAtLeast(0)).resolves.toBe(true);
        } finally {
            h.dispose();
        }
    });

    it("waits through falsy evaluations and resolves when the condition flips", async () => {
        const h = makeUntil();
        try {
            let settled = false;
            const p = h.waitAtLeast(2).then(v => ((settled = true), v));
            flush();
            await tick();
            expect(settled).toBe(false);

            h.setCount(1);
            flush();
            await tick();
            expect(settled).toBe(false);

            h.setCount(2);
            flush();
            await expect(p).resolves.toBe(true);
        } finally {
            h.dispose();
        }
    });

    it("rejects with TimeoutError after `timeout` ms (ParamObject overload)", async () => {
        const h = makeUntil();
        try {
            const err = await h.waitWithTimeout(5, 10).then(
                () => null,
                e => e
            );
            expect(err).toBeInstanceOf(TimeoutError);
            expect(err.name).toBe("TimeoutError");
        } finally {
            h.dispose();
        }
    });

    it("rejects with TimeoutError via the UntilOptions pojo overload", async () => {
        const h = makeUntil();
        try {
            await expect(h.waitWithOptions(5, 10)).rejects.toBeInstanceOf(TimeoutError);
        } finally {
            h.dispose();
        }
    });

    it("the timeout does not fire when the condition turns true first", async () => {
        const h = makeUntil();
        try {
            const p = h.waitWithTimeout(1, 50);
            h.setCount(1);
            flush();
            await expect(p).resolves.toBe(true);
        } finally {
            h.dispose();
        }
    });

    it("F# `:? TimeoutError` type test matches the rejection (imported class binding)", async () => {
        const h = makeUntil();
        try {
            await expect(h.classify(5, 10)).resolves.toBe("timeout:Timed out waiting for condition");
            h.setCount(5);
            flush();
            await expect(h.classify(5, 10)).resolves.toBe("resolved");
        } finally {
            h.dispose();
        }
    });

    it("rejects with the abort reason when the signal aborts", async () => {
        const h = makeUntil();
        try {
            const ctl = new AbortController();
            const p = h.waitWithSignal(5, ctl.signal);
            ctl.abort("stop");
            await expect(p).rejects.toBe("stop");
            // an already-aborted signal rejects straight away
            await expect(h.waitWithSignal(5, ctl.signal)).rejects.toBe("stop");
        } finally {
            h.dispose();
        }
    });

    it("calling until inside a tracking scope throws in dev", () => {
        const h = makeUntil();
        try {
            expect(h.untilInsideMemo()).toMatch(/^threw:Cannot call until inside a reactive scope/);
        } finally {
            h.dispose();
        }
    });
});

describe("refresh", () => {
    it("sync memo: resolves with the re-executed value", async () => {
        const h = makeRefreshSync();
        try {
            flush();
            expect(h.memo()).toBe(10);
            const p = h.refreshMemo();
            flush();
            await expect(p).resolves.toBe(20);
            expect(h.memo()).toBe(20);
            expect(h.runs()).toBe(2);
        } finally {
            h.dispose();
        }
    });

    it("async memo: re-asks the source and resolves with the landed value", async () => {
        const gates = [];
        const h = makeRefreshAsync(n => {
            const d = deferred();
            gates.push(d);
            return d.promise;
        });
        try {
            flush();
            gates[0].resolve("v1");
            await settle();
            expect(h.values).toEqual(["v1"]);

            const p = h.refreshIt();
            flush();
            await tick();
            expect(h.calls()).toBe(2);
            gates[1].resolve("v2");
            await expect(p).resolves.toBe("v2");
            await settle();
            expect(h.values).toEqual(["v1", "v2"]);
        } finally {
            h.dispose();
        }
    });

    it("refresh of a plain function (not a Solid source) throws INVALID_REFRESH_TARGET in dev", () => {
        expect(refreshPlainFunction()).toMatch(/^\[INVALID_REFRESH_TARGET\]/);
    });
});

describe("action (generator protocol authored in F#)", () => {
    it("a synchronous action resolves with the generator's return value", async () => {
        // steps: yield 1 (sent back 1), yield 2 (sent back 2) → start + 1 + 2
        await expect(syncSumAction(10)).resolves.toBe(13);
    });

    it("optimistic write is visible while the server call is in flight, then confirmed", async () => {
        const h = makeLikeAction();
        try {
            flush();
            expect(h.log).toEqual(["opt:10", "likes:10"]);
            const server = deferred();
            const result = h.like(server.promise);
            flush();
            await tick();
            expect(h.optimistic()).toBe(11);
            expect(h.likes()).toBe(10);
            expect(h.log).toEqual(["opt:10", "likes:10", "opt:11"]);

            server.resolve(11);
            // The return step reads likes() synchronously after setLikes(11) in the previous step:
            // writes batch until flush, so the action observes the committed value (10), not its own write.
            await expect(result).resolves.toBe("liked:10");
            await settle();
            expect(h.likes()).toBe(11);
            expect(h.optimistic()).toBe(11);
            expect(h.log.at(-1)).toBe("likes:11");
            // no revert flicker back to 10 between the optimistic write and the confirmation
            expect(h.log.slice(2)).not.toContain("opt:10");
        } finally {
            h.dispose();
        }
    });

    it("a rejected step fails the action and reverts the optimistic write", async () => {
        const h = makeLikeAction();
        try {
            flush();
            const server = deferred();
            const result = h.like(server.promise);
            flush();
            await tick();
            expect(h.optimistic()).toBe(11);

            const boom = new Error("server down");
            server.reject(boom);
            await expect(result).rejects.toBe(boom);
            await settle();
            expect(h.optimistic()).toBe(10);
            expect(h.likes()).toBe(10);
            expect(h.log.at(-1)).toBe("opt:10");
        } finally {
            h.dispose();
        }
    });

    it("calling an action inside an owned scope throws ACTION_CALLED_IN_OWNED_SCOPE in dev", () => {
        const h = makeLikeAction();
        try {
            expect(h.likeInsideOwnedScope()).toMatch(/^\[ACTION_CALLED_IN_OWNED_SCOPE\]/);
        } finally {
            h.dispose();
        }
    });
});

describe("createOptimisticStore (fn-form) driven by an action", () => {
    it("the optimistic append is visible in flight, then replaced by the re-derived source", async () => {
        const h = makeOptimisticListAction();
        try {
            flush();
            expect(h.log).toEqual([1]);
            expect(h.optimisticItems.map(i => i.name)).toEqual(["one"]);

            const server = deferred();
            const result = h.add("two", server.promise);
            flush();
            await tick();
            expect(h.optimisticItems.map(i => [i.id, i.name])).toEqual([[1, "one"], [-1, "two"]]);
            expect(h.items.length).toBe(1);
            expect(h.log).toEqual([1, 2]);

            server.resolve("two!");
            await result;
            await settle();
            expect(h.items.map(i => [i.id, i.name])).toEqual([[1, "one"], [2, "two!"]]);
            // the optimistic overlay is dropped and the store re-derives from the authoritative list
            expect(h.optimisticItems.map(i => [i.id, i.name])).toEqual([[1, "one"], [2, "two!"]]);
            expect(h.log.at(-1)).toBe(2);
            // no flicker back to the pre-action length between optimistic write and confirmation
            expect(h.log.slice(1)).not.toContain(1);
        } finally {
            h.dispose();
        }
    });

    it("a failed action reverts the optimistic append", async () => {
        const h = makeOptimisticListAction();
        try {
            flush();
            const server = deferred();
            const result = h.add("two", server.promise);
            flush();
            await tick();
            expect(h.optimisticItems.length).toBe(2);

            const boom = new Error("rejected");
            server.reject(boom);
            await expect(result).rejects.toBe(boom);
            await settle();
            expect(h.optimisticItems.map(i => i.name)).toEqual(["one"]);
            expect(h.items.length).toBe(1);
            expect(h.log.at(-1)).toBe(1);
        } finally {
            h.dispose();
        }
    });
});
