namespace rec Partas.Solid.ApexCharts

open Fable.Core
open Fable.Core.JsInterop
open System
open Partas.Solid.Experimental.U

module Locale =
    [<JS.Pojo>]
    type Toolbar
        (
            ?download: string,
            ?selection: string,
            ?selectionZoom: string,
            ?zoomIn: string,
            ?zoomOut: string,
            ?pan: string,
            ?reset: string,
            ?exportToSVG: string,
            ?exportToPNG: string,
            ?exportToCSV: string
        ) =
        member val download: string = nativeOnly with get, set
        member val selection: string = nativeOnly with get, set
        member val selectionZoom: string = nativeOnly with get, set
        member val zoomIn: string = nativeOnly with get, set
        member val zoomOut: string = nativeOnly with get, set
        member val pan: string = nativeOnly with get, set
        member val reset: string = nativeOnly with get, set
        member val exportToSVG: string = nativeOnly with get, set
        member val exportToPNG: string = nativeOnly with get, set
        member val exportToCSV: string = nativeOnly with get, set

    [<JS.Pojo>]
    type Options
        (
            ?months: array<string>,
            ?shortMonths: array<string>,
            ?days: array<string>,
            ?shortDays: array<string>,
            ?toolbar: Toolbar
        ) =
        member val months: array<string> = nativeOnly with get, set
        member val shortMonths: array<string> = nativeOnly with get, set
        member val days: array<string> = nativeOnly with get, set
        member val shortDays: array<string> = nativeOnly with get, set
        member val toolbar: Toolbar = nativeOnly with get, set

[<JS.Pojo>]
type Locale(?name: string, ?options: Locale.Options) =
    member val name: string = JS.undefined with get, set
    member val options: Locale.Options = JS.undefined with get, set

[<JS.Pojo>]
type Annotations
    (
        ?yaxis: array<Annotations.YAxis>,
        ?xaxis: array<Annotations.XAxis>,
        ?points: array<Annotations.Point>,
        ?texts: array<Annotations.Text>,
        ?images: array<Annotations.Image>
    ) =
    member val yaxis: array<Annotations.YAxis> = JS.undefined with get, set
    member val xaxis: array<Annotations.XAxis> = JS.undefined with get, set
    member val points: array<Annotations.Point> = JS.undefined with get, set
    member val texts: array<Annotations.Text> = JS.undefined with get, set
    member val images: array<Annotations.Image> = JS.undefined with get, set



module Annotations =
    [<JS.Pojo>]
    type Label
        (
            ?borderColor: string,
            ?borderWidth: float,
            ?borderRadius: float,
            ?text: string,
            ?textAnchor: string,
            ?offsetX: float,
            ?offsetY: float,
            ?style: Style,
            ?position: string,
            ?orientation: string,
            ?mouseEnter: Action,
            ?mouseLeave: Action,
            ?click: Action
        ) =
        member val borderColor: string = JS.undefined with get, set
        member val borderWidth: float = JS.undefined with get, set
        member val borderRadius: float = JS.undefined with get, set
        member val text: string = JS.undefined with get, set
        member val textAnchor: string = JS.undefined with get, set
        member val offsetX: float = JS.undefined with get, set
        member val offsetY: float = JS.undefined with get, set
        member val style: Style = JS.undefined with get, set
        member val position: string = JS.undefined with get, set
        member val orientation: string = JS.undefined with get, set
        member val mouseEnter: Action = JS.undefined with get, set
        member val mouseLeave: Action = JS.undefined with get, set
        member val click: Action = JS.undefined with get, set

    module Style =
        [<JS.Pojo>]
        type Padding(?left: float, ?right: float, ?top: float, ?bottom: float) =
            member val left: float = nativeOnly with get, set
            member val right: float = nativeOnly with get, set
            member val top: float = nativeOnly with get, set
            member val bottom: float = nativeOnly with get, set

    [<JS.Pojo>]
    type Style
        (
            ?background: string,
            ?color: string,
            ?fontFamily: string,
            ?fontWeight: U2<string, float>,
            ?fontSize: string,
            ?cssClass: string,
            ?padding: Style.Padding
        ) =
        member val background: string = JS.undefined with get, set
        member val color: string = JS.undefined with get, set
        member val fontFamily: string = JS.undefined with get, set
        member val fontWeight: U2<string, float> = JS.undefined with get, set
        member val fontSize: string = JS.undefined with get, set
        member val cssClass: string = JS.undefined with get, set
        member val padding: Style.Padding = JS.undefined with get, set

    [<JS.Pojo>]
    type XAxis
        (
            ?id: U2<float, string>,
            ?x: U2<float, string>,
            ?x2: U2<float, string>,
            ?strokeDashArray: float,
            ?fillColor: string,
            ?borderColor: string,
            ?borderWidth: float,
            ?opacity: float,
            ?offsetX: float,
            ?offsetY: float,
            ?label: Label
        ) =
        member val id: U2<float, string> = JS.undefined with get, set
        member val x: U2<float, string> = JS.undefined with get, set
        member val x2: U2<float, string> = JS.undefined with get, set
        member val strokeDashArray: float = JS.undefined with get, set
        member val fillColor: string = JS.undefined with get, set
        member val borderColor: string = JS.undefined with get, set
        member val borderWidth: float = JS.undefined with get, set
        member val opacity: float = JS.undefined with get, set
        member val offsetX: float = JS.undefined with get, set
        member val offsetY: float = JS.undefined with get, set
        member val label: Label = JS.undefined with get, set

    [<JS.Pojo>]
    type YAxis
        (
            ?id: U2<float, string>,
            ?y: U2<float, string>,
            ?y2: U2<float, string>,
            ?strokeDashArray: float,
            ?fillColor: string,
            ?borderColor: string,
            ?borderWidth: float,
            ?opacity: float,
            ?offsetX: float,
            ?offsetY: float,
            ?width: U2<float, string>,
            ?yAxisIndex: float,
            ?label: Label
        ) =
        member val id: U2<float, string> = JS.undefined with get, set
        member val y: U2<float, string> = JS.undefined with get, set
        member val y2: U2<float, string> = JS.undefined with get, set
        member val strokeDashArray: float = JS.undefined with get, set
        member val fillColor: string = JS.undefined with get, set
        member val borderColor: string = JS.undefined with get, set
        member val borderWidth: float = JS.undefined with get, set
        member val opacity: float = JS.undefined with get, set
        member val offsetX: float = JS.undefined with get, set
        member val offsetY: float = JS.undefined with get, set
        member val width: U2<float, string> = JS.undefined with get, set
        member val yAxisIndex: float = JS.undefined with get, set
        member val label: Label = JS.undefined with get, set

    [<JS.Pojo>]
    type Image(?path: string, ?x: float, ?y: float, ?width: float, ?height: float) =
        member val path: string = JS.undefined with get, set
        member val x: float = JS.undefined with get, set
        member val y: float = JS.undefined with get, set
        member val width: float = JS.undefined with get, set
        member val height: float = JS.undefined with get, set

    module Point =
        [<JS.Pojo>]
        type Marker
            (
                ?size: float,
                ?fillColor: string,
                ?strokeColor: string,
                ?strokeWidth: float,
                ?shape: string,
                ?offsetX: float,
                ?offsetY: float,
                ?cssClass: string
            ) =
            member val size: float = nativeOnly with get, set
            member val fillColor: string = nativeOnly with get, set
            member val strokeColor: string = nativeOnly with get, set
            member val strokeWidth: float = nativeOnly with get, set
            member val shape: string = nativeOnly with get, set
            member val offsetX: float = nativeOnly with get, set
            member val offsetY: float = nativeOnly with get, set
            member val cssClass: string = nativeOnly with get, set

        type Image = Annotations.Image

    [<JS.Pojo>]
    type Point
        (
            ?id: U2<float, string>,
            ?x: U2<float, string>,
            ?y: float,
            ?yAxisIndex: float,
            ?seriesIndex: float,
            ?mouseEnter: Action,
            ?mouseLeave: Action,
            ?click: Action,
            ?marker: Point.Marker,
            ?label: Label,
            ?image: Point.Image
        ) =
        member val id: U2<float, string> = JS.undefined with get, set
        member val x: U2<float, string> = JS.undefined with get, set
        member val y: float = JS.undefined with get, set
        member val yAxisIndex: float = JS.undefined with get, set
        member val seriesIndex: float = JS.undefined with get, set
        member val mouseEnter: Action = JS.undefined with get, set
        member val mouseLeave: Action = JS.undefined with get, set
        member val click: Action = JS.undefined with get, set
        member val marker: Point.Marker = JS.undefined with get, set
        member val label: Label = JS.undefined with get, set
        member val image: Point.Image = JS.undefined with get, set

    [<JS.Pojo>]
    type Text
        (
            ?x: float,
            ?y: float,
            ?text: string,
            ?textAnchor: string,
            ?foreColor: string,
            ?fontSize: U2<string, float>,
            ?fontFamily: string,
            ?fontWeight: U2<string, float>,
            ?backgroundColor: string,
            ?borderColor: string,
            ?borderRadius: float,
            ?borderWidth: float,
            ?paddingLeft: float,
            ?paddingRight: float,
            ?paddingTop: float,
            ?paddingBottom: float
        ) =
        member val x: float = JS.undefined with get, set
        member val y: float = JS.undefined with get, set
        member val text: string = JS.undefined with get, set
        member val textAnchor: string = JS.undefined with get, set
        member val foreColor: string = JS.undefined with get, set
        member val fontSize: U2<string, float> = JS.undefined with get, set
        member val fontFamily: string = JS.undefined with get, set
        member val fontWeight: U2<string, float> = JS.undefined with get, set
        member val backgroundColor: string = JS.undefined with get, set
        member val borderColor: string = JS.undefined with get, set
        member val borderRadius: float = JS.undefined with get, set
        member val borderWidth: float = JS.undefined with get, set
        member val paddingLeft: float = JS.undefined with get, set
        member val paddingRight: float = JS.undefined with get, set
        member val paddingTop: float = JS.undefined with get, set
        member val paddingBottom: float = JS.undefined with get, set

module Chart =
    module Animations =
        [<JS.Pojo>]
        type Dynamic(?enabled: bool, ?speed: float) =
            member val enabled: bool = nativeOnly with get, set
            member val speed: float = nativeOnly with get, set

        [<JS.Pojo>]

        type Gradually(?enabled: bool, ?delay: float) =
            member val enabled: bool = nativeOnly with get, set
            member val delay: float = nativeOnly with get, set

    [<JS.Pojo>]
    type Animations
        (?enabled: bool, ?speed: float, ?animateGradually: Animations.Gradually, ?dynamicAnimation: Animations.Dynamic)
        =
        member val enabled: bool = nativeOnly with get, set
        member val speed: float = nativeOnly with get, set
        member val animateGradually: Animations.Gradually = nativeOnly with get, set
        member val dynamicAnimation: Animations.Dynamic = nativeOnly with get, set

    [<JS.Pojo>]
    type Events
        (
            animationEnd: unit,
            beforeMount: unit,
            mounted: unit,
            updated: unit,
            mouseMove: unit,
            mouseLeave: unit,
            click: unit,
            xAxisLabelClick: unit,
            legendClick: unit,
            markerClick: unit,
            selection: unit,
            dataPointSelection: unit,
            dataPointMouseEnter: unit,
            dataPointMouseLeave: unit,
            beforeZoom: unit,
            beforeResetZoom: unit,
            zoomed: unit,
            scrolled: unit,
            brushScrolled: unit
        ) =
        member val animationEnd: unit = nativeOnly with get, set
        member val beforeMount: unit = nativeOnly with get, set
        member val mounted: unit = nativeOnly with get, set
        member val updated: unit = nativeOnly with get, set
        member val mouseMove: unit = nativeOnly with get, set
        member val mouseLeave: unit = nativeOnly with get, set
        member val click: unit = nativeOnly with get, set
        member val xAxisLabelClick: unit = nativeOnly with get, set
        member val legendClick: unit = nativeOnly with get, set
        member val markerClick: unit = nativeOnly with get, set
        member val selection: unit = nativeOnly with get, set
        member val dataPointSelection: unit = nativeOnly with get, set
        member val dataPointMouseEnter: unit = nativeOnly with get, set
        member val dataPointMouseLeave: unit = nativeOnly with get, set
        member val beforeZoom: unit = nativeOnly with get, set
        member val beforeResetZoom: unit = nativeOnly with get, set
        member val zoomed: unit = nativeOnly with get, set
        member val scrolled: unit = nativeOnly with get, set
        member val brushScrolled: unit = nativeOnly with get, set

    module Selection =
        [<JS.Pojo>]
        type Fill(?color: string, ?opacity: float) =
            member val color: string = nativeOnly with get, set
            member val opacity: float = nativeOnly with get, set

        [<JS.Pojo>]
        type Stroke(?width: float, ?color: string, ?opacity: float, ?dashArray: float) =

            member val width: float = nativeOnly with get, set
            member val color: string = nativeOnly with get, set
            member val opacity: float = nativeOnly with get, set
            member val dashArray: float = nativeOnly with get, set

        [<JS.Pojo>]
        type XAxis(?min: float, ?max: float) =

            member val min: float = nativeOnly with get, set
            member val max: float = nativeOnly with get, set

        [<JS.Pojo>]
        type YAxis(?min: float, ?max: float) =
            member val min: float = nativeOnly with get, set
            member val max: float = nativeOnly with get, set

    [<JS.Pojo>]
    type Selection
        (
            ?enabled: bool,
            ?``type``: string,
            ?fill: Selection.Fill,
            ?stroke: Selection.Stroke,
            ?xaxis: Selection.XAxis,
            ?yaxis: Selection.YAxis
        ) =

        member val enabled: bool = nativeOnly with get, set
        member val ``type``: string = nativeOnly with get, set
        member val fill: Selection.Fill = nativeOnly with get, set
        member val stroke: Selection.Stroke = nativeOnly with get, set
        member val xaxis: Selection.XAxis = nativeOnly with get, set
        member val yaxis: Selection.YAxis = nativeOnly with get, set


    module ZoomedArea =
        [<JS.Pojo>]
        type Fill(?color: string, ?opacity: float) =
            member val color: string = nativeOnly with get, set
            member val opacity: float = nativeOnly with get, set

        [<JS.Pojo>]
        type Stroke(?color: string, ?opacity: float, ?width: float) =
            member val color: string = nativeOnly with get, set
            member val opacity: float = nativeOnly with get, set
            member val width: float = nativeOnly with get, set

    [<JS.Pojo>]
    type ZoomedArea(?fill: ZoomedArea.Fill, ?stroke: ZoomedArea.Stroke) =
        member val fill = fill with get, set
        member val stroke = stroke with get, set

    [<JS.Pojo>]
    type Zoom
        (
            ?enabled: bool,
            ?``type``: Enums.Chart.Zoom.Type,
            ?autoScaleYaxis: bool,
            ?allowMouseWheelZoom: bool,
            ?zoomedArea: ZoomedArea
        ) =
        member val enabled: bool = nativeOnly with get, set
        member val ``type``: Enums.Chart.Zoom.Type = nativeOnly with get, set
        member val autoScaleYaxis: bool = nativeOnly with get, set
        member val allowMouseWheelZoom: bool = nativeOnly with get, set
        member val zoomedArea: ZoomedArea = nativeOnly with get, set

    [<JS.Pojo>]
    type Brush(?enabled: bool, ?autoScaleYaxis: bool, ?target: string, ?targets: array<string>) =
        member val enabled: bool = nativeOnly with get, set
        member val autoScaleYaxis: bool = nativeOnly with get, set
        member val target: string = nativeOnly with get, set
        member val targets: array<string> = nativeOnly with get, set

    [<JS.Pojo>]
    type Sparkline(?enabled: bool) =
        member val enabled: bool = nativeOnly with get, set

    module Toolbar =
        module Tools =
            [<JS.Pojo>]
            type CustomIcons
                (
                    click: Chart -> Options -> Browser.Types.Event -> unit,
                    ?icon: string,
                    ?title: string,
                    ?index: float,
                    ?``class``: string
                ) =

                member val click: obj = nativeOnly with get, set
                member val icon: string = nativeOnly with get, set
                member val title: string = nativeOnly with get, set
                member val index: float = nativeOnly with get, set
                member val ``class``: string = nativeOnly with get, set

        [<JS.Pojo>]
        type Tools
            (
                ?download: U2<bool, string>,
                ?selection: U2<bool, string>,
                ?zoom: U2<bool, string>,
                ?zoomin: U2<bool, string>,
                ?zoomout: U2<bool, string>,
                ?pan: U2<bool, string>,
                ?reset: U2<bool, string>,
                ?customIcons: array<Tools.CustomIcons>
            ) =
            member val download: U2<bool, string> = nativeOnly with get, set
            member val selection: U2<bool, string> = nativeOnly with get, set
            member val zoom: U2<bool, string> = nativeOnly with get, set
            member val zoomin: U2<bool, string> = nativeOnly with get, set
            member val zoomout: U2<bool, string> = nativeOnly with get, set
            member val pan: U2<bool, string> = nativeOnly with get, set
            member val reset: U2<bool, string> = nativeOnly with get, set
            member val customIcons: array<Tools.CustomIcons> = nativeOnly with get, set

        module Export =
            [<JS.Pojo>]
            type Csv
                (
                    categoryFormatter: obj,
                    valueFormatter: obj,
                    ?filename: string,
                    ?columnDelimiter: string,
                    ?headerCategory: string,
                    ?headerValue: string
                ) =
                member val categoryFormatter: obj = nativeOnly with get, set
                member val valueFormatter: obj = nativeOnly with get, set
                member val filename: string = nativeOnly with get, set
                member val columnDelimiter: string = nativeOnly with get, set
                member val headerCategory: string = nativeOnly with get, set
                member val headerValue: string = nativeOnly with get, set

            [<JS.Pojo>]
            type Svg(?filename: string) =

                member val filename: string = nativeOnly with get, set

            [<JS.Pojo>]
            type Png(?filename: string) =

                member val filename: string = nativeOnly with get, set

        [<JS.Pojo>]
        type Export(?csv: Export.Csv, ?svg: Export.Svg, ?png: Export.Png, ?width: float, ?scale: float) =

            member val csv: Export.Csv = nativeOnly with get, set
            member val svg: Export.Svg = nativeOnly with get, set
            member val png: Export.Png = nativeOnly with get, set
            member val width: float = nativeOnly with get, set
            member val scale: float = nativeOnly with get, set

    [<JS.Pojo>]
    type Toolbar
        (
            ?show: bool,
            ?offsetX: float,
            ?offsetY: float,
            ?tools: Toolbar.Tools,
            ?export: Toolbar.Export,
            ?autoSelected: Enums.Chart.Toolbar.AutoSelected
        ) =
        member val show: bool = nativeOnly with get, set
        member val offsetX: float = nativeOnly with get, set
        member val offsetY: float = nativeOnly with get, set
        member val tools: Toolbar.Tools = nativeOnly with get, set
        member val export: Toolbar.Export = nativeOnly with get, set
        member val autoSelected: Enums.Chart.Toolbar.AutoSelected = nativeOnly with get, set

[<JS.Pojo>]
type Chart
    (
        ?width: U2<string, float>,
        ?height: U2<string, float>,
        ?``type``: Chart.Type,
        ?foreColor: string,
        ?fontFamily: string,
        ?background: string,
        ?offsetX: float,
        ?offsetY: float,
        ?dropShadow: obj,
        ?events: Chart.Events,
        ?brush: Chart.Brush,
        ?id: string,
        ?group: string,
        ?locales: array<Locale>,
        ?defaultLocale: string,
        ?parentHeightOffset: float,
        ?redrawOnParentResize: bool,
        ?redrawOnWindowResize: U2<bool, Action>,
        ?sparkline: Chart.Sparkline,
        ?stacked: bool,
        ?stackType: Enums.Chart.StackType,
        ?stackOnlyBar: bool,
        ?toolbar: Chart.Toolbar,
        ?zoom: Chart.Zoom,
        ?selection: Chart.Selection,
        ?animations: Chart.Animations
    ) =
    member val width: U2<string, float> = JS.undefined with get, set
    member val height: U2<string, float> = JS.undefined with get, set
    member val ``type``: Chart.Type = JS.undefined with get, set
    member val foreColor: string = JS.undefined with get, set
    member val fontFamily: string = JS.undefined with get, set
    member val background: string = JS.undefined with get, set
    member val offsetX: float = JS.undefined with get, set
    member val offsetY: float = JS.undefined with get, set
    member val dropShadow: obj = JS.undefined with get, set
    member val events: Chart.Events = JS.undefined with get, set
    member val brush: Chart.Brush = JS.undefined with get, set
    member val id: string = JS.undefined with get, set
    member val group: string = JS.undefined with get, set
    member val locales: array<Locale> = JS.undefined with get, set
    member val defaultLocale: string = JS.undefined with get, set
    member val parentHeightOffset: float = JS.undefined with get, set
    member val redrawOnParentResize: bool = JS.undefined with get, set
    member val redrawOnWindowResize: U2<bool, Action> = JS.undefined with get, set
    member val sparkline: Chart.Sparkline = JS.undefined with get, set
    member val stacked: bool = JS.undefined with get, set
    member val stackType: Enums.Chart.StackType = JS.undefined with get, set
    member val stackOnlyBar: bool = JS.undefined with get, set
    member val toolbar: Chart.Toolbar = JS.undefined with get, set
    member val zoom: Chart.Zoom = JS.undefined with get, set
    member val selection: Chart.Selection = JS.undefined with get, set
    member val animations: Chart.Animations = JS.undefined with get, set


[<JS.Pojo>]
type DropShadow(?enabled: bool, ?top: float, ?left: float, ?blur: float, ?opacity: float, ?color: string) =
    member val enabled: bool = JS.undefined with get, set
    member val top: float = JS.undefined with get, set
    member val left: float = JS.undefined with get, set
    member val blur: float = JS.undefined with get, set
    member val opacity: float = JS.undefined with get, set
    member val color: string = JS.undefined with get, set

module DataLabels =
    [<JS.Pojo>]
    type Style(?fontSize: string, ?fontFamily: string, ?fontWeight: U2<string, float>, ?colors: array<obj>) =
        member val fontSize: string = nativeOnly with get, set
        member val fontFamily: string = nativeOnly with get, set
        member val fontWeight: U2<string, float> = nativeOnly with get, set
        member val colors: array<obj> = nativeOnly with get, set

    [<JS.Pojo>]
    type Background
        (
            ?enabled: bool,
            ?foreColor: string,
            ?borderRadius: float,
            ?padding: float,
            ?opacity: float,
            ?borderWidth: float,
            ?borderColor: string,
            ?dropShadow: DropShadow
        ) =
        member val enabled: bool = nativeOnly with get, set
        member val foreColor: string = nativeOnly with get, set
        member val borderRadius: float = nativeOnly with get, set
        member val padding: float = nativeOnly with get, set
        member val opacity: float = nativeOnly with get, set
        member val borderWidth: float = nativeOnly with get, set
        member val borderColor: string = nativeOnly with get, set
        member val dropShadow: DropShadow = nativeOnly with get, set

[<JS.Pojo>]
type DataLabels
    (
        ?enabled: bool,
        ?enabledOnSeries: array<float>,
        ?textAnchor: Data.Labels.Text.Anchor,
        ?distributed: bool,
        ?offsetX: float,
        ?offsetY: float,
        ?style: DataLabels.Style,
        ?background: DataLabels.Background,
        ?dropShadow: DropShadow,
        ?formatter: U3<string, float, array<float>> * obj option -> U3<string, float, array<U2<string, string>>>
    ) =
    member val enabled: bool = JS.undefined with get, set
    member val enabledOnSeries: array<float> = JS.undefined with get, set
    member val textAnchor: Data.Labels.Text.Anchor = JS.undefined with get, set
    member val distributed: bool = JS.undefined with get, set
    member val offsetX: float = JS.undefined with get, set
    member val offsetY: float = JS.undefined with get, set
    member val style: DataLabels.Style = JS.undefined with get, set
    member val background: DataLabels.Background = JS.undefined with get, set
    member val dropShadow: DropShadow = JS.undefined with get, set

    member val formatter: U3<string, float, array<float>> * obj option -> U3<string, float, array<U2<string, string>>> =
        JS.undefined

[<JS.Pojo>]
type ColorStop(?offset: float, ?color: string, ?opacity: float) =
    member val offset: float = JS.undefined with get, set
    member val color: string = JS.undefined with get, set
    member val opacity: float = JS.undefined with get, set

module Fill =
    [<JS.Pojo>]
    type Gradient
        (
            ?shade: string,
            ?``type``: string,
            ?shadeIntensity: float,
            ?gradientToColors: array<string>,
            ?inverseColors: bool,
            ?opacityFrom: U2<float, array<float>>,
            ?opacityTo: U2<float, array<float>>,
            ?stops: array<float>,
            ?colorStops: U2<array<array<ColorStop>>, array<ColorStop>>
        ) =
        member val shade: string = nativeOnly with get, set
        member val ``type``: string = nativeOnly with get, set
        member val shadeIntensity: float = nativeOnly with get, set
        member val gradientToColors: array<string> = nativeOnly with get, set
        member val inverseColors: bool = nativeOnly with get, set
        member val opacityFrom: U2<float, array<float>> = nativeOnly with get, set
        member val opacityTo: U2<float, array<float>> = nativeOnly with get, set
        member val stops: array<float> = nativeOnly with get, set
        member val colorStops: U2<array<array<ColorStop>>, array<ColorStop>> = nativeOnly with get, set

    [<JS.Pojo>]
    type Image(?src: U2<string, array<string>>, ?width: float, ?height: float) =
        member val src: U2<string, array<string>> = nativeOnly with get, set
        member val width: float = nativeOnly with get, set
        member val height: float = nativeOnly with get, set

    [<JS.Pojo>]
    type Pattern(?style: U2<string, array<string>>, ?width: float, ?height: float, ?strokeWidth: float) =
        member val style: U2<string, array<string>> = nativeOnly with get, set
        member val width: float = nativeOnly with get, set
        member val height: float = nativeOnly with get, set
        member val strokeWidth: float = nativeOnly with get, set

[<JS.Pojo>]
type Fill
    (
        ?colors: array<obj>,
        ?opacity: U2<float, array<float>>,
        ?``type``: U2<string, array<string>>,
        ?gradient: Fill.Gradient,
        ?image: Fill.Image,
        ?pattern: Fill.Pattern
    ) =
    member val colors: array<obj> = JS.undefined with get, set
    member val opacity: U2<float, array<float>> = JS.undefined with get, set
    member val ``type``: U2<string, array<string>> = JS.undefined with get, set
    member val gradient: Fill.Gradient = JS.undefined with get, set
    member val image: Fill.Image = JS.undefined with get, set
    member val pattern: Fill.Pattern = JS.undefined with get, set

[<JS.Pojo>]
type ForecastDataPoints(?count: float, ?fillOpacity: float, ?strokeWidth: float, ?dashArray: float) =
    member val count: float = JS.undefined with get, set
    member val fillOpacity: float = JS.undefined with get, set
    member val strokeWidth: float = JS.undefined with get, set
    member val dashArray: float = JS.undefined with get, set

module Legend =

    [<JS.Pojo>]
    type Labels(?colors: U2<string, array<string>>, ?useSeriesColors: bool) =
        member val colors: U2<string, array<string>> = nativeOnly with get, set
        member val useSeriesColors: bool = nativeOnly with get, set

    [<JS.Pojo>]
    type Markers
        (
            customHTML: obj,
            onClick: unit,
            ?size: float,
            ?strokeWidth: float,
            ?fillColors: array<string>,
            ?shape: ApexMarkerShape,
            ?offsetX: float,
            ?offsetY: float
        ) =
        member val customHTML: obj = nativeOnly with get, set
        member val onClick: unit = nativeOnly with get, set
        member val size: float = nativeOnly with get, set
        member val strokeWidth: float = nativeOnly with get, set
        member val fillColors: array<string> = nativeOnly with get, set
        member val shape: ApexMarkerShape = nativeOnly with get, set
        member val offsetX: float = nativeOnly with get, set
        member val offsetY: float = nativeOnly with get, set

    [<JS.Pojo>]
    type ItemMargin(?horizontal: float, ?vertical: float) =

        member val horizontal: float = nativeOnly with get, set
        member val vertical: float = nativeOnly with get, set

    [<JS.Pojo>]
    type OnItemClick(?toggleDataSeries: bool) =

        member val toggleDataSeries: bool = nativeOnly with get, set

    [<JS.Pojo>]
    type OnItemHover(?highlightDataSeries: bool) =

        member val highlightDataSeries: bool = nativeOnly with get, set

[<JS.Pojo>]
type Legend
    (
        ?show: bool,
        ?showForSingleSeries: bool,
        ?showForNullSeries: bool,
        ?showForZeroSeries: bool,
        ?floating: bool,
        ?inverseOrder: bool,
        ?position: Legend.Position,
        ?horizontalAlign: Legend.Align.Horizontal,
        ?fontSize: string,
        ?fontFamily: string,
        ?fontWeight: U2<string, float>,
        ?width: float,
        ?height: float,
        ?offsetX: float,
        ?offsetY: float,
        ?formatter: U2<string, string * obj> -> string,
        ?tooltipHoverFormatter: U2<string, string * obj> -> string,
        ?customLegendItems: array<string>,
        ?clusterGroupedSeries: bool,
        ?clusterGroupedSeriesOrientation: string,
        ?labels: Legend.Labels,
        ?markers: Legend.Markers,
        ?itemMargin: Legend.ItemMargin,
        ?onItemClick: Legend.OnItemClick,
        ?onItemHover: Legend.OnItemHover
    ) =
    member val show: bool = JS.undefined with get, set
    member val showForSingleSeries: bool = JS.undefined with get, set
    member val showForNullSeries: bool = JS.undefined with get, set
    member val showForZeroSeries: bool = JS.undefined with get, set
    member val floating: bool = JS.undefined with get, set
    member val inverseOrder: bool = JS.undefined with get, set
    member val position: Legend.Position = JS.undefined with get, set
    member val horizontalAlign: Legend.Align.Horizontal = JS.undefined with get, set
    member val fontSize: string = JS.undefined with get, set
    member val fontFamily: string = JS.undefined with get, set
    member val fontWeight: U2<string, float> = JS.undefined with get, set
    member val width: float = JS.undefined with get, set
    member val height: float = JS.undefined with get, set
    member val offsetX: float = JS.undefined with get, set
    member val offsetY: float = JS.undefined with get, set
    member val formatter: U2<string, string * obj> -> string = JS.undefined
    member val tooltipHoverFormatter: U2<string, string * obj> -> string = JS.undefined
    member val customLegendItems: array<string> = JS.undefined with get, set
    member val clusterGroupedSeries: bool = JS.undefined with get, set
    member val clusterGroupedSeriesOrientation: string = JS.undefined with get, set
    member val labels: Legend.Labels = JS.undefined with get, set
    member val markers: Legend.Markers = JS.undefined with get, set
    member val itemMargin: Legend.ItemMargin = JS.undefined with get, set
    member val onItemClick: Legend.OnItemClick = JS.undefined with get, set
    member val onItemHover: Legend.OnItemHover = JS.undefined with get, set

[<JS.Pojo>]
type DiscretePoint
    (
        ?seriesIndex: float,
        ?dataPointIndex: float,
        ?fillColor: string,
        ?strokeColor: string,
        ?size: float,
        ?shape: Marker.Shape
    ) =
    member val seriesIndex: float = JS.undefined with get, set
    member val dataPointIndex: float = JS.undefined with get, set
    member val fillColor: string = JS.undefined with get, set
    member val strokeColor: string = JS.undefined with get, set
    member val size: float = JS.undefined with get, set
    member val shape: Marker.Shape = JS.undefined with get, set

module Markers =
    [<JS.Pojo>]
    type Hover(?size: float, ?sizeOffset: float) =

        member val size: float = nativeOnly with get, set
        member val sizeOffset: float = nativeOnly with get, set

[<JS.Pojo>]
type Markers
    (
        ?size: U2<float, array<float>>,
        ?colors: U2<string, array<string>>,
        ?strokeColors: U2<string, array<string>>,
        ?strokeWidth: U2<float, array<float>>,
        ?strokeOpacity: U2<float, array<float>>,
        ?strokeDashArray: U2<float, array<float>>,
        ?fillOpacity: U2<float, array<float>>,
        ?discrete: array<DiscretePoint>,
        ?shape: ApexMarkerShape,
        ?offsetX: float,
        ?offsetY: float,
        ?showNullDataPoints: bool,
        ?onClick: obj -> unit,
        ?onDblClick: obj -> unit,
        ?hover: Markers.Hover
    ) =
    member val size: U2<float, array<float>> = JS.undefined with get, set
    member val colors: U2<string, array<string>> = JS.undefined with get, set
    member val strokeColors: U2<string, array<string>> = JS.undefined with get, set
    member val strokeWidth: U2<float, array<float>> = JS.undefined with get, set
    member val strokeOpacity: U2<float, array<float>> = JS.undefined with get, set
    member val strokeDashArray: U2<float, array<float>> = JS.undefined with get, set
    member val fillOpacity: U2<float, array<float>> = JS.undefined with get, set
    member val discrete: array<DiscretePoint> = JS.undefined with get, set
    member val shape: ApexMarkerShape = JS.undefined with get, set
    member val offsetX: float = JS.undefined with get, set
    member val offsetY: float = JS.undefined with get, set
    member val showNullDataPoints: bool = JS.undefined with get, set
    member val onClick: obj -> unit = JS.undefined
    member val onDblClick: obj -> unit = JS.undefined
    member val hover: Markers.Hover = JS.undefined with get, set

module Grid =
    [<JS.Pojo>]
    type Lines(?show: bool, ?offsetX: float, ?offsetY: float) =

        member val show: bool = nativeOnly with get, set
        member val offsetX: float = nativeOnly with get, set
        member val offsetY: float = nativeOnly with get, set

    [<JS.Pojo>]
    type XAxis(?lines: Lines) =

        member val lines: Lines = nativeOnly with get, set

    [<JS.Pojo>]
    type YAxis(?lines: Lines) =

        member val lines: Lines = nativeOnly with get, set

    [<JS.Pojo>]
    type Row(?colors: array<string>, ?opacity: float) =

        member val colors: array<string> = nativeOnly with get, set
        member val opacity: float = nativeOnly with get, set

    [<JS.Pojo>]
    type Column(?colors: array<string>, ?opacity: float) =

        member val colors: array<string> = nativeOnly with get, set
        member val opacity: float = nativeOnly with get, set

    [<JS.Pojo>]
    type Padding(?top: float, ?right: float, ?bottom: float, ?left: float) =

        member val top: float = nativeOnly with get, set
        member val right: float = nativeOnly with get, set
        member val bottom: float = nativeOnly with get, set
        member val left: float = nativeOnly with get, set

[<JS.Pojo>]
type Grid
    (
        ?show: bool,
        ?borderColor: string,
        ?strokeDashArray: float,
        ?position: Grid.Position,
        ?xaxis: Grid.XAxis,
        ?yaxis: Grid.YAxis,
        ?row: Grid.Row,
        ?column: Grid.Column,
        ?padding: Grid.Padding
    ) =
    member val show: bool = JS.undefined with get, set
    member val borderColor: string = JS.undefined with get, set
    member val strokeDashArray: float = JS.undefined with get, set
    member val position: Grid.Position = JS.undefined with get, set
    member val xaxis: Grid.XAxis = JS.undefined with get, set
    member val yaxis: Grid.YAxis = JS.undefined with get, set
    member val row: Grid.Row = JS.undefined with get, set
    member val column: Grid.Column = JS.undefined with get, set
    member val padding: Grid.Padding = JS.undefined with get, set

module NoData =
    [<JS.Pojo>]
    type Style(?color: string, ?fontSize: string, ?fontFamily: string) =

        member val color: string = nativeOnly with get, set
        member val fontSize: string = nativeOnly with get, set
        member val fontFamily: string = nativeOnly with get, set

[<JS.Pojo>]
type NoData
    (
        ?text: string,
        ?align: NoData.Align.Horizontal,
        ?verticalAlign: NoData.Align.Vertical,
        ?offsetX: float,
        ?offsetY: float,
        ?style: NoData.Style
    ) =
    member val text: string = JS.undefined with get, set
    member val align: NoData.Align.Horizontal = JS.undefined with get, set
    member val verticalAlign: NoData.Align.Vertical = JS.undefined with get, set
    member val offsetX: float = JS.undefined with get, set
    member val offsetY: float = JS.undefined with get, set
    member val style: NoData.Style = JS.undefined with get, set

module PlotOptions =

    [<JS.Pojo>]
    type Line(?isSlopeChart: bool, ?colors: PlotOptions.Line.Colors) =

        member val isSlopeChart: bool = nativeOnly with get, set
        member val colors: PlotOptions.Line.Colors = nativeOnly with get, set

    [<JS.Pojo>]
    type Area(?fillTo: Area.Fill.To) =

        member val fillTo: Area.Fill.To = nativeOnly with get, set

    [<JS.Pojo>]
    type Bar
        (
            ?horizontal: bool,
            ?columnWidth: U2<string, float>,
            ?barHeight: U2<string, float>,
            ?distributed: bool,
            ?borderRadius: float,
            ?borderRadiusApplication: Bar.Radius.Application,
            ?borderRadiusWhenStacked: Bar.Radius.Stacked,
            ?hideZeroBarsWhenGrouped: bool,
            ?rangeBarOverlap: bool,
            ?rangeBarGroupRows: bool,
            ?isDumbbell: bool,
            ?dumbbellColors: array<array<string>>,
            ?isFunnel: bool,
            ?isFunnel3d: bool,
            ?colors: PlotOptions.Bar.Colors,
            ?dataLabels: PlotOptions.Bar.DataLabels
        ) =
        member val horizontal: bool = nativeOnly with get, set
        member val columnWidth: U2<string, float> = nativeOnly with get, set
        member val barHeight: U2<string, float> = nativeOnly with get, set
        member val distributed: bool = nativeOnly with get, set
        member val borderRadius: float = nativeOnly with get, set
        member val borderRadiusApplication: Bar.Radius.Application = nativeOnly with get, set
        member val borderRadiusWhenStacked: Bar.Radius.Stacked = nativeOnly with get, set
        member val hideZeroBarsWhenGrouped: bool = nativeOnly with get, set
        member val rangeBarOverlap: bool = nativeOnly with get, set
        member val rangeBarGroupRows: bool = nativeOnly with get, set
        member val isDumbbell: bool = nativeOnly with get, set
        member val dumbbellColors: array<array<string>> = nativeOnly with get, set
        member val isFunnel: bool = nativeOnly with get, set
        member val isFunnel3d: bool = nativeOnly with get, set
        member val colors: PlotOptions.Bar.Colors = nativeOnly with get, set
        member val dataLabels: PlotOptions.Bar.DataLabels = nativeOnly with get, set

    [<JS.Pojo>]
    type Bubble(?zScaling: bool, ?minBubbleRadius: float, ?maxBubbleRadius: float) =
        member val zScaling: bool = nativeOnly with get, set
        member val minBubbleRadius: float = nativeOnly with get, set
        member val maxBubbleRadius: float = nativeOnly with get, set

    [<JS.Pojo>]
    type CandleStick(?colors: PlotOptions.CandleStick.Colors, ?wick: PlotOptions.CandleStick.Wick) =

        member val colors: PlotOptions.CandleStick.Colors = nativeOnly with get, set
        member val wick: PlotOptions.CandleStick.Wick = nativeOnly with get, set

    [<JS.Pojo>]
    type BoxPlot(?colors: PlotOptions.BoxPlot.Colors) =

        member val colors: PlotOptions.BoxPlot.Colors = nativeOnly with get, set

    [<JS.Pojo>]
    type HeatMap
        (
            ?radius: float,
            ?enableShades: bool,
            ?shadeIntensity: float,
            ?reverseNegativeShade: bool,
            ?distributed: bool,
            ?useFillColorAsStroke: bool,
            ?colorScale: PlotOptions.HeatMap.ColorScale
        ) =

        member val radius: float = nativeOnly with get, set
        member val enableShades: bool = nativeOnly with get, set
        member val shadeIntensity: float = nativeOnly with get, set
        member val reverseNegativeShade: bool = nativeOnly with get, set
        member val distributed: bool = nativeOnly with get, set
        member val useFillColorAsStroke: bool = nativeOnly with get, set
        member val colorScale: PlotOptions.HeatMap.ColorScale = nativeOnly with get, set

    [<JS.Pojo>]
    type TreeMap
        (
            ?enableShades: bool,
            ?shadeIntensity: float,
            ?distributed: bool,
            ?reverseNegativeShade: bool,
            ?useFillColorAsStroke: bool,
            ?dataLabels: PlotOptions.TreeMap.DataLabels,
            ?borderRadius: float,
            ?colorScale: PlotOptions.TreeMap.ColorScale,
            ?seriesTitle: PlotOptions.TreeMap.SeriesTitle
        ) =

        member val enableShades: bool = nativeOnly with get, set
        member val shadeIntensity: float = nativeOnly with get, set
        member val distributed: bool = nativeOnly with get, set
        member val reverseNegativeShade: bool = nativeOnly with get, set
        member val useFillColorAsStroke: bool = nativeOnly with get, set
        member val dataLabels: PlotOptions.TreeMap.DataLabels = nativeOnly with get, set
        member val borderRadius: float = nativeOnly with get, set
        member val colorScale: PlotOptions.TreeMap.ColorScale = nativeOnly with get, set
        member val seriesTitle: PlotOptions.TreeMap.SeriesTitle = nativeOnly with get, set

    [<JS.Pojo>]
    type Pie
        (
            ?startAngle: float,
            ?endAngle: float,
            ?customScale: float,
            ?offsetX: float,
            ?offsetY: float,
            ?expandOnClick: bool,
            ?dataLabels: PlotOptions.Pie.DataLabels,
            ?donut: PlotOptions.Pie.Donut
        ) =

        member val startAngle: float = nativeOnly with get, set
        member val endAngle: float = nativeOnly with get, set
        member val customScale: float = nativeOnly with get, set
        member val offsetX: float = nativeOnly with get, set
        member val offsetY: float = nativeOnly with get, set
        member val expandOnClick: bool = nativeOnly with get, set
        member val dataLabels: PlotOptions.Pie.DataLabels = nativeOnly with get, set
        member val donut: PlotOptions.Pie.Donut = nativeOnly with get, set

    [<JS.Pojo>]
    type PolarArea(?rings: PlotOptions.PolarArea.Rings, ?spokes: PlotOptions.PolarArea.Spokes) =

        member val rings: PlotOptions.PolarArea.Rings = nativeOnly with get, set
        member val spokes: PlotOptions.PolarArea.Spokes = nativeOnly with get, set

    [<JS.Pojo>]
    type Radar(?size: float, ?offsetX: float, ?offsetY: float, ?polygons: PlotOptions.Radar.Polygons) =

        member val size: float = nativeOnly with get, set
        member val offsetX: float = nativeOnly with get, set
        member val offsetY: float = nativeOnly with get, set
        member val polygons: PlotOptions.Radar.Polygons = nativeOnly with get, set

    [<JS.Pojo>]
    type RadialBar
        (
            ?inverseOrder: bool,
            ?startAngle: float,
            ?endAngle: float,
            ?offsetX: float,
            ?offsetY: float,
            ?hollow: PlotOptions.RadialBar.Hollow,
            ?track: PlotOptions.RadialBar.Track,
            ?dataLabels: PlotOptions.RadialBar.DataLabels,
            ?barLabels: PlotOptions.RadialBar.BarLabels
        ) =

        member val inverseOrder: bool = nativeOnly with get, set
        member val startAngle: float = nativeOnly with get, set
        member val endAngle: float = nativeOnly with get, set
        member val offsetX: float = nativeOnly with get, set
        member val offsetY: float = nativeOnly with get, set
        member val hollow: PlotOptions.RadialBar.Hollow = nativeOnly with get, set
        member val track: PlotOptions.RadialBar.Track = nativeOnly with get, set
        member val dataLabels: PlotOptions.RadialBar.DataLabels = nativeOnly with get, set
        member val barLabels: PlotOptions.RadialBar.BarLabels = nativeOnly with get, set

    module Line =
        [<JS.Pojo>]
        type Colors(?threshold: float, ?colorAboveThreshold: string, ?colorBelowThreshold: string) =

            member val threshold: float = nativeOnly with get, set
            member val colorAboveThreshold: string = nativeOnly with get, set
            member val colorBelowThreshold: string = nativeOnly with get, set

    module Bar =

        [<JS.Pojo>]
        type Colors
            (
                ?ranges: array<PlotOptions.Bar.Colors.Ranges>,
                ?backgroundBarColors: array<string>,
                ?backgroundBarOpacity: float,
                ?backgroundBarRadius: float
            ) =

            member val ranges: array<PlotOptions.Bar.Colors.Ranges> = nativeOnly with get, set
            member val backgroundBarColors: array<string> = nativeOnly with get, set
            member val backgroundBarOpacity: float = nativeOnly with get, set
            member val backgroundBarRadius: float = nativeOnly with get, set

        [<JS.Pojo>]
        type DataLabels
            (
                ?maxItems: float,
                ?hideOverflowingLabels: bool,
                ?position: string,
                ?orientation: Enums.PlotOptions.DataLabels.Orientation,
                ?total: PlotOptions.Bar.DataLabels.Total
            ) =

            member val maxItems: float = nativeOnly with get, set
            member val hideOverflowingLabels: bool = nativeOnly with get, set
            member val position: string = nativeOnly with get, set
            member val orientation: Enums.PlotOptions.DataLabels.Orientation = nativeOnly with get, set
            member val total: PlotOptions.Bar.DataLabels.Total = nativeOnly with get, set

        module Colors =

            [<JS.Pojo>]
            type Ranges(?from: float, ?``to``: float, ?color: string) =
                member val from: float = nativeOnly with get, set
                member val ``to``: float = nativeOnly with get, set
                member val color: string = nativeOnly with get, set

        module DataLabels =



            [<JS.Pojo>]
            type Total
                (
                    formatter: string,
                    ?enabled: bool,
                    ?offsetX: float,
                    ?offsetY: float,
                    ?style: PlotOptions.Bar.DataLabels.Total.Style
                ) =

                member val formatter: string = nativeOnly with get, set
                member val enabled: bool = nativeOnly with get, set
                member val offsetX: float = nativeOnly with get, set
                member val offsetY: float = nativeOnly with get, set
                member val style: PlotOptions.Bar.DataLabels.Total.Style = nativeOnly with get, set

            module Total =

                [<JS.Pojo>]
                type Style(?color: string, ?fontSize: string, ?fontFamily: string, ?fontWeight: U2<float, string>) =

                    member val color: string = nativeOnly with get, set
                    member val fontSize: string = nativeOnly with get, set
                    member val fontFamily: string = nativeOnly with get, set
                    member val fontWeight: U2<float, string> = nativeOnly with get, set

    module CandleStick =

        [<JS.Pojo>]
        type Colors(?upward: U2<string, array<string>>, ?downward: U2<string, array<string>>) =
            member val upward: U2<string, array<string>> = nativeOnly with get, set
            member val downward: U2<string, array<string>> = nativeOnly with get, set

        [<JS.Pojo>]
        type Wick(?useFillColor: bool) =

            member val useFillColor: bool = nativeOnly with get, set

    module BoxPlot =

        [<JS.Pojo>]
        type Colors(?upper: U2<string, array<string>>, ?lower: U2<string, array<string>>) =

            member val upper: U2<string, array<string>> = nativeOnly with get, set
            member val lower: U2<string, array<string>> = nativeOnly with get, set

    module HeatMap =

        [<JS.Pojo>]

        type ColorScale(?ranges: array<PlotOptions.HeatMap.ColorScale.Ranges>, ?inverse: bool, ?min: float, ?max: float)
            =

            member val ranges: array<PlotOptions.HeatMap.ColorScale.Ranges> = nativeOnly with get, set
            member val inverse: bool = nativeOnly with get, set
            member val min: float = nativeOnly with get, set
            member val max: float = nativeOnly with get, set

        module ColorScale =

            [<JS.Pojo>]

            type Ranges(?from: float, ?``to``: float, ?color: string, ?foreColor: string, ?name: string) =

                member val from: float = nativeOnly with get, set
                member val ``to``: float = nativeOnly with get, set
                member val color: string = nativeOnly with get, set
                member val foreColor: string = nativeOnly with get, set
                member val name: string = nativeOnly with get, set

    module TreeMap =

        [<JS.Pojo>]

        type DataLabels(?format: Data.Labels.Format) =

            member val format: Data.Labels.Format = nativeOnly with get, set

        [<JS.Pojo>]

        type ColorScale(?inverse: bool, ?ranges: array<PlotOptions.TreeMap.ColorScale.Ranges>, ?min: float, ?max: float)
            =

            member val inverse: bool = nativeOnly with get, set
            member val ranges: array<PlotOptions.TreeMap.ColorScale.Ranges> = nativeOnly with get, set
            member val min: float = nativeOnly with get, set
            member val max: float = nativeOnly with get, set

        [<JS.Pojo>]

        type SeriesTitle
            (
                ?show: bool,
                ?offsetY: float,
                ?offsetX: float,
                ?borderColor: string,
                ?borderWidth: float,
                ?borderRadius: float,
                ?style: PlotOptions.TreeMap.SeriesTitle.Style
            ) =

            member val show: bool = nativeOnly with get, set
            member val offsetY: float = nativeOnly with get, set
            member val offsetX: float = nativeOnly with get, set
            member val borderColor: string = nativeOnly with get, set
            member val borderWidth: float = nativeOnly with get, set
            member val borderRadius: float = nativeOnly with get, set
            member val style: PlotOptions.TreeMap.SeriesTitle.Style = nativeOnly with get, set


        module ColorScale =

            [<JS.Pojo>]

            type Ranges(?from: float, ?``to``: float, ?color: string, ?foreColor: string, ?name: string) =

                member val from: float = nativeOnly with get, set
                member val ``to``: float = nativeOnly with get, set
                member val color: string = nativeOnly with get, set
                member val foreColor: string = nativeOnly with get, set
                member val name: string = nativeOnly with get, set

        module SeriesTitle =
            [<JS.Pojo>]
            type Style
                (
                    ?background: string,
                    ?color: string,
                    ?fontSize: string,
                    ?fontFamily: string,
                    ?fontWeight: U2<float, string>,
                    ?cssClass: string,
                    ?padding: PlotOptions.TreeMap.SeriesTitle.Style.Padding
                ) =
                member val background: string = nativeOnly with get, set
                member val color: string = nativeOnly with get, set
                member val fontSize: string = nativeOnly with get, set
                member val fontFamily: string = nativeOnly with get, set
                member val fontWeight: U2<float, string> = nativeOnly with get, set
                member val cssClass: string = nativeOnly with get, set
                member val padding: PlotOptions.TreeMap.SeriesTitle.Style.Padding = nativeOnly with get, set

            module Style =

                [<JS.Pojo>]
                type Padding(?left: float, ?right: float, ?top: float, ?bottom: float) =
                    member val left: float = nativeOnly with get, set
                    member val right: float = nativeOnly with get, set
                    member val top: float = nativeOnly with get, set
                    member val bottom: float = nativeOnly with get, set

    module Pie =

        [<JS.Pojo>]
        type DataLabels(?offset: float, ?minAngleToShowLabel: float) =
            member val offset: float = nativeOnly with get, set
            member val minAngleToShowLabel: float = nativeOnly with get, set

        [<JS.Pojo>]
        type Donut(?size: string, ?background: string, ?labels: PlotOptions.Pie.Donut.Labels) =
            member val size: string = nativeOnly with get, set
            member val background: string = nativeOnly with get, set
            member val labels: PlotOptions.Pie.Donut.Labels = nativeOnly with get, set

        module Donut =

            [<JS.Pojo>]
            type Labels
                (
                    ?show: bool,
                    ?name: PlotOptions.Pie.Donut.Labels.Name,
                    ?value: PlotOptions.Pie.Donut.Labels.Value,
                    ?total: PlotOptions.Pie.Donut.Labels.Total
                ) =
                member val show: bool = nativeOnly with get, set
                member val name: PlotOptions.Pie.Donut.Labels.Name = nativeOnly with get, set
                member val value: PlotOptions.Pie.Donut.Labels.Value = nativeOnly with get, set
                member val total: PlotOptions.Pie.Donut.Labels.Total = nativeOnly with get, set

            module Labels =
                [<JS.Pojo>]
                type Name
                    (
                        formatter: string,
                        ?show: bool,
                        ?fontSize: string,
                        ?fontFamily: string,
                        ?fontWeight: U2<string, float>,
                        ?color: string,
                        ?offsetY: float
                    ) =
                    member val formatter: string = nativeOnly with get, set
                    member val show: bool = nativeOnly with get, set
                    member val fontSize: string = nativeOnly with get, set
                    member val fontFamily: string = nativeOnly with get, set
                    member val fontWeight: U2<string, float> = nativeOnly with get, set
                    member val color: string = nativeOnly with get, set
                    member val offsetY: float = nativeOnly with get, set

                [<JS.Pojo>]
                type Value
                    (
                        formatter: string,
                        ?show: bool,
                        ?fontSize: string,
                        ?fontFamily: string,
                        ?fontWeight: U2<string, float>,
                        ?color: string,
                        ?offsetY: float
                    ) =
                    member val formatter: string = nativeOnly with get, set
                    member val show: bool = nativeOnly with get, set
                    member val fontSize: string = nativeOnly with get, set
                    member val fontFamily: string = nativeOnly with get, set
                    member val fontWeight: U2<string, float> = nativeOnly with get, set
                    member val color: string = nativeOnly with get, set
                    member val offsetY: float = nativeOnly with get, set

                [<JS.Pojo>]
                type Total
                    (
                        formatter: string,
                        ?show: bool,
                        ?showAlways: bool,
                        ?fontFamily: string,
                        ?fontWeight: U2<string, float>,
                        ?fontSize: string,
                        ?label: string,
                        ?color: string
                    ) =
                    member val formatter: string = nativeOnly with get, set
                    member val show: bool = nativeOnly with get, set
                    member val showAlways: bool = nativeOnly with get, set
                    member val fontFamily: string = nativeOnly with get, set
                    member val fontWeight: U2<string, float> = nativeOnly with get, set
                    member val fontSize: string = nativeOnly with get, set
                    member val label: string = nativeOnly with get, set
                    member val color: string = nativeOnly with get, set

    module PolarArea =
        [<JS.Pojo>]
        type Rings(?strokeWidth: float, ?strokeColor: string) =
            member val strokeWidth: float = nativeOnly with get, set
            member val strokeColor: string = nativeOnly with get, set

        [<JS.Pojo>]
        type Spokes(?strokeWidth: float, ?connectorColors: U2<string, array<string>>) =
            member val strokeWidth: float = nativeOnly with get, set
            member val connectorColors: U2<string, array<string>> = nativeOnly with get, set

    module Radar =
        [<JS.Pojo>]
        type Polygons
            (
                ?strokeColors: U2<string, array<string>>,
                ?strokeWidth: U2<string, array<string>>,
                ?connectorColors: U2<string, array<string>>,
                ?fill: PlotOptions.Radar.Polygons.Fill
            ) =
            member val strokeColors: U2<string, array<string>> = nativeOnly with get, set
            member val strokeWidth: U2<string, array<string>> = nativeOnly with get, set
            member val connectorColors: U2<string, array<string>> = nativeOnly with get, set
            member val fill: PlotOptions.Radar.Polygons.Fill = nativeOnly with get, set

        module Polygons =
            [<JS.Pojo>]
            type Fill(?colors: array<string>) =
                member val colors: array<string> = nativeOnly with get, set

    module RadialBar =
        [<JS.Pojo>]
        type Hollow
            (
                ?margin: float,
                ?size: string,
                ?background: string,
                ?image: string,
                ?imageWidth: float,
                ?imageHeight: float,
                ?imageOffsetX: float,
                ?imageOffsetY: float,
                ?imageClipped: bool,
                ?position: RadialBar.Hollow,
                ?dropShadow: DropShadow
            ) =
            member val margin: float = nativeOnly with get, set
            member val size: string = nativeOnly with get, set
            member val background: string = nativeOnly with get, set
            member val image: string = nativeOnly with get, set
            member val imageWidth: float = nativeOnly with get, set
            member val imageHeight: float = nativeOnly with get, set
            member val imageOffsetX: float = nativeOnly with get, set
            member val imageOffsetY: float = nativeOnly with get, set
            member val imageClipped: bool = nativeOnly with get, set
            member val position: RadialBar.Hollow = nativeOnly with get, set
            member val dropShadow: DropShadow = nativeOnly with get, set

        [<JS.Pojo>]
        type Track
            (
                ?show: bool,
                ?startAngle: float,
                ?endAngle: float,
                ?background: U2<string, array<string>>,
                ?strokeWidth: string,
                ?opacity: float,
                ?margin: float,
                ?dropShadow: DropShadow
            ) =
            member val show: bool = nativeOnly with get, set
            member val startAngle: float = nativeOnly with get, set
            member val endAngle: float = nativeOnly with get, set
            member val background: U2<string, array<string>> = nativeOnly with get, set
            member val strokeWidth: string = nativeOnly with get, set
            member val opacity: float = nativeOnly with get, set
            member val margin: float = nativeOnly with get, set
            member val dropShadow: DropShadow = nativeOnly with get, set

        [<JS.Pojo>]
        type DataLabels
            (
                ?show: bool,
                ?name: PlotOptions.RadialBar.DataLabels.Name,
                ?value: PlotOptions.RadialBar.DataLabels.Value,
                ?total: PlotOptions.RadialBar.DataLabels.Total
            ) =
            member val show: bool = nativeOnly with get, set
            member val name: PlotOptions.RadialBar.DataLabels.Name = nativeOnly with get, set
            member val value: PlotOptions.RadialBar.DataLabels.Value = nativeOnly with get, set
            member val total: PlotOptions.RadialBar.DataLabels.Total = nativeOnly with get, set

        [<JS.Pojo>]
        type BarLabels
            (
                ?enabled: bool,
                ?offsetX: float,
                ?offsetY: float,
                ?useSeriesColors: bool,
                ?fontFamily: string,
                ?fontWeight: U2<string, float>,
                ?fontSize: string,
                ?formatter: PlotOptions.RadialBar.BarLabels.formatter,
                ?onClick: PlotOptions.RadialBar.BarLabels.onClick
            ) =
            member val enabled: bool = nativeOnly with get, set
            member val offsetX: float = nativeOnly with get, set
            member val offsetY: float = nativeOnly with get, set
            member val useSeriesColors: bool = nativeOnly with get, set
            member val fontFamily: string = nativeOnly with get, set
            member val fontWeight: U2<string, float> = nativeOnly with get, set
            member val fontSize: string = nativeOnly with get, set
            member val formatter: PlotOptions.RadialBar.BarLabels.formatter = nativeOnly with get, set
            member val onClick: PlotOptions.RadialBar.BarLabels.onClick = nativeOnly with get, set


        module DataLabels =
            [<JS.Pojo>]
            type Name
                (
                    ?show: bool,
                    ?fontFamily: string,
                    ?fontWeight: U2<string, float>,
                    ?fontSize: string,
                    ?color: string,
                    ?offsetY: float
                ) =
                member val show: bool = nativeOnly with get, set
                member val fontFamily: string = nativeOnly with get, set
                member val fontWeight: U2<string, float> = nativeOnly with get, set
                member val fontSize: string = nativeOnly with get, set
                member val color: string = nativeOnly with get, set
                member val offsetY: float = nativeOnly with get, set

            [<JS.Pojo>]
            type Value
                (
                    formatter: string,
                    ?show: bool,
                    ?fontFamily: string,
                    ?fontSize: string,
                    ?fontWeight: U2<string, float>,
                    ?color: string,
                    ?offsetY: float
                ) =
                member val formatter: string = nativeOnly with get, set
                member val show: bool = nativeOnly with get, set
                member val fontFamily: string = nativeOnly with get, set
                member val fontSize: string = nativeOnly with get, set
                member val fontWeight: U2<string, float> = nativeOnly with get, set
                member val color: string = nativeOnly with get, set
                member val offsetY: float = nativeOnly with get, set

            [<JS.Pojo>]
            type Total
                (
                    formatter: string,
                    ?show: bool,
                    ?label: string,
                    ?color: string,
                    ?fontFamily: string,
                    ?fontWeight: U2<string, float>,
                    ?fontSize: string
                ) =
                member val formatter: string = nativeOnly with get, set
                member val show: bool = nativeOnly with get, set
                member val label: string = nativeOnly with get, set
                member val color: string = nativeOnly with get, set
                member val fontFamily: string = nativeOnly with get, set
                member val fontWeight: U2<string, float> = nativeOnly with get, set
                member val fontSize: string = nativeOnly with get, set

        module BarLabels =
            type formatter = delegate of barName: string * ?opts: obj -> string
            type onClick = delegate of barName: string * ?opts: obj -> unit

[<JS.Pojo>]
type PlotOptions
    (
        ?line: PlotOptions.Line,
        ?area: PlotOptions.Area,
        ?bar: PlotOptions.Bar,
        ?bubble: PlotOptions.Bubble,
        ?candlestick: PlotOptions.CandleStick,
        ?boxPlot: PlotOptions.BoxPlot,
        ?heatmap: PlotOptions.HeatMap,
        ?treemap: PlotOptions.TreeMap,
        ?pie: PlotOptions.Pie,
        ?polarArea: PlotOptions.PolarArea,
        ?radar: PlotOptions.Radar,
        ?radialBar: PlotOptions.RadialBar
    ) =
    member val line: PlotOptions.Line = JS.undefined with get, set
    member val area: PlotOptions.Area = JS.undefined with get, set
    member val bar: PlotOptions.Bar = JS.undefined with get, set
    member val bubble: PlotOptions.Bubble = JS.undefined with get, set
    member val candlestick: PlotOptions.CandleStick = JS.undefined with get, set
    member val boxPlot: PlotOptions.BoxPlot = JS.undefined with get, set
    member val heatmap: PlotOptions.HeatMap = JS.undefined with get, set
    member val treemap: PlotOptions.TreeMap = JS.undefined with get, set
    member val pie: PlotOptions.Pie = JS.undefined with get, set
    member val polarArea: PlotOptions.PolarArea = JS.undefined with get, set
    member val radar: PlotOptions.Radar = JS.undefined with get, set
    member val radialBar: PlotOptions.RadialBar = JS.undefined with get, set

[<JS.Pojo>]
type Responsive(?breakpoint: float, ?options: obj) =
    member val breakpoint: float = JS.undefined with get, set
    member val options: obj = JS.undefined with get, set

module AxisChartSeries =
    [<JS.Pojo>]
    type Data
        (
            x: obj,
            y: obj,
            ?fill: Fill,
            ?fillColor: string,
            ?strokeColor: string,
            ?meta: obj,
            ?goals: array<Goals>,
            ?barHeightOffset: float,
            ?columnWidthOffset: float
        ) =
        member val x: obj = nativeOnly with get, set
        member val y: obj = nativeOnly with get, set
        member val fill: Fill = nativeOnly with get, set
        member val fillColor: string = nativeOnly with get, set
        member val strokeColor: string = nativeOnly with get, set
        member val meta: obj = nativeOnly with get, set
        member val goals: array<Goals> = nativeOnly with get, set
        member val barHeightOffset: float = nativeOnly with get, set
        member val columnWidthOffset: float = nativeOnly with get, set

    [<JS.Pojo>]
    type Goals
        (
            value: float,
            ?name: string,
            ?strokeHeight: float,
            ?strokeWidth: float,
            ?strokeColor: string,
            ?strokeDashArray: float,
            ?strokeLineCap: Stroke.Cap
        ) =
        member val value: float = nativeOnly with get, set
        member val name: string = nativeOnly with get, set
        member val strokeHeight: float = nativeOnly with get, set
        member val strokeWidth: float = nativeOnly with get, set
        member val strokeColor: string = nativeOnly with get, set
        member val strokeDashArray: float = nativeOnly with get, set
        member val strokeLineCap: Stroke.Cap = nativeOnly with get, set

[<JS.Pojo>]
type AxisChartSeries
    (
        data:
            U9<
                array<float>,
                array<int>,
                array<AxisChartSeries.Data>,
                array<float * float>,
                array<int * int>,
                array<float * array<float>>,
                array<int * array<int>>,
                array<array<float>>,
                array<array<int>>
             >,
        ?name: string,
        ?``type``: string,
        ?color: string,
        ?group: string,
        ?hidden: bool,
        ?zIndex: float
    ) =
    member val data: U9<
                array<float>,
                array<int>,
                array<AxisChartSeries.Data>,
                array<float * float>,
                array<int * int>,
                array<float * array<float>>,
                array<int * array<int>>,
                array<array<float>>,
                array<array<int>>
             > = nativeOnly with get, set

    member val name: string = nativeOnly with get, set
    member val ``type``: string = nativeOnly with get, set
    member val color: string = nativeOnly with get, set
    member val group: string = nativeOnly with get, set
    member val hidden: bool = nativeOnly with get, set
    member val zIndex: float = nativeOnly with get, set

module States =
    [<JS.Pojo>]
    type Hover(?filter: Filter) =
        member val filter: Filter = nativeOnly with get, set

    [<JS.Pojo>]
    type Active(?allowMultipleDataPointsSelection: bool, ?filter: Filter) =
        member val allowMultipleDataPointsSelection: bool = nativeOnly with get, set
        member val filter: Filter = nativeOnly with get, set

    [<JS.Pojo>]
    type Filter(?``type``: string) =
        member val ``type``: string = nativeOnly with get, set

[<JS.Pojo>]
type States(?hover: States.Hover, ?active: States.Active) =
    member val hover: States.Hover = JS.undefined with get, set
    member val active: States.Active = JS.undefined with get, set

[<JS.Pojo>]
type Stroke
    (
        ?show: bool,
        ?curve: Stroke.Curve,
        ?lineCap: Stroke.Cap,
        ?colors: U2<array<obj>, array<string>>,
        ?width: U2<float, array<float>>,
        ?dashArray: U2<float, array<float>>,
        ?fill: Fill
    ) =
    member val show: bool = JS.undefined with get, set
    member val curve: Stroke.Curve = JS.undefined with get, set
    member val lineCap: Stroke.Cap = JS.undefined with get, set
    member val colors: U2<array<obj>, array<string>> = JS.undefined with get, set
    member val width: U2<float, array<float>> = JS.undefined with get, set
    member val dashArray: U2<float, array<float>> = JS.undefined with get, set
    member val fill: Fill = JS.undefined with get, set

module TitleSubtitle =
    [<JS.Pojo>]
    type Style(?fontSize: string, ?fontFamily: string, ?fontWeight: U2<string, float>, ?color: string) =
        member val fontSize: string = nativeOnly with get, set
        member val fontFamily: string = nativeOnly with get, set
        member val fontWeight: U2<string, float> = nativeOnly with get, set
        member val color: string = nativeOnly with get, set

[<JS.Pojo>]
type TitleSubtitle
    (
        ?text: string,
        ?align: Enums.TitleSubtitle.Align,
        ?margin: float,
        ?offsetX: float,
        ?offsetY: float,
        ?floating: bool,
        ?style: TitleSubtitle.Style
    ) =
    member val text: string = JS.undefined with get, set
    member val align: Enums.TitleSubtitle.Align = JS.undefined with get, set
    member val margin: float = JS.undefined with get, set
    member val offsetX: float = JS.undefined with get, set
    member val offsetY: float = JS.undefined with get, set
    member val floating: bool = JS.undefined with get, set
    member val style: TitleSubtitle.Style = JS.undefined with get, set

module Tooltip =
    [<JS.Pojo>]
    type Style(?fontSize: string, ?fontFamily: string) =
        member val fontSize: string = nativeOnly with get, set
        member val fontFamily: string = nativeOnly with get, set

    [<JS.Pojo>]
    type OnDatasetHover(?highlightDataSeries: bool) =
        member val highlightDataSeries: bool = nativeOnly with get, set

    [<JS.Pojo>]
    type X(formatter: string, ?show: bool, ?format: string) =
        member val formatter: string = nativeOnly with get, set
        member val show: bool = nativeOnly with get, set
        member val format: string = nativeOnly with get, set

    module Y =
        [<JS.Pojo>]
        type Title(formatter: string) =
            member val formatter: string = nativeOnly with get, set

    [<JS.Pojo>]
    type Y(?title: Y.Title, ?formatter: float * obj option -> string) =
        member val title: Y.Title = JS.undefined with get, set
        member val formatter: float * obj option -> string = JS.undefined

    [<JS.Pojo>]
    type Z(formatter: string, ?title: string) =
        member val formatter: string = nativeOnly with get, set
        member val title: string = nativeOnly with get, set

    [<JS.Pojo>]
    type Marker(?show: bool, ?fillColors: array<string>) =
        member val show: bool = nativeOnly with get, set
        member val fillColors: array<string> = nativeOnly with get, set

    [<JS.Pojo>]
    type Items(?display: string) =
        member val display: string = nativeOnly with get, set

    [<JS.Pojo>]
    type Fixed(?enabled: bool, ?position: string, ?offsetX: float, ?offsetY: float) =
        member val enabled: bool = nativeOnly with get, set
        member val position: string = nativeOnly with get, set
        member val offsetX: float = nativeOnly with get, set
        member val offsetY: float = nativeOnly with get, set

[<JS.Pojo>]
type Tooltip
    (
        ?enabled: bool,
        ?enabledOnSeries: array<float>,
        ?shared: bool,
        ?followCursor: bool,
        ?intersect: bool,
        ?inverseOrder: bool,
        ?custom: U2<(obj -> obj), array<(obj -> obj)>>,
        ?fillSeriesColor: bool,
        ?theme: string,
        ?cssClass: string,
        ?hideEmptySeries: bool,
        ?style: Tooltip.Style,
        ?onDatasetHover: Tooltip.OnDatasetHover,
        ?x: Tooltip.X,
        ?y: U2<Tooltip.Y, array<Tooltip.Y>>,
        ?z: Tooltip.Z,
        ?marker: Tooltip.Marker,
        ?items: Tooltip.Items,
        ?``fixed``: Tooltip.Fixed
    ) =
    member val enabled: bool = JS.undefined with get, set
    member val enabledOnSeries: array<float> = JS.undefined with get, set
    member val shared: bool = JS.undefined with get, set
    member val followCursor: bool = JS.undefined with get, set
    member val intersect: bool = JS.undefined with get, set
    member val inverseOrder: bool = JS.undefined with get, set
    member val custom: U2<(obj -> obj), array<(obj -> obj)>> = JS.undefined with get, set
    member val fillSeriesColor: bool = JS.undefined with get, set
    member val theme: string = JS.undefined with get, set
    member val cssClass: string = JS.undefined with get, set
    member val hideEmptySeries: bool = JS.undefined with get, set
    member val style: Tooltip.Style = JS.undefined with get, set
    member val onDatasetHover: Tooltip.OnDatasetHover = JS.undefined with get, set
    member val x: Tooltip.X = JS.undefined with get, set
    member val y: U2<Tooltip.Y, array<Tooltip.Y>> = JS.undefined with get, set
    member val z: Tooltip.Z = JS.undefined with get, set
    member val marker: Tooltip.Marker = JS.undefined with get, set
    member val items: Tooltip.Items = JS.undefined with get, set
    member val ``fixed``: Tooltip.Fixed = JS.undefined with get, set

module Theme =
    [<JS.Pojo>]
    type Monochrome(?enabled: bool, ?color: string, ?shadeTo: Enums.Theme.Monochrome.ShadeTo, ?shadeIntensity: float) =
        member val enabled: bool = nativeOnly with get, set
        member val color: string = nativeOnly with get, set
        member val shadeTo: Enums.Theme.Monochrome.ShadeTo = nativeOnly with get, set
        member val shadeIntensity: float = nativeOnly with get, set

[<JS.Pojo>]
type Theme(?mode: Theme.Mode, ?palette: string, ?monochrome: Theme.Monochrome) =
    member val mode: Theme.Mode = JS.undefined with get, set
    member val palette: string = JS.undefined with get, set
    member val monochrome: Theme.Monochrome = JS.undefined with get, set

module XAxis =
    [<JS.Pojo>]
    type Labels
        (
            formatter: U2<string, array<string>>,
            ?show: bool,
            ?rotate: float,
            ?rotateAlways: bool,
            ?hideOverlappingLabels: bool,
            ?showDuplicates: bool,
            ?trim: bool,
            ?minHeight: float,
            ?maxHeight: float,
            ?style: XAxis.Labels.Style,
            ?offsetX: float,
            ?offsetY: float,
            ?format: string,
            ?datetimeUTC: bool,
            ?datetimeFormatter: XAxis.Labels.DateTimeFormatter
        ) =
        member val formatter: U2<string, array<string>> = nativeOnly with get, set
        member val show: bool = nativeOnly with get, set
        member val rotate: float = nativeOnly with get, set
        member val rotateAlways: bool = nativeOnly with get, set
        member val hideOverlappingLabels: bool = nativeOnly with get, set
        member val showDuplicates: bool = nativeOnly with get, set
        member val trim: bool = nativeOnly with get, set
        member val minHeight: float = nativeOnly with get, set
        member val maxHeight: float = nativeOnly with get, set
        member val style: XAxis.Labels.Style = nativeOnly with get, set
        member val offsetX: float = nativeOnly with get, set
        member val offsetY: float = nativeOnly with get, set
        member val format: string = nativeOnly with get, set
        member val datetimeUTC: bool = nativeOnly with get, set
        member val datetimeFormatter: XAxis.Labels.DateTimeFormatter = nativeOnly with get, set

    [<JS.Pojo>]
    type Group(?groups: array<XAxis.Group.Groups>, ?style: XAxis.Group.Style) =
        member val groups: array<XAxis.Group.Groups> = nativeOnly with get, set
        member val style: XAxis.Group.Style = nativeOnly with get, set

    [<JS.Pojo>]
    type AxisBorder(?show: bool, ?color: string, ?offsetX: float, ?offsetY: float, ?strokeWidth: float) =
        member val show: bool = nativeOnly with get, set
        member val color: string = nativeOnly with get, set
        member val offsetX: float = nativeOnly with get, set
        member val offsetY: float = nativeOnly with get, set
        member val strokeWidth: float = nativeOnly with get, set

    [<JS.Pojo>]
    type AxisTicks(?show: bool, ?borderType: string, ?color: string, ?height: float, ?offsetX: float, ?offsetY: float) =
        member val show: bool = nativeOnly with get, set
        member val borderType: string = nativeOnly with get, set
        member val color: string = nativeOnly with get, set
        member val height: float = nativeOnly with get, set
        member val offsetX: float = nativeOnly with get, set
        member val offsetY: float = nativeOnly with get, set

    [<JS.Pojo>]
    type Title(?text: string, ?offsetX: float, ?offsetY: float, ?style: XAxis.Title.Style) =
        member val text: string = nativeOnly with get, set
        member val offsetX: float = nativeOnly with get, set
        member val offsetY: float = nativeOnly with get, set
        member val style: XAxis.Title.Style = nativeOnly with get, set

    [<JS.Pojo>]
    type Crosshairs
        (
            ?show: bool,
            ?width: U2<float, string>,
            ?position: string,
            ?opacity: float,
            ?stroke: XAxis.Crosshairs.Stroke,
            ?fill: XAxis.Crosshairs.Fill,
            ?dropShadow: DropShadow
        ) =
        member val show: bool = nativeOnly with get, set
        member val width: U2<float, string> = nativeOnly with get, set
        member val position: string = nativeOnly with get, set
        member val opacity: float = nativeOnly with get, set
        member val stroke: XAxis.Crosshairs.Stroke = nativeOnly with get, set
        member val fill: XAxis.Crosshairs.Fill = nativeOnly with get, set
        member val dropShadow: DropShadow = nativeOnly with get, set

    [<JS.Pojo>]
    type Tooltip(formatter: string, ?enabled: bool, ?offsetY: float, ?style: XAxis.Tooltip.Style) =
        member val formatter: string = nativeOnly with get, set
        member val enabled: bool = nativeOnly with get, set
        member val offsetY: float = nativeOnly with get, set
        member val style: XAxis.Tooltip.Style = nativeOnly with get, set

    module Labels =
        [<JS.Pojo>]
        type Style
            (
                ?colors: U2<string, array<string>>,
                ?fontSize: string,
                ?fontFamily: string,
                ?fontWeight: U2<string, float>,
                ?cssClass: string
            ) =
            member val colors: U2<string, array<string>> = nativeOnly with get, set
            member val fontSize: string = nativeOnly with get, set
            member val fontFamily: string = nativeOnly with get, set
            member val fontWeight: U2<string, float> = nativeOnly with get, set
            member val cssClass: string = nativeOnly with get, set

        [<JS.Pojo>]
        type DateTimeFormatter
            (?year: string, ?month: string, ?day: string, ?hour: string, ?minute: string, ?second: string) =
            member val year: string = nativeOnly with get, set
            member val month: string = nativeOnly with get, set
            member val day: string = nativeOnly with get, set
            member val hour: string = nativeOnly with get, set
            member val minute: string = nativeOnly with get, set
            member val second: string = nativeOnly with get, set

    module Group =
        [<JS.Pojo>]
        type Groups(title: string, cols: float) =
            member val title: string = nativeOnly with get, set
            member val cols: float = nativeOnly with get, set

        [<JS.Pojo>]
        type Style
            (
                ?colors: U2<string, array<string>>,
                ?fontSize: string,
                ?fontFamily: string,
                ?fontWeight: U2<string, float>,
                ?cssClass: string
            ) =
            member val colors: U2<string, array<string>> = nativeOnly with get, set
            member val fontSize: string = nativeOnly with get, set
            member val fontFamily: string = nativeOnly with get, set
            member val fontWeight: U2<string, float> = nativeOnly with get, set
            member val cssClass: string = nativeOnly with get, set

    module Title =
        [<JS.Pojo>]
        type Style
            (?color: string, ?fontFamily: string, ?fontWeight: U2<string, float>, ?fontSize: string, ?cssClass: string)
            =
            member val color: string = nativeOnly with get, set
            member val fontFamily: string = nativeOnly with get, set
            member val fontWeight: U2<string, float> = nativeOnly with get, set
            member val fontSize: string = nativeOnly with get, set
            member val cssClass: string = nativeOnly with get, set

    module Crosshairs =
        [<JS.Pojo>]
        type Stroke(?color: string, ?width: float, ?dashArray: float) =
            member val color: string = nativeOnly with get, set
            member val width: float = nativeOnly with get, set
            member val dashArray: float = nativeOnly with get, set

        [<JS.Pojo>]
        type Fill(?``type``: string, ?color: string, ?gradient: XAxis.Crosshairs.Fill.Gradient) =
            member val ``type``: string = nativeOnly with get, set
            member val color: string = nativeOnly with get, set
            member val gradient: XAxis.Crosshairs.Fill.Gradient = nativeOnly with get, set

        module Fill =
            [<JS.Pojo>]
            type Gradient
                (?colorFrom: string, ?colorTo: string, ?stops: array<float>, ?opacityFrom: float, ?opacityTo: float) =
                member val colorFrom: string = nativeOnly with get, set
                member val colorTo: string = nativeOnly with get, set
                member val stops: array<float> = nativeOnly with get, set
                member val opacityFrom: float = nativeOnly with get, set
                member val opacityTo: float = nativeOnly with get, set

    module Tooltip =
        [<JS.Pojo>]
        type Style(?fontSize: string, ?fontFamily: string) =
            member val fontSize: string = nativeOnly with get, set
            member val fontFamily: string = nativeOnly with get, set

[<JS.Pojo>]
type XAxis
    (
        ?``type``: Axis.X.Type,
        ?categories: obj,
        ?overwriteCategories: U2<array<float>, array<string>>,
        ?offsetX: float,
        ?offsetY: float,
        ?sorted: bool,
        ?labels: XAxis.Labels,
        ?group: XAxis.Group,
        ?axisBorder: XAxis.AxisBorder,
        ?axisTicks: XAxis.AxisTicks,
        ?tickPlacement: string,
        ?tickAmount: Enums.Axis.X.TickAmount,
        ?stepSize: float,
        ?min: float,
        ?max: float,
        ?range: float,
        ?floating: bool,
        ?decimalsInFloat: float,
        ?position: string,
        ?title: XAxis.Title,
        ?crosshairs: XAxis.Crosshairs,
        ?tooltip: XAxis.Tooltip
    ) =
    member val ``type``: Axis.X.Type = JS.undefined with get, set
    member val categories: obj = JS.undefined with get, set
    member val overwriteCategories: U2<array<float>, array<string>> = JS.undefined with get, set
    member val offsetX: float = JS.undefined with get, set
    member val offsetY: float = JS.undefined with get, set
    member val sorted: bool = JS.undefined with get, set
    member val labels: XAxis.Labels = JS.undefined with get, set
    member val group: XAxis.Group = JS.undefined with get, set
    member val axisBorder: XAxis.AxisBorder = JS.undefined with get, set
    member val axisTicks: XAxis.AxisTicks = JS.undefined with get, set
    member val tickPlacement: string = JS.undefined with get, set
    member val tickAmount: Enums.Axis.X.TickAmount = JS.undefined with get, set
    member val stepSize: float = JS.undefined with get, set
    member val min: float = JS.undefined with get, set
    member val max: float = JS.undefined with get, set
    member val range: float = JS.undefined with get, set
    member val floating: bool = JS.undefined with get, set
    member val decimalsInFloat: float = JS.undefined with get, set
    member val position: string = JS.undefined with get, set
    member val title: XAxis.Title = JS.undefined with get, set
    member val crosshairs: XAxis.Crosshairs = JS.undefined with get, set
    member val tooltip: XAxis.Tooltip = JS.undefined with get, set

module YAxis =
    [<JS.Pojo>]
    type Labels
        (
            formatter: U2<string, array<string>>,
            ?show: bool,
            ?showDuplicates: bool,
            ?minWidth: float,
            ?maxWidth: float,
            ?offsetX: float,
            ?offsetY: float,
            ?rotate: float,
            ?align: Axis.Y.Labels.Align,
            ?padding: float,
            ?style: YAxis.Labels.Style
        ) =
        member val formatter: U2<string, array<string>> = nativeOnly with get, set
        member val show: bool = nativeOnly with get, set
        member val showDuplicates: bool = nativeOnly with get, set
        member val minWidth: float = nativeOnly with get, set
        member val maxWidth: float = nativeOnly with get, set
        member val offsetX: float = nativeOnly with get, set
        member val offsetY: float = nativeOnly with get, set
        member val rotate: float = nativeOnly with get, set
        member val align: Axis.Y.Labels.Align = nativeOnly with get, set
        member val padding: float = nativeOnly with get, set
        member val style: YAxis.Labels.Style = nativeOnly with get, set

    [<JS.Pojo>]
    type AxisBorder(?show: bool, ?color: string, ?width: float, ?offsetX: float, ?offsetY: float) =
        member val show: bool = nativeOnly with get, set
        member val color: string = nativeOnly with get, set
        member val width: float = nativeOnly with get, set
        member val offsetX: float = nativeOnly with get, set
        member val offsetY: float = nativeOnly with get, set

    [<JS.Pojo>]
    type AxisTicks(?show: bool, ?color: string, ?width: float, ?offsetX: float, ?offsetY: float) =
        member val show: bool = nativeOnly with get, set
        member val color: string = nativeOnly with get, set
        member val width: float = nativeOnly with get, set
        member val offsetX: float = nativeOnly with get, set
        member val offsetY: float = nativeOnly with get, set

    [<JS.Pojo>]
    type Title(?text: string, ?rotate: float, ?offsetX: float, ?offsetY: float, ?style: YAxis.Title.Style) =
        member val text: string = nativeOnly with get, set
        member val rotate: float = nativeOnly with get, set
        member val offsetX: float = nativeOnly with get, set
        member val offsetY: float = nativeOnly with get, set
        member val style: YAxis.Title.Style = nativeOnly with get, set

    [<JS.Pojo>]
    type Crosshairs(?show: bool, ?position: string, ?stroke: YAxis.Crosshairs.Stroke) =
        member val show: bool = nativeOnly with get, set
        member val position: string = nativeOnly with get, set
        member val stroke: YAxis.Crosshairs.Stroke = nativeOnly with get, set

    [<JS.Pojo>]
    type Tooltip(?enabled: bool, ?offsetX: float) =
        member val enabled: bool = nativeOnly with get, set
        member val offsetX: float = nativeOnly with get, set

    module Labels =
        [<JS.Pojo>]
        type Style
            (
                ?colors: U2<string, array<string>>,
                ?fontSize: string,
                ?fontWeight: U2<string, float>,
                ?fontFamily: string,
                ?cssClass: string
            ) =
            member val colors: U2<string, array<string>> = nativeOnly with get, set
            member val fontSize: string = nativeOnly with get, set
            member val fontWeight: U2<string, float> = nativeOnly with get, set
            member val fontFamily: string = nativeOnly with get, set
            member val cssClass: string = nativeOnly with get, set

    module Title =
        [<JS.Pojo>]
        type Style
            (?color: string, ?fontSize: string, ?fontWeight: U2<string, float>, ?fontFamily: string, ?cssClass: string)
            =
            member val color: string = nativeOnly with get, set
            member val fontSize: string = nativeOnly with get, set
            member val fontWeight: U2<string, float> = nativeOnly with get, set
            member val fontFamily: string = nativeOnly with get, set
            member val cssClass: string = nativeOnly with get, set

    module Crosshairs =
        [<JS.Pojo>]
        type Stroke(?color: string, ?width: float, ?dashArray: float) =
            member val color: string = nativeOnly with get, set
            member val width: float = nativeOnly with get, set
            member val dashArray: float = nativeOnly with get, set

[<JS.Pojo>]
type YAxis
    (
        ?show: bool,
        ?showAlways: bool,
        ?showForNullSeries: bool,
        ?seriesName: U2<string, array<string>>,
        ?opposite: bool,
        ?reversed: bool,
        ?logarithmic: bool,
        ?logBase: float,
        ?tickAmount: float,
        ?stepSize: float,
        ?forceNiceScale: bool,
        ?min: U2<float, (float -> float)>,
        ?max: U2<float, (float -> float)>,
        ?floating: bool,
        ?decimalsInFloat: float,
        ?labels: YAxis.Labels,
        ?axisBorder: YAxis.AxisBorder,
        ?axisTicks: YAxis.AxisTicks,
        ?title: YAxis.Title,
        ?crosshairs: YAxis.Crosshairs,
        ?tooltip: YAxis.Tooltip
    ) =
    member val show: bool = JS.undefined with get, set
    member val showAlways: bool = JS.undefined with get, set
    member val showForNullSeries: bool = JS.undefined with get, set
    member val seriesName: U2<string, array<string>> = JS.undefined with get, set
    member val opposite: bool = JS.undefined with get, set
    member val reversed: bool = JS.undefined with get, set
    member val logarithmic: bool = JS.undefined with get, set
    member val logBase: float = JS.undefined with get, set
    member val tickAmount: float = JS.undefined with get, set
    member val stepSize: float = JS.undefined with get, set
    member val forceNiceScale: bool = JS.undefined with get, set
    member val min: U2<float, (float -> float)> = JS.undefined with get, set
    member val max: U2<float, (float -> float)> = JS.undefined with get, set
    member val floating: bool = JS.undefined with get, set
    member val decimalsInFloat: float = JS.undefined with get, set
    member val labels: YAxis.Labels = JS.undefined with get, set
    member val axisBorder: YAxis.AxisBorder = JS.undefined with get, set
    member val axisTicks: YAxis.AxisTicks = JS.undefined with get, set
    member val title: YAxis.Title = JS.undefined with get, set
    member val crosshairs: YAxis.Crosshairs = JS.undefined with get, set
    member val tooltip: YAxis.Tooltip = JS.undefined with get, set

[<JS.Pojo>]
type Options
    (
        ?annotations: Annotations,
        ?chart: Chart,
        ?colors: array<obj>,
        ?dataLabels: DataLabels,
        ?fill: Fill,
        ?forecastDataPoints: ForecastDataPoints,
        ?grid: Grid,
        ?labels: array<string>,
        ?legend: Legend,
        ?markers: Markers,
        ?noData: NoData,
        ?plotOptions: PlotOptions,
        ?responsive: array<Responsive>,
        ?series: U2<AxisChartSeries, float array>,
        ?states: States,
        ?stroke: Stroke,
        ?subtitle: TitleSubtitle,
        ?theme: Theme,
        ?title: TitleSubtitle,
        ?tooltip: Tooltip,
        ?xaxis: XAxis,
        ?yaxis: U2<YAxis, array<YAxis>>
    ) =
    member val annotations: Annotations = JS.undefined with get, set
    member val chart: Chart = JS.undefined with get, set
    member val colors: array<obj> = JS.undefined with get, set
    member val dataLabels: DataLabels = JS.undefined with get, set
    member val fill: Fill = JS.undefined with get, set
    member val forecastDataPoints: ForecastDataPoints = JS.undefined with get, set
    member val grid: Grid = JS.undefined with get, set
    member val labels: array<string> = JS.undefined with get, set
    member val legend: Legend = JS.undefined with get, set
    member val markers: Markers = JS.undefined with get, set
    member val noData: NoData = JS.undefined with get, set
    member val plotOptions: PlotOptions = JS.undefined with get, set
    member val responsive: array<Responsive> = JS.undefined with get, set
    member val series: U2<AxisChartSeries, float array> = JS.undefined with get, set
    member val states: States = JS.undefined with get, set
    member val stroke: Stroke = JS.undefined with get, set
    member val subtitle: TitleSubtitle = JS.undefined with get, set
    member val theme: Theme = JS.undefined with get, set
    member val title: TitleSubtitle = JS.undefined with get, set
    member val tooltip: Tooltip = JS.undefined with get, set
    member val xaxis: XAxis = JS.undefined with get, set
    member val yaxis: U2<YAxis, array<YAxis>> = JS.undefined with get, set

module Options =
    let inline init () = Options()

    let inline annotations (value: Annotations) (options: Options) : Options =
        options.annotations <- value
        options

    let inline chart (value: Chart) (options: Options) : Options =
        options.chart <- value
        options

    let inline colors (value: array<obj>) (options: Options) : Options =
        options.colors <- value
        options

    let inline dataLabels (value: DataLabels) (options: Options) : Options =
        options.dataLabels <- value
        options

    let inline fill (value: Fill) (options: Options) : Options =
        options.fill <- value
        options

    let inline forecastDataPoints (value: ForecastDataPoints) (options: Options) : Options =
        options.forecastDataPoints <- value
        options

    let inline grid (value: Grid) (options: Options) : Options =
        options.grid <- value
        options

    let inline labels (value: array<string>) (options: Options) : Options =
        options.labels <- value
        options

    let inline legend (value: Legend) (options: Options) : Options =
        options.legend <- value
        options

    let inline markers (value: Markers) (options: Options) : Options =
        options.markers <- value
        options

    let inline noData (value: NoData) (options: Options) : Options =
        options.noData <- value
        options

    let inline plotOptions (value: PlotOptions) (options: Options) : Options =
        options.plotOptions <- value
        options

    let inline responsive (value: array<Responsive>) (options: Options) : Options =
        options.responsive <- value
        options

    let inline series (value: U2<AxisChartSeries, float array>) (options: Options) : Options =
        options.series <- value
        options

    let inline states (value: States) (options: Options) : Options =
        options.states <- value
        options

    let inline stroke (value: Stroke) (options: Options) : Options =
        options.stroke <- value
        options

    let inline subtitle (value: TitleSubtitle) (options: Options) : Options =
        options.subtitle <- value
        options

    let inline theme (value: Theme) (options: Options) : Options =
        options.theme <- value
        options

    let inline title (value: TitleSubtitle) (options: Options) : Options =
        options.title <- value
        options

    let inline tooltip (value: Tooltip) (options: Options) : Options =
        options.tooltip <- value
        options

    let inline xaxis (value: XAxis) (options: Options) : Options =
        options.xaxis <- value
        options

    let inline yaxis (value: U2<YAxis, array<YAxis>>) (options: Options) : Options =
        options.yaxis <- value
        options

module DataUri =
    [<JS.Pojo>]
    type Options(?scale: float, ?width: float) =
        member val scale: float = nativeOnly with get, set
        member val width: float = nativeOnly with get, set

[<JS.Pojo>]
type DataUri(?imgURI: string, ?blob: obj) =
    member val imgURI: string = JS.undefined with get, set
    member val blob: obj = JS.undefined with get, set

[<JS.Pojo>]
type Exports
    (
        cleanup: string,
        svgUrl: string,
        dataURI: JS.Promise<DataUri>,
        exportToSVG: unit,
        exportToPng: unit,
        exportToCSV: unit,
        getSvgString: unit,
        triggerDownload: unit
    ) =
    member val cleanup: string = nativeOnly with get, set
    member val svgUrl: string = nativeOnly with get, set
    member val dataURI: JS.Promise<DataUri> = nativeOnly with get, set
    member val exportToSVG: unit = nativeOnly with get, set
    member val exportToPng: unit = nativeOnly with get, set
    member val exportToCSV: unit = nativeOnly with get, set
    member val getSvgString: unit = nativeOnly with get, set
    member val triggerDownload: unit = nativeOnly with get, set

    [<Import("ApexCharts", "apexcharts"); EmitConstructor>]
    static member ApexCharts(el: obj, options: obj) : ApexCharts = nativeOnly

[<Interface>]
type ApexCharts =
    abstract member render: unit -> JS.Promise<unit>

    abstract member updateOptions:
        options: obj * ?redrawPaths: bool * ?animate: bool * ?updateSyncedCharts: bool -> JS.Promise<unit>

    abstract member updateSeries: newSeries: U2<AxisChartSeries, float array> * ?animate: bool -> JS.Promise<unit>
    abstract member appendSeries: newSeries: U2<AxisChartSeries, float array> * ?animate: bool -> JS.Promise<unit>
    abstract member appendData: data: array<obj> * ?overwriteInitialSeries: bool -> unit
    abstract member toggleSeries: seriesName: string -> obj
    abstract member highlightSeries: seriesName: string -> obj
    abstract member showSeries: seriesName: string -> unit
    abstract member hideSeries: seriesName: string -> unit
    abstract member resetSeries: unit -> unit
    abstract member zoomX: min: float * max: float -> unit
    abstract member toggleDataPointSelection: seriesIndex: float * ?dataPointIndex: float -> obj
    abstract member destroy: unit -> unit
    abstract member setLocale: localeName: string -> unit
    abstract member paper: unit -> unit
    abstract member addXaxisAnnotation: options: obj * ?pushToMemory: bool * ?context: obj -> unit
    abstract member addYaxisAnnotation: options: obj * ?pushToMemory: bool * ?context: obj -> unit
    abstract member addPointAnnotation: options: obj * ?pushToMemory: bool * ?context: obj -> unit
    abstract member removeAnnotation: id: string * ?options: obj -> unit
    abstract member clearAnnotations: ?options: obj -> unit
    abstract member dataURI: ?options: DataUri.Options -> JS.Promise<DataUri>

    static member inline exec(chartID: string, fn: string, [<ParamArray>] args: array<obj>[]) : obj =
        emitJsExpr
            (chartID, fn, args)
            $$"""
import { ApexCharts } from "apexcharts";
ApexCharts.exec($0, $1, $2)"""

    static member inline getChartByID(chartID: string) : ApexCharts =
        emitJsExpr
            (chartID)
            $$"""
import { ApexCharts } from "apexcharts";
ApexCharts.getChartByID($0)"""

    static member inline initOnLoad() : unit =
        emitJsExpr
            ()
            $$"""
import { ApexCharts } from "apexcharts";
ApexCharts.initOnLoad()"""

    abstract member exports: Exports with get, set
