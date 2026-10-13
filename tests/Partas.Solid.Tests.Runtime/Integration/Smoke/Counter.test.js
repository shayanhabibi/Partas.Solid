import { describe, it, expect } from "vitest";
import { mount, click, text } from "../../helpers/index.js";
import { Counter } from "./Counter.fs.jsx";

describe("Integration smoke: SolidTypeComponent counter", () => {
  it("renders the initial prop value", () => {
    const { container } = mount(Counter, { initial: 3 });
    expect(text(container.querySelector(".value"))).toBe("3");
  });

  it("updates text after click + flush", () => {
    const { container } = mount(Counter, { initial: 0 });
    const button = container.querySelector("button.inc");
    const value = container.querySelector(".value");
    click(button);
    expect(text(value)).toBe("1");
    click(button);
    click(button);
    expect(text(value)).toBe("3");
  });
});
