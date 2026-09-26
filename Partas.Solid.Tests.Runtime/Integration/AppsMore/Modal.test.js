import {describe, it, expect, vi} from "vitest";
import {createSignal} from "solid-js";
import {createComponent} from "@solidjs/web";
import {mount, flush, act, text} from "../../helpers/index.js";
import {click, key} from "./guard.js";
import {Modal, ConfirmDeleteApp} from "./A-Modal.fs.jsx";

const $ = (root, sel) => root.querySelector(sel);
const $$ = (root, sel) => [...root.querySelectorAll(sel)];
const dialog = () => document.body.querySelector('[role="dialog"]');
const escape = (target = document.body) => key(target, "Escape");

/** Mount <Modal> driven by a signal owned by the test, with an opener button in the tree. */
function setupModal(initialOpen = false) {
    const [isOpen, setOpen] = createSignal(initialOpen);
    const onClose = vi.fn(() => setOpen(false));
    const {container} = mount(
        () => {
            const opener = document.createElement("button");
            opener.className = "opener";
            opener.onclick = () => setOpen(true);
            return [
                opener,
                createComponent(Modal, {
                    get isOpen() {
                        return isOpen();
                    },
                    heading: "Settings",
                    onClose,
                    get children() {
                        const p = document.createElement("p");
                        p.className = "modal-content";
                        p.textContent = "Body text";
                        return p;
                    }
                })
            ];
        },
        undefined,
        {thunk: true}
    );
    return {c: container, isOpen, setOpen, onClose, opener: $(container, ".opener")};
}

describe("AppsMore: modal dialog (Show + Portal)", () => {
    it("renders nothing while closed", () => {
        const {c, opener} = setupModal(false);
        expect(opener).not.toBeNull();
        expect(dialog()).toBeNull();
        expect(document.body.querySelector(".backdrop")).toBeNull();
    });

    it("opening portals the dialog into document.body, outside the component's container", () => {
        const {c, setOpen} = setupModal(false);
        act(() => setOpen(true));
        const d = dialog();
        expect(d).not.toBeNull();
        expect(c.contains(d)).toBe(false);
        expect(document.body.contains(d)).toBe(true);
        expect(text($(d, ".modal-title"))).toBe("Settings");
        expect(text($(d, ".modal-body"))).toBe("Body text");
        expect(d.getAttribute("tabindex")).toBe("-1");
    });

    it("the close button calls onClose and the dialog unmounts", () => {
        const {setOpen, onClose} = setupModal(false);
        act(() => setOpen(true));
        click($(dialog(), ".modal-close"));
        expect(onClose).toHaveBeenCalledTimes(1);
        expect(dialog()).toBeNull();
        expect(document.body.querySelector(".backdrop")).toBeNull();
    });

    it("Escape anywhere in the document closes the open dialog", () => {
        const {setOpen, onClose} = setupModal(false);
        act(() => setOpen(true));
        escape(document.body);
        expect(onClose).toHaveBeenCalledTimes(1);
        expect(dialog()).toBeNull();
    });

    it("other keys do not close the dialog", () => {
        const {setOpen, onClose} = setupModal(false);
        act(() => setOpen(true));
        key(document.body, "Enter");
        expect(onClose).not.toHaveBeenCalled();
        expect(dialog()).not.toBeNull();
    });

    it("the document Escape listener is removed when the dialog closes", () => {
        const {setOpen, onClose} = setupModal(false);
        act(() => setOpen(true));
        act(() => setOpen(false));
        escape(document.body);
        escape(document.body);
        expect(onClose).not.toHaveBeenCalled();
    });

    it("clicking the backdrop closes, clicking inside the dialog does not", () => {
        const {setOpen, onClose} = setupModal(false);
        act(() => setOpen(true));
        click($(dialog(), ".modal-body"));
        click(dialog());
        expect(onClose).not.toHaveBeenCalled();
        click(document.body.querySelector(".backdrop"));
        expect(onClose).toHaveBeenCalledTimes(1);
        expect(dialog()).toBeNull();
    });

    it("focuses the dialog when opened and restores focus to the opener on close", () => {
        const {opener} = setupModal(false);
        opener.focus();
        expect(document.activeElement).toBe(opener);
        act(() => opener.click());
        expect(document.activeElement).toBe(dialog());
        escape(document.body);
        expect(dialog()).toBeNull();
        expect(document.activeElement).toBe(opener);
    });

    it("re-opening mounts a fresh dialog", () => {
        const {setOpen} = setupModal(false);
        act(() => setOpen(true));
        const first = dialog();
        act(() => setOpen(false));
        act(() => setOpen(true));
        expect(dialog()).not.toBeNull();
        expect(dialog()).not.toBe(first);
        expect(document.body.querySelectorAll('[role="dialog"]')).toHaveLength(1);
    });

    // BUG: ariaModal / ariaLabelledBy are emitted verbatim instead of aria-modal / aria-labelledby.
    it.fails("marks the dialog aria-modal and labels it by its heading", () => {
        const {setOpen} = setupModal(false);
        act(() => setOpen(true));
        expect(dialog().getAttribute("aria-modal")).toBe("true");
        expect(dialog().getAttribute("aria-labelledby")).toBe("modal-title");
    });
});

const setupApp = (items = ["alpha", "beta", "gamma"]) => {
    const onDeleted = vi.fn();
    const {container, dispose} = mount(ConfirmDeleteApp, {items, onDeleted});
    return {c: container, onDeleted, dispose};
};
const names = c => $$(c, "li.item .name").map(text);
const deleteBtn = (c, name) => $(c, `li.item[data-name="${name}"] button.delete`);

describe("AppsMore: confirm-delete app using the modal", () => {
    it("lists the items with no dialog open", () => {
        const {c} = setupApp();
        expect(names(c)).toEqual(["alpha", "beta", "gamma"]);
        expect(dialog()).toBeNull();
        expect(text($(c, ".closes"))).toBe("closed 0");
    });

    it("Delete opens the confirmation for that item", () => {
        const {c} = setupApp();
        click(deleteBtn(c, "beta"));
        expect(dialog()).not.toBeNull();
        expect(text($(dialog(), ".modal-title"))).toBe("Confirm delete");
        expect(text($(dialog(), ".question"))).toBe("Delete beta?");
    });

    it("confirming removes the item, reports it and closes the dialog", () => {
        const {c, onDeleted} = setupApp();
        click(deleteBtn(c, "beta"));
        click($(dialog(), "button.confirm"));
        expect(names(c)).toEqual(["alpha", "gamma"]);
        expect(onDeleted.mock.calls).toEqual([["beta"]]);
        expect(dialog()).toBeNull();
    });

    it("unmounting the app while the dialog is open removes the portal and its document listener", () => {
        const add = vi.spyOn(document, "addEventListener");
        const remove = vi.spyOn(document, "removeEventListener");
        try {
            const {c, dispose} = setupApp();
            click(deleteBtn(c, "alpha"));
            expect(dialog()).not.toBeNull();
            const added = add.mock.calls.filter(([type]) => type === "keydown").map(([, h]) => h);
            expect(added).toHaveLength(1);
            dispose();
            expect(dialog()).toBeNull();
            expect(document.body.querySelector(".backdrop")).toBeNull();
            expect(remove.mock.calls.filter(([type]) => type === "keydown").map(([, h]) => h)).toEqual(added);
        } finally {
            add.mockRestore();
            remove.mockRestore();
        }
    });

    // BUG: `onClose = close` (local unit -> unit fn) is dropped from the emitted <Modal> props; expected onClose={() => {...}}.
    it.fails("the dialog's Close button cancels without deleting and counts the close", () => {
        const {c, onDeleted} = setupApp();
        click(deleteBtn(c, "gamma"));
        click($(dialog(), ".modal-close"));
        expect(dialog()).toBeNull();
        expect(names(c)).toEqual(["alpha", "beta", "gamma"]);
        expect(onDeleted).not.toHaveBeenCalled();
        expect(text($(c, ".closes"))).toBe("closed 1");
    });

    // BUG: `onClose = close` (local unit -> unit fn) is dropped from the emitted <Modal> props; expected onClose={() => {...}}.
    it.fails("Escape cancels the confirmation", () => {
        const {c} = setupApp();
        click(deleteBtn(c, "alpha"));
        escape(document.body);
        expect(dialog()).toBeNull();
        expect(names(c)).toEqual(["alpha", "beta", "gamma"]);
        expect(text($(c, ".closes"))).toBe("closed 1");
    });
});
