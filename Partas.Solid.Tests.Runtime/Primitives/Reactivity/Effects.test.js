// Effects through the Partas bindings: two-phase createEffect, cleanups, error bundles, defer,
// createRenderEffect, createTrackedEffect, onSettled, onCleanup and flush batching.
import {describe, it, expect} from "vitest";
import {flush} from "../../helpers/index.js";
import {
    makeTwoPhase,
    makeComputePrev,
    makeErrorBundle,
    makeErrorPojo,
    makeDeferred,
    makeRenderVsUser,
    makeRenderCleanup,
    makeTracked,
    makeLifecycle,
    makeMemoCleanup,
    makeBatching
} from "./C-Effects.fs.jsx";

const count = (log, entry) => log.filter(e => e === entry).length;

describe("createEffect(compute, effect): two phases", () => {
    it("compute runs at creation, the effect phase on flush", () => {
        const e = makeTwoPhase();
        expect(e.log).toEqual(["compute:1"]);
        flush();
        expect(e.log).toEqual(["compute:1", "effect:1:100"]);
        e.dispose();
    });

    it("on change: compute, then the previous cleanup, then the effect", () => {
        const e = makeTwoPhase();
        flush();
        e.set(2);
        expect(e.log).toEqual(["compute:1", "effect:1:100"]);
        flush();
        expect(e.log).toEqual(["compute:1", "effect:1:100", "compute:2", "cleanup:1", "effect:2:100"]);
        e.dispose();
    });

    it("reads inside the effect phase are untracked", () => {
        const e = makeTwoPhase();
        flush();
        e.setOther(200);
        flush();
        expect(e.log).toEqual(["compute:1", "effect:1:100"]);
        // the untracked read still sees the current value on the next legitimate run
        e.set(2);
        flush();
        expect(e.log.at(-1)).toBe("effect:2:200");
        e.dispose();
    });

    it("dispose runs the last cleanup once and stops further runs", () => {
        const e = makeTwoPhase();
        flush();
        e.dispose();
        expect(e.log).toEqual(["compute:1", "effect:1:100", "cleanup:1"]);
        e.set(5);
        flush();
        expect(e.log).toEqual(["compute:1", "effect:1:100", "cleanup:1"]);
    });

    it("compute receives its previous value (None on the first run)", () => {
        const e = makeComputePrev();
        flush();
        e.set(5);
        flush();
        expect(e.log).toEqual(["prev:none", "effect:2", "prev:2", "effect:10"]);
        e.dispose();
    });

    it("defer = true skips the initial effect phase", () => {
        const e = makeDeferred();
        flush();
        expect(e.log).toEqual([]);
        e.set(2);
        flush();
        expect(e.log).toEqual(["effect:2"]);
        e.dispose();
    });
});

describe("createEffect with an error arm ({ effect, error })", () => {
    it("EffectErrorHandler receives the compute error and the effect's cleanup; recovery runs effect again", () => {
        const e = makeErrorBundle();
        flush();
        e.set(-1);
        flush();
        expect(e.log).toEqual(["effect:1", "error:negative:-1", "cleanup:1"]);
        e.set(4);
        flush();
        expect(e.log).toEqual(["effect:1", "error:negative:-1", "cleanup:1", "effect:4"]);
        e.dispose();
        expect(e.log.at(-1)).toBe("cleanup:4");
    });

    it("named effect/error arguments (ParamObject overload) route compute errors to error", () => {
        const e = makeErrorPojo();
        flush();
        e.set(13);
        flush();
        e.set(14);
        flush();
        expect(e.log).toEqual(["effect:1", "error:unlucky", "effect:14"]);
        e.dispose();
    });
});

describe("createRenderEffect", () => {
    it("runs its first effect phase synchronously and precedes user effects within a flush", () => {
        const e = makeRenderVsUser();
        expect(e.log).toEqual(["render:1"]);
        flush();
        expect(e.log).toEqual(["render:1", "user:1"]);
        e.set(2);
        flush();
        expect(e.log).toEqual(["render:1", "user:1", "render:2", "user:2"]);
        e.dispose();
    });

    it("returned cleanup runs before the next run and on dispose", () => {
        const e = makeRenderCleanup();
        e.set(2);
        flush();
        expect(e.log).toEqual(["render:1", "cleanup:1", "render:2"]);
        e.dispose();
        expect(e.log).toEqual(["render:1", "cleanup:1", "render:2", "cleanup:2"]);
    });
});

describe("createTrackedEffect", () => {
    it("tracks dynamically, cleans up before each re-run and on dispose", () => {
        const e = makeTracked();
        flush();
        expect(e.log).toEqual(["run:1"]);

        e.setOther(200); // not read while value is odd
        flush();
        expect(e.log).toEqual(["run:1"]);

        e.set(2);
        flush();
        expect(e.log).toEqual(["run:1", "cleanup:1", "run:2:200"]);

        e.setOther(300); // now a dependency
        flush();
        expect(e.log).toEqual(["run:1", "cleanup:1", "run:2:200", "cleanup:2", "run:2:300"]);

        e.dispose();
        expect(e.log.at(-1)).toBe("cleanup:2");
    });
});

describe("onSettled / onCleanup", () => {
    it("onSettled runs once after the first flush, never again on updates", () => {
        const e = makeLifecycle();
        expect(e.log).toEqual([]);
        flush();
        expect(count(e.log, "settled")).toBe(1);
        expect(count(e.log, "settled-with-cleanup")).toBe(1);
        expect(count(e.log, "effect:1")).toBe(1);
        e.set(2);
        flush();
        e.set(3);
        flush();
        expect(count(e.log, "settled")).toBe(1);
        expect(count(e.log, "settled-with-cleanup")).toBe(1);
        expect(e.log.filter(x => x.startsWith("effect:"))).toEqual(["effect:1", "effect:2", "effect:3"]);
        e.dispose();
    });

    it("dispose runs onCleanup and the cleanup returned by onSettled, exactly once each", () => {
        const e = makeLifecycle();
        flush();
        expect(e.log).not.toContain("root-cleanup");
        expect(e.log).not.toContain("settled-cleanup");
        e.dispose();
        expect(count(e.log, "root-cleanup")).toBe(1);
        expect(count(e.log, "settled-cleanup")).toBe(1);
    });

    // Solid 2 defers the previous run's cleanups (signals/src/core/core.ts recompute) until the new run commits,
    // so memo-cleanup:1 lands after memo:2 - unlike Solid 1.
    it("onCleanup inside a memo runs once per superseded run (after the new run commits) and on dispose", () => {
        const e = makeMemoCleanup();
        flush();
        e.set(2);
        flush();
        expect(e.log).toEqual(["memo:1", "memo:2", "memo-cleanup:1"]);
        e.dispose();
        expect(e.log).toEqual(["memo:1", "memo:2", "memo-cleanup:1", "memo-cleanup:2"]);
    });
});

describe("flush batching", () => {
    it("writes to two sources before one flush produce a single effect run with both values", () => {
        const b = makeBatching();
        flush();
        b.setA(10);
        b.setB(20);
        flush();
        expect(b.log).toEqual(["sum:3", "sum:30"]);
        b.dispose();
    });

    it("flush(fn) applies the callback's writes before returning and passes its result through", () => {
        const b = makeBatching();
        flush();
        const result = b.writeBothInFlush(2, 3);
        expect(result).toBe(6);
        expect(b.log).toEqual(["sum:3", "sum:5"]);
        b.dispose();
    });
});
