import {describe, it, expect, beforeEach, afterEach} from "vitest";
import {flush} from "../../helpers/index.js";
import {makeRegistry, makeSession, makeBoard, busy, failed, idle} from "./B-Shapes.fs.jsx";

describe("map-like stores: pojo dictionary with dynamic keys", () => {
    let h;
    beforeEach(() => {
        h = makeRegistry();
        flush();
    });
    afterEach(() => h.dispose());

    it("a memo reading a missing key picks it up when the key is added", () => {
        expect(h.labelOfX()).toBe("missing");
        h.put("x", "Ex");
        flush();
        expect(h.labelOfX()).toBe("Ex");
    });

    it("adding and deleting keys reruns a memo over Object.keys", () => {
        expect(h.keyCount()).toBe(1);
        h.put("x", "Ex");
        flush();
        expect(h.keyCount()).toBe(2);
        h.remove("a");
        flush();
        expect(h.keyCount()).toBe(1);
        expect(Object.keys(h.registry.byId)).toEqual(["x"]);
    });

    it("deleting the key a memo reads flips it back to missing", () => {
        h.put("x", "Ex");
        flush();
        h.labelOfX();
        h.remove("x");
        flush();
        expect(h.labelOfX()).toBe("missing");
    });

    it("a nested write on one entry does not rerun the key-count memo or other entries' readers", () => {
        h.put("x", "Ex");
        flush();
        h.labelOfX();
        h.keyCount();
        const before = {...h.runs};
        h.hit("a");
        flush();
        h.labelOfX();
        h.keyCount();
        expect(h.runs.keyCount).toBe(before.keyCount);
        expect(h.runs.labelOfX).toBe(before.labelOfX);
        expect(h.registry.byId.a.hits).toBe(1);
    });

    it("an F# Map field: replacing it with Map.Add is visible to Map.tryFind in a memo", () => {
        expect(h.lookupK()).toBe(-1);
        h.mapAdd("k", 7);
        flush();
        expect(h.lookupK()).toBe(7);
        h.mapAdd("k", 8);
        flush();
        expect(h.lookupK()).toBe(8);
        h.mapRemove("k");
        flush();
        expect(h.lookupK()).toBe(-1);
    });

    it("an F# Map replacement that does not touch the read key still yields the same value", () => {
        h.mapAdd("k", 7);
        flush();
        h.lookupK();
        h.mapAdd("z", 1);
        flush();
        expect(h.lookupK()).toBe(7);
    });
});

describe("unions and options as store values", () => {
    let h;
    beforeEach(() => {
        h = makeSession();
        flush();
    });
    afterEach(() => h.dispose());

    it("union cases written through the setter are matched in memos and effects", () => {
        expect(h.describe()).toBe("idle");
        h.log.length = 0;
        h.setStatus(busy(40));
        flush();
        expect(h.describe()).toBe("busy 40%");
        expect(h.isBusy()).toBe(true);
        h.setStatus(failed("timeout"));
        flush();
        expect(h.describe()).toBe("failed: timeout");
        expect(h.isBusy()).toBe(false);
        h.setStatus(idle());
        flush();
        expect(h.describe()).toBe("idle");
        expect(h.log).toEqual(["busy 40%", "failed: timeout", "idle"]);
    });

    it("an option field: Some -> None -> Some", () => {
        expect(h.selectedText()).toBe("none");
        h.select(3);
        flush();
        expect(h.selectedText()).toBe("#3");
        h.select(undefined);
        flush();
        expect(h.selectedText()).toBe("none");
        h.select(0);
        flush();
        expect(h.selectedText()).toBe("#0");
    });

    it("an optional nested record becomes reactive once it is Some", () => {
        expect(h.nick()).toBe("anon");
        h.login("ann");
        flush();
        expect(h.nick()).toBe("ann");
        h.setAvatar("pic.png");
        flush();
        expect(h.nick()).toBe("ann@pic.png");
        h.setAvatar(undefined);
        flush();
        expect(h.nick()).toBe("ann");
        h.logout();
        flush();
        expect(h.nick()).toBe("anon");
        expect(h.log).toEqual(["idle", "anon", "ann", "ann@pic.png", "ann", "anon"]);
    });
});

describe("stores holding signals", () => {
    let h;
    beforeEach(() => {
        h = makeBoard();
        flush();
    });
    afterEach(() => h.dispose());

    it("accessors stored in a store are returned as plain functions", () => {
        expect(h.firstIsFunction()).toBe(true);
        expect(h.total()).toBe(11);
    });

    it("a memo calling a stored accessor tracks the underlying signal", () => {
        h.setA(5);
        flush();
        expect(h.total()).toBe(15);
        h.setB(20);
        flush();
        expect(h.total()).toBe(25);
    });

    it("swapping the stored accessor rewires the memo to the new signal", () => {
        h.rewire();
        flush();
        expect(h.total()).toBe(110);
        const runs = h.runs();
        h.setA(99);
        flush();
        h.total();
        expect(h.runs()).toBe(runs);
        h.setC(200);
        flush();
        expect(h.total()).toBe(210);
    });
});
