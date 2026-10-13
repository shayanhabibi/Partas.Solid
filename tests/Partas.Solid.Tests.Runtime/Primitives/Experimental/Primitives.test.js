// Signal options, owners (getOwner / createOwner / runWithOwner / Root.dispose / isDisposed),
// disposal ordering, createErrorBoundary and flush through the Partas bindings.
import {describe, it, expect} from "vitest";
import {flush} from "../../helpers/index.js";
import {
    makeNamedSignal,
    makeDerivedClamp,
    makeDerivedNamed,
    makeOwnerShape,
    makeManualOwner,
    makeAsyncOwner,
    makeOrdering,
    makeNestedEffect,
    makeBoundary,
    makeFlushProbe
} from "./B-Primitives.fs.jsx";

describe("createSignal options", () => {
    it("named optional args (name + equals) reach Solid: equal writes do not notify", () => {
        const s = makeNamedSignal();
        flush();
        expect(s.notifications()).toBe(1);
        s.set("HELLO");
        flush();
        expect(s.notifications()).toBe(1);
        expect(s.value()).toBe("Hello");
        s.set("World");
        flush();
        expect(s.notifications()).toBe(2);
        expect(s.value()).toBe("World");
        s.dispose();
    });

    it("function form + MixedSignalMemoOptions: derived, overridable, reset by the source", () => {
        const d = makeDerivedClamp();
        flush();
        expect(d.clamped()).toBe(5);
        d.setSource(50);
        flush();
        expect(d.clamped()).toBe(10);
        d.setClamped(3);
        flush();
        expect(d.clamped()).toBe(3);
        d.setSource(7);
        flush();
        expect(d.clamped()).toBe(7);
        expect(d.notifications()).toBe(4);
        d.dispose();
    });

    it("function form + equals: a recompute to the same value does not notify", () => {
        const d = makeDerivedClamp();
        d.setSource(20);
        flush();
        expect(d.notifications()).toBe(1);
        expect(d.clamped()).toBe(10);
        d.setSource(30); // still clamps to 10
        flush();
        expect(d.notifications()).toBe(1);
        d.dispose();
    });

    it("function form with named optional args (ParamObject) behaves the same", () => {
        const d = makeDerivedNamed();
        flush();
        d.setSource(12);
        flush();
        expect(d.clamped()).toBe(10);
        d.setClamped(1);
        flush();
        expect(d.clamped()).toBe(1);
        d.dispose();
    });
});

describe("owners", () => {
    it("getOwner in a root converts ToRoot; a memo's owner does not", () => {
        const o = makeOwnerShape();
        expect(o.rootIsRoot).toBe(true);
        expect(o.memoOwnerIsRoot()).toBe(false);
        expect(o.rootDisposed()).toBe(false);
        o.dispose();
        expect(o.rootDisposed()).toBe(true);
    });

    it("createOwner + runWithOwner returns the callback's value and owns its effects", () => {
        const o = makeManualOwner();
        expect(o.runResult).toBe(42);
        flush();
        o.set(2);
        flush();
        expect(o.log).toEqual(["effect:1", "effect:2"]);
        expect(o.isDisposed()).toBe(false);
        o.disposeAll();
        expect(o.isDisposed()).toBe(true);
        expect(o.log).toEqual(["effect:1", "effect:2", "child-cleanup", "owner-cleanup"]);
        o.set(3);
        flush();
        expect(o.log.at(-1)).toBe("owner-cleanup");
    });

    it("Root.dispose(false) tears down children and cleanups but leaves the owner alive", () => {
        const o = makeManualOwner();
        flush();
        o.disposeChildrenOnly();
        expect(o.isDisposed()).toBe(false);
        expect(o.log).toEqual(["effect:1", "child-cleanup", "owner-cleanup"]);
        o.set(2);
        flush();
        expect(o.log).toEqual(["effect:1", "child-cleanup", "owner-cleanup"]);
    });

    it("runWithOwner after an await re-enters the captured root", async () => {
        const a = makeAsyncOwner();
        flush();
        await new Promise(r => setTimeout(r, 0));
        expect(a.attachLater()).toBe("outside:false inside:true");
        flush();
        a.set(2);
        flush();
        expect(a.log).toEqual(["late:1", "late:2"]);
        a.dispose();
        expect(a.log).toEqual(["late:1", "late:2", "late-cleanup"]);
        a.set(3);
        flush();
        expect(a.log.at(-1)).toBe("late-cleanup");
    });

    it("dispose order: child roots (newest first), then own cleanups in registration order", () => {
        const o = makeOrdering();
        o.dispose();
        expect(o.log).toEqual(["child-b", "child-a", "parent-1", "parent-2"]);
    });

    it("a child root disposed early is not disposed again with its parent", () => {
        const o = makeOrdering();
        o.disposeFirstChild();
        expect(o.log).toEqual(["child-a"]);
        o.dispose();
        expect(o.log).toEqual(["child-a", "child-b", "parent-1", "parent-2"]);
    });

    it("disposing a child root stops only its effects; disposing the parent stops the rest", () => {
        const n = makeNestedEffect();
        flush();
        n.disposeChild();
        n.set(2);
        flush();
        expect(n.log).toEqual(["parent:1", "child:1", "parent:2"]);
        n.dispose();
        n.set(3);
        flush();
        expect(n.log).toEqual(["parent:1", "child:1", "parent:2"]);
    });
});

describe("createErrorBoundary (primitive)", () => {
    it("returns fn's value while it succeeds", () => {
        const b = makeBoundary();
        flush();
        expect(b.result()).toBe("ok:1");
        b.set(2);
        flush();
        expect(b.result()).toBe("ok:2");
        b.dispose();
    });

    it("switches to the fallback with the thrown error, and back after reset", () => {
        const b = makeBoundary();
        flush();
        b.set(-1);
        flush();
        expect(b.result()).toBe("error:bad:-1");
        b.set(4);
        b.reset();
        flush();
        expect(b.result()).toBe("ok:4");
        b.dispose();
    });
});

describe("flush", () => {
    it("flush(fn) batches the writes into one effect run and returns fn's result", () => {
        const f = makeFlushProbe();
        flush();
        expect(f.writeBoth(3, 4)).toBe("wrote:3,4");
        expect(f.log).toEqual(["1+2|0", "3+4|0"]);
        f.dispose();
    });

    it("untrack inside an effect compute is peeked, not subscribed", () => {
        const f = makeFlushProbe();
        flush();
        f.setPeek(9);
        flush();
        expect(f.log).toEqual(["1+2|0"]);
        f.writeBoth(5, 5);
        expect(f.log).toEqual(["1+2|0", "5+5|9"]);
        f.dispose();
    });

    it("writes are not visible to effects until a flush", () => {
        const f = makeFlushProbe();
        flush();
        f.setA(7);
        expect(f.log).toEqual(["1+2|0"]);
        flush();
        expect(f.log).toEqual(["1+2|0", "7+2|0"]);
        f.dispose();
    });
});
