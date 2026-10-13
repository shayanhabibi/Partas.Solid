import {describe, it, expect} from "vitest";
import {mount, text} from "../../helpers/index.js";
import {click, input} from "./guard.js";
import {PagedList} from "./C-Pager.fs.jsx";

const $ = (root, sel) => root.querySelector(sel);
const $$ = (root, sel) => [...root.querySelectorAll(sel)];

const fruit = ["Apple", "Apricot", "Banana", "Blueberry", "Cherry", "Grape", "Lemon", "Mango"];
const veg = ["Carrot", "Celery", "Kale", "Leek", "Onion", "Pepper"];
const products = [
    ...fruit.map((name, i) => ({id: i + 1, name, category: "fruit"})),
    ...veg.map((name, i) => ({id: 100 + i, name, category: "veg"}))
]; // 14 products

const names = c => $$(c, "li.result").map(text);
const pageButtons = c => $$(c, "button.page").map(text);
const currentPage = c => $$(c, "button.page.current").map(text);
const setup = (props = {}) => mount(PagedList, {products, ...props}).container;

describe("AppsMore: paginated list with store-backed filtering", () => {
    it("shows the first page with the default page size of 5", () => {
        const c = setup();
        expect(names(c)).toEqual(["Apple", "Apricot", "Banana", "Blueberry", "Cherry"]);
        expect(pageButtons(c)).toEqual(["1", "2", "3"]);
        expect(currentPage(c)).toEqual(["1"]);
        expect(text($(c, ".range"))).toBe("Showing 1-5 of 14");
        expect(text($(c, ".page-of"))).toBe("Page 1 of 3");
    });

    it("builds the category options from the data (sorted, distinct)", () => {
        const c = setup();
        expect($$(c, "select.category option").map(o => o.value)).toEqual(["all", "fruit", "veg"]);
    });

    it("Prev is disabled on the first page, Next on the last", () => {
        const c = setup();
        expect($(c, "button.prev").disabled).toBe(true);
        expect($(c, "button.next").disabled).toBe(false);
        click($(c, 'button.page[data-page="3"]'));
        expect($(c, "button.prev").disabled).toBe(false);
        expect($(c, "button.next").disabled).toBe(true);
        expect(names(c)).toEqual(["Kale", "Leek", "Onion", "Pepper"]);
        expect(text($(c, ".range"))).toBe("Showing 11-14 of 14");
    });

    it("Next / Prev step through pages", () => {
        const c = setup();
        click($(c, "button.next"));
        expect(currentPage(c)).toEqual(["2"]);
        expect(names(c)).toEqual(["Grape", "Lemon", "Mango", "Carrot", "Celery"]);
        click($(c, "button.next"));
        click($(c, "button.prev"));
        expect(text($(c, ".page-of"))).toBe("Page 2 of 3");
    });

    it("page buttons are stable nodes while the page count is unchanged", () => {
        const c = setup();
        const before = $$(c, "button.page");
        click(before[1]);
        const after = $$(c, "button.page");
        after.forEach((b, i) => expect(b).toBe(before[i]));
        expect(before.map(b => b.className)).toEqual(["page", "page current", "page"]);
    });

    it("typing a search filters case-insensitively and resets to page 1", () => {
        const c = setup();
        click($(c, 'button.page[data-page="2"]'));
        input($(c, "input.search"), "AP");
        expect(names(c)).toEqual(["Apple", "Apricot", "Grape"]);
        expect(currentPage(c)).toEqual(["1"]);
        expect(pageButtons(c)).toEqual(["1"]);
        expect(text($(c, ".range"))).toBe("Showing 1-3 of 3");
        expect($(c, "button.next").disabled).toBe(true);
    });

    it("the search box is controlled by the store", () => {
        const c = setup();
        const box = $(c, "input.search");
        input(box, "  le ");
        expect(box.value).toBe("  le ");
        // Trimmed for matching.
        expect(names(c)).toEqual(["Apple", "Lemon", "Celery", "Kale", "Leek"]);
        expect(text($(c, ".range"))).toBe("Showing 1-5 of 5");
    });

    it("a search with no hits shows the fallback, 'No results' and a single disabled page", () => {
        const c = setup();
        input($(c, "input.search"), "zzz");
        expect(names(c)).toEqual([]);
        expect(text($(c, "li.empty"))).toBe("Nothing found");
        expect(text($(c, ".range"))).toBe("No results");
        expect(pageButtons(c)).toEqual(["1"]);
        expect($(c, "button.prev").disabled).toBe(true);
        expect($(c, "button.next").disabled).toBe(true);
        input($(c, "input.search"), "");
        expect($(c, "li.empty")).toBeNull();
        expect(names(c)).toHaveLength(5);
    });

    it("the category select filters and combines with the search", () => {
        const c = setup();
        input($(c, "select.category"), "veg", "change");
        expect(names(c)).toEqual(["Carrot", "Celery", "Kale", "Leek", "Onion"]);
        expect(pageButtons(c)).toEqual(["1", "2"]);
        input($(c, "input.search"), "e");
        expect(names(c)).toEqual(["Celery", "Kale", "Leek", "Pepper"]);
        expect(pageButtons(c)).toEqual(["1"]);
        input($(c, "input.search"), "c");
        expect(names(c)).toEqual(["Carrot", "Celery"]);
        input($(c, "select.category"), "all", "change");
        expect(names(c)).toEqual(["Apricot", "Cherry", "Carrot", "Celery"]);
    });

    it("changing the page size re-pages from the first page", () => {
        const c = setup();
        click($(c, "button.next"));
        input($(c, "select.page-size"), "10", "change");
        expect(currentPage(c)).toEqual(["1"]);
        expect(names(c)).toHaveLength(10);
        expect(pageButtons(c)).toEqual(["1", "2"]);
        input($(c, "select.page-size"), "2", "change");
        expect(pageButtons(c)).toHaveLength(7);
        expect(text($(c, ".range"))).toBe("Showing 1-2 of 14");
        expect($(c, "select.page-size").value).toBe("2");
    });

    it("initialPageSize overrides the merged default", () => {
        const c = setup({initialPageSize: 10});
        expect(names(c)).toHaveLength(10);
        expect($(c, "select.page-size").value).toBe("10");
        expect(text($(c, ".page-of"))).toBe("Page 1 of 2");
    });

    it("rows are keyed: a row surviving a page-size change keeps its node", () => {
        const c = setup();
        const apple = $(c, 'li.result[data-id="1"]');
        input($(c, "select.page-size"), "10", "change");
        expect($(c, 'li.result[data-id="1"]')).toBe(apple);
    });
});
