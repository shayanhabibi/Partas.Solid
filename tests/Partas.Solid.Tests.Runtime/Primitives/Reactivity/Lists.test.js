// mapArray (keyed / unkeyed) and repeat through the Partas bindings.
// Note: Fable compiles `int array` to Int32Array, hence Array.from on index arrays.
import {describe, it, expect} from "vitest";
import {flush} from "../../helpers/index.js";
import {makeKeyedList, makeUnkeyedList, makeRepeated} from "./E-Lists.fs.jsx";

describe("mapArray (keyed by identity)", () => {
    it("maps each item once and exposes reactive indexes", () => {
        const l = makeKeyedList(["a", "b"]);
        expect(l.mapped()).toEqual(["A", "B"]);
        expect(l.mapCalls).toEqual(["a", "b"]);
        expect(Array.from(l.indexes())).toEqual([0, 1]);
        l.dispose();
    });

    it("inserting reuses existing rows, maps only the new item and shifts indexes", () => {
        const l = makeKeyedList(["a", "b"]);
        l.mapped();
        l.setItems(["c", "a", "b"]);
        flush();
        expect(l.mapped()).toEqual(["C", "A", "B"]);
        expect(l.mapCalls).toEqual(["a", "b", "c"]);
        expect(Array.from(l.indexes())).toEqual([0, 1, 2]);
        l.dispose();
    });

    it("removing an item disposes only that row's owner", () => {
        const l = makeKeyedList(["a", "b", "c"]);
        l.mapped();
        l.setItems(["c", "b"]);
        flush();
        expect(l.mapped()).toEqual(["C", "B"]);
        expect(l.disposed).toEqual(["a"]);
        expect(l.mapCalls).toEqual(["a", "b", "c"]);
        // surviving rows are reused and their index accessors move ("c" 2 -> 0, "b" stays 1)
        expect(Array.from(l.indexes())).toEqual([0, 1]);
        l.dispose();
        expect([...l.disposed].sort()).toEqual(["a", "b", "c"]);
    });

    it("a new array with the same items maps nothing new", () => {
        const l = makeKeyedList(["a", "b"]);
        l.mapped();
        l.setItems(["a", "b"]);
        flush();
        expect(l.mapped()).toEqual(["A", "B"]);
        expect(l.mapCalls).toEqual(["a", "b"]);
        expect(l.disposed).toEqual([]);
        l.dispose();
    });
});

describe("mapArrayUnkeyed (keyed = false)", () => {
    it("keeps rows per index and updates the item accessor in place", () => {
        const l = makeUnkeyedList(["x", "y"]);
        expect(l.mapped()).toEqual(["0:x", "1:y"]);
        expect(l.mapCalls()).toBe(2);
        l.setItems(["z", "y"]);
        flush();
        expect(l.mapped()).toEqual(["0:z", "1:y"]);
        expect(l.mapCalls()).toBe(2);
        l.setItems(["z", "y", "w"]);
        flush();
        expect(l.mapped()).toEqual(["0:z", "1:y", "2:w"]);
        expect(l.mapCalls()).toBe(3);
        l.dispose();
    });
});

describe("repeat", () => {
    it("maps each index once; growing maps only new indexes and shrinking maps nothing", () => {
        const r = makeRepeated(2);
        expect(r.values()).toEqual(["#0", "#1"]);
        r.setCount(4);
        flush();
        expect(r.values()).toEqual(["#0", "#1", "#2", "#3"]);
        expect(r.mapCalls).toEqual([0, 1, 2, 3]);
        r.setCount(1);
        flush();
        expect(r.values()).toEqual(["#0"]);
        expect(r.mapCalls).toEqual([0, 1, 2, 3]);
        r.dispose();
    });
});
