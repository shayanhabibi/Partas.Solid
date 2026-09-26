import {describe, it, expect} from "vitest";
import {flush} from "../../helpers/index.js";
import {createSignal} from "solid-js";
import {
    buttonProps,
    omitSizeVariant,
    omitDollarKeys,
    omitWithSymbolKey,
    omitOverStore,
    mergeDefaults,
    mergeSingle,
    mergeWithFalsy,
    mergeOverStore,
    isStaticReport
} from "./E-Utils.fs.jsx";

describe("omit", () => {
    it("key-list overload (ParamArray) hides exactly the listed keys", () => {
        const rest = omitSizeVariant(buttonProps());
        expect(Object.keys(rest).sort()).toEqual(["disabled", "label"]);
        expect(rest.label).toBe("Save");
        expect(rest.disabled).toBe(false);
        expect("size" in rest).toBe(false);
        expect(rest.variant).toBeUndefined();
    });

    it("predicate overload hides every key the predicate accepts", () => {
        const rest = omitDollarKeys();
        expect(Object.keys(rest)).toEqual(["id", "label"]);
        expect("$theme" in rest).toBe(false);
        expect(rest.$slot).toBeUndefined();
        expect({...rest}).toEqual({id: "x", label: "L"});
    });

    it("predicate overload over an object with a symbol-keyed prop: string reads still work", () => {
        const sym = Symbol("slot");
        const rest = omitWithSymbolKey(sym);
        expect(rest.id).toBe("x");
        expect("$hidden" in rest).toBe(false);
    });

    // BUG: Bindings.omit(obj, hidden: string -> bool) types the predicate over strings, but rc.9
    // omit also calls it with symbol keys (utils.ts:1100 `keyof T & (string | symbol)`), so an
    // idiomatic `k.StartsWith "$"` predicate makes key enumeration (Reflect.ownKeys, {...rest}, the
    // spread Solid does when forwarding props) throw TypeError on any source carrying a symbol key.
    it.fails("predicate overload over an object with a symbol-keyed prop enumerates its keys", () => {
        const sym = Symbol("slot");
        const rest = omitWithSymbolKey(sym);
        const keys = Reflect.ownKeys(rest);
        expect(keys).toContain("id");
        expect(keys).toContain(sym);
        expect(keys).not.toContain("$hidden");
    });

    it("omit over a store is a live view that sees committed writes", () => {
        const h = omitOverStore();
        try {
            expect(h.rest.n).toBe(1);
            h.bump();
            expect(h.rest.n).toBe(1);
            flush();
            expect(h.rest.n).toBe(2);
        } finally {
            h.dispose();
        }
    });

    it("omit views silently ignore writes and deletes (set/deleteProperty traps return true)", () => {
        const rest = omitSizeVariant(buttonProps());
        expect(() => {
            rest.label = "changed";
            delete rest.disabled;
        }).not.toThrow();
        expect(rest.label).toBe("Save");
        expect(rest.disabled).toBe(false);
    });
});

describe("merge", () => {
    it("later sources win; keys from every source are present", () => {
        const merged = mergeDefaults({size: "sm", label: "OK"});
        expect(merged.size).toBe("sm");
        expect(merged.label).toBe("OK");
        expect(merged.variant).toBe("ghost");
        expect(Object.keys(merged).sort()).toEqual(["label", "size", "variant"]);
    });

    it("is a live view: reads go through to the sources at access time", () => {
        const props = {size: "sm"};
        const merged = mergeDefaults(props);
        props.size = "xl";
        expect(merged.size).toBe("xl");
    });

    it("reads through getters on the sources (reactive props)", () => {
        const [size, setSize] = createSignal("sm");
        const merged = mergeDefaults({
            get size() {
                return size();
            }
        });
        expect(merged.size).toBe("sm");
        setSize("lg");
        flush();
        expect(merged.size).toBe("lg");
    });

    it("a single source is returned as-is (same reference)", () => {
        const props = {a: 1};
        expect(mergeSingle(props)).toBe(props);
    });

    it("falsy sources (null, false) are skipped", () => {
        const props = {a: 1};
        const merged = mergeWithFalsy(props);
        expect(merged).toBe(props);
        expect(merged.a).toBe(1);
    });

    it("merge over a store overrides defaults and follows store writes", () => {
        const h = mergeOverStore();
        try {
            expect(h.merged.n).toBe(1);
            expect(h.merged.extra).toBe("e");
            h.setN(9);
            flush();
            expect(h.merged.n).toBe(9);
        } finally {
            h.dispose();
        }
    });
});

describe("isStatic", () => {
    const r = isStaticReport();

    it("plain (anonymous record) data property and absent key are static", () => {
        expect(r.plainAs).toBe(true);
        expect(r.plainMissing).toBe(true);
    });

    it("an F# record field (own data property on a Fable class instance) is static", () => {
        expect(r.recordField).toBe(true);
    });

    it("store keys, and views where a store owns the key, are not static", () => {
        expect(r.storeKey).toBe(false);
        expect(r.mergeStoreOverPlain).toBe(false);
        expect(r.omitStore).toBe(false);
    });

    it("a plain literal shadowing a store in merge is static; omit over plain stays static", () => {
        expect(r.mergePlainOverStore).toBe(true);
        expect(r.omitPlain).toBe(true);
    });

    it("F# object-expression getters live on the prototype, so isStatic reports them static", () => {
        // Fable compiles `{ new IDynamicProps with member _.dyn = sig() }` to a class instance with
        // prototype getters. isStatic only inspects OWN descriptors (store/utils.ts:260), so a
        // reactive getter authored this way is classified static. Upstream semantics, not a Partas
        // bug, but a hazard when passing object expressions as props.
        expect(r.getterDyn).toBe(true);
    });
});
