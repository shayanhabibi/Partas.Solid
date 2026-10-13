import {describe, it, expect, vi} from "vitest";
import {mount, text, deferred, settle} from "../../helpers/index.js";
import {click} from "./guard.js";
import {LikeButton} from "./G-Like.fs.jsx";

const $ = (root, sel) => root.querySelector(sel);

const view = c => ({
    liked: $(c, "button.like").classList.contains("liked"),
    icon: text($(c, "button.like")),
    count: text($(c, ".count")),
    confirmed: text($(c, ".confirmed")),
    error: $(c, ".like-error") ? text($(c, ".like-error")) : null
});

function setup({initialLikes = 41, initiallyLiked = false} = {}) {
    const calls = [];
    const save = vi.fn(liked => {
        const d = deferred();
        calls.push(d);
        return d.promise;
    });
    const {container} = mount(LikeButton, {initialLikes, initiallyLiked, save});
    return {c: container, save, calls};
}

describe("AppsMore: optimistic like button (createOptimistic + action)", () => {
    it("renders the initial state", () => {
        const {c} = setup();
        expect(view(c)).toEqual({liked: false, icon: "♡", count: "41 likes", confirmed: "41", error: null});
    });

    it("clicking flips the heart and count immediately, before the server answers", async () => {
        const {c, save} = setup();
        click($(c, "button.like"));
        await settle(1);
        expect(save.mock.calls).toEqual([[true]]);
        expect(view(c)).toMatchObject({liked: true, icon: "♥", count: "42 likes", confirmed: "41"});
    });

    it("the server confirmation becomes the authoritative value", async () => {
        const {c, calls} = setup();
        click($(c, "button.like"));
        await settle(1);
        calls[0].resolve(100); // someone else liked it meanwhile
        await settle();
        expect(view(c)).toEqual({liked: true, icon: "♥", count: "100 likes", confirmed: "100", error: null});
    });

    it("a failed save reverts the optimistic values and shows an error", async () => {
        const {c, calls} = setup();
        click($(c, "button.like"));
        await settle(1);
        expect(view(c).liked).toBe(true);
        calls[0].reject(new Error("offline"));
        await settle();
        expect(view(c)).toEqual({
            liked: false,
            icon: "♡",
            count: "41 likes",
            confirmed: "41",
            error: "Could not save: offline"
        });
    });

    it("the error clears on the next attempt, and unliking works from a liked start", async () => {
        const {c, calls, save} = setup({initialLikes: 7, initiallyLiked: true});
        expect(view(c)).toMatchObject({liked: true, icon: "♥", count: "7 likes"});
        click($(c, "button.like"));
        await settle(1);
        calls[0].reject(new Error("offline"));
        await settle();
        expect(view(c).error).toBe("Could not save: offline");

        click($(c, "button.like"));
        await settle(1);
        // Optimistic values show at once. setError("") runs in the same batch as the action call, so it
        // is part of the action's transition and only commits with it (Solid 2 semantics).
        expect(view(c)).toMatchObject({liked: false, count: "6 likes"});
        calls[1].resolve(6);
        await settle();
        expect(view(c)).toEqual({liked: false, icon: "♡", count: "6 likes", confirmed: "6", error: null});
        expect(save.mock.calls).toEqual([[false], [false]]);
    });
});
