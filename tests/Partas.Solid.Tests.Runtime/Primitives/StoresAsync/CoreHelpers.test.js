import {describe, it, expect} from "vitest";
import {flush} from "../../helpers/index.js";
import {wrappable, equality, makeNotReady} from "./E-CoreHelpers.fs.jsx";

describe("isWrappable", () => {
    it("wraps plain objects and arrays, and leaves typed arrays, primitives, null, class instances and frozen objects", () => {
        expect(wrappable()).toEqual([true, true, false, false, false, false, false, false]);
    });
});

describe("isEqual", () => {
    it("is strict equality", () => {
        expect(equality()).toEqual([true, false, true, false]);
    });
});

describe("NotReadyError", () => {
    it("is what a read of a pending source throws, matched by an F# type test", () => {
        const h = makeNotReady();
        try {
            flush();
            expect(h.readUntracked()).toBe("not-ready");
        } finally {
            h.dispose();
        }
    });

    it("can be caught inside a memo", () => {
        const h = makeNotReady();
        try {
            flush();
            expect(h.readInMemo()).toBe("not-ready");
        } finally {
            h.dispose();
        }
    });

    it("carries the pending source", () => {
        const h = makeNotReady();
        try {
            flush();
            expect(h.hasSource()).toBe(true);
        } finally {
            h.dispose();
        }
    });
});
