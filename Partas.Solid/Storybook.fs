namespace Partas.Solid.Storybook

open System.Runtime.CompilerServices
open Fable.Core

#nowarn 49

[<AutoOpen; Erase>]
module Builder =
    type Storybook<'T> = interface end
    type StorybookArgs<'T> = interface end
    type StorybookFun<'T> = Storybook<'T> -> unit
    type StorybookArgsFun<'T> = StorybookArgs<'T> -> unit

    type Storybook<'T> with
        member inline _.Combine
            ([<InlineIfLambda>] PARTAS_FIRST: StorybookFun<'T>, [<InlineIfLambda>] PARTAS_SECOND: StorybookFun<'T>)
            : StorybookFun<'T> =
            fun PARTAS_BUILDER ->
                PARTAS_FIRST PARTAS_BUILDER
                PARTAS_SECOND PARTAS_BUILDER

        member inline _.Zero() : StorybookFun<'T> = ignore
        member inline _.Yield(_: unit) : StorybookFun<'T> = ignore

        member inline _.Delay([<InlineIfLambda>] PARTAS_DELAY: unit -> StorybookFun<'T>) : StorybookFun<'T> =
            PARTAS_DELAY ()

        member inline _.Yield([<InlineIfLambda>] PARTAS_ELEMENT: 'T -> 'Value) : StorybookFun<'T> =
            fun PARTAS_YIELD -> ignore PARTAS_ELEMENT

        member inline _.Yield(PARTAS_VALUE: StorybookArgs<'T>) : StorybookFun<'T> =
            fun PARTAS_YIELD -> ignore PARTAS_VALUE

        [<CustomOperation "cases">]
        member inline _.Cases([<InlineIfLambda>] PARTAS_FIRST: StorybookFun<'T>, PARTAS_CASES: ('T -> obj)) : StorybookFun<'T> =
            fun PARTAS_BUILDER ->
                ignore PARTAS_CASES
                PARTAS_FIRST PARTAS_BUILDER

        [<CustomOperation "args">]
        member inline _.Args([<InlineIfLambda>] PARTAS_FIRST: StorybookFun<'T>, [<InlineIfLambda>] PARTAS_ARGS: ('T -> unit)) : StorybookFun<'T> =
            fun PARTAS_BUILDER ->
                ignore PARTAS_ARGS
                PARTAS_FIRST PARTAS_BUILDER

        [<CustomOperation "args">]
        member inline _.Args
            ([<InlineIfLambda>] PARTAS_FIRST: StorybookFun<'T>, PARTAS_VARIANT: string, [<InlineIfLambda>] PARTAS_VARIANT_ARGS: ('T -> unit))
            : StorybookFun<'T> =
            fun PARTAS_BUILDER ->
                fun (PARTAS_ARG_BUILDER: 'T) ->
                    ignore (
                        "PARTAS_VARIANT"
                        + PARTAS_VARIANT
                    )
                    PARTAS_ARG_BUILDER |> PARTAS_VARIANT_ARGS
                |> ignore
                PARTAS_FIRST PARTAS_BUILDER

        member inline _.For
            ([<InlineIfLambda>] PARTAS_FIRST: StorybookFun<'T>, [<InlineIfLambda>] PARTAS_SECOND: unit -> StorybookFun<'T>)
            : StorybookFun<'T> =
            fun PARTAS_BUILDER ->
                PARTAS_FIRST PARTAS_BUILDER
                PARTAS_SECOND () PARTAS_BUILDER

        [<CustomOperation "render">]
        member inline _.Render
            ([<InlineIfLambda>] PARTAS_FIRST: StorybookFun<'T>, [<InlineIfLambda>] PARTAS_RENDER: 'T -> #Partas.Solid.Builder.HtmlElement)
            : StorybookFun<'T> =
            fun PARTAS_BUILDER ->
                ignore PARTAS_RENDER
                PARTAS_FIRST PARTAS_BUILDER

        [<CustomOperation "render">]
        member inline _.Render
            (
                [<InlineIfLambda>] PARTAS_FIRST: StorybookFun<'T>,
                PARTAS_RENDER_VARIANT: string,
                [<InlineIfLambda>] PARTAS_VARIANT_RENDER: 'T -> #Partas.Solid.Builder.HtmlElement
            ) : StorybookFun<'T> =
            fun PARTAS_BUILDER ->
                fun PARTAS_RENDER_BUILDER ->
                    ignore (
                        "PARTAS_RENDER_VARIANT"
                        + PARTAS_RENDER_VARIANT
                    )
                    PARTAS_RENDER_BUILDER |> PARTAS_VARIANT_RENDER
                |> ignore
                PARTAS_FIRST PARTAS_BUILDER
        [<CustomOperation "decorator">]
        member inline _.Decorator([<InlineIfLambda>] PARTAS_FIRST: StorybookFun<'T>, [<InlineIfLambda>] PARTAS_DECORATOR: (unit -> 'T) -> Partas.Solid.Builder.HtmlElement) =
            fun PARTAS_BUILDER ->
                ignore PARTAS_DECORATOR
                PARTAS_FIRST PARTAS_BUILDER
        [<CustomOperation "decorator">]
        member inline _.Decorator([<InlineIfLambda>] PARTAS_FIRST: StorybookFun<'T>, PARTAS_DECORATOR_VARIANT: string, [<InlineIfLambda>] PARTAS_VARIANT_DECORATOR: (unit -> 'T) -> Partas.Solid.Builder.HtmlElement) =
            fun PARTAS_BUILDER ->
                fun PARTAS_DECORATOR_BUILDER ->
                    ignore (
                        "PARTAS_DECORATOR_VARIANT"
                        + PARTAS_DECORATOR_VARIANT
                        )
                    PARTAS_VARIANT_DECORATOR PARTAS_DECORATOR_BUILDER
                |> ignore
                PARTAS_FIRST PARTAS_BUILDER

    type StorybookArgs<'T> with
        member inline _.Combine
            ([<InlineIfLambda>] PARTAS_FIRST: StorybookArgsFun<'T>, [<InlineIfLambda>] PARTAS_SECOND: StorybookArgsFun<'T>)
            : StorybookArgsFun<'T> =
            fun PARTAS_BUILDER ->
                PARTAS_FIRST PARTAS_BUILDER
                PARTAS_SECOND PARTAS_BUILDER

        member inline _.Zero() : StorybookArgsFun<'T> = ignore

        member inline _.Delay([<InlineIfLambda>] PARTAS_DELAY: unit -> StorybookArgsFun<'T>) : StorybookArgsFun<'T> =
            PARTAS_DELAY ()

        member inline _.Yield(PARTAS_VALUE: ('T -> 'Value) * ('Value)) : StorybookArgsFun<'T> =
            fun PARTAS_YIELD -> ignore PARTAS_VALUE

    type StorybookExtensions =
        [<Extension; Erase>]
        static member Run(PARTAS_THIS: Storybook<'T>, PARTAS_RUN: StorybookFun<'T>) =
            PARTAS_RUN PARTAS_THIS
            PARTAS_THIS

        [<Extension; Erase>]
        static member Run(PARTAS_THIS: StorybookArgs<'T>, PARTAS_RUN: StorybookArgsFun<'T>) =
            PARTAS_RUN PARTAS_THIS
            PARTAS_THIS

    let storybook<'T> = Unchecked.defaultof<Storybook<'T>>
    let args<'T> = Unchecked.defaultof<StorybookArgs<'T>>
