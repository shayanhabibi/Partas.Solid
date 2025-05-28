namespace Partas.Solid

open System.Runtime.CompilerServices
open Browser.Types
open JetBrains.Annotations
open Fable.Core
open Fable.Core.JsInterop
open Partas.Solid.Experimental.U

#nowarn 1182

[<AutoOpen>]
module HtmlAttributes =
    type HtmlContainer with
        [<Erase>]
        member _.children
            with get(): HtmlElement = jsNative
    type HTMLAttributes with
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
        member _.onCopy 
            with set(value: ClipboardEvent -> unit) = ()
            and [<Erase>] get(): ClipboardEvent -> unit = unbox ()
        member _.onCut 
            with set(value: ClipboardEvent -> unit) = ()
            and [<Erase>] get(): ClipboardEvent -> unit = unbox ()
        member _.onPaste 
            with set(value: ClipboardEvent -> unit) = ()
            and [<Erase>] get(): ClipboardEvent -> unit = unbox ()
        member _.onCompositionEnd 
            with set(value: CompositionEvent -> unit) = ()
            and [<Erase>] get(): CompositionEvent -> unit = unbox ()
        member _.onCompositionStart 
            with set(value: CompositionEvent -> unit) = ()
            and [<Erase>] get(): CompositionEvent -> unit = unbox ()
        member _.onCompositionUpdate 
            with set(value: CompositionEvent -> unit) = ()
            and [<Erase>] get(): CompositionEvent -> unit = unbox ()
        member _.onFocusOut 
            with set(value: FocusEvent -> unit) = ()
            and [<Erase>] get(): FocusEvent -> unit = unbox ()
        member _.onFocusIn 
            with set(value: FocusEvent -> unit) = ()
            and [<Erase>] get(): FocusEvent -> unit = unbox ()
        member _.onEncrypted 
            with set(value: Event -> unit) = ()
            and [<Erase>] get(): Event -> unit = unbox ()
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
        [<Erase>]
        member _.class'
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div accessKey='", Suffix = "' >")>]
        member _.accessKey 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div contenteditable='", Suffix = "' >")>]
        member _.contenteditable 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div contextmenu='", Suffix = "' >")>]
        member _.contextmenu 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div dir='", Suffix = "' >")>]
        member _.dir 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div draggable='", Suffix = "' >")>]
        member _.draggable 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div hidden='", Suffix = "' >")>]
        member _.hidden 
            with set (_: U2<string, bool>) = ()
            and [<Erase>] get(): U2<string, bool> = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div id='", Suffix = "' >")>]
        member _.id 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div is='", Suffix = "' >")>]
        member _.is 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div inert='", Suffix = "' >")>]
        member _.inert 
            with set (_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div lang='", Suffix = "' >")>]
        member _.lang 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div spellcheck='", Suffix = "' >")>]
        member _.spellcheck 
            with set (_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.JAVASCRIPT, Prefix = "<div style='", Suffix = "' />")>]
        member _.style 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div tabindex='", Suffix = "' >")>]
        member _.tabindex 
            with set (_: int) = ()
            and [<Erase>] get(): int = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div title='", Suffix = "' >")>]
        member _.title 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div translate='", Suffix = "' >")>]
        member _.translate 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div about='", Suffix = "' >")>]
        member _.about 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div datatype='", Suffix = "' >")>]
        member _.datatype 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div inlist='", Suffix = "' >")>]
        member _.inlist 
            with set (_: obj) = ()
            and [<Erase>] get() = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div popover='", Suffix = "' >")>]
        member _.popover 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div prefix='", Suffix = "' >")>]
        member _.prefix 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div property='", Suffix = "' >")>]
        member _.property 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div resource='", Suffix = "' >")>]
        member _.resource 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div typeof='", Suffix = "' >")>]
        member _.typeof 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div vocab='", Suffix = "' >")>]
        member _.vocab 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div autocapitalize='", Suffix = "' >")>]
        member _.autocapitalize 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div slot='", Suffix = "' >")>]
        member _.slot 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div color='", Suffix = "' >")>]
        member _.color 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div itemprop='", Suffix = "' >")>]
        member _.itemprop 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div itemscope='", Suffix = "' >")>]
        member _.itemscope 
            with set (_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div itemtype='", Suffix = "' >")>]
        member _.itemtype 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div itemid='", Suffix = "' >")>]
        member _.itemid 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div itemref='", Suffix = "' >")>]
        member _.itemref 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div part='", Suffix = "' >")>]
        member _.part 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div exportparts='", Suffix = "' >")>]
        member _.exportparts 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div inputmode='", Suffix = "' >")>]
        member _.inputmode 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div contentEditable='", Suffix = "' >")>]
        member _.contentEditable 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div contextMenu='", Suffix = "' >")>]
        member _.contextMenu 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div tabIndex='", Suffix = "' >")>]
        member _.tabIndex 
            with set (_: int) = ()
            and [<Erase>] get(): int = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div autoCapitalize='", Suffix = "' >")>]
        member _.autoCapitalize 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div itemProp='", Suffix = "' >")>]
        member _.itemProp 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div itemScope='", Suffix = "' >")>]
        member _.itemScope 
            with set (_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div itemType='", Suffix = "' >")>]
        member _.itemType 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div itemId='", Suffix = "' >")>]
        member _.itemId 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div itemRef='", Suffix = "' >")>]
        member _.itemRef 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div exportParts='", Suffix = "' >")>]
        member _.exportParts 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<div inputMode='", Suffix = "' >")>]
        member _.inputMode 
            with set (_: string) = ()
            and [<Erase>] get(): string = unbox ()

    type AnchorHTMLAttributes with
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<a download='", Suffix = "' >")>]
        member _.download
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<a href='", Suffix = "' >")>]
        member _.href
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<a hreflang='", Suffix = "' >")>]
        member _.hreflang
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<a media='", Suffix = "' >")>]
        member _.media
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<a ping='", Suffix = "' >")>]
        member _.ping
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<a referrerpolicy='", Suffix = "' >")>]
        member _.referrerpolicy
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<a rel='", Suffix = "' >")>]
        member _.rel
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<a target='", Suffix = "' >")>]
        member _.target
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<a type='", Suffix = "' >")>]
        member _.type'
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase; LanguageInjection(InjectedLanguage.HTML, Prefix = "<a referrerPolicy='", Suffix = "' >")>]
        member _.referrerPolicy
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            


    type AreaHTMLAttributes with
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<area alt='", Suffix = "' >")>]
        member _.alt
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<area coords='", Suffix = "' >")>]
        member _.coords
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<area download='", Suffix = "' >")>]
        member _.download
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<area href='", Suffix = "' >")>]
        member _.href
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<area hreflang='", Suffix = "' >")>]
        member _.hreflang
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<area ping='", Suffix = "' >")>]
        member _.ping
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<area referrerpolicy='", Suffix = "' >")>]
        member _.referrerpolicy
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<area rel='", Suffix = "' >")>]
        member _.rel
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<area shape='", Suffix = "' >")>]
        member _.shape
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<area target='", Suffix = "' >")>]
        member _.target
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<area referrerPolicy='", Suffix = "' >")>]
        member _.referrerPolicy
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type BaseHTMLAttributes with
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<base href='", Suffix = "' >")>]        
        member _.href
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<base target='", Suffix = "' >")>]        
        member _.target
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type BlockquoteHTMLAttributes with
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<blockquote cite='", Suffix = "' >")>]        
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
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<button form='", Suffix = "' >")>]        
        member _.form
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.formNoValidate
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<button name='", Suffix = "' >")>]        
        member _.name
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<button type='", Suffix = "' >")>]        
        member _.type'
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<button value='", Suffix = "' >")>]        
        member _.value
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<button formAction='", Suffix = "' >")>]        
        member _.formAction
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<button formEnctype='", Suffix = "' >")>]        
        member _.formEnctype
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<button formMethod='", Suffix = "' >")>]        
        member _.formMethod
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<button formTarget='", Suffix = "' >")>]        
        member _.formTarget
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<button popoverTarget='", Suffix = "' >")>]        
        member _.popoverTarget
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<button popoverTargetAction='", Suffix = "' >")>]        
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
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<data value='", Suffix = "' >")>]        
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
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<embed src='", Suffix = "' >")>]        
        member _.src
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<embed type='", Suffix = "' >")>]        
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
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<fieldset form='", Suffix = "' >")>]        
        member _.form
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<fieldset name='", Suffix = "' >")>]        
        member _.name
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type FormHTMLAttributes with
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<form accept='", Suffix = "' >")>]        
        member _.accept
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<form action='", Suffix = "' >")>]        
        member _.action
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<form autocomplete='", Suffix = "' >")>]        
        member _.autocomplete
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "< encoding='", Suffix = "' >")>]        
        member _.encoding
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<form enctype='", Suffix = "' >")>]        
        member _.enctype
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<form method='", Suffix = "' >")>]        
        member _.method
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<form name='", Suffix = "' >")>]        
        member _.name
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<form noValidate='", Suffix = "' >")>]        
        member _.noValidate
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<form target='", Suffix = "' >")>]        
        member _.target
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
            

    type IframeHTMLAttributes with
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<iframe allow='", Suffix = "' >")>]        
        member _.allow
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<iframe allowfullscreen='", Suffix = "' >")>]        
        member _.allowfullscreen
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.height
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<iframe loading='", Suffix = "' >")>]        
        member _.loading
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<iframe name='", Suffix = "' >")>]        
        member _.name
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<iframe referrerpolicy='", Suffix = "' >")>]        
        member _.referrerpolicy
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<iframe sandbox='", Suffix = "' >")>]        
        member _.sandbox
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<iframe src='", Suffix = "' >")>]        
        member _.src
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<iframe srcdoc='", Suffix = "' >")>]        
        member _.srcdoc
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.width
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<iframe referrerPolicy='", Suffix = "' >")>]        
        member _.referrerPolicy
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type ImgHTMLAttributes with
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<img alt='", Suffix = "' >")>]        
        member _.alt
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<img crossorigin='", Suffix = "' >")>]        
        member _.crossorigin
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<img decoding='", Suffix = "' >")>]        
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
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<img loading='", Suffix = "' >")>]        
        member _.loading
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<img referrerpolicy='", Suffix = "' >")>]        
        member _.referrerpolicy
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<img referrerPolicy='", Suffix = "' >")>]        
        member _.referrerPolicy
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<img sizes='", Suffix = "' >")>]        
        member _.sizes
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<img src='", Suffix = "' >")>]        
        member _.src
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<img srcset='", Suffix = "' >")>]        
        member _.srcset
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<img srcSet='", Suffix = "' >")>]        
        member _.srcSet
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<img usemap='", Suffix = "' >")>]        
        member _.usemap
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<img useMap='", Suffix = "' >")>]        
        member _.useMap
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.width
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<img crossOrigin='", Suffix = "' >")>]        
        member _.crossOrigin
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<img elementtiming='", Suffix = "' >")>]        
        member _.elementtiming
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<img fetchpriority='", Suffix = "' >")>]        
        member _.fetchpriority
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type InputHTMLAttributes with
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<input accept='", Suffix = "' >")>]        
        member _.accept
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<input alt='", Suffix = "' >")>]        
        member _.alt
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<input autocomplete='", Suffix = "' >")>]        
        member _.autocomplete
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<input autocorrect='", Suffix = "' >")>]        
        member _.autocorrect
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.autofocus
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<input capture='", Suffix = "' >")>]        
        member _.capture
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.checked'
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<input crossorigin='", Suffix = "' >")>]        
        member _.crossorigin
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<input disabled='", Suffix = "' >")>]        
        member _.disabled
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<input enterkeyhint='", Suffix = "' >")>]        
        member _.enterkeyhint
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<input form='", Suffix = "' >")>]        
        member _.form
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.formNoValidate
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<input formtarget='", Suffix = "' >")>]        
        member _.formtarget
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.height
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<input incremental='", Suffix = "' >")>]        
        member _.incremental
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<input list='", Suffix = "' >")>]        
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
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<input name='", Suffix = "' >")>]        
        member _.name
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<input pattern='", Suffix = "' >")>]        
        member _.pattern
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<input placeholder='", Suffix = "' >")>]        
        member _.placeholder
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.readOnly
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<input results='", Suffix = "' >")>]        
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
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<input src='", Suffix = "' >")>]        
        member _.src
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<input step='", Suffix = "' >")>]        
        member _.step
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<input type='", Suffix = "' >")>]        
        member _.type'
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<input value='", Suffix = "' >")>]        
        member _.value
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.width
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<input crossOrigin='", Suffix = "' >")>]        
        member _.crossOrigin
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<input formAction='", Suffix = "' >")>]        
        member _.formAction
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<input formEnctype='", Suffix = "' >")>]        
        member _.formEnctype
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<input formMethod='", Suffix = "' >")>]        
        member _.formMethod
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.formnovalidate
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<input formTarget='", Suffix = "' >")>]        
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
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<ins cite='", Suffix = "' >")>]        
        member _.cite
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<ins dateTime='", Suffix = "' >")>]        
        member _.dateTime
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type KeygenHTMLAttributes with
        [<Erase>]
        member _.autofocus
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<keygen challenge='", Suffix = "' >")>]        
        member _.challenge
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.disabled
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<keygen form='", Suffix = "' >")>]        
        member _.form
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<keygen keytype='", Suffix = "' >")>]        
        member _.keytype
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<keygen keyparams='", Suffix = "' >")>]        
        member _.keyparams
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<keygen name='", Suffix = "' >")>]        
        member _.name
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type LabelHTMLAttributes with
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<label for='", Suffix = "' >")>]        
        member _.for'
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<label form='", Suffix = "' >")>]        
        member _.form
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type LiHTMLAttributes with
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<li value='", Suffix = "' >")>]        
        member _.value
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type LinkHTMLAttributes with
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<link as='", Suffix = "' >")>]        
        member _.as'
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<link crossorigin='", Suffix = "' >")>]        
        member _.crossorigin
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.disabled
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<link fetchpriority='", Suffix = "' >")>]        
        member _.fetchpriority
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<link href='", Suffix = "' >")>]        
        member _.href
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<link hreflang='", Suffix = "' >")>]        
        member _.hreflang
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<link imagesizes='", Suffix = "' >")>]        
        member _.imagesizes
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<link imagesrcset='", Suffix = "' >")>]        
        member _.imagesrcset
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<link integrity='", Suffix = "' >")>]        
        member _.integrity
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<link media='", Suffix = "' >")>]        
        member _.media
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<link referrerpolicy='", Suffix = "' >")>]        
        member _.referrerpolicy
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<link rel='", Suffix = "' >")>]        
        member _.rel
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<link sizes='", Suffix = "' >")>]        
        member _.sizes
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<link type='", Suffix = "' >")>]        
        member _.type'
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<link crossOrigin='", Suffix = "' >")>]        
        member _.crossOrigin
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<link referrerPolicy='", Suffix = "' >")>]        
        member _.referrerPolicy
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type MapHTMLAttributes with
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<map name='", Suffix = "' >")>]        
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
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<media controlslist='", Suffix = "' >")>]        
        member _.controlslist
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<media crossorigin='", Suffix = "' >")>]        
        member _.crossorigin
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.loop
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<media mediagroup='", Suffix = "' >")>]        
        member _.mediagroup
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.muted
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<media preload='", Suffix = "' >")>]        
        member _.preload
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<media src='", Suffix = "' >")>]        
        member _.src
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<media crossOrigin='", Suffix = "' >")>]        
        member _.crossOrigin
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<media mediaGroup='", Suffix = "' >")>]        
        member _.mediaGroup
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type MenuHTMLAttributes with
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<menu label='", Suffix = "' >")>]        
        member _.label
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<menu type='", Suffix = "' >")>]        
        member _.type'
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<menu charset='", Suffix = "' >")>]        
        member _.charset
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<menu content='", Suffix = "' >")>]        
        member _.content
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<menu http='", Suffix = "' >")>]        
        member _.http
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<menu name='", Suffix = "' >")>]        
        member _.name
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<menu media='", Suffix = "' >")>]        
        member _.media
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type MeterHTMLAttributes with
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<meter form='", Suffix = "' >")>]        
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
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<quote cite='", Suffix = "' >")>]        
        member _.cite
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type ObjectHTMLAttributes with
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<object data='", Suffix = "' >")>]        
        member _.data
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<object form='", Suffix = "' >")>]        
        member _.form
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.height
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<object name='", Suffix = "' >")>]        
        member _.name
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<object type='", Suffix = "' >")>]        
        member _.type'
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<object usemap='", Suffix = "' >")>]        
        member _.usemap
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.width
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<object useMap='", Suffix = "' >")>]        
        member _.useMap
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type OlHTMLAttributes with
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<ol reversed='", Suffix = "' >")>]        
        member _.reversed
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<ol start='", Suffix = "' >")>]        
        member _.start
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<ol type='", Suffix = "' >")>]        
        member _.type'
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type OptgroupHTMLAttributes with
        [<Erase>]
        member _.disabled
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<optgroup label='", Suffix = "' >")>]        
        member _.label
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type OptionHTMLAttributes with
        [<Erase>]
        member _.disabled
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<option label='", Suffix = "' >")>]        
        member _.label
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.selected
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<option value='", Suffix = "' >")>]        
        member _.value
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type OutputHTMLAttributes with
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<output form='", Suffix = "' >")>]        
        member _.form
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<output for='", Suffix = "' >")>]        
        member _.for'
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<output name='", Suffix = "' >")>]        
        member _.name
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type ParamHTMLAttributes with
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<param name='", Suffix = "' >")>]        
        member _.name
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<param value='", Suffix = "' >")>]        
        member _.value
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type ProgressHTMLAttributes with
        [<Erase>]
        member _.max
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<progress value='", Suffix = "' >")>]        
        member _.value
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type ScriptHTMLAttributes with
        [<Erase>]
        member _.async
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<script charset='", Suffix = "' >")>]        
        member _.charset
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<script crossorigin='", Suffix = "' >")>]        
        member _.crossorigin
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<script defer='", Suffix = "' >")>]        
        member _.defer
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<script integrity='", Suffix = "' >")>]        
        member _.integrity
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.noModule
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<script nonce='", Suffix = "' >")>]        
        member _.nonce
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<script referrerpolicy='", Suffix = "' >")>]        
        member _.referrerpolicy
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<script src='", Suffix = "' >")>]        
        member _.src
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<script type='", Suffix = "' >")>]        
        member _.type'
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<script crossOrigin='", Suffix = "' >")>]        
        member _.crossOrigin
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.nomodule
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<script referrerPolicy='", Suffix = "' >")>]        
        member _.referrerPolicy
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type SelectHTMLAttributes with
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<select autocomplete='", Suffix = "' >")>]        
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
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<select form='", Suffix = "' >")>]        
        member _.form
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.multiple
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<select name='", Suffix = "' >")>]        
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
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<select value='", Suffix = "' >")>]        
        member _.value
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type HTMLSlotElementAttributes with
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<slot name='", Suffix = "' >")>]        
        member _.name
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type SourceHTMLAttributes with
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<source media='", Suffix = "' >")>]        
        member _.media
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<source sizes='", Suffix = "' >")>]        
        member _.sizes
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<source src='", Suffix = "' >")>]        
        member _.src
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<source srcset='", Suffix = "' >")>]        
        member _.srcset
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<source type='", Suffix = "' >")>]        
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
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<style media='", Suffix = "' >")>]        
        member _.media
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<style nonce='", Suffix = "' >")>]        
        member _.nonce
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<style scoped='", Suffix = "' >")>]        
        member _.scoped
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<style type='", Suffix = "' >")>]        
        member _.type'
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type TdHTMLAttributes with
        [<Erase>]
        member _.colspan
            with set(_: int) = ()
            and [<Erase>] get(): int = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<td headers='", Suffix = "' >")>]        
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
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<template content='", Suffix = "' >")>]        
        member _.content
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type TextareaHTMLAttributes with
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<textarea autocomplete='", Suffix = "' >")>]        
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
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<textarea dirname='", Suffix = "' >")>]        
        member _.dirname
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        member _.disabled
            with set(_: bool) = ()
            and [<Erase>] get(): bool = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<textarea enterkeyhint='", Suffix = "' >")>]        
        member _.enterkeyhint
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<textarea form='", Suffix = "' >")>]        
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
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<textarea name='", Suffix = "' >")>]        
        member _.name
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<textarea placeholder='", Suffix = "' >")>]        
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
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<textarea value='", Suffix = "' >")>]        
        member _.value
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<textarea wrap='", Suffix = "' >")>]        
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
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<th headers='", Suffix = "' >")>]        
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
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<th scope='", Suffix = "' >")>]        
        member _.scope
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type TimeHTMLAttributes with
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<time datetime='", Suffix = "' >")>]        
        member _.datetime
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<time dateTime='", Suffix = "' >")>]        
        member _.dateTime
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            

    type TrackHTMLAttributes with
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<track default='", Suffix = "' >")>]        
        member _.default'
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<track kind='", Suffix = "' >")>]        
        member _.kind
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<track label='", Suffix = "' >")>]        
        member _.label
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<track src='", Suffix = "' >")>]        
        member _.src
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
            
        [<Erase>]
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<track srclang='", Suffix = "' >")>]        
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
        [<LanguageInjection(InjectedLanguage.HTML, Prefix = "<video poster='", Suffix = "' >")>]        
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
    
    type CoreSVGAttributes with
        [<Erase>]
        member _.id 
            with set(_: string) = ()
            and get() = unbox () 
        [<Erase>]
        member _.lang 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.tabIndex 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.tabindex 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
    type StylableSVGAttributes with
        [<Erase>]
        member _.class' 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.style 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
    type TransformableSVGAttributes with
        [<Erase>]
        member _.transform 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
    type ConditionalProcessingSVGAttributes with
        [<Erase>]
        member _.requiredExtensions 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.requiredFeatures 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.systemLanguage 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
    type ExternalResourceSVGAttributes with
        [<Erase>]
        member _.externalResourcesRequired 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
    type AnimationTimingSVGAttributes with
        [<Erase>]
        member _.begin' 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.dur 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.end' 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.min 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.max 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.restart 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase>]
        member _.repeatCount 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase>]
        member _.repeatDur 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.fill 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
    type AnimationValueSVGAttributes with
        [<Erase>]
        member _.calcMode 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase>]
        member _.values 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.keyTimes 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.keySplines 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.from 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.to' 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.by 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
    type AnimationAdditionSVGAttributes with
        [<Erase>]
        member _.attributeName 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.additive 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.accumulate 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
    type AnimationAttributeTargetSVGAttributes with
        [<Erase>]
        member _.attributeName 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.attributeType 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
    
    type ContainerElementSVGAttributes with
        [<Erase>]
        member _.``color-rendering`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
        member this.colorRendering
            with inline set(value: string) = this.``color-rendering`` <- value
            and inline get() = this.``color-rendering``
        [<Erase>]
        member _.``clip-path`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
        member this.clipPath
            with inline set(value: string) = this.``clip-path`` <- value
            and inline get() = this.``clip-path``
        [<Erase>]
        member _.cursor 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.``color-interpolation`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
        member this.colorInterpolation
            with inline set(value: string) = this.``color-interpolation`` <- value
            and inline get() = this.``color-interpolation``
        [<Erase>]
        member _.``enable-background`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
        member this.enableBackground
            with inline set(value: string) = this.``enable-background`` <- value
            and inline get() = this.``enable-background``
        [<Erase>]
        member _.filter 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.mask 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.opacity 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
    
    type GraphicsElementSVGAttributes with
        [<Erase>]
        member _.``clip-rule`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
        member this.clipRule
            with inline set(value: string) = this.``clip-rule`` <- value
            and inline get() = this.``clip-rule``
        [<Erase>]
        member _.mask 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.``pointer-events`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
        member this.pointerEvents
            with inline set(value: string) = this.``pointer-events`` <- value
            and inline get() = this.``pointer-events``
        [<Erase>]
        member _.cursor 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.opacity 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.filter 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.display 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.visibility 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.``color-interpolation`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.colorInterpolation
            with inline set(value: string) = this.``color-interpolation`` <- value
            and inline get() = this.``color-interpolation``
        [<Erase>]
        member _.``color-rendering`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.colorRendering
            with inline set(value: string) = this.``color-rendering`` <- value
            and inline get() = this.``color-rendering``

        [<Erase>]
        member _.stroke 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.``stroke-dasharray`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.strokeDasharray
            with inline set(value: string) = this.``stroke-dasharray`` <- value
            and inline get() = this.``stroke-dasharray``
        [<Erase>]
        member _.``stroke-dashoffset`` 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        member this.strokeDashoffset
            with inline set(value: string) = this.``stroke-dashoffset`` <- !^value
            and inline get() = this.``stroke-dashoffset``
        [<Erase>]
        member _.``stroke-linecap`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.strokeLinecap
            with inline set(value: string) = this.``stroke-linecap`` <- value
            and inline get() = this.``stroke-linecap``
        [<Erase>]
        member _.``stroke-linejoin`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.strokeLinejoin
            with inline set(value: string) = this.``stroke-linejoin`` <- value
            and inline get() = this.``stroke-linejoin``
        [<Erase>]
        member _.``stroke-miterlimit`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.strokeMiterlimit
            with inline set(value: string) = this.``stroke-miterlimit`` <- value
            and inline get() = this.``stroke-miterlimit``
        [<Erase>]
        member _.``stroke-opacity`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.strokeOpacity
            with inline set(value: string) = this.``stroke-opacity`` <- value
            and inline get() = this.``stroke-opacity``
        [<Erase>]
        member _.``stroke-width`` 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        member this.strokeWidth
            with inline set(value: string) = this.``stroke-width`` <- !^value
            and inline get() = this.``stroke-width``
        [<Erase>]
        member _.color 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.fill 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.``fill-opacity`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.fillOpacity
            with inline set(value: string) = this.``fill-opacity`` <- value
            and inline get() = this.``fill-opacity``
        [<Erase>]
        member _.``fill-rule`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.fillRule
            with inline set(value: string) = this.``fill-rule`` <- value
            and inline get() = this.``fill-rule``
        [<Erase>]
        member _.``shape-rendering`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.shapeRendering
            with inline set(value: string) = this.``shape-rendering`` <- value
            and inline get() = this.``shape-rendering``
        [<Erase>]
        member _.pathLength 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 

    type TextContentElementSVGAttributes with
        [<Erase>]
        member _.``font-family`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.fontFamily
            with inline set(value: string) = this.``font-family`` <- value
            and inline get() = this.``font-family``
        [<Erase>]
        member _.``font-size`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.fontSize
            with inline set(value: string) = this.``font-size`` <- value
            and inline get() = this.``font-size``
        [<Erase>]
        member _.``font-size-adjust`` 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        member this.fontSizeAdjust
            with inline set(value: string) = this.``font-size-adjust`` <- !^value
            and inline get() = this.``font-size-adjust``
        [<Erase>]
        member _.``font-stretch`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.fontStretch
            with inline set(value: string) = this.``font-stretch`` <- value
            and inline get() = this.``font-stretch``
        [<Erase>]
        member _.``font-style`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.fontStyle
            with inline set(value: string) = this.``font-style`` <- value
            and inline get() = this.``font-style``
        [<Erase>]
        member _.``font-variant`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.fontVariant
            with inline set(value: string) = this.``font-variant`` <- value
            and inline get() = this.``font-variant``
        [<Erase>]
        member _.``font-weight`` 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        member this.fontWeight
            with inline set(value: string) = this.``font-weight`` <- !^value
            and inline get() = this.``font-weight``
        [<Erase>]
        member _.kerning 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.``letter-spacing`` 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        member this.letterSpacing
            with inline set(value: string) = this.``letter-spacing`` <- !^value
            and inline get() = this.``letter-spacing``
        [<Erase>]
        member _.``word-spacing`` 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        member this.wordSpacing
            with inline set(value: string) = this.``word-spacing`` <- !^value
            and inline get() = this.``word-spacing``
        [<Erase>]
        member _.``text-decoration`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.textDecoration
            with inline set(value: string) = this.``text-decoration`` <- value
            and inline get() = this.``text-decoration``
        [<Erase>]
        member _.``glyph-orientation-horizontal`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.glyphOrientationHorizontal
            with inline set(value: string) = this.``glyph-orientation-horizontal`` <- value
            and inline get() = this.``glyph-orientation-horizontal``
        [<Erase>]
        member _.``glyph-orientation-vertical`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.glyphOrientationVertical
            with inline set(value: string) = this.``glyph-orientation-vertical`` <- value
            and inline get() = this.``glyph-orientation-vertical``
        [<Erase>]
        member _.direction 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.``unicode-bidi`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.unicodeBidi
            with inline set(value: string) = this.``unicode-bidi`` <- value
            and inline get() = this.``unicode-bidi``
        [<Erase>]
        member _.``text-anchor`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.textAnchor
            with inline set(value: string) = this.``text-anchor`` <- value
            and inline get() = this.``text-anchor``
        [<Erase>]
        member _.``dominant-baseline`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.dominantBaseline
            with inline set(value: string) = this.``dominant-baseline`` <- value
            and inline get() = this.``dominant-baseline``
        [<Erase>]
        member _.color 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.fill 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.``fill-opacity`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.fillOpacity
            with inline set(value: string) = this.``fill-opacity`` <- value
            and inline get() = this.``fill-opacity``
        [<Erase>]
        member _.``fill-rule`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.fillRule
            with inline set(value: string) = this.``fill-rule`` <- value
            and inline get() = this.``fill-rule``
        [<Erase>]
        member _.stroke 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.``stroke-dasharray`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.strokeDasharray
            with inline set(value: string) = this.``stroke-dasharray`` <- value
            and inline get() = this.``stroke-dasharray``
        [<Erase>]
        member _.``stroke-dashoffset`` 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        member this.strokeDashoffset
            with inline set(value: string) = this.``stroke-dashoffset`` <- !^value
            and inline get() = this.``stroke-dashoffset``
        [<Erase>]
        member _.``stroke-linecap`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.strokeLinecap
            with inline set(value: string) = this.``stroke-linecap`` <- value
            and inline get() = this.``stroke-linecap``
        [<Erase>]
        member _.``stroke-linejoin`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.strokeLinejoin
            with inline set(value: string) = this.``stroke-linejoin`` <- value
            and inline get() = this.``stroke-linejoin``
        [<Erase>]
        member _.``stroke-miterlimit`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.strokeMiterlimit
            with inline set(value: string) = this.``stroke-miterlimit`` <- value
            and inline get() = this.``stroke-miterlimit``
        [<Erase>]
        member _.``stroke-opacity`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.strokeOpacity
            with inline set(value: string) = this.``stroke-opacity`` <- value
            and inline get() = this.``stroke-opacity``
        [<Erase>]
        member _.``stroke-width`` 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        member this.strokeWidth
            with inline set(value: string) = this.``stroke-width`` <- !^value
            and inline get() = this.``stroke-width``

    type AnimateSVGAttributes with
        [<Erase>]
        member _.``color-interpolation`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.colorInterpolation
            with inline set(value: string) = this.``color-interpolation`` <- value
            and inline get() = this.``color-interpolation``
        [<Erase>]
        member _.``color-rendering`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.colorRendering
            with inline set(value: string) = this.``color-rendering`` <- value
            and inline get() = this.``color-rendering``
    
    type SwitchSVGAttributes with
        [<Erase>]
        member _.display 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.visibility 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        
        
    type PresentationSVGAttributes with
        [<Erase>]
        member _.``alignment-baseline`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.alignmentBaseline
            with inline set(value: string) = this.``alignment-baseline`` <- value
            and inline get() = this.``alignment-baseline``
        [<Erase>]
        member _.``baseline-shift`` 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        member this.baselineShift
            with inline set(value: string) = this.``baseline-shift`` <- !^value
            and inline get() = this.``baseline-shift``
        [<Erase>]
        member _.clip 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.``clip-path`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.clipPath
            with inline set(value: string) = this.``clip-path`` <- value
            and inline get() = this.``clip-path``
        [<Erase>]
        member _.``clip-rule`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.clipRule
            with inline set(value: string) = this.``clip-rule`` <- value
            and inline get() = this.``clip-rule``
        [<Erase>]
        member _.color 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.``color-interpolation`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.colorInterpolation
            with inline set(value: string) = this.``color-interpolation`` <- value
            and inline get() = this.``color-interpolation``
        [<Erase>]
        member _.``color-interpolation-filters`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.colorInterpolationFilters
            with inline set(value: string) = this.``color-interpolation-filters`` <- value
            and inline get() = this.``color-interpolation-filters``
        [<Erase>]
        member _.``color-profile`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.colorProfile
            with inline set(value: string) = this.``color-profile`` <- value
            and inline get() = this.``color-profile``
        [<Erase>]
        member _.``color-rendering`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.colorRendering
            with inline set(value: string) = this.``color-rendering`` <- value
            and inline get() = this.``color-rendering``
        [<Erase>]
        member _.cursor 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.direction 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.display 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.``dominant-baseline`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.dominantBaseline
            with inline set(value: string) = this.``dominant-baseline`` <- value
            and inline get() = this.``dominant-baseline``
        [<Erase>]
        member _.``enable-background`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.enableBackground
            with inline set(value: string) = this.``enable-background`` <- value
            and inline get() = this.``enable-background``
        [<Erase>]
        member _.fill 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.``fill-opacity`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.fillOpacity
            with inline set(value: string) = this.``fill-opacity`` <- value
            and inline get() = this.``fill-opacity``
        [<Erase>]
        member _.``fill-rule`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.fillRule
            with inline set(value: string) = this.``fill-rule`` <- value
            and inline get() = this.``fill-rule``
        [<Erase>]
        member _.filter 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.``flood-color`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.floodColor
            with inline set(value: string) = this.``flood-color`` <- value
            and inline get() = this.``flood-color``
        [<Erase>]
        member _.``flood-opacity`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.floodOpacity
            with inline set(value: string) = this.``flood-opacity`` <- value
            and inline get() = this.``flood-opacity``
        [<Erase>]
        member _.``font-family`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.fontFamily
            with inline set(value: string) = this.``font-family`` <- value
            and inline get() = this.``font-family``
        [<Erase>]
        member _.``font-size`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.fontSize
            with inline set(value: string) = this.``font-size`` <- value
            and inline get() = this.``font-size``
        [<Erase>]
        member _.``font-size-adjust`` 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        member this.fontSizeAdjust
            with inline set(value: string) = this.``font-size-adjust`` <- !^value
            and inline get() = this.``font-size-adjust``
        [<Erase>]
        member _.``font-stretch`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.fontStretch
            with inline set(value: string) = this.``font-stretch`` <- value
            and inline get() = this.``font-stretch``
        [<Erase>]
        member _.``font-style`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.fontStyle
            with inline set(value: string) = this.``font-style`` <- value
            and inline get() = this.``font-style``
        [<Erase>]
        member _.``font-variant`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.fontVariant
            with inline set(value: string) = this.``font-variant`` <- value
            and inline get() = this.``font-variant``
        [<Erase>]
        member _.``font-weight`` 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        member this.fontWeight
            with inline set(value: string) = this.``font-weight`` <- !^value
            and inline get() = this.``font-weight``
        [<Erase>]
        member _.``glyph-orientation-horizontal`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.glyphOrientationHorizontal
            with inline set(value: string) = this.``glyph-orientation-horizontal`` <- value
            and inline get() = this.``glyph-orientation-horizontal``
        [<Erase>]
        member _.``glyph-orientation-vertical`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.glyphOrientationVertical
            with inline set(value: string) = this.``glyph-orientation-vertical`` <- value
            and inline get() = this.``glyph-orientation-vertical``
        [<Erase>]
        member _.``image-rendering`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.imageRendering
            with inline set(value: string) = this.``image-rendering`` <- value
            and inline get() = this.``image-rendering``
        [<Erase>]
        member _.kerning 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.``letter-spacing`` 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        member this.letterSpacing
            with inline set(value: string) = this.``letter-spacing`` <- !^value
            and inline get() = this.``letter-spacing``
        [<Erase>]
        member _.``lighting-color`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.lightingColor
            with inline set(value: string) = this.``lighting-color`` <- value
            and inline get() = this.``lighting-color``
        [<Erase>]
        member _.``marker-end`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.markerEnd
            with inline set(value: string) = this.``marker-end`` <- value
            and inline get() = this.``marker-end``
        [<Erase>]
        member _.``marker-mid`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.markerMid
            with inline set(value: string) = this.``marker-mid`` <- value
            and inline get() = this.``marker-mid``
        [<Erase>]
        member _.``marker-start`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.markerStart
            with inline set(value: string) = this.``marker-start`` <- value
            and inline get() = this.``marker-start``
        [<Erase>]
        member _.mask 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.opacity 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.overflow 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.pathLength 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.``pointer-events`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.pointerEvents
            with inline set(value: string) = this.``pointer-events`` <- value
            and inline get() = this.``pointer-events``
        [<Erase>]
        member _.``shape-rendering`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.shapeRendering
            with inline set(value: string) = this.``shape-rendering`` <- value
            and inline get() = this.``shape-rendering``
        [<Erase>]
        member _.``stop-color`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.stopColor
            with inline set(value: string) = this.``stop-color`` <- value
            and inline get() = this.``stop-color``
        [<Erase>]
        member _.``stop-opacity`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.stopOpacity
            with inline set(value: string) = this.``stop-opacity`` <- value
            and inline get() = this.``stop-opacity``
        [<Erase>]
        member _.stroke 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.``stroke-dasharray`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.strokeDasharray
            with inline set(value: string) = this.``stroke-dasharray`` <- value
            and inline get() = this.``stroke-dasharray``
        [<Erase>]
        member _.``stroke-dashoffset`` 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        member this.strokeDashoffset
            with inline set(value: string) = this.``stroke-dashoffset`` <- !^value
            and inline get() = this.``stroke-dashoffset``
        [<Erase>]
        member _.``stroke-linecap`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.strokeLinecap
            with inline set(value: string) = this.``stroke-linecap`` <- value
            and inline get() = this.``stroke-linecap``
        [<Erase>]
        member _.``stroke-linejoin`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.strokeLinejoin
            with inline set(value: string) = this.``stroke-linejoin`` <- value
            and inline get() = this.``stroke-linejoin``
        [<Erase>]
        member _.``stroke-miterlimit`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.strokeMiterlimit
            with inline set(value: string) = this.``stroke-miterlimit`` <- value
            and inline get() = this.``stroke-miterlimit``
        [<Erase>]
        member _.``stroke-opacity`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.strokeOpacity
            with inline set(value: string) = this.``stroke-opacity`` <- value
            and inline get() = this.``stroke-opacity``
        [<Erase>]
        member _.``stroke-width`` 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        member this.strokeWidth
            with inline set(value: string) = this.``stroke-width`` <- !^value
            and inline get() = this.``stroke-width``
        [<Erase>]
        member _.``text-anchor`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.textAnchor
            with inline set(value: string) = this.``text-anchor`` <- value
            and inline get() = this.``text-anchor``
        [<Erase>]
        member _.``text-decoration`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.textDecoration
            with inline set(value: string) = this.``text-decoration`` <- value
            and inline get() = this.``text-decoration``
        [<Erase>]
        member _.``text-rendering`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.textRendering
            with inline set(value: string) = this.``text-rendering`` <- value
            and inline get() = this.``text-rendering``
        [<Erase>]
        member _.``unicode-bidi`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.unicodeBidi
            with inline set(value: string) = this.``unicode-bidi`` <- value
            and inline get() = this.``unicode-bidi``
        [<Erase>]
        member _.visibility 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.``word-spacing`` 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        member this.wordSpacing
            with inline set(value: string) = this.``word-spacing`` <- !^value
            and inline get() = this.``word-spacing``
        [<Erase>]
        member _.``writing-mode`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.writingMode
            with inline set(value: string) = this.``writing-mode`` <- value
            and inline get() = this.``writing-mode``


    type FilterPrimitiveElementSVGAttributes with
        [<Erase>]
        member _.x 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.y 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.width 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.height 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.result 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.``color-interpolation-filters`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.colorInterpolationFilters
            with inline set(value: string) = this.``color-interpolation-filters`` <- value
            and inline get() = this.``color-interpolation-filters``

    type SingleInputFilterSVGAttributes with
        [<Erase>]
        member _.in'
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
    type DoubleInputFilterSVGAttributes with
        [<Erase>]
        member _.in' 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.in2 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
    type FitToViewBoxSVGAttributes with
        [<Erase>]
        member _.viewBox 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.preserveAspectRatio 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
    type GradientElementSVGAttributes with
        [<Erase>]
        member _.gradientUnits 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.gradientTransform 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.spreadMethod 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.href 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
    type NewViewportSVGAttributes with
        [<Erase>]
        member _.viewBox 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.overflow 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
        [<Erase>]
        member _.clip 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
    type ZoomAndPanSVGAttributes with
        [<Erase>]
        member _.zoomAndPan 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
    type AnimateMotionSVGAttributes with
        [<Erase>]
        member _.path 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.keyPoints 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.rotate 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.origin 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
    type AnimateTransformSVGAttributes with
        [<Erase>]
        member _.type' 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
    type CircleSVGAttributes with
        [<Erase>]
        member _.cx 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.cy 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.r 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
    type ClipPathSVGAttributes with
        [<Erase>]
        member _.clipPathUnits 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.``clip-path`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.clipPath
            with inline set(value: string) = this.``clip-path`` <- value
            and inline get() = this.``clip-path``
        
    type EllipseSVGAttributes with
        [<Erase>]
        member _.cx 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.cy 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.rx 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.ry 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
    type FeBlendSVGAttributes with
        [<Erase>]
        member _.mode 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
    type FeColorMatrixSVGAttributes with
        [<Erase>]
        member _.type' 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.values 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
    type FeCompositeSVGAttributes with
        [<Erase>]
        member _.operator 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.k1 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.k2 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.k3 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.k4 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
    type FeConvolveMatrixSVGAttributes with
        [<Erase>]
        member _.order 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.kernelMatrix 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.divisor 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.bias 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.targetX 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.targetY 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.edgeMode 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.kernelUnitLength 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.preserveAlpha 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
    type FeDiffuseLightingSVGAttributes with
        [<Erase>]
        member _.``lighting-color`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.lightingColor
            with inline set(value: string) = this.``lighting-color`` <- value
            and inline get() = this.``lighting-color``
        [<Erase>]
        member _.color 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.surfaceScale 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.diffuseConstant 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.kernelUnitLength 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
    type FeDisplacementMapSVGAttributes with
        [<Erase>]
        member _.scale 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.xChannelSelector 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.yChannelSelector 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
    type FeDistantLightSVGAttributes with
        [<Erase>]
        member _.azimuth 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.elevation 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
    type FeDropShadowSVGAttributes with        
        [<Erase>]
        member _.color 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.``flood-color`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.floodColor
            with inline set(value: string) = this.``flood-color`` <- value
            and inline get() = this.``flood-color``
        [<Erase>]
        member _.``flood-opacity`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.floodOpacity
            with inline set(value: string) = this.``flood-opacity`` <- value
            and inline get() = this.``flood-opacity``
        [<Erase>]
        member _.dx 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.dy 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.stdDeviation 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
    type FeFloodSVGAttributes with            
        [<Erase>]
        member _.color 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.``flood-color`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.floodColor
            with inline set(value: string) = this.``flood-color`` <- value
            and inline get() = this.``flood-color``
        [<Erase>]
        member _.``flood-opacity`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.floodOpacity
            with inline set(value: string) = this.``flood-opacity`` <- value
            and inline get() = this.``flood-opacity``

    type FeFuncSVGAttributes with
        [<Erase>]
        member _.type' 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.tableValues 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.slope 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.intercept 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.amplitude 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.exponent 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.offset 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
    type FeGaussianBlurSVGAttributes with
        [<Erase>]
        member _.stdDeviation 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
    type FeImageSVGAttributes with
        [<Erase>]
        member _.preserveAspectRatio 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.href 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
    type FeMorphologySVGAttributes with
        [<Erase>]
        member _.operator 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.radius 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
    type FeOffsetSVGAttributes with
        [<Erase>]
        member _.dx 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.dy 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
    type FePointLightSVGAttributes with
        [<Erase>]
        member _.x 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.y 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.z 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
    type FeSpecularLightingSVGAttributes with
        [<Erase>]
        member _.``lighting-color`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.lightingColor
            with inline set(value: string) = this.``lighting-color`` <- value
            and inline get() = this.``lighting-color``
        [<Erase>]
        member _.color 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.surfaceScale 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.specularConstant 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.specularExponent 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.kernelUnitLength 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
    type FeSpotLightSVGAttributes with
        [<Erase>]
        member _.x 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.y 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.z 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.pointsAtX 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.pointsAtY 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.pointsAtZ 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.specularExponent 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.limitingConeAngle 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
    type FeTurbulanceSVGAttributes with
        [<Erase>]
        member _.baseFrequency 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.numOctaves 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.seed 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.stitchTiles 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.type' 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
    type FilterSVGAttributes with
        [<Erase>]
        member _.filterUnits 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.primitiveUnits 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.x 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.y 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.width 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.height 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.filterRes 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
    type ForeignObjectSVGAttributes with
        [<Erase>]
        member _.visibility 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.display 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.x 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.y 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.width 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.height 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
    type GSVGAttributes with
        [<Erase>]
        member _.visibility 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.display 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 

    type ImageSVGAttributes with
        [<Erase>]
        member _.``image-rendering`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.imageRendering
            with inline set(value: string) = this.``image-rendering`` <- value
            and inline get() = this.``image-rendering``
        [<Erase>]
        member _.``color-profile`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.colorProfile
            with inline set(value: string) = this.``color-profile`` <- value
            and inline get() = this.``color-profile``
        [<Erase>]
        member _.x 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.y 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.width 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.height 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.preserveAspectRatio 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.href 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
    type LineSVGAttributes with
        [<Erase>]
        member _.``marker-end`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.markerEnd
            with inline set(value: string) = this.``marker-end`` <- value
            and inline get() = this.``marker-end``
        [<Erase>]
        member _.``marker-mid`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.markerMid
            with inline set(value: string) = this.``marker-mid`` <- value
            and inline get() = this.``marker-mid``
        [<Erase>]
        member _.``marker-start`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.markerStart
            with inline set(value: string) = this.``marker-start`` <- value
            and inline get() = this.``marker-start``
        [<Erase>]
        member _.x1 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.y1 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.x2 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.y2 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
    type LinearGradientSVGAttributes with
        [<Erase>]
        member _.x1 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.x2 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.y1 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.y2 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
    type MarkerSVGAttributes with
        [<Erase>]
        member _.clip 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.overflow 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.markerUnits 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.refX 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.refY 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.markerWidth 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.markerHeight 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.orient 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
    type MaskSVGAttributes with
        [<Erase>]
        member _.filter 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.opacity 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.maskUnits 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.maskContentUnits 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.x 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.y 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.width 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.height 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
    type PathSVGAttributes with
        [<Erase>]
        member _.``marker-end`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.markerEnd
            with inline set(value: string) = this.``marker-end`` <- value
            and inline get() = this.``marker-end``
        [<Erase>]
        member _.``marker-mid`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.markerMid
            with inline set(value: string) = this.``marker-mid`` <- value
            and inline get() = this.``marker-mid``
        [<Erase>]
        member _.``marker-start`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.markerStart
            with inline set(value: string) = this.``marker-start`` <- value
            and inline get() = this.``marker-start``
        [<Erase>]
        member _.d 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.pathLength 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
    type PatternSVGAttributes with
        [<Erase>]
        member _.x 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.y 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.width 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.height 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.patternUnits 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.patternContentUnits 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.patternTransform 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.href 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.clip 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.overflow 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        
    type PolygonSVGAttributes with
        [<Erase>]
        member _.``marker-end`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.markerEnd
            with inline set(value: string) = this.``marker-end`` <- value
            and inline get() = this.``marker-end``
        [<Erase>]
        member _.``marker-mid`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.markerMid
            with inline set(value: string) = this.``marker-mid`` <- value
            and inline get() = this.``marker-mid``
        [<Erase>]
        member _.``marker-start`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.markerStart
            with inline set(value: string) = this.``marker-start`` <- value
            and inline get() = this.``marker-start``
        [<Erase>]
        member _.points 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
    type PolylineSVGAttributes with
        [<Erase>]
        member _.``marker-end`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.markerEnd
            with inline set(value: string) = this.``marker-end`` <- value
            and inline get() = this.``marker-end``
        [<Erase>]
        member _.``marker-mid`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.markerMid
            with inline set(value: string) = this.``marker-mid`` <- value
            and inline get() = this.``marker-mid``
        [<Erase>]
        member _.``marker-start`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.markerStart
            with inline set(value: string) = this.``marker-start`` <- value
            and inline get() = this.``marker-start``
        [<Erase>]
        member _.points 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
    type RadialGradientSVGAttributes with
        [<Erase>]
        member _.cx 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.cy 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.r 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.fx 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.fy 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 

    type RectSVGAttributes with
        [<Erase>]
        member _.x 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.y 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.width 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.height 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.rx 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.ry 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
    type StopSVGAttributes with
        [<Erase>]
        member _.color 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.``stop-color`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.stopColor
            with inline set(value: string) = this.``stop-color`` <- value
            and inline get() = this.``stop-color``
        [<Erase>]
        member _.``stop-opacity`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.stopOpacity
            with inline set(value: string) = this.``stop-opacity`` <- value
            and inline get() = this.``stop-opacity``
        [<Erase>]
        member _.offset 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 

    type SvgSVGAttributes with
        [<Erase>]
        member _.version 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.baseProfile 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.x 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.y 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.width 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.height 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.contentScriptType 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.contentStyleType 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.xmlns 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 


    type SymbolSVGAttributes with
        [<Erase>]
        member _.width 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.height 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.preserveAspectRatio 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.refX 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.refY 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.viewBox 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.x 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.y 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 

    type TextSVGAttributes with
        [<Erase>]
        member _.x 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.y 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.dx 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.dy 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.rotate 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.textLength 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.lengthAdjust 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.``writing-mode`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.writingMode
            with inline set(value: string) = this.``writing-mode`` <- value
            and inline get() = this.``writing-mode``
        [<Erase>]
        member _.``text-rendering`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.textRendering
            with inline set(value: string) = this.``text-rendering`` <- value
            and inline get() = this.``text-rendering``

    type TextPathSVGAttributes with
        [<Erase>]
        member _.startOffset 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.method 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.spacing 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.``alignment-baseline`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.alignmentBaseline
            with inline set(value: string) = this.``alignment-baseline`` <- value
            and inline get() = this.``alignment-baseline``
        [<Erase>]
        member _.``baseline-shift`` 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        member this.baselineShift
            with inline set(value: string) = this.``baseline-shift`` <- !^value
            and inline get() = this.``baseline-shift``
        [<Erase>]
        member _.display 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.visibility 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.href 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 

    type TSpanSVGAttributes with
        [<Erase>]
        member _.x 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.y 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.dx 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.dy 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.rotate 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.textLength 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.lengthAdjust 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.``alignment-baseline`` 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        member this.alignmentBaseline
            with inline set(value: string) = this.``alignment-baseline`` <- value
            and inline get() = this.``alignment-baseline``
        [<Erase>]
        member _.``baseline-shift`` 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        member this.baselineShift
            with inline set(value: string) = this.``baseline-shift`` <- !^value
            and inline get() = this.``baseline-shift``
        [<Erase>]
        member _.display 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 
        [<Erase>]
        member _.visibility 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 

    type UseSVGAttributes with
        [<Erase>]
        member _.x 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.y 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.width 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.height 
            with set(_: U2<float, string>) = ()
            and [<Erase>] get(): U2<float, string> = unbox () 
        [<Erase>]
        member _.href 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox () 

    type ViewSVGAttributes with
        [<Erase>]
        member _.viewTarget 
            with set(_: string) = ()
            and [<Erase>] get(): string = unbox ()
