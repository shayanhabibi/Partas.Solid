import {describe, it, expect} from "vitest";
import {flush, settle, deferred} from "../../helpers/index.js";
import {makeSummary, makeFilteredView, makeSelection, makeOptimisticAccount} from "./C-Derived.fs.jsx";

const tick = () => new Promise(r => setTimeout(r, 0));

function using(h, fn) {
    return (async () => {
        try {
            flush();
            await fn(h);
        } finally {
            h.dispose();
        }
    })();
}

describe("fn-form createStore derived from another store", () => {
    it("derived counts follow source toggles", () =>
        using(makeSummary(), h => {
            const s = h.summary;
            const open = () => s.pending;
            expect(open()).toBe(2);
            expect(s.closed).toBe(1);
            expect([...s.owners]).toEqual(["ann", "bob"]);
            h.toggle(1);
            flush();
            expect(open()).toBe(1);
            expect(s.closed).toBe(2);
        }));

    it("adding a task with a new owner extends the owners list", () =>
        using(makeSummary(), h => {
            h.add(4, "test", "cy");
            flush();
            expect([...h.summary.owners]).toEqual(["ann", "bob", "cy"]);
            expect(h.summary.closed).toBe(1);
        }));

    it("the derive re-runs for writes it read (done) but not for titles it never read", () =>
        using(makeSummary(), h => {
            const before = h.derives();
            expect(before).toBeGreaterThan(0);
            h.retitle(1, "plan!");
            flush();
            // the derive iterates whole task records via Seq.filter, so it tracked their shape;
            // titles were never read, so a title write must not re-derive
            expect(h.derives()).toBe(before);
            h.toggle(1);
            flush();
            expect(h.summary.closed).toBe(2);
            expect(h.derives()).toBe(before + 1);
        }));
});

describe("fn-form createStore returning a keyed list (ProjectionOptions key = \"id\")", () => {
    it("filters the source by a signal", () =>
        using(makeFilteredView(), h => {
            expect(h.view.map(v => v.label)).toEqual(["plan", "ship"]);
            h.setOwner("bob");
            flush();
            expect(h.view.map(v => v.label)).toEqual(["build"]);
        }));

    it("rows that survive a re-derive keep their proxy identity (keyed by id)", () =>
        using(makeFilteredView(), h => {
            const ship = h.view[1];
            h.retitle(1, "plan v2");
            flush();
            expect(h.view.map(v => v.label)).toEqual(["plan v2", "ship"]);
            expect(h.view[1]).toBe(ship);
        }));

    it("an unrelated owner filter change and back restores the rows", () =>
        using(makeFilteredView(), h => {
            h.setOwner("nobody");
            flush();
            expect(h.view.length).toBe(0);
            h.setOwner("ann");
            flush();
            expect(h.view.map(v => v.id)).toEqual([1, 3]);
        }));
});

describe("selection projection (createSelector replacement)", () => {
    const ids = [1, 2, 3, 4];

    it("marks exactly the selected row", () =>
        using(makeSelection(ids), h => {
            expect(ids.map(id => h.rowMemos[id]())).toEqual([true, false, false, false]);
        }));

    it("changing the selection reruns only the two rows that flip", () =>
        using(makeSelection(ids), h => {
            ids.forEach(id => h.rowMemos[id]());
            const before = {...h.rowRuns};
            h.select(3);
            flush();
            ids.forEach(id => h.rowMemos[id]());
            expect(ids.map(id => h.rowMemos[id]())).toEqual([false, false, true, false]);
            expect(h.rowRuns[1]).toBe(before[1] + 1);
            expect(h.rowRuns[3]).toBe(before[3] + 1);
            expect(h.rowRuns[2]).toBe(before[2]);
            expect(h.rowRuns[4]).toBe(before[4]);
        }));

    it("re-selecting the same id reruns nothing", () =>
        using(makeSelection(ids), h => {
            ids.forEach(id => h.rowMemos[id]());
            const before = {...h.rowRuns};
            h.select(1);
            flush();
            ids.forEach(id => h.rowMemos[id]());
            expect({...h.rowRuns}).toEqual(before);
        }));

    it("the projection store exposes the selection map", () =>
        using(makeSelection(ids), h => {
            h.select(2);
            flush();
            expect(h.isSelected["2"]).toBe(true);
            expect(h.isSelected["1"]).toBe(false);
        }));
});

describe("createOptimisticStore rollback via action (nested record)", () => {
    it("the optimistic nested write shows in flight, then settles to the server value", () =>
        using(makeOptimisticAccount(), async h => {
            expect(h.log).toEqual(["light/12"]);
            const reply = deferred();
            const done = h.applyTheme("dark", reply.promise);
            flush();
            await tick();
            expect(h.view.settings.theme).toBe("dark");
            expect(h.view.settings.fontSize).toBe(16);
            expect(h.server.settings.theme).toBe("light");

            reply.resolve("dark-hc");
            await done;
            await settle();
            expect(h.server.settings.theme).toBe("dark-hc");
            // the overlay is discarded: font size re-derives from the server (12), theme is confirmed
            expect(h.view.settings.theme).toBe("dark-hc");
            expect(h.view.settings.fontSize).toBe(12);
            expect(h.log.at(-1)).toBe("dark-hc/12");
        }));

    it("a failed action rolls every optimistic nested field back", () =>
        using(makeOptimisticAccount(), async h => {
            const reply = deferred();
            const done = h.applyTheme("dark", reply.promise);
            flush();
            await tick();
            expect(h.view.settings.theme).toBe("dark");

            const boom = new Error("nope");
            reply.reject(boom);
            await expect(done).rejects.toBe(boom);
            await settle();
            expect(h.view.settings.theme).toBe("light");
            expect(h.view.settings.fontSize).toBe(12);
            expect(h.server.settings.theme).toBe("light");
            expect(h.log.at(-1)).toBe("light/12");
        }));
});
