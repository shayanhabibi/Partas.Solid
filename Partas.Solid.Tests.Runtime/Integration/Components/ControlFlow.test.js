import {describe, it, expect, vi} from "vitest";
import {createSignal} from "solid-js";
import {mount, click, act, flush, settle, deferred, waitFor, text} from "../../helpers/index.js";
import {
    PureKeyedList,
    LetKeyedList,
    PureIndexList,
    KeyedList,
    IndexList,
    TodoApp,
    Stars,
    Gate,
    UserCard,
    LiveUserCard,
    TrafficLight,
    SafeZone,
    SafeZoneRaw,
    StaticErrorZone,
    StaticErrorZoneRaw,
    AsyncDetail,
    RevealPair
} from "./C-ControlFlow.fs.jsx";

const rows = container => [...container.querySelectorAll("li.row")];
const rowTexts = container => rows(container).map(text);

describe("Components: For (keyed)", () => {
    it("renders the fallback for an empty list and rows with index accessors otherwise", () => {
        const [items, setItems] = createSignal([]);
        const {container} = mount(PureKeyedList, {
            get items() {
                return items();
            }
        });
        expect(text(container.querySelector("li.empty"))).toBe("nothing");
        expect(rows(container)).toHaveLength(0);

        act(() => setItems([{id: 1, title: "a"}, {id: 2, title: "b"}]));
        expect(container.querySelector("li.empty")).toBeNull();
        expect(rowTexts(container)).toEqual(["a@0", "b@1"]);

        act(() => setItems([]));
        expect(text(container.querySelector("li.empty"))).toBe("nothing");
    });

    it("preserves DOM nodes by item identity across add, remove and reorder", () => {
        const a = {id: 1, title: "a"};
        const b = {id: 2, title: "b"};
        const c = {id: 3, title: "c"};
        const [items, setItems] = createSignal([a, b]);
        const {container} = mount(PureKeyedList, {
            get items() {
                return items();
            }
        });
        const [liA, liB] = rows(container);

        act(() => setItems([a, b, c]));
        expect(rowTexts(container)).toEqual(["a@0", "b@1", "c@2"]);
        expect(rows(container)[0]).toBe(liA);
        expect(rows(container)[1]).toBe(liB);
        const liC = rows(container)[2];

        act(() => setItems([c, a, b]));
        // same nodes, moved; the index accessor re-evaluates in place
        expect(rows(container)).toEqual([liC, liA, liB]);
        expect(rowTexts(container)).toEqual(["c@0", "a@1", "b@2"]);

        act(() => setItems([c, b]));
        expect(rows(container)).toEqual([liC, liB]);
        expect(liA.isConnected).toBe(false);
        expect(rowTexts(container)).toEqual(["c@0", "b@1"]);
    });

    it("recreates a row when the item identity changes even if its content is equal", () => {
        const [items, setItems] = createSignal([{id: 1, title: "a"}]);
        const {container} = mount(PureKeyedList, {
            get items() {
                return items();
            }
        });
        const first = rows(container)[0];
        act(() => setItems([{id: 1, title: "a"}]));
        expect(rows(container)[0]).not.toBe(first);
        expect(rowTexts(container)).toEqual(["a@0"]);
    });

    it("supports a let binding before the row element", () => {
        const {container} = mount(LetKeyedList, {items: [{id: 1, title: "x"}, {id: 2, title: "y"}]});
        expect(rowTexts(container)).toEqual(["X", "Y"]);
    });

    it("rebuilds rows for the real TodoApp while keeping surviving nodes", () => {
        const {container} = mount(TodoApp);
        const todo = id => container.querySelector(`#todo-${id}`);
        const t1 = todo(1);
        const t2 = todo(2);
        expect(text(container.querySelector(".total"))).toBe("2");
        expect([...container.querySelectorAll("li.todo")].map(text)).toEqual(["a", "b"]);

        click(container.querySelector(".add"));
        click(container.querySelector(".add"));
        expect([...container.querySelectorAll("li.todo")].map(text)).toEqual(["a", "b", "t3", "t4"]);
        expect(text(container.querySelector(".total"))).toBe("4");
        expect(todo(1)).toBe(t1);

        click(container.querySelector(".reverse"));
        expect([...container.querySelectorAll("li.todo")].map(el => el.id)).toEqual([
            "todo-4",
            "todo-3",
            "todo-2",
            "todo-1"
        ]);
        expect(todo(1)).toBe(t1);
        expect(todo(2)).toBe(t2);

        click(container.querySelector(".remove-first"));
        expect(todo(4)).toBeNull();
        expect(text(container.querySelector(".total"))).toBe("3");
        expect(todo(2)).toBe(t2);
    });
});

describe("Components: For (non-keyed)", () => {
    it("keys rows by position and updates the item accessor in place", () => {
        const [items, setItems] = createSignal(["a", "b"]);
        const {container} = mount(PureIndexList, {
            get items() {
                return items();
            }
        });
        const [r0, r1] = rows(container);
        expect(rowTexts(container)).toEqual(["0:a", "1:b"]);

        act(() => setItems(["z", "b", "c"]));
        expect(rowTexts(container)).toEqual(["0:z", "1:b", "2:c"]);
        expect(rows(container)[0]).toBe(r0);
        expect(rows(container)[1]).toBe(r1);

        act(() => setItems(["c", "z"]));
        // positions survive, values move
        expect(rows(container)).toEqual([r0, r1]);
        expect(rowTexts(container)).toEqual(["0:c", "1:z"]);
    });
});

describe("Components: Repeat", () => {
    it("renders count rows, grows/shrinks in place and shows the fallback at zero", () => {
        const [count, setCount] = createSignal(3);
        const {container} = mount(Stars, {
            get count() {
                return count();
            }
        });
        const stars = () => [...container.querySelectorAll("b.star")];
        expect(stars().map(text)).toEqual(["0", "1", "2"]);
        const [s0, s1] = stars();

        act(() => setCount(5));
        expect(stars().map(text)).toEqual(["0", "1", "2", "3", "4"]);
        expect(stars()[0]).toBe(s0);

        act(() => setCount(2));
        expect(stars()).toEqual([s0, s1]);

        act(() => setCount(0));
        expect(stars()).toHaveLength(0);
        expect(text(container.querySelector("em"))).toBe("no stars");
    });

    it("offsets the index with `from`", () => {
        const [from, setFrom] = createSignal(10);
        const {container} = mount(Stars, {
            count: 3, get from() {
                return from();
            }
        });
        const stars = () => [...container.querySelectorAll("b.star")].map(text);
        expect(stars()).toEqual(["10", "11", "12"]);
        act(() => setFrom(11));
        expect(stars()).toEqual(["11", "12", "13"]);
    });
});

describe("Components: Show", () => {
    it("toggles between children and fallback", () => {
        const [open, setOpen] = createSignal(false);
        const {root} = mount(Gate, {
            get isOpen() {
                return open();
            }
        });
        expect(root.innerHTML).toBe('<span class="closed">closed</span>');
        act(() => setOpen(true));
        expect(root.innerHTML).toBe('<span class="open">open</span>');
        act(() => setOpen(false));
        expect(root.innerHTML).toBe('<span class="closed">closed</span>');
    });

    it("keyed: passes the raw value and remounts only when the value identity changes", () => {
        const ada = {id: 1, title: "Ada"};
        const [user, setUser] = createSignal(undefined);
        const onMount = vi.fn();
        const {root} = mount(UserCard, {
            get user() {
                return user();
            }, onMount
        });
        expect(text(root)).toBe("anonymous");
        expect(onMount).not.toHaveBeenCalled();

        act(() => setUser(ada));
        const who = root.querySelector(".who");
        expect(text(who)).toBe("Ada");
        expect(onMount.mock.calls).toEqual([["Ada"]]);

        // new identity -> remount
        act(() => setUser({id: 2, title: "Grace"}));
        expect(text(root.querySelector(".who"))).toBe("Grace");
        expect(root.querySelector(".who")).not.toBe(who);
        expect(onMount.mock.calls).toEqual([["Ada"], ["Grace"]]);

        act(() => setUser(undefined));
        expect(text(root)).toBe("anonymous");
    });

    it("non-keyed: passes an accessor and keeps the child across truthy values", () => {
        const [user, setUser] = createSignal({id: 1, title: "Ada"});
        const onMount = vi.fn();
        const {root} = mount(LiveUserCard, {
            get user() {
                return user();
            }, onMount
        });
        const who = root.querySelector(".who");
        expect(text(who)).toBe("Ada");

        act(() => setUser({id: 2, title: "Grace"}));
        expect(root.querySelector(".who")).toBe(who);
        expect(text(who)).toBe("Grace");
        expect(onMount).toHaveBeenCalledTimes(1);

        act(() => setUser(undefined));
        expect(root.querySelector(".who")).toBeNull();
        act(() => setUser({id: 3, title: "Linus"}));
        expect(text(root.querySelector(".who"))).toBe("Linus");
        expect(onMount).toHaveBeenCalledTimes(2);
    });
});

describe("Components: Switch / Match", () => {
    it("renders the first matching branch, or the fallback", () => {
        const [state, setState] = createSignal("red");
        const {root} = mount(TrafficLight, {
            get state() {
                return state();
            }
        });
        expect(root.innerHTML).toBe('<span class="red">stop</span>');
        act(() => setState("green"));
        expect(root.innerHTML).toBe('<span class="green">go</span>');
        act(() => setState("amber"));
        expect(root.innerHTML).toBe('<span class="amber">wait</span>');
        act(() => setState("blue"));
        expect(root.innerHTML).toBe('<span class="unknown">unknown</span>');
    });
});

describe("Components: Errored", () => {
    it("renders children until they throw, shows the callback fallback, and heals when the source changes", () => {
        const [value, setValue] = createSignal(1);
        const {container} = mount(SafeZoneRaw, {value});
        const zone = container.querySelector(".zone");
        expect(text(container.querySelector(".bomb"))).toBe("1");

        act(() => setValue(3));
        expect(container.querySelector(".bomb")).toBeNull();
        expect(zone.querySelectorAll(".error")).toHaveLength(1);
        expect(text(container.querySelector(".error .message"))).toBe("boom at 3");

        act(() => setValue(4));
        expect(text(container.querySelector(".error .message"))).toBe("boom at 4");

        // Solid 2: the errored computation still tracks `value`, so a good value heals the boundary
        // without reset().
        act(() => setValue(0));
        expect(container.querySelector(".error")).toBeNull();
        expect(text(container.querySelector(".bomb"))).toBe("0");

        act(() => setValue(2));
        expect(text(container.querySelector(".bomb"))).toBe("2");
    });

    it("re-runs the children when the fallback calls reset()", () => {
        // untracked source: only reset() can make the boundary try again
        let current = 7;
        const {container} = mount(SafeZoneRaw, {value: () => current});
        expect(text(container.querySelector(".error .message"))).toBe("boom at 7");

        current = 8;
        click(container.querySelector(".reset"));
        expect(text(container.querySelector(".error .message"))).toBe("boom at 8");

        current = 2;
        click(container.querySelector(".reset"));
        expect(container.querySelector(".error")).toBeNull();
        expect(text(container.querySelector(".bomb"))).toBe("2");
    });

    it("renders a static element fallback", () => {
        const spy = vi.spyOn(console, "error").mockImplementation(() => {
        });
        try {
            const {root} = mount(StaticErrorZoneRaw);
            expect(root.innerHTML).toBe('<span class="error">static fallback</span>');
        } finally {
            spy.mockRestore();
        }
    });
});

describe("Components: Loading", () => {
    it("shows the fallback, then the async content, keeping chrome outside the boundary", async () => {
        const pending = new Map();
        const fetch = vi.fn(id => {
            const d = deferred();
            pending.set(id, d);
            return d.promise;
        });
        const [id, setId] = createSignal(1);
        const {container} = mount(AsyncDetail, {
            get id() {
                return id();
            }, fetch
        });
        const chrome = container.querySelector(".chrome");
        expect(text(container.querySelector(".spinner"))).toBe("loading");
        expect(container.querySelector(".content")).toBeNull();
        expect(fetch).toHaveBeenCalledTimes(1);

        pending.get(1).resolve("one");
        await settle();
        expect(container.querySelector(".spinner")).toBeNull();
        expect(text(container.querySelector(".content"))).toBe("one");

        const content = container.querySelector(".content");
        act(() => setId(2));
        expect(fetch).toHaveBeenCalledTimes(2);
        expect(fetch).toHaveBeenLastCalledWith(2);
        // the boundary has already revealed once: while id 2 is pending the old content stays
        // (no spinner, and nothing blanks)
        expect(container.querySelector(".spinner")).toBeNull();
        expect(text(container.querySelector(".content"))).toBe("one");
        pending.get(2).resolve("two");
        await waitFor(() => text(container.querySelector(".content")) === "two");
        expect(container.querySelector(".chrome")).toBe(chrome);
        expect(container.querySelector(".content")).toBe(content);
        expect(container.querySelector(".spinner")).toBeNull();
        expect(fetch).toHaveBeenCalledTimes(2);
    });
});

describe("Components: Reveal", () => {
    it("together: holds every slot on its fallback until all are ready", async () => {
        const a = deferred();
        const b = deferred();
        const {container} = mount(RevealPair, {first: () => a.promise, second: () => b.promise, order: "together"});
        expect(container.querySelector(".fb-a")).not.toBeNull();
        expect(container.querySelector(".fb-b")).not.toBeNull();

        a.resolve("A");
        await settle();
        expect(container.querySelector(".a")).toBeNull();
        expect(container.querySelector(".fb-a")).not.toBeNull();

        b.resolve("B");
        await settle();
        expect(text(container.querySelector(".a"))).toBe("A");
        expect(text(container.querySelector(".b"))).toBe("B");
    });

    it("sequential: a later slot waits for earlier ones", async () => {
        const a = deferred();
        const b = deferred();
        const {container} = mount(RevealPair, {first: () => a.promise, second: () => b.promise, order: "sequential"});

        b.resolve("B");
        await settle();
        expect(container.querySelector(".b")).toBeNull();
        expect(container.querySelector(".fb-a")).not.toBeNull();

        a.resolve("A");
        await settle();
        expect(text(container.querySelector(".a"))).toBe("A");
        expect(text(container.querySelector(".b"))).toBe("B");
    });

    it("natural: each slot reveals on its own", async () => {
        const a = deferred();
        const b = deferred();
        const {container} = mount(RevealPair, {first: () => a.promise, second: () => b.promise, order: "natural"});
        b.resolve("B");
        await settle();
        expect(text(container.querySelector(".b"))).toBe("B");
        expect(container.querySelector(".fb-a")).not.toBeNull();
        a.resolve("A");
        await settle();
        expect(text(container.querySelector(".a"))).toBe("A");
    });
});

// Known plugin bugs are kept last: an uncaught render error halts the reactive system for the
// rest of the file in Solid 2 dev builds.
describe("Components: control-flow plugin bugs", () => {
    // BUG: a two-argument child lambda (For.Keyed / For.NonKeyed) whose body is `statement; element`
    // compiles to `(item, index) => { statement; }` - the element is dropped and nothing renders.
    it.fails("For.Keyed row callback with a leading statement still renders its element", () => {
        const onRowCreated = vi.fn();
        const {container} = mount(KeyedList, {items: [{id: 1, title: "a"}, {id: 2, title: "b"}], onRowCreated});
        expect(onRowCreated.mock.calls).toEqual([["a"], ["b"]]);
        expect(rowTexts(container)).toEqual(["a@0", "b@1"]);
    });

    // BUG: same dropped-return as above, for For.NonKeyed.
    it.fails("For.NonKeyed row callback with a leading statement still renders its element", () => {
        const onRowCreated = vi.fn();
        const {container} = mount(IndexList, {items: ["a", "b"], onRowCreated});
        expect(onRowCreated.mock.calls).toEqual([[0], [1]]);
        expect(rowTexts(container)).toEqual(["0:a", "1:b"]);
    });

    // BUG: `Errored(fallbackFn = ...)` emits a literal `fallbackFn` prop instead of `fallback`
    // (the inline setter helper is not expanded), so Errored has no fallback to render.
    it.fails("Errored fallbackFn helper renders the callback fallback", () => {
        const [value, setValue] = createSignal(1);
        const {container} = mount(SafeZone, {value});
        act(() => setValue(3));
        expect(text(container.querySelector(".error .message"))).toBe("boom at 3");
    });

    // BUG: `Errored(fallbackEle = ...)` likewise emits `fallbackEle` instead of `fallback`.
    it.fails("Errored fallbackEle helper renders the static fallback", () => {
        const spy = vi.spyOn(console, "error").mockImplementation(() => {
        });
        try {
            const {root} = mount(StaticErrorZone);
            expect(root.innerHTML).toBe('<span class="error">static fallback</span>');
        } finally {
            spy.mockRestore();
        }
    });
});
