import {describe, it, expect} from "vitest";
import {createSignal} from "solid-js";
import {mount, act} from "../../helpers/index.js";
import {
    Payload, UnionChild, AccessorValues, Conditional, BoundChildren, MatchedChildren, NestedChildren,
    DiscardedChildren, TextChildren, ArrayProp, ArrayCardinality, EmptyArray, Produced,
    MatchedArrayHost, RenderFunction, ScopedArray, DelayedEffects, OpaqueSequence, LoopSequence, SkippedArray, MatchedItems
} from "./A-Collection.fs.jsx";

const render = fn => mount(fn, undefined, {thunk: true}).root;
const html = root => root.innerHTML.replace(/<!---->/g, "");

describe("JSX child collection", () => {
    it("retains union case fields and nested field access", () => {
        expect(html(render(() => UnionChild(new Payload(1, ["hello"])))))
            .toBe("<span><b>hello</b><i>5</i></span>");
        expect(html(render(() => UnionChild(new Payload(2, [7, "seven"])))))
            .toBe('<span title="seven">7seven</span>');
    });

    it("retains list, option and tuple accessors", () => {
        expect(html(render(() => AccessorValues())))
            .toBe("<span>a</span><b>b</b><i>c</i><strong>de</strong>");
    });

    it("keeps both branch children and updates the selected branch", () => {
        const [flag, setFlag] = createSignal(true);
        const root = render(() => Conditional(flag));
        expect(html(root)).toBe("head<span>one</span><b>two</b>tail");
        act(() => setFlag(false));
        expect(html(root)).toBe("head<i>three</i><strong>four</strong>tail");
        act(() => setFlag(true));
        expect(html(root)).toBe("head<span>one</span><b>two</b>tail");
    });

    it("retains bound children, following siblings and reactive scope", () => {
        const [text, setText] = createSignal("hello");
        const root = render(() => BoundChildren(text));
        expect(html(root)).toBe("head<span>HELLO</span>middle<b>hello</b>tail");
        act(() => setText("bye"));
        expect(html(root)).toBe("head<span>BYE</span>middle<b>bye</b>tail");
    });

    it.each([
        [new Payload(1, ["hello"]), "<span>HELLO</span><b>hello</b>"],
        [new Payload(1, ["hi"]), "<i>hi</i>short"],
        [new Payload(0, []), "no<strong>payload</strong>"]
    ])("retains all matched children with guards and local bindings (%#)", (value, expected) => {
        expect(html(render(() => MatchedChildren(value)))).toBe(expected);
    });

    it("retains nested bound children", () => {
        expect(html(render(() => NestedChildren("hello"))))
            .toBe("<span><b>HELLO</b><i>hello</i></span>tail");
    });

    it("retains discarded expressions and runs effects in source order", () => {
        const logs = [];
        const root = render(() => DiscardedChildren(true, x => { logs.push(x); }, () => {
            logs.push("produce");
            return Produced();
        }));
        expect(html(root)).toBe('<span title="one">one</span><em>produced</em>text<b>two</b>');
        expect(logs).toEqual(["before", "span", "between", "produce", "after"]);
    });

    it("retains multiple explicitly yielded text children", () => {
        expect(render(() => TextChildren(true)).textContent).toBe("onetwo");
        expect(render(() => TextChildren(false)).textContent).toBe("empty");
    });

    it("retains multiple render-function children and leading effects", () => {
        const logs = [];
        expect(html(render(() => RenderFunction(x => { logs.push(x); }))))
            .toBe("<span>one</span><b>one</b><span>two</span><b>two</b>");
        expect(logs).toEqual(["one", "two"]);
    });
});

describe("attribute collection semantics", () => {
    it.each([
        [new Payload(0, []), ["head", "tail"]],
        [new Payload(1, ["hello"]), ["head", "hello", "tail"]],
        [new Payload(2, [7, "seven"]), ["head", "7", "seven", "tail"]]
    ])("retains zero/one/many union-match items and effects (%#)", (value, expected) => {
        const logs = [];
        expect(render(() => MatchedItems(value, x => { logs.push(x); return x; })).title).toBe(expected.join(":"));
        expect(logs).toEqual(expected);
    });
    it.each([false, true])("preserves scoped bindings and delayed effects (flag=%s)", flag => {
        const scopedLogs = [];
        const scoped = render(() => ScopedArray(flag, x => { scopedLogs.push(x); return x; }));
        expect(scoped.title).toBe(flag ? "head:bound:branch:tail" : "head:tail");
        expect(scopedLogs).toEqual(flag ? ["head", "bound", "branch", "tail"] : ["head", "bound", "tail"]);
        const effectLogs = [];
        const effects = render(() => DelayedEffects(flag, x => { effectLogs.push(x); return x; }));
        expect(effects.title).toBe(flag ? "one:tail" : "two:tail");
        expect(effectLogs).toEqual(["before", flag ? "one" : "two", "between", "tail"]);
        expect(render(() => SkippedArray(flag)).title).toBe(flag ? "head:one:tail" : "head:two:tail");
    });

    it("preserves sequence construction before enumeration for an opaque append", () => {
        const logs = [];
        expect(render(() => OpaqueSequence(x => { logs.push(x); return x; })).title).toBe("left:right");
        expect(logs).toEqual(["right", "left"]);
    });

    it("retains unsupported sequence shapes and their effects", () => {
        const logs = [];
        expect(render(() => LoopSequence(["a", "", "b"], x => { logs.push(x); return x; })).title).toBe("a:b");
        expect(logs).toEqual(["a", "b"]);
    });
    it("keeps matched props in delayed sequences", () => {
        expect(render(() => MatchedArrayHost("hello", true)).title).toBe("hello:hello");
        expect(render(() => MatchedArrayHost("hello", false)).title).toBe("hello");
        expect(render(() => MatchedArrayHost("skip", true)).title).toBe("");
    });
    it.each([false, true])("keeps zero/many sequence items and effect order (flag=%s)", flag => {
        const logs = [];
        const root = render(() => ArrayProp(flag, x => { logs.push(x); return x; }));
        const expected = flag ? ["head", "one", "two", "tail"] : ["head", "tail"];
        expect(root.title).toBe(expected.join(":"));
        expect(logs).toEqual(expected);
        expect(render(() => ArrayCardinality(flag)).title).toBe(flag ? "2" : "1");
        expect(render(() => EmptyArray(flag)).title).toBe(flag ? "1" : "0");
    });
});
