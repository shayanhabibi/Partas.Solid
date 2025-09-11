
import { Show, createSignal, splitProps } from "solid-js";

export function ReactiveComponent(props) {
    const [PARTAS_LOCAL, PARTAS_OTHERS] = splitProps(props, ["myAttribute"]);
    return <button>
        {PARTAS_LOCAL.myAttribute ? ("MyComponent") : ("Button")}
    </button>;
}

export function UnreactiveComponent(props) {
    const [PARTAS_LOCAL, PARTAS_OTHERS] = splitProps(props, ["myAttribute"]);
    const buttonText = PARTAS_LOCAL.myAttribute ? "MyComponent" : "Button";
    return <button>
        {buttonText}
    </button>;
}

export function MyReactiveComponentExample() {
    const patternInput = createSignal(false);
    const sign = patternInput[0];
    return <div>
        <button onClick={(_arg) => {
                patternInput[1](!sign());
            }}>
            Click me!
        </button>
        <ReactiveComponent myAttribute={sign()} />
    </div>;
}

export function MyUnreactiveComponentExample() {
    const patternInput = createSignal(false);
    const sign = patternInput[0];
    return <div>
        <button onClick={(_arg) => {
                patternInput[1](!sign());
            }}>
            Click me!
        </button>
        <UnreactiveComponent myAttribute={sign()} />
    </div>;
}

export function Conditional() {
    const patternInput = createSignal(false);
    const truthy = patternInput[0];
    return <div>
        <button onClick={(_arg) => {
                patternInput[1](!truthy());
            }}>
            Click me!
        </button>
        {truthy() ? ("Boo!") : (undefined)}
        <Show when={!truthy()}>
            Do you hear that?
        </Show>
    </div>;
}

