import { class_type } from "../fable-library-js.5.0.0-alpha.13/Reflection.js";

export class BindingsModule_Route {
    constructor() {
    }
}

export function BindingsModule_Route_$reflection() {
    return class_type("Partas.Solid.Router.BindingsModule.Route", undefined, BindingsModule_Route);
}

export function BindingsModule_Route_$ctor() {
    return new BindingsModule_Route();
}

export class BindingsModule_Router {
    constructor() {
    }
}

export function BindingsModule_Router_$reflection() {
    return class_type("Partas.Solid.Router.BindingsModule.Router", undefined, BindingsModule_Router);
}

export function BindingsModule_Router_$ctor() {
    return new BindingsModule_Router();
}

export class BindingsModule_HashRouter extends BindingsModule_Router {
    constructor() {
        super();
    }
}

export function BindingsModule_HashRouter_$reflection() {
    return class_type("Partas.Solid.Router.BindingsModule.HashRouter", undefined, BindingsModule_HashRouter, BindingsModule_Router_$reflection());
}

export function BindingsModule_HashRouter_$ctor() {
    return new BindingsModule_HashRouter();
}

