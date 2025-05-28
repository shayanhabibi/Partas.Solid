namespace Partas.Solid

open System.Runtime.CompilerServices
open Fable.Core

#nowarn 49
#nowarn 64

[<AutoOpen>]
module Builder =
    [<AllowNullLiteral>]
    [<Interface>]
    type HTMLAttributes = interface end
    [<AllowNullLiteral; Interface>]
    type HtmlElement = interface end
    [<AllowNullLiteral; Interface>]
    type HtmlTag =
        inherit HTMLAttributes
        inherit HtmlElement
    [<AllowNullLiteral; Interface>]
    type HtmlContainer = inherit HtmlElement
    [<AllowNullLiteral; Interface>]
    type SvgTag = inherit HtmlElement
    
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

    [<AllowNullLiteral>]
    [<Interface>]
    type MediaHTMLAttributes =
        inherit HTMLAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type AnchorHTMLAttributes =
        inherit HTMLAttributes
        
    [<AllowNullLiteral>]
    [<Interface>]
    type AudioHTMLAttributes =
        inherit MediaHTMLAttributes
        
    [<AllowNullLiteral>]
    [<Interface>]
    type AreaHTMLAttributes =
        inherit HTMLAttributes
        
    [<AllowNullLiteral>]
    [<Interface>]
    type BaseHTMLAttributes =
        inherit HTMLAttributes
        
    [<AllowNullLiteral>]
    [<Interface>]
    type BlockquoteHTMLAttributes =
        inherit HTMLAttributes
        
    [<AllowNullLiteral>]
    [<Interface>]
    type ButtonHTMLAttributes =
        inherit HTMLAttributes
        
    [<AllowNullLiteral>]
    [<Interface>]
    type CanvasHTMLAttributes =
        inherit HTMLAttributes
        
    [<AllowNullLiteral>]
    [<Interface>]
    type ColHTMLAttributes =
        inherit HTMLAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type ColgroupHTMLAttributes =
        inherit HTMLAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type DataHTMLAttributes =
        inherit HTMLAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type DetailsHtmlAttributes =
        inherit HTMLAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type DialogHtmlAttributes =
        inherit HTMLAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type EmbedHTMLAttributes =
        inherit HTMLAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type FieldsetHTMLAttributes =
        inherit HTMLAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type FormHTMLAttributes =
        inherit HTMLAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type IframeHTMLAttributes =
        inherit HTMLAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type ImgHTMLAttributes =
        inherit HTMLAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type InputHTMLAttributes =
        inherit HTMLAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type InsHTMLAttributes =
        inherit HTMLAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type KeygenHTMLAttributes =
        inherit HTMLAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type LabelHTMLAttributes =
        inherit HTMLAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type LiHTMLAttributes =
        inherit HTMLAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type LinkHTMLAttributes =
        inherit HTMLAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type MapHTMLAttributes =
        inherit HTMLAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type MenuHTMLAttributes =
        inherit HTMLAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type MetaHTMLAttributes =
        inherit HTMLAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type MeterHTMLAttributes =
        inherit HTMLAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type QuoteHTMLAttributes =
        inherit HTMLAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type ObjectHTMLAttributes =
        inherit HTMLAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type OlHTMLAttributes =
        inherit HTMLAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type OptgroupHTMLAttributes =
        inherit HTMLAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type OptionHTMLAttributes =
        inherit HTMLAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type OutputHTMLAttributes =
        inherit HTMLAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type ParamHTMLAttributes =
        inherit HTMLAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type ProgressHTMLAttributes =
        inherit HTMLAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type ScriptHTMLAttributes =
        inherit HTMLAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type SelectHTMLAttributes =
        inherit HTMLAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type HTMLSlotElementAttributes =
        inherit HTMLAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type SourceHTMLAttributes =
        inherit HTMLAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type StyleHTMLAttributes =
        inherit HTMLAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type TdHTMLAttributes =
        inherit HTMLAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type TemplateHTMLAttributes =
        inherit HTMLAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type TextareaHTMLAttributes =
        inherit HTMLAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type ThHTMLAttributes =
        inherit HTMLAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type TimeHTMLAttributes =
        inherit HTMLAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type TrackHTMLAttributes =
        inherit HTMLAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type VideoHTMLAttributes =
        inherit MediaHTMLAttributes
        
    [<AllowNullLiteral>]
    [<Interface>]
    type CoreSVGAttributes = interface end

    [<AllowNullLiteral>]
    [<Interface>]
    type ExternalResourceSVGAttributes = interface end

    [<AllowNullLiteral>]
    [<Interface>]
    type ConditionalProcessingSVGAttributes = interface end
    [<AllowNullLiteral>]
    [<Interface>]
    type AnimationElementSVGAttributes =
        inherit CoreSVGAttributes
        inherit ExternalResourceSVGAttributes
        inherit ConditionalProcessingSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type ShapeElementSVGAttributes =
        inherit CoreSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type ContainerElementSVGAttributes =
        inherit CoreSVGAttributes
        inherit ShapeElementSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type FilterPrimitiveElementSVGAttributes =
        inherit CoreSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type StylableSVGAttributes = interface end

    [<AllowNullLiteral>]
    [<Interface>]
    type TransformableSVGAttributes = interface end



    [<AllowNullLiteral>]
    [<Interface>]
    type AnimationTimingSVGAttributes = interface end

    [<AllowNullLiteral>]
    [<Interface>]
    type AnimationValueSVGAttributes = interface end

    [<AllowNullLiteral>]
    [<Interface>]
    type AnimationAdditionSVGAttributes = interface end

    [<AllowNullLiteral>]
    [<Interface>]
    type AnimationAttributeTargetSVGAttributes = interface end

    [<AllowNullLiteral>]
    [<Interface>]
    type PresentationSVGAttributes = interface end

    [<AllowNullLiteral>]
    [<Interface>]
    type SingleInputFilterSVGAttributes = interface end

    [<AllowNullLiteral>]
    [<Interface>]
    type DoubleInputFilterSVGAttributes = interface end

    [<AllowNullLiteral>]
    [<Interface>]
    type FitToViewBoxSVGAttributes = interface end

    [<AllowNullLiteral>]
    [<Interface>]
    type GradientElementSVGAttributes =
        inherit CoreSVGAttributes
        inherit ExternalResourceSVGAttributes
        inherit StylableSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type GraphicsElementSVGAttributes =
        inherit CoreSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type LightSourceElementSVGAttributes =
        inherit CoreSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type NewViewportSVGAttributes =
        inherit CoreSVGAttributes



    [<AllowNullLiteral>]
    [<Interface>]
    type TextContentElementSVGAttributes =
        inherit CoreSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type ZoomAndPanSVGAttributes = interface end

    [<AllowNullLiteral>]
    [<Interface>]
    type AnimateSVGAttributes =
        inherit AnimationElementSVGAttributes
        inherit AnimationAttributeTargetSVGAttributes
        inherit AnimationTimingSVGAttributes
        inherit AnimationValueSVGAttributes
        inherit AnimationAdditionSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type AnimateMotionSVGAttributes =
        inherit AnimationElementSVGAttributes
        inherit AnimationTimingSVGAttributes
        inherit AnimationValueSVGAttributes
        inherit AnimationAdditionSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type AnimateTransformSVGAttributes =
        inherit AnimationElementSVGAttributes
        inherit AnimationAttributeTargetSVGAttributes
        inherit AnimationTimingSVGAttributes
        inherit AnimationValueSVGAttributes
        inherit AnimationAdditionSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type CircleSVGAttributes =
        inherit GraphicsElementSVGAttributes
        inherit ShapeElementSVGAttributes
        inherit ConditionalProcessingSVGAttributes
        inherit StylableSVGAttributes
        inherit TransformableSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type ClipPathSVGAttributes =
        inherit CoreSVGAttributes
        inherit ConditionalProcessingSVGAttributes
        inherit ExternalResourceSVGAttributes
        inherit StylableSVGAttributes
        inherit TransformableSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type DefsSVGAttributes =
        inherit ContainerElementSVGAttributes
        inherit ConditionalProcessingSVGAttributes
        inherit ExternalResourceSVGAttributes
        inherit StylableSVGAttributes
        inherit TransformableSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type DescSVGAttributes =
        inherit CoreSVGAttributes
        inherit StylableSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type EllipseSVGAttributes =
        inherit GraphicsElementSVGAttributes
        inherit ShapeElementSVGAttributes
        inherit ConditionalProcessingSVGAttributes
        inherit ExternalResourceSVGAttributes
        inherit StylableSVGAttributes
        inherit TransformableSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type FeBlendSVGAttributes =
        inherit FilterPrimitiveElementSVGAttributes
        inherit DoubleInputFilterSVGAttributes
        inherit StylableSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type FeColorMatrixSVGAttributes =
        inherit FilterPrimitiveElementSVGAttributes
        inherit SingleInputFilterSVGAttributes
        inherit StylableSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type FeComponentTransferSVGAttributes =
        inherit FilterPrimitiveElementSVGAttributes
        inherit SingleInputFilterSVGAttributes
        inherit StylableSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type FeCompositeSVGAttributes =
        inherit FilterPrimitiveElementSVGAttributes
        inherit DoubleInputFilterSVGAttributes
        inherit StylableSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type FeConvolveMatrixSVGAttributes =
        inherit FilterPrimitiveElementSVGAttributes
        inherit SingleInputFilterSVGAttributes
        inherit StylableSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type FeDiffuseLightingSVGAttributes =
        inherit FilterPrimitiveElementSVGAttributes
        inherit SingleInputFilterSVGAttributes
        inherit StylableSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type FeDisplacementMapSVGAttributes =
        inherit FilterPrimitiveElementSVGAttributes
        inherit DoubleInputFilterSVGAttributes
        inherit StylableSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type FeDistantLightSVGAttributes =
        inherit LightSourceElementSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type FeDropShadowSVGAttributes =
        inherit CoreSVGAttributes
        inherit FilterPrimitiveElementSVGAttributes
        inherit StylableSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type FeFloodSVGAttributes =
        inherit FilterPrimitiveElementSVGAttributes
        inherit StylableSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type FeFuncSVGAttributes =
        inherit CoreSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type FeGaussianBlurSVGAttributes =
        inherit FilterPrimitiveElementSVGAttributes
        inherit SingleInputFilterSVGAttributes
        inherit StylableSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type FeImageSVGAttributes =
        inherit FilterPrimitiveElementSVGAttributes
        inherit ExternalResourceSVGAttributes
        inherit StylableSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type FeMergeSVGAttributes =
        inherit FilterPrimitiveElementSVGAttributes
        inherit StylableSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type FeMergeNodeSVGAttributes =
        inherit CoreSVGAttributes
        inherit SingleInputFilterSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type FeMorphologySVGAttributes =
        inherit FilterPrimitiveElementSVGAttributes
        inherit SingleInputFilterSVGAttributes
        inherit StylableSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type FeOffsetSVGAttributes =
        inherit FilterPrimitiveElementSVGAttributes
        inherit SingleInputFilterSVGAttributes
        inherit StylableSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type FePointLightSVGAttributes =
        inherit LightSourceElementSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type FeSpecularLightingSVGAttributes =
        inherit FilterPrimitiveElementSVGAttributes
        inherit SingleInputFilterSVGAttributes
        inherit StylableSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type FeSpotLightSVGAttributes =
        inherit LightSourceElementSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type FeTileSVGAttributes =
        inherit FilterPrimitiveElementSVGAttributes
        inherit SingleInputFilterSVGAttributes
        inherit StylableSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type FeTurbulanceSVGAttributes =
        inherit FilterPrimitiveElementSVGAttributes
        inherit StylableSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type FilterSVGAttributes =
        inherit CoreSVGAttributes
        inherit ExternalResourceSVGAttributes
        inherit StylableSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type ForeignObjectSVGAttributes =
        inherit NewViewportSVGAttributes
        inherit ConditionalProcessingSVGAttributes
        inherit ExternalResourceSVGAttributes
        inherit StylableSVGAttributes
        inherit TransformableSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type GSVGAttributes =
        inherit ContainerElementSVGAttributes
        inherit ConditionalProcessingSVGAttributes
        inherit ExternalResourceSVGAttributes
        inherit StylableSVGAttributes
        inherit TransformableSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type ImageSVGAttributes =
        inherit NewViewportSVGAttributes
        inherit GraphicsElementSVGAttributes
        inherit ConditionalProcessingSVGAttributes
        inherit StylableSVGAttributes
        inherit TransformableSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type LineSVGAttributes =
        inherit GraphicsElementSVGAttributes
        inherit ShapeElementSVGAttributes
        inherit ConditionalProcessingSVGAttributes
        inherit ExternalResourceSVGAttributes
        inherit StylableSVGAttributes
        inherit TransformableSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type LinearGradientSVGAttributes =
        inherit GradientElementSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type MarkerSVGAttributes =
        inherit ContainerElementSVGAttributes
        inherit ExternalResourceSVGAttributes
        inherit StylableSVGAttributes
        inherit FitToViewBoxSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type MaskSVGAttributes =
        inherit ConditionalProcessingSVGAttributes
        inherit ExternalResourceSVGAttributes
        inherit StylableSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type MetadataSVGAttributes =
        inherit CoreSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type MPathSVGAttributes =
        inherit CoreSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type PathSVGAttributes =
        inherit GraphicsElementSVGAttributes
        inherit ShapeElementSVGAttributes
        inherit ConditionalProcessingSVGAttributes
        inherit ExternalResourceSVGAttributes
        inherit StylableSVGAttributes
        inherit TransformableSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type PatternSVGAttributes =
        inherit ContainerElementSVGAttributes
        inherit ConditionalProcessingSVGAttributes
        inherit ExternalResourceSVGAttributes
        inherit StylableSVGAttributes
        inherit FitToViewBoxSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type PolygonSVGAttributes =
        inherit GraphicsElementSVGAttributes
        inherit ShapeElementSVGAttributes
        inherit ConditionalProcessingSVGAttributes
        inherit ExternalResourceSVGAttributes
        inherit StylableSVGAttributes
        inherit TransformableSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type PolylineSVGAttributes =
        inherit GraphicsElementSVGAttributes
        inherit ShapeElementSVGAttributes
        inherit ConditionalProcessingSVGAttributes
        inherit ExternalResourceSVGAttributes
        inherit StylableSVGAttributes
        inherit TransformableSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type RadialGradientSVGAttributes =
        inherit GradientElementSVGAttributes
    [<AllowNullLiteral>]
    [<Interface>]
    type RectSVGAttributes =
        inherit GraphicsElementSVGAttributes
        inherit ShapeElementSVGAttributes
        inherit ConditionalProcessingSVGAttributes
        inherit ExternalResourceSVGAttributes
        inherit StylableSVGAttributes
        inherit TransformableSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type SetSVGAttributes =
        inherit CoreSVGAttributes
        inherit StylableSVGAttributes
        inherit AnimationTimingSVGAttributes

    [<AllowNullLiteral>]
    [<Interface>]
    type StopSVGAttributes =
        inherit CoreSVGAttributes
        inherit StylableSVGAttributes
    [<AllowNullLiteral>]
    [<Interface>]
    type SvgSVGAttributes =
        inherit ContainerElementSVGAttributes
        inherit NewViewportSVGAttributes
        inherit ConditionalProcessingSVGAttributes
        inherit ExternalResourceSVGAttributes
        inherit StylableSVGAttributes
        inherit FitToViewBoxSVGAttributes
        inherit ZoomAndPanSVGAttributes
        inherit PresentationSVGAttributes
    [<AllowNullLiteral>]
    [<Interface>]
    type SwitchSVGAttributes =
        inherit ContainerElementSVGAttributes
        inherit ConditionalProcessingSVGAttributes
        inherit ExternalResourceSVGAttributes
        inherit StylableSVGAttributes
        inherit TransformableSVGAttributes
    [<AllowNullLiteral>]
    [<Interface>]
    type SymbolSVGAttributes =
        inherit ContainerElementSVGAttributes
        inherit NewViewportSVGAttributes
        inherit ExternalResourceSVGAttributes
        inherit StylableSVGAttributes
        inherit FitToViewBoxSVGAttributes
    [<AllowNullLiteral>]
    [<Interface>]
    type TextSVGAttributes =
        inherit TextContentElementSVGAttributes
        inherit GraphicsElementSVGAttributes
        inherit ConditionalProcessingSVGAttributes
        inherit ExternalResourceSVGAttributes
        inherit StylableSVGAttributes
        inherit TransformableSVGAttributes
    [<AllowNullLiteral>]
    [<Interface>]
    type TextPathSVGAttributes =
        inherit TextContentElementSVGAttributes
        inherit ConditionalProcessingSVGAttributes
        inherit ExternalResourceSVGAttributes
        inherit StylableSVGAttributes
    [<AllowNullLiteral>]
    [<Interface>]
    type TSpanSVGAttributes =
        inherit TextContentElementSVGAttributes
        inherit ConditionalProcessingSVGAttributes
        inherit ExternalResourceSVGAttributes
        inherit StylableSVGAttributes
    [<AllowNullLiteral>]
    [<Interface>]
    type UseSVGAttributes =
        inherit CoreSVGAttributes
        inherit StylableSVGAttributes
        inherit ConditionalProcessingSVGAttributes
        inherit GraphicsElementSVGAttributes
        inherit PresentationSVGAttributes
        inherit ExternalResourceSVGAttributes
        inherit TransformableSVGAttributes
    [<AllowNullLiteral>]
    [<Interface>]
    type ViewSVGAttributes =
        inherit CoreSVGAttributes
        inherit ExternalResourceSVGAttributes
        inherit FitToViewBoxSVGAttributes
        inherit ZoomAndPanSVGAttributes
