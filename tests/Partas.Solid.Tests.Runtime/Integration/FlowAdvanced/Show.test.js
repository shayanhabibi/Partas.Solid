import {describe, it, expect, vi} from "vitest";
import {createSignal} from "solid-js";
import {mount, click, act, text} from "../../helpers/index.js";
import {
    NestedGate,
    BareShow,
    Toggler,
    KeyedNumber,
    LiveNumber,
    KeyedPerson,
    NestedKeyed,
    ShowKeyedFlag
} from "./A-Show.fs.jsx";

describe("FlowAdvanced: nested Show", () => {
    it("resolves both levels and their fallbacks independently", () => {
        const [signedIn, setSignedIn] = createSignal(false);
        const [isAdmin, setIsAdmin] = createSignal(false);
        const {root} = mount(NestedGate, {
            get signedIn() {
                return signedIn();
            },
            get isAdmin() {
                return isAdmin();
            }
        });
        expect(root.innerHTML).toBe('<span class="login">please log in</span>');

        act(() => setSignedIn(true));
        expect(root.innerHTML).toBe('<span class="welcome">welcome</span><span class="user-panel">user</span>');

        act(() => setIsAdmin(true));
        expect(root.innerHTML).toBe('<span class="welcome">welcome</span><span class="admin-panel">admin</span>');

        // outer goes false: the inner branch disappears entirely
        act(() => setSignedIn(false));
        expect(root.innerHTML).toBe('<span class="login">please log in</span>');
    });

    it("keeps the outer child nodes when only the inner condition flips", () => {
        const [isAdmin, setIsAdmin] = createSignal(false);
        const {root} = mount(NestedGate, {
            signedIn: true,
            get isAdmin() {
                return isAdmin();
            }
        });
        const welcome = root.querySelector(".welcome");
        expect(welcome).not.toBeNull();
        act(() => setIsAdmin(true));
        expect(root.querySelector(".admin-panel")).not.toBeNull();
        expect(root.querySelector(".welcome")).toBe(welcome);
        act(() => setIsAdmin(false));
        expect(root.querySelector(".welcome")).toBe(welcome);
        expect(root.querySelector(".user-panel")).not.toBeNull();
    });

    it("does not recreate the child for truthy -> truthy changes of a non-keyed Show", () => {
        const [flag, setFlag] = createSignal(1);
        const {root} = mount(BareShow, {
            get visible() {
                return flag();
            }
        });
        const content = root.querySelector(".content");
        expect(text(content)).toBe("here");
        act(() => setFlag(2));
        expect(root.querySelector(".content")).toBe(content);
        expect(content.isConnected).toBe(true);
    });
});

describe("FlowAdvanced: Show without fallback", () => {
    it("renders nothing while falsy", () => {
        const [visible, setVisible] = createSignal(false);
        const {root} = mount(BareShow, {
            get visible() {
                return visible();
            }
        });
        expect(root.innerHTML).toBe("");
        act(() => setVisible(true));
        expect(root.innerHTML).toBe('<span class="content">here</span>');
        act(() => setVisible(false));
        expect(root.innerHTML).toBe("");
    });
});

describe("FlowAdvanced: Show from local state", () => {
    it("tracks a condition built from two local signals", () => {
        const {container} = mount(Toggler);
        const shown = () => container.querySelector(".shown");
        expect(text(container.querySelector(".hidden"))).toBe("hidden");

        click(container.querySelector(".toggle"));
        expect(shown()).toBeNull();
        click(container.querySelector(".inc"));
        click(container.querySelector(".inc"));
        expect(text(shown())).toBe("count 2");

        const el = shown();
        click(container.querySelector(".inc"));
        expect(shown()).toBe(el);
        expect(text(el)).toBe("count 3");

        click(container.querySelector(".toggle"));
        expect(shown()).toBeNull();
        expect(container.querySelector(".hidden")).not.toBeNull();
    });
});

describe("FlowAdvanced: Show.Keyed", () => {
    it("treats 0 as falsy and remounts the child for each new number", () => {
        const [value, setValue] = createSignal(0);
        const onMount = vi.fn();
        const {root} = mount(KeyedNumber, {
            get value() {
                return value();
            },
            onMount
        });
        expect(root.innerHTML).toBe('<span class="zero">zero</span>');
        expect(onMount).not.toHaveBeenCalled();

        act(() => setValue(5));
        const first = root.querySelector(".num");
        expect(text(first)).toBe("n=5");

        act(() => setValue(6));
        expect(text(root.querySelector(".num"))).toBe("n=6");
        expect(root.querySelector(".num")).not.toBe(first);
        expect(onMount.mock.calls).toEqual([[5], [6]]);

        // same value: no remount
        act(() => setValue(6));
        expect(onMount).toHaveBeenCalledTimes(2);

        act(() => setValue(0));
        expect(root.innerHTML).toBe('<span class="zero">zero</span>');
    });

    it("passes the narrowed record to a pure-expression child", () => {
        const [person, setPerson] = createSignal(undefined);
        const {root} = mount(KeyedPerson, {
            get person() {
                return person();
            }
        });
        expect(root.innerHTML).toBe("");
        act(() => setPerson({id: 1, name: "Ada"}));
        const ada = root.querySelector(".name");
        expect(text(ada)).toBe("Ada");
        act(() => setPerson({id: 2, name: "Grace"}));
        expect(text(root.querySelector(".name"))).toBe("Grace");
        expect(root.querySelector(".name")).not.toBe(ada);
        act(() => setPerson(undefined));
        expect(root.innerHTML).toBe("");
    });

    it("nests keyed Shows, each with its own fallback", () => {
        const [person, setPerson] = createSignal(undefined);
        const [title, setTitle] = createSignal("");
        const {root} = mount(NestedKeyed, {
            get person() {
                return person();
            },
            get title() {
                return title();
            }
        });
        expect(text(root)).toBe("nobody");

        act(() => setPerson({id: 1, name: "Ada"}));
        expect(root.innerHTML).toBe('<span class="plain">Ada</span>');

        act(() => setTitle("Countess"));
        expect(root.innerHTML).toBe('<span class="titled">Countess Ada</span>');

        act(() => setPerson({id: 2, name: "Grace"}));
        expect(root.innerHTML).toBe('<span class="titled">Countess Grace</span>');

        act(() => setPerson(undefined));
        expect(text(root)).toBe("nobody");
    });

    it("base Show with keyed=true and a static child re-renders from props", () => {
        const [value, setValue] = createSignal("");
        const {root} = mount(ShowKeyedFlag, {
            get value() {
                return value();
            }
        });
        expect(root.innerHTML).toBe("<em>empty</em>");
        act(() => setValue("x"));
        expect(root.innerHTML).toBe('<span class="value">x</span>');
        act(() => setValue("y"));
        expect(text(root)).toBe("y");
    });
});

describe("FlowAdvanced: Show.NonKeyed", () => {
    it("keeps one child for all truthy numbers and reads the latest through the accessor", () => {
        const [value, setValue] = createSignal(3);
        const {root} = mount(LiveNumber, {
            get value() {
                return value();
            }
        });
        const num = root.querySelector(".num");
        expect(text(num)).toBe("n=3");

        act(() => setValue(4));
        expect(root.querySelector(".num")).toBe(num);
        expect(text(num)).toBe("n=4");

        act(() => setValue(0));
        expect(root.innerHTML).toBe('<span class="zero">zero</span>');

        act(() => setValue(9));
        expect(root.querySelector(".num")).not.toBe(num);
        expect(text(root.querySelector(".num"))).toBe("n=9");
    });
});
