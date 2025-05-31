namespace Partas.Solid

open Fable.Core
module Svg =

    [<AllowNullLiteral>]
    [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
    type ConditionalProcessingSVGAttributes = interface end

    [<AllowNullLiteral>]
    [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
    type AnimationElementSVGAttributes =
        inherit ConditionalProcessingSVGAttributes

    [<AllowNullLiteral>]
    [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
    type ShapeElementSVGAttributes = interface end

    [<AllowNullLiteral>]
    [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
    type ContainerElementSVGAttributes =
        inherit ShapeElementSVGAttributes

    [<AllowNullLiteral>]
    [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
    type FilterPrimitiveElementSVGAttributes = interface end

    [<AllowNullLiteral>]
    [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
    type TransformableSVGAttributes = interface end

    [<AllowNullLiteral>]
    [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
    type AnimationTimingSVGAttributes = interface end

    [<AllowNullLiteral>]
    [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
    type AnimationValueSVGAttributes = interface end

    [<AllowNullLiteral>]
    [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
    type AnimationAdditionSVGAttributes = interface end

    [<AllowNullLiteral>]
    [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
    type AnimationAttributeTargetSVGAttributes = interface end

    [<AllowNullLiteral>]
    [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
    type PresentationSVGAttributes = interface end

    [<AllowNullLiteral>]
    [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
    type SingleInputFilterSVGAttributes = interface end

    [<AllowNullLiteral>]
    [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
    type DoubleInputFilterSVGAttributes = interface end

    [<AllowNullLiteral>]
    [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
    type FitToViewBoxSVGAttributes = interface end

    [<AllowNullLiteral>]
    [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
    type GradientElementSVGAttributes = interface end

    [<AllowNullLiteral>]
    [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
    type GraphicsElementSVGAttributes = interface end

    [<AllowNullLiteral>]
    [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
    type LightSourceElementSVGAttributes = interface end

    [<AllowNullLiteral>]
    [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
    type NewViewportSVGAttributes = interface end

    [<AllowNullLiteral>]
    [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
    type TextContentElementSVGAttributes = interface end

    [<AllowNullLiteral>]
    [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
    type ZoomAndPanSVGAttributes = interface end

    type TransformableSVGAttributes with
        [<Erase>]
        member _.transform
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    type ConditionalProcessingSVGAttributes with
        [<Erase>]
        member _.requiredExtensions
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.requiredFeatures
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.systemLanguage
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    type AnimationTimingSVGAttributes with
        [<Erase>]
        member _.begin'
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.dur
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.end'
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.min
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.max
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.restart
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.repeatCount
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.repeatDur
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.fill
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    type AnimationValueSVGAttributes with
        [<Erase>]
        member _.calcMode
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.values
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.keyTimes
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.keySplines
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.from
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.to'
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.by
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    type AnimationAdditionSVGAttributes with
        [<Erase>]
        member _.attributeName
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.additive
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.accumulate
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    type AnimationAttributeTargetSVGAttributes with
        [<Erase>]
        member _.attributeName
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.attributeType
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    type ContainerElementSVGAttributes with
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``color-rendering``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.colorRendering
            with inline set (value: string) = this.``color-rendering`` <- value
            and inline get(): string = this.``color-rendering``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``clip-path``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.clipPath
            with inline set (value: string) = this.``clip-path`` <- value
            and inline get(): string = this.``clip-path``
        [<Erase>]
        member _.cursor
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``color-interpolation``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.colorInterpolation
            with inline set (value: string) = this.``color-interpolation`` <- value
            and inline get(): string = this.``color-interpolation``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``enable-background``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.enableBackground
            with inline set (value: string) = this.``enable-background`` <- value
            and inline get(): string = this.``enable-background``
        [<Erase>]
        member _.filter
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.mask
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.opacity
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    type GraphicsElementSVGAttributes with
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``clip-rule``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.clipRule
            with inline set (value: string) = this.``clip-rule`` <- value
            and inline get(): string = this.``clip-rule``
        [<Erase>]
        member _.mask
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``pointer-events``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.pointerEvents
            with inline set (value: string) = this.``pointer-events`` <- value
            and inline get(): string = this.``pointer-events``
        [<Erase>]
        member _.cursor
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.opacity
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.filter
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.display
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.visibility
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``color-interpolation``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.colorInterpolation
            with inline set (value: string) = this.``color-interpolation`` <- value
            and inline get(): string = this.``color-interpolation``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``color-rendering``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.colorRendering
            with inline set (value: string) = this.``color-rendering`` <- value
            and inline get(): string = this.``color-rendering``
        [<Erase>]
        member _.stroke
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``stroke-dasharray``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.strokeDasharray
            with inline set (value: string) = this.``stroke-dasharray`` <- value
            and inline get(): string = this.``stroke-dasharray``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``stroke-dashoffset``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.strokeDashoffset
            with inline set (value: string) = this.``stroke-dashoffset`` <- value
            and inline get(): string = this.``stroke-dashoffset``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``stroke-linecap``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.strokeLinecap
            with inline set (value: string) = this.``stroke-linecap`` <- value
            and inline get(): string = this.``stroke-linecap``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``stroke-linejoin``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.strokeLinejoin
            with inline set (value: string) = this.``stroke-linejoin`` <- value
            and inline get(): string = this.``stroke-linejoin``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``stroke-miterlimit``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.strokeMiterlimit
            with inline set (value: string) = this.``stroke-miterlimit`` <- value
            and inline get(): string = this.``stroke-miterlimit``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``stroke-opacity``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.strokeOpacity
            with inline set (value: string) = this.``stroke-opacity`` <- value
            and inline get(): string = this.``stroke-opacity``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``stroke-width``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.strokeWidth
            with inline set (value: string) = this.``stroke-width`` <- value
            and inline get(): string = this.``stroke-width``
        [<Erase>]
        member _.color
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.fill
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``fill-opacity``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.fillOpacity
            with inline set (value: string) = this.``fill-opacity`` <- value
            and inline get(): string = this.``fill-opacity``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``fill-rule``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.fillRule
            with inline set (value: string) = this.``fill-rule`` <- value
            and inline get(): string = this.``fill-rule``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``shape-rendering``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.shapeRendering
            with inline set (value: string) = this.``shape-rendering`` <- value
            and inline get(): string = this.``shape-rendering``
        [<Erase>]
        member _.pathLength
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    type TextContentElementSVGAttributes with
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``font-family``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.fontFamily
            with inline set (value: string) = this.``font-family`` <- value
            and inline get(): string = this.``font-family``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``font-size``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.fontSize
            with inline set (value: string) = this.``font-size`` <- value
            and inline get(): string = this.``font-size``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``font-size-adjust``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.fontSizeAdjust
            with inline set (value: string) = this.``font-size-adjust`` <- value
            and inline get(): string = this.``font-size-adjust``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``font-stretch``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.fontStretch
            with inline set (value: string) = this.``font-stretch`` <- value
            and inline get(): string = this.``font-stretch``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``font-style``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.fontStyle
            with inline set (value: string) = this.``font-style`` <- value
            and inline get(): string = this.``font-style``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``font-variant``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.fontVariant
            with inline set (value: string) = this.``font-variant`` <- value
            and inline get(): string = this.``font-variant``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``font-weight``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.fontWeight
            with inline set (value: string) = this.``font-weight`` <- value
            and inline get(): string = this.``font-weight``
        [<Erase>]
        member _.kerning
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``letter-spacing``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.letterSpacing
            with inline set (value: string) = this.``letter-spacing`` <- value
            and inline get(): string = this.``letter-spacing``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``word-spacing``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.wordSpacing
            with inline set (value: string) = this.``word-spacing`` <- value
            and inline get(): string = this.``word-spacing``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``text-decoration``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.textDecoration
            with inline set (value: string) = this.``text-decoration`` <- value
            and inline get(): string = this.``text-decoration``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``glyph-orientation-horizontal``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.glyphOrientationHorizontal
            with inline set (value: string) = this.``glyph-orientation-horizontal`` <- value
            and inline get(): string = this.``glyph-orientation-horizontal``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``glyph-orientation-vertical``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.glyphOrientationVertical
            with inline set (value: string) = this.``glyph-orientation-vertical`` <- value
            and inline get(): string = this.``glyph-orientation-vertical``
        [<Erase>]
        member _.direction
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``unicode-bidi``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.unicodeBidi
            with inline set (value: string) = this.``unicode-bidi`` <- value
            and inline get(): string = this.``unicode-bidi``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``text-anchor``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.textAnchor
            with inline set (value: string) = this.``text-anchor`` <- value
            and inline get(): string = this.``text-anchor``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``dominant-baseline``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.dominantBaseline
            with inline set (value: string) = this.``dominant-baseline`` <- value
            and inline get(): string = this.``dominant-baseline``
        [<Erase>]
        member _.color
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.fill
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``fill-opacity``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.fillOpacity
            with inline set (value: string) = this.``fill-opacity`` <- value
            and inline get(): string = this.``fill-opacity``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``fill-rule``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.fillRule
            with inline set (value: string) = this.``fill-rule`` <- value
            and inline get(): string = this.``fill-rule``
        [<Erase>]
        member _.stroke
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``stroke-dasharray``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.strokeDasharray
            with inline set (value: string) = this.``stroke-dasharray`` <- value
            and inline get(): string = this.``stroke-dasharray``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``stroke-dashoffset``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.strokeDashoffset
            with inline set (value: string) = this.``stroke-dashoffset`` <- value
            and inline get(): string = this.``stroke-dashoffset``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``stroke-linecap``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.strokeLinecap
            with inline set (value: string) = this.``stroke-linecap`` <- value
            and inline get(): string = this.``stroke-linecap``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``stroke-linejoin``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.strokeLinejoin
            with inline set (value: string) = this.``stroke-linejoin`` <- value
            and inline get(): string = this.``stroke-linejoin``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``stroke-miterlimit``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.strokeMiterlimit
            with inline set (value: string) = this.``stroke-miterlimit`` <- value
            and inline get(): string = this.``stroke-miterlimit``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``stroke-opacity``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.strokeOpacity
            with inline set (value: string) = this.``stroke-opacity`` <- value
            and inline get(): string = this.``stroke-opacity``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``stroke-width``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.strokeWidth
            with inline set (value: string) = this.``stroke-width`` <- value
            and inline get(): string = this.``stroke-width``

    type PresentationSVGAttributes with
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``alignment-baseline``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.alignmentBaseline
            with inline set (value: string) = this.``alignment-baseline`` <- value
            and inline get(): string = this.``alignment-baseline``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``baseline-shift``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.baselineShift
            with inline set (value: string) = this.``baseline-shift`` <- value
            and inline get(): string = this.``baseline-shift``
        [<Erase>]
        member _.clip
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``clip-path``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.clipPath
            with inline set (value: string) = this.``clip-path`` <- value
            and inline get(): string = this.``clip-path``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``clip-rule``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.clipRule
            with inline set (value: string) = this.``clip-rule`` <- value
            and inline get(): string = this.``clip-rule``
        [<Erase>]
        member _.color
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``color-interpolation``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.colorInterpolation
            with inline set (value: string) = this.``color-interpolation`` <- value
            and inline get(): string = this.``color-interpolation``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``color-interpolation-filters``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.colorInterpolationFilters
            with inline set (value: string) = this.``color-interpolation-filters`` <- value
            and inline get(): string = this.``color-interpolation-filters``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``color-profile``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.colorProfile
            with inline set (value: string) = this.``color-profile`` <- value
            and inline get(): string = this.``color-profile``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``color-rendering``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.colorRendering
            with inline set (value: string) = this.``color-rendering`` <- value
            and inline get(): string = this.``color-rendering``
        [<Erase>]
        member _.cursor
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.direction
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.display
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``dominant-baseline``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.dominantBaseline
            with inline set (value: string) = this.``dominant-baseline`` <- value
            and inline get(): string = this.``dominant-baseline``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``enable-background``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.enableBackground
            with inline set (value: string) = this.``enable-background`` <- value
            and inline get(): string = this.``enable-background``
        [<Erase>]
        member _.fill
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``fill-opacity``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.fillOpacity
            with inline set (value: string) = this.``fill-opacity`` <- value
            and inline get(): string = this.``fill-opacity``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``fill-rule``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.fillRule
            with inline set (value: string) = this.``fill-rule`` <- value
            and inline get(): string = this.``fill-rule``
        [<Erase>]
        member _.filter
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``flood-color``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.floodColor
            with inline set (value: string) = this.``flood-color`` <- value
            and inline get(): string = this.``flood-color``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``flood-opacity``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.floodOpacity
            with inline set (value: string) = this.``flood-opacity`` <- value
            and inline get(): string = this.``flood-opacity``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``font-family``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.fontFamily
            with inline set (value: string) = this.``font-family`` <- value
            and inline get(): string = this.``font-family``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``font-size``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.fontSize
            with inline set (value: string) = this.``font-size`` <- value
            and inline get(): string = this.``font-size``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``font-size-adjust``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.fontSizeAdjust
            with inline set (value: string) = this.``font-size-adjust`` <- value
            and inline get(): string = this.``font-size-adjust``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``font-stretch``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.fontStretch
            with inline set (value: string) = this.``font-stretch`` <- value
            and inline get(): string = this.``font-stretch``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``font-style``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.fontStyle
            with inline set (value: string) = this.``font-style`` <- value
            and inline get(): string = this.``font-style``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``font-variant``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.fontVariant
            with inline set (value: string) = this.``font-variant`` <- value
            and inline get(): string = this.``font-variant``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``font-weight``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.fontWeight
            with inline set (value: string) = this.``font-weight`` <- value
            and inline get(): string = this.``font-weight``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``glyph-orientation-horizontal``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.glyphOrientationHorizontal
            with inline set (value: string) = this.``glyph-orientation-horizontal`` <- value
            and inline get(): string = this.``glyph-orientation-horizontal``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``glyph-orientation-vertical``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.glyphOrientationVertical
            with inline set (value: string) = this.``glyph-orientation-vertical`` <- value
            and inline get(): string = this.``glyph-orientation-vertical``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``image-rendering``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.imageRendering
            with inline set (value: string) = this.``image-rendering`` <- value
            and inline get(): string = this.``image-rendering``
        [<Erase>]
        member _.kerning
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``letter-spacing``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.letterSpacing
            with inline set (value: string) = this.``letter-spacing`` <- value
            and inline get(): string = this.``letter-spacing``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``lighting-color``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.lightingColor
            with inline set (value: string) = this.``lighting-color`` <- value
            and inline get(): string = this.``lighting-color``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``marker-end``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.markerEnd
            with inline set (value: string) = this.``marker-end`` <- value
            and inline get(): string = this.``marker-end``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``marker-mid``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.markerMid
            with inline set (value: string) = this.``marker-mid`` <- value
            and inline get(): string = this.``marker-mid``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``marker-start``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.markerStart
            with inline set (value: string) = this.``marker-start`` <- value
            and inline get(): string = this.``marker-start``
        [<Erase>]
        member _.mask
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.opacity
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.overflow
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.pathLength
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``pointer-events``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.pointerEvents
            with inline set (value: string) = this.``pointer-events`` <- value
            and inline get(): string = this.``pointer-events``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``shape-rendering``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.shapeRendering
            with inline set (value: string) = this.``shape-rendering`` <- value
            and inline get(): string = this.``shape-rendering``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``stop-color``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.stopColor
            with inline set (value: string) = this.``stop-color`` <- value
            and inline get(): string = this.``stop-color``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``stop-opacity``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.stopOpacity
            with inline set (value: string) = this.``stop-opacity`` <- value
            and inline get(): string = this.``stop-opacity``
        [<Erase>]
        member _.stroke
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``stroke-dasharray``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.strokeDasharray
            with inline set (value: string) = this.``stroke-dasharray`` <- value
            and inline get(): string = this.``stroke-dasharray``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``stroke-dashoffset``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.strokeDashoffset
            with inline set (value: string) = this.``stroke-dashoffset`` <- value
            and inline get(): string = this.``stroke-dashoffset``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``stroke-linecap``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.strokeLinecap
            with inline set (value: string) = this.``stroke-linecap`` <- value
            and inline get(): string = this.``stroke-linecap``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``stroke-linejoin``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.strokeLinejoin
            with inline set (value: string) = this.``stroke-linejoin`` <- value
            and inline get(): string = this.``stroke-linejoin``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``stroke-miterlimit``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.strokeMiterlimit
            with inline set (value: string) = this.``stroke-miterlimit`` <- value
            and inline get(): string = this.``stroke-miterlimit``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``stroke-opacity``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.strokeOpacity
            with inline set (value: string) = this.``stroke-opacity`` <- value
            and inline get(): string = this.``stroke-opacity``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``stroke-width``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.strokeWidth
            with inline set (value: string) = this.``stroke-width`` <- value
            and inline get(): string = this.``stroke-width``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``text-anchor``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.textAnchor
            with inline set (value: string) = this.``text-anchor`` <- value
            and inline get(): string = this.``text-anchor``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``text-decoration``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.textDecoration
            with inline set (value: string) = this.``text-decoration`` <- value
            and inline get(): string = this.``text-decoration``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``text-rendering``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.textRendering
            with inline set (value: string) = this.``text-rendering`` <- value
            and inline get(): string = this.``text-rendering``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``unicode-bidi``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.unicodeBidi
            with inline set (value: string) = this.``unicode-bidi`` <- value
            and inline get(): string = this.``unicode-bidi``
        [<Erase>]
        member _.visibility
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``word-spacing``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.wordSpacing
            with inline set (value: string) = this.``word-spacing`` <- value
            and inline get(): string = this.``word-spacing``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``writing-mode``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.writingMode
            with inline set (value: string) = this.``writing-mode`` <- value
            and inline get(): string = this.``writing-mode``

    type FilterPrimitiveElementSVGAttributes with
        [<Erase>]
        member _.x
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.y
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.width
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.height
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.result
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``color-interpolation-filters``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.colorInterpolationFilters
            with inline set (value: string) = this.``color-interpolation-filters`` <- value
            and inline get(): string = this.``color-interpolation-filters``

    type SingleInputFilterSVGAttributes with
        [<Erase>]
        member _.in'
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    type DoubleInputFilterSVGAttributes with
        [<Erase>]
        member _.in'
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.in2
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    type FitToViewBoxSVGAttributes with
        [<Erase>]
        member _.viewBox
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.preserveAspectRatio
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    type GradientElementSVGAttributes with
        [<Erase>]
        member _.gradientUnits
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.gradientTransform
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.spreadMethod
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.href
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type animate() =
        interface RegularNode
        interface AnimationElementSVGAttributes
        interface AnimationAttributeTargetSVGAttributes
        interface AnimationTimingSVGAttributes
        interface AnimationValueSVGAttributes
        interface AnimationAdditionSVGAttributes
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``color-interpolation``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.colorInterpolation
            with inline set (value: string) = this.``color-interpolation`` <- value
            and inline get(): string = this.``color-interpolation``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``color-rendering``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.colorRendering
            with inline set (value: string) = this.``color-rendering`` <- value
            and inline get(): string = this.``color-rendering``

    [<Erase>]
    type animateMotion() =
        interface RegularNode
        interface AnimationElementSVGAttributes
        interface AnimationTimingSVGAttributes
        interface AnimationValueSVGAttributes
        interface AnimationAdditionSVGAttributes
        [<Erase>]
        member _.path
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.keyPoints
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.rotate
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.origin
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type animateTransform() =
        interface RegularNode
        interface AnimationElementSVGAttributes
        interface AnimationAttributeTargetSVGAttributes
        interface AnimationTimingSVGAttributes
        interface AnimationValueSVGAttributes
        interface AnimationAdditionSVGAttributes
        [<Erase>]
        member _.type'
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type circle() =
        interface RegularNode
        interface GraphicsElementSVGAttributes
        interface ShapeElementSVGAttributes
        interface ConditionalProcessingSVGAttributes
        interface TransformableSVGAttributes
        [<Erase>]
        member _.cx
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.cy
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.r
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type clipPath() =
        interface RegularNode
        interface ConditionalProcessingSVGAttributes
        interface TransformableSVGAttributes
        [<Erase>]
        member _.clipPathUnits
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``clip-path``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.clipPath
            with inline set (value: string) = this.``clip-path`` <- value
            and inline get(): string = this.``clip-path``

    [<Erase>]
    type defs() =
        interface RegularNode
        interface ContainerElementSVGAttributes
        interface ConditionalProcessingSVGAttributes
        interface TransformableSVGAttributes

    [<Erase>]
    type desc() =
        interface RegularNode

    [<Erase>]
    type ellipse() =
        interface RegularNode
        interface GraphicsElementSVGAttributes
        interface ShapeElementSVGAttributes
        interface ConditionalProcessingSVGAttributes
        interface TransformableSVGAttributes
        [<Erase>]
        member _.cx
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.cy
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.rx
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.ry
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type feBlend() =
        interface RegularNode
        interface FilterPrimitiveElementSVGAttributes
        interface DoubleInputFilterSVGAttributes
        [<Erase>]
        member _.mode
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type feColorMatrix() =
        interface RegularNode
        interface FilterPrimitiveElementSVGAttributes
        interface SingleInputFilterSVGAttributes
        [<Erase>]
        member _.type'
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.values
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type feComponentTransfer() =
        interface RegularNode
        interface FilterPrimitiveElementSVGAttributes
        interface SingleInputFilterSVGAttributes

    [<Erase>]
    type feComposite() =
        interface RegularNode
        interface FilterPrimitiveElementSVGAttributes
        interface DoubleInputFilterSVGAttributes
        [<Erase>]
        member _.operator
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.k1
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.k2
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.k3
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.k4
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type feConvolveMatrix() =
        interface RegularNode
        interface FilterPrimitiveElementSVGAttributes
        interface SingleInputFilterSVGAttributes
        [<Erase>]
        member _.order
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.kernelMatrix
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.divisor
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.bias
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.targetX
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.targetY
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.edgeMode
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.kernelUnitLength
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.preserveAlpha
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type feDiffuseLighting() =
        interface RegularNode
        interface FilterPrimitiveElementSVGAttributes
        interface SingleInputFilterSVGAttributes
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``lighting-color``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.lightingColor
            with inline set (value: string) = this.``lighting-color`` <- value
            and inline get(): string = this.``lighting-color``
        [<Erase>]
        member _.color
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.surfaceScale
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.diffuseConstant
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.kernelUnitLength
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type feDisplacementMap() =
        interface RegularNode
        interface FilterPrimitiveElementSVGAttributes
        interface DoubleInputFilterSVGAttributes
        [<Erase>]
        member _.scale
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.xChannelSelector
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.yChannelSelector
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type feDistantLight() =
        interface RegularNode
        interface LightSourceElementSVGAttributes
        [<Erase>]
        member _.azimuth
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.elevation
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type feDropShadow() =
        interface RegularNode
        interface FilterPrimitiveElementSVGAttributes
        [<Erase>]
        member _.color
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``flood-color``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.floodColor
            with inline set (value: string) = this.``flood-color`` <- value
            and inline get(): string = this.``flood-color``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``flood-opacity``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.floodOpacity
            with inline set (value: string) = this.``flood-opacity`` <- value
            and inline get(): string = this.``flood-opacity``
        [<Erase>]
        member _.dx
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.dy
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.stdDeviation
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type feFlood() =
        interface RegularNode
        interface FilterPrimitiveElementSVGAttributes
        [<Erase>]
        member _.color
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``flood-color``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.floodColor
            with inline set (value: string) = this.``flood-color`` <- value
            and inline get(): string = this.``flood-color``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``flood-opacity``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.floodOpacity
            with inline set (value: string) = this.``flood-opacity`` <- value
            and inline get(): string = this.``flood-opacity``

    [<Erase>]
    type feFuncA() =
        interface RegularNode
        [<Erase>]
        member _.type'
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.tableValues
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.slope
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.intercept
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.amplitude
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.exponent
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.offset
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type feFuncB() =
        interface RegularNode
        [<Erase>]
        member _.type'
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.tableValues
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.slope
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.intercept
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.amplitude
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.exponent
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.offset
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type feFuncG() =
        interface RegularNode
        [<Erase>]
        member _.type'
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.tableValues
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.slope
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.intercept
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.amplitude
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.exponent
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.offset
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type feFuncR() =
        interface RegularNode
        [<Erase>]
        member _.type'
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.tableValues
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.slope
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.intercept
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.amplitude
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.exponent
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.offset
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type feGaussianBlur() =
        interface RegularNode
        interface FilterPrimitiveElementSVGAttributes
        interface SingleInputFilterSVGAttributes
        [<Erase>]
        member _.stdDeviation
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type feImage() =
        interface RegularNode
        interface FilterPrimitiveElementSVGAttributes
        [<Erase>]
        member _.preserveAspectRatio
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.href
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type feMerge() =
        interface RegularNode
        interface FilterPrimitiveElementSVGAttributes

    [<Erase>]
    type feMergeNode() =
        interface VoidNode
        interface SingleInputFilterSVGAttributes

    [<Erase>]
    type feMorphology() =
        interface RegularNode
        interface FilterPrimitiveElementSVGAttributes
        interface SingleInputFilterSVGAttributes
        [<Erase>]
        member _.operator
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.radius
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type feOffset() =
        interface RegularNode
        interface FilterPrimitiveElementSVGAttributes
        interface SingleInputFilterSVGAttributes
        [<Erase>]
        member _.dx
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.dy
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type fePointLight() =
        interface RegularNode
        interface LightSourceElementSVGAttributes
        [<Erase>]
        member _.x
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.y
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.z
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type feSpecularLighting() =
        interface RegularNode
        interface FilterPrimitiveElementSVGAttributes
        interface SingleInputFilterSVGAttributes
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``lighting-color``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.lightingColor
            with inline set (value: string) = this.``lighting-color`` <- value
            and inline get(): string = this.``lighting-color``
        [<Erase>]
        member _.color
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.surfaceScale
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.specularConstant
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.specularExponent
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.kernelUnitLength
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type feSpotLight() =
        interface RegularNode
        interface LightSourceElementSVGAttributes
        [<Erase>]
        member _.x
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.y
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.z
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.pointsAtX
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.pointsAtY
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.pointsAtZ
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.specularExponent
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.limitingConeAngle
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type feTile() =
        interface RegularNode
        interface FilterPrimitiveElementSVGAttributes
        interface SingleInputFilterSVGAttributes

    [<Erase>]
    type feTurbulence() =
        interface RegularNode
        interface FilterPrimitiveElementSVGAttributes
        [<Erase>]
        member _.baseFrequency
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.numOctaves
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.seed
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.stitchTiles
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.type'
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type filter() =
        interface RegularNode
        [<Erase>]
        member _.filterUnits
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.primitiveUnits
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.x
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.y
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.width
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.height
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.filterRes
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type foreignObject() =
        interface RegularNode
        interface NewViewportSVGAttributes
        interface ConditionalProcessingSVGAttributes
        interface TransformableSVGAttributes
        [<Erase>]
        member _.visibility
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.display
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.x
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.y
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.width
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.height
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type g() =
        interface RegularNode
        interface ContainerElementSVGAttributes
        interface ConditionalProcessingSVGAttributes
        interface TransformableSVGAttributes
        [<Erase>]
        member _.visibility
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.display
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type image() =
        interface RegularNode
        interface NewViewportSVGAttributes
        interface GraphicsElementSVGAttributes
        interface ConditionalProcessingSVGAttributes
        interface TransformableSVGAttributes
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``image-rendering``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.imageRendering
            with inline set (value: string) = this.``image-rendering`` <- value
            and inline get(): string = this.``image-rendering``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``color-profile``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.colorProfile
            with inline set (value: string) = this.``color-profile`` <- value
            and inline get(): string = this.``color-profile``
        [<Erase>]
        member _.x
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.y
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.width
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.height
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.preserveAspectRatio
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.href
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type line() =
        interface RegularNode
        interface GraphicsElementSVGAttributes
        interface ShapeElementSVGAttributes
        interface ConditionalProcessingSVGAttributes
        interface TransformableSVGAttributes
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``marker-end``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.markerEnd
            with inline set (value: string) = this.``marker-end`` <- value
            and inline get(): string = this.``marker-end``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``marker-mid``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.markerMid
            with inline set (value: string) = this.``marker-mid`` <- value
            and inline get(): string = this.``marker-mid``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``marker-start``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.markerStart
            with inline set (value: string) = this.``marker-start`` <- value
            and inline get(): string = this.``marker-start``
        [<Erase>]
        member _.x1
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.y1
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.x2
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.y2
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type linearGradient() =
        interface RegularNode
        interface GradientElementSVGAttributes
        [<Erase>]
        member _.x1
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.x2
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.y1
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.y2
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type marker() =
        interface RegularNode
        interface ContainerElementSVGAttributes
        interface FitToViewBoxSVGAttributes
        [<Erase>]
        member _.clip
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.overflow
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.markerUnits
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.refX
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.refY
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.markerWidth
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.markerHeight
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.orient
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type mask() =
        interface RegularNode
        interface ConditionalProcessingSVGAttributes
        [<Erase>]
        member _.filter
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.opacity
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.maskUnits
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.maskContentUnits
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.x
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.y
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.width
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.height
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type metadata() =
        interface RegularNode

    [<Erase>]
    type mpath() =
        interface VoidNode

    [<Erase>]
    type path() =
        interface RegularNode
        interface GraphicsElementSVGAttributes
        interface ShapeElementSVGAttributes
        interface ConditionalProcessingSVGAttributes
        interface TransformableSVGAttributes
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``marker-end``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.markerEnd
            with inline set (value: string) = this.``marker-end`` <- value
            and inline get(): string = this.``marker-end``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``marker-mid``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.markerMid
            with inline set (value: string) = this.``marker-mid`` <- value
            and inline get(): string = this.``marker-mid``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``marker-start``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.markerStart
            with inline set (value: string) = this.``marker-start`` <- value
            and inline get(): string = this.``marker-start``
        [<Erase>]
        member _.d
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.pathLength
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type pattern() =
        interface RegularNode
        interface ContainerElementSVGAttributes
        interface ConditionalProcessingSVGAttributes
        interface FitToViewBoxSVGAttributes
        [<Erase>]
        member _.x
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.y
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.width
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.height
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.patternUnits
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.patternContentUnits
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.patternTransform
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.href
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.clip
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.overflow
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type polygon() =
        interface RegularNode
        interface GraphicsElementSVGAttributes
        interface ShapeElementSVGAttributes
        interface ConditionalProcessingSVGAttributes
        interface TransformableSVGAttributes
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``marker-end``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.markerEnd
            with inline set (value: string) = this.``marker-end`` <- value
            and inline get(): string = this.``marker-end``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``marker-mid``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.markerMid
            with inline set (value: string) = this.``marker-mid`` <- value
            and inline get(): string = this.``marker-mid``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``marker-start``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.markerStart
            with inline set (value: string) = this.``marker-start`` <- value
            and inline get(): string = this.``marker-start``
        [<Erase>]
        member _.points
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type polyline() =
        interface RegularNode
        interface GraphicsElementSVGAttributes
        interface ShapeElementSVGAttributes
        interface ConditionalProcessingSVGAttributes
        interface TransformableSVGAttributes
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``marker-end``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.markerEnd
            with inline set (value: string) = this.``marker-end`` <- value
            and inline get(): string = this.``marker-end``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``marker-mid``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.markerMid
            with inline set (value: string) = this.``marker-mid`` <- value
            and inline get(): string = this.``marker-mid``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``marker-start``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.markerStart
            with inline set (value: string) = this.``marker-start`` <- value
            and inline get(): string = this.``marker-start``
        [<Erase>]
        member _.points
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type radialGradient() =
        interface RegularNode
        interface GradientElementSVGAttributes
        [<Erase>]
        member _.cx
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.cy
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.r
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.fx
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.fy
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type rect() =
        interface RegularNode
        interface GraphicsElementSVGAttributes
        interface ShapeElementSVGAttributes
        interface ConditionalProcessingSVGAttributes
        interface TransformableSVGAttributes
        [<Erase>]
        member _.x
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.y
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.width
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.height
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.rx
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.ry
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type set() =
        interface RegularNode
        interface AnimationTimingSVGAttributes

    [<Erase>]
    type stop() =
        interface VoidNode
        [<Erase>]
        member _.color
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``stop-color``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.stopColor
            with inline set (value: string) = this.``stop-color`` <- value
            and inline get(): string = this.``stop-color``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``stop-opacity``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.stopOpacity
            with inline set (value: string) = this.``stop-opacity`` <- value
            and inline get(): string = this.``stop-opacity``
        [<Erase>]
        member _.offset
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type svg() =
        interface RegularNode
        interface ContainerElementSVGAttributes
        interface NewViewportSVGAttributes
        interface ConditionalProcessingSVGAttributes
        interface FitToViewBoxSVGAttributes
        interface ZoomAndPanSVGAttributes
        interface PresentationSVGAttributes
        [<Erase>]
        member _.version
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.baseProfile
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.x
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.y
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.width
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.height
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.contentScriptType
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.contentStyleType
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.xmlns
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type switch() =
        interface RegularNode
        interface ContainerElementSVGAttributes
        interface ConditionalProcessingSVGAttributes
        interface TransformableSVGAttributes
        [<Erase>]
        member _.display
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.visibility
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type symbol() =
        interface RegularNode
        interface ContainerElementSVGAttributes
        interface NewViewportSVGAttributes
        interface FitToViewBoxSVGAttributes
        [<Erase>]
        member _.width
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.height
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.preserveAspectRatio
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.refX
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.refY
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.viewBox
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.x
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.y
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type text() =
        interface RegularNode
        interface TextContentElementSVGAttributes
        interface GraphicsElementSVGAttributes
        interface ConditionalProcessingSVGAttributes
        interface TransformableSVGAttributes
        [<Erase>]
        member _.x
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.y
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.dx
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.dy
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.rotate
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.textLength
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.lengthAdjust
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``writing-mode``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.writingMode
            with inline set (value: string) = this.``writing-mode`` <- value
            and inline get(): string = this.``writing-mode``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``text-rendering``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.textRendering
            with inline set (value: string) = this.``text-rendering`` <- value
            and inline get(): string = this.``text-rendering``

    [<Erase>]
    type textPath() =
        interface RegularNode
        interface TextContentElementSVGAttributes
        interface ConditionalProcessingSVGAttributes
        [<Erase>]
        member _.startOffset
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.method
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.spacing
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``alignment-baseline``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.alignmentBaseline
            with inline set (value: string) = this.``alignment-baseline`` <- value
            and inline get(): string = this.``alignment-baseline``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``baseline-shift``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.baselineShift
            with inline set (value: string) = this.``baseline-shift`` <- value
            and inline get(): string = this.``baseline-shift``
        [<Erase>]
        member _.display
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.visibility
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.href
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type tspan() =
        interface RegularNode
        interface TextContentElementSVGAttributes
        interface ConditionalProcessingSVGAttributes
        [<Erase>]
        member _.x
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.y
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.dx
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.dy
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.rotate
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.textLength
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.lengthAdjust
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``alignment-baseline``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.alignmentBaseline
            with inline set (value: string) = this.``alignment-baseline`` <- value
            and inline get(): string = this.``alignment-baseline``
        [<Erase>]
        [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
        member _.``baseline-shift``
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member this.baselineShift
            with inline set (value: string) = this.``baseline-shift`` <- value
            and inline get(): string = this.``baseline-shift``
        [<Erase>]
        member _.display
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.visibility
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type use'() =
        interface RegularNode
        interface ConditionalProcessingSVGAttributes
        interface GraphicsElementSVGAttributes
        interface PresentationSVGAttributes
        interface TransformableSVGAttributes
        [<Erase>]
        member _.x
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.y
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.width
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.height
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
        [<Erase>]
        member _.href
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined

    [<Erase>]
    type view() =
        interface RegularNode
        interface FitToViewBoxSVGAttributes
        interface ZoomAndPanSVGAttributes
        [<Erase>]
        member _.viewTarget
            with set (_: string) = ()
            and [<Erase>] get(): string = JS.undefined
