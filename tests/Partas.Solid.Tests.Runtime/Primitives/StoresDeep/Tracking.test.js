import {describe, it, expect, beforeEach, afterEach} from "vitest";
import {flush} from "../../helpers/index.js";
import {makeOrg, mkPerson, initialOrg, equalityReport, makeCart, line, lines, cart} from "./A-Tracking.fs.jsx";

let h;
beforeEach(() => {
    h = makeOrg();
    flush();
});
afterEach(() => h.dispose());

/** Pull every memo so lazy ones settle, then return a copy of the counters. */
function pullRuns() {
    for (const k of Object.keys(h.memos)) h.memos[k]();
    return {...h.runs};
}

/** Run `write`, flush, pull all memos and return which memo counters moved. */
function rerunsAfter(write) {
    const before = pullRuns();
    write();
    flush();
    const after = pullRuns();
    return Object.keys(after).filter(k => after[k] !== before[k]).sort();
}

describe("fine-grained tracking through nested records", () => {
    it("initial reads resolve every nested path", () => {
        expect(h.memos.city0()).toBe("London");
        expect(h.memos.lat0()).toBe(1);
        expect(h.memos.names()).toBe("Ada,Brian,Cleo");
        expect(h.memos.totalAge()).toBe(105);
        expect(h.memos.score1()).toBe(20);
        expect(h.log).toEqual(["city0:London", "company0:Co1", "names:Ada,Brian,Cleo", "totalAge:105"]);
    });

    it("a write to people[0].company.address.city reruns only the city memo", () => {
        expect(rerunsAfter(() => h.setCity(1, "Leeds"))).toEqual(["city0"]);
        expect(h.memos.city0()).toBe("Leeds");
    });

    it("a write to a sibling person's city reruns nothing that reads person 0", () => {
        expect(rerunsAfter(() => h.setCity(2, "Lyon"))).toEqual([]);
        expect(h.state.people[1].company.address.city).toBe("Lyon");
    });

    it("a five-level-deep write (geo.lat) reruns only the lat memo", () => {
        expect(rerunsAfter(() => h.setLat(1, 51.5))).toEqual(["lat0"]);
        expect(h.memos.lat0()).toBe(51.5);
    });

    it("replacing the geo record reruns the lat memo, not the city memo", () => {
        expect(rerunsAfter(() => h.replaceGeo(1, 9, 9))).toEqual(["lat0"]);
        expect(h.state.people[0].company.address.geo.lng).toBe(9);
    });

    it("a write to the root title is isolated from nested memos", () => {
        expect(rerunsAfter(() => h.setTitle("Globex"))).toEqual(["title"]);
    });

    it("copy-update of a nested record ({ c with name }) keeps the unchanged city observers quiet", () => {
        h.log.length = 0;
        const reruns = rerunsAfter(() => h.copyUpdateCompanyName(1, "NewCo"));
        expect(h.memos.company0()).toBe("NewCo");
        expect(h.state.people[0].company.address.city).toBe("London");
        // the company object was replaced, so readers through it recompute...
        expect(reruns).toContain("company0");
        // ...but the city value is unchanged, so the downstream effect does not fire again
        expect(h.log).toEqual(["company0:NewCo"]);
    });

    it("the copied nested record stays reactive after adoption", () => {
        h.copyUpdateCompanyName(1, "NewCo");
        flush();
        expect(rerunsAfter(() => h.setCity(1, "York"))).toEqual(["city0"]);
        expect(h.memos.city0()).toBe("York");
    });

    it("replacing a record with a structurally-equal copy fires no value-level effect", () => {
        h.log.length = 0;
        h.replaceCompanySame(1);
        flush();
        pullRuns();
        expect(h.log).toEqual([]);
        expect(h.memos.city0()).toBe("London");
    });

    it("an age write reruns the aggregate memo but not the names memo", () => {
        const reruns = rerunsAfter(() => h.birthday(2));
        expect(reruns).toEqual(["totalAge"]);
        expect(h.memos.totalAge()).toBe(106);
    });

    it("several setter calls before one flush coalesce", () => {
        const reruns = rerunsAfter(() => h.bulk());
        expect(reruns).toEqual(["title"]);
        expect(h.state.version).toBe(2);
        expect(h.state.title).toBe("Acme!");
    });
});

describe("array updates on the draft via ResizeArray / Array APIs", () => {
    it("Add pushes: count, names and aggregate rerun; person-0 value effects stay quiet", () => {
        h.log.length = 0;
        const reruns = rerunsAfter(() => h.addPerson(4, "Dan"));
        expect(reruns).toEqual(expect.arrayContaining(["count", "names", "totalAge"]));
        // Fable's ResizeArray indexer (`item(i, xs)`) bounds-checks against `xs.length`, so every
        // positional reader also tracks length and recomputes on a push. Values are unchanged, so
        // the effects downstream of those memos do not fire.
        expect(h.log).toEqual(["names:Ada,Brian,Cleo,Dan", "totalAge:125"]);
        expect(h.state.people.map(p => p.name)).toEqual(["Ada", "Brian", "Cleo", "Dan"]);
        expect(h.state.people[3].company.address.city).toBe("Oslo");
    });

    it("Insert at index 0 shifts rows and reruns the positional readers", () => {
        h.insertPersonAt(0, 9, "Zed");
        flush();
        expect(h.state.people.map(p => p.id)).toEqual([9, 1, 2, 3]);
        expect(h.memos.city0()).toBe("Oslo");
        expect(h.memos.name1()).toBe("Ada");
    });

    it("Insert in the middle leaves index 0 values (and their effects) unchanged", () => {
        h.log.length = 0;
        h.insertPersonAt(1, 9, "Zed");
        flush();
        expect(h.memos.city0()).toBe("London");
        expect(h.memos.name1()).toBe("Zed");
        expect(h.log).toEqual(["names:Ada,Zed,Brian,Cleo", "totalAge:125"]);
    });

    it("RemoveAt (FindIndex) splices out a person by id", () => {
        h.removePersonById(2);
        flush();
        expect(h.state.people.map(p => p.id)).toEqual([1, 3]);
        expect(h.memos.name1()).toBe("Cleo");
        expect(h.memos.totalAge()).toBe(77);
    });

    it("RemoveAll with a predicate removes every match in place", () => {
        h.removeAllOlderThan(30);
        flush();
        expect(h.state.people.map(p => p.name)).toEqual(["Brian"]);
        expect(h.memos.count()).toBe(1);
        expect(h.memos.city0()).toBe("Paris");
    });

    it("Sort with a comparison sorts the store array in place", () => {
        h.addPerson(4, "Aaron");
        flush();
        h.sortByName();
        flush();
        expect(h.state.people.map(p => p.name)).toEqual(["Aaron", "Ada", "Brian", "Cleo"]);
        expect(h.memos.names()).toBe("Aaron,Ada,Brian,Cleo");
    });

    it("assigning Seq.sortByDescending output re-orders and keeps the row data", () => {
        h.sortByAgeDesc();
        flush();
        expect(h.state.people.map(p => p.age)).toEqual([41, 36, 28]);
        expect(h.memos.city0()).toBe("Rome");
        expect(h.state.people[1].company.address.geo.lat).toBe(1);
    });

    it("Reverse reverses in place", () => {
        h.reversePeople();
        flush();
        expect(h.memos.names()).toBe("Cleo,Brian,Ada");
    });

    it("swapping two items through indexed assignment moves the rows", () => {
        const ada = h.state.people[0];
        h.swapFirstTwo();
        flush();
        expect(h.state.people.map(p => p.name)).toEqual(["Brian", "Ada", "Cleo"]);
        expect(h.state.people[1]).toBe(ada);
        expect(h.memos.city0()).toBe("Paris");
    });

    it("Clear empties the array", () => {
        h.clearPeople();
        flush();
        expect(h.state.people.length).toBe(0);
        expect(h.memos.count()).toBe(0);
        expect(h.memos.names()).toBe("");
        expect(h.memos.name1()).toBe("-");
    });

    it("AddRange appends several rows in one write", () => {
        h.log.length = 0;
        h.addRange(["Eve", "Finn"]);
        flush();
        expect(h.state.people.map(p => p.name)).toEqual(["Ada", "Brian", "Cleo", "Eve", "Finn"]);
        expect(h.log).toEqual(["names:Ada,Brian,Cleo,Eve,Finn", "totalAge:165"]);
    });

    it("an updater assigning a filtered list keeps survivors' proxies", () => {
        const cleo = h.state.people[2];
        h.keepWhere(p => p.name !== "Brian");
        flush();
        expect(h.state.people.map(p => p.name)).toEqual(["Ada", "Cleo"]);
        expect(h.state.people[1]).toBe(cleo);
        expect(h.memos.name1()).toBe("Cleo");
    });

    it("assigning copy-updated records ({ p with age }) updates the aggregate", () => {
        h.ageEveryone();
        flush();
        expect(h.state.people.map(p => p.age)).toEqual([37, 29, 42]);
        expect(h.memos.totalAge()).toBe(108);
        expect(h.memos.city0()).toBe("London");
    });

    it("nested array push/remove (tags) reruns only the tags memo", () => {
        expect(rerunsAfter(() => h.addTag(1, "vip"))).toEqual(["tags0"]);
        expect(h.memos.tags0()).toBe("t1,vip");
        expect(rerunsAfter(() => h.removeTag(1, "t1"))).toEqual(["tags0"]);
        expect(h.memos.tags0()).toBe("vip");
    });

    it("a tag write on another person does not rerun tags0", () => {
        expect(rerunsAfter(() => h.addTag(2, "x"))).toEqual([]);
        expect([...h.state.people[1].tags]).toEqual(["t2", "x"]);
    });
});

describe("F# array (int[]) fields", () => {
    it("int[] compiles to an Int32Array, which Solid does not wrap: per-index writes are inert", () => {
        // Fable emits numeric F# arrays as typed arrays; Solid 2's isWrappable rejects platform
        // objects, so the array is stored raw. The write lands, but nothing is notified.
        expect(h.state.scores).toBeInstanceOf(Int32Array);
        expect(rerunsAfter(() => h.setScore(1, 21))).toEqual([]);
        expect([...h.state.scores]).toEqual([10, 21, 30]);
        expect(h.memos.score1()).toBe(20);
    });

    it("Array.map produces a new array that replaces the field", () => {
        h.mapScores(x => x * 2);
        flush();
        expect([...h.state.scores]).toEqual([20, 40, 60]);
        expect(h.memos.score1()).toBe(40);
    });

    it("Array.sortInPlaceWith sorts the store array in place", () => {
        h.sortScoresInPlace();
        flush();
        expect([...h.state.scores]).toEqual([30, 20, 10]);
        expect(h.memos.score1()).toBe(20);
    });
});

describe("whole-state replacement and snapshot", () => {
    it("an updater returning a new Org replaces everything and memos follow", () => {
        const next = initialOrg();
        next.title = "Other";
        next.people = [mkPerson(7, "Gil", 50, "Madrid")];
        h.resetTo(next);
        flush();
        expect(h.memos.title()).toBe("Other");
        expect(h.memos.city0()).toBe("Madrid");
        expect(h.memos.count()).toBe(1);
        // still reactive after adoption
        h.setCity(7, "Seville");
        flush();
        expect(h.memos.city0()).toBe("Seville");
    });

    it("snapshot is a plain, non-proxied deep copy of the current state", () => {
        h.setCity(1, "Leeds");
        flush();
        const snap = h.snap();
        expect(snap).not.toBe(h.state);
        expect(snap.people).not.toBe(h.state.people);
        expect(Array.isArray(snap.people)).toBe(true);
        expect(snap.people[0].company.address.city).toBe("Leeds");
        h.setCity(1, "Bath");
        flush();
        expect(snap.people[0].company.address.city).toBe("Leeds");
    });
});

describe("F# structural equality over store proxies and snapshots", () => {
    let r;
    beforeEach(() => {
        r = equalityReport();
    });

    // Solid 2 store proxies report Object.prototype from getPrototypeOf (also for plain JS class
    // instances), so Fable's record Equals (sameConstructor check) rejects a live proxy. This is
    // upstream proxy behaviour, not Partas output: compare snapshots, which keep the prototype.
    it("a live store proxy of a record is NOT `=` to an equal record literal (compare snapshots)", () => {
        expect(r.proxyCompanyEqualsLiteral).toBe(false);
        expect(r.proxyGeoEqualsLiteral).toBe(false);
        expect(r.proxyGeoNotEqualOther).toBe(true);
    });

    it("snapshot of the store is `=` to a freshly built initial value", () => {
        expect(r.snapshotEqualsInitial).toBe(true);
        expect(r.snapshotPersonEqualsLiteral).toBe(true);
    });

    it("a snapshot record hashes like an equal literal; a live proxy is not `=` to it", () => {
        expect(r.hashesMatch).toBe(true);
        expect(r.proxyEqualsSnapshot).toBe(false);
    });

    it("snapshots taken before and after a write compare unequal", () => {
        expect(r.snapshotAfterWriteDiffers).toBe(true);
        expect(r.compareSnapshots).toBe(1);
    });
});

describe("reconcile on arrays of records", () => {
    let c;
    beforeEach(() => {
        c = makeCart();
        flush();
        for (const id of ["a", "b", "c"]) c.qtyOf(id);
    });
    afterEach(() => c.dispose());

    const runs = () => ({...c.runs});

    it("root reconcile (default id key) only reruns the memo of the row that changed", () => {
        const before = runs();
        c.reconcileRoot(cart("ann", [line("a", 1, "A-1"), line("b", 5, "B-1"), line("c", 3, "C-1")]));
        flush();
        ["a", "b", "c"].forEach(id => c.qtyOf(id));
        expect(c.qtyOf("b")).toBe(5);
        expect(c.runs.b).toBe(before.b + 1);
        expect(c.runs.a).toBe(before.a);
        expect(c.runs.c).toBe(before.c);
        expect(c.runs.owner).toBe(before.owner);
        expect(c.runs.count).toBe(before.count);
        expect(c.cart.lines[1].qty).toBe(5);
    });

    it("root reconcile keeps the proxy of a matched row and applies its field changes", () => {
        const rowA = c.cart.lines[0];
        c.reconcileRoot(cart("bea", [line("a", 9, "A-2"), line("c", 3, "C-1")]));
        flush();
        expect(c.cart.owner).toBe("bea");
        expect(c.cart.lines.map(l => l.id)).toEqual(["a", "c"]);
        expect(c.cart.lines[0]).toBe(rowA);
        expect(rowA.qty).toBe(9);
        expect(rowA.sku).toBe("A-2");
        expect(c.qtyOf("b")).toBe(-1);
    });

    it("a nested reconcile (reconcile next s.lines inside the updater) merges just the lines", () => {
        const rowC = c.cart.lines[2];
        c.reconcileLines(lines([line("c", 30, "C-1"), line("a", 1, "A-1"), line("d", 4, "D-1")]));
        flush();
        expect(c.cart.lines.map(l => l.id)).toEqual(["c", "a", "d"]);
        expect(c.cart.lines.find(l => l.id === "c")).toBe(rowC);
        expect(rowC.qty).toBe(30);
        expect(c.qtyOf("c")).toBe(30);
        expect(c.cart.owner).toBe("ann");
    });

    it("reconcile keyed by a named field (sku) treats an id change with a stable sku as the same row", () => {
        const rowB = c.cart.lines[1];
        c.reconcileBySku(lines([line("a", 1, "A-1"), line("bb", 2, "B-1"), line("c", 3, "C-1")]));
        flush();
        expect(c.cart.lines[1]).toBe(rowB);
        expect(rowB.id).toBe("bb");
        expect(c.qtyOf("b")).toBe(-1);
    });
});
