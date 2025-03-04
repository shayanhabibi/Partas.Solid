namespace Partas.Solid

open System.Runtime.CompilerServices
open Fable.Core

#nowarn 49

[<AutoOpen>]
module Builder =

    [<Struct>]
    [<Erase>]
    type HtmlAttribute = { Name: string; Value: obj }

    type HtmlElement = interface end
    type HtmlTag =
        inherit HtmlElement
    type HtmlContainer =
        inherit HtmlElement

    [<Erase>]
    type RegularNode() =
        interface HtmlTag
        interface HtmlContainer

    [<Erase>]
    type FragmentNode() =
        interface HtmlContainer

    [<Erase>]
    type VoidNode() =
        interface HtmlTag

    type HtmlContainerFun = HtmlContainer -> unit

    type HtmlContainer with
        member inline _.Combine
            ([<InlineIfLambda>] PARTAS_FIRST: HtmlContainerFun, [<InlineIfLambda>] PARTAS_SECOND: HtmlContainerFun)
            : HtmlContainerFun =
            fun PARTAS_BUILDER ->
                PARTAS_FIRST PARTAS_BUILDER
                PARTAS_SECOND PARTAS_BUILDER

        [<Erase>]
        member inline _.Zero() : HtmlContainerFun = ignore

        [<Erase>]
        member inline _.Delay([<InlineIfLambda>] PARTAS_DELAY: unit -> HtmlContainerFun) : HtmlContainerFun = PARTAS_DELAY()

        [<Erase>]
        member inline _.Yield(PARTAS_ELEMENT: #HtmlElement) : HtmlContainerFun = fun PARTAS_YIELD -> ignore PARTAS_ELEMENT

        [<Erase>]
        member inline _.Yield(PARTAS_TEXT: string) : HtmlContainerFun = fun PARTAS_YIELD -> ignore PARTAS_TEXT

        [<Erase>]
        member inline _.Yield(PARTAS_TEXT: int) : HtmlContainerFun = fun PARTAS_YIELD -> ignore PARTAS_TEXT

    [<Erase>]
    type HtmlContainerExtensions =
        [<Extension; Erase>]
        static member Run(PARTAS_THIS: #HtmlContainer, PARTAS_RUN: HtmlContainerFun) =
            PARTAS_RUN PARTAS_THIS
            PARTAS_THIS