import { describe, it, expect } from "vitest";
import { createSignal } from "solid-js";
import { mount, click, act, text } from "../../helpers/index.js";
import {
  ThemeContext,
  CounterContext,
  ThemedLabel,
  ThemeApp,
  CounterDisplay,
  CounterButton,
  CounterApp
} from "./B-Context.fs.jsx";

const themes = container => [...container.querySelectorAll(".theme")].map(text);

describe("Components: context", () => {
  it("falls back to the createContext default outside any provider", () => {
    const { root } = mount(ThemedLabel);
    expect(root.outerHTML).toBe('<span class="theme">light</span>');
  });

  it("the F#-created context works when rendered as the Solid 2 provider component directly", () => {
    const { container } = mount(
      () => (
        <div>
          <ThemedLabel />
          <ThemeContext value="dark">
            <ThemedLabel />
            <ThemeContext value="nested">
              <ThemedLabel />
            </ThemeContext>
          </ThemeContext>
        </div>
      ),
      undefined,
      { thunk: true }
    );
    expect(themes(container)).toEqual(["light", "dark", "nested"]);
  });

  it("F# consumers share a reactive store across component boundaries (direct provider)", () => {
    const [count, setCount] = createSignal(0);
    const store = { count, increment: () => setCount(count() + 1) };
    const { container } = mount(
      () => (
        <CounterContext value={store}>
          <section>
            <CounterDisplay />
          </section>
          <footer>
            <CounterButton />
          </footer>
        </CounterContext>
      ),
      undefined,
      { thunk: true }
    );
    const display = container.querySelector(".display");
    expect(text(display)).toBe("0");
    click(container.querySelector(".inc"));
    click(container.querySelector(".inc"));
    expect(text(display)).toBe("2");
    act(() => setCount(10));
    expect(text(container.querySelector(".display"))).toBe("10");
  });
  it("throws ContextNotFoundError for a default-less context without a provider", () => {
    let caught;
    try {
      mount(CounterDisplay);
    } catch (e) {
      caught = e;
    }
    // render wraps the ContextNotFoundError (dev message) in a StatusError whose cause is the original
    expect(caught).toBeInstanceOf(Error);
    const original = caught.cause ?? caught;
    expect(original.message).toBe(
      "Context must either be created with a default value or a value must be provided before accessing it."
    );
  });

  // BUG: the plugin emits `<Ctx.Provider value=...>`, but Solid 2 contexts are the provider component
  // themselves (solid/packages/solid/src/client/core.ts createContext returns `provider` with no
  // `.Provider` member), so the idiomatic `Ctx value { children }` renders an undefined component.
  it.fails("provides nested context values through the idiomatic `Ctx value { ... }` syntax", () => {
    const { container } = mount(ThemeApp);
    expect(themes(container)).toEqual(["light", "dark", "nested"]);
  });

  // BUG: same `.Provider` emission as above; a store passed through context never reaches consumers.
  it.fails("shares a reactive store across component boundaries via the idiomatic provider", () => {
    const { container } = mount(CounterApp);
    expect(text(container.querySelector(".display"))).toBe("0");
    click(container.querySelector(".inc"));
    click(container.querySelector(".inc"));
    expect(text(container.querySelector(".display"))).toBe("2");
  });
});
