// createMemo through the Partas bindings: chains, laziness, equality dedupe, diamonds, dynamic dependencies.
import {describe, it, expect} from "vitest";
import {flush} from "../../helpers/index.js";
import {
    makeChain,
    makeLazyProbe,
    makeDedupe,
    makeCoarse,
    makeCoarseAnnotated,
    makeCoarseNamed,
    makeDiamond,
    makeSwitching,
    makeAccumulator
} from "./B-Memos.fs.jsx";

const count = (log, entry) => log.filter(e => e === entry).length;

describe("createMemo: derived chains", () => {
    it("computes eagerly on creation, in dependency order", () => {
        const c = makeChain();
        expect(c.log).toEqual(["doubled", "plusOne"]);
        expect(c.doubled()).toBe(2);
        expect(c.plusOne()).toBe(3);
        // reading does not recompute a clean memo
        expect(c.log).toEqual(["doubled", "plusOne"]);
        c.dispose();
    });

    it("propagates a source write through the chain once per memo", () => {
        const c = makeChain();
        c.setA(5);
        flush();
        expect(c.plusOne()).toBe(11);
        expect(c.doubled()).toBe(10);
        expect(c.log).toEqual(["doubled", "plusOne", "doubled", "plusOne"]);
        c.dispose();
    });

    it("a pending write is not visible through the memo until flush", () => {
        const c = makeChain();
        c.setA(5);
        expect(c.plusOne()).toBe(3);
        flush();
        expect(c.plusOne()).toBe(11);
        c.dispose();
    });

    it("disposed memos stop recomputing", () => {
        const c = makeChain();
        c.dispose();
        const before = c.log.length;
        c.setA(7);
        flush();
        expect(c.log.length).toBe(before);
        // a disposed memo keeps serving its last value instead of recomputing on read
        expect(c.plusOne()).toBe(3);
        expect(c.log.length).toBe(before);
    });
});

describe("createMemo: lazy option", () => {
    it("eager memo runs at creation; lazy = true memo waits for its first read", () => {
        const p = makeLazyProbe();
        expect(p.eagerRuns()).toBe(1);
        expect(p.lazyRuns()).toBe(0);
        expect(p.lazyMemo()).toBe(203);
        expect(p.lazyRuns()).toBe(1);
        expect(p.lazyMemo()).toBe(203);
        expect(p.lazyRuns()).toBe(1);
        p.dispose();
    });

    it("an unread lazy memo does not recompute on source changes", () => {
        const p = makeLazyProbe();
        p.setSource(4);
        flush();
        expect(p.lazyRuns()).toBe(0);
        expect(p.eager()).toBe(104);
        expect(p.lazyMemo()).toBe(204);
        expect(p.lazyRuns()).toBe(1);
        p.dispose();
    });
});

describe("createMemo: equality dedupe", () => {
    it("recomputing to an equal value does not re-run downstream memos or effects", () => {
        const d = makeDedupe();
        flush();
        expect(d.effectSeen).toEqual(["ODD"]);
        expect([d.parityRuns(), d.downstreamRuns()]).toEqual([1, 1]);

        d.setN(3);
        flush();
        expect(d.parity()).toBe("odd");
        expect([d.parityRuns(), d.downstreamRuns()]).toEqual([2, 1]);
        expect(d.effectSeen).toEqual(["ODD"]);

        d.setN(4);
        flush();
        expect([d.parityRuns(), d.downstreamRuns()]).toEqual([3, 2]);
        expect(d.effectSeen).toEqual(["ODD", "EVEN"]);
        d.dispose();
    });

    const coarseSpec = make => () => {
        const c = make();
        flush();
        expect(c.effectSeen).toEqual([0]);
        c.setRaw(3); // |3-0| < 5: equal, memo keeps 0
        flush();
        expect(c.coarse()).toBe(0);
        c.setRaw(7);
        flush();
        expect(c.coarse()).toBe(7);
        c.setRaw(9); // |9-7| < 5
        flush();
        expect(c.coarse()).toBe(7);
        expect(c.effectSeen).toEqual([0, 7]);
        c.dispose();
    };

    it("custom equals via named optional argument (ParamObject overload)", coarseSpec(makeCoarseNamed));

    // BUG: createMemo(compute, MemoOptions(...)) emits createMemo(compute, { loadingValue: { equals } }) - the options
    // POJO is wrapped as `loadingValue` (the ParamObject(1) `createMemo(compute: 'T -> 'T, loadingValue: 'T)`
    // overload's shape), so every MemoOptions field (equals, lazy, name...) is silently ignored.
    it.fails("custom equals via MemoOptions POJO", coarseSpec(makeCoarse));

    // BUG: same as above; annotating `prev: int option` (or passing `options = ...` by name) does not avoid it.
    it.fails("custom equals via MemoOptions POJO with an annotated compute", coarseSpec(makeCoarseAnnotated));
});

describe("createMemo: diamond dependencies", () => {
    it("sum over two branches of one source recomputes once per change, never torn", () => {
        const d = makeDiamond();
        flush();
        expect(d.log).toEqual(["left", "right", "sum", "effect"]);
        expect(d.sum()).toBe(12);

        d.log.length = 0;
        d.setA(2);
        flush();
        expect(count(d.log, "left")).toBe(1);
        expect(count(d.log, "right")).toBe(1);
        expect(count(d.log, "sum")).toBe(1);
        expect(count(d.log, "effect")).toBe(1);
        expect(d.log.indexOf("sum")).toBeGreaterThan(d.log.indexOf("left"));
        expect(d.log.indexOf("sum")).toBeGreaterThan(d.log.indexOf("right"));
        expect(d.sumSeen).toEqual(["2+10", "3+20"]);
        expect(d.sum()).toBe(23);
        d.dispose();
    });
});

describe("createMemo: conditional dependency switching", () => {
    it("only tracks the branch it last read", () => {
        const s = makeSwitching();
        expect(s.runs()).toBe(1);
        expect(s.picked()).toBe("L1");

        s.setRight("R2"); // not a dependency yet
        flush();
        expect(s.runs()).toBe(1);

        s.setUseLeft(false);
        flush();
        expect(s.runs()).toBe(2);
        expect(s.picked()).toBe("R2");

        s.setLeft("L2"); // dropped dependency
        flush();
        expect(s.runs()).toBe(2);

        s.setRight("R3");
        flush();
        expect(s.runs()).toBe(3);
        expect(s.picked()).toBe("R3");
        s.dispose();
    });
});

describe("createMemo: previous value", () => {
    it("compute receives None first, then its last value", () => {
        const a = makeAccumulator();
        expect(a.total()).toBe(5);
        a.add(3);
        flush();
        expect(a.total()).toBe(8);
        a.add(10);
        flush();
        expect(a.total()).toBe(18);
        expect(a.prevSeen).toEqual(["none", "5", "8"]);
        a.dispose();
    });
});
