import { describe, it, expect } from "vitest";
import { mount } from "../../helpers/index.js";
import { Icon, Chart } from "./E-Svg.fs.jsx";

const SVG_NS = "http://www.w3.org/2000/svg";
const XLINK_NS = "http://www.w3.org/1999/xlink";

describe("Dom/Elements SVG", () => {
  it("creates the svg root and every descendant in the SVG namespace", () => {
    const { root } = mount(Icon);
    expect(root.namespaceURI).toBe(SVG_NS);
    expect(root).toBeInstanceOf(SVGSVGElement);
    for (const el of root.querySelectorAll("*")) {
      expect(el.namespaceURI, el.tagName).toBe(SVG_NS);
    }
    // jsdom has no per-tag SVG classes; localName + namespace is the equivalent check
    expect(["#p", "#c", "#r", "#t"].map(s => root.querySelector(s).localName)).toEqual(["path", "circle", "rect", "text"]);
    expect(root.querySelector("#p")).toBeInstanceOf(SVGElement);
  });

  it("keeps case-sensitive attribute names such as viewBox and applies numeric values as attributes", () => {
    const { root } = mount(Icon);
    expect(root.getAttribute("viewBox")).toBe("0 0 24 24");
    expect(root.hasAttribute("viewbox")).toBe(false);
    expect(root.viewBox.baseVal.width).toBe(24);
    expect(root.getAttribute("width")).toBe("24");
    expect(root.getAttribute("height")).toBe("24");
    expect(root.getAttribute("fill")).toBe("none");
    expect(root.querySelector("#sym").getAttribute("viewBox")).toBe("0 0 10 10");
  });

  it("renders path d, stroke and hyphenated stroke-width", () => {
    const { root } = mount(Icon);
    const p = root.querySelector("#p");
    expect(p.getAttribute("d")).toBe("M0 0L24 24");
    expect(p.getAttribute("stroke")).toBe("red");
    expect(p.getAttribute("stroke-width")).toBe("2");
    expect(p.hasAttribute("strokeWidth")).toBe(false);
  });

  it("renders geometry attributes from ints and strings alike", () => {
    const { root } = mount(Icon);
    const c = root.querySelector("#c");
    expect([c.getAttribute("cx"), c.getAttribute("cy"), c.getAttribute("r")]).toEqual(["12", "12", "5"]);
    const r = root.querySelector("#r");
    expect([r.getAttribute("x"), r.getAttribute("y"), r.getAttribute("width"), r.getAttribute("height")])
      .toEqual(["1", "2", "3", "4"]);
  });

  it("applies class on SVG elements as the class attribute", () => {
    const { root } = mount(Icon);
    const g = root.querySelector("#grp");
    expect(g.getAttribute("class")).toBe("group");
    expect(g.classList.contains("group")).toBe(true);
  });

  it("renders use href and xlink:href (the latter in the xlink namespace)", () => {
    const { root } = mount(Icon);
    expect(root.querySelector("#u1").getAttribute("href")).toBe("#sym");
    const u2 = root.querySelector("#u2");
    expect(u2.getAttributeNS(XLINK_NS, "href")).toBe("#sym");
    expect(u2.getAttribute("xlink:href")).toBe("#sym");
  });

  it("renders SVG text content", () => {
    const { root } = mount(Icon);
    expect(root.querySelector("#t").textContent).toBe("label");
  });

  it("an SVG child from its own component, placed inside an svg, is an SVG element", () => {
    const { root } = mount(Chart);
    expect(root.namespaceURI).toBe(SVG_NS);
    const dot = root.querySelector("#dot");
    expect(dot).not.toBeNull();
    expect(dot.namespaceURI).toBe(SVG_NS);
    expect(dot).toBeInstanceOf(SVGElement);
    expect(dot.localName).toBe("circle");
  });
});
