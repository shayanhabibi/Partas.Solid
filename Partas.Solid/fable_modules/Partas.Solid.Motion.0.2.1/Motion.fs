[<AutoOpen>]
module Partas.Solid.Motion.Animate

open Browser.Types
open Fable.Core
open Partas.Solid.Motion.LibDom
open Partas.Solid.Motion
open Partas.Solid.Experimental.U

type VoidFunction = (unit -> unit)

[<AllowNullLiteral>]
[<Interface>]
type BasicAnimationControls =
    /// <summary>
    /// Play the animation.
    ///
    /// <code lang="javascript">
    /// animation.play()
    /// </code>
    /// </summary>
    abstract member play: VoidFunction with get, set
    // /// <summary>
    // /// Pause the animation.
    // ///
    // /// <code lang="javascript">
    // /// animation.pause()
    // /// </code>
    // /// </summary>
    abstract member pause: VoidFunction with get, set
    abstract member commitStyles: VoidFunction with get, set
    // /// <summary>
    // /// Cancels the animation and reverts the element to its underlying styles.
    // ///
    // /// <code lang="javascript">
    // /// animation.cancel()
    // /// </code>
    // /// </summary>
    abstract member cancel: VoidFunction with get, set
    abstract member stop: VoidFunction with get, set
    abstract member playState: AnimationPlayState with get, set
    abstract member finished: JS.Promise<obj> with get, set
    abstract member startTime: float option with get, set
    /// <summary>
    /// Get/set the current play time of the animation in seconds. This can be used to scrub through the animation.
    ///
    /// <code lang="javascript">
    /// const currentTime = animation.currentTime
    /// animation.currentTime = 1
    /// </code>
    /// </summary>
    abstract member currentTime: float option with get, set

[<AllowNullLiteral>]
[<Interface>]
type AnimationControls =
    inherit BasicAnimationControls
    /// <summary>
    /// Stop the animation and set the current value to the element style.
    ///
    /// <code lang="javascript">
    /// animation.stop()
    /// </code>
    /// </summary>
    abstract member stop: VoidFunction with get, set
    /// <summary>
    /// Immediately completes the animation and commits the final keyframe to the element's styles.
    ///
    /// <code lang="javascript">
    /// animation.finish()
    /// </code>
    /// </summary>
    abstract member finish: VoidFunction with get, set
    /// <summary>
    /// Reverses the playback of the animation.
    ///
    /// <code lang="javascript">
    /// animation.reverse()
    /// </code>
    /// </summary>
    /// <remarks>
    /// Currently non-functional.
    /// </remarks>
    abstract member reverse: VoidFunction with get, set
    /// <summary>
    /// A <c>Promise</c> that resolves when the animation has finished.
    ///
    /// <code lang="javascript">
    /// await animation.finished
    /// </code>
    /// </summary>
    abstract member finished: JS.Promise<obj> with get, set
    /// <summary>
    /// Get the current play time of the animation in seconds. This can be especially useful
    /// when an animation has used the default duration, or when a timeline has dynamically
    /// generated a duration from the provided sequence.
    ///
    /// This prop is currently **read-only**.
    /// </summary>
    abstract member duration: float with get, set
    /// <summary>
    /// Get/set the current playback rate of the animation.
    ///
    /// - <c>1</c> is normal time.
    /// - <c>2</c> doubles the playback rate.
    /// - <c>-1</c> reverses playback.
    ///
    /// <code lang="javascript">
    /// animation.playbackRate = animation.playbackRate * 2
    /// </code>
    /// </summary>
    abstract member playbackRate: float with get, set
    /// <summary>
    /// Returns the current state of the animation. Can be <c>idle</c>, <c>running</c>, <c>paused</c> or <c>finished</c>.
    /// </summary>
    abstract member playState: AnimationPlayState with get, set

[<AllowNullLiteral>]
[<Interface>]
type AnimationPlaybackControls =
    /// <summary>
    /// The current time of the animation, in seconds.
    /// </summary>
    abstract member time: float with get, set
    /// <summary>
    /// The playback speed of the animation.
    /// 1 = normal speed, 2 = double speed, 0.5 = half speed.
    /// </summary>
    abstract member speed: float with get, set
    /// <summary>
    /// The start time of the animation, in milliseconds.
    /// </summary>
    abstract member startTime: float option with get, set
    abstract member duration: float with get, set
    /// <summary>
    /// Stops the animation at its current state, and prevents it from
    /// resuming when the animation is played again.
    /// </summary>
    abstract member stop: (unit -> unit) with get, set
    /// <summary>
    /// Plays the animation.
    /// </summary>
    abstract member play: (unit -> unit) with get, set
    /// <summary>
    /// Pauses the animation.
    /// </summary>
    abstract member pause: (unit -> unit) with get, set
    /// <summary>
    /// Completes the animation and applies the final state.
    /// </summary>
    abstract member complete: (unit -> unit) with get, set
    /// <summary>
    /// Cancels the animation and applies the initial state.
    /// </summary>
    abstract member cancel: (unit -> unit) with get, set
    abstract member finished: JS.Promise<obj> with get, set

type AnimationControlsThen =
        delegate of onResolve: VoidFunction * ?onReject: VoidFunction -> JS.Promise<unit>
[<AllowNullLiteral>]
[<Interface>]
type AnimationPlaybackControlsWithThen =
    inherit AnimationPlaybackControls
    abstract member ``then``: AnimationControlsThen with get, set

[<RequireQualifiedAccess; StringEnum>]
type RepeatType =
    | Loop
    | Reverse
    | Mirror
    
[<RequireQualifiedAccess>]
[<StringEnum>]
type AnimationGeneratorType =
    | Decay
    | Spring
    | Keyframes
    | Tween
    | Inertia

[<JS.Pojo>]
type AnimateTransition
    (
        ?delay: float,
        ?elapsed: float,
        ?driver: obj,
        ?``type``: AnimationGeneratorType,
        ?duration: float,
        ?autoplay: bool,
        ?startTime: float,
        ?repeat: float,
        ?repeatType: RepeatType,
        ?repeatDelay: float,
        ?stiffness: float,
        ?damping: float,
        ?mass: float,
        ?visualDuration: float,
        ?bounce: float,
        ?velocity: float,
        ?restSpeed: float,
        ?restDelta: float,
        ?power: float,
        ?timeConstant: float,
        ?modifyTarget: (float -> float),
        ?bounceStiffness: float,
        ?bounceDamping: float,
        ?min: float,
        ?max: float,
        ?ease: U2<EasingFunction, Easing>,
        ?times: ResizeArray<float>
    ) =
    [<DefaultValue>]
    val mutable delay: float
    [<DefaultValue>]
    val mutable elapsed: float
    [<DefaultValue>]
    val mutable driver: obj
    [<DefaultValue>]
    val mutable ``type``: AnimationGeneratorType
    [<DefaultValue>]
    val mutable autoplay: bool
    [<DefaultValue>]
    val mutable startTime: float
    [<DefaultValue>]
    val mutable repeat: float
    [<DefaultValue>]
    val mutable repeatType: RepeatType
    [<DefaultValue>]
    val mutable repeatDelay: float
    [<DefaultValue>]
    val mutable stiffness: float
    [<DefaultValue>]
    val mutable damping: float
    [<DefaultValue>]
    val mutable mass: float
    [<DefaultValue>]
    val mutable duration: float
    [<DefaultValue>]
    val mutable visualDuration: float
    [<DefaultValue>]
    val mutable bounce: float
    [<DefaultValue>]
    val mutable velocity: float
    [<DefaultValue>]
    val mutable restSpeed: float
    [<DefaultValue>]
    val mutable restDelta: float
    [<DefaultValue>]
    val mutable power: float
    [<DefaultValue>]
    val mutable timeConstant: float
    [<DefaultValue>]
    val mutable modifyTarget: (float -> float)
    [<DefaultValue>]
    val mutable bounceStiffness: float
    [<DefaultValue>]
    val mutable bounceDamping: float
    [<DefaultValue>]
    val mutable min: float
    [<DefaultValue>]
    val mutable max: float
    [<DefaultValue>]
    val mutable ease: U2<EasingFunction, Easing>
    [<DefaultValue>]
    val mutable times: ResizeArray<float>

[<AutoOpen; Erase>]
type MotionAnimate =
    [<Import("animate", "motion"); ParamObject(1)>]
    static member animate(sequence: 'T[], ?delay: float, ?duration: float, ?defaultTransition: AnimateTransition, ?repeat: int, ?repeatType: RepeatType, ?repeatDelay: float): AnimationPlaybackControlsWithThen = jsNative
    [<Import("animate", "motion")>]
    static member animate(value: 'T, keyframes: 'T[], ?options: AnimateTransition): AnimationPlaybackControlsWithThen = jsNative
    [<Import("animate", "motion")>]
    static member animate(element: #Element, keyframes: obj, ?options: AnimateTransition): AnimationPlaybackControlsWithThen = jsNative

        
