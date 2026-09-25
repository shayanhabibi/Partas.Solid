import {describe, it, expect, vi, beforeAll, afterAll} from "vitest";
import {createSignal} from "solid-js";
import {mount, act, text} from "../../helpers/index.js";
import {click, input} from "./guard.js";
import {
    ThemeContext,
    ThemeProvider,
    ThemedButton,
    ThemeToggle,
    ThemePicker,
    ThemedPage,
    LonelyButton
} from "./F-Theme.fs.jsx";

const $ = (root, sel) => root.querySelector(sel);
const $$ = (root, sel) => [...root.querySelectorAll(sel)];

describe("AppsMore: theme context - consumers without a provider", () => {
    it("consumers fall back to the createContext default", () => {
        const {container: c} = mount(LonelyButton);
        expect($(c, ".themed-btn").className).toBe("themed-btn btn-light");
        expect(text($(c, ".themed-btn"))).toBe("Theme is light");
    });

    it("the default api's toggle is a harmless no-op", () => {
        const {container: c} = mount(LonelyButton);
        click($(c, ".toggle-theme"));
        expect(text($(c, ".themed-btn"))).toBe("Theme is light");
    });

    it("F# consumers read a context value supplied by the Solid 2 provider component (JSX)", () => {
        const [theme, setTheme] = createSignal("dark");
        const api = {theme, setTheme, toggle: () => setTheme(theme() === "dark" ? "light" : "dark")};
        const {container: c} = mount(
            () => (
                <ThemeContext value={api}>
                    <ThemeToggle/>
                    <ThemePicker/>
                    <ThemedButton/>
                </ThemeContext>
            ),
            undefined,
            {thunk: true}
        );
        expect(text($(c, ".themed-btn"))).toBe("Theme is dark");
        expect($(c, ".theme-picker").value).toBe("dark");
        click($(c, ".toggle-theme"));
        expect(text($(c, ".themed-btn"))).toBe("Theme is light");
        input($(c, ".theme-picker"), "sepia", "change");
        expect(theme()).toBe("sepia");
        expect($(c, ".themed-btn").className).toBe("themed-btn btn-sepia");
    });
});

// The F# `ThemeContext api { ... }` provider syntax is emitted as <ThemeContext.Provider value=...>.
// Solid 2 contexts are the provider component themselves and have no `.Provider`, so everything that
// renders ThemeProvider breaks. The same scenarios run again below with a `.Provider` shim.
const providerScenarios = (it) => {
    it("ThemeProvider mirrors the theme to data-theme and provides it to consumers", () => {
        const {container: c} = mount(ThemeProvider, {
            initial: "dark",
            // A getter, so the consumer is created under the provider rather than eagerly here.
            get children() {
                return <ThemedButton/>;
            }
        });
        expect($(c, ".theme-root").dataset.theme).toBe("dark");
        expect(text($(c, ".themed-btn"))).toBe("Theme is dark");
    });

    it("the toolbar toggle flips every consumer under the provider and reports changes", () => {
        const onThemeChange = vi.fn();
        const {container: c} = mount(ThemedPage, {onThemeChange});
        const main = $(c, ".content .themed-btn");
        expect(text(main)).toBe("Theme is light");
        click($(c, ".toggle-theme"));
        expect(text(main)).toBe("Theme is dark");
        expect($(c, ".theme-root").dataset.theme).toBe("dark");
        expect($(c, ".theme-picker").value).toBe("dark");
        click($(c, ".toggle-theme"));
        expect(onThemeChange.mock.calls).toEqual([["dark"], ["light"]]);
    });

    it("the picker sets any theme; re-picking the current theme is not a change", () => {
        const onThemeChange = vi.fn();
        const {container: c} = mount(ThemedPage, {onThemeChange});
        input($(c, ".theme-picker"), "sepia", "change");
        expect($(c, ".content .themed-btn").className).toBe("themed-btn btn-sepia");
        input($(c, ".theme-picker"), "sepia", "change");
        expect(onThemeChange.mock.calls).toEqual([["sepia"]]);
    });

    it("a nested provider overrides the theme for its subtree only", () => {
        const {container: c} = mount(ThemedPage, {
            onThemeChange: () => {
            }
        });
        const forced = $(c, ".forced .themed-btn");
        expect(text(forced)).toBe("Theme is sepia");
        click($(c, ".toggle-theme"));
        expect(text($(c, ".content .themed-btn"))).toBe("Theme is dark");
        expect(text(forced)).toBe("Theme is sepia");
        expect($$(c, ".theme-root").map(r => r.dataset.theme)).toEqual(["dark", "sepia"]);
    });
};

describe("AppsMore: theme context - F# provider (idiomatic `Ctx value { ... }`)", () => {
    // BUG: provider emitted as <ThemeContext.Provider value=...>; Solid 2 contexts are themselves the provider.
    providerScenarios((name, fn) => it.fails(name, fn));
});

describe("AppsMore: theme context - F# provider with a runtime .Provider shim (control group)", () => {
    let hadProvider;
    beforeAll(() => {
        hadProvider = Object.prototype.hasOwnProperty.call(ThemeContext, "Provider");
        if (!hadProvider) ThemeContext.Provider = ThemeContext;
    });
    afterAll(() => {
        if (!hadProvider) delete ThemeContext.Provider;
    });
    providerScenarios(it);
});
