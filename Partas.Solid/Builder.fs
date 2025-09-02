namespace Partas.Solid

open System.Runtime.CompilerServices
open Fable.Core

#nowarn 49
#nowarn 64

[<AutoOpen>]
module Builder =
    [<AllowNullLiteral>]
    [<Interface>]
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    type HTMLAttributes = interface end

    [<AllowNullLiteral; Interface>]
    type HtmlElement = interface end

    [<AllowNullLiteral; Interface>]
    type HtmlTag =
        inherit HTMLAttributes
        inherit HtmlElement

    [<AllowNullLiteral; Interface>]
    type HtmlContainer =
        inherit HtmlElement

    [<AllowNullLiteral; Interface>]
    type RegularNode =
        inherit HtmlTag
        inherit HtmlContainer

    [<AllowNullLiteral; Interface>]
    type FragmentNode =
        inherit HtmlContainer

    [<AllowNullLiteral; Interface>]
    type VoidNode =
        inherit HtmlTag


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
    type TagValue(tag: FSharpFunc<_, #HtmlElement>) =
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
        member this.render(PARTAS_CONSTRUCTOR: 'T) : 'T = jsNative

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
        static member inline (%)(left: TagValue, right: 'T) : 'T =
            left.render (right)

        [<Erase>]
        member this.render(PARTAS_PROPERTIES: obj) : RegularNode = jsNative

        [<Erase>]
        member this.render() : RegularNode = jsNative

        static member inline (%)(left: TagValue, right: obj) =
            left.render (right)

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
    let (!@) (this: FSharpFunc<_, #HtmlElement>) =
        TagValue (unbox this)

    /// Alias used in the provided builder
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    type HtmlContainerFun = HtmlContainer -> unit

    type HtmlContainer with
        [<EditorBrowsable(EditorBrowsableState.Never)>]
        member inline _.Combine
            ([<InlineIfLambda>] PARTAS_FIRST: HtmlContainerFun, [<InlineIfLambda>] PARTAS_SECOND: HtmlContainerFun)
            : HtmlContainerFun =
            fun PARTAS_BUILDER ->
                PARTAS_FIRST PARTAS_BUILDER
                PARTAS_SECOND PARTAS_BUILDER

        [<Erase>]
        [<EditorBrowsable(EditorBrowsableState.Never)>]
        member inline _.Zero() : HtmlContainerFun = ignore

        [<Erase>]
        [<EditorBrowsable(EditorBrowsableState.Never)>]
        member inline _.Delay([<InlineIfLambda>] PARTAS_DELAY: unit -> HtmlContainerFun) : HtmlContainerFun =
            PARTAS_DELAY ()

        [<Erase>]
        [<EditorBrowsable(EditorBrowsableState.Never)>]
        member inline _.Yield(PARTAS_ELEMENT: #HtmlElement) : HtmlContainerFun =
            fun PARTAS_YIELD -> ignore PARTAS_ELEMENT

        [<Erase>]
        [<EditorBrowsable(EditorBrowsableState.Never)>]
        member inline _.Yield(PARTAS_TEXT: string) : HtmlContainerFun =
            fun PARTAS_YIELD -> ignore PARTAS_TEXT

        [<Erase>]
        [<EditorBrowsable(EditorBrowsableState.Never)>]
        member inline _.Yield(PARTAS_TEXT: int) : HtmlContainerFun =
            fun PARTAS_YIELD -> ignore PARTAS_TEXT

        [<Erase>]
        [<EditorBrowsable(EditorBrowsableState.Never)>]
        member inline _.Yield(PARTAS_TEXT: float) : HtmlContainerFun =
            fun PARTAS_YIELD -> ignore PARTAS_TEXT

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    type IChildLambdaProvider =
        inherit HtmlElement

    /// <summary>
    /// Interface this with the parameter type and the type of children
    /// and their descendants to accept to allow the component to accept
    /// a lambda function passing that parameter which returns the type of children
    /// provided.
    /// </summary>
    /// <typeparam name="'Param1">Type parameter for the lambda</typeparam>
    /// <typeparam name="'Children">Type parameter for the children</typeparam>
    type ChildLambdaProviderStrict<'Param1, 'Children> =
        inherit IChildLambdaProvider

    /// <summary>
    /// Interface this with the parameter type and the type of children
    /// and their descendants to accept to allow the component to accept
    /// a lambda function passing that parameter which returns the type of children
    /// provided.
    /// </summary>
    /// <typeparam name="'Param1">Type parameter for the first parameter of the lambda</typeparam>
    /// <typeparam name="'Param2">Type parameter for the second parameter of the lambda</typeparam>
    /// <typeparam name="'Children">Type parameter for the children</typeparam>
    type ChildLambdaProviderStrict2<'Param1, 'Param2, 'Children> =
        inherit IChildLambdaProvider

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
    type ChildLambdaProviderStrict3<'Param1, 'Param2, 'Param3, 'Children> =
        inherit IChildLambdaProvider

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
    type ChildLambdaProviderStrict4<'Param1, 'Param2, 'Param3, 'Param4, 'Children> =
        inherit IChildLambdaProvider

    /// <summary>
    /// Interface this with the parameter type to allow the component to accept
    /// a lambda function passing that parameter to the children
    /// </summary>
    /// <typeparam name="'Param1">Type parameter for the first parameter of the lambda</typeparam>
    type ChildLambdaProvider<'Param1> =
        inherit IChildLambdaProvider

    /// <summary>
    /// Interface this with the parameter type to allow the component to accept
    /// a lambda function passing that parameter to the children
    /// </summary>
    /// <typeparam name="'Param1">Type parameter for the first parameter of the lambda</typeparam>
    /// <typeparam name="'Param2">Type parameter for the second parameter of the lambda</typeparam>
    type ChildLambdaProvider2<'Param1, 'Param2> =
        inherit IChildLambdaProvider

    /// <summary>
    /// Interface this with the parameter type to allow the component to accept
    /// a lambda function passing that parameter to the children
    /// </summary>
    /// <typeparam name="'Param1">Type parameter for the first parameter of the lambda</typeparam>
    /// <typeparam name="'Param2">Type parameter for the second parameter of the lambda</typeparam>
    /// <typeparam name="'Param3">Type parameter for the third parameter of the lambda</typeparam>
    type ChildLambdaProvider3<'Param1, 'Param2, 'Param3> =
        inherit IChildLambdaProvider

    /// <summary>
    /// Interface this with the parameter type to allow the component to accept
    /// a lambda function passing that parameter to the children
    /// </summary>
    /// <typeparam name="'Param1">Type parameter for the first parameter of the lambda</typeparam>
    /// <typeparam name="'Param2">Type parameter for the second parameter of the lambda</typeparam>
    /// <typeparam name="'Param3">Type parameter for the third parameter of the lambda</typeparam>
    /// <typeparam name="'Param4">Type parameter for the fourth parameter of the lambda</typeparam>
    type ChildLambdaProvider4<'Param1, 'Param2, 'Param3, 'Param4> =
        inherit IChildLambdaProvider

    /// Alias used in the provided builder
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    type ChildProviderFun = IChildLambdaProvider -> unit

    type IChildLambdaProvider with
        [<Erase>]
        [<EditorBrowsable(EditorBrowsableState.Never)>]
        member inline _.Delay([<InlineIfLambda>] PARTAS_DELAY: unit -> ChildProviderFun) : ChildProviderFun =
            PARTAS_DELAY ()

        [<Erase>]
        [<EditorBrowsable(EditorBrowsableState.Never)>]
        member inline _.Zero() : ChildProviderFun = ignore

    type ChildLambdaProviderStrict<'Param1, 'Children> with
        [<Erase>]
        [<EditorBrowsable(EditorBrowsableState.Never)>]
        member inline _.Yield(PARTAS_ELEMENT: #'Param1 -> #'Children) : ChildProviderFun =
            fun PARTAS_CONT -> ignore PARTAS_ELEMENT

    type ChildLambdaProviderStrict2<'Param1, 'Param2, 'Children> with
        [<Erase>]
        [<EditorBrowsable(EditorBrowsableState.Never)>]
        member inline _.Yield(PARTAS_ELEMENT: #'Param1 -> #'Param2 -> #'Children) : ChildProviderFun =
            fun PARTAS_CONT -> ignore PARTAS_ELEMENT

    type ChildLambdaProviderStrict3<'Param1, 'Param2, 'Param3, 'Children> with
        [<Erase>]
        [<EditorBrowsable(EditorBrowsableState.Never)>]
        member inline _.Yield(PARTAS_ELEMENT: #'Param1 -> #'Param2 -> #'Param3 -> #'Children) : ChildProviderFun =
            fun PARTAS_CONT -> ignore PARTAS_ELEMENT

    type ChildLambdaProviderStrict4<'Param1, 'Param2, 'Param3, 'Param4, 'Children> with
        [<Erase>]
        [<EditorBrowsable(EditorBrowsableState.Never)>]
        member inline _.Yield(PARTAS_ELEMENT: #'Param1 -> #'Param2 -> #'Param3 -> #'Param4 -> #'Children) : ChildProviderFun =
            fun PARTAS_CONT -> ignore PARTAS_ELEMENT

    type ChildLambdaProvider<'Param1> with
        [<Erase>]
        [<EditorBrowsable(EditorBrowsableState.Never)>]
        member inline _.Yield(PARTAS_ELEMENT: #'Param1 -> #HtmlElement) : ChildProviderFun =
            fun PARTAS_CONT -> ignore PARTAS_ELEMENT

    type ChildLambdaProvider2<'Param1, 'Param2> with
        [<Erase>]
        [<EditorBrowsable(EditorBrowsableState.Never)>]
        member inline _.Yield(PARTAS_ELEMENT: #'Param1 -> #'Param2 -> #HtmlElement) : ChildProviderFun =
            fun PARTAS_CONT -> ignore PARTAS_ELEMENT

    type ChildLambdaProvider3<'Param1, 'Param2, 'Param3> with
        [<Erase>]
        [<EditorBrowsable(EditorBrowsableState.Never)>]
        member inline _.Yield(PARTAS_ELEMENT: #'Param1 -> #'Param2 -> #'Param3 -> #HtmlElement) : ChildProviderFun =
            fun PARTAS_CONT -> ignore PARTAS_ELEMENT

    type ChildLambdaProvider4<'Param1, 'Param2, 'Param3, 'Param4> with
        [<Erase>]
        [<EditorBrowsable(EditorBrowsableState.Never)>]
        member inline _.Yield(PARTAS_ELEMENT: #'Param1 -> #'Param2 -> #'Param3 -> #'Param4 -> #HtmlElement) : ChildProviderFun =
            fun PARTAS_CONT -> ignore PARTAS_ELEMENT

    [<Erase>]
    type HtmlContainerExtensions =
        [<Extension; Erase>]
        [<EditorBrowsable(EditorBrowsableState.Never)>]
        static member Run(PARTAS_THIS: #HtmlContainer, PARTAS_RUN: HtmlContainerFun) =
            PARTAS_RUN PARTAS_THIS
            PARTAS_THIS

        [<Erase; Extension>]
        [<EditorBrowsable(EditorBrowsableState.Never)>]
        static member Run(PARTAS_THIS: #IChildLambdaProvider, PARTAS_RUN: ChildProviderFun) =
            PARTAS_RUN PARTAS_THIS
            PARTAS_THIS

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type MediaHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type AnchorHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type AudioHTMLAttributes =
        inherit MediaHTMLAttributes

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type AreaHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type BaseHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type BlockquoteHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type ButtonHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type CanvasHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type ColHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type ColgroupHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type DataHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type DetailsHtmlAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type DialogHtmlAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type EmbedHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type FieldsetHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type FormHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type IframeHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type ImgHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type InputHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type InsHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type KeygenHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type LabelHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type LiHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type LinkHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type MapHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type MenuHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type MetaHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type MeterHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type QuoteHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type ObjectHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type OlHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type OptgroupHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type OptionHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type OutputHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type ParamHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type ProgressHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type ScriptHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type SelectHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type HTMLSlotElementAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type SourceHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type StyleHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type TdHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type TemplateHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type TextareaHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type ThHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type TimeHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type TrackHTMLAttributes = interface end

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    [<AllowNullLiteral>]
    [<Interface>]
    type VideoHTMLAttributes =
        inherit MediaHTMLAttributes
