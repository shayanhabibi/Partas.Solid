import {describe, it, expect} from "vitest";
import {mount} from "../../helpers/index.js";
import {
    ListToArray,
    ArrayOfList,
    SeqToArray,
    PipedToArray,
    ListToArrayLiteral,
    ArrayMapped,
    ListMappedToArray,
    ToArrayInLet
} from "./D-Conversions.fs.jsx";

const rows = (container) => [...container.querySelectorAll("li")].map((li) => li.textContent);

describe("FlowAdvanced: For over a list converted to an array", () => {
    it("Array.map over an array signal", () => {
        expect(rows(mount(ArrayMapped).container)).toEqual(["a", "b", "c"]);
    });

    it("List.toArray", () => {
        expect(rows(mount(ListToArray).container)).toEqual(["a", "b", "c"]);
    });

    it("Array.ofList", () => {
        expect(rows(mount(ArrayOfList).container)).toEqual(["a", "b", "c"]);
    });

    it("Seq.toArray", () => {
        expect(rows(mount(SeqToArray).container)).toEqual(["a", "b", "c"]);
    });

    it("piped into List.toArray", () => {
        expect(rows(mount(PipedToArray).container)).toEqual(["a", "b", "c"]);
    });

    it("List.toArray over a list literal", () => {
        expect(rows(mount(ListToArrayLiteral).container)).toEqual(["a", "b", "c"]);
    });

    it("List.map then List.toArray", () => {
        expect(rows(mount(ListMappedToArray).container)).toEqual(["a", "b", "c"]);
    });

    it("List.toArray inside a local function", () => {
        expect(rows(mount(ToArrayInLet).container)).toEqual(["a", "b", "c"]);
    });
});
