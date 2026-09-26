import {describe, it, expect, vi} from "vitest";
import {createSignal} from "solid-js";
import {mount, click, act, text} from "../../helpers/index.js";
import {
    Grader,
    NoFallbackSwitch,
    SelectionView,
    SelectionViewPlain,
    Tabs,
    KeyedRows,
    KeyFnRows,
    SlotRows,
    Grid,
    Picker,
    Dots,
    Ratings
} from "./B-Lists.fs.jsx";

const rows = c => [...c.querySelectorAll("li.row")];
const rowTexts = c => rows(c).map(text);
const reactive = (initial) => {
    const [get, set] = createSignal(initial);
    return [get, set];
};

describe("FlowAdvanced: Switch / Match", () => {
    it("first truthy Match wins when several conditions hold", () => {
        const [score, setScore] = reactive(95);
        const {root} = mount(Grader, {
            get score() {
                return score();
            }
        });
        expect(root.innerHTML).toBe('<span class="a">A</span>');
        act(() => setScore(85));
        expect(root.innerHTML).toBe('<span class="b">B</span>');
        act(() => setScore(72));
        expect(root.innerHTML).toBe('<span class="c">C</span>');
        act(() => setScore(10));
        expect(root.innerHTML).toBe('<span class="f">F</span>');
        act(() => setScore(100));
        expect(root.innerHTML).toBe('<span class="a">A</span>');
    });

    it("keeps the active branch node while the same Match stays selected", () => {
        const [score, setScore] = reactive(81);
        const {root} = mount(Grader, {
            get score() {
                return score();
            }
        });
        const b = root.querySelector(".b");
        expect(text(b)).toBe("B");
        act(() => setScore(82));
        act(() => setScore(89));
        expect(root.querySelector(".b")).toBe(b);
    });

    it("renders nothing when no Match holds and there is no fallback", () => {
        const [mode, setMode] = reactive("z");
        const {root} = mount(NoFallbackSwitch, {
            get mode() {
                return mode();
            }
        });
        expect(root.innerHTML).toBe("");
        act(() => setMode("y"));
        expect(root.innerHTML).toBe('<span class="y">Y</span>');
        act(() => setMode("q"));
        expect(root.innerHTML).toBe("");
    });

    const matchCallbacks = Comp => () => {
        const [selected, setSelected] = reactive(undefined);
        const [count, setCount] = reactive(0);
        const onMount = vi.fn();
        const {root} = mount(Comp, {
            get selected() {
                return selected();
            },
            get count() {
                return count();
            },
            onMount
        });
        expect(root.innerHTML).toBe('<span class="none">none</span>');

        act(() => setCount(2));
        const countEl = root.querySelector(".count");
        expect(text(countEl)).toBe("count 2");
        act(() => setCount(3));
        expect(root.querySelector(".count")).toBe(countEl);
        expect(text(countEl)).toBe("count 3");

        // earlier Match takes priority over the count branch
        act(() => setSelected({id: 1, label: "one"}));
        expect(root.innerHTML).toBe('<span class="item">one</span>');
        act(() => setSelected({id: 2, label: "two"}));
        expect(text(root)).toBe("two");
        expect(onMount.mock.calls).toEqual([["one"], ["two"]]);

        act(() => setSelected(undefined));
        expect(text(root)).toBe("count 3");
    };

    it("Match.Keyed passes the raw value and remounts on identity change; Match.NonKeyed passes an accessor",
        matchCallbacks(SelectionViewPlain));

    // BUG: Match.Keyed `when'option = props.selected` is dropped: plugin emits `<KeyedMatch>` with no `when` prop.
    it.fails("Match.Keyed with the when'option setter matches on Some", matchCallbacks(SelectionView));

    it("drives a tabbed UI from local state, keeping the tab node while it stays active", () => {
        const {container} = mount(Tabs);
        expect(text(container.querySelector("section.home"))).toBe("home page");

        click(container.querySelector(".go-settings"));
        expect(container.querySelector("section.home")).toBeNull();
        const input = container.querySelector("input.setting");
        input.value = "typed";
        click(container.querySelector(".go-settings"));
        expect(container.querySelector("input.setting")).toBe(input);
        expect(input.value).toBe("typed");

        click(container.querySelector(".go-missing"));
        expect(text(container.querySelector(".not-found"))).toBe("404");
        expect(container.querySelector("section")).toBeNull();

        click(container.querySelector(".go-settings"));
        expect(container.querySelector("input.setting")).not.toBe(input);
    });
});

describe("FlowAdvanced: For (keyed by identity)", () => {
    const a = {id: 1, label: "a"};
    const b = {id: 2, label: "b"};
    const c = {id: 3, label: "c"};
    const d = {id: 4, label: "d"};

    const setup = initial => {
        const [items, setItems] = reactive(initial);
        const h = mount(KeyedRows, {
            get items() {
                return items();
            }
        });
        return {...h, setItems};
    };

    it("inserts in the middle without touching neighbours and updates indices", () => {
        const {container, setItems} = setup([a, b, c]);
        const [la, lb, lc] = rows(container);
        act(() => setItems([a, d, b, c]));
        expect(rowTexts(container)).toEqual(["a#0", "d#1", "b#2", "c#3"]);
        const now = rows(container);
        expect(now[0]).toBe(la);
        expect(now[2]).toBe(lb);
        expect(now[3]).toBe(lc);
    });

    it("removes from the middle and disconnects only that node", () => {
        const {container, setItems} = setup([a, b, c]);
        const [la, lb, lc] = rows(container);
        act(() => setItems([a, c]));
        expect(rows(container)).toEqual([la, lc]);
        expect(lb.isConnected).toBe(false);
        expect(rowTexts(container)).toEqual(["a#0", "c#1"]);
    });

    it("swaps two rows by moving nodes", () => {
        const {container, setItems} = setup([a, b, c, d]);
        const [la, lb, lc, ld] = rows(container);
        act(() => setItems([a, d, c, b]));
        expect(rows(container)).toEqual([la, ld, lc, lb]);
        expect(rowTexts(container)).toEqual(["a#0", "d#1", "c#2", "b#3"]);
    });

    it("toggles the fallback on empty and back", () => {
        const {container, setItems} = setup([]);
        expect(text(container.querySelector("li.empty"))).toBe("empty");
        act(() => setItems([b]));
        expect(container.querySelector("li.empty")).toBeNull();
        expect(rowTexts(container)).toEqual(["b#0"]);
        act(() => setItems([]));
        expect(rows(container)).toHaveLength(0);
        expect(container.querySelector("li.empty")).not.toBeNull();
    });

    it("handles duplicate items (same reference twice)", () => {
        const {container, setItems} = setup([a, a]);
        expect(rowTexts(container)).toEqual(["a#0", "a#1"]);
        act(() => setItems([a]));
        expect(rowTexts(container)).toEqual(["a#0"]);
    });
});

describe("FlowAdvanced: For keyed by a key function", () => {
    it("keeps rows for equal keys even when objects are replaced, and updates the item accessor", () => {
        const [items, setItems] = reactive([{id: 1, label: "a"}, {id: 2, label: "b"}]);
        const {container} = mount(KeyFnRows, {
            get items() {
                return items();
            }
        });
        const [r1, r2] = rows(container);
        expect(rowTexts(container)).toEqual(["a#0", "b#1"]);

        act(() => setItems([{id: 2, label: "B"}, {id: 1, label: "A"}]));
        expect(rows(container)).toEqual([r2, r1]);
        expect(rowTexts(container)).toEqual(["B#0", "A#1"]);

        act(() => setItems([{id: 3, label: "c"}, {id: 1, label: "A"}]));
        expect(rows(container)[1]).toBe(r1);
        expect(r2.isConnected).toBe(false);
        expect(rowTexts(container)).toEqual(["c#0", "A#1"]);
    });
});

describe("FlowAdvanced: For (non-keyed)", () => {
    it("shows the fallback when empty and reuses positional rows when refilled", () => {
        const [items, setItems] = reactive([]);
        const {container} = mount(SlotRows, {
            get items() {
                return items();
            }
        });
        expect(text(container.querySelector("li.empty"))).toBe("no slots");
        act(() => setItems(["x", "y", "z"]));
        const [r0, r1, r2] = rows(container);
        expect(rowTexts(container)).toEqual(["0=x", "1=y", "2=z"]);

        // remove from the middle: the last slot goes, the values shift
        act(() => setItems(["x", "z"]));
        expect(rows(container)).toEqual([r0, r1]);
        expect(r2.isConnected).toBe(false);
        expect(rowTexts(container)).toEqual(["0=x", "1=z"]);

        act(() => setItems([]));
        expect(text(container.querySelector("li.empty"))).toBe("no slots");
    });
});

describe("FlowAdvanced: nested For", () => {
    it("renders a grid with outer and inner index accessors", () => {
        const r0 = ["a", "b"];
        const r1 = ["c"];
        const [grid, setGrid] = reactive([r0, r1]);
        const {container} = mount(Grid, {
            get rows() {
                return grid();
            }
        });
        const cells = () => [...container.querySelectorAll("tr")].map(tr => [...tr.querySelectorAll("td")].map(text));
        expect(cells()).toEqual([["0,0:a", "0,1:b"], ["1,0:c"]]);

        const firstTr = container.querySelector("tr");
        expect(firstTr).not.toBeNull();
        act(() => setGrid([r1, r0]));
        expect(cells()).toEqual([["0,0:c"], ["1,0:a", "1,1:b"]]);
        expect(container.querySelectorAll("tr")[1]).toBe(firstTr);
    });
});

describe("FlowAdvanced: Show inside For rows", () => {
    it("marks the selected row and survives removal of other rows", () => {
        const {container} = mount(Picker);
        const opt = id => container.querySelector(`#opt-${id}`);
        expect(container.querySelector(".tick")).toBeNull();

        click(opt(2));
        expect(text(container.querySelector(".current"))).toBe("2");
        expect(opt(2).querySelector(".tick")).not.toBeNull();
        expect(container.querySelectorAll(".tick")).toHaveLength(1);

        click(opt(3));
        expect(opt(2).querySelector(".tick")).toBeNull();
        expect(opt(3).querySelector(".tick")).not.toBeNull();

        const three = opt(3);
        click(container.querySelector(".drop-first"));
        expect(opt(1)).toBeNull();
        expect(opt(3)).toBe(three);
        expect(three.querySelector(".tick")).not.toBeNull();
        expect(text(three)).toBe("three*");
    });
});

describe("FlowAdvanced: Repeat", () => {
    it("renders nothing at zero without a fallback and grows/shrinks", () => {
        const [count, setCount] = reactive(0);
        const {root} = mount(Dots, {
            get count() {
                return count();
            }
        });
        expect(root.innerHTML).toBe("");
        act(() => setCount(2));
        expect([...root.querySelectorAll("i.dot")].map(text)).toEqual(["0", "1"]);
        const first = root.querySelector("i.dot");
        act(() => setCount(4));
        expect(root.querySelector("i.dot")).toBe(first);
        expect(root.querySelectorAll("i.dot")).toHaveLength(4);
        act(() => setCount(0));
        expect(root.innerHTML).toBe("");
    });

    it("works nested in a keyed For", () => {
        const [items, setItems] = reactive([{id: 1, label: "a"}, {id: 3, label: "c"}]);
        const {container} = mount(Ratings, {
            get items() {
                return items();
            }
        });
        const stars = () => [...container.querySelectorAll("li.rating")].map(text);
        expect(stars()).toEqual(["*", "***"]);
        act(() => setItems([{id: 2, label: "b"}]));
        expect(stars()).toEqual(["**"]);
    });
});
