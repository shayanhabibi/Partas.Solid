module Partas.Solid.examples.kobalte

open Partas.Solid
open Partas.Solid.Kobalte
open Fable.Core

[<SolidComponent>]
let ColorAreaExample() =
    let color,setColor = createSignal<Color>(JS.undefined)
    let colorLabel format =
        if color() |> unbox then color().toString(format)
        else $"{format}"
    ColorArea(
        class' = "relative flex flex-col
                align-items-center w-[200px]"
        ,value = color()
        // ,onChange = unbox setColor
        ) {
        div(class' = "flex flex-col pb-2") {
        ColorArea.Label() { colorLabel Color.ToString.Format.Hsl }
        ColorArea.Label() { colorLabel Color.ToString.Format.Rgb }
        ColorArea.Label() { colorLabel Color.ToString.Format.Hex }
        ColorArea.Label() { colorLabel Color.ToString.Format.Hsb }
        }
        ColorArea.Background(
            class' = "relative rounded-sm
                    h-[150px] w-[150px]"
            ) {
            ColorArea.Thumb(
                class' = "block w-[16px] h-[16px]
                rounded-full border-1 border-border
                bg-(--kb-color-current)"
                ) {
                ColorArea.HiddenInputX()
                ColorArea.HiddenInputY()
            }
        }
    }
