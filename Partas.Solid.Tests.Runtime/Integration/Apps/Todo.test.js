import {describe, it, expect, vi} from "vitest";
import {mount, click, input, flush, act, text} from "../../helpers/index.js";
import {TodoApp} from "./A-Todo.fs.jsx";

const $ = (root, sel) => root.querySelector(sel);
const $$ = (root, sel) => [...root.querySelectorAll(sel)];
const labels = root => $$(root, ".todo-list li.todo .label").map(text);
const rowById = (root, id) => $(root, `li.todo[data-id="${id}"]`);

// The F# default `props.onRemainingChange <- ignore` is dropped by the plugin (see the it.fails
// test at the bottom), so every other test passes an explicit callback.
const mountTodo = (initial, extra = {}) =>
    mount(TodoApp, {
        initial, onRemainingChange: () => {
        }, ...extra
    });

/** Type into the new-todo input and submit the form by clicking the submit button. */
function addTodo(root, value) {
    input($(root, "input.new-todo"), value);
    click($(root, "button.add-btn"));
}

describe("Apps: todo list (store + memos + keyed For)", () => {
    it("renders seeded todos with positions and a pluralised count", () => {
        const {container} = mountTodo(["milk", "eggs"]);
        expect(labels(container)).toEqual(["milk", "eggs"]);
        expect($$(container, ".todo-list .pos").map(text)).toEqual(["1", "2"]);
        expect(text($(container, ".count"))).toBe("2 items left");
        expect($(container, ".todo-list li.empty")).toBeNull();
        // Nothing completed yet: the clear button is not rendered.
        expect($(container, "button.clear")).toBeNull();
    });

    it("shows the For fallback when there are no todos", () => {
        const {container} = mountTodo([]);
        expect(text($(container, ".todo-list li.empty"))).toBe("Nothing to show");
        expect(text($(container, ".count"))).toBe("0 items left");
    });

    it("adds a todo through the controlled input + form submit, then clears the input", () => {
        const {container} = mountTodo(["milk"]);
        const field = $(container, "input.new-todo");
        addTodo(container, "  bread  ");
        expect(labels(container)).toEqual(["milk", "bread"]);
        expect(rowById(container, 2)).not.toBeNull();
        // Controlled input: setDraft("") propagated back to the DOM property.
        expect(field.value).toBe("");
        expect(text($(container, ".count"))).toBe("2 items left");
    });

    it("ignores blank submissions", () => {
        const {container} = mountTodo([]);
        addTodo(container, "   ");
        expect(labels(container)).toEqual([]);
        expect($(container, ".todo-list li.empty")).not.toBeNull();
    });

    it("toggling a checkbox mutates the store draft in place: class, checked, counts", () => {
        const {container} = mountTodo(["a", "b", "c"]);
        const rowB = rowById(container, 2);
        click($(rowB, "input.toggle"));
        // Same <li> node: in-place draft mutation must not recreate the keyed row.
        expect(rowById(container, 2)).toBe(rowB);
        expect(rowB.className).toBe("todo done");
        expect($(rowB, "input.toggle").checked).toBe(true);
        expect(text($(container, ".count"))).toBe("2 items left");
        expect(text($(container, "button.clear"))).toBe("Clear completed (1)");

        click($(rowB, "input.toggle"));
        expect(rowB.className).toBe("todo");
        expect($(rowB, "input.toggle").checked).toBe(false);
        expect($(container, "button.clear")).toBeNull();
    });

    it("uses singular 'item' when exactly one remains", () => {
        const {container} = mountTodo(["a", "b"]);
        click($(rowById(container, 1), "input.toggle"));
        expect(text($(container, ".count"))).toBe("1 item left");
    });

    it("removes a todo and renumbers the remaining rows", () => {
        const {container} = mountTodo(["a", "b", "c"]);
        const rowC = rowById(container, 3);
        click($(rowById(container, 2), "button.remove"));
        expect(labels(container)).toEqual(["a", "c"]);
        expect(rowById(container, 2)).toBeNull();
        // The index accessor of a surviving keyed row updates in place.
        expect(rowById(container, 3)).toBe(rowC);
        expect(text($(rowC, ".pos"))).toBe("2");
    });

    it("filters by active / completed and marks the selected filter button", () => {
        const {container} = mountTodo(["a", "b", "c"]);
        click($(rowById(container, 1), "input.toggle"));
        click($(rowById(container, 3), "input.toggle"));

        const filterBtn = f => $(container, `button.filter[data-filter="${f}"]`);
        expect($$(container, "button.filter").map(text)).toEqual(["all", "active", "completed"]);
        expect(filterBtn("all").className).toBe("filter selected");

        click(filterBtn("active"));
        expect(labels(container)).toEqual(["b"]);
        expect(filterBtn("active").className).toBe("filter selected");
        expect(filterBtn("all").className).toBe("filter");

        click(filterBtn("completed"));
        expect(labels(container)).toEqual(["a", "c"]);

        // A row toggled while filtered out of view disappears from the completed view.
        click($(rowById(container, 1), "input.toggle"));
        expect(labels(container)).toEqual(["c"]);

        click(filterBtn("all"));
        expect(labels(container)).toEqual(["a", "b", "c"]);
    });

    it("shows the fallback when the active filter matches nothing", () => {
        const {container} = mountTodo(["a"]);
        click($(container, `button.filter[data-filter="completed"]`));
        expect(labels(container)).toEqual([]);
        expect(text($(container, ".todo-list li.empty"))).toBe("Nothing to show");
    });

    it("clear completed removes exactly the completed todos", () => {
        const {container} = mountTodo(["a", "b", "c", "d"]);
        click($(rowById(container, 2), "input.toggle"));
        click($(rowById(container, 4), "input.toggle"));
        expect(text($(container, "button.clear"))).toBe("Clear completed (2)");
        click($(container, "button.clear"));
        expect(labels(container)).toEqual(["a", "c"]);
        expect($(container, "button.clear")).toBeNull();
        expect(text($(container, ".count"))).toBe("2 items left");
    });

    it("toggle all completes everything, then un-completes everything", () => {
        const {container} = mountTodo(["a", "b", "c"]);
        click($(rowById(container, 2), "input.toggle"));
        click($(container, "button.toggle-all"));
        expect($$(container, "li.todo").map(li => li.className)).toEqual(["todo done", "todo done", "todo done"]);
        expect(text($(container, ".count"))).toBe("0 items left");
        click($(container, "button.toggle-all"));
        expect($$(container, "li.todo").map(li => li.className)).toEqual(["todo", "todo", "todo"]);
        expect(text($(container, ".count"))).toBe("3 items left");
    });

    it("new ids keep increasing after removals (no id reuse)", () => {
        const {container} = mountTodo(["a", "b"]);
        click($(rowById(container, 2), "button.remove"));
        addTodo(container, "c");
        expect($$(container, "li.todo").map(li => li.dataset.id)).toEqual(["1", "3"]);
    });

    it("the remaining-count effect reports each distinct settled value once, in order", () => {
        const onRemainingChange = vi.fn();
        const {container} = mountTodo(["a", "b"], {onRemainingChange});
        expect(onRemainingChange.mock.calls).toEqual([[2]]);

        addTodo(container, "c");
        expect(onRemainingChange.mock.calls).toEqual([[2], [3]]);

        // Two writes in one flush settle once: 3 -> 1 without reporting 2.
        act(() => {
            $(rowById(container, 1), "input.toggle").click();
            $(rowById(container, 2), "input.toggle").click();
        });
        expect(onRemainingChange.mock.calls).toEqual([[2], [3], [1]]);

        // Removing a completed todo leaves "remaining" at 1: memo equality stops the effect.
        click($(rowById(container, 1), "button.remove"));
        expect(onRemainingChange).toHaveBeenCalledTimes(3);
    });

    it("does not update the DOM before the scheduler flushes (Solid 2 batching)", () => {
        const {container} = mountTodo(["a"]);
        const toggle = $(rowById(container, 1), "input.toggle");
        toggle.click(); // no flush
        expect(text($(container, ".count"))).toBe("1 item left");
        flush();
        expect(text($(container, ".count"))).toBe("0 items left");
    });

    // BUG: `props.onRemainingChange <- ignore` default is dropped from the emitted merge({...}).
    it.fails("works without the optional onRemainingChange callback (default prop via merge)", () => {
        const {container} = mount(TodoApp, {initial: ["a"]});
        expect(() => click($(rowById(container, 1), "input.toggle"))).not.toThrow();
        expect(text($(container, ".count"))).toBe("0 items left");
    });
});
