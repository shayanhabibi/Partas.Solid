import { splitProps, createSignal } from "solid-js";
import { equals } from "../fable_modules/fable-library-js.5.0.0-alpha.13/Util.js";

export function Button(title, variant) {
    return <button style={(variant === "ghost") ? "" : "background-color: var(--sb-important-background-color)"}>
        {title}
    </button>;
}

export function ButtonExample() {
    const patternInput = createSignal("ghost");
    const variant = patternInput[0];
    return <div>
        {Button("Primary", "primary")}
        {Button("Ghost", "ghost")}
        {Button("Responsive", variant())}
        <button onClick={(_arg) => {
                patternInput[1](equals(variant(), "primary") ? "ghost" : "primary");
            }}>
            Click me
        </button>
    </div>;
}

export function MyComponent(props) {
    const [PARTAS_LOCAL, PARTAS_OTHERS] = splitProps(props, ["myAttribute"]);
    return <button>
        {PARTAS_LOCAL.myAttribute ? ("MyComponent") : ("Button")}
    </button>;
}

export function MyComponentExample() {
    const patternInput = createSignal(false);
    const sign = patternInput[0];
    return <div>
        <button onClick={(_arg) => {
                patternInput[1](!sign());
            }}>
            Click me!
        </button>
        <MyComponent myAttribute={sign()} />
    </div>;
}

