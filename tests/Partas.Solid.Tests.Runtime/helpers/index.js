// Runtime test helpers for Partas.Solid compiled output.
//
// Scheduling model (solid-js 2.0.0-rc.9, packages/signals/src/core/scheduler.ts):
//   * A signal write marks dependents and calls schedule(), which queues ONE microtask that runs
//     flush(). Nothing downstream (memos read by effects, render effects, DOM) is updated
//     synchronously by the write itself.
//   * flush() drains the queue synchronously; flush(fn) runs fn and then drains.
//   * render() from @solidjs/web flushes once before returning, so the initial DOM is present
//     as soon as mount() returns.
//   * Async memos/Loading resolve on promise settlement (a later microtask/macrotask), after which
//     the scheduler queues another flush. Use settle()/waitFor() for those.
import { afterEach } from "vitest";
import { flush, createRoot } from "solid-js";
import { render, createComponent } from "@solidjs/web";

export { flush };

const mounted = new Set();

/**
 * Render `component` (a compiled Partas component function) with `props` into a fresh <div>
 * appended to document.body. Returns { container, dispose, root }.
 * `component` may also be a thunk returning JSX (pass props = undefined and set `thunk: true`).
 * Everything mounted is disposed and removed automatically after each test.
 */
export function mount(component, props = {}, { thunk = false } = {}) {
  const container = document.createElement("div");
  container.setAttribute("data-testroot", "");
  document.body.appendChild(container);
  const code = thunk ? component : () => createComponent(component, props);
  let disposeRender = render(code, container);
  const handle = {
    container,
    /** First element rendered by the component. */
    get root() {
      return container.firstElementChild;
    },
    dispose() {
      if (!disposeRender) return;
      const d = disposeRender;
      disposeRender = undefined;
      mounted.delete(handle);
      d();
      container.remove();
    }
  };
  mounted.add(handle);
  return handle;
}

/**
 * Run `fn` inside a reactive root (for primitives-level tests that need an owner) and return
 * { value, dispose }. The root is disposed automatically after each test.
 */
export function withRoot(fn) {
  let dispose;
  const value = createRoot(d => {
    dispose = d;
    return fn(d);
  });
  const handle = {
    value,
    dispose() {
      if (!dispose) return;
      const d = dispose;
      dispose = undefined;
      mounted.delete(handle);
      d();
    }
  };
  mounted.add(handle);
  return handle;
}

/** Perform `fn` (e.g. a setter call or DOM event) and synchronously drain Solid's queue. */
export function act(fn) {
  return flush(fn);
}

/** Dispatch a bubbling click (delegated events are attached to the document) and flush. */
export function click(el) {
  if (!el) throw new Error("click(): element is null");
  flush(() => el.click());
}

/** Dispatch an input event after setting `value`, then flush. */
export function input(el, value) {
  if (!el) throw new Error("input(): element is null");
  flush(() => {
    el.value = value;
    el.dispatchEvent(new Event("input", { bubbles: true }));
  });
}

const tick = () => new Promise(r => setTimeout(r, 0));

/**
 * Let pending promises settle and drain Solid's queue. `rounds` macrotask turns, each followed
 * by a flush. Use after resolving an async source that feeds an async memo / <Loading>.
 */
export async function settle(rounds = 3) {
  for (let i = 0; i < rounds; i++) {
    await tick();
    flush();
  }
}

/**
 * Poll `predicate` (flushing between polls) until it returns truthy or `timeout` ms elapse.
 * Returns the truthy value. Throws with `message` on timeout.
 */
export async function waitFor(predicate, { timeout = 1000, message } = {}) {
  const start = Date.now();
  let last;
  for (;;) {
    flush();
    try {
      last = predicate();
      if (last) return last;
    } catch (e) {
      last = e;
    }
    if (Date.now() - start > timeout)
      throw new Error(message ?? `waitFor timed out after ${timeout}ms (last: ${String(last)})`);
    await tick();
  }
}

/** A promise with its resolve/reject exposed, for driving async memos deterministically. */
export function deferred() {
  let resolve, reject;
  const promise = new Promise((res, rej) => {
    resolve = res;
    reject = rej;
  });
  return { promise, resolve, reject };
}

/** Collapse whitespace so assertions don't depend on Fable's JSX line breaks. */
export function text(el) {
  return (el?.textContent ?? "").replace(/\s+/g, " ").trim();
}

export function cleanupAll() {
  for (const h of [...mounted]) h.dispose();
  mounted.clear();
  document.body.innerHTML = "";
}

afterEach(() => {
  cleanupAll();
});
