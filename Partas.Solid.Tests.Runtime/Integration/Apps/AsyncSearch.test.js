import {describe, it, expect} from "vitest";
import {mount, input, click, settle, deferred, text} from "../../helpers/index.js";
import {SearchBox, SafeSearchBox, SafeSearchBoxRaw, PendingBadge, PendingBadgeMemo} from "./F-AsyncSearch.fs.jsx";

const $ = (root, sel) => root.querySelector(sel);
const $$ = (root, sel) => [...root.querySelectorAll(sel)];
const hits = c => $$(c, "li.hit").map(text);

/** A search backend whose every call returns a deferred the test resolves explicitly. */
function backend() {
    const pending = {};
    const calls = [];
    const search = q => {
        calls.push(q);
        pending[q] = deferred();
        return pending[q].promise;
    };
    return {search, calls, pending};
}

describe("Apps: async search box (Promise memo under <Loading>)", () => {
    it("shows the Loading fallback until the first result arrives", async () => {
        const b = backend();
        const {container: c} = mount(SearchBox, {search: b.search});
        expect(b.calls).toEqual([""]);
        expect(text($(c, "p.loading"))).toBe("Searching...");
        expect($(c, ".results")).toBeNull();
        // The input sits outside the boundary and is mounted immediately.
        expect($(c, "input.q")).not.toBeNull();

        b.pending[""].resolve([]);
        await settle();
        expect($(c, "p.loading")).toBeNull();
        expect($(c, ".results").className).toBe("results");
        expect(text($(c, "p.no-results"))).toBe("No results");
        expect(text($(c, ".count"))).toBe("0 results");
    });

    it("keeps stale content (marked via isPending) while a new query loads, instead of re-showing the fallback", async () => {
        const b = backend();
        const {container: c} = mount(SearchBox, {search: b.search});
        b.pending[""].resolve([]);
        await settle();

        input($(c, "input.q"), "a");
        expect(b.calls).toEqual(["", "a"]);
        await settle();
        expect($(c, "p.loading")).toBeNull();
        expect($(c, ".results").className).toBe("results stale");
        expect(text($(c, ".count"))).toBe("0 results");

        b.pending["a"].resolve(["apple", "avocado"]);
        await settle();
        expect($(c, ".results").className).toBe("results");
        expect(hits(c)).toEqual(["apple", "avocado"]);
        expect(text($(c, ".count"))).toBe("2 results");
    });

    it("holds the query-dependent UI outside the boundary until the async value settles (transition)", async () => {
        const b = backend();
        const {container: c} = mount(SearchBox, {search: b.search});
        b.pending[""].resolve([]);
        await settle();

        input($(c, "input.q"), "a");
        await settle();
        // Solid 2 entangles the query write with the pending fetch: the echo is not committed yet.
        expect(text($(c, ".typed"))).toBe("");
        b.pending["a"].resolve(["apple"]);
        await settle();
        expect(text($(c, ".typed"))).toBe("a");
    });

    it("ignores out-of-order responses: the latest query wins", async () => {
        const b = backend();
        const {container: c} = mount(SearchBox, {search: b.search});
        b.pending[""].resolve([]);
        await settle();

        input($(c, "input.q"), "ap");
        input($(c, "input.q"), "app");
        expect(b.calls).toEqual(["", "ap", "app"]);

        b.pending["app"].resolve(["apple"]);
        await settle();
        expect(hits(c)).toEqual(["apple"]);
        expect(text($(c, ".typed"))).toBe("app");

        // The superseded request resolves late and must not overwrite the newer result.
        b.pending["ap"].resolve(["apple", "apricot"]);
        await settle();
        expect(hits(c)).toEqual(["apple"]);
        expect(text($(c, ".count"))).toBe("1 results");
    });

    it("uses the initialQuery prop for the first fetch", async () => {
        const b = backend();
        const {container: c} = mount(SearchBox, {search: b.search, initialQuery: "kiwi"});
        expect(b.calls).toEqual(["kiwi"]);
        b.pending["kiwi"].resolve(["kiwi"]);
        await settle();
        expect(text($(c, ".typed"))).toBe("kiwi");
        expect(hits(c)).toEqual(["kiwi"]);
    });
});

describe("Apps: async search inside <Errored>", () => {
    function flaky() {
        let n = 0;
        const search = () => {
            n++;
            return n === 1 ? Promise.reject(new Error("boom")) : Promise.resolve([`hit${n}`]);
        };
        return {search, count: () => n};
    }

    it("a rejected search renders the error fallback (raw `fallback` property)", async () => {
        const s = flaky();
        const {container: c} = mount(SafeSearchBoxRaw, {search: s.search});
        expect(text($(c, "p.loading"))).toBe("Searching...");
        await settle();
        expect($(c, "p.loading")).toBeNull();
        expect(text($(c, ".error .message"))).toBe("Search failed: Error: boom");
        expect($(c, "ul.hits")).toBeNull();
    });

    it("Retry resets the boundary, re-runs the search and shows the recovered results", async () => {
        const s = flaky();
        const {container: c} = mount(SafeSearchBoxRaw, {search: s.search});
        await settle();
        click($(c, "button.retry"));
        await settle();
        expect(s.count()).toBe(2);
        expect($(c, ".error")).toBeNull();
        expect(hits(c)).toEqual(["hit2"]);

        input($(c, "input.q"), "z");
        await settle();
        expect(s.count()).toBe(3);
        expect(hits(c)).toEqual(["hit3"]);
    });

    // BUG: the `fallbackFn` inline setter is emitted as a literal `fallbackFn=` prop, which Errored ignores.
    it.fails("the fallbackFn helper renders the same error fallback", async () => {
        const s = flaky();
        const {container: c} = mount(SafeSearchBox, {search: s.search});
        await settle();
        expect(text($(c, ".error .message"))).toBe("Search failed: Error: boom");
    });
});

/** First load resolves immediately; the second is held on `next` until the test resolves it. */
function twoStepLoad() {
    const next = deferred();
    const keys = [];
    const load = k => {
        keys.push(k);
        return keys.length === 1 ? Promise.resolve(k.toUpperCase()) : next.promise;
    };
    return {load, next, keys};
}

/** Shared scenario: the value span is marked pending while the second key loads, then cleared. */
async function expectPendingCycle(Component) {
    const l = twoStepLoad();
    const {container: c} = mount(Component, {load: l.load});
    await settle();
    expect(text($(c, ".value"))).toBe("A");
    expect($(c, "span.value").className).toBe("value");
    click($(c, "button.next"));
    await settle();
    expect(l.keys).toEqual(["a", "aa"]);
    expect($(c, "span.loading")).toBeNull(); // stale content is kept, not replaced by the fallback
    expect($(c, "span.value").className).toBe("value pending");
    expect(text($(c, ".value"))).toBe("A");
    l.next.resolve("AA");
    await settle();
    expect($(c, "span.value").className).toBe("value");
    expect(text($(c, ".value"))).toBe("AA");
}

describe("Apps: isPending written inline in an attribute", () => {
    it("renders the Loading fallback, then the resolved value", async () => {
        const d = deferred();
        const {container: c} = mount(PendingBadge, {load: () => d.promise});
        expect(text($(c, "span.loading"))).toBe("...");
        expect($(c, "span.value")).toBeNull();
        d.resolve("A");
        await settle();
        expect($(c, "span.loading")).toBeNull();
        expect(text($(c, ".value"))).toBe("A");
    });

    // Control: the identical check hoisted into a memo keeps its thunk and passes. This pins the
    // it.fails below to the inline-thunk unwrapping rather than to rc.9 isPending semantics.
    it("control: isPending hoisted into a memo marks the value pending while the next key loads", async () => {
        await expectPendingCycle(PendingBadgeMemo);
    });

    // BUG: the plugin's ValueUnroller strips the `fun () -> ...` thunk, emitting isPending(value()).
    it.fails("marks the value as pending while the next key loads", async () => {
        await expectPendingCycle(PendingBadge);
    });
});
