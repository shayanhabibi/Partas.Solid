namespace Partas.Solid.Motion

open System.Runtime.CompilerServices
open Fable.Core.JS
open JetBrains.Annotations
open Partas.Solid
open Fable.Core
open Fable.Core.JsInterop

[<Erase>]
type OptionKeys = interface end
[<Erase>]
type AttrKey = interface end
[<Erase>]
type MotionEventKeys = interface end

[<Erase; AutoOpen>]
module Extensions =
    open System.ComponentModel
    type AttrKey with
            
        [<Erase>]
        [<LanguageInjection("jsx", Prefix = "<", Suffix = " >")>]
        member _.tag
            with set(value: string) = ()
        
        member this.tagValue
            with inline set(value: TagValue) = this.tag <- !!value
    
    type OptionKeys with
        [<CompiledName("initial");
          Erase;
          EditorBrowsable(EditorBrowsableState.Never)>]
        member _.initial' with set(value: obj) = ()
        [<CompiledName("animate");
          Erase;
          EditorBrowsable(EditorBrowsableState.Never)>]
        member _.animate' with set(value: obj) = ()
        [<CompiledName("inView");
          Erase;
          EditorBrowsable(EditorBrowsableState.Never)>]
        member _.inView' with set(value: obj) = ()
        [<CompiledName("inViewOptions");
          Erase;
          EditorBrowsable(EditorBrowsableState.Never)>]
        member _.inViewOptions' with set(value: obj) = ()
        [<CompiledName("hover");
          Erase;
          EditorBrowsable(EditorBrowsableState.Never)>]
        member _.hover' with set(value: obj) = ()
        [<CompiledName("press");
          Erase;
          EditorBrowsable(EditorBrowsableState.Never)>]
        member _.press' with set(value: obj) = ()
        [<CompiledName("variants");
          Erase;
          EditorBrowsable(EditorBrowsableState.Never)>]
        member _.variants' with set(value: obj) = ()
        [<CompiledName("transition");
          Erase;
          EditorBrowsable(EditorBrowsableState.Never)>]
        member _.transition' with set(value: obj) = ()
        [<CompiledName("exit");
          Erase;
          EditorBrowsable(EditorBrowsableState.Never)>]
        member _.exit' with set(value: obj) = ()
        /// <summary>
        /// Alias for the <c>initial</c> prop, except you provide a string with css completion in supporting IDEs like Rider
        /// </summary>
        /// <example><code>
        /// Motion.div(initial = "{ opacity: 0 }")
        /// </code></example>
        [<LanguageInjection("jsx", Prefix = "<div initial={", Suffix = "} >")>]
        member this.initialJSX
            with inline set(value: string) = this.initial' <- JSX.jsx value
        /// <summary>
        /// Provide a list of props via <c>MotionStyle.</c>. Feliz style.
        /// </summary>
        member this.initial
            with inline set(value: IMotionStyle list) = this.initial' <- createObj !!value
            and [<Erase>] get() = undefined
        /// <summary>
        /// Alias for the <c>animate</c> prop, except you provide a string with css completion in supporting IDEs like Rider
        /// </summary>
        /// <example><code>
        /// Motion.div(animate = "{ opacity: 0 }")
        /// </code></example>
        [<LanguageInjection("jsx", Prefix = "<div animate={", Suffix = "} >")>]
        member this.animateJSX
            with inline set(value: string) = this.animate' <- JSX.jsx value
        /// <summary>
        /// Provide a list of props via <c>MotionStyle.</c>. Feliz style.
        /// </summary>
        member this.animate
            with inline set(value: IMotionStyle list) = this.animate' <- createObj !!value
            and [<Erase>] get() = undefined
        /// <summary>
        /// Alias for the <c>inView</c> prop, except you provide a string with css completion in supporting IDEs like Rider
        /// </summary>
        /// <example><code>
        /// Motion.div(inView = "{ opacity: 0 }")
        /// </code></example>
        [<LanguageInjection("jsx", Prefix = "<div inView={", Suffix = "} >")>]
        member this.inViewJSX
            with inline set(value: string) = this.inView' <- JSX.jsx value
        /// <summary>
        /// Provide a list of props via <c>MotionStyle.</c>. Feliz style.
        /// </summary>
        member this.inView
            with inline set(value: IMotionStyle list) = this.inView' <- createObj !!value
            and [<Erase>] get() = undefined
        /// <summary>
        /// Provide a list of props via <c>InViewOption.</c>. Feliz style.
        /// </summary>
        member this.inViewOptions
            with inline set(value: IInViewOption list) = this.inViewOptions' <- createObj !!value
            and [<Erase>] get() = undefined
        /// <summary>
        /// Alias for the <c>hover</c> prop, except you provide a string with css completion in supporting IDEs like Rider
        /// </summary>
        /// <example><code>
        /// Motion.div(hover = "{ opacity: 0 }")
        /// </code></example>
        [<LanguageInjection("jsx", Prefix = "<div hover={", Suffix = "} >")>]
        member this.hoverJSX
            with inline set(value: string) = this.hover' <- JSX.jsx value
        /// <summary>
        /// Provide a list of props via <c>MotionStyle.</c>. Feliz style.
        /// </summary>
        member this.hover
            with inline set(value: IMotionStyle list) = this.hover' <- createObj !!value
            and [<Erase>] get() = undefined
        /// <summary>
        /// Alias for the <c>press</c> prop, except you provide a string with css completion in supporting IDEs like Rider
        /// </summary>
        /// <example><code>
        /// Motion.div(press = "{ opacity: 0 }")
        /// </code></example>
        [<LanguageInjection("jsx", Prefix = "<div press={", Suffix = "} >")>]
        member this.pressJSX
            with inline set(value: string) = this.press' <- JSX.jsx value
        /// <summary>
        /// Provide a list of props via <c>MotionStyle.</c>. Feliz style.
        /// </summary>
        member this.press
            with inline set(value: IMotionStyle list) = this.press' <- createObj !!value
            and [<Erase>] get() = undefined
        /// <summary>
        /// Alias for the <c>variants</c> prop, except you provide a string with css completion in supporting IDEs like Rider
        /// </summary>
        /// <example><code>
        /// Motion.div(variants = "{ opacity: 0 }")
        /// </code></example>
        [<LanguageInjection("jsx", Prefix = "<div variants={", Suffix = "} >")>]
        member this.variantsJSX
            with inline set(value: string) = this.variants' <- JSX.jsx value
        /// <summary>
        /// Provide a list of props via <c>MotionStyle.</c>. Feliz style.
        /// </summary>
        member this.variants
            with inline set(value: IMotionStyle list) = this.variants' <- createObj !!value
            and [<Erase>] get() = undefined
        /// <summary>
        /// Provide a list of props via <c>MotionTransition.</c>. Feliz style.
        /// </summary>
        member this.transition
            with inline set(value: IMotionTransition list) = this.transition' <- createObj !!value
            and [<Erase>] get() = undefined
        /// <summary>
        /// Alias for the <c>exit</c> prop, except you provide a string with css completion in supporting IDEs like Rider
        /// </summary>
        /// <example><code>
        /// Motion.div(exit = "{ opacity: 0 }")
        /// </code></example>
        [<LanguageInjection("jsx", Prefix = "<div exit={", Suffix = "} >")>]
        member this.exitJSX
            with inline set(value: string) = this.exit' <- JSX.jsx value
        /// <summary>
        /// Provide a list of props via <c>MotionStyle.</c>. Feliz style.
        /// </summary>
        member this.exit
            with inline set(value: IMotionStyle list) = this.exit' <- createObj !!value
            and [<Erase>] get() = undefined
    type MotionEventKeys with
        member _.onMotionStart with set(handler: MotionEvent<string> -> unit) = ()
        member _.onMotionComplete with set(handler: MotionEvent<string> -> unit) = ()
        member _.onHoverStart with set(handler: CustomPointerEvent -> unit) = ()
        member _.onHoverEnd with set(handler: CustomPointerEvent -> unit) = ()
        member _.onPressStart with set(handler: CustomPointerEvent -> unit) = ()
        member _.onPressEnd with set(handler: CustomPointerEvent -> unit) = ()
        member _.onViewEnter with set(handler: ViewEvent -> unit) = ()
        member _.onViewLeave with set(handler: ViewEvent -> unit) = ()

[<Import("Presence", "solid-motionone")>]
type Presence() =
    interface HtmlContainer
    [<Erase>] member val exitBeforeEnter: bool = unbox null with get,set
    [<Erase>] member val initial: bool = unbox null with get,set

[<JS.Pojo>]
type Options() =
    interface OptionKeys

[<AllowNullLiteral; Interface>]
type Generator<'T> =
    abstract member next: (unit -> unit) with get

module MotionState =
    [<RequireQualifiedAccess>]
    [<StringEnum>]
    type Type =
        | Initial
        | Animate
        | InView
        | Hover
        | Press
        | Exit

    type setActive =
        delegate of ``type``: Type * isActive: bool -> unit
        
[<AllowNullLiteral>]
[<Interface>]
type MotionState =
    abstract member update: (Options -> unit) with get, set
    abstract member getDepth: (unit -> float) with get, set
    abstract member getTarget: (unit -> string) with get, set
    abstract member getOptions: (unit -> Options) with get, set
    abstract member getContext: (unit -> MotionStateContext) with get, set
    abstract member setActive: MotionState.setActive with get, set
    abstract member mount: (HtmlElement -> (unit -> unit)) with get, set
    abstract member isMounted: (unit -> bool) with get, set
    abstract member animateUpdates: (unit -> Generator<unit>) with get, set

[<Erase; AutoOpen>]
type Exports =
    /// <summary>
    /// createMotion provides MotionOne as a compact Solid primitive.
    /// </summary>
    /// <param name="target">
    /// Target Element to animate.
    /// </param>
    /// <param name="options">
    /// Options to effect the animation.
    /// </param>
    /// <param name="presenceState">
    /// Optional PresenceContext override, defaults to current parent.
    /// </param>
    /// <returns>
    /// Object to access MotionState
    /// </returns>
    [<Import("createMotion", "solid-motionone")>]
    static member createMotion (target: HtmlElement, options: U2<Accessor<Options>, Options>, ?presenceState: obj) : MotionState = nativeOnly

// [<Erase>]
// type Directives =
    /// <summary>
    /// motion is a Solid directive that makes binding to elements easier.
    /// </summary>
    /// <param name="el">
    /// Target Element to bind to.
    /// </param>
    /// <param name="props">
    /// Options to effect the animation.
    /// </param>
    // [<Import("motion", "solid-motionone")>]
    // [<Extension; Erase>]
    // static member inline useMotion (el: #HtmlTag, props: Accessor<Options>): #HtmlTag = el.use'("motion", props)
