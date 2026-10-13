namespace Partas.Solid

open System.Runtime.CompilerServices
open Browser.Types
open Fable.Core
open Fable.Core.JsInterop

#nowarn 1182

[<AutoOpen>]
module SpecialAttributes =
    /// <summary>
    /// An additional special syntax that allows full control of capture, passive, once and signal is an intersection or combination of
    /// <c>EventListenerObject</c> &amp; <c>AddEventListenerOptions</c>
    /// </summary>
    [<JS.Pojo>]
    type OnHandler(handleEvent: Event -> unit, ?once: bool, ?passive: bool, ?capture: bool) =
        member val handleEvent: (Event -> unit) = JS.undefined with get, set
        member val once: bool = JS.undefined with get, set
        member val passive: bool = JS.undefined with get, set
        member val capture: bool = JS.undefined with get, set

module Decorators =
    [<Emit("/*@once*/")>]
    let once: unit = jsNative


[<AutoOpen>]
module Tags =
    [<EB(EBState.Never)>]
    type DomType<^Tag, ^DomType when ^Tag:(member asDomElement: ^DomType)> = ^Tag
    [<EB(EBState.Never)>]
    type IntrinsicDomType<^Tag, ^DomType when ^DomType :> HTMLElement and DomType<^Tag, ^DomType> and ^Tag :> IntrinsicDOMElement> = ^Tag

    [<AutoOpen>]
    type Extensions =
        [<Erase>] static member inline asDomElement (comp: ^A): ^B when IntrinsicDomType<^A, ^B> = (^A : (member asDomElement: ^B) comp)

    [<Erase; RequireQualifiedAccess>]
    type Ref<^DomType> =
        | Singleton of ^DomType option
        | Callback of (^DomType -> unit)
        | Array of Ref<^DomType> array
    module Ref =
        let inline cast (ref: Ref<^DomType> when ^DomType :> #HTMLElement): Ref<#HTMLElement> = unbox ref

    /// Fragment (or template) node, only renders children, not itself
    [<Erase>]
    type Fragment() =
        interface FragmentNode

    /// Set of html extensions that keep original type
    [<Erase>]
    type HtmlElementExtensions =
        /// Add an attribute to the element
        [<Extension; Erase>]
        static member attr(this: #HtmlTag, name: string, value: obj) = this

        /// Add data attribute to the element
        [<Extension; Erase>]
        static member data(this: #HtmlTag, name: string, value: string) = this

        [<Extension; Erase; CompiledName("ref"); EB(EBState.Never)>]
        static member _refSRTPImplementation(this: #IntrinsicDOMElement, el: obj): #IntrinsicDOMElement = this
        [<Extension; Erase>]
        static member ref(this: #RefAttributeExtension, el: HTMLElement): #RefAttributeExtension = this
        [<Extension; Erase>]
        static member ref(this: #RefAttributeExtension, el: HTMLElement option): #RefAttributeExtension = this
        [<Extension; Erase>]
        static member ref(this: #RefAttributeExtension, el: HTMLElement -> unit): #RefAttributeExtension = this
        [<Extension; Erase>]
        static member ref(this: #RefAttributeExtension, el: (HTMLElement -> unit) array): #RefAttributeExtension = this
        [<Extension; Erase>]
        static member ref(this: #RefAttributeExtension, el: Ref<HTMLElement>): #RefAttributeExtension = this

        /// <remarks>
        /// If you want to create a mutable ref to an element that captures any HTMLElement:
        /// <code>
        /// let mutable myRef: HTMLElement = JS.undefined
        /// div().ref(myRef :?> _) // works; will upcast to HTMLDivElement
        /// div().ref(myRef) // compiler error - needs HTMLDivElement
        /// </code>
        /// </remarks>
        [<Extension; Erase>]
        static member inline ref(this: ^T, el: ^U when IntrinsicDomType<^T, ^U>): ^T = this._refSRTPImplementation(el)
        [<Extension; Erase>]
        static member inline ref(this: ^T, el: ^U option when IntrinsicDomType<^T, ^U>): ^T = this._refSRTPImplementation(el)
        [<Extension; Erase>]
        static member inline ref(this: ^T, [<InlineIfLambda>] el: ^U -> unit when IntrinsicDomType<^T, ^U>): ^T = this._refSRTPImplementation(el)
        [<Extension; Erase>]
        static member inline ref(this: ^T, el: Ref<^U> when IntrinsicDomType<^T, ^U>): ^T = this._refSRTPImplementation(el)
        [<Extension; Erase>]
        static member inline ref(this: ^T, el: Ref<^U>[] when IntrinsicDomType<^T, ^U>): ^T = this._refSRTPImplementation(el)


        // /// Referenced native HTML element (before connecting to DOM)
        // [<Extension; Erase>]
        // static member ref(this: #RefAttributeExtension, el: #Element -> unit) = this

        /// Usage `elem.style(createObj ["color", "green"; "background-color", state.myColor ])`
        [<Extension; Erase>]
        static member style'(this: #HtmlTag, styleObj: obj) = this

        /// <summary>
        /// Usage <c>elem.style' [Style.backgroundColor BackgroundColor.Blue; "--my-variable" ==> BackgroundColor.Black]</c>
        /// </summary>
        [<Extension; Erase>]
        static member inline style'(this: #HtmlTag, styles: (string * obj) list) =
            this.style' (createObj styles)

        /// Usage `elem.classList(createObj ["active", true; "disabled", state.disabled ])`
        [<Extension; Erase>]
        static member class'(this: #HtmlTag, classListObj: obj) = this

        /// Adds or removes attribute without value
        [<Extension; Erase>]
        static member bool(this: #HtmlTag, name: string, value: bool) = this

        /// Spreads the passed identifier within the Tag
        [<Extension; Erase>]
        static member spread(this: #HtmlElement, value: obj) = this

    [<Erase>]
    type a() =
        interface IntrinsicParentNode
        interface AnchorHTMLAttributes
        member inline this.asDomElement: HTMLAnchorElement = unbox this

    [<Erase>]
    type abbr() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type address() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type area() =
        interface IntrinsicVoidNode
        interface AreaHTMLAttributes
        member inline this.asDomElement: HTMLAreaElement = unbox this

    [<Erase>]
    type article() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type aside() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type audio() =
        interface IntrinsicParentNode
        interface AudioHTMLAttributes
        member inline this.asDomElement: HTMLAudioElement = unbox this

    [<Erase>]
    type b() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type base'() =
        interface IntrinsicVoidNode
        member inline this.asDomElement: HTMLBaseElement = unbox this

    [<Erase>]
    type bdi() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type bdo() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type blockquote() =
        interface IntrinsicParentNode
        interface BlockquoteHTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type body() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLBodyElement = unbox this

    [<Erase>]
    type br() =
        interface IntrinsicVoidNode
        member inline this.asDomElement: HTMLBRElement = unbox this

    [<Erase>]
    type button() =
        interface IntrinsicParentNode
        interface ButtonHTMLAttributes
        member inline this.asDomElement: HTMLButtonElement = unbox this

    [<Erase>]
    type canvas() =
        interface IntrinsicParentNode
        interface CanvasHTMLAttributes
        member inline this.asDomElement: HTMLCanvasElement = unbox this

    [<Erase>]
    type caption() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type cite() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type code() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type col() =
        interface IntrinsicVoidNode
        interface ColHTMLAttributes
        member inline this.asDomElement: HTMLTableColElement = unbox this

    [<Erase>]
    type colgroup() =
        interface IntrinsicParentNode
        interface ColgroupHTMLAttributes
        member inline this.asDomElement: HTMLTableColElement = unbox this

    [<Erase>]
    type data() =
        interface IntrinsicParentNode
        interface DataHTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type datalist() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLDataListElement = unbox this

    [<Erase>]
    type dd() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type del() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type details() =
        interface IntrinsicParentNode
        interface DetailsHtmlAttributes
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type dfn() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type dialog() =
        interface IntrinsicParentNode
        interface DialogHtmlAttributes
        member inline this.asDomElement: HTMLDialogElement = unbox this

    [<Erase>]
    type div() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLDivElement = unbox this

    [<Erase>]
    type dl() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLDListElement = unbox this

    [<Erase>]
    type dt() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type em() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type embed() =
        interface IntrinsicVoidNode
        interface EmbedHTMLAttributes
        member inline this.asDomElement: HTMLEmbedElement = unbox this

    [<Erase>]
    type fieldset() =
        interface IntrinsicParentNode
        interface FieldsetHTMLAttributes
        member inline this.asDomElement: HTMLFieldSetElement = unbox this

    [<Erase>]
    type figcaption() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type figure() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type footer() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type form() =
        interface IntrinsicParentNode
        interface FormHTMLAttributes
        member inline this.asDomElement: HTMLFormElement = unbox this

    [<Erase>]
    type h1() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLHeadingElement = unbox this

    [<Erase>]
    type h2() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLHeadingElement = unbox this

    [<Erase>]
    type h3() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLHeadingElement = unbox this

    [<Erase>]
    type h4() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLHeadingElement = unbox this

    [<Erase>]
    type h5() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLHeadingElement = unbox this

    [<Erase>]
    type h6() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLHeadingElement = unbox this

    [<Erase>]
    type head() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLHeadElement = unbox this

    [<Erase>]
    type header() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type hgroup() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type hr() =
        interface IntrinsicVoidNode
        member inline this.asDomElement: HTMLHRElement = unbox this

    [<Erase>]
    type html() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLHtmlElement = unbox this

    [<Erase>]
    type i() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type iframe() =
        interface IntrinsicParentNode
        interface IframeHTMLAttributes
        member inline this.asDomElement: HTMLIFrameElement = unbox this

    [<Erase>]
    type img() =
        interface IntrinsicVoidNode
        interface ImgHTMLAttributes
        member inline this.asDomElement: HTMLImageElement = unbox this

    [<Erase>]
    type input() =
        interface IntrinsicVoidNode
        interface InputHTMLAttributes
        member inline this.asDomElement: HTMLInputElement = unbox this

    [<Erase>]
    type ins() =
        interface IntrinsicParentNode
        interface InsHTMLAttributes
        member inline this.asDomElement: HTMLModElement = unbox this

    [<Erase>]
    type kbd() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type label() =
        interface IntrinsicParentNode
        interface LabelHTMLAttributes
        member inline this.asDomElement: HTMLLabelElement = unbox this

    [<Erase>]
    type legend() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLLegendElement = unbox this

    [<Erase>]
    type li() =
        interface IntrinsicParentNode
        interface LiHTMLAttributes
        member inline this.asDomElement: HTMLLIElement = unbox this

    [<Erase>]
    type link() =
        interface IntrinsicVoidNode
        interface LinkHTMLAttributes
        member inline this.asDomElement: HTMLLinkElement = unbox this

    [<Erase>]
    type main() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type map() =
        interface IntrinsicParentNode
        interface MapHTMLAttributes
        member inline this.asDomElement: HTMLMapElement = unbox this

    [<Erase>]
    type mark() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type menu() =
        interface IntrinsicParentNode
        interface MenuHTMLAttributes
        member inline this.asDomElement: HTMLMenuElement = unbox this

    [<Erase>]
    type meta() =
        interface IntrinsicVoidNode
        interface MetaHTMLAttributes
        member inline this.asDomElement: HTMLMetaElement = unbox this

    [<Erase>]
    type meter() =
        interface IntrinsicParentNode
        interface MeterHTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type nav() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type noscript() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type object'() =
        interface IntrinsicParentNode
        interface ObjectHTMLAttributes
        member inline this.asDomElement: HTMLObjectElement = unbox this

    [<Erase>]
    type ol() =
        interface IntrinsicParentNode
        interface OlHTMLAttributes
        member inline this.asDomElement: HTMLOListElement = unbox this

    [<Erase>]
    type optgroup() =
        interface IntrinsicParentNode
        interface OptgroupHTMLAttributes
        member inline this.asDomElement: HTMLOptGroupElement = unbox this

    [<Erase>]
    type option'() =
        interface IntrinsicParentNode
        interface OptionHTMLAttributes
        member inline this.asDomElement: HTMLOptionElement = unbox this

    [<Erase>]
    type output() =
        interface IntrinsicParentNode
        interface OutputHTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type p() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLParagraphElement = unbox this

    [<Erase>]
    type picture() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type pre() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLPreElement = unbox this

    [<Erase>]
    type progress() =
        interface IntrinsicParentNode
        interface ProgressHTMLAttributes
        member inline this.asDomElement: HTMLProgressElement = unbox this

    [<Erase>]
    type q() =
        interface IntrinsicParentNode
        interface QuoteHTMLAttributes
        member inline this.asDomElement: HTMLQuoteElement = unbox this

    [<Erase>]
    type rp() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type rt() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type ruby() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type s() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type samp() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type script() =
        interface IntrinsicParentNode
        interface ScriptHTMLAttributes
        member inline this.asDomElement: HTMLScriptElement = unbox this

    [<Erase>]
    type search() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type section() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type select() =
        interface IntrinsicParentNode
        interface SelectHTMLAttributes
        member inline this.asDomElement: HTMLSelectElement = unbox this

    [<Erase>]
    type small() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type source() =
        interface IntrinsicVoidNode
        interface SourceHTMLAttributes
        member inline this.asDomElement: HTMLSourceElement = unbox this

    [<Erase>]
    type span() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLSpanElement = unbox this

    [<Erase>]
    type strong() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type style() =
        interface IntrinsicParentNode
        interface StyleHTMLAttributes
        member inline this.asDomElement: HTMLStyleElement = unbox this

    [<Erase>]
    type sub() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type summary() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type sup() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type table() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLTableElement = unbox this

    [<Erase>]
    type tbody() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLTableSectionElement = unbox this

    [<Erase>]
    type td() =
        interface IntrinsicParentNode
        interface TdHTMLAttributes
        member inline this.asDomElement: HTMLTableCellElement = unbox this

    [<Erase>]
    type textarea() =
        interface IntrinsicParentNode
        interface TextareaHTMLAttributes
        member inline this.asDomElement: HTMLTextAreaElement = unbox this

    [<Erase>]
    type tfoot() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLTableSectionElement = unbox this

    [<Erase>]
    type th() =
        interface IntrinsicParentNode
        interface ThHTMLAttributes
        member inline this.asDomElement: HTMLTableCellElement = unbox this

    [<Erase>]
    type thead() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLTableSectionElement = unbox this

    [<Erase>]
    type time() =
        interface IntrinsicParentNode
        interface TimeHTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type title() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLTitleElement = unbox this

    [<Erase>]
    type tr() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLTableRowElement = unbox this

    [<Erase>]
    type track() =
        interface IntrinsicParentNode
        interface TrackHTMLAttributes
        member inline this.asDomElement: HTMLTrackElement = unbox this

    [<Erase>]
    type u() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type ul() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLUListElement = unbox this

    [<Erase>]
    type var() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type video() =
        interface IntrinsicParentNode
        interface VideoHTMLAttributes
        member inline this.asDomElement: HTMLVideoElement = unbox this

    [<Erase>]
    type wbr() =
        interface IntrinsicParentNode
        member inline this.asDomElement: HTMLElement = unbox this

