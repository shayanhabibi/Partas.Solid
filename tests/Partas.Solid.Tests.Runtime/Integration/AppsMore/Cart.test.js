import {describe, it, expect, vi} from "vitest";
import {mount, act, text} from "../../helpers/index.js";
import {click, input} from "./guard.js";
import {ShopApp, money} from "./E-Cart.fs.jsx";

const $ = (root, sel) => root.querySelector(sel);
const $$ = (root, sel) => [...root.querySelectorAll(sel)];

const catalog = [
    {sku: "mug", name: "Mug", price: 1250},
    {sku: "tee", name: "T-Shirt", price: 2000},
    {sku: "cap", name: "Cap", price: 999}
];

const add = (c, sku) => click($(c, `.catalog-item[data-sku="${sku}"] button.add`));
const line = (c, sku) => $(c, `li.line[data-sku="${sku}"]`);
const lines = c => $$(c, "li.line").map(l => [l.dataset.sku, $(l, "input.qty").value, text($(l, ".line-total"))]);
const summary = c => ({
    subtotal: text($(c, ".subtotal")),
    discount: $(c, ".discount") ? text($(c, ".discount")) : null,
    shipping: text($(c, ".shipping")),
    total: text($(c, ".total"))
});

const setup = (extra = {}) => {
    const onTotal = vi.fn();
    const {container} = mount(ShopApp, {catalog, onTotal, ...extra});
    return {c: container, onTotal};
};

describe("AppsMore: shopping cart (store lines + memo totals)", () => {
    it("formats cents as money", () => {
        expect(money(0)).toBe("$0.00");
        expect(money(5)).toBe("$0.05");
        expect(money(1250)).toBe("$12.50");
        expect(money(-125)).toBe("-$1.25");
    });

    it("renders the catalog and an empty cart", () => {
        const {c} = setup();
        expect($$(c, ".catalog-item .name").map(text)).toEqual(["Mug", "T-Shirt", "Cap"]);
        expect($$(c, ".catalog-item .price").map(text)).toEqual(["$12.50", "$20.00", "$9.99"]);
        expect(text($(c, "li.empty"))).toBe("Your cart is empty");
        expect(text($(c, ".cart h2"))).toBe("Cart (0)");
        expect(summary(c)).toEqual({subtotal: "$0.00", discount: null, shipping: "Free", total: "$0.00"});
        expect($(c, "button.checkout").disabled).toBe(true);
    });

    it("adding items creates lines, and adding again increments the existing line", () => {
        const {c} = setup();
        add(c, "mug");
        add(c, "cap");
        add(c, "mug");
        expect(lines(c)).toEqual([
            ["mug", "2", "$25.00"],
            ["cap", "1", "$9.99"]
        ]);
        expect(text($(c, ".cart h2"))).toBe("Cart (3)");
        expect(summary(c)).toEqual({subtotal: "$34.99", discount: null, shipping: "$4.99", total: "$39.98"});
        expect($(c, "button.checkout").disabled).toBe(false);
        expect($(c, "li.empty")).toBeNull();
    });

    it("incrementing an existing line mutates it in place (same row node)", () => {
        const {c} = setup();
        add(c, "tee");
        const row = line(c, "tee");
        click($(row, "button.inc"));
        add(c, "tee");
        expect(line(c, "tee")).toBe(row);
        expect($(row, "input.qty").value).toBe("3");
        expect(text($(row, ".line-total"))).toBe("$60.00");
    });

    it("the - button decrements, and removes the line when it reaches zero", () => {
        const {c} = setup();
        add(c, "cap");
        add(c, "cap");
        click($(line(c, "cap"), "button.dec"));
        expect(lines(c)).toEqual([["cap", "1", "$9.99"]]);
        click($(line(c, "cap"), "button.dec"));
        expect(lines(c)).toEqual([]);
        expect(text($(c, "li.empty"))).toBe("Your cart is empty");
    });

    it("typing a quantity sets it; 0 or garbage removes the line", () => {
        const {c} = setup();
        add(c, "mug");
        add(c, "tee");
        input($(line(c, "mug"), "input.qty"), "4", "change");
        expect(lines(c)[0]).toEqual(["mug", "4", "$50.00"]);
        expect(text($(c, ".cart h2"))).toBe("Cart (5)");
        input($(line(c, "tee"), "input.qty"), "abc", "change");
        expect(lines(c).map(l => l[0])).toEqual(["mug"]);
        input($(line(c, "mug"), "input.qty"), "0", "change");
        expect(lines(c)).toEqual([]);
    });

    it("Remove deletes just that line and keeps the other rows' nodes", () => {
        const {c} = setup();
        add(c, "mug");
        add(c, "tee");
        add(c, "cap");
        const capRow = line(c, "cap");
        click($(line(c, "tee"), "button.remove"));
        expect(lines(c).map(l => l[0])).toEqual(["mug", "cap"]);
        expect(line(c, "cap")).toBe(capRow);
    });

    it("free shipping at $50 or more after discount", () => {
        const {c} = setup();
        add(c, "tee");
        add(c, "tee");
        expect(summary(c).shipping).toBe("$4.99");
        add(c, "cap");
        expect(summary(c)).toEqual({subtotal: "$49.99", discount: null, shipping: "$4.99", total: "$54.98"});
        add(c, "mug");
        expect(summary(c)).toEqual({subtotal: "$62.49", discount: null, shipping: "Free", total: "$62.49"});
    });

    it("SAVE10 takes 10% off and can push the order back under free shipping", () => {
        const {c} = setup();
        add(c, "tee");
        add(c, "tee");
        add(c, "mug"); // 52.50
        expect(summary(c).shipping).toBe("Free");
        input($(c, "input.code"), " save10 ");
        click($(c, "button.apply"));
        // 52.50 - 5.25 = 47.25, under the threshold again.
        expect(summary(c)).toEqual({subtotal: "$52.50", discount: "-$5.25", shipping: "$4.99", total: "$52.24"});
    });

    it("FIVEOFF is capped at the subtotal; an unknown code shows an error", () => {
        const {c} = setup();
        add(c, "cap");
        input($(c, "input.code"), "bogus");
        click($(c, "button.apply"));
        expect(text($(c, ".code-error"))).toBe("Code not valid");
        expect(summary(c).discount).toBeNull();
        input($(c, "input.code"), "FIVEOFF");
        click($(c, "button.apply"));
        expect($(c, ".code-error")).toBeNull();
        expect(summary(c)).toEqual({subtotal: "$9.99", discount: "-$5.00", shipping: "$4.99", total: "$9.98"});
    });

    it("the discount tracks the cart after the code is applied", () => {
        const {c} = setup();
        add(c, "tee");
        input($(c, "input.code"), "SAVE10");
        click($(c, "button.apply"));
        expect(summary(c).discount).toBe("-$2.00");
        add(c, "tee");
        expect(summary(c).discount).toBe("-$4.00");
        click($(line(c, "tee"), "button.remove"));
        expect(summary(c).discount).toBeNull();
        expect(summary(c).total).toBe("$0.00");
    });

    it("the total memo recomputes once per batched change, and not for unrelated input", () => {
        const {c, onTotal} = setup();
        expect(onTotal.mock.calls).toEqual([[0]]);
        add(c, "mug");
        expect(onTotal.mock.calls.at(-1)).toEqual([1250 + 499]);
        const n = onTotal.mock.calls.length;
        // Typing a promo code (not yet applied) does not touch the total.
        input($(c, "input.code"), "SAVE10");
        expect(onTotal).toHaveBeenCalledTimes(n);
        // Two quantity writes in one flush: the total settles once. Both handlers read the
        // committed qty (1) because store writes are not visible until the flush, so both write 2.
        act(() => {
            $(line(c, "mug"), "button.inc").click();
            $(line(c, "mug"), "button.inc").click();
        });
        expect(onTotal).toHaveBeenCalledTimes(n + 1);
        expect($(line(c, "mug"), "input.qty").value).toBe("2");
        expect(text($(c, ".total"))).toBe("$29.99");
    });
});
