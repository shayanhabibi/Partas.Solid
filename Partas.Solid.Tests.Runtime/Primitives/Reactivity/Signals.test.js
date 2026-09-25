// createSignal through the Partas bindings: values, updaters, equality options, writable derived signals.
// Solid 2 rc.9: a setter only queues a microtask flush; reads and effects see the write after flush().
import {describe, it, expect} from "vitest";
import {flush} from "../../helpers/index.js";
import {
    makeIntSignal,
    makeCounter,
    makeOptionalSignal,
    makeLastDigitSignal,
    makeAlwaysNotifySignal,
    makePointSignal,
    makeWritableDerived,
    Point
} from "./A-Signals.fs.jsx";

describe("createSignal: value writes", () => {
    it("initial effect phase is queued until flush", () => {
        const s = makeIntSignal(1);
        expect(s.value()).toBe(1);
        expect(s.seen).toEqual([]);
        flush();
        expect(s.seen).toEqual([1]);
        s.dispose();
    });

    it("a write is not visible to reads or effects until flush", () => {
        const s = makeIntSignal(1);
        flush();
        s.set(2);
        expect(s.value()).toBe(1);
        expect(s.seen).toEqual([1]);
        flush();
        expect(s.value()).toBe(2);
        expect(s.seen).toEqual([1, 2]);
        s.dispose();
    });

    it("the queued microtask flush applies the write without an explicit flush()", async () => {
        const s = makeIntSignal(1);
        flush();
        s.set(5);
        await new Promise(r => setTimeout(r, 0));
        expect(s.value()).toBe(5);
        expect(s.seen).toEqual([1, 5]);
        s.dispose();
    });

    it("writing the same value (default reference equality) does not notify", () => {
        const s = makeIntSignal(3);
        flush();
        s.set(3);
        flush();
        expect(s.seen).toEqual([3]);
        s.dispose();
    });

    it("several writes before a flush collapse into one notification of the last value", () => {
        const s = makeIntSignal(0);
        flush();
        s.set(1);
        s.set(2);
        s.set(3);
        flush();
        expect(s.seen).toEqual([0, 3]);
        s.dispose();
    });

    it("disposing the owning root stops the effect", () => {
        const s = makeIntSignal(0);
        flush();
        s.dispose();
        s.set(9);
        flush();
        expect(s.seen).toEqual([0]);
    });
});

describe("createSignal: updater functions via Setter.Invoke / InvokeAndGet", () => {
    it("Invoke(fun prev -> ...) composes over pending writes in the same batch", () => {
        const c = makeCounter(1);
        c.increment();
        c.increment();
        c.increment();
        expect(c.count()).toBe(1);
        flush();
        expect(c.count()).toBe(4);
        c.dispose();
    });

    it("InvokeAndGet(handler) applies the update", () => {
        const c = makeCounter(1);
        c.addAndGet(5);
        flush();
        expect(c.count()).toBe(6);
        c.dispose();
    });

    // BUG: Setter.InvokeAndGet emits `setter(handler); return undefined` - Setter<'T> is typed 'T -> unit, so
    // `setter handler |> unbox<'T>` returns unit and Fable drops the value Solid's setter returns.
    it.fails("InvokeAndGet(handler) returns the new value the setter returned", () => {
        const c = makeCounter(1);
        expect(c.addAndGet(5)).toBe(6);
        c.dispose();
    });

    // BUG: Setter.InvokeAndGet(value) emits `setter(value); return undefined` for the same reason.
    it.fails("InvokeAndGet(value) returns the written value", () => {
        const c = makeCounter(1);
        expect(c.setAndGet(7)).toBe(7);
        c.dispose();
    });
});

describe("createSignal<'T>() (no initial value)", () => {
    it("starts as None/undefined and round-trips Some/None", () => {
        const o = makeOptionalSignal();
        expect(o.current()).toBeUndefined();
        expect(o.isSet()).toBe(false);
        o.set("a");
        flush();
        expect(o.current()).toBe("a");
        expect(o.isSet()).toBe(true);
        o.clear();
        flush();
        expect(o.current()).toBeUndefined();
        expect(o.isSet()).toBe(false);
        o.dispose();
    });
});

describe("createSignal: equals option", () => {
    it("custom comparator (named optional arg): equal writes are dropped, unequal ones notify", () => {
        const s = makeLastDigitSignal(1);
        flush();
        s.set(11); // same last digit -> equal
        flush();
        expect(s.value()).toBe(1);
        expect(s.seen).toEqual([1]);
        s.set(12);
        flush();
        expect(s.value()).toBe(12);
        expect(s.seen).toEqual([1, 12]);
        s.set(22);
        flush();
        expect(s.value()).toBe(12);
        expect(s.seen).toEqual([1, 12]);
        s.dispose();
    });

    it("comparator that is always false (SignalOptions POJO) notifies on identical writes", () => {
        const s = makeAlwaysNotifySignal(1);
        flush();
        s.set(1);
        flush();
        s.set(1);
        flush();
        expect(s.seen).toEqual([1, 1, 1]);
        s.dispose();
    });

    it("default equality is by reference: a structurally equal F# record still notifies", () => {
        const s = makePointSignal(false);
        flush();
        s.set(new Point(0, 0));
        flush();
        expect(s.seen.length).toBe(2);
        s.dispose();
    });

    it("F# structural equality as comparator suppresses equal records", () => {
        const s = makePointSignal(true);
        flush();
        s.set(new Point(0, 0));
        flush();
        expect(s.seen.length).toBe(1);
        s.set(new Point(1, 0));
        flush();
        expect(s.seen.map(p => [p.x, p.y])).toEqual([[0, 0], [1, 0]]);
        s.dispose();
    });
});

describe("createSignal(fn): writable derived signal", () => {
    it("derives from its source, can be overwritten, and re-derives when the source changes", () => {
        const w = makeWritableDerived();
        expect(w.derived()).toBe(10);
        w.setDerived(99);
        flush();
        expect(w.derived()).toBe(99);
        w.setSource(2);
        flush();
        expect(w.derived()).toBe(20);
        w.dispose();
    });
});
