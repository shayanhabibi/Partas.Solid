import {describe, it, expect, vi} from "vitest";
import {mount, click, text} from "../../helpers/index.js";
import {
    Table, CamelRowSpan, DynamicColspan, Disclosure, DialogOpen, Media, Voids, Misc, BoolsFalseMisc, DialogEvents,
    StringTypedBools
} from "./C-Tags.fs.jsx";

describe("Dom/Forms tables", () => {
    it("builds a full table: caption, colgroup/col span, thead/tbody/tfoot sections", () => {
        const {root} = mount(Table);
        expect(root.caption.textContent.trim()).toBe("Totals");
        expect(root.tHead.rows).toHaveLength(1);
        expect(root.tBodies).toHaveLength(1);
        expect(root.tBodies[0].rows).toHaveLength(2);
        expect(root.tFoot.rows).toHaveLength(1);
        const cols = [...root.querySelectorAll("col")];
        expect(cols.map(c => c.span)).toEqual([2, 1]);
        expect(root.rows).toHaveLength(4);
    });

    it("colspan / rowspan / headers / scope land on the cells", () => {
        const {root} = mount(Table);
        const scores = root.tHead.rows[0].cells[1];
        expect(scores.colSpan).toBe(2);
        expect(scores.getAttribute("scope")).toBe("col");
        const rs = root.querySelector("#rs");
        expect(rs.rowSpan).toBe(2);
        expect(rs.getAttribute("headers")).toBe("h-name");
        // the second body row starts with "3": "Ann" spans both rows
        expect(root.tBodies[0].rows[1].cells[0].textContent.trim()).toBe("3");
    });

    it("the camelCase colSpan binding sets the colspan attribute", () => {
        const {root} = mount(Table);
        const cs = root.querySelector("#cs");
        expect(cs.getAttribute("colspan")).toBe("3");
        expect(cs.colSpan).toBe(3);
    });

    it("the camelCase rowSpan binding sets the rowspan attribute", () => {
        const {root} = mount(CamelRowSpan);
        const th = root.querySelector("#th-rs");
        expect(th.rowSpan).toBe(2);
        expect(th.getAttribute("rowspan")).toBe("2");
    });

    it("colspan follows a signal", () => {
        const {container} = mount(DynamicColspan);
        const td = container.querySelector("#dc-td");
        expect(td.colSpan).toBe(1);
        click(container.querySelector("#dc-grow"));
        click(container.querySelector("#dc-grow"));
        expect(td.getAttribute("colspan")).toBe("3");
    });
});

describe("Dom/Forms details and dialog", () => {
    it("details open true renders open; open false is absent", () => {
        const {container} = mount(Disclosure, {
            log: () => {
            }
        });
        expect(container.querySelector("#d-static").open).toBe(true);
        expect(container.querySelector("#d-static").getAttribute("open")).toBe("");
        expect(container.querySelector("#d-closed").open).toBe(false);
        expect(container.querySelector("#d-closed").hasAttribute("open")).toBe(false);
        expect(container.querySelector("#d-static summary").textContent.trim()).toBe("always");
    });

    it("details open follows a signal", () => {
        const {container} = mount(Disclosure, {
            log: () => {
            }
        });
        const d = container.querySelector("#d-dyn");
        expect(d.open).toBe(false);
        click(container.querySelector("#d-btn"));
        expect(d.open).toBe(true);
        click(container.querySelector("#d-btn"));
        expect(d.hasAttribute("open")).toBe(false);
    });

    it("details onToggle (non-delegated) receives toggle events", () => {
        const log = vi.fn();
        const {container} = mount(Disclosure, {log});
        container.querySelector("#d-dyn").dispatchEvent(new Event("toggle"));
        expect(log).toHaveBeenCalledWith("toggle");
    });

    it("dialog open attribute follows a signal", () => {
        const {container} = mount(DialogOpen);
        const dlg = container.querySelector("#dlg-el");
        expect(dlg.open).toBe(false);
        expect(dlg.hasAttribute("open")).toBe(false);
        click(container.querySelector("#dlg-show"));
        expect(dlg.open).toBe(true);
        expect(dlg.getAttribute("open")).toBe("");
        click(container.querySelector("#dlg-hide"));
        expect(dlg.hasAttribute("open")).toBe(false);
        expect(container.querySelector("#dlg-el form").getAttribute("method")).toBe("dialog");
    });

    it("dialog onClose / onCancel handlers are attached", () => {
        const log = vi.fn();
        const {root} = mount(DialogEvents, {log});
        root.dispatchEvent(new Event("close"));
        root.dispatchEvent(new Event("cancel", {cancelable: true}));
        expect(log.mock.calls.map(c => c[0])).toEqual(["close", "cancel"]);
    });
});

describe("Dom/Forms media and links", () => {
    it("img attributes: sizing, lazy loading, srcset, crossorigin, referrerpolicy", () => {
        const {container} = mount(Media);
        const img = container.querySelector("#m-img");
        expect(img.getAttribute("alt")).toBe("");
        expect(img.width).toBe(64);
        expect(img.height).toBe(32);
        expect(img.getAttribute("loading")).toBe("lazy");
        expect(img.getAttribute("decoding")).toBe("async");
        expect(img.getAttribute("srcset")).toBe("/a.png 1x, /a@2x.png 2x");
        expect(img.getAttribute("sizes")).toBe("64px");
        expect(img.getAttribute("crossorigin")).toBe("anonymous");
        expect(img.getAttribute("referrerpolicy")).toBe("no-referrer");
    });

    it("anchor download / hreflang / type / ping", () => {
        const {container} = mount(Media);
        const a = container.querySelector("#m-a");
        expect(a.download).toBe("report.pdf");
        expect(a.hreflang).toBe("en");
        expect(a.type).toBe("application/pdf");
        expect(a.getAttribute("ping")).toBe("/ping");
    });

    it("iframe src / title / size / sandbox / allow / name", () => {
        const {container} = mount(Media);
        const f = container.querySelector("#m-if");
        expect(f.tagName).toBe("IFRAME");
        expect(f.getAttribute("src")).toBe("about:blank");
        expect(f.title).toBe("frame");
        expect(f.getAttribute("width")).toBe("300");
        expect(f.getAttribute("height")).toBe("150");
        expect(f.getAttribute("sandbox")).toBe("allow-scripts");
        expect(f.getAttribute("allow")).toBe("fullscreen");
        expect(f.name).toBe("fr");
    });

    it("video boolean attributes true are set; audio booleans false are omitted", () => {
        const {container} = mount(Media);
        const v = container.querySelector("#m-vid");
        expect(v.controls).toBe(true);
        expect(v.loop).toBe(true);
        expect(v.hasAttribute("playsinline")).toBe(true);
        expect(v.getAttribute("poster")).toBe("/p.png");
        expect(v.getAttribute("preload")).toBe("none");
        // muted is a stateful property in Solid 2 (dom_with_state VIDEO/muted)
        expect(v.muted).toBe(true);
        const au = container.querySelector("#m-aud");
        expect(au.hasAttribute("controls")).toBe(false);
        expect(au.hasAttribute("autoplay")).toBe(false);
    });

    it("track default' (bound as a string) and iframe allowfullscreen render as present attributes", () => {
        const {container} = mount(StringTypedBools);
        const trk = container.querySelector("#trk");
        expect(trk.hasAttribute("default")).toBe(true);
        expect(trk.kind).toBe("subtitles");
        expect(trk.srclang).toBe("en");
        expect(trk.label).toBe("English");
        expect(container.querySelector("#ifs").hasAttribute("allowfullscreen")).toBe(true);
    });
});

describe("Dom/Forms void and misc elements", () => {
    it("void elements render with no children and keep attributes; text around them survives", () => {
        const {root} = mount(Voids);
        const tags = [...root.children].map(c => c.tagName.toLowerCase());
        expect(tags).toEqual(["br", "hr", "wbr", "img", "input", "embed", "source", "area", "meta", "link"]);
        for (const c of root.children) expect(c.childNodes).toHaveLength(0);
        expect(root.querySelector("hr").className).toBe("rule");
        expect(root.querySelector("area").getAttribute("coords")).toBe("0,0,1,1");
        expect(root.querySelector("area").getAttribute("shape")).toBe("rect");
        expect(root.querySelector("link").getAttribute("rel")).toBe("stylesheet");
        expect(root.querySelector("input").value).toBe("v");
        expect(text(root)).toBe("a b");
    });

    it("ol start/reversed, meter, progress and time attributes", () => {
        const {container} = mount(Misc);
        const ol = container.querySelector("#ol");
        expect(ol.start).toBe(3);
        expect(ol.reversed).toBe(true);
        const met = container.querySelector("#met");
        expect(met.getAttribute("value")).toBe("7");
        expect(met.getAttribute("low")).toBe("2");
        expect(met.getAttribute("high")).toBe("8");
        expect(met.getAttribute("optimum")).toBe("5");
        const prog = container.querySelector("#prog");
        expect(prog.getAttribute("max")).toBe("100");
        expect(prog.getAttribute("value")).toBe("40");
        expect(container.querySelector("#tm").getAttribute("datetime")).toBe("2024-03-15");
    });

    it("boolean attributes set false on details/dialog/select/option/video/form/fieldset/textarea are all absent", () => {
        const {root} = mount(BoolsFalseMisc);
        for (const el of root.querySelectorAll("[id]")) {
            expect(el.getAttributeNames(), el.id).toEqual(["id"]);
        }
        expect(root.innerHTML).not.toContain("false");
        expect(root.querySelector("#bf-select").multiple).toBe(false);
        expect(root.querySelector("#bf-form").noValidate).toBe(false);
    });
});
