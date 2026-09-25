// Partas.Solid.Experimental builders (Partas.Solid/Experimental.fs) at runtime: effect, memo, mount,
// cleanup, reaction, lambda, children and lazyload, standalone and inside a component body.
import {describe, it, expect} from "vitest";
import {createSignal, Loading} from "solid-js";
import {flush, mount, settle, text} from "../../helpers/index.js";
import {
    makeEffect,
    makeEffectTuple,
    makeEffectPreamble,
    makeEffectBranching,
    makeEffectGuard,
    makeEffectOverMemo,
    makeEffectMirror,
    makeMemo,
    makeMemoUntrack,
    makeMemoWithCleanup,
    makeClassified,
    makeMemoChain,
    makeMemoBind,
    makeMount,
    makeMountStatements,
    makeMountMatch,
    makeCleanup,
    makeCleanupStatements,
    makeCleanupMatch,
    makeCleanupInCompute,
    makeReaction,
    makeRearmingReaction,
    makeReactionStatements,
    makeLambda,
    makeChildren,
    makeSingleChild,
    LazyGreeting,
    getLazyLoads,
    BuilderHost
} from "./A-Builders.fs.jsx";

describe("effect { let! ... }", () => {
    it("runs the compute at creation and the effect phase on the first flush", () => {
        const e = makeEffect();
        expect(e.log).toEqual(["compute"]);
        flush();
        expect(e.log).toEqual(["compute", "effect:1:100"]);
        e.dispose();
    });

    it("re-runs compute then effect when the bound source changes", () => {
        const e = makeEffect();
        flush();
        e.set(2);
        expect(e.log).toEqual(["compute", "effect:1:100"]);
        flush();
        expect(e.log).toEqual(["compute", "effect:1:100", "compute", "effect:2:100"]);
        e.dispose();
    });

    it("reads after the let! (the effect phase) are untracked", () => {
        const e = makeEffect();
        flush();
        e.setOther(200);
        flush();
        expect(e.log).toEqual(["compute", "effect:1:100"]);
        e.set(3);
        flush();
        expect(e.log.at(-1)).toBe("effect:3:200");
        e.dispose();
    });

    it("dispose stops the effect", () => {
        const e = makeEffect();
        flush();
        e.dispose();
        e.set(9);
        flush();
        expect(e.log).toEqual(["compute", "effect:1:100"]);
    });

    it("a tuple-returning lambda source tracks both signals and destructures in the effect", () => {
        const e = makeEffectTuple();
        flush();
        e.set(2);
        flush();
        e.setOther(20);
        flush();
        expect(e.log).toEqual(["pair:1:10", "pair:2:10", "pair:2:20"]);
        e.dispose();
    });

    it("writes to both sources in one flush produce a single effect run", () => {
        const e = makeEffectTuple();
        flush();
        e.set(5);
        e.setOther(50);
        flush();
        expect(e.log).toEqual(["pair:1:10", "pair:5:50"]);
        e.dispose();
    });

    it("statements before the let! run once, immediately, and not on re-runs", () => {
        const e = makeEffectPreamble();
        expect(e.log).toEqual(["preamble"]);
        flush();
        e.set(2);
        flush();
        expect(e.log).toEqual(["preamble", "effect:1", "effect:2"]);
        e.dispose();
    });

    it("match / if-else in the effect phase take the right branch on each run", () => {
        const e = makeEffectBranching();
        flush();
        for (const v of [2, -4, 6, 7]) {
            e.set(v);
            flush();
        }
        expect(e.log).toEqual(["one", "two", "negative", "even", "odd"]);
        e.dispose();
    });

    it("an else-less if as the last statement runs only when the guard holds", () => {
        const e = makeEffectGuard();
        flush();
        e.set(2);
        flush();
        e.set(3);
        flush();
        expect(e.log).toEqual(["big:3"]);
        e.dispose();
    });

    it("an effect bound to a memo only re-runs when the memo value changes", () => {
        const e = makeEffectOverMemo();
        flush();
        e.set(3); // still odd
        flush();
        e.set(4);
        flush();
        expect(e.log.filter(x => x.startsWith("effect"))).toEqual(["effect:odd", "effect:even"]);
        expect(e.log.filter(x => x === "parity").length).toBe(3);
        e.dispose();
    });

    it("the effect phase can write another signal (mirror)", () => {
        const m = makeEffectMirror();
        expect(m.mirrored()).toBe(0);
        flush();
        expect(m.mirrored()).toBe(10);
        m.set(4);
        flush();
        expect(m.mirrored()).toBe(40);
        m.dispose();
    });
});

describe("memo { }", () => {
    it("computes once at creation and caches between reads", () => {
        const m = makeMemo();
        expect(m.runs()).toBe(1);
        expect(m.value()).toBe(4);
        expect(m.value()).toBe(4);
        expect(m.runs()).toBe(1);
        m.dispose();
    });

    it("recomputes after its source changes, and not when a signal it never reads is written", () => {
        const m = makeMemo();
        m.setOther(1);
        flush();
        expect(m.runs()).toBe(1);
        m.set(5);
        flush();
        expect(m.value()).toBe(10);
        expect(m.runs()).toBe(2);
        m.dispose();
    });

    it("untrack inside the body peeks without subscribing", () => {
        const m = makeMemoUntrack();
        expect(m.value()).toBe(11);
        m.setOther(20);
        flush();
        expect(m.value()).toBe(11);
        expect(m.runs()).toBe(1);
        m.set(2);
        flush();
        expect(m.value()).toBe(22);
        expect(m.runs()).toBe(2);
        m.dispose();
    });

    // Solid 2 defers the superseded run's cleanups until the new run commits (see Reactivity/Effects.test.js).
    it("cleanup { } inside a memo runs once per superseded run (after the new run) and on dispose", () => {
        const m = makeMemoWithCleanup();
        flush();
        m.set(2);
        flush();
        expect(m.log).toEqual(["memo:1", "effect:1", "memo:2", "memo-cleanup:1", "effect:2"]);
        m.dispose();
        expect(m.log.at(-1)).toBe("memo-cleanup:2");
    });

    it("if/elif/else and match bodies classify correctly", () => {
        const c = makeClassified();
        expect([c.sign(), c.size()]).toEqual(["zero", "none"]);
        c.set(-3);
        flush();
        expect([c.sign(), c.size()]).toEqual(["negative", "small"]);
        c.set(42);
        flush();
        expect([c.sign(), c.size()]).toEqual(["positive", "large"]);
        c.dispose();
    });

    it("chained memo builders propagate", () => {
        const c = makeMemoChain();
        expect(c.label()).toBe("total=15");
        c.set(4);
        flush();
        expect(c.total()).toBe(20);
        expect(c.label()).toBe("total=20");
        expect(c.labelRuns()).toBe(2);
        c.dispose();
    });

    // BUG: memo { let! v = src; return v * 2 } emits createMemo(_ => () => ...) (BaseLambdaBuilder Bind/Return add a thunk Run keeps), caching a closure that never tracks src; should be createMemo(_ => src() * 2).
    it.fails("let! + return inside memo yields a tracked value, not a closure", () => {
        const m = makeMemoBind();
        expect(m.value()).toBe(4);
        expect(m.runs()).toBe(1);
        m.set(3);
        flush();
        expect(m.value()).toBe(6);
        expect(m.runs()).toBe(2);
        m.dispose();
    });
});

describe("mount { } (onSettled)", () => {
    it("runs after the body, on flush, not at creation", () => {
        const m = makeMount();
        expect(m.log).toEqual(["body"]);
        flush();
        expect(m.log).toEqual(["body", "mounted:1"]);
        m.dispose();
    });

    it("runs once: signal reads inside are untracked", () => {
        const m = makeMount();
        flush();
        m.set(2);
        flush();
        m.set(3);
        flush();
        expect(m.log).toEqual(["body", "mounted:1"]);
        m.dispose();
    });

    it("never runs when the owner is disposed before the first flush", () => {
        const m = makeMount();
        m.dispose();
        flush();
        expect(m.log).toEqual(["body"]);
    });

    // BUG: mount { } drops statements after an else-less `if`: NullLambdaBuilder.Combine returns an uncalled `() => { first(); second() }` that OnSettledBuilder.Run ignores.
    it.fails("runs every statement after an else-less if", () => {
        const m = makeMountStatements();
        flush();
        expect(m.log).toEqual(["first", "guarded", "last"]);
        m.dispose();
    });

    // BUG: mount { } drops statements after a `match`: NullLambdaBuilder.Combine returns an uncalled closure that OnSettledBuilder.Run ignores.
    it.fails("runs the statement after a match", () => {
        const m = makeMountMatch();
        flush();
        expect(m.log).toEqual(["one", "after-match"]);
        m.dispose();
    });
});

describe("cleanup { } (onCleanup)", () => {
    it("runs nothing until dispose; children first, then own cleanups in registration order", () => {
        const c = makeCleanup();
        flush();
        expect(c.log).toEqual([]);
        c.dispose();
        expect(c.log).toEqual(["child", "first", "second", "third"]);
    });

    it("runs each cleanup exactly once", () => {
        const c = makeCleanup();
        c.dispose();
        c.dispose();
        expect(c.log.length).toBe(4);
    });

    // BUG: cleanup { } drops statements after an else-less `if`: NullLambdaBuilder.Combine returns an uncalled closure that OnCleanupBuilder.Run ignores.
    it.fails("runs every statement after an else-less if", () => {
        const c = makeCleanupStatements();
        c.dispose();
        expect(c.log).toEqual(["release-a", "release-guarded", "release-b"]);
    });

    // BUG: cleanup { } drops statements after a `match`: NullLambdaBuilder.Combine returns an uncalled closure that OnCleanupBuilder.Run ignores.
    it.fails("runs the statement after a match", () => {
        const c = makeCleanupMatch();
        c.dispose();
        expect(c.log).toEqual(["one", "after-match"]);
    });

    it("inside an effect's compute runs before each re-compute and on dispose", () => {
        const c = makeCleanupInCompute();
        flush();
        c.set(2);
        flush();
        expect(c.log).toEqual(["effect:1", "compute-cleanup:1", "effect:2"]);
        c.dispose();
        expect(c.log.at(-1)).toBe("compute-cleanup:2");
    });
});

describe("reaction { } (createReaction)", () => {
    it("does nothing until armed, and arming alone does not fire", () => {
        const r = makeReaction();
        r.set(1);
        flush();
        r.arm();
        flush();
        expect(r.log).toEqual([]);
        r.dispose();
    });

    it("fires once on the first change after arming, then stays quiet", () => {
        const r = makeReaction();
        r.arm();
        flush();
        r.set(1);
        flush();
        r.set(2);
        flush();
        expect(r.log).toEqual(["fired:1:0"]);
        r.dispose();
    });

    it("only the tracked expression arms it; other signals do not fire it", () => {
        const r = makeReaction();
        r.arm();
        r.setOther(5);
        flush();
        expect(r.log).toEqual([]);
        r.set(1);
        flush();
        expect(r.log).toEqual(["fired:1:5"]);
        r.dispose();
    });

    it("re-arming from the body fires on every change", () => {
        const r = makeRearmingReaction();
        r.arm();
        for (const v of [1, 2, 3]) {
            r.set(v);
            flush();
        }
        expect(r.log).toEqual(["fired:1", "fired:2", "fired:3"]);
        r.dispose();
    });

    it("an armed reaction never fires after its owner is disposed", () => {
        const r = makeReaction();
        r.arm();
        r.dispose();
        r.set(1);
        flush();
        expect(r.log).toEqual([]);
    });

    it("runs every statement in its body", () => {
        const r = makeReactionStatements();
        r.arm();
        r.set(7);
        flush();
        expect(r.log).toEqual(["start", "value:7", "end"]);
        r.dispose();
    });
});

describe("lambda { }", () => {
    it("defers its body and re-evaluates on every call", () => {
        const l = makeLambda();
        expect(l.calls()).toBe(0);
        expect(l.read()).toBe(2);
        l.set(5);
        flush();
        expect(l.read()).toBe(6);
        expect(l.calls()).toBe(2);
        l.dispose();
    });

    it("mutable locals assigned in match arms", () => {
        const l = makeLambda();
        expect(l.describe()).toBe("one");
        l.set(2);
        flush();
        expect(l.describe()).toBe("two");
        l.set(3);
        flush();
        expect(l.describe()).toBe("?");
        l.dispose();
    });

    it("an else-less if before the result runs its side effect and still returns", () => {
        const l = makeLambda();
        expect(l.guarded()).toBe(3);
        expect(l.calls()).toBe(0);
        l.set(11);
        flush();
        expect(l.guarded()).toBe(33);
        expect(l.calls()).toBe(100);
        l.dispose();
    });
});

describe("children { }", () => {
    it("flattens nested arrays and resolves thunks", () => {
        const c = makeChildren();
        expect(c.resolved()).toEqual(["a", "b", "c", "n=2"]);
        expect(c.asArray()).toEqual(["a", "b", "c", "n=2"]);
        c.dispose();
    });

    it("is memoised and reactive to signals read by nested thunks", () => {
        const c = makeChildren();
        c.resolved();
        c.resolved();
        expect(c.runs()).toBe(1);
        c.set(3);
        flush();
        expect(c.resolved()).toEqual(["a", "b", "c", "n=3"]);
        c.dispose();
    });

    it("toArray wraps a single resolved value", () => {
        const c = makeSingleChild();
        expect(c.resolved()).toBe("only:1");
        expect(c.asArray()).toEqual(["only:1"]);
        c.set(2);
        flush();
        expect(c.asArray()).toEqual(["only:2"]);
        c.dispose();
    });
});

describe("lazyload { }", () => {
    it("does not call the loader until preload or render, and loads once", async () => {
        expect(getLazyLoads()).toBe(0);
        const mod = await LazyGreeting.preload();
        expect(typeof mod.default).toBe("function");
        await LazyGreeting.preload();
        expect(getLazyLoads()).toBe(1);
    });

    it("renders the default export inside Loading", async () => {
        const {container} = mount(() => (
            <Loading fallback={<i>wait</i>}>
                <LazyGreeting/>
            </Loading>
        ), undefined, {thunk: true});
        await settle();
        expect(container.querySelector("span.lazy-target")?.textContent).toBe("loaded");
    });
});

describe("builders inside a [<SolidTypeComponent>]", () => {
    it("memo/effect/mount track props and render", () => {
        const [value, setValue] = createSignal(2);
        const log = [];
        const {container, dispose} = mount(BuilderHost, {
            get value() {
                return value();
            },
            log
        });
        expect(text(container.querySelector(".host"))).toBe("4");
        expect(log).toEqual(["effect:4", "mount"]);
        setValue(5);
        flush();
        expect(text(container.querySelector(".host"))).toBe("10");
        expect(log).toEqual(["effect:4", "mount", "effect:10"]);
        dispose();
        expect(log.at(-1)).toBe("cleanup");
    });
});
