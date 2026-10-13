import { describe, it, expect, beforeEach } from "vitest";
import { mount, click, flush, settle, waitFor } from "../../helpers/index.js";
import { App, Bare, history, getGreeting } from "./A-Router.fs.jsx";

// Every spec shares one module-level router on a memory history; start each at "/".
beforeEach(() => {
  history.set({ value: "/", replace: true });
  flush();
});

const go = async path => {
  history.set({ value: path });
  await settle();
};

const text = (container, sel) => container.querySelector(sel)?.textContent.trim();

describe("Integration/Router @solidjs/router 2", () => {
  it("renders the route matching the location inside the root layout", () => {
    const { container } = mount(App);
    expect(text(container, "main h1")).toBe("Home");
    expect(text(container, "#path")).toBe("/");
  });

  it("renders without a root layout", async () => {
    const { container } = mount(Bare);
    await go("/about");
    expect(text(container, "h1")).toBe("About");
  });

  it("follows a plain anchor click", async () => {
    const { container } = mount(App);
    click(container.querySelector("#to-about"));
    await settle();
    expect(text(container, "main h1")).toBe("About");
    expect(history.get()).toBe("/about");
  });

  it("marks the anchor of the current location", async () => {
    const { container } = mount(App);
    await go("/about");
    expect(container.querySelector("#to-about").getAttribute("aria-current")).toBe("page");
    expect(container.querySelector("#to-about").hasAttribute("data-active")).toBe(true);
    expect(container.querySelector("#to-home").hasAttribute("aria-current")).toBe(false);
  });

  it("replaces the history entry for an anchor with replace", async () => {
    const { container } = mount(App);
    await go("/about");
    click(container.querySelector("#to-user"));
    await settle();
    expect(history.get()).toBe("/users/42");
    history.back();
    await settle();
    // The /about entry was replaced, so back lands on "/".
    expect(history.get()).toBe("/");
  });

  it("passes route params to the component's props", async () => {
    const { container } = mount(App);
    await go("/users/42");
    expect(text(container, "#user")).toBe("User 42");
  });

  it("nests a child route inside its parent's layout", async () => {
    const { container } = mount(App);
    await go("/users");
    expect(text(container, "#users h2")).toBe("Users");
    expect(text(container, "#users #users-index")).toBe("Pick a user");
    await go("/users/5");
    expect(text(container, "#users #user")).toBe("User 5");
  });

  it("reads params with useParams in a nested route", async () => {
    const { container } = mount(App);
    await go("/users/9/settings");
    expect(text(container, "#settings")).toBe("Settings of 9");
  });

  it("applies match filters: a non-integer id falls through to the wildcard route", async () => {
    const { container } = mount(App);
    await go("/users/abc");
    expect(text(container, "#not-found")).toBe("Not found");
  });

  it("navigates with useNavigate, by path and by history delta", async () => {
    const { container } = mount(App);
    click(container.querySelector("#go"));
    await settle();
    expect(text(container, "#settings")).toBe("Settings of 7");
    expect(text(container, "#path")).toBe("/users/7/settings");
    click(container.querySelector("#back"));
    await settle();
    expect(text(container, "main h1")).toBe("Home");
  });

  it("builds hrefs from the typed path proxy", () => {
    const { container } = mount(App);
    expect(container.querySelector("#to-paths").getAttribute("href")).toBe("/users/7");
  });

  it("reads and merges search params", async () => {
    const { container } = mount(App);
    await go("/search?q=fsharp");
    expect(text(container, "#q")).toBe("fsharp");
    click(container.querySelector("#set-q"));
    await settle();
    expect(text(container, "#q")).toBe("solid");
    expect(history.get()).toBe("/search?q=solid");
  });

  it("reads a query through an async memo, under Loading", async () => {
    const { container } = mount(App);
    await go("/greet/Ada");
    const p = await waitFor(() => container.querySelector("#greet"));
    expect(p.textContent.trim()).toBe("Hello Ada");
  });

  it("exposes a query's key and per-argument key", () => {
    expect(getGreeting.key).toBe("greeting");
    expect(getGreeting.keyFor("Ada")).toBe(`greeting[${JSON.stringify("Ada")}]`);
  });

  it("calls an action through useAction and lists its settled submissions", async () => {
    const { container } = mount(App);
    await go("/actions");
    click(container.querySelector("#add"));
    await waitFor(() => text(container, "#added") === "added x");
  });

  it("posts a form to an action with a bound argument", async () => {
    const { container } = mount(App);
    await go("/actions");
    container.querySelector("#save").requestSubmit();
    await waitFor(() => text(container, "#saved") === "3:Draft");
  });

  it("reports link state with useLinkState", async () => {
    const { container } = mount(App);
    expect(text(container, "#users-active")).toBe("inactive");
    await go("/users/3");
    expect(text(container, "#users-active")).toBe("active");
    expect(text(container, "#users-current")).toBe("not current");
    await go("/users");
    expect(text(container, "#users-current")).toBe("current");
  });

  it("matches a pattern with useMatch and lists the matched routes with useRouteMatches", async () => {
    const { container } = mount(App);
    expect(text(container, "#user-match")).toBe("none");
    expect(text(container, "#depth")).toBe("1");
    await go("/users/8");
    expect(text(container, "#user-match")).toBe("8");
    expect(text(container, "#depth")).toBe("2");
  });

  it("pushes an anchor's state attribute as location.state", async () => {
    const { container } = mount(App);
    click(container.querySelector("#with-state"));
    await settle();
    expect(text(container, "main h1")).toBe("About");
    expect(text(container, "#state")).toBe("nav");
  });

  it("writes preload = \"false\" as the attribute the router reads", () => {
    const { container } = mount(App);
    expect(container.querySelector("#no-preload").getAttribute("preload")).toBe("false");
  });
});
