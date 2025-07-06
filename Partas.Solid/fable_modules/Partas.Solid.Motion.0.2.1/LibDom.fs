namespace rec Partas.Solid.Motion.LibDom

open Fable.Core
open Partas.Solid

[<RequireQualifiedAccess>]
[<StringEnum>]
type AnimationPlayState =
    | Finished
    | Idle
    | Paused
    | Running

[<RequireQualifiedAccess>]
[<StringEnum>]
type AnimationReplaceState =
    | Active
    | Persisted
    | Removed
    
[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.KebabCase)>]
type PlaybackDirection =
    | Alternate
    | AlternateReverse
    | Normal
    | Reverse
    
[<AllowNullLiteral>]
[<Interface>]
type DOMRectReadOnly =
    /// <summary>
    /// <a href="https://developer.mozilla.org/docs/Web/API/DOMRectReadOnly/bottom">MDN Reference</a>
    /// </summary>
    abstract member bottom: float with get
    /// <summary>
    /// <a href="https://developer.mozilla.org/docs/Web/API/DOMRectReadOnly/height">MDN Reference</a>
    /// </summary>
    abstract member height: float with get
    /// <summary>
    /// <a href="https://developer.mozilla.org/docs/Web/API/DOMRectReadOnly/left">MDN Reference</a>
    /// </summary>
    abstract member left: float with get
    /// <summary>
    /// <a href="https://developer.mozilla.org/docs/Web/API/DOMRectReadOnly/right">MDN Reference</a>
    /// </summary>
    abstract member right: float with get
    /// <summary>
    /// <a href="https://developer.mozilla.org/docs/Web/API/DOMRectReadOnly/top">MDN Reference</a>
    /// </summary>
    abstract member top: float with get
    /// <summary>
    /// <a href="https://developer.mozilla.org/docs/Web/API/DOMRectReadOnly/width">MDN Reference</a>
    /// </summary>
    abstract member width: float with get
    /// <summary>
    /// <a href="https://developer.mozilla.org/docs/Web/API/DOMRectReadOnly/x">MDN Reference</a>
    /// </summary>
    abstract member x: float with get
    /// <summary>
    /// <a href="https://developer.mozilla.org/docs/Web/API/DOMRectReadOnly/y">MDN Reference</a>
    /// </summary>
    abstract member y: float with get
    abstract member toJSON: unit -> obj
        
[<AllowNullLiteral>]
[<Interface>]
type IntersectionObserverEntry =
    /// <summary>
    /// <a href="https://developer.mozilla.org/docs/Web/API/IntersectionObserverEntry/boundingClientRect">MDN Reference</a>
    /// </summary>
    abstract member boundingClientRect: DOMRectReadOnly with get
    /// <summary>
    /// <a href="https://developer.mozilla.org/docs/Web/API/IntersectionObserverEntry/intersectionRatio">MDN Reference</a>
    /// </summary>
    abstract member intersectionRatio: float with get
    /// <summary>
    /// <a href="https://developer.mozilla.org/docs/Web/API/IntersectionObserverEntry/intersectionRect">MDN Reference</a>
    /// </summary>
    abstract member intersectionRect: DOMRectReadOnly with get
    /// <summary>
    /// <a href="https://developer.mozilla.org/docs/Web/API/IntersectionObserverEntry/isIntersecting">MDN Reference</a>
    /// </summary>
    abstract member isIntersecting: bool with get
    /// <summary>
    /// <a href="https://developer.mozilla.org/docs/Web/API/IntersectionObserverEntry/rootBounds">MDN Reference</a>
    /// </summary>
    abstract member rootBounds: DOMRectReadOnly option with get
    /// <summary>
    /// <a href="https://developer.mozilla.org/docs/Web/API/IntersectionObserverEntry/target">MDN Reference</a>
    /// </summary>
    abstract member target: HtmlElement with get
    /// <summary>
    /// <a href="https://developer.mozilla.org/docs/Web/API/IntersectionObserverEntry/time">MDN Reference</a>
    /// </summary>
    abstract member time: int with get
