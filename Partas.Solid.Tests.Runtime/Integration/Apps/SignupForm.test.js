import {describe, it, expect, vi} from "vitest";
import {mount, click, input, flush, text} from "../../helpers/index.js";
import {SignupForm} from "./B-SignupForm.fs.jsx";

const $ = (root, sel) => root.querySelector(sel);
const $$ = (root, sel) => [...root.querySelectorAll(sel)];

/** onBlur is attached directly (blur does not bubble), so dispatch on the element and flush. */
const blur = el => flush(() => el.dispatchEvent(new FocusEvent("blur")));
const submit = root => click($(root, "button.submit"));
const errors = root => $$(root, "p.error").map(text);

function setup(props = {}) {
    const onSave = vi.fn();
    const h = mount(SignupForm, {onSave, ...props});
    const c = h.container;
    return {
        c,
        onSave,
        name: $(c, "input.name"),
        email: $(c, "input.email"),
        code: $(c, "input.code")
    };
}

describe("Apps: controlled signup form with validation", () => {
    it("renders a pristine form: no errors, no summary, counter at zero, noValidate set", () => {
        const {c, name} = setup();
        expect(errors(c)).toEqual([]);
        expect($(c, "p.summary")).toBeNull();
        expect($(c, "p.success")).toBeNull();
        expect(text($(c, ".counter"))).toBe("0 chars");
        expect($(c, "form.signup").noValidate).toBe(true);
        expect(name.value).toBe("");
        expect($(c, 'label[for="su-name"]')).not.toBeNull();
    });

    it("typing updates the live character counter without showing errors", () => {
        const {c, name} = setup();
        input(name, "A");
        expect(text($(c, ".counter"))).toBe("1 chars");
        input(name, "Ada L");
        expect(text($(c, ".counter"))).toBe("5 chars");
        expect(errors(c)).toEqual([]);
    });

    it("shows a name error only after blur, and the message tracks the value", () => {
        const {c, name} = setup();
        blur(name);
        expect(text($(c, "p.name-error"))).toBe("Name is required");
        expect($(c, "p.name-error").getAttribute("role")).toBe("alert");

        input(name, "A");
        expect(text($(c, "p.name-error"))).toBe("Name must be at least 2 characters");

        input(name, "Al");
        expect($(c, "p.name-error")).toBeNull();

        // Whitespace-only counts as empty after trimming.
        input(name, "   ");
        expect(text($(c, "p.name-error"))).toBe("Name is required");
    });

    it("the email field validates independently of the name field", () => {
        const {c, email} = setup();
        blur(email);
        expect(errors(c)).toEqual(["Email is required"]);
        input(email, "ada");
        expect(errors(c)).toEqual(["Email is invalid"]);
        input(email, "ada@example.com");
        expect(errors(c)).toEqual([]);
    });

    it("submitting an empty form reveals every error and a pluralised summary, without saving", () => {
        const {c, onSave} = setup();
        submit(c);
        expect(errors(c)).toEqual(["Name is required", "Email is required"]);
        expect(text($(c, "p.summary"))).toBe("Please fix 2 errors");
        expect(onSave).not.toHaveBeenCalled();
    });

    it("after a failed submit, fixing fields shrinks the summary and then removes it", () => {
        const {c, name, email} = setup();
        submit(c);
        input(name, "Ada");
        expect(text($(c, "p.summary"))).toBe("Please fix 1 error");
        expect(errors(c)).toEqual(["Email is required"]);
        input(email, "ada@x.io");
        expect($(c, "p.summary")).toBeNull();
        expect(errors(c)).toEqual([]);
    });

    it("a valid submit saves trimmed values, greets the user and resets every controlled input", () => {
        const {c, onSave, name, email, code} = setup();
        input(name, "  Ada  ");
        input(email, " ada@x.io ");
        input(code, "ab1");
        submit(c);

        expect(onSave).toHaveBeenCalledTimes(1);
        expect(onSave.mock.calls[0][0]).toMatchObject({name: "Ada", email: "ada@x.io", code: "AB1"});
        expect(text($(c, "p.success"))).toBe("Welcome, Ada!");

        expect(name.value).toBe("");
        expect(email.value).toBe("");
        expect(code.value).toBe("");
        expect(text($(c, ".counter"))).toBe("0 chars");
        // attempted/touched were reset, so the now-empty fields show no errors.
        expect(errors(c)).toEqual([]);
        expect($(c, "p.summary")).toBeNull();
    });

    it("a second valid submit replaces the greeting and calls onSave again", () => {
        const {c, onSave, name, email} = setup();
        input(name, "Ada");
        input(email, "a@x");
        submit(c);
        input(name, "Grace");
        input(email, "g@x");
        submit(c);
        expect(onSave).toHaveBeenCalledTimes(2);
        expect(onSave.mock.calls[1][0]).toMatchObject({name: "Grace", email: "g@x", code: ""});
        expect(text($(c, "p.success"))).toBe("Welcome, Grace!");
    });

    it("transforming controlled input writes the upper-cased signal value back into the DOM", () => {
        const {code} = setup();
        input(code, "abc");
        expect(code.value).toBe("ABC");
        input(code, "ABCd");
        expect(code.value).toBe("ABCD");
    });

    it("a minNameLength prop overrides the merged default", () => {
        const {c, name} = setup({minNameLength: 5});
        input(name, "Ada");
        blur(name);
        expect(text($(c, "p.name-error"))).toBe("Name must be at least 5 characters");
        input(name, "Adaline");
        expect($(c, "p.name-error")).toBeNull();
    });

    it("an invalid submit does not clear what the user typed", () => {
        const {c, name, email} = setup();
        input(name, "Ada");
        input(email, "nope");
        submit(c);
        expect(name.value).toBe("Ada");
        expect(email.value).toBe("nope");
        expect(errors(c)).toEqual(["Email is invalid"]);
    });

    // BUG: ariaInvalid is emitted verbatim as `ariaInvalid=` instead of `aria-invalid=`.
    it.fails("marks invalid fields with aria-invalid", () => {
        const {c, name} = setup();
        expect(name.getAttribute("aria-invalid")).toBe("false");
        submit(c);
        expect(name.getAttribute("aria-invalid")).toBe("true");
        input(name, "Ada");
        expect(name.getAttribute("aria-invalid")).toBe("false");
    });
});
