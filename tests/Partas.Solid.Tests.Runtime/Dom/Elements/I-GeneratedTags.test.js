import { describe, it, expect } from "vitest";
import { mount } from "../../helpers/index.js";
import { NewSlot, NewTemplate, NewSelectedContent, FixedElements, Formula } from "./I-GeneratedTags.fs.jsx";

const MATHML_NS = "http://www.w3.org/1998/Math/MathML";

describe("Dom/Elements generated tags", () => {
  it("renders <slot> with its name", () => {
    const { root } = mount(NewSlot);
    const slot = root.querySelector("#slot");
    expect(slot).toBeInstanceOf(HTMLSlotElement);
    expect(slot.getAttribute("name")).toBe("header");
  });

  it("renders <template> with its shadowrootmode", () => {
    const { root } = mount(NewTemplate);
    const tpl = root.querySelector("#tpl");
    expect(tpl).toBeInstanceOf(HTMLTemplateElement);
    expect(tpl.getAttribute("shadowrootmode")).toBe("open");
  });

  it("renders <selectedcontent>", () => {
    const { root } = mount(NewSelectedContent);
    expect(root.querySelector("#sc").localName).toBe("selectedcontent");
  });

  it("gives <base> its href and target", () => {
    const { root } = mount(FixedElements);
    const base = root.querySelector("#base");
    expect(base).toBeInstanceOf(HTMLBaseElement);
    expect(base.getAttribute("href")).toBe("/app/");
    expect(base.getAttribute("target")).toBe("_self");
  });

  it("gives <meta> its name, content and http-equiv", () => {
    const { root } = mount(FixedElements);
    const meta = root.querySelector("#meta");
    expect(meta.getAttribute("name")).toBe("description");
    expect(meta.getAttribute("content")).toBe("generated");
    expect(meta.getAttribute("http-equiv")).toBe("content-security-policy");
  });

  it("renders <track> as a void child of <video> with typed attributes", () => {
    const { root } = mount(FixedElements);
    const track = root.querySelector("#trk");
    expect(track).toBeInstanceOf(HTMLTrackElement);
    expect(track.parentElement.id).toBe("vid");
    expect(track.getAttribute("kind")).toBe("subtitles");
    expect(track.hasAttribute("default")).toBe(true);
    expect(track.childNodes.length).toBe(0);
  });

  it("renders <wbr> as a void element", () => {
    const { root } = mount(FixedElements);
    expect(root.querySelector("#wrap").innerHTML).toBe("long<wbr>word");
  });

  it("renders <bdo> with dir and <caption> in its table", () => {
    const { root } = mount(FixedElements);
    expect(root.querySelector("#bdo").getAttribute("dir")).toBe("rtl");
    const cap = root.querySelector("#cap");
    expect(cap).toBeInstanceOf(HTMLTableCaptionElement);
    expect(cap.textContent).toBe("Scores");
  });

  it("creates <math> and every descendant in the MathML namespace", () => {
    const { root } = mount(Formula);
    expect(root.localName).toBe("math");
    expect(root.namespaceURI).toBe(MATHML_NS);
    for (const el of root.querySelectorAll("*")) {
      expect(el.namespaceURI, el.localName).toBe(MATHML_NS);
    }
  });

  it("renders MathML attributes, including the hyphenated annotation-xml element", () => {
    const { root } = mount(Formula);
    expect(root.getAttribute("display")).toBe("block");
    expect(root.querySelector("#frac").getAttribute("linethickness")).toBe("2px");
    const ann = root.querySelector("#ann");
    expect(ann.localName).toBe("annotation-xml");
    expect(ann.getAttribute("encoding")).toBe("MathML-Content");
  });
});
