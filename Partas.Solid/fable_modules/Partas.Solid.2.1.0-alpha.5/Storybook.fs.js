
import { class_type } from "../fable-library-js.5.0.0-alpha.14/Reflection.js";
import { defaultOf } from "../fable-library-js.5.0.0-alpha.14/Util.js";

export class StorybookExtensions {
    constructor() {
    }
}

export function StorybookExtensions_$reflection() {
    return class_type("Partas.Solid.Storybook.Builder.StorybookExtensions", undefined, StorybookExtensions);
}

export function storybook() {
    return defaultOf();
}

export function args() {
    return defaultOf();
}

