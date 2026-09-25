import {describe, it, expect, vi} from "vitest";
import {mount, flush, text, deferred, settle} from "../../helpers/index.js";
import {click, input, rethrowing} from "./guard.js";
import {AccountForm} from "./D-Form.fs.jsx";

const $ = (root, sel) => root.querySelector(sel);
const $$ = (root, sel) => [...root.querySelectorAll(sel)];

const field = (c, name) => $(c, `input[name="${name}"]`);
const errorOf = (c, name) => {
    const e = $(c, `.field-${name} .error`);
    return e ? text(e) : null;
};
const invalidFields = c => $$(c, ".field.invalid").map(f => [...f.classList].find(k => k.startsWith("field-")).slice(6));
const blur = el => rethrowing(() => flush(() => el.dispatchEvent(new FocusEvent("blur", {bubbles: true}))));
const type = (c, name, value) => {
    input(field(c, name), value);
    blur(field(c, name));
};
const submit = c => click($(c, "button.submit"));
const check = (c, on) => {
    const box = field(c, "agree");
    box.checked = on;
    rethrowing(() => flush(() => box.dispatchEvent(new Event("change", {bubbles: true}))));
};

function fillValid(c) {
    type(c, "username", "ada");
    type(c, "email", "ada@example.com");
    type(c, "password", "correcthorse");
    type(c, "confirm", "correcthorse");
    check(c, true);
}

const setup = (onSubmit = vi.fn(() => Promise.resolve(""))) => ({
    c: mount(AccountForm, {onSubmit}).container,
    onSubmit
});

describe("AppsMore: account form with store-based validation", () => {
    it("starts clean: no errors shown, nothing invalid, submit enabled", () => {
        const {c} = setup();
        expect($$(c, ".error")).toEqual([]);
        expect(invalidFields(c)).toEqual([]);
        expect($(c, "button.submit").disabled).toBe(false);
        expect(text($(c, "button.submit"))).toBe("Create account");
    });

    it("shows a field's error only after it is blurred", () => {
        const {c} = setup();
        input(field(c, "email"), "nope");
        expect(errorOf(c, "email")).toBeNull();
        blur(field(c, "email"));
        expect(errorOf(c, "email")).toBe("Enter a valid email");
        expect(invalidFields(c)).toEqual(["email"]);
        // Untouched fields still hide their errors.
        expect(errorOf(c, "username")).toBeNull();
    });

    it("error text follows the value as the user types", () => {
        const {c} = setup();
        type(c, "username", "");
        expect(errorOf(c, "username")).toBe("Username is required");
        input(field(c, "username"), "ada lovelace");
        expect(errorOf(c, "username")).toBe("Username cannot contain spaces");
        input(field(c, "username"), "ada");
        expect(errorOf(c, "username")).toBeNull();
        expect(invalidFields(c)).toEqual([]);
    });

    it("cross-field rule: confirm must match password, re-checked when password changes", () => {
        const {c} = setup();
        type(c, "password", "longenough");
        type(c, "confirm", "longenough");
        expect(errorOf(c, "confirm")).toBeNull();
        input(field(c, "password"), "longenough2");
        expect(errorOf(c, "confirm")).toBe("Passwords do not match");
        expect(errorOf(c, "password")).toBeNull();
    });

    it("password length rule", () => {
        const {c} = setup();
        type(c, "password", "short");
        expect(errorOf(c, "password")).toBe("Password must be at least 8 characters");
    });

    it("submitting an empty form reveals every error and a summary, and does not call onSubmit", () => {
        const {c, onSubmit} = setup();
        submit(c);
        expect(invalidFields(c)).toEqual(["username", "email", "password"]);
        expect(errorOf(c, "agree")).toBe("You must accept the terms");
        expect(text($(c, ".summary"))).toBe("Please fix 4 field(s)");
        expect(onSubmit).not.toHaveBeenCalled();
    });

    it("the summary count shrinks as fields are fixed after a failed submit", () => {
        const {c} = setup();
        submit(c);
        input(field(c, "username"), "ada");
        expect(text($(c, ".summary"))).toBe("Please fix 3 field(s)");
        check(c, true);
        expect(text($(c, ".summary"))).toBe("Please fix 2 field(s)");
        expect(errorOf(c, "agree")).toBeNull();
    });

    it("controlled inputs reflect the store", () => {
        const {c} = setup();
        input(field(c, "username"), "grace");
        expect(field(c, "username").value).toBe("grace");
        check(c, true);
        expect(field(c, "agree").checked).toBe(true);
    });

    it("a valid submit passes a plain copy of the values, shows Saving..., then resets on success", async () => {
        const server = deferred();
        const onSubmit = vi.fn(() => server.promise);
        const {c} = setup(onSubmit);
        fillValid(c);
        submit(c);
        expect(onSubmit).toHaveBeenCalledTimes(1);
        const sent = onSubmit.mock.calls[0][0];
        expect({...sent}).toEqual({
            username: "ada",
            email: "ada@example.com",
            password: "correcthorse",
            confirm: "correcthorse",
            agree: true
        });
        expect($(c, "button.submit").disabled).toBe(true);
        expect(text($(c, "button.submit"))).toBe("Saving...");

        server.resolve("");
        await settle();
        expect(text($(c, ".saved"))).toBe("Account ada created");
        expect($(c, "button.submit").disabled).toBe(false);
        expect(field(c, "username").value).toBe("");
        expect(field(c, "agree").checked).toBe(false);
        // Reset also cleared touched/submitted, so the now-empty form shows no errors.
        expect($$(c, ".error")).toEqual([]);
        expect(sent.username).toBe("ada");
    });

    it("a server-side error is shown and the values are kept", async () => {
        const {c} = setup(vi.fn(() => Promise.resolve("Username already taken")));
        fillValid(c);
        submit(c);
        await settle();
        expect(text($(c, ".server-error"))).toBe("Username already taken");
        expect(field(c, "username").value).toBe("ada");
        expect($(c, ".saved")).toBeNull();
        expect($(c, "button.submit").disabled).toBe(false);
    });

    it("the server error clears on the next submit attempt", async () => {
        const responses = ["Username already taken", ""];
        const {c} = setup(vi.fn(() => Promise.resolve(responses.shift())));
        fillValid(c);
        submit(c);
        await settle();
        expect($(c, ".server-error")).not.toBeNull();
        submit(c);
        expect($(c, ".server-error")).toBeNull();
        await settle();
        expect(text($(c, ".saved"))).toBe("Account ada created");
    });

    it("Reset clears values, touched state and shown errors", () => {
        const {c} = setup();
        type(c, "email", "bad");
        submit(c);
        expect($$(c, ".error").length).toBeGreaterThan(0);
        click($(c, "button.reset"));
        expect($$(c, ".error")).toEqual([]);
        expect($(c, ".summary")).toBeNull();
        expect(field(c, "email").value).toBe("");
    });
});
