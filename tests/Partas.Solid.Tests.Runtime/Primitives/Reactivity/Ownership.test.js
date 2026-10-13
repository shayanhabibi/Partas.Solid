// Ownership through the Partas bindings: createRoot, getOwner/runWithOwner, untrack, createReaction, createContext.
import {describe, it, expect} from "vitest";
import {flush} from "../../helpers/index.js";
import {
    makeOwnerProbe,
    makeNestedRoots,
    makeUntracked,
    makeReaction,
    probeContext,
  makeObserverProbe
} from "./D-Ownership.fs.jsx";

describe("getOwner / runWithOwner", () => {
    it("getOwner is None outside a root and Some inside", () => {
        const p = makeOwnerProbe();
        expect(p.ownerOutside).toBe(false);
        expect(p.ownerInside).toBe(true);
        p.dispose();
    });

    it("onCleanup registered later through runWithOwner runs when the root is disposed", () => {
        const p = makeOwnerProbe();
        p.registerLater("a");
        p.registerLater("b");
        expect(p.log).toEqual([]);
        p.dispose();
        expect([...p.log].sort()).toEqual(["cleanup:a", "cleanup:b"]);
    });

    it("an effect created later through runWithOwner is owned by the root", () => {
        const p = makeOwnerProbe();
        p.effectLater();
        flush();
        p.setValue(2);
        flush();
        expect(p.log).toEqual(["late-effect:1", "late-effect:2"]);
        p.dispose();
        p.setValue(3);
        flush();
        expect(p.log).toEqual(["late-effect:1", "late-effect:2"]);
    });
});

describe("createRoot nesting", () => {
    it("a nested root can be disposed on its own", () => {
        const r = makeNestedRoots();
        r.disposeChild();
        expect(r.log).toEqual(["child"]);
        r.disposeParent();
        expect(r.log).toEqual(["child", "parent"]);
    });

    it("disposing the parent root also disposes a nested root", () => {
        const r = makeNestedRoots();
        r.disposeParent();
        expect([...r.log].sort()).toEqual(["child", "parent"]);
    });
});

describe("untrack", () => {
    it("an untracked read inside a memo is not a dependency, but sees the latest value", () => {
        const u = makeUntracked();
        expect(u.combined()).toBe("1/10");
        expect(u.runs()).toBe(1);
        u.setPeeked(20);
        flush();
        expect(u.runs()).toBe(1);
        expect(u.combined()).toBe("1/10");
        u.setTracked(2);
        flush();
        expect(u.runs()).toBe(2);
        expect(u.combined()).toBe("2/20");
        u.dispose();
    });
});

describe("createReaction", () => {
    it("does nothing until armed", () => {
        const r = makeReaction();
        r.setA(1);
        flush();
        expect(r.fired()).toBe(0);
        r.dispose();
    });

    it("fires once per arm on the next change of what was tracked", () => {
        const r = makeReaction();
        r.arm();
        expect(r.fired()).toBe(0);
        r.setA(1);
        flush();
        expect(r.fired()).toBe(1);
        r.setA(2); // not re-armed
        flush();
        expect(r.fired()).toBe(1);
        r.arm();
        r.setB(5); // b is not tracked
        flush();
        expect(r.fired()).toBe(1);
        r.setA(3);
        flush();
        expect(r.fired()).toBe(2);
        r.dispose();
    });

    it("an armed reaction does not fire after its root is disposed", () => {
        const r = makeReaction();
        r.arm();
        r.dispose();
        r.setA(1);
        flush();
        expect(r.fired()).toBe(0);
    });
});

describe("createContext / useContext / tryUseContext", () => {
  it("useContext returns the default value with no provider; default-less contexts and missing owners are Errors", () => {
    const p = probeContext();
    expect(p.withDefault).toBe(42);
    expect(p.withoutDefaultIsError).toBe(true);
    // dev-mode upstream messages (signals/src/core/error.ts): distinguishes ContextNotFoundError from NoOwnerError
    // and from an unrelated exception (e.g. a mis-emitted call) that would also make the Result an Error.
    expect(p.withoutDefaultMessage).toMatch(/created with a default value/);
    expect(p.noOwnerIsError).toBe(true);
    expect(p.noOwnerMessage).toMatch(/reactive root/);
  });
});

describe("getObserver / isDisposed", () => {
  it("getObserver is Some only inside a tracking computation, and None under untrack", () => {
    const p = makeObserverProbe();
    expect(p.observerOutside).toBe(false);
    expect(p.observerInRootBody).toBe(false);
    expect(p.observerInMemo()).toBe(true);
    expect(p.observerInUntrack()).toBe(false);
    p.dispose();
  });

  it("isDisposed(owner) flips when the root is disposed", () => {
    const p = makeObserverProbe();
    expect(p.rootDisposed()).toBe(false);
    p.dispose();
    expect(p.rootDisposed()).toBe(true);
  });
});
