import { createSignal } from "solid-js";
import { HiddenInputY, HiddenInputX, Thumb, Background, Label, Root } from "@kobalte/core/color-area";

export function ColorAreaExample() {
    const color = createSignal(void 0)[0];
    const colorLabel = (format) => {
        if (color()) {
            return color().toString(format);
        }
        else {
            return `${format}`;
        }
    };
    return <Root class="relative flex flex-col
                align-items-center w-[200px]"
        value={color()}>
        <div class="flex flex-col pb-2">
            <Label>
                {colorLabel("hsl")}
            </Label>
            <Label>
                {colorLabel("rgb")}
            </Label>
            <Label>
                {colorLabel("hex")}
            </Label>
            <Label>
                {colorLabel("hsb")}
            </Label>
        </div>
        <Background class="relative rounded-sm
                    h-[150px] w-[150px]">
            <Thumb class="block w-[16px] h-[16px]
                rounded-full border-1 border-border
                bg-(--kb-color-current)">
                <HiddenInputX />
                <HiddenInputY />
            </Thumb>
        </Background>
    </Root>;
}

