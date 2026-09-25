import {describe, it, expect} from "vitest";
import {flush} from "../../helpers/index.js";
import {makeReconcileStore, todo, todoList} from "./B-Reconcile.fs.jsx";

function withStore(fn) {
    const h = makeReconcileStore();
    try {
        flush();
        return fn(h);
    } finally {
        h.dispose();
    }
}

const swapped = () => todoList([todo(2, "two", false), todo(1, "one", false)]);

describe("reconcile", () => {
    it("default key ('id'): reorders rows; captured row proxies keep reading their own key", () =>
        withStore(h => {
            const one = h.todos[0];
            const two = h.todos[1];
            h.byDefaultKey(swapped());
            flush();
            // rc.9 adopts the incoming raw rows on a keyed array reorder (upstream reconcile.test.ts
            // "Reconcile reorder a keyed array" asserts snapshot identity with the NEW rows), so
            // proxy identity through the array is not a contract; key-matched captures stay valid.
            expect(h.todos.map(t => t.id)).toEqual([2, 1]);
            expect(h.todos.map(t => t.title)).toEqual(["two", "one"]);
            expect(one.id).toBe(1);
            expect(one.title).toBe("one");
            expect(two.title).toBe("two");
            expect(h.log).toEqual(["one", "two"]);
        }));

    it("named key overload behaves like the default key", () =>
        withStore(h => {
            const one = h.todos[0];
            h.byNamedKey(swapped());
            flush();
            expect(h.todos.map(t => t.id)).toEqual([2, 1]);
            expect(one.title).toBe("one");
            expect(h.todos[0].id).toBe(2);
            expect(h.log).toEqual(["one", "two"]);
        }));

    it("key-function overload is called per item (not with the whole array) and keys by it", () =>
        withStore(h => {
            const one = h.todos[0];
            h.byKeyFn(swapped());
            flush();
            expect(h.todos.map(t => t.id)).toEqual([2, 1]);
            // A positional reconcile would rewrite slot 0 in place and `one` would read "two"; this
            // separates a working per-item key function from one that keys everything as undefined.
            expect(one.title).toBe("one");
            expect(h.keyFnCalls()).toBeGreaterThanOrEqual(2);
            expect(h.log).toEqual(["one", "two"]);
        }));

    it("positional (null key): items keep their slot and take the new field values", () =>
        withStore(h => {
            const slot0 = h.todos[0];
            h.positional(swapped());
            flush();
            expect(h.todos[0]).toBe(slot0);
            expect(h.todos[0].id).toBe(2);
            expect(h.todos[0].title).toBe("two");
            expect(slot0.title).toBe("two");
            expect(h.log).toEqual(["one", "two"]);
        }));

    it("an unchanged field on a matched item does not notify", () =>
        withStore(h => {
            const one = h.todos[0];
            h.byDefaultKey(todoList([todo(1, "one", true), todo(2, "two", false)]));
            flush();
            expect(h.todos[0]).toBe(one);
            expect(h.todos[0].completed).toBe(true);
            // the title effect on index 0 saw no change
            expect(h.log).toEqual(["one"]);
        }));

    it("adds and removes keyed items", () =>
        withStore(h => {
            const two = h.todos[1];
            h.byDefaultKey(todoList([todo(2, "two", false), todo(3, "three", false)]));
            flush();
            expect(h.todos.length).toBe(2);
            expect(h.todos.map(t => t.id)).toEqual([2, 3]);
            expect(two.title).toBe("two");
            expect(h.todos[1].title).toBe("three");
            expect(h.log).toEqual(["one", "two"]);
        }));
});
