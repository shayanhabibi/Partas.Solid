import {describe, it, expect, vi} from "vitest";
import {createSignal} from "solid-js";
import {mount, click, input, act, flush, text} from "../../helpers/index.js";
import {DataTable} from "./E-DataTable.fs.jsx";

const $ = (root, sel) => root.querySelector(sel);
const $$ = (root, sel) => [...root.querySelectorAll(sel)];

const people = [
    {id: 1, name: "Carol", age: 30, city: "London"},
    {id: 2, name: "alice", age: 9, city: "Paris"},
    {id: 3, name: "Bob", age: 100, city: "Berlin"},
    {id: 4, name: "Dave", age: 25, city: "Londonderry"}
];

const col = (c, cls) => $$(c, `tbody td.${cls}`).map(text);
const ids = c => $$(c, "tbody tr[data-id]").map(r => r.dataset.id);
const header = (c, k) => $(c, `th[data-key="${k}"] button.sort`);

function setup(rows = people) {
    const onSortComputed = vi.fn();
    const {container} = mount(DataTable, {rows, onSortComputed});
    return {c: container, onSortComputed};
}

describe("Apps: sortable, filterable data table", () => {
    it("renders headers, rows in source order, positions and summary", () => {
        const {c} = setup();
        expect($$(c, "thead button.sort").map(text)).toEqual(["name", "age", "city"]);
        expect(ids(c)).toEqual(["1", "2", "3", "4"]);
        expect(col(c, "pos")).toEqual(["1", "2", "3", "4"]);
        expect(col(c, "age")).toEqual(["30", "9", "100", "25"]);
        expect(text($(c, ".summary"))).toBe("4 of 4 rows");
    });

    it("a header click cycles ascending -> descending -> original order", () => {
        const {c} = setup();
        click(header(c, "name"));
        // Ordinal string compare: upper-case letters sort before lower-case.
        expect(col(c, "name")).toEqual(["Bob", "Carol", "Dave", "alice"]);
        click(header(c, "name"));
        expect(col(c, "name")).toEqual(["alice", "Dave", "Carol", "Bob"]);
        click(header(c, "name"));
        expect(ids(c)).toEqual(["1", "2", "3", "4"]);
    });

    it("sorts numbers numerically, not lexicographically", () => {
        const {c} = setup();
        click(header(c, "age"));
        expect(col(c, "age")).toEqual(["9", "25", "30", "100"]);
        click(header(c, "age"));
        expect(col(c, "age")).toEqual(["100", "30", "25", "9"]);
    });

    it("switching to another column restarts at ascending", () => {
        const {c} = setup();
        click(header(c, "age"));
        click(header(c, "age")); // age desc
        click(header(c, "city"));
        expect(col(c, "city")).toEqual(["Berlin", "London", "Londonderry", "Paris"]);
    });

    it("keyed rows move instead of re-rendering, and positions follow the new order", () => {
        const {c} = setup();
        const rowsBefore = new Map($$(c, "tbody tr").map(r => [r.dataset.id, r]));
        click(header(c, "age"));
        const rowsAfter = $$(c, "tbody tr");
        expect(rowsAfter.map(r => r.dataset.id)).toEqual(["2", "4", "1", "3"]);
        rowsAfter.forEach(r => expect(r).toBe(rowsBefore.get(r.dataset.id)));
        expect(col(c, "pos")).toEqual(["1", "2", "3", "4"]);
    });

    it("filters case-insensitively on name or city and keeps the sort", () => {
        const {c} = setup();
        click(header(c, "age"));
        input($(c, "input.filter"), "LONDON");
        expect(col(c, "name")).toEqual(["Dave", "Carol"]);
        expect(text($(c, ".summary"))).toBe("2 of 4 rows");
        input($(c, "input.filter"), "ali");
        expect(col(c, "name")).toEqual(["alice"]);
        input($(c, "input.filter"), "  ");
        expect(ids(c)).toEqual(["2", "4", "1", "3"]);
    });

    it("shows the fallback row when the filter matches nothing, then recovers", () => {
        const {c} = setup();
        input($(c, "input.filter"), "zzz");
        expect(ids(c)).toEqual([]);
        expect(text($(c, "tbody tr.empty"))).toBe("No rows");
        expect($(c, "tbody tr.empty td").getAttribute("colspan")).toBe("4");
        expect(text($(c, ".summary"))).toBe("0 of 4 rows");
        input($(c, "input.filter"), "");
        expect($(c, "tbody tr.empty")).toBeNull();
        expect(ids(c)).toEqual(["1", "2", "3", "4"]);
    });

    it("the sorted memo recomputes once per interaction, and once for batched interactions", () => {
        const {c, onSortComputed} = setup();
        expect(onSortComputed).toHaveBeenCalledTimes(1);
        click(header(c, "name"));
        expect(onSortComputed).toHaveBeenCalledTimes(2);
        // Two signal writes (dir + key) in the third click, and two clicks in one flush: one recompute.
        act(() => {
            header(c, "name").click();
            header(c, "name").click();
        });
        expect(onSortComputed).toHaveBeenCalledTimes(3);
        // An input event that leaves the query signal unchanged does not recompute.
        input($(c, "input.filter"), "");
        flush();
        expect(onSortComputed).toHaveBeenCalledTimes(3);
    });

    it("reacts to a reactive rows prop (getter-backed props survive the omit)", () => {
        const [rows, setRows] = createSignal(people.slice(0, 2));
        const {container: c} = mount(DataTable, {
            get rows() {
                return rows();
            },
            onSortComputed: () => {
            }
        });
        click(header(c, "age"));
        expect(col(c, "name")).toEqual(["alice", "Carol"]);
        act(() => setRows(people));
        expect(col(c, "name")).toEqual(["alice", "Dave", "Carol", "Bob"]);
        expect(text($(c, ".summary"))).toBe("4 of 4 rows");
    });

    it("renders the empty-table fallback for zero rows", () => {
        const {c} = setup([]);
        expect(text($(c, "tbody tr.empty"))).toBe("No rows");
        expect(text($(c, ".summary"))).toBe("0 of 0 rows");
    });

    // BUG: ariaSort is emitted verbatim as `ariaSort=` instead of `aria-sort=`.
    it.fails("exposes the sort state through aria-sort", () => {
        const {c} = setup();
        const th = k => $(c, `th[data-key="${k}"]`);
        expect(th("name").getAttribute("aria-sort")).toBe("none");
        click(header(c, "name"));
        expect(th("name").getAttribute("aria-sort")).toBe("ascending");
        expect(th("age").getAttribute("aria-sort")).toBe("none");
        click(header(c, "name"));
        expect(th("name").getAttribute("aria-sort")).toBe("descending");
    });
});
