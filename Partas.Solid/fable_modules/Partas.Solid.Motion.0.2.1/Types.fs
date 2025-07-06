namespace rec Partas.Solid.Motion

open Partas.Solid.Motion.LibDom
open Partas.Solid.Motion.Style
open Fable.Core
open Partas.Solid.Experimental.U
open Browser.Types
open Partas.Solid

[<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
type IInViewOption = interface end
[<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
type IMotionStyle = interface end

[<StringEnum; RequireQualifiedAccess>]
type InViewOptionAmount =
    | Any
    | All
type InViewOption =
    static member inline root(value: U2<HtmlElement, Document>): IInViewOption = unbox("root", value)
    static member inline margin(value: string): IInViewOption = unbox("margin", value)
    static member inline amount(value: U3<int, float, InViewOptionAmount>): IInViewOption = unbox("amount", value)
    static member inline once(value: bool): IInViewOption = unbox("once", value)


[<AutoOpen>]
module StyleExtensions =
    type CssStyle<'T, 'U> with
        static member inline x (value: 'T): 'U = "x" ==>! value 
        static member inline y (value: 'T): 'U = "y" ==>! value 
        static member inline z (value: 'T): 'U = "z" ==>! value 
        static member inline rotateX (value: 'T): 'U = "rotateX" ==>! value 
        static member inline rotateY (value: 'T): 'U = "rotateY" ==>! value 
        static member inline rotateZ (value: 'T): 'U = "rotateZ" ==>! value 
        static member inline scaleX (value: 'T): 'U = "scaleX" ==>! value 
        static member inline scaleY (value: 'T): 'U = "scaleY" ==>! value 
        static member inline scaleZ (value: 'T): 'U = "scaleZ" ==>! value 
        static member inline skewX (value: 'T): 'U = "skewX" ==>! value 
        static member inline skewY (value: 'T): 'U = "skewY" ==>! value 

[<Interface; Erase>]
type MotionStyle =
    inherit CssStyle<U6<string, string[], int, int[], float, float[]>, IMotionStyle>
    static member inline transition (value: IMotionTransition list): IMotionStyle = "transition" ==>! value

[<AllowNullLiteral>]
[<Interface>]
type MotionStateContext =
    abstract member initial: string option with get, set
    abstract member animate: string option with get, set
    abstract member inView: string option with get, set
    abstract member hover: string option with get, set
    abstract member press: string option with get, set
    abstract member exit: string option with get, set


[<Interface; AllowNullLiteral>]
type MotionEvent<'T> =
    inherit CustomEvent
    abstract target: 'T with get
    
[<Interface; AllowNullLiteral>]
type CustomPointerEvent =
    inherit CustomEvent
    abstract originalEvent: PointerEvent with get
    
[<Interface; AllowNullLiteral>]
type ViewEvent =
    inherit CustomEvent
    abstract originalEntry: LibDom.IntersectionObserverEntry with get


[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.KebabCase)>]
type Easing =
    | Linear
    | Ease
    | EaseIn
    | EaseOut
    | EaseInOut
    | StepStart
    | StepEnd

[<JS.Pojo>]
type AnimationOptions
    (
        ?duration: float,
        ?easing: Easing,
        ?offset: ResizeArray<float>,
        ?delay: float,
        ?endDelay: float,
        ?repeat: float,
        ?direction: PlaybackDirection,
        ?persist: bool,
        ?autoplay: bool,
        ?record: bool,
        ?allowWebkitAcceleration: bool
    ) =
    /// <summary>
    /// A duration, in seconds, that the animation will take to complete.
    ///
    /// <c>0.3</c>
    /// </summary>
    member val duration: float option = duration with get, set
    /// <summary>
    /// An easing to use for the whole animation, or list of easings to use between individual keyframes.
    ///
    /// Accepted <c>easing</c> options are:
    ///
    /// - **Basic easings:** <c>"linear"</c>, <c>"ease"</c>, <c>"ease-in"</c>, <c>"ease-out"</c>, <c>"ease-in-out"</c>
    /// - **[Cubic bezier curve](https://cubic-bezier.com/):** e.g. <c>[.17,.67,.83,.67]</c>
    /// - **Stepped easing:** e.g. <c>"steps(2, start)"</c>
    /// - **Custom easing:** A JavaScript easing function, for example [this bounce easing function](https://easings.net/#easeOutBounce).
    ///
    /// <c>"ease"</c>
    /// </summary>
    member val easing: Easing option = easing with get, set
    /// <summary>
    /// The offset of each keyframe, as a number between 0 and 1.
    ///
    /// The number of <c>offset</c> entries must match the number of <c>keyframes</c> entries.
    ///
    /// <c>[0, 1]</c>
    /// </summary>
    member val offset: ResizeArray<float> option = offset with get, set
    /// <summary>
    /// A duration, in seconds, that the animation will be delayed before starting.
    ///
    /// <c>0</c>
    /// </summary>
    member val delay: float option = delay with get, set
    /// <summary>
    /// A duration, in seconds, that the animation will wait at the end before ending.
    ///
    /// <c>0</c>
    /// </summary>
    member val endDelay: float option = endDelay with get, set
    /// <summary>
    /// A duration, in seconds, that the animation will take to complete.
    ///
    /// <c>0.3</c>
    /// </summary>
    member val repeat: float option = repeat with get, set
    /// <summary>
    /// The direction of animation playback. <c>"normal"</c>, <c>"reverse"</c>, <c>"alternate"</c>, or <c>"alternate-reverse"</c>.
    ///
    /// <c>"normal"</c>
    /// </summary>
    member val direction: PlaybackDirection option = direction with get, set
    member val persist: bool option = persist with get, set
    /// <summary>
    /// Whether the animation should start automatically.
    ///
    /// <c>true</c>
    /// </summary>
    member val autoplay: bool option = autoplay with get, set
    member val record: bool option = record with get, set
    /// <summary>
    /// Because of numerous timing bugs in Webkit's accelerated animations, these are disabled by default in Webkit-powered browsers.
    ///
    /// However, if the your animation is being disrupted by heavy processing, you can allow acceleration with this setting.
    /// It's advised you test these animations thoroughly in both Safari and iOS Chrome.
    ///
    /// <c>false</c>
    /// </summary>
    member val allowWebkitAcceleration: bool option = allowWebkitAcceleration with get, set

[<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
type IMotionTransition = interface end

[<Interface>]
type MotionTransition =
    inherit CssStyle<AnimationOptions, IMotionTransition>
    static member inline duration (value: float): IMotionTransition = "duration" ==>! value
    /// <summary>
    /// An easing to use for the whole animation, or list of easings to use between individual keyframes.
    ///
    /// Accepted <c>easing</c> options are:
    ///
    /// - **Basic easings:** <c>"linear"</c>, <c>"ease"</c>, <c>"ease-in"</c>, <c>"ease-out"</c>, <c>"ease-in-out"</c>
    /// - **[Cubic bezier curve](https://cubic-bezier.com/):** e.g. <c>[.17,.67,.83,.67]</c>
    /// - **Stepped easing:** e.g. <c>"steps(2, start)"</c>
    /// - **Custom easing:** A JavaScript easing function, for example [this bounce easing function](https://easings.net/#easeOutBounce).
    ///
    /// <c>"ease"</c>
    /// </summary>
    static member inline easing (value: Easing): IMotionTransition = "easing" ==>! value
    /// <summary>
    /// The offset of each keyframe, as a number between 0 and 1.
    ///
    /// The number of <c>offset</c> entries must match the number of <c>keyframes</c> entries.
    ///
    /// <c>[0, 1]</c>
    /// </summary>
    static member inline offset (value: float array): IMotionTransition = "offset" ==>! value
    /// <summary>
    /// A duration, in seconds, that the animation will be delayed before starting.
    ///
    /// <c>0</c>
    /// </summary>
    static member inline delay (value: float): IMotionTransition = "delay" ==>! value
    /// <summary>
    /// A duration, in seconds, that the animation will wait at the end before ending.
    ///
    /// <c>0</c>
    /// </summary>
    static member inline endDelay (value: float): IMotionTransition = "endDelay" ==>! value
    /// <summary>
    /// A duration, in seconds, that the animation will take to complete.
    ///
    /// <c>0.3</c>
    /// </summary>
    static member inline repeat (value: float): IMotionTransition = "repeat" ==>! value
    /// <summary>
    /// The direction of animation playback. <c>"normal"</c>, <c>"reverse"</c>, <c>"alternate"</c>, or <c>"alternate-reverse"</c>.
    ///
    /// <c>"normal"</c>
    /// </summary>
    static member inline direction (value: PlaybackDirection): IMotionTransition = "direction" ==>! value
    static member inline persist (value: bool): IMotionTransition = "persist" ==>! value
    /// <summary>
    /// Whether the animation should start automatically.
    ///
    /// <c>true</c>
    /// </summary>
    static member inline autoplay (value: bool): IMotionTransition = "autoplay" ==>! value
    static member inline record (value: bool): IMotionTransition = "record" ==>! value
    /// <summary>
    /// Because of numerous timing bugs in Webkit's accelerated animations, these are disabled by default in Webkit-powered browsers.
    ///
    /// However, if the your animation is being disrupted by heavy processing, you can allow acceleration with this setting.
    /// It's advised you test these animations thoroughly in both Safari and iOS Chrome.
    ///
    /// <c>false</c>
    /// </summary>
    static member inline allowWebkitAcceleration (value: bool): IMotionTransition = "allowWebkitAcceleration" ==>! value

type EasingFunction = float -> float
