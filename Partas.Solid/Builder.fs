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
    
    type IChildLambdaProvider = inherit HtmlElement
    /// <summary>
    /// Interface this with the parameter type and the type of children
    /// and their descendants to accept to allow the component to accept
    /// a lambda function passing that parameter which returns the type of children
    /// provided.
    /// </summary>
    /// <typeparam name="'Param1">Type parameter for the lambda</typeparam>
    /// <typeparam name="'Children">Type parameter for the children</typeparam>
    type ChildLambdaProviderStrict<'Param1, 'Children> = inherit IChildLambdaProvider
    /// <summary>
    /// Interface this with the parameter type and the type of children
    /// and their descendants to accept to allow the component to accept
    /// a lambda function passing that parameter which returns the type of children
    /// provided.
    /// </summary>
    /// <typeparam name="'Param1">Type parameter for the first parameter of the lambda</typeparam>
    /// <typeparam name="'Param2">Type parameter for the second parameter of the lambda</typeparam>
    /// <typeparam name="'Children">Type parameter for the children</typeparam>
    type ChildLambdaProviderStrict2<'Param1, 'Param2, 'Children> = inherit IChildLambdaProvider
    /// <summary>
    /// Interface this with the parameter type and the type of children
    /// and their descendants to accept to allow the component to accept
    /// a lambda function passing that parameter which returns the type of children
    /// provided.
    /// </summary>
    /// <typeparam name="'Param1">Type parameter for the first parameter of the lambda</typeparam>
    /// <typeparam name="'Param2">Type parameter for the second parameter of the lambda</typeparam>
    /// <typeparam name="'Param3">Type parameter for the third parameter of the lambda</typeparam>
    /// <typeparam name="'Children">Type parameter for the children</typeparam>
    type ChildLambdaProviderStrict3<'Param1, 'Param2, 'Param3, 'Children> = inherit IChildLambdaProvider
    /// <summary>
    /// Interface this with the parameter type and the type of children
    /// and their descendants to accept to allow the component to accept
    /// a lambda function passing that parameter which returns the type of children
    /// provided.
    /// </summary>
    /// <typeparam name="'Param1">Type parameter for the first parameter of the lambda</typeparam>
    /// <typeparam name="'Param2">Type parameter for the second parameter of the lambda</typeparam>
    /// <typeparam name="'Param3">Type parameter for the third parameter of the lambda</typeparam>
    /// <typeparam name="'Param4">Type parameter for the fourth parameter of the lambda</typeparam>
    /// <typeparam name="'Children">Type parameter for the children</typeparam>
    type ChildLambdaProviderStrict4<'Param1, 'Param2, 'Param3, 'Param4, 'Children> = inherit IChildLambdaProvider
    /// <summary>
    /// Interface this with the parameter type to allow the component to accept
    /// a lambda function passing that parameter to the children
    /// </summary>
    /// <typeparam name="'Param1">Type parameter for the first parameter of the lambda</typeparam>
    type ChildLambdaProvider<'Param1> = inherit IChildLambdaProvider
    /// <summary>
    /// Interface this with the parameter type to allow the component to accept
    /// a lambda function passing that parameter to the children
    /// </summary>
    /// <typeparam name="'Param1">Type parameter for the first parameter of the lambda</typeparam>
    /// <typeparam name="'Param2">Type parameter for the second parameter of the lambda</typeparam>
    type ChildLambdaProvider2<'Param1, 'Param2> = inherit IChildLambdaProvider
    /// <summary>
    /// Interface this with the parameter type to allow the component to accept
    /// a lambda function passing that parameter to the children
    /// </summary>
    /// <typeparam name="'Param1">Type parameter for the first parameter of the lambda</typeparam>
    /// <typeparam name="'Param2">Type parameter for the second parameter of the lambda</typeparam>
    /// <typeparam name="'Param3">Type parameter for the third parameter of the lambda</typeparam>
    type ChildLambdaProvider3<'Param1, 'Param2, 'Param3> = inherit IChildLambdaProvider
    /// <summary>
    /// Interface this with the parameter type to allow the component to accept
    /// a lambda function passing that parameter to the children
    /// </summary>
    /// <typeparam name="'Param1">Type parameter for the first parameter of the lambda</typeparam>
    /// <typeparam name="'Param2">Type parameter for the second parameter of the lambda</typeparam>
    /// <typeparam name="'Param3">Type parameter for the third parameter of the lambda</typeparam>
    /// <typeparam name="'Param4">Type parameter for the fourth parameter of the lambda</typeparam>
    type ChildLambdaProvider4<'Param1, 'Param2, 'Param3, 'Param4> = inherit IChildLambdaProvider
    
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
    let (!@) (this: unit -> 'T) = TagValue(unbox this)
    
    /// Alias used in the provided builder
    type HtmlContainerFun = HtmlContainer -> unit
    /// Alias used in the provided builder
    type ChildProviderFun = IChildLambdaProvider -> unit
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

    
    type IChildLambdaProvider with
        [<Erase>]
        member inline _.Delay([<InlineIfLambda>] PARTAS_DELAY: unit -> ChildProviderFun): ChildProviderFun = PARTAS_DELAY()
    type ChildLambdaProviderStrict<'Param1, 'Children> with
        [<Erase>]
        member inline _.Yield(PARTAS_ELEMENT: #'Param1 -> #'Children): ChildProviderFun = fun PARTAS_CONT -> ignore PARTAS_ELEMENT
    type ChildLambdaProviderStrict2<'Param1, 'Param2, 'Children> with
        [<Erase>]
        member inline _.Yield(PARTAS_ELEMENT: #'Param1 -> #'Param2 -> #'Children): ChildProviderFun = fun PARTAS_CONT -> ignore PARTAS_ELEMENT
    type ChildLambdaProviderStrict3<'Param1, 'Param2, 'Param3, 'Children> with
        [<Erase>]
        member inline _.Yield(PARTAS_ELEMENT: #'Param1 -> #'Param2 -> #'Param3 -> #'Children): ChildProviderFun = fun PARTAS_CONT -> ignore PARTAS_ELEMENT
    type ChildLambdaProviderStrict4<'Param1, 'Param2, 'Param3, 'Param4, 'Children> with
        [<Erase>]
        member inline _.Yield(PARTAS_ELEMENT: #'Param1 -> #'Param2 -> #'Param3 -> #'Param4 -> #'Children): ChildProviderFun = fun PARTAS_CONT -> ignore PARTAS_ELEMENT
    type ChildLambdaProvider<'Param1> with
        [<Erase>]
        member inline _.Yield(PARTAS_ELEMENT: #'Param1 -> #HtmlElement): ChildProviderFun = fun PARTAS_CONT -> ignore PARTAS_ELEMENT
    type ChildLambdaProvider2<'Param1, 'Param2> with
        [<Erase>]
        member inline _.Yield(PARTAS_ELEMENT: #'Param1 -> #'Param2 -> #HtmlElement): ChildProviderFun = fun PARTAS_CONT -> ignore PARTAS_ELEMENT
    type ChildLambdaProvider3<'Param1, 'Param2, 'Param3> with
        [<Erase>]
        member inline _.Yield(PARTAS_ELEMENT: #'Param1 -> #'Param2 -> #'Param3 -> #HtmlElement): ChildProviderFun = fun PARTAS_CONT -> ignore PARTAS_ELEMENT
    type ChildLambdaProvider4<'Param1, 'Param2, 'Param3, 'Param4> with
        [<Erase>]
        member inline _.Yield(PARTAS_ELEMENT: #'Param1 -> #'Param2 -> #'Param3 -> #'Param4 -> #HtmlElement): ChildProviderFun = fun PARTAS_CONT -> ignore PARTAS_ELEMENT
    
    [<Erase>]
    type HtmlContainerExtensions =
        [<Extension; Erase>]
        static member Run(PARTAS_THIS: #HtmlContainer, PARTAS_RUN: HtmlContainerFun) =
            PARTAS_RUN PARTAS_THIS
            PARTAS_THIS
        [<Erase; Extension>]
        static member Run(PARTAS_THIS: #IChildLambdaProvider, PARTAS_RUN: ChildProviderFun) =
            PARTAS_RUN PARTAS_THIS
            PARTAS_THIS