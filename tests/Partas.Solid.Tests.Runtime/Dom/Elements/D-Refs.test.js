import {describe, it, expect} from "vitest";
import {mount, click} from "../../helpers/index.js";
import {RefCallback, RefVariable, RefSeesAttributes} from "./D-Refs.fs.jsx";

describe("Dom/Elements refs", () => {
    it("a ref callback receives the rendered element exactly once", () => {
        const got = [];
        const {container} = mount(RefCallback, {got: el => got.push(el)});
        expect(got).toHaveLength(1);
        expect(got[0]).toBe(container.querySelector("#target"));
        expect(got[0]).toBeInstanceOf(HTMLDivElement);
        // the consumed "got" prop is not rendered
        expect(container.querySelector(".rc").getAttributeNames()).toEqual(["class"]);
    });

    it("a ref bound to a local variable lets a later handler drive the element", () => {
        const {container} = mount(RefVariable);
        const field = container.querySelector("#field");
        expect(field.value).toBe("");
        click(container.querySelector("#fill"));
        expect(field.value).toBe("filled via ref");
        expect(field.hasAttribute("ref")).toBe(false);
    });

    it("static attributes are already applied when the ref callback runs", () => {
        const reports = [];
        mount(RefSeesAttributes, {report: s => reports.push(s)});
        expect(reports).toEqual(["SECTION|sec|has-attrs"]);
    });
});
