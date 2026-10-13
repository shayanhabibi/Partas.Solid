import {describe, it, expect} from "vitest";
import {flush} from "../../helpers/index.js";
import {createRoot, createStore, createEffect} from "solid-js";
import {
    makeAppStore,
    makeShallowStore,
    makeNamedStore,
    wrapExistingStore,
    makeDeepStore,
    makeShallowReadStore
} from "./A-Stores.fs.jsx";

// Every harness here is created inside createRoot by the fixture; `dispose` is called in finally.
function withApp(fn) {
    const h = makeAppStore();
    try {
        flush();
        return fn(h);
    } finally {
        h.dispose();
    }
}

describe("createStore: object, nested and array state (F# record drafts)", () => {
    it("initial read and initial effect run expose every tracked path", () =>
        withApp(h => {
            expect(h.state.count).toBe(0);
            expect(h.state.user.name).toBe("Ada");
            expect(h.state.user.address.city).toBe("London");
            expect(h.state.todos.length).toBe(2);
            expect(h.state.todos[1].title).toBe("run tests");
            expect(h.log).toEqual(["count:0", "name:Ada", "city:London", "len:2", "done:1"]);
        }));

    it("writes batch: reads keep the committed value until flush", () =>
        withApp(h => {
            h.increment();
            expect(h.state.count).toBe(0);
            flush();
            expect(h.state.count).toBe(1);
        }));

    it("updater draft mutation only reruns the effect that tracks the written key", () =>
        withApp(h => {
            h.log.length = 0;
            h.increment();
            flush();
            expect(h.log).toEqual(["count:1"]);
        }));

    it("the draft sees its own writes within one updater, and the effect runs once", () =>
        withApp(h => {
            h.log.length = 0;
            h.incrementThrice();
            flush();
            expect(h.state.count).toBe(3);
            expect(h.log).toEqual(["count:3"]);
        }));

    it("several setter calls before a flush coalesce into one effect run with the final value", () =>
        withApp(h => {
            h.log.length = 0;
            h.increment();
            h.increment();
            h.rename("Grace");
            flush();
            expect(h.state.count).toBe(2);
            expect(h.log).toEqual(["count:2", "name:Grace"]);
        }));

    it("nested writes are fine-grained: city write does not rerun the name effect", () =>
        withApp(h => {
            h.log.length = 0;
            h.moveCity("Paris");
            flush();
            expect(h.log).toEqual(["city:Paris"]);
            expect(h.state.user.address.city).toBe("Paris");
            expect(h.state.user.name).toBe("Ada");
        }));

    it("replacing a nested object reruns every effect below it", () =>
        withApp(h => {
            h.log.length = 0;
            h.replaceUser("Linus", "Helsinki");
            flush();
            expect(h.log).toEqual(["name:Linus", "city:Helsinki"]);
            expect(h.state.user.address.zip).toBe("?");
        }));

    it("an equal-value write does not rerun the effect", () =>
        withApp(h => {
            h.log.length = 0;
            h.rename("Ada");
            flush();
            expect(h.log).toEqual([]);
        }));

    it("ResizeArray.Add on the draft pushes into the store array", () =>
        withApp(h => {
            h.log.length = 0;
            h.addTodo("ship it");
            flush();
            expect(h.state.todos.length).toBe(3);
            expect(h.state.todos[2].title).toBe("ship it");
            expect(h.state.todos[2].id).toBe(3);
            // Solid 2 effects have no equality gate (_equals: false): the done-count compute iterates
            // the array, so it re-runs on the push and its effect fires again with the same value.
            expect(h.log).toEqual(["len:3", "done:1"]);
        }));

    it("mutating an array item found with a for-loop over the draft updates just that item", () =>
        withApp(h => {
            h.log.length = 0;
            h.toggle(1);
            flush();
            expect(h.state.todos[0].completed).toBe(true);
            expect(h.state.todos[1].completed).toBe(true);
            expect(h.log).toEqual(["done:2"]);
            h.retitle(1, "renamed");
            flush();
            expect(h.state.todos[0].title).toBe("renamed");
            expect(h.state.todos[1].title).toBe("run tests");
            // the title write is not observed by the done-count effect
            expect(h.log).toEqual(["done:2"]);
        }));

    it("item proxies keep their identity across unrelated writes", () =>
        withApp(h => {
            const first = h.state.todos[0];
            h.toggle(2);
            h.addTodo("x");
            flush();
            expect(h.state.todos[0]).toBe(first);
            expect(h.state.todos[1].completed).toBe(false);
            expect(h.state.todos.length).toBe(3);
        }));

    it("assigning a filtered array (canonical removal idiom) removes items and keeps survivors", () =>
        withApp(h => {
            const survivor = h.state.todos[0];
            h.log.length = 0;
            h.removeCompleted();
            flush();
            expect(h.state.todos.map(t => t.id)).toEqual([1]);
            expect(h.state.todos[0]).toBe(survivor);
            expect(h.log).toEqual(["len:1", "done:0"]);
        }));

    it("RemoveAt on the draft splices the store array", () =>
        withApp(h => {
            h.log.length = 0;
            h.removeFirst();
            flush();
            expect(h.state.todos.map(t => t.id)).toEqual([2]);
            expect(h.state.todos[0].title).toBe("run tests");
            expect(h.log).toContain("len:1");
        }));

    it("an updater returning a brand-new record replaces the state", () =>
        withApp(h => {
            h.log.length = 0;
            h.replaceState();
            flush();
            expect(h.state.count).toBe(100);
            expect(h.state.user.name).toBe("Zed");
            expect(h.state.user.address.city).toBe("Nowhere");
            expect(h.state.todos.length).toBe(0);
            expect(h.log).toEqual(["count:100", "name:Zed", "city:Nowhere", "len:0", "done:0"]);
        }));

    it("an updater returning a record copy ({ s with count }) is adopted like a JS spread return", () => {
        // Reference: the same shape authored directly in JS, `setStore(s => ({ ...s, count: 7 }))`.
        const jsLog = [];
        let disposeJs;
        const [js, setJs] = createRoot(d => {
            disposeJs = d;
            const r = createStore({
                count: 0,
                user: {name: "Ada", address: {city: "London"}},
                todos: [{completed: true}]
            });
            createEffect(() => r[0].count, v => {
                jsLog.push(`count:${v}`);
            });
            createEffect(() => r[0].user.name, v => {
                jsLog.push(`name:${v}`);
            });
            createEffect(() => r[0].user.address.city, v => {
                jsLog.push(`city:${v}`);
            });
            return r;
        });
        flush();
        jsLog.length = 0;
        setJs(s => ({...s, count: 7}));
        flush();
        expect(js.count).toBe(7);
        disposeJs();

        withApp(h => {
            h.log.length = 0;
            h.copyWithCount(7);
            flush();
            expect(h.state.count).toBe(7);
            expect(h.state.user.name).toBe("Ada");
            expect(h.state.todos.length).toBe(2);
            // Adoption of a returned object re-runs the computes of every adopted key; effects have no
            // equality gate, so all of them fire — exactly as for the JS spread return above.
            expect(jsLog).toEqual(["count:7", "name:Ada", "city:London"]);
            expect(h.log).toEqual(["count:7", "name:Ada", "city:London", "len:2", "done:1"]);
            // nested state stays reactive after the copy was adopted
            h.log.length = 0;
            h.moveCity("Rome");
            flush();
            expect(h.log).toEqual(["city:Rome"]);
        });
    });

    it("snapshot returns an unwrapped value that tracks nothing and does not alias the proxy", () =>
        withApp(h => {
            h.increment();
            flush();
            const snap = h.snap();
            expect(snap).not.toBe(h.state);
            expect(snap.count).toBe(1);
            expect(snap.user.address.city).toBe("London");
            expect(snap.todos.map(t => t.title)).toEqual(["write tests", "run tests"]);
            // the snapshot is a plain value graph: later store writes are not visible through it
            h.increment();
            flush();
            expect(h.state.count).toBe(2);
            expect(snap.count).toBe(1);
        }));
});

describe("createStore option overloads", () => {
    it("deep store (shallow=false): an in-place nested row write is reactive", () => {
        const h = makeShallowStore(false);
        try {
            flush();
            h.mutateRowInPlace("b");
            flush();
            expect(h.table.rows[0].label).toBe("b");
            expect(h.log).toEqual(["a", "b"]);
        } finally {
            h.dispose();
        }
    });

    it("shallow store: only root keys are reactive; nested in-place mutation is inert", () => {
        const h = makeShallowStore(true);
        try {
            flush();
            h.mutateRowInPlace("b");
            flush();
            // the raw row really was mutated, but nothing was notified
            expect(h.table.rows[0].label).toBe("b");
            expect(h.log).toEqual(["a"]);
            h.replaceRows("c");
            flush();
            expect(h.table.rows[0].label).toBe("c");
            expect(h.log).toEqual(["a", "c"]);
        } finally {
            h.dispose();
        }
    });

    it("StoreOptions pojo overload creates a working store", () => {
        const s = makeNamedStore();
        expect(s.label).toBe("named");
    });

    it("createStore over an existing store returns the same proxy", () => {
        const [first, second] = wrapExistingStore();
        expect(second).toBe(first);
        expect(second.label).toBe("x");
    });
});

describe("deep()", () => {
    it("an effect over deep(store) reruns on any nested write", () => {
        const h = makeDeepStore();
        try {
            flush();
            expect(h.runs()).toBe(1);
            h.moveCity("Oslo");
            flush();
            expect(h.runs()).toBe(2);
            h.increment();
            flush();
            expect(h.runs()).toBe(3);
        } finally {
            h.dispose();
        }
    });

    it("without deep(), an effect reading only the nested reference ignores nested writes", () => {
        const h = makeShallowReadStore();
        try {
            flush();
            expect(h.runs()).toBe(1);
            h.moveCity("Oslo");
            h.increment();
            flush();
            expect(h.runs()).toBe(1);
        } finally {
            h.dispose();
        }
    });
});
