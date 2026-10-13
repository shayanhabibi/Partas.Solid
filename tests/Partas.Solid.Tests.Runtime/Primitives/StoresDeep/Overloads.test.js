import {describe, it, expect} from "vitest";
import {flush} from "../../helpers/index.js";
import {makeAnonStore, makeKeyed, makePlainOptimistic, makeTally} from "./D-Overloads.fs.jsx";

function using(h, fn) {
    try {
        flush();
        return fn(h);
    } finally {
        h.dispose();
    }
}

describe("anonymous records and F# lists in a store", () => {
    it("copy-update ({| p with size |}) replaces the root; the theme memo value is unchanged", () =>
        using(makeAnonStore(), h => {
            h.theme();
            h.setSize(14);
            flush();
            expect(h.size()).toBe(14);
            expect(h.theme()).toBe("light");
        }));

    it("copy-update of the read key reruns its memo", () =>
        using(makeAnonStore(), h => {
            h.setTheme("dark");
            flush();
            expect(h.theme()).toBe("dark");
        }));

    it("an F# list field: cons and tail are observed by a List.map memo", () =>
        using(makeAnonStore(), h => {
            expect(h.tagText()).toBe("A|B");
            h.consTag("z");
            flush();
            expect(h.tagText()).toBe("Z|A|B");
            h.dropHead();
            h.dropHead();
            flush();
            expect(h.tagText()).toBe("B");
        }));
});

describe("key overloads (per-item key function)", () => {
    it("createStore(fn, seed, key) calls the key per row and keeps matched rows' identity", () =>
        using(makeKeyed(), h => {
            expect(h.rows.map(r => r.label)).toEqual(["one", "two"]);
            const two = h.rows[1];
            h.setSource([{id: 2, label: "two!"}, {id: 3, label: "three"}]);
            flush();
            expect(h.rows.map(r => r.label)).toEqual(["two!", "three"]);
            expect(h.rows[0]).toBe(two);
            expect(h.keyCalls()).toBeGreaterThan(0);
        }));

    it("createOptimisticStore(fn, seed, key) derives the same keyed list", () =>
        using(makeKeyed(), h => {
            expect(h.optRows.map(r => r.id)).toEqual([1, 2]);
            h.setSource([{id: 1, label: "uno"}]);
            flush();
            expect(h.optRows.map(r => r.label)).toEqual(["uno"]);
        }));
});

describe("plain-value createOptimisticStore", () => {
    it("an ambient optimistic write shows for one flush and then reverts", () =>
        using(makePlainOptimistic(), h => {
            h.edit("typing");
            flush();
            expect(h.log).toEqual(["saved", "typing", "saved"]);
            expect(h.draft.text).toBe("saved");
        }));
});

describe("snapshot and deep over a derived store", () => {
    it("snapshot of a derived store is a plain copy that does not follow later derives", () =>
        using(makeTally(), h => {
            const snap = h.snap();
            expect([...snap.counts]).toEqual([1, 2]);
            h.setN(4);
            flush();
            expect([...h.live.counts]).toEqual([1, 2, 3, 4]);
            expect([...snap.counts]).toEqual([1, 2]);
        }));

    it("deep() over a derived store reruns when the derive rewrites a nested array", () =>
        using(makeTally(), h => {
            const runs = h.deepRuns();
            h.setN(3);
            flush();
            expect(h.deepRuns()).toBe(runs + 1);
        }));
});
