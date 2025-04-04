namespace Partas.Solid

open System.Runtime.CompilerServices
open Fable.Core

#nowarn 49
#nowarn 64

[<AutoOpen>]
module Builder =
    type HtmlElement = interface end
    type HtmlTag =
        inherit HtmlElement
    type HtmlContainer =
        inherit HtmlElement

    type SvgTag =
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

    [<Erase>]
    type SvgNode() =
        interface SvgTag
        interface HtmlContainer
    
    /// <summary>
    /// Explicit plugin declaration that the identifier that this
    /// is bound to can be treated as a Tag. <br/>
    /// This allows you to pass tags constructors as functions/values 
    /// </summary>
    /// <example><code>
    /// // Verbose syntax
    /// let tag = TagValue(div)
    /// // Operator overload
    /// let tag = !@div
    /// </code></example>
    [<Erase>]
    type TagValue(tag: unit -> #HtmlElement) =
        /// <summary>
        /// Directs the plugin to build the call site as a Tag
        /// in JSX.
        /// </summary>
        /// <example><code>
        /// let tag = TagValue(div)
        /// // Verbose syntax
        /// tag.render({| class' = "hello" |})
        /// // alt
        /// tag.render(div(class' = "hello"))
        /// // Operator overload
        /// tag % {| class' = "hello |}
        /// // alt
        /// tag % div(class' = "hello")
        /// </code></example>
        [<Erase>]
        member this.render (PARTAS_CONSTRUCTOR: 'T): 'T = jsNative
        
        /// <summary>
        /// Directs the plugin to build the call site as a Tag
        /// in JSX.
        /// </summary>
        /// <example><code>
        /// let tag = TagValue(div)
        /// // Verbose syntax
        /// tag.render({| class' = "hello" |})
        /// // alt
        /// tag.render(div(class' = "hello"))
        /// // Operator overload
        /// tag % {| class' = "hello |}
        /// // alt
        /// tag % div(class' = "hello")
        /// </code></example>
        static member inline (%) (left: TagValue, right: 'T): 'T = left.render(right)
        [<Erase>]
        member this.render (PARTAS_PROPERTIES: obj): RegularNode = jsNative
        
        [<Erase>]
        member this.render (): RegularNode = jsNative
        static member inline (%) (left: TagValue, right: obj) = left.render(right)
    
    [<Erase>]
    let (!@) (this: unit -> 'T) = TagValue(unbox this)
    
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
        
        // [<Erase>]
        // member inline _.For(PARTAS_VALUES: #seq<'T>, [<InlineIfLambda>] PARTAS_BODY: 'T -> HtmlContainerFun): HtmlContainerFun =
        //     fun PARTAS_BUILDER ->
        //         for PARTAS_VALUE in PARTAS_VALUES do
        //             PARTAS_BODY PARTAS_VALUE PARTAS_BUILDER

    [<Erase>]
    type HtmlContainerExtensions =
        [<Extension; Erase>]
        static member Run(PARTAS_THIS: #HtmlContainer, PARTAS_RUN: HtmlContainerFun) =
            PARTAS_RUN PARTAS_THIS
            PARTAS_THIS