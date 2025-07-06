import { class_type } from "../fable-library-js.5.0.0-alpha.13/Reflection.js";
import { append } from "../fable-library-js.5.0.0-alpha.13/Array.js";

export class BindingsModule_SolidStorePath$2 {
    constructor(setter, path) {
        this.setter = setter;
        this.path = path;
    }
}

export function BindingsModule_SolidStorePath$2_$reflection(gen0, gen1) {
    return class_type("Partas.Solid.BindingsModule.SolidStorePath`2", [gen0, gen1], BindingsModule_SolidStorePath$2);
}

export function BindingsModule_SolidStorePath$2_$ctor_33ECB2AF(setter, path) {
    return new BindingsModule_SolidStorePath$2(setter, path);
}

export function BindingsModule_SolidStorePath$2__get_Setter(_) {
    return _.setter;
}

export function BindingsModule_SolidStorePath$2__get_Path(_) {
    return _.path;
}

/**
 * Update store item using new value
 */
export function BindingsModule_SolidStorePath$2__Update_2B594(this$, value) {
    BindingsModule_SolidStorePath$2__get_Setter(this$)(...append(BindingsModule_SolidStorePath$2__get_Path(this$), [value]));
}

/**
 * Update store item specifying updater function from old value to new value
 */
export function BindingsModule_SolidStorePath$2__Update_Z1D9544EA(this$, updater) {
    BindingsModule_SolidStorePath$2__get_Setter(this$)(...append(BindingsModule_SolidStorePath$2__get_Path(this$), [updater]));
}

/**
 * Access more convenient way of updating store items
 */
export function Partas_Solid_BindingsModule_SolidStoreSetter$1__SolidStoreSetter$1_get_Path(this$) {
    return BindingsModule_SolidStorePath$2_$ctor_33ECB2AF(this$, []);
}

