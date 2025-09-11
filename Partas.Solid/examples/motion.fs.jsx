
import { Show, onCleanup, createEffect, createSignal, splitProps, mergeProps } from "solid-js";
import { Motion, Presence } from "solid-motionone";
import { item } from "../fable_modules/fable-library-js.5.0.0-alpha.14/Array.js";

export function WordRotate(props) {
    props = mergeProps({
        duration: 2500,
    }, props);
    const [PARTAS_LOCAL, PARTAS_OTHERS] = splitProps(props, ["words", "duration"]);
    const patternInput = createSignal(0);
    const index = patternInput[0];
    createEffect(() => {
        const interval = setInterval(() => {
            patternInput[1]((prevIndex) => (((prevIndex + 1) % PARTAS_LOCAL.words.length) | 0));
        }, PARTAS_LOCAL.duration) | 0;
        onCleanup(() => {
            clearInterval(interval);
        });
    });
    return <Presence exitBeforeEnter={true}>
        <Show when={index() + 1}
            keyed={true}>
            <Motion initial={{
                    opacity: 0,
                    y: -50,
                }}
                animate={{
                    opacity: 1,
                    y: 0,
                }}
                exit={{
                    opacity: 0,
                    y: 50,
                }}
                transition={{
                    duration: 0.25,
                    easing: "ease-out",
                }}
                {...PARTAS_OTHERS} bool:n$={false}>
                {item(index(), PARTAS_LOCAL.words)}
            </Motion>
        </Show>
    </Presence>;
}

export function WordRotateExample() {
    return <div>
        <WordRotate style="font-size: 4rem"
            words={["Word", "Rotate"]} />
    </div>;
}

export function KeyframeExample() {
    return <Motion.div animate={{
            scale: [1, 2, 2, 1, 1],
            rotate: [0, 0, 180, 180, 0],
            borderRadius: [`${0}%`, `${0}%`, `${50}%`, `${50}%`, `${0}%`],
        }}
        transition={{
            duration: 2,
            easing: "ease-in-out",
            offset: [0, 0.2, 0.5, 0.8, 1],
            repeat: Infinity,
            endDelay: 1,
        }}
        style="width: 100px; height: 100px; background-color: green" />;
}

