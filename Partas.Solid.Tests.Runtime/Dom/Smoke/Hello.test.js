import { describe, it, expect } from "vitest";
import { mount } from "../../helpers/index.js";
import { Hello } from "./Hello.fs.jsx";

describe("Dom smoke: SolidComponent with a class and text child", () => {
  it("renders <div class=\"x\">hello</div>", () => {
    const { container } = mount(Hello);
    expect(container.innerHTML).toBe('<div class="x">hello</div>');
    const el = container.querySelector("div.x");
    expect(el).not.toBeNull();
    expect(el.textContent).toBe("hello");
  });
});
