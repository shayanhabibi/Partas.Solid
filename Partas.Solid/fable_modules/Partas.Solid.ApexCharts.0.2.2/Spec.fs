namespace Partas.Solid.ApexCharts

open Fable.Core
open Partas.Solid.Experimental.U

[<Erase; AutoOpen>]
module Enums =
    [<StringEnum; RequireQualifiedAccess>]
    type ApexMarkerShape =
        | Circle
        | Square
        | Rect
        | Line
        | Cross
        | Plus
        | Star
        | Sparkle
        | Diamond
        | Triangle

    [<RequireQualifiedAccess; StringEnum>]
    type ApexLineCap =
        | Butt
        | Square
        | Round

    [<RequireQualifiedAccess; StringEnum>]
    type ApexStrokeCurve =
        | Smooth
        | Straight
        | Stepline
        | Linestep
        | MonotoneCubic

    [<RequireQualifiedAccess; StringEnum>]
    type ApexAreaFillTo =
        | Origin
        | End

    [<RequireQualifiedAccess; StringEnum>]
    type ApexBarRadiusApplication =
        | Around
        | End

    [<RequireQualifiedAccess; StringEnum>]
    type ApexBarRadiusStacked =
        | All
        | Last

    [<RequireQualifiedAccess; StringEnum>]
    type ApexLegendPosition =
        | Top
        | Right
        | Bottom
        | Left

    [<RequireQualifiedAccess; StringEnum>]
    type ApexHorizontalAlign =
        | Left
        | Center
        | Right

    [<RequireQualifiedAccess; StringEnum>]
    type ApexVerticalAlign =
        | Top
        | Middle
        | Bottom

    [<RequireQualifiedAccess; StringEnum>]
    type ApexTextAnchor =
        | Start
        | Middle
        | End

    [<RequireQualifiedAccess; StringEnum>]
    type ApexAxisType =
        | Category
        | Datetime
        | Numeric

    [<RequireQualifiedAccess; StringEnum>]
    type ApexFrontBackPosition =
        | Front
        | Back

    [<RequireQualifiedAccess; StringEnum>]
    type ApexThemeMode =
        | Light
        | Dark

    [<RequireQualifiedAccess; StringEnum>]
    type ApexChartType =
        | Line
        | Area
        | Bar
        | Pie
        | Donut
        | RadialBar
        | Scatter
        | Bubble
        | Heatmap
        | Candlestick
        | BoxPlot
        | Radar
        | PolarArea
        | RangeBar
        | RangeArea
        | Treemap

    [<RequireQualifiedAccess; StringEnum>]
    type ApexDataLabelFormat =
        | Scale
        | Truncate

    [<RequireQualifiedAccess; StringEnum(CaseRules.LowerAll)>]
    type ApexZoomType =
        | X
        | Y
        | XY

    [<RequireQualifiedAccess; StringEnum>]
    type ApexStackType =
        | Normal
        /// Same as ``100%``
        | [<CompiledName("100%")>] Full
        | ``100%``

    [<RequireQualifiedAccess; StringEnum>]
    type ApexAutoSelected =
        | Zoom
        | Selection
        | Pan

    [<RequireQualifiedAccess; StringEnum>]
    type ApexOrientation =
        | Horizontal
        | Vertical

    [<RequireQualifiedAccess>]
    [<StringEnum>]
    type ApexTickAmount = | DataPoints

    [<Erase>]
    module TitleSubtitle =
        type Align = ApexHorizontalAlign

    [<Erase>]
    module Chart =
        type Type = ApexChartType
        type StackType = ApexStackType

        [<Erase>]
        module Toolbar =
            type AutoSelected = ApexAutoSelected

        [<Erase>]
        module Zoom =
            type Type = ApexZoomType

    [<Erase>]
    module PlotOptions =
        [<Erase>]
        module DataLabels =
            type Orientation = ApexOrientation

    [<Erase>]
    module Axis =
        [<Erase>]
        module X =
            type TickAmount = ApexTickAmount
            type Type = ApexAxisType

            [<Erase>]
            module Labels =
                type Align = ApexHorizontalAlign

        [<Erase>]
        module Y =
            type Type = ApexAxisType

            [<Erase>]
            module Labels =
                type Align = ApexHorizontalAlign

    [<Erase>]
    module Theme =
        type Mode = ApexThemeMode

        [<Erase>]
        module Monochrome =
            type ShadeTo = ApexThemeMode

    [<Erase>]
    module Grid =
        type Position = ApexFrontBackPosition

    [<Erase>]
    module Data =
        [<Erase>]
        module Labels =
            type Format = ApexDataLabelFormat

            [<Erase>]
            module Text =
                type Anchor = ApexTextAnchor

    [<Erase>]
    module NoData =
        [<Erase>]
        module Align =
            type Horizontal = ApexHorizontalAlign
            type Vertical = ApexVerticalAlign

    [<Erase>]
    module Legend =
        type Position = ApexLegendPosition

        [<Erase>]
        module Align =
            type Horizontal = ApexHorizontalAlign

    [<Erase>]
    module Area =
        [<Erase>]
        module Fill =
            type To = ApexAreaFillTo

    [<Erase>]
    module RadialBar =
        type Hollow = ApexFrontBackPosition

    [<Erase>]
    module Bar =
        [<Erase>]
        module Radius =
            type Application = ApexBarRadiusApplication
            type Stacked = ApexBarRadiusStacked

    [<Erase>]
    module Marker =
        type Shape = ApexMarkerShape

    [<Erase>]
    module Stroke =
        type Cap = ApexLineCap
        type Curve = ApexStrokeCurve
