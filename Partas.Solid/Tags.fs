namespace Partas.Solid

open System.Runtime.CompilerServices
open Browser.Types
open JetBrains.Annotations
open Fable.Core
open Fable.Core.JsInterop

#nowarn 1182

[<AutoOpen>]
module Tags =
    /// Fragment (or template) node, only renders children, not itself
    [<Erase>]
    type Fragment() =
        inherit FragmentNode()

    /// Set of html extensions that keep original type
    [<Erase>]
    type HtmlElementExtensions =

        /// Add an attribute to the element
        [<Extension; Erase>]
        static member attr(this: #HtmlTag, name: string, value: obj) = this

        /// Add event handler to the element through the corresponding attribute
        [<Extension; Erase>]
        static member on(this: #HtmlTag, eventName: string, eventHandler: Event -> unit) = this

        /// Add data attribute to the element
        [<Extension; Erase>]
        static member data(this: #HtmlTag, name: string, value: string) = this

        /// Referenced native HTML element
        [<Extension; Erase>]
        static member ref(this: #HtmlTag, el: #Element) = this

        /// Referenced native HTML element (before connecting to DOM)
        [<Extension; Erase>]
        static member ref(this: #HtmlTag, el: #Element -> unit) = this

        /// Usage `elem.style(createObj ["color", "green"; "background-color", state.myColor ])`
        [<Extension; Erase>]
        static member style'(this: #HtmlTag, styleObj: obj) = this
        /// <summary>
        /// Usage <c>elem.style' [Style.backgroundColor BackgroundColor.Blue; "--my-variable" ==> BackgroundColor.Black]</c>
        /// </summary>
        [<Extension; Erase>]
        static member inline style'(this: #HtmlTag, styles: (string * obj) list) = this.style'(createObj styles)

        /// Usage `elem.classList(createObj ["active", true; "disabled", state.disabled ])`
        [<Extension; Erase>]
        static member classList(this: #HtmlTag, classListObj: obj) = this

        /// Adds or removes attribute without value
        [<Extension; Erase>]
        static member bool(this: #HtmlTag, name: string, value: bool) = this
        
        /// Spreads the passed identifier within the Tag
        [<Extension; Erase>]
        static member spread(this: #HtmlTag, value: obj) = this
        /// Directive usage
        [<Extension; Erase>]
        static member use'(this: #HtmlTag, name: string, value: obj) = this

    // ... SVG Tags
    // This should be implemented above Tags
    // so that identifiers resolve to tags first.
    // conflicting tag names will require the Svg qualifier.
    [<Erase; AutoOpen>]
    module Svg =
        [<Erase>]
        type animate() =
            interface SvgTag
            interface HtmlContainer
            interface AnimateSVGAttributes
        [<Erase>]
        type animateMotion() =
            interface SvgTag
            interface HtmlContainer
            interface AnimateMotionSVGAttributes
        [<Erase>]
        type animateTransform() =
            interface SvgTag
            interface HtmlContainer
            interface AnimateTransformSVGAttributes
        [<Erase>]
        type circle() =
            interface SvgTag
            interface HtmlContainer
            interface CircleSVGAttributes
        [<Erase>]
        type clipPath() =
            interface SvgTag
            interface HtmlContainer
            interface ClipPathSVGAttributes
        [<Erase>]
        type defs() =
            interface SvgTag
            interface HtmlContainer
            interface DefsSVGAttributes
        [<Erase>]
        type desc() =
            interface SvgTag
            interface HtmlContainer
            interface DescSVGAttributes
        [<Erase>]
        type ellipse() =
            interface SvgTag
            interface HtmlContainer
            interface EllipseSVGAttributes
        [<Erase>]
        type feBlend() =
            interface SvgTag
            interface HtmlContainer
            interface FeBlendSVGAttributes
        [<Erase>]
        type feColorMatrix() =
            interface SvgTag
            interface HtmlContainer
            interface FeColorMatrixSVGAttributes
        [<Erase>]
        type feComponentTransfer() =
            interface SvgTag
            interface HtmlContainer
            interface FeComponentTransferSVGAttributes
        [<Erase>]
        type feComposite() =
            interface SvgTag
            interface HtmlContainer
            interface FeCompositeSVGAttributes
        [<Erase>]
        type feConvolveMatrix() =
            interface SvgTag
            interface HtmlContainer
            interface FeConvolveMatrixSVGAttributes
        [<Erase>]
        type feDiffuseLighting() =
            interface SvgTag
            interface HtmlContainer
            interface FeDiffuseLightingSVGAttributes
        [<Erase>]
        type feDisplacementMap() =
            interface SvgTag
            interface HtmlContainer
            interface FeDisplacementMapSVGAttributes
        [<Erase>]
        type feDistantLight() =
            interface SvgTag
            interface HtmlContainer
            interface FeDistantLightSVGAttributes
        [<Erase>]
        type feDropShadow() =
            interface SvgTag
            interface HtmlContainer
            interface FeDropShadowSVGAttributes
        [<Erase>]
        type feFlood() =
            interface SvgTag
            interface HtmlContainer
            interface FeFloodSVGAttributes
        [<Erase>]
        type feFuncA() =
            interface SvgTag
            interface HtmlContainer
            interface FeFuncSVGAttributes
        [<Erase>]
        type feFuncB() =
            interface SvgTag
            interface HtmlContainer
            interface FeFuncSVGAttributes
        [<Erase>]
        type feFuncG() =
            interface SvgTag
            interface HtmlContainer
            interface FeFuncSVGAttributes
        [<Erase>]
        type feFuncR() =
            interface SvgTag
            interface HtmlContainer
            interface FeFuncSVGAttributes
        [<Erase>]
        type feGaussianBlur() =
            interface SvgTag
            interface HtmlContainer
            interface FeGaussianBlurSVGAttributes
        [<Erase>]
        type feImage() =
            interface SvgTag
            interface HtmlContainer
            interface FeImageSVGAttributes
        [<Erase>]
        type feMerge() =
            interface SvgTag
            interface HtmlContainer
            interface FeMergeSVGAttributes
        [<Erase>]
        type feMergeNode() =
            interface SvgTag
            interface HtmlContainer
            interface FeMergeNodeSVGAttributes
        [<Erase>]
        type feMorphology() =
            interface SvgTag
            interface HtmlContainer
            interface FeMorphologySVGAttributes
        [<Erase>]
        type feOffset() =
            interface SvgTag
            interface HtmlContainer
            interface FeOffsetSVGAttributes
        [<Erase>]
        type fePointLight() =
            interface SvgTag
            interface HtmlContainer
            interface FePointLightSVGAttributes
        [<Erase>]
        type feSpecularLighting() =
            interface SvgTag
            interface HtmlContainer
            interface FeSpecularLightingSVGAttributes
        [<Erase>]
        type feSpotLight() =
            interface SvgTag
            interface HtmlContainer
            interface FeSpotLightSVGAttributes
        [<Erase>]
        type feTile() =
            interface SvgTag
            interface HtmlContainer
            interface FeTileSVGAttributes
        [<Erase>]
        type feTurbulence() =
            interface SvgTag
            interface HtmlContainer
            interface FeTurbulanceSVGAttributes
        [<Erase>]
        type filter() =
            interface SvgTag
            interface HtmlContainer
            interface FilterSVGAttributes
        [<Erase>]
        type foreignObject() =
            interface SvgTag
            interface HtmlContainer
            interface ForeignObjectSVGAttributes
        [<Erase>]
        type g() =
            interface SvgTag
            interface HtmlContainer
            interface GSVGAttributes
        [<Erase>]
        type image() =
            interface SvgTag
            interface HtmlContainer
            interface ImageSVGAttributes
        [<Erase>]
        type line() =
            interface SvgTag
            interface HtmlContainer
            interface LineSVGAttributes
        [<Erase>]
        type linearGradient() =
            interface SvgTag
            interface HtmlContainer
            interface LinearGradientSVGAttributes
        [<Erase>]
        type marker() =
            interface SvgTag
            interface HtmlContainer
            interface MarkerSVGAttributes
        [<Erase>]
        type mask() =
            interface SvgTag
            interface HtmlContainer
            interface MaskSVGAttributes
        [<Erase>]
        type metadata() =
            interface SvgTag
            interface HtmlContainer
            interface MetadataSVGAttributes
        [<Erase>]
        type mpath() =
            interface SvgTag
            interface HtmlContainer
            interface MPathSVGAttributes
        [<Erase>]
        type path() =
            interface SvgTag
            interface HtmlContainer
            interface PathSVGAttributes
        [<Erase>]
        type pattern() =
            interface SvgTag
            interface HtmlContainer
            interface PatternSVGAttributes
        [<Erase>]
        type polygon() =
            interface SvgTag
            interface HtmlContainer
            interface PolygonSVGAttributes
        [<Erase>]
        type polyline() =
            interface SvgTag
            interface HtmlContainer
            interface PolylineSVGAttributes
        [<Erase>]
        type radialGradient() =
            interface SvgTag
            interface HtmlContainer
            interface RadialGradientSVGAttributes
        [<Erase>]
        type rect() =
            interface SvgTag
            interface HtmlContainer
            interface RectSVGAttributes
        [<Erase>]
        type set() =
            interface SvgTag
            interface HtmlContainer
            interface SetSVGAttributes
        [<Erase>]
        type stop() =
            interface SvgTag
            interface HtmlContainer
            interface StopSVGAttributes
        [<Erase>]
        type svg() =
            interface SvgTag
            interface HtmlContainer
            interface SvgSVGAttributes
        [<Erase>]
        type switch() =
            interface SvgTag
            interface HtmlContainer
            interface SwitchSVGAttributes
        [<Erase>]
        type symbol() =
            interface SvgTag
            interface HtmlContainer
            interface SymbolSVGAttributes
        [<Erase>]
        type text() =
            interface SvgTag
            interface HtmlContainer
            interface TextSVGAttributes
        [<Erase>]
        type textPath() =
            interface SvgTag
            interface HtmlContainer
            interface TextPathSVGAttributes
        [<Erase>]
        type tspan() =
            interface SvgTag
            interface HtmlContainer
            interface TSpanSVGAttributes
        [<Erase>]
        type use'() =
            interface SvgTag
            interface HtmlContainer
            interface UseSVGAttributes
        [<Erase>]
        type view() =
            interface SvgTag
            interface HtmlContainer
            interface ViewSVGAttributes

    [<Erase>]
    type a() =
        inherit RegularNode()
        interface AnchorHTMLAttributes
        member inline this.asDomElement: HTMLAnchorElement = unbox this
    [<Erase>]
    type abbr() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type address() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type area() =
        inherit RegularNode()
        interface AreaHTMLAttributes
        member inline this.asDomElement: HTMLAreaElement = unbox this
    [<Erase>]
    type article() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type aside() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type audio() =
        inherit RegularNode()
        interface AudioHTMLAttributes
        member inline this.asDomElement: HTMLAudioElement = unbox this
    [<Erase>]
    type b() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type base'() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLBaseElement = unbox this
    [<Erase>]
    type bdi() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type bdo() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type blockquote() =
        inherit RegularNode()
        interface BlockquoteHTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type body() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLBodyElement = unbox this
    [<Erase>]
    type br() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLBRElement = unbox this
    [<Erase>]
    type button() =
        inherit RegularNode()
        interface ButtonHTMLAttributes
        member inline this.asDomElement: HTMLButtonElement = unbox this
    [<Erase>]
    type canvas() =
        inherit RegularNode()
        interface CanvasHTMLAttributes
        member inline this.asDomElement: HTMLCanvasElement = unbox this
    [<Erase>]
    type caption() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type cite() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type code() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type col() =
        inherit RegularNode()
        interface ColHTMLAttributes
        member inline this.asDomElement: HTMLTableColElement = unbox this
    [<Erase>]
    type colgroup() =
        inherit RegularNode()
        interface ColgroupHTMLAttributes
        member inline this.asDomElement: HTMLTableColElement = unbox this
    [<Erase>]
    type data() =
        inherit RegularNode()
        interface DataHTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type datalist() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLDataListElement = unbox this
    [<Erase>]
    type dd() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type del() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type details() =
        inherit RegularNode()
        interface DetailsHtmlAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type dfn() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type dialog() =
        inherit RegularNode()
        interface DialogHtmlAttributes
        member inline this.asDomElement: HTMLDialogElement = unbox this
    [<Erase>]
    type div() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLDivElement = unbox this
    [<Erase>]
    type dl() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLDListElement = unbox this
    [<Erase>]
    type dt() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type em() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type embed() =
        inherit RegularNode()
        interface EmbedHTMLAttributes
        member inline this.asDomElement: HTMLEmbedElement = unbox this
    [<Erase>]
    type fieldset() =
        inherit RegularNode()
        interface FieldsetHTMLAttributes
        member inline this.asDomElement: HTMLFieldSetElement = unbox this
    [<Erase>]
    type figcaption() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type figure() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type footer() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type form() =
        inherit RegularNode()
        interface FormHTMLAttributes
        member inline this.asDomElement: HTMLFormElement = unbox this
    [<Erase>]
    type h1() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLHeadingElement = unbox this
    [<Erase>]
    type h2() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLHeadingElement = unbox this
    [<Erase>]
    type h3() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLHeadingElement = unbox this
    [<Erase>]
    type h4() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLHeadingElement = unbox this
    [<Erase>]
    type h5() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLHeadingElement = unbox this
    [<Erase>]
    type h6() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLHeadingElement = unbox this
    [<Erase>]
    type head() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLHeadElement = unbox this
    [<Erase>]
    type header() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type hgroup() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type hr() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLHRElement = unbox this
    [<Erase>]
    type html() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLHtmlElement = unbox this
    [<Erase>]
    type i() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type iframe() =
        inherit RegularNode()
        interface IframeHTMLAttributes
        member inline this.asDomElement: HTMLIFrameElement = unbox this
    [<Erase>]
    type img() =
        inherit RegularNode()
        interface ImgHTMLAttributes
        member inline this.asDomElement: HTMLImageElement = unbox this
    [<Erase>]
    type input() =
        inherit RegularNode()
        interface InputHTMLAttributes
        member inline this.asDomElement: HTMLInputElement = unbox this
    [<Erase>]
    type ins() =
        inherit RegularNode()
        interface InsHTMLAttributes
        member inline this.asDomElement: HTMLModElement = unbox this
    [<Erase>]
    type kbd() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type label() =
        inherit RegularNode()
        interface LabelHTMLAttributes
        member inline this.asDomElement: HTMLLabelElement = unbox this
    [<Erase>]
    type legend() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLLegendElement = unbox this
    [<Erase>]
    type li() =
        inherit RegularNode()
        interface LiHTMLAttributes
        member inline this.asDomElement: HTMLLIElement = unbox this
    [<Erase>]
    type link() =
        inherit RegularNode()
        interface LinkHTMLAttributes
        member inline this.asDomElement: HTMLLinkElement = unbox this
    [<Erase>]
    type main() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type map() =
        inherit RegularNode()
        interface MapHTMLAttributes
        member inline this.asDomElement: HTMLMapElement = unbox this
    [<Erase>]
    type mark() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type menu() =
        inherit RegularNode()
        interface MenuHTMLAttributes
        member inline this.asDomElement: HTMLMenuElement = unbox this
    [<Erase>]
    type meta() =
        inherit RegularNode()
        interface MetaHTMLAttributes
        member inline this.asDomElement: HTMLMetaElement = unbox this
    [<Erase>]
    type meter() =
        inherit RegularNode()
        interface MeterHTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type nav() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type noscript() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type object'() =
        inherit RegularNode()
        interface ObjectHTMLAttributes
        member inline this.asDomElement: HTMLObjectElement = unbox this
    [<Erase>]
    type ol() =
        inherit RegularNode()
        interface OlHTMLAttributes
        member inline this.asDomElement: HTMLOListElement = unbox this
    [<Erase>]
    type optgroup() =
        inherit RegularNode()
        interface OptgroupHTMLAttributes
        member inline this.asDomElement: HTMLOptGroupElement = unbox this
    [<Erase>]
    type option'() =
        inherit RegularNode()
        interface OptionHTMLAttributes
        member inline this.asDomElement: HTMLOptionElement = unbox this
    [<Erase>]
    type output() =
        inherit RegularNode()
        interface OutputHTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type p() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLParagraphElement = unbox this
    [<Erase>]
    type picture() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type pre() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLPreElement = unbox this
    [<Erase>]
    type progress() =
        inherit RegularNode()
        interface ProgressHTMLAttributes
        member inline this.asDomElement: HTMLProgressElement = unbox this
    [<Erase>]
    type q() =
        inherit RegularNode()
        interface QuoteHTMLAttributes
        member inline this.asDomElement: HTMLQuoteElement = unbox this
    [<Erase>]
    type rp() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type rt() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type ruby() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type s() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type samp() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type script() =
        inherit RegularNode()
        interface ScriptHTMLAttributes
        member inline this.asDomElement: HTMLScriptElement = unbox this
    [<Erase>]
    type search() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type section() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type select() =
        inherit RegularNode()
        interface SelectHTMLAttributes
        member inline this.asDomElement: HTMLSelectElement = unbox this
    [<Erase>]
    type small() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type source() =
        inherit RegularNode()
        interface SourceHTMLAttributes
        member inline this.asDomElement: HTMLSourceElement = unbox this
    [<Erase>]
    type span() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLSpanElement = unbox this
    [<Erase>]
    type strong() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type style() =
        inherit RegularNode()
        interface StyleHTMLAttributes
        member inline this.asDomElement: HTMLStyleElement = unbox this
    [<Erase>]
    type sub() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type summary() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type sup() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type table() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLTableElement = unbox this
    [<Erase>]
    type tbody() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLTableSectionElement = unbox this
    [<Erase>]
    type td() =
        inherit RegularNode()
        interface TdHTMLAttributes
        member inline this.asDomElement: HTMLTableCellElement = unbox this
    [<Erase>]
    type textarea() =
        inherit RegularNode()
        interface TextareaHTMLAttributes
        member inline this.asDomElement: HTMLTextAreaElement = unbox this
    [<Erase>]
    type tfoot() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLTableSectionElement = unbox this
    [<Erase>]
    type th() =
        inherit RegularNode()
        interface ThHTMLAttributes
        member inline this.asDomElement: HTMLTableCellElement = unbox this
    [<Erase>]
    type thead() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLTableSectionElement = unbox this
    [<Erase>]
    type time() =
        inherit RegularNode()
        interface TimeHTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type title() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLTitleElement = unbox this
    [<Erase>]
    type tr() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLTableRowElement = unbox this
    [<Erase>]
    type track() =
        inherit RegularNode()
        interface TrackHTMLAttributes
        member inline this.asDomElement: HTMLTrackElement = unbox this
    [<Erase>]
    type u() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type ul() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLUListElement = unbox this
    [<Erase>]
    type var() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
    [<Erase>]
    type video() =
        inherit RegularNode()
        interface VideoHTMLAttributes
        member inline this.asDomElement: HTMLVideoElement = unbox this
    [<Erase>]
    type wbr() =
        inherit RegularNode()
        interface HTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this
