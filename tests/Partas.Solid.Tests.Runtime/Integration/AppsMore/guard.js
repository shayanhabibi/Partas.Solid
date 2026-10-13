// Local helpers for the AppsMore specs.
//
// An exception thrown inside a DOM event listener does not propagate out of dispatchEvent: jsdom
// reports it on `window` and vitest turns it into an "Uncaught Exception" that fails the whole run,
// even inside an `it.fails`. These wrappers catch such listener errors and rethrow them from the
// test body, so a handler crash fails only the test that triggered it.
import {flush} from "../../helpers/index.js";

export function rethrowing(fn) {
    const errors = [];
    const onError = e => {
        errors.push(e.error ?? new Error(e.message));
        e.preventDefault();
    };
    window.addEventListener("error", onError);
    try {
        fn();
    } finally {
        window.removeEventListener("error", onError);
    }
    if (errors.length) throw errors[0];
}

/** click + flush, surfacing handler exceptions. */
export const click = el => {
    if (!el) throw new Error("click(): element is null");
    rethrowing(() => flush(() => el.click()));
};

/** Set value, dispatch `input` (or `change`), flush; surfaces handler exceptions. */
export const input = (el, value, type = "input") => {
    if (!el) throw new Error("input(): element is null");
    rethrowing(() =>
        flush(() => {
            el.value = value;
            el.dispatchEvent(new Event(type, {bubbles: true}));
        })
    );
};

/** Dispatch a bubbling keydown with `key` on `el`, then flush. */
export const key = (el, k) =>
    rethrowing(() => flush(() => el.dispatchEvent(new KeyboardEvent("keydown", {key: k, bubbles: true}))));
