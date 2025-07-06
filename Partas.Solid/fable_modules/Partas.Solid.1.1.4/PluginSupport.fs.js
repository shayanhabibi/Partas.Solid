import { Attribute } from "../fable-library-js.5.0.0-alpha.13/Types.js";
import { class_type } from "../fable-library-js.5.0.0-alpha.13/Reflection.js";

export class PartasImportAttribute extends Attribute {
    constructor(selector, path) {
        super();
    }
}

export function PartasImportAttribute_$reflection() {
    return class_type("Partas.Solid.PartasImportAttribute", undefined, PartasImportAttribute, class_type("System.Attribute"));
}

export function PartasImportAttribute_$ctor_Z384F8060(selector, path) {
    return new PartasImportAttribute(selector, path);
}

