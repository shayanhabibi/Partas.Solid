namespace Partas.Solid

open System.Runtime.CompilerServices
open Browser.Types
open JetBrains.Annotations
open Fable.Core
open Fable.Core.JsInterop
open Partas.Solid.Experimental.U

#nowarn 1182

[<AutoOpen>]
module Tags =
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
        static member inline style'(this: #HtmlTag, styles: (string * obj) list) =
            this.style' (createObj styles)

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

    [<Erase>]
    type a() =
        interface RegularNode
        interface AnchorHTMLAttributes
        member inline this.asDomElement: HTMLAnchorElement = unbox this

    [<Erase>]
    type abbr() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type address() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type area() =
        interface VoidNode
        interface AreaHTMLAttributes
        member inline this.asDomElement: HTMLAreaElement = unbox this

    [<Erase>]
    type article() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type aside() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type audio() =
        interface RegularNode
        interface AudioHTMLAttributes
        member inline this.asDomElement: HTMLAudioElement = unbox this

    [<Erase>]
    type b() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type base'() =
        interface VoidNode
        member inline this.asDomElement: HTMLBaseElement = unbox this

    [<Erase>]
    type bdi() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type bdo() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type blockquote() =
        interface RegularNode
        interface BlockquoteHTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type body() =
        interface RegularNode
        member inline this.asDomElement: HTMLBodyElement = unbox this

    [<Erase>]
    type br() =
        interface VoidNode
        member inline this.asDomElement: HTMLBRElement = unbox this

    [<Erase>]
    type button() =
        interface RegularNode
        interface ButtonHTMLAttributes
        member inline this.asDomElement: HTMLButtonElement = unbox this

    [<Erase>]
    type canvas() =
        interface RegularNode
        interface CanvasHTMLAttributes
        member inline this.asDomElement: HTMLCanvasElement = unbox this

    [<Erase>]
    type caption() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type cite() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type code() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type col() =
        interface VoidNode
        interface ColHTMLAttributes
        member inline this.asDomElement: HTMLTableColElement = unbox this

    [<Erase>]
    type colgroup() =
        interface RegularNode
        interface ColgroupHTMLAttributes
        member inline this.asDomElement: HTMLTableColElement = unbox this

    [<Erase>]
    type data() =
        interface RegularNode
        interface DataHTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type datalist() =
        interface RegularNode
        member inline this.asDomElement: HTMLDataListElement = unbox this

    [<Erase>]
    type dd() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type del() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type details() =
        interface RegularNode
        interface DetailsHtmlAttributes
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type dfn() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type dialog() =
        interface RegularNode
        interface DialogHtmlAttributes
        member inline this.asDomElement: HTMLDialogElement = unbox this

    [<Erase>]
    type div() =
        interface RegularNode
        member inline this.asDomElement: HTMLDivElement = unbox this

    [<Erase>]
    type dl() =
        interface RegularNode
        member inline this.asDomElement: HTMLDListElement = unbox this

    [<Erase>]
    type dt() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type em() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type embed() =
        interface VoidNode
        interface EmbedHTMLAttributes
        member inline this.asDomElement: HTMLEmbedElement = unbox this

    [<Erase>]
    type fieldset() =
        interface RegularNode
        interface FieldsetHTMLAttributes
        member inline this.asDomElement: HTMLFieldSetElement = unbox this

    [<Erase>]
    type figcaption() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type figure() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type footer() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type form() =
        interface RegularNode
        interface FormHTMLAttributes
        member inline this.asDomElement: HTMLFormElement = unbox this

    [<Erase>]
    type h1() =
        interface RegularNode
        member inline this.asDomElement: HTMLHeadingElement = unbox this

    [<Erase>]
    type h2() =
        interface RegularNode
        member inline this.asDomElement: HTMLHeadingElement = unbox this

    [<Erase>]
    type h3() =
        interface RegularNode
        member inline this.asDomElement: HTMLHeadingElement = unbox this

    [<Erase>]
    type h4() =
        interface RegularNode
        member inline this.asDomElement: HTMLHeadingElement = unbox this

    [<Erase>]
    type h5() =
        interface RegularNode
        member inline this.asDomElement: HTMLHeadingElement = unbox this

    [<Erase>]
    type h6() =
        interface RegularNode
        member inline this.asDomElement: HTMLHeadingElement = unbox this

    [<Erase>]
    type head() =
        interface RegularNode
        member inline this.asDomElement: HTMLHeadElement = unbox this

    [<Erase>]
    type header() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type hgroup() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type hr() =
        interface VoidNode
        member inline this.asDomElement: HTMLHRElement = unbox this

    [<Erase>]
    type html() =
        interface RegularNode
        member inline this.asDomElement: HTMLHtmlElement = unbox this

    [<Erase>]
    type i() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type iframe() =
        interface RegularNode
        interface IframeHTMLAttributes
        member inline this.asDomElement: HTMLIFrameElement = unbox this

    [<Erase>]
    type img() =
        interface VoidNode
        interface ImgHTMLAttributes
        member inline this.asDomElement: HTMLImageElement = unbox this

    [<Erase>]
    type input() =
        interface VoidNode
        interface InputHTMLAttributes
        member inline this.asDomElement: HTMLInputElement = unbox this

    [<Erase>]
    type ins() =
        interface RegularNode
        interface InsHTMLAttributes
        member inline this.asDomElement: HTMLModElement = unbox this

    [<Erase>]
    type kbd() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type label() =
        interface RegularNode
        interface LabelHTMLAttributes
        member inline this.asDomElement: HTMLLabelElement = unbox this

    [<Erase>]
    type legend() =
        interface RegularNode
        member inline this.asDomElement: HTMLLegendElement = unbox this

    [<Erase>]
    type li() =
        interface RegularNode
        interface LiHTMLAttributes
        member inline this.asDomElement: HTMLLIElement = unbox this

    [<Erase>]
    type link() =
        interface VoidNode
        interface LinkHTMLAttributes
        member inline this.asDomElement: HTMLLinkElement = unbox this

    [<Erase>]
    type main() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type map() =
        interface RegularNode
        interface MapHTMLAttributes
        member inline this.asDomElement: HTMLMapElement = unbox this

    [<Erase>]
    type mark() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type menu() =
        interface RegularNode
        interface MenuHTMLAttributes
        member inline this.asDomElement: HTMLMenuElement = unbox this

    [<Erase>]
    type meta() =
        interface VoidNode
        interface MetaHTMLAttributes
        member inline this.asDomElement: HTMLMetaElement = unbox this

    [<Erase>]
    type meter() =
        interface RegularNode
        interface MeterHTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type nav() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type noscript() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type object'() =
        interface RegularNode
        interface ObjectHTMLAttributes
        member inline this.asDomElement: HTMLObjectElement = unbox this

    [<Erase>]
    type ol() =
        interface RegularNode
        interface OlHTMLAttributes
        member inline this.asDomElement: HTMLOListElement = unbox this

    [<Erase>]
    type optgroup() =
        interface RegularNode
        interface OptgroupHTMLAttributes
        member inline this.asDomElement: HTMLOptGroupElement = unbox this

    [<Erase>]
    type option'() =
        interface RegularNode
        interface OptionHTMLAttributes
        member inline this.asDomElement: HTMLOptionElement = unbox this

    [<Erase>]
    type output() =
        interface RegularNode
        interface OutputHTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type p() =
        interface RegularNode
        member inline this.asDomElement: HTMLParagraphElement = unbox this

    [<Erase>]
    type picture() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type pre() =
        interface RegularNode
        member inline this.asDomElement: HTMLPreElement = unbox this

    [<Erase>]
    type progress() =
        interface RegularNode
        interface ProgressHTMLAttributes
        member inline this.asDomElement: HTMLProgressElement = unbox this

    [<Erase>]
    type q() =
        interface RegularNode
        interface QuoteHTMLAttributes
        member inline this.asDomElement: HTMLQuoteElement = unbox this

    [<Erase>]
    type rp() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type rt() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type ruby() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type s() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type samp() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type script() =
        interface RegularNode
        interface ScriptHTMLAttributes
        member inline this.asDomElement: HTMLScriptElement = unbox this

    [<Erase>]
    type search() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type section() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type select() =
        interface RegularNode
        interface SelectHTMLAttributes
        member inline this.asDomElement: HTMLSelectElement = unbox this

    [<Erase>]
    type small() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type source() =
        interface VoidNode
        interface SourceHTMLAttributes
        member inline this.asDomElement: HTMLSourceElement = unbox this

    [<Erase>]
    type span() =
        interface RegularNode
        member inline this.asDomElement: HTMLSpanElement = unbox this

    [<Erase>]
    type strong() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type style() =
        interface RegularNode
        interface StyleHTMLAttributes
        member inline this.asDomElement: HTMLStyleElement = unbox this

    [<Erase>]
    type sub() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type summary() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type sup() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type table() =
        interface RegularNode
        member inline this.asDomElement: HTMLTableElement = unbox this

    [<Erase>]
    type tbody() =
        interface RegularNode
        member inline this.asDomElement: HTMLTableSectionElement = unbox this

    [<Erase>]
    type td() =
        interface RegularNode
        interface TdHTMLAttributes
        member inline this.asDomElement: HTMLTableCellElement = unbox this

    [<Erase>]
    type textarea() =
        interface RegularNode
        interface TextareaHTMLAttributes
        member inline this.asDomElement: HTMLTextAreaElement = unbox this

    [<Erase>]
    type tfoot() =
        interface RegularNode
        member inline this.asDomElement: HTMLTableSectionElement = unbox this

    [<Erase>]
    type th() =
        interface RegularNode
        interface ThHTMLAttributes
        member inline this.asDomElement: HTMLTableCellElement = unbox this

    [<Erase>]
    type thead() =
        interface RegularNode
        member inline this.asDomElement: HTMLTableSectionElement = unbox this

    [<Erase>]
    type time() =
        interface RegularNode
        interface TimeHTMLAttributes
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type title() =
        interface RegularNode
        member inline this.asDomElement: HTMLTitleElement = unbox this

    [<Erase>]
    type tr() =
        interface RegularNode
        member inline this.asDomElement: HTMLTableRowElement = unbox this

    [<Erase>]
    type track() =
        interface RegularNode
        interface TrackHTMLAttributes
        member inline this.asDomElement: HTMLTrackElement = unbox this

    [<Erase>]
    type u() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type ul() =
        interface RegularNode
        member inline this.asDomElement: HTMLUListElement = unbox this

    [<Erase>]
    type var() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this

    [<Erase>]
    type video() =
        interface RegularNode
        interface VideoHTMLAttributes
        member inline this.asDomElement: HTMLVideoElement = unbox this

    [<Erase>]
    type wbr() =
        interface RegularNode
        member inline this.asDomElement: HTMLElement = unbox this
