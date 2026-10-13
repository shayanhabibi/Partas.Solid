module Partas.Solid.Tests.Plugin.Compiled.SolidCases.ValueUnrollerDecisionTree.ValueUnrollerDecisionTree

open Partas.Solid
open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop

[<Erase>]
type Lib =
    [<ImportMember("tailwind-merge")>]
    static member twMerge(classes: string) : string = jsNative

    [<ImportMember("clsx")>]
    static member clsx(classes: obj) : string = jsNative

    [<CompiledName("cn")>]
    static member cn(classes: string array) : string =
        classes
        |> Lib.clsx
        |> Lib.twMerge

[<StringEnum>]
type Variant =
    | Black
    | Brown
    | Green

[<Erase>]
type ValueUnrollTest() =
    interface RegularNode

    [<DefaultValue>]
    val mutable variant: Variant

    [<SolidTypeComponent>]
    member props.__ =
        div (
            class' =
                Lib.cn
                    [| "bg-primary"
                       match props.variant with
                       | Brown -> "brown"
                       | Black -> "black"
                       | Green -> "green" |]
        )

[<StringEnum>]
type AltVariant =
    | Blue
    | Orange
    | Yellow

[<Erase>]
type ValueUnrollNestedTest() =
    interface RegularNode

    [<DefaultValue>]
    val mutable variant: Variant

    [<DefaultValue>]
    val mutable altVariant: AltVariant

    [<SolidTypeComponent>]
    member props.__ =
        div (
            class' =
                Lib.cn
                    [| "bg-primary"
                       match props.variant with
                       | Brown -> "brown"
                       | Black when props.altVariant = Blue -> "black & blue"
                       | Black -> "black"
                       | Green ->
                           match props.altVariant with
                           | Blue -> "green & blue"
                           | Orange -> "orange & blue"
                           | Yellow -> "yellow & blue" |]
        )
