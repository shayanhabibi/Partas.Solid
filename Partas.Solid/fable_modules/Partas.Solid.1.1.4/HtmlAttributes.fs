namespace Partas.Solid

open System.Runtime.CompilerServices
open Browser.Types
open JetBrains.Annotations
open Fable.Core
open Fable.Core.JsInterop
open Partas.Solid.Experimental.U

[<AutoOpen>]
module HtmlAttributes =
    type HtmlContainer with
        [<Erase>]
        member _.children
            with get(): HtmlElement = jsNative
    type HTMLAttributes with
        [<LanguageInjection(InjectedLanguage.HTML)>]
        [<Erase>]
        member _.innerHTML
            with set(value: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.innerText
            with set(value: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.textContent
            with set(value: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.onCopy 
            with set(value: ClipboardEvent -> unit) = ()
            and [<Erase>] get(): ClipboardEvent -> unit = unbox ()
        [<Erase>]
        member _.onCut 
            with set(value: ClipboardEvent -> unit) = ()
            and [<Erase>] get(): ClipboardEvent -> unit = unbox ()
        [<Erase>]
        member _.onPaste 
            with set(value: ClipboardEvent -> unit) = ()
            and [<Erase>] get(): ClipboardEvent -> unit = unbox ()
        [<Erase>]
        member _.onCompositionEnd 
            with set(value: CompositionEvent -> unit) = ()
            and [<Erase>] get(): CompositionEvent -> unit = unbox ()
        [<Erase>]
        member _.onCompositionStart 
            with set(value: CompositionEvent -> unit) = ()
            and [<Erase>] get(): CompositionEvent -> unit = unbox ()
        [<Erase>]
        member _.onCompositionUpdate 
            with set(value: CompositionEvent -> unit) = ()
            and [<Erase>] get(): CompositionEvent -> unit = unbox ()
        [<Erase>]
        member _.onFocusOut 
            with set(value: FocusEvent -> unit) = ()
            and [<Erase>] get(): FocusEvent -> unit = unbox ()
        [<Erase>]
        member _.onFocusIn 
            with set(value: FocusEvent -> unit) = ()
            and [<Erase>] get(): FocusEvent -> unit = unbox ()
        [<Erase>]
        member _.onEncrypted 
            with set(value: Event -> unit) = ()
            and [<Erase>] get(): Event -> unit = unbox ()
        [<Erase>]
        member _.onDragExit 
            with set(value: DragEvent -> unit) = ()
            and [<Erase>] get(): DragEvent -> unit = unbox ()
        [<Erase>]
        member _.onAbort 
            with set(value: Event -> unit) = ()
            and [<Erase>] get(): Event -> unit = unbox ()
        [<Erase>]
        member _.onAnimationEnd 
            with set(value: AnimationEvent -> unit) = ()
            and [<Erase>] get(): AnimationEvent -> unit = unbox ()
        [<Erase>]
        member _.onAnimationIteration 
            with set(value: AnimationEvent -> unit) = ()
            and [<Erase>] get(): AnimationEvent -> unit = unbox ()
        [<Erase>]
        member _.onAnimationStart 
            with set(value: AnimationEvent -> unit) = ()
            and [<Erase>] get(): AnimationEvent -> unit = unbox ()
        [<Erase>]
        member _.onAuxClick 
            with set(value: MouseEvent -> unit) = ()
            and [<Erase>] get(): MouseEvent -> unit = unbox ()
        [<Erase>]
        member _.onBeforeInput 
            with set(value: InputEvent -> unit) = ()
            and [<Erase>] get(): InputEvent -> unit = unbox ()
        [<Erase>]
        member _.onBeforeToggle 
            with set(value: ToggleEvent -> unit) = ()
            and [<Erase>] get(): ToggleEvent -> unit = unbox ()
        [<Erase>]
        member _.onBlur 
            with set(value: FocusEvent -> unit) = ()
            and [<Erase>] get(): FocusEvent -> unit = unbox ()
        [<Erase>]
        member _.onCanPlay 
            with set(value: Event -> unit) = ()
            and [<Erase>] get(): Event -> unit = unbox ()
        [<Erase>]
        member _.onCanPlayThrough 
            with set(value: Event -> unit) = ()
            and [<Erase>] get(): Event -> unit = unbox ()
        [<Erase>]
        member _.onChange 
            with set(value: Event -> unit) = ()
            and [<Erase>] get(): Event -> unit = unbox ()
        [<Erase>]
        member _.onClick 
            with set(value: MouseEvent -> unit) = ()
            and [<Erase>] get(): MouseEvent -> unit = unbox ()
        [<Erase>]
        member _.onContextMenu 
            with set(value: MouseEvent -> unit) = ()
            and [<Erase>] get(): MouseEvent -> unit = unbox ()
        [<Erase>]
        member _.onDblClick 
            with set(value: MouseEvent -> unit) = ()
            and [<Erase>] get(): MouseEvent -> unit = unbox ()
        [<Erase>]
        member _.onDrag 
            with set(value: DragEvent -> unit) = ()
            and [<Erase>] get(): DragEvent -> unit = unbox ()
        [<Erase>]
        member _.onDragEnd 
            with set(value: DragEvent -> unit) = ()
            and [<Erase>] get(): DragEvent -> unit = unbox ()
        [<Erase>]
        member _.onDragEnter 
            with set(value: DragEvent -> unit) = ()
            and [<Erase>] get(): DragEvent -> unit = unbox ()
        [<Erase>]
        member _.onDragLeave 
            with set(value: DragEvent -> unit) = ()
            and [<Erase>] get(): DragEvent -> unit = unbox ()
        [<Erase>]
        member _.onDragOver 
            with set(value: DragEvent -> unit) = ()
            and [<Erase>] get(): DragEvent -> unit = unbox ()
        [<Erase>]
        member _.onDragStart 
            with set(value: DragEvent -> unit) = ()
            and [<Erase>] get(): DragEvent -> unit = unbox ()
        [<Erase>]
        member _.onDrop 
            with set(value: DragEvent -> unit) = ()
            and [<Erase>] get(): DragEvent -> unit = unbox ()
        [<Erase>]
        member _.onDurationChange 
            with set(value: Event -> unit) = ()
            and [<Erase>] get(): Event -> unit = unbox ()
        [<Erase>]
        member _.onEmptied 
            with set(value: Event -> unit) = ()
            and [<Erase>] get(): Event -> unit = unbox ()
        [<Erase>]
        member _.onEnded 
            with set(value: Event -> unit) = ()
            and [<Erase>] get(): Event -> unit = unbox ()
        [<Erase>]
        member _.onError 
            with set(value: Event -> unit) = ()
            and [<Erase>] get(): Event -> unit = unbox ()
        [<Erase>]
        member _.onFocus 
            with set(value: FocusEvent -> unit) = ()
            and [<Erase>] get(): FocusEvent -> unit = unbox ()
        [<Erase>]
        member _.onGotPointerCapture 
            with set(value: PointerEvent -> unit) = ()
            and [<Erase>] get(): PointerEvent -> unit = unbox ()
        [<Erase>]
        member _.onInput 
            with set(value: InputEvent -> unit) = ()
            and [<Erase>] get(): InputEvent -> unit = unbox ()
        [<Erase>]
        member _.onInvalid 
            with set(value: Event -> unit) = ()
            and [<Erase>] get(): Event -> unit = unbox ()
        [<Erase>]
        member _.onKeyDown 
            with set(value: KeyboardEvent -> unit) = ()
            and [<Erase>] get(): KeyboardEvent -> unit = unbox ()
        [<Erase>]
        member _.onKeyPress 
            with set(value: KeyboardEvent -> unit) = ()
            and [<Erase>] get(): KeyboardEvent -> unit = unbox ()
        [<Erase>]
        member _.onKeyUp 
            with set(value: KeyboardEvent -> unit) = ()
            and [<Erase>] get(): KeyboardEvent -> unit = unbox ()
        [<Erase>]
        member _.onLoad 
            with set(value: Event -> unit) = ()
            and [<Erase>] get(): Event -> unit = unbox ()
        [<Erase>]
        member _.onLoadedData 
            with set(value: Event -> unit) = ()
            and [<Erase>] get(): Event -> unit = unbox ()
        [<Erase>]
        member _.onLoadedMetadata 
            with set(value: Event -> unit) = ()
            and [<Erase>] get(): Event -> unit = unbox ()
        [<Erase>]
        member _.onLoadStart 
            with set(value: Event -> unit) = ()
            and [<Erase>] get(): Event -> unit = unbox ()
        [<Erase>]
        member _.onLostPointerCapture 
            with set(value: PointerEvent -> unit) = ()
            and [<Erase>] get(): PointerEvent -> unit = unbox ()
        [<Erase>]
        member _.onMouseDown 
            with set(value: MouseEvent -> unit) = ()
            and [<Erase>] get(): MouseEvent -> unit = unbox ()
        [<Erase>]
        member _.onMouseEnter 
            with set(value: MouseEvent -> unit) = ()
            and [<Erase>] get(): MouseEvent -> unit = unbox ()
        [<Erase>]
        member _.onMouseLeave 
            with set(value: MouseEvent -> unit) = ()
            and [<Erase>] get(): MouseEvent -> unit = unbox ()
        [<Erase>]
        member _.onMouseMove 
            with set(value: MouseEvent -> unit) = ()
            and [<Erase>] get(): MouseEvent -> unit = unbox ()
        [<Erase>]
        member _.onMouseOut 
            with set(value: MouseEvent -> unit) = ()
            and [<Erase>] get(): MouseEvent -> unit = unbox ()
        [<Erase>]
        member _.onMouseOver 
            with set(value: MouseEvent -> unit) = ()
            and [<Erase>] get(): MouseEvent -> unit = unbox ()
        [<Erase>]
        member _.onMouseUp 
            with set(value: MouseEvent -> unit) = ()
            and [<Erase>] get(): MouseEvent -> unit = unbox ()
        [<Erase>]
        member _.onPause 
            with set(value: Event -> unit) = ()
            and [<Erase>] get(): Event -> unit = unbox ()
        [<Erase>]
        member _.onPlay 
            with set(value: Event -> unit) = ()
            and [<Erase>] get(): Event -> unit = unbox ()
        [<Erase>]
        member _.onPlaying 
            with set(value: Event -> unit) = ()
            and [<Erase>] get(): Event -> unit = unbox ()
        [<Erase>]
        member _.onPointerCancel 
            with set(value: PointerEvent -> unit) = ()
            and [<Erase>] get(): PointerEvent -> unit = unbox ()
        [<Erase>]
        member _.onPointerDown 
            with set(value: PointerEvent -> unit) = ()
            and [<Erase>] get(): PointerEvent -> unit = unbox ()
        [<Erase>]
        member _.onPointerEnter 
            with set(value: PointerEvent -> unit) = ()
            and [<Erase>] get(): PointerEvent -> unit = unbox ()
        [<Erase>]
        member _.onPointerLeave 
            with set(value: PointerEvent -> unit) = ()
            and [<Erase>] get(): PointerEvent -> unit = unbox ()
        [<Erase>]
        member _.onPointerMove 
            with set(value: PointerEvent -> unit) = ()
            and [<Erase>] get(): PointerEvent -> unit = unbox ()
        [<Erase>]
        member _.onPointerOut 
            with set(value: PointerEvent -> unit) = ()
            and [<Erase>] get(): PointerEvent -> unit = unbox ()
        [<Erase>]
        member _.onPointerOver 
            with set(value: PointerEvent -> unit) = ()
            and [<Erase>] get(): PointerEvent -> unit = unbox ()
        [<Erase>]
        member _.onPointerUp 
            with set(value: PointerEvent -> unit) = ()
            and [<Erase>] get(): PointerEvent -> unit = unbox ()
        [<Erase>]
        member _.onProgress 
            with set(value: ProgressEvent -> unit) = ()
            and [<Erase>] get(): ProgressEvent -> unit = unbox ()
        [<Erase>]
        member _.onRateChange 
            with set(value: Event -> unit) = ()
            and [<Erase>] get(): Event -> unit = unbox ()
        [<Erase>]
        member _.onReset 
            with set(value: Event -> unit) = ()
            and [<Erase>] get(): Event -> unit = unbox ()
        [<Erase>]
        member _.onScroll 
            with set(value: Event -> unit) = ()
            and [<Erase>] get(): Event -> unit = unbox ()
        [<Erase>]
        member _.onScrollEnd 
            with set(value: Event -> unit) = ()
            and [<Erase>] get(): Event -> unit = unbox ()
        [<Erase>]
        member _.onSeeked 
            with set(value: Event -> unit) = ()
            and [<Erase>] get(): Event -> unit = unbox ()
        [<Erase>]
        member _.onSeeking 
            with set(value: Event -> unit) = ()
            and [<Erase>] get(): Event -> unit = unbox ()
        [<Erase>]
        member _.onSelect 
            with set(value: Event -> unit) = ()
            and [<Erase>] get(): Event -> unit = unbox ()
        [<Erase>]
        member _.onStalled 
            with set(value: Event -> unit) = ()
            and [<Erase>] get(): Event -> unit = unbox ()
        [<Erase>]
        member _.onSubmit 
            with set(value: SubmitEvent -> unit) = ()
            and [<Erase>] get(): SubmitEvent -> unit = unbox ()
        [<Erase>]
        member _.onSuspend 
            with set(value: Event -> unit) = ()
            and [<Erase>] get(): Event -> unit = unbox ()
        [<Erase>]
        member _.onTimeUpdate 
            with set(value: Event -> unit) = ()
            and [<Erase>] get(): Event -> unit = unbox ()
        [<Erase>]
        member _.onToggle 
            with set(value: ToggleEvent -> unit) = ()
            and [<Erase>] get(): ToggleEvent -> unit = unbox ()
        [<Erase>]
        member _.onTouchCancel 
            with set(value: TouchEvent -> unit) = ()
            and [<Erase>] get(): TouchEvent -> unit = unbox ()
        [<Erase>]
        member _.onTouchEnd 
            with set(value: TouchEvent -> unit) = ()
            and [<Erase>] get(): TouchEvent -> unit = unbox ()
        [<Erase>]
        member _.onTouchMove 
            with set(value: TouchEvent -> unit) = ()
            and [<Erase>] get(): TouchEvent -> unit = unbox ()
        [<Erase>]
        member _.onTouchStart 
            with set(value: TouchEvent -> unit) = ()
            and [<Erase>] get(): TouchEvent -> unit = unbox ()
        [<Erase>]
        member _.onTransitionStart 
            with set(value: TransitionEvent -> unit) = ()
            and [<Erase>] get(): TransitionEvent -> unit = unbox ()
        [<Erase>]
        member _.onTransitionEnd 
            with set(value: TransitionEvent -> unit) = ()
            and [<Erase>] get(): TransitionEvent -> unit = unbox ()
        [<Erase>]
        member _.onTransitionRun 
            with set(value: TransitionEvent -> unit) = ()
            and [<Erase>] get(): TransitionEvent -> unit = unbox ()
        [<Erase>]
        member _.onTransitionCancel 
            with set(value: TransitionEvent -> unit) = ()
            and [<Erase>] get(): TransitionEvent -> unit = unbox ()
        [<Erase>]
        member _.onVolumeChange 
            with set(value: Event -> unit) = ()
            and [<Erase>] get(): Event -> unit = unbox ()
        [<Erase>]
        member _.onWaiting 
            with set(value: Event -> unit) = ()
            and [<Erase>] get(): Event -> unit = unbox ()
        [<Erase>]
        member _.onWheel 
            with set(value: WheelEvent -> unit) = ()
            and [<Erase>] get(): WheelEvent -> unit = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div class='", Suffix = "' >")>]
        member _.class'
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div accessKey='", Suffix = "' >")>]
        member _.accessKey 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div contenteditable='", Suffix = "' >")>]
        member _.contenteditable 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div contextmenu='", Suffix = "' >")>]
        member _.contextmenu 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div dir='", Suffix = "' >")>]
        member _.dir 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div draggable='", Suffix = "' >")>]
        member _.draggable 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div hidden='", Suffix = "' >")>]
        member _.hidden 
            with set (_: U2<string, bool>) = ()
            and [<Erase>] get(): U2<string, bool> = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div id='", Suffix = "' >")>]
        member _.id 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div is='", Suffix = "' >")>]
        member _.is 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div inert='", Suffix = "' >")>]
        member _.inert 
            with set (_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div lang='", Suffix = "' >")>]
        member _.lang 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div spellcheck='", Suffix = "' >")>]
        member _.spellcheck 
            with set (_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never); Erase>]
        member _.style''
            with set(_: obj) = ()
            and get(): obj = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div style='", Suffix = "' />")>]
        member this.style 
            with inline set (value: string) = this.style'' <- value
            and inline [<Erase>] get(): string = !!this.style''
        [<Erase; LanguageInjection("jsx", Prefix = "<div tabindex='", Suffix = "' >")>]
        member _.tabindex 
            with set (_: int) = ()
            and [<Erase>] get(): int = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div title='", Suffix = "' >")>]
        member _.title 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div translate='", Suffix = "' >")>]
        member _.translate 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div about='", Suffix = "' >")>]
        member _.about 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div datatype='", Suffix = "' >")>]
        member _.datatype 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div inlist='", Suffix = "' >")>]
        member _.inlist 
            with set (_: obj) = ()
            and [<Erase>] get() = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div popover='", Suffix = "' >")>]
        member _.popover 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div prefix='", Suffix = "' >")>]
        member _.prefix 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div property='", Suffix = "' >")>]
        member _.property 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div resource='", Suffix = "' >")>]
        member _.resource 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div typeof='", Suffix = "' >")>]
        member _.typeof 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div vocab='", Suffix = "' >")>]
        member _.vocab 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div autocapitalize='", Suffix = "' >")>]
        member _.autocapitalize 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div slot='", Suffix = "' >")>]
        member _.slot 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div color='", Suffix = "' >")>]
        member _.color 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div itemprop='", Suffix = "' >")>]
        member _.itemprop 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div itemscope='", Suffix = "' >")>]
        member _.itemscope 
            with set (_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div itemtype='", Suffix = "' >")>]
        member _.itemtype 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div itemid='", Suffix = "' >")>]
        member _.itemid 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div itemref='", Suffix = "' >")>]
        member _.itemref 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div part='", Suffix = "' >")>]
        member _.part 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div exportparts='", Suffix = "' >")>]
        member _.exportparts 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div inputmode='", Suffix = "' >")>]
        member _.inputmode 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div contentEditable='", Suffix = "' >")>]
        member _.contentEditable 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div contextMenu='", Suffix = "' >")>]
        member _.contextMenu 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div tabIndex='", Suffix = "' >")>]
        member _.tabIndex 
            with set (_: int) = ()
            and [<Erase>] get(): int = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div autoCapitalize='", Suffix = "' >")>]
        member _.autoCapitalize 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div itemProp='", Suffix = "' >")>]
        member _.itemProp 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div itemScope='", Suffix = "' >")>]
        member _.itemScope 
            with set (_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div itemType='", Suffix = "' >")>]
        member _.itemType 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div itemId='", Suffix = "' >")>]
        member _.itemId 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div itemRef='", Suffix = "' >")>]
        member _.itemRef 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div exportParts='", Suffix = "' >")>]
        member _.exportParts 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection("jsx", Prefix = "<div inputMode='", Suffix = "' >")>]
        member _.inputMode 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()

    type AnchorHTMLAttributes with
        [<Erase; LanguageInjection("jsx", Prefix = "<a download='", Suffix = "' >")>]
        member _.download
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase; LanguageInjection("jsx", Prefix = "<a href='", Suffix = "' >")>]
        member _.href
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase; LanguageInjection("jsx", Prefix = "<a hreflang='", Suffix = "' >")>]
        member _.hreflang
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase; LanguageInjection("jsx", Prefix = "<a media='", Suffix = "' >")>]
        member _.media
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase; LanguageInjection("jsx", Prefix = "<a ping='", Suffix = "' >")>]
        member _.ping
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase; LanguageInjection("jsx", Prefix = "<a referrerpolicy='", Suffix = "' >")>]
        member _.referrerpolicy
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase; LanguageInjection("jsx", Prefix = "<a rel='", Suffix = "' >")>]
        member _.rel
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase; LanguageInjection("jsx", Prefix = "<a target='", Suffix = "' >")>]
        member _.target
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase; LanguageInjection("jsx", Prefix = "<a type='", Suffix = "' >")>]
        member _.type'
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase; LanguageInjection("jsx", Prefix = "<a referrerPolicy='", Suffix = "' >")>]
        member _.referrerPolicy
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            


    type AreaHTMLAttributes with
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<area alt='", Suffix = "' >")>]
        member _.alt
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<area coords='", Suffix = "' >")>]
        member _.coords
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<area download='", Suffix = "' >")>]
        member _.download
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<area href='", Suffix = "' >")>]
        member _.href
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<area hreflang='", Suffix = "' >")>]
        member _.hreflang
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<area ping='", Suffix = "' >")>]
        member _.ping
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<area referrerpolicy='", Suffix = "' >")>]
        member _.referrerpolicy
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<area rel='", Suffix = "' >")>]
        member _.rel
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<area shape='", Suffix = "' >")>]
        member _.shape
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<area target='", Suffix = "' >")>]
        member _.target
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<area referrerPolicy='", Suffix = "' >")>]
        member _.referrerPolicy
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type BaseHTMLAttributes with
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<base href='", Suffix = "' >")>]        
        member _.href
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<base target='", Suffix = "' >")>]        
        member _.target
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type BlockquoteHTMLAttributes with
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<blockquote cite='", Suffix = "' >")>]        
        member _.cite
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type ButtonHTMLAttributes with
        [<Erase>]
        member _.autofocus
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        member _.disabled
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<button form='", Suffix = "' >")>]        
        member _.form
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.formNoValidate
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<button name='", Suffix = "' >")>]        
        member _.name
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<button type='", Suffix = "' >")>]        
        member _.type'
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<button value='", Suffix = "' >")>]        
        member _.value
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<button formAction='", Suffix = "' >")>]        
        member _.formAction
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<button formEnctype='", Suffix = "' >")>]        
        member _.formEnctype
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<button formMethod='", Suffix = "' >")>]        
        member _.formMethod
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<button formTarget='", Suffix = "' >")>]        
        member _.formTarget
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<button popoverTarget='", Suffix = "' >")>]        
        member _.popoverTarget
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<button popoverTargetAction='", Suffix = "' >")>]        
        member _.popoverTargetAction
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type CanvasHTMLAttributes with
        [<Erase>]
        member _.width
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        member _.height
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            

    type ColHTMLAttributes with
        [<Erase>]
        member _.span
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        member _.width
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            

    type ColgroupHTMLAttributes with
        [<Erase>]
        member _.span
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            

    type DataHTMLAttributes with
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<data value='", Suffix = "' >")>]        
        member _.value
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type DetailsHtmlAttributes with
        [<Erase>]
        member _.open'
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        member _.onToggle
            with set(_: Event -> unit) = ()
            and [<Erase>] get(): Event -> unit = unbox ()
            

    type DialogHtmlAttributes with
        [<Erase>]
        member _.open'
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        member _.onClose
            with set(_: Event -> unit) = ()
            and [<Erase>] get(): Event -> unit = unbox ()
            
        [<Erase>]
        member _.onCancel
            with set(_: Event -> unit) = ()
            and [<Erase>] get(): Event -> unit = unbox ()
            

    type EmbedHTMLAttributes with
        [<Erase>]
        member _.height
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<embed src='", Suffix = "' >")>]        
        member _.src
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<embed type='", Suffix = "' >")>]        
        member _.type'
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.width
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            

    type FieldsetHTMLAttributes with
        [<Erase>]
        member _.disabled
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<fieldset form='", Suffix = "' >")>]        
        member _.form
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<fieldset name='", Suffix = "' >")>]        
        member _.name
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type FormHTMLAttributes with
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<form accept='", Suffix = "' >")>]        
        member _.accept
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<form action='", Suffix = "' >")>]        
        member _.action
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<form autocomplete='", Suffix = "' >")>]        
        member _.autocomplete
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "< encoding='", Suffix = "' >")>]        
        member _.encoding
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<form enctype='", Suffix = "' >")>]        
        member _.enctype
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<form method='", Suffix = "' >")>]        
        member _.method
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<form name='", Suffix = "' >")>]        
        member _.name
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<form noValidate='", Suffix = "' >")>]        
        member _.noValidate
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<form target='", Suffix = "' >")>]        
        member _.target
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
            

    type IframeHTMLAttributes with
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<iframe allow='", Suffix = "' >")>]        
        member _.allow
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<iframe allowfullscreen='", Suffix = "' >")>]        
        member _.allowfullscreen
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.height
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<iframe loading='", Suffix = "' >")>]        
        member _.loading
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<iframe name='", Suffix = "' >")>]        
        member _.name
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<iframe referrerpolicy='", Suffix = "' >")>]        
        member _.referrerpolicy
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<iframe sandbox='", Suffix = "' >")>]        
        member _.sandbox
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<iframe src='", Suffix = "' >")>]        
        member _.src
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<iframe srcdoc='", Suffix = "' >")>]        
        member _.srcdoc
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.width
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<iframe referrerPolicy='", Suffix = "' >")>]        
        member _.referrerPolicy
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type ImgHTMLAttributes with
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<img alt='", Suffix = "' >")>]        
        member _.alt
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<img crossorigin='", Suffix = "' >")>]        
        member _.crossorigin
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<img decoding='", Suffix = "' >")>]        
        member _.decoding
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.height
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        member _.isMap
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<img loading='", Suffix = "' >")>]        
        member _.loading
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<img referrerpolicy='", Suffix = "' >")>]        
        member _.referrerpolicy
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<img referrerPolicy='", Suffix = "' >")>]        
        member _.referrerPolicy
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<img sizes='", Suffix = "' >")>]        
        member _.sizes
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<img src='", Suffix = "' >")>]        
        member _.src
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<img srcset='", Suffix = "' >")>]        
        member _.srcset
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<img srcSet='", Suffix = "' >")>]        
        member _.srcSet
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<img usemap='", Suffix = "' >")>]        
        member _.usemap
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<img useMap='", Suffix = "' >")>]        
        member _.useMap
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.width
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<img crossOrigin='", Suffix = "' >")>]        
        member _.crossOrigin
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<img elementtiming='", Suffix = "' >")>]        
        member _.elementtiming
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<img fetchpriority='", Suffix = "' >")>]        
        member _.fetchpriority
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type InputHTMLAttributes with
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<input accept='", Suffix = "' >")>]        
        member _.accept
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<input alt='", Suffix = "' >")>]        
        member _.alt
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<input autocomplete='", Suffix = "' >")>]        
        member _.autocomplete
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<input autocorrect='", Suffix = "' >")>]        
        member _.autocorrect
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.autofocus
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<input capture='", Suffix = "' >")>]        
        member _.capture
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.checked'
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<input crossorigin='", Suffix = "' >")>]        
        member _.crossorigin
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<input disabled='", Suffix = "' >")>]        
        member _.disabled
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<input enterkeyhint='", Suffix = "' >")>]        
        member _.enterkeyhint
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<input form='", Suffix = "' >")>]        
        member _.form
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.formNoValidate
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<input formtarget='", Suffix = "' >")>]        
        member _.formtarget
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.height
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<input incremental='", Suffix = "' >")>]        
        member _.incremental
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<input list='", Suffix = "' >")>]        
        member _.list
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.max
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        member _.maxLength
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        member _.min
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        member _.minLength
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        member _.multiple
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<input name='", Suffix = "' >")>]        
        member _.name
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<input pattern='", Suffix = "' >")>]        
        member _.pattern
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<input placeholder='", Suffix = "' >")>]        
        member _.placeholder
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.readOnly
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<input results='", Suffix = "' >")>]        
        member _.results
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.required
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        member _.size
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<input src='", Suffix = "' >")>]        
        member _.src
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<input step='", Suffix = "' >")>]        
        member _.step
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<input type='", Suffix = "' >")>]        
        member _.type'
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<input value='", Suffix = "' >")>]        
        member _.value
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.width
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<input crossOrigin='", Suffix = "' >")>]        
        member _.crossOrigin
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<input formAction='", Suffix = "' >")>]        
        member _.formAction
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<input formEnctype='", Suffix = "' >")>]        
        member _.formEnctype
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<input formMethod='", Suffix = "' >")>]        
        member _.formMethod
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.formnovalidate
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<input formTarget='", Suffix = "' >")>]        
        member _.formTarget
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.maxlength
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        member _.minlength
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        member _.readonly
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            

    type InsHTMLAttributes with
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<ins cite='", Suffix = "' >")>]        
        member _.cite
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<ins dateTime='", Suffix = "' >")>]        
        member _.dateTime
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type KeygenHTMLAttributes with
        [<Erase>]
        member _.autofocus
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<keygen challenge='", Suffix = "' >")>]        
        member _.challenge
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.disabled
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<keygen form='", Suffix = "' >")>]        
        member _.form
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<keygen keytype='", Suffix = "' >")>]        
        member _.keytype
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<keygen keyparams='", Suffix = "' >")>]        
        member _.keyparams
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<keygen name='", Suffix = "' >")>]        
        member _.name
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type LabelHTMLAttributes with
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<label for='", Suffix = "' >")>]        
        member _.for'
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<label form='", Suffix = "' >")>]        
        member _.form
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type LiHTMLAttributes with
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<li value='", Suffix = "' >")>]        
        member _.value
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type LinkHTMLAttributes with
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<link as='", Suffix = "' >")>]        
        member _.as'
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<link crossorigin='", Suffix = "' >")>]        
        member _.crossorigin
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.disabled
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<link fetchpriority='", Suffix = "' >")>]        
        member _.fetchpriority
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<link href='", Suffix = "' >")>]        
        member _.href
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<link hreflang='", Suffix = "' >")>]        
        member _.hreflang
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<link imagesizes='", Suffix = "' >")>]        
        member _.imagesizes
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<link imagesrcset='", Suffix = "' >")>]        
        member _.imagesrcset
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<link integrity='", Suffix = "' >")>]        
        member _.integrity
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<link media='", Suffix = "' >")>]        
        member _.media
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<link referrerpolicy='", Suffix = "' >")>]        
        member _.referrerpolicy
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<link rel='", Suffix = "' >")>]        
        member _.rel
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<link sizes='", Suffix = "' >")>]        
        member _.sizes
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<link type='", Suffix = "' >")>]        
        member _.type'
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<link crossOrigin='", Suffix = "' >")>]        
        member _.crossOrigin
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<link referrerPolicy='", Suffix = "' >")>]        
        member _.referrerPolicy
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type MapHTMLAttributes with
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<map name='", Suffix = "' >")>]        
        member _.name
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type MediaHTMLAttributes with
        [<Erase>]
        member _.autoplay
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        member _.controls
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<media controlslist='", Suffix = "' >")>]        
        member _.controlslist
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<media crossorigin='", Suffix = "' >")>]        
        member _.crossorigin
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.loop
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<media mediagroup='", Suffix = "' >")>]        
        member _.mediagroup
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.muted
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<media preload='", Suffix = "' >")>]        
        member _.preload
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<media src='", Suffix = "' >")>]        
        member _.src
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<media crossOrigin='", Suffix = "' >")>]        
        member _.crossOrigin
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<media mediaGroup='", Suffix = "' >")>]        
        member _.mediaGroup
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type MenuHTMLAttributes with
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<menu label='", Suffix = "' >")>]        
        member _.label
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<menu type='", Suffix = "' >")>]        
        member _.type'
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<menu charset='", Suffix = "' >")>]        
        member _.charset
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<menu content='", Suffix = "' >")>]        
        member _.content
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<menu http='", Suffix = "' >")>]        
        member _.http
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<menu name='", Suffix = "' >")>]        
        member _.name
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<menu media='", Suffix = "' >")>]        
        member _.media
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type MeterHTMLAttributes with
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<meter form='", Suffix = "' >")>]        
        member _.form
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.high
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        member _.low
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        member _.max
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        member _.min
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        member _.optimum
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        member _.value
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            

    type QuoteHTMLAttributes with
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<quote cite='", Suffix = "' >")>]        
        member _.cite
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type ObjectHTMLAttributes with
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<object data='", Suffix = "' >")>]        
        member _.data
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<object form='", Suffix = "' >")>]        
        member _.form
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.height
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<object name='", Suffix = "' >")>]        
        member _.name
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<object type='", Suffix = "' >")>]        
        member _.type'
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<object usemap='", Suffix = "' >")>]        
        member _.usemap
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.width
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<object useMap='", Suffix = "' >")>]        
        member _.useMap
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type OlHTMLAttributes with
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<ol reversed='", Suffix = "' >")>]        
        member _.reversed
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<ol start='", Suffix = "' >")>]        
        member _.start
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<ol type='", Suffix = "' >")>]        
        member _.type'
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type OptgroupHTMLAttributes with
        [<Erase>]
        member _.disabled
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<optgroup label='", Suffix = "' >")>]        
        member _.label
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type OptionHTMLAttributes with
        [<Erase>]
        member _.disabled
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<option label='", Suffix = "' >")>]        
        member _.label
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.selected
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<option value='", Suffix = "' >")>]        
        member _.value
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type OutputHTMLAttributes with
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<output form='", Suffix = "' >")>]        
        member _.form
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<output for='", Suffix = "' >")>]        
        member _.for'
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<output name='", Suffix = "' >")>]        
        member _.name
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type ParamHTMLAttributes with
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<param name='", Suffix = "' >")>]        
        member _.name
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<param value='", Suffix = "' >")>]        
        member _.value
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type ProgressHTMLAttributes with
        [<Erase>]
        member _.max
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<progress value='", Suffix = "' >")>]        
        member _.value
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type ScriptHTMLAttributes with
        [<Erase>]
        member _.async
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<script charset='", Suffix = "' >")>]        
        member _.charset
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<script crossorigin='", Suffix = "' >")>]        
        member _.crossorigin
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<script defer='", Suffix = "' >")>]        
        member _.defer
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<script integrity='", Suffix = "' >")>]        
        member _.integrity
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.noModule
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<script nonce='", Suffix = "' >")>]        
        member _.nonce
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<script referrerpolicy='", Suffix = "' >")>]        
        member _.referrerpolicy
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<script src='", Suffix = "' >")>]        
        member _.src
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<script type='", Suffix = "' >")>]        
        member _.type'
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<script crossOrigin='", Suffix = "' >")>]        
        member _.crossOrigin
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.nomodule
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<script referrerPolicy='", Suffix = "' >")>]        
        member _.referrerPolicy
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type SelectHTMLAttributes with
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<select autocomplete='", Suffix = "' >")>]        
        member _.autocomplete
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.autofocus
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        member _.disabled
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<select form='", Suffix = "' >")>]        
        member _.form
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.multiple
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<select name='", Suffix = "' >")>]        
        member _.name
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.required
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        member _.size
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<select value='", Suffix = "' >")>]        
        member _.value
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type HTMLSlotElementAttributes with
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<slot name='", Suffix = "' >")>]        
        member _.name
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type SourceHTMLAttributes with
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<source media='", Suffix = "' >")>]        
        member _.media
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<source sizes='", Suffix = "' >")>]        
        member _.sizes
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<source src='", Suffix = "' >")>]        
        member _.src
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<source srcset='", Suffix = "' >")>]        
        member _.srcset
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<source type='", Suffix = "' >")>]        
        member _.type'
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.width
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        member _.height
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            

    type StyleHTMLAttributes with
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<style media='", Suffix = "' >")>]        
        member _.media
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<style nonce='", Suffix = "' >")>]        
        member _.nonce
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<style scoped='", Suffix = "' >")>]        
        member _.scoped
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<style type='", Suffix = "' >")>]        
        member _.type'
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type TdHTMLAttributes with
        [<Erase>]
        member _.colspan
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<td headers='", Suffix = "' >")>]        
        member _.headers
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.rowspan
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        member _.colSpan
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        member _.rowSpan
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            

    type TemplateHTMLAttributes with
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<template content='", Suffix = "' >")>]        
        member _.content
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type TextareaHTMLAttributes with
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<textarea autocomplete='", Suffix = "' >")>]        
        member _.autocomplete
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.autofocus
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        member _.cols
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<textarea dirname='", Suffix = "' >")>]        
        member _.dirname
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.disabled
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<textarea enterkeyhint='", Suffix = "' >")>]        
        member _.enterkeyhint
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<textarea form='", Suffix = "' >")>]        
        member _.form
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.maxLength
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        member _.minLength
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<textarea name='", Suffix = "' >")>]        
        member _.name
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<textarea placeholder='", Suffix = "' >")>]        
        member _.placeholder
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.readOnly
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        member _.required
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        member _.rows
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<textarea value='", Suffix = "' >")>]        
        member _.value
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<textarea wrap='", Suffix = "' >")>]        
        member _.wrap
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.maxlength
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        member _.minlength
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        member _.readonly
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            

    type ThHTMLAttributes with
        [<Erase>]
        member _.colspan
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<th headers='", Suffix = "' >")>]        
        member _.headers
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.rowspan
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        member _.colSpan
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        member _.rowSpan
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<th scope='", Suffix = "' >")>]        
        member _.scope
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type TimeHTMLAttributes with
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<time datetime='", Suffix = "' >")>]        
        member _.datetime
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<time dateTime='", Suffix = "' >")>]        
        member _.dateTime
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type TrackHTMLAttributes with
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<track default='", Suffix = "' >")>]        
        member _.default'
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<track kind='", Suffix = "' >")>]        
        member _.kind
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<track label='", Suffix = "' >")>]        
        member _.label
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<track src='", Suffix = "' >")>]        
        member _.src
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<track srclang='", Suffix = "' >")>]        
        member _.srclang
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type VideoHTMLAttributes with
        [<Erase>]
        member _.height
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        member _.playsinline
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<video poster='", Suffix = "' >")>]        
        member _.poster
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.width
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        member _.disablepictureinpicture
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        member _.disableremoteplayback
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
