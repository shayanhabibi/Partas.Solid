namespace Partas.Solid

open System.Runtime.CompilerServices
open JetBrains.Annotations
open Fable.Core
open Fable.Core.JsInterop
open Partas.Solid.Experimental.U
open Fable.Core.TS
open Fable.Core.TS.Dom

/// Hand-written element members the JSX generator leaves alone: event handlers, children and the
/// structured `style`. Attributes are generated into HtmlAttributes.fs by tools/JsxGen.
[<AutoOpen>]
module HtmlEvents =
    type HtmlContainer with
        [<Erase>]
        member _.children
            with set (value: HtmlElement) = JS.undefined
            and get (): HtmlElement = JS.undefined

    type FlowContainer<'A> with
        [<Erase>]
        member _.children
            with set(value: 'A) = JS.undefined
            and get (): 'A = JS.undefined

    type HTMLAttributes with
        [<Erase>]
        member _.onCopy
            with set (value: ClipboardEvent -> unit) = ()
            and [<Erase>] get (): ClipboardEvent -> unit = unbox ()

        [<Erase>]
        member _.onCut
            with set (value: ClipboardEvent -> unit) = ()
            and [<Erase>] get (): ClipboardEvent -> unit = unbox ()

        [<Erase>]
        member _.onPaste
            with set (value: ClipboardEvent -> unit) = ()
            and [<Erase>] get (): ClipboardEvent -> unit = unbox ()

        [<Erase>]
        member _.onCompositionEnd
            with set (value: CompositionEvent -> unit) = ()
            and [<Erase>] get (): CompositionEvent -> unit = unbox ()

        [<Erase>]
        member _.onCompositionStart
            with set (value: CompositionEvent -> unit) = ()
            and [<Erase>] get (): CompositionEvent -> unit = unbox ()

        [<Erase>]
        member _.onCompositionUpdate
            with set (value: CompositionEvent -> unit) = ()
            and [<Erase>] get (): CompositionEvent -> unit = unbox ()

        [<Erase>]
        member _.onFocusOut
            with set (value: FocusEvent -> unit) = ()
            and [<Erase>] get (): FocusEvent -> unit = unbox ()

        [<Erase>]
        member _.onFocusIn
            with set (value: FocusEvent -> unit) = ()
            and [<Erase>] get (): FocusEvent -> unit = unbox ()

        [<Erase>]
        member _.onEncrypted
            with set (value: Event -> unit) = ()
            and [<Erase>] get (): Event -> unit = unbox ()

        [<Erase>]
        member _.onDragExit
            with set (value: DragEvent -> unit) = ()
            and [<Erase>] get (): DragEvent -> unit = unbox ()

        [<Erase>]
        member _.onAbort
            with set (value: Event -> unit) = ()
            and [<Erase>] get (): Event -> unit = unbox ()

        [<Erase>]
        member _.onAnimationEnd
            with set (value: AnimationEvent -> unit) = ()
            and [<Erase>] get (): AnimationEvent -> unit = unbox ()

        [<Erase>]
        member _.onAnimationIteration
            with set (value: AnimationEvent -> unit) = ()
            and [<Erase>] get (): AnimationEvent -> unit = unbox ()

        [<Erase>]
        member _.onAnimationStart
            with set (value: AnimationEvent -> unit) = ()
            and [<Erase>] get (): AnimationEvent -> unit = unbox ()

        [<Erase>]
        member _.onAuxClick
            with set (value: MouseEvent -> unit) = ()
            and [<Erase>] get (): MouseEvent -> unit = unbox ()

        [<Erase>]
        member _.onBeforeInput
            with set (value: InputEvent -> unit) = ()
            and [<Erase>] get (): InputEvent -> unit = unbox ()

        [<Erase>]
        member _.onBeforeToggle
            with set (value: ToggleEvent -> unit) = ()
            and [<Erase>] get (): ToggleEvent -> unit = unbox ()

        [<Erase>]
        member _.onBlur
            with set (value: FocusEvent -> unit) = ()
            and [<Erase>] get (): FocusEvent -> unit = unbox ()

        [<Erase>]
        member _.onCanPlay
            with set (value: Event -> unit) = ()
            and [<Erase>] get (): Event -> unit = unbox ()

        [<Erase>]
        member _.onCanPlayThrough
            with set (value: Event -> unit) = ()
            and [<Erase>] get (): Event -> unit = unbox ()

        [<Erase>]
        member _.onChange
            with set (value: Event -> unit) = ()
            and [<Erase>] get (): Event -> unit = unbox ()

        [<Erase>]
        member _.onClick
            with set (value: MouseEvent -> unit) = ()
            and [<Erase>] get (): MouseEvent -> unit = unbox ()

        [<Erase>]
        member _.onContextMenu
            with set (value: MouseEvent -> unit) = ()
            and [<Erase>] get (): MouseEvent -> unit = unbox ()

        [<Erase>]
        member _.onDblClick
            with set (value: MouseEvent -> unit) = ()
            and [<Erase>] get (): MouseEvent -> unit = unbox ()

        [<Erase>]
        member _.onDrag
            with set (value: DragEvent -> unit) = ()
            and [<Erase>] get (): DragEvent -> unit = unbox ()

        [<Erase>]
        member _.onDragEnd
            with set (value: DragEvent -> unit) = ()
            and [<Erase>] get (): DragEvent -> unit = unbox ()

        [<Erase>]
        member _.onDragEnter
            with set (value: DragEvent -> unit) = ()
            and [<Erase>] get (): DragEvent -> unit = unbox ()

        [<Erase>]
        member _.onDragLeave
            with set (value: DragEvent -> unit) = ()
            and [<Erase>] get (): DragEvent -> unit = unbox ()

        [<Erase>]
        member _.onDragOver
            with set (value: DragEvent -> unit) = ()
            and [<Erase>] get (): DragEvent -> unit = unbox ()

        [<Erase>]
        member _.onDragStart
            with set (value: DragEvent -> unit) = ()
            and [<Erase>] get (): DragEvent -> unit = unbox ()

        [<Erase>]
        member _.onDrop
            with set (value: DragEvent -> unit) = ()
            and [<Erase>] get (): DragEvent -> unit = unbox ()

        [<Erase>]
        member _.onDurationChange
            with set (value: Event -> unit) = ()
            and [<Erase>] get (): Event -> unit = unbox ()

        [<Erase>]
        member _.onEmptied
            with set (value: Event -> unit) = ()
            and [<Erase>] get (): Event -> unit = unbox ()

        [<Erase>]
        member _.onEnded
            with set (value: Event -> unit) = ()
            and [<Erase>] get (): Event -> unit = unbox ()

        [<Erase>]
        member _.onError
            with set (value: Event -> unit) = ()
            and [<Erase>] get (): Event -> unit = unbox ()

        [<Erase>]
        member _.onFocus
            with set (value: FocusEvent -> unit) = ()
            and [<Erase>] get (): FocusEvent -> unit = unbox ()

        [<Erase>]
        member _.onGotPointerCapture
            with set (value: PointerEvent -> unit) = ()
            and [<Erase>] get (): PointerEvent -> unit = unbox ()

        [<Erase>]
        member _.onInput
            with set (value: InputEvent -> unit) = ()
            and [<Erase>] get (): InputEvent -> unit = unbox ()

        [<Erase>]
        member _.onInvalid
            with set (value: Event -> unit) = ()
            and [<Erase>] get (): Event -> unit = unbox ()

        [<Erase>]
        member _.onKeyDown
            with set (value: KeyboardEvent -> unit) = ()
            and [<Erase>] get (): KeyboardEvent -> unit = unbox ()

        [<Erase>]
        member _.onKeyPress
            with set (value: KeyboardEvent -> unit) = ()
            and [<Erase>] get (): KeyboardEvent -> unit = unbox ()

        [<Erase>]
        member _.onKeyUp
            with set (value: KeyboardEvent -> unit) = ()
            and [<Erase>] get (): KeyboardEvent -> unit = unbox ()

        [<Erase>]
        member _.onLoad
            with set (value: Event -> unit) = ()
            and [<Erase>] get (): Event -> unit = unbox ()

        [<Erase>]
        member _.onLoadedData
            with set (value: Event -> unit) = ()
            and [<Erase>] get (): Event -> unit = unbox ()

        [<Erase>]
        member _.onLoadedMetadata
            with set (value: Event -> unit) = ()
            and [<Erase>] get (): Event -> unit = unbox ()

        [<Erase>]
        member _.onLoadStart
            with set (value: Event -> unit) = ()
            and [<Erase>] get (): Event -> unit = unbox ()

        [<Erase>]
        member _.onLostPointerCapture
            with set (value: PointerEvent -> unit) = ()
            and [<Erase>] get (): PointerEvent -> unit = unbox ()

        [<Erase>]
        member _.onMouseDown
            with set (value: MouseEvent -> unit) = ()
            and [<Erase>] get (): MouseEvent -> unit = unbox ()

        [<Erase>]
        member _.onMouseEnter
            with set (value: MouseEvent -> unit) = ()
            and [<Erase>] get (): MouseEvent -> unit = unbox ()

        [<Erase>]
        member _.onMouseLeave
            with set (value: MouseEvent -> unit) = ()
            and [<Erase>] get (): MouseEvent -> unit = unbox ()

        [<Erase>]
        member _.onMouseMove
            with set (value: MouseEvent -> unit) = ()
            and [<Erase>] get (): MouseEvent -> unit = unbox ()

        [<Erase>]
        member _.onMouseOut
            with set (value: MouseEvent -> unit) = ()
            and [<Erase>] get (): MouseEvent -> unit = unbox ()

        [<Erase>]
        member _.onMouseOver
            with set (value: MouseEvent -> unit) = ()
            and [<Erase>] get (): MouseEvent -> unit = unbox ()

        [<Erase>]
        member _.onMouseUp
            with set (value: MouseEvent -> unit) = ()
            and [<Erase>] get (): MouseEvent -> unit = unbox ()

        [<Erase>]
        member _.onPause
            with set (value: Event -> unit) = ()
            and [<Erase>] get (): Event -> unit = unbox ()

        [<Erase>]
        member _.onPlay
            with set (value: Event -> unit) = ()
            and [<Erase>] get (): Event -> unit = unbox ()

        [<Erase>]
        member _.onPlaying
            with set (value: Event -> unit) = ()
            and [<Erase>] get (): Event -> unit = unbox ()

        [<Erase>]
        member _.onPointerCancel
            with set (value: PointerEvent -> unit) = ()
            and [<Erase>] get (): PointerEvent -> unit = unbox ()

        [<Erase>]
        member _.onPointerDown
            with set (value: PointerEvent -> unit) = ()
            and [<Erase>] get (): PointerEvent -> unit = unbox ()

        [<Erase>]
        member _.onPointerEnter
            with set (value: PointerEvent -> unit) = ()
            and [<Erase>] get (): PointerEvent -> unit = unbox ()

        [<Erase>]
        member _.onPointerLeave
            with set (value: PointerEvent -> unit) = ()
            and [<Erase>] get (): PointerEvent -> unit = unbox ()

        [<Erase>]
        member _.onPointerMove
            with set (value: PointerEvent -> unit) = ()
            and [<Erase>] get (): PointerEvent -> unit = unbox ()

        [<Erase>]
        member _.onPointerOut
            with set (value: PointerEvent -> unit) = ()
            and [<Erase>] get (): PointerEvent -> unit = unbox ()

        [<Erase>]
        member _.onPointerOver
            with set (value: PointerEvent -> unit) = ()
            and [<Erase>] get (): PointerEvent -> unit = unbox ()

        [<Erase>]
        member _.onPointerUp
            with set (value: PointerEvent -> unit) = ()
            and [<Erase>] get (): PointerEvent -> unit = unbox ()

        [<Erase>]
        member _.onProgress
            with set (value: ProgressEvent<EventTarget> -> unit) = ()
            and [<Erase>] get (): ProgressEvent<EventTarget> -> unit = unbox ()

        [<Erase>]
        member _.onRateChange
            with set (value: Event -> unit) = ()
            and [<Erase>] get (): Event -> unit = unbox ()

        [<Erase>]
        member _.onReset
            with set (value: Event -> unit) = ()
            and [<Erase>] get (): Event -> unit = unbox ()

        [<Erase>]
        member _.onScroll
            with set (value: Event -> unit) = ()
            and [<Erase>] get (): Event -> unit = unbox ()

        [<Erase>]
        member _.onScrollEnd
            with set (value: Event -> unit) = ()
            and [<Erase>] get (): Event -> unit = unbox ()

        [<Erase>]
        member _.onSeeked
            with set (value: Event -> unit) = ()
            and [<Erase>] get (): Event -> unit = unbox ()

        [<Erase>]
        member _.onSeeking
            with set (value: Event -> unit) = ()
            and [<Erase>] get (): Event -> unit = unbox ()

        [<Erase>]
        member _.onSelect
            with set (value: Event -> unit) = ()
            and [<Erase>] get (): Event -> unit = unbox ()

        [<Erase>]
        member _.onStalled
            with set (value: Event -> unit) = ()
            and [<Erase>] get (): Event -> unit = unbox ()

        [<Erase>]
        member _.onSubmit
            with set (value: SubmitEvent -> unit) = ()
            and [<Erase>] get (): SubmitEvent -> unit = unbox ()

        [<Erase>]
        member _.onSuspend
            with set (value: Event -> unit) = ()
            and [<Erase>] get (): Event -> unit = unbox ()

        [<Erase>]
        member _.onTimeUpdate
            with set (value: Event -> unit) = ()
            and [<Erase>] get (): Event -> unit = unbox ()

        [<Erase>]
        member _.onToggle
            with set (value: ToggleEvent -> unit) = ()
            and [<Erase>] get (): ToggleEvent -> unit = unbox ()

        [<Erase>]
        member _.onTouchCancel
            with set (value: TouchEvent -> unit) = ()
            and [<Erase>] get (): TouchEvent -> unit = unbox ()

        [<Erase>]
        member _.onTouchEnd
            with set (value: TouchEvent -> unit) = ()
            and [<Erase>] get (): TouchEvent -> unit = unbox ()

        [<Erase>]
        member _.onTouchMove
            with set (value: TouchEvent -> unit) = ()
            and [<Erase>] get (): TouchEvent -> unit = unbox ()

        [<Erase>]
        member _.onTouchStart
            with set (value: TouchEvent -> unit) = ()
            and [<Erase>] get (): TouchEvent -> unit = unbox ()

        [<Erase>]
        member _.onTransitionStart
            with set (value: TransitionEvent -> unit) = ()
            and [<Erase>] get (): TransitionEvent -> unit = unbox ()

        [<Erase>]
        member _.onTransitionEnd
            with set (value: TransitionEvent -> unit) = ()
            and [<Erase>] get (): TransitionEvent -> unit = unbox ()

        [<Erase>]
        member _.onTransitionRun
            with set (value: TransitionEvent -> unit) = ()
            and [<Erase>] get (): TransitionEvent -> unit = unbox ()

        [<Erase>]
        member _.onTransitionCancel
            with set (value: TransitionEvent -> unit) = ()
            and [<Erase>] get (): TransitionEvent -> unit = unbox ()

        [<Erase>]
        member _.onVolumeChange
            with set (value: Event -> unit) = ()
            and [<Erase>] get (): Event -> unit = unbox ()

        [<Erase>]
        member _.onWaiting
            with set (value: Event -> unit) = ()
            and [<Erase>] get (): Event -> unit = unbox ()

        [<Erase>]
        member _.onWheel
            with set (value: WheelEvent -> unit) = ()
            and [<Erase>] get (): WheelEvent -> unit = unbox ()

        [<EB(EBState.Never); Erase>]
        member _.style''
            with set (_: obj) = ()
            and get (): obj = unbox ()

        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<div style='", Suffix = "' />")>]
        member this.style
            with inline set (value: string) = this.style'' <- value
            and inline [<Erase>] get (): string = !!this.style''

        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<div inlist='", Suffix = "' >")>]
        member _.inlist
            with set (_: obj) = ()
            and [<Erase>] get () = unbox ()

    type DetailsHtmlAttributes with
        [<Erase>]
        member _.onToggle
            with set (_: Event -> unit) = ()
            and [<Erase>] get (): Event -> unit = unbox ()

    type DialogHtmlAttributes with
        [<Erase>]
        member _.onClose
            with set (_: Event -> unit) = ()
            and [<Erase>] get (): Event -> unit = unbox ()

        [<Erase>]
        member _.onCancel
            with set (_: Event -> unit) = ()
            and [<Erase>] get (): Event -> unit = unbox ()
